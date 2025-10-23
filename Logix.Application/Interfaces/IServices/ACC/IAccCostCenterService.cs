using Logix.Application.DTOs.ACC;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;
namespace Logix.Application.Interfaces.IServices.ACC
{
    public interface IAccCostCenterService : IGenericQueryService<AccCostCenterDto, AccCostCenterVws>, IGenericWriteService<AccCostCenterDto, AccCostCenterEditDto>
    {
        Task<IResult> UpdateParentId(long Id, CancellationToken cancellationToken = default);
        Task<IResult<IEnumerable<AccCostCenterVws>>> Search(AccCostCenterFilterDto filter, CancellationToken cancellationToken = default);

        AccCostCenterFilterDto GetAccCostCenterFilter(Dictionary<string, string> dictionary);

    }
}
