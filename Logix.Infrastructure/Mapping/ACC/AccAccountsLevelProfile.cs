using AutoMapper;
using Logix.Application.DTOs.ACC;
using Logix.Domain.ACC;

namespace Logix.Infrastructure.Mapping.ACC
{
    public class AccAccountsLevelProfile : Profile
    {
        public AccAccountsLevelProfile()
        {
            CreateMap<AccAccountsLevelDto, AccAccountsLevel>().ReverseMap();
            CreateMap<AccAccountsLevelEditDto, AccAccountsLevel>().ReverseMap();


        }


    }
}
