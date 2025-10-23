using AutoMapper;
using Logix.Application.DTOs.ACC;
using Logix.Application.DTOs.GB;
using Logix.Domain.ACC;

namespace Logix.Infrastructure.Mapping.ACC
{
    public class AccBankProfile : Profile
    {
        public AccBankProfile()
        {
            CreateMap<AccBankDto, AccBank>().ReverseMap();
            CreateMap<AccBankEditDto, AccBank>().ReverseMap();
        }
    }
}
