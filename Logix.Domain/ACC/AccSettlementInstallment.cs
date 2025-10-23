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
    [Table("Acc_Settlement_Installments")]
    public partial class AccSettlementInstallment:TraceEntity
    {
        [Key]
        [Column("ID")]
        public long Id { get; set; }
        [Column("Installment_No")]
        public long? InstallmentNo { get; set; }
        [Column("SS_ID")]
        public long? SsId { get; set; }
        [Column("Installment_Value", TypeName = "decimal(18, 2)")]
        public decimal? InstallmentValue { get; set; }
        [Column("Installment_Date")]
        [StringLength(10)]
        public string? InstallmentDate { get; set; }
        public string? Description { get; set; }

    }
}
