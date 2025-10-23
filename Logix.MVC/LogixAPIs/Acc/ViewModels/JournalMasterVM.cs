
using Logix.Domain.ACC;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Logix.MVC.LogixAPIs.Acc.ViewModels
{
    public class JournalMasterVM2
    {
        public long JId { get; set; }
        public string? JCode { get; set; }
        [StringLength(10)]
        public string? JDateGregorian { get; set; }
        [StringLength(50)]
        public string? DocTypeName { get; set; }
        [StringLength(50)]
        public string? DocTypeName2 { get; set; }
        [Column("BRA_NAME")]
        public string? BraName { get; set; }
        [Column("BRA_NAME2")]
        public string? BraName2 { get; set; }


        [StringLength(50)]
        public string? ReferenceCode { get; set; }

        [StringLength(50)]
        public string? StatusName { get; set; }
        [StringLength(50)]
        public string? StatusName2 { get; set; }
        public long? ReferenceNo { get; set; } = 0;
        public int? DocTypeId { get; set; } = 0;
        [StringLength(50)]
        public string? UserFullname { get; set; }
        public decimal? sumCredit { get; set; } = 0;
        public decimal? sumDebit { get; set; } = 0;





        public int? DeleteUserId { get; set; }


        public bool? IsDeleted { get; set; }



  

  
        public int? InsertUserId { get; set; }
        public DateTime? InsertDate { get; set; }
        public string? CollectionEmpCode { get; set; }

   
        public List<AccJournalDetailesVw> Children { get; set; }
    }

    public class SettlementJournalVM
    {
        public long Id { get; set; }
        public long SsId { get; set; }
        public long? Code { get; set; }
        [StringLength(10)]
        public string? InstallmentDate { get; set; }
        [StringLength(5000)]
        public string? DescriptionM { get; set; }
     
        public decimal? sumCredit { get; set; } = 0;
        public decimal? sumDebit { get; set; } = 0;





        public int? DeleteUserId { get; set; }


        public bool? IsDeleted { get; set; }






        public int? InsertUserId { get; set; }
        public DateTime? InsertDate { get; set; }
        public string? CollectionEmpCode { get; set; }


        public List<AccSettlementScheduleDVw> Children { get; set; }
    }
}
