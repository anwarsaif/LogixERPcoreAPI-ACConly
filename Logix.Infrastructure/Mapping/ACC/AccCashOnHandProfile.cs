using AutoMapper;
using Logix.Application.DTOs.ACC;
using Logix.Domain.ACC;

namespace Logix.Infrastructure.Mapping.ACC
{
    public class AccCashOnHandProfile : Profile
    {
        public AccCashOnHandProfile()
        {
            CreateMap<AccCashOnHandDto, AccCashOnHand>().ReverseMap();
            CreateMap<AccCashOnHandEditDto, AccCashOnHand>().ReverseMap();
        }
    }
    

}
