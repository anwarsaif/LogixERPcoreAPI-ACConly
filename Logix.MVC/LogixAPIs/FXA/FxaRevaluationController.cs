using AutoMapper;
using Logix.Application.Common;
using Logix.Application.DTOs.FXA;
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
    public class FxaRevaluationController : BaseFxaApiController
    {
        /// <summary>
        /// this controller will deal with FxaTransactionService.
        /// in FxaTransactionService we save the revaluation as (transaction & transactionAsset).
        /// then we also save it into FXATransactionsRevaluation table
        /// </summary>

        private readonly IFxaServiceManager fxaServiceManager;
        private readonly IAccServiceManager accServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;

        public FxaRevaluationController(IFxaServiceManager fxaServiceManager,
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
        public async Task<ActionResult> Search(FxaRevaluationFilterVM filter)
        {
            try
            {
                var chk = await permission.HasPermission(1256, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var branchsId = session.Branches.Split(',');
                filter.BranchId ??= 0;
                filter.FxNo ??= 0;

                var items = await fxaServiceManager.FxaTransactionsRevaluationService.GetAllVW(a => a.TransTypeId == 8
                        && a.FacilityId == session.FacilityId && a.IsDeleted == false
                        && (string.IsNullOrEmpty(filter.Code) || (!string.IsNullOrEmpty(a.Code) && a.Code.Equals(filter.Code)))
                        && (filter.FxNo == 0 || (a.FxNo == filter.FxNo))
                        && (string.IsNullOrEmpty(filter.FxName) || (!string.IsNullOrEmpty(a.FxName) && a.FxName.Contains(filter.FxName)))
                        && (filter.BranchId == 0 || (a.BranchId == filter.BranchId))
                        && ((filter.BranchId != 0) || branchsId.Contains(a.BranchId.ToString()))
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

                    List<FxaRevaluationFilterVM> final = new();

                    foreach (var item in res)
                    {
                        FxaRevaluationFilterVM obj = new()
                        {
                            Id = item.Id,
                            Code = item.Code,
                            BranchId = item.BranchId,
                            FxNo = item.FxNo,
                            FxName = item.FxName,

                            TransDate = item.TransDate,
                            AmountOld = item.AmountOld,
                            AmountNew = item.AmountNew,
                            AmountDepreciation = item.AmountDepreciation,
                            ProfitAndLoss = item.ProfitAndLoss
                        };

                        //get journal code and date
                        var getJournal = await accServiceManager.AccJournalMasterService.GetOne(
                                j => j.ReferenceNo == item.Id && j.DocTypeId == 41 && j.FlagDelete == false
                            );
                        if (getJournal.Succeeded)
                        {
                            obj.JCode = getJournal.Data.JCode;
                            obj.JDate = getJournal.Data.JDateGregorian;
                        }

                        final.Add(obj);
                    }
                    return Ok(await Result<List<FxaRevaluationFilterVM>>.SuccessAsync(final));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp Message: {ex.Message}"));
            }
        }

        [HttpPost("Add")]
        public async Task<ActionResult> Add(FxaTransactionDto_Revaluation obj)
        {
            try
            {
                var chk = await permission.HasPermission(1256, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                obj.TransTypeId = 8;
                var add = await fxaServiceManager.FxaTransactionService.Add_Revaluation(obj);
                return Ok(add);
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
                var chk = await permission.HasPermission(1256, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await fxaServiceManager.FxaTransactionService.RemoveTransaction(id, 41);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp Message: {ex.Message}"));
            }
        }
    }
}