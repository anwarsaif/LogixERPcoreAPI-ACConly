
using Logix.Domain.ACC;

namespace Logix.Application.Interfaces.IRepositories.ACC
{
    public interface IAccSettlementInstallmentRepository : IGenericRepository<AccSettlementInstallment>
    {
        Task<AccSettlementInstallmentsVw> GetByIdVW(long Id);
    }

}