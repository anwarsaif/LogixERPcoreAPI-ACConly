using Logix.Application.Interfaces.IRepositories.ACC;
using Logix.Domain.ACC;
using Logix.Infrastructure.DbContexts;

namespace Logix.Infrastructure.Repositories.ACC
{
    public class AccPettyCashExpensesTypeRepository : GenericRepository<AccPettyCashExpensesType>, IAccPettyCashExpensesTypeRepository
    {
        private readonly ApplicationDbContext context;

        public AccPettyCashExpensesTypeRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }
       

    }
}
