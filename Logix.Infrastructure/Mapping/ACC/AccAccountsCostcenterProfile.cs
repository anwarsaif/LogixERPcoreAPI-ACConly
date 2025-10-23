using AutoMapper;
using Logix.Application.DTOs.ACC;
using Logix.Domain.ACC;

namespace Logix.Infrastructure.Mapping.ACC
{
    public class AccAccountsCostcenterProfile : Profile
    {
        public AccAccountsCostcenterProfile()
        {
            CreateMap<AccAccountsCostcenterDto, AccAccountsCostcenter>().ReverseMap();
            CreateMap<AccAccountsCostcenterEditDto, AccAccountsCostcenter>().ReverseMap();
        }
    }
}
