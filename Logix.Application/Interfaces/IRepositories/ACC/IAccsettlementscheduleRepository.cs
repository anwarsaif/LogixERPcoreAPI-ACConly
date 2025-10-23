
using Logix.Domain.ACC;

namespace Logix.Application.Interfaces.IRepositories.ACC
{
    public interface IAccsettlementscheduleRepository : IGenericRepository<AccSettlementSchedule>
    {
        Task<int> GetCountSettlement(long facilityId, string startDate, string endDate);

    }

}