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
    public partial class AccJournalMasterExportVw
    {
        [Column("J_Code")]
        [StringLength(255)]
        public string? JCode { get; set; }
        [Column("J_Date_Gregorian")]
        [StringLength(10)]
        public string? JDateGregorian { get; set; }
        [Column("Acc_Account_Code")]
        [StringLength(50)]
        public string? AccAccountCode { get; set; }
        [Column("Acc_Account_Name")]
        [StringLength(258)]
        public string? AccAccountName { get; set; }
        [Column("CostCenter_Code")]
        [StringLength(50)]
        public string? CostCenterCode { get; set; }
        [Column("CostCenter_Name")]
        [StringLength(150)]
        public string? CostCenterName { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Debit { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Credit { get; set; }
        [StringLength(2000)]
        public string? Description { get; set; }
        [Column("D_Date_Gregorian")]
        [StringLength(10)]
        public string? DDateGregorian { get; set; }
        [Column("Period_ID")]
        public long? PeriodId { get; set; }
        [Column("Fin_year")]
        public long? FinYear { get; set; }
        public bool? FlagDelete { get; set; }
        [Column("Facility_ID")]
        public long? FacilityId { get; set; }
        [Column("Doc_Type_ID")]
        public int? DocTypeId { get; set; }
        [Column("J_ID")]
        public long JId { get; set; }
        [Column("USER_FULLNAME")]
        [StringLength(50)]
        public string? UserFullname { get; set; }
        [Column("Doc_Type_Name")]
        [StringLength(50)]
        public string? DocTypeName { get; set; }
        [Column("Status_Name")]
        [StringLength(50)]
        public string? StatusName { get; set; }
        [Column("Fin_year_Gregorian")]
        public int FinYearGregorian { get; set; }
        [Column("Reference_No")]
        public long? ReferenceNo { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Amount { get; set; }
        [Column("J_Description")]
        [StringLength(2000)]
        public string? JDescription { get; set; }
        [StringLength(4000)]
        public string? AmountWrite { get; set; }
        [Column("J_Bian")]
        [StringLength(2500)]
        public string? JBian { get; set; }
        [StringLength(4000)]
        public string? Name { get; set; }
        public string? Code { get; set; }
        [Column("Reference_Type_ID")]
        public int? ReferenceTypeId { get; set; }
        public int SortCol { get; set; }
        [Column("Chequ_No")]
        [StringLength(50)]
        public string? ChequNo { get; set; }
        [Column("Chequ_Date_Hijri")]
        [StringLength(10)]
        public string? ChequDateHijri { get; set; }
        [Column("Debit_Curr", TypeName = "decimal(18, 2)")]
        public decimal? DebitCurr { get; set; }
        [Column("Credit_Curr", TypeName = "decimal(18, 2)")]
        public decimal? CreditCurr { get; set; }
        [Column("Reference_Code")]
        [StringLength(50)]
        public string? ReferenceCode { get; set; }
        [Column("Status_Show")]
        public string? StatusShow { get; set; }
        [Column("Status_Id")]
        public int? StatusId { get; set; }
        [Column("Branch_ID")]
        public long? BranchId { get; set; }
        [Column("BRA_NAME")]
        public string? BraName { get; set; }
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
        [Column("CostCenter_Code_All")]
        [StringLength(262)]
        public string? CostCenterCodeAll { get; set; }
        [Column("CostCenter_Name_All")]
        [StringLength(762)]
        public string? CostCenterNameAll { get; set; }
        [Column("Reference_Type_Name")]
        [StringLength(50)]
        public string? ReferenceTypeName { get; set; }
        [Column("Emp_Code")]
        [StringLength(50)]
        public string? EmpCode { get; set; }
        [Column("Acc_Account_Name2")]
        [StringLength(255)]
        public string? AccAccountName2 { get; set; }
    }
}
