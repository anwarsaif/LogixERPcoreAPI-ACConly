using Logix.Application.Interfaces.IRepositories.ACC;
using Logix.Domain.ACC;
using Logix.Infrastructure.DbContexts;

namespace Logix.Infrastructure.Repositories.ACC
{
    public class AccCashOnHandRepository : GenericRepository<AccCashOnHand, AccCashOnHandVw>, IAccCashOnHandRepository
    {
        private readonly ApplicationDbContext context;

        public AccCashOnHandRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }


    }
    

}
