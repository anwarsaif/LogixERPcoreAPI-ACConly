using Logix.Application.DTOs.ACC;
using Logix.Application.Interfaces.IRepositories.ACC;
using Logix.Domain.ACC;
using Logix.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Logix.Infrastructure.Repositories.ACC
{
    public class AccBranchAccountRepository : GenericRepository<AccBranchAccount>, IAccBranchAccountRepository
    {
        private readonly ApplicationDbContext context;

        public AccBranchAccountRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }
        public async Task<IEnumerable<AccBranchAccountsVw>> GetAllVW()
        {
            return await context.AccBranchAccountsVws.ToListAsync();
        }
        public async Task<IEnumerable<AccBranchAccountsVw>> GetAllVW(Expression<Func<AccBranchAccountsVw, bool>> expression)
        {
            return await context.AccBranchAccountsVws.Where(expression).ToListAsync();
        }
        public async Task<AccBranchAccountsVw?> GetOneVW(Expression<Func<AccBranchAccountsVw, bool>> expression)
        {
            return await context.AccBranchAccountsVws.Where(expression).SingleOrDefaultAsync();
        }

    }
}
