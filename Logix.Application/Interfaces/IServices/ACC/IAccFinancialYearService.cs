using Logix.Application.DTOs.ACC;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Logix.Application.Interfaces.IServices.ACC
{
    public interface IAccFinancialYearService : IGenericQueryService<AccFinancialYearDto, AccFinancialYearVw>, IGenericWriteService<AccFinancialYearDto, AccFinancialYearEditDto>
    {
        Task<Result<IEnumerable<BalanceSheetFinancialYear>>> GetBalanceSheetData(BalanceSheetFinancialYearFilter entity);
        Task<IResult<ClosingFinancialYearDto>> CreateJournal(ClosingFinancialYearDto entity, CancellationToken cancellationToken = default);
    }
}
