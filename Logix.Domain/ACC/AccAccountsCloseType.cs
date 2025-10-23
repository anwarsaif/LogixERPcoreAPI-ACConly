using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logix.Domain.ACC
{
    [Table("ACC_Accounts_Close_Type")]
    public partial class AccAccountsCloseType
    {
        [Key]
        [Column("Account_Close_type_ID")]
        public int AccountCloseTypeId { get; set; }
        [Column("Account_Close_type_name")]
        [StringLength(50)]
        public string? AccountCloseTypeName { get; set; }
    }
}
