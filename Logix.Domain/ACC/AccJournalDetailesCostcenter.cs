using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Logix.Domain.ACC
{
    [Table("ACC_Journal_Detailes_Costcenter")]
    [Index("AccAccountId", Name = "Index_Account")]
    [Index("CcId", Name = "Index_Costcenter")]
    public partial class AccJournalDetailesCostcenter
    {
        [Key]
        [Column("ID")]
        public long Id { get; set; }
        [Column("j_Detailes_ID")]
        public long? JDetailesId { get; set; }
        [Column("J_ID")]
        public long? JId { get; set; }
        [Column("Acc_Account_ID")]
        public long? AccAccountId { get; set; }
        [Column("CC_ID")]
        public long? CcId { get; set; }
        [StringLength(2000)]
        public string? Description { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Debit { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Credit { get; set; }
        [Column("Insert_User_ID")]
        public int? InsertUserId { get; set; }
        [Column("Update_User_ID")]
        public int? UpdateUserId { get; set; }
        [Column("Delete_User_ID")]
        public int? DeleteUserId { get; set; }
        [Column("Insert_Date", TypeName = "datetime")]
        public DateTime? InsertDate { get; set; }
        [Column("Update_Date", TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        [Column("Delete_Date", TypeName = "datetime")]
        public DateTime? DeleteDate { get; set; }
        public bool? FlagDelete { get; set; }
    }
}
