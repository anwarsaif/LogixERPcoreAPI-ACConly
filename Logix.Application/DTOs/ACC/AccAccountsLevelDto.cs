using System.ComponentModel.DataAnnotations;

namespace Logix.Application.DTOs.ACC
{
    public class AccAccountsLevelDto
    {

        public long LevelId { get; set; }
        public int? NoOfDigit { get; set; }
        [StringLength(50)]
        public string? Color { get; set; }
    }
    public class AccAccountsLevelEditDto
    {

        public long LevelId { get; set; }
        public int? NoOfDigit { get; set; }
        [StringLength(50)]
        public string? Color { get; set; }
    }

}
