using Logix.Application.Common;
using Logix.Application.DTOs.ACC;
using Logix.Application.DTOs.Main;
using Logix.Application.Helpers.Acc;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;
using Logix.Domain.Main;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.Acc.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Logix.MVC.LogixAPIs.Acc
{
    public class AccSharedController : BaseAccApiController
    {
        private readonly ILocalizationService localization;
        private readonly ISysConfigurationHelper configurationHelper;
        private readonly ICurrentData session;
        private readonly IAccServiceManager accServiceManager;
        private readonly IMainServiceManager mainServiceManager;
        private readonly IGetAccountIDByCodeHelper getAccountIDByCodeHelper;
        private readonly IDDListHelper listHelper;

        public AccSharedController(

           ISysConfigurationHelper configurationHelper,
           ILocalizationService localization
           , ICurrentData session,
           IAccServiceManager accServiceManager,
          IMainServiceManager mainServiceManager,
          IGetAccountIDByCodeHelper getAccountIDByCodeHelper,
           IDDListHelper listHelper


         )
        {


            this.localization = localization;
            this.configurationHelper = configurationHelper;
            this.session = session;
            this.accServiceManager = accServiceManager;
            this.mainServiceManager = mainServiceManager;
            this.getAccountIDByCodeHelper = getAccountIDByCodeHelper;
            this.listHelper = listHelper;
        }
        [HttpGet("GetProperties")]
        public async Task<ActionResult> GetProperties()
        {
            try
            {
                //get properties that indicates if cost center 2,3,4 and 5 is visible or not
                var cc2Visible = await configurationHelper.GetValue(27, session.FacilityId);
                var cc3Visible = await configurationHelper.GetValue(28, session.FacilityId);
                var cc4Visible = await configurationHelper.GetValue(29, session.FacilityId);
                var cc5Visible = await configurationHelper.GetValue(30, session.FacilityId);
                var branchVisible = await configurationHelper.GetValue(31, session.FacilityId);
                var assVisible = await configurationHelper.GetValue(32, session.FacilityId);
                var eMPVisible = await configurationHelper.GetValue(33, session.FacilityId);

                AddAccProperties obj = new()
                {
                    CC2Visible = cc2Visible == "1",
                    CC3Visible = cc3Visible == "1",
                    CC4Visible = cc4Visible == "1",
                    CC5Visible = cc5Visible == "1",
                    BranchVisible = branchVisible == "1",
                    AssVisible = assVisible == "1",
                    EMPVisible = eMPVisible == "1",
                    CC1Title = await configurationHelper.GetValue(45, session.FacilityId),
                    CC2Title = await configurationHelper.GetValue(46, session.FacilityId),
                    CC3Title = await configurationHelper.GetValue(47, session.FacilityId),
                    CC4Title = await configurationHelper.GetValue(48, session.FacilityId),
                    CC5Title = await configurationHelper.GetValue(49, session.FacilityId)
                };

                return Ok(await Result<AddAccProperties>.SuccessAsync(obj));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in GetProperties FxaFixedAssetController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetCurrencyID")]
        public async Task<IActionResult> GetCurrencyID(long AccountType, string code)
        {
            var CurrencyID = await accServiceManager.AccJournalMasterService.GetCurrencyID(AccountType, code, session.FacilityId);


            return Ok(await Result<long>.SuccessAsync(CurrencyID));
        }
        [HttpGet("GetCustomerID")]
        public async Task<IActionResult> GetCustomerID(long CusTypeId, string code)
        {
            try
            {
                var CustomerData = await mainServiceManager.SysCustomerService.GetOneVW(x => x.CusTypeId == CusTypeId && x.Code.Equals(code) && x.FacilityId == session.FacilityId && x.IsDeleted == false);
                if (CustomerData.Succeeded)
                {
                    var obj = CustomerData.Data;
                    return Ok(await Result<SysCustomerVw>.SuccessAsync(obj, $""));
                }
                else
                {
                    return Ok(CustomerData);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<SysCustomerVw>.FailAsync($"====== Exp in GetCustomerID Acc SysCustomerVw, MESSAGE: {ex.Message}"));
            }

        }

        [HttpGet("GetByJCode")]
        public async Task<IActionResult> GetByJCode(string Code, int DocTypeId)
        {
            try
            {


                if (string.IsNullOrEmpty(Code))
                {
                    return Ok(await Result<AccJournalMasterDto>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var getItem = await accServiceManager.AccJournalMasterService.GetAll(s => s.JCode.Equals(Code) && s.FlagDelete == false && s.DocTypeId == DocTypeId);
                if (getItem.Succeeded)
                {
                    var obj = getItem.Data.FirstOrDefault();
                    return Ok(await Result<AccJournalMasterDto>.SuccessAsync(obj, $""));
                }
                else
                {
                    return Ok(getItem);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccJournalMasterDto>.FailAsync($"====== Exp in GetByJCode Acc Journal, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("CheckJournalDetaile")]
        public async Task<IActionResult> CheckJournalDetaile(AccJournalDetaileDto obj)
        {
            string MsgErrors = "";
            long AccountID = await getAccountIDByCodeHelper.GetAccountIDByCode((long)obj.ReferenceTypeId, obj.AccAccountCode, session.FacilityId);

            if (AccountID == 0)
            {
                return Ok(await Result<string>.FailAsync(localization.GetMessagesResource("AccAccountNotfind")));
            }
            if (obj.ReferenceTypeId == 1)
            {
                var ISHelpAccount = await accServiceManager.AccAccountService.ISHelpAccount(obj.AccAccountCode, session.FacilityId);
                if (ISHelpAccount == true)
                { return Ok(await Result<string>.FailAsync(localization.GetMessagesResource("ISHelpAccount"))); }
            }

            if (!string.IsNullOrEmpty(obj.CostCenterCode))
            {
                var CostCenter = await accServiceManager.AccAccountsCostcenterService.GetAll(X => X.IsDeleted == false && X.AccAccountId == AccountID);
                if (CostCenter.Succeeded)
                {
                    var CostCenterOne = CostCenter.Data.Where(x => x.CcNo == 1);
                    foreach (var costCenter in CostCenterOne)
                    {
                        if (!string.IsNullOrEmpty(costCenter.CcIdFrom) && !string.IsNullOrEmpty(costCenter.CcIdTo))
                        {
                            if (long.Parse(obj.CostCenterCode) > long.Parse(costCenter.CcIdTo) ||
                                long.Parse(obj.CostCenterCode) < long.Parse(costCenter.CcIdFrom))
                            {
                                return Ok(await Result<string>.FailAsync(localization.GetAccResource("Allowablecostcenters") + costCenter.CcIdFrom + " - " + costCenter.CcIdTo));
                            }
                        }
                        else
                        {
                            var costCenterOne = CostCenter.Data.Where(x => x.CcNo == 1);
                            foreach (var cost in costCenterOne)
                            {
                                if (cost.IsRequired == true)
                                {
                                    MsgErrors += localization.GetAccResource("CostcenterAccountRequired") + ",";
                                }
                            }
                        }
                    }

                    var CostCenterTow = CostCenter.Data.Where(x => x.CcNo == 2);
                    foreach (var costCenter in CostCenterTow)
                    {
                        if (costCenter.IsRequired == true)
                        {
                            MsgErrors += localization.GetAccResource("Costcenter2AccountRequired") + ",";
                        }
                    }
                    //  'هل مطلوب مركز تكلفة ام لا  CcNo3

                    var CostCenterThree = CostCenter.Data.Where(x => x.CcNo == 3);
                    foreach (var costCenter in CostCenterThree)
                    {
                        costCenter.IsRequired = true;
                        if (costCenter.IsRequired == true)
                        {
                            MsgErrors += localization.GetAccResource("Costcenter3AccountRequired") + ",";

                        }
                    }
                    //  'هل مطلوب مركز تكلفة ام لا  CcNo4
                    var CostCenterFour = CostCenter.Data.Where(x => x.CcNo == 4);
                    foreach (var costCenter in CostCenterFour)
                    {
                        if (costCenter.IsRequired == true)
                        {
                            MsgErrors += localization.GetAccResource("Costcenter4AccountRequired") + ",";

                        }
                    }
                    //  'هل مطلوب مركز تكلفة ام لا  CcNo5
                    var CostCenterFive = CostCenter.Data.Where(x => x.CcNo == 5);
                    foreach (var costCenter in CostCenterFive)
                    {
                        if (costCenter.IsRequired == true)
                        {
                            MsgErrors += localization.GetAccResource("Costcenter5AccountRequired") + ",";
                        }
                    }

                    if (!string.IsNullOrEmpty(MsgErrors))
                    {
                        return Ok(await Result<string>.FailAsync(MsgErrors));

                    }


                }
            }
            //--------------------------------DateD
            if (session.CalendarType == "1")
            {
                if (await DateHelper.CheckDate(obj.JDateGregorian, session.FacilityId, session.CalendarType) == false)
                { return Ok(await Result<string>.FailAsync(localization.GetResource1("DateIsWrong"))); }
            }
            else
            {
                if (await DateHelper.CheckDateH(obj.JDateGregorian) == false)
                { return Ok(await Result<string>.FailAsync(localization.GetResource1("DateIsWrong"))); }
            }
            //------------------------تشيك الفترة
            if (await accServiceManager.AccPeriodsService.CheckDateInPeriod(obj.PeriodId ?? 0, obj.JDateGregorian) == false)
            {
                return Ok(await Result<string>.FailAsync(localization.GetResource1("DateOutOfPERIOD")));
            }

            return Ok(await Result<string>.SuccessAsync(localization.GetMessagesResource("success")));
        }


        [HttpGet("GetJIDByJCode2")]
        public async Task<IActionResult> GetJIDByJCode2(string JCode, int DocTypeId)
        {
            var JID = await accServiceManager.AccJournalMasterService.GetJIDByJCode2(JCode, DocTypeId, session.FacilityId, session.FinYear);

            return Ok(await Result<long>.SuccessAsync(JID));
        }

        [HttpGet("GetCustomersName")]
        public async Task<IActionResult> GetCustomersName(string Name)
        {
            try
            {
                var list = new List<SysCustomerDto>();
                List<int> currentBranch = MethodsHelper.ConvertStringToIntList(session.Branches);

                var accounts = await mainServiceManager.SysCustomerService.GetAll(x =>
                    x.Name.Contains(Name) &&
                    (x.BranchId != null && currentBranch.Contains(x.BranchId ?? 0)) &&
                    x.IsDeleted == false &&
                    x.CusTypeId == 2
                );

                if (accounts.Succeeded && accounts.Data != null)
                {
                    list.AddRange(accounts.Data.ToList());
                }

                // Order the list by Code
                list = list.OrderBy(x => x.Code).ToList();

                var result = list.Select(s => new { Id = s.Id, Code = s.Code, Name = s.Name, Name2 = s.Name2 });

                return Ok(await Result<object>.SuccessAsync(result, ""));
            }
            catch (Exception exp)
            {
                return Ok(await Result<string>.FailAsync($"EXP, Message: {exp.Message}"));
            }
        }



        [HttpPost("GetPettyCashTemp")]
        public async Task<IActionResult> GetPettyCashTemp([FromQuery] AccPettyCashTempVM filter)
        {
            try
            {
                filter.Total ??= 0;
                var res = await accServiceManager.AccPettyCashService.GetPettyCashTemp(
                    x => x.EmpId == session.EmpId &&
                    (filter.Total == 0 || x.Total == filter.Total) &&
                    (string.IsNullOrEmpty(filter.SupplierName) ||
                        (!string.IsNullOrEmpty(x.SupplierName) && x.SupplierName.Contains(filter.SupplierName))) &&
                    (string.IsNullOrEmpty(filter.ReferenceCode) ||
                        (!string.IsNullOrEmpty(x.ReferenceCode) && x.ReferenceCode.Contains(filter.ReferenceCode)))
               && (string.IsNullOrEmpty(filter.ReferenceDate) ||
                        (!string.IsNullOrEmpty(x.ReferenceDate) && x.ReferenceDate == filter.ReferenceDate))
                        );
                var items = res.Data.ToList();
                return Ok(await Result<List<AccPettyCashTempVw>>.SuccessAsync(items, ""));

            }
            catch (Exception exp)
            {
                return Ok(Result<List<AccPettyCashTempVw>>.Fail($"EXP, Message: {exp.Message}"));
            }
        }

        [HttpGet("DDLReferenceTypebyParentId")]
        public async Task<IActionResult> DDLReferenceTypebyParentId(long ParentId)
        {
            var lang = session.Language;

            try
            {
                var items = await accServiceManager.AccReferenceTypeService.GetAllVW(x => x.FlagDelete == false && x.ParentId == ParentId);
                var res = items.Data.OrderBy(x => x.ParentId).ThenBy(x => x.ReferenceTypeId).ToList();
                var ddReferenceTypeList = listHelper.GetFromList<long>(items.Data.Select(s => new DDListItem<long>
                {
                    Name = lang == 1 ? s.ReferenceTypeName ?? "" : s.ReferenceTypeName2 ?? "",
                    Value = s.ReferenceTypeId
                }), hasDefault: false, defaultText: localization.GetResource1("Choose"));


                return Ok(await Result<SelectList>.SuccessAsync(ddReferenceTypeList));


            }

            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
        [HttpGet("DDLReferenceType")]
        public async Task<IActionResult> DDLReferenceType(string? ReferenceTypeId)
        {
            var lang = session.Language;

            try
            {
                var items = await accServiceManager.AccReferenceTypeService.GetAllVW(x => x.FlagDelete == false && (
                string.IsNullOrEmpty(ReferenceTypeId) || ReferenceTypeId.Contains(x.ReferenceTypeId.ToString()))
                );
                var res = items.Data.OrderBy(x => x.ParentId).ThenBy(x => x.ReferenceTypeId).ToList();
                var ddReferenceTypeList = listHelper.GetFromList<long>(items.Data.Select(s => new DDListItem<long>
                {
                    Name = lang == 1 ? s.ReferenceTypeName ?? "" : s.ReferenceTypeName2 ?? "",
                    Value = s.ReferenceTypeId
                }), hasDefault: false, defaultText: localization.GetResource1("Choose"));


                return Ok(await Result<SelectList>.SuccessAsync(ddReferenceTypeList));


            }

            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        //  لايجاد جميع الملفات التابعه لعنصر وقت التعديل  
        [HttpGet("getFilesDocument")]
        public async Task<IActionResult> getFilesDocument(long AppTypeId, long ScreenId)
        {
            try
            {
                var item = await mainServiceManager.SysFilesDocumentService.GetAllVW(x => x.AppTypeId == AppTypeId && x.ScreenId == ScreenId && x.IsDeleted == false);
                return Ok(item);
            }
            catch (Exception ex)
            {
                return Ok(await Result<SysFileDto>.FailAsync($"====== Exp in GetFiles, MESSAGE: {ex.Message}"));
            }
        }

    }
}
