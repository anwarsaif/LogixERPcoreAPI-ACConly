using Logix.Application.DTOs.ACC;
using Logix.Domain.ACC;

namespace Logix.Application.Interfaces.IServices.ACC
{
    public interface IAccSettlementInstallmentService : IGenericQueryService<AccSettlementInstallmentDto, AccSettlementInstallmentsVw>, IGenericWriteService<AccSettlementInstallmentDto, AccSettlementInstallmentEditDto>
    {

    }
}
