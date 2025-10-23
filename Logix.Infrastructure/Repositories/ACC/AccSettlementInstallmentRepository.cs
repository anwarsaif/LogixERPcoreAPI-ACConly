using Logix.Application.Interfaces.IRepositories.ACC;
using Logix.Domain.ACC;
using Logix.Infrastructure.DbContexts;

namespace Logix.Infrastructure.Repositories.ACC
{
    public class AccSettlementInstallmentRepository : GenericRepository<AccSettlementInstallment>, IAccSettlementInstallmentRepository
    {
        private readonly ApplicationDbContext context;

        public AccSettlementInstallmentRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<AccSettlementInstallmentsVw> GetByIdVW(long Id)
        {
            return await context.Set<AccSettlementInstallmentsVw>().FindAsync(Id);
        }

    }

}
