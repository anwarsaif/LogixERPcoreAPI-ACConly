using System.ComponentModel.DataAnnotations;

namespace Logix.MVC.LogixAPIs.FXA.ViewModels.ReportVm
{
    public class FxaDeprec2ReportVm
    {
        public long? No { get; set; }
        [StringLength(50)]
        public string? Code { get; set; }
        [StringLength(4000)]
        public string? Name { get; set; }
        public string? TypeName3 { get; set; }
        public string? TypeName2 { get; set; }
        public string? TypeName { get; set; }
        public string? ClassificationName { get; set; }
        public string? Description { get; set; }
        public string? BraName { get; set; }
        public string? LocationName { get; set; }
        public decimal? Amount { get; set; }
        [StringLength(10)]
        public string? PurchaseDate { get; set; }
    }
}