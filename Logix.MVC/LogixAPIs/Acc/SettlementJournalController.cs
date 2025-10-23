using Logix.Application.Common;
using Logix.Application.DTOs.ACC;
using Logix.Application.Helpers.Acc;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Services;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.Acc.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Logix.MVC.LogixAPIs.Acc
{
    public class SettlementJournalController : BaseAccApiController
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

        public SettlementJournalController(
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
            var chk = await permission.HasPermission(950, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var JournalItems = await accServiceManager.AccJournalMasterService.GetAllVW(a => a.DocTypeId == 33 && a.FlagDelete == false && a.FacilityId == _session.FacilityId);

                var items = await accServiceManager.AccSettlementInstallmentService.GetAllVW(x =>
                          x.FacilityId == _session.FacilityId && x.IsDeleted == false && x.IsDeletedM == false && x.StatusId == 1
                           && !JournalItems.Data.Select(x => x.ReferenceNo).Contains(x.Id));
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();

                    var final = res.ToList();
                    var result = new List<SettlementJournalVM>();
                    foreach (var item in final)
                    {
                        var children = await accServiceManager.AccSettlementScheduleDService.GetAllVW(x => x.IsDeleted == false && x.SsId == item.SsId);
                        var result2 = new SettlementJournalVM
                        {
                            Code = item.Code,
                            SsId = item.SsId ?? 0,
                            Id=item.Id,
                            InstallmentDate = item.InstallmentDate,
                            DescriptionM = item.DescriptionM,
                            sumCredit = children.Data.ToList().Sum(a => a.Credit),
                            sumDebit = children.Data.ToList().Sum(b => b.Debit),
                            Children = children.Data.ToList()
                        };
                        result.Add(result2);
                    }

                    return Ok(await Result<List<SettlementJournalVM>>.SuccessAsync(result, ""));
                }
                return Ok(items);
                    }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }
        [HttpPost("Search")]
        public async Task<IActionResult> Search(AccSettlementInstalFilterDto filter)
        {
            var chk = await permission.HasPermission(950, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {



                var branchsId = _session.Branches.Split(',');
                filter.Code ??= 0;
                filter.Code2 ??= 0; filter.StatusId ??= 0;filter.DocTypeId ??= 0;
                 filter.ReferenceNo ??= 0;filter.InsertUserId ??= 0; filter.BranchId ??= 0;
                filter.Debit ??= 0;  filter.Credit ??= 0;
                var JournalItems = await accServiceManager.AccJournalMasterService.GetAllVW(a => a.DocTypeId == 33 && a.FlagDelete == false && a.FacilityId == _session.FacilityId);

                var items = await accServiceManager.AccSettlementInstallmentService.GetAllVW(x =>
    x.FacilityId == _session.FacilityId && x.IsDeleted == false && x.IsDeletedM == false && x.StatusId == 1
    && !JournalItems.Data.Select(x => x.ReferenceNo).Contains(x.Id)
    && (filter.BranchId == 0 || (x.BranchId == filter.BranchId))
    //&& (filter.DocTypeId == 0 || (x.DocTypeId != null|| x.DocTypeId == filter.DocTypeId))
   && ((filter.BranchId != 0) || branchsId.Contains(x.BranchId.ToString()))
   && (filter.Code == 0 || (x.Code != null && x.Code >= filter.Code && x.Code <= filter.Code2))
     && (string.IsNullOrEmpty(filter.ReferenceCode) || (x.ReferenceCode != null && x.ReferenceCode.Contains(filter.ReferenceCode)))
     && (string.IsNullOrEmpty(filter.Description) || (x.DescriptionM != null && x.DescriptionM.Contains(filter.Description)))
     && (filter.InsertUserId == 0 || x.CreatedBy==filter.InsertUserId)

);
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    if (filter == null)
                    {
                        return Ok(items.Data.Where(x => x.IsDeleted == false));
                    }

                    if (!string.IsNullOrEmpty(filter.CostCenterCode) || filter.Debit > 0 || filter.Credit > 0 || !string.IsNullOrEmpty(filter.AccountCode) || !string.IsNullOrEmpty(filter.AccountName))
                    {
                        var details = await accServiceManager.AccSettlementScheduleDService.GetAllVW(x => x.IsDeleted == false);
                        if (!string.IsNullOrEmpty(filter.CostCenterCode))
                        {

                            res = res.Where(s => details.Data.Any(d => d.SsId == s.SsId && d.CcCode != null && d.CcCode == filter.CostCenterCode));

                        }
                        if (!string.IsNullOrEmpty(filter.CostcenterName))
                        {
                            res = res.Where(s => details.Data.Any(d => d.SsId == s.SsId && d.CostCenterName != null && d.CostCenterName.Contains(filter.CostcenterName)));

                        }

                        if (filter.Debit > 0)
                        {
                            res = res.Where(s => details.Data.Any(d => d.SsId == s.SsId && d.Debit == filter.Debit));


                        }
                        if (filter.Credit > 0)
                        {
                            res = res.Where(s => details.Data.Any(d => d.SsId == s.SsId && d.Credit == filter.Credit));


                        }
                        if (!string.IsNullOrEmpty(filter.AccountCode))
                        {
                            res = res.Where(s => details.Data.Any(d => d.SsId == s.SsId && d.AccAccountCode != null && d.AccAccountCode == filter.AccountCode));

                        }
                        if (!string.IsNullOrEmpty(filter.AccountName))
                        {
                            res = res.Where(s => details.Data.Any(d => d.SsId == s.SsId && d.AccAccountName != null && d.AccAccountName.Contains(filter.AccountName)));

                        }

                       

                    }

                    if (!string.IsNullOrEmpty(filter.StartDate))
                    {
                        DateTime startDate = DateTime.ParseExact(filter.StartDate, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                        res = res.Where(s => DateTime.ParseExact(s.InstallmentDate, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= startDate);
                    }

                    if (!string.IsNullOrEmpty(filter.EndDate))
                    {
                        DateTime endDate = DateTime.ParseExact(filter.EndDate, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                        res = res.Where(s => DateTime.ParseExact(s.InstallmentDate, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= endDate);
                    }




                    var final = res.ToList();
                    var result = new List<SettlementJournalVM>();
                    foreach (var item in final)
                    {
                        var children = await accServiceManager.AccSettlementScheduleDService.GetAllVW(x => x.IsDeleted == false && x.SsId == item.SsId);
                        var result2 = new SettlementJournalVM
                        {
                            Code = item.Code,
                            SsId = item.SsId??0,
                            Id = item.Id,
                            InstallmentDate = item.InstallmentDate,
                            DescriptionM = item.DescriptionM,
                            sumCredit = children.Data.ToList().Sum(a => a.Credit),
                            sumDebit = children.Data.ToList().Sum(b => b.Debit),
                            Children = children.Data.ToList()
                        };
                        result.Add(result2);
                    }

                    return Ok(await Result<List<SettlementJournalVM>>.SuccessAsync(result, ""));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<SettlementJournalVM>.FailAsync($"======= Exp in Search SettlementJournalVM, MESSAGE: {ex.Message}"));
            }
        }



        #endregion "transactions"


        #region "transactions_CreateJournal"
        [HttpPost("CreateJournal")]
        public async Task<IActionResult> CreateJournal(AccJournalSchedulDto obj)
        {
            var chk = await permission.HasPermission(950, PermissionType.Add);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<AccJournalSchedulDto>.FailAsync(localization.GetMessagesResource("dataRequire")));
                }

                var add = await accServiceManager.AccsettlementscheduleService.CreateJournal(obj);
                return Ok(add);

            }
            catch (Exception ex)
            {
                return Ok(await Result<AccJournalSchedulDto>.FailAsync($"======= Exp in Acc Journal add: {ex.Message}"));
            }
        }
        #endregion "transactions_CreateJournal"


    }
}

