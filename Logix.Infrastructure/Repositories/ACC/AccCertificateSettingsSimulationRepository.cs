using Logix.Application.Interfaces.IRepositories.ACC;
using Logix.Domain.ACC;
using Logix.Infrastructure.DbContexts;

namespace Logix.Infrastructure.Repositories.ACC
{
    public class AccCertificateSettingsSimulationRepository : GenericRepository<AccCertificateSettingsSimulation>, IAccCertificateSettingsSimulationRepository
    {
        public AccCertificateSettingsSimulationRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
