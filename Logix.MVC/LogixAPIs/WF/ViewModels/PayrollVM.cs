namespace Logix.MVC.LogixAPIs.WF.ViewModels
{
    public class PayrollVM
    {
        public long MsId { get; set; }
        public long MsdId { get; set; }
        public int FinancialYear { get; set; }
        public string? MsMonth { get; set; }
        public string? MsMothTxt { get; set; }
        public decimal Salary { get; set; }
        //public decimal Total { get; set; }

        // details
        public decimal Allowance { get; set; }
        public decimal Absence { get; set; }
        public decimal Delay { get; set; }
        public decimal Loan { get; set; }
        public decimal Deduction { get; set; }
        public decimal Penalties { get; set; }
    }
}
