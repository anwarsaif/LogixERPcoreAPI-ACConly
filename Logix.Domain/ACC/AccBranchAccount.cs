using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Logix.Domain.ACC
{
    [Table("Acc_Branch_Accounts")]
    public partial class AccBranchAccount
    {
        [Key]
        [Column("ID")]
        public long Id { get; set; }
        [Column("Br_Acc_Type_ID")]
        public long? BrAccTypeId { get; set; }
        [Column("Branch_ID")]
        public long? BranchId { get; set; }
        [Column("Account_ID")]
        public long? AccountId { get; set; }
        public long CreatedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedOn { get; set; }
        public long? ModifiedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ModifiedOn { get; set; }
        public bool IsDeleted { get; set; }
    }
}
