using Logix.Application.Common;
using Logix.Application.DTOs.ACC;
using Logix.Application.Interfaces.IRepositories;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Services;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;
using Logix.Infrastructure.Repositories;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.Acc.ViewModels;
using Logix.MVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Logix.MVC.LogixAPIs.Acc
{
    public class AccReportsController : BaseAccApiController
    {

        private readonly IAccServiceManager accServiceManager;
        private readonly IPermissionHelper permission;
        private readonly IWebHostEnvironment env;
        private readonly ILocalizationService localization;
        private readonly ICurrentData _session;

        public AccReportsController(
            IAccServiceManager accServiceManager,
            IPermissionHelper permission,
             IWebHostEnvironment env,
            IHttpContextAccessor httpContextAccessor,
             ILocalizationService localization
             , ISysConfigurationHelper configurationHelper
            , ICurrentData session
            )
        {
            this.accServiceManager = accServiceManager;
            this.permission = permission;
            this.env = env;
            this.localization = localization;
            this._session = session;
        }

        //==================كشف حساب
        #region "Account transactions"


        [HttpPost("GetAccounttransactions")]
        public async Task<IActionResult> GetAccounttransactions(AccounttransactionsDto filter)
        {
            var chk = await permission.HasPermission(75, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                filter.branchId ??= 0;

                var items = await accServiceManager.AccReportsService.GetAccounttransactions(filter);

                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    var final = res.ToList();
                    return Ok(await Result<List<AccountBalanceSheetDto>>.SuccessAsync(final, ""));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccountBalanceSheetDto>.FailAsync($"======= Exp in Search Account transactions, MESSAGE: {ex.Message}"));
            }
        }



        #endregion "Account transactions"

        //==================كشف حساب عميل
        #region "Customer account statement"


        [HttpPost("GetCustomerAccountstatement")]
        public async Task<IActionResult> GetCustomerAccountstatement(CustomerAccountStatementDto filter)
        {
            var chk = await permission.HasPermission(392, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                filter.branchId ??= 0;
                filter.ReferenceTypeId ??= 0;
                var items = await accServiceManager.AccReportsService.GetCustomerAccounttransactions(filter);

                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    var final = res.ToList();
                    return Ok(await Result<List<AccountBalanceSheetDto>>.SuccessAsync(final, ""));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccountBalanceSheetDto>.FailAsync($"======= Exp in Search Customer account statement, MESSAGE: {ex.Message}"));
            }
        }



        #endregion "Customer account statement"

        //================== كشف حساب مقاول
        #region "Contractor's account statement"


        [HttpPost("GetContractorsAccountstatement")]
        public async Task<IActionResult> GetContractorsAccountstatement(ContractorsAccountStatementDto filter)
        {
            var chk = await permission.HasPermission(943, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                filter.branchId ??= 0;
                filter.ReferenceTypeId ??= 0;
                var items = await accServiceManager.AccReportsService.GetContractorsAccounttransactions(filter);

                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    var final = res.ToList();
                    return Ok(await Result<List<AccountBalanceSheetDto>>.SuccessAsync(final, ""));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccountBalanceSheetDto>.FailAsync($"======= Exp in Search Contractor's account statement, MESSAGE: {ex.Message}"));
            }
        }



        #endregion "Contractor's account statements"

        //==================كشف حساب مورد
        #region "Supplier account statement"


        [HttpPost("GetSupplierAccountstatement")]
        public async Task<IActionResult> GetSupplierAccountstatement(SupplierAccountStatementDto filter)
        {
            var chk = await permission.HasPermission(467, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                filter.branchId ??= 0;
                filter.ReferenceTypeId ??= 0;
                var items = await accServiceManager.AccReportsService.GetSupplierAccounttransactions(filter);

                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    var final = res.ToList();
                    return Ok(await Result<List<AccountBalanceSheetDto>>.SuccessAsync(final, ""));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccountBalanceSheetDto>.FailAsync($"======= Exp in Search Supplier account statement, MESSAGE: {ex.Message}"));
            }
        }



        #endregion "Supplier account statements"

        //==================كشف حساب صندوق

        #region "Funds account statement"

        [HttpPost("GetFundsstatementtransactions")]
        public async Task<IActionResult> GetFundsstatementtransactions(FundsstatementtransactionsDto filter)
        {

            var hasPermission = await permission.HasPermission(1017, PermissionType.Show);
            if (!hasPermission)
            {
                return Ok(await Result.AccessDenied("Access Denied"));
            }

            try
            {

                filter.branchId ??= 0;
                var items = await accServiceManager.AccReportsService.GetFundsstatementtransactions(filter);

                if (items.Succeeded && items.Data != null)
                {
                    var final = items.Data.ToList();
                    return Ok(await Result<List<AccountBalanceSheetDto>>.SuccessAsync(final, ""));
                }


                return Ok(items);
            }
            catch (Exception ex)
            {

                return Ok(await Result<List<AccountBalanceSheetDto>>.FailAsync($"An error occurred: {ex.Message}"));
            }
        }

        #endregion

        //==================كشف حساب مجموعة
        #region "Account transactions Group"

        [HttpPost("GetAccounttransactionsGroup")]
        public async Task<IActionResult> GetAccounttransactionsGroup(AccounttransactionsFilterGroupDto filter)
        {
            var chk = await permission.HasPermission(91, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                filter.branchId ??= 0;

                var items = await accServiceManager.AccReportsService.GetAccounttransactionsGroup(filter);

                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    var final = res.ToList();
                    return Ok(await Result<List<AccounttransactionsGroupDto>>.SuccessAsync(final, ""));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccounttransactionsGroupDto>.FailAsync($"======= Exp in Search Account transactions Group, MESSAGE: {ex.Message}"));
            }
        }

        #endregion

        //==================كشف حساب مركز تكلفة
        #region "Costcenter transactions "

        [HttpPost("GetCostcentertransactions")]
        public async Task<IActionResult> GetCostcentertransactions(CostcentertransactionsFilterDto filter)
        {
            var chk = await permission.HasPermission(291, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                filter.branchId ??= 0;

                var items = await accServiceManager.AccReportsService.GetCostcentertransactions(filter);

                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    var final = res.ToList();
                    return Ok(await Result<List<CostcentertransactionsDto>>.SuccessAsync(final, ""));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccountBalanceSheetDto>.FailAsync($"======= Exp in Search Costcenter transactions, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("GetACCGroup")]
        public async Task<IActionResult> GetACCGroup()
        {
            try
            {
                var AccGroup = await accServiceManager.AccGroupService.GetAll(b => b.IsDeleted == false && b.FacilityId == _session.FacilityId);
                if (AccGroup.Succeeded)
                {
                    List<AccGroupVM> list = new List<AccGroupVM>();
                    foreach (var item in AccGroup.Data)
                    {
                        var sysgroupVM = new AccGroupVM { AccGroupId = item.AccGroupId, AccGroupName = _session.Language == 1 ? item.AccGroupName : item.AccGroupName2 };
                        list.Add(sysgroupVM);
                    }
                    return Ok(await Result<List<AccGroupVM>>.SuccessAsync(list));
                }
                return Ok(AccGroup);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Acc Group , MESSAGE: {ex.Message}"));
            }
        }

        #endregion Costcenter transactions 


        //==================كشف حساب من الى

        #region "Account Transactions From To"
        [HttpPost("GetAccounttransactionsFromTo")]
        public async Task<IActionResult> GetAccounttransactionsFromTo(AccounttransactionsFromToFilterDto filter)
        {
            var chk = await permission.HasPermission(380, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                filter.branchId ??= 0;

                var items = await accServiceManager.AccReportsService.GetAccounttransactionsFromTo(filter);

                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    var final = res.ToList();
                    return Ok(await Result<List<AccounttransactionsFromToDto>>.SuccessAsync(final, ""));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccountBalanceSheetDto>.FailAsync($"======= Exp in Search Account Transactions From To, MESSAGE: {ex.Message}"));
            }
        }


        #endregion  "Account Transactions From To"

        //==================كشف حساب بالعملة الأجنبية
        #region "Currency transactions "

        [HttpPost("GetCurrencytransactions")]
        public async Task<IActionResult> GetCurrencytransactions(CurrencytransactionsFilterDto filter)
        {
            var chk = await permission.HasPermission(802, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                filter.branchId ??= 0;
                filter.ReferenceTypeId ??= 0;
                filter.CurrencyId ??= 0;
                var items = await accServiceManager.AccReportsService.GetCurrencytransactions(filter);

                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    var final = res.ToList();
                    return Ok(await Result<List<AccountBalanceSheetDto>>.SuccessAsync(final, ""));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccountBalanceSheetDto>.FailAsync($"======= Exp in Search Currency transactions, MESSAGE: {ex.Message}"));
            }
        }

        #endregion Currency transactions 


        //==================كشف حساب مجموعة مركز تكلفة
        #region "Costcenter transactions Group "

        [HttpPost("GetCostcentertransactionsGroup")]
        public async Task<IActionResult> GetCostcentertransactionsGroup(CostcentertransactionsGroupFilterDto filter)
        {
            var chk = await permission.HasPermission(1553, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                filter.branchId ??= 0;

                var items = await accServiceManager.AccReportsService.GetCostcentertransactionsGroup(filter);

                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    var final = res.ToList();
                    return Ok(await Result<List<CostcentertransactionsGroupDto>>.SuccessAsync(final, ""));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccountBalanceSheetDto>.FailAsync($"======= Exp in Search Costcenter transactions Group, MESSAGE: {ex.Message}"));
            }
        }

        #endregion Costcenter transactions  Group

        //=====================================  كشف حساب بتاريخ العملية
        #region  Account Statement Transaction Date

        [HttpPost("GetAccountStatementTransactionDate")]
        public async Task<IActionResult> GetAccountStatementTransactionDate(AccountTransactionDateFilterDto filter)
        {
            var chk = await permission.HasPermission(75, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                filter.CurrencyId ??= 0;
                filter.branchId ??= 0;

                var items = await accServiceManager.AccReportsService.GetAccountStatementTransactionDate(filter);

                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    var final = res.ToList();
                    return Ok(await Result<List<AccountBalanceSheetDto>>.SuccessAsync(final, ""));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccountBalanceSheetDto>.FailAsync($"======= Exp in Search Account Statement Transaction Date, MESSAGE: {ex.Message}"));
            }
        }


        #endregion  Account Statement Transaction Date

        // =====================================  كشف حساب مركز تكلفة من الى 
        #region "Costcenter Transactions From To"
        [HttpPost("GetCostcenterTransactionsFromTo")]
        public async Task<IActionResult> GetCostcenterTransactionsFromTo(CostcenterTransactionsFromToFilterDto filter)
        {
            var chk = await permission.HasPermission(566, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {


                var items = await accServiceManager.AccReportsService.GetCostcenterTransactionsFromTo(filter);

                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    var final = res.ToList();
                    return Ok(await Result<List<CostcenterTransactionsFromToDto>>.SuccessAsync(final, ""));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<CostcenterTransactionsFromToDto>.FailAsync($"======= Exp in Search Costcenter Transactions From To, MESSAGE: {ex.Message}"));
            }
        }



        #endregion Costcenter Transactions From To

        // ===================================== كشف حساب العملاء من رقم الى رقم 

        #region "Customer Transactions From To"
        
        [HttpPost("GetCustomerTransactionFromTo")]
        public async Task<IActionResult> GetCustomerTransactionFromTo(CustomerTransactionFilterDto filter)
        {
            var chk = await permission.HasPermission(430, PermissionType.Show);
            chk = await permission.HasPermission(1054, PermissionType.Show);


            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {



                var items = await accServiceManager.AccReportsService.GetCustomerTransactionsFromTo(filter);

                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    var final = res.ToList();
                    return Ok(await Result<List<CustomerTransactionDto>>.SuccessAsync(final, ""));

                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<TrialBalanceSheetDtoResult>.FailAsync($"======= Exp in Search Trial Balance Sheet Transactions From To, MESSAGE: {ex.Message}"));
            }
        }


        #endregion Customer Transactions From To



        #region //========================================== ميزان المراجعة

        [HttpPost("GetTrialBalanceSheet")]
        public async Task<IActionResult> GetTrialBalanceSheet(TrialBalanceSheetDto filter)
        {
            var chk = await permission.HasPermission(74, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                filter.accountLevel??= 0;

                filter.branchId??= 0;

                var items = await accServiceManager.AccReportsService.GetTrialBalanceSheet(filter);

                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    var final = res.ToList();
                    return Ok(await Result<List<TrialBalanceSheetDtoResult>>.SuccessAsync(final, ""));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<TrialBalanceSheetDtoResult>.FailAsync($"======= Exp in Search Trial Balance Sheet Transactions From To, MESSAGE: {ex.Message}"));
            }
        }



        #endregion ================================== ميزان المراجعة

        #region ========================================== الاستاذ العام

        [HttpPost("GetGeneralLedger")]
        public async Task<IActionResult> GetGeneralLedger(GeneralLedgerDto filter)
        {
            var chk = await permission.HasPermission(78, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {


                var items = await accServiceManager.AccReportsService.GetGeneralLedger(filter);

                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    var final = res.ToList();
                    return Ok(await Result<List<GeneralLedgerDtoResult>>.SuccessAsync(final, ""));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<GeneralLedgerDtoResult>.FailAsync($"======= Exp in Search General Ledger Sheet Transactions From To, MESSAGE: {ex.Message}"));
            }
        }

        #endregion ================================== الاستاذ العام

        #region ========================================== قائمة الدخل
        [HttpPost("GetIncomeStatement")]
        public async Task<IActionResult> GetIncomeStatement(IncomeStatementDto filter)
        {
            var chk = await permission.HasPermission(88, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {

                filter.AccountLevel ??= 0;
                filter.BranchId ??= 0;
                filter.GroupExpenses ??= 0;
                filter.GroupIncome ??= 0;
                var items = await accServiceManager.AccReportsService.GetIncomeStatement(filter);

                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    var final = res.ToList();
                    return Ok(await Result<List<IncomeStatementDtoResult>>.SuccessAsync(final, ""));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<IncomeStatementDtoResult>.FailAsync($"======= Exp in Search Income Statement Sheet Transactions From To, MESSAGE: {ex.Message}"));
            }
        }


        [HttpPost("IncomeStatementDetails")]
        public async Task<IActionResult> IncomeStatementDetails(IncomeStatementDetailsDto filter)
        {
          

            try
            {

                filter.FacilityId ??= 0;
                filter.ccId ??= 0;
                var items = await accServiceManager.AccReportsService.IncomeStatementDetails(filter);

                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    var final = res.ToList();
                    return Ok(await Result<List<IncomeStatementDetailsDtoResult>>.SuccessAsync(final, ""));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<IncomeStatementDetailsDtoResult>.FailAsync($"======= Exp in Search Income Statement Details Sheet Transactions From To, MESSAGE: {ex.Message}"));
            }
        }
        #endregion ================================== قائمة الدخل


        #region ==========================================   قائمة المركز المالي 

        

        //[HttpPost("FinancialCenterList")]
        //public async Task<IActionResult> FinancialCenterList(FinancialCenterListDto filter)
        //{
        //    var chk = await permission.HasPermission(146, PermissionType.Show);
        //    if (!chk)
        //    {
        //        return Ok(await Result.AccessDenied("AccessDenied"));
        //    }

        //    try
        //    {


        //        var items = await accServiceManager.AccReportsService.FinancialCenterList(filter);

        //        if (items.Succeeded)
        //        {
        //            var res = items.Data.AsQueryable();
        //            var final = res.ToList();
        //            return Ok(await Result<List<FinancialCenterListDtoResult>>.SuccessAsync(final, ""));
        //        }
        //        return Ok(items);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(await Result<FinancialCenterListDtoResult>.FailAsync($"======= Exp in Search Financial Center List , MESSAGE: {ex.Message}"));
        //    }
        //}
        [HttpPost("FinancialCenterListBindData")]
        public async Task<IActionResult> FinancialCenterListBindData(FinancialCenterListBindDataDto filter)
        {
            var chk = await permission.HasPermission(146, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {


                var items = await accServiceManager.AccReportsService.FinancialCenterListBindData(filter);

                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    var final = res.ToList();
                    return Ok(await Result<List<FinancialCenterListBindDataDtoResult>>.SuccessAsync(final, ""));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<FinancialCenterListBindDataDtoResult>.FailAsync($"======= Exp in Search Financial Center List , MESSAGE: {ex.Message}"));
            }
        }


        #endregion ==================================  قائمة المركز المالي


        #region ==========================================   قائمة الدخل شهري

      
        [HttpPost("GetIncomeStatementMonth")]
        public async Task<IActionResult> GetIncomeStatementMonth(IncomeStatementMonthtDto filter)
        {
            var chk = await permission.HasPermission(148, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
               
                var items = await accServiceManager.AccReportsService.GetIncomeStatementMonth(filter);

                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    var final = res.ToList();
                    return Ok(await Result<List<IncomeStatementMonthResultDto>>.SuccessAsync(final, ""));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<IncomeStatementMonthResultDto>.FailAsync($"======= Exp in Search Financial Center List , MESSAGE: {ex.Message}"));
            }
        }


        #endregion ==================================  قائمة الدخل شهري



        #region ==========================================  الأرباح والخسائر

        [HttpPost("GetProfitandLoss")]
        public async Task<IActionResult> GetProfitandLoss(ProfitandLossDto filter)
        {
            var chk = await permission.HasPermission(379, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {

                var items = await accServiceManager.AccReportsService.GetProfitandLoss(filter);

                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    var final = res.ToList();
                    return Ok(await Result<List<ProfitandLossResultDto>>.SuccessAsync(final, ""));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<ProfitandLossResultDto>.FailAsync($"======= Exp in Search Profit and Loss, MESSAGE: {ex.Message}"));
            }
        }



        #endregion ==================================  الأرباح والخسائر


        #region ==========================================  قائمة التدفقات النقدية

        [HttpPost("GetCashFlows")]
        public async Task<IActionResult> GetCashFlows(CashFlowsDto filter)
        {
            var chk = await permission.HasPermission(792, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {

                var items = await accServiceManager.AccReportsService.GetCashFlows(filter);

                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    var final = res.ToList();
                    return Ok(await Result<List<CashFlowsResultDto>>.SuccessAsync(final, ""));
                }
                return Ok(items);
            }
            catch (Exception ex)
            { 
                return Ok(await Result<CashFlowsResultDto>.FailAsync($"======= Exp in Search Cash Flows, MESSAGE: {ex.Message}"));
            }
        }
        #endregion ==========================================  قائمة التدفقات النقدية


        #region ==========================================  اعمار الديون


        [HttpPost("GetAgedReceivables")]
        public async Task<IActionResult> GetAgedReceivables(AgedReceivablesDto filter)
        {
            var chk = await permission.HasPermission(249, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var items = await accServiceManager.AccReportsService.GetAgedReceivables(filter);
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    var final = res.ToList();
                    return Ok(await Result<List<AgedReceivablesResultDto>>.SuccessAsync(final, ""));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AgedReceivablesResultDto>.FailAsync($"======= Exp in Search Aged Receivables, MESSAGE: {ex.Message}"));
            }
        }



        #endregion ==========================================  اعمار الديون


        #region ==========================================  أعمار الديون - شهري

        [HttpPost("GetAgedReceivablesMonthly")]
        public async Task<IActionResult> GetAgedReceivablesMonthly(AgedReceivablesMonthlyDto filter)
        {
            var chk = await permission.HasPermission(804, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var items = await accServiceManager.AccReportsService.GetAgedReceivablesMonthly(filter);
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    var final = res.ToList();
                    return Ok(await Result<List<AgedReceivablesMonthlyResultDto>>.SuccessAsync(final, ""));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AgedReceivablesMonthlyResultDto>.FailAsync($"======= Exp in Search Aged Receivables, MESSAGE: {ex.Message}"));
            }
        }





        #endregion ==========================================  أعمار الديون - شهري


        #region ==========================================  مقارنة بالسنوات

        [HttpPost("GetBudgetEstimateCompareyears")]
        public async Task<IActionResult> GetBudgetEstimateCompareyears(CompareyearsDto filter)
        {
            var chk = await permission.HasPermission(1554, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var items = await accServiceManager.AccReportsService.GetBudgetEstimateCompareyears(filter);
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    var final = res.ToList();
                    return Ok(await Result<List<CompareyearsDtoResultDto>>.SuccessAsync(final, ""));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<CompareyearsDtoResultDto>.FailAsync($"======= Exp in Search Budget Estimate Compare years , MESSAGE: {ex.Message}"));
            }
        }



        #endregion ================================== مقارنة بالسنوات


        #region ========================================== تقرير احصائي
       
        [HttpPost("GetDashboardData")]
        public async Task<IActionResult> GetDashboardData(DashboardRequestDto filter)
        {
            var chk = await permission.HasPermission(1027, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var items = await accServiceManager.AccReportsService.GetDashboardData(filter);
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    var final = res.ToList();
                    return Ok(await Result<List<DashboardResultDto>>.SuccessAsync(final, ""));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<DashboardResultDto>.FailAsync($"======= Exp in Search Dashboard Result  , MESSAGE: {ex.Message}"));
            }
        }



        #endregion ================================== قرير احصائي

    }
}





