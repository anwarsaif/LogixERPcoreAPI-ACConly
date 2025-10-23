using Logix.Application.Common;
using Logix.Application.DTOs.ACC;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;

namespace Logix.MVC.LogixAPIs.Acc
{
    public class ClosingFinancialYearController : BaseAccApiController
    {
        private readonly IPermissionHelper permission;
        private readonly IAccServiceManager accServiceManager;
        private readonly ILocalizationService localization;
        private readonly ICurrentData session;
        private readonly IApiDDLHelper ddlHelper;

        public ClosingFinancialYearController(
             IPermissionHelper permission,
               IAccServiceManager accServiceManager,
                ILocalizationService localization,
                ICurrentData session,
                IApiDDLHelper ddlHelper

            )
        {
            this.permission = permission;
            this.accServiceManager = accServiceManager;
            this.localization = localization;
            this.session = session;
            this.ddlHelper = ddlHelper;
        }


        #region "transactions"


        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var chk = await permission.HasPermission(735, PermissionType.Show);
          ;

            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var items = await accServiceManager.AccJournalMasterService.GetAllVW(x => x.FlagDelete == false && x.FacilityId == session.FacilityId && x.DocTypeId == 2 && x.FinYear == session.FinYear);
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    res = res.OrderBy(e => e.JId);
                    return Ok(items);
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }
        [HttpPost("Search")]
        public async Task<IActionResult> Search(BalanceSheetFinancialYearFilter filter)
        {
            var chk = await permission.HasPermission(735, PermissionType.Show);


            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
               
             
                var items = await accServiceManager.AccFinancialYearService.GetBalanceSheetData(filter);

                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                  

                    var final = res.ToList();
                    return Ok(await Result<List<BalanceSheetFinancialYear>>.SuccessAsync(final, ""));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<BalanceSheetFinancialYear>.FailAsync($"======= Exp in Search Balance Sheet Financial Year, MESSAGE: {ex.Message}"));
            }
        }
        #endregion "transactions"

        #region "transactions_CreateJournal"
        [HttpPost("CreateJournal")]
        public async Task<IActionResult> CreateJournal(ClosingFinancialYearDto obj)
        {
            var chk = await permission.HasPermission(735, PermissionType.Add);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<ClosingFinancialYearDto>.FailAsync(localization.GetMessagesResource("dataRequire")));
                }

                var add = await accServiceManager.AccFinancialYearService.CreateJournal(obj);
                return Ok(add);

            }
            catch (Exception ex)
            {
                return Ok(await Result<ClosingFinancialYearDto>.FailAsync($"======= Exp in Acc Journal add: {ex.Message}"));
            }
        }
        #endregion "transactions_CreateJournal"
        [HttpGet("DDLAccPeriods")]
        public async Task<IActionResult> DDLAccPeriods(long FinYear)
        {
            try
            {
                var list = new SelectList(new List<DDListItem<AccPeriodDateVws>>());
                list = await ddlHelper.GetAnyLis<AccPeriodDateVws, long>(p => p.PeriodState == 1 && p.FinYear == FinYear
                        && p.FacilityId == session.FacilityId && p.FlagDelete == false, "PeriodId", (session.Language == 1) ? "PeriodDate" : "PeriodDate2");
                return Ok(await Result<SelectList>.SuccessAsync(list));

            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
    }
}
