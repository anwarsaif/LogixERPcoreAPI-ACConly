using Logix.Application.Wrapper;
using Logix.Domain.ACC;
using System.Linq.Expressions;

namespace Logix.Application.Interfaces.IRepositories.ACC
{
    public interface IAccPettyCashDRepository : IGenericRepository<AccPettyCashD>
    {
    
        Task<List<AccPettyCashDVw>> GetPettyCashDetails2(long PettyCashID);
        Task<IEnumerable<AccPettyCashDVw>> GetAllVW(Expression<Func<AccPettyCashDVw, bool>> expression, CancellationToken cancellationToken = default);
        Task<IEnumerable<AccPettyCashTempVw>> GetPettyCashTemp(Expression<Func<AccPettyCashTempVw, bool>> expression, CancellationToken cancellationToken = default);

    }

}
