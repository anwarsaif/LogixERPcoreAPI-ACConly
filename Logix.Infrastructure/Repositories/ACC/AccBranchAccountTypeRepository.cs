using Logix.Application.Interfaces.IRepositories.ACC;
using Logix.Domain.ACC;
using Logix.Infrastructure.DbContexts;

namespace Logix.Infrastructure.Repositories.ACC
{
    public class AccBranchAccountTypeRepository : GenericRepository<AccBranchAccountType>, IAccBranchAccountTypeRepository
    {
        public AccBranchAccountTypeRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
