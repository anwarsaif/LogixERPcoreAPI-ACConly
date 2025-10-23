using Logix.Application.DTOs.ACC;
using Logix.Application.Wrapper;

namespace Logix.Application.Interfaces.IServices.ACC
{
    
    public interface IAccReportsService 
    {

        Task<IResult<IEnumerable<AccountBalanceSheetDto>>> GetAccounttransactions(AccounttransactionsDto obj);

        Task<IResult<IEnumerable<AccountBalanceSheetDto>>> GetFundsstatementtransactions(FundsstatementtransactionsDto obj);
        Task<IResult<IEnumerable<AccountBalanceSheetDto>>> GetCustomerAccounttransactions(CustomerAccountStatementDto obj);
        Task<IResult<IEnumerable<AccountBalanceSheetDto>>> GetContractorsAccounttransactions(ContractorsAccountStatementDto obj);
        Task<IResult<IEnumerable<AccountBalanceSheetDto>>> GetSupplierAccounttransactions(SupplierAccountStatementDto obj);

        Task<IResult<IEnumerable<AccounttransactionsGroupDto>>> GetAccounttransactionsGroup(AccounttransactionsFilterGroupDto obj);
       Task<IResult<IEnumerable<CostcentertransactionsDto>>> GetCostcentertransactions(CostcentertransactionsFilterDto obj);
        Task<IResult<IEnumerable<AccounttransactionsFromToDto>>> GetAccounttransactionsFromTo(AccounttransactionsFromToFilterDto obj);

        Task<IResult<IEnumerable<AccountBalanceSheetDto>>> GetAccountStatementTransactionDate(AccountTransactionDateFilterDto obj);
        Task<IResult<IEnumerable<AccountBalanceSheetDto>>> GetCurrencytransactions(CurrencytransactionsFilterDto obj);

        Task<IResult<IEnumerable<CostcentertransactionsGroupDto>>> GetCostcentertransactionsGroup(CostcentertransactionsGroupFilterDto obj);
        Task<IResult<IEnumerable<CostcenterTransactionsFromToDto>>> GetCostcenterTransactionsFromTo(CostcenterTransactionsFromToFilterDto filter);

        Task<IResult<IEnumerable<TrialBalanceSheetDtoResult>>> GetTrialBalanceSheet(TrialBalanceSheetDto obj);


        Task<IResult<IEnumerable<CustomerTransactionDto>>> GetCustomerTransactionsFromTo(CustomerTransactionFilterDto obj);
        Task<IResult<IEnumerable<GeneralLedgerDtoResult>>> GetGeneralLedger(GeneralLedgerDto obj);

        Task<IResult<IEnumerable<IncomeStatementDtoResult>>> GetIncomeStatement(IncomeStatementDto obj);
        Task<IResult<IEnumerable<IncomeStatementDetailsDtoResult>>> IncomeStatementDetails(IncomeStatementDetailsDto obj);
        //Task<IResult<IEnumerable<FinancialCenterListDtoResult>>> FinancialCenterList(FinancialCenterListDto obj);
        Task<IResult<IEnumerable<FinancialCenterListBindDataDtoResult>>> FinancialCenterListBindData(FinancialCenterListBindDataDto obj);
        Task<IResult<IEnumerable<IncomeStatementMonthResultDto>>> GetIncomeStatementMonth(IncomeStatementMonthtDto obj);
        Task<IResult<IEnumerable<ProfitandLossResultDto>>> GetProfitandLoss(ProfitandLossDto obj);
       
        Task<IResult<IEnumerable<CashFlowsResultDto>>> GetCashFlows(CashFlowsDto obj);

        Task<IResult<IEnumerable<AgedReceivablesResultDto>>> GetAgedReceivables(AgedReceivablesDto obj);

        Task<IResult<IEnumerable<AgedReceivablesMonthlyResultDto>>> GetAgedReceivablesMonthly(AgedReceivablesMonthlyDto obj);
        Task<IResult<IEnumerable<CompareyearsDtoResultDto>>> GetBudgetEstimateCompareyears(CompareyearsDto obj);
        Task<IResult<IEnumerable<DashboardResultDto>>> GetDashboardData(DashboardRequestDto obj);


    }
}
