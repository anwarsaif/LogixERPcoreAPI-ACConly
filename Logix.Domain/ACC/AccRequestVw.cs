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
    public partial class AccRequestVw
    {
        [Column("ID")]
        public long Id { get; set; }
        [Column("App_Code")]
        public long? AppCode { get; set; }
        [Column("App_Date")]
        [StringLength(50)]
        public string? AppDate { get; set; }

        [Column("Dep_ID")]
        public long? DepId { get; set; }
        [Column("Type_ID")]
        public int? TypeId { get; set; }

        public decimal? Amount { get; set; }
        public string? Description { get; set; }
        public string? Note { get; set; }
        [Column("Refrance_No")]
        [StringLength(250)]
        public string? RefranceNo { get; set; }
        [Column("Account_ID")]
        public long? AccountId { get; set; }
        [Column("Status_ID")]
        public int? StatusId { get; set; }
        [Column("Fin_User_ID")]
        public long? FinUserId { get; set; }
        [Column("Fin_Date", TypeName = "datetime")]
        public DateTime? FinDate { get; set; }
        [Column("Has_Credit")]
        public int? HasCredit { get; set; }
        [Column("Balance_Status_ID")]
        public int? BalanceStatusId { get; set; }
        [Column("Fin_Note")]
        public string? FinNote { get; set; }
        [Column("GM_User_ID")]
        public long? GmUserId { get; set; }
        [Column("GM_Date", TypeName = "datetime")]
        public DateTime? GmDate { get; set; }
        [Column("Exchange_Status_ID")]
        public int? ExchangeStatusId { get; set; }
        [Column("GM_Note")]
        public string? GmNote { get; set; }
        public bool IsDeleted { get; set; }
        //[Column("Dep_Name")]
        //[StringLength(200)]
        //public string DepName { get; set; } = null!;
        //[Column("Dep_Name2")]
        //[StringLength(200)]
        //public string DepName2 { get; set; } = null!;
        [Column("Account_Code")]
        [StringLength(50)]
        public string? AccountCode { get; set; }
        [Column("Account_Name")]
        [StringLength(255)]
        public string? AccountName { get; set; }
        [Column("Has_Credit_Name")]
        [StringLength(250)]
        public string? HasCreditName { get; set; }
        [Column("Exchange_Status_Name")]
        [StringLength(250)]
        public string? ExchangeStatusName { get; set; }
        [Column("Balance_Status_Name")]
        [StringLength(250)]
        public string? BalanceStatusName { get; set; }
        [Column("Status2_ID")]
        public int? Status2Id { get; set; }
        //[Column("App_ID")]
        //public long? AppId { get; set; }
        [Column("Reference_No")]
        public long? ReferenceNo { get; set; }
        [Column("Reference_Type_ID")]
        public long? ReferenceTypeId { get; set; }
        [Column("Reference_Type_Name")]
        [StringLength(50)]
        public string? ReferenceTypeName { get; set; }
        [Column("Parent_ID")]
        public int? ParentId { get; set; }
        public string? Code { get; set; }
        [StringLength(4000)]
        public string? Name { get; set; }
        [Column("Branch_ID")]
        public long? BranchId { get; set; }
        [Column("Facility_ID")]
        public long? FacilityId { get; set; }
        [Column("BRA_NAME")]
        public string? BraName { get; set; }
        [Column("BRA_NAME2")]
        public string? BraName2 { get; set; }
        [Column("Application_Code")]
        public long? ApplicationCode { get; set; }
        [Column("J_ID")]
        public long? JId { get; set; }
        public string? Amountwrite { get; set; }
        [Column("CC_ID")]
        public long? CcId { get; set; }
        [Column("CostCenter_Code")]
        [StringLength(50)]
        public string? CostCenterCode { get; set; }
        [Column("CostCenter_Name")]
        [StringLength(150)]
        public string? CostCenterName { get; set; }
        [Column("CostCenter_Name2")]
        [StringLength(150)]
        public string? CostCenterName2 { get; set; }
        [Column("Trans_Type_ID")]
        public int? TransTypeId { get; set; }
        [Column("Refrance_ID")]
        public long? RefranceId { get; set; }
        [Column("Refrane_Code")]
        public long? RefraneCode { get; set; }
        [Column("Refrane_Date")]
        [StringLength(50)]
        public string? RefraneDate { get; set; }
        [Column("Currency_ID")]
        public int? CurrencyId { get; set; }
        [Column("Exchange_Rate", TypeName = "decimal(18, 10)")]
        public decimal? ExchangeRate { get; set; }
        [Column("ID_NO")]
        [StringLength(50)]
        public string? IdNo { get; set; }
        [Column("IBAN")]
        [StringLength(50)]
        public string? Iban { get; set; }
        [Column("Customer_Name")]
        [StringLength(550)]
        public string? CustomerName { get; set; }
        [Column("Bank_ID")]
        public int? BankId { get; set; }
        [Column("Customer_Cont")]
        public int? CustomerCont { get; set; }
        [Column("Bank_Name")]
        [StringLength(250)]
        public string? BankName { get; set; }
        [Column("Bank_Code")]
        [StringLength(50)]
        public string? BankCode { get; set; }
        [Column("Deductions_Total", TypeName = "decimal(18, 2)")]
        public decimal? DeductionsTotal { get; set; }
        [Column("Deductions_Note")]
        public string? DeductionsNote { get; set; }
        [Column("Badget_No")]
        public long? BadgetNo { get; set; }
        [Column("App_Date_H")]
        [StringLength(10)]
        public string? AppDateH { get; set; }
        public long CreatedBy { get; set; }
       
    }
}
