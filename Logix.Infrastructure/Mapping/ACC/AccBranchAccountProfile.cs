using AutoMapper;
using Logix.Application.DTOs.ACC;
using Logix.Domain.ACC;

namespace Logix.Infrastructure.Mapping.ACC
{
    public class AccBranchAccountProfile : Profile
    {
        public AccBranchAccountProfile()
        {
            CreateMap<AccBranchAccountDto, AccBranchAccount>().ReverseMap();
            CreateMap<AccBranchAccountsVwsDto, AccBranchAccountsVw>().ReverseMap();
        }
    }
}
