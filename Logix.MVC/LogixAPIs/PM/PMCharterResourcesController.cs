using Logix.Application.Common;
using Logix.Application.DTOs.PM;
using Logix.Application.DTOs.PM.Shared;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Infrastructure.Repositories;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;

namespace Logix.MVC.LogixAPIs.PM
{
    //  موارد المشروع
    public class PMCharterResourcesController : BasePMApiController
    {
        private readonly IPMServiceManager pMServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IWFServiceManager wFServiceManager;
        private readonly IMainServiceManager mainServiceManager;



        public PMCharterResourcesController(IPMServiceManager pMServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IWFServiceManager wFServiceManager, IMainServiceManager mainServiceManager)
        {
            this.pMServiceManager = pMServiceManager;
            this.permission = permission;
            this.session = session;
            this.localization = localization;
            this.wFServiceManager = wFServiceManager;
            this.mainServiceManager = mainServiceManager;
        }



        [HttpPost("Search")]
        public async Task<IActionResult> Search(PmProjectsResourceFilterDto filter)
        {
            var chk = await permission.HasPermission(1753, PermissionType.Show);

            if (!chk)
                return Ok(await Result.AccessDenied("AccessDenied"));

            try
            {
                filter.ProjectCode ??= 0;
                filter.Id ??= 0;
                filter.DeptId ??= 0;


                var items = await pMServiceManager.PmProjectsResourceService.GetAllVW(e => e.IsDeleted == false
                && (string.IsNullOrEmpty(filter.ProjectName) || ((e.ProjectName != null && e.ProjectName.Contains(filter.ProjectName))))
                && (filter.ProjectCode == 0 || e.ProjectCode == filter.ProjectCode)
                && (filter.Id == 0 || e.Id == filter.Id)
                && (filter.DeptId == 0 || e.DeptId == filter.DeptId)
                );

                if (!items.Succeeded)
                    return Ok(await Result<object>.FailAsync(items.Status.message));
                if (!items.Data.Any())
                    return Ok(await Result<object>.SuccessAsync(new List<object>(), localization.GetResource1("NosearchResult")));

                var res = items.Data.AsQueryable();

                if (!res.Any())
                    return Ok(await Result<object>.SuccessAsync(new List<object>(), localization.GetResource1("NosearchResult")));


                return Ok(await Result<object>.SuccessAsync(res, ""));
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"======= Exp in {this.GetType()}: {ex.Message}"));
            }
        }




        [HttpGet("ResourceView")]
        public async Task<IActionResult> ResourceView(long id)
        {
            try
            {
                // Check permissions
                var check = await permission.HasPermission(1753, PermissionType.Edit);

                if (!check)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetMessagesResource("NoIdInUpdate")));

                var item = await pMServiceManager.PmProjectsResourceService.GetOneVW(x => x.IsDeleted == false && x.Id == id);
                if (!item.Succeeded || item.Data == null)
                    return Ok(await Result<object>.FailAsync(item.Succeeded ? localization.GetMessagesResource("NoIdInUpdate") : item.Status.message));

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

                // Prepare response
                var response = new
                {
                    Data = item.Data,
                    projectDetails
                };

                return Ok(await Result<object>.SuccessAsync(response, string.Empty, 200));
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"Error in PMCharterResourcesController View method, MESSAGE: {ex.Message}"));
            }
        }



        [HttpPost("Add")]
        public async Task<IActionResult> Add(PmProjectsResourceDto obj)
        {

            try
            {
                var chk = await permission.HasPermission(1753, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                obj.ProjectCode ??= 0;
                obj.InternalOrExternal ??= 0;
                obj.DeptId ??= 0;
                obj.ManagerId ??= 0;
                obj.AppTypeId ??= 0;

                //  نوع المورد (الوصف)
                if (string.IsNullOrEmpty(obj.ResourceType))
                    return Ok(await Result<object>.FailAsync(localization.GetCommonResource("Tdate")));

                //  داخلي أو خارجي 
                if (obj.InternalOrExternal <= 0) return Ok(await Result<object>.FailAsync("داخلي أو خارجي"));

                //   الادارة	
                if (obj.DeptId <= 0) return Ok(await Result<object>.FailAsync("الادارة"));

                //   المدير	
                if (obj.ManagerId <= 0) return Ok(await Result<object>.FailAsync("المدير"));

                // المشروع 
                if (obj.ProjectCode <= 0) return Ok(await Result<object>.FailAsync(localization.GetPMResource("Project")));

                if (string.IsNullOrEmpty(obj.Note)) obj.Note = "";
                // فحص تواجد المشروع
                var Project = await pMServiceManager.PMProjectsService.GetOne(x => x.IsDeleted == false && x.Code == obj.ProjectCode);
                if (!Project.Succeeded || Project.Data == null)
                    return Ok(await Result<object>.FailAsync(Project.Succeeded ? "المشروع غير موجود" : Project.Status.message));
                obj.ProjectId = Project.Data.Id;
                obj.SendToWorkFlow = 1;
                var result = await pMServiceManager.PmProjectsResourceService.Add(obj);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"======= Exp in {this.GetType()}: {ex.Message}"));

            }

        }


    }
}
