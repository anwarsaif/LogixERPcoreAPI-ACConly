namespace Logix.MVC.LogixAPIs.FXA.ViewModels
{
    public class FxaFixedAssetVM
    {
        public long Id { get; set; }
        public long? No { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public decimal? Amount { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public string? DeprecMethodName { get; set; }
        public string? MainAssetName { get; set; }
        public decimal? DeprecMonthlyRate { get; set; }
        public decimal? InstallmentValue { get; set; }
    }
}