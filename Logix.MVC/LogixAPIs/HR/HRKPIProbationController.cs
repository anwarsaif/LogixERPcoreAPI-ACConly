using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.HR.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Result = Logix.Application.Wrapper.Result;

namespace Logix.MVC.LogixAPIs.HR
{

    // تقييمات الفترة التجريبية
    public class HRKPIProbationController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IMainServiceManager mainServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ILocalizationService localization;
        private readonly ICurrentData session;

        public HRKPIProbationController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ILocalizationService localization, ICurrentData session, IMainServiceManager mainServiceManager)
        {
            this.permission = permission;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.session = session;
            this.mainServiceManager = mainServiceManager;
        }

        #region الشاشة الرئيسية


        [HttpPost("Search")]
        public async Task<IActionResult> Search(HRKpiFilterDto filter)
        {
            var chk = await permission.HasPermission(1683, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                List<HRKpiFilterDto> result = new List<HRKpiFilterDto>();

                filter.DeptId ??= 0;
                filter.Location ??= 0;
                filter.BranchId ??= 0;
                filter.PerformanceId ??= 0;

                var BranchesList = session.Branches.Split(',');

                var items = await hrServiceManager.HrKpiService.GetAllVW(x => x.IsDeleted == false
                    && (x.TypeId == 2)
                    && (!string.IsNullOrEmpty(x.EvaDate))
                    && (filter.PerformanceId == 0 || x.PerformanceId == filter.PerformanceId)
                    && (filter.DeptId == 0 || x.DeptId == filter.DeptId)
                    && (filter.Location == 0 || x.Location == filter.Location)
                    && (filter.EvaluationStatus == 0 || x.StatusId == filter.EvaluationStatus)
                    && (x.TypeId == 2)
                    && (x.FacilityId == session.FacilityId)
                    && (BranchesList.Contains(x.BranchId.ToString()))
                    && (string.IsNullOrEmpty(filter.EmpName) || x.EmpName.ToLower().Contains(filter.EmpName) || x.EmpName2.ToLower().Contains(filter.EmpName))
                    && (string.IsNullOrEmpty(filter.EmpCode) || x.EmpCode == filter.EmpCode)
                );
                if (items.Succeeded)
                {
                    if (items.Data.Count() > 0)
                    {
                        var KPIRes = items.Data.AsQueryable();

                        if (!string.IsNullOrEmpty(filter.FinancialYear))
                        {
                            KPIRes = KPIRes.Where(c => c.EvaDate.Substring(0, 4) == filter.FinancialYear);
                        }
                        if (filter.BranchId > 0)
                        {
                            KPIRes = KPIRes.Where(c => c.BranchId != null && c.BranchId == filter.BranchId);
                        }
                        if (KPIRes.Count() > 0)
                        {
                            string newStatusName = localization.GetHrResource("New");
                            string approveStatusName = localization.GetHrResource("Approve");
                            string rejectStatusName = localization.GetHrResource("Reject");

                            var KpiDetailsItems = await hrServiceManager.HrKpiDetaileService.GetAllVW(x => x.IsDeleted == false);
                            var KpiDetailsData = KpiDetailsItems.Data.AsQueryable();
                            foreach (var item in KPIRes)
                            {
                                var sumDegrees = KpiDetailsData.Where(d => d.KpiId == item.Id).Sum(d => d.Degree);
                                var sumScores = KpiDetailsData.Where(d => d.KpiId == item.Id).Sum(d => d.Score);
                                decimal? TotalDegree = 0;
                                if (sumScores != 0)
                                {
                                    TotalDegree = sumDegrees / sumScores * 100;
                                }
                                var newItem = new HRKpiFilterDto
                                {
                                    EmpCode = item.EmpCode,
                                    EmpName = session.Language == 1 ? item.EmpName : item.EmpName2,
                                    Id = item.Id,
                                    EvaDate = item.EvaDate,
                                    StatusName = item.StatusId == 1 ? newStatusName : (item.StatusId == 2 ? approveStatusName : rejectStatusName),
                                    BranchName = session.Language == 1 ? item.BraName : item.BraName2,
                                    TemplateName = session.Language == 1 ? item.TemName : item.TemName2,
                                    TotalDegree = TotalDegree,
                                };
                                result.Add(newItem);
                            }

                            return Ok(await Result<object>.SuccessAsync(result.ToList(), ""));

                        }
                        return Ok(await Result<List<HRKpiFilterDto>>.SuccessAsync(result, localization.GetResource1("NosearchResult")));

                    }

                    return Ok(await Result<List<HRKpiFilterDto>>.SuccessAsync(result, localization.GetResource1("NosearchResult")));

                }

                return Ok(await Result<object>.FailAsync(items.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync(ex.Message));
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long Id)
        {
            try
            {
                if (!await permission.HasPermission(1683, PermissionType.Edit))
                {
                    return Ok(await Result.AccessDenied("AccessDenied"));
                }

                // Validate Id parameter
                if (Id <= 0)
                {
                    return Ok(await Result.FailAsync(localization.GetMessagesResource("NoIdInUpdate")));
                }

                // Fetch KPI data
                var getKPIResult = await hrServiceManager.HrKpiService.GetOneVW(x => x.Id == Id && x.IsDeleted == false);

                if (!getKPIResult.Succeeded || getKPIResult.Data == null)
                {
                    return Ok(await Result.FailAsync(localization.GetMessagesResource("NoItemFoundToEdit")));
                }

                var kpi = getKPIResult.Data;

                // Determine button enable states based on KPI status
                bool btnRejectEnable = kpi.StatusId != 3;
                bool btnSaveEnable = kpi.StatusId != 2;
                bool btnApproveEnable = kpi.StatusId != 2;

                // Fetch KPI details
                var getKpisDetailsResult = await hrServiceManager.HrKpiDetaileService.GetAllVW(x => x.IsDeleted == false && x.KpiId == Id);

                var kpisDetails1 = new List<HrKpiDetailesVw>();
                var kpisDetails2 = new List<HrKpiDetailesVw>();

                if (getKpisDetailsResult != null && getKpisDetailsResult.Data != null)
                {
                    var kpisDetails = getKpisDetailsResult.Data.ToList();
                    kpisDetails1 = kpisDetails.Where(x => x.TypeId == 1).ToList();
                    kpisDetails2 = kpisDetails.Where(x => x.TypeId == 2).ToList();
                }

                // Return successful response
                return Ok(await Result<object>.SuccessAsync(new
                {
                    Kpi = kpi,
                    KpisDetails1 = kpisDetails1,
                    KpisDetails2 = kpisDetails2,
                    btnApproveEnable,
                    btnRejectEnable,
                    btnSaveEnable
                }));
            }
            catch (Exception ex)
            {
                // Log exception details if needed and return a generic failure response
                return Ok(await Result<HrDecisionsVw>.FailAsync($"Exception in HRKPIProbationController Edit method: {ex.Message}"));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var chk = await permission.HasPermission(1683, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await hrServiceManager.HrKpiService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete HRKPIProbationController, MESSAGE: {ex.Message}"));
            }
        }



        [HttpGet("KPIView")]
        public async Task<IActionResult> KPIView(long Id)
        {
            try
            {
                var chk = await permission.HasPermission(1683, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var KpiItem = await hrServiceManager.HrKpiService.GetOneVW(x => x.Id == Id && x.IsDeleted == false);
                if (KpiItem.Succeeded)
                {
                    if (KpiItem.Data == null) return Ok(await Result.FailAsync($"التقييم غير موجود"));

                    var KpiDetailsItems1 = await hrServiceManager.HrKpiDetaileService.GetAllVW(x => x.KpiId == Id && x.TypeId == 1 && x.IsDeleted == false);
                    var KpiDetailsItems2 = await hrServiceManager.HrKpiDetaileService.GetAllVW(x => x.KpiId == Id && x.TypeId == 2 && x.IsDeleted == false);
                    var PerformanceItems = await hrServiceManager.HrPerformanceService.GetAllVW(x => x.Id == KpiItem.Data.PerformanceId && x.IsDeleted == false);


                    return Ok(await Result<object>.SuccessAsync(new { kPIData = KpiItem.Data, KpiDetailsItems1 = KpiDetailsItems1.Data, KpiDetailsItems2 = KpiDetailsItems2.Data, PerformanceItems = PerformanceItems.Data }));
                }
                return Ok(await Result<HrLeaveVw>.FailAsync(KpiItem.Status.message));

            }
            catch (Exception ex)
            {
                return Ok(await Result<HrLeaveVw>.FailAsync($"====== Exp in HRKPIProbationController  KPIView, MESSAGE: {ex.Message}"));
            }
        }
        #endregion

        #region  شاشة التعديل
        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(HrKpiEditDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(1683, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));


                if (string.IsNullOrEmpty(obj.EmpCode)) return Ok(await Result<object>.FailAsync($"{localization.GetResource1("EmployeeIsNumber")} "));
                if (string.IsNullOrEmpty(obj.EvaDate)) return Ok(await Result<object>.FailAsync($"يجب ادخال تاريخ التقييم "));
                if (string.IsNullOrEmpty(obj.StartDate)) return Ok(await Result<object>.FailAsync($"يجب ادخال فترة التقييم من تاريخ "));
                if (string.IsNullOrEmpty(obj.EndDate)) return Ok(await Result<object>.FailAsync($"يجب ادخال  الى تاريخ"));
                if (obj.ProbationResult <= 0) return Ok(await Result<object>.FailAsync($"يجب ادخال  التوصيات المقترحة ")); obj.TypeId = 2;
                if (obj.KpiTemId <= 0) return Ok(await Result<object>.FailAsync($"يجب ادخال  نموذج التقييم  "));
                obj.TypeId = 2;

                var update = await hrServiceManager.HrKpiService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrKpiEditDto>.FailAsync($"====== Exp in Hr Hr HRKPIProbationController   getById, MESSAGE: {ex.Message}"));
            }
        }



        [HttpGet("Approve")]
        public async Task<IActionResult> Approve(long id)
        {
            try
            {
                var chk = await permission.HasPermission(1683, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));

                var Approve = await hrServiceManager.HrKpiService.UpdateKpiStatus(id, 2);
                if (Approve.Succeeded) return Ok(await Result<object>.SuccessAsync("تم إعتماد التقييم بنجاح"));
                return Ok(await Result<object>.SuccessAsync("لم يتم إعتماد التقييم"));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Approve HRKPIProbationController, MESSAGE: {ex.Message}"));
            }
        }


        [HttpGet("Reject")]
        public async Task<IActionResult> Reject(long id)
        {
            try
            {
                var chk = await permission.HasPermission(1683, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));

                var Approve = await hrServiceManager.HrKpiService.UpdateKpiStatus(id, 3);
                if (Approve.Succeeded) return Ok(await Result<object>.SuccessAsync("تم رفض التقييم بنجاح"));
                return Ok(await Result<object>.SuccessAsync("لم يتم رفض التقييم "));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Reject HRKPIProbationController, MESSAGE: {ex.Message}"));
            }
        }
        #endregion


        #region شاشة الاضافة

        [HttpPost("Add")]
        public async Task<ActionResult> Add(HrKpiDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(1683, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));


                if (string.IsNullOrEmpty(obj.EmpCode)) return Ok(await Result<object>.FailAsync($"{localization.GetResource1("EmployeeIsNumber")} "));
                if (string.IsNullOrEmpty(obj.EvaDate)) return Ok(await Result<object>.FailAsync($"يجب ادخال تاريخ التقييم "));
                if (string.IsNullOrEmpty(obj.StartDate)) return Ok(await Result<object>.FailAsync($"يجب ادخال فترة التقييم من تاريخ "));
                if (string.IsNullOrEmpty(obj.EndDate)) return Ok(await Result<object>.FailAsync($"يجب ادخال  الى تاريخ"));
                if (obj.ProbationResult <= 0) return Ok(await Result<object>.FailAsync($"يجب ادخال  التوصيات المقترحة "));
                if (obj.KpiTemId <= 0) return Ok(await Result<object>.FailAsync($"يجب ادخال  نموذج التقييم  "));

                obj.TypeId = 2;

                var add = await hrServiceManager.HrKpiService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add  HRKPIProbationController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("TemplateIDChanged")]
        public async Task<IActionResult> TemplateIDChanged(long Id)
        {
            try
            {
                var chk = await permission.HasPermission(1683, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id <= 0)
                {
                    return Ok(await Result.FailAsync($"يجب اختيار نموذج التقييم "));
                }

                var KpiItem = await hrServiceManager.HrKpiTemplateService.GetOne(x => x.Id == Id && x.IsDeleted == false);
                if (KpiItem.Data == null)
                {
                    return Ok(await Result.FailAsync($" نموذج التقييم غير موجود "));

                }
                var CompetenceItem1 = await hrServiceManager.HrKpiTemplatesCompetenceService.GetAllVW(x => x.IsDeleted == false && x.TemplateId == Id && x.TypeId == 1);
                var CompetenceItem2 = await hrServiceManager.HrKpiTemplatesCompetenceService.GetAllVW(x => x.IsDeleted == false && x.TemplateId == Id && x.TypeId == 2);
                return Ok(await Result<object>.SuccessAsync(new { KpiItem = KpiItem.Data, CompetenceItem1 = CompetenceItem1.Data.ToList(), CompetenceItem2 = CompetenceItem2.Data.ToList() }));

            }
            catch (Exception ex)
            {
                return Ok(await Result<HrDecisionsVw>.FailAsync($"====== Exp in Hr HRKPIProbationController  TemplateIDChanged, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("EmpCodeChanged")]
        public async Task<IActionResult> EmpCodeChanged(string EmpCode)
        {
            try
            {
                List<HRKpiFilterDto> PreviousEvaluations = new List<HRKpiFilterDto>();
                List<HrEmpGoalIndicatorsVw> EmpGoalIndicators = new List<HrEmpGoalIndicatorsVw>();
                List<HrKpiTemplatesCompetencesVw> CompetenceItems2 = new List<HrKpiTemplatesCompetencesVw>();
                HrKpiTemplateDto LastTemplate = new HrKpiTemplateDto();
                long? Template = 0;
                int? ReadKPIsID = 0;
                bool DDLTemplateEnable = true;
                var chk = await permission.HasPermission(1683, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (string.IsNullOrEmpty(EmpCode))
                {
                    return Ok(await Result.FailAsync(localization.GetResource1("EmployeeIsNumber")));
                }

                var checkEmpExist = await mainServiceManager.InvestEmployeeService.GetOne(i => i.EmpId == EmpCode && i.Isdel == false && i.IsDeleted == false);
                if (checkEmpExist.Succeeded)
                {
                    if (checkEmpExist.Data == null) return Ok(await Result.FailAsync(localization.GetResource1("EmployeeNotFound")));

                    //get TEmplates
                    var getAllusers = await mainServiceManager.SysUserService.GetAllVW(x => x.IsDeleted == false && x.EmpId == checkEmpExist.Data.Id);
                    var allusers = getAllusers.Data.AsQueryable();
                    var AllGroups = allusers.Select(x => Convert.ToInt32(x.GroupsId)).Distinct().ToList();

                    // Fetch all KPI items and filter them based on the presence of any group in allGroups
                    var GetKpiItems = await hrServiceManager.HrKpiTemplateService.GetAll(x => x.IsDeleted == false && x.GroupsId != null && x.GroupsId != "");
                    var KpiItems = GetKpiItems.Data.AsQueryable();

                    // Filter KPI items 
                    var filteredKpiItems = KpiItems.Where(x => x.GroupsId.Split(',', StringSplitOptions.RemoveEmptyEntries).Any(groupId => AllGroups.Contains(int.Parse(groupId)))).ToList();
                    if (filteredKpiItems.Count() > 0)
                    {
                        LastTemplate = filteredKpiItems.MaxBy(x => x.Id);
                        Template = LastTemplate.Id;
                        ReadKPIsID = LastTemplate.ReadKpisId;
                    }
                    //من  النموذج
                    if (ReadKPIsID == 1)
                    {
                        var GetCompetenceItems2 = await hrServiceManager.HrKpiTemplatesCompetenceService.GetAllVW(x => x.IsDeleted == false && x.TemplateId == Template && x.TypeId == 2);
                        if (GetCompetenceItems2.Data != null)
                        {
                            CompetenceItems2 = GetCompetenceItems2.Data.ToList();
                        }
                    }
                    //مؤشرات اداء الموظف
                    else if (ReadKPIsID == 2)
                    {
                        var GetEmployyeGoalIndicators = await hrServiceManager.HrEmpGoalIndicatorService.GetAllVW(x => x.IsDeleted == false && x.TemplateId == Template && x.EmpId == checkEmpExist.Data.Id);
                        if (GetEmployyeGoalIndicators.Data != null)
                        {
                            EmpGoalIndicators = GetEmployyeGoalIndicators.Data.ToList();
                        }
                    }
                    //من  كليهما
                    else if (ReadKPIsID == 3)
                    {
                        var GetKPITemplatesCompetences = await hrServiceManager.HrKpiTemplatesCompetenceService.GetAllVW(x => x.IsDeleted == false && x.TemplateId == Template && (x.TypeId == 2 || x.TypeId == 3));
                        if (GetKPITemplatesCompetences.Data != null)
                        {
                            CompetenceItems2 = GetKPITemplatesCompetences.Data.ToList();
                        }
                        var GetEmployyeGoalIndicators = await hrServiceManager.HrEmpGoalIndicatorService.GetAllVW(x => x.IsDeleted == false && x.TemplateId == Template && x.EmpId == checkEmpExist.Data.Id);
                        if (GetEmployyeGoalIndicators.Data != null)
                        {
                            EmpGoalIndicators = GetEmployyeGoalIndicators.Data.ToList();
                        }
                    }
                    var CompetenceItems1 = await hrServiceManager.HrKpiTemplatesCompetenceService.GetAllVW(x => x.IsDeleted == false && x.TemplateId == Template && x.TypeId == 1);
                    if (Template != 0)
                    {
                        DDLTemplateEnable = false;
                    }
                    //  التقيمات السابقة
                    var GetPreviousEvaluations = await hrServiceManager.HrKpiService.GetAllVW(x => x.IsDeleted == false && x.StatusId == 2 && x.EmpId == checkEmpExist.Data.Id);
                    if (GetPreviousEvaluations.Data.Count() > 0)
                    {
                        var KPIRes = GetPreviousEvaluations.Data.AsQueryable();
                        var KpiDetailsItems = await hrServiceManager.HrKpiDetaileService.GetAllVW(x => x.IsDeleted == false);
                        var KpiDetailsData = KpiDetailsItems.Data.AsQueryable();
                        foreach (var item in KPIRes)
                        {
                            var sumDegrees = KpiDetailsData.Where(d => d.KpiId == item.Id).Sum(d => d.Degree);
                            var sumScores = KpiDetailsData.Where(d => d.KpiId == item.Id).Sum(d => d.Score);
                            decimal? TotalDegree = sumScores != 0 ? (sumDegrees / sumScores) * 100 : 0;

                            var newItem = new HRKpiFilterDto
                            {
                                Id = item.Id,
                                TotalDegree = TotalDegree,
                                EvaDate = item.EvaDate,
                            };
                            PreviousEvaluations.Add(newItem);
                        }
                    }


                    return Ok(await Result<object>.SuccessAsync(new { TemplateItem = LastTemplate ?? new HrKpiTemplateDto(), CompetenceItem1 = CompetenceItems1.Data.ToList(), DDLTemplateEnable = DDLTemplateEnable, CompetenceItems2 = CompetenceItems2, EmpGoalIndicators = EmpGoalIndicators, PreviousEvaluations = PreviousEvaluations })); ;


                }
                return Ok(await Result<EmpIdChangedVM>.FailAsync($"{checkEmpExist.Status.message}"));

            }
            catch (Exception ex)
            {
                return Ok(await Result<HrDecisionsVw>.FailAsync($"====== Exp in Hr HRKPIProbationController  EmpCodeChanged, MESSAGE: {ex.Message}"));
            }
        }

        #endregion
    }
}
