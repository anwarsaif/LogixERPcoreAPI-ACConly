using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Logix.Domain.Base;

namespace Logix.Domain.ACC
{
    [Table("ACC_Journal_Detailes")]
    [Index("Debit", "Credit", Name = "Crd_Deb")]
    [Index("JDateGregorian", Name = "Date_ind")]
    [Index("AccAccountId", Name = "Index_Account_ID")]
    [Index("JId", Name = "index_Master_ID")]
    public partial class AccJournalDetaile : TraceEntity
    {
        [Key]
        [Column("j_Detailes_ID")]
        public long JDetailesId { get; set; }
        [Column("J_ID")]
        public long? JId { get; set; }
        [Column("J_Date_Gregorian")]
        [StringLength(10)]
        public string? JDateGregorian { get; set; }
        [Column("Acc_Account_ID")]
        public long? AccAccountId { get; set; }
        [Column("CC_ID")]
        public long? CcId { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Debit { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Credit { get; set; }
        [Column("Reference_Type_ID")]
        public int? ReferenceTypeId { get; set; }
        /// <summary>
        /// رقم المرجع في نظام التقسيط
        /// </summary>
        [Column("Reference_No")]
        public long? ReferenceNo { get; set; }
        [StringLength(2000)]
        public string? Description { get; set; }
        //[Column("Insert_User_ID")]
        //public int? InsertUserId { get; set; }
        //[Column("Update_User_ID")]
        //public int? UpdateUserId { get; set; }
        [Column("Delete_User_ID")]
        public int? DeleteUserId { get; set; }
        //[Column("Insert_Date", TypeName = "datetime")]
        //public DateTime? InsertDate { get; set; }
        //[Column("Update_Date", TypeName = "datetime")]
        //public DateTime? UpdateDate { get; set; }
        [Column("Delete_Date", TypeName = "datetime")]
        public DateTime? DeleteDate { get; set; }
        //public bool? FlagDelete { get; set; }
        public bool? Auto { get; set; }
        [Column("Currency_ID")]
        public int? CurrencyId { get; set; }
        [Column("Exchange_Rate", TypeName = "decimal(18, 10)")]
        public decimal? ExchangeRate { get; set; }
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
