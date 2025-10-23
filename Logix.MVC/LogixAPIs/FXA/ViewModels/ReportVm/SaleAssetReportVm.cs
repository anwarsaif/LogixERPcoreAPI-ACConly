namespace Logix.MVC.LogixAPIs.FXA.ViewModels.ReportVm
{
    public class SaleAssetReportVm
    {
        public long? FxNo { get; set; }
        //public string? FxCode { get; set; }
        public string? FxName { get; set; }
        public string? AccAccountCode { get; set; }
        public string? AccAccountName { get; set; }
        public string? JCode { get; set; }
        public string? TransDate { get; set; }
        public decimal? Total { get; set; }
    }
}