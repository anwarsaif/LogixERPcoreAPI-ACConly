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
    public partial class AccPettyCashVw
    {
        [Column("ID")]
        public long Id { get; set; }
        public long? Code { get; set; }
        [Column("T_Date")]
        [StringLength(10)]
        public string? TDate { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Amount { get; set; }
        public string? Description { get; set; }
        [Column("Payment_Type_ID")]
        public int? PaymentTypeId { get; set; }
        [Column("Chequ_No")]
        [StringLength(50)]
        public string? ChequNo { get; set; }
        [Column("Chequ_Date_Hijri")]
        [StringLength(10)]
        public string? ChequDateHijri { get; set; }
        [Column("Bank_ID")]
        public long? BankId { get; set; }
        [Column("CashOrBank_Account_ID")]
        public long? CashOrBankAccountId { get; set; }
        [Column("Status_ID")]
        public int? StatusId { get; set; }
        [Column("Facility_ID")]
        public long? FacilityId { get; set; }
        [Column("Branch_ID")]
        public long? BranchId { get; set; }
        [Column("Period_ID")]
        public long? PeriodId { get; set; }
        public bool IsDeleted { get; set; }
        [Column("Emp_ID")]
        public long? EmpId { get; set; }
        [Column("Emp_Code")]
        [StringLength(50)]
        public string EmpCode { get; set; } = null!;
        [Column("Emp_name")]
        [StringLength(250)]
        public string? EmpName { get; set; }
        [Column("Petty_Cash_Type")]
        public int? PettyCashType { get; set; }
        [Column("Petty_Cash_Type_Name")]
        [StringLength(250)]
        public string? PettyCashTypeName { get; set; }
        [Column("Type_ID")]
        public int? TypeId { get; set; }
        [Column("Type_Name")]
        [StringLength(250)]
        public string? TypeName { get; set; }
        [Column("App_ID")]
        public long? AppId { get; set; }
        [Column("Status_Name")]
        [StringLength(50)]
        public string? StatusName { get; set; }
        [Column("Application_Code")]
        public long? ApplicationCode { get; set; }
        [Column("J_ID")]
        public long? JId { get; set; }
        [Column("J_Code")]
        [StringLength(255)]
        public string? JCode { get; set; }
        [Column("Step_Name")]
        [StringLength(250)]
        public string? StepName { get; set; }
        public string? Note { get; set; }
        [Column("Acc_Account_ID")]
        public long? AccAccountId { get; set; }
        [Column("Reference_Type_ID")]
        public int? ReferenceTypeId { get; set; }
        [Column("Reference_No")]
        public long? ReferenceNo { get; set; }
        [Column("Reference_Code")]
        [StringLength(50)]
        public string? ReferenceCode { get; set; }
        [Column("Reference_Name")]
        [StringLength(4000)]
        public string? ReferenceName { get; set; }
        [Column("Acc_Account_Name")]
        [StringLength(255)]
        public string? AccAccountName { get; set; }
        [Column("Acc_Account_Code")]
        [StringLength(50)]
        public string? AccAccountCode { get; set; }
        [Column("Emp_name2")]
        [StringLength(250)]
        public string? EmpName2 { get; set; }
        [Column("BRA_NAME")]
        public string? BraName { get; set; }
        [Column("BRA_NAME2")]
        public string? BraName2 { get; set; }
        [Column("Reference_Type_Name")]
        [StringLength(50)]
        public string? ReferenceTypeName { get; set; }
        [Column("Reference_Type_Name2")]
        [StringLength(50)]
        public string? ReferenceTypeName2 { get; set; }
        [Column("Type_Name2")]
        [StringLength(250)]
        public string? TypeName2 { get; set; }
        [Column("Petty_Cash_Type_Name2")]
        [StringLength(250)]
        public string? PettyCashTypeName2 { get; set; }
    }
}
