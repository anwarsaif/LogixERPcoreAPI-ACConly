using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Logix.Domain.Base;
using Microsoft.EntityFrameworkCore;

namespace Logix.Domain.ACC
{
    [Table("Acc_Receivables_Payables_Transaction_D")]
    public partial class AccReceivablesPayablesTransactionD:TraceEntity
    {
        [Key]
        [Column("ID")]
        public long Id { get; set; }
        [Column("T_ID")]
        public long? TId { get; set; }
        [Column("RP_D_ID")]
        public long? RpDId { get; set; }
        [Column("Curr_Statue_ID")]
        public int? CurrStatueId { get; set; }
        [Column("New_Statue_ID")]
        public int? NewStatueId { get; set; }
        [Column("Crd_Account_ID")]
        public long? CrdAccountId { get; set; }
        [Column("Dbt_Account_ID")]
        public long? DbtAccountId { get; set; }
        [StringLength(2500)]
        public string? Note { get; set; }
        [Column("Due_Date_Old")]
        [StringLength(50)]
        public string? DueDateOld { get; set; }
        [Column("AccBankID")]
        public int? AccBankId { get; set; }
    }
}
