namespace Logix.MVC.LogixAPIs.FXA.ViewModels
{
    public class FxaFixedAssetTypeVm2
    {
        //this vm used in add asset page. when user write asset type code we get this data to fill asset data
        //public long Id { get; set; }
        //public string? Code { get; set; }
        public string? TypeName { get; set; }
        public decimal? DeprecYearlyRate { get; set; }
        public int? Age { get; set; }
        public string? AccountCode { get; set; }
        //public string? AccountName { get; set; }

        public string? AccountCode2 { get; set; }
        //public string? AccountName2 { get; set; }

        public string? AccountCode3 { get; set; }
        //public string? AccountName3 { get; set; }
    }
}
