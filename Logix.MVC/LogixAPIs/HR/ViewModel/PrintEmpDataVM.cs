using Logix.Application.DTOs.HR;
using Logix.Domain.HR;

namespace Logix.MVC.LogixAPIs.HR.ViewModel
{
    public class PrintEmpDataVM : HrEmployeeVw
    {
        public decimal? TotalSalary { get; set; }
        public decimal? NetSalary { get; set; }
        public List<HrAllowanceDeductionVw>? SalryAllownce { get; set; }
        public List<HrAllowanceDeductionVw>? SalryDeduction { get; set; }
    }
}
