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
    public class ACCPeriodsProfile : Profile
    {
        public ACCPeriodsProfile() {
            CreateMap<AccPeriodsDto, AccPeriods>().ReverseMap();
            CreateMap<AccPeriodsEditDto, AccPeriods>().ReverseMap();


           /* CreateMap<AccPeriodsDto, AccPeriods>().ForMember(s => s.CreatedOn, d => d.MapFrom(x => x.InsertDate)).
                ForMember(s => s.IsDeleted, d => d.MapFrom(x => x.FlagDelete))
                .ReverseMap();*/
        }

        
    }
}
