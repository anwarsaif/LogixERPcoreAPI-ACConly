
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Logix.Application.DTOs.HR;
using Microsoft.EntityFrameworkCore;

namespace Logix.MVC.LogixAPIs.HR.ViewModel
{
    public class EmpManagerIdChangedVM
    {
        public string? EmpId { get; set; }
        public string? EmpName { get; set; }

    }
    public class EmpIdChangedVM
    {
        public string? EmpId { get; set; }
        public string? EmpName { get; set; }
        public string? EmpName2 { get; set; }
        public string? IdNo { get; set; }
        public int? BankId { get; set; }
        public string? Iban { get; set; }
        public int? NationalityId { get; set; }
        public int? BranchId { get; set; }
        public int? Gender { get; set; }
        public string? InsuranceCardNo { get; set; }
        public decimal? Salary { get; set; }
        public long? SalaryGroupId { get; set; }



    }
}
