using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Logix.Domain.Base;
using Microsoft.EntityFrameworkCore;

namespace Logix.Domain.ACC
{
    [Table("ACC_CostCenter")]
    public partial class AccCostCenter: TraceEntity
    {
        [Key]
        [Column("CC_ID")]
        public long CcId { get; set; }
        [Column("Facility_ID")]
        public long? FacilityId { get; set; }
        [Column("Fin_year")]
        public long? FinYear { get; set; }
        [Column("Period_ID")]
        public long? PeriodId { get; set; }
        [Column("CostCenter_Code")]
        [StringLength(50)]
        public string? CostCenterCode { get; set; }
        [Column("CostCenter_Name")]
        [StringLength(150)]
        public string? CostCenterName { get; set; }
        [Column("CostCenter_Name2")]
        [StringLength(150)]
        public string? CostCenterName2 { get; set; }
        [Column("CC_Parent_ID")]
        public long? CcParentId { get; set; }
        //[Column("Insert_User_ID")]
        //public int? InsertUserId { get; set; }
        //[Column("Update_User_ID")]
        //public int? UpdateUserId { get; set; }
        [Column("Delete_User_ID")]
        public int? DeleteUserId { get; set; }
        //[Column("Insert_Date", TypeName = "datetime")]
        //public DateTime? InsertDate { get; set; }
        //[Column("Update_Date", TypeName = "datetime")]
        //public DateTime? UpdateDate { get; set; }
        [Column("Delete_Date", TypeName = "datetime")]
        public DateTime? DeleteDate { get; set; }
        //public bool? FlagDelete { get; set; }
        [Column("CostCenter_Level")]
        public int? CostCenterLevel { get; set; }
        [Column("Is_Parent")]
        public bool? IsParent { get; set; }
        public bool? IsActive { get; set; }
        [Column("CostCenter_Code_Parent")]
        [StringLength(50)]
        public string? CostCenterCodeParent { get; set; }
        [Column("CostCenter_Code2")]
        [StringLength(50)]
        public string? CostCenterCode2 { get; set; }
        public string? Note { get; set; }
    }
}
