using Logix.Domain.Base;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Logix.Domain.ACC
{
    [Keyless]
    public partial class AccRequestEmployeeVw : TraceEntity
    {

        [Column("ID")]
        public long Id { get; set; }

        [Column("Acc_Request_ID")]
        public long? AccRequestId { get; set; }

        [Column("Reference_Type_ID")]
        public long? ReferenceTypeId { get; set; }

        [Column("Acc_Account_ID")]
        public long? AccAccountId { get; set; }


        [Column("Account_Name")]
        [StringLength(255)]
        public string? AccountName { get; set; }

        [Column("Reference_Type_Name")]
        [StringLength(50)]
        public string? ReferenceTypeName { get; set; }

        [Column("Account_Code")]
        [StringLength(50)]
        public string? AccountCode { get; set; }

        [Column("Reference_No")]
        public long? ReferenceNo { get; set; }

        [StringLength(50)]
        public string? Code { get; set; }

        [StringLength(4000)]
        public string? Name { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Amount { get; set; }

        public string? Amountwrite { get; set; }

        [Column("CC_ID")]
        public long? CcId { get; set; }

        [Column("CostCenter_Code")]
        [StringLength(50)]
        public string? CostCenterCode { get; set; }

        [Column("CostCenter_Name")]
        [StringLength(150)]
        public string? CostCenterName { get; set; }

        public string? Note { get; set; }

        [Column("Refrance_No")]
        [StringLength(250)]
        public string? RefranceNo { get; set; }

        [Column("Status_ID")]
        public int? StatusId { get; set; }
    }
}
