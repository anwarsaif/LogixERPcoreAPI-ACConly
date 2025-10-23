namespace Logix.MVC.LogixAPIs.FXA.ViewModels
{
    public class FixedAssetDataForTransferVm
    {
        public int? FromBranchId { get; set; } 
        public long? FromFacilityId { get; set; }
        public string? CcCode { get; set; }
        public string? CcName { get; set; }
        public string? FromEmpCode { get; set; }
        public string? FromEmpName { get; set; }
        public long? FromLocationId { get; set; }
    }
}
