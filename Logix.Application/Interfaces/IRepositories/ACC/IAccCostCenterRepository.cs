
using Logix.Application.DTOs.ACC;
using Logix.Domain.ACC;

namespace Logix.Application.Interfaces.IRepositories.ACC
{

    public interface IAccCostCenterRepository : IGenericRepository<AccCostCenter, AccCostCenterVws>
    {
        Task<long> GetCostCenterIdByCode(string code, long facilityId);
        Task<CostCenterCodeResult> GetCostCenterCode(long FacilityId, long parentId);
        Task<IEnumerable<AccCostCenterVws>> GetAllVW();
        Task<long> getCostCenterByCode(string code, long facilityId);
    }
}