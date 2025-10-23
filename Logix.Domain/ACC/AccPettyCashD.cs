using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logix.Domain.Base;

namespace Logix.Domain.ACC
{
    [Table("Acc_Petty_Cash_D")]
    public partial class AccPettyCashD: TraceEntity
    {
        [Key]
        [Column("ID")]
        public long Id { get; set; }
        [Column("Petty_Cash_ID")]
        public long? PettyCashId { get; set; }
        [Column("Expense_ID")]
        public int? ExpenseId { get; set; }
        [Column("CC_ID")]
        public long? CcId { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Amount { get; set; }
        [Column("VAT_Amount", TypeName = "decimal(18, 2)")]
        public decimal? VatAmount { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Total { get; set; }
        [Column("Reference_Code")]
        [StringLength(50)]
        public string? ReferenceCode { get; set; }
        [Column("Reference_Date")]
        [StringLength(50)]
        public string? ReferenceDate { get; set; }
        [Column("Meter_Reading")]
        [StringLength(50)]
        public string? MeterReading { get; set; }
        [Column("Meter_Reading_Previous")]
        [StringLength(50)]
        public string? MeterReadingPrevious { get; set; }
        public string? Description { get; set; }
      
        [Column("CC2_ID")]
        public long? Cc2Id { get; set; }
        [Column("CC3_ID")]
        public long? Cc3Id { get; set; }
        [Column("CC4_ID")]
        public long? Cc4Id { get; set; }
        [Column("CC5_ID")]
        public long? Cc5Id { get; set; }
        [Column("Branch_ID")]
        public long? BranchId { get; set; }
        [Column("Activity_ID")]
        public long? ActivityId { get; set; }
        [Column("Assest_ID")]
        public long? AssestId { get; set; }
        [Column("Emp_ID")]
        public long? EmpId { get; set; }
        [StringLength(2500)]
        public string? SupplierName { get; set; }
        [StringLength(250)]
        public string? SupplierVatNumber { get; set; }
        [Column("Temp_ID")]
        public long? TempId { get; set; }
        [Column("VAT", TypeName = "decimal(18, 2)")]
        public decimal? Vat { get; set; }
    }
}
