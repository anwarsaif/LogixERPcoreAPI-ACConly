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
    [Table("Acc_Settlement_Schedule_D")]
    public partial class AccSettlementScheduleD:TraceEntity
    {
        [Key]
        [Column("ID")]
        public long Id { get; set; }
        [Column("SS_ID")]
        public long? SsId { get; set; }
        [Column("Acc_Account_ID")]
        public long? AccAccountId { get; set; }
        [Column("CC_ID")]
        public long? CcId { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Debit { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Credit { get; set; }
        [StringLength(2000)]
        public string? Description { get; set; }
        [Column("Currency_ID")]
        public int? CurrencyId { get; set; }
        [Column("Exchange_Rate", TypeName = "decimal(18, 10)")]
        public decimal? ExchangeRate { get; set; }

        [Column("Is_Main")]
        public bool? IsMain { get; set; }
        [Column("Reference_Type_ID")]
        public int? ReferenceTypeId { get; set; }
        [Column("Reference_No")]
        public long? ReferenceNo { get; set; }
        [Column("CC2_ID")]
        public long? Cc2Id { get; set; }
        [Column("CC3_ID")]
        public long? Cc3Id { get; set; }
        [Column("Reference_Code")]
        [StringLength(50)]
        public string? ReferenceCode { get; set; }
        [Column("CC4_ID")]
        public long? Cc4Id { get; set; }
        [Column("CC5_ID")]
        public long? Cc5Id { get; set; }
        [Column("Branch_ID")]
        public long? BranchId { get; set; }
        [Column("Activity_ID")]
        public long? ActivityId { get; set; }
        [Column("Assest_ID")]
        public long? AssestId { get; set; }
        [Column("Emp_ID")]
        public long? EmpId { get; set; }
    }
}
