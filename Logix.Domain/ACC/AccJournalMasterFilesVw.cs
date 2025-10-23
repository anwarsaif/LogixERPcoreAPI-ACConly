using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Logix.Domain.ACC
{
    [Keyless]
    public partial class AccJournalMasterFilesVw
    {
        [Column("ID")]
        public long Id { get; set; }
        [Column("J_ID")]
        public long? JId { get; set; }
        [Column("File_Name")]
        [StringLength(50)]
        public string? FileName { get; set; }
        [Column("File_Description")]
        [StringLength(4000)]
        public string? FileDescription { get; set; }
        [Column("File_Date")]
        [StringLength(10)]
        public string? FileDate { get; set; }
        [Column("File_Type")]
        public int? FileType { get; set; }
        [Column("Source_File")]
        [StringLength(500)]
        public string? SourceFile { get; set; }
        [Column("File_URL")]
        public string? FileUrl { get; set; }
        [Column("File_Ext")]
        [StringLength(50)]
        public string? FileExt { get; set; }
        public bool? IsDeleted { get; set; }
        [Column("J_Code")]
        [StringLength(255)]
        public string? JCode { get; set; }
        [Column("J_Date_Hijri")]
        [StringLength(10)]
        public string? JDateHijri { get; set; }
        [Column("J_Description")]
        [StringLength(2000)]
        public string? JDescription { get; set; }
        [Column("Period_ID")]
        public long? PeriodId { get; set; }
        [Column("Fin_year")]
        public long? FinYear { get; set; }
        [Column("Facility_ID")]
        public long? FacilityId { get; set; }
        [Column("Doc_Type_ID")]
        public int? DocTypeId { get; set; }
        public bool? FlagDelete { get; set; }
        [Column("Doc_Type_Name")]
        [StringLength(50)]
        public string? DocTypeName { get; set; }
        [Column("J_Bian")]
        [StringLength(2500)]
        public string? JBian { get; set; }
        [Column("USER_FULLNAME")]
        [StringLength(50)]
        public string? UserFullname { get; set; }
        [Column("Reference_No")]
        public long? ReferenceNo { get; set; }
        [Column("Status_Name")]
        [StringLength(50)]
        public string? StatusName { get; set; }
        [Column("Reference_Code")]
        [StringLength(50)]
        public string? ReferenceCode { get; set; }
        [Column("Payment_Type_ID")]
        public int? PaymentTypeId { get; set; }
        [Column("J_Date_Gregorian")]
        [StringLength(10)]
        public string? JDateGregorian { get; set; }
    }
}
