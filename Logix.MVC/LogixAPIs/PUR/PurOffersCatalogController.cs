using Logix.Application.Common;
using Logix.Application.DTOs.ACC;
using Logix.Application.DTOs.PUR;
using Logix.Application.Helpers.Acc;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Services;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;
using Logix.Domain.PUR;
using Logix.Infrastructure.Mapping.PUR;
using Logix.MVC.Helpers;

using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Logix.MVC.LogixAPIs.PUR
{
    public class PurOffersCatalogController : BasePurApiController
    {
        private readonly IPurServiceManager purServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;

        public PurOffersCatalogController(IPurServiceManager purServiceManager,
            IPermissionHelper permission,
            ICurrentData session,
            ILocalizationService localization)
        {
            this.purServiceManager = purServiceManager;
            this.permission = permission;
            this.session = session;
            this.localization = localization;
        }

        #region "GetAll - Search"
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var chk = await permission.HasPermission(631, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var items = await purServiceManager.PurDiscountCatalogService.GetAll(x => x.IsDeleted == false);
                if (items.Succeeded)
                {
                    return Ok(await Result<List<PurDiscountCatalogDto>>.SuccessAsync(items.Data.ToList(), ""));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }
        [HttpPost("Search")]
        public async Task<IActionResult> Search(PurDiscountCatalogFilterDto filter)
        {
            var chk = await permission.HasPermission(631, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                filter.BranchId ??= 0;
                if (filter.BranchId == 0) { filter.BranchId = session.BranchId; }
                var items = await purServiceManager.PurDiscountCatalogService.GetAll(x => x.IsDeleted == false
                && (filter.BranchId == 0 || x.BranchId == filter.BranchId)
                );
                if ((items.Succeeded))
                {
                    return Ok(await Result<List<PurDiscountCatalogDto>>.SuccessAsync(items.Data.ToList(), ""));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<PurDiscountCatalogDto>.FailAsync($"======= Exp in Search Pur PurDiscountCatalog Schedule, MESSAGE: {ex.Message}"));
            }
        }
        #endregion "GetAll - Search"

        #region "Add - Edit"
        [HttpPost("Add")]
        public async Task<IActionResult> Add(PurDiscountCatalogDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(631, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync(localization.GetMessagesResource("dataRequire")));
                var add = await purServiceManager.PurDiscountCatalogService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(PurDiscountCatalogEditDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(631, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var Edit = await purServiceManager.PurDiscountCatalogService.Update(obj);
                if (Edit.Succeeded)
                {
                    return Ok(Edit);
                }
                else
                {
                    return Ok(await Result<PurDiscountCatalogEditDto>.FailAsync(localization.GetResource1("UpdateError")));
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<PurDiscountCatalogEditDto>.FailAsync($"======= Exp in Pur PurDiscountCatalog Schedule  edit: {ex.Message}"));
            }
        }

        #endregion "Add - Edit"

        #region "Delete"
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long Id)
        {
            var chk = await permission.HasPermission(631, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var removedItem = await purServiceManager.PurDiscountCatalogService.Remove(Id);
                if (removedItem.Succeeded)
                {
                    return Ok(removedItem);
                }
                else
                {
                    return Ok(await Result<PurDiscountCatalogDto>.FailAsync(localization.GetResource1("DeleteFail")));
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<PurDiscountCatalogDto>.FailAsync($"======= Exp in PUR PurDiscountCatalog Schedule   Delete: {ex.Message}"));
            }
        }
        #endregion "Delete"

        #region "GetByIdForEdit - GetById - GetAmountDetailsByCatalogId - GetQtyDetailsByCatalogId - GetProductDetailsByCatalogId"

        [HttpGet("GetByIdForEdit")]
        public async Task<IActionResult> GetByIdForEdit(long id)
        {
            try
            {
                var chk = await permission.HasPermission(631, PermissionType.Show);
                if (!chk)
                {
                    return Ok(await Result.AccessDenied("AccessDenied"));
                }

                if (id <= 0)
                {
                    return Ok(await Result<PurDiscountCatalogEditDto>.FailAsync(localization.GetMessagesResource("InCorrectId")));
                }

                var getItem = await purServiceManager.PurDiscountCatalogService.GetForUpdate<PurDiscountCatalogEditDto>(id);
                if (getItem.Succeeded)
                {
                    if(getItem.Data.DiscountType == 1)
                    {
                        // by mount
                        var itemByAmount = await purServiceManager.PurDiscountByAmountService.GetAll(x=>x.IsDeleted==false && x.DisCatalogId ==  id);
                        getItem.Data.purDiscountByAmounts = itemByAmount.Data.ToList();

                    }else if(getItem.Data.DiscountType == 2)
                    {
                        // by qty
                        var itemByQty = await purServiceManager.PurDiscountByQtyService.GetAll(x => x.IsDeleted == false && x.DisCatalogId == id);
                        getItem.Data.purDiscountByQties = itemByQty.Data.ToList();
                    }
                    var itemProducts = await purServiceManager.PurDiscountProductService.GetAll(x => x.IsDeleted == false && x.DisCatalogId == id);
                    getItem.Data.purDiscountProducts = itemProducts.Data.ToList();
                    return Ok(await Result<PurDiscountCatalogEditDto>.SuccessAsync(getItem.Data, $""));
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
                var chk = await permission.HasPermission(631, PermissionType.Show);
                if (!chk)
                {
                    return Ok(await Result.AccessDenied("AcessDenied"));
                }

                if (id <= 0)
                {
                    return Ok(await Result<PurDiscountCatalogDto>.FailAsync(localization.GetMessagesResource("InCorrectId")));
                }

                var getItem = await purServiceManager.PurDiscountCatalogService.GetOne(s => s.Id == id && s.IsDeleted == false);
                if (getItem.Succeeded)
                {
                    var obj = getItem.Data;
                    return Ok(await Result<PurDiscountCatalogDto>.SuccessAsync(obj, $""));
                }
                else
                {
                    return Ok(getItem);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<PurDiscountCatalogDto>.FailAsync($"====== Exp MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetAmountDetailsByCatalogId")]
        public async Task<IActionResult> GetAmountDetailsByCatalogId(long id)
        {
            try
            {
                if (id <= 0)
                {
                    return Ok(await Result<PurDiscountByAmountDto>.FailAsync(localization.GetMessagesResource("InCorrectId")));
                }

                var getItem = await purServiceManager.PurDiscountByAmountService.GetAll(x => x.DisCatalogId == id && x.IsDeleted == false);
                if (getItem.Succeeded)
                {
                    var obj = getItem.Data;
                    return Ok(await Result<List<PurDiscountByAmountDto>>.SuccessAsync(obj.ToList(), $""));
                }
                else
                {
                    return Ok(getItem);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<List<PurDiscountByAmountDto>>.FailAsync($"====== Exp in GetAmountDetailsByCatalogId Pur , MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetQtyDetailsByCatalogId")]
        public async Task<IActionResult> GetQtyDetailsByCatalogId(long id)
        {
            try
            {
                if (id <= 0)
                {
                    return Ok(await Result<PurDiscountByQtyDto>.FailAsync(localization.GetMessagesResource("InCorrectId")));
                }

                var getItem = await purServiceManager.PurDiscountByQtyService.GetAll(x => x.DisCatalogId == id && x.IsDeleted == false);
                if (getItem.Succeeded)
                {
                    var obj = getItem.Data;
                    return Ok(await Result<List<PurDiscountByQtyDto>>.SuccessAsync(obj.ToList(), $""));
                }
                else
                {
                    return Ok(getItem);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<List<PurDiscountByQtyDto>>.FailAsync($"====== Exp in GetQtyDetailsByCatalogId Pur , MESSAGE: {ex.Message}"));
            }
        }
        
        [HttpGet("GetProductDetailsByCatalogId")]
        public async Task<IActionResult> GetProductDetailsByCatalogId(long id)
        {
            try
            {
                if (id <= 0)
                {
                    return Ok(await Result<PurDiscountProductDto>.FailAsync(localization.GetMessagesResource("InCorrectId")));
                }

                var getItem = await purServiceManager.PurDiscountProductService.GetAll(x => x.DisCatalogId == id && x.IsDeleted == false);
                if (getItem.Succeeded)
                {
                    var obj = getItem.Data;
                    return Ok(await Result<List<PurDiscountProductDto>>.SuccessAsync(obj.ToList(), $""));
                }
                else
                {
                    return Ok(getItem);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<List<PurDiscountProductDto>>.FailAsync($"====== Exp in GetProductDetailsByCatalogId Pur , MESSAGE: {ex.Message}"));
            }
        }

        #endregion "GetByIdForEdit - GetById - GetAmountDetailsByCatalogId - GetQtyDetailsByCatalogId - GetProductDetailsByCatalogId"
    }
}
