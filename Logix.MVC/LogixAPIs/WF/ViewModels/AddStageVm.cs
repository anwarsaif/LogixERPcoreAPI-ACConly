namespace Logix.MVC.LogixAPIs.WF.ViewModels
{
    public class AddStageVm
    {
        public List<WfStatusChkBoxVm> StatusChkBoxVms { get; set; }
        public List<SysGroupChkBoxVm> SysGroupChkBoxVms { get; set; }
        public AddStageVm()
        {
            StatusChkBoxVms=new List<WfStatusChkBoxVm>();
            SysGroupChkBoxVms = new List<SysGroupChkBoxVm>();
        }
    }

    public class WfStatusChkBoxVm
    {
        public int Id { get; set; }
        public string? StatusName { get; set; }
        public bool IsSelected { get; set; }
    }

    public class SysGroupChkBoxVm
    {
        public int GroupId { get; set; }
        public string? GroupName { get; set; }
        public bool IsSelected { get; set; }
    }
}
