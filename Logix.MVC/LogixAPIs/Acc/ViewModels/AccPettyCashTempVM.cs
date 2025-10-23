using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Logix.MVC.LogixAPIs.Acc.ViewModels
{
    public class AccPettyCashTempVM
    {
        [StringLength(2500)]
        public string? SupplierName { get; set; }
        public decimal? Total { get; set; }
        public string? ReferenceCode { get; set; }
        public string? ReferenceDate { get; set; }
    }
}
