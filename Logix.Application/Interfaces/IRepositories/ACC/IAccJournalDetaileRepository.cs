using Logix.Application.DTOs.ACC;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;
using System.Linq.Expressions;

namespace Logix.Application.Interfaces.IRepositories.ACC
{
    public interface IAccJournalDetaileRepository : IGenericRepository<AccJournalDetaile, AccJournalDetailesVw>
    {
        Task<IEnumerable<AccJournalDetailesVw>> GetAllFromView(Expression<Func<AccJournalDetailesVw, bool>> expression);

        Task<IResult<AccJournalDetaile>> AddAccJournalDetail(AccJournalDetaileDto entity, CancellationToken cancellationToken = default);
        Task<IResult<AccJournalDetaile>> UpdateAccJournalDetail(AccJournalDetaileDto entity, CancellationToken cancellationToken = default);
        Task<IResult<AccJournalDetailesVw>> SelectACCJournalDetailesDebitor(long JID);
        Task<IResult<AccJournalDetailesVw>> SelectACCJournalDetailesCredit(long JID);
        Task<List<AccJournalDetaile>> SelectACCJournalDetailesFacilityByID(long JId);
        Task<List<long>> GetJournalIds(string jIds, long FacilityId);
        Task<bool> AccountHasTransactions(long? accountId);
        Task<int> GetCountAccJournalDetailes(long facilityId, CancellationToken cancellationToken = default);
        Task<bool> ValdJournalDetailes(string accAccountCode, string costCenterCode);
    }
}
