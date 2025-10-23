using DocumentFormat.OpenXml.Wordprocessing;
using Logix.Application.Common;
using Logix.Application.DTOs.Main;
using Logix.Application.DTOs.PUR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.PUR;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Evaluation;
using System.Globalization;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using YamlDotNet.Serialization;
using Logix.Application.DTOs.ACC;
using Logix.Application.Services;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Logix.Infrastructure.Repositories;

namespace Logix.MVC.LogixAPIs.PUR
{
    public class PurCreditMemoController : BasePurApiController
    {
        private readonly IPurServiceManager purServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IMainServiceManager mainServiceManager;

        public PurCreditMemoController(
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

        #region "GetAll - Search - GetCreditMemoTransByBillCode"

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var chk = await permission.HasPermission(403, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var items = await purServiceManager.PurTransactionsDiscountService.GetAll(x => x.IsDeleted == false);
                if (items.Succeeded)
                {
                    return Ok(await Result<List<PurTransactionsDiscountDto>>.SuccessAsync(items.Data.ToList(), ""));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }
        [HttpPost("Search")]
        public async Task<IActionResult> Search(PurTransactionsDiscountCMFilterDto filter)
        {
            var chk = await permission.HasPermission(403, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var branchsId = session.Branches.Split(',');
                filter.FacilityId ??= 0;
                filter.BranchId ??= 0;
                filter.ProjectCode ??= 0;
                filter.DiscountAmount ??= 0;
                filter.CreatedBy ??= 0;
                long FacilityId = session.FacilityId;
        
                var items = await purServiceManager.PurTransactionsDiscountService.GetAllVW(x =>
                    x.IsDeleted == false &&
                    (filter.BranchId == 0 || (x.BranchId == filter.BranchId)) &&
                    ((filter.BranchId == 0) || branchsId.Contains(x.BranchId.ToString())) &&
                    (x.FacilityId == FacilityId) &&
                    (string.IsNullOrEmpty(filter.BillCode) || x.BillCode == filter.BillCode) &&
                    (string.IsNullOrEmpty(filter.Code) || x.Code == filter.Code) &&
                    (string.IsNullOrEmpty(filter.SupplierCode) || x.SupplierCode == filter.SupplierCode) &&
                    (string.IsNullOrEmpty(filter.SupplierName) || x.SupplierName == filter.SupplierName) &&
                    (filter.ProjectCode == 0 || x.ProjectCode == filter.ProjectCode) &&
                    (filter.DiscountAmount == 0 || x.DiscountAmount == filter.DiscountAmount)
                );
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    if (!string.IsNullOrEmpty(filter.StartDate))
                    {
                        DateTime startDate = DateTime.ParseExact(filter.StartDate, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                        res = res.Where(s => DateTime.ParseExact(s.DiscountDate, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= startDate);
                    }

                    if (!string.IsNullOrEmpty(filter.EndDate))
                    {
                        DateTime endDate = DateTime.ParseExact(filter.EndDate, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                        res = res.Where(s => DateTime.ParseExact(s.DiscountDate, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= endDate);
                    }
                    var list = res.ToList();
                    return Ok(await Result<List<PurTransactionsDiscountVw>>.SuccessAsync(list, $"count = {list.Count}"));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<PurTransactionsDiscountCMFilterDto>.FailAsync($"======= Exp in Search MESSAGE: {ex.Message}"));
            }
        }
        
        [HttpGet("GetCreditMemoTransByBillCode")]
        public async Task<IActionResult> GetCreditMemoTransByBillCode(string BillCode)
        {
            try
            {
                var obj = new PurTransactionCMDto();
                decimal amount = 0;
                var BillData = await purServiceManager.PurTransactionService.GetOneVW(x =>
                    x.IsDeleted == false &&
                    x.FacilityId == session.FacilityId &&
                    (x.Code != null && x.Code.Equals(BillCode))
                );

                if (BillData.Succeeded && BillData.Data != null)
                {
                    var ActiveCCIDCreditData = await mainServiceManager.SysPropertyValueService.GetByProperty(session.FacilityId, 57);

                    if (ActiveCCIDCreditData.Data.PropertyValue == "1")
                    {
                        amount = BillData.Data.Total ?? 0;
                    }
                    else
                    {
                        amount = BillData.Data.Subtotal ?? 0;
                    }
                    obj = new PurTransactionCMDto
                    {
                        Total = BillData.Data.Total,
                        Subtotal = BillData.Data.NewSubtotal,
                        CurrencyId = BillData.Data.CurrencyId,
                        SupplierCode = BillData.Data.SupplierCode,
                        SupplierName = BillData.Data.SupplierName,
                        ExchangeRate = BillData.Data.ExchangeRate,
                        Vat = BillData.Data.Vat,
                        Date1 = BillData.Data.Date1,
                        DiscountAmount = BillData.Data.DiscountAmount,
                        DiscountNotice = BillData.Data.DiscountAmount1,
                        AmountDiscount = (BillData.Data.Total - BillData.Data.DiscountAmount1)??0,
                        BranchId = BillData.Data.BranchId,
                        Id = BillData.Data.Id,
                    };
                    var products = await purServiceManager.PurTransactionsProductService.GetAllVW(x => x.TransactionId == BillData.Data.Id);
                    if (products.Data.Count() > 0) 
                    { 
                        BillData.Data.Vat = products.Data.FirstOrDefault().Vat;
                    }
                    return Ok(await Result<PurTransactionCMDto>.SuccessAsync(obj, $""));
                }
                return Ok(obj);
            }
            catch (Exception exp)
            {
                return Ok(await Result<PurTransactionCMDto>.FailAsync($"Exception Message: {exp.Message}"));
            }
        }

        #endregion "GetAll - Search - GetCreditMemoTransByBillCode"

        #region "Add - Edit"
        [HttpPost("Add")]
        public async Task<IActionResult> Add(PurTransactionsDiscountDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(403, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync(localization.GetMessagesResource("dataRequire")));

                var add = await purServiceManager.PurTransactionsDiscountService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(PurTransactionsDiscountEditDto obj)
        {
            var chk = await permission.HasPermission(403, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<PurTransactionsDiscountEditDto>.FailAsync(localization.GetMessagesResource("dataRequire")));
                }

                var Edit = await purServiceManager.PurTransactionsDiscountService.Update(obj);
                return Ok(Edit);
            }
            catch (Exception ex)
            {
                return Ok(await Result<PurTransactionsDiscountEditDto>.FailAsync($"======= Exp in edit: {ex.Message}"));
            }
        }
        #endregion "Add - Edit"

        #region "Delete"
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long Id)
        {
            var chk = await permission.HasPermission(403, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var del = await purServiceManager.PurTransactionsDiscountService.Remove(Id);
                return Ok(del);
            }
            catch (Exception ex)
            {
                return Ok(await Result<PurTransactionsDiscountDto>.FailAsync($"======= Exp in Delete: {ex.Message}"));
            }
        }
        #endregion "Delete"

        #region "GetByIdForEdit - GetById - GetCreditMemoByPurTransDiscountId"

        [HttpGet("GetByIdForEdit")]
        public async Task<IActionResult> GetByIdForEdit(long id)
        {
            try
            {
                var chk = await permission.HasPermission(403, PermissionType.Show);
                if (!chk)
                {
                    return Ok(await Result.AccessDenied("AccessDenied"));
                }

                if (id <= 0)
                {
                    return Ok(await Result<PurTransactionsDiscountEditDto>.FailAsync(localization.GetMessagesResource("InCorrectId")));
                }

                var getItem = await purServiceManager.PurTransactionsDiscountService.GetForUpdate<PurTransactionsDiscountEditDto>(id);
                if (getItem.Succeeded)
                {
                    return Ok(await Result<PurTransactionsDiscountEditDto>.SuccessAsync(getItem.Data, $""));
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
                var chk = await permission.HasPermission(403, PermissionType.Show);
                if (!chk)
                {
                    return Ok(await Result.AccessDenied("AcessDenied"));
                }

                if (id <= 0)
                {
                    return Ok(await Result<PurTransactionsDiscountDto>.FailAsync(localization.GetMessagesResource("NoIdInUpdate")));
                }

                var getItem = await purServiceManager.PurTransactionsDiscountService.GetOne(s => s.Id == id && s.IsDeleted == false);
                if (getItem.Succeeded)
                {
                    var obj = getItem.Data;
                    return Ok(await Result<PurTransactionsDiscountDto>.SuccessAsync(obj, $""));
                }
                else
                {
                    return Ok(getItem);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<PurTransactionsDiscountDto>.FailAsync($"====== Exp MESSAGE: {ex.Message}"));
            }
        }
        [HttpGet("GetCreditMemoByPurTransDiscountId")]
        public async Task<IActionResult> GetCreditMemoByPurTransDiscountId(long id)
        {
            try
            {
                var chk = await permission.HasPermission(403, PermissionType.Show);
                if (!chk)
                {
                    return Ok(await Result.AccessDenied("AccessDenied"));
                }

                if (id <= 0)
                {
                    return Ok(await Result<PurTransactionsDiscountDto>.FailAsync(localization.GetMessagesResource("InCorrectId")));
                }

                var getItem = await purServiceManager.PurTransactionsDiscountService.GetAll(x => x.TransactionId == id && x.IsDeleted == false);
                if (getItem.Succeeded)
                {
                    var obj = getItem.Data;
                    return Ok(await Result<List<PurTransactionsDiscountDto>>.SuccessAsync(obj.ToList(), $""));
                }
                else
                {
                    return Ok(getItem);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<List<PurTransactionsDiscountDto>>.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }

        #endregion "GetByIdForEdit - GetById - GetCreditMemoPurTransDiscountId"
    }
}
