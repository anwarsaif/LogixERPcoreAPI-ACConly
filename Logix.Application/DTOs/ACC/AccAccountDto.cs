using System.ComponentModel.DataAnnotations;

namespace Logix.Application.DTOs.ACC
{
    public class AccAccountPopDto
    {

        [StringLength(50)]
        public string? AccAccountCode { get; set; }
        [StringLength(255)]
        public string? AccAccountName { get; set; }

    }
    public class AccAccountFilterDto
    {
        public string? Code { get; set; }
        public string? Code2 { get; set; }

        [StringLength(50)]
        public string? AccAccountCode { get; set; }
        [StringLength(255)]
        public string? AccAccountName { get; set; }

        public long? AccAccountParentId { get; set; }

        public bool? IsActive { get; set; }

        public long? AccGroupId { get; set; }


        [StringLength(50)]
        public string? AccAccountParentCode { get; set; }


        [StringLength(250)]
        public string? AccAccountnameParent { get; set; }


        public int? AccountLevel { get; set; }

    }
    public class AccAccountAddDto
    {

        //public long AccAccountId { get; set; }


        [StringLength(50)]
        public string? AccAccountCode { get; set; }
        [Required]

        [StringLength(255)]
        public string? AccAccountName { get; set; }

        [StringLength(255)]
        public string? AccAccountName2 { get; set; }
        [Range(1, long.MaxValue)]
        public long? AccGroupId { get; set; }
        [Range(1, long.MaxValue)]


        public long? AccAccountParentId { get; set; }
        ///// <summary>
        ///// مركز التكلفة الافتراضي
        [Range(1, long.MaxValue)]
        public long? CcId { get; set; }



        [Range(1, long.MaxValue)]
        public int? AccountCloseTypeId { get; set; }
        [Required]

        public int? AccountLevel { get; set; }




        public bool? IsActive { get; set; }


        [Range(1, long.MaxValue)]
        public int? CurrencyId { get; set; }



    }

    public class AccAccountExcelDto
    {

        [StringLength(50)]
        public string? AccAccountCode { get; set; }


        [StringLength(255)]
        public string? AccAccountName { get; set; }

        [StringLength(255)]
        public string? AccAccountName2 { get; set; }
        public int? CurrencyId { get; set; }

        public int? AccountLevel { get; set; }
        public string? CostCenterCode { get; set; }

        public bool? IsSub { get; set; }

        public bool? IsHelpAccount { get; set; }
        public long? AccGroupId { get; set; }



        [StringLength(50)]
        public string? AccAccountParentCode { get; set; }
        [StringLength(250)]
        public string? AccAccountnameParent { get; set; }
    }
    public class AccAccountExcelVM
    {
        public AccAccountExcelVM(AccAccountResultExcelDto accAccountResultExcelDto, List<AccAccountResultExcelDto> children)
        {
            AccAccountResultExcelDto = accAccountResultExcelDto;
            Children = children;
        }

        public AccAccountResultExcelDto AccAccountResultExcelDto { get; set; }
        public List<AccAccountResultExcelDto> Children { get; set; } = new();
    }


    public class AccAccountResultExcelDto
    {
        [StringLength(50)]
        public string? AccAccountCode { get; set; }
        
        [StringLength(255)]
        public string? AccAccountName { get; set; }

        [StringLength(255)]
        public string? AccAccountName2 { get; set; }
        public int? CurrencyId { get; set; }
        public int? AccountLevel { get; set; }
        public string? CostCenterCode { get; set; }
        public bool IsSub { get; set; }
        public string? CostCenterName { get; set; }
        public string? CurrencyName { get; set; }
        public bool IsHelpAccount { get; set; }
        public long? AccGroupId { get; set; }
        [StringLength(50)]
        public string? AccAccountParentCode { get; set; }
        [StringLength(250)]
        public string? AccAccountnameParent { get; set; }
        public List<AccAccountResultExcelDto> Children { get; set; }

    }
    public class AccAccountDto
    {

        public long AccAccountId { get; set; }
        [Required]


        [StringLength(255)]
        public string? AccAccountName { get; set; }
        [Required]
        [StringLength(255)]

        public string? AccAccountName2 { get; set; }
        [Range(1, long.MaxValue)]

        public long? AccGroupId { get; set; }
        //[Required]


        [StringLength(50)]
        public string? AccAccountCode { get; set; }

        public bool IsSub { get; set; }
        [Range(1, long.MaxValue)]

        public long? AccAccountParentId { get; set; }
        /// <summary>
        /// مركز التكلفة الافتراضي
        [Range(1, long.MaxValue)]

        public long? CcId { get; set; }

        public int? CreatedBy { get; set; }

        public int? ModifiedBy { get; set; }

        public int? DeleteUserId { get; set; }

        public DateTime? CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public DateTime? DeleteDate { get; set; }
        public bool? IsDeleted { get; set; }
        [Range(1, long.MaxValue)]

        public int? AccountCloseTypeId { get; set; }
        //[Required]


        public int? AccountLevel { get; set; }

        public bool IsHelpAccount { get; set; }

        public bool? Aggregate { get; set; }

        public bool? IsActive { get; set; }

        public long? FacilityId { get; set; }
        [Range(1, long.MaxValue)]

        public int? CurrencyId { get; set; }

        public int? BranchId { get; set; }

        public long? AccAccountParentId2 { get; set; }

        public long? AccAccountParentId3 { get; set; }
        //[Required]


        [StringLength(50)]
        public string? AccAccountParentCode { get; set; }
        //[Required]

        [StringLength(250)]
        public string? AccAccountnameParent { get; set; }


        public string? Note { get; set; }
        public bool Numbring { get; set; }



        public int? AccAccountType { get; set; }



        public int? DeptID { get; set; }
        public string? Code { get; set; }
        public string? Code2 { get; set; }
        public int? SystemId { get; set; }
    }

    public class AccAccountEditDto
    {
        public long AccAccountId { get; set; }
        [Required]


        [StringLength(255)]
        public string? AccAccountName { get; set; }
        [Required]
        [StringLength(255)]

        public string? AccAccountName2 { get; set; }
        [Range(1, long.MaxValue)]

        public long? AccGroupId { get; set; }
        [Required]

        [StringLength(50)]
        public string? AccAccountCode { get; set; }
        public bool IsSub { get; set; }
        [Range(1, long.MaxValue)]

        public long? AccAccountParentId { get; set; }
        /// <summary>
        /// مركز التكلفة الافتراضي
        [Range(1, long.MaxValue)]
        public long? CcId { get; set; }

        [Range(1, long.MaxValue)]
        public int? AccountCloseTypeId { get; set; }
        [Required]


        public int? AccountLevel { get; set; }
        public bool IsHelpAccount { get; set; }

        public bool? Aggregate { get; set; }
        [Range(1, long.MaxValue)]
        public bool? IsActive { get; set; }

        [Range(1, long.MaxValue)]


        public int? CurrencyId { get; set; }

        public int? BranchId { get; set; }


        public string? Note { get; set; }
        public bool Numbring { get; set; }
    }

    public class AccountBalanceSheetDto
    {
        public int RowID { get; set; }
        public decimal? Balance { get; set; }
        public string? JDateGregorian { get; set; }
        public string? ReferenceDNo { get; set; }
        public decimal? Credit { get; set; }
        public decimal? Debit { get; set; }
        public int? SortNo { get; set; }
        public string? Description { get; set; }
        public string? CostCenterName2 { get; set; }
        public string? CostCenterName { get; set; }
        public string? JCode { get; set; }
        public string? DocTypeName { get; set; }
        public string? DocTypeName2 { get; set; }
        public string? ReferenceCode { get; set; }
        public string? ReferenceTypeName { get; set; }
        public string? ReferenceTypeName2 { get; set; }
        public int? NatureAccount { get; set; }
        public int? DocTypeId { get; set; }
        public long? JId { get; set; }
        public long? JDetailesId { get; set; }
        public long? ReferenceNo { get; set; }
        public decimal? CostsCenter { get; set; }
        public string? ChequNo { get; set; }
    }
    public class AccountBalanceSheetFilterDto
    {
        public long? FacilityId { get; set; }
        public int? FinancialYear { get; set; }
        public int? BranchId { get; set; }
        public int? referenceTypeId { get; set; }
        public string? EmpCode { get; set; }
        public string? EmpName { get; set; }
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public bool? chkAllYear { get; set; } = false;

    }


    public class AccountFromToFilterDto
    {
        public long? FacilityId { get; set; } = 0;
        public int? FinancialYear { get; set; }
        public int? BranchId { get; set; }
        public int? referenceTypeId { get; set; }
        public string? EmpCode { get; set; }
        public string? EmpName { get; set; }
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public string? CodeTo { get; set; }
        public string? CodeFrom { get; set; }
        public bool? NoZero { get; set; } = false;
        public decimal? Debit { get; set; }
        public decimal? Credit { get; set; }
        public string? AmountPrevDebit { get; set; }
        public string? AmountPrevCredit { get; set; }
        public string? AmountCurrentCredit { get; set; }
        public string? AmountCurrentDebit { get; set; }
        public string? AccountId { get; set; }
        public string? AccountName { get; set; }

    }


    public class AccounttransactionsDto
    {
        public string? AccountCode { get; set; }
        public string? CostCenterCode { get; set; }
        public string? dateFrom { get; set; }
        public string? dateTo { get; set; }
        public int? branchId { get; set; }
        public long? accountId { get; set; }
        public long facilityId { get; set; }
        public long FinYear { get; set; }
        public bool chkAllYear { get; set; } = false;
        public long? ReferenceDNo { get; set; }
        public int? ParentReferenceTypeId { get; set; }
        public long? ccId { get; set; }
        public int? ReferenceTypeId { get; set; }
    }


    public class FundsstatementtransactionsDto
    {

        public string? dateFrom { get; set; }
        public string? dateTo { get; set; }
        public int? branchId { get; set; }
        public long? accountId { get; set; }

        public bool chkAllYear { get; set; } = false;


    }
    public class CustomerAccountStatementDto
    {
        public string? AccountCode { get; set; }
        public string? CostCenterCode { get; set; }
        public string? dateFrom { get; set; }
        public string? dateTo { get; set; }
        public int? branchId { get; set; }
        public bool chkAllYear { get; set; } = false;
        public int? ReferenceTypeId { get; set; }
    }

    public class ContractorsAccountStatementDto
    {
        public string? AccountCode { get; set; }
        public string? CostCenterCode { get; set; }
        public string? dateFrom { get; set; }
        public string? dateTo { get; set; }
        public int? branchId { get; set; }
        public bool chkAllYear { get; set; } = false;
        public int? ReferenceTypeId { get; set; }
    }


    public class SupplierAccountStatementDto
    {
        public string? AccountCode { get; set; }
        public string? CostCenterCode { get; set; }
        public string? dateFrom { get; set; }
        public string? dateTo { get; set; }
        public int? branchId { get; set; }
        public bool chkAllYear { get; set; } = false;
        public int? ReferenceTypeId { get; set; }
    }




    public class AccounttransactionsFilterGroupDto
    {

        public long facilityId { get; set; }

        public string? AccountCodeParent { get; set; }
        public long FinYear { get; set; }

        public string? dateFrom { get; set; }
        public string? dateTo { get; set; }
        public int? branchId { get; set; }
        public string? CostCenterCodeFrom { get; set; }
        public string? CostCenterCodeTo { get; set; }
    }
    public class AccounttransactionsGroupDto
    {
        public string AccAccountName { get; set; }
        public string AccAccountName2 { get; set; }
        public string AccAccountCode { get; set; }
        public decimal? Debit { get; set; }
        public decimal? Credit { get; set; }
        public decimal? Balance { get; set; }

        public decimal? Rate { get; set; }
    }


}
