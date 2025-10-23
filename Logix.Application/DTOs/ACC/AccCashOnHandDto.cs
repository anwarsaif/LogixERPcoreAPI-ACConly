using System.ComponentModel.DataAnnotations;

namespace Logix.Application.DTOs.ACC
{
    public class AccCashOnHandFilterDto
    {
        public long? BranchId { get; set; }
        public long? Code { get; set; }
        [StringLength(250)]
        public string? Name { get; set; }
        [StringLength(250)]
        public string? Name2 { get; set; }
    }
    public class AccCashOnHandUsersDto
    {

        public long? ID { get; set; }

        public string? UsersPermission { get; set; }
    }
    public class AccCashOnHandDto
    {

        public long Id { get; set; }

        public long? Code { get; set; }
        [Required]
        [StringLength(250)]
        public string? Name { get; set; }
        [Required]
        [StringLength(250)]
        public string? Name2 { get; set; }
        [StringLength(2500)]
        public string? Description { get; set; }
        [Range(1, long.MaxValue)]
        public long? BranchId { get; set; }

        public long? FacilityId { get; set; }

        public long? AccAccountId { get; set; }
        public long? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }
        public long? ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }
        public bool? IsDeleted { get; set; }

        public string? UsersPermission { get; set; }
        public long RBMainAccountType { get; set; }
        public string? AccountCode { get; set; }
        public string? AccountName { get; set; }
        public string? AccountParentCode { get; set; }

        public string? AccountParentName { get; set; }
        public long? AccAccountParentID { get; set; }
    }
    public class AccCashOnHandEditDto
    {

        public long Id { get; set; }
        public long? Code { get; set; }
        [Required]
        [StringLength(250)]
        public string? Name { get; set; }
        [Required]
        [StringLength(250)]
        public string? Name2 { get; set; }
        [StringLength(2500)]
        public string? Description { get; set; }
        [Range(1, long.MaxValue)]

        public long? BranchId { get; set; }


        public long? AccAccountId { get; set; }

        [StringLength(258)]
        public string? AccAccountName { get; set; }

        [StringLength(50)]
        public string? AccAccountCode { get; set; }
        public string? AccountName { get; set; }
        public string? AccountParentCode { get; set; }

        public string? AccountParentName { get; set; }

        public long? AccAccountParentID { get; set; }

    }
}
