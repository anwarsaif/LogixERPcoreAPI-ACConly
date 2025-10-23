using AutoMapper;
using Logix.Application.DTOs.ACC;
using Logix.Domain.ACC;

namespace Logix.Infrastructure.Mapping.ACC
{
    public class AccCertificateSettingProfile : Profile
    {
        public AccCertificateSettingProfile()
        {
            CreateMap<AccCertificateSettingDto, AccCertificateSetting>().ReverseMap();
            CreateMap<AccCertificateSettingEditDto, AccCertificateSetting>().ReverseMap();
        }
    }
}
