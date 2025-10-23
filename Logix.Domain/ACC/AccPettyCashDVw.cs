using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logix.Domain.ACC
{
    [Keyless]
    public partial class AccPettyCashDVw
    {
        [Column("ID")]
        public long Id { get; set; }
        [Column("Petty_Cash_ID")]
        public long? PettyCashId { get; set; }
        [Column("CC_ID")]
        public long? CcId { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Amount { get; set; }
        [Column("VAT_Amount", TypeName = "decimal(18, 2)")]
        public decimal? VatAmount { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Total { get; set; }
        [Column("Reference_Code")]
        [StringLength(50)]
        public string? ReferenceCode { get; set; }
        [Column("Reference_Date")]
        [StringLength(50)]
        public string? ReferenceDate { get; set; }
        [Column("Meter_Reading")]
        [StringLength(50)]
        public string? MeterReading { get; set; }
        public string? Description { get; set; }
        public bool? IsDeleted { get; set; }
        [Column("Expense_ID")]
        public int? ExpenseId { get; set; }
        [Column("CostCenter_Code")]
        [StringLength(50)]
        public string? CostCenterCode { get; set; }
        [Column("CostCenter_Name")]
        [StringLength(150)]
        public string? CostCenterName { get; set; }
        [Column("CC2_ID")]
        public long? Cc2Id { get; set; }
        [Column("CC3_ID")]
        public long? Cc3Id { get; set; }
        [Column("CC4_ID")]
        public long? Cc4Id { get; set; }
        [Column("CC5_ID")]
        public long? Cc5Id { get; set; }
        [StringLength(2500)]
        public string? SupplierName { get; set; }
        [Column("Emp_ID")]
        public long? EmpId { get; set; }
        [Column("Assest_ID")]
        public long? AssestId { get; set; }
        [Column("Activity_ID")]
        public long? ActivityId { get; set; }
        [Column("Branch_ID")]
        public long? BranchId { get; set; }
        [StringLength(250)]
        public string? SupplierVatNumber { get; set; }
        [Column("Expense_Name")]
        [StringLength(250)]
        public string? ExpenseName { get; set; }
        [Column("Acc_Account_ID")]
        public long? AccAccountId { get; set; }
        public long? Code { get; set; }
        [Column("T_Date")]
        [StringLength(10)]
        public string? TDate { get; set; }
        [Column("Emp_Code")]
        [StringLength(50)]
        public string EmpCode { get; set; } = null!;
        [Column("Emp_name")]
        [StringLength(250)]
        public string? EmpName { get; set; }
        [Column("Facility_ID")]
        public long? FacilityId { get; set; }
        [Column("Emp_name2")]
        [StringLength(250)]
        public string? EmpName2 { get; set; }
        [Column("Type_ID")]
        public int? TypeId { get; set; }
        [Column("Petty_Cash_Type")]
        public int? PettyCashType { get; set; }
        [Column("M_Branch_ID")]
        public long? MBranchId { get; set; }
        [Column("CostCenter_Name2")]
        [StringLength(150)]
        public string? CostCenterName2 { get; set; }
        [Column("Expense_Name2")]
        [StringLength(250)]
        public string? ExpenseName2 { get; set; }
        [Column("M_BRA_NAME")]
        public string? MBraName { get; set; }
        [Column("M_BRA_NAME2")]
        public string? MBraName2 { get; set; }
        [Column("Reference_Type_Name")]
        [StringLength(50)]
        public string? ReferenceTypeName { get; set; }
        [Column("Reference_Type_Name2")]
        [StringLength(50)]
        public string? ReferenceTypeName2 { get; set; }
        [Column("Petty_Cash_Type_Name")]
        [StringLength(250)]
        public string? PettyCashTypeName { get; set; }
        [Column("Type_Name")]
        [StringLength(250)]
        public string? TypeName { get; set; }
        [Column("Type_Name2")]
        [StringLength(250)]
        public string? TypeName2 { get; set; }
        [Column("Petty_Cash_Type_Name2")]
        [StringLength(250)]
        public string? PettyCashTypeName2 { get; set; }
        [Column("VAT", TypeName = "decimal(18, 2)")]
        public decimal? Vat { get; set; }
    }
}
