using Logix.Application.Common;
using Logix.Application.DTOs.Main;
using Logix.Application.DTOs.WH;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Services;
using Logix.Application.Wrapper;
using Logix.Domain.WH;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.WH.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.WH
{
    public class WhUnitController : BaseWHApiController
    {
        private readonly IWhServiceManager whServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;

        public WhUnitController(IWhServiceManager whServiceManager, IPermissionHelper permission, ICurrentData session)
        {
            this.whServiceManager = whServiceManager;
            this.permission = permission;
            this.session = session;
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var chk = await permission.HasPermission(204, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var items = await whServiceManager.WhUnitService.GetAll(e => e.IsDeleted == false);
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    res = res.OrderBy(e => e.UnitId);
                    return Ok(items);
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<IEnumerable<WhUnitDto>>.FailAsync($"======= Exp in Wh whunit : {ex.Message}"));

            }
        }
                [HttpGet("GetByIdForEdit")]
        public async Task<IActionResult> GetByIdForEdit(long unitId)
        {
            var chk = await permission.HasPermission(204, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var items = await whServiceManager.WhUnitService.GetById(unitId);
               
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<IEnumerable<WhUnitDto>>.FailAsync($"======= Exp in Wh whunit : {ex.Message}"));

            }
        }
           
        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(long Id)
        {
            var chk = await permission.HasPermission(204, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var items = await whServiceManager.WhUnitService.GetById(Id);
               
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<IEnumerable<WhUnitDto>>.FailAsync($"======= Exp in Wh whunit : {ex.Message}"));

            }
        }

        [HttpPost("Search")]
        public async Task<IActionResult> Search(UnitSearch entity)
        {
            var chk = await permission.HasPermission(204, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
              
                var items = await whServiceManager.WhUnitService.GetAll(e => e.IsDeleted == false);


                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    if (entity != null)
                    {
                        if(entity.UnitName != null)
                        {
                            res = res.Where(e => (e.UnitName??"").Contains(entity.UnitName) || (e.UnitName2 ?? "").Contains(entity.UnitName));

                        }
                    return Ok(Result<IEnumerable<WhUnitDto>>.Success(res));
                    }

                   
                    res = res.OrderBy(e => e.UnitId);
                    return Ok(items);
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<IEnumerable<WhUnitDto>>.FailAsync($"======= Exp in Wh whunit : {ex.Message}"));

            }
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add(WhUnitDto entity)
        {
        
              var chk = await permission.HasPermission(204, PermissionType.Add);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
             
                var items = await whServiceManager.WhUnitService.Add(entity);
             
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<WhUnitDto>.FailAsync($"======= Exp in Wh whunit : {ex.Message}"));

            } 

        }

        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(WhUnitEditDto entity)
        {
              var chk = await permission.HasPermission(204, PermissionType.Edit);
                if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
           try
            { 
                var items = await whServiceManager.WhUnitService.Update(entity);
             
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<WhUnitDto>.FailAsync($"======= Exp in Wh whunit : {ex.Message}"));

            } 

        }
        
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete( long Id=0)
        {
              var chk = await permission.HasPermission(204, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            { 
                var items = await whServiceManager.WhUnitService.Remove(Id);
             
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<WhUnitDto>.FailAsync($"======= Exp in Wh whunit delete : {ex.Message}"));

            } 

        }

    }
}
