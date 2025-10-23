using Logix.Application.Common;
using Logix.Application.Interfaces.IRepositories;
using Logix.Application.Interfaces.IRepositories.ACC;
using Logix.Application.Interfaces.IRepositories.Main;
using Logix.Domain.ACC;
using Logix.Domain.Main;
using Logix.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Digests;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsappBusiness.CloudApi.Webhook;

namespace Logix.Infrastructure.Repositories.ACC
{
    public class AccPeriodsRepository : GenericRepository<AccPeriods>, IAccPeriodsRepository
    {
        private readonly ApplicationDbContext context;
        private readonly ICurrentData session;

        public AccPeriodsRepository(ApplicationDbContext context, ICurrentData session) : base(context)
        {
            this.context = context;
            this.session = session;
        }

        public async Task<bool> CheckDateInPeriod(long PeriodId, string Date)
        {
            var periods = await context.Set<AccPeriods>().Where(p => p.PeriodId == PeriodId).AsNoTracking().ToListAsync();
            var periodRes = periods.Where(p => !string.IsNullOrEmpty(p.PeriodStartDateGregorian) && !string.IsNullOrEmpty(p.PeriodEndDateGregorian) &&
                DateTime.ParseExact(Date, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= DateTime.ParseExact(p.PeriodStartDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture)
                && DateTime.ParseExact(Date, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= DateTime.ParseExact(p.PeriodEndDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture)
             );

            return periodRes.Any(); //true if contains any period, otherWise false
        }

        public async Task<bool> CheckDateInPeriodByYear(long FinYear, string Date)
        {
            var periods = await context.Set<AccPeriods>().Where(p => p.FinYear == FinYear && p.FlagDelete == false).AsNoTracking().ToListAsync();
            var periodRes = periods.Where(p => !string.IsNullOrEmpty(p.PeriodStartDateGregorian) && !string.IsNullOrEmpty(p.PeriodEndDateGregorian) &&
                DateTime.ParseExact(Date, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= DateTime.ParseExact(p.PeriodStartDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture)
                && DateTime.ParseExact(Date, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= DateTime.ParseExact(p.PeriodEndDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture)
             );

            return periodRes.Any(); //true if contains any period, otherWise false
        }

        public async Task<long> GetPreiodIDByDate(string Date, long FacilityId)
        {
            var periods = await context.Set<AccPeriods>().Where(p => p.PeriodState == 1 && p.FlagDelete == false && p.FacilityId == FacilityId).AsNoTracking().ToListAsync();
            var periodRes = periods.Where(p => !string.IsNullOrEmpty(p.PeriodStartDateGregorian) && !string.IsNullOrEmpty(p.PeriodEndDateGregorian) &&
                DateTime.ParseExact(Date, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= DateTime.ParseExact(p.PeriodStartDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture)
                && DateTime.ParseExact(Date, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= DateTime.ParseExact(p.PeriodEndDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture)
             ).FirstOrDefault();
            if (periodRes != null)
            {
                return periodRes.PeriodId;
            }
            else
            {
                return 0;
            }

        }


    }
}