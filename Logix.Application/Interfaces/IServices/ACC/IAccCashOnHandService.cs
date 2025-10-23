using Logix.Application.DTOs.ACC;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;

namespace Logix.Application.Interfaces.IServices.ACC
{
    public interface IAccCashOnHandService : IGenericQueryService<AccCashOnHandDto, AccCashOnHandVw>, IGenericWriteService<AccCashOnHandDto, AccCashOnHandEditDto>

    {
        Task<IResult> UpdateUsersPermission(long ID, string UsersPermission, CancellationToken cancellationToken = default);

    }


}
