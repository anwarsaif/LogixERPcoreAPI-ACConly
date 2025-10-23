using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Logix.Domain.ACC
{
  

    [Keyless]
    public partial class AccAccountsReportsVw
    {
        [Column("Acc_Account_Code_Parent")]
        [StringLength(50)]
        public string? AccAccountCodeParent { get; set; }
        [Column("Acc_Account_ID")]
        public long AccAccountId { get; set; }
        [Column("Acc_Account_Name")]
        [StringLength(258)]
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
        [Column("CC_ID")]
        public long? CcId { get; set; }
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
        [Column("Acc_group_Name")]
        [StringLength(50)]
        public string AccGroupName { get; set; } = null!;
        [Column("Facility_ID")]
        public long? FacilityId { get; set; }
        [Column("Account_Close_type_ID")]
        public int? AccountCloseTypeId { get; set; }
        [Column("Account_level")]
        public int? AccountLevel { get; set; }
        [Column("Account_Close_type_name")]
        [StringLength(50)]
        public string? AccountCloseTypeName { get; set; }
        [Column("Acc_group_Code")]
        [StringLength(50)]
        public string AccGroupCode { get; set; } = null!;
        [Column("Is_Help_Account")]
        public bool? IsHelpAccount { get; set; }
        [Column("Acc_group_Name2")]
        [StringLength(50)]
        public string? AccGroupName2 { get; set; }
    }
}
