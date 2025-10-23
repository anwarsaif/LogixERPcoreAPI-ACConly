using AutoMapper;
using Logix.Application.DTOs.ACC;
using Logix.Domain.ACC;

namespace Logix.Infrastructure.Mapping.ACC
{
    public class AccSettlementScheduleDProfile : Profile
    {
        public AccSettlementScheduleDProfile()
        {
            CreateMap<AccSettlementScheduleDDto, AccSettlementScheduleD>().ReverseMap();
            CreateMap<AccSettlementScheduleDEditDto, AccSettlementScheduleD>().ReverseMap();
        }
    }
}
