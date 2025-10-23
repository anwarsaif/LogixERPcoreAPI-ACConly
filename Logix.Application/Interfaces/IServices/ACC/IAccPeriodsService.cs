using Logix.Application.DTOs.ACC;
using Logix.Application.DTOs.Main;
using Logix.Application.Interfaces.IRepositories;
using Logix.Domain.ACC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logix.Application.Interfaces.IServices.ACC
{
    public interface IAccPeriodsService : IGenericQueryService<AccPeriodsDto, AccPeriodDateVws>, IGenericWriteService<AccPeriodsDto, AccPeriodsEditDto>
    {
        Task<bool> CheckDateInPeriod(long PeriodId, string Date);
       
    }
}
