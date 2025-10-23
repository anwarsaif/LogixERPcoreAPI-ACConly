using System.ComponentModel.DataAnnotations;

namespace Logix.Application.DTOs.ACC
{
    public class AccGroupDto
    {

        public long AccGroupId { get; set; }
        [Required]
        public string AccGroupName { get; set; } = null!;
        [Required]
        public string? AccGroupName2 { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public bool? IsDeleted { get; set; }

        public long? FacilityId { get; set; }
        [Required]
        public string AccGroupCode { get; set; } = null!;

        public int? NatureAccount { get; set; }
    }
    public class AccGroupEditDto
    {
        public long AccGroupId { get; set; }
        [Required]
        public string AccGroupName { get; set; } = null!;
        [Required]
        public string? AccGroupName2 { get; set; }
        [Required]
        public string AccGroupCode { get; set; } = null!;
        public int? NatureAccount { get; set; }
    }
}
