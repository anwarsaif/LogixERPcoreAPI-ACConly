using Logix.Application.Common;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Services;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.FXA.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.FXA
{
    public class SharedController : BaseFxaApiController
    {
        private readonly IFxaServiceManager fxaServiceManager;
        private readonly ISysConfigurationHelper configurationHelper;
        private readonly ICurrentData session;

        public SharedController(IFxaServiceManager fxaServiceManager,
            ICurrentData session,
            ISysConfigurationHelper configurationHelper
            )
        {
            this.fxaServiceManager = fxaServiceManager;
            this.session = session;
            this.configurationHelper = configurationHelper;
        }

        [HttpGet("GetCostCenterProperties")]
        public async Task<ActionResult> GetCostCenterProperties()
        {
            try
            {
                //get properties that indicates if cost center 2,3,4 and 5 is visible or not
                var cc2Visible = await configurationHelper.GetValue(27, session.FacilityId);
                var cc3Visible = await configurationHelper.GetValue(28, session.FacilityId);
                var cc4Visible = await configurationHelper.GetValue(29, session.FacilityId);
                var cc5Visible = await configurationHelper.GetValue(30, session.FacilityId);

                AddAssetProperties obj = new()
                {
                    CC2Visible = cc2Visible == "1",
                    CC3Visible = cc3Visible == "1",
                    CC4Visible = cc4Visible == "1",
                    CC5Visible = cc5Visible == "1",

                    CC1Title = await configurationHelper.GetValue(45, session.FacilityId),
                    CC2Title = await configurationHelper.GetValue(46, session.FacilityId),
                    CC3Title = await configurationHelper.GetValue(47, session.FacilityId),
                    CC4Title = await configurationHelper.GetValue(48, session.FacilityId),
                    CC5Title = await configurationHelper.GetValue(49, session.FacilityId)
                };

                return Ok(await Result<AddAssetProperties>.SuccessAsync(obj));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in GetProperties SharedController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetAssetDataByNo")]
        public async Task<ActionResult> GetAssetDataByNo(long no)
        {
            // this action converted from FXA_FixedAsset_SP when @CMDTYPE = 7
            try
            {
                var getItem = await fxaServiceManager.FxaFixedAssetService.GetOneVW(f => f.IsDeleted == false
                        && f.No == no && f.StatusId == 1 && f.FacilityId == session.FacilityId);

                if (getItem.Succeeded)
                {
                    var item = getItem.Data;
                    FxaFixedAssetVm2 obj = new()
                    {
                        Id = item.Id,
                        No = item.No,
                        Name = item.Name,
                        AccountCode = item.AccAccountCode,
                        AccountCode2 = item.AccAccountCode2,
                        AccountCode3 = item.AccAccountCode3,

                        AccountName = item.AccAccountName,
                        AccountName2 = item.AccAccountName2,
                        AccountName3 = item.AccAccountName3,

                        StartDate = item.StartDate,
                        EndDate = item.EndDate,
                    };

                    decimal depreAmount = 0; decimal fxAmount = 0; decimal balance = 0; decimal newFxAmount = 0;
                    var getAllAssetTrans = await fxaServiceManager.FxaTransactionsAssetService.GetAllVW(t => t.FixedAssetId == item.Id && t.IsDeleted == false);
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

                        obj.DepreAmount = depreAmount;
                        obj.FxAmount = fxAmount;
                        obj.Balance = balance;
                        obj.NewFxAmount = newFxAmount;

                        return Ok(await Result<FxaFixedAssetVm2>.SuccessAsync(obj));
                    }
                    return Ok(getAllAssetTrans);
                }
                return Ok(getItem);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in GetAssetDataByNo SharedController, MESSAGE: {ex.Message}"));
            }
        }
    }
}