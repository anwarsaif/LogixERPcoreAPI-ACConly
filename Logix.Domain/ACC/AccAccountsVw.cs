using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Logix.Domain.ACC
{
    [Keyless]
    public class AccAccountsVw
    {
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
        [Column("Account_Close_type_ID")]
        public int? AccountCloseTypeId { get; set; }
        [Column("Account_level")]
        public int? AccountLevel { get; set; }
        [Column("Acc_group_Code")]
        [StringLength(50)]
        public string AccGroupCode { get; set; } = null!;
        [Column("Is_Help_Account")]
        public bool? IsHelpAccount { get; set; }
        [Column("Nature_account")]
        public int? NatureAccount { get; set; }
        [Column("aggregate")]
        public bool? Aggregate { get; set; }
        public bool? IsActive { get; set; }
        [Column("Facility_ID")]
        public long? FacilityId { get; set; }
        [Column("Acc_Account_name_Parent")]
        [StringLength(255)]
        public string? AccAccountNameParent { get; set; }
        [Column("Acc_Account_Code_Parent")]
        [StringLength(50)]
        public string? AccAccountCodeParent { get; set; }
        [Column("Currency_ID")]
        public int? CurrencyId { get; set; }
        [Column("CostCenter_Code")]
        [StringLength(50)]
        public string? CostCenterCode { get; set; }
        [Column("CostCenter_Name")]
        [StringLength(150)]
        public string? CostCenterName { get; set; }
        [Column("CostCenter_Name2")]
        [StringLength(150)]
        public string? CostCenterName2 { get; set; }
        [Column("Acc_group_Name2")]
        [StringLength(50)]
        public string? AccGroupName2 { get; set; }
        [Column("Acc_Account_name_Parent2")]
        [StringLength(255)]
        public string? AccAccountNameParent2 { get; set; }
        [Column("Facility_Name")]
        [StringLength(500)]
        public string? FacilityName { get; set; }
        [Column("Facility_Name2")]
        [StringLength(500)]
        public string? FacilityName2 { get; set; }
        [Column("Dept_ID")]
        public int DeptID { get; set; }


        [StringLength(500)]
        public string? Branches { get; set; }

        [StringLength(500)]
        public int? duration { get; set; }
        [Column("Fin_year")]
        public long? FinYear { get; set; }
        public int? itemType { get; set; }
        [Column("System_Id")]
        public int? SystemId { get; set; }
        [StringLength(500)]
        public string? Color { get; set; }
    }
}
