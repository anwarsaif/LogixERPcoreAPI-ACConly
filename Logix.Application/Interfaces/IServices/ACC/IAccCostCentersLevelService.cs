using Logix.Application.DTOs.ACC;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;

namespace Logix.Application.Interfaces.IServices.ACC
{
    public interface IAccCostCentersLevelService : IGenericQueryService<AccCostCentersLevelDto, AccCostCentersLevel>, IGenericWriteService<AccCostCentersLevelDto, AccCostCentersLevelEditDto>

    {
        Task<IResult> Updatedigit(long level, int digit, CancellationToken cancellationToken = default);
    }
}
