using Logix.Application.DTOs.ACC;
using Logix.Application.DTOs.HR;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;

namespace Logix.Application.Interfaces.IServices.ACC
{


    public interface IAccAccountService : IGenericQueryService<AccAccountDto, AccAccountsVw>, IGenericWriteService<AccAccountDto, AccAccountEditDto>

    {
        Task<IResult> UpdateParentId(long Id, CancellationToken cancellationToken = default);
        Task<IResult<List<AccountBalanceSheetDto>>> GetEmployeeAccountTransactionsForCurrentYear(AccountBalanceSheetFilterDto filter);
        Task<IResult<List<AccountBalanceSheetDto>>> GetEmployeeAccountTransactionsForAllYears(AccountBalanceSheetFilterDto filter);
        Task<IResult<List<AccountFromToFilterDto>>> GetEmployeeAccountTransactionsFromTo(AccountFromToFilterDto filter);
        Task<bool> ISHelpAccount(string code, long facilityId);
        Task<IResult<IEnumerable<AccAccountsVw>>> Search(AccAccountFilterDto filter, CancellationToken cancellationToken = default);

        AccAccountFilterDto GetAccAccountFilter(Dictionary<string, string> dictionary);
        Task<IResult<List<AccAccountResultExcelDto>>> SaveAccountsExcel(List<AccAccountResultExcelDto> items, CancellationToken cancellationToken = default);

        Task<IResult> DeleteAllAccAccounts(CancellationToken cancellationToken = default);
    Task<IResult<List<AccountBalanceSheetDto>>> AccountTransactionsSearch(AccountBalanceSheetFilterDto filter, CancellationToken cancellationToken = default);

  }
}
