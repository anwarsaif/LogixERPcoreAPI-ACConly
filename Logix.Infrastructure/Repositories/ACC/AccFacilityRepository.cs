using Logix.Application.Interfaces.IRepositories.ACC;
using Logix.Domain.ACC;
using Logix.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Logix.Infrastructure.Repositories.ACC
{
    public class AccFacilityRepository : GenericRepository<AccFacility>, IAccFacilityRepository
    {
        private readonly ApplicationDbContext context;

        public AccFacilityRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }

        //public async Task<IEnumerable<AccFacilitiesVw>> GetAllVW()
        //{
        //    return await context.AccFacilitiesVws.ToListAsync();
        //}
        public async Task<long> GetPurchasesVATAccount(long facilityId)
        {
            var facility = await context.AccFacilities
                .Where(f => f.FacilityId == facilityId)
                .Select(f => f.PurchasesVatAccountId ?? 0)
                .FirstOrDefaultAsync();


            return facility;
        }
    }
}
