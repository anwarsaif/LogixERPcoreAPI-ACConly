using Logix.Application.Common;
using Logix.Application.Interfaces.IRepositories.ACC;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;
using Logix.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;

namespace Logix.Infrastructure.Repositories.ACC
{
    public class AccPettyCashDRepository : GenericRepository<AccPettyCashD>, IAccPettyCashDRepository
    {
        private readonly ApplicationDbContext context;
        public AccPettyCashDRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }


        public async Task<IEnumerable<AccPettyCashDVw>> GetAllVW(Expression<Func<AccPettyCashDVw, bool>> expression, CancellationToken cancellationToken = default)
        {
            var result = await context.AccPettyCashDVw.Where(expression).ToListAsync(cancellationToken);
            return result;
        }

        public async Task<List<AccPettyCashDVw>> GetPettyCashDetails2(long PettyCashID)
        {
            var result = await context.AccPettyCashDVw
                .Where(d => d.PettyCashId == PettyCashID && d.IsDeleted == false)
                .GroupBy(d => new { d.AccAccountId, d.CcId })
                .Select(g => new AccPettyCashDVw
                {
                    AccAccountId = g.Key.AccAccountId,
                    CcId = g.Key.CcId,
                    Amount = g.Sum(x => x.Amount),
                    Total = g.Sum(x => x.Total),
                    VatAmount = g.Sum(x => x.VatAmount),
                    Description = context.AccPettyCashD
                        .Where(pd => pd.PettyCashId == PettyCashID && pd.CcId == g.Key.CcId)
                        .Select(pd => pd.Description)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return result;
        }
        public async Task<IEnumerable<AccPettyCashTempVw>> GetPettyCashTemp(Expression<Func<AccPettyCashTempVw, bool>> expression, CancellationToken cancellationToken = default)
        {
            return await context.AccPettyCashTempVw
                .Where(item => !context.AccPettyCashD.Any(d => d.IsDeleted == false && d.TempId == item.Id))
                .Where(expression)
                .OrderBy(item => item.Id)
                .ToListAsync();
        }
    }
}
