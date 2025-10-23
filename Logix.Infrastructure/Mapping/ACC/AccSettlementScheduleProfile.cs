using AutoMapper;
using Logix.Application.DTOs.ACC;
using Logix.Domain.ACC;

namespace Logix.Infrastructure.Mapping.ACC
{
    public class AccSettlementScheduleProfile : Profile
    {
        public AccSettlementScheduleProfile()
        {
            CreateMap<AccSettlementScheduleDto, AccSettlementSchedule>().ReverseMap();
            CreateMap<AccSettlementScheduleEditDto, AccSettlementSchedule>().ReverseMap();
        }
    }
}
