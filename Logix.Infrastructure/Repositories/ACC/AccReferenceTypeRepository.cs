using Logix.Application.Interfaces.IRepositories.ACC;
using Logix.Domain.ACC;
using Logix.Infrastructure.DbContexts;

namespace Logix.Infrastructure.Repositories.ACC
{
    public class AccReferenceTypeRepository : GenericRepository<AccReferenceType>, IAccReferenceTypeRepository
    {
        public AccReferenceTypeRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
