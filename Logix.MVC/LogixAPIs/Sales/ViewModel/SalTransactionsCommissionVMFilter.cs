using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Logix.MVC.LogixAPIs.Sales.ViewModel
{
    public class SalTransactionsCommissionVMFilter
    {

        public long? TransactionId { get; set; }
        public int? TypeId { get; set; }

        public decimal? Rate { get; set; }
       
        public decimal? Amount { get; set; }
        public string? EmpName { get; set; }
        public string? EmpCode { get; set; }

        public long? ProjectCode { get; set; }
        public string? ProjectName { get; set; }


    }
}
