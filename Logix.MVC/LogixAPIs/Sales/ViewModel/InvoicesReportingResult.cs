namespace Logix.MVC.LogixAPIs.Sales.ViewModel
{
    public class InvoicesReportingResult
    {
        public long InvoicesCount { get; set; } = 0;
        public long Reported { get; set; } = 0;
        public long ReportedWithWorning { get; set; } = 0;
        public long ReportedNotIcepted { get; set; } = 0;
        public long NotReported { get; set; } = 0;
    }
}
