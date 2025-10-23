namespace Logix.MVC.LogixAPIs.FXA.ViewModels
{
    public class FxaFixedAssetTransferSearchVM
    {
        public long Id { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? DateTransfer { get; set; }
        public string? FromEmpName { get; set; }
        public string? ToEmpName { get; set; }
        public string? FromLocationName { get; set; }
        public string? ToLocationName { get; set; }
    }
}