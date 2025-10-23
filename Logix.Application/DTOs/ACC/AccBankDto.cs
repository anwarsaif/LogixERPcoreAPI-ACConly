using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Logix.Application.DTOs.ACC
{
    public class ACCBankUsersDto
    {

        public long? USERID { get; set; }
        public long? BankID { get; set; }

        public string? UsersPermission { get; set; }
    }
    public class AccBankFilterDto
    {
        public long? BranchId { get; set; }
        public int? Bank { get; set; }
        [StringLength(255)]
        public string? BankName { get; set; }

        [StringLength(255)]
        public string? BankName2 { get; set; }
        public string? BankAccountNo { get; set; }

        public int? StatusId { get; set; }
        public string? Iban { get; set; }

    }
    public class AccBankDto
    {

        public long BankId { get; set; }

        public string? Code { get; set; }

        [Required]
        public string? BankName { get; set; }
        [Required]
        public string? BankName2 { get; set; }


        public int? DeleteUserId { get; set; }

        [Column("Delete_Date", TypeName = "datetime")]
        public DateTime? DeleteDate { get; set; }

        public long? AccAccountId { get; set; }

        public long? FacilityId { get; set; }
        [Range(1, long.MaxValue)]
        public long? BranchId { get; set; }
        [Range(1, long.MaxValue)]
        public int? Bank { get; set; }
        [Range(1, long.MaxValue)]
        public string? BranchBank { get; set; }
        [Range(1, long.MaxValue)]
        public int? AccountType { get; set; }
        [Range(1, long.MaxValue)]
        public int? CurrencyId { get; set; }
        [Range(1, long.MaxValue)]
        public int? StatusId { get; set; }
        [Required]
        public string? BankAccountNo { get; set; }
        [Required]
        public string? Iban { get; set; }
        public string? Note { get; set; }

        public string? UsersPermission { get; set; }
        public int? CreatedBy { get; set; }

        public int? ModifiedBy { get; set; }


        public DateTime? CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public bool? IsDeleted { get; set; }
        public long RBMainAccountType { get; set; }
        public string? AccountCode { get; set; }
        public string? AccountName { get; set; }
        public string? AccountParentCode { get; set; }

        public string? AccountParentName { get; set; }
        public long? AccAccountParentID { get; set; }
    }
    public class AccBankUsersVM
    {
        public long ID { get; set; }
    }
    public class AccBankEditDto
    {

        public long BankId { get; set; }

        public string? Code { get; set; }

        [Required]
        public string? BankName { get; set; }
        [Required]
        public string? BankName2 { get; set; }


        public int? DeleteUserId { get; set; }

        [Column("Delete_Date", TypeName = "datetime")]
        public DateTime? DeleteDate { get; set; }

        public long? AccAccountId { get; set; }

        public long? FacilityId { get; set; }
        [Range(1, long.MaxValue)]
        public long? BranchId { get; set; }
        [Range(1, long.MaxValue)]
        public int? Bank { get; set; }
        [Range(1, long.MaxValue)]
        public string? BranchBank { get; set; }
        [Range(1, long.MaxValue)]
        public int? AccountType { get; set; }
        [Range(1, long.MaxValue)]
        public int? CurrencyId { get; set; }
        [Required]
        public int? StatusId { get; set; }
        [Required]
        public string? BankAccountNo { get; set; }
        [Required]
        public string? Iban { get; set; }
        public string? Note { get; set; }

        public string? UsersPermission { get; set; }

        public string? AccountCode { get; set; }
        public string? AccountName { get; set; }
        public string? AccountParentCode { get; set; }

        public string? AccountParentName { get; set; }
        public long? AccAccountParentID { get; set; }

    }
}
