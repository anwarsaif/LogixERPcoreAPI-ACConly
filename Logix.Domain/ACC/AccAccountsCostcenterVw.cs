using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Logix.Domain.ACC
{
    [Keyless]
    public partial class AccAccountsCostcenterVw
    {
        [Column("ID")]
        public long Id { get; set; }
        [Column("Acc_Account_ID")]
        public long? AccAccountId { get; set; }
        [Column("CC_NO")]
        public long? CcNo { get; set; }
        [Column("Is_Required")]
        public bool? IsRequired { get; set; }
        [Column("Is_Editable")]
        public bool? IsEditable { get; set; }
        public bool? IsDeleted { get; set; }
        [Column("Acc_Account_Code")]
        [StringLength(50)]
        public string? AccAccountCode { get; set; }
        [Column("Acc_Account_Name")]
        [StringLength(255)]
        public string? AccAccountName { get; set; }
        [Column("CC_ID_From")]
        [StringLength(50)]
        public string? CcIdFrom { get; set; }
        [Column("CC_ID_To")]
        [StringLength(50)]
        public string? CcIdTo { get; set; }
        [Column("CC_ID_Default")]
        public long? CcIdDefault { get; set; }
        [Column("CC_Code_Default")]
        [StringLength(50)]
        public string? CcCodeDefault { get; set; }
        [Column("CC_Name_Default")]
        [StringLength(150)]
        public string? CcNameDefault { get; set; }
    }

}
