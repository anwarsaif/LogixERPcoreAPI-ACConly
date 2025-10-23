using Logix.Application.DTOs.ACC;
using Logix.Application.Wrapper;

namespace Logix.Application.Interfaces.IRepositories.ACC
{
    public interface IAccReportsRepository

    {
        Task<IEnumerable<AccountBalanceSheetDto>> GetAccounttransactionsCurrentYearBalance(AccounttransactionsDto obj);
        Task<IEnumerable<AccountBalanceSheetDto>> GetAccounttransactionsAllYearBalance(AccounttransactionsDto obj);
        Task<IEnumerable<AccountBalanceSheetDto>> GetCustomerCurrentYearBalance(AccounttransactionsDto obj);
        Task<IEnumerable<AccountBalanceSheetDto>> GetCustomerAllYearBalance(AccounttransactionsDto obj);
        Task<IEnumerable<AccountBalanceSheetDto>> GetSupplierCurrentYearBalance(AccounttransactionsDto obj);
        Task<IEnumerable<AccountBalanceSheetDto>> GetSupplierAllYearBalance(AccounttransactionsDto obj);
        Task<IEnumerable<AccounttransactionsGroupDto>> GetAccounttransactionsGroup(AccounttransactionsFilterGroupDto obj);
        Task<IEnumerable<CostcentertransactionsDto>> GetCostcentertransactions(CostcentertransactionsFilterDto obj);
        Task<IEnumerable<AccounttransactionsFromToDto>> GetAccounttransactionsFromTo(AccounttransactionsFromToFilterDto obj);

      Task<IEnumerable<AccountBalanceSheetDto>> GetAccountStatementTransactionDateYear(AccountTransactionDateFilterDto obj);
        Task<IEnumerable<AccountBalanceSheetDto>> GetAccountStatementTransactionDateAllYear(AccountTransactionDateFilterDto obj);

        Task<IEnumerable<AccountBalanceSheetDto>> GetCurrencytransactionsCurrentYearBalance(CurrencytransactionsFilterDto obj);
        Task<IEnumerable<AccountBalanceSheetDto>> GetCurrencytransactionsAllYearBalance(CurrencytransactionsFilterDto obj);
        Task<IEnumerable<CostcentertransactionsGroupDto>> GetCostcentertransactionsGroup(CostcentertransactionsGroupFilterDto obj);
        
        Task<string> GetCostCenterIds(long parentId);
         Task<IEnumerable<CustomerTransactionDto>> GetCustomerTransactionsFromTo(CustomerTransactionFilterDto obj);
        Task<IEnumerable<CostcenterTransactionsFromToDto>> GetCostcenterTransactionsFromTo(CostcenterTransactionsFromToFilterDto filter);
        Task<IEnumerable<TrialBalanceSheetDtoResult>> GetTrialBalanceSheet(TrialBalanceSheetDto obj);
        Task<IEnumerable<GeneralLedgerDtoResult>> GetGeneralLedger(GeneralLedgerDto obj);
        Task<IEnumerable<IncomeStatementDtoResult>> GetIncomeStatement(IncomeStatementDto obj);
        Task<IEnumerable<IncomeStatementDetailsDtoResult>> IncomeStatementDetails(IncomeStatementDetailsDto obj);
        //Task<IEnumerable<FinancialCenterListDtoResult>> FinancialCenterList(FinancialCenterListDto obj);

        Task<IEnumerable<FinancialCenterListBindDataDtoResult>> FinancialCenterListBindData(FinancialCenterListBindDataDto obj);


    }
}