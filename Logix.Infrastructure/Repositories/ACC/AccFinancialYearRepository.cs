using Logix.Application.Interfaces.IRepositories.ACC;
using Logix.Application.Interfaces.IRepositories.Main;
using Logix.Domain.ACC;
using Logix.Domain.Main;
using Logix.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using Polly;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logix.Infrastructure.Repositories.ACC
{
    public class AccFinancialYearRepository : GenericRepository<AccFinancialYear>, IAccFinancialYearRepository
    {
        private readonly ApplicationDbContext context;

        public AccFinancialYearRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }

       

        public async Task<bool> CheckDateInFinancialYear(long FinYear, string Date)
        {
            var finYears = await context.Set<AccFinancialYear>().Where(f => f.FinYear == FinYear && f.IsDeleted == false).AsNoTracking().ToListAsync();
            var finYearRes = finYears.Where(f => !string.IsNullOrEmpty(f.StartDateGregorian) && !string.IsNullOrEmpty(f.EndDateGregorian) &&
                DateTime.ParseExact(Date, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= DateTime.ParseExact(f.StartDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture)
                && DateTime.ParseExact(Date, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= DateTime.ParseExact(f.EndDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture)
             );

            return finYearRes.Any(); //true if contains any financial year, otherWise false
        }
        public async Task<int?> CheckFinyearStatus(long Finyear)
        {
            try
            {
                if (Finyear > 0)
                {
                    return await context.AccFinancialYears.Where(X => X.FinYear == Finyear && X.IsDeleted == false).Select(x => x.FinState).SingleOrDefaultAsync();
                }
                else
                {
                    return 1;
                }
            }
            catch (Exception)
            {
                throw new NotImplementedException();
            }
        }
        public async Task<int?> checkClosed(long? ReferenceNo, long? FacilityID)
        {
            int count = await context.AccJournalMasters
                .Where(x => x.DocTypeId == 4 && x.FlagDelete == false && x.FacilityId == FacilityID && x.ReferenceNo == ReferenceNo)
                .Select(x => x.JCode)
                .CountAsync();

            return count > 0 ? count : (int?)null;
        }

    }
}
