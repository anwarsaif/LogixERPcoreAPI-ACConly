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
    public partial class AccSettlementScheduleDVw
    {
        [Column("ID")]
        public long Id { get; set; }
        [Column("SS_ID")]
        public long? SsId { get; set; }
        [Column("Acc_Account_ID")]
        public long? AccAccountId { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Debit { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Credit { get; set; }
        [Column("Reference_Type_ID")]
        public int? ReferenceTypeId { get; set; }
        [Column("Reference_No")]
        public long? ReferenceNo { get; set; }
        [StringLength(2000)]
        public string? Description { get; set; }
        public bool IsDeleted { get; set; }
        [Column("Acc_Account_Name")]
        [StringLength(258)]
        public string? AccAccountName { get; set; }
        [Column("Acc_Account_Code")]
        [StringLength(50)]
        public string? AccAccountCode { get; set; }
        [Column("Acc_group_ID")]
        public long? AccGroupId { get; set; }
        [Column("Acc_Account_Name2")]
        [StringLength(255)]
        public string? AccAccountName2 { get; set; }
        [Column("CC_ID")]
        public long? CcId { get; set; }
        [Column("CC_Code")]
        [StringLength(50)]
        public string? CcCode { get; set; }
        [Column("CostCenter_Name")]
        [StringLength(150)]
        public string? CostCenterName { get; set; }
        public string? Code { get; set; }
        [StringLength(4000)]
        public string? Name { get; set; }
        [Column("Exchange_Rate", TypeName = "decimal(18, 10)")]
        public decimal? ExchangeRate { get; set; }
        [Column("Currency_ID")]
        public int? CurrencyId { get; set; }
        [Column("Reference_Type_Name")]
        [StringLength(50)]
        public string? ReferenceTypeName { get; set; }
        [Column("Parent_ID")]
        public int? ParentId { get; set; }
        [Column("CC2_ID")]
        public long? Cc2Id { get; set; }
        [Column("CC3_ID")]
        public long? Cc3Id { get; set; }
        [Column("Reference_Code")]
        [StringLength(50)]
        public string? ReferenceCode { get; set; }
        [Column("CC4_ID")]
        public long? Cc4Id { get; set; }
        [Column("CC5_ID")]
        public long? Cc5Id { get; set; }
        [Column("Branch_ID")]
        public long? BranchId { get; set; }
        [Column("Activity_ID")]
        public long? ActivityId { get; set; }
        [Column("Emp_ID")]
        public long? EmpId { get; set; }
        [Column("Assest_ID")]
        public long? AssestId { get; set; }
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
        [Column("Assest_Code")]
        [StringLength(50)]
        public string? AssestCode { get; set; }
        [Column("Assest_Name")]
        [StringLength(4000)]
        public string? AssestName { get; set; }
        [Column("Emp_name")]
        [StringLength(250)]
        public string? EmpName { get; set; }
        [Column("Emp_Code")]
        [StringLength(50)]
        public string? EmpCode { get; set; }
    }
}
