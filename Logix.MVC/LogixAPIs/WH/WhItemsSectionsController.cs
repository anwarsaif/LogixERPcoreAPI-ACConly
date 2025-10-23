using Logix.Application.Common;
using Logix.Application.DTOs.WH;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.WH.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.WH
{
	public class WhItemsSectionsController : BaseWHApiController
    {
        private readonly IWhServiceManager whServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;

        public WhItemsSectionsController(IWhServiceManager whServiceManager, IPermissionHelper permission, ICurrentData session)
        {
            this.whServiceManager = whServiceManager;
            this.permission = permission;
            this.session = session;
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var chk = await permission.HasPermission(737, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var items = await whServiceManager.WhItemsSectionService.GetAll(e => e.IsDeleted == false);
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    res = res.OrderBy(e => e.Id);
                    return Ok(items);
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<IEnumerable<WhItemsSectionDto>>.FailAsync($"======= Exp in Wh whunit : {ex.Message}"));

            }
        }
                [HttpGet("GetByIdForEdit")]
        public async Task<IActionResult> GetByIdForEdit(long unitId)
        {
            var chk = await permission.HasPermission(737, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var items = await whServiceManager.WhItemsSectionService.GetById(unitId);
               
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<IEnumerable<WhItemsSectionDto>>.FailAsync($"======= Exp in Wh whunit : {ex.Message}"));

            }
        }
           
        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(long Id)
        {
            var chk = await permission.HasPermission(737, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var items = await whServiceManager.WhItemsSectionService.GetById(Id);
               
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<IEnumerable<WhItemsSectionDto>>.FailAsync($"======= Exp in Wh whunit : {ex.Message}"));

            }
        }

		[HttpPost("Search")]
		public async Task<IActionResult> Search(WhItemsSectionVWDto filter)
		{
			try
			{
				var chk = await permission.HasPermission(737, PermissionType.Show);
				if (!chk)
				{
					return Ok(await Result.AccessDenied("AccessDenied"));
				}

				var items = await whServiceManager.WhItemsSectionService.GetAllVW(e => e.IsDeleted == false
                    && (string.IsNullOrEmpty(filter.ItemName) || e.ItemName == filter.ItemName)
                    && (string.IsNullOrEmpty(filter.ItemCode) || e.ItemCode.Contains(filter.ItemCode))
                    && (string.IsNullOrEmpty(filter.SectionName) || e.SectionName.Contains(filter.SectionName))
                    && (filter.InventoryId == null || e.InventoryId == filter.InventoryId)
                );


				return Ok(items);
			}
			catch (Exception ex)
			{
				return Ok(await Result<IEnumerable<WhItemsSectionDto>>.FailAsync($"Error: {ex.Message}"));
			}
		}

		[HttpPost("Add")]
        public async Task<IActionResult> Add(WhItemsSectionDto entity)
        {
        
              var chk = await permission.HasPermission(737, PermissionType.Add);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
             
                var items = await whServiceManager.WhItemsSectionService.Add(entity);
             
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<WhItemsSectionDto>.FailAsync($"======= Exp in Wh whunit : {ex.Message}"));

            } 

        }

        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(WhItemsSectionEditDto entity)
        {
              var chk = await permission.HasPermission(737, PermissionType.Edit);
                if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
           try
            { 
                var items = await whServiceManager.WhItemsSectionService.Update(entity);
             
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<WhItemsSectionDto>.FailAsync($"======= Exp in Wh whunit : {ex.Message}"));

            } 

        }
        
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete( long Id=0)
        {
              var chk = await permission.HasPermission(737, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            { 
                var items = await whServiceManager.WhItemsSectionService.Remove(Id);
             
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<WhItemsSectionDto>.FailAsync($"======= Exp in Wh whunit delete : {ex.Message}"));

            } 

        }

    }
}
