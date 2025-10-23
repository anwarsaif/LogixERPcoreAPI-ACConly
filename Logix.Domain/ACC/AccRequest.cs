using Logix.Domain.Base;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Logix.Domain.ACC
{
    [Table("Acc_Request")]
    public partial class AccRequest : TraceEntity
    {
        [Key]
        [Column("ID")]
        public long Id { get; set; }
        [Column("App_Code")]
        public long? AppCode { get; set; }
        [Column("App_Date")]
        [StringLength(50)]
        public string? AppDate { get; set; }
        [Column("Type_ID")]
        public int? TypeId { get; set; }
        [Column("Dep_ID")]
        public long? DepId { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
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

        [Column("Status2_ID")]
        public int? Status2Id { get; set; }
        [Column("App_ID")]
        public long? AppId { get; set; }
        [Column("Reference_No")]
        public long? ReferenceNo { get; set; }
        [Column("Reference_Type_ID")]
        public long? ReferenceTypeId { get; set; }
        [Column("Branch_ID")]
        public long? BranchId { get; set; }
        [Column("Facility_ID")]
        public long? FacilityId { get; set; }
        [Column("J_ID")]
        public long? JId { get; set; }
        public string? Amountwrite { get; set; }
        [Column("CC_ID")]
        public long? CcId { get; set; }
        [Column("Trans_Type_ID")]
        public int? TransTypeId { get; set; }
        [Column("Refrance_ID")]
        public long? RefranceId { get; set; }
        [Column("Currency_ID")]
        public int? CurrencyId { get; set; }
        [Column("Exchange_Rate", TypeName = "decimal(18, 10)")]
        public decimal? ExchangeRate { get; set; }
        [Column("ID_NO")]
        [StringLength(50)]
        public string? IdNo { get; set; }
        [Column("Bank_ID")]
        public int? BankId { get; set; }
        [Column("IBAN")]
        [StringLength(50)]
        public string? Iban { get; set; }
        [Column("Customer_Name")]
        [StringLength(550)]
        public string? CustomerName { get; set; }
        [Column("Customer_Cont")]
        public int? CustomerCont { get; set; }
        [Column("Deductions_Total", TypeName = "decimal(18, 2)")]
        public decimal? DeductionsTotal { get; set; }
        [Column("Deductions_Note")]
        public string? DeductionsNote { get; set; }
        [Column("Badget_No")]
        public long? BadgetNo { get; set; }
        [Column("App_Date_H")]
        [StringLength(10)]
        public string? AppDateH { get; set; }

        [Column("ISMulti")]
        public bool? ISMulti { get; set; }
    }

}
