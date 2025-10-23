using AutoMapper;
using Logix.Application.DTOs.ACC;
using Logix.Application.DTOs.GB;
using Logix.Domain.ACC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logix.Infrastructure.Mapping.ACC
{
    public class AccJournalDetaileProfile : Profile
    {
        public AccJournalDetaileProfile()
        {
            CreateMap<AccJournalDetaileDto, AccJournalDetaile>().ReverseMap();
            CreateMap<AccJournalDetaileEditDto, AccJournalDetaile>().ReverseMap();
            
    }
}
}
