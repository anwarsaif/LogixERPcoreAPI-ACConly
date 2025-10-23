namespace Logix.MVC.LogixAPIs.FXA.ViewModels.ReportVm
{
    public class FxaDeprecByCategoryReportVm
    {
        public string? TypeCode { get; set; }
        public string? TypeName { get; set; }
        public decimal? Amount { get; set; }
        public decimal? Additions { get; set; }
        public decimal? Discards { get; set; }
        public decimal? DepreciationOld { get; set; }
        public decimal? DepreciationNow { get; set; }
        public decimal? DepreciateDiscardation { get; set; } = 0;
        public decimal? ProfitAndLoss { get; set; }

        public string? AccountCode { get; set; }
        public string? AccountName { get; set; }
        public string? AccountCode2 { get; set; }
        public string? AccountName2 { get; set; }
        public string? AccountCode3 { get; set; }
        public string? AccountName3 { get; set; }
    }
}