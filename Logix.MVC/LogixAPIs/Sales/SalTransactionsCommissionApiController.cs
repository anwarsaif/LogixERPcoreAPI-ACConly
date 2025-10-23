using Logix.Application.Common;
using Logix.Application.DTOs.PM.PmProjectsStage;
using Logix.Application.DTOs.PM;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.PM;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.PM.ViewModel.PmProjectsStageVMFilter;
using Microsoft.AspNetCore.Mvc;
using Logix.Domain.SAL;
using Logix.Application.DTOs.SAL.SALTransactionsCommission;
using Logix.MVC.LogixAPIs.Sales.ViewModel;
using System.Linq.Expressions;
using Microsoft.IdentityModel.Tokens;

namespace Logix.MVC.LogixAPIs.Sales
{
    public class SalTransactionsCommissionApiController : BaseSalesApiController
    {
        private readonly ISalServiceManager salServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;

        public SalTransactionsCommissionApiController(ISalServiceManager salServiceManager, IPermissionHelper permission, ICurrentData session)
        {
            this.salServiceManager = salServiceManager;
            this.permission = permission;
            this.session = session;
        }
        public static Expression<Func<SalTransactionsCommissionVw, bool>> FilterTest(SalTransactionsCommissionVMFilter filter)
        {
            
            return obj =>
            obj.IsDeleted == false
         
            || (filter.Amount != null && obj.Amount == filter.Amount) 
            || (filter.Rate != null && obj.Rate == filter.Rate)
            ; //end
        } 
     
        [HttpPost("Search")]
        public async Task<IActionResult> Search(SalTransactionsCommissionVMFilter filter)
        {
            var chk = await permission.HasPermission(964, PermissionType.Show);

            if (!chk)

            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
              
                var items = await salServiceManager.SalTransactionsCommissionService.GetAllVW(FilterTest(filter));
                //var items = await salServiceManager.SalTransactionsCommissionService.GetAllVW(x=>x.EmpCode==filter.EmpCode);
                
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    res = res.OrderBy(e => e.Id);

                    if(filter.Amount != null)
                    {
                        res = res.Where(x=>x.Amount==filter.Amount);
                    }
                    if (filter.Rate != null)
                    {
                        res = res.Where(x => x.Rate == filter.Rate);
                    }
                    if (!string.IsNullOrEmpty(filter.EmpCode) )
                    {
                        res = res.Where(x => x.EmpCode == filter.EmpCode);
                    }  
                    if (filter.ProjectCode !=null )
                    {
                        res = res.Where(x => x.ProjectCode == filter.ProjectCode);
                    } 
                    if (filter.TypeId !=null )
                    {
                        res = res.Where(x => x.TypeId == filter.TypeId);
                    }
                    return Ok(await Result<List<SalTransactionsCommissionVw>>.SuccessAsync(res.ToList(), items.Status.message));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<List<SalTransactionsCommissionVw>>.FailAsync($"======= Exp in Wh whunit : {ex.Message}"));

            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(long id)
        {
            var chk = await permission.HasPermission(964, PermissionType.Show);

            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var items = await salServiceManager.SalTransactionsCommissionService.GetOneVW(x => x.Id == id  && x.IsDeleted == false);

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<List<PmProjectsStagesVw>>.FailAsync($"======= Exp in Wh whunit delete : {ex.Message}"));
            }
        }


        [HttpPost("Add")]
        public async Task<IActionResult> Add(SalTransactionsCommissionAddDto entity)
        {

            var chk = await permission.HasPermission(964, PermissionType.Add);

            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
           

                var items = await salServiceManager.SalTransactionsCommissionService.Add(entity);

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<SalTransactionsCommissionAddDto>.FailAsync($"======= Exp in Wh whunit : {ex.Message}"));

            }

        }
    
        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(SalTransactionsCommissionEditDto entity)
        {
            var chk = await permission.HasPermission(964, PermissionType.Edit);

            if (!chk)
            {

                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var items = await salServiceManager.SalTransactionsCommissionService.Update(entity);

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<SalTransactionsCommissionEditDto>.FailAsync($"======= Exp in Wh whunit : {ex.Message}"));

            }

        }
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {

            var chk = await permission.HasPermission(964, PermissionType.Delete);

            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var items = await salServiceManager.SalTransactionsCommissionService.Remove(id);

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<SalTransactionsCommissionDto>.FailAsync($"======= Exp in Wh whunit delete : {ex.Message}"));
            }

        }


  /*      [HttpPost("FilterStage")]
        public async Task<IActionResult> FilterStage(PmProjectsStageFilter filter)
        {
            var chk = await permission.HasPermission(1932, PermissionType.Show);

            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var items = await pMServiceManager.PMProjectsStageService.GetAllVW(x => x.IsDeleted == false);


                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    if (filter.Id != 0)
                    {
                        res = res.Where(x => x.Id == filter.Id);

                    }
                    if (filter.Name is null)
                    {
                        res = res.Where(x => x.Name.Contains(filter.Name) || x.Name2.Contains(filter.Name));

                    }
                }

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<List<PmProjectsStagesVw>>.FailAsync($"======= Exp in Wh whunit delete : {ex.Message}"));

            }

        }
        [HttpPost("FilterSubStage")]
        public async Task<IActionResult> FilterSubStage(PmProjectsSubStageFilter filter)
        {
            var chk = await permission.HasPermission(1932, PermissionType.Show);

            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var items = await pMServiceManager.PMProjectsStageService.GetAllVW(x => x.IsDeleted == false && x.Id != x.ParentId);


                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    if (filter.Id != 0)
                    {
                        res = res.Where(x => x.Id == filter.Id);

                    }
                    if (filter.ParentId != 0)
                    {
                        res = res.Where(x => x.ParentId == filter.ParentId);

                    }
                    if (filter.Name is null)
                    {
                        res = res.Where(x => x.Name.Contains(filter.Name) || x.Name2.Contains(filter.Name));

                    }
                }

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<List<PmProjectsStagesVw>>.FailAsync($"======= Exp in Wh whunit delete : {ex.Message}"));

            }

        }

*/

    }


}
