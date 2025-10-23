namespace Logix.MVC.LogixAPIs.Main.ViewModels
{
    public class UserMailBoxVM
    {
        public long Id { get; set; }
        public long? ReferralId { get; set; }
        public bool IsRead { get; set; }
        public string? UserFullName { get; set; }
        public string? ModifiedOn { get; set; }
    }
}