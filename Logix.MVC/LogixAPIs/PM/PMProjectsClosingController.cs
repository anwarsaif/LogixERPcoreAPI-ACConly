using DocumentFormat.OpenXml.Office2010.Excel;
using Logix.Application.Common;
using Logix.Application.DTOs.PM;
using Logix.Application.DTOs.PM.Shared;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.PM;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;

namespace Logix.MVC.LogixAPIs.PM
{
    //  اغلاق المشاريع
    public class PMProjectsClosingController : BasePMApiController
    {
        private readonly IPMServiceManager pMServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IWFServiceManager wFServiceManager;
        private readonly IMainServiceManager mainServiceManager;



        public PMProjectsClosingController(IPMServiceManager pMServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IWFServiceManager wFServiceManager, IMainServiceManager mainServiceManager)
        {
            this.pMServiceManager = pMServiceManager;
            this.permission = permission;
            this.session = session;
            this.localization = localization;
            this.wFServiceManager = wFServiceManager;
            this.mainServiceManager = mainServiceManager;
        }



        [HttpPost("Search")]
        public async Task<IActionResult> Search(PMSharedFilterDto filter)
        {
            var chk = await permission.HasPermission(1752, PermissionType.Show);

            if (!chk)
                return Ok(await Result.AccessDenied("AccessDenied"));

            try
            {
                filter.ProjectCode ??= 0;


                var items = await pMServiceManager.PmProjectsClosingService.GetAllVW(e => e.IsDeleted == false
                && (string.IsNullOrEmpty(filter.ProjectName) || ((e.ProjectName != null && e.ProjectName.Contains(filter.ProjectName))))
                && (filter.ProjectCode == 0 || e.ProjectCode == filter.ProjectCode)
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
                    res = res.Where(x => x.ClosingDate != null && x.ClosingDate != "" && DateHelper.StringToDate(x.ClosingDate) >= DateFrom && DateHelper.StringToDate(x.ClosingDate) <= DateTo);
                }

                if (!res.Any())
                    return Ok(await Result<object>.SuccessAsync(new List<object>(), localization.GetResource1("NosearchResult")));


                res = res.OrderByDescending(x => x.Id);
                var response = res.Select(x => new
                {
                    x.Id,
                    x.ClosingDate,
                    x.ProjectCode,
                    x.ProjectName
                }).ToList();
                return Ok(await Result<object>.SuccessAsync(response, ""));
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
                var chk = await permission.HasPermission(1752, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await pMServiceManager.PmProjectsClosingService.Remove(id);

                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"======= Exp in {this.GetType()}: {ex.Message}"));
            }

        }



        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long id)
        {
            try
            {
                // Check permissions
                var check = await permission.HasPermission(1752, PermissionType.Edit);

                if (!check)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetMessagesResource("NoIdInUpdate")));

                var item = await pMServiceManager.PmProjectsClosingService.GetOneVW(x => x.IsDeleted == false && x.Id == id);
                if (!item.Succeeded || item.Data == null)
                    return Ok(await Result<object>.FailAsync(item.Succeeded ? localization.GetMessagesResource("NoIdInUpdate") : item.Status.message));

                // Retrieve Workflow Data if available
                var wfData = new object();
                if (item.Data.AppId > 0)
                {
                    var wfApplicationResult = await wFServiceManager.WfApplicationService.GetOneVW(x => x.Isdecision == false && x.Id == item.Data.AppId && x.CreatedBy == session.UserId);
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
                var projectDataResult = await pMServiceManager.PMProjectsService.GetOneFromProjectsEditVw(e => e.IsDeleted == false && e.Id == item.Data.ProjectId && e.FacilityId == session.FacilityId);
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

                // سجل المخاطر
                var GetRisks = await pMServiceManager.PMProjectsRiskService.GetAllVW(x => x.IsDeleted == false && x.ProjectId == item.Data.ProjectId);

                // الدروس المستفادة
                var GetLessons = await pMServiceManager.PmProjectsLessonsLearnedDetailService.GetAllVW(x => x.IsDeleted == false && x.ProjectId == item.Data.ProjectId);

                //سجل طلبات التغيير
                var GetChangeRequests = await pMServiceManager.PmChangeRequestService.GetAllVW(x => x.IsDeleted == false && x.ProjectId == item.Data.ProjectId);


                // Retrieve Files
                var fileResult = await mainServiceManager.SysFileService.GetFilesForUser(id, 1752);
                var fileDtos = fileResult.Data;

                // Prepare response
                var response = new
                {
                    Data = item.Data,
                    projectDetails,
                    Risks = GetRisks.Data.AsQueryable(),
                    Lessons = GetLessons.Data.AsQueryable(),
                    ChangeRequests = GetChangeRequests.Data.AsQueryable(),
                    FileDtos = fileDtos,
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
        public async Task<IActionResult> Edit(PmProjectsClosingEditDto obj)
        {
            try
            {


                var chk = await permission.HasPermission(1752, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (obj.Id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));


                obj.ProjectCode ??= 0;
                obj.BranchId ??= 0;

                // التاريخ
                if (string.IsNullOrEmpty(obj.ClosingDate))
                    return Ok(await Result<object>.FailAsync(localization.GetCommonResource("Tdate")));

                // الموظف 
                if (string.IsNullOrEmpty(obj.EmpCode))
                    return Ok(await Result<object>.FailAsync(localization.GetHrResource("EmpNo")));


                // المشروع 
                if (obj.ProjectCode <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("Project")));


                // الفرع 
                if (obj.BranchId <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetCommonResource("branch")));

                var result = await pMServiceManager.PmProjectsClosingService.Update(obj);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"======= Exp in {this.GetType()}: {ex.Message}"));

            }

        }







        [HttpPost("Add")]
        public async Task<IActionResult> Add(PmProjectsClosingDto obj)
        {

            try
            {


                var chk = await permission.HasPermission(1752, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                obj.ProjectCode ??= 0;
                obj.BranchId ??= 0;
                obj.AppTypeId ??= 0;


                // التاريخ
                if (string.IsNullOrEmpty(obj.ClosingDate))
                    return Ok(await Result<object>.FailAsync(localization.GetCommonResource("Tdate")));

                // الموظف 
                if (string.IsNullOrEmpty(obj.EmpCode))
                    return Ok(await Result<object>.FailAsync(localization.GetHrResource("EmpNo")));


                // المشروع 
                if (obj.ProjectCode <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("Project")));


                // الفرع 
                if (obj.BranchId <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetCommonResource("branch")));

                var result = await pMServiceManager.PmProjectsClosingService.Add(obj);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"======= Exp in {this.GetType()}: {ex.Message}"));

            }

        }


        [HttpGet("ProjectCodeChanged")]
        public async Task<IActionResult> ProjectCodeChanged(long Code)
        {
            if (Code == 0)
            {
                return Ok(await Result<object>.SuccessAsync("there is no id passed"));
            }
            try
            {
                var getProject = await pMServiceManager.PMProjectsService.GetOneVW(f => f.Code == Code && f.IsDeleted == false);
                if (!getProject.Succeeded)
                    return Ok(await Result<object>.FailAsync($"{getProject.Status.message}"));

                if (getProject.Data == null) return Ok(await Result<object>.SuccessAsync($"{localization.GetPMResource("projectisnotinprojectlist")}"));

                var projectData = getProject.Data;

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

                // سجل المخاطر
                var GetRisks = await pMServiceManager.PMProjectsRiskService.GetAllVW(x => x.IsDeleted == false && x.ProjectId == getProject.Data.Id);

                // الدروس المستفادة
                var GetLessons = await pMServiceManager.PmProjectsLessonsLearnedDetailService.GetAllVW(x => x.IsDeleted == false && x.ProjectId == getProject.Data.Id);

                //سجل طلبات التغيير
                var GetChangeRequests = await pMServiceManager.PmChangeRequestService.GetAllVW(x => x.IsDeleted == false && x.ProjectId == getProject.Data.Id);

                // Prepare response
                var response = new
                {
                    projectDetails,
                    Risks = GetRisks.Data.AsQueryable(),
                    Lessons = GetLessons.Data.AsQueryable(),
                    ChangeRequests = GetChangeRequests.Data.AsQueryable(),
                };
                return Ok(await Result<object>.SuccessAsync(response, string.Empty, 200));


            }
            catch (Exception exp)
            {
                return Ok(await Result<object>.FailAsync($"{exp.Message}"));
            }
        }

    }
}
