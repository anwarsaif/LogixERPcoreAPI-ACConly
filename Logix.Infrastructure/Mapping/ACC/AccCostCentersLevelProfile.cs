using AutoMapper;
using Logix.Application.DTOs.ACC;
using Logix.Domain.ACC;

namespace Logix.Infrastructure.Mapping.ACC
{
    public class AccCostCentersLevelProfile : Profile
    {
        public AccCostCentersLevelProfile()
        {
            CreateMap<AccCostCentersLevelDto, AccCostCentersLevel>().ReverseMap();
            CreateMap<AccCostCentersLevelEditDto, AccCostCentersLevel>().ReverseMap();


        }


    }
}
