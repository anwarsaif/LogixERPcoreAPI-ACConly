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
    public partial class AccSettlementInstallmentsVw
    {
        [Column("ID")]
        public long Id { get; set; }
        [Column("Installment_No")]
        public long? InstallmentNo { get; set; }
        [Column("SS_ID")]
        public long? SsId { get; set; }
        [Column("Installment_Value", TypeName = "decimal(18, 2)")]
        public decimal? InstallmentValue { get; set; }
        [Column("Installment_Date")]
        [StringLength(10)]
        public string? InstallmentDate { get; set; }
        public string? Description { get; set; }
        public long CreatedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedOn { get; set; }
        public long? ModifiedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ModifiedOn { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsDeletedM { get; set; }
        [Column("Facility_ID")]
        public long? FacilityId { get; set; }
        [Column("Branch_ID")]
        public long? BranchId { get; set; }
        [Column("Status_ID")]
        public int? StatusId { get; set; }
        public string? DescriptionM { get; set; }
        [Column("J_Code")]
        [StringLength(255)]
        public string? JCode { get; set; }
        [Column("Reference_Code")]
        [StringLength(50)]
        public string? ReferenceCode { get; set; }
        public long? Code { get; set; }
        [Column("Currency_ID")]
        public int? CurrencyId { get; set; }
        [Column("Exchange_Rate", TypeName = "decimal(18, 10)")]
        public decimal? ExchangeRate { get; set; }

    }
}
