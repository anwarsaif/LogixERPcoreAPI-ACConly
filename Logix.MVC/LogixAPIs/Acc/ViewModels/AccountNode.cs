namespace Logix.MVC.LogixAPIs.Acc.ViewModels
{
    public class AccountNode
    {
        public long Id { get; set; }
        public string Text { get; set; }
        public string Icon { get; set; }
        public List<AccountNode> Children { get; set; }
    }

    public class AccountApiNode
    {
        public long AccountId { get; set; }
        public string AccountName { get; set; }
        public string AccountName2 { get; set; }
        public string Icon { get; set; }
        public List<AccountApiNode> Children { get; set; }
    }
}
