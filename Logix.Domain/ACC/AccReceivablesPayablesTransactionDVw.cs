using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Logix.Domain.ACC
{
    [Keyless]
    public partial class AccReceivablesPayablesTransactionDVw
    {
        [Column("ID")]
        public long Id { get; set; }
        [Column("T_ID")]
        public long? TId { get; set; }
        [Column("RP_D_ID")]
        public long? RpDId { get; set; }
        [Column("Curr_Statue_ID")]
        public int? CurrStatueId { get; set; }
        [Column("New_Statue_ID")]
        public int? NewStatueId { get; set; }
        [Column("Crd_Account_ID")]
        public long? CrdAccountId { get; set; }
        [Column("Dbt_Account_ID")]
        public long? DbtAccountId { get; set; }
        [StringLength(2500)]
        public string? Note { get; set; }
        public long? No { get; set; }
        [Column("RP_ID")]
        public long? RpId { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Amount { get; set; }
        [Column("T_Date")]
        [StringLength(10)]
        public string? TDate { get; set; }
        [Column("Due_Date")]
        [StringLength(10)]
        public string? DueDate { get; set; }
        [Column("Currency_ID")]
        public int? CurrencyId { get; set; }
        [Column("Exchange_Rate", TypeName = "decimal(18, 10)")]
        public decimal? ExchangeRate { get; set; }
        [Column("Bank_Account")]
        [StringLength(500)]
        public string? BankAccount { get; set; }
        [Column("Bank_ID")]
        public long? BankId { get; set; }
        [Column("Cheque_NO")]
        [StringLength(500)]
        public string? ChequeNo { get; set; }
        [Column("Currency_Name")]
        [StringLength(50)]
        public string? CurrencyName { get; set; }
        [Column("Currency_Name2")]
        [StringLength(50)]
        public string? CurrencyName2 { get; set; }
        [StringLength(50)]
        public string? Code { get; set; }
        [Column("Customer_Code")]
        [StringLength(250)]
        public string? CustomerCode { get; set; }
        [Column("Customer_Name")]
        [StringLength(2500)]
        public string? CustomerName { get; set; }
        [Column("Customer_Name2")]
        [StringLength(2550)]
        public string? CustomerName2 { get; set; }
        [Column("Emp_name2")]
        [StringLength(250)]
        public string? EmpName2 { get; set; }
        [Column("Emp_Code")]
        [StringLength(50)]
        public string? EmpCode { get; set; }
        [Column("Acc_Account_Name")]
        [StringLength(255)]
        public string? AccAccountName { get; set; }
        [Column("Acc_Account_Name2")]
        [StringLength(255)]
        public string? AccAccountName2 { get; set; }
        [Column("Account_ID")]
        public long? AccountId { get; set; }
        [Column("Acc_Account_Code")]
        [StringLength(50)]
        public string? AccAccountCode { get; set; }
        [Column("Bank_Name")]
        [StringLength(250)]
        public string? BankName { get; set; }
        [Column("Bank_Name2")]
        [StringLength(250)]
        public string? BankName2 { get; set; }
        [Column("Type_Name")]
        [StringLength(500)]
        public string? TypeName { get; set; }
        [Column("TYpe_Name2")]
        [StringLength(500)]
        public string? TypeName2 { get; set; }
        [StringLength(50)]
        public string? Mobile { get; set; }
        [Column("VAT_Number")]
        [StringLength(250)]
        public string? VatNumber { get; set; }
        public bool IsDeleted { get; set; }
        [Column("Statue_ID")]
        public long? StatueId { get; set; }
        [Column("Due_Date_Old")]
        [StringLength(50)]
        public string? DueDateOld { get; set; }
        [Column("Type_ID")]
        public int? TypeId { get; set; }
        [Column("Facility_ID")]
        public long? FacilityId { get; set; }
        [Column("Cus_Type_Id")]
        public int? CusTypeId { get; set; }
        [Column("Branch_ID")]
        public long? BranchId { get; set; }
        [Column("Emp_name")]
        [StringLength(250)]
        public string? EmpName { get; set; }
        [Column("Status_Name")]
        [StringLength(500)]
        public string? StatusName { get; set; }
        [Column("Status_Name2")]
        [StringLength(500)]
        public string? StatusName2 { get; set; }
        [Column("Customer_ID")]
        public long? CustomerId { get; set; }
        [Column("T_T_Date")]
        [StringLength(10)]
        public string? TTDate { get; set; }
        [Column("AccBankID")]
        public int? AccBankId { get; set; }
        [Column("AccBank_Name")]
        [StringLength(255)]
        public string? AccBankName { get; set; }
        [Column("AccBank_Name2")]
        [StringLength(255)]
        public string? AccBankName2 { get; set; }
    }
}
