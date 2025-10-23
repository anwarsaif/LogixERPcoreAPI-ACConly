using System.Globalization;
using Logix.Application.Common;
using Logix.Application.DTOs.FXA;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.FXA;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.FXA
{
    public class FxaPurchaseController : BaseFxaApiController
    {
        private readonly IFxaServiceManager fxaServiceManager;
        private readonly IAccServiceManager accServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly ISysConfigurationHelper configurationHelper;

        public FxaPurchaseController(IFxaServiceManager fxaServiceManager,
            IAccServiceManager accServiceManager,
            IPermissionHelper permission,
            ICurrentData session,
            ILocalizationService localization,
            ISysConfigurationHelper configurationHelper)
        {
            this.fxaServiceManager = fxaServiceManager;
            this.accServiceManager = accServiceManager;
            this.permission = permission;
            this.session = session;
            this.localization = localization;
            this.configurationHelper = configurationHelper;
        }

        [HttpPost("Search")]
        public async Task<IActionResult> Search(FxaTransactionFilterDto filter)
        {
            try
            {
                var chk = await permission.HasPermission(991, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                filter.Code ??= 0; filter.Code2 ??= 0; filter.PaymentTermsId ??= 0; filter.BranchId ??= 0;
                var branchesId = session.Branches.Split(',');

                var items = await fxaServiceManager.FxaTransactionService.GetAllVW(x => x.IsDeleted == false && x.TransTypeId == 2 && x.FacilityId == session.FacilityId
                && (
                    filter.Code == 0
                    || (filter.Code2 != 0 && Convert.ToInt64(x.Code) >= filter.Code && Convert.ToInt64(x.Code) <= filter.Code2)
                    || x.Code!.Contains(filter.Code.ToString() ?? "")
                    )
                && (filter.PaymentTermsId == 0 || x.PaymentTermsId == filter.PaymentTermsId)
                && (filter.BranchId == 0 || x.BranchId == filter.BranchId)
                && branchesId.Contains(x.BranchId.ToString())
                && (string.IsNullOrEmpty(filter.RefNumber) || x.RefNumber == filter.RefNumber)
                && (string.IsNullOrEmpty(filter.SupplierCode) || x.SupplierCode == filter.SupplierCode)
                );

                if (items.Succeeded)
                {
                    // filter by date
                    var res = items.Data;
                    if (!string.IsNullOrEmpty(filter.StartDate) && !string.IsNullOrEmpty(filter.EndDate))
                    {
                        DateTime startDate = DateTime.ParseExact(filter.StartDate ?? "", "yyyy/MM/dd", CultureInfo.InvariantCulture);
                        DateTime endDate = DateTime.ParseExact(filter.EndDate ?? "", "yyyy/MM/dd", CultureInfo.InvariantCulture);

                        res = res.Where(r => !string.IsNullOrEmpty(r.TransDate) && DateTime.ParseExact(r.TransDate, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= startDate
                            && DateTime.ParseExact(r.TransDate, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= endDate);
                    }

                    List<FxaTransactionsVw> final = new();
                    if (!string.IsNullOrEmpty(filter.FxCode))
                    {
                        // get all FXA_Transactions_VW
                        var getAssetTransVw = await fxaServiceManager.FxaTransactionsAssetService.GetAllVW(x => x.IsDeleted == false);
                        var assetTransVw = getAssetTransVw.Data;
                        foreach (var item in res)
                        {
                            var chkExist = assetTransVw.Where(x => x.TransactionId == item.Id && x.FxCode == filter.FxCode);
                            if (chkExist.Any())
                                final.Add(item);
                        }

                        return Ok(await Result<List<FxaTransactionsVw>>.SuccessAsync(final));
                    }
                    else
                    {
                        return Ok(await Result<List<FxaTransactionsVw>>.SuccessAsync(res.ToList()));
                    }
                }

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var chk = await permission.HasPermission(991, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await fxaServiceManager.FxaTransactionService.RemoveTransaction(id, 19);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp Message: {ex.Message}"));
            }
        }
    }
}
