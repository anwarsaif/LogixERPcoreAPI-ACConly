using Logix.Application.DTOs.ACC;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;
using System.Linq.Expressions;

namespace Logix.Application.Interfaces.IServices.ACC
{
    public interface IAccPettyCashService : IGenericQueryService<AccPettyCashDto, AccPettyCashVw>, IGenericWriteService<AccPettyCashDto, AccPettyCashEditDto>

    {
        Task<IResult<AccPettyCashDto>> CreateJournal(AccPettyCashDto entity, CancellationToken cancellationToken = default);
        Task<IResult<AccPettyCashDto>> CreateJournal2(AccPettyCashDto entity, CancellationToken cancellationToken = default);
        Task<IResult<IEnumerable<AccPettyCashTempVw>>> GetPettyCashTemp(Expression<Func<AccPettyCashTempVw, bool>> expression);


    }

}
