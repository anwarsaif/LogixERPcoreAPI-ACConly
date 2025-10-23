using AutoMapper;
using Logix.Application.DTOs.ACC;
using Logix.Domain.ACC;

namespace Logix.Infrastructure.Mapping.ACC
{
    public class AccReceivablesPayablesTransactionDProfile : Profile
    {
        public AccReceivablesPayablesTransactionDProfile()
        {
            CreateMap<AccReceivablesPayablesTransactionDDto, AccReceivablesPayablesTransactionD>().ReverseMap();
            CreateMap<AccReceivablesPayablesTransactionDEditDto, AccReceivablesPayablesTransactionD>().ReverseMap();

        }
    }
}
