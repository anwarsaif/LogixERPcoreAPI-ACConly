using Logix.Application.Interfaces.IRepositories.ACC;
using Logix.Domain.ACC;
using Logix.Infrastructure.DbContexts;

namespace Logix.Infrastructure.Repositories.ACC
{
    public class AccCertificateSettingRepository : GenericRepository<AccCertificateSetting>, IAccCertificateSettingRepository
    {
        public AccCertificateSettingRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
