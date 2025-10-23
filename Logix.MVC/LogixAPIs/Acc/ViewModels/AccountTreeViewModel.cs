namespace Logix.MVC.LogixAPIs.Acc.ViewModels;

public class AccountTreeViewModel
{
    public long AccountId { get; set; }
    public string AccountName { get; set; }
    public IEnumerable<AccountTreeViewModel> ChildAccounts { get; set; }
}
