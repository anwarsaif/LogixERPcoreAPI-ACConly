using System.ComponentModel.DataAnnotations;

namespace Logix.Application.DTOs.ACC
{
    public class AccFacilityDto
    {
        public long FacilityId { get; set; }
        public string? FacilityName { get; set; }
        public string? FacilityName2 { get; set; }
        public string? FacilityLogo { get; set; }
        public string? FacilityPhone { get; set; }
        public string? FacilityMobile { get; set; }
        public string? FacilityEmail { get; set; }
        public string? FacilitySite { get; set; }
        public string? FacilityAddress { get; set; }
        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public int? DeleteUserId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public DateTime? DeleteDate { get; set; }
        public bool? IsDeleted { get; set; }
        public string? BillHeader { get; set; }
        public string? BillFooter { get; set; }
        public long? AccountCash { get; set; }
        public long? AccountChequ { get; set; }
        public long? AccountCustomer { get; set; }
        public long? AccountInvestor { get; set; }
        public long? AccountSupplier { get; set; }
        public long? AccountChequUnderCollection { get; set; }
        public long? AccountCostSales { get; set; }
        public long? AccountSalesProfits { get; set; }
        public long? AccountInstallmentsUnderCollection { get; set; }
        public long? AccountInvestorProfits { get; set; }
        public long? AccountSales { get; set; }
        public long? AccountFeeManage { get; set; }
        public long? AccountProfitInstallment { get; set; }
        public long? AccountProfitInstallmentDeferred { get; set; }
        public long? AccountMerchandiseInventory { get; set; }
        public long? AccountCostGoodsSold { get; set; }
        public long? AccountCashSales { get; set; }
        public long? AccountReceivablesSales { get; set; }
        public long? AccountInventoryTransit { get; set; }
        public long? AccountBranches { get; set; }
        public long? GroupIncame { get; set; }
        public long? GroupExpenses { get; set; }
        public long? GroupAssets { get; set; }
        public long? GroupLiabilities { get; set; }
        public long? GroupCopyrights { get; set; }
        public long? AccountContractors { get; set; }
        public int? LnkInoviceInventory { get; set; }
        public int? LnkBillInventroy { get; set; }
        public long? CcIdProjects { get; set; }
        public long? DiscountAccountId { get; set; }
        public long? DiscountCreditAccountId { get; set; }
        public string? LogoPrint { get; set; }
        public string? HeaderName { get; set; }
        public string? ImgFooter { get; set; }
        public string? IdNumber { get; set; }
        public string? NoLabourOfficeFile { get; set; }
        public string? CommissionerName { get; set; }
        public int? Posting { get; set; }
        public int? LnkReInoviceInventory { get; set; }
        public int? LnkReBillInventroy { get; set; }
        public long? CcIdItems { get; set; }
        public int? LnkTransfersInventory { get; set; }
        public string? Stamp { get; set; }
        public int? SalesAccountType { get; set; }
        public bool? SeparateAccountCustomer { get; set; }
        public bool? SeparateAccountSupplier { get; set; }
        public long? ProfitAndLossAccountId { get; set; }
        public bool? UsingPurchaseAccount { get; set; }
        public long? PurchaseAccountId { get; set; }
        public bool? VatEnable { get; set; }
        public long? SalesVatAccountId { get; set; }
        public long? PurchasesVatAccountId { get; set; }
        public string? VatNumber { get; set; }
        public long? AccountDeferredIncome { get; set; }
        public bool? SeparateAccountContractor { get; set; }
        public long? AccountMembers { get; set; }

        public int? IdentitiType { get; set; }

        [StringLength(50)]
        public string? PostalCode { get; set; }

        [StringLength(50)]
        public string? RegionName { get; set; }

        [StringLength(100)]
        public string? StreetName { get; set; }

        [StringLength(100)]
        public string? DistrictName { get; set; }

        [StringLength(50)]
        public string? BuildingNumber { get; set; }
        [StringLength(50)]
        public string? CountryCode { get; set; }
        [StringLength(50)]
        public string? AdditionalStreetAddress { get; set; }
    }

    public class AccFacilityFilterDto
    {
        public string? FacilityName { get; set; }
        public string? IdNumber { get; set; }
        public string? NoLabourOfficeFile { get; set; }
        public string? FacilityPhone { get; set; }
        public string? FacilityEmail { get; set; }
        public string? CommissionerName { get; set; }
        public string? FacilityAddress { get; set; }
    }

    public class AccFacilityEditDto
    {
        public long FacilityId { get; set; }
        [Required]
        public string? FacilityName { get; set; }
        [Required]
        public string? FacilityName2 { get; set; }
        [Required]
        public string? CommissionerName { get; set; }
        [Required]
        public string? FacilityMobile { get; set; }
        [Required]
        public string? FacilityPhone { get; set; }
        [Required]
        public string? FacilitySite { get; set; }
        [Required]
        public string? FacilityEmail { get; set; }
        [Required]
        public string? IdNumber { get; set; }
        [Required]
        public string? NoLabourOfficeFile { get; set; }
        public string? VatNumber { get; set; }
        [Required]
        public string? FacilityAddress { get; set; }

        public int? IdentitiType { get; set; }
        [StringLength(50)]
        public string? RegionName { get; set; }
        [StringLength(100)]
        public string? StreetName { get; set; }
        [StringLength(100)]
        public string? DistrictName { get; set; }

        [StringLength(50)]
        public string? BuildingNumber { get; set; }
        [StringLength(50)]
        public string? PostalCode { get; set; }
        [StringLength(50)]
        public string? CountryCode { get; set; }
        [StringLength(50)]
        public string? AdditionalStreetAddress { get; set; }

        //public IFormFile? FileUploaded { get; set; }
        //public string? Stamp { get; set; }
    }

    public class AccFacilityEditProfileDto
    {
        public long FacilityId { get; set; }
        [Required]
        public string? FacilityName { get; set; }
        [Required]
        public string? FacilityName2 { get; set; }
        [Required]
        public string? CommissionerName { get; set; }
        [Required]
        public string? FacilityMobile { get; set; }
        [Required]
        public string? FacilityPhone { get; set; }
        [Required]
        public string? FacilitySite { get; set; }
        [Required]
        public string? FacilityEmail { get; set; }
        [Required]
        public string? IdNumber { get; set; }
        [Required]
        public string? NoLabourOfficeFile { get; set; }
        public string? VatNumber { get; set; }
        public int? IdentitiType { get; set; }

        [Required]
        public string? FacilityAddress { get; set; }

        //public IFormFile? FacilityLogoUploaded { get; set; }
        public string? FacilityLogo { get; set; }

        //public IFormFile? LogoPrintUploaded { get; set; }
        public string? LogoPrint { get; set; }

        //public IFormFile? ImgFooterUploaded { get; set; }
        public string? ImgFooter { get; set; }

        //public IFormFile? StampUploaded { get; set; }
        //public string? Stamp { get; set; }
    }

    public class AccFacilityProfileDto
    {
        public long FacilityId { get; set; }
        [Required]
        public string? FacilityName { get; set; }
        [Required]
        public string? FacilityName2 { get; set; }
        [Required]

        public string? CommissionerName { get; set; }
        [Required]

        public string? FacilityMobile { get; set; }
        [Required]

        public string? FacilityPhone { get; set; }
        [Required]
        public string? FacilitySite { get; set; }
        [Required]
        public string? FacilityEmail { get; set; }
        [Required]
        public string? IdNumber { get; set; }

        [Required]
        public string? FacilityAddress { get; set; }

        public string? FacilityLogo { get; set; }
        [Required]
        public string? NoLabourOfficeFile { get; set; }
        public string? VatNumber { get; set; }
        public int? IdentitiType { get; set; }
        public int? Posting { get; set; }
        public int? SalesAccountType { get; set; }
        public long? AccountCash { get; set; }
        public string? AccountCodeCash { get; set; }
        public string? AccountNameCash { get; set; }
        public long? AccountChequ { get; set; }
        public string? AccountCodeChequ { get; set; }
        public string? AccountNameChequ { get; set; }
        public long? AccountSupplier { get; set; }

        public string? AccountCodeSupplier { get; set; }
        public string? AccountNameSupplier { get; set; }
        public long? AccountContractors { get; set; }

        public string? AccountCodeContractors { get; set; }
        public string? AccountNameContractors { get; set; }
        public long? AccountChequUnderCollection { get; set; }

        public string? AccountCodeChequundercollection { get; set; }
        public string? AccountNameChequundercollection { get; set; }
        public long? AccountCostSales { get; set; }

        public string? AccountCodeCostsales { get; set; }
        public string? AccountNameCostsales { get; set; }
        public long? AccountSalesProfits { get; set; }

        public string? AccountCodeSalesprofits { get; set; }
        public string? AccountNameSalesprofits { get; set; }

        public long? AccountSales { get; set; }

        public string? AccountCodeSales { get; set; }
        public string? AccountNameSales { get; set; }
        public long? PurchaseAccountId { get; set; }

        public bool UsingPurchaseAccount { get; set; }
        public string? PurchaseAccountCode { get; set; }
        public string? PurchaseAccountName { get; set; }
        public long? AccountMerchandiseInventory { get; set; }
        public string? AccountcodemerchandiseInventory { get; set; }
        public string? AccountNamemerchandiseInventory { get; set; }
        public long? AccountCostGoodsSold { get; set; }

        public string? AccountcodeCostGoodsSold { get; set; }
        public string? AcccountnameCostGoodsSold { get; set; }
        public long? AccountCashSales { get; set; }

        public string? AccountcodeCashsales { get; set; }
        public string? AccountNameCashsales { get; set; }
        public long? AccountReceivablesSales { get; set; }

        public string? AccountcodeReceivablesSales { get; set; }
        public string? AccountnameReceivablesSales { get; set; }
        public long? AccountMembers { get; set; }

        public string? MemberAccountCode { get; set; }
        public string? MemberAccountName { get; set; }
        public long? AccountInventoryTransit { get; set; }

        public string? AccountcodeInventoryTransit { get; set; }
        public string? AccountnameInventoryTransit { get; set; }
        public long? AccountBranches { get; set; }
        public string? AccountcodeBranches { get; set; }
        public string? AccountnameBranches { get; set; }
        public long? GroupAssets { get; set; }

        public string? GroupAssetsCode { get; set; }
        public string? GroupAssetsName { get; set; }
        public long? GroupLiabilities { get; set; }

        public string? GroupLiabilitiesCode { get; set; }
        public string? GroupLiabilitiesName { get; set; }
        public long? GroupCopyrights { get; set; }

        public string? GroupCopyrightsCode { get; set; }
        public string? GroupCopyrightsName { get; set; }
        public long? GroupIncame { get; set; }

        public string? GroupIncameCode { get; set; }
        public string? GroupIncameName { get; set; }
        public long? GroupExpenses { get; set; }

        public string? GroupExpensesCode { get; set; }
        public string? GroupExpensesName { get; set; }
        public long? CcIdProjects { get; set; }

        public string? CostCenterCode { get; set; }
        public string? CostCenterName { get; set; }
        public int? LnkInoviceInventory { get; set; }
        public int? LnkBillInventroy { get; set; }
        public long? DiscountAccountId { get; set; }

        public string? AccountDiscountCode { get; set; }
        public string? AccountDiscountName { get; set; }
        public long? DiscountCreditAccountId { get; set; }

        public string? AccountDiscountCreditCode { get; set; }
        public string? AccountDiscountCreditName { get; set; }
        public long? ProfitAndLossAccountId { get; set; }

        public string? profitandLossAccountCode { get; set; }
        public string? profitandLossAccountName { get; set; }
        public long? SalesVatAccountId { get; set; }

        public string? SalesVATAccountCode { get; set; }
        public string? SalesVATAccountName { get; set; }
        public long? PurchasesVatAccountId { get; set; }
        public bool? VatEnable { get; set; }

        public string? PurchasesVATAccountCode { get; set; }
        public string? PurchasesVATAccountName { get; set; }
        public string? LogoPrint { get; set; }
        public string? ImgFooter { get; set; }
    }

}
