namespace Logix.MVC.LogixAPIs.Main.ViewModels
{
    public class UserNotificationsVM
    {
        public long Id { get; set; }
        public string? MsgTxt { get; set; }
        public string? UserFullname { get; set; }
        public string? Url { get; set; }
        public string? CreatedOn { get; set; }
    }
}
