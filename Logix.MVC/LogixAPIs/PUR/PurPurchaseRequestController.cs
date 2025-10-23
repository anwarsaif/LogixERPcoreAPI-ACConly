using Logix.Application.Common;
using Logix.Application.DTOs.Main;
using Logix.Application.DTOs.PUR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Services;
using Logix.Application.Wrapper;
using Logix.Domain.PUR;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Data;
using System.Globalization;

namespace Logix.MVC.LogixAPIs.PUR
{
    public class PurPurchaseRequestController : BasePurApiController
    {
        private readonly IPurServiceManager purServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly IMainServiceManager mainServiceManager;
        private readonly ILocalizationService localization;

        public PurPurchaseRequestController(IPurServiceManager purServiceManager,
            IPermissionHelper permission,
            ICurrentData CurrentData,
            ICurrentData session,
            IMainServiceManager mainServiceManager,
            ILocalizationService localization)
        {
            this.purServiceManager = purServiceManager;
            this.permission = permission;
            this.session = session;
            this.mainServiceManager = mainServiceManager;
            this.localization = localization;
        }
        #region "GetAll - Search"
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var chk = await permission.HasPermission(398, PermissionType.Show);
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
            var chk = await permission.HasPermission(398, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                filter.BranchId ??= 0;
                filter.InventoryId ??= 0;
                long FacilityId = session.FacilityId;
                filter.ProjectCode ??= 0;
                //filter.EmpId ??= 0;
                filter.ConvertStatus ??= 0;
                filter.RequestStatus ??= 0;
                //  حقول غير موجودة في الشاشة
                //filter.CusTypeId ??= 0;
                //filter.PaymentTermsId ??= 0;
                //filter.Subtotal ??= 0;
                var items = await purServiceManager.PurTransactionService.GetAllVW(x =>
                    x.IsDeleted == false &&
                    x.TransTypeId == 6 &&
                    (string.IsNullOrEmpty(filter.Code) || x.Code == filter.Code) &&
                    //(string.IsNullOrEmpty(filter.DeliveryDate) || x.DeliveryDate == filter.DeliveryDate) &&
                    //(string.IsNullOrEmpty(filter.ExpirationDate) || x.ExpirationDate == filter.ExpirationDate) &&
                    (filter.BranchId == 0 || x.BranchId == filter.BranchId) &&
                    (filter.InventoryId == 0 || x.InventoriesId == filter.InventoryId) &&
                    (filter.ConvertStatus == 0 || x.StatusId == filter.ConvertStatus) &&
                    (filter.RequestStatus == 0 || x.RfqStutesId == filter.RequestStatus) &&
                    (filter.ProjectCode == 0 || x.ProjectCode == filter.ProjectCode) &&
                    (string.IsNullOrEmpty(filter.ProjectName) || x.ProjectName == filter.ProjectName) &&
                    (FacilityId == 0 || x.FacilityId == FacilityId) &&
                    (string.IsNullOrEmpty(filter.EmpCode) || x.EmpCode == filter.EmpCode)
                // حقول غير موجودة في الشاشة
                //(filter.CusTypeId == 0 || x.CusTypeId == filter.CusTypeId) &&
                //(string.IsNullOrEmpty(filter.SupplierCode) || x.SupplierCode == filter.SupplierCode) &&
                //(string.IsNullOrEmpty(filter.SupplierName) || x.SupplierName == filter.SupplierName) &&
                //(filter.PaymentTermsId == 0 || x.PaymentTermsId == filter.PaymentTermsId) &&
                //(filter.Subtotal == 0 || x.Subtotal == filter.Subtotal) &&
                //(string.IsNullOrEmpty(filter.RefNumber) || x.RefNumber == filter.RefNumber)

                );
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    if (!string.IsNullOrEmpty(filter.ItemCode))
                    {
                        var productData = await purServiceManager.PurTransactionsProductService.GetAllVW(x => x.IsDeleted == false && x.TransTypeId == 6);

                        res = res.Where(x => productData.Data.Any(s => s.TransactionId == x.Id && s.ItemCode != null && s.ItemCode.Contains(filter.ItemCode)));
                    };

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
        public async Task<IActionResult> Add(PurTransactionPRDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(398, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync(localization.GetMessagesResource("dataRequire")));

                if (string.IsNullOrEmpty(obj.Subject))
                    return Ok(await Result.FailAsync($"- يرجى إضافة عنوان الطلب"));
                if (string.IsNullOrEmpty(obj.DeliveryAddress))
                    return Ok(await Result.FailAsync($"- يرجى إضافة مكان التنفيذ او التوريد"));
                if (string.IsNullOrEmpty(obj.DeliveryAddress))
                    return Ok(await Result.FailAsync($"- يرجى إضافة تاريخ بداية التنفيذ او التوريد"));
                if (string.IsNullOrEmpty(obj.ExpirationDate))
                    return Ok(await Result.FailAsync($"- يرجى إضافة تاريخ نهاية التنفيذ او التوريد"));

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
                        if (product.ProductId == 0)
                            return Ok(await Result.FailAsync($"-قم بإضافة الأصناف اولاً"));
                        if (product.Qty <= 0)
                            return Ok(await Result.FailAsync($"-حدد الكمية اولاً"));
                    }
                }
                var add = await purServiceManager.PurTransactionService.AddPurchaseRequest(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add PurExpenseController, MESSAGE: {ex.Message}"));
            }
        }
        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(PurTransactionEditPRDto obj)
        {
            var chk = await permission.HasPermission(398, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<PurTransactionEditPRDto>.FailAsync(localization.GetMessagesResource("dataRequire")));
                }

                var Edit = await purServiceManager.PurTransactionService.UpdatePurchaseRequest(obj);
                return Ok(Edit);
            }
            catch (Exception ex)
            {
                return Ok(await Result<PurTransactionEditPRDto>.FailAsync($"======= Exp in edit: {ex.Message}"));
            }
        }
        #endregion "Add - Edit"

        #region "Delete"
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long Id)
        {
            var chk = await permission.HasPermission(398, PermissionType.Delete);
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
                return Ok(await Result<PurTransactionPRDto>.FailAsync($"======= Exp in Delete: {ex.Message}"));
            }
        }
        #endregion "Delete"

        #region "GetByIdForEdit - GetById"

        [HttpGet("GetByIdForEdit")]
        public async Task<IActionResult> GetByIdForEdit(long id)
        {
            try
            {
                var chk = await permission.HasPermission(398, PermissionType.Show);
                if (!chk)
                {
                    return Ok(await Result.AccessDenied("AccessDenied"));
                }

                if (id <= 0)
                {
                    return Ok(await Result<PurTransactionEditDto>.FailAsync(localization.GetMessagesResource("InCorrectId")));
                }

                var getItem = await purServiceManager.PurTransactionService.GetForUpdate<PurTransactionEditDto>(id);
                if (getItem.Succeeded)
                {
                    //var products = await purServiceManager.PurTransactionsProductService.GetAll(x => x.IsDeleted == false && x.TransactionId == id && x.ProductId != 0 && x.BranchId != 0);
                    var products = await purServiceManager.PurTransactionsProductService.GetAll(x => x.IsDeleted == false && x.TransactionId == id && x.ProductId != 0);
                    getItem.Data.Products = products.Data.ToList();
                    var suppliers = await purServiceManager.PurTransactionsSupplierService.GetAll(x => x.IsDeleted == false && x.TransactionId == id && x.SupplierId != 0);
                    getItem.Data.Suppliers = suppliers.Data.ToList();
                    var files = await mainServiceManager.SysFileService.GetAll(x => x.IsDeleted == false && x.PrimaryKey == id && x.TableId == 19);

                    getItem.Data.FileDtos = files.Data.Select(file => new SaveFileDto
                    {
                        Id = file.Id,
                        FileURL = file.FileUrl ?? string.Empty,
                        FileName = file.FileName ?? string.Empty,
                        IsDeleted = file.IsDeleted,
                        FileDate = file.FileDate
                    }).ToList();

                    return Ok(await Result<PurTransactionEditDto>.SuccessAsync(getItem.Data, $""));
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
                var chk = await permission.HasPermission(398, PermissionType.Show);
                if (!chk)
                {
                    return Ok(await Result.AccessDenied("AcessDenied"));
                }

                if (id <= 0)
                {
                    return Ok(await Result<PurTransactionDto>.FailAsync(localization.GetMessagesResource("NoIdInUpdate")));
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

        #endregion "GetByIdForEdit - GetById"
    }
}
