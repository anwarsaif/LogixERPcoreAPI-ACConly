namespace Logix.MVC.LogixAPIs.Acc.ViewModels;

public class ACCCostCenterTreeViewModel
{
    public long CcId { get; set; }

    public string CostCenterName { get; set; }
    public IEnumerable<ACCCostCenterTreeViewModel> ChildAccounts { get; set; }
}
