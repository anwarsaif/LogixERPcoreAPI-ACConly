using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Logix.Domain.ACC
{
    [Table("Acc_CostCenters_Level")]
    public partial class AccCostCentersLevel
    {
        [Key]
        [Column("Level_ID")]
        public long LevelId { get; set; }
        public int? NoOfDigit { get; set; }
    }
}
