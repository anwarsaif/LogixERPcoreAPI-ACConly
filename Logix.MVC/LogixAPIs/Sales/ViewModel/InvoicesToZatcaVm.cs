namespace Logix.MVC.LogixAPIs.Sales.ViewModel
{
    public class InvoicesToZatcaVm
    {
        public bool SendAll { get; set; }
        public int InvoiceType { get; set; }
        public List<InvoicesToZatca> Invoices { get; set; }
    }

    public class InvoicesToZatca
    {
        public long InvoiceId { get; set; }
        public string Code { get; set; }
        public int BranchId { get; set; }
    }
}
