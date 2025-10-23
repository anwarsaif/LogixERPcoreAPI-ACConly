using Logix.Application.DTOs.ACC;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;

namespace Logix.Application.Interfaces.IServices.ACC
{
    public interface IAccsettlementscheduleService : IGenericQueryService<AccSettlementScheduleDto, AccSettlementSchedule>, IGenericWriteService<AccSettlementScheduleDto, AccSettlementScheduleEditDto>
    {
        Task<IResult<AccJournalSchedulDto>> CreateJournal(AccJournalSchedulDto entity, CancellationToken cancellationToken = default);

    }
}
