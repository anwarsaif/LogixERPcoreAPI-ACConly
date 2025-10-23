using Logix.Application.DTOs.ACC;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;

namespace Logix.Application.Interfaces.IServices.ACC
{
    public interface IAccBranchAccountService : IGenericService<AccBranchAccountDto>
    {
        Task<IResult<IEnumerable<AccBranchAccountsVwsDto>>> GetAllVW(CancellationToken cancellationToken = default);
        Task<IResult<IEnumerable<AccBranchAccountsVwsDto>>> Update(IEnumerable<AccBranchAccountsVwsDto> entities, CancellationToken cancellationToken = default);
    }
}
