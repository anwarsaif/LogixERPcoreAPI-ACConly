using Logix.Application.DTOs.ACC;
using Logix.Domain.ACC;

namespace Logix.Application.Interfaces.IServices.ACC
{
    public interface IAccJournalCommentService : IGenericQueryService<AccJournalCommentDto, AccJournalComment>, IGenericWriteService<AccJournalCommentDto, AccJournalCommentDto>

    {


    }

}
