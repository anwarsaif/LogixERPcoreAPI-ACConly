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
    public partial class AccCostCenteHelpVw
    {
        [Column("CostCenter_Code")]
        [StringLength(50)]
        public string? CostCenterCode { get; set; }
        [Column("CostCenter_Name")]
        [StringLength(150)]
        public string? CostCenterName { get; set; }
        [Column("CostCenter_Name2")]
        [StringLength(150)]
        public string? CostCenterName2 { get; set; }
        [Column("Is_Parent")]
        public bool? IsParent { get; set; }
        [Column("isdel")]
        public bool? Isdel { get; set; }
        [Column("CC_ID")]
        public long CcId { get; set; }
        [Column("Facility_ID")]
        public long? FacilityId { get; set; }
        public bool? IsActive { get; set; }
        [Column("CC_Parent_ID")]
        public long? CcParentId { get; set; }
    }
}
