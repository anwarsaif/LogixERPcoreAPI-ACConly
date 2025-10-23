using Logix.Application.Interfaces.IRepositories.ACC;
using Logix.Domain.ACC;
using Logix.Infrastructure.DbContexts;

namespace Logix.Infrastructure.Repositories.ACC
{
    public class AccAccountsLevelRepository : GenericRepository<AccAccountsLevel>, IAccAccountsLevelRepository
    {
        private readonly ApplicationDbContext context;

        public AccAccountsLevelRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }

    }

}
