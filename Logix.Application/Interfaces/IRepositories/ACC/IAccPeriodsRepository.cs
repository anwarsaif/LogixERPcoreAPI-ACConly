
using Logix.Domain.ACC;
using Logix.Domain.Main;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logix.Application.Interfaces.IRepositories.ACC
{

    public interface IAccPeriodsRepository : IGenericRepository<AccPeriods>
    {
        Task<bool> CheckDateInPeriod(long PeriodId, string Date);
        Task<bool> CheckDateInPeriodByYear(long FinYear, string Date);
        Task<long> GetPreiodIDByDate(string Date, long FacilityId);
       

    }
}