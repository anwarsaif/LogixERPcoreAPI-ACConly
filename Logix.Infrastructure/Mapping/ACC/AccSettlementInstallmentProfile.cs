using AutoMapper;
using Logix.Application.DTOs.ACC;
using Logix.Domain.ACC;

namespace Logix.Infrastructure.Mapping.ACC
{
    public class AccSettlementInstallmentProfile : Profile
    {
        public AccSettlementInstallmentProfile()
        {
            CreateMap<AccSettlementInstallmentDto, AccSettlementInstallment>().ReverseMap();
            CreateMap<AccSettlementInstallmentEditDto, AccSettlementInstallment>().ReverseMap();
        }
    }
}
