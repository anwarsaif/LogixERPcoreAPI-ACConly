using AutoMapper;
using Logix.Application.DTOs.ACC;
using Logix.Domain.ACC;

namespace Logix.Infrastructure.Mapping.ACC
{
    public class AccPettyCashProfile : Profile
    {
        public AccPettyCashProfile()
        {
            CreateMap<AccPettyCashDto, AccPettyCash>().ReverseMap();
            CreateMap<AccPettyCashEditDto, AccPettyCash>().ReverseMap();

        }
    }
}
