using Logix.Application.Common;
using Logix.Application.DTOs.Main;
using Logix.Application.DTOs.PM;
using Logix.Application.DTOs.PM.PmProjects;
using Logix.Application.DTOs.PM.Shared;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.PM;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;

namespace Logix.MVC.LogixAPIs.PM
{
    //  الدروس المستفادة 
    public class PMLessonLearnedController : BasePMApiController
    {
        private readonly IPMServiceManager pMServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IWFServiceManager wFServiceManager;
        private readonly IMainServiceManager mainServiceManager;



        public PMLessonLearnedController(IPMServiceManager pMServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IWFServiceManager wFServiceManager, IMainServiceManager mainServiceManager)
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
        public async Task<IActionResult> Search(PMSharedFilterDto filter)
        {
            var chk = await permission.HasPermission(1751, PermissionType.Show);

            if (!chk)
                return Ok(await Result.AccessDenied("AccessDenied"));

            try
            {
                filter.ProjectCode ??= 0;


                var items = await pMServiceManager.PmProjectsLessonsLearnedMasterService.GetAllVW(e => e.IsDeleted == false
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
                    res = res.Where(x => x.CreationDate != null && x.CreationDate != "" && DateHelper.StringToDate(x.CreationDate) >= DateFrom && DateHelper.StringToDate(x.CreationDate) <= DateTo);
                }

                if (!res.Any())
                    return Ok(await Result<object>.SuccessAsync(new List<object>(), localization.GetResource1("NosearchResult")));


                res = res.OrderByDescending(x => x.Id);

                return Ok(await Result<object>.SuccessAsync(res, ""));
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
                var chk = await permission.HasPermission(1751, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await pMServiceManager.PmProjectsLessonsLearnedMasterService.Remove(id);

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
                var check = await permission.HasPermission(1751, PermissionType.Edit);

                if (!check)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetMessagesResource("NoIdInUpdate")));

                var item = await pMServiceManager.PmProjectsLessonsLearnedMasterService.GetOneVW(x => x.IsDeleted == false && x.Id == id);
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

                // Retrieve Files
                var fileResult = await mainServiceManager.SysFileService.GetFilesForUser(id, 1751);
                var fileDtos = fileResult.Data;

                // Prepare response
                var response = new
                {
                    Data = item.Data,
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
        public async Task<IActionResult> Edit(PmProjectsLessonsLearnedMasterEditDto obj)
        {
            try
            {


                var chk = await permission.HasPermission(1751, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (obj.Id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));


                obj.ProjectCode ??= 0;
                obj.LessonLearnedCats ??= 0;
                obj.ImpactType ??= 0;
                obj.ProcedureType ??= 0;
                obj.FollowApplyType ??= 0;

                // التاريخ
                if (string.IsNullOrEmpty(obj.CreationDate))
                    return Ok(await Result<object>.FailAsync(localization.GetCommonResource("Tdate")));

                // الموظف 
                if (string.IsNullOrEmpty(obj.EmpCode))
                    return Ok(await Result<object>.FailAsync(localization.GetHrResource("EmpNo")));


                //    وصف التجربة/الحدث 

                if (string.IsNullOrEmpty(obj.Description))
                    return Ok(await Result<object>.FailAsync(" وصف التجربة/الحدث"));

                //    وصف الأثر الحاصل على المشروع 

                if (string.IsNullOrEmpty(obj.ProjectImpact))
                    return Ok(await Result<object>.FailAsync("وصف الأثر الحاصل على المشروع"));

                //    مسببات الأثر الحاصل 

                if (string.IsNullOrEmpty(obj.ProjectEffect))
                    return Ok(await Result<object>.FailAsync("مسببات الأثر الحاصل"));

                //    المقترحات والحلول  

                if (string.IsNullOrEmpty(obj.Solutions))
                    return Ok(await Result<object>.FailAsync("المقترحات والحلول"));

                //    الدرس المستفاد  

                if (string.IsNullOrEmpty(obj.LessonLeaned))
                    return Ok(await Result<object>.FailAsync("الدرس المستفاد"));

                // المشروع 
                if (obj.ProjectCode <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("Project")));

                var result = await pMServiceManager.PmProjectsLessonsLearnedMasterService.Update(obj);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"======= Exp in {this.GetType()}: {ex.Message}"));

            }

        }

        #endregion




        #region Add Page


        [HttpPost("Add")]
        public async Task<IActionResult> Add(PmProjectsLessonsLearnedMasterDto obj)
        {

            try
            {


                var chk = await permission.HasPermission(1751, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (obj.Id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));


                obj.ProjectCode ??= 0;
                obj.LessonLearnedCats ??= 0;
                obj.ImpactType ??= 0;
                obj.ProcedureType ??= 0;
                obj.FollowApplyType ??= 0;

                // التاريخ
                if (string.IsNullOrEmpty(obj.CreationDate))
                    return Ok(await Result<object>.FailAsync(localization.GetCommonResource("Tdate")));

                // الموظف 
                if (string.IsNullOrEmpty(obj.EmpCode))
                    return Ok(await Result<object>.FailAsync(localization.GetHrResource("EmpNo")));


                //    وصف التجربة/الحدث 

                if (string.IsNullOrEmpty(obj.Description))
                    return Ok(await Result<object>.FailAsync(" وصف التجربة/الحدث"));

                //    وصف الأثر الحاصل على المشروع 

                if (string.IsNullOrEmpty(obj.ProjectImpact))
                    return Ok(await Result<object>.FailAsync("وصف الأثر الحاصل على المشروع"));

                //    مسببات الأثر الحاصل 

                if (string.IsNullOrEmpty(obj.ProjectEffect))
                    return Ok(await Result<object>.FailAsync("مسببات الأثر الحاصل"));

                //    المقترحات والحلول  

                if (string.IsNullOrEmpty(obj.Solutions))
                    return Ok(await Result<object>.FailAsync("المقترحات والحلول"));

                //    الدرس المستفاد  

                if (string.IsNullOrEmpty(obj.LessonLeaned))
                    return Ok(await Result<object>.FailAsync("الدرس المستفاد"));

                // المشروع 
                if (obj.ProjectCode <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("Project")));

                var result = await pMServiceManager.PmProjectsLessonsLearnedMasterService.Add(obj);
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
