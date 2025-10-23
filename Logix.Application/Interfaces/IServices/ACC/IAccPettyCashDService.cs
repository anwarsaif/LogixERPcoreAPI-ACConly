using Logix.Application.DTOs.ACC;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;

namespace Logix.Application.Interfaces.IServices.ACC
{
    public interface IAccPettyCashDService : IGenericQueryService<AccPettyCashDDto, AccPettyCashDVw>, IGenericWriteService<AccPettyCashDDto, AccPettyCashDEditDto>
    {
        Task<IResult<IEnumerable<AccPettyCashDVw>>> GetAllVW(CancellationToken cancellationToken = default);

    }
}
