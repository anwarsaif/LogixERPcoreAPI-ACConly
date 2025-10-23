using System.ComponentModel.DataAnnotations;

namespace Logix.MVC.LogixAPIs.FXA.ViewModels.ReportVm
{
    public class SaleAssetReportFilter
    {
        public int? BranchId { get; set; }
        public string? FxCode { get; set; }
        public string? FxName { get; set; }
        public long? LocationId { get; set; }
        [StringLength(10)]
        public string? StartDate { get; set; }
        [StringLength(10)]
        public string? EndDate { get; set; }
        //public string? Description { get; set; }
    }
}