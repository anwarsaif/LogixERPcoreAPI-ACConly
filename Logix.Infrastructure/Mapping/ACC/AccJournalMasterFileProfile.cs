using AutoMapper;
using Logix.Application.DTOs.ACC;
using Logix.Domain.ACC;

namespace Logix.Infrastructure.Mapping.ACC
{
    public class AccJournalMasterFileProfile : Profile
    {
        public AccJournalMasterFileProfile()
        {
            CreateMap<AccJournalMasterFileDto, AccJournalMasterFile>().ReverseMap();
            CreateMap<AccJournalMasterFileEditDto, AccJournalMasterFile>().ReverseMap();
        }
    }


}
