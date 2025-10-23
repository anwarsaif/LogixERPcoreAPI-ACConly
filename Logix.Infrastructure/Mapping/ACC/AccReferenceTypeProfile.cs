using AutoMapper;
using Logix.Application.DTOs.ACC;
using Logix.Domain.ACC;

namespace Logix.Infrastructure.Mapping.ACC
{
    public class AccReferenceTypeProfile : Profile
    {
        public AccReferenceTypeProfile()
        {
            CreateMap<AccReferenceTypeDto, AccReferenceType>().ReverseMap();
            CreateMap<AccReferenceTypeEditDto, AccReferenceType>().ReverseMap();
        }
    }
}
