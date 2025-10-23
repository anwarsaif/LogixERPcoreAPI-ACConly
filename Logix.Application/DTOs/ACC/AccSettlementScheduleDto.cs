using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logix.Application.DTOs.Main;

namespace Logix.Application.DTOs.ACC
{
    public class AccSettlementScheduleFilterDto
    {
        public string? Description { get; set; }

        [StringLength(10)]
        public string? StartDate { get; set; }
        [StringLength(10)]
        public string? EndDate { get; set; }

    }
    public  class AccSettlementScheduleDto
    {
        
        public long Id { get; set; }
       
        public long? JId { get; set; }
        public string? Description { get; set; }
      
        public int? InstallmentCnt { get; set; }
     
        public decimal? InstallmentValue { get; set; }
  
        [StringLength(10)]
        public string? StartDate { get; set; }
     
        [StringLength(10)]
      
        public int? StatusId { get; set; }
      
        public long? FinYear { get; set; }
       
        public int? DocTypeId { get; set; }
    
        public long? FacilityId { get; set; }
      
        public long? BranchId { get; set; }
      
        public int? CurrencyId { get; set; }
      
        public decimal? ExchangeRate { get; set; }
        public long? Code { get; set; }

        public bool? IsDeleted { get; set; }
        public List<AccSettlementScheduleDDto>? DetailsList { get; set; }
        public List<AccSettlementInstallmentDto>? InstallmentsList { get; set; }
    }

    public class AccSettlementScheduleEditDto
    {
        public long Id { get; set; }

        public long? JId { get; set; }
        public string? Description { get; set; }

        public int? InstallmentCnt { get; set; }

        public decimal? InstallmentValue { get; set; }

        [StringLength(10)]
        public string? StartDate { get; set; }



        public int? StatusId { get; set; }


        public int? DocTypeId { get; set; }


       public long? BranchId { get; set; }

        public int? CurrencyId { get; set; }

        public decimal? ExchangeRate { get; set; }
        public long? Code { get; set; }
        public List<AccSettlementScheduleDDto>? DetailsList { get; set; }
        public List<AccSettlementInstallmentDto>? InstallmentsList { get; set; }

    }


    public class AccJournalSchedulDto
    {
        public long?  Id { get; set; } = 0;

        public string? SelectedId { get; set; }
    }
}
