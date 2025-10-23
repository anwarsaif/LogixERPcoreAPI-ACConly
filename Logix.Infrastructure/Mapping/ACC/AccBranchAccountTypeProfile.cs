using AutoMapper;
using Logix.Application.DTOs.ACC;
using Logix.Domain.ACC;

namespace Logix.Infrastructure.Mapping.ACC
{
    public class AccBranchAccountTypeProfile : Profile
    {
        public AccBranchAccountTypeProfile()
        {
            CreateMap<AccBranchAccountTypeDto, AccBranchAccountType>().ReverseMap();
        }
    }
}
