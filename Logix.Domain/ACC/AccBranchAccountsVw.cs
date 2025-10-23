using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Logix.Domain.ACC
{
    [Keyless]
    public partial class AccBranchAccountsVw
    {
        [Column("Acc_Account_Name")]
        [StringLength(255)]
        public string? AccAccountName { get; set; }
        [Column("Acc_Account_Code")]
        [StringLength(255)]
        public string? AccAccountCode { get; set; }
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
        [Column("Acc_Account_Name2")]
        [StringLength(255)]
        public string? AccAccountName2 { get; set; }
        [StringLength(250)]
        public string? Name { get; set; }
        [StringLength(250)]
        public string? Name2 { get; set; }
    }
}
