namespace Logix.MVC.LogixAPIs.WF.ViewModels
{
    public class LoanVM
    {
        //public long Id { get; set; }
        public decimal? LoanValue { get; set; }
        public decimal? InstallmentValue { get; set; }
        public int? InstallmentCount { get; set; }
        public decimal? RemainingAmount { get; set; }
        public string? Note { get; set; }
        public List<LoanDetailVM>? Details { get; set; }
    }

    public class LoanDetailVM
    {
        public int? InstallmentNo { get; set; }
        public decimal? Amount { get; set; }
        public string? DueDate { get; set; }
        public bool IsPaid { get; set; }
    }
}
