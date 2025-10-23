namespace Logix.MVC.LogixAPIs.PM.ViewModel
{
    public class ProjectsPermissionsFilter
    {
        public long? ProjectCodeFrom { get; set; } 
        public long? ProjectCodeTo { get; set; }
        public string? ProjectName { get; set; }
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public string? ProjectManagerName { get; set; }
        public string? ProjectManagerCode { get; set; }// in view Emp_ID

    }

}
