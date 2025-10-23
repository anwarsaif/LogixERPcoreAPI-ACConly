using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Logix.Domain.ACC
{
    [Table("Acc_Accounts_Level")]
    public partial class AccAccountsLevel
    {
        [Key]
        [Column("Level_ID")]
        public long LevelId { get; set; }
        public int? NoOfDigit { get; set; }
        [StringLength(50)]
        public string? Color { get; set; }
    }
}
