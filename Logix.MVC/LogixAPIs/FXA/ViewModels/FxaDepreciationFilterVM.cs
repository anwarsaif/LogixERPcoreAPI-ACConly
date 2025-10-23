using System.ComponentModel.DataAnnotations;

namespace Logix.MVC.LogixAPIs.FXA.ViewModels
{
    public class FxaDepreciationFilterVM
    {
        public long? Id { get; set; }
        public string? Code { get; set; }
        [StringLength(10)]
        public string? StartDate { get; set; }
        [StringLength(10)]
        public string? EndDate { get; set; }

        // return
        public string? TransDate { get; set; }
        public decimal? Total { get; set; }
    }
}