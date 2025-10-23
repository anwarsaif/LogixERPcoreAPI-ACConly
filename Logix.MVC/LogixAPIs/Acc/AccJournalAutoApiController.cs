using Logix.Application.Common;
using Logix.Application.DTOs.ACC;
using Logix.Application.Helpers.Acc;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Logix.MVC.LogixAPIs.Acc
{
    public class AccJournalAutoApiController : BaseAccApiController
    {

        private readonly IAccServiceManager accServiceManager;
        private readonly IMainServiceManager mainServiceManager;
        private readonly IPermissionHelper permission;
        private readonly IWebHostEnvironment env;
        private readonly IFilesHelper filesHelper;
        private readonly IDDListHelper listHelper;
        private readonly ILocalizationService localization;
        private readonly ISysConfigurationHelper configurationHelper;
        private readonly ICurrentData _session;
        private readonly IGetAccountIDByCodeHelper getAccountIDByCodeHelper;

        public AccJournalAutoApiController(
            IAccServiceManager accServiceManager,
            IMainServiceManager mainServiceManager,
            IPermissionHelper permission,
             IWebHostEnvironment env,
            IHttpContextAccessor httpContextAccessor,
            IFilesHelper filesHelper,
            IDDListHelper listHelper,
             ILocalizationService localization
             , ISysConfigurationHelper configurationHelper
            , ICurrentData session,
             IGetAccountIDByCodeHelper getAccountIDByCodeHelper
            )
        {
            this.accServiceManager = accServiceManager;
            this.mainServiceManager = mainServiceManager;
            this.permission = permission;
            this.env = env;
            this.filesHelper = filesHelper;
            this.listHelper = listHelper;
            this.localization = localization;
            this.configurationHelper = configurationHelper;
            this._session = session;
            this.getAccountIDByCodeHelper = getAccountIDByCodeHelper;
        }
        #region "transactions"


        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var chk = await permission.HasPermission(845, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var items = await accServiceManager.AccJournalMasterService.GetAllVW(x => x.FlagDelete == false && x.FacilityId == _session.FacilityId  && x.FinYear == _session.FinYear);
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    res = res.OrderBy(e => e.JId);
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
        public async Task<IActionResult> Search(AccJournalMasterfilterDto filter)
        {
            var chk = await permission.HasPermission(845, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                long fromParsed, toParsed;
                var branchsId = _session.Branches.Split(',');
                filter.PaymentTypeId ??= 0;
                filter.ReferenceNo ??= 0;
                filter.DocTypeId ??= 0;
                filter.StatusId ??= 0;
                filter.PeriodId ??= 0;
                filter.CreatedBy ??= 0;
                filter.Debit ??= 0;
                filter.Credit ??= 0;
                filter.BranchId ??= 0;
                var items = await accServiceManager.AccJournalMasterService.GetAllVW(x =>
    x.FacilityId == _session.FacilityId && x.FlagDelete == false
     &&
     x.FinYear == _session.FinYear
     &&
    (filter.ReferenceNo > 0 ? x.ReferenceNo.Equals(filter.ReferenceNo) : true)
    &&
    (filter.DocTypeId > 0 ? x.DocTypeId.Equals(filter.DocTypeId) : true) &&
    (filter.InsertUserId > 0 ? x.InsertUserId.Equals(filter.InsertUserId) : true) &&
    (filter.PeriodId > 0 ? x.PeriodId.Equals(filter.PeriodId) : true) &&
    (filter.JCode == null || (x.JCode != null && x.JCode.CompareTo(filter.JCode) >= 0 && x.JCode.CompareTo(filter.JCode2) <= 0)) &&
 (string.IsNullOrEmpty(filter.ReferenceCode) || (x.ReferenceCode != null && x.ReferenceCode.Contains(filter.ReferenceCode)))
 && (filter.BranchId == 0 || (x.BranchId == filter.BranchId))
        && ((filter.BranchId != 0) || branchsId.Contains(x.BranchId.ToString()))
   && (filter.StatusId == 0 || (x.StatusId == filter.StatusId))
);
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    if (filter == null)
                    {
                        return Ok(items.Data.Where(x => x.FlagDelete == false));
                    }

                    if (!string.IsNullOrEmpty(filter.CostCenterCode) || filter.Debit > 0 || filter.Credit > 0 || !string.IsNullOrEmpty(filter.AccountCode) || !string.IsNullOrEmpty(filter.AccountName) || !string.IsNullOrEmpty(filter.Description))
                    {
                        var details = await accServiceManager.AccJournalDetaileService.GetAllVW(x => x.FlagDelete == false);
                        if (!string.IsNullOrEmpty(filter.CostCenterCode))
                        {

                            res = res.Where(s => details.Data.Any(d => d.JId == s.JId && d.CostCenterCode != null && d.CostCenterCode == filter.CostCenterCode));

                        }
                        if (!string.IsNullOrEmpty(filter.CostcenterName))
                        {
                            res = res.Where(s => details.Data.Any(d => d.JId == s.JId && d.CostCenterName != null && d.CostCenterName.Contains(filter.CostcenterName)));

                        }

                        if (filter.Debit > 0)
                        {
                            res = res.Where(s => details.Data.Any(d => d.JId == s.JId && d.Debit == filter.Debit));


                        }
                        if (filter.Credit > 0)
                        {
                            res = res.Where(s => details.Data.Any(d => d.JId == s.JId && d.Credit == filter.Credit));


                        }
                        if (!string.IsNullOrEmpty(filter.AccountCode))
                        {
                            res = res.Where(s => details.Data.Any(d => d.JId == s.JId && d.AccAccountCode != null && d.AccAccountCode == filter.AccountCode));

                        }
                        if (!string.IsNullOrEmpty(filter.AccountName))
                        {
                            res = res.Where(s => details.Data.Any(d => d.JId == s.JId && d.AccAccountName != null && d.AccAccountName.Contains(filter.AccountName)));

                        }

                        if (!string.IsNullOrEmpty(filter.Description))
                        {
                            res = res.Where(s => details.Data.Any(d => d.JId == s.JId && d.Description.IndexOf(filter.Description, StringComparison.OrdinalIgnoreCase) >= 0));
                        }

                    }

                    if (!string.IsNullOrEmpty(filter.JDateGregorian))
                    {
                        DateTime startDate = DateTime.ParseExact(filter.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                        res = res.Where(s => DateTime.ParseExact(s.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= startDate);
                    }

                    if (!string.IsNullOrEmpty(filter.JDateGregorian2))
                    {
                        DateTime endDate = DateTime.ParseExact(filter.JDateGregorian2, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                        res = res.Where(s => DateTime.ParseExact(s.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= endDate);
                    }






                    var final = res.ToList();
                    return Ok(await Result<List<AccJournalMasterVw>>.SuccessAsync(final, ""));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccJournalMasterVw>.FailAsync($"======= Exp in Search AccJournalMasterVw, MESSAGE: {ex.Message}"));
            }
        }
        #endregion "transactions"

        #region "transactions_ADD"
        [HttpPost("Add")]
        public async Task<IActionResult> Add(AccJournalMasterDtoVW obj)
        {
            var chk = await permission.HasPermission(845, PermissionType.Add);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<AccJournalMasterDtoVW>.FailAsync(localization.GetMessagesResource("dataRequire")));
                }
          
                var add = await accServiceManager.AccJournalMasterService.AddDetaile(obj);
                return Ok(add);

            }
            catch (Exception ex)
            {
                return Ok(await Result<AccJournalMasterDtoVW>.FailAsync($"======= Exp in Acc Journal add: {ex.Message}"));
            }
        }
        #endregion "transactions_Add"

        #region "transactions_Update"

        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(AccJournalMasterEditDtoVW obj)
        {
            var chk = await permission.HasPermission(845, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<AccJournalMasterEditDtoVW>.FailAsync(localization.GetMessagesResource("dataRequire")));
                }

                var Edit = await accServiceManager.AccJournalMasterService.UpdateDetaile(obj);
                return Ok(Edit);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccJournalMasterEditDtoVW>.FailAsync($"======= Exp in Acc Journal  edit: {ex.Message}"));
            }
        }

        #endregion "transactions_Update"

        #region "transactions_Delete"
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long Id)
        {
            var chk = await permission.HasPermission(845, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var add = await accServiceManager.AccJournalMasterService.Remove(Id);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccJournalMasterDto>.FailAsync($"======= Exp in Acc Journal  Delete: {ex.Message}"));
            }
        }
        #endregion "transactions_Delete"

        #region "transactions_GetById"

        [HttpGet("GetByIdForEdit")]
        public async Task<IActionResult> GetByIdForEdit(long id)
        {
            try
            {
                var chk = await permission.HasPermission(845, PermissionType.Show);
                if (!chk)
                {
                    return Ok(await Result.AccessDenied("AccessDenied"));
                }

                if (id <= 0)
                {
                    return Ok(await Result<AccJournalMasterDto>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var getItem = await accServiceManager.AccJournalMasterService.GetForUpdate<AccJournalMasterDto>(id);
                if (getItem.Succeeded)
                {

                    var obj = new AccJournalMasterDtoVW
                    {
                        AccJournalMasterDto = getItem.Data,

                    };
                    var getDetails = await accServiceManager.AccJournalDetaileService.GetAllVW(x => x.FlagDelete == false && x.JId == id);

                    if (getDetails.Succeeded)
                    {
                        var AccJournalDetaileDtolist = new List<AccJournalDetaileDto>();

                        foreach (var detail in getDetails.Data)
                        {
                            var AccJournalDetaileDto = new AccJournalDetaileDto
                            {
                                JDetailesId = detail.JDetailesId,
                                JId = detail.JId,
                                JDateGregorian = detail.JDateGregorian,
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
                                IsDeleted = detail.FlagDelete,
                                AccAccountCode = detail.ReferenceTypeId == 1 ? detail.AccAccountCode : detail.Code,
                                AccAccountName = detail.ReferenceTypeId == 1 ? detail.AccAccountName : detail.Name,
                                CostCenterCode = detail.CostCenterCode,
                                CostCenterName = detail.CostCenterName,
                                CreatedOn = detail.InsertDate,
                                ModifiedOn = detail.UpdateDate,
                            };

                            AccJournalDetaileDtolist.Add(AccJournalDetaileDto);
                        }

                        obj.DetailsList = AccJournalDetaileDtolist;
                    }




                    return Ok(await Result<AccJournalMasterDtoVW>.SuccessAsync(obj, $""));
                }
                else
                {
                    return Ok(getItem);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccJournalMasterDtoVW>.FailAsync($"====== Exp in GetByIdForEdit Acc Journal, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(long id)
        {
            try
            {
                var chk = await permission.HasPermission(845, PermissionType.Show);
                if (!chk)
                {
                    return Ok(await Result.AccessDenied("AcessDenied"));
                }

                if (id <= 0)
                {
                    return Ok(await Result<AccJournalMasterDto>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var getItem = await accServiceManager.AccJournalMasterService.GetOne(s => s.JId == id);
                if (getItem.Succeeded)
                {
                    var obj = getItem.Data;
                    return Ok(await Result<AccJournalMasterDto>.SuccessAsync(obj, $""));
                }
                else
                {
                    return Ok(getItem);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccJournalMasterDto>.FailAsync($"====== Exp in GetById Acc Journal, MESSAGE: {ex.Message}"));
            }
        }





        #endregion "transactions_GetById"

       

    }
}


