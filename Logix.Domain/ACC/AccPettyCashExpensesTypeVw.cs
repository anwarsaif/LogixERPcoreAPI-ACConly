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
    public partial class AccPettyCashExpensesTypeVw
    {
        [Column("Acc_Account_Name")]
        [StringLength(255)]
        public string? AccAccountName { get; set; }
        [Column("Acc_Account_Code")]
        [StringLength(50)]
        public string? AccAccountCode { get; set; }
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
        public long? CreatedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedOn { get; set; }
        public long? ModifiedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ModifiedOn { get; set; }
        public bool? IsDeleted { get; set; }
        [Column("Facility_ID")]
        public int? FacilityId { get; set; }
    }
}
