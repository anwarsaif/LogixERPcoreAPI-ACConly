using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Logix.Domain.ACC
{
    [Keyless]
    public partial class AccBalanceSheet
    {
        [Column("j_Detailes_ID")]
        public long JDetailesId { get; set; }
        [Column("J_ID")]
        public long? JId { get; set; }
        [Column("Acc_Account_ID")]
        public long? AccAccountId { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Debit { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Credit { get; set; }
        [Column("Insert_User_ID")]
        public int? InsertUserId { get; set; }
        [Column("Update_User_ID")]
        public int? UpdateUserId { get; set; }
        [Column("Delete_User_ID")]
        public int? DeleteUserId { get; set; }
        [Column("Insert_Date", TypeName = "datetime")]
        public DateTime? InsertDate { get; set; }
        [Column("Update_Date", TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        [Column("Delete_Date", TypeName = "datetime")]
        public DateTime? DeleteDate { get; set; }
        public bool? FlagDelete { get; set; }
        [Column("Acc_Account_Name")]
        [StringLength(255)]
        public string? AccAccountName { get; set; }
        [Column("Acc_Account_Code")]
        [StringLength(50)]
        public string? AccAccountCode { get; set; }
        [Column("Acc_group_ID")]
        public long? AccGroupId { get; set; }
        [Column("Period_ID")]
        public long? PeriodId { get; set; }
        [Column("Fin_year")]
        public long? FinYear { get; set; }
        [Column("Facility_ID")]
        public long? FacilityId { get; set; }
        [Column("Status_Id")]
        public int? StatusId { get; set; }
        [Column("Acc_group_Name")]
        [StringLength(50)]
        public string AccGroupName { get; set; } = null!;
        public bool? Expr1 { get; set; }
        public bool? Expr2 { get; set; }
        public bool? Expr3 { get; set; }
        [Column("J_Code")]
        [StringLength(255)]
        public string? JCode { get; set; }
        [Column("Nature_account")]
        public int? NatureAccount { get; set; }
        [Column("Sort_No")]
        public int? SortNo { get; set; }
        [Column("Acc_Account_Parent_ID")]
        public long? AccAccountParentId { get; set; }
        [Column("Doc_Type_ID")]
        public int? DocTypeId { get; set; }
        [Column("Payment_Type_ID")]
        public int? PaymentTypeId { get; set; }
        [Column("Reference_No")]
        public long? ReferenceNo { get; set; }
        [Column("Is_Sub")]
        public bool? IsSub { get; set; }
        [Column("Acc_group_Code")]
        [StringLength(50)]
        public string AccGroupCode { get; set; } = null!;
        [Column("Bank_ID")]
        public long? BankId { get; set; }
        [Column("aggregate")]
        public bool? Aggregate { get; set; }
        [Column("Doc_Type_Name")]
        [StringLength(50)]
        public string? DocTypeName { get; set; }
        [Column("CC_ID")]
        public long? CcId { get; set; }
        [Column("CostCenter_Name")]
        [StringLength(150)]
        public string? CostCenterName { get; set; }
        [Column("BALANCE")]
        public int Balance { get; set; }
        [Column("J_Date_Hijri")]
        [StringLength(10)]
        public string? JDateHijri { get; set; }
        [Column("J_Date_Gregorian")]
        [StringLength(10)]
        public string? JDateGregorian { get; set; }
        [Column("Reference_Type_ID")]
        public int? ReferenceTypeId { get; set; }
        [Column("Reference_D_No")]
        public long? ReferenceDNo { get; set; }
        [Column("Currency_ID")]
        public int? CurrencyId { get; set; }
        [Column("Exchange_Rate", TypeName = "decimal(18, 10)")]
        public decimal? ExchangeRate { get; set; }
        [Column("Acc_Curreny_ID")]
        public int? AccCurrenyId { get; set; }
        [Column("Parent_Reference_Type_ID")]
        public int? ParentReferenceTypeId { get; set; }
        [Column("J_Date_Gregorian2")]
        [StringLength(10)]
        public string? JDateGregorian2 { get; set; }
        [Column("Reference_Code")]
        [StringLength(50)]
        public string? ReferenceCode { get; set; }
        [Column("CostCenter_Code")]
        [StringLength(50)]
        public string? CostCenterCode { get; set; }
        [StringLength(2086)]
        public string? Description { get; set; }
        [Column("Debit_1", TypeName = "decimal(18, 2)")]
        public decimal? Debit1 { get; set; }
        [Column("Credit_1", TypeName = "decimal(18, 2)")]
        public decimal? Credit1 { get; set; }
        [Column("Exchange_Rate_M", TypeName = "decimal(18, 10)")]
        public decimal? ExchangeRateM { get; set; }
        [Column("Currency_ID_M")]
        public int? CurrencyIdM { get; set; }
        [Column("CC2_ID")]
        public long? Cc2Id { get; set; }
        [Column("CC3_ID")]
        public long? Cc3Id { get; set; }
        [StringLength(50)]
        public string? Expr4 { get; set; }
        [Column("CC4_ID")]
        public long? Cc4Id { get; set; }
        [Column("CC5_ID")]
        public long? Cc5Id { get; set; }
        [Column("Activity_ID")]
        public long? ActivityId { get; set; }
        [Column("Branch_ID")]
        public long? BranchId { get; set; }
        [Column("Assest_ID")]
        public long? AssestId { get; set; }
        [Column("Emp_ID")]
        public long? EmpId { get; set; }
        [Column("MBranch_ID")]
        public long? MbranchId { get; set; }
        [Column("Chequ_No")]
        [StringLength(50)]
        public string? ChequNo { get; set; }
        [Column("Reference_Type_Name")]
        [StringLength(50)]
        public string? ReferenceTypeName { get; set; }
        [Column(TypeName = "numeric(2, 2)")]
        public decimal AmountPrev { get; set; }
        [Column("Account_level")]
        public int? AccountLevel { get; set; }
        [Column("Fin_year_Gregorian")]
        public int FinYearGregorian { get; set; }
        [Column("BRA_NAME")]
        public string? BraName { get; set; }
        [Column("Doc_Type_Name2")]
        [StringLength(50)]
        public string? DocTypeName2 { get; set; }
        [Column("BRA_NAME2")]
        public string? BraName2 { get; set; }
        [Column("Acc_Account_Name2")]
        [StringLength(255)]
        public string? AccAccountName2 { get; set; }
        [Column("CostCenter_Name2")]
        [StringLength(150)]
        public string? CostCenterName2 { get; set; }
        [Column("Reference_Type_Name2")]
        [StringLength(50)]
        public string? ReferenceTypeName2 { get; set; }
        [Column("Acc_group_Name2")]
        [StringLength(50)]
        public string? AccGroupName2 { get; set; }
        [Column("CC_Parent_ID")]
        public long? CcParentId { get; set; }
    }
}
