using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Logix.Domain.ACC
{
    [Keyless]
    public partial class AccFacilitiesVw
    {
        [Column("Facility_ID")]
        public long FacilityId { get; set; }
        [Column("Facility_Name")]
        [StringLength(500)]
        public string? FacilityName { get; set; }
        [Column("Facility_Name2")]
        [StringLength(500)]
        public string? FacilityName2 { get; set; }
        [Column("Facility_Logo")]
        [StringLength(2000)]
        public string? FacilityLogo { get; set; }
        [Column("Facility_Phone")]
        [StringLength(50)]
        public string? FacilityPhone { get; set; }
        [Column("Facility_mobile")]
        [StringLength(50)]
        public string? FacilityMobile { get; set; }
        [Column("Facility_Email")]
        [StringLength(50)]
        public string? FacilityEmail { get; set; }
        [Column("Facility_Site")]
        [StringLength(50)]
        public string? FacilitySite { get; set; }
        [Column("Facility_Address")]
        [StringLength(2000)]
        public string? FacilityAddress { get; set; }
        [Column("Insert_User_ID")]
        public int? InsertUserId { get; set; }
        [Column("Update_User_ID")]
        public int? UpdateUserId { get; set; }
        [Column("Delete_User_ID")]
        public int? DeleteUserId { get; set; }
        [Column("Insert_Date", TypeName = "datetime")]
        public DateTime? InsertDate { get; set; }
        [Column("Update_Date", TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        [Column("Delete_Date", TypeName = "datetime")]
        public DateTime? DeleteDate { get; set; }
        public bool? FlagDelete { get; set; }
        [Column("Bill_Header")]
        [StringLength(50)]
        public string? BillHeader { get; set; }
        [Column("Bill_Footer")]
        [StringLength(50)]
        public string? BillFooter { get; set; }
        [Column("Account_Cash")]
        public long? AccountCash { get; set; }
        [Column("Account_Chequ")]
        public long? AccountChequ { get; set; }
        [Column("Account_Customer")]
        public long? AccountCustomer { get; set; }
        [Column("Account_Investor")]
        public long? AccountInvestor { get; set; }
        [Column("Account_Supplier")]
        public long? AccountSupplier { get; set; }
        [Column("Account_Chequ_under_collection")]
        public long? AccountChequUnderCollection { get; set; }
        [Column("Account_Cost_sales")]
        public long? AccountCostSales { get; set; }
        [Column("Account_Sales_profits")]
        public long? AccountSalesProfits { get; set; }
        [Column("Account_Installments_under_collection")]
        public long? AccountInstallmentsUnderCollection { get; set; }
        [Column("Account_Investor_profits")]
        public long? AccountInvestorProfits { get; set; }
        [Column("Account_Sales")]
        public long? AccountSales { get; set; }
        [Column("Account_Fee_Manage")]
        public long? AccountFeeManage { get; set; }
        [Column("Account_Profit_Installment")]
        public long? AccountProfitInstallment { get; set; }
        [Column("Account_Profit_Installment_Deferred")]
        public long? AccountProfitInstallmentDeferred { get; set; }
        [Column("Account_merchandise_Inventory")]
        public long? AccountMerchandiseInventory { get; set; }
        [Column("Account_Cost_Goods_Sold")]
        public long? AccountCostGoodsSold { get; set; }
        [Column("Account_Cash_sales")]
        public long? AccountCashSales { get; set; }
        [Column("Account_Receivables_Sales")]
        public long? AccountReceivablesSales { get; set; }
        [Column("Account_Inventory_Transit")]
        public long? AccountInventoryTransit { get; set; }
        [Column("Account_Branches")]
        public long? AccountBranches { get; set; }
        [Column("Account_NameCash")]
        [StringLength(255)]
        public string? AccountNameCash { get; set; }
        [Column("Account_CodeCash")]
        [StringLength(50)]
        public string? AccountCodeCash { get; set; }
        [Column("Account_NameChequ")]
        [StringLength(255)]
        public string? AccountNameChequ { get; set; }
        [Column("Account_CodeChequ")]
        [StringLength(50)]
        public string? AccountCodeChequ { get; set; }
        [Column("Account_NameCustomer")]
        [StringLength(255)]
        public string? AccountNameCustomer { get; set; }
        [Column("Account_CodeCustomer")]
        [StringLength(50)]
        public string? AccountCodeCustomer { get; set; }
        [Column("Account_NameInvestor")]
        [StringLength(255)]
        public string? AccountNameInvestor { get; set; }
        [Column("Account_CodeInvestor")]
        [StringLength(50)]
        public string? AccountCodeInvestor { get; set; }
        [Column("Account_NameSupplier")]
        [StringLength(255)]
        public string? AccountNameSupplier { get; set; }
        [Column("Account_CodeSupplier")]
        [StringLength(50)]
        public string? AccountCodeSupplier { get; set; }
        [Column("account_NameChequ_under_collection")]
        [StringLength(255)]
        public string? AccountNameChequUnderCollection { get; set; }
        [Column("account_codeChequ_under_collection")]
        [StringLength(50)]
        public string? AccountCodeChequUnderCollection { get; set; }
        [Column("Account_NameSales")]
        [StringLength(255)]
        public string? AccountNameSales { get; set; }
        [Column("Account_CodeSales")]
        [StringLength(50)]
        public string? AccountCodeSales { get; set; }
        [Column("account_nameFee_Manage")]
        [StringLength(255)]
        public string? AccountNameFeeManage { get; set; }
        [Column("account_codeFee_Manage")]
        [StringLength(50)]
        public string? AccountCodeFeeManage { get; set; }
        [Column("account_nameProfit_Installment")]
        [StringLength(255)]
        public string? AccountNameProfitInstallment { get; set; }
        [Column("account_codeProfit_Installment")]
        [StringLength(50)]
        public string? AccountCodeProfitInstallment { get; set; }
        [Column("account_nameProfit_Installment_Deferred")]
        [StringLength(255)]
        public string? AccountNameProfitInstallmentDeferred { get; set; }
        [Column("account_CodeProfit_Installment_Deferred")]
        [StringLength(50)]
        public string? AccountCodeProfitInstallmentDeferred { get; set; }
        [Column("account_Namemerchandise_Inventory")]
        [StringLength(255)]
        public string? AccountNamemerchandiseInventory { get; set; }
        [Column("account_codemerchandise_Inventory")]
        [StringLength(50)]
        public string? AccountCodemerchandiseInventory { get; set; }
        [Column("account_nameCost_Goods_Sold")]
        [StringLength(255)]
        public string? AccountNameCostGoodsSold { get; set; }
        [Column("account_codeCost_Goods_Sold")]
        [StringLength(50)]
        public string? AccountCodeCostGoodsSold { get; set; }
        [Column("account_nameCash_sales")]
        [StringLength(255)]
        public string? AccountNameCashSales { get; set; }
        [Column("account_codeCash_sales")]
        [StringLength(50)]
        public string? AccountCodeCashSales { get; set; }
        [Column("account_nameReceivables_Sales")]
        [StringLength(255)]
        public string? AccountNameReceivablesSales { get; set; }
        [Column("account_codeReceivables_Sales")]
        [StringLength(50)]
        public string? AccountCodeReceivablesSales { get; set; }
        [Column("account_nameInventory_Transit")]
        [StringLength(255)]
        public string? AccountNameInventoryTransit { get; set; }
        [Column("account_codeInventory_Transit")]
        [StringLength(50)]
        public string? AccountCodeInventoryTransit { get; set; }
        [Column("account_nameBranches")]
        [StringLength(255)]
        public string? AccountNameBranches { get; set; }
        [Column("account_codeBranches")]
        [StringLength(50)]
        public string? AccountCodeBranches { get; set; }
        [Column("account_nameCost_sales")]
        [StringLength(255)]
        public string? AccountNameCostSales { get; set; }
        [Column("account_codeCost_sales")]
        [StringLength(50)]
        public string? AccountCodeCostSales { get; set; }
        [Column("account_nameSales_profits")]
        [StringLength(255)]
        public string? AccountNameSalesProfits { get; set; }
        [Column("account_codeSales_profits")]
        [StringLength(50)]
        public string? AccountCodeSalesProfits { get; set; }
        [Column("account_nameInstallments_under_collection")]
        [StringLength(255)]
        public string? AccountNameInstallmentsUnderCollection { get; set; }
        [Column("account_codeInstallments_under_collection")]
        [StringLength(50)]
        public string? AccountCodeInstallmentsUnderCollection { get; set; }
        [Column("account_nameInvestor_profits")]
        [StringLength(255)]
        public string? AccountNameInvestorProfits { get; set; }
        [Column("account_codeInvestor_profits")]
        [StringLength(50)]
        public string? AccountCodeInvestorProfits { get; set; }
        [Column("Group_Incame_Name")]
        [StringLength(50)]
        public string? GroupIncameName { get; set; }
        [Column("Group_Incame_Code")]
        [StringLength(50)]
        public string? GroupIncameCode { get; set; }
        [Column("Group_Expenses_Name")]
        [StringLength(50)]
        public string? GroupExpensesName { get; set; }
        [Column("Group_Expenses_Code")]
        [StringLength(50)]
        public string? GroupExpensesCode { get; set; }
        [Column("Group_Assets_Code")]
        [StringLength(50)]
        public string? GroupAssetsCode { get; set; }
        [Column("Group_Assets_Name")]
        [StringLength(50)]
        public string? GroupAssetsName { get; set; }
        [Column("Group_Liabilities_Name")]
        [StringLength(50)]
        public string? GroupLiabilitiesName { get; set; }
        [Column("Group_Liabilities_Code")]
        [StringLength(50)]
        public string? GroupLiabilitiesCode { get; set; }
        [Column("Group_Copyrights_Name")]
        [StringLength(50)]
        public string? GroupCopyrightsName { get; set; }
        [Column("Group_Copyrights_Code")]
        [StringLength(50)]
        public string? GroupCopyrightsCode { get; set; }
        [Column("Account_Contractors_Name")]
        [StringLength(255)]
        public string? AccountContractorsName { get; set; }
        [Column("Account_Contractors_Code")]
        [StringLength(50)]
        public string? AccountContractorsCode { get; set; }
        [Column("CostCenter_Code")]
        [StringLength(50)]
        public string? CostCenterCode { get; set; }
        [Column("CostCenter_Name")]
        [StringLength(150)]
        public string? CostCenterName { get; set; }
        [Column("Group_Incame")]
        public long? GroupIncame { get; set; }
        [Column("Group_Expenses")]
        public long? GroupExpenses { get; set; }
        [Column("Group_Assets")]
        public long? GroupAssets { get; set; }
        [Column("Group_Liabilities")]
        public long? GroupLiabilities { get; set; }
        [Column("Group_Copyrights")]
        public long? GroupCopyrights { get; set; }
        [Column("Account_Contractors")]
        public long? AccountContractors { get; set; }
        [Column("CC_ID_Projects")]
        public long? CcIdProjects { get; set; }
        [Column("Lnk_Inovice_Inventory")]
        public int? LnkInoviceInventory { get; set; }
        [Column("Lnk_Bill_Inventroy")]
        public int? LnkBillInventroy { get; set; }
        [Column("Account_Discount_Name")]
        [StringLength(255)]
        public string? AccountDiscountName { get; set; }
        [Column("Account_Discount_Code")]
        [StringLength(50)]
        public string? AccountDiscountCode { get; set; }
        [Column("Discount_Account_ID")]
        public long? DiscountAccountId { get; set; }
        [Column("Discount_Credit_Account_ID")]
        public long? DiscountCreditAccountId { get; set; }
        [Column("Account_Discount_Credit_Name")]
        [StringLength(255)]
        public string? AccountDiscountCreditName { get; set; }
        [Column("Account_Discount_Credit_Code")]
        [StringLength(50)]
        public string? AccountDiscountCreditCode { get; set; }
        [Column("Header_Name")]
        [StringLength(250)]
        public string? HeaderName { get; set; }
        [Column("Logo_Print")]
        [StringLength(250)]
        public string? LogoPrint { get; set; }
        [Column("Img_Footer")]
        [StringLength(500)]
        public string? ImgFooter { get; set; }
        [Column("ID_Number")]
        [StringLength(50)]
        public string? IdNumber { get; set; }
        [Column("No_Labour_Office_File")]
        [StringLength(50)]
        public string? NoLabourOfficeFile { get; set; }
        [Column("Commissioner_Name")]
        [StringLength(50)]
        public string? CommissionerName { get; set; }
        public int? Posting { get; set; }
        [Column("Lnk_ReInovice_Inventory")]
        public int? LnkReInoviceInventory { get; set; }
        [Column("Lnk_ReBill_Inventroy")]
        public int? LnkReBillInventroy { get; set; }
        [Column("Lnk_Transfers_Inventory")]
        public int? LnkTransfersInventory { get; set; }
        public string? Stamp { get; set; }
        public int? SalesAccountType { get; set; }
        [Column("Profit_and_Loss_Account_ID")]
        public long? ProfitAndLossAccountId { get; set; }
        [Column("Profit_and_Loss_Account_Code")]
        [StringLength(50)]
        public string? ProfitAndLossAccountCode { get; set; }
        [Column("Profit_and_Loss_Account_Name")]
        [StringLength(255)]
        public string? ProfitAndLossAccountName { get; set; }
        [Column("Using_Purchase_Account")]
        public bool? UsingPurchaseAccount { get; set; }
        [Column("Purchase_Account_ID")]
        public long? PurchaseAccountId { get; set; }
        [Column("Purchase_Account_Code")]
        [StringLength(50)]
        public string? PurchaseAccountCode { get; set; }
        [Column("Purchase_Account_Name")]
        [StringLength(255)]
        public string? PurchaseAccountName { get; set; }
        [Column("VAT_Enable")]
        public bool? VatEnable { get; set; }
        [Column("VAT_Number")]
        [StringLength(250)]
        public string? VatNumber { get; set; }
        [Column("Sales_VAT_Account_ID")]
        public long? SalesVatAccountId { get; set; }
        [Column("Sales_VAT_Account_Code")]
        [StringLength(50)]
        public string? SalesVatAccountCode { get; set; }
        [Column("Sales_VAT_Account_Name")]
        [StringLength(255)]
        public string? SalesVatAccountName { get; set; }
        [Column("Purchases_VAT_Account_ID")]
        public long? PurchasesVatAccountId { get; set; }
        [Column("Purchases_VAT_Account_Code")]
        [StringLength(50)]
        public string? PurchasesVatAccountCode { get; set; }
        [Column("Purchases_VAT_Account_Name")]
        [StringLength(255)]
        public string? PurchasesVatAccountName { get; set; }
        [Column("Account_Members")]
        public long? AccountMembers { get; set; }
        [Column("Member_Account_Name")]
        [StringLength(255)]
        public string? MemberAccountName { get; set; }
        [Column("Member_Account_Name2")]
        [StringLength(255)]
        public string? MemberAccountName2 { get; set; }
        [Column("Member_Account_Code")]
        [StringLength(50)]
        public string? MemberAccountCode { get; set; }
    }
}
