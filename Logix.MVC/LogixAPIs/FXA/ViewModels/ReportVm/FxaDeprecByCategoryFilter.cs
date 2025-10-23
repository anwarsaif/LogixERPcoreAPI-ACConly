
using System.ComponentModel.DataAnnotations;

namespace Logix.MVC.LogixAPIs.FXA.ViewModels.ReportVm
{
    public class FxaDeprecByCategoryFilter
    {
        [StringLength(50)]
        public string? Code { get; set; }
        [StringLength(4000)]
        public string? Name { get; set; }
        public long? LocationId { get; set; }
        public int? BranchId { get; set; }
        public int? TypeId { get; set; }
        public int? StatusId { get; set; }
        [Required]
        [StringLength(10)]
        public string? StartDate { get; set; }
        [Required]
        [StringLength(10)]
        public string? EndDate { get; set; }
        public string? Description { get; set; }
    }
}   