using AutoMapper;
using Logix.Application.Common;
using Logix.Application.DTOs.FXA;
using Logix.Application.DTOs.Main;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.FXA.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Logix.MVC.LogixAPIs.FXA
{
    public class FxaDepreciationController : BaseFxaApiController
    {
        private readonly IFxaServiceManager fxaServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IMapper _mapper;

        public FxaDepreciationController(IFxaServiceManager fxaServiceManager,
            IPermissionHelper permission,
            ICurrentData session,
            ILocalizationService localization,
             IMapper mapper)
        {
            this.fxaServiceManager = fxaServiceManager;
            this.permission = permission;
            this.session = session;
            this.localization = localization;
            this._mapper = mapper;
        }

        [HttpPost("Search")]
        public async Task<ActionResult> Search(FxaDepreciationFilterVM filter)
        {
            try
            {
                var chk = await permission.HasPermission(354, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var items = await fxaServiceManager.FxaTransactionService.GetAllVW(a => a.IsDeleted == false
                        && a.FacilityId == session.FacilityId && a.TransTypeId == 5
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

                    List<FxaDepreciationFilterVM> final = new();

                    foreach (var item in res)
                    {
                        final.Add(new FxaDepreciationFilterVM
                        {
                            Id = item.Id,
                            Code = item.Code,
                            StartDate = item.StartDate,
                            EndDate = item.EndDate,
                            TransDate = item.TransDate,
                            Total = item.Total
                        });
                    }
                    return Ok(await Result<List<FxaDepreciationFilterVM>>.SuccessAsync(final));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp Message: {ex.Message}"));
            }
        }


        #region ================================================== Add Depreciation ================================================
        [HttpGet("GetLastDeprecDate")]
        public async Task<IActionResult> GetLastDeprecDate()
        {
            try
            {
                string maxDeprecDate = "";
                var getLastDeprecDate = await fxaServiceManager.FxaTransactionService.GetAll(t => t.EndDate,
                        t => t.TransTypeId == 5 && t.FacilityId == session.FacilityId && t.IsDeleted == false);
                if (getLastDeprecDate.Succeeded)
                {
                    if (getLastDeprecDate.Data.Any())
                    {
                        maxDeprecDate = getLastDeprecDate.Data.Max() ?? "";
                    }
                }
                return Ok(await Result<string>.SuccessAsync(maxDeprecDate));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp Message: {ex.Message}"));
            }
        }
        
        [HttpPost("GetAssetForDeprec")]
        public async Task<ActionResult> GetAssetForDeprec(AssetsForDeprecFilter filter)
        {
            // this action converted from stored procedure [FXA_Transactions_SP] @CMDTYPE=6
            try
            {
                filter.TypeId ??= 0;
                filter.BranchId ??= 0;
                var getAssets = await fxaServiceManager.FxaFixedAssetService.GetAllVW(a => a.IsDeleted == false && a.StatusId == 1 && a.FacilityId == session.FacilityId
                       && (filter.BranchId == 0 || a.BranchId == filter.BranchId)
                       && (filter.TypeId == 0 || a.TypeId == filter.TypeId)
                      );

                if (getAssets.Succeeded)
                {
                    List<AssetsForDeprecVm> deprecTbl = new();
                    deprecTbl = _mapper.Map<List<AssetsForDeprecVm>>(getAssets.Data);

                    foreach (var record in deprecTbl)
                    {
                        //get Balance
                        decimal balance = 0; decimal credit = 0; decimal debet = 0;
                        var getTransAsset = await fxaServiceManager.FxaTransactionsAssetService.GetAll(t => t.FixedAssetId == record.Id && t.IsDeleted == false);
                        if (getTransAsset.Succeeded)
                        {
                            if (getTransAsset.Data.Any())
                            {
                                credit = getTransAsset.Data.Sum(x => Convert.ToDecimal(x.Credit));
                                debet = getTransAsset.Data.Sum(x => Convert.ToDecimal(x.Debet));
                                balance = credit - debet;
                                record.Balance = balance;
                            }
                        }
                        else
                            return Ok(getTransAsset);

                        //get LastDeprecDate
                        var getTransAssetVw = await fxaServiceManager.FxaTransactionsAssetService.GetAllVW(t => t.FixedAssetId == record.Id
                            && t.TransTypeId == 5 && t.IsDeletedTrans == false && t.IsDeleted == false && !string.IsNullOrEmpty(t.EndDate));

                        if (getTransAssetVw.Succeeded)
                        {
                            string maxDate = "";
                            if (getTransAssetVw.Data.Any())
                            {
                                maxDate = getTransAssetVw.Data.Max(x => x.EndDate) ?? "";
                            }

                            if (string.IsNullOrEmpty(maxDate))
                            {
                                var getStartDate = await fxaServiceManager.FxaFixedAssetService.GetOne(a => a.StartDate, a => a.Id == record.Id);
                                record.LastDeprecDate = getStartDate.Data;
                            }
                            else
                                record.LastDeprecDate = maxDate;
                        }
                        else
                            return Ok(getTransAssetVw);

                        //get InstallmentValue
                        if (record.DeprecMethod == 1)
                            record.InstallmentValue = record.InstallmentValue;
                        else if (record.DeprecMethod == 2)
                            record.InstallmentValue = record.DeprecMonthlyRate;
                    }

                    //do other operations on deprecTbl
                    List<AssetsForDeprecVm> final = new();
                    foreach (var record in deprecTbl)
                    {
                        decimal installmentValue = record.InstallmentValue ?? 0;
                        decimal balance = record.Balance ?? 0;
                        decimal scrapValue = record.ScrapValue ?? 0;
                        decimal deprecValue = 0;
                        decimal deprecValueDialy = 0;

                        //get month count, dayCount, and then calculate Deprec_Value
                        DateTime lastDeprecDate = DateTime.ParseExact(record.LastDeprecDate ?? "", "yyyy/MM/dd", CultureInfo.InvariantCulture);
                        DateTime endDate = DateTime.ParseExact(filter.EndDate ?? "", "yyyy/MM/dd", CultureInfo.InvariantCulture);

                        int monthDiff = (endDate.Year * 12 + endDate.Month) - (lastDeprecDate.Year * 12 + lastDeprecDate.Month);
                        TimeSpan diffInDay = endDate - lastDeprecDate;
                        int dayDiff = Convert.ToInt32(diffInDay.TotalDays);

                        record.CntMonth = monthDiff;
                        record.CntDays = dayDiff;

                        if ((balance > scrapValue) && ((filter.PeriodTypeId == 1 && monthDiff > 0) || (filter.PeriodTypeId == 2 && dayDiff > 0)))
                        {
                            //calculate Deprec_Value (monthly deprec)
                            if ((monthDiff * installmentValue) > balance)
                            {
                                if (balance <= scrapValue)
                                    deprecValue = 0;
                                else
                                    deprecValue = balance;
                            }
                            else if ((monthDiff * installmentValue) > (balance - scrapValue))
                            {
                                if ((balance - scrapValue) <= 0)
                                    deprecValue = 0;
                                else
                                    deprecValue = balance - scrapValue;
                            }
                            else
                            {
                                deprecValue = monthDiff * installmentValue;
                            }

                            record.DeprecValue = deprecValue;

                            //calculate Deprec_Value_Dialy (dialy deprec)
                            if ((dayDiff * (installmentValue / 30)) > balance)
                            {
                                if (balance <= scrapValue)
                                    deprecValueDialy = 0;
                                else
                                    deprecValueDialy = balance;
                            }
                            else if ((dayDiff * (installmentValue / 30)) > (balance - scrapValue))
                            {
                                if ((balance - scrapValue) <= 0)
                                    deprecValueDialy = 0;
                                else
                                    deprecValueDialy = balance - scrapValue;
                            }
                            else
                            {
                                deprecValueDialy = dayDiff * (installmentValue / 30);
                            }

                            record.DeprecValueDialy = deprecValueDialy;

                            final.Add(record);
                        }
                    }
                    return Ok(await Result<List<AssetsForDeprecVm>>.SuccessAsync(final));
                }
                return Ok(getAssets);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp Message: {ex.Message}"));
            }
        }

        [HttpPost("Add")]
        public async Task<ActionResult> Add(AssetsDeprecAddDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(354, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var add = await fxaServiceManager.FxaTransactionService.Add_Depreciation(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp Message: {ex.Message}"));
            }
        }
        #endregion ============================================== End Add Depreciation =============================================


        [HttpGet("GetByIdForEdit")]
        public async Task<IActionResult> GetByIdForEdit(long id)
        {
            try
            {
                var chk = await permission.HasPermission(354, PermissionType.View);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var getItem = await fxaServiceManager.FxaTransactionService.GetOne(t => t.Id == id && t.FacilityId == session.FacilityId && t.IsDeleted == false);
                if (getItem.Succeeded)
                {
                    var item = getItem.Data;
                    AssetsDeprecEditDto obj = new()
                    {
                        Id = item.Id,
                        Code = item.Code,
                        TransDate = item.TransDate,
                        StartDate = item.StartDate,
                        EndDate = item.EndDate
                    };

                    //GetFXA_Transactions_Assest
                    var getTransAssets = await fxaServiceManager.FxaTransactionsAssetService.GetAllVW(t => t.TransactionId == id && t.IsDeleted == false);
                    if (getTransAssets.Succeeded)
                    {
                        foreach (var asset in getTransAssets.Data)
                        {
                            obj.AssetList.Add(new AssetsForDeprecEditVm()
                            {
                                FxId = asset.FixedAssetId ?? 0,
                                FxNo = asset.FxNo ?? 0,
                                FxName = asset.FxName,
                                DeprecValue = asset.Debet ?? 0
                            });
                        }
                    }

                    return Ok(await Result<AssetsDeprecEditDto>.SuccessAsync(obj));
                }
                return Ok(getItem);
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
                var chk = await permission.HasPermission(354, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await fxaServiceManager.FxaTransactionService.RemoveTransaction(id, 18);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp Message: {ex.Message}"));
            }
        }
    }
}