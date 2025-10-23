using AutoMapper;
using Logix.Application.DTOs.ACC;
using Logix.Application.DTOs.GB;
using Logix.Domain.ACC;

namespace Logix.Infrastructure.Mapping.ACC
{
    public class AccFacilitiesVwProfile : Profile
    {
        public AccFacilitiesVwProfile()
        {
            CreateMap<AccFacilitiesVwDto, AccFacilitiesVw>().ReverseMap();

        }
    }

}
