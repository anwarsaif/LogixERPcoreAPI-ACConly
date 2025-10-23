using AutoMapper;
using Logix.Application.DTOs.ACC;
using Logix.Application.DTOs.GB;
using Logix.Domain.ACC;

namespace Logix.Infrastructure.Mapping.ACC
{
    public class AccAccountProfile : Profile
    {
        public AccAccountProfile()
        {
            CreateMap<AccAccountDto, AccAccount>().ReverseMap();
            CreateMap<AccAccountEditDto, AccAccount>().ReverseMap();
            CreateMap<AccAccountEditDto, AccAccountDto>().ReverseMap();
            CreateMap<SubitemsDto, AccAccount>().ReverseMap();
            CreateMap<SubitemsEditDto, AccAccount>().ReverseMap();
            CreateMap<SubitemsEditDto, SubitemsDto>().ReverseMap();
            CreateMap<AccAccountResultExcelDto, AccAccountDto>().ReverseMap();
            CreateMap<AccAccountResultExcelDto, AccAccount>().ReverseMap();

        }
    }
}
