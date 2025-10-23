namespace Logix.MVC.LogixAPIs.Acc.ViewModels
{
    public class ACCCostCenterNode
    {
        public long CcId { get; set; }
        public string? CostCenterName { get; set; }
        public string? CostCenterName2 { get; set; }
        public string Icon { get; set; }
        public List<ACCCostCenterNode> Children { get; set; }
    }
}
