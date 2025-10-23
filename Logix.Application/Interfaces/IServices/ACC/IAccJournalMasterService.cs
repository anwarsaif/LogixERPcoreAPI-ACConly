using Logix.Application.DTOs.ACC;
using Logix.Application.DTOs.SAL;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logix.Application.Interfaces.IServices.ACC
{
    public interface IAccJournalMasterService : IGenericQueryService<AccJournalMasterDto, AccJournalMasterVw>, IGenericWriteService<AccJournalMasterDto, AccJournalMasterEditDto>

    {
        Task<IResult<AccJournalMasterDto>> AddDetaile(AccJournalMasterDtoVW entity, CancellationToken cancellationToken = default);
        Task<IResult<AccJournalMasterEditDto>> UpdateDetaile(AccJournalMasterEditDtoVW entity, CancellationToken cancellationToken = default);
        Task<IResult<AccIncomeDto>> AddIncome(AccIncomeDto entity, CancellationToken cancellationToken = default);
        Task<IResult<AccIncomeEditDto>> UpdateIncome(AccIncomeMasterEditDtoVW entity, CancellationToken cancellationToken = default);
        Task<IResult<AccExpensesDto>> AddExpenses(AccExpensesDto entity, CancellationToken cancellationToken = default);
        Task<IResult<AccExpensesEditDto>> UpdateExpenses(AccExpensesMasterEditDtoVW entity, CancellationToken cancellationToken = default);
        Task<IResult<AccJournalReverseDto>> AddJournalReverse(AccJournalReverseDtoVW entity, CancellationToken cancellationToken = default);
        Task<IResult<AccJournalReverseEditDto>> UpdateJournalReverse(AccExpensesMasterEditDtoVW entity, CancellationToken cancellationToken = default);

        Task<long> GetCurrencyID(long AccountType, string code, long facilityId);
        Task<int> GetBookSerial(long facilityId, long branchId, int DocTypeId);
        Task<IResult<AccJournalMasterStatusDto>> UpdateACCJournalComment(AccJournalMasterStatusDto accJournalMasterDtoList, CancellationToken cancellationToken = default);
        Task<IResult<AccJournalMasterStatusDto>> UpdateTransferGeneralTo(AccJournalMasterStatusDto accJournalMasterDtoList, CancellationToken cancellationToken = default);
        Task<IResult<AccJournalMasterStatusDto>> UpdateTransferGeneralAllTo(AccJournalMasterStatusDto accJournalMasterDtoList, CancellationToken cancellationToken = default);
        Task<IResult<FirstTimeBalanceDto>> AddFirstTimeBalances(FirstTimeBalanceDtoVW entity, CancellationToken cancellationToken = default);
        Task<IResult<FirstTimeBalanceEditDto>> UpdateFirstTimeBalance(FirstTimeBalanceEditDtoVW entity, CancellationToken cancellationToken = default);
        Task<IResult<OpeningBalanceDto>> AddOpeningBalance(OpeningBalanceDtoVW entity, CancellationToken cancellationToken = default);
        Task<IResult<OpeningBalanceEditDto>> UpdateOpeningBalance(OpeningBalanceEditDtoVW entity, CancellationToken cancellationToken = default);
        Task<long> GetJIDByJCode2(string JCode, int DocTypeId, long facilityId, long Finyear);

        Task<string?> GetJCodeByReferenceNo(long ReferenceNo, int DocTypeID);
        Task<int?> GetJournalMasterStatuse(long ReferenceNo, int DocTypeID);
        Task<IResult<AccJournalMasterStatusDto>> UpdateTransferGeneralFrom(AccJournalMasterStatusDto accJournalMasterDtoList, CancellationToken cancellationToken = default);
        Task<IResult<AccJournalMasterStatusDto>> UpdateTransferGeneralAllFrom(AccJournalMasterStatusDto accJournalMasterDtoList, CancellationToken cancellationToken = default);
		Task<IResult<List<AccJournalMasterVw>>> Search(AccJournalMasterfilterDto filter, CancellationToken cancellationToken = default);
		Task<IResult<List<AccJournalMasterVw>>> RepIncomeSearch(AccJournalMasterfilterDto filter, CancellationToken cancellationToken = default);
		Task<IResult<List<AccJournalMasterVw>>> RepExpensesSearch(AccJournalMasterfilterDto filter, CancellationToken cancellationToken = default);
		Task<IResult<List<AccJournalMasterfilterDto>>> TransferFromgeneralledgerSearch(AccJournalMasterfilterDto filter, CancellationToken cancellationToken = default);


	}
}
