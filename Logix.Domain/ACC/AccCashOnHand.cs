using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Logix.Domain.ACC
{
    [Table("Acc_Cash_on_hand")]
    
    public partial class AccCashOnHand
    {
        [Key]
        [Column("ID")]
        public long Id { get; set; }
        public long? Code { get; set; }
        [StringLength(250)]
        public string? Name { get; set; }
        [StringLength(250)]
        public string? Name2 { get; set; }
        [StringLength(2500)]
        public string? Description { get; set; }
        [Column("Branch_ID")]
        public long? BranchId { get; set; }
        [Column("Facility_ID")]
        public long? FacilityId { get; set; }
        [Column("Acc_Account_ID")]
        public long? AccAccountId { get; set; }
        public long? CreatedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedOn { get; set; }
        public long? ModifiedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ModifiedOn { get; set; }
        public bool? IsDeleted { get; set; }
        [Column("Users_Permission")]
        public string? UsersPermission { get; set; }
        [Column("Acc_Account_Parent_ID")]
        public long? AccAccountParentID { get; set; }
    }
}
