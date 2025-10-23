using System.ComponentModel.DataAnnotations;

namespace Logix.Application.DTOs.ACC
{
    public class AccAccountsCostcenterFilterDto
    {

        public long Id { get; set; }
        public long? AccAccountId { get; set; }
        public long? CcNo { get; set; }

        public string? CcIdFrom { get; set; }

        public string? CcIdTo { get; set; }

        public bool? IsRequired { get; set; }

        public bool? IsEditable { get; set; }

        public bool? IsDeleted { get; set; }

        public long? CcIdDefault { get; set; }
        [StringLength(50)]

        public string? AccAccountCode { get; set; }
        [StringLength(255)]

        public string? AccAccountName { get; set; }
        public string? CostCenterCode { get; set; }
        public string? CostCenterName { get; set; }
    }
    public class AccAccountsCostcenterDto
    {

        public long Id { get; set; }
        public long? AccAccountId { get; set; }
        public long? CcNo { get; set; }

        public string? CcIdFrom { get; set; }

        public string? CcIdTo { get; set; }

        public bool? IsRequired { get; set; }

        public bool? IsEditable { get; set; }

        public bool? IsDeleted { get; set; }

        public long? CcIdDefault { get; set; }
        [StringLength(50)]
        [Required]

        public string? AccAccountCode { get; set; }
        [StringLength(255)]
        [Required]

        public string? AccAccountName { get; set; }
        public string? CostCenterCode { get; set; }
        public string? CostCenterName { get; set; }
    }
    public class AccAccountsCostcenterEditDto
    {

        public long Id { get; set; }
        public long? AccAccountId { get; set; }
        public long? CcNo { get; set; }

        public string? CcIdFrom { get; set; }

        public string? CcIdTo { get; set; }

        public bool? IsRequired { get; set; }

        public bool? IsEditable { get; set; }


        public long? CcIdDefault { get; set; }
    }
}
