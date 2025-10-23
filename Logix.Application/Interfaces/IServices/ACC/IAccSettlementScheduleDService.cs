using Logix.Application.DTOs.ACC;
using Logix.Domain.ACC;

namespace Logix.Application.Interfaces.IServices.ACC
{
    public interface IAccSettlementScheduleDService : IGenericQueryService<AccSettlementScheduleDDto, AccSettlementScheduleDVw>, IGenericWriteService<AccSettlementScheduleDDto, AccSettlementScheduleDEditDto>
    {

    }
}
