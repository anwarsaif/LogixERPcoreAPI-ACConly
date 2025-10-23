using AutoMapper;
using Logix.Application.DTOs.ACC;
using Logix.Domain.ACC;

namespace Logix.Infrastructure.Mapping.ACC
{
    public class AccPettyCashDProfile : Profile
    {
        public AccPettyCashDProfile()
        {
            CreateMap<AccPettyCashDDto, AccPettyCashD>().ReverseMap();
            CreateMap<AccPettyCashDEditDto, AccPettyCashD>().ReverseMap();

        }
    }
}
