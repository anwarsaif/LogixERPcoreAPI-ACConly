using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Logix.Domain.Base;
using Microsoft.EntityFrameworkCore;

namespace Logix.Domain.ACC
{
    [Table("ACC_Facilities")]
    public partial class AccFacility: TraceEntity
    {
        [Key]
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
        [Column("Lnk_Inovice_Inventory")]
        public int? LnkInoviceInventory { get; set; }
        [Column("Lnk_Bill_Inventroy")]
        public int? LnkBillInventroy { get; set; }
        [Column("CC_ID_Projects")]
        public long? CcIdProjects { get; set; }
        [Column("Discount_Account_ID")]
        public long? DiscountAccountId { get; set; }
        [Column("Discount_Credit_Account_ID")]
        public long? DiscountCreditAccountId { get; set; }
        [Column("Logo_Print")]
        [StringLength(250)]
        public string? LogoPrint { get; set; }
        [Column("Header_Name")]
        [StringLength(250)]
        public string? HeaderName { get; set; }
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
        [Column("CC_ID_Items")]
        public long? CcIdItems { get; set; }
        [Column("Lnk_Transfers_Inventory")]
        public int? LnkTransfersInventory { get; set; }
        public string? Stamp { get; set; }
        public int? SalesAccountType { get; set; }
        [Column("Separate_Account_Customer")]
        public bool? SeparateAccountCustomer { get; set; }
        [Column("Separate_Account_Supplier")]
        public bool? SeparateAccountSupplier { get; set; }
        [Column("Profit_and_Loss_Account_ID")]
        public long? ProfitAndLossAccountId { get; set; }
        [Column("Using_Purchase_Account")]
        public bool? UsingPurchaseAccount { get; set; }
        [Column("Purchase_Account_ID")]
        public long? PurchaseAccountId { get; set; }
        [Column("VAT_Enable")]
        public bool? VatEnable { get; set; }
        [Column("Sales_VAT_Account_ID")]
        public long? SalesVatAccountId { get; set; }
        [Column("Purchases_VAT_Account_ID")]
        public long? PurchasesVatAccountId { get; set; }
        [Column("VAT_Number")]
        [StringLength(250)]
        public string? VatNumber { get; set; }
        [Column("Account_Deferred_income")]
        public long? AccountDeferredIncome { get; set; }
        [Column("Separate_Account_Contractor")]
        public bool? SeparateAccountContractor { get; set; }
        [Column("Account_Members")]
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
}
