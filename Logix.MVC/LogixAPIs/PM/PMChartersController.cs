using DocumentFormat.OpenXml.Spreadsheet;
using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.DTOs.PM;
using Logix.Application.DTOs.PM.PmProjects;
using Logix.Application.DTOs.PM.PmProjectsStaff;
using Logix.Application.DTOs.TS;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Services;
using Logix.Application.Services.PM;
using Logix.Application.Wrapper;
using Logix.Infrastructure.Repositories;
using Logix.Infrastructure.Repositories.PM;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.PM.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Evaluation;
using Microsoft.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace Logix.MVC.LogixAPIs.PM
{
    // اعداد ميثاق المشاريع
    public class PMChartersController : BasePMApiController
    {
        private readonly IPMServiceManager pMServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IWFServiceManager wFServiceManager;
        private readonly IMainServiceManager mainServiceManager;
        private readonly ITsServiceManager tsServiceManager;
        private readonly IHrServiceManager hrServiceManager;


        public PMChartersController(IPMServiceManager pMServiceManager, 
            IPermissionHelper permission, 
            ICurrentData session, 
            ILocalizationService localization, 
            IWFServiceManager wFServiceManager, 
            IMainServiceManager mainServiceManager,
            ITsServiceManager tsServiceManager,
            IHrServiceManager hrServiceManager)
        {
            this.pMServiceManager = pMServiceManager;
            this.permission = permission;
            this.session = session;
            this.localization = localization;
            this.wFServiceManager = wFServiceManager;
            this.mainServiceManager = mainServiceManager;
            this.tsServiceManager = tsServiceManager;
            this.hrServiceManager = hrServiceManager;
        }

        #region Index Page


        [HttpPost("Search")]
        public async Task<IActionResult> Search(PMChartsFilterDto filter)
        {
            var chk = await permission.HasPermission(1727, PermissionType.Show);

            if (!chk)
                return Ok(await Result.AccessDenied("AccessDenied"));

            try
            {
                var BranchesList = session.Branches.Split(',');

                var items = await pMServiceManager.PMProjectsService.GetAll(e => e.IsDeleted == false
                    && e.FacilityId == session.FacilityId
                    && e.IsSubContract == false
                    && BranchesList.Contains(e.BranchId.ToString())
                    && (string.IsNullOrEmpty(filter.ProjectName) || ((e.Name != null && e.Name.Contains(filter.ProjectName)) || (e.Name2 != null && e.Name2.Contains(filter.ProjectName))))
                    && (string.IsNullOrEmpty(filter.ProjectCode) || ((e.Code == Convert.ToInt64(filter.ProjectCode)) || (e.No == filter.ProjectCode))));

                if (!items.Succeeded)
                    return Ok(await Result<object>.FailAsync(items.Status.message));
                if (!items.Data.Any())
                    return Ok(await Result<object>.SuccessAsync(new List<object>(), localization.GetResource1("NosearchResult")));

                var res = items.Data.AsQueryable();

                if (!string.IsNullOrEmpty(filter.From) && !string.IsNullOrEmpty(filter.To))
                {
                    var DateFrom = DateHelper.StringToDate(filter.From);
                    var DateTo = DateHelper.StringToDate(filter.To);
                    res = res.Where(x => DateHelper.StringToDate(x.DateG) >= DateFrom && DateHelper.StringToDate(x.DateG) <= DateTo);
                }

                if (!res.Any())
                    return Ok(await Result<object>.SuccessAsync(new List<object>(), localization.GetResource1("NosearchResult")));

                // Get related applications
                var applications = await wFServiceManager.WfApplicationService.GetAllVW(x => x.IsDeleted == false);

                // Join and select final result
                var result = res
                    .GroupJoin(applications.Data,
                        project => project.AppId,
                        app => app.Id,
                        (project, appGroup) => new { project, app = appGroup.FirstOrDefault() }) // Left Join equivalent
                    .Select(joined => new
                    {
                        joined.project.Id,
                        joined.project.Code,
                        joined.project.ProjectType,
                        joined.project.DateG,
                        joined.project.Name,
                        joined.project.ProjectEnd,
                        StepName = joined.app != null ? joined.app.StepName : "",
                        AppStatusName = joined.app != null ? joined.app.StatusName : "جديد",
                        AppStatusName2 = joined.app != null ? joined.app.StatusName2 : "New",
                    })
                    .ToList();

                return Ok(await Result<object>.SuccessAsync(result, ""));
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"======= Exp in {this.GetType()}: {ex.Message}"));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var chk = await permission.HasPermission(1727, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));


                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await pMServiceManager.PMProjectsService.Remove(id);

                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"======= Exp in {this.GetType()}: {ex.Message}"));
            }

        }

        #endregion

        #region Edit Page


        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long Id)
        {
            try
            {
                string? Message = String.Empty;
                var BranchesList = session.Branches.Split(',');

                var chk1 = await permission.HasPermission(1737, PermissionType.Show);
                var chk2 = await permission.HasPermission(1727 , PermissionType.Show);
                if (!chk1|| !chk1)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id <= 0)
                {
                    return Ok(await Result<object>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var item = await pMServiceManager.PMProjectsService.GetOneFromProjectsEditVw(x => x.IsDeleted == false && x.Id == Id);
                if (!item.Succeeded)
                    return Ok(await Result<object>.FailAsync(item.Status.message));

                if (item.Data == null)
                    return Ok(await Result<object>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                if (item.Data.StatusId == 2)
                    Message = "حالة المشروع مغلق ولهذا لاتستطيع رفع ميثاق للإعتماد";

                // توصيف نطاق عمل المشروع  +  بطاقة المشروع
                var GetProjects = await pMServiceManager.PMProjectsService.GetAll(e => e.IsDeleted == false && e.FacilityId == session.FacilityId && e.IsSubContract == false && BranchesList.Contains(e.BranchId.ToString()));
                var res = GetProjects.Data.AsQueryable();
                var applications = await wFServiceManager.WfApplicationService.GetAllVW(x => x.IsDeleted == false);

                var ProjectsData = res
                    .GroupJoin(applications.Data,
                        project => project.AppId,
                        app => app.Id,
                        (project, appGroup) => new { project, app = appGroup.FirstOrDefault() }) // Left Join equivalent
                    .Select(joined => new
                    {
                        joined.project.Id,
                        joined.project.Code,
                        joined.project.ProjectType,
                        joined.project.DateG,
                        joined.project.Name,
                        joined.project.ProjectEnd,
                        StepName = joined.app != null ? joined.app.StepName : "",
                        AppStatusName = joined.app != null ? joined.app.StatusName : "جديد",
                        AppStatusName2 = joined.app != null ? joined.app.StatusName2 : "New",
                    })
                    .ToList();


                // أهداف المشروع
                var GetObjective = await pMServiceManager.PmProjectsObjectiveService.GetAll(x => x.IsDeleted == false && x.ProjectId == Id);

                // ارتباط المشروع بالأهداف الاستراتيجية 
                var GetStrategicLink = await pMServiceManager.PmProjectsStrategicLinkService.GetAllVW(x => x.IsDeleted == false && x.ProjectId == Id);

                // ارتباط المشروع بمشاريع اخرى
                var GetInterconnections = await pMServiceManager.PmProjectsInterconnectionService.GetAllVW(x => x.IsDeleted == false && x.ProjectId == Id);


                // مؤشرات اداء المشروع
                var GetPerformanceIndicators = await pMServiceManager.PmProjectsPerformanceIndicatorService.GetAll(x => x.IsDeleted == false && x.ProjectId == Id);

                //  مخرجات المشروع
                var deliverables = await pMServiceManager.PmProjectsDeliverableService.GetAllVW(
                    d => d.IsDeleted == false && d.ProjectId == Id);

                if (!deliverables.Succeeded)
                    return Ok(await Result<object>.FailAsync(deliverables.Status.message));
                var deliverableIds = deliverables.Data.Select(d => d.Id).ToList();
                var transactionDetails = await pMServiceManager.PmDeliverableTransactionsDetailService.GetAll(
                    t => t.IsDeleted == false && t.DeliverableId.HasValue && deliverableIds.Contains(t.DeliverableId.Value)
                );

                var GetProjectsDeliverable = deliverables.Data
                    .GroupJoin(transactionDetails.Data,
                        deliverable => deliverable.Id,
                        transaction => transaction.DeliverableId,
                        (deliverable, transactions) => new
                        {
                            deliverable,
                            DeliveredQty = transactions.Any() ? transactions.Sum(t => t.Qty ?? 0) : 0 // Ensure DeliveredQty is 0 when there are no matching transactions
                        })
                    .Select(joined => new
                    {
                        joined.deliverable.Id,
                        joined.deliverable.ProjectId,
                        joined.deliverable.Name,
                        joined.deliverable.Description,
                        joined.deliverable.StatusId,
                        joined.deliverable.Note,
                        joined.DeliveredQty,
                        joined.deliverable.Qty,
                        joined.deliverable.Type,
                        joined.deliverable.ProjectCode,
                        joined.deliverable.CreatedOn,

                    }).ToList();

                // معايير قبول المخرجات
                var GetDeliverableAcceptCriterion = await pMServiceManager.PmProjectsDeliverableAcceptCriterionService.GetAll(x => x.IsDeleted == false && x.ProjectId == Id);
                // معالم / مراحل المشروع الرئيسية
                var GetPhaseOrMilestones = await tsServiceManager.TsTaskService.GetAllVW(x => x.Isdel == false && x.ProjectId == Id && x.TypeId == 2);

                // أصحاب المصلحة
                var stakeholders = await pMServiceManager.PMProjectsStokeholderService.GetAll(s => s.IsDeleted == false && s.ProjectId == Id);

                if (!stakeholders.Succeeded)
                    return Ok(await Result<object>.FailAsync(stakeholders.Status.message));

                var GetStokeholder = stakeholders.Data?
                     .Select(stakeholder => new
                     {
                         stakeholder.Id,
                         stakeholder.ProjectId,
                         stakeholder.Name,
                         stakeholder.Note,
                         stakeholder.JobName,
                         stakeholder.InternalOrExternal,
                         InternalOrExternalName = stakeholder.InternalOrExternal == 1 ? "داخلي" : "خارجي", // Conditional mapping

                     })
                     .ToList();


                //  فريق عمل المشروع
                var GetProjectStaff = await pMServiceManager.PMProjectsStaffService.GetAllVW(x => x.IsDeleted == false && x.ProjectId == Id && x.TypeId == 0);

                // مخاطر وعوائق المشروع  
                var GetProjectRisks = await pMServiceManager.PMProjectsRiskService.GetAllVW(x => x.IsDeleted == false && x.ProjectId == Id);

                //  افتراضات المشروع
                var GetAssumption = await pMServiceManager.PmProjectsAssumptionService.GetAll(x => x.IsDeleted == false && x.ProjectId == Id);
                //  الموارد المطلوبة للمشروع
                var GetResources = await pMServiceManager.PmProjectsResourceService.GetAllVW(x => x.IsDeleted == false && x.ProjectId == Id);


                // حوكمة المشروع
                var GetGovernances = await pMServiceManager.PmProjectsGovernanceService.GetAll(x => x.IsDeleted == false && x.ProjectId == Id);





                var response = new
                {
                    Data = item.Data,
                    Message = Message,
                    Projects = ProjectsData,
                    Objectives = GetObjective.Data,
                    StrategicLinks = GetStrategicLink.Data,
                    Interconnections = GetInterconnections.Data,
                    PhaseOrMilestones = GetPhaseOrMilestones.Data,
                    PerformanceIndicators = GetPerformanceIndicators.Data,
                    Deliverables = GetProjectsDeliverable,
                    DeliverableAcceptCriterion = GetDeliverableAcceptCriterion.Data,
                    Stakeholders = GetStokeholder,
                    Assumptions = GetAssumption.Data,
                    Governances = GetGovernances.Data,
                    Resources = GetResources.Data,
                    ProjectStaff = GetProjectStaff.Data,
                    ProjectRisks = GetProjectRisks.Data
                };

                return Ok(await Result<object>.SuccessAsync(response, "", 200));

            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"====== Exp in PMChartersController getById, MESSAGE: {ex.Message}"));
            }
        }



        #region تعديل  بطاقة المشروع

        [HttpPost("EditProjectInfo")]
        public async Task<IActionResult> EditProjectInfo(PMProjectChartEditDto obj)
        {

            var chk = await permission.HasPermission(1727, PermissionType.Edit);

            if (chk == false)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                obj.Dept ??= 0;
                obj.Duration ??= 0;
                obj.DurationType ??= 0;
                obj.EstimatedCost ??= 0;
                obj.AllowRate ??= 0;
                if (string.IsNullOrEmpty(obj.ProjectName))
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("ProjectName")));

                if (string.IsNullOrEmpty(obj.OwnerCode))
                    return Ok(await Result<object>.FailAsync("مالك المشروع"));

                if (string.IsNullOrEmpty(obj.StartDate))
                    return Ok(await Result<object>.FailAsync(" تاريخ البدء المخطط"));
                if (string.IsNullOrEmpty(obj.EndDate))
                    return Ok(await Result<object>.FailAsync(" تاريخ الإنتهاء المخطط"));
                if (obj.EstimatedCost <= 0)
                    return Ok(await Result<object>.FailAsync("القيمة التقديرية للمشروع "));

                if (obj.AllowRate <= 0)
                    return Ok(await Result<object>.FailAsync("نسبة السماح في الميزانية "));

                if (obj.ProjectId <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("Project")));

                if (obj.Duration <= 0 || obj.DurationType <= 0)
                    return Ok(await Result<object>.FailAsync("المدة المخططة لتنفيذ المشروع"));


                var edit = await pMServiceManager.PMProjectsService.EditProjectInfo(obj);
                return Ok(edit);
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"======= Exp in {this.GetType()}: {ex.Message}"));

            }

        }

        #endregion



        #region تعديل أهداف المشروع


        [HttpPost("AddObjective")]
        public async Task<IActionResult> AddObjective(PmProjectsObjectiveDto obj)
        {

            try
            {
                var chk = await permission.HasPermission(1727, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (string.IsNullOrEmpty(obj.Objective))
                    return Ok(await Result<object>.FailAsync("الأهداف"));

                if (string.IsNullOrEmpty(obj.SuccessCriteria))
                    return Ok(await Result<object>.FailAsync("معايير النجاح"));

                if (string.IsNullOrEmpty(obj.Responsible))
                    return Ok(await Result<object>.FailAsync("الشخص المعتمد"));
                if (obj.ProjectId <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("Project")));
                var result = await pMServiceManager.PmProjectsObjectiveService.Add(obj);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"======= Exp in {this.GetType()}: {ex.Message}"));

            }

        }

        [HttpPost("EditObjective")]
        public async Task<IActionResult> EditObjective(PmProjectsObjectiveEditDto obj)
        {

            try
            {
                var chk = await permission.HasPermission(1727, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (obj.Id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));

                if (string.IsNullOrEmpty(obj.Objective))
                    return Ok(await Result<object>.FailAsync("الأهداف"));

                if (string.IsNullOrEmpty(obj.SuccessCriteria))
                    return Ok(await Result<object>.FailAsync("معايير النجاح"));

                if (string.IsNullOrEmpty(obj.Responsible))
                    return Ok(await Result<object>.FailAsync("الشخص المعتمد"));
                if (obj.ProjectId <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("Project")));
                var result = await pMServiceManager.PmProjectsObjectiveService.Update(obj);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"======= Exp in {this.GetType()}: {ex.Message}"));

            }

        }


        [HttpDelete("DeleteObjective")]
        public async Task<IActionResult> DeleteObjective(long id)
        {
            try
            {
                var chk = await permission.HasPermission(1727, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await pMServiceManager.PmProjectsObjectiveService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete Objective in PMCharters Controller, MESSAGE: {ex.Message}"));
            }
        }

        #endregion




        #region   تعديل  ارتباط المشروع بالأهداف الاستراتيجية 


        [HttpPost("AddStrategicLink")]
        public async Task<IActionResult> AddStrategicLink(PmProjectsStrategicLinkDto obj)
        {

            try
            {
                obj.ProjectId ??= 0;
                obj.Rate ??= 0;
                obj.StrategicObjective ??= 0;
                var chk = await permission.HasPermission(1727, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (obj.Rate == null)
                    return Ok(await Result<object>.FailAsync("مدى المساهمة"));

                if (obj.Rate <= 0)
                    return Ok(await Result<object>.FailAsync("المساهمة يجب أن تكون نسبة مئوية من 100 "));


                if (obj.StrategicObjective <= 0)
                    return Ok(await Result<object>.FailAsync("  الهدف الاستراتيجي "));

                if (obj.ProjectId <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("Project")));
                var result = await pMServiceManager.PmProjectsStrategicLinkService.Add(obj);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"======= Exp in {this.GetType()}: {ex.Message}"));

            }

        }

        [HttpPost("EditStrategicLink")]
        public async Task<IActionResult> EditStrategicLink(PmProjectsStrategicLinkEditDto obj)
        {

            try
            {
                obj.ProjectId ??= 0;
                obj.Rate ??= 0;
                obj.StrategicObjective ??= 0;
                var chk = await permission.HasPermission(1727, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (obj.Id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));

                if (obj.Rate == null)
                    return Ok(await Result<object>.FailAsync("مدى المساهمة"));

                if (obj.Rate <= 0)
                    return Ok(await Result<object>.FailAsync("المساهمة يجب أن تكون نسبة مئوية من 100 "));


                if (obj.StrategicObjective <= 0)
                    return Ok(await Result<object>.FailAsync("  الهدف الاستراتيجي "));

                if (obj.ProjectId <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("Project")));
                var result = await pMServiceManager.PmProjectsStrategicLinkService.Update(obj);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"======= Exp in {this.GetType()}: {ex.Message}"));

            }

        }


        [HttpDelete("DeleteStrategicLink")]
        public async Task<IActionResult> DeleteStrategicLink(long id)
        {
            try
            {
                var chk = await permission.HasPermission(1727, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await pMServiceManager.PmProjectsStrategicLinkService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete Objective in PMCharters Controller, MESSAGE: {ex.Message}"));
            }
        }

        #endregion


        #region   تعديل ارتباط المشروع بمشاريع اخرى  


        [HttpPost("AddInterconnection")]
        public async Task<IActionResult> AddInterconnection(PmProjectsInterconnectionDto obj)
        {

            try
            {
                obj.ProjectId ??= 0;
                obj.LinkProjectId ??= 0;
                var chk = await permission.HasPermission(1727, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (obj.ProjectId <= 0)
                    return Ok(await Result<object>.FailAsync(" رقم المشروع  "));
                var result = await pMServiceManager.PmProjectsInterconnectionService.Add(obj);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"======= Exp in {this.GetType()}: {ex.Message}"));

            }

        }

        [HttpPost("EditInterconnection")]
        public async Task<IActionResult> EditInterconnection(PmProjectsInterconnectionEditDto obj)
        {

            try
            {
                obj.ProjectId ??= 0;
                obj.LinkProjectId ??= 0;
                var chk = await permission.HasPermission(1727, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (obj.Id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));

                if (obj.ProjectId <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("Project")));

                if (string.IsNullOrEmpty(obj.OtherProjectName))
                    return Ok(await Result<object>.FailAsync("اسم المشروع الاخر "));

                var result = await pMServiceManager.PmProjectsInterconnectionService.Update(obj);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"======= Exp in {this.GetType()}: {ex.Message}"));

            }

        }


        [HttpDelete("DeleteInterconnection")]
        public async Task<IActionResult> DeleteInterconnection(long id)
        {
            try
            {
                var chk = await permission.HasPermission(1727, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await pMServiceManager.PmProjectsInterconnectionService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete Objective in PMCharters Controller, MESSAGE: {ex.Message}"));
            }
        }

        #endregion



        #region   تعديل توصيف نطاق عمل المشروع  



        [HttpPost("EditProjectScope")]
        public async Task<IActionResult> EditProjectScope(PMProjectScopeEditDto obj)
        {

            try
            {

                var chk = await permission.HasPermission(1727, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (obj.ProjectId <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("Project")));

                var result = await pMServiceManager.PMProjectsService.EditProjectScope(obj);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"======= Exp in {this.GetType()}: {ex.Message}"));

            }

        }


        #endregion


        #region   تعديل  مؤشرات اداء المشروع 


        [HttpPost("AddPerformanceIndicator")]
        public async Task<IActionResult> AddPerformanceIndicator(PmProjectsPerformanceIndicatorDto obj)
        {

            try
            {
                obj.ProjectId ??= 0;
                var chk = await permission.HasPermission(1727, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (string.IsNullOrEmpty(obj.Indicator))
                    return Ok(await Result<object>.FailAsync("المؤشر"));

                if (string.IsNullOrEmpty(obj.Target))
                    return Ok(await Result<object>.FailAsync("المستهدف"));

                if (string.IsNullOrEmpty(obj.Baseline))
                    return Ok(await Result<object>.FailAsync("خط الاساس"));

                if (string.IsNullOrEmpty(obj.Responsible))
                    return Ok(await Result<object>.FailAsync("المسؤول"));

                if (obj.ProjectId <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("Project")));

                var result = await pMServiceManager.PmProjectsPerformanceIndicatorService.Add(obj);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"======= Exp in {this.GetType()}: {ex.Message}"));

            }

        }

        [HttpPost("EditPerformanceIndicator")]
        public async Task<IActionResult> EditPerformanceIndicator(PmProjectsPerformanceIndicatorEditDto obj)
        {

            try
            {

                var chk = await permission.HasPermission(1727, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (obj.Id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                obj.ProjectId ??= 0;
                if (string.IsNullOrEmpty(obj.Indicator))
                    return Ok(await Result<object>.FailAsync("المؤشر"));
                if (string.IsNullOrEmpty(obj.Target))
                    return Ok(await Result<object>.FailAsync("المستهدف"));
                if (string.IsNullOrEmpty(obj.Baseline))
                    return Ok(await Result<object>.FailAsync("خط الاساس"));
                if (string.IsNullOrEmpty(obj.Responsible))
                    return Ok(await Result<object>.FailAsync("المسؤول"));
                var result = await pMServiceManager.PmProjectsPerformanceIndicatorService.Update(obj);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"======= Exp in {this.GetType()}: {ex.Message}"));

            }

        }


        [HttpDelete("DeletePerformanceIndicator")]
        public async Task<IActionResult> DeletePerformanceIndicator(long id)
        {
            try
            {
                var chk = await permission.HasPermission(1727, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await pMServiceManager.PmProjectsPerformanceIndicatorService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete PerformanceIndicator  in PMCharters Controller, MESSAGE: {ex.Message}"));
            }
        }

        #endregion

        #region   تعديل  مخرجات المشروع 


        [HttpPost("AddDeliverable")]
        public async Task<IActionResult> AddDeliverable(PmProjectsDeliverableDto obj)
        {

            try
            {
                obj.ProjectId ??= 0;
                obj.StatusId ??= 0;
                obj.Cost ??= 0;
                obj.PItemId ??= 0;
                var chk = await permission.HasPermission(1727, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));


                if (obj.StatusId <= 0)
                    obj.StatusId = 0;

                if (obj.Cost <= 0)
                    obj.Cost = 0;


                if (obj.PItemId <= 0)
                    obj.PItemId = 0;
                if (string.IsNullOrEmpty(obj.Name))
                    return Ok(await Result<object>.FailAsync("المخرج"));

                if (string.IsNullOrEmpty(obj.Type))
                    return Ok(await Result<object>.FailAsync("النوع"));

                if (obj.Qty <= 0)
                    return Ok(await Result<object>.FailAsync("العدد"));
                if (string.IsNullOrEmpty(obj.DeliveryDate))
                    return Ok(await Result<object>.FailAsync("موعد التسليم"));

                if (obj.ProjectId <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("Project")));



                var result = await pMServiceManager.PmProjectsDeliverableService.Add(obj);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"======= Exp in {this.GetType()}: {ex.Message}"));

            }

        }

        [HttpPost("EditDeliverable")]
        public async Task<IActionResult> EditDeliverable(PmProjectsDeliverableEditDto obj)
        {

            try
            {

                var chk = await permission.HasPermission(1727, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (obj.Id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));

                obj.ProjectId ??= 0;
                obj.StatusId ??= 0;
                obj.Cost ??= 0;
                obj.PItemId ??= 0;


                if (obj.StatusId <= 0)
                    obj.StatusId = 0;

                if (obj.Cost <= 0)
                    obj.Cost = 0;

                if (obj.PItemId <= 0)
                    obj.PItemId = 0;

                if (string.IsNullOrEmpty(obj.Name))
                    return Ok(await Result<object>.FailAsync("المخرج"));

                if (string.IsNullOrEmpty(obj.Type))
                    return Ok(await Result<object>.FailAsync("النوع"));

                if (obj.Qty <= 0)
                    return Ok(await Result<object>.FailAsync("العدد"));
                if (string.IsNullOrEmpty(obj.DeliveryDate))
                    return Ok(await Result<object>.FailAsync("موعد التسليم"));

                if (obj.ProjectId <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("Project")));



                var result = await pMServiceManager.PmProjectsDeliverableService.Update(obj);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"======= Exp in {this.GetType()}: {ex.Message}"));

            }

        }


        [HttpDelete("DeleteDeliverable")]
        public async Task<IActionResult> DeleteDeliverable(long id)
        {
            try
            {
                var chk = await permission.HasPermission(1727, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await pMServiceManager.PmProjectsDeliverableService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete Deliverable  in PMCharters Controller, MESSAGE: {ex.Message}"));
            }
        }


        #endregion



        #region   معايير قبول المخرجات 


        [HttpPost("AddDeliverableAcceptCriteria")]
        public async Task<IActionResult> AddDeliverableAcceptCriteria(PmProjectsDeliverableAcceptCriterionDto obj)
        {

            try
            {

                var chk = await permission.HasPermission(1727, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                obj.DeliverableId ??= 0;
                obj.ProjectId ??= 0;
                if (obj.DeliverableId <= 0)
                    return Ok(await Result<object>.FailAsync("المخرج"));

                if (string.IsNullOrEmpty(obj.OutputDescription))
                    return Ok(await Result<object>.FailAsync(" وصف المخرج"));


                if (string.IsNullOrEmpty(obj.Criteria))
                    return Ok(await Result<object>.FailAsync("المعايير"));

                if (string.IsNullOrEmpty(obj.OutputFormat))
                    return Ok(await Result<object>.FailAsync("صيغة المخرج"));

                if (string.IsNullOrEmpty(obj.Responsible))
                    return Ok(await Result<object>.FailAsync("مسؤول الاعتماد للمخرج"));

                if (obj.ProjectId <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("Project")));

                var result = await pMServiceManager.PmProjectsDeliverableAcceptCriterionService.Add(obj);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"======= Exp in {this.GetType()}: {ex.Message}"));

            }

        }

        [HttpPost("EditDeliverableAcceptCriteria")]
        public async Task<IActionResult> EditDeliverableAcceptCriteria(PmProjectsDeliverableAcceptCriterionEditDto obj)
        {

            try
            {

                var chk = await permission.HasPermission(1727, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (obj.Id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));

                obj.ProjectId ??= 0;
                obj.DeliverableId ??= 0;

                if (obj.DeliverableId <= 0)
                    return Ok(await Result<object>.FailAsync("المخرج"));

                if (string.IsNullOrEmpty(obj.OutputDescription))
                    return Ok(await Result<object>.FailAsync(" وصف المخرج"));


                if (string.IsNullOrEmpty(obj.Criteria))
                    return Ok(await Result<object>.FailAsync("المعايير"));

                if (string.IsNullOrEmpty(obj.OutputFormat))
                    return Ok(await Result<object>.FailAsync("صيغة المخرج"));

                if (string.IsNullOrEmpty(obj.Responsible))
                    return Ok(await Result<object>.FailAsync("مسؤول الاعتماد للمخرج"));

                if (obj.ProjectId <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("Project")));

                var result = await pMServiceManager.PmProjectsDeliverableAcceptCriterionService.Update(obj);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"======= Exp in {this.GetType()}: {ex.Message}"));

            }

        }


        [HttpDelete("DeleteDeliverableAcceptCriteria")]
        public async Task<IActionResult> DeleteDeliverableAcceptCriteria(long id)
        {
            try
            {
                var chk = await permission.HasPermission(1727, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await pMServiceManager.PmProjectsDeliverableAcceptCriterionService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete DeliverableAcceptCriteria  in PMCharters Controller, MESSAGE: {ex.Message}"));
            }
        }


        #endregion


        #region  معالم / مراحل المشروع الرئيسية 


        [HttpPost("AddPhaseOrMilestone")]
        public async Task<IActionResult> AddPhaseOrMilestone(TsTaskDto obj)
        {

            try
            {

                var chk = await permission.HasPermission(1727, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                obj.ProjectId ??= 0;
                obj.WorkFlowType ??= 0;
                obj.SendEmailNotifications = false;
                obj.RequiredApprovalPercentage = 0;
                obj.ParentId = 0;
                obj.Priority = 0;
                obj.StatusId = 0;
                obj.ReferenceCode = "0";
                obj.DeptId = 0;
                obj.Depend = "0";
                obj.ProjectPlansId = 0;
                obj.CompletionRate = 0;
                obj.PercentageOfProject = 0;
                obj.DeliverableId ??= 0;
                obj.Qty ??= 0;
                obj.UnitPrice ??= 0;
                obj.UnitPriceTotal ??= 0;
                obj.ActivityId ??= 0;
                obj.ClassificationId ??= 0;


                if (string.IsNullOrEmpty(obj.Subject))
                    return Ok(await Result<object>.FailAsync("المعلم / مراحل"));

                if (string.IsNullOrEmpty(obj.DueDate))
                    return Ok(await Result<object>.FailAsync(" موعد التسليم"));

                if (obj.ProjectId <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("Project")));

                var checkIfProjectExist = await pMServiceManager.PMProjectsService.GetOne(x => x.IsDeleted == false && x.Id == obj.ProjectId);
                if (checkIfProjectExist.Succeeded == false || checkIfProjectExist.Data == null)
                    return Ok(await Result<object>.FailAsync("المشروع غير موجود"));

                obj.FacilityId = session.FacilityId;
                obj.TypeId = 2;
                var result = await tsServiceManager.TsTaskService.Add(obj);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"======= Exp in {this.GetType()}: {ex.Message}"));

            }

        }

        [HttpPost("EditPhaseOrMilestone")]
        public async Task<IActionResult> EditPhaseOrMilestone(TsTaskEditDto obj)
        {

            try
            {

                var chk = await permission.HasPermission(1727, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (obj.Id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));

                obj.ProjectId ??= 0;
                obj.WorkFlowType ??= 0;
                obj.SendEmailNotifications = false;
                obj.RequiredApprovalPercentage = 0;
                obj.ParentId = 0;
                obj.Priority = 0;
                obj.StatusId = 0;
                obj.ReferenceCode = "0";
                obj.DeptId = 0;
                obj.Depend = "0";
                obj.ProjectPlansId = 0;
                obj.CompletionRate = 0;
                obj.PercentageOfProject = 0;
                obj.DeliverableId ??= 0;
                obj.Qty ??= 0;
                obj.UnitPrice ??= 0;
                obj.UnitPriceTotal ??= 0;
                obj.ActivityId ??= 0;
                obj.ClassificationId ??= 0;


                if (string.IsNullOrEmpty(obj.Subject))
                    return Ok(await Result<object>.FailAsync("المعلم / مراحل"));

                if (string.IsNullOrEmpty(obj.DueDate))
                    return Ok(await Result<object>.FailAsync(" موعد التسليم"));

                if (obj.ProjectId <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("Project")));

                var checkIfProjectExist = await pMServiceManager.PMProjectsService.GetOne(x => x.IsDeleted == false && x.Id == obj.ProjectId);
                if (checkIfProjectExist.Succeeded == false || checkIfProjectExist.Data == null)
                    return Ok(await Result<object>.FailAsync("المشروع غير موجود"));

                obj.FacilityId = session.FacilityId;
                obj.TypeId = 2;

                var result = await tsServiceManager.TsTaskService.Update(obj);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"======= Exp in {this.GetType()}: {ex.Message}"));

            }

        }


        [HttpDelete("DeletePhaseOrMilestone")]
        public async Task<IActionResult> DeletePhaseOrMilestone(long id)
        {
            try
            {
                var chk = await permission.HasPermission(1727, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await tsServiceManager.TsTaskService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete PhaseOrMilestone  in PMCharters Controller, MESSAGE: {ex.Message}"));
            }
        }


        #endregion



        #region  قائمة أصحاب المصلحة 


        [HttpPost("AddStokeholder")]
        public async Task<IActionResult> AddStokeholder(PMProjectsStokeholderDto obj)
        {

            try
            {

                var chk = await permission.HasPermission(1727, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));


                obj.ProjectId ??= 0;
                obj.InternalOrExternal ??= 0;
                obj.TypeId ??= 0;

                if (string.IsNullOrEmpty(obj.Name))
                    return Ok(await Result<object>.FailAsync("صاحب المصلحة"));

                if (string.IsNullOrEmpty(obj.JobName))
                    return Ok(await Result<object>.FailAsync(" الدور"));

                if (obj.InternalOrExternal <= 0)
                    return Ok(await Result<object>.FailAsync("داخلي / خارجي "));

                if (obj.ProjectId <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("Project")));

                var result = await pMServiceManager.PMProjectsStokeholderService.Add(obj);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"======= Exp in {this.GetType()}: {ex.Message}"));

            }

        }

        [HttpPost("EditStokeholder")]
        public async Task<IActionResult> EditStokeholder(PmProjectsStokeholderEditDto obj)
        {

            try
            {

                var chk = await permission.HasPermission(1727, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (obj.Id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));

                obj.ProjectId ??= 0;
                obj.InternalOrExternal ??= 0;
                obj.TypeId ??= 0;

                if (string.IsNullOrEmpty(obj.Name))
                    return Ok(await Result<object>.FailAsync("صاحب المصلحة"));

                if (string.IsNullOrEmpty(obj.JobName))
                    return Ok(await Result<object>.FailAsync(" الدور"));

                if (obj.InternalOrExternal <= 0)
                    return Ok(await Result<object>.FailAsync("داخلي / خارجي "));

                if (obj.ProjectId <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("Project")));

                var result = await pMServiceManager.PMProjectsStokeholderService.Update(obj);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"======= Exp in {this.GetType()}: {ex.Message}"));

            }

        }

        [HttpDelete("DeleteStokeholder")]
        public async Task<IActionResult> DeleteStokeholder(long id)
        {
            try
            {
                var chk = await permission.HasPermission(1727, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await pMServiceManager.PMProjectsStokeholderService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete Stokeholder  in PMCharters Controller, MESSAGE: {ex.Message}"));
            }
        }


        #endregion


        #region  فريق عمل المشروع 


        [HttpPost("AddStaff")]
        public async Task<IActionResult> AddStaff(PmProjectsStaffDto obj)
        {

            try
            {
                var chk = await permission.HasPermission(1727, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));


                obj.ProjectId ??= 0;
                obj.TypeId = 0;
                obj.DepId = 0;
                obj.PlanningTeam = false;
                obj.ParentId = 0;
                obj.DateH = DateTime.Now.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);
                obj.Responsibility = "0";
                obj.ImplementationTeam = false;

                if (string.IsNullOrEmpty(obj.Responsibility))
                    return Ok(await Result<object>.FailAsync(" المسؤولية"));

                if (obj.EmpId <= 0)
                    return Ok(await Result<object>.FailAsync("الاسم"));

                if (obj.ProjectId <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("Project")));

                var result = await pMServiceManager.PMProjectsStaffService.AddStaffInCharters(obj);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"======= Exp in {this.GetType()}: {ex.Message}"));

            }

        }

        [HttpPost("EditStaff")]
        public async Task<IActionResult> EditStaff(PmProjectsStaffEditDto obj)
        {

            try
            {

                var chk = await permission.HasPermission(1727, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (obj.Id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));

                obj.ProjectId ??= 0;
                obj.TypeId = 0;
                obj.DepId = 0;
                obj.PlanningTeam = false;
                obj.ParentId = 0;
                obj.DateH = DateTime.Now.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);
                obj.Responsibility = "0";
                obj.ImplementationTeam = false;

                if (string.IsNullOrEmpty(obj.Responsibility))
                    return Ok(await Result<object>.FailAsync(" المسؤولية"));

                if (obj.EmpId <= 0)
                    return Ok(await Result<object>.FailAsync("الاسم"));

                if (obj.ProjectId <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("Project")));

                var result = await pMServiceManager.PMProjectsStaffService.EditStaffInCharters(obj);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"======= Exp in {this.GetType()}: {ex.Message}"));

            }

        }

        [HttpDelete("DeleteStaff")]
        public async Task<IActionResult> DeleteStaff(long id)
        {
            try
            {
                var chk = await permission.HasPermission(1727, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await pMServiceManager.PMProjectsStaffService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete Staff  in PMCharters Controller, MESSAGE: {ex.Message}"));
            }
        }


        #endregion

        #region  مخاطر وعوائق المشروع 


        [HttpPost("AddRisks")]
        public async Task<IActionResult> AddRisks(PmProjectsRisk2Dto obj)
        {

            try
            {
                var chk = await permission.HasPermission(1727, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));


                obj.ProjectId ??= 0;
                obj.EmpId ??= 0;
                obj.Effect ??= 0;
                obj.Impact ??= 0;


                if (string.IsNullOrEmpty(obj.Description))
                    return Ok(await Result<object>.FailAsync(" وصف الخطر / العائق"));

                if (obj.ProjectId <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("Project")));

                var result = await pMServiceManager.PMProjectsRiskService.Add(obj);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"======= Exp in {this.GetType()}: {ex.Message}"));

            }

        }

        [HttpPost("EditRisks")]
        public async Task<IActionResult> EditRisks(PmProjectsRisk2Dto obj)
        {

            try
            {

                var chk = await permission.HasPermission(1727, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (obj.Id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));

                obj.ProjectId ??= 0;
                obj.EmpId ??= 0;
                obj.Effect ??= 0;
                obj.Impact ??= 0;

                if (string.IsNullOrEmpty(obj.Description))
                    return Ok(await Result<object>.FailAsync(" وصف الخطر / العائق"));

                if (obj.ProjectId <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("Project")));

                var result = await pMServiceManager.PMProjectsRiskService.Update(obj);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"======= Exp in {this.GetType()}: {ex.Message}"));

            }

        }

        [HttpDelete("DeleteRisks")]
        public async Task<IActionResult> DeleteRisks(long id)
        {
            try
            {
                var chk = await permission.HasPermission(1727, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await pMServiceManager.PMProjectsRiskService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete Risks  in PMCharters Controller, MESSAGE: {ex.Message}"));
            }
        }


        #endregion

        #region  افتراضات المشروع 


        [HttpPost("AddAssumption")]
        public async Task<IActionResult> AddAssumption(PmProjectsAssumptionDto obj)
        {

            try
            {
                var chk = await permission.HasPermission(1727, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));


                obj.ProjectId ??= 0;

                if (string.IsNullOrEmpty(obj.Assumption))
                    return Ok(await Result<object>.FailAsync(" الافتراض"));

                if (obj.ProjectId <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("Project")));

                var result = await pMServiceManager.PmProjectsAssumptionService.Add(obj);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"======= Exp in {this.GetType()}: {ex.Message}"));

            }

        }

        [HttpPost("EditAssumption")]
        public async Task<IActionResult> EditAssumption(PmProjectsAssumptionEditDto obj)
        {

            try
            {

                var chk = await permission.HasPermission(1727, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (obj.Id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));

                obj.ProjectId ??= 0;

                if (string.IsNullOrEmpty(obj.Assumption))
                    return Ok(await Result<object>.FailAsync(" الافتراض"));

                if (obj.ProjectId <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("Project")));

                var result = await pMServiceManager.PmProjectsAssumptionService.Update(obj);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"======= Exp in {this.GetType()}: {ex.Message}"));

            }

        }

        [HttpDelete("DeleteAssumption")]
        public async Task<IActionResult> DeleteAssumption(long id)
        {
            try
            {
                var chk = await permission.HasPermission(1727, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await pMServiceManager.PmProjectsAssumptionService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete Assumption  in PMCharters Controller, MESSAGE: {ex.Message}"));
            }
        }


        #endregion



        #region  الموارد المطلوبة للمشروع 


        [HttpPost("AddResource")]
        public async Task<IActionResult> AddResource(PmProjectsResourceDto obj)
        {

            try
            {

                var chk = await permission.HasPermission(1727, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));


                obj.ProjectId ??= 0;
                obj.InternalOrExternal ??= 0;
                obj.DeptId ??= 0;
                obj.ManagerId ??= 0;
                obj.AppId = 0;


                if (string.IsNullOrEmpty(obj.ResourceType))
                    return Ok(await Result<object>.FailAsync(" (نوع المورد (الوصف "));

                if (obj.InternalOrExternal <= 0)
                    return Ok(await Result<object>.FailAsync("داخلي / خارجي "));

                if (obj.ProjectId <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("Project")));

                var result = await pMServiceManager.PmProjectsResourceService.Add(obj);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"======= Exp in {this.GetType()}: {ex.Message}"));

            }

        }

        [HttpPost("EditResource")]
        public async Task<IActionResult> EditResource(PmProjectsResourceEditDto obj)
        {

            try
            {

                var chk = await permission.HasPermission(1727, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (obj.Id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));

                obj.ProjectId ??= 0;
                obj.InternalOrExternal ??= 0;
                obj.DeptId ??= 0;
                obj.ManagerId ??= 0;
                obj.AppId = 0;


                if (string.IsNullOrEmpty(obj.ResourceType))
                    return Ok(await Result<object>.FailAsync(" (نوع المورد (الوصف "));

                if (obj.InternalOrExternal <= 0)
                    return Ok(await Result<object>.FailAsync("داخلي / خارجي "));

                if (obj.ProjectId <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("Project")));

                var result = await pMServiceManager.PmProjectsResourceService.Update(obj);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"======= Exp in {this.GetType()}: {ex.Message}"));

            }

        }

        [HttpDelete("DeleteResource")]
        public async Task<IActionResult> DeleteResource(long id)
        {
            try
            {
                var chk = await permission.HasPermission(1727, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await pMServiceManager.PmProjectsResourceService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete Resource  in PMCharters Controller, MESSAGE: {ex.Message}"));
            }
        }
        //  ارسال  المواردالى سير العمل
        [HttpPost("SendResourceToWorkflow")]
        public async Task<IActionResult> SendResourceToWorkflow(long id)
        {
            try
            {
                var chk = await permission.HasPermission(1727, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await pMServiceManager.PmProjectsResourceService.SendResourceToWorkflow(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in SendResourceToWorkflow  in PMCharters Controller, MESSAGE: {ex.Message}"));
            }
        }

        #endregion


        #region  حوكمة المشروع 


        [HttpPost("AddGovernance")]
        public async Task<IActionResult> AddGovernance(PmProjectsGovernanceDto obj)
        {

            try
            {

                var chk = await permission.HasPermission(1727, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));


                obj.ProjectId ??= 0;

                if (string.IsNullOrEmpty(obj.Team1))
                    return Ok(await Result<object>.FailAsync("أطراف العلاقة الداخلية"));

                if (string.IsNullOrEmpty(obj.Team2))
                    return Ok(await Result<object>.FailAsync("أطراف العلاقة الخارجية"));

                if (string.IsNullOrEmpty(obj.MeetingPeriodicity))
                    return Ok(await Result<object>.FailAsync("دورية الإجتماعات"));

                if (string.IsNullOrEmpty(obj.Tasks))
                    return Ok(await Result<object>.FailAsync("الادوار"));

                if (obj.ProjectId <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("Project")));

                var result = await pMServiceManager.PmProjectsGovernanceService.Add(obj);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"======= Exp in {this.GetType()}: {ex.Message}"));

            }

        }

        [HttpPost("EditGovernance")]
        public async Task<IActionResult> EditGovernance(PmProjectsGovernanceEditDto obj)
        {

            try
            {

                var chk = await permission.HasPermission(1727, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (obj.Id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));

                obj.ProjectId ??= 0;

                if (string.IsNullOrEmpty(obj.Team1))
                    return Ok(await Result<object>.FailAsync("أطراف العلاقة الداخلية"));

                if (string.IsNullOrEmpty(obj.Team2))
                    return Ok(await Result<object>.FailAsync("أطراف العلاقة الخارجية"));

                if (string.IsNullOrEmpty(obj.MeetingPeriodicity))
                    return Ok(await Result<object>.FailAsync("دورية الإجتماعات"));

                if (string.IsNullOrEmpty(obj.Tasks))
                    return Ok(await Result<object>.FailAsync("الادوار"));

                if (obj.ProjectId <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("Project")));

                var result = await pMServiceManager.PmProjectsGovernanceService.Update(obj);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"======= Exp in {this.GetType()}: {ex.Message}"));

            }

        }

        [HttpDelete("DeleteGovernance")]
        public async Task<IActionResult> DeleteGovernance(long id)
        {
            try
            {
                var chk = await permission.HasPermission(1727, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await pMServiceManager.PmProjectsGovernanceService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete Governance  in PMCharters Controller, MESSAGE: {ex.Message}"));
            }
        }


        #endregion

        #region   ارسال  المشروع الى  سير العمل



        /// <summary>
        /// 
        /// </summary>
        /// <param name="ProjectId">رقم المشروع</param>
        /// <param name="CompleteRate">نسبة اكتمال الميثاق</param>
        /// <param name="AppTypeId">رقم الطلب في الخدمة الذاتية </param>
        /// <returns></returns>
        [HttpPost("SendToWorkflow")]
        public async Task<IActionResult> SendToWorkflow(long ProjectId, int CompleteRate, int AppTypeId = 0)
        {
            try
            {
                var chk = await permission.HasPermission(1727, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (ProjectId <= 0)
                    return Ok(await Result.FailAsync($"رقم المشروع "));
                if (CompleteRate != 100)
                    return Ok(await Result.FailAsync($"عفوا, لم نتمكن من ارسال الطلب بنجاح الرجاء التأكد من نسبة  اكتمال الميثاق"));
                var delete = await pMServiceManager.PMProjectsService.SendCharterToWorkFlow(ProjectId, CompleteRate, AppTypeId);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in SendToWorkflow  in PMCharters Controller, MESSAGE: {ex.Message}"));
            }
        }
        #endregion

        #endregion




        #region Add Page


        [HttpGet("GetManagerAndOwnerOnPageLoad")]
        public async Task<IActionResult> GetManagerAndOwnerOnPageLoad()
        {

            try
            {

                var chk = await permission.HasPermission(1727, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (session.EmpId <= 0)
                    return Ok(await Result.AccessDenied("AccessDenied"));


                //------------------------- مدير  المشروع
                var GetCurrentUser = await hrServiceManager.HrEmployeeService.GetOne(x => x.Id == session.EmpId);
                if (GetCurrentUser.Data == null)
                    return Ok(await Result<string>.FailAsync(GetCurrentUser.Status.message));

                string OwnerCode = string.Empty;
                string OwnerName = string.Empty;
                string ManagerCode = GetCurrentUser.Data.EmpId ?? "";
                string ManagerName = (session.Language == 1 ? (GetCurrentUser.Data.EmpName ?? "") : (GetCurrentUser.Data.EmpName2 ?? ""));
                if (GetCurrentUser.Data.ManagerId > 0)
                {
                    //------------------------- مالك المشروع

                    var GetOwnerData = await hrServiceManager.HrEmployeeService.GetOne(x => x.Id == GetCurrentUser.Data.ManagerId);
                    if (GetOwnerData.Data != null)
                    {
                        OwnerCode = GetOwnerData.Data.EmpId ?? "";
                        OwnerName = (session.Language == 1 ? (GetOwnerData.Data.EmpName ?? "") : (GetOwnerData.Data.EmpName2 ?? ""));

                    }

                }
                var result = new
                {
                    ManagerCode = ManagerCode,
                    ManagerName = ManagerName,
                    OwnerCode = OwnerCode,
                    OwnerName = OwnerName,
                };
                return Ok(await Result<object>.SuccessAsync(result, ""));
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"======= Exp in {this.GetType()}: {ex.Message}"));

            }

        }


        [HttpPost("Add")]
        public async Task<IActionResult> Add(PmProjectsCharterAddDto obj)
        {

            try
            {
                //  DDLProjectOwner   الاداراة
                obj.OwnerDept ??= 0;
                obj.Duration ??= 0;
                obj.DurationType ??= 0;
                var chk = await permission.HasPermission(1727, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));


                if (string.IsNullOrEmpty(obj.ProjectName))
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("ProjectName")));

                if (string.IsNullOrEmpty(obj.OwnerCode))
                    return Ok(await Result<object>.FailAsync("مالك المشروع"));

                if (string.IsNullOrEmpty(obj.ManagerCode))
                    return Ok(await Result<object>.FailAsync("مدير  المشروع"));

                if (string.IsNullOrEmpty(obj.StartDate))
                    return Ok(await Result<object>.FailAsync("تاريخ البدء المخطط"));


                if (obj.Duration <= 0)
                    return Ok(await Result<object>.FailAsync("المدة المخططة لتنفيذ المشروع"));

                if (obj.DurationType <= 0)
                    return Ok(await Result<object>.FailAsync("المدة المخططة لتنفيذ المشروع"));


                if (string.IsNullOrEmpty(obj.EndDate))
                    return Ok(await Result<object>.FailAsync("تاريخ الإنتهاء المخطط"));

                if (DateHelper.StringToDate(obj.StartDate) >= DateHelper.StringToDate(obj.EndDate))
                    return Ok(await Result<object>.FailAsync("تاريخ بداية المشروع يجب ان يكون قبل تاريخ نهاية المشروع"));

                if (obj.EstimatedCost <= 0)
                    return Ok(await Result<object>.FailAsync("القيمة التقديرية للمشروع"));

                if (obj.AllowRate <= 0)
                    return Ok(await Result<object>.FailAsync("نسبة السماح في الميزانية"));

                var result = await pMServiceManager.PMProjectsService.AddNewCharter(obj);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"======= Exp in {this.GetType()}: {ex.Message}"));

            }

        }

        #endregion

    }
}
