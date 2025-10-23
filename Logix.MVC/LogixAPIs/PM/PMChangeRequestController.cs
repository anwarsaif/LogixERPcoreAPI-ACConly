using Logix.Application.Common;
using Logix.Application.DTOs.Main;
using Logix.Application.DTOs.PM;
using Logix.Application.DTOs.PM.PmProjects;
using Logix.Application.DTOs.PM.PmProjectsStaff;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.PM;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Evaluation;
using Microsoft.CodeAnalysis;
using System.Globalization;

namespace Logix.MVC.LogixAPIs.PM
{
    //  طلبات التغيير
    public class PMChangeRequestController : BasePMApiController
    {
        private readonly IPMServiceManager pMServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IWFServiceManager wFServiceManager;
        private readonly IMainServiceManager mainServiceManager;


        public PMChangeRequestController(IPMServiceManager pMServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IWFServiceManager wFServiceManager, IMainServiceManager mainServiceManager)
        {
            this.pMServiceManager = pMServiceManager;
            this.permission = permission;
            this.session = session;
            this.localization = localization;
            this.wFServiceManager = wFServiceManager;
            this.mainServiceManager = mainServiceManager;
        }

        #region Index Page


        [HttpPost("Search")]
        public async Task<IActionResult> Search(PMChangeRequestFilterDto filter)
        {
            var chk = await permission.HasPermission(1729, PermissionType.Show);

            if (!chk)
                return Ok(await Result.AccessDenied("AccessDenied"));

            try
            {
                filter.CrType ??= 0;
                filter.IsDiscussed ??= 0;
                filter.EffectType ??= 0;
                filter.Priority ??= 0;
                filter.CrCost ??= 0;

                var items = await pMServiceManager.PmChangeRequestService.GetAllVW(e => e.IsDeleted == false
                && (filter.CrType == 0 || e.CrType == filter.CrType)
                && (filter.IsDiscussed == 0 || e.IsDiscussed == filter.IsDiscussed)
                && (filter.EffectType == 0 || e.EffectType == filter.EffectType)
                && (filter.Priority == 0 || e.Priority == filter.Priority)
                && (filter.CrCost == 0 || e.CrCost == filter.CrCost)
                && (string.IsNullOrEmpty(filter.ProjectName) || ((e.ProjectName != null && e.ProjectName.Contains(filter.ProjectName))))
                && (string.IsNullOrEmpty(filter.ProjectCode) || ((e.ProjectCode == Convert.ToInt64(filter.ProjectCode))))
                );

                if (!items.Succeeded)
                    return Ok(await Result<object>.FailAsync(items.Status.message));
                if (!items.Data.Any())
                    return Ok(await Result<object>.SuccessAsync(new List<object>(), localization.GetResource1("NosearchResult")));

                var res = items.Data.AsQueryable();

                if (!string.IsNullOrEmpty(filter.From) && !string.IsNullOrEmpty(filter.To))
                {
                    var DateFrom = DateHelper.StringToDate(filter.From);
                    var DateTo = DateHelper.StringToDate(filter.To);
                    res = res.Where(x => x.CrDate != null && x.CrDate != "" && DateHelper.StringToDate(x.CrDate) >= DateFrom && DateHelper.StringToDate(x.CrDate) <= DateTo);
                }

                if (!res.Any())
                    return Ok(await Result<object>.SuccessAsync(new List<object>(), localization.GetResource1("NosearchResult")));


                var result = res.Select(g => new
                {
                    g.Id,
                    g.ProjectCode,
                    g.ProjectName,
                    g.CrDate,
                    g.Description,
                    g.CrCost,
                    g.PriorityName,
                    g.CrTypeName,
                    g.EffectTypeName,
                    g.ChangeProjectCost,
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
                var chk = await permission.HasPermission(1729, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await pMServiceManager.PmChangeRequestService.Remove(id);

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
        public async Task<IActionResult> Edit(long id)
        {
            try
            {
                // Check permissions
                var hasEditPermission = await permission.HasPermission(1729, PermissionType.Edit);
                var hasViewPermission = await permission.HasPermission(1729, PermissionType.Show);

                if (!hasEditPermission && !hasViewPermission)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetMessagesResource("NoIdInUpdate")));

                // Retrieve Change Request data
                var changeRequestResult = await pMServiceManager.PmChangeRequestService.GetOneVW(x => x.IsDeleted == false && x.Id == id);
                if (!changeRequestResult.Succeeded || changeRequestResult.Data == null)
                    return Ok(await Result<object>.FailAsync(changeRequestResult.Succeeded ? localization.GetMessagesResource("NoIdInUpdate") : changeRequestResult.Status.message));

                // Retrieve Workflow Data if available
                var wfData = new object();
                if (changeRequestResult.Data.AppId > 0)
                {
                    var wfApplicationResult = await wFServiceManager.WfApplicationService.GetOneVW(x => x.Isdecision == false && x.Id == changeRequestResult.Data.AppId && x.CreatedBy == session.UserId);
                    if (wfApplicationResult.Data != null)
                    {
                        wfData = new
                        {
                            AppTypeID = wfApplicationResult.Data.ApplicationsTypeId,
                            ApplicantsID = wfApplicationResult.Data.ApplicantsId,
                            StatusID = wfApplicationResult.Data.StatusId
                        };
                    }
                }

                // Retrieve Project Data
                var projectDataResult = await pMServiceManager.PMProjectsService.GetOneFromProjectsEditVw(e => e.IsDeleted == false && e.Id == changeRequestResult.Data.ProjectId && e.FacilityId == session.FacilityId);
                var projectData = projectDataResult.Data;

                // Prepare Project details with null checks
                var projectDetails = new
                {
                    ProjectName = projectData?.Name ?? string.Empty,
                    ProjectCode = projectData?.Code ?? 0,
                    ProjectId = projectData?.Id ?? 0,
                    ProjectValue = projectData?.ProjectValue ?? 0,
                    ProjectStart = projectData?.ProjectStart ?? string.Empty,
                    ProjectEnd = projectData?.ProjectEnd ?? string.Empty,
                    ProjectManagerName = projectData?.EmpName ?? string.Empty,
                    ProjectOwnerName = projectData?.OwnerName ?? string.Empty
                };

                // Retrieve Project Items
                var projectItemsResult = await pMServiceManager.PMProjectsItemService.GetAllVW(e => e.IsDeleted == false && e.ProjectId == changeRequestResult.Data.ProjectId);
                //var projectItems = projectItemsResult.Data.AsQueryable();
                var projectItems = projectItemsResult.Data.Select(item => new { item.Id, item.ItemName }).AsQueryable();
                // Retrieve Project Deliverables and Transactions
                var deliverablesResult = await pMServiceManager.PmProjectsDeliverableService.GetAllVW(d => d.IsDeleted == false && d.ProjectId == changeRequestResult.Data.ProjectId);
                if (!deliverablesResult.Succeeded)
                    return Ok(await Result<object>.FailAsync(deliverablesResult.Status.message));

                var deliverableIds = deliverablesResult.Data.Select(d => d.Id).ToList();
                var transactionDetailsResult = await pMServiceManager.PmDeliverableTransactionsDetailService.GetAll(
                    t => t.IsDeleted == false && t.DeliverableId.HasValue && deliverableIds.Contains(t.DeliverableId.Value)
                );

                var projectDeliverables = deliverablesResult.Data
                    .GroupJoin(transactionDetailsResult.Data,
                        deliverable => deliverable.Id,
                        transaction => transaction.DeliverableId,
                        (deliverable, transactions) => new
                        {
                            deliverable.Id,
                            deliverable.ProjectId,
                            deliverable.Name,
                            deliverable.Description,
                            deliverable.StatusId,
                            deliverable.Note,
                            DeliveredQty = transactions.Any() ? transactions.Sum(t => t.Qty ?? 0) : 0,
                            deliverable.Qty,
                            deliverable.Type,
                            deliverable.ProjectCode,
                            deliverable.CreatedOn
                        })
                    .ToList();

                // Retrieve Files
                var fileResult = await mainServiceManager.SysFileService.GetFilesForUser(id, 128);
                var fileDtos = fileResult.Data;

                // Prepare response
                var response = new
                {
                    ChangeRequestData = changeRequestResult.Data,
                    ProjectItems = projectItems,
                    Deliverables = projectDeliverables,
                    FileDtos = fileDtos,
                    projectDetails,
                    WFData = wfData,
                };

                return Ok(await Result<object>.SuccessAsync(response, string.Empty, 200));
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"Error in PMChangeRequestController Edit method, MESSAGE: {ex.Message}"));
            }
        }


        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(PmChangeRequestEditDto obj)
        {
            try
            {
                obj.ProjectCode ??= 0;
                obj.Priority ??= 0;
                obj.IsDiscussed ??= 0;
                obj.CrType ??= 0;
                obj.EffectType ??= 0;
                obj.CrCost ??= 0;
                obj.PrevCrCost ??= 0;
                obj.BeforProjectCost ??= 0;
                obj.AfterProjectCost ??= 0;
                obj.CrDuration ??= 0;
                obj.CrDurationType ??= 0;
                obj.CrDuration ??= 0;
                obj.CrDuration ??= 0;
                obj.CrCost ??= 0;
                obj.AfterProjectCost ??= 0;
                ///////////
                obj.StatusId = 1;
                var chk = await permission.HasPermission(1729, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (obj.Id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));

                // طالب التغيير
                if (string.IsNullOrEmpty(obj.RequesterEmpCode))
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("ChangeRequester")));
                //هل تم مناقشة التغيير 
                if (obj.IsDiscussed <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("IsDiscussed")));

                //  تاريخ طلب التغيير

                if (string.IsNullOrEmpty(obj.CrDate))
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("CRDate")));

                // تاريخ التغيير
                if (string.IsNullOrEmpty(obj.DueDate))
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("ChangeDate")));

                // تاريخ اتخاذ القرار التغيير
                if (string.IsNullOrEmpty(obj.DueDateDecision))
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("DueDateDecision")));
                //الاولوية 
                if (obj.Priority <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("Priority")));

                // وصف التغيير
                if (string.IsNullOrEmpty(obj.Description))
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("CRDescription")));

                // الأثر على تحقيق أهداف المشروع
                if (string.IsNullOrEmpty(obj.GoalsEffect))
                    return Ok(await Result<object>.FailAsync("الأثر على تحقيق أهداف المشروع"));

                // الأثر على تحقيق المشروع للهدف الاستراتيجي المرتبط به 
                if (string.IsNullOrEmpty(obj.ProjectStrategicGoalsEffect))
                    return Ok(await Result<object>.FailAsync("الأثر على تحقيق المشروع للهدف الاستراتيجي المرتبط به "));

                // الأثر المتوقع على الموارد البشرية
                if (string.IsNullOrEmpty(obj.Hreffect))
                    return Ok(await Result<object>.FailAsync("الأثر المتوقع على الموارد البشرية  "));

                // الأثر المتوقع على مجالات المشروع الأخرى 
                if (string.IsNullOrEmpty(obj.OtherEffect))
                    return Ok(await Result<object>.FailAsync("الأثر المتوقع على مجالات المشروع الأخرى  "));


                // الأثر المتوقع على سير المشروع  
                if (string.IsNullOrEmpty(obj.ProjectProgressEffect))
                    return Ok(await Result<object>.FailAsync("الأثر المتوقع على سير المشروع "));

                // (التقدم المحرز في المشروع (وفقا لتقرير حالة المشروع   
                if (string.IsNullOrEmpty(obj.ProgressBasedOnReport))
                    return Ok(await Result<object>.FailAsync("(التقدم المحرز في المشروع (وفقا لتقرير حالة المشروع "));



                // ماهي المشاريع الأخرى أو المستفيدين الاخرين الذين يجب اخطارهم بهذا التغيير  
                if (string.IsNullOrEmpty(obj.OtherProjectToChange))
                    return Ok(await Result<object>.FailAsync("ماهي المشاريع الأخرى أو المستفيدين الاخرين الذين يجب اخطارهم بهذا التغيير"));

                // ماهي الآثار المترتبة على عدم اجراءالتغيير 
                if (string.IsNullOrEmpty(obj.NotChangeEffect))
                    return Ok(await Result<object>.FailAsync("ماهي الآثار المترتبة على عدم اجراءالتغيير"));

                // التاثير المتوقع من التغيير 
                if (string.IsNullOrEmpty(obj.Effect))
                    return Ok(await Result<object>.FailAsync("التاثير المتوقع من التغيير"));

                //  اهمية التغيير 
                if (string.IsNullOrEmpty(obj.Importance))
                    return Ok(await Result<object>.FailAsync("  اهمية التغيير  "));

                //  الأثر على نطاق المشروع  
                if (string.IsNullOrEmpty(obj.Implications))
                    return Ok(await Result<object>.FailAsync("  الأثر على نطاق المشروع   "));


                // تغيير قيمة العقد  

                if (obj.ChangeProjectCost == true)
                {
                    // قيمة العقد بعد التغيير  
                    if (obj.AfterProjectCost <= 0)
                        return Ok(await Result<object>.FailAsync("  قيمة العقد بعد التغيير    "));


                    // قيمة التغيير السابقة  
                    obj.PrevCrCost ??= 0;
                    // قيمة العقد قبل التغيير  
                    obj.BeforProjectCost ??= 0;
                }

                // تغيير مدة العقد  

                if (obj.ChangeProjectDuration == true)
                {

                    //  ويجب ارجاعه للباك GeProjectsAndPreviousChangeCostByCodeOnEdit  ياتي عند تغير كود المشروع من الدالة
                    // تاريخ انتهاء العقد قبل التغيير
                    //
                    if (string.IsNullOrEmpty(obj.BeforProjectEndDate))
                        return Ok(await Result<object>.FailAsync("  تاريخ انتهاء العقد قبل التغيير   "));


                    // تاريخ انتهاء العقد بعد امر التغيير
                    if (string.IsNullOrEmpty(obj.AfterProjectEndDate))
                        return Ok(await Result<object>.FailAsync(" تاريخ انتهاء العقد بعد امر التغيير  "));


                }

                var result = await pMServiceManager.PmChangeRequestService.Update(obj);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"======= Exp in {this.GetType()}: {ex.Message}"));

            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ProjectCode"> رقم المشروع / العقد </param>
        /// <param name="isSubContract">  (النوع   (مشروع / عقد   </param>
        /// <returns></returns>
        [HttpGet("GeProjectsAndPreviousChangeCostByCodeOnEdit")]
        public async Task<IActionResult> GeProjectsAndPreviousChangeCostByCodeOnEdit(long ProjectCode, bool isSubContract = false)
        {
            try
            {
                var result = await pMServiceManager.PMProjectsService.GetPMProjectsByCode(ProjectCode, session.FacilityId, isSubContract);
                if (result.Succeeded && result.Data != null)
                {
                    decimal? totalCost = 0;
                    var GetApplication = await wFServiceManager.WfApplicationsStatusService
                        .GetAll(e => e.NewStatusId == 5);

                    var validApplicationIds = GetApplication.Data.Select(e => e.ApplicationsId);

                    var GetCost = await pMServiceManager.PmChangeRequestService
                        .GetAll(e => e.IsDeleted == false && e.ProjectId == result.Data.Id &&
                                     (e.AppId == 0 || validApplicationIds.Contains(e.AppId)));
                    totalCost = GetCost.Data.Sum(e => e.CrCost) ?? 0;
                    result.Data.PrevChangeRequestCost = totalCost;

                }
                return Ok(result);

            }
            catch (Exception ex)
            {
                return Ok(await Result<PMProjectsSimpleInfoDto>.FailAsync($"======= Exp in  : {ex.Message}"));
            }

        }

        #endregion




        #region Add Page


        [HttpPost("Add")]
        public async Task<IActionResult> Add(PmChangeRequestDto obj)
        {

            try
            {
                obj.ProjectCode ??= 0;
                obj.Priority ??= 0;
                obj.IsDiscussed ??= 0;
                obj.CrType ??= 0;
                obj.EffectType ??= 0;
                obj.CrCost ??= 0;
                obj.PrevCrCost ??= 0;
                obj.BeforProjectCost ??= 0;

                obj.AfterProjectCost ??= 0;
                obj.CrDuration ??= 0;
                obj.CrDurationType ??= 0;
                obj.CrDuration ??= 0;
                obj.CrDuration ??= 0;
                obj.CrCost ??= 0;
                obj.AppTypeId ??= 0;
                obj.AfterProjectCost ??= 0;
                ///////////
                obj.StatusId = 1;
                var chk = await permission.HasPermission(1729, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (obj.Id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));

                // طالب التغيير
                if (string.IsNullOrEmpty(obj.RequesterEmpCode))
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("ChangeRequester")));
                //هل تم مناقشة التغيير 
                if (obj.IsDiscussed <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("IsDiscussed")));

                //  تاريخ طلب التغيير

                if (string.IsNullOrEmpty(obj.CrDate))
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("CRDate")));

                // تاريخ التغيير
                if (string.IsNullOrEmpty(obj.DueDate))
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("ChangeDate")));

                // تاريخ اتخاذ القرار التغيير
                if (string.IsNullOrEmpty(obj.DueDateDecision))
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("DueDateDecision")));
                //الاولوية 
                if (obj.Priority <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("Priority")));

                // وصف التغيير
                if (string.IsNullOrEmpty(obj.Description))
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("CRDescription")));

                // الأثر على تحقيق أهداف المشروع
                if (string.IsNullOrEmpty(obj.GoalsEffect))
                    return Ok(await Result<object>.FailAsync("الأثر على تحقيق أهداف المشروع"));

                // الأثر على تحقيق المشروع للهدف الاستراتيجي المرتبط به 
                if (string.IsNullOrEmpty(obj.ProjectStrategicGoalsEffect))
                    return Ok(await Result<object>.FailAsync("الأثر على تحقيق المشروع للهدف الاستراتيجي المرتبط به "));

                // الأثر المتوقع على الموارد البشرية
                if (string.IsNullOrEmpty(obj.Hreffect))
                    return Ok(await Result<object>.FailAsync("الأثر المتوقع على الموارد البشرية  "));

                // الأثر المتوقع على مجالات المشروع الأخرى 
                if (string.IsNullOrEmpty(obj.OtherEffect))
                    return Ok(await Result<object>.FailAsync("الأثر المتوقع على مجالات المشروع الأخرى  "));


                // الأثر المتوقع على سير المشروع  
                if (string.IsNullOrEmpty(obj.ProjectProgressEffect))
                    return Ok(await Result<object>.FailAsync("الأثر المتوقع على سير المشروع "));

                // (التقدم المحرز في المشروع (وفقا لتقرير حالة المشروع   
                if (string.IsNullOrEmpty(obj.ProgressBasedOnReport))
                    return Ok(await Result<object>.FailAsync("(التقدم المحرز في المشروع (وفقا لتقرير حالة المشروع "));



                // ماهي المشاريع الأخرى أو المستفيدين الاخرين الذين يجب اخطارهم بهذا التغيير  
                if (string.IsNullOrEmpty(obj.OtherProjectToChange))
                    return Ok(await Result<object>.FailAsync("ماهي المشاريع الأخرى أو المستفيدين الاخرين الذين يجب اخطارهم بهذا التغيير"));

                // ماهي الآثار المترتبة على عدم اجراءالتغيير 
                if (string.IsNullOrEmpty(obj.NotChangeEffect))
                    return Ok(await Result<object>.FailAsync("ماهي الآثار المترتبة على عدم اجراءالتغيير"));

                // التاثير المتوقع من التغيير 
                if (string.IsNullOrEmpty(obj.Effect))
                    return Ok(await Result<object>.FailAsync("التاثير المتوقع من التغيير"));

                //  اهمية التغيير 
                if (string.IsNullOrEmpty(obj.Importance))
                    return Ok(await Result<object>.FailAsync("  اهمية التغيير  "));

                //  الأثر على نطاق المشروع  
                if (string.IsNullOrEmpty(obj.Implications))
                    return Ok(await Result<object>.FailAsync("  الأثر على نطاق المشروع   "));

                // تغيير قيمة العقد  

                if (obj.ChangeProjectCost == true)
                {

                    // قيمة العقد بعد التغيير  
                    if (obj.AfterProjectCost <= 0)
                        return Ok(await Result<object>.FailAsync("  قيمة العقد بعد التغيير    "));

                    // قيمة التغيير السابقة  
                    obj.PrevCrCost ??= 0;
                    // قيمة العقد قبل التغيير  

                    obj.BeforProjectCost ??= 0;

                }

                // تغيير مدة العقد  

                if (obj.ChangeProjectDuration == true)
                {
                    // تاريخ انتهاء العقد قبل التغيير  
                    if (string.IsNullOrEmpty(obj.BeforProjectEndDate))
                        return Ok(await Result<object>.FailAsync("  تاريخ انتهاء العقد قبل التغيير   "));


                    // تاريخ انتهاء العقد بعد امر التغيير
                    if (string.IsNullOrEmpty(obj.AfterProjectEndDate))
                        return Ok(await Result<object>.FailAsync(" تاريخ انتهاء العقد بعد امر التغيير  "));


                }


                var result = await pMServiceManager.PmChangeRequestService.Add(obj);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"======= Exp in {this.GetType()}: {ex.Message}"));

            }

        }


        [HttpGet("GeProjectsAndPreviousChangeCostByCodeOnAdd")]
        public async Task<IActionResult> GeProjectsAndPreviousChangeCostByCodeOnAdd(long ProjectCode, bool isSubContract = false)
        {
            try
            {
                // Check permissions
                var hasAddPermission = await permission.HasPermission(1729, PermissionType.Add);

                if (!hasAddPermission)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                // Retrieve Project Data

                var GetProject = await pMServiceManager.PMProjectsService.GetPMProjectsByCode(ProjectCode, session.FacilityId, isSubContract);
                //  في حالة المشروع غير موجود
                if (!GetProject.Succeeded || GetProject.Data == null)
                {

                    return Ok(GetProject);
                }
                // GetPM_Projects_byParent_ID
                var GetProjectsbyParent = await pMServiceManager.PMProjectsService.GetOneVW(x => x.IsDeleted == false && x.ParentId == GetProject.Data.Id && x.IsSubContract == true);
                if (GetProjectsbyParent.Succeeded && GetProjectsbyParent.Data != null)
                {
                    GetProject.Data.Name = GetProjectsbyParent.Data.Name;
                    GetProject.Data.Code = GetProjectsbyParent.Data.Code;
                    GetProject.Data.ProjectStart = GetProjectsbyParent.Data.ProjectStart;
                    GetProject.Data.ProjectEnd = GetProjectsbyParent.Data.ProjectEnd;
                    GetProject.Data.ProjectValue = GetProjectsbyParent.Data.ProjectValue;
                    GetProject.Data.BeforProjectCost = GetProjectsbyParent.Data.ProjectValue;
                    GetProject.Data.BeforeProjectEndDate = GetProjectsbyParent.Data.ProjectEnd;

                }


                //  Get Project Prev Cost
                decimal? totalCost = 0;
                var GetApplication = await wFServiceManager.WfApplicationsStatusService
                    .GetAll(e => e.NewStatusId == 5);

                var validApplicationIds = GetApplication.Data.Select(e => e.ApplicationsId);

                var GetCost = await pMServiceManager.PmChangeRequestService
                    .GetAll(e => e.IsDeleted == false && e.ProjectId == GetProject.Data.Id &&
                                 (e.AppId == 0 || validApplicationIds.Contains(e.AppId)));
                totalCost = GetCost.Data.Sum(e => e.CrCost) ?? 0;
                GetProject.Data.PrevChangeRequestCost = totalCost;


                // Retrieve Project Items
                var projectItemsResult = await pMServiceManager.PMProjectsItemService.GetAllVW(e => e.IsDeleted == false && e.ProjectId == GetProject.Data.Id);
                //var projectItems = projectItemsResult.Data.AsQueryable();
                var projectItems = projectItemsResult.Data.AsQueryable();
                // Retrieve Project Deliverables and Transactions
                var deliverablesResult = await pMServiceManager.PmProjectsDeliverableService.GetAllVW(d => d.IsDeleted == false && d.ProjectId == GetProject.Data.Id);
                if (!deliverablesResult.Succeeded)
                    return Ok(await Result<object>.FailAsync(deliverablesResult.Status.message));

                var deliverableIds = deliverablesResult.Data.Select(d => d.Id).ToList();
                var transactionDetailsResult = await pMServiceManager.PmDeliverableTransactionsDetailService.GetAll(
                    t => t.IsDeleted == false && t.DeliverableId.HasValue && deliverableIds.Contains(t.DeliverableId.Value)
                );

                var projectDeliverables = deliverablesResult.Data
                    .GroupJoin(transactionDetailsResult.Data,
                        deliverable => deliverable.Id,
                        transaction => transaction.DeliverableId,
                        (deliverable, transactions) => new
                        {
                            deliverable.Id,
                            deliverable.ProjectId,
                            deliverable.Name,
                            deliverable.Description,
                            deliverable.StatusId,
                            deliverable.Note,
                            DeliveredQty = transactions.Any() ? transactions.Sum(t => t.Qty ?? 0) : 0,
                            deliverable.Qty,
                            deliverable.Type,
                            deliverable.ProjectCode,
                            deliverable.CreatedOn
                        })
                    .ToList();

                // Retrieve Files
                var fileResult = new List<SaveFileDto>();

                // Prepare response
                var response = new
                {
                    ProjectData = GetProject.Data,
                    ProjectItems = projectItems,
                    Deliverables = projectDeliverables,
                    FileDtos = fileResult,
                };

                return Ok(await Result<object>.SuccessAsync(response, string.Empty, 200));
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"Error in PMChangeRequestController Edit method, MESSAGE: {ex.Message}"));
            }
        }


        #endregion

        #region Add2 Page


        [HttpPost("Add2")]
        public async Task<IActionResult> Add2(PmChangeRequestAdd2Dto obj)
        {

            try
            {
                obj.ProjectId ??= 0;
                obj.Priority ??= 0;
                obj.IsDiscussed ??= 0;
                obj.CrType ??= 0;
                obj.EffectType ??= 0;
                obj.CrCost ??= 0;
                obj.PrevCrCost ??= 0;
                obj.BeforProjectCost ??= 0;

                obj.AfterProjectCost ??= 0;
                obj.CrDuration ??= 0;
                obj.CrDurationType ??= 0;
                obj.CrDuration ??= 0;
                obj.CrDuration ??= 0;
                obj.CrCost ??= 0;
                obj.AppTypeId ??= 0;
                obj.AfterProjectCost ??= 0;
                ///////////
                obj.StatusId = 1;
                var chk = await permission.HasPermission(1729, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                // طالب التغيير
                if (string.IsNullOrEmpty(obj.RequesterEmpCode))
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("ChangeRequester")));
                //هل تم مناقشة التغيير 
                if (obj.IsDiscussed <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("IsDiscussed")));

                //  تاريخ طلب التغيير

                if (string.IsNullOrEmpty(obj.CrDate))
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("CRDate")));

                // تاريخ التغيير
                if (string.IsNullOrEmpty(obj.DueDate))
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("ChangeDate")));

                // تاريخ اتخاذ القرار التغيير
                if (string.IsNullOrEmpty(obj.DueDateDecision))
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("DueDateDecision")));
                //الاولوية 
                if (obj.Priority <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("Priority")));

                // وصف التغيير
                if (string.IsNullOrEmpty(obj.Description))
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("CRDescription")));

                // الأثر على تحقيق أهداف المشروع
                if (string.IsNullOrEmpty(obj.GoalsEffect))
                    return Ok(await Result<object>.FailAsync("الأثر على تحقيق أهداف المشروع"));

                // الأثر على تحقيق المشروع للهدف الاستراتيجي المرتبط به 
                if (string.IsNullOrEmpty(obj.ProjectStrategicGoalsEffect))
                    return Ok(await Result<object>.FailAsync("الأثر على تحقيق المشروع للهدف الاستراتيجي المرتبط به "));

                // الأثر المتوقع على الموارد البشرية
                if (string.IsNullOrEmpty(obj.Hreffect))
                    return Ok(await Result<object>.FailAsync("الأثر المتوقع على الموارد البشرية  "));

                // الأثر المتوقع على مجالات المشروع الأخرى 
                if (string.IsNullOrEmpty(obj.OtherEffect))
                    return Ok(await Result<object>.FailAsync("الأثر المتوقع على مجالات المشروع الأخرى  "));


                // الأثر المتوقع على سير المشروع  
                if (string.IsNullOrEmpty(obj.ProjectProgressEffect))
                    return Ok(await Result<object>.FailAsync("الأثر المتوقع على سير المشروع "));

                // (التقدم المحرز في المشروع (وفقا لتقرير حالة المشروع   
                if (string.IsNullOrEmpty(obj.ProgressBasedOnReport))
                    return Ok(await Result<object>.FailAsync("(التقدم المحرز في المشروع (وفقا لتقرير حالة المشروع "));



                // ماهي المشاريع الأخرى أو المستفيدين الاخرين الذين يجب اخطارهم بهذا التغيير  
                if (string.IsNullOrEmpty(obj.OtherProjectToChange))
                    return Ok(await Result<object>.FailAsync("ماهي المشاريع الأخرى أو المستفيدين الاخرين الذين يجب اخطارهم بهذا التغيير"));

                // ماهي الآثار المترتبة على عدم اجراءالتغيير 
                if (string.IsNullOrEmpty(obj.NotChangeEffect))
                    return Ok(await Result<object>.FailAsync("ماهي الآثار المترتبة على عدم اجراءالتغيير"));

                // التاثير المتوقع من التغيير 
                if (string.IsNullOrEmpty(obj.Effect))
                    return Ok(await Result<object>.FailAsync("التاثير المتوقع من التغيير"));

                //  اهمية التغيير 
                if (string.IsNullOrEmpty(obj.Importance))
                    return Ok(await Result<object>.FailAsync("  اهمية التغيير  "));

                //  الأثر على نطاق المشروع  
                if (string.IsNullOrEmpty(obj.Implications))
                    return Ok(await Result<object>.FailAsync("  الأثر على نطاق المشروع   "));

                // تغيير قيمة العقد  

                if (obj.ChangeProjectCost == true)
                {


                    // قيمة العقد بعد التغيير  
                    if (obj.AfterProjectCost <= 0)
                        return Ok(await Result<object>.FailAsync("  قيمة العقد بعد التغيير    "));

                    // قيمة التغيير السابقة  
                    obj.PrevCrCost ??= 0;
                    // قيمة العقد قبل التغيير  
                    obj.BeforProjectCost ??= 0;
                }

                // تغيير مدة العقد  

                if (obj.ChangeProjectDuration == true)
                {
                    // تاريخ انتهاء العقد قبل التغيير  
                    if (string.IsNullOrEmpty(obj.BeforProjectEndDate))
                        return Ok(await Result<object>.FailAsync("  تاريخ انتهاء العقد قبل التغيير   "));


                    // تاريخ انتهاء العقد بعد امر التغيير
                    if (string.IsNullOrEmpty(obj.AfterProjectEndDate))
                        return Ok(await Result<object>.FailAsync(" تاريخ انتهاء العقد بعد امر التغيير  "));


                }

                if (obj.ProjectId <= 0)
                    return Ok(await Result<object>.FailAsync("رقم المشروع "));

                // Check If Project Exist
                var IsProjectExist = await pMServiceManager.PMProjectsService.GetOne(x => x.Id == obj.ProjectId && x.IsDeleted == false && x.FacilityId == session.FacilityId);
                if (IsProjectExist.Data == null)
                    return Ok(await Result<object>.FailAsync($"{localization.GetResource1("TheProjectNumberIsNotFoundInTheProjectList")}"));


                if (string.IsNullOrEmpty(obj.BeforProjectEndDate))
                    return Ok(await Result<object>.FailAsync("  تاريخ انتهاء العقد قبل التغيير   "));

                var result = await pMServiceManager.PmChangeRequestService.Add(obj);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"======= Exp in {this.GetType()}: {ex.Message}"));

            }

        }
        // يتم استعاها قبل اضافة بند من اجل التأكد هل رقم البند موجود مسبقا
        [HttpGet("CheckIfItemCodeExist")]
        public async Task<IActionResult> CheckIfItemCodeExist(long ProjectId, string ItemCode)
        {

            try
            {

                var chk = await permission.HasPermission(1729, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (ProjectId <= 0)
                    return Ok(await Result<object>.FailAsync($"{localization.GetPMResource("ContractNo")}"));

                if (string.IsNullOrEmpty(ItemCode))
                    return Ok(await Result<object>.FailAsync($"{localization.GetPMResource("ProItemNo")}"));

                var result = await pMServiceManager.PMProjectsItemService.GetAll(x => x.IsDeleted == false && x.ProjectId == ProjectId && x.ItemCode == ItemCode);
                if (!result.Succeeded)
                    return Ok(await Result<object>.FailAsync($"{result.Status.message}"));
                if (result.Data.Count() == 0)
                    return Ok(await Result<object>.SuccessAsync("", 200));
                return Ok(await Result<object>.FailAsync("رقم البند موجود مسبقاُ  في قائمة بنود المشروع"));

            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"======= Exp in {this.GetType()}: {ex.Message}"));

            }

        }

        #endregion


        #region Edit Page2


        [HttpGet("GetById2")]
        public async Task<IActionResult> Edit2(long id)
        {
            try
            {
                // Check permissions
                var hasEditPermission = await permission.HasPermission(1729, PermissionType.Edit);
                var hasViewPermission = await permission.HasPermission(1729, PermissionType.Show);

                if (!hasEditPermission && !hasViewPermission)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetMessagesResource("NoIdInUpdate")));

                // Retrieve Change Request data
                var changeRequestResult = await pMServiceManager.PmChangeRequestService.GetOneVW(x => x.IsDeleted == false && x.Id == id);
                if (!changeRequestResult.Succeeded || changeRequestResult.Data == null)
                    return Ok(await Result<object>.FailAsync(changeRequestResult.Succeeded ? localization.GetMessagesResource("NoIdInUpdate") : changeRequestResult.Status.message));

                // Retrieve Workflow Data if available
                var wfData = new object();
                if (changeRequestResult.Data.AppId > 0)
                {
                    var wfApplicationResult = await wFServiceManager.WfApplicationService.GetOneVW(x => x.Isdecision == false && x.Id == changeRequestResult.Data.AppId && x.CreatedBy == session.UserId);
                    if (wfApplicationResult.Data != null)
                    {
                        wfData = new
                        {
                            AppTypeID = wfApplicationResult.Data.ApplicationsTypeId,
                            ApplicantsID = wfApplicationResult.Data.ApplicantsId,
                            StatusID = wfApplicationResult.Data.StatusId
                        };
                    }
                }

                // Retrieve Project Data
                var projectDataResult = await pMServiceManager.PMProjectsService.GetOneFromProjectsEditVw(e => e.IsDeleted == false && e.Id == changeRequestResult.Data.ProjectId && e.FacilityId == session.FacilityId);
                var projectData = projectDataResult.Data;

                // Prepare Project details with null checks
                var projectDetails = new
                {
                    ProjectName = projectData?.Name ?? string.Empty,
                    ProjectCode = projectData?.Code ?? 0,
                    ProjectId = projectData?.Id ?? 0,
                    IsSubContract = projectData.IsSubContract,
                    ProjectValue = projectData?.ProjectValue ?? 0,
                    ProjectStart = projectData?.ProjectStart ?? string.Empty,
                    ProjectEnd = projectData?.ProjectEnd ?? string.Empty,
                    ProjectManagerName = projectData?.EmpName ?? string.Empty,
                    ProjectOwnerName = projectData?.OwnerName ?? string.Empty
                };

                // Retrieve ChangeRequestItem
                var ChangeRequestItemResult = await pMServiceManager.PmChangeRequestItemService.GetAllVW(e => e.IsDeleted == false && e.ChangeRequestId == id);
                //var projectItems = projectItemsResult.Data.AsQueryable();
                var ChangeRequestItems = ChangeRequestItemResult.Data.AsQueryable();


                // Retrieve projectItems
                var projectItemsResult = await pMServiceManager.PMProjectsItemService.GetAllVW(e => e.IsDeleted == false && e.ProjectId == changeRequestResult.Data.Id);
                var projectItems = projectItemsResult.Data.Select(x=>new {x.Id,x.ItemName}).AsQueryable();

                // Retrieve Project Deliverables and Transactions
                var deliverablesResult = await pMServiceManager.PmProjectsDeliverableService.GetAllVW(d => d.IsDeleted == false && d.ProjectId == changeRequestResult.Data.ProjectId);
                if (!deliverablesResult.Succeeded)
                    return Ok(await Result<object>.FailAsync(deliverablesResult.Status.message));

                var deliverableIds = deliverablesResult.Data.Select(d => d.Id).ToList();
                var transactionDetailsResult = await pMServiceManager.PmDeliverableTransactionsDetailService.GetAll(
                    t => t.IsDeleted == false && t.DeliverableId.HasValue && deliverableIds.Contains(t.DeliverableId.Value)
                );

                var projectDeliverables = deliverablesResult.Data
                    .GroupJoin(transactionDetailsResult.Data,
                        deliverable => deliverable.Id,
                        transaction => transaction.DeliverableId,
                        (deliverable, transactions) => new
                        {
                            deliverable.Id,
                            deliverable.ProjectId,
                            deliverable.Name,
                            deliverable.Description,
                            deliverable.StatusId,
                            deliverable.Note,
                            DeliveredQty = transactions.Any() ? transactions.Sum(t => t.Qty ?? 0) : 0,
                            deliverable.Qty,
                            deliverable.Type,
                            deliverable.ProjectCode,
                            deliverable.CreatedOn
                        })
                    .ToList();

                // Retrieve Files
                var fileResult = await mainServiceManager.SysFileService.GetFilesForUser(id, 128);
                var fileDtos = fileResult.Data;

                // Prepare response
                var response = new
                {
                    ChangeRequestData = changeRequestResult.Data,
                    ChangeRequestItems = ChangeRequestItems,
                    projectItems = projectItems,
                    Deliverables = projectDeliverables,
                    FileDtos = fileDtos,
                    projectDetails,
                    WFData = wfData,
                };

                return Ok(await Result<object>.SuccessAsync(response, string.Empty, 200));
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"Error in PMChangeRequestController Edit2 method, MESSAGE: {ex.Message}"));
            }
        }


        [HttpPost("Edit2")]
        public async Task<IActionResult> Edit2(PmChangeRequestEdit2Dto obj)
        {

            try
            {

                obj.Priority ??= 0;
                obj.IsDiscussed ??= 0;
                obj.CrType ??= 0;
                obj.EffectType ??= 0;
                obj.CrCost ??= 0;
                obj.PrevCrCost ??= 0;
                obj.BeforProjectCost ??= 0;

                obj.AfterProjectCost ??= 0;
                obj.CrDuration ??= 0;
                obj.CrDurationType ??= 0;
                obj.CrDuration ??= 0;
                obj.CrDuration ??= 0;
                obj.CrCost ??= 0;
                obj.AppTypeId ??= 0;
                obj.AfterProjectCost ??= 0;
                ///////////
                obj.StatusId = 1;
                var chk = await permission.HasPermission(1729, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));


                if (obj.Id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));

                // طالب التغيير
                if (string.IsNullOrEmpty(obj.RequesterEmpCode))
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("ChangeRequester")));
                //هل تم مناقشة التغيير 
                if (obj.IsDiscussed <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("IsDiscussed")));

                //  تاريخ طلب التغيير

                if (string.IsNullOrEmpty(obj.CrDate))
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("CRDate")));

                // تاريخ التغيير
                if (string.IsNullOrEmpty(obj.DueDate))
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("ChangeDate")));

                // تاريخ اتخاذ القرار التغيير
                if (string.IsNullOrEmpty(obj.DueDateDecision))
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("DueDateDecision")));
                //الاولوية 
                if (obj.Priority <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("Priority")));

                // وصف التغيير
                if (string.IsNullOrEmpty(obj.Description))
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("CRDescription")));

                // الأثر على تحقيق أهداف المشروع
                if (string.IsNullOrEmpty(obj.GoalsEffect))
                    return Ok(await Result<object>.FailAsync("الأثر على تحقيق أهداف المشروع"));

                // الأثر على تحقيق المشروع للهدف الاستراتيجي المرتبط به 
                if (string.IsNullOrEmpty(obj.ProjectStrategicGoalsEffect))
                    return Ok(await Result<object>.FailAsync("الأثر على تحقيق المشروع للهدف الاستراتيجي المرتبط به "));

                // الأثر المتوقع على الموارد البشرية
                if (string.IsNullOrEmpty(obj.Hreffect))
                    return Ok(await Result<object>.FailAsync("الأثر المتوقع على الموارد البشرية  "));

                // الأثر المتوقع على مجالات المشروع الأخرى 
                if (string.IsNullOrEmpty(obj.OtherEffect))
                    return Ok(await Result<object>.FailAsync("الأثر المتوقع على مجالات المشروع الأخرى  "));


                // الأثر المتوقع على سير المشروع  
                if (string.IsNullOrEmpty(obj.ProjectProgressEffect))
                    return Ok(await Result<object>.FailAsync("الأثر المتوقع على سير المشروع "));

                // (التقدم المحرز في المشروع (وفقا لتقرير حالة المشروع   
                if (string.IsNullOrEmpty(obj.ProgressBasedOnReport))
                    return Ok(await Result<object>.FailAsync("(التقدم المحرز في المشروع (وفقا لتقرير حالة المشروع "));



                // ماهي المشاريع الأخرى أو المستفيدين الاخرين الذين يجب اخطارهم بهذا التغيير  
                if (string.IsNullOrEmpty(obj.OtherProjectToChange))
                    return Ok(await Result<object>.FailAsync("ماهي المشاريع الأخرى أو المستفيدين الاخرين الذين يجب اخطارهم بهذا التغيير"));

                // ماهي الآثار المترتبة على عدم اجراءالتغيير 
                if (string.IsNullOrEmpty(obj.NotChangeEffect))
                    return Ok(await Result<object>.FailAsync("ماهي الآثار المترتبة على عدم اجراءالتغيير"));

                // التاثير المتوقع من التغيير 
                if (string.IsNullOrEmpty(obj.Effect))
                    return Ok(await Result<object>.FailAsync("التاثير المتوقع من التغيير"));

                //  اهمية التغيير 
                if (string.IsNullOrEmpty(obj.Importance))
                    return Ok(await Result<object>.FailAsync("  اهمية التغيير  "));

                //  الأثر على نطاق المشروع  
                if (string.IsNullOrEmpty(obj.Implications))
                    return Ok(await Result<object>.FailAsync("  الأثر على نطاق المشروع   "));
                // تغيير قيمة العقد  

                if (obj.ChangeProjectCost == true)
                {

                    // قيمة العقد بعد التغيير  
                    if (obj.AfterProjectCost <= 0)
                        return Ok(await Result<object>.FailAsync("  قيمة العقد بعد التغيير    "));

                    // قيمة التغيير السابقة  
                    obj.PrevCrCost ??= 0;
                    // قيمة العقد قبل التغيير  
                    obj.BeforProjectCost ??= 0;
                }

                // تغيير مدة العقد  

                if (obj.ChangeProjectDuration == true)
                {

                    //  ويجب ارجاعه للباك GeProjectsAndPreviousChangeCostByCodeOnEdit  ياتي عند تغير كود المشروع من الدالة
                    // تاريخ انتهاء العقد قبل التغيير
                    //
                    if (string.IsNullOrEmpty(obj.BeforProjectEndDate))
                        return Ok(await Result<object>.FailAsync("  تاريخ انتهاء العقد قبل التغيير   "));


                    // تاريخ انتهاء العقد بعد امر التغيير
                    if (string.IsNullOrEmpty(obj.AfterProjectEndDate))
                        return Ok(await Result<object>.FailAsync(" تاريخ انتهاء العقد بعد امر التغيير  "));


                }


                var result = await pMServiceManager.PmChangeRequestService.Update(obj);
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
