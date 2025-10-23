namespace Logix.MVC.LogixAPIs.WF.ViewModels
{
    public class WfDashboardVM
    {
        public MyDataVM? MyData { get; set; }
        public List<BalanceVM>? Balances { get; set; }
        public List<TrialNotifiVM>? TrialNotifications { get; set; }
        public List<NotificationVM>? Notifications { get; set; }
        public SalaryDataVM? SalaryData { get; set; }
        public List<PayrollVM>? Payrolls { get; set; }
        public List<TaskVM>? Tasks { get; set; }
        public List<DuesVM>? Dues { get; set; }
        public decimal DegreeTotal { get; set; }
        public List<KpiReportVM>? KpiReport { get; set; }
        public List<GoalsVM>? Goals { get; set; }
        public List<LoanVM>? Loans { get; set; }
        public List<TeamMemberVM>? TeamMembers { get; set; }
        public List<ArchiveFileVM>? ArchiveFiles { get; set; }
    }
}
