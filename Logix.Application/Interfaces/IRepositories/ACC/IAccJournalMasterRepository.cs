using Logix.Application.DTOs.ACC;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;
using System.Linq.Expressions;

namespace Logix.Application.Interfaces.IRepositories.ACC
{
    public interface IAccJournalMasterRepository : IGenericRepository<AccJournalMaster, AccJournalMasterVw>
    {
        Task<IEnumerable<AccJournalMasterVw>> GetAllAccJournalMasterVw();
        int GetCount(Expression<Func<AccJournalMasterVw, bool>> expression);
        Task<int?> GetJournalMasterStatuse(long ReferenceNo, int DocTypeID);

        Task<bool> DeleteJournalWithDetailsbyReference(long ReferenceNo, int DocTypeId);

        Task<IResult<AccJournalMaster>> AddACCJournalMaster(AccJournalMasterDto entity, CancellationToken cancellationToken = default);
        Task<string?> GetJCodeByReferenceNo(long ReferenceNo, int DocTypeID);
        Task<int?> GetPostingStatuse(long facilityId);
        Task<AccJournalMaster> DeleteJournalWithDetailsByJId(long JId);
        Task<IResult<AccJournalMaster>> UpdateACCJournalMaster(AccJournalMasterEditDto entity, CancellationToken cancellationToken = default);
        Task<int?> GetJournalMasterStatuseByJId(long JId);
        Task<int> NumberExists(string referenceCode, long ccId, int docTypeId, long facilityId, long periodId);
        Task<int> GetBookSerial(long facilityId, long branchId, int DocTypeId);
        Task<decimal> GetBalanceForAccount(long accAccountId, long facilityId, long finYear);
        Task<AccJournalMaster> SelectACCJournalFacilityByID(long JId);
        Task<long> GetJIDByJCode2(string JCode, int DocTypeId, long facilityId, long Finyear);
        Task<IResult<AccJournalMaster>> TransferOprations(AccJournalMasterEditDto entity, CancellationToken cancellationToken = default);
        Task<int> CheckFinyearHasTransaction(long FinYear);
        Task<int> CheckPreiodHasTransaction(long periodId);
        Task<int> CheckCostCenterHasTransaction(long CCID);


    }
}