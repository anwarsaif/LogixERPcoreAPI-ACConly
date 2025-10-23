using Logix.Application.Interfaces.IRepositories.ACC;
using Logix.Domain.ACC;
using Logix.Infrastructure.DbContexts;

namespace Logix.Infrastructure.Repositories.ACC
{
    public class AccJournalCommentRepository : GenericRepository<AccJournalComment>, IAccJournalCommentRepository
    {
        public AccJournalCommentRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
