using AutoMapper;
using Logix.Application.DTOs.ACC;
using Logix.Domain.ACC;

namespace Logix.Infrastructure.Mapping.ACC
{
    public class AccFacilityProfile : Profile
    {
        public AccFacilityProfile()
        {
            CreateMap<AccFacilityDto, AccFacility>().ReverseMap();
            CreateMap<AccFacilityEditDto, AccFacility>().ReverseMap();
            //CreateMap<AccFacilityEditFileDto, AccFacility>().ReverseMap();
            CreateMap<AccFacilityEditProfileDto, AccFacility>().ReverseMap();
            CreateMap<AccFacilityProfileDto, AccFacility>().ReverseMap();

        }
    }
}
