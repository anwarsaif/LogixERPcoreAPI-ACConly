using Logix.Application.Interfaces.IRepositories.ACC;
using Logix.Application.Interfaces.IRepositories.Main;
using Logix.Domain.ACC;
using Logix.Domain.Main;
using Logix.Infrastructure.DbContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logix.Infrastructure.Repositories.ACC
{
    public class AccGroupRepository : GenericRepository<AccGroup>, IAccGroupRepository
    {
        public AccGroupRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
