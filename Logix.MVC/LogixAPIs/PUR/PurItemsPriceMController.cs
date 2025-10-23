using Logix.Application.Common;
using Logix.Application.DTOs.PUR;
using Logix.Application.DTOs.SAL;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.PUR;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.PUR
{
    public class PurItemsPriceMController : BasePurApiController
    {
        private readonly IPurServiceManager purServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ILocalizationService localization;
        private readonly ICurrentData session;

        public PurItemsPriceMController(IPurServiceManager purServiceManager,
            IPermissionHelper permission,
            ICurrentData CurrentData,
            ILocalizationService localization,
            ICurrentData session)
        {
            this.purServiceManager = purServiceManager;
            this.permission = permission;
            this.localization = localization;
            this.session = session;
        }
        #region "GetAll - Search"
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var chk = await permission.HasPermission(2033, PermissionType.Show);
            if (!chk)
                return Ok(await Result.AccessDenied("AccessDenied"));
            try
            {
                var items = await purServiceManager.PurItemsPriceMService.GetAllVW(e => e.IsDeleted == false);
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in GetAll PurItemsPriceMController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Search")]
        public async Task<ActionResult> Search(PurItemsPriceMFilterDto filter)
        {
            try
            {
                var chk = await permission.HasPermission(2033, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                filter.FacilityId ??= 0;
                filter.BranchId ??= 0;
                var items = await purServiceManager.PurItemsPriceMService.GetAllVW(x => x.IsDeleted == false
                && (string.IsNullOrEmpty(filter.Name) || x.Name == filter.Name)
                && (filter.FacilityId == 0 || x.FacilityId == filter.FacilityId)
                && (filter.BranchId == 0 || x.BranchId == filter.BranchId)
                );
                if ((items.Succeeded))
                {
                    return Ok(await Result<List<PurItemsPriceMVw>>.SuccessAsync(items.Data.ToList(), ""));
                }
                return Ok(await Result<PurItemsPriceMFilterDto>.SuccessAsync(filter, localization.GetResource1("NosearchResult")));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Search SalItemsPriceMController, MESSAGE: {ex.Message}"));
            }
        }
        #endregion "GetAll - Search"
        
        #region "Add - Edit"
        [HttpPost("Add")]
        public async Task<IActionResult> Add(PurItemsPriceMDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(2033, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync(localization.GetMessagesResource("dataRequire")));

                var add = await purServiceManager.PurItemsPriceMService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(PurItemsPriceMEditDto obj)
        {
            var chk = await permission.HasPermission(2033, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<PurItemsPriceMEditDto>.FailAsync(localization.GetMessagesResource("dataRequire")));
                }
                var Edit = await purServiceManager.PurItemsPriceMService.Update(obj);
                if (Edit.Succeeded)
                {
                    return Ok(Edit);
                }
                else
                {
                    return Ok(await Result<PurItemsPriceMEditDto>.FailAsync(localization.GetResource1("UpdateError")));
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<PurItemsPriceMEditDto>.FailAsync($"======= Exp in PurItemsPriceM edit: {ex.Message}"));
            }
        }
        #endregion "Add - Edit"

        #region "Delete"
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long Id)
        {
            var chk = await permission.HasPermission(2033, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var removedItem = await purServiceManager.PurItemsPriceMService.Remove(Id);
                if (removedItem.Succeeded)
                {
                    return Ok(removedItem);
                }
                else
                {
                    return Ok(await Result<PurItemsPriceMDto>.FailAsync(localization.GetResource1("DeleteFail")));
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<PurItemsPriceMDto>.FailAsync($"======= Exp in PurItemsPriceM Schedule   Delete: {ex.Message}"));
            }
        }
        #endregion "Delete"

        #region "GetByIdForEdit - GetById - GetItemsPriceDByItemsPriceMId"

        [HttpGet("GetByIdForEdit")]
        public async Task<IActionResult> GetByIdForEdit(long id)
        {
            try
            {
                var chk = await permission.HasPermission(2033, PermissionType.Show);
                if (!chk)
                {
                    return Ok(await Result.AccessDenied("AccessDenied"));
                }

                if (id <= 0)
                {
                    return Ok(await Result<PurItemsPriceMEditDto>.FailAsync(localization.GetMessagesResource("InCorrectId")));
                }

                var getItem = await purServiceManager.PurItemsPriceMService.GetForUpdate<PurItemsPriceMEditDto>(id);
                if (getItem.Succeeded)
                {
                    var itemProducts = await purServiceManager.PurItemsPriceDService.GetAll(x => x.IsDeleted == false && x.ItemPriceMId == id);
                    getItem.Data.Details = itemProducts.Data.ToList();
                    return Ok(await Result<PurItemsPriceMEditDto>.SuccessAsync(getItem.Data, $""));
                }
                else
                {
                    return Ok(getItem);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(long id)
        {
            try
            {
                var chk = await permission.HasPermission(2033, PermissionType.Show);
                if (!chk)
                {
                    return Ok(await Result.AccessDenied("AcessDenied"));
                }

                if (id <= 0)
                {
                    return Ok(await Result<PurItemsPriceMDto>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var getItem = await purServiceManager.PurItemsPriceMService.GetOne(s => s.Id == id && s.IsDeleted == false);
                if (getItem.Succeeded)
                {
                    var obj = getItem.Data;
                    return Ok(await Result<PurItemsPriceMDto>.SuccessAsync(obj, $""));
                }
                else
                {
                    return Ok(getItem);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<PurItemsPriceMDto>.FailAsync($"====== Exp MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetDetailsById")]
        public async Task<IActionResult> GetItemsPriceDByItemsPriceMId(long id)
        {
            try
            {
                if (id <= 0)
                {
                    return Ok(await Result<PurItemsPriceDDto>.FailAsync(localization.GetMessagesResource("InCorrectId")));
                }

                var getItem = await purServiceManager.PurItemsPriceDService.GetAll(x => x.ItemPriceMId == id && x.IsDeleted == false);
                if (getItem.Succeeded)
                {
                    var obj = getItem.Data;
                    return Ok(await Result<List<PurItemsPriceDDto>>.SuccessAsync(obj.ToList(), $""));
                }
                else
                {
                    return Ok(getItem);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<List<PurItemsPriceDDto>>.FailAsync($"====== Exp in GetByIdForEdit Acc Settlement Schedule , MESSAGE: {ex.Message}"));
            }
        }
        #endregion "GetByIdForEdit - GetById - GetItemsPriceDByItemsPriceMId"
    }
}
