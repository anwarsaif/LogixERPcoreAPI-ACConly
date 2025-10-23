using Logix.Application.Common;
using Logix.Application.DTOs.Main;
using Logix.Application.DTOs.PUR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.PUR;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Logix.MVC.LogixAPIs.PUR
{
    public class PurPurchaseOrderFollowController : BasePurApiController
    {
        private readonly IPurServiceManager purServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IMainServiceManager mainServiceManager;

        public PurPurchaseOrderFollowController(
            IPurServiceManager purServiceManager,
            IPermissionHelper permission,
            ICurrentData session,
            ILocalizationService localization,
            IMainServiceManager mainServiceManager)
        {
            this.purServiceManager = purServiceManager;
            this.permission = permission;
            this.session = session;
            this.localization = localization;
            this.mainServiceManager = mainServiceManager;
        }

        #region "Search"

        [HttpPost("Search")]
        public async Task<IActionResult> Search(PurTransactionPOFilterDto filter)
        {
            var chk = await permission.HasPermission(1514, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var branchsId = session.Branches.Split(',');
                filter.FacilityId ??= 0;
                filter.BranchId ??= 0;
                filter.CusTypeId ??= 0;
                filter.ProjectCode ??= 0;
                filter.PaymentTermsId ??= 0;
                filter.Subtotal ??= 0;
                filter.InventoriesId ??= 0;
                filter.IsConverted ??= 0;
                filter.CreatedBy ??= 0;
                long FacilityId = session.FacilityId;

                var items = await purServiceManager.PurTransactionService.GetAllVW(x =>
                    x.IsDeleted == false &&
                    x.TransTypeId == 2 &&
                    (x.FacilityId == FacilityId) &&
                    (filter.BranchId == 0 || (x.BranchId == filter.BranchId)) &&
                    ((filter.BranchId == 0) || branchsId.Contains(x.BranchId.ToString())) &&
                    (string.IsNullOrEmpty(filter.Code) || x.Code == filter.Code) &&
                    (string.IsNullOrEmpty(filter.RefNumber) || x.RefNumber == filter.RefNumber) &&
                    (filter.CusTypeId == 0 || x.CusTypeId == filter.CusTypeId) &&
                    (filter.ProjectCode == 0 || x.ProjectCode == filter.ProjectCode) &&
                    (string.IsNullOrEmpty(filter.ProjectName) || x.ProjectName == filter.ProjectName) &&
                    (string.IsNullOrEmpty(filter.SupplierCode) || x.SupplierCode == filter.SupplierCode) &&
                    (string.IsNullOrEmpty(filter.SupplierName) || x.SupplierName == filter.SupplierName) &&
                    (filter.PaymentTermsId == 0 || x.PaymentTermsId == filter.PaymentTermsId) &&
                    (filter.Subtotal == 0 || x.Subtotal == filter.Subtotal) &&
                    (filter.InventoriesId == 0 || x.InventoriesId == filter.InventoriesId) &&
                    (filter.IsConverted == 0 || x.StatusId == filter.IsConverted) &&
                    (filter.CreatedBy == 0 || x.CreatedBy == filter.CreatedBy)
                );
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    if (!string.IsNullOrEmpty(filter.StartDate))
                    {
                        DateTime startDate = DateTime.ParseExact(filter.StartDate, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                        res = res.Where(s => DateTime.ParseExact(s.Date1, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= startDate);
                    }

                    if (!string.IsNullOrEmpty(filter.EndDate))
                    {
                        DateTime endDate = DateTime.ParseExact(filter.EndDate, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                        res = res.Where(s => DateTime.ParseExact(s.Date1, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= endDate);
                    }
                    var list = res.ToList();
                    return Ok(await Result<List<PurTransactionsVw>>.SuccessAsync(list, $"count = {list.Count}"));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<PurTransactionPOFilterDto>.FailAsync($"======= Exp in Search MESSAGE: {ex.Message}"));
            }
        }

        //[HttpPost("Search")]
        //public async Task<IActionResult> Search(PurTransactionPOFilterDto filter)
        //{
        //    var hasPermission = await permission.HasPermission(1514, PermissionType.Show);
        //    if (!hasPermission)
        //    {
        //        return Ok(await Result.AccessDenied("AccessDenied"));
        //    }

        //    try
        //    {
        //        var branchIds = session.Branches.Split(',');
        //        long facilityId = session.FacilityId;

        //        filter.FacilityId ??= facilityId;
        //        filter.BranchId ??= 0;
        //        filter.PaymentTermsId ??= 0;
        //        filter.ProjectCode ??= 0;
        //        filter.Subtotal ??= 0;
        //        filter.InventoriesId ??= 0;
        //        filter.IsConverted ??= 0;

        //        var items = await purServiceManager.PurTransactionService.GetAllVW(x =>
        //            x.IsDeleted == false &&
        //            x.TransTypeId == 2 &&
        //            (x.FacilityId == facilityId) &&
        //            (filter.BranchId == 0 || x.BranchId == filter.BranchId || branchIds.Contains(x.BranchId.ToString())) &&
        //            (string.IsNullOrEmpty(filter.Code) || x.Code == filter.Code) &&
        //            (string.IsNullOrEmpty(filter.RefNumber) || x.RefNumber == filter.RefNumber) &&
        //            (filter.ProjectCode == 0 || x.ProjectCode == filter.ProjectCode) &&
        //            (string.IsNullOrEmpty(filter.ProjectName) || x.ProjectName.Contains(filter.ProjectName)) &&
        //            (string.IsNullOrEmpty(filter.SupplierCode) || x.SupplierCode == filter.SupplierCode) &&
        //            (string.IsNullOrEmpty(filter.SupplierName) || x.SupplierName.Contains(filter.SupplierName)) &&
        //            (filter.PaymentTermsId == 0 || x.PaymentTermsId == filter.PaymentTermsId) &&
        //            (filter.Subtotal == 0 || x.Subtotal == filter.Subtotal) &&
        //            (filter.InventoriesId == 0 || x.InventoriesId == filter.InventoriesId) &&
        //            (filter.IsConverted == 0 ||
        //                (filter.IsConverted == 1 && !purServiceManager.PurTransactionService.HasRefrence(x.Id)) ||
        //                (filter.IsConverted == 2 && purServiceManager.PurTransactionService.HasRefrence(x.Id)))
        //        );

        //        if (items.Succeeded)
        //        {
        //            // Additional date range filtering
        //            var query = items.Data.AsQueryable();

        //            if (!string.IsNullOrEmpty(filter.StartDate))
        //            {
        //                DateTime startDate = DateTime.ParseExact(filter.StartDate, "yyyy/MM/dd", CultureInfo.InvariantCulture);
        //                query = query.Where(x => DateTime.ParseExact(x.Date1, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= startDate);
        //            }

        //            if (!string.IsNullOrEmpty(filter.EndDate))
        //            {
        //                DateTime endDate = DateTime.ParseExact(filter.EndDate, "yyyy/MM/dd", CultureInfo.InvariantCulture);
        //                query = query.Where(x => DateTime.ParseExact(x.Date1, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= endDate);
        //            }

        //            // Supply date range filtering
        //            if (!string.IsNullOrEmpty(filter.SupplyStartDate) && !string.IsNullOrEmpty(filter.SupplyEndDate))
        //            {
        //                DateTime supplyStartDate = DateTime.ParseExact(filter.SupplyStartDate, "yyyy/MM/dd", CultureInfo.InvariantCulture);
        //                DateTime supplyEndDate = DateTime.ParseExact(filter.SupplyEndDate, "yyyy/MM/dd", CultureInfo.InvariantCulture);
        //                query = query.Where(x =>
        //                    x.SupplyStartDate != null &&
        //                    DateTime.ParseExact(x.SupplyStartDate, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= supplyStartDate &&
        //                    x.SupplyEndDate != null &&
        //                    DateTime.ParseExact(x.SupplyEndDate, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= supplyEndDate);
        //            }

        //            var result = query.ToList();
        //            return Ok(await Result<List<PurTransactionsVw>>.SuccessAsync(result, $"count = {result.Count}"));
        //        }

        //        return Ok(items);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle exceptions
        //        return Ok(await Result.FailAsync($"Search failed with message: {ex.Message}"));
        //    }
        //}
        #endregion "Search"

        #region "Add - Edit - AddMultiInventories"
        [HttpPost("Add")]
        public async Task<IActionResult> Add(PurTransactionPODto obj)
        {
            try
            {
                var chk = await permission.HasPermission(280, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync(localization.GetMessagesResource("dataRequire")));

                if (obj.Products.Count <= 0)
                {
                    return Ok(await Result.FailAsync("قم بإضافة الأصناف اولاُ"));
                }
                else
                {
                    foreach (var product in obj.Products)
                    {
                        product.ProductId ??= 0;
                        product.Qty ??= 0;
                        if (string.IsNullOrEmpty(product.ProductCode))
                            return Ok(await Result.FailAsync($"-قم بإضافة الأصناف اولاً"));
                        if (product.Qty <= 0)
                            return Ok(await Result.FailAsync($"-حدد الكمية اولاً"));
                    }
                }
                var add = await purServiceManager.PurTransactionService.AddPurchaseOrder(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("AddMultiInventories")]
        public async Task<IActionResult> AddMultiInventories(PurTransactionPODto obj)
        {
            try
            {
                var chk = await permission.HasPermission(280, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync(localization.GetMessagesResource("dataRequire")));

                if (obj.Products.Count <= 0)
                {
                    return Ok(await Result.FailAsync("قم بإضافة الأصناف اولاُ"));
                }
                else
                {
                    foreach (var product in obj.Products)
                    {
                        product.ProductId ??= 0;
                        product.Qty ??= 0;
                        if (string.IsNullOrEmpty(product.ProductCode))
                            return Ok(await Result.FailAsync($"-قم بإضافة الأصناف اولاً"));
                        if (product.Qty <= 0)
                            return Ok(await Result.FailAsync($"-حدد الكمية اولاً"));
                    }
                }
                var add = await purServiceManager.PurTransactionService.AddPurchaseOrder(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add, MESSAGE: {ex.Message}"));
            }
        }
        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(PurTransactionEditPODto obj)
        {
            var chk = await permission.HasPermission(280, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<PurTransactionEditPODto>.FailAsync(localization.GetMessagesResource("dataRequire")));
                }

                var Edit = await purServiceManager.PurTransactionService.UpdatePurcaseOrder(obj);
                return Ok(Edit);
            }
            catch (Exception ex)
            {
                return Ok(await Result<PurTransactionEditPODto>.FailAsync($"======= Exp in edit: {ex.Message}"));
            }
        }
        #endregion "Add - Edit - AddMultiInventories"

        #region "Delete"
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long Id)
        {
            var chk = await permission.HasPermission(280, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var add = await purServiceManager.PurTransactionService.Remove(Id);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<PurTransactionDto>.FailAsync($"======= Exp in Delete: {ex.Message}"));
            }
        }
        #endregion "Delete"

        #region "GetByIdForEdit - GetById - GetProductsByPOrderId - GetFilesByPOrderId"

        [HttpGet("GetByIdForEdit")]
        public async Task<IActionResult> GetByIdForEdit(long id)
        {
            try
            {
                var chk = await permission.HasPermission(280, PermissionType.Show);
                if (!chk)
                {
                    return Ok(await Result.AccessDenied("AccessDenied"));
                }

                if (id <= 0)
                {
                    return Ok(await Result<PurTransactionPODto>.FailAsync(localization.GetMessagesResource("InCorrectId")));
                }
                var obj = new PurTransactionPOVWDto();
                var getItem = await purServiceManager.PurTransactionService.GetOneVW(x => x.Id == id);
                if (getItem.Succeeded)
                {
                    obj.PurTransactions = getItem.Data;

                    var itemProducts = await purServiceManager.PurTransactionsProductService.GetAllVW(x => x.IsDeleted == false && x.TransactionId == id);

                    obj.Products = itemProducts.Data.Select(product => new PurTransactionsProductPODto
                    {
                        Id = product.Id,
                        TransactionId = product.TransactionId,
                        ProductCode = product.ItemCode,
                        ProductId = product.ProductId,
                        UnitId = product.UnitId,
                        Price = product.Price,
                        Qty = product.Qty,
                        DiscRate = product.DiscRate,
                        DiscountAmount = product.DiscountAmount,
                        Total = product.Total,
                        Vat = product.Vat,
                        BranchId = product.BranchId,
                        Desc = product.Desc,
                        IsDeleted = product.IsDeleted,
                        CcId = product.CcId,
                        CurrencyId = product.CurrencyId,
                        ExchangeRate = product.ExchangeRate,
                        VatAmount = product.VatAmount,
                        PurTId = product.PurTId,
                        PurPId = product.PurPId,
                        InventoryId = product.InventoryId
                    }).ToList();

                    //obj.Products = itemProducts.Data.ToList();

                    return Ok(await Result<PurTransactionPOVWDto>.SuccessAsync(obj, $""));
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
                var chk = await permission.HasPermission(280, PermissionType.Show);
                if (!chk)
                {
                    return Ok(await Result.AccessDenied("AcessDenied"));
                }

                if (id <= 0)
                {
                    return Ok(await Result<PurTransactionDto>.FailAsync(localization.GetMessagesResource("InCorrectId")));
                }

                var getItem = await purServiceManager.PurTransactionService.GetOne(s => s.Id == id && s.IsDeleted == false);
                if (getItem.Succeeded)
                {
                    var obj = getItem.Data;
                    return Ok(await Result<PurTransactionDto>.SuccessAsync(obj, $""));
                }
                else
                {
                    return Ok(getItem);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<PurTransactionDto>.FailAsync($"====== Exp MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetProductsByPOrderId")]
        public async Task<IActionResult> GetProductsByPOrderId(long id)
        {
            try
            {
                var chk = await permission.HasPermission(280, PermissionType.Show);
                if (!chk)
                {
                    return Ok(await Result.AccessDenied("AccessDenied"));
                }

                if (id <= 0)
                {
                    return Ok(await Result<PurTransactionsProductDto>.FailAsync(localization.GetMessagesResource("InCorrectId")));
                }

                var getItem = await purServiceManager.PurTransactionsProductService.GetAll(x => x.TransactionId == id && x.IsDeleted == false);
                if (getItem.Succeeded)
                {
                    var obj = getItem.Data;
                    return Ok(await Result<List<PurTransactionsProductDto>>.SuccessAsync(obj.ToList(), $""));
                }
                else
                {
                    return Ok(getItem);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<List<PurTransactionsProductDto>>.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetFilesByPOrderId")]
        public async Task<IActionResult> GetFilesByPOrderId(long id)
        {
            try
            {
                var chk = await permission.HasPermission(280, PermissionType.Show);
                if (!chk)
                {
                    return Ok(await Result.AccessDenied("AccessDenied"));
                }

                if (id <= 0)
                {
                    return Ok(await Result<SysFileDto>.FailAsync(localization.GetMessagesResource("InCorrectId")));
                }

                var getItem = await mainServiceManager.SysFileService.GetAll(x => x.PrimaryKey == id && x.TableId == 28 && x.IsDeleted == false);
                if (getItem.Succeeded)
                {
                    var obj = getItem.Data;
                    return Ok(await Result<List<SysFileDto>>.SuccessAsync(obj.ToList(), $""));
                }
                else
                {
                    return Ok(getItem);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<List<SysFileDto>>.FailAsync($"====== Exp in GetQtyDetailsByCatalogId Pur , MESSAGE: {ex.Message}"));
            }
        }

        #endregion "GetByIdForEdit - GetById - GetProductsByPOrderId - GetFilesByPOrderId"
    }
}
