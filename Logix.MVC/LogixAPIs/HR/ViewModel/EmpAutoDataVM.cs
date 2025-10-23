using Logix.Application.DTOs.HR;
using Logix.Domain.HR;

namespace Logix.MVC.LogixAPIs.HR.ViewModel
{
    public class EmpAutoDataVM
    {
        public decimal? Salary { get; set; }
        public decimal? NetSalary { get; set; }
        public DateTime DueDate { get; set; } = DateTime.Now;
        public string? EmpName { get; set; }
        public decimal? SalryAllownce { get; set; }
        public decimal? SalryDeduction { get; set; }
        public decimal? DeductionRate { get; set; }
        public decimal? DeductionAmount { get; set; }
        public string? NumberofRepetitionDays { get; set; }
        public int ActionTypeId { get; set; }
        public int? DegreeId { get; set; }
        public int? LevelId { get; set; }
        public int? JobCatagoriesId { get; set; }
        public long? JobId { get; set; }
        public List<HrAllowanceDeductionVw>? allowances { get; set; }
        public List<HrAllowanceDeductionVw>? deduction { get; set; }


    }
}
