
using Logix.Domain.ACC;
using Logix.Domain.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logix.Application.Interfaces.IRepositories.ACC
{

    public interface IAccFinancialYearRepository : IGenericRepository<AccFinancialYear>
    {
        Task<bool> CheckDateInFinancialYear(long FinYear, string Date);
        Task<int?> CheckFinyearStatus(long Finyear);
        Task<int?> checkClosed(long? ReferenceNo, long? FacilityID);

    }
}