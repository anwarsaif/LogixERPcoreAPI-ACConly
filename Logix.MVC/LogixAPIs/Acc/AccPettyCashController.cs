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
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Operations;
using System.Globalization;

namespace Logix.MVC.LogixAPIs.Acc
{
    public class AccPettyCashController : BaseAccApiController
    {
        private readonly IAccServiceManager accServiceManager;
        private readonly IPermissionHelper permission;
        private readonly IWebHostEnvironment env;
        private readonly IFilesHelper filesHelper;
        private readonly IDDListHelper listHelper;
        private readonly ILocalizationService localization;
        private readonly ISysConfigurationHelper configurationHelper;
        private readonly ICurrentData _session;
        private readonly IApiDDLHelper ddlHelper;

        public AccPettyCashController(
            IAccServiceManager accServiceManager,
            IPermissionHelper permission,
             IWebHostEnvironment env,
            IHttpContextAccessor httpContextAccessor,
            IFilesHelper filesHelper,
            IDDListHelper listHelper,
             ILocalizationService localization
             , ISysConfigurationHelper configurationHelper
            , ICurrentData session,
            IApiDDLHelper ddlHelper
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
            this.ddlHelper = ddlHelper;
        }
        #region "transactions"


        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var chk = await permission.HasPermission(811, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var items = await accServiceManager.AccPettyCashService.GetAllVW(x => x.IsDeleted == false && x.FacilityId == _session.FacilityId);
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
        public async Task<IActionResult> Search(AccPettyCashFilterDto filter)
        {
            var chk = await permission.HasPermission(811, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var branchsId = _session.Branches.Split(',');
                filter.BranchId ??= 0;
                filter.Code ??= 0; filter.ApplicationCode ??= 0; filter.StatusId ??= 0; filter.TypeId ??= 0; filter.PettyCashType ??= 0; filter.ReferenceTypeId ??= 0;
                var items = await accServiceManager.AccPettyCashService.GetAllVW(x => x.IsDeleted == false && x.FacilityId == _session.FacilityId &&
                (filter.Code == 0 || x.Code==filter.Code)
                && (filter.ApplicationCode == 0 || x.ApplicationCode==filter.ApplicationCode)
                && (filter.StatusId == 0 || x.StatusId == filter.StatusId)
                && (filter.TypeId == 0 || x.TypeId == filter.TypeId)
                && (filter.PettyCashType == 0 || x.PettyCashType == filter.PettyCashType)
                && (filter.EmpCode == null || (x.EmpCode != null && x.EmpCode.Contains(filter.EmpCode)))
                && (filter.EmpName == null || (x.EmpName != null && x.EmpName.Contains(filter.EmpName)))
                && (filter.BranchId == 0 || (x.BranchId == filter.BranchId))
                && ((filter.BranchId != 0) || branchsId.Contains(x.BranchId.ToString()))
                && (filter.ReferenceTypeId == 0 || x.ReferenceTypeId == filter.ReferenceTypeId)
                && (filter.Description == null || (x.Description != null && x.Description.Contains(filter.Description)))
                && (filter.AccAccountName == null || (x.AccAccountName != null && x.AccAccountName.Contains(filter.AccAccountName)))
                && (filter.AccAccountCode == null || (x.AccAccountCode != null && x.AccAccountCode.Equals(filter.AccAccountCode))) 
                        );
                if (items.Succeeded)
                {
                    
                    var res = items.Data.AsQueryable();
                    if (filter.AmountFrom > 0 || filter.AmountTo > 0|| !string.IsNullOrEmpty(filter.ReferenceCode))
                    {
                        var details = await accServiceManager.AccPettyCashDService.GetAllVW(x => x.IsDeleted == false);

                        if (filter.AmountFrom > 0 || filter.AmountTo > 0)
                        {
                            res = res.Where(s =>details.Data.Any(d =>
                                    d.PettyCashId == s.Id && d.Total != null &&d.Total >= filter.AmountFrom &&d.Total <= filter.AmountTo));
                        }

                        if (!string.IsNullOrEmpty(filter.ReferenceCode))
                        {
                            res = res.Where(s => details.Data.Any(d => d.PettyCashId == s.Id && d.ReferenceCode != null && d.ReferenceCode.Contains(filter.ReferenceCode)));
                        }



                    }

                    if (!string.IsNullOrEmpty(filter.StartDate) && !string.IsNullOrEmpty(filter.EndDate))
                    {
                        DateTime startDate = DateTime.ParseExact(filter.StartDate ?? "", "yyyy/MM/dd", CultureInfo.InvariantCulture);
                        DateTime endDate = DateTime.ParseExact(filter.EndDate ?? "", "yyyy/MM/dd", CultureInfo.InvariantCulture);

                        res = res.Where(r => !string.IsNullOrEmpty(r.TDate) && DateTime.ParseExact(r.TDate, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= startDate
                            && DateTime.ParseExact(r.TDate, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= endDate);
                    }

                    var final = res.ToList();
                    foreach (var item in final)
                    {
                        var JournalMaster = await accServiceManager.AccJournalMasterService.GetOneVW(x => x.FlagDelete == false && x.DocTypeId == 34 && x.ReferenceNo == item.Id);
                        if (JournalMaster.Succeeded)
                        {
                            item.JId = JournalMaster.Data.JId;
                           item.JCode = JournalMaster.Data.JCode;
                        }
                    }
                    return Ok(await Result<List<AccPettyCashVw>>.SuccessAsync(final, ""));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccPettyCashVw>.FailAsync($"======= Exp in Search AccPettyCashsVw, MESSAGE: {ex.Message}"));
            }
        }


        #endregion "transactions"

        #region "transactions_ADD"
        [HttpPost("Add")]
        public async Task<IActionResult> Add(AccPettyCashDto obj)
        {
            var chk = await permission.HasPermission(811, PermissionType.Add);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<AccPettyCashDto>.FailAsync(localization.GetMessagesResource("dataRequire")));
                }
                var add = await accServiceManager.AccPettyCashService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccPettyCashDto>.FailAsync($"======= Exp in Acc PettyCash  add: {ex.Message}"));
            }
        }
        [HttpPost("CreateJournal")]
        public async Task<IActionResult> CreateJournal(AccPettyCashDto obj)
        {
            var chk = await permission.HasPermission(811, PermissionType.Add);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<AccPettyCashDto>.FailAsync(localization.GetMessagesResource("dataRequire")));
                }
                if(obj.ReferenceTypeId==3)
                {

                    var add = await accServiceManager.AccPettyCashService.CreateJournal2(obj);
                    return Ok(add);
                }
                else
                {
                    var add = await accServiceManager.AccPettyCashService.CreateJournal(obj);
                    return Ok(add);
                }
                
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccPettyCashDto>.FailAsync($"======= Exp in Acc PettyCash  add: {ex.Message}"));
            }
        }
        #endregion "transactions_Add"

        #region "transactions_Update"
        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(AccPettyCashEditDto obj)
        {
            var chk = await permission.HasPermission(811, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<AccPettyCashEditDto>.FailAsync(localization.GetMessagesResource("dataRequire")));
                }
                var Edit = await accServiceManager.AccPettyCashService.Update(obj);
                if (Edit.Succeeded)
                {
                    return Ok(Edit);
                }
                else
                {
                    return Ok(await Result<AccPettyCashEditDto>.FailAsync(localization.GetResource1("UpdateError")));


                }

            }
            catch (Exception ex)
            {
                return Ok(await Result<AccPettyCashEditDto>.FailAsync($"======= Exp in Acc PettyCash edit: {ex.Message}"));
            }
        }

        #endregion "transactions_Update"

        #region "transactions_Delete"
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long Id)
        {
            var chk = await permission.HasPermission(811, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
               
                var add = await accServiceManager.AccPettyCashService.Remove(Id);
                if (add.Succeeded)
                {
                    return Ok(add);
                }
                else
                {
                    return Ok(await Result<AccPettyCashDto>.FailAsync(localization.GetResource1("DeleteFail")));
                }

            }
            catch (Exception ex)
            {
                return Ok(await Result<AccPettyCashDto>.FailAsync($"======= Exp in Acc PettyCash  Delete: {ex.Message}"));
            }
        }
        #endregion "transactions_Delete"
        #region "transactions_GetById"

        [HttpGet("GetByIdForEdit")]
        public async Task<IActionResult> GetByIdForEdit(long id)
        {
            try
            {
                var chk = await permission.HasPermission(811, PermissionType.Show);
                if (!chk)
                {
                    return Ok(await Result.AccessDenied("AccessDenied"));
                }

                if (id <= 0)
                {
                    return Ok(await Result<AccPettyCashEditDto>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var getItem = await accServiceManager.AccPettyCashService.GetOneVW(x=>x.Id==id);
                if (getItem.Succeeded)
                {
                    var obj = new AccPettyCashEditDto();
                    obj.AccPettyCashVw = getItem.Data;
                    //===========================التفاصيل
                    var getItemD = await accServiceManager.AccPettyCashDService.GetAllVW(x => x.PettyCashId == id);
                    var AccPettyCashDetaileDtolist = new List<AccPettyCashDEditDto>();
                    //================================= رقم القيد
                    var JCode = await accServiceManager.AccJournalMasterService.GetJCodeByReferenceNo(id, 34);
                    obj.AccPettyCashVw.JCode = JCode;
                    //================================ حالة القيد 
                    int ?StatusJournal = await accServiceManager.AccJournalMasterService.GetJournalMasterStatuse(id,34);
                    obj.StatusJournal = StatusJournal ?? 0;
                    foreach (var detail in getItemD.Data)
                    {
                        var AccPettyCashDetaileDto = new AccPettyCashDEditDto
                        {
                            Id = detail.Id,
                            PettyCashId = detail.PettyCashId,
                            ExpenseId = detail.ExpenseId,
                            CcId = detail.CcId,
                            Amount = detail.Amount,
                            VatAmount = detail.VatAmount,
                            SupplierName = detail.SupplierName,
                            SupplierVatNumber = detail.SupplierVatNumber,
                            Total = detail.Total,
                            Description = detail.Description,
                            ReferenceDate = detail.ReferenceDate,
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
                            Vat = detail.Vat,
                            ExpenseName = _session.Language==1 ? detail.ExpenseName : detail.ExpenseName2,
                            CostCenterCode = detail.CostCenterCode,
                            CostCenterName = _session.Language == 1 ? detail.CostCenterName : detail.CostCenterName2,
                        };

                        AccPettyCashDetaileDtolist.Add(AccPettyCashDetaileDto);
                    }
                    obj.DetailsList = AccPettyCashDetaileDtolist;


                    return Ok(await Result<AccPettyCashEditDto>.SuccessAsync(obj, $""));
                }
                else
                {
                    return Ok(getItem);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccPettyCashEditDto>.FailAsync($"====== Exp in GetByIdForEdit Acc PettyCash, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(long id)
        {
            try
            {
                var chk = await permission.HasPermission(811, PermissionType.Show);
                if (!chk)
                {
                    return Ok(await Result.AccessDenied("AcessDenied"));
                }

                if (id <= 0)
                {
                    return Ok(await Result<AccPettyCashDto>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var getItem = await accServiceManager.AccPettyCashService.GetOne(s => s.Id == id && s.IsDeleted == false);
                if (getItem.Succeeded)
                {
                    var obj = getItem.Data;
                    return Ok(await Result<AccPettyCashDto>.SuccessAsync(obj, $""));
                }
                else
                {
                    return Ok(getItem);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccPettyCashDto>.FailAsync($"====== Exp in GetById Acc PettyCash, MESSAGE: {ex.Message}"));
            }
        }
        #endregion "transactions_GetById"

        #region "Reports Acc PettyCash "


        [HttpPost("ReportSearchPettyCash")]
        public async Task<IActionResult> ReportSearchPettyCash(AccPettyCashFilterDto filter)
        {
            var chk = await permission.HasPermission(1716, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var branchsId = _session.Branches.Split(',');
                filter.BranchId ??= 0;
                filter.Code ??= 0; filter.ApplicationCode ??= 0; filter.StatusId ??= 0; filter.TypeId ??= 0; filter.PettyCashType ??= 0; filter.ReferenceTypeId ??= 0; filter.ExpenseId ??= 0;
                var items = await accServiceManager.AccPettyCashService.GetAllVW(x => x.IsDeleted == false && x.FacilityId == _session.FacilityId &&
                (filter.Code == 0 || x.Code == filter.Code)
                && (filter.ApplicationCode == 0 || x.ApplicationCode == filter.ApplicationCode)
                && (filter.StatusId == 0 || x.StatusId == filter.StatusId)
                && (filter.TypeId == 0 || x.TypeId == filter.TypeId)
                && (filter.PettyCashType == 0 || x.PettyCashType == filter.PettyCashType)
                && (filter.EmpCode == null || (x.EmpCode != null && x.EmpCode.Contains(filter.EmpCode)))
                && (filter.EmpName == null || (x.EmpName != null && x.EmpName.Contains(filter.EmpName)))
                && (filter.BranchId == 0 || (x.BranchId == filter.BranchId))
                && ((filter.BranchId != 0) || branchsId.Contains(x.BranchId.ToString()))
                && (filter.ReferenceTypeId == 0 || x.ReferenceTypeId == filter.ReferenceTypeId)
                && (filter.Description == null || (x.Description != null && x.Description.Contains(filter.Description)))
                && (filter.AccAccountName == null || (x.AccAccountName != null && x.AccAccountName.Contains(filter.AccAccountName)))
                && (filter.AccAccountCode == null || (x.AccAccountCode != null && x.AccAccountCode.Equals(filter.AccAccountCode)))
                        );
                if (items.Succeeded)
                {

                    var res = items.Data.AsQueryable();
                    if (filter.AmountFrom > 0 || filter.AmountTo > 0 || !string.IsNullOrEmpty(filter.ReferenceCode) || filter.ExpenseId > 0)
                    {
                        var details = await accServiceManager.AccPettyCashDService.GetAllVW(x => x.IsDeleted == false);

                        if (filter.AmountFrom > 0 || filter.AmountTo > 0)
                        {
                            res = res.Where(s => details.Data.Any(d =>
                                    d.PettyCashId == s.Id && d.Total != null && d.Total >= filter.AmountFrom && d.Total <= filter.AmountTo));
                        }

                        if (!string.IsNullOrEmpty(filter.ReferenceCode))
                        {
                            res = res.Where(s => details.Data.Any(d => d.PettyCashId == s.Id && d.ReferenceCode != null && d.ReferenceCode.Contains(filter.ReferenceCode)));
                        }
                        if (filter.ExpenseId >0)
                        {
                            res = res.Where(s => details.Data.Any(d => d.PettyCashId == s.Id && d.ExpenseId != null && d.ExpenseId==filter.ExpenseId));
                        }


                    }

                    if (!string.IsNullOrEmpty(filter.StartDate) && !string.IsNullOrEmpty(filter.EndDate))
                    {
                        DateTime startDate = DateTime.ParseExact(filter.StartDate ?? "", "yyyy/MM/dd", CultureInfo.InvariantCulture);
                        DateTime endDate = DateTime.ParseExact(filter.EndDate ?? "", "yyyy/MM/dd", CultureInfo.InvariantCulture);

                        res = res.Where(r => !string.IsNullOrEmpty(r.TDate) && DateTime.ParseExact(r.TDate, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= startDate
                            && DateTime.ParseExact(r.TDate, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= endDate);
                    }

                    var final = res.ToList();
                  
                    return Ok(await Result<List<AccPettyCashVw>>.SuccessAsync(final, ""));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccPettyCashVw>.FailAsync($"======= Exp in Search AccPettyCashsVw, MESSAGE: {ex.Message}"));
            }
        }


        #endregion "Reports Acc Petty Cash"

    }
}
