using Logix.Domain.ACC;

namespace Logix.Application.Interfaces.IRepositories.ACC
{
    public interface IAccAccountsCostcenterRepository : IGenericRepository<AccAccountsCostcenter>
    {
        Task<long> GetCostCenterIdByCode(string code, long facilityId);
    }
}