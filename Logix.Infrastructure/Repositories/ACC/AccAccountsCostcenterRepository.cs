using Logix.Application.Interfaces.IRepositories;
using Logix.Application.Interfaces.IRepositories.ACC;
using Logix.Domain.ACC;
using Logix.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Logix.Infrastructure.Repositories.ACC
{
    public class AccAccountsCostcenterRepository : GenericRepository<AccAccountsCostcenter>, IAccAccountsCostcenterRepository
    {
        private readonly ApplicationDbContext context;

        public AccAccountsCostcenterRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<long> GetCostCenterIdByCode(string code, long facilityId)
        {
            // return 0 if no cost center with this code or an exception occur
            try
            {
                return await context.AccCostCenter
                .Where(c => c.CostCenterCode == code && c.IsDeleted == false && c.FacilityId == facilityId && c.IsActive == true)
                .Select(x => x.CcId).SingleOrDefaultAsync();
            }
            catch
            {
                return 0;
            }
        }
    }
}