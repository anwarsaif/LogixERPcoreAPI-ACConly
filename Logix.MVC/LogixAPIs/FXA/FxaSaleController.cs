using Logix.Application.Common;
using Logix.Application.DTOs.FXA;
using Logix.Application.DTOs.Main;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.FXA.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Operations;
using System.Globalization;

namespace Logix.MVC.LogixAPIs.FXA
{
    public class FxaSaleController : BaseFxaApiController
    {
        private readonly IFxaServiceManager fxaServiceManager;
        private readonly IAccServiceManager accServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;

        public FxaSaleController(IFxaServiceManager fxaServiceManager,
            IAccServiceManager accServiceManager,
            IPermissionHelper permission,
            ICurrentData session,
            ILocalizationService localization)
        {
            this.fxaServiceManager = fxaServiceManager;
            this.accServiceManager = accServiceManager;
            this.permission = permission;
            this.session = session;
            this.localization = localization;
        }


        [HttpPost("Search")]
        public async Task<ActionResult> Search(FxaSaleFilterVM filter)
        {
            try
            {
                var chk = await permission.HasPermission(360, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var items = await fxaServiceManager.FxaTransactionService.GetAllVW(a => a.IsDeleted == false
                        && a.FacilityId == session.FacilityId && a.TransTypeId == 4
                        && (string.IsNullOrEmpty(filter.Code) || (!string.IsNullOrEmpty(a.Code) && a.Code.Equals(filter.Code)))
                       );

                if (items.Succeeded)
                {
                    var res = items.Data.OrderBy(r => r.Code).AsQueryable();
                    if (!string.IsNullOrEmpty(filter.StartDate) && !string.IsNullOrEmpty(filter.EndDate))
                    {
                        DateTime startDate = DateTime.ParseExact(filter.StartDate ?? "", "yyyy/MM/dd", CultureInfo.InvariantCulture);
                        DateTime endDate = DateTime.ParseExact(filter.EndDate ?? "", "yyyy/MM/dd", CultureInfo.InvariantCulture);

                        res = res.Where(r => !string.IsNullOrEmpty(r.TransDate) && DateTime.ParseExact(r.TransDate, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= startDate
                            && DateTime.ParseExact(r.TransDate, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= endDate);
                    }

                    List<FxaSaleFilterVM> final = new();

                    foreach (var item in res)
                    {
                        final.Add(new FxaSaleFilterVM
                        {
                            Id = item.Id,
                            Code = item.Code,
                            TransDate = item.TransDate,
                            FxNo = item.FxNo,
                            FxName = item.FxName,
                            Total = item.Total,
                            Note = item.Note
                        });
                    }
                    return Ok(await Result<List<FxaSaleFilterVM>>.SuccessAsync(final));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Search FxaSaleController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Add")]
        public async Task<ActionResult> Add(FxaTransactionDto_Sale obj)
        {
            try
            {
                var chk = await permission.HasPermission(360, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                obj.TransTypeId = 4;
                var add = await fxaServiceManager.FxaTransactionService.Add_Sale(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add FxaSaleController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("View")]
        public async Task<IActionResult> View(long id)
        {
            try
            {
                var chk = await permission.HasPermission(360, PermissionType.View);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInView")}"));
                }

                var result = await GetTransactionData(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in View FxaSaleController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetByIdForEdit")]
        public async Task<IActionResult> GetByIdForEdit(long id)
        {
            try
            {
                var chk = await permission.HasPermission(360, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var result = await GetTransactionData(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in GetByIdForEdit FxaSaleController, MESSAGE: {ex.Message}"));
            }
        }


        private async Task<Result<FxaTransactionDto_Sale>> GetTransactionData(long id)
        {
            //this function used in View and GetByIdForEdit functions
            try
            {
                var getItem = await fxaServiceManager.FxaTransactionService.GetOneVW(t => t.Id == id && t.FacilityId == session.FacilityId && t.IsDeleted == false);
                if (getItem.Succeeded)
                {
                    var res = getItem.Data;

                    FxaTransactionDto_Sale obj = new()
                    {
                        Id = res.Id,
                        TransDate = res.TransDate,
                        BranchId = res.BranchId,
                        DebAccCode = res.AccAccountCode,
                        ProfitLossAccCode = res.AccAccountCode2,
                        SaleAmount = res.Total,
                        Note = res.Note,
                        FacilityId = res.FacilityId
                    };

                    var getJournalCode = await accServiceManager.AccJournalMasterService.GetOne(j => j.ReferenceNo == id && j.DocTypeId == 21 && j.FlagDelete == false);
                    obj.JCode = getJournalCode.Data.JCode;
                    obj.PeriodId = getJournalCode.Data.PeriodId;

                    var getCostCenters = await accServiceManager.AccCostCenteHelpVwService.GetAll(c => c.CcId == res.CcId || c.CcId == res.CcId2
                            || c.CcId == res.CcId3 || c.CcId == res.CcId4 || c.CcId == res.CcId5);
                    if (getCostCenters.Succeeded)
                    {
                        var costCenters = getCostCenters.Data;
                        obj.CcCode = costCenters.SingleOrDefault(c => res.CcId > 0 && c.CcId == res.CcId)?.CostCenterCode;
                        obj.CcCode2 = costCenters.SingleOrDefault(c => res.CcId2 > 0 && c.CcId == res.CcId2)?.CostCenterCode;
                        obj.CcCode3 = costCenters.SingleOrDefault(c => res.CcId3 > 0 && c.CcId == res.CcId3)?.CostCenterCode;
                        obj.CcCode4 = costCenters.SingleOrDefault(c => res.CcId4 > 0 && c.CcId == res.CcId4)?.CostCenterCode;
                        obj.CcCode5 = costCenters.SingleOrDefault(c => res.CcId5 > 0 && c.CcId == res.CcId5)?.CostCenterCode;
                    }

                    //get fixed asset id
                    var getFxId = await fxaServiceManager.FxaTransactionsAssetService.GetOne(t => t.FixedAssetId, t => t.TransactionId == res.Id && t.IsDeleted == false);

                    var getFixedAsset = await fxaServiceManager.FxaFixedAssetService.GetOneVW(f => f.IsDeleted == false
                        && f.Id == getFxId.Data && f.FacilityId == session.FacilityId);

                    obj.FxNo = getFixedAsset.Data.No;
                    //obj.Name = item.Name,
                    obj.AccCode = getFixedAsset.Data.AccAccountCode;
                    obj.AccCode2 = getFixedAsset.Data.AccAccountCode2;
                    obj.AccCode3 = getFixedAsset.Data.AccAccountCode3;

                    //obj.AccName = getFixedAsset.Data.AccAccountName;
                    //obj.AccName2 = getFixedAsset.Data.AccAccountName2;
                    //obj.AccName3 = getFixedAsset.Data.AccAccountName3;

                    decimal depreAmount = 0; decimal fxAmount = 0; decimal balance = 0; decimal newFxAmount = 0;
                    var getAllAssetTrans = await fxaServiceManager.FxaTransactionsAssetService.GetAllVW(t => t.FixedAssetId == getFxId.Data && t.IsDeleted == false);
                    if (getAllAssetTrans.Succeeded)
                    {
                        var allAssetTrans = getAllAssetTrans.Data;

                        // get DepreAmount
                        var deprecTrans = allAssetTrans.Where(t => t.TransTypeId == 5);
                        depreAmount = deprecTrans.Sum(x => x.Debet ?? 0);

                        // get FxAmount
                        var amountTrans = allAssetTrans.Where(t => t.TransTypeId == 1 || t.TransTypeId == 2 || t.TransTypeId == 3);
                        fxAmount = amountTrans.Sum(x => x.Credit ?? 0);

                        // get Balance
                        var balanceTrans = allAssetTrans.Where(t => t.TransTypeId == 1 || t.TransTypeId == 2 || t.TransTypeId == 3 || t.TransTypeId == 5 || t.TransTypeId == 8);
                        var credit = balanceTrans.Sum(x => x.Credit ?? 0);
                        var debit = balanceTrans.Sum(x => x.Debet ?? 0);
                        balance = credit - debit;

                        // get NewFxAmount
                        newFxAmount = balance;

                        obj.DeprecAmount = depreAmount;
                        obj.Amount = fxAmount;
                        obj.Balance = balance;
                        //obj.NewFxAmount = newFxAmount;
                    }

                    return await Result<FxaTransactionDto_Sale>.SuccessAsync(obj);
                }

                return await Result<FxaTransactionDto_Sale>.FailAsync(getItem.Status.message);
            }
            catch (Exception ex)
            {
                return await Result<FxaTransactionDto_Sale>.FailAsync($"====== Exp in GetTransactionData FxaSaleController, MESSAGE: {ex.Message}");
            }
        }


        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(FxaTransactionDto_Sale obj)
        {
            try
            {
                var chk = await permission.HasPermission(360, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var update = await fxaServiceManager.FxaTransactionService.Update_Sal(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Edit FxaFixedAssetController, MESSAGE: {ex.Message}"));
            }
        }


        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var chk = await permission.HasPermission(360, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await fxaServiceManager.FxaTransactionService.RemoveTransaction(id, 21);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp Message: {ex.Message}"));
            }
        }

    }
}