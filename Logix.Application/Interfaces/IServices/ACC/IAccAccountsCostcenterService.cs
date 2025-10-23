using Logix.Application.DTOs.ACC;
using Logix.Domain.ACC;

namespace Logix.Application.Interfaces.IServices.ACC
{
    public interface IAccAccountsCostcenterService : IGenericQueryService<AccAccountsCostcenterDto, AccAccountsCostcenter>, IGenericWriteService<AccAccountsCostcenterDto, AccAccountsCostcenterEditDto>

    {
    }
}
