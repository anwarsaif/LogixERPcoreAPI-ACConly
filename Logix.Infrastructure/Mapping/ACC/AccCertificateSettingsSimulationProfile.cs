using AutoMapper;
using Logix.Application.DTOs.ACC;
using Logix.Domain.ACC;

namespace Logix.Infrastructure.Mapping.ACC
{
    public class AccCertificateSettingsSimulationProfile : Profile
    {
        public AccCertificateSettingsSimulationProfile()
        {
            CreateMap<AccCertificateSettingsSimulationDto, AccCertificateSettingsSimulation>().ReverseMap();
            CreateMap<AccCertificateSettingsSimulationEditDto, AccCertificateSettingsSimulation>().ReverseMap();
        }
    }
}
