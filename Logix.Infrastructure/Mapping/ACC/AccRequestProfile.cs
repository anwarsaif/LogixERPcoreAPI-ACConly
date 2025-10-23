using AutoMapper;
using Logix.Application.DTOs.ACC;
using Logix.Application.DTOs.GB;
using Logix.Domain.ACC;

namespace Logix.Infrastructure.Mapping.ACC
{
    public class AccRequestProfile : Profile
    {
        public AccRequestProfile()
        {
            CreateMap<AccRequestDto, AccRequest>().ReverseMap();
            CreateMap<AccRequestEditDto, AccRequest>().ReverseMap();
            
                     CreateMap<AccRequestPaymentDto, AccRequest>().ReverseMap();
            CreateMap<AccRequestPaymentEditDto, AccRequest>().ReverseMap();

        }
    }
    
}
