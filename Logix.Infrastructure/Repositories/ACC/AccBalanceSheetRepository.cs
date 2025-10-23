using Logix.Application.Interfaces.IRepositories.ACC;
using Logix.Domain.ACC;
using Logix.Infrastructure.DbContexts;

namespace Logix.Infrastructure.Repositories.ACC
{
    public class AccBalanceSheetRepository : GenericRepository<AccBalanceSheet>, IAccBalanceSheetRepository
    {
        private readonly ApplicationDbContext context;

        public AccBalanceSheetRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }
      

    }
    
}
