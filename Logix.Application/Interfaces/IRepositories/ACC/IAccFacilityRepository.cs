using Logix.Application.Wrapper;
using Logix.Domain.ACC;
using System.Linq.Expressions;

namespace Logix.Application.Interfaces.IRepositories.ACC
{
    public interface IAccFacilityRepository : IGenericRepository<AccFacility>
    {
        //Task<IEnumerable<AccFacilitiesVw>> GetAllVW();
        Task<long> GetPurchasesVATAccount(long facilityId);
    }
}
