using Logix.Application.DTOs.ACC;
using Logix.Domain.ACC;
using System.Linq.Expressions;

namespace Logix.Application.Interfaces.IRepositories.ACC
{
    public interface IAccBranchAccountRepository : IGenericRepository<AccBranchAccount>
    {
        Task<IEnumerable<AccBranchAccountsVw>> GetAllVW();
        Task<IEnumerable<AccBranchAccountsVw>> GetAllVW(Expression<Func<AccBranchAccountsVw, bool>> expression);
        Task<AccBranchAccountsVw?> GetOneVW(Expression<Func<AccBranchAccountsVw, bool>> expression);
    }
}
