using System.ComponentModel.DataAnnotations;

namespace Logix.MVC.LogixAPIs.WF.ViewModels
{
    public class MyDataVM
    {
        public long Id { get; set; }
        public string EmpId { get; set; } = null!;
        public string? EmpName { get; set; }
        //public string? EmpName2 { get; set; }
        public string? Mobile { get; set; }
        public string? Email { get; set; }
        public string? EmpPhoto { get; set; }
        public string? DepName { get; set; }
        //public string? DepName2 { get; set; }
        public string? LocationName { get; set; }
        public int? AttendanceType { get; set; }
        public string? CatName { get; set; }

        public string? ManagerName { get; set; }
        //public string? ManagerName2 { get; set; }

        public bool LinkJobCatVisible { get; set;}
        public string? JobCatUrl { get; set;}
    }
}
