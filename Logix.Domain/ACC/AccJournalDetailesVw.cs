using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
    public partial class AccJournalDetailesVw
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
        [Column("Reference_Type_ID")]
        public int? ReferenceTypeId { get; set; }
        [Column("Reference_No")]
        public long? ReferenceNo { get; set; }
        [StringLength(2000)]
        public string? Description { get; set; }
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
        [StringLength(258)]
        public string? AccAccountName { get; set; }
        [Column("Acc_Account_Code")]
        [StringLength(50)]
        public string? AccAccountCode { get; set; }
        [Column("Acc_group_ID")]
        public long? AccGroupId { get; set; }
        [Column("Acc_Account_Name2")]
        [StringLength(255)]
        public string? AccAccountName2 { get; set; }
        [Column("CC_ID")]
        public long? CcId { get; set; }
        [Column("CostCenter_Code")]
        [StringLength(50)]
        public string? CostCenterCode { get; set; }
        [Column("CostCenter_Name")]
        [StringLength(150)]
        public string? CostCenterName { get; set; }
        [Column("J_Date_Gregorian")]
        [StringLength(10)]
        public string? JDateGregorian { get; set; }
        [StringLength(50)]
        public string? Code { get; set; }
        [StringLength(4000)]
        public string? Name { get; set; }
        [Column("Exchange_Rate", TypeName = "decimal(18, 10)")]
        public decimal? ExchangeRate { get; set; }
        [Column("Currency_ID")]
        public int? CurrencyId { get; set; }
        [Column("Reference_Type_Name")]
        [StringLength(50)]
        public string? ReferenceTypeName { get; set; }
        [Column("Parent_ID")]
        public int? ParentId { get; set; }
        [Column("CC2_ID")]
        public long? Cc2Id { get; set; }
        [Column("CC3_ID")]
        public long? Cc3Id { get; set; }
        [Column("Reference_Code")]
        [StringLength(50)]
        public string? ReferenceCode { get; set; }
        [Column("CC4_ID")]
        public long? Cc4Id { get; set; }
        [Column("CC5_ID")]
        public long? Cc5Id { get; set; }
        [Column("Branch_ID")]
        public long? BranchId { get; set; }
        [Column("Activity_ID")]
        public long? ActivityId { get; set; }
        [Column("Emp_ID")]
        public long? EmpId { get; set; }
        [Column("Assest_ID")]
        public long? AssestId { get; set; }
        [Column("CostCenter2_Code")]
        [StringLength(50)]
        public string? CostCenter2Code { get; set; }
        [Column("CostCenter2_Name")]
        [StringLength(150)]
        public string? CostCenter2Name { get; set; }
        [Column("CostCenter3_Code")]
        [StringLength(50)]
        public string? CostCenter3Code { get; set; }
        [Column("CostCenter3_Name")]
        [StringLength(150)]
        public string? CostCenter3Name { get; set; }
        [Column("CostCenter4_Code")]
        [StringLength(50)]
        public string? CostCenter4Code { get; set; }
        [Column("CostCenter4_Name")]
        [StringLength(150)]
        public string? CostCenter4Name { get; set; }
        [Column("CostCenter5_Code")]
        [StringLength(50)]
        public string? CostCenter5Code { get; set; }
        [Column("CostCenter5_Name")]
        [StringLength(150)]
        public string? CostCenter5Name { get; set; }
        [Column("Assest_Code")]
        [StringLength(50)]
        public string? AssestCode { get; set; }
        [Column("Assest_Name")]
        [StringLength(4000)]
        public string? AssestName { get; set; }
        [Column("Emp_name")]
        [StringLength(250)]
        public string? EmpName { get; set; }
        [Column("Emp_Code")]
        [StringLength(50)]
        public string? EmpCode { get; set; }
        [Column("Reference_Type_Name2")]
        [StringLength(50)]
        public string? ReferenceTypeName2 { get; set; }
        [Column("CostCenter_Name2")]
        [StringLength(150)]
        public string? CostCenterName2 { get; set; }
        [Column("Status_ID")]
        public int? StatusId { get; set; }
        [Column("J_Code")]
        [StringLength(255)]
        public string? JCode { get; set; }
        [Column("J_Date_Hijri")]
        [StringLength(10)]
        public string? JDateHijri { get; set; }
        [Column("M_J_Date_Gregorian")]
        [StringLength(10)]
        public string? MJDateGregorian { get; set; }
        [Column("J_time")]
        [StringLength(50)]
        public string? JTime { get; set; }
        [Column("J_Description")]
        [StringLength(2000)]
        public string? JDescription { get; set; }
        [Column("Period_ID")]
        public long? PeriodId { get; set; }
        [Column("Fin_year")]
        public long? FinYear { get; set; }
        [Column("Facility_ID")]
        public long? FacilityId { get; set; }
        [Column("M_CC_ID")]
        public long? MCcId { get; set; }
        [Column("Doc_Type_ID")]
        public int? DocTypeId { get; set; }
        [Column("M_Insert_User_ID")]
        public int? MInsertUserId { get; set; }
        [Column("M_Update_User_ID")]
        public int? MUpdateUserId { get; set; }
        [Column("M_Delete_User_ID")]
        public int? MDeleteUserId { get; set; }
        [Column("M_Insert_Date", TypeName = "datetime")]
        public DateTime? MInsertDate { get; set; }
        [Column("M_Update_Date", TypeName = "datetime")]
        public DateTime? MUpdateDate { get; set; }
        [Column("M_Delete_Date", TypeName = "datetime")]
        public DateTime? MDeleteDate { get; set; }
        [Column("M_FlagDelete")]
        public bool? MFlagDelete { get; set; }
        [Column("M_Status_Id")]
        public int? MStatusId { get; set; }
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
        [Column("J_Bian")]
        [StringLength(2500)]
        public string? JBian { get; set; }
        [Column("Doc_Type_Name")]
        [StringLength(50)]
        public string? DocTypeName { get; set; }
        [Column("M_USER_FULLNAME")]
        [StringLength(50)]
        public string? MUserFullname { get; set; }
        [Column("M_Reference_No")]
        public long? MReferenceNo { get; set; }
        [Column("M_Status_Name")]
        [StringLength(50)]
        public string? MStatusName { get; set; }
        [Column("Fin_year_Gregorian")]
        public int? FinYearGregorian { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Amount { get; set; }
        [StringLength(4000)]
        public string? AmountWrite { get; set; }
        [StringLength(250)]
        public string? BankName { get; set; }
        [Column("Payment_Type_Name")]
        [StringLength(255)]
        public string? PaymentTypeName { get; set; }
        [Column("M_Reference_Code")]
        [StringLength(50)]
        public string? MReferenceCode { get; set; }
        [Column("Collection_Emp_ID")]
        public long? CollectionEmpId { get; set; }
        [Column("Collection_Emp_Code")]
        [StringLength(50)]
        public string? CollectionEmpCode { get; set; }
        [Column("Collection_Emp_Name")]
        [StringLength(250)]
        public string? CollectionEmpName { get; set; }
        [Column("M_Branch_ID")]
        public long? MBranchId { get; set; }
        [Column("M_Exchange_Rate", TypeName = "decimal(18, 10)")]
        public decimal? MExchangeRate { get; set; }
        [Column("M_Currency_ID")]
        public int? MCurrencyId { get; set; }
        [Column("M_BRA_NAME")]
        public string? MBraName { get; set; }
        [Column("M_BRA_NAME2")]
        public string? MBraName2 { get; set; }
        [Column("Payment_Type_Name2")]
        [StringLength(255)]
        public string? PaymentTypeName2 { get; set; }
        [Column("M_Status_Name2")]
        [StringLength(50)]
        public string? MStatusName2 { get; set; }
        [Column("Currency_Name")]
        [StringLength(50)]
        public string? CurrencyName { get; set; }
        [Column("Currency_Name2")]
        [StringLength(50)]
        public string? CurrencyName2 { get; set; }
        [Column("Reference_Mapping_ID")]
        public long? ReferenceMappingId { get; set; }
    }
}
