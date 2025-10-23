using Logix.Application.Common;
using Logix.Application.DTOs.PM;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Services;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.PM.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Data;

namespace Logix.MVC.LogixAPIs.PM
{
    public class PmCostControlController : BasePMApiController
    {
        private readonly IPMServiceManager pmServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;

        public PmCostControlController(IPMServiceManager pMServiceManager,
            IPermissionHelper permission,
            ICurrentData session,
            ILocalizationService localization)
        {
            this.pmServiceManager = pMServiceManager;
            this.permission = permission;
            this.session = session;
            this.localization = localization;
        }

        [HttpGet("Search")]
        public async Task<IActionResult> Search(long projectCode)
        {
            try
            {
                var chk = await permission.HasPermission(1513, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

               
            
                return Ok(await GetPMProjectsItemsByPMCode(projectCode));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"======= Exp: {ex.Message}"));
            }
        }    
        [HttpGet("GetPMProjectsItemsByPMCodeOnAdd")]
        public async Task<IActionResult> GetPMProjectsItemsByPMCodeOnAdd(long projectCode)
        {
            try
            {
                var chk = await permission.HasPermission(1513, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

               
            
                return Ok(await GetPMProjectsItemsByPMCode(projectCode));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"======= Exp: {ex.Message}"));
            }
        }
         private async Task<IResult<IEnumerable<PmCostControlVM>>> GetPMProjectsItemsByPMCode(long projectCode)
        {
            try
            {
                
                var items = await pmServiceManager.PMProjectsItemService.GetAllVW(p => p.IsDeleted == false
                 && p.ProjectCode == projectCode
                );
                if (items.Succeeded)
                {
                    var res = items.Data;

                  

                    List<PmCostControlVM> result = new();
                    foreach (var item in res)
                    {
                        PmCostControlVM obj = new()
                        {
                            Id = item.Id,
                            CostPrice = item.CostPrice,
                            ItemName=  item.ItemName,
                            Price = item.Price,
                            ProjectCode = projectCode,
                            ProjectName = item.ProjectName,
                            Qty = item.Qty,
                            Total = item.Total,
                            TotalCost =item.Qty*item.CostPrice,
                     
                        };
                        result.Add(obj);
                    }
                    return await Result<List<PmCostControlVM>>.SuccessAsync(result);
                }
                return await Result<IEnumerable<PmCostControlVM>>.FailAsync(items.Status.message);
            }
            catch (Exception ex)
            {
                return await Result<IEnumerable<PmCostControlVM>>.FailAsync($"======= Exp: {ex.Message}");
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

                var delete = await pmServiceManager.PMProjectPlanService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }
        [HttpPost("PmCostControlAdd")]
        public async Task<IActionResult> PmCostControlAdd(PmCostControlAddVM entity)
        {
            
           
            if(entity.CheckUpdateProject == true)
            {
                var tempProject = await pmServiceManager.PMProjectsService.GetOne(p=>p.Code == entity.ProjectCode && p.IsDeleted ==false);
                if(tempProject == null )
                {
                    return Ok(await Result<PMProjectsItemDto>.FailAsync($"{localization.GetPMResource("projectisnotinprojectlist")}"));

                }
            }

            var chk = await permission.HasPermission(256, PermissionType.Add);

            if (chk == false)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {

                var items = await pmServiceManager.PMProjectsItemService.PmCostControlAdd(entity);

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<PMProjectsItemDto>.FailAsync($"======= Exp in  : {ex.Message}"));

            }

        }



    }



}
