using Logix.Application.Interfaces.IRepositories.ACC;
using Logix.Domain.ACC;
using Logix.Infrastructure.DbContexts;


namespace Logix.Infrastructure.Repositories.ACC
{
    public class AccJournalMasterFileRepository : GenericRepository<AccJournalMasterFile>, IAccJournalMasterFileRepository
    {
        private readonly ApplicationDbContext context;

        public AccJournalMasterFileRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }

    }
}

