using Logix.Application.Interfaces.IRepositories.ACC;
using Logix.Domain.ACC;
using Logix.Infrastructure.DbContexts;

namespace Logix.Infrastructure.Repositories.ACC
{
    public class AccCostCentersLevelRepository : GenericRepository<AccCostCentersLevel>, IAccCostCentersLevelRepository
    {
        private readonly ApplicationDbContext context;

        public AccCostCentersLevelRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }

    }

}
