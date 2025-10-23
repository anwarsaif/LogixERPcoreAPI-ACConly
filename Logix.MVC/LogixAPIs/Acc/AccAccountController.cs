using Logix.Application.Common;
using Logix.Application.DTOs.ACC;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Logix.MVC.LogixAPIs.Acc
{


    public class AccAccountController : BaseAccApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IAccServiceManager accServiceManager;
        private readonly IPermissionHelper permission;
        private readonly IWebHostEnvironment env;
        private readonly IFilesHelper filesHelper;
        private readonly IDDListHelper listHelper;
        private readonly ILocalizationService localization;
        private readonly ISysConfigurationHelper configurationHelper;
        private readonly ICurrentData _currentData;
        private readonly IApiDDLHelper ddlHelper;

        public AccAccountController(
            IMainServiceManager mainServiceManager,
            IAccServiceManager accServiceManager,
            IPermissionHelper permission,
             IWebHostEnvironment env,
            IHttpContextAccessor httpContextAccessor,
            IFilesHelper filesHelper,
            IDDListHelper listHelper,
             ILocalizationService localization
             , ISysConfigurationHelper configurationHelper
            , ICurrentData currentData,
            IApiDDLHelper ddlHelper
            )
        {
            this.mainServiceManager = mainServiceManager;
            this.accServiceManager = accServiceManager;
            this.permission = permission;
            this.env = env;
            this.filesHelper = filesHelper;
            this.listHelper = listHelper;
            this.localization = localization;
            this.configurationHelper = configurationHelper;
            this._currentData = currentData;
            this.ddlHelper = ddlHelper;
        }
        #region "transactions"


        [HttpPost("Search")]
        public async Task<IActionResult> Search(AccAccountFilterDto filter)
        {
            var chk = await permission.HasPermission(357, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {

                var items = await accServiceManager.AccAccountService.Search(filter);
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable().DistinctBy(x => x.AccAccountCode);
                    var final = res.ToList();
                    return Ok(await Result<List<AccAccountsVw>>.SuccessAsync(final, ""));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccAccountsVw>.FailAsync($"======= Exp in Search AccAccountsVw, MESSAGE: {ex.Message}"));
            }
        }
        #endregion "transactions"

        #region "transactions_ADD"

        [HttpPost("Add")]
        public async Task<IActionResult> Add(AccAccountDto obj)
        {
            var chk = await permission.HasPermission(357, PermissionType.Add);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<AccAccountAddDto>.FailAsync(localization.GetMessagesResource("dataRequire")));
                }
                var add = await accServiceManager.AccAccountService.Add(obj);

                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccAccountDto>.FailAsync($"======= Exp in Acc Account  add: {ex.Message}"));
            }
        }

        #endregion "transactions_Add"

        #region "transactions_Update"
        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(AccAccountEditDto obj)
        {
            var chk = await permission.HasPermission(357, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<AccAccountEditDto>.FailAsync(localization.GetMessagesResource("dataRequire")));
                }
                var Edit = await accServiceManager.AccAccountService.Update(obj);


                return Ok(Edit);



            }
            catch (Exception ex)
            {
                return Ok(await Result<AccAccountEditDto>.FailAsync($"======= Exp in Acc Account edit: {ex.Message}"));
            }
        }

        #endregion "transactions_Update"

        #region "transactions_Delete"
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long Id)
        {
            var chk = await permission.HasPermission(357, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var chjd = await accServiceManager.AccJournalDetaileService.GetAll(x => x.AccAccountId == Id);
                if (chjd.Succeeded && chjd.Data.Count() > 0)
                {
                    return Ok(await Result<AccAccountDto>.FailAsync(localization.GetAccResource("chkAccountJournal")));


                }

                var ch = await accServiceManager.AccAccountService.GetAll(x => x.SystemId == 2 && x.IsDeleted == false && x.AccAccountParentId == Id && x.AccAccountId != x.AccAccountParentId);
                if (ch.Succeeded && ch.Data.Count() > 0)
                {
                    return Ok(await Result<AccAccountDto>.FailAsync(localization.GetAccResource("chkAccountParentId")));


                }

                var delete = await accServiceManager.AccAccountService.Remove(Id); ;
                return Ok(delete);

            }
            catch (Exception ex)
            {
                return Ok(await Result<AccAccountDto>.FailAsync($"======= Exp in Acc Account  Delete: {ex.Message}"));
            }
        }
        #endregion "transactions_Delete"

        #region "transactions_GetById"

        [HttpGet("GetByIdForEdit")]
        public async Task<IActionResult> GetByIdForEdit(long id)
        {
            try
            {
                var chk = await permission.HasPermission(357, PermissionType.Show);
                if (!chk)
                {
                    return Ok(await Result.AccessDenied("AccessDenied"));
                }

                if (id <= 0)
                {
                    return Ok(await Result<AccAccountEditDto>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var getItem = await accServiceManager.AccAccountService.GetForUpdate<AccAccountEditDto>(id);
                if (getItem.Succeeded)
                {
                    var obj = getItem.Data;



                    return Ok(await Result<AccAccountEditDto>.SuccessAsync(obj, $""));
                }
                else
                {
                    return Ok(getItem);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccAccountEditDto>.FailAsync($"====== Exp in GetByIdForEdit Acc Account, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(long id)
        {
            try
            {
                var chk = await permission.HasPermission(357, PermissionType.Show);
                if (!chk)
                {
                    return Ok(await Result.AccessDenied("AcessDenied"));
                }

                if (id <= 0)
                {
                    return Ok(await Result<AccAccountDto>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var getItem = await accServiceManager.AccAccountService.GetOne(s => s.AccAccountId == id && s.IsDeleted == false);
                if (getItem.Succeeded)
                {
                    var obj = getItem.Data;
                    return Ok(await Result<AccAccountDto>.SuccessAsync(obj, $""));
                }
                else
                {
                    return Ok(getItem);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccAccountDto>.FailAsync($"====== Exp in GetById Acc Account, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetAccountLevelValue")]
        public async Task<IActionResult> GetAccountLevelValue(long id)
        {
            var data = await accServiceManager.AccAccountService.GetOne(s => s.AccountLevel, x => x.AccAccountId == id && x.FacilityId == _currentData.FacilityId);
            var dataTemp = data.Data ?? 0;

            return Ok(await Result<decimal>.SuccessAsync(dataTemp + 1));
        }




        [HttpGet("DDLAccountParentByGroupId")]
        public async Task<IActionResult> DDLAccountParentByGroupId(long Id)
        {
            var lang = _currentData.Language;

            try
            {
                var items = await accServiceManager.AccAccountService.GetAllVW(d => d.FlagDelete == false && d.AccGroupId == Id && d.IsSub == true && d.FacilityId == _currentData.FacilityId);
                var itemsList = items.Data.ToList();
                var hierarchicalList = BindAccountTree(itemsList, lang);

                var ddReferenceTypeList = listHelper.GetFromList<long>(hierarchicalList.Select(s => new DDListItem<long>
                {
                    Name = s.Name,
                    Value = s.Value
                }), hasDefault: false, defaultText: localization.GetResource1("Choose"));

                return Ok(await Result<SelectList>.SuccessAsync(ddReferenceTypeList));

            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }


        [NonAction]
        private List<DDListItem<long>> BindAccountTree(List<AccAccountsVw> items, int languageId)
        {
            List<DDListItem<long>> selectList = new List<DDListItem<long>>();

            foreach (var item in items.Where(i => i.AccAccountParentId == i.AccAccountId))
            {
                selectList.Add(new DDListItem<long> { Value = item.AccAccountId, Name = $"{item.AccAccountCode}-{GetAccountName(item, languageId)}" });

                AddChildren(items, selectList, item, languageId, 1);
            }

            return selectList;
        }
        [NonAction]

        private void AddChildren(List<AccAccountsVw> items, List<DDListItem<long>> selectList, AccAccountsVw parent, int languageId, int level)
        {
            foreach (var child in items.Where(i => i.AccAccountParentId == parent.AccAccountId && i.AccAccountId != i.AccAccountParentId))
            {
                string prefix = new string('-', level * 2);
                selectList.Add(new DDListItem<long> { Value = child.AccAccountId, Name = $"{prefix} {child.AccAccountCode}-{GetAccountName(child, languageId)}" });

                // Recursively add grandchildren
                AddGrandChildren(items, selectList, child, languageId, level + 1);
            }
        }
        [NonAction]

        private void AddGrandChildren(List<AccAccountsVw> items, List<DDListItem<long>> selectList, AccAccountsVw parent, int languageId, int level)
        {
            foreach (var grandChild in items.Where(i => i.AccAccountParentId == parent.AccAccountId && i.AccAccountId != i.AccAccountParentId))
            {
                string prefix = new string('-', level * 2);
                selectList.Add(new DDListItem<long> { Value = grandChild.AccAccountId, Name = $"{prefix} {grandChild.AccAccountCode}-{GetAccountName(grandChild, languageId)}" });

                // Recursively add great-grandchildren
                AddGrandChildren(items, selectList, grandChild, languageId, level + 1);
            }
        }
        [NonAction]

        private string GetAccountName(AccAccountsVw account, int languageId)
        {
            if (account.AccAccountId == account.AccAccountParentId)
            {
                return languageId == 1 ? (account.AccAccountName ?? "") : account.AccAccountName2;
            }
            else
            {
                return languageId == 1 ? ((account.AccAccountName ?? "")) : (" - " + account.AccAccountName2);
            }
        }


        [HttpGet("GetByCode")]
        public async Task<IActionResult> GetByCode(string Code)
        {
            try
            {


                if (string.IsNullOrEmpty(Code))
                {
                    return Ok(await Result<AccAccountsVw>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var getItem = await accServiceManager.AccAccountService.GetOneVW(s => s.AccAccountCode.Equals(Code) && s.FlagDelete == false);
                if (getItem.Succeeded)
                {
                    var obj = getItem.Data;
                    return Ok(await Result<AccAccountsVw>.SuccessAsync(obj, $""));
                }
                else
                {
                    return Ok(getItem);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccAccountDto>.FailAsync($"====== Exp in GetByCode Acc Account, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetCurrencyID")]
        public async Task<IActionResult> GetCurrencyID(long id)
        {
            var data = await accServiceManager.AccAccountService.GetOne(s => s.AccountLevel, x => x.AccAccountId == id && x.FacilityId == _currentData.FacilityId && x.SystemId == 2);
            var dataTemp = data.Data ?? 0;

            return Ok(await Result<decimal>.SuccessAsync(dataTemp + 1));
        }
        #endregion "transactions_GetById"


        #region "transactions_Excel"



        [HttpPost("ShowUploadAccountsExcel")]
        public async Task<IActionResult> ShowUploadAccountsExcel(List<AccAccountExcelDto> accounts)
        {
            string MsgErrorCount = null;
            string MsgErrorAccountLevel = null;
            string MsgErrorsubAccountLevel = null;
            string MsgErrors = null;
            string MsgErrorsCurrency = null;
            string MsgErrorsubAccountLevellarger = null;
            string MsgErrorsCostCenter = null;
            string MsgErrorCircularReference = null;

            try
            {
                Dictionary<string, AccAccountResultExcelDto> Subitems = new();
                HashSet<string> usedAsChild = new();

                string subAccountLevelStr = await configurationHelper.GetValue(99, _currentData.FacilityId);
                int subAccountLevel = 0;
                if (!int.TryParse(subAccountLevelStr, out subAccountLevel))
                {
                    MsgErrorsubAccountLevel = "Configuration key 99 is missing or invalid.";
                }

                foreach (var account in accounts)
                {
                    bool AddCount = true;
                    var accAccountCode = account.AccAccountCode;
                    var AccountLevel = account.AccountLevel ?? 0;
                    var Currencytemp = account.CurrencyId ?? 0;
                    var CostCenterCode = account.CostCenterCode;
                    var IsSub = account.IsSub;
                    var IsHelpAccount = account.IsHelpAccount;

                    // تحقق من تكرار رقم الحساب
                    if (!string.IsNullOrEmpty(accAccountCode))
                    {
                        var exists = await accServiceManager.AccAccountService
                            .GetAll(x => x.AccAccountCode == accAccountCode && x.IsDeleted == false && x.FacilityId == _currentData.FacilityId && x.SystemId == 2);
                        if (exists.Data.Any())
                        {
                            MsgErrorCount += accAccountCode + ",";
                            AddCount = false;
                        }
                    }

                    // تحقق من صلاحية العملة
                    if (Currencytemp != 0)
                    {
                        var currency = await mainServiceManager.SysCurrencyService
                            .GetAll(x => x.Id == Currencytemp && x.IsDeleted == false);
                        if (!currency.Data.Any())
                        {
                            MsgErrorsCurrency += accAccountCode + ",";
                            AddCount = false;
                        }
                    }

                    // تحقق من مركز التكلفة
                    if (!string.IsNullOrEmpty(CostCenterCode))
                    {
                        var costCenter = await accServiceManager.AccCostCenterService
                            .GetAll(x => x.CostCenterCode == CostCenterCode && x.IsDeleted == false);
                        if (!costCenter.Data.Any())
                        {
                            MsgErrorsCostCenter += accAccountCode + ",";
                            AddCount = false;
                        }
                    }

                    // تحقق من مستوى الحسابات
                    if (subAccountLevel > 0)
                    {
                        if (AccountLevel >= subAccountLevel)
                        {
                            MsgErrors += accAccountCode + ",";
                            AddCount = false;
                        }

                        if (IsSub == false && subAccountLevel != AccountLevel + 1)
                        {
                            MsgErrorsubAccountLevellarger += accAccountCode + ",";
                            AddCount = false;
                        }
                    }

                    // سجل أن الحساب هو ابن
                    if (!string.IsNullOrEmpty(account.AccAccountParentCode))
                    {
                        usedAsChild.Add(accAccountCode);
                    }

                    // إضافة العنصر إن كان صالحًا
                    if (AddCount && !string.IsNullOrEmpty(accAccountCode))
                    {
                        var currencyName = (await mainServiceManager.SysCurrencyService
                            .GetOne(s => s.Name, x => x.Id == Currencytemp && x.IsDeleted == false))?.Data;

                        var costCenterName = (await accServiceManager.AccCostCenterService
                            .GetOne(s => s.CostCenterName, x => x.CostCenterCode == CostCenterCode && x.IsDeleted == false))?.Data;

                        Subitems[accAccountCode] = new AccAccountResultExcelDto
                        {
                            AccAccountCode = accAccountCode,
                            AccAccountName = account.AccAccountName,
                            AccAccountName2 = account.AccAccountName2,
                            AccGroupId = account.AccGroupId,
                            AccAccountnameParent = account.AccAccountnameParent,
                            AccAccountParentCode = account.AccAccountParentCode,
                            AccountLevel = AccountLevel,
                            CurrencyId = Currencytemp,
                            CurrencyName = currencyName,
                            CostCenterCode = CostCenterCode,
                            CostCenterName = costCenterName,
                            IsSub = IsSub ?? false,
                            IsHelpAccount = IsHelpAccount ?? false,
                            Children = new()
                        };
                    }
                }

                // بناء شجرة الحسابات
                foreach (var item in Subitems.Values.ToList())
                {
                    if (item.AccAccountCode == item.AccAccountParentCode)
                        continue;

                    if (!string.IsNullOrEmpty(item.AccAccountParentCode) && Subitems.TryGetValue(item.AccAccountParentCode, out var parent))
                    {
                        if (item.AccountLevel < parent.AccountLevel)
                        {
                            MsgErrorAccountLevel += item.AccAccountCode + ",";
                            continue;
                        }

                        // السماح بأن يكون الأب ابناً أيضًا ولكن تجنب التكرار الدائري
                        if (item.AccAccountCode == parent.AccAccountCode)
                        {
                            MsgErrorCircularReference += $"({item.AccAccountCode}->{item.AccAccountParentCode}), ";
                            continue;
                        }

                        parent.Children.Add(item);
                    }
                }

                // استخراج العناصر الجذرية فقط
                var childCodes = Subitems.Values.SelectMany(x => x.Children).Select(x => x.AccAccountCode).ToHashSet();
                var rootItems = Subitems.Values.Where(x => !childCodes.Contains(x.AccAccountCode)).ToList();

                // عرض الأخطاء
                string finalMessage = null;

                if (!string.IsNullOrEmpty(MsgErrorsCurrency))
                    finalMessage += localization.GetMessagesResource("DepartmentNumberIsNot") + $" ({MsgErrorsCurrency.TrimEnd(',')}) | ";
                if (!string.IsNullOrEmpty(MsgErrorCount))
                    finalMessage += localization.GetMessagesResource("AccountNumberAlready") + $" ({MsgErrorCount.TrimEnd(',')}) | ";
                if (!string.IsNullOrEmpty(MsgErrorsubAccountLevel))
                    finalMessage += localization.GetMessagesResource("levelConfiguration") + $" ({MsgErrorsubAccountLevel.TrimEnd(',')}) | ";
                if (!string.IsNullOrEmpty(MsgErrorAccountLevel))
                    finalMessage += localization.GetMessagesResource("Accountlargerlevel") + $" ({MsgErrorAccountLevel.TrimEnd(',')}) | ";
                if (!string.IsNullOrEmpty(MsgErrorsubAccountLevellarger))
                    finalMessage += localization.GetMessagesResource("subAccountLevel") + $" ({MsgErrorsubAccountLevellarger.TrimEnd(',')}) | ";
                if (!string.IsNullOrEmpty(MsgErrorsCostCenter))
                    finalMessage += localization.GetMessagesResource("CostCenterNumberIsNot") + $" ({MsgErrorsCostCenter.TrimEnd(',')}) | ";
                if (!string.IsNullOrEmpty(MsgErrors))
                    finalMessage += localization.GetMessagesResource("Accountlargerlevel") + $" ({MsgErrors.TrimEnd(',')}) | ";
                if (!string.IsNullOrEmpty(MsgErrorCircularReference))
                    finalMessage += "لا يمكن أن يكون الحساب أباً وابناً في نفس الوقت: " + MsgErrorCircularReference.TrimEnd(',', ' ') + " | ";

                // إرجاع الخطأ إن وُجد
                if (!string.IsNullOrEmpty(finalMessage))
                    return Ok(await Result<List<AccAccountResultExcelDto>>.FailAsync(finalMessage.TrimEnd(' ', '|')));

                return Ok(await Result<List<AccAccountResultExcelDto>>.SuccessAsync(rootItems));
            }
            catch (Exception ex)
            {
                return Ok(await Result<List<AccAccountResultExcelDto>>.FailAsync($"EX: {ex.Message}"));
            }
        }





        [HttpPost("SaveAccountsExcel")]
        public async Task<IActionResult> SaveAccountsExcel(List<AccAccountResultExcelDto> accounts)
        {
            try
            {
                if (accounts == null || !accounts.Any())
                {

                    return Ok(Result<List<AccAccountResultExcelDto>>.FailAsync(localization.GetMessagesResource("NoDataFound")));

                }

                // استدعاء الخدمة التي تحفظ الحسابات من الإكسل
                var saveResult = await accServiceManager.AccAccountService.SaveAccountsExcel(accounts);

                if (saveResult.Succeeded)
                {
                    return Ok(saveResult);
                }
                else
                {

                    return Ok(Result<List<AccAccountResultExcelDto>>.FailAsync(localization.GetMessagesResource("ImportError")));
                }
            }
            catch (Exception ex)
            {

                return Ok(Result<List<AccAccountResultExcelDto>>.FailAsync($"حدث خطأ أثناء الاستيراد: {ex.Message}"));
            }
        }




        #region "transactions_Delete"

        [HttpDelete("DeleteAllAccAccounts")]

        public async Task<IActionResult> DeleteAllAccAccounts()
        {
            var chk = await permission.HasPermission(357, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {

                var delete = await accServiceManager.AccAccountService.DeleteAllAccAccounts();
                return Ok(delete);

            }
            catch (Exception ex)
            {
                return Ok(await Result<AccAccountDto>.FailAsync($"======= Exp in Acc Account  Delete: {ex.Message}"));
            }
        }
        #endregion "transactions_Delete"

        #endregion "transactions_Excel"

        #region "transactions_Reports"

        [HttpPost("SearchReports")]
        public async Task<IActionResult> SearchReports()
        {
            var chk = await permission.HasPermission(79, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                //x.SystemId == 2
                var items = await accServiceManager.AccAccountsReportsVWService.GetAllVW(x => x.FlagDelete == false && x.FacilityId == _currentData.FacilityId);
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable().DistinctBy(x => x.AccAccountCode);
                    var final = res.ToList();
                    return Ok(await Result<List<AccAccountsReportsVw>>.SuccessAsync(final, ""));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccAccountsReportsVw>.FailAsync($"======= Exp in Search AccAccountsVw, MESSAGE: {ex.Message}"));
            }
        }
        #endregion "transactions_Reports"




    }
}
