using Logix.Application.Interfaces.IRepositories.ACC;
using Logix.Domain.ACC;
using Logix.Infrastructure.DbContexts;

namespace Logix.Infrastructure.Repositories.ACC
{
    public class AccAccountsCostcenterVWRepository : GenericRepository<AccAccountsCostcenterVw>, IAccAccountsCostcenterVWRepository
    {
        private readonly ApplicationDbContext context;

        public AccAccountsCostcenterVWRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }
      

    }
}
