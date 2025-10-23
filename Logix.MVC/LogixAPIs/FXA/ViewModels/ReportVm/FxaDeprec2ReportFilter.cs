using System.ComponentModel.DataAnnotations;

namespace Logix.MVC.LogixAPIs.FXA.ViewModels.ReportVm
{
    public class FxaDeprec2ReportFilter
    {
        public int? BranchId { get; set; }
        public long? No { get; set; }
        [StringLength(50)]
        public string? Code { get; set; }
        [StringLength(4000)]
        public string? Name { get; set; }
        public int? TypeId { get; set; }
        public int? TypeId2 { get; set; }
        public int? TypeId3 { get; set; }
        public int? ClassificationId { get; set; }
        public int? StatusId { get; set; }
        public long? LocationId { get; set; }
        [StringLength(10)]
        public string? StartDate { get; set; }
        [StringLength(10)]
        public string? EndDate { get; set; }
        public string? Description { get; set; }
    }
}