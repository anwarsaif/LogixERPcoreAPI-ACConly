using AutoMapper;
using Logix.Application.DTOs.ACC;
using Logix.Domain.ACC;

namespace Logix.Infrastructure.Mapping.ACC
{
    public class AccJournalCommentProfile : Profile
    {
        public AccJournalCommentProfile()
        {
            CreateMap<AccJournalCommentDto, AccJournalComment>().ReverseMap();
        }
    }
}
