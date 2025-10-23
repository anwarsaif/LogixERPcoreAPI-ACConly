using Logix.Domain.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
namespace Logix.Domain.ACC
{
    [Table("Acc_Accounts_Costcenter")]
    public partial class AccAccountsCostcenter:TraceEntity
    {
        [Key]
        [Column("ID")]
        public long Id { get; set; }
        [Column("Acc_Account_ID")]
        public long? AccAccountId { get; set; }
        [Column("CC_NO")]
        public long? CcNo { get; set; }
        [Column("CC_ID_From")]
        [StringLength(50)]
        public string? CcIdFrom { get; set; }
        [Column("CC_ID_To")]
        [StringLength(50)]
        public string? CcIdTo { get; set; }
        [Column("Is_Required")]
        public bool? IsRequired { get; set; }
        [Column("Is_Editable")]
        public bool? IsEditable { get; set; }
  
        [Column("CC_ID_Default")]
        public long? CcIdDefault { get; set; }
     
    }
}
