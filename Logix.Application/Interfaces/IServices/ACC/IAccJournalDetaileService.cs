using Logix.Application.DTOs.ACC;
using Logix.Application.Services;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logix.Application.Interfaces.IServices.ACC
{
    public interface IAccJournalDetaileService : IGenericQueryService<AccJournalDetaileDto, AccJournalDetailesVw>, IGenericWriteService<AccJournalDetaileDto, AccJournalDetaileEditDto>

    {
        Task<IResult<AccJournalDetailesVw>> SelectACCJournalDetailesDebitor(long JID);
        Task<IResult<AccJournalDetailesVw>> SelectACCJournalDetailesCredit(long JID);
    

    }
}
