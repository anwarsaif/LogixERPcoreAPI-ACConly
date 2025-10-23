using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logix.Domain.ACC
{
    [Keyless]
    public partial class AccAccountsSubHelpeVw
    {
        [Column("Acc_Account_ID")]
        public long AccAccountId { get; set; }
        [Column("Acc_Account_Code")]
        [StringLength(50)]
        public string? AccAccountCode { get; set; }
        [Column("Acc_Account_Name")]
        [StringLength(255)]
        public string? AccAccountName { get; set; }
        [Column("Is_Help_Account")]
        public bool? IsHelpAccount { get; set; }
        public bool? FlagDelete { get; set; }
        [Column("Is_Sub")]
        public bool? IsSub { get; set; }
        [Column("isdel")]
        public bool? Isdel { get; set; }
        [Column("Acc_Account_Name2")]
        [StringLength(255)]
        public string? AccAccountName2 { get; set; }
        [Column("Facility_ID")]
        public long? FacilityId { get; set; }
        public bool? IsActive { get; set; }
        [Column("Acc_group_ID")]
        public long? AccGroupId { get; set; }
        [Column("Acc_Account_Parent_ID")]
        public long? AccAccountParentID { get; set; }
        [Column("Fin_year")]
        public long? FinYear { get; set; }
        public int? itemType { get; set; }
        [Column("System_Id")]
        public int? SystemId { get; set; }
    }
}
