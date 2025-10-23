using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logix.Domain.Base;

namespace Logix.Domain.ACC
{
    [Table("Acc_Settlement_Schedule")]
    public partial class AccSettlementSchedule:TraceEntity
    {
        [Key]
        [Column("ID")]
        public long Id { get; set; }
        [Column("J_ID")]
        public long? JId { get; set; }
        public string? Description { get; set; } 
        [Column("Installment_Cnt")]
        public int? InstallmentCnt { get; set; }
        [Column("Installment_Value", TypeName = "decimal(18, 2)")]
        public decimal? InstallmentValue { get; set; }
        [Column("Start_Date")]
        [StringLength(10)]
        public string? StartDate { get; set; }
        [Column("End_Date")]
        [StringLength(10)]
        public string? EndDate { get; set; }
        [Column("Status_ID")]
        public int? StatusId { get; set; }
        [Column("Fin_year")]
        public long? FinYear { get; set; }
        [Column("Doc_Type_ID")]
        public int? DocTypeId { get; set; }
        [Column("Facility_ID")]
        public long? FacilityId { get; set; }
        [Column("Branch_ID")]
        public long? BranchId { get; set; }
        [Column("Currency_ID")]
        public int? CurrencyId { get; set; }
        [Column("Exchange_Rate", TypeName = "decimal(18, 10)")]
        public decimal? ExchangeRate { get; set; }
        public long? Code { get; set; }
    }
}
