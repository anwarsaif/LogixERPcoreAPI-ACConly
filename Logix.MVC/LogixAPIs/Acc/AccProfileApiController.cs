using Logix.Application.Common;
using Logix.Application.DTOs.ACC;
using Logix.Application.DTOs.Main;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Services;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;
using Logix.Domain.Main;
using Logix.MVC.Helpers;

using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;
namespace Logix.MVC.LogixAPIs.Acc
{
    public class AccProfileApiController : BaseAccApiController
    {

        private readonly IAccServiceManager accServiceManager;
      
        private readonly IPermissionHelper permission;
        private readonly IWebHostEnvironment env;
        private readonly IFilesHelper filesHelper;
        private readonly IDDListHelper listHelper;
        private readonly ILocalizationService localization;
        private readonly ISysConfigurationHelper configurationHelper;
        private readonly ICurrentData _session;

        public AccProfileApiController(
            IAccServiceManager accServiceManager,
           
            IPermissionHelper permission,
             IWebHostEnvironment env,
            IHttpContextAccessor httpContextAccessor,
            IFilesHelper filesHelper,
            IDDListHelper listHelper,
             ILocalizationService localization
             , ISysConfigurationHelper configurationHelper
            , ICurrentData session
            )
        {
            this.accServiceManager = accServiceManager;
           
            this.permission = permission;
            this.env = env;
            this.filesHelper = filesHelper;
            this.listHelper = listHelper;
            this.localization = localization;
            this.configurationHelper = configurationHelper;
            this._session = session;
        }
        #region "transactions"

        [HttpGet("GetByIdForEdit")]
        public async Task<IActionResult> GetByIdForEdit()
        {
            try
            {
                var chk = await permission.HasPermission(66, PermissionType.Show);
                if (!chk)
                {
                    return Ok(await Result.AccessDenied("AccessDenied"));
                }
                long id = _session.FacilityId;
                if (id <= 0)
                {
                    return Ok(await Result<AccFacilityProfileDto>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var getItem = await accServiceManager.AccFacilityService.GetForUpdate<AccFacilityProfileDto>(id);
                if (getItem.Succeeded)
                {
                    var obj = new AccFacilityProfileDto();
                    obj = getItem.Data;
                    //===============================حساب الصندوق 
                    var AccountCash = await accServiceManager.AccAccountService.GetOne(x => x.AccAccountId == getItem.Data.AccountCash);
                    if (AccountCash.Data != null)
                    {  obj.AccountCodeCash = AccountCash.Data.AccAccountCode;
                        if (_session.Language == 1)
                        { obj.AccountNameCash = AccountCash.Data.AccAccountName;
                        }else{obj.AccountNameCash = AccountCash.Data.AccAccountName2;
                        }}
                    //===============================نهاية حساب الصندوق 
                    //===============================الحسابات البنكية  
                    var AccountChequ = await accServiceManager.AccAccountService.GetOne(x => x.AccAccountId == getItem.Data.AccountChequ);
                    if (AccountChequ.Data != null)
                    {
                        obj.AccountCodeChequ = AccountChequ.Data.AccAccountCode;
                        if (_session.Language == 1)
                        {
                            obj.AccountNameChequ = AccountChequ.Data.AccAccountName;
                        }
                        else
                        {
                            obj.AccountNameChequ = AccountChequ.Data.AccAccountName2;
                        }
                    }
                    //=============================== نهاية حسابات البنكية 

                    //===============================حساب الموردين 
                    var AccountSupplier = await accServiceManager.AccAccountService.GetOne(x => x.AccAccountId == getItem.Data.AccountSupplier);
                    if (AccountSupplier.Data != null)
                    {
                        obj.AccountCodeSupplier = AccountSupplier.Data.AccAccountCode;
                        if (_session.Language == 1)
                        {
                            obj.AccountNameSupplier = AccountSupplier.Data.AccAccountName;
                        }
                        else
                        {
                            obj.AccountNameSupplier = AccountSupplier.Data.AccAccountName2;
                        }
                    }
                    //===============================نهاية حساب الموردين 

                    //===============================حساب المقاولون
                    var AccountContractors = await accServiceManager.AccAccountService.GetOne(x => x.AccAccountId == getItem.Data.AccountContractors);
                    if (AccountContractors.Data != null)
                    {
                        obj.AccountCodeContractors = AccountContractors.Data.AccAccountCode;
                        if (_session.Language == 1)
                        {
                            obj.AccountNameContractors = AccountContractors.Data.AccAccountName;
                        }
                        else
                        {
                            obj.AccountNameContractors = AccountContractors.Data.AccAccountName2;
                        }
                    }
                    //===============================نهاية حساب المقاولون 
                    //===========================حساب شيكات تحت التحصيل
                    var AccountChequUnderCollection = await accServiceManager.AccAccountService.GetOne(x => x.AccAccountId == getItem.Data.AccountChequUnderCollection);
                    if (AccountChequUnderCollection.Data != null)
                    {
                        obj.AccountCodeChequundercollection = AccountChequUnderCollection.Data.AccAccountCode;
                        if (_session.Language == 1)
                        {
                            obj.AccountNameChequundercollection = AccountChequUnderCollection.Data.AccAccountName;
                        }
                        else
                        {
                            obj.AccountNameChequundercollection = AccountChequUnderCollection.Data.AccAccountName2;
                        }
                    }
                    //===============================حساب شيكات تحت التحصيل  


                    //===========================حساب تكلفة المبيعات 
                    var Costsales = await accServiceManager.AccAccountService.GetOne(x => x.AccAccountId == getItem.Data.AccountCostSales);
                    if (Costsales.Data != null)
                    {
                        obj.AccountCodeCostsales = Costsales.Data.AccAccountCode;
                        if (_session.Language == 1)
                        {
                            obj.AccountNameCostsales = Costsales.Data.AccAccountName;
                        }
                        else
                        {
                            obj.AccountNameCostsales = Costsales.Data.AccAccountName2;
                        }
                    }
                    //===============================حساب تكلفة المبيعات  
                    //===========================حساب ارباح المبيعات  
                    var Salesprofits = await accServiceManager.AccAccountService.GetOne(x => x.AccAccountId == getItem.Data.AccountSalesProfits);
                    if (Salesprofits.Data != null)
                    {
                        obj.AccountCodeSalesprofits = Salesprofits.Data.AccAccountCode;
                        if (_session.Language == 1)
                        {
                            obj.AccountNameSalesprofits = Salesprofits.Data.AccAccountName;
                        }
                        else
                        {
                            obj.AccountNameSalesprofits = Salesprofits.Data.AccAccountName2;
                        }
                    }
                    //===============================حساب ارباح المبيعات  
                    //===========================حساب المبيعات   
                    var AccountSales = await accServiceManager.AccAccountService.GetOne(x => x.AccAccountId == getItem.Data.AccountSales);
                    if (AccountSales.Data != null)
                    {
                        obj.AccountCodeSales = AccountSales.Data.AccAccountCode;
                        if (_session.Language == 1)
                        {
                            obj.AccountNameSales = AccountSales.Data.AccAccountName;
                        }
                        else
                        {
                            obj.AccountNameSales = AccountSales.Data.AccAccountName2;
                        }
                    }
                    //===============================حساب المبيعات   
                    //===========================بضاعة لدى حساب المخازن 

                    var AccountInventory = await accServiceManager.AccAccountService.GetOne(x => x.AccAccountId == getItem.Data.AccountMerchandiseInventory);
                    if (AccountInventory.Data != null)
                    {
                        obj.AccountcodemerchandiseInventory = AccountInventory.Data.AccAccountCode;
                        if (_session.Language == 1)
                        {
                            obj.AccountNamemerchandiseInventory = AccountInventory.Data.AccAccountName;
                        }
                        else
                        {
                            obj.AccountNamemerchandiseInventory = AccountInventory.Data.AccAccountName2;
                        }
                    }
                    //===============================بضاعة لدى حساب المخازن 
                    //===========================حساب تكلفة البضاعة المباعة  

                    var AccountCostGoodsSold = await accServiceManager.AccAccountService.GetOne(x => x.AccAccountId == getItem.Data.AccountCostGoodsSold);
                    if (AccountCostGoodsSold.Data != null)
                    {
                        obj.AccountcodeCostGoodsSold = AccountCostGoodsSold.Data.AccAccountCode;
                        if (_session.Language == 1)
                        {
                            obj.AcccountnameCostGoodsSold = AccountCostGoodsSold.Data.AccAccountName;
                        }
                        else
                        {
                            obj.AcccountnameCostGoodsSold = AccountCostGoodsSold.Data.AccAccountName2;
                        }
                    }
                    //===============================حساب تكلفة البضاعة المباعة
                    //===========================حساب المبيعات النقدية   

                    var AccountCashsales = await accServiceManager.AccAccountService.GetOne(x => x.AccAccountId == getItem.Data.AccountCashSales);
                    if (AccountCashsales.Data != null)
                    {
                        obj.AccountcodeCashsales = AccountCashsales.Data.AccAccountCode;
                        if (_session.Language == 1)
                        {
                            obj.AccountNameCashsales = AccountCashsales.Data.AccAccountName;
                        }
                        else
                        {
                            obj.AccountNameCashsales = AccountCashsales.Data.AccAccountName2;
                        }
                    }
                    //===============================حساب تكلفة البضاعة المباعة 

                    //===========================حساب العملاء(ذمم المبيعات)   

                    var ReceivablesSales = await accServiceManager.AccAccountService.GetOne(x => x.AccAccountId == getItem.Data.AccountReceivablesSales);
                    if (ReceivablesSales.Data != null)
                    {
                        obj.AccountcodeReceivablesSales = ReceivablesSales.Data.AccAccountCode;
                        if (_session.Language == 1)
                        {
                            obj.AccountnameReceivablesSales = ReceivablesSales.Data.AccAccountName;
                        }
                        else
                        {
                            obj.AccountnameReceivablesSales = ReceivablesSales.Data.AccAccountName2;
                        }
                    }
                    //===============================حساب العملاء(ذمم المبيعات) 

                    //===========================رقم حساب الاعضاء او المشتركين   

                    var AccountMembers = await accServiceManager.AccAccountService.GetOne(x => x.AccAccountId == getItem.Data.AccountMembers);
                    if (AccountMembers.Data != null)
                    {
                        obj.MemberAccountCode = AccountMembers.Data.AccAccountCode;
                        if (_session.Language == 1)
                        {
                            obj.MemberAccountName = AccountMembers.Data.AccAccountName;
                        }
                        else
                        {
                            obj.MemberAccountName = AccountMembers.Data.AccAccountName2;
                        }
                    }
                    //===============================رقم حساب الاعضاء او المشتركين

                    //===========================حساب المخزون المرحل    

                    var InventoryTransit = await accServiceManager.AccAccountService.GetOne(x => x.AccAccountId == getItem.Data.AccountInventoryTransit);
                    if (InventoryTransit.Data != null)
                    {
                        obj.AccountcodeInventoryTransit = InventoryTransit.Data.AccAccountCode;
                        if (_session.Language == 1)
                        {
                            obj.AccountnameInventoryTransit = InventoryTransit.Data.AccAccountName;
                        }
                        else
                        {
                            obj.AccountnameInventoryTransit = InventoryTransit.Data.AccAccountName2;
                        }
                    }
                    //===============================حساب المخزون المرحل 

                    //===========================حساب جاري الفروع     

                    var AccountBranches = await accServiceManager.AccAccountService.GetOne(x => x.AccAccountId == getItem.Data.AccountBranches);
                    if (AccountBranches.Data != null)
                    {
                        obj.AccountcodeBranches = AccountBranches.Data.AccAccountCode;
                        if (_session.Language == 1)
                        {
                            obj.AccountnameBranches = AccountBranches.Data.AccAccountName;
                        }
                        else
                        {
                            obj.AccountnameBranches = AccountBranches.Data.AccAccountName2;
                        }
                    }
                    //===============================حساب جاري الفروع 
                    //===========================مجموعة الاصول - قائمة المركز المالي     

                    var GroupAssets = await accServiceManager.AccAccountService.GetOne(x => x.AccAccountId == getItem.Data.GroupAssets);
                    if (GroupAssets.Data != null)
                    {
                        obj.GroupAssetsCode = GroupAssets.Data.AccAccountCode;
                        if (_session.Language == 1)
                        {
                            obj.GroupAssetsName = GroupAssets.Data.AccAccountName;
                        }
                        else
                        {
                            obj.GroupAssetsName = GroupAssets.Data.AccAccountName2;
                        }
                    }
                    //===============================مجموعة الاصول - قائمة المركز المالي 
                    //===========================مجموعة الخصوم / الالتزامات - قائمة المركز المالي     

                    var GroupLiabilities = await accServiceManager.AccAccountService.GetOne(x => x.AccAccountId == getItem.Data.GroupLiabilities);
                    if (GroupLiabilities.Data != null)
                    {
                        obj.GroupLiabilitiesCode = GroupLiabilities.Data.AccAccountCode;
                        if (_session.Language == 1)
                        {
                            obj.GroupLiabilitiesName = GroupLiabilities.Data.AccAccountName;
                        }
                        else
                        {
                            obj.GroupLiabilitiesName = GroupLiabilities.Data.AccAccountName2;
                        }
                    }
                    //===============================مجموعة الخصوم / الالتزامات - قائمة المركز المالي 
                    //===========================مجموعة حقوق الملكية - قائمة المركز المالي      

                    var GroupCopyrights = await accServiceManager.AccAccountService.GetOne(x => x.AccAccountId == getItem.Data.GroupCopyrights);
                    if (GroupCopyrights.Data != null)
                    {
                        obj.GroupCopyrightsCode = GroupCopyrights.Data.AccAccountCode;
                        if (_session.Language == 1)
                        {
                            obj.GroupCopyrightsName = GroupCopyrights.Data.AccAccountName;
                        }
                        else
                        {
                            obj.GroupCopyrightsName = GroupCopyrights.Data.AccAccountName2;
                        }
                    }
                    //===============================مجموعة حقوق الملكية - قائمة المركز المالي 
                    //===========================مجموعة الإيرادات - قائمة الدخل      

                    var GroupIncame = await accServiceManager.AccAccountService.GetOne(x => x.AccAccountId == getItem.Data.GroupIncame);
                    if (GroupIncame.Data != null)
                    {
                        obj.GroupIncameCode = GroupIncame.Data.AccAccountCode;
                        if (_session.Language == 1)
                        {
                            obj.GroupIncameName = GroupIncame.Data.AccAccountName;
                        }
                        else
                        {
                            obj.GroupIncameName = GroupIncame.Data.AccAccountName2;
                        }
                    }
                    //===============================مجموعة الإيرادات - قائمة الدخل 
                    //===========================مجموعة المصروفات - قائمة الدخل       

                    var GroupExpenses = await accServiceManager.AccAccountService.GetOne(x => x.AccAccountId == getItem.Data.GroupExpenses);
                    if (GroupExpenses.Data != null)
                    {
                        obj.GroupExpensesCode = GroupExpenses.Data.AccAccountCode;
                        if (_session.Language == 1)
                        {
                            obj.GroupExpensesName = GroupExpenses.Data.AccAccountName;
                        }
                        else
                        {
                            obj.GroupExpensesName = GroupExpenses.Data.AccAccountName2;
                        }
                    }
                    //===============================مجموعة المصروفات - قائمة الدخل 
                    //===========================مركز التكلفة المشاريع        

                    var CostCenter = await accServiceManager.AccAccountService.GetOne(x => x.AccAccountId == getItem.Data.CcIdProjects);
                    if (CostCenter.Data != null)
                    {
                        obj.CostCenterCode = CostCenter.Data.AccAccountCode;
                        if (_session.Language == 1)
                        {
                            obj.CostCenterName = CostCenter.Data.AccAccountName;
                        }
                        else
                        {
                            obj.CostCenterName = CostCenter.Data.AccAccountName2;
                        }
                    }
                    //===============================مركز التكلفة المشاريع  

                    //===========================حساب الخصم المسموح به        

                    var DiscountAccount = await accServiceManager.AccAccountService.GetOne(x => x.AccAccountId == getItem.Data.DiscountAccountId);
                    if (DiscountAccount.Data != null)
                    {
                        obj.AccountDiscountCode = DiscountAccount.Data.AccAccountCode;
                        if (_session.Language == 1)
                        {
                            obj.AccountDiscountName = DiscountAccount.Data.AccAccountName;
                        }
                        else
                        {
                            obj.AccountDiscountName = DiscountAccount.Data.AccAccountName2;
                        }
                    }
                    //===============================حساب الخصم المسموح به 
                    //===========================حساب الخصم المكتسب        

                    var DiscountCredit = await accServiceManager.AccAccountService.GetOne(x => x.AccAccountId == getItem.Data.DiscountCreditAccountId);
                    if (DiscountCredit.Data != null)
                    {
                        obj.AccountDiscountCreditCode = DiscountCredit.Data.AccAccountCode;
                        if (_session.Language == 1)
                        {
                            obj.AccountDiscountCreditName = DiscountCredit.Data.AccAccountName;
                        }
                        else
                        {
                            obj.AccountDiscountCreditName = DiscountCredit.Data.AccAccountName2;
                        }
                    }
                    //===============================حساب الخصم المكتسب
                    //===========================حساب الأرباح والخسائر


                    var profitandLossAccount = await accServiceManager.AccAccountService.GetOne(x => x.AccAccountId == getItem.Data.ProfitAndLossAccountId);
                    if (profitandLossAccount.Data != null)
                    {
                        obj.profitandLossAccountCode = profitandLossAccount.Data.AccAccountCode;
                        if (_session.Language == 1)
                        {
                            obj.profitandLossAccountName = profitandLossAccount.Data.AccAccountName;
                        }
                        else
                        {
                            obj.profitandLossAccountName = profitandLossAccount.Data.AccAccountName2;
                        }
                    }
                    //===============================حساب الأرباح والخسائر
                    //===========================توسيط حساب المشتريات
                    var PurchaseAccount = await accServiceManager.AccAccountService.GetOne(x => x.AccAccountId == getItem.Data.PurchaseAccountId);
                    if (PurchaseAccount.Data != null)
                    {
                        obj.PurchaseAccountCode = PurchaseAccount.Data.AccAccountCode;
                        if (_session.Language == 1)
                        {
                            obj.PurchaseAccountName = PurchaseAccount.Data.AccAccountName;
                        }
                        else
                        {
                            obj.PurchaseAccountName = PurchaseAccount.Data.AccAccountName2;
                        }
                    }
                    //===============================توسيط حساب المشتريات

                    //===========================حساب مبيعات ضريبة القيمة المضافة
                    var SalesVATAccount = await accServiceManager.AccAccountService.GetOne(x => x.AccAccountId == getItem.Data.SalesVatAccountId);
                    if (SalesVATAccount.Data != null)
                    {
                        obj.SalesVATAccountCode = SalesVATAccount.Data.AccAccountCode;
                        if (_session.Language == 1)
                        {
                            obj.SalesVATAccountName = SalesVATAccount.Data.AccAccountName;
                        }
                        else
                        {
                            obj.SalesVATAccountName = SalesVATAccount.Data.AccAccountName2;
                        }
                    }
                    //===============================حساب مبيعات ضريبة القيمة المضافة

                    //===========================حساب مشتريات ضريبة القيمة المضافة
                    var PurchasesVATAccount = await accServiceManager.AccAccountService.GetOne(x => x.AccAccountId == getItem.Data.PurchasesVatAccountId);
                    if (PurchasesVATAccount.Data != null)
                    {
                        obj.PurchasesVATAccountCode = PurchasesVATAccount.Data.AccAccountCode;
                        if (_session.Language == 1)
                        {
                            obj.PurchasesVATAccountName = PurchasesVATAccount.Data.AccAccountName;
                        }
                        else
                        {
                            obj.PurchasesVATAccountName = PurchasesVATAccount.Data.AccAccountName2;
                        }
                    }
                    //=================== حساب مشتريات ضريبة القيمة المضافة
                    return Ok(await Result<AccFacilityProfileDto>.SuccessAsync(obj, $""));
                }
                else
                {
                    return Ok(getItem);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccFacilityProfileDto>.FailAsync($"======= Exp in Acc Facility  edit: {ex.Message}"));
            }
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var chk = await permission.HasPermission(66, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var items = await accServiceManager.AccFacilityService.GetAllVW(x=>x.FlagDelete==false);
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    res = res.OrderBy(e => e.FacilityId);
                    return Ok(await Result<List<AccFacilitiesVw>>.SuccessAsync(res.ToList(), ""));

                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccFacilitiesVw>.FailAsync($"======= Exp in Acc Facility  edit: {ex.Message}"));

            }
        }

        #endregion "transactions"

        #region "transactions_Update"
        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(AccFacilityProfileDto obj)
        {
            var chk = await permission.HasPermission(66, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<AccFacilityProfileDto>.FailAsync(localization.GetMessagesResource("dataRequire")));
                }

                var Edit = await accServiceManager.AccFacilityService.UpdateProfileEdit(obj);
                return Ok(Edit);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccFacilityProfileDto>.FailAsync($"======= Exp in Acc Facility  edit: {ex.Message}"));
            }
        }

        #endregion "transactions_Update"
       
     

        #region "transactions_UpdateDateProfile"

        [HttpPost("EditDateProfile")]
        public async Task<IActionResult> EditDateProfile(long ID, int Value, long Number)
        {
            var chk = await permission.HasPermission(66, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<AccFacilityProfileDto>.FailAsync(localization.GetMessagesResource("dataRequire")));
                }
              

                var Edit = await accServiceManager.AccFacilityService.UpdateValue(ID, Value, Number);
                return Ok(Edit);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccFacilityProfileDto>.FailAsync($"======= Exp in Acc Profile  edit: {ex.Message}"));
            }
        }
        #endregion "transactions_UpdateDateProfile"



        

        

        

        

        



        #region "transactions_UpdatePurchaseAccount"

        [HttpPost("EditPurchaseAccount")]
        public async Task<IActionResult> EditPurchaseAccount(long ID, string AccountCode,bool UsingPurchaseAccount)
        {
            var chk = await permission.HasPermission(66, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<AccFacilityProfileDto>.FailAsync(localization.GetMessagesResource("dataRequire")));
                }
                long AccountId = 0;
                if (!string.IsNullOrEmpty(AccountCode))
                {
                    var Accountdata = await accServiceManager.AccAccountsSubHelpeVwService.GetOne(x => x.Isdel == false && x.FacilityId == _session.FacilityId && x.AccAccountCode == AccountCode && x.IsActive == true);

                    AccountId = Accountdata.Data.AccAccountId;
                }

                var Edit = await accServiceManager.AccFacilityService.UpdatePurchaseAccount(ID, AccountId, UsingPurchaseAccount);
                return Ok(Edit);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccFacilityProfileDto>.FailAsync($"======= Exp in Acc Profile  edit: {ex.Message}"));
            }
        }
        #endregion "transactions_UpdatePurchaseAccount"




        #region "transactions_UpdateVATAccount"

        [HttpPost("EditVATAccount")]
        
        public async Task<IActionResult> EditVATAccount(long ID, string AccountSalesCode, string AccountPurchasesCode,bool VATEnable)
        {
            var chk = await permission.HasPermission(66, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<AccFacilityProfileDto>.FailAsync(localization.GetMessagesResource("dataRequire")));
                }
                long AccountSalesId = 0;
                if (!string.IsNullOrEmpty(AccountSalesCode))
                {
                    var Accountdata = await accServiceManager.AccAccountsSubHelpeVwService.GetOne(x => x.Isdel == false && x.FacilityId == _session.FacilityId && x.AccAccountCode == AccountSalesCode && x.IsActive == true);

                    AccountSalesId = Accountdata.Data.AccAccountId;
                }
                //----------------------------------------
                long AccountPurchasesId = 0;
                if (!string.IsNullOrEmpty(AccountPurchasesCode))
                {
                    var Accountdata = await accServiceManager.AccAccountsSubHelpeVwService.GetOne(x => x.Isdel == false && x.FacilityId == _session.FacilityId && x.AccAccountCode == AccountPurchasesCode && x.IsActive == true);

                    AccountPurchasesId = Accountdata.Data.AccAccountId;
                }

                var Edit = await accServiceManager.AccFacilityService.UpdateValueVAT(ID, AccountSalesId, AccountPurchasesId,VATEnable);
                return Ok(Edit);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccFacilityProfileDto>.FailAsync($"======= Exp in Acc Profile  edit: {ex.Message}"));
            }
        }
        #endregion "transactions_UpdateVATAccount"



        [HttpPost("EditAccountCode")]
        public async Task<IActionResult> EditAccountCode(long ID, string AccountCode, long Number)
        {
            var chk = await permission.HasPermission(66, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<AccFacilityProfileDto>.FailAsync(localization.GetMessagesResource("dataRequire")));
                }
                long AccountId = 0;
                if (!string.IsNullOrEmpty(AccountCode))
                {
                    var Accountdata = await accServiceManager.AccAccountsSubHelpeVwService.GetOne(x => x.Isdel == false && x.FacilityId == _session.FacilityId && x.AccAccountCode == AccountCode && x.IsActive == true);

                    AccountId = Accountdata.Data.AccAccountId;
                }

                var Edit = await accServiceManager.AccFacilityService.UpdateValue(ID, AccountId, Number);
                return Ok(Edit);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccFacilityProfileDto>.FailAsync($"======= Exp in Acc Profile  edit: {ex.Message}"));
            }
        }


        #region "UpdateLogo"

        [HttpPost("UpdateLogo")]
        public async Task<IActionResult> UpdateLogo(long ID, string FacilityLogo,long TypeId)
        {
            var chk = await permission.HasPermission(66, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<AccFacilityProfileDto>.FailAsync(localization.GetMessagesResource("dataRequire")));
                }


                var Edit = await accServiceManager.AccFacilityService.UpdateLogo(ID, FacilityLogo, TypeId);
                return Ok(Edit);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccFacilityProfileDto>.FailAsync($"======= Exp in Acc Profile  edit: {ex.Message}"));
            }
        }

        #endregion "UpdateLogo"




    }
}