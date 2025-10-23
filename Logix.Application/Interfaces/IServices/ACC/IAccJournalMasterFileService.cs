using Logix.Application.DTOs.ACC;
using Logix.Domain.ACC;

namespace Logix.Application.Interfaces.IServices.ACC
{
    public interface IAccJournalMasterFileService : IGenericQueryService<AccJournalMasterFileDto, AccJournalMasterFilesVw>, IGenericWriteService<AccJournalMasterFileDto, AccJournalMasterFileEditDto>
    {

    }
}
