using Logix.Application.Interfaces.IRepositories.ACC;
using Logix.Domain.ACC;
using Logix.Infrastructure.DbContexts;

namespace Logix.Infrastructure.Repositories.ACC
{
    public class AccPettyCashRepository : GenericRepository<AccPettyCash>, IAccPettyCashRepository
    {
        public AccPettyCashRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
