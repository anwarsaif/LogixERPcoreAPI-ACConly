using Logix.Application.Interfaces.IRepositories.ACC;
using Logix.Domain.ACC;
using Logix.Domain.PM;
using Logix.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Logix.Infrastructure.Repositories.ACC
{
    public class AccReceivablesPayablesTransactionDRepository : GenericRepository<AccReceivablesPayablesTransactionD, AccReceivablesPayablesTransactionDVw>, IAccReceivablesPayablesTransactionDRepository
    {
        private readonly ApplicationDbContext context;

        public AccReceivablesPayablesTransactionDRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }

    }
}
