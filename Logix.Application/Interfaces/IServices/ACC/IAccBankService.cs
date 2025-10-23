using Logix.Application.DTOs.ACC;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;

namespace Logix.Application.Interfaces.IServices.ACC
{
    public interface IAccBankService : IGenericQueryService<AccBankDto, AccBankVw>, IGenericWriteService<AccBankDto, AccBankEditDto>

    {
        Task<IResult> UpdateUsersPermission(long BankID,string UsersPermission, CancellationToken cancellationToken = default);


    }
}
