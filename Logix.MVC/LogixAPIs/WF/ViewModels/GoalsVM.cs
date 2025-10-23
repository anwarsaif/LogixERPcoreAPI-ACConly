namespace Logix.MVC.LogixAPIs.WF.ViewModels
{
    public class GoalsVM
    {
        public long Id { get; set; }
        public string? Title { get; set; }
        public string? Target { get; set; }
        public long Weight { get; set; }
        public decimal TargetValue { get; set; }
    }
}
