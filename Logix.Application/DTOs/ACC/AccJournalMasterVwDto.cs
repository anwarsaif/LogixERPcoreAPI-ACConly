using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Logix.Application.DTOs.OPM;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Logix.Domain.ACC;

namespace Logix.Application.DTOs.ACC
{
    [Keyless]
    public class AccJournalMasterVwDto
    {

        public long JId { get; set; }

        [StringLength(255)]
        public string? JCode { get; set; }

        [StringLength(10)]
        public string? JDateHijri { get; set; }

        [StringLength(10)]
        public string? JDateGregorian { get; set; }

        [StringLength(50)]
        public string? JTime { get; set; }

        [StringLength(2000)]
        public string? JDescription { get; set; }

        public long? PeriodId { get; set; }

        public long? FinYear { get; set; }

        public long? FacilityId { get; set; }

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
        [Column("Doc_Type_Name")]
        [StringLength(50)]
        public string? DocTypeName { get; set; }
        [Column("USER_FULLNAME")]
        [StringLength(50)]
        public string? UserFullname { get; set; }
        [Column("Reference_No")]
        public long? ReferenceNo { get; set; }
        [Column("Status_Name")]
        [StringLength(50)]
        public string? StatusName { get; set; }
        [Column("Fin_year_Gregorian")]
        public int FinYearGregorian { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Amount { get; set; }
        [StringLength(4000)]
        public string? AmountWrite { get; set; }
        [StringLength(250)]
        public string? BankName { get; set; }
        [Column("Payment_Type_Name")]
        [StringLength(255)]
        public string? PaymentTypeName { get; set; }
        [Column("Reference_Code")]
        [StringLength(50)]
        public string? ReferenceCode { get; set; }
        [Column("Collection_Emp_ID")]
        public long? CollectionEmpId { get; set; }
        [Column("Collection_Emp_Code")]
        [StringLength(50)]
        public string? CollectionEmpCode { get; set; }
        [Column("Collection_Emp_Name")]
        [StringLength(250)]
        public string? CollectionEmpName { get; set; }
        [Column("Branch_ID")]
        public long? BranchId { get; set; }
        [Column("Exchange_Rate", TypeName = "decimal(18, 10)")]
        public decimal? ExchangeRate { get; set; }
        [Column("Currency_ID")]
        public int? CurrencyId { get; set; }
        [Column("BRA_NAME")]
        public string? BraName { get; set; }
        [Column("BRA_NAME2")]
        public string? BraName2 { get; set; }
        [Column("Payment_Type_Name2")]
        [StringLength(255)]
        public string? PaymentTypeName2 { get; set; }
        [Column("Status_Name2")]
        [StringLength(50)]
        public string? StatusName2 { get; set; }
        [Column("Currency_Name")]
        [StringLength(50)]
        public string? CurrencyName { get; set; }
        [Column("Currency_Name2")]
        [StringLength(50)]
        public string? CurrencyName2 { get; set; }
    }
    public class PrintAccJournalMasterVM
    {
        public string? FacilityLogo { get; set; }
        public string? FacilityName { get; set; }
        public string? FacilityName2 { get; set; }
        public string? PrintDate { get; set; } = DateTime.Now.ToString("HH:mm:fff yyyy-MM-dd", CultureInfo.InvariantCulture);


        public int? FinYearName { get; set; }
        public string? JCode { get; set; }
        public string? JDateGregorian { get; set; }
        public string? DocTypeName { get; set; }
        public string? BraName { get; set; }
        public string? DocTypeName2 { get; set; }
        public string? BraName2 { get; set; }
        public string? ReferenceCode { get; set; }
        public string? Description { get; set; }
        public string? UserCreator { get; set; }
        public string? SellerAddress { get; set; }
        public string? UserFullname { get; set; }

        public int?StatusId { get; set; }
        public int? DocTypeId { get; set; }
        public int? ChequNo { get; set; }
        public long? ReferenceNo { get; set; }
        public List<AccJournalMasterExportVw> Details { get; set; }
 

        public PrintAccJournalMasterVM()
        {
            Details = new List<AccJournalMasterExportVw>();
            AccJournalSignatureVw=new AccJournalSignatureVw();
        }
        public AccJournalSignatureVw AccJournalSignatureVw { get; set; }

    }
}
