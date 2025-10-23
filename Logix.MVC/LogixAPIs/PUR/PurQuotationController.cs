using Logix.Application.Common;
using Logix.Application.DTOs.Main;
using Logix.Application.DTOs.PUR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Services;
using Logix.Application.Wrapper;
using Logix.Domain.PUR;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Linq;

namespace Logix.MVC.LogixAPIs.PUR
{
    public class PurQuotationController : BasePurApiController
    {
        private readonly IPurServiceManager purServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IMainServiceManager mainServiceManager;

        public PurQuotationController(
            IPurServiceManager purServiceManager,
            IPermissionHelper permission,
            ICurrentData session,
            ILocalizationService localization,
            IMainServiceManager mainServiceManager
            )
        {
            this.purServiceManager = purServiceManager;
            this.permission = permission;
            this.session = session;
            this.localization = localization;
            this.mainServiceManager = mainServiceManager;
        }
        #region "GetAll - Search"
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var chk = await permission.HasPermission(279, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var items = await purServiceManager.PurTransactionService.GetAll(x => x.IsDeleted == false);
                if (items.Succeeded)
                {
                    return Ok(await Result<List<PurTransactionDto>>.SuccessAsync(items.Data.ToList(), ""));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }
        [HttpPost("Search")]
        public async Task<IActionResult> Search(PurTransactionFilterDto filter)
        {
            var chk = await permission.HasPermission(279, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var branchsId = session.Branches.Split(',');
                filter.BranchId ??= 0;
                filter.InventoryId ??= 0;
                filter.CusTypeId ??= 0;
                filter.ProjectCode ??= 0;
                filter.FacilityId ??= 0;
                long FacilityId = session.FacilityId;
                filter.ConvertStatus ??= 0;
                filter.PaymentTermsId ??= 0;
                filter.Subtotal ??= 0;
                var items = await purServiceManager.PurTransactionService.GetAllVW(x =>
                    x.IsDeleted == false &&
                    x.TransTypeId == 3 &&
                    (filter.FacilityId == 0 || x.FacilityId == FacilityId) &&
                    (filter.BranchId == 0 || (x.BranchId == filter.BranchId)) &&
                    ((filter.BranchId == 0) || branchsId.Contains(x.BranchId.ToString())) &&
                    (filter.CusTypeId == 0 || x.CusTypeId == filter.CusTypeId) &&
                    (string.IsNullOrEmpty(filter.Code) || x.Code == filter.Code) &&
                    (string.IsNullOrEmpty(filter.RefNumber) || x.RefNumber == filter.RefNumber) &&
                    (filter.ProjectCode == 0 || x.ProjectCode == filter.ProjectCode) &&
                    (string.IsNullOrEmpty(filter.ProjectName) || x.ProjectName == filter.ProjectName) &&
                    (string.IsNullOrEmpty(filter.SupplierCode) || x.SupplierCode == filter.SupplierCode) &&
                    (string.IsNullOrEmpty(filter.SupplierName) || x.SupplierName == filter.SupplierName) &&
                    (filter.PaymentTermsId == 0 || x.PaymentTermsId == filter.PaymentTermsId) &&
                    (filter.Subtotal == 0 || x.Subtotal == filter.Subtotal) &&
                    (filter.InventoryId == 0 || x.InventoriesId == filter.InventoryId) &&
                    (filter.ConvertStatus == 0 || x.StatusId == filter.ConvertStatus)
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
                return Ok(await Result<PurTransactionFilterDto>.FailAsync($"======= Exp in Search MESSAGE: {ex.Message}"));
            }
        }
        #endregion "GetAll - Search"

        #region "Add - Edit"
        [HttpPost("Add")]
        public async Task<IActionResult> Add(PurTransactionQDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(279, PermissionType.Add);
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
                var add = await purServiceManager.PurTransactionService.AddQuotation(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add, MESSAGE: {ex.Message}"));
            }
        }
        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(PurTransactionEditQDto obj)
        {
            var chk = await permission.HasPermission(279, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<PurTransactionEditQDto>.FailAsync(localization.GetMessagesResource("dataRequire")));
                }

                var Edit = await purServiceManager.PurTransactionService.UpdateQuotation(obj);
                return Ok(Edit);
            }
            catch (Exception ex)
            {
                return Ok(await Result<PurTransactionEditQDto>.FailAsync($"======= Exp in edit: {ex.Message}"));
            }
        }
        #endregion "Add - Edit"

        #region "Delete"
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long Id)
        {
            var chk = await permission.HasPermission(279, PermissionType.Delete);
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

        #region "GetByIdForEdit - GetById - PurTransactionsProductDto - GetFilesByQuotationId"

        [HttpGet("GetByIdForEdit")]
        public async Task<IActionResult> GetByIdForEdit(long id)
        {
            try
            {
                var chk = await permission.HasPermission(279, PermissionType.Show);
                if (!chk)
                {
                    return Ok(await Result.AccessDenied("AccessDenied"));
                }

                if (id <= 0)
                {
                    return Ok(await Result<PurTransactionQDto>.FailAsync(localization.GetMessagesResource("InCorrectId")));
                }

                var getItem = await purServiceManager.PurTransactionService.GetForUpdate<PurTransactionQDto>(id);
                if (getItem.Succeeded)
                {
                    var itemProducts = await purServiceManager.PurTransactionsProductService.GetAll(x => x.IsDeleted == false && x.TransactionId == id);
                    getItem.Data.Products = itemProducts.Data.ToList();
                    var files = await mainServiceManager.SysFileService.GetAll(x => x.IsDeleted == false && x.PrimaryKey == id && x.TableId == 28);

                    getItem.Data.FileDtos = files.Data.Select(file => new SaveFileDto
                    {
                        Id = file.Id,
                        FileURL = file.FileUrl ?? string.Empty,
                        FileName = file.FileName ?? string.Empty,
                        IsDeleted = file.IsDeleted,
                        FileDate = file.FileDate
                    }).ToList();
                    return Ok(await Result<PurTransactionQDto>.SuccessAsync(getItem.Data, $""));
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
                var chk = await permission.HasPermission(279, PermissionType.Show);
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

        [HttpGet("GetProductsByQuotationId")]
        public async Task<IActionResult> GetProductsByQuotationId(long id)
        {
            try
            {
                var chk = await permission.HasPermission(279, PermissionType.Show);
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

        [HttpGet("GetFilesByQuotationId")]
        public async Task<IActionResult> GetFilesByQuotationId(long id)
        {
            try
            {
                var chk = await permission.HasPermission(279, PermissionType.Show);
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

        #endregion "GetByIdForEdit - GetById - GetProductsByQuotationId - GetFilesByQuotationId"
    }
}
