
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Logix.MVC.LogixAPIs.FXA.ViewModels.ReportVm
{
    public class PurchaseAssetReportFilter
    {
        public int? BranchId { get; set; }
        public long? FxNo { get; set; }
        public string? FxName { get; set; }
        public long? TypeId { get; set; } //asset type
        public long? StatusId { get; set; }
        public long? LocationId { get; set; }
        [StringLength(10)]
        public string? StartDate { get; set; }
        [StringLength(10)]
        public string? EndDate { get; set; }
        public string? Description { get; set; }
    }
}