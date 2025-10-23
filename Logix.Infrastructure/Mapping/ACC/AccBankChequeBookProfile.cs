using AutoMapper;
using Logix.Application.DTOs.ACC;
using Logix.Domain.ACC;

namespace Logix.Infrastructure.Mapping.ACC
{
    public class AccBankChequeBookProfile : Profile
    {
        public AccBankChequeBookProfile()
        {
            CreateMap<AccBankChequeBookDto, AccBankChequeBook>().ReverseMap();
            CreateMap<AccBankChequeBookEditDto, AccBankChequeBook>().ReverseMap();
        }
    }


}
