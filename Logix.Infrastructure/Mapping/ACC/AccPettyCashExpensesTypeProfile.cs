using AutoMapper;
using Logix.Application.DTOs.ACC;
using Logix.Domain.ACC;

namespace Logix.Infrastructure.Mapping.ACC
{
    public class AccPettyCashExpensesTypeProfile : Profile
    {
        public AccPettyCashExpensesTypeProfile()
        {
            CreateMap<AccPettyCashExpensesTypeDto, AccPettyCashExpensesType>().ReverseMap();
            CreateMap<AccPettyCashExpensesTypeEditDto, AccPettyCashExpensesType>().ReverseMap();
        }


    }
}
