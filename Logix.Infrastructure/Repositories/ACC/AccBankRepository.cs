using Logix.Application.Interfaces.IRepositories.ACC;
using Logix.Domain.ACC;
using Logix.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Logix.Infrastructure.Repositories.ACC
{
    public class AccBankRepository : GenericRepository<AccBank, AccBankVw>, IAccBankRepository
    {
        private readonly ApplicationDbContext context;

        public AccBankRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }
       

    }
    

}
