using Logix.Application.Interfaces.IRepositories.ACC;
using Logix.Domain.ACC;
using Logix.Infrastructure.DbContexts;

namespace Logix.Infrastructure.Repositories.ACC
{
    public class AccAccountsSubHelpeVwRepository : GenericRepository<AccAccountsSubHelpeVw>, IAccAccountsSubHelpeVwRepository
    {
        private readonly ApplicationDbContext context;

        public AccAccountsSubHelpeVwRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }

    }

}
