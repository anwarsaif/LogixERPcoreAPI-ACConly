using Logix.Application.Interfaces.IRepositories.ACC;
using Logix.Domain.ACC;
using Logix.Infrastructure.DbContexts;

namespace Logix.Infrastructure.Repositories.ACC
{
    public class AccBankChequeBookRepository : GenericRepository<AccBankChequeBook>, IAccBankChequeBookRepository
    {
        private readonly ApplicationDbContext context;

        public AccBankChequeBookRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }
       

    }
}
