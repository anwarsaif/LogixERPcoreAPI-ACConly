using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
    [Table("ACC_Petty_Cash_Expenses_Type")]
    public partial class AccPettyCashExpensesType:TraceEntity
    {
        [Key]
        [Column("ID")]
        public long Id { get; set; }
        [StringLength(250)]
        public string? Name { get; set; }
        [StringLength(250)]
        public string? Name2 { get; set; }
        [Column("Vat_Rate", TypeName = "decimal(18, 2)")]
        public decimal? VatRate { get; set; }
        [Column("Link_Accounting")]
        public bool? LinkAccounting { get; set; }
        [Column("Acc_Account_ID")]
        public long? AccAccountId { get; set; }
      
        [Column("Facility_ID")]
        public int? FacilityId { get; set; }
    }
}
