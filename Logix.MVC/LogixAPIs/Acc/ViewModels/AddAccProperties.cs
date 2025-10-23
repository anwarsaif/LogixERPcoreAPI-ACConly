namespace Logix.MVC.LogixAPIs.Acc.ViewModels
{
    public class AddAccProperties
    {
        public bool CC2Visible { get; set; }
        public bool CC3Visible { get; set; }
        public bool CC4Visible { get; set; }
        public bool CC5Visible { get; set; }

        public string? CC1Title { get; set; }
        public string? CC2Title { get; set; }
        public string? CC3Title { get; set; }
        public string? CC4Title { get; set; }
        public string? CC5Title { get; set; }

        public bool BranchVisible { get; set; }
        public bool AssVisible { get; set; }
        public bool EMPVisible { get; set; }
    }
}