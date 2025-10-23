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
    public class AccJournalMasterProfile : Profile
    {
        public AccJournalMasterProfile()
        {
            CreateMap<AccJournalMasterDto, AccJournalMaster>().ReverseMap();
            CreateMap<AccJournalMasterEditDto, AccJournalMaster>().ReverseMap();
            CreateMap<AccIncomeDto, AccJournalMaster>().ReverseMap();
            CreateMap<AccIncomeEditDto, AccJournalMaster>().ReverseMap();
            CreateMap<AccExpensesDto, AccJournalMaster>().ReverseMap();
            CreateMap<AccExpensesEditDto, AccJournalMaster>().ReverseMap();
            CreateMap<AccJournalReverseDto, AccJournalMaster>().ReverseMap();
            CreateMap<AccJournalReverseEditDto, AccJournalMaster>().ReverseMap();
            CreateMap<FirstTimeBalanceDto, AccJournalMaster>().ReverseMap();
            CreateMap<FirstTimeBalanceEditDto, AccJournalMaster>().ReverseMap();
            CreateMap<OpeningBalanceDto, AccJournalMaster>().ReverseMap();
            CreateMap<OpeningBalanceEditDto, AccJournalMaster>().ReverseMap();

        }
    }
}
