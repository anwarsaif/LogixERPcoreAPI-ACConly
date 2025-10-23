using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Logix.Domain.Base;
using Microsoft.EntityFrameworkCore;

namespace Logix.Domain.ACC
{
    [Table("ACC_Journal_Master")]
    [Index("FinYear", "FacilityId", Name = "Fin_ind")]
    [Index("JDateHijri", "JDateGregorian", Name = "Index_Date")]
    [Index("DocTypeId", Name = "Indx_Doc_Type")]
    public partial class AccJournalMaster
    {
        [Key]
        [Column("J_ID")]
        public long JId { get; set; }
        [Column("J_Code")]
        [StringLength(255)]
        public string? JCode { get; set; }
        [Column("J_Date_Hijri")]
        [StringLength(10)]
        public string? JDateHijri { get; set; }
        [Column("J_Date_Gregorian")]
        [StringLength(10)]
        public string? JDateGregorian { get; set; }
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
        [Column("CC_ID")]
        public long? CcId { get; set; }
        [Column("Doc_Type_ID")]
        public int? DocTypeId { get; set; }
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
        [Column("Status_Id")]
        public int? StatusId { get; set; }
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
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Amount { get; set; }
        [StringLength(4000)]
        public string? AmountWrite { get; set; }
        [Column("Reference_No")]
        public long? ReferenceNo { get; set; }
        [Column("Collection_Emp_ID")]
        public long? CollectionEmpId { get; set; }
        [Column("Currency_ID")]
        public int? CurrencyId { get; set; }
        [Column("Exchange_Rate", TypeName = "decimal(18, 10)")]
        public decimal? ExchangeRate { get; set; }
        [Column("Reference_Code")]
        [StringLength(50)]
        public string? ReferenceCode { get; set; }
        [Column("J_Due_Date")]
        [StringLength(10)]
        public string? JDueDate { get; set; }
        [Column("Reference_Mapping_ID")]
        public long? ReferenceMappingId { get; set; }

        //public bool? IsDeleted { get; set; }
    }
}
