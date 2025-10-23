using Logix.Application.Common;
using Logix.Application.DTOs.PM.Shared;
using Logix.Application.DTOs.WF;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.PM;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;

namespace Logix.MVC.LogixAPIs.PM
{
    // http://localhost:8080/Apps/ProjectsMangement/Charter/MyProjects2
    public class PMMyProjects2Controller : BasePMApiController
    {
        private readonly IPMServiceManager pMServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;



        public PMMyProjects2Controller(IPMServiceManager pMServiceManager, IPermissionHelper permission, ICurrentData session)
        {
            this.pMServiceManager = pMServiceManager;
            this.permission = permission;
            this.session = session;
        }

        #region Index Page


        [HttpGet("GetProjectsStatusCount")]
        public async Task<IActionResult> GetProjectsStatusCount(int Id)
        {
            try
            {
                var chk = await permission.HasPermission(1740, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (Id <= 0)
                    return Ok(await Result<object>.FailAsync("ownerDeptId Is Required"));

                var FacilityId = session.FacilityId;
                var filter = new DeptProjectDetailsFilterDto
                {
                    StatusId = 0,
                    OwnerDeptId = Id,
                    CharterStatus = 0,
                    EmpId = 0,
                    FacilityId = session.FacilityId,
                    ProjectId = 0,
                    CurrDate = DateTime.Now,

                };
                var result = await pMServiceManager.PMSharedService.GetProjectStatusCounts(filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"Error fetching  data, MESSAGE: {ex.Message}"));
            }
        }

    

        [HttpGet("GetData")]
        public async Task<IActionResult> GetData(int ownerDeptId, int statusId = 0)
        {
            try
            {
                var hasPermission = await permission.HasPermission(1740, PermissionType.Show);
                if (!hasPermission)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if(ownerDeptId<=0)
                    return Ok(await Result<object>.FailAsync("ownerDeptId Is Required"));

                var filter = new DeptProjectDetailsFilterDto
                {
                    StatusId = statusId,
                    OwnerDeptId = ownerDeptId,
                    CharterStatus = 0,
                    EmpId = 0,
                    FacilityId = 0,
                    ProjectId = 0,
                    CurrDate = DateTime.Now,

                };
                var result = await pMServiceManager.PMSharedService.GetProjectsByDeeptAndStatus(filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"Error fetching project details, MESSAGE: {ex.Message}"));
            }
        }
      
        
        
        #endregion
    }
}
