using System.ComponentModel.DataAnnotations;

namespace Logix.Application.DTOs.ACC
{

    //public string? PeriodStartDateHijri { get; set; }
    //[CustomRequired("FinStartDateValidation")]
    //[CustomDisplay("NumbringType", "Acc")]

    public class AccCostCenterFilterDto
    {

        public string? CostCenterCode { get; set; }

        public string? CostCenterName { get; set; }

        public string? CostCenterCodeParent { get; set; }

        public string? CostCenterNameParent { get; set; }
        public string? Code { get; set; }
        public string? Code2 { get; set; }


    }
    public class CostCenterCodeResult
    {
        public string CostCenterCode { get; set; }
        public int CostCenterLevel { get; set; }
    }
    public class AccCostCenterDto
    {


        public long CcId { get; set; }
        public long? FacilityId { get; set; }

        public long? FinYear { get; set; }

        public long? PeriodId { get; set; }

        public string? CostCenterCode { get; set; }
        [Required]
        public string? CostCenterName { get; set; }
        [Required]
        public string? CostCenterName2 { get; set; }

        public long? CcParentId { get; set; }

        public int? CreatedBy { get; set; }


        public DateTime? CreatedOn { get; set; }

        public bool? IsDeleted { get; set; }

        public int? CostCenterLevel { get; set; }

        public bool? IsParent { get; set; } = false;
        public bool? IsActive { get; set; }

        public string? CostCenterCodeParent { get; set; }

        public string? CostCenterCode2 { get; set; }

        public string? Code { get; set; }

        public string? Code2 { get; set; }

        public string? CostCenterNameParent { get; set; }

        public List<AccCostCenterDto>? Children { get; set; }
        public string? Note { get; set; }
        public bool Numbring { get; set; }
    }
    public class AccCostCenterEditDto
    {


        public long CcId { get; set; }
        [Required]
        public long? FacilityId { get; set; }


        public long? PeriodId { get; set; }
        public string? CostCenterCode { get; set; }
        [Required]
        public string? CostCenterName { get; set; }
        [Required]
        public string? CostCenterName2 { get; set; }
        public long? CcParentId { get; set; }
        public int? CostCenterLevel { get; set; }
        public bool? IsParent { get; set; }
        //public bool? IsActive { get; set; }

        public string? CostCenterCodeParent { get; set; }

        public string? CostCenterCode2 { get; set; }
        public string? Note { get; set; }
    }
}
