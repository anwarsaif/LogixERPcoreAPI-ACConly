using Logix.Application.Common;
using Logix.Application.DTOs.ACC;
using Logix.Application.Helpers.Acc;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.Acc.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Logix.MVC.LogixAPIs.Acc
{
    public class AccTransferFromgeneralledgerController : BaseAccApiController
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

        public AccTransferFromgeneralledgerController(
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
            var chk = await permission.HasPermission(71, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                string Posting_By_User_Doc_Type = "1";
                Posting_By_User_Doc_Type = await configurationHelper.GetValue(20, _session.FacilityId);
                string Acc_Posting = "1";
                Acc_Posting = await mainServiceManager.SysUserService.GetUserPosting(_session.UserId) ?? "1";
                var items = await accServiceManager.AccJournalMasterService.GetAllVW(x => x.FlagDelete == false && x.FacilityId == _session.FacilityId && x.FinYear == _session.FinYear && x.StatusId == 2 && Posting_By_User_Doc_Type.Equals("1") || Acc_Posting.Contains(x.DocTypeId.ToString()));
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
            var chk = await permission.HasPermission(71, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                //string Posting_By_User_Doc_Type = "1";
                //Posting_By_User_Doc_Type = await configurationHelper.GetValue(20, _session.FacilityId);
                //                string Acc_Posting = "0";
                //                Acc_Posting = await mainServiceManager.SysUserService.GetUserPosting(_session.UserId) ?? "0";


                //                var branchsId = _session.Branches.Split(',');
                //                filter.PaymentTypeId ??= 0;
                //                filter.StatusId ??= 0;
                //                filter.DocTypeId ??= 0;
                //                filter.ReferenceNo ??= 0;
                //                filter.PeriodId ??= 0;
                //                filter.InsertUserId ??= 0;
                //                filter.BranchId ??= 0;
                //                filter.Debit ??= 0;
                //                filter.Credit ??= 0;
                //                var items = await accServiceManager.AccJournalMasterService.GetAllVW(x =>
                //    x.FacilityId == _session.FacilityId && x.FlagDelete == false && x.StatusId == 2 && (Posting_By_User_Doc_Type.Contains("1") || Acc_Posting.Contains(x.DocTypeId.ToString()))
                //    && x.FinYear == _session.FinYear
                //     && (filter.PeriodId == 0 || (x.PeriodId.Equals(filter.PeriodId)))
                //     && (filter.BranchId == 0 || (x.BranchId == filter.BranchId))
                //   && ((filter.BranchId != 0) || branchsId.Contains(x.BranchId.ToString()))
                //   && (filter.DocTypeId == 0 || x.DocTypeId.Equals(filter.DocTypeId))
                //    && (string.IsNullOrEmpty(filter.JCode) || (x.JCode != null && x.JCode.CompareTo(filter.JCode) >= 0 && x.JCode.CompareTo(filter.JCode2) <= 0))
                //     && (filter.ReferenceNo == 0 || x.ReferenceNo.Equals(filter.ReferenceNo))
                //     && (string.IsNullOrEmpty(filter.ReferenceCode) || (x.ReferenceCode != null && x.ReferenceCode.Contains(filter.ReferenceCode)))

                //);
                //                if (items.Succeeded)
                //                {
                //                    var res = items.Data.AsQueryable();
                //                    if (filter == null)
                //                    {
                //                        return Ok(items.Data.Where(x => x.FlagDelete == false));
                //                    }

                //                    if (!string.IsNullOrEmpty(filter.CostCenterCode) || filter.Debit > 0 || filter.Credit > 0 || !string.IsNullOrEmpty(filter.AccountCode) || !string.IsNullOrEmpty(filter.AccountName) || !string.IsNullOrEmpty(filter.Description))
                //                    {
                //                        var details = await accServiceManager.AccJournalDetaileService.GetAllVW(x => x.FlagDelete == false);
                //                        if (!string.IsNullOrEmpty(filter.CostCenterCode))
                //                        {

                //                            res = res.Where(s => details.Data.Any(d => d.JId == s.JId && d.CostCenterCode != null && d.CostCenterCode == filter.CostCenterCode));

                //                        }
                //                        if (!string.IsNullOrEmpty(filter.CostcenterName))
                //                        {
                //                            res = res.Where(s => details.Data.Any(d => d.JId == s.JId && d.CostCenterName != null && d.CostCenterName.Contains(filter.CostcenterName)));

                //                        }

                //                        if (filter.Debit > 0)
                //                        {
                //                            res = res.Where(s => details.Data.Any(d => d.JId == s.JId && d.Debit == filter.Debit));


                //                        }
                //                        if (filter.Credit > 0)
                //                        {
                //                            res = res.Where(s => details.Data.Any(d => d.JId == s.JId && d.Credit == filter.Credit));


                //                        }
                //                        if (!string.IsNullOrEmpty(filter.AccountCode))
                //                        {
                //                            res = res.Where(s => details.Data.Any(d => d.JId == s.JId && d.AccAccountCode != null && d.AccAccountCode == filter.AccountCode));

                //                        }
                //                        if (!string.IsNullOrEmpty(filter.AccountName))
                //                        {
                //                            res = res.Where(s => details.Data.Any(d => d.JId == s.JId && d.AccAccountName != null && d.AccAccountName.Contains(filter.AccountName)));

                //                        }

                //                        if (!string.IsNullOrEmpty(filter.Description))
                //                        {
                //                            res = res.Where(s => details.Data.Any(d => d.JId == s.JId && d.Description.IndexOf(filter.Description, StringComparison.OrdinalIgnoreCase) >= 0));
                //                        }

                //                    }

                //                    if (!string.IsNullOrEmpty(filter.JDateGregorian))
                //                    {
                //                        DateTime startDate = DateTime.ParseExact(filter.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                //                        res = res.Where(s => DateTime.ParseExact(s.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= startDate);
                //                    }

                //                    if (!string.IsNullOrEmpty(filter.JDateGregorian2))
                //                    {
                //                        DateTime endDate = DateTime.ParseExact(filter.JDateGregorian2, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                //                        res = res.Where(s => DateTime.ParseExact(s.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= endDate);
                //                    }




                //                    var final = res.ToList();
                //                    var result = new List<JournalMasterVM>();
                //                    foreach (var item in final)
                //                    {
                //                        var children = await accServiceManager.AccJournalDetaileService.GetAllVW(x => x.FlagDelete == false && x.JId == item.JId);
                //                        var result2 = new JournalMasterVM
                //                        {
                //                            JCode = item.JCode,
                //                            JId = item.JId,
                //                            JDateGregorian = item.JDateGregorian,
                //                            DocTypeName = item.DocTypeName,
                //                            DocTypeName2 = item.DocTypeName,
                //                            BraName = item.BraName,
                //                            BraName2 = item.BraName2,
                //                            ReferenceCode = item.ReferenceCode,
                //                            InsertDate = item.InsertDate,
                //                            StatusName = item.StatusName,
                //                            StatusName2 = item.StatusName2,
                //                            DocTypeId = item.DocTypeId,
                //                            ReferenceNo = item.ReferenceNo,
                //                            UserFullname = item.UserFullname,
                //                            sumCredit = children.Data.ToList().Sum(a => a.Credit),
                //                            sumDebit = children.Data.ToList().Sum(b => b.Debit),
                //                            Children = children.Data.ToList()
                //                        };
                //                        result.Add(result2);
                //                    }

                //                    return Ok(await Result<List<JournalMasterVM>>.SuccessAsync(result, ""));
                //                }
                var items = await accServiceManager.AccJournalMasterService.TransferFromgeneralledgerSearch(filter);

				return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccJournalMasterVw>.FailAsync($"======= Exp in Search AccJournalMasterVw, MESSAGE: {ex.Message}"));
            }
        }



        #endregion "transactions"
        #region "transactions_ADD"

        [HttpPost("UpdateTransferGeneralFrom")]
        public async Task<IActionResult> UpdateTransferGeneralFrom(AccJournalMasterStatusDto obj)
        {
            var chk = await permission.HasPermission(71, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                if (string.IsNullOrEmpty(obj.SelectedJId))
                    return Ok(await Result.FailAsync($"{localization.GetAccResource("selectingProcessJournal")}"));

                obj.StatusId = 1;
                var add = await accServiceManager.AccJournalMasterService.UpdateTransferGeneralFrom(obj);

                return Ok(add);


            }
            catch (Exception ex)
            {
                return Ok(await Result<AccJournalMasterDtoVW>.FailAsync($"======= Exp in Acc Journal add: {ex.Message}"));
            }
        }

        [HttpPost("UpdateTransferGeneralAllFrom")]
        public async Task<IActionResult> UpdateTransferGeneralAllFrom(AccJournalMasterStatusDto obj)
        {
            var chk = await permission.HasPermission(71, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                if (string.IsNullOrEmpty(obj.SelectedJId))
                    return Ok(await Result.FailAsync($"{localization.GetPMResource("selectingProcessextract")}"));

                obj.StatusId = 1;
                var add = await accServiceManager.AccJournalMasterService.UpdateTransferGeneralAllFrom(obj);

                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccJournalMasterStatusDto>.FailAsync($"======= Exp in Acc Journal add: {ex.Message}"));
            }
        }


        #endregion "transactions_Add"

        

    }
}

