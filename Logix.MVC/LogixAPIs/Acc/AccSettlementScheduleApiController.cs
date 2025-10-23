using Logix.Application.Common;
using Logix.Application.DTOs.ACC;
using Logix.Application.Helpers.Acc;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Services;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;
using Logix.MVC.Helpers;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Cmp;
using System.Globalization;

namespace Logix.MVC.LogixAPIs.Acc
{
    public class AccSettlementScheduleApiController : BaseAccApiController
    {

        private readonly IAccServiceManager accServiceManager;
 
    private readonly IPermissionHelper permission;
    private readonly IWebHostEnvironment env;
    private readonly IFilesHelper filesHelper;
    private readonly IDDListHelper listHelper;
    private readonly ILocalizationService localization;
    private readonly ICurrentData _session;
        private readonly IGetAccountIDByCodeHelper getAccountIDByCodeHelper;

        public AccSettlementScheduleApiController(
        IAccServiceManager accServiceManager,
       
        IPermissionHelper permission,
         IWebHostEnvironment env,
        IHttpContextAccessor httpContextAccessor,
        IFilesHelper filesHelper,
        IDDListHelper listHelper,
         ILocalizationService localization
        , ICurrentData session,
            IGetAccountIDByCodeHelper getAccountIDByCodeHelper
        )
    {
        this.accServiceManager = accServiceManager;
      
        this.permission = permission;
        this.env = env;
        this.filesHelper = filesHelper;
        this.listHelper = listHelper;
        this.localization = localization;
        this._session = session;
            this.getAccountIDByCodeHelper = getAccountIDByCodeHelper;
        }



    #region "transactions"


    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll()
    {
        var chk = await permission.HasPermission(939, PermissionType.Show);
        if (!chk)
        {
            return Ok(await Result.AccessDenied("AccessDenied"));
        }

        try
        {
            var items = await accServiceManager.AccsettlementscheduleService.GetAll(x => x.IsDeleted == false && x.FacilityId == _session.FacilityId);
            if (items.Succeeded)
            {
                var res = items.Data.AsQueryable();
                res = res.OrderBy(e => e.Id);
                return Ok(items);
            }
            return Ok(items);
        }
        catch (Exception ex)
        {
            return Ok(ex.Message);
        }
    }
        [HttpPost("Search")]
        public async Task<IActionResult> Search(AccSettlementScheduleFilterDto filter)
        {
            var chk = await permission.HasPermission(939, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var items = await accServiceManager.AccsettlementscheduleService.GetAll(x => x.IsDeleted == false && x.FacilityId == _session.FacilityId &&
                      (filter.Description == null || (x.Description != null && x.Description.Contains(filter.Description))) 
                

                     );

                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();


                    if (!string.IsNullOrEmpty(filter.StartDate) && !string.IsNullOrEmpty(filter.EndDate))
                    {
                        DateTime startDate = DateTime.ParseExact(filter.StartDate ?? "", "yyyy/MM/dd", CultureInfo.InvariantCulture);
                        DateTime endDate = DateTime.ParseExact(filter.EndDate ?? "", "yyyy/MM/dd", CultureInfo.InvariantCulture);

                        res = res.Where(r => !string.IsNullOrEmpty(r.StartDate) && DateTime.ParseExact(r.StartDate, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= startDate
                            && DateTime.ParseExact(r.StartDate, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= endDate);
                    }

                   
                    var final = res.ToList();
                    return Ok(await Result<List<AccSettlementScheduleDto>>.SuccessAsync(final, ""));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccSettlementScheduleDto>.FailAsync($"======= Exp in Search Acc Settlement Schedule, MESSAGE: {ex.Message}"));
            }
        }
        #endregion "transactions"

        #region "transactions_ADD"
        [HttpPost("Add")]
    public async Task<IActionResult> Add(AccSettlementScheduleDto obj)
    {
        var chk = await permission.HasPermission(939, PermissionType.Add);
        if (!chk)
        {
            return Ok(await Result.AccessDenied("AccessDenied"));
        }
        try
        {
            if (!ModelState.IsValid)
            {
                return Ok(await Result<AccSettlementScheduleDto>.FailAsync(localization.GetMessagesResource("dataRequire")));
            }
            obj.FacilityId = _session.FacilityId;
            var add = await accServiceManager.AccsettlementscheduleService.Add(obj);
            return Ok(add);
        }
        catch (Exception ex)
        {
            return Ok(await Result<AccSettlementScheduleDto>.FailAsync($"======= Exp in Acc Settlement Schedule  add: {ex.Message}"));
        }
    }
    #endregion "transactions_Add"

    #region "transactions_Update"
    [HttpPost("Edit")]
    public async Task<IActionResult> Edit(AccSettlementScheduleEditDto obj)
    {
        var chk = await permission.HasPermission(939, PermissionType.Edit);
        if (!chk)
        {
            return Ok(await Result.AccessDenied("AccessDenied"));
        }
        try
        {
            if (!ModelState.IsValid)
            {
                return Ok(await Result<AccSettlementScheduleEditDto>.FailAsync(localization.GetMessagesResource("dataRequire")));
            }

            var Edit = await accServiceManager.AccsettlementscheduleService.Update(obj);
            return Ok(Edit);
        }
        catch (Exception ex)
        {
            return Ok(await Result<AccSettlementScheduleEditDto>.FailAsync($"======= Exp in Acc Settlement Schedule  edit: {ex.Message}"));
        }
    }

    #endregion "transactions_Update"
    #region "transactions_Delete"
    [HttpDelete("Delete")]
    public async Task<IActionResult> Delete(long Id)
    {
        var chk = await permission.HasPermission(939, PermissionType.Delete);
        if (!chk)
        {
            return Ok(await Result.AccessDenied("AccessDenied"));
        }
        try
        {
            var add = await accServiceManager.AccsettlementscheduleService.Remove(Id);
            return Ok(add);
        }
        catch (Exception ex)
        {
            return Ok(await Result<AccSettlementScheduleDto>.FailAsync($"======= Exp in Acc Settlement Schedule   Delete: {ex.Message}"));
        }
    }
    #endregion "transactions_Delete"
    #region "transactions_GetById"

    [HttpGet("GetByIdForEdit")]
    public async Task<IActionResult> GetByIdForEdit(long id)
    {
        try
        {
            var chk = await permission.HasPermission(939, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            if (id <= 0)
            {
                return Ok(await Result<AccSettlementScheduleEditDto>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
            }

            var getItem = await accServiceManager.AccsettlementscheduleService.GetForUpdate<AccSettlementScheduleEditDto>(id);
            if (getItem.Succeeded)
            {
                var obj = getItem.Data;
                    //===========================التفاصيل
                    var getItemD = await accServiceManager.AccSettlementScheduleDService.GetAllVW(x => x.SsId == id);
                    var SettlementDetaileDtolist = new List<AccSettlementScheduleDDto>();

                    foreach (var detail in getItemD.Data)
                    {
                        var SettlementDetaileDto = new AccSettlementScheduleDDto
                        {
                            Id = detail.Id,
                            SsId = detail.SsId,
                            AccAccountId = detail.AccAccountId,
                            CcId = detail.CcId,
                            Debit = detail.Debit,
                            Credit = detail.Credit,
                            ReferenceTypeId = detail.ReferenceTypeId,
                            ReferenceNo = detail.ReferenceNo,
                            Description = detail.Description,
                            CurrencyId = detail.CurrencyId,
                            ExchangeRate = detail.ExchangeRate,
                            Cc2Id = detail.Cc2Id,
                            Cc3Id = detail.Cc3Id,
                            ReferenceCode = detail.ReferenceCode,
                            Cc4Id = detail.Cc4Id,
                            Cc5Id = detail.Cc5Id,
                            BranchId = detail.BranchId,
                            ActivityId = detail.ActivityId,
                            AssestId = detail.AssestId,
                            EmpId = detail.EmpId,
                            IsDeleted = detail.IsDeleted,
                            AccAccountCode = detail.ReferenceTypeId == 1 ? detail.AccAccountCode : detail.Code.ToString(),
                            AccAccountName = detail.ReferenceTypeId == 1 ? detail.AccAccountName : detail.Name,
                         
                            CostCenterCode = detail.CcCode,
                            CostCenterName = detail.CostCenterName,
                        };

                        SettlementDetaileDtolist.Add(SettlementDetaileDto);
                    }
                    obj.DetailsList = SettlementDetaileDtolist;
                    //===========================الدفعات
                    var getItemInstall = await accServiceManager.AccSettlementInstallmentService.GetAll(x => x.SsId == id);
                    obj.InstallmentsList = getItemInstall.Data.ToList();
                    return Ok(await Result<AccSettlementScheduleEditDto>.SuccessAsync(obj, $""));
            }
            else
            {
                return Ok(getItem);
            }
        }
        catch (Exception ex)
        {
            return Ok(await Result<AccSettlementScheduleEditDto>.FailAsync($"====== Exp in GetByIdForEdit Acc Settlement Schedule , MESSAGE: {ex.Message}"));
        }
    }

    [HttpGet("GetById")]
    public async Task<IActionResult> GetById(long id)
    {
        try
        {
            var chk = await permission.HasPermission(939, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AcessDenied"));
            }

            if (id <= 0)
            {
                return Ok(await Result<AccSettlementScheduleDto>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
            }

            var getItem = await accServiceManager.AccsettlementscheduleService.GetOne(s => s.Id == id && s.IsDeleted == false);
            if (getItem.Succeeded)
            {
                var obj = getItem.Data;
                return Ok(await Result<AccSettlementScheduleDto>.SuccessAsync(obj, $""));
            }
            else
            {
                return Ok(getItem);
            }
        }
        catch (Exception ex)
        {
            return Ok(await Result<AccSettlementScheduleDto>.FailAsync($"====== Exp in GetById Acc Settlement Schedule , MESSAGE: {ex.Message}"));
        }
    }

        [HttpGet("GetDetailsById")]
        public async Task<IActionResult> GetDetailsById(long id)
        {
            try
            {
               

                if (id <= 0)
                {
                    return Ok(await Result<AccSettlementScheduleDDto>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var getItem = await accServiceManager.AccSettlementScheduleDService.GetAllVW(x=>x.SsId==id);
                if (getItem.Succeeded)
                {
                    var obj = getItem.Data;
                    var SettlementDetaileDtolist = new List<AccSettlementScheduleDDto>();

                    foreach (var detail in getItem.Data)
                    {
                        var SettlementDetaileDto = new AccSettlementScheduleDDto
                        {
                            Id = detail.Id,
                            SsId = detail.SsId,
                            AccAccountId = detail.AccAccountId,
                            CcId = detail.CcId,
                            Debit = detail.Debit,
                            Credit = detail.Credit,
                            ReferenceTypeId = detail.ReferenceTypeId,
                            ReferenceNo = detail.ReferenceNo,
                            Description = detail.Description,
                            CurrencyId = detail.CurrencyId,
                            ExchangeRate = detail.ExchangeRate,
                            Cc2Id = detail.Cc2Id,
                            Cc3Id = detail.Cc3Id,
                            ReferenceCode = detail.ReferenceCode,
                            Cc4Id = detail.Cc4Id,
                            Cc5Id = detail.Cc5Id,
                            BranchId = detail.BranchId,
                            ActivityId = detail.ActivityId,
                            AssestId = detail.AssestId,
                            EmpId = detail.EmpId,
                            IsDeleted = detail.IsDeleted,
                            //AccAccountCode = detail.ReferenceTypeId == 1 ? detail.AccAccountCode : detail.Code,
                            //AccAccountName = detail.ReferenceTypeId == 1 ? detail.AccAccountName : detail.Name,
                            AccAccountCode = null,
                            AccAccountName = null,
                            CostCenterCode = null,
                            CostCenterName = null,
                        };

                        SettlementDetaileDtolist.Add(SettlementDetaileDto);
                    }



                    return Ok(await Result<List<AccSettlementScheduleDDto>>.SuccessAsync(SettlementDetaileDtolist, $""));
                }
                else
                {
                    return Ok(getItem);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result < List < AccSettlementScheduleDDto>>.FailAsync($"====== Exp in GetByIdForEdit Acc Settlement Schedule , MESSAGE: {ex.Message}"));
            }
        }
        #endregion "transactions_GetById"
        [HttpPost("CheckScheduleDetaile")]
        public async Task<IActionResult> CheckScheduleDetaile(AccJournalDetaileDto obj)
        {
            string MsgErrors = "";
            long AccountID = await getAccountIDByCodeHelper.GetAccountIDByCode((long)obj.ReferenceTypeId, obj.AccAccountCode, _session.FacilityId);

            if (AccountID == 0)
            {
                return Ok(await Result<string>.FailAsync(localization.GetMessagesResource("AccAccountNotfind")));
            }
            if (obj.ReferenceTypeId == 1)
            {
                var ISHelpAccount = await accServiceManager.AccAccountService.ISHelpAccount(obj.AccAccountCode, _session.FacilityId);
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
                            if (Int64.Parse(obj.CostCenterCode) > Int64.Parse(costCenter.CcIdTo) ||
                                Int64.Parse(obj.CostCenterCode) < Int64.Parse(costCenter.CcIdFrom))
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
            if (_session.CalendarType == "1")
            {
                if (await DateHelper.CheckDate(obj.JDateGregorian, _session.FacilityId, _session.CalendarType) == false)
                { return Ok(await Result<string>.FailAsync(localization.GetResource1("DateIsWrong"))); }
            }
            else
            {
                if (await DateHelper.CheckDateH(obj.JDateGregorian) == false)
                { return Ok(await Result<string>.FailAsync(localization.GetResource1("DateIsWrong"))); }
            }
            ////------------------------تشيك الفترة
            //if (await accServiceManager.AccPeriodsService.CheckDateInPeriod(obj.PeriodId ?? 0, obj.JDateGregorian) == false)
            //{
            //    return Ok(await Result<string>.FailAsync(localization.GetResource1("DateOutOfPERIOD")));
            //}

            return Ok(await Result<string>.SuccessAsync(localization.GetMessagesResource("success")));
        }
    }
}