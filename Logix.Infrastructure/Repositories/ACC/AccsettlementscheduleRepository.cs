using Logix.Application.Interfaces.IRepositories.ACC;
using Logix.Domain.ACC;
using Logix.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Logix.Infrastructure.Repositories.ACC
{
    public class AccsettlementscheduleRepository : GenericRepository<AccSettlementSchedule>, IAccsettlementscheduleRepository
    {
        private readonly ApplicationDbContext context;

        public AccsettlementscheduleRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }
        public async Task<int> GetCountSettlement(long facilityId, string startDate, string endDate)
        {
            try
            {
               
                var countList = context.AccSettlementInstallmentsVw
                    .Where(x => x.IsDeleted==false && x.IsDeletedM == false && x.FacilityId == facilityId && x.StatusId == 1 &&
                           !context.AccJournalMasters.Any(j => j.FlagDelete == false && j.DocTypeId == 33 && j.ReferenceNo == x.Id))
                    .AsEnumerable()
                    .Where(x => DateTime.ParseExact(x.InstallmentDate, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= DateTime.ParseExact(startDate, "yyyy/MM/dd", CultureInfo.InvariantCulture)
                             && DateTime.ParseExact(x.InstallmentDate, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= DateTime.ParseExact(endDate, "yyyy/MM/dd", CultureInfo.InvariantCulture))
                    .Select(x => x.Id)
                    .ToList(); // Materialize the query into a list

                int count = countList.Count; // Get the count of items in the list

                return count;
            }
            catch (Exception)
            {
                throw new NotImplementedException();
            }
        }
    }
}
