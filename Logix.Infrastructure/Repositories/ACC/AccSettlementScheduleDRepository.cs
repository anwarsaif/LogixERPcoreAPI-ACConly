using Logix.Application.Interfaces.IRepositories.ACC;
using Logix.Domain.ACC;
using Logix.Infrastructure.DbContexts;

namespace Logix.Infrastructure.Repositories.ACC
{
    public class AccSettlementScheduleDRepository : GenericRepository<AccSettlementScheduleD>, IAccSettlementScheduleDRepository
    {
        public AccSettlementScheduleDRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
