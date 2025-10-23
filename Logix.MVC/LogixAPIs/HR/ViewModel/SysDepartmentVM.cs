using Logix.Application.DTOs.Main;

namespace Logix.MVC.LogixAPIs.HR.ViewModel
{
    public class SysDepartmentVM
    {
        public long DeptId { get; set; }
        public string? DeptName { get; set; }
        public string? DeptName2 { get; set; }
        public string? EmpName1 { get; set; }
        public string? EmpName2 { get; set; }
        public long ParentId { get; set; }
    }

}
