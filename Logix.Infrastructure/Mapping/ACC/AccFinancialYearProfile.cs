using AutoMapper;
using AutoMapper;
using Logix.Application.DTOs.ACC;
using Logix.Domain.ACC;
using Logix.Domain.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logix.Infrastructure.Mapping.ACC
{
    public class AccFinancialYearProfile : Profile
    {
        public AccFinancialYearProfile() {
            CreateMap<AccFinancialYearDto, AccFinancialYear>().ReverseMap();
            CreateMap<AccFinancialYearEditDto, AccFinancialYear>().ReverseMap();
        }


    }
}
