using System.ComponentModel.DataAnnotations;

namespace Logix.MVC.LogixAPIs.FXA.ViewModels
{
    public class FxaFixedAssetVm2
    {
        //this vm used in asset exclusion and Revaluation. when user write asset No we get this data to fill asset data
        public long Id { get; set; }
        public long? No { get; set; }
        public string? Name { get; set; }
        public decimal? DepreAmount { get; set; }
        public decimal? FxAmount { get; set; }
        public decimal? Balance { get; set; }
        public decimal? NewFxAmount { get; set; }
        public string? AccountCode { get; set; }
        public string? AccountCode2 { get; set; }
        public string? AccountCode3 { get; set; }
        public string? AccountName { get; set; }
        public string? AccountName2 { get; set; }
        public string? AccountName3 { get; set; }

        // use in frontend (asset additionand excludion)
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
    }
}
