using Logix.Application.Common;
using Logix.Application.DTOs.Main;
using Logix.Application.DTOs.PM;
using Logix.Application.DTOs.PM.PmProjects;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;

namespace Logix.MVC.LogixAPIs.PM
{
    //   خطط المشاريع في اعداد المشاريع
    public class PMPlansController : BasePMApiController
    {
        private readonly IPMServiceManager pMServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IWFServiceManager wFServiceManager;
        private readonly ITsServiceManager tsServiceManager;
        private readonly IMainServiceManager mainServiceManager;
        private readonly IHrServiceManager hrServiceManager;


        public PMPlansController(IPMServiceManager pMServiceManager, 
            IPermissionHelper permission,
            ICurrentData session,
            ILocalizationService localization,
            IWFServiceManager wFServiceManager,
            ITsServiceManager tsServiceManager,
            IMainServiceManager mainServiceManager,
            IHrServiceManager hrServiceManager)
        {
            this.pMServiceManager = pMServiceManager;
            this.permission = permission;
            this.session = session;
            this.localization = localization;
            this.wFServiceManager = wFServiceManager;
            this.tsServiceManager = tsServiceManager;
            this.mainServiceManager = mainServiceManager;
            this.hrServiceManager = hrServiceManager;
        }

        #region Index Page


        [HttpPost("Search")]
        public async Task<IActionResult> Search(PmProjectPlanFilterDto filter)
        {
            try
            {
                List<PmProjectPlanFilterDto> result = new();

                var chk = await permission.HasPermission(1420, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                filter.Id ??= 0; filter.PlanType ??= 0; filter.StatusId ??= 0; filter.ProjectCode ??= 0;
                var items = await pMServiceManager.PMProjectPlanService.GetAllVW(p => p.IsDeleted == false
                    && (filter.Id == 0 || p.Id == filter.Id)
                    && (filter.StatusId == 0 || p.StatusId == filter.StatusId)
                    && (filter.PlanType == 0 || p.PlanType == filter.PlanType)
                    && (filter.ProjectCode == 0 || p.ProjectCode == filter.ProjectCode)
                );
                if (!items.Succeeded)
                    return Ok(await Result<object>.FailAsync(items.Status.message));
                if (!items.Data.Any())
                    return Ok(await Result<object>.SuccessAsync(new List<object>(), localization.GetResource1("NosearchResult")));

                var res = items.Data.AsQueryable();


                if (!(string.IsNullOrEmpty(filter.StartDate) && string.IsNullOrEmpty(filter.EndDate)))
                {
                    DateTime startDate = DateHelper.StringToDate(filter.StartDate);
                    DateTime endDate = DateHelper.StringToDate(filter.EndDate);

                    res = res.Where(r => DateHelper.StringToDate(r.PlanDate) >= startDate && DateHelper.StringToDate(r.PlanDate) <= endDate);
                }
                if (!res.Any())
                    return Ok(await Result<object>.SuccessAsync(new List<object>(), localization.GetResource1("NosearchResult")));

                foreach (var item in res)
                {
                    PmProjectPlanFilterDto obj = new()
                    {
                        Id = item.Id,
                        PlanDate = item.PlanDate,
                        Subject = item.Subject,
                        ProjectName = item.ProjectName,
                        StartDate = item.StartDate,
                        EndDate = item.EndDate,
                        PlanTypeName = item.PlanTypeName,
                        StatusName = item.StatusName,
                        Note = item.Note,
                        TypeId = item.TypeId
                    };
                    result.Add(obj);
                }
                return Ok(await Result<List<PmProjectPlanFilterDto>>.SuccessAsync(result));

            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"======= Exp: {ex.Message}"));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var chk = await permission.HasPermission(1420, PermissionType.Delete);
            if (!chk)
                return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await pMServiceManager.PMProjectPlanService.Remove(id);

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

                var chk = await permission.HasPermission(1420, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id <= 0)
                    return Ok(await Result<object>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));

                var currentDate = DateTime.Now.ToString("yyyy/MM/dd");

                // بيانات الخطة
                var item = await pMServiceManager.PMProjectPlanService.GetOneVW(x => x.IsDeleted == false && x.Id == Id);
                if (!item.Succeeded)
                    return Ok(await Result<object>.FailAsync(item.Status.message));

                if (item.Data == null)
                    return Ok(await Result<object>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));

                //  مهام الخطة
                var tasks = await tsServiceManager.TsTaskService.GetAllVW(x => x.Isdel == false && x.ProjectPlansId == Id);

                // Step 3: Calculate Date_Rate for each task in-memory
                var Taskresult = tasks.Data.Select(task =>
                {
                    // Parse dates as needed (assuming task.SendDate and task.DueDate are DateTime or string in "yyyy/MM/dd" format)
                    var sendDate = DateHelper.StringToDate(task.SendDate);
                    var dueDate = DateHelper.StringToDate(task.DueDate);
                    var parsedCurrentDate = DateTime.Parse(currentDate);

                    double dateRate;

                    if (parsedCurrentDate >= sendDate)
                    {
                        if (parsedCurrentDate > dueDate)
                        {
                            dateRate = 100;
                        }
                        else
                        {
                            var daysPassed = (parsedCurrentDate - sendDate).TotalDays;
                            var totalDuration = (dueDate - sendDate).TotalDays > 0
                                                ? (dueDate - sendDate).TotalDays
                                                : 1.0;

                            dateRate = (daysPassed * 100.0) / totalDuration;
                        }
                    }
                    else if (sendDate >= parsedCurrentDate)
                    {
                        dateRate = 0;
                    }
                    else
                    {
                        dateRate = 100;
                    }

                    int elapsedDays = (int)(parsedCurrentDate - sendDate).TotalDays;
                    int totalDays = (int)(dueDate - sendDate).TotalDays;
                    double completionPercentage = (elapsedDays / (double)totalDays) * 100;

                    // Round the percentage to two decimal places
                    double roundedPercentage = Math.Round(completionPercentage, 2);
                    // If no elapsed time or total days, set the percentage to 0
                    if (totalDays == 0)
                    {
                        roundedPercentage = 0;
                    }

                    // Handle the case where the percentage is greater than or equal to 100
                    if (roundedPercentage >= 100)
                    {
                        roundedPercentage = 100;
                    }

                    // Optionally, convert to string if needed
                    string roundedPercentageStr = roundedPercentage.ToString();

                    return new
                    {
                        task.Id,
                        task.SendDate,
                        task.DueDate,
                        task.ActualStartDate,
                        task.ActualEndDate,
                        task.Durations,
                        task.DurationType,
                        task.Subject,
                        task.StatusId,
                        task.PercentageOfProject,
                        task.AssigneeToUserId,
                        task.ProjectPlansId,
                        DateRate = dateRate,
                        ChartPercentage= roundedPercentageStr,
                        task.Isdel,

                    };
                }).ToList();



                // الملفات
                var fileDtos = new List<SaveFileDto>();
                var getFiles = await mainServiceManager.SysFileService.GetFilesForUser(Id, 91);
                var response = new
                {
                    PlanData = item.Data,
                    Tasks = Taskresult,
                    fileDtos = getFiles.Data,
                };

                return Ok(await Result<object>.SuccessAsync(response, "", 200));

            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"====== Exp in PMPlanController getById, MESSAGE: {ex.Message}"));
            }
        }


        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(PmProjectPlanAddDto obj)
        {
            try
            {
                obj.ProjectCode ??= 0;
                obj.PlanType ??= 0;
                obj.Duration ??= 0;
                obj.DurationType ??= 0;
                obj.Wbs ??= "";
                var chk = await permission.HasPermission(1420, PermissionType.Add);
                if (chk == false)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (obj.Id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));

                if (obj.PMProjectPlanTaskList.Count < 1)
                    return Ok(await Result.FailAsync("يتم إضافة المهام اولا"));

                if (obj.ProjectCode <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("Project")));


                if (string.IsNullOrEmpty(obj.PlanDate))
                    return Ok(await Result<object>.FailAsync(localization.GetCommonResource("Tdate")));

                if (obj.PlanType <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("PlanType")));


                if (obj.StatusId <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("Status")));

                if (string.IsNullOrEmpty(obj.Subject))
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("PlanTitle")));


                if (string.IsNullOrEmpty(obj.StartDate))
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("StartDate")));

                if (obj.Duration <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("PlanDuration")));

                if (obj.DurationType <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("Number")));

                if (string.IsNullOrEmpty(obj.EndDate))
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("EEndDate")));

                foreach (var task in obj.PMProjectPlanTaskList)
                {

                    //  المهمة
                    if (string.IsNullOrEmpty(task.Subject))
                        return Ok(await Result<object>.FailAsync(localization.GetPMResource("Task")));

                    //  تاريخ بداية ونهاية المهمة 

                    if (string.IsNullOrEmpty(task.StartDate))
                        return Ok(await Result<object>.FailAsync(localization.GetPMResource("SStartDate")));

                    if (string.IsNullOrEmpty(task.EndDate))
                        return Ok(await Result<object>.FailAsync(localization.GetPMResource("EndDate")));
                    var PlanStartDate = DateHelper.StringToDate(obj.StartDate);
                    var PlanEndDate = DateHelper.StringToDate(obj.StartDate);
                    if (string.IsNullOrEmpty(task.Durations))
                        return Ok(await Result<object>.FailAsync(localization.GetPMResource("PlanDuration")));

                    if (task.DurationType <= 0)
                        return Ok(await Result<object>.FailAsync(localization.GetPMResource("Number")));

                    var StartDate = DateHelper.StringToDate(task.StartDate);
                    var EndDate = DateHelper.StringToDate(task.EndDate);
                    if (StartDate > EndDate)
                        return Ok(await Result<object>.FailAsync($" {task.Subject} تاريخ بداية المهمة يجب ان يكون قبل تاريخ نهاية المهمة  للمهمة "));

                    // فحص اذا كان تاريخ بداية  ونهاية المهمة متوافق ضمن  تاريخ بداية  ونهاية الخطة

                    if (DateHelper.StringToDate(task.StartDate) < PlanStartDate)
                        return Ok(await Result<object>.FailAsync($" {task.Subject}  : تاريخ بداية المهمة يجب ان يكون بعد تاريخ بداية الخطة للمهمة "));

                    if (DateHelper.StringToDate(task.EndDate) > PlanEndDate)
                        return Ok(await Result<object>.FailAsync($" {task.Subject} : تاريخ نهاية المهمة يجب ان يكون قبل تاريخ نهاية الخطة للمهمة  "));

                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    if (!(string.IsNullOrEmpty(task.ActualStartDate) && string.IsNullOrEmpty(task.EndDate)))
                    {
                        var ActualStartDate = DateHelper.StringToDate(task.ActualStartDate);
                        var ActualEndDate = DateHelper.StringToDate(task.ActualEndDate);
                        if (ActualStartDate > ActualEndDate)
                            return Ok(await Result<object>.FailAsync($"  {task.Subject}    تاريخ بداية المهمة الفعلي يجب ان يكون قبل تاريخ نهاية المهمة الفعلي للمهمة"));

                        // فحص اذا كان تاريخ بداية  ونهاية المهمة  الفعلي متوافق ضمن  تاريخ بداية  ونهاية الخطة

                        if (ActualStartDate < PlanStartDate)
                            return Ok(await Result<object>.FailAsync($" {task.Subject}  : تاريخ بداية المهمة الفعلي يجب ان يكون بعد تاريخ بداية الخطة للمهمة "));

                        if (ActualEndDate > PlanEndDate)
                            return Ok(await Result<object>.FailAsync($" {task.Subject} : تاريخ نهاية المهمةالفعلي يجب ان يكون قبل تاريخ نهاية الخطة للمهمة  "));

                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    }

                    // نسبة المهمة من المشروع %
                    if (task.PercentageOfProject <= 0)
                        return Ok(await Result<object>.FailAsync(localization.GetPMResource("TaskPercentageFromProjectPer")));


                    if (string.IsNullOrEmpty(task.AssigneeToUserId))
                        return Ok(await Result<object>.FailAsync(localization.GetPMResource("AssignedUser")));



                }
                var result = await pMServiceManager.PMProjectPlanService.Update(obj);
                return Ok(result);

            }
            catch (Exception ex)
            {
                return Ok(await Result<PMProjectsDto>.FailAsync($"======= Exp in  : {ex.Message}"));
            }
        }


        #endregion




        #region Add Page

        [HttpPost("Add")]
        public async Task<IActionResult> Add(PmProjectPlanAddDto obj)
        {
            try
            {
                obj.ProjectCode ??= 0;
                obj.PlanType ??= 0;
                obj.Duration ??= 0;
                obj.DurationType ??= 0;
                obj.Wbs ??= "";
                var chk = await permission.HasPermission(1420, PermissionType.Add);
                if (chk == false)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (obj.PMProjectPlanTaskList.Count < 1)
                    return Ok(await Result.FailAsync("يتم إضافة المهام اولا"));

                if (obj.ProjectCode <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("Project")));


                if (string.IsNullOrEmpty(obj.PlanDate))
                    return Ok(await Result<object>.FailAsync(localization.GetCommonResource("Tdate")));

                if (obj.PlanType <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("PlanType")));


                if (obj.StatusId <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("Status")));

                if (string.IsNullOrEmpty(obj.Subject))
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("PlanTitle")));


                if (string.IsNullOrEmpty(obj.StartDate))
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("StartDate")));

                if (obj.Duration <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("PlanDuration")));

                if (obj.DurationType <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("Number")));

                if (string.IsNullOrEmpty(obj.EndDate))
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("EEndDate")));

                foreach (var task in obj.PMProjectPlanTaskList)
                {

                    //  المهمة
                    if (string.IsNullOrEmpty(task.Subject))
                        return Ok(await Result<object>.FailAsync(localization.GetPMResource("Task")));

                    //  تاريخ بداية ونهاية المهمة 

                    if (string.IsNullOrEmpty(task.StartDate))
                        return Ok(await Result<object>.FailAsync(localization.GetPMResource("SStartDate")));

                    if (string.IsNullOrEmpty(task.EndDate))
                        return Ok(await Result<object>.FailAsync(localization.GetPMResource("EndDate")));
                    var PlanStartDate = DateHelper.StringToDate(obj.StartDate);
                    var PlanEndDate = DateHelper.StringToDate(obj.StartDate);
                    if (string.IsNullOrEmpty(task.Durations))
                        return Ok(await Result<object>.FailAsync(localization.GetPMResource("PlanDuration")));

                    if (task.DurationType <= 0)
                        return Ok(await Result<object>.FailAsync(localization.GetPMResource("Number")));

                    var StartDate = DateHelper.StringToDate(task.StartDate);
                    var EndDate = DateHelper.StringToDate(task.EndDate);
                    if (StartDate > EndDate)
                        return Ok(await Result<object>.FailAsync($" {task.Subject} تاريخ بداية المهمة يجب ان يكون قبل تاريخ نهاية المهمة  للمهمة "));

                    // فحص اذا كان تاريخ بداية  ونهاية المهمة متوافق ضمن  تاريخ بداية  ونهاية الخطة

                    if (DateHelper.StringToDate(task.StartDate) < PlanStartDate)
                        return Ok(await Result<object>.FailAsync($" {task.Subject}  : تاريخ بداية المهمة يجب ان يكون بعد تاريخ بداية الخطة للمهمة "));

                    if (DateHelper.StringToDate(task.EndDate) > PlanEndDate)
                        return Ok(await Result<object>.FailAsync($" {task.Subject} : تاريخ نهاية المهمة يجب ان يكون قبل تاريخ نهاية الخطة للمهمة  "));

                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    if (!(string.IsNullOrEmpty(task.ActualStartDate) && string.IsNullOrEmpty(task.EndDate)))
                    {
                        var ActualStartDate = DateHelper.StringToDate(task.ActualStartDate);
                        var ActualEndDate = DateHelper.StringToDate(task.ActualEndDate);
                        if (ActualStartDate > ActualEndDate)
                            return Ok(await Result<object>.FailAsync($"  {task.Subject}    تاريخ بداية المهمة الفعلي يجب ان يكون قبل تاريخ نهاية المهمة الفعلي للمهمة"));

                        // فحص اذا كان تاريخ بداية  ونهاية المهمة  الفعلي متوافق ضمن  تاريخ بداية  ونهاية الخطة

                        if (ActualStartDate < PlanStartDate)
                            return Ok(await Result<object>.FailAsync($" {task.Subject}  : تاريخ بداية المهمة الفعلي يجب ان يكون بعد تاريخ بداية الخطة للمهمة "));

                        if (ActualEndDate > PlanEndDate)
                            return Ok(await Result<object>.FailAsync($" {task.Subject} : تاريخ نهاية المهمةالفعلي يجب ان يكون قبل تاريخ نهاية الخطة للمهمة  "));

                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    }

                    // نسبة المهمة من المشروع %
                    if (task.PercentageOfProject <= 0)
                        return Ok(await Result<object>.FailAsync(localization.GetPMResource("TaskPercentageFromProjectPer")));


                    if (string.IsNullOrEmpty(task.AssigneeToUserId))
                        return Ok(await Result<object>.FailAsync(localization.GetPMResource("AssignedUser")));



                }
                var result = await pMServiceManager.PMProjectPlanService.Add(obj);
                return Ok(result);

            }
            catch (Exception ex)
            {
                return Ok(await Result<PMProjectsDto>.FailAsync($"======= Exp in  : {ex.Message}"));
            }
        }

        [HttpGet("GetPMProjectsByCode")]
        public async Task<IActionResult> GetPMProjectsByCode(long ProjectCode)
        {
            try
            {
                return Ok(await pMServiceManager.PMProjectsService.GetPMProjectsByCode(ProjectCode,session.FacilityId));

            }
            catch (Exception ex)
            {
                return Ok(await Result<PMProjectsSimpleInfoDto>.FailAsync($"======= Exp in  : {ex.Message}"));
            }

        }

        #endregion

    }
}
