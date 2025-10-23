namespace Logix.MVC.LogixAPIs.WF.ViewModels
{
    public class AttendanceOnlineVM
    {
        public string? TimeIn { get; set; }
        public string? TimeOut { get; set; }
        public string? TimeTableName { get; set; }
        public string? BeginIn { get; set; }
        public string? EndIn { get; set; }
        public string? BeginOut { get; set; }
        public string? EndOut { get; set; }
        public string? OnDutyTime { get; set; }
        public string? OffDutyTime { get; set; }

        // allowed location data
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public string? LocationName { get; set; }
    }
}
