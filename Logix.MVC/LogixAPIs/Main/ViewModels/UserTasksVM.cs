namespace Logix.MVC.LogixAPIs.Main.ViewModels
{
    public class UserTasksVM
    {
        public long Id { get; set; }
        public string? DueDate { get; set; }
        public string? Subject { get; set; }
        public string? UserFullname { get; set; }
        public double? Percent { get; set; }
    }
}