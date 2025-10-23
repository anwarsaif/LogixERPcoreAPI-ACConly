using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Logix.Domain.Base;
using Microsoft.EntityFrameworkCore;

namespace Logix.Domain.ACC
{
    [Table("ACC_Accounts")]
    [Index("AccAccountParentId", Name = "Index_Parent_ID")]
    public partial class AccAccount: TraceEntity
    {
        [Key]
        [Column("Acc_Account_ID")]
        public long AccAccountId { get; set; }
        [Column("Acc_Account_Name")]
        [StringLength(255)]
        public string? AccAccountName { get; set; }
        [Column("Acc_Account_Name2")]
        [StringLength(255)]
        public string? AccAccountName2 { get; set; }
        [Column("Acc_group_ID")]
        public long? AccGroupId { get; set; }
        [Column("Acc_Account_Code")]
        [StringLength(50)]
        public string? AccAccountCode { get; set; }
        [Column("Is_Sub")]
        public bool? IsSub { get; set; }
        [Column("Acc_Account_Parent_ID")]
        public long? AccAccountParentId { get; set; }
        /// <summary>
        /// مركز التكلفة الافتراضي
        /// </summary>
        [Column("CC_ID")]
        public long? CcId { get; set; }
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
        [Column("Dept_ID")]
        public int? DeptID { get; set; }
        [Column("Delete_Date", TypeName = "datetime")]
        public DateTime? DeleteDate { get; set; }
        //public bool? FlagDelete { get; set; }
        [Column("Account_Close_type_ID")]
        public int? AccountCloseTypeId { get; set; }
        [Column("Account_level")]
        public int? AccountLevel { get; set; }
        [Column("Is_Help_Account")]
        public bool? IsHelpAccount { get; set; }
        [Column("aggregate")]
        public bool? Aggregate { get; set; }
        public bool? IsActive { get; set; }
        [Column("Facility_ID")]
        public long? FacilityId { get; set; }
        [Column("Currency_ID")]
        public int? CurrencyId { get; set; }
        [Column("Branch_ID")]
        public int? BranchId { get; set; }
        [Column("Acc_Account_Parent_ID2")]
        public long? AccAccountParentId2 { get; set; }
        [Column("Acc_Account_Parent_ID3")]
        public long? AccAccountParentId3 { get; set; }
        [Column("Acc_Account_Parent_Code")]
        [StringLength(50)]
        public string? AccAccountParentCode { get; set; }

        [StringLength(500)]
        public string? Note { get; set; }
        [StringLength(500)]
        [Column("Refrance_No")]
        public string? RefranceNo { get; set; }

        [StringLength(500)]
        public string? Branches { get; set; }

        [StringLength(500)]
        public int? duration { get; set; }
        [Column("Fin_year")]
        public long? FinYear { get; set; }
        public int? itemType { get; set; }
        [Column("System_Id")]
        public int? SystemId { get; set; }

    }
}
