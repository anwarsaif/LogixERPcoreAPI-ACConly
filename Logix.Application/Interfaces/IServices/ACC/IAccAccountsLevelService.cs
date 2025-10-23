using Logix.Application.DTOs.ACC;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;

namespace Logix.Application.Interfaces.IServices.ACC
{
    public interface IAccAccountsLevelService : IGenericQueryService<AccAccountsLevelDto, AccAccountsLevel>, IGenericWriteService<AccAccountsLevelDto, AccAccountsLevelEditDto>

    {
        Task<IResult> Updatedigit(long level, int digit, string Color, CancellationToken cancellationToken = default);
    }
}
