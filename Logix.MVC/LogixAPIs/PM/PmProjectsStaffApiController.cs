using Logix.Application.Common;
using Logix.Application.DTOs.PM;
using Logix.Application.DTOs.PM.PmProjectsStaff;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.PM;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;

namespace Logix.MVC.LogixAPIs.PM
{
    public class PmProjectsStaffApiController : BasePMApiController
    {
        private readonly IPMServiceManager pMServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;

        public PmProjectsStaffApiController(IPMServiceManager pMServiceManager, IPermissionHelper permission, ICurrentData session)
        {
            this.pMServiceManager = pMServiceManager;
            this.permission = permission;
            this.session = session;
        }
   
        [HttpGet("GetAllVM")]
        public async Task<IActionResult> GetAllVM()
        {
            var chk = await permission.HasPermission(1810, PermissionType.Show);
          
            if (chk ==false )
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {

                var items = await pMServiceManager.PMProjectsStaffService.GetAllVW(e => e.IsDeleted == false);
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    res = res.OrderBy(e => e.Id);
                    return Ok(await Result<List<PmProjectsStaffVw>>.SuccessAsync(res.ToList(),items.Status.message));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<List<PmProjectsStaffVw>>.FailAsync($"======= Exp in Wh whunit : {ex.Message}"));

            }
        }
          
        [HttpGet("GetAllStaffByProject")]
        public async Task<IActionResult> GetAllStaffByProject(long projectId)
        {
            var chk = await permission.HasPermission(1810, PermissionType.Show);
          
            if (chk ==false )
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {

                var items = await pMServiceManager.PMProjectsStaffService.GetAllVW(e => e.ProjectId == projectId&& e.IsDeleted == false); ;
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    res = res.OrderBy(e => e.Id);
                    return Ok(await Result<List<PmProjectsStaffVw>>.SuccessAsync(res.ToList(),items.Status.message));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<List<PmProjectsStaffVw>>.FailAsync($"======= Exp in Wh whunit : {ex.Message}"));

            }
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add(PMProjectsStaffAddDto entity)
        {

            var chk = await permission.HasPermission(256, PermissionType.Add);
         
            if (chk == false )
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
              
                var items = await pMServiceManager.PMProjectsStaffService.Add(entity);

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<PmProjectsStaffDto>.FailAsync($"======= Exp in Wh whunit : {ex.Message}"));

            }

        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
           
            var chk = await permission.HasPermission(256, PermissionType.Delete);
         
            if (chk == false )
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var items = await pMServiceManager.PMProjectsStaffService.Remove(id);

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<PmProjectsStaffDto>.FailAsync($"======= Exp in Wh whunit delete : {ex.Message}"));
            }

        }   
        
   
    }


}
