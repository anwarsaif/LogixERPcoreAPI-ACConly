using Logix.Application.Common;
using Logix.Application.DTOs.ACC;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Logix.MVC.LogixAPIs.Acc
{
    public class AccJournalReverseApiController : BaseAccApiController
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

        public AccJournalReverseApiController(
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
            var chk = await permission.HasPermission(814, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var items = await accServiceManager.AccJournalMasterService.GetAllVW(x => x.FlagDelete == false && x.FacilityId == _session.FacilityId && x.DocTypeId == 35 && x.FinYear == _session.FinYear);
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
            var chk = await permission.HasPermission(814, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                long fromParsed, toParsed;
                filter.PaymentTypeId ??= 0;
                filter.StatusId ??= 0;
                filter.PeriodId ??= 0;
                filter.CreatedBy ??= 0;
                filter.BranchId ??= 0;
                filter.Debit ??= 0;
                filter.Credit ??= 0;
                var branchsId = _session.Branches.Split(',');
                filter.BranchId ??= 0;
                var items = await accServiceManager.AccJournalMasterService.GetAllVW(x =>
    x.FacilityId == _session.FacilityId && x.FlagDelete == false &&
    x.DocTypeId == 35 &&
               x.FinYear == _session.FinYear &&
    (filter.InsertUserId > 0 ? x.InsertUserId.Equals(filter.InsertUserId) : true) &&
    (filter.PeriodId > 0 ? x.PeriodId.Equals(filter.PeriodId) : true) &&
    (string.IsNullOrEmpty(filter.JCode) || (x.JCode != null && x.JCode.CompareTo(filter.JCode) >= 0 && x.JCode.CompareTo(filter.JCode2) <= 0))
    && (filter.BranchId == 0 || (x.BranchId == filter.BranchId))
       && ((filter.BranchId != 0) || branchsId.Contains(x.BranchId.ToString())) 
                    && (filter.PaymentTypeId == 0 || (x.PaymentTypeId == filter.PaymentTypeId))
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
        public async Task<IActionResult> Add(AccJournalReverseDtoVW obj)
        {
            var chk = await permission.HasPermission(814, PermissionType.Add);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<AccJournalReverseDto>.FailAsync(localization.GetMessagesResource("dataRequire")));
                }
                var add = await accServiceManager.AccJournalMasterService.AddJournalReverse(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccJournalReverseDto>.FailAsync($"======= Exp in Acc Journal add: {ex.Message}"));
            }
        }
        #endregion "transactions_Add"

        #region "transactions_Update"

        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(AccExpensesMasterEditDtoVW obj)
        {
            var chk = await permission.HasPermission(814, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<AccJournalReverseEditDto>.FailAsync(localization.GetMessagesResource("dataRequire")));
                }
                var Edit = await accServiceManager.AccJournalMasterService.UpdateJournalReverse(obj);
                return Ok(Edit);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccJournalReverseEditDto>.FailAsync($"======= Exp in Acc Journal  edit: {ex.Message}"));
            }
        }

        #endregion "transactions_Update"

        #region "transactions_Delete"
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long Id)
        {
            var chk = await permission.HasPermission(814, PermissionType.Delete);
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
                var chk = await permission.HasPermission(814, PermissionType.Show);
                if (!chk)
                {
                    return Ok(await Result.AccessDenied("AccessDenied"));
                }

                if (id <= 0)
                {
                    return Ok(await Result<AccJournalMasterDto>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var getItem = await accServiceManager.AccJournalMasterService.GetOneVW(x => x.JId == id);
                if (getItem.Succeeded)
                {

                    var AccJournalMasterDto = new AccIncomeDto
                    {
                        JId = getItem.Data.JId,
                        JCode = getItem.Data.JCode,
                        PeriodId = getItem.Data.PeriodId,
                        ReferenceNo = getItem.Data.ReferenceNo,
                        CurrencyId = getItem.Data.CurrencyId,
                        ExchangeRate = getItem.Data.ExchangeRate,
                        Amount = getItem.Data.Amount,
                        AmountWrite = getItem.Data.AmountWrite,
                        JDateGregorian = getItem.Data.JDateGregorian,
                        CcId = getItem.Data.CcId,
                        CollectionEmpCode = getItem.Data.CollectionEmpCode,
                        ReferenceCode = getItem.Data.ReferenceCode,
                        PaymentTypeId = getItem.Data.PaymentTypeId,
                        JBian = getItem.Data.JBian,
                        ChequNo = getItem.Data.ChequNo,
                        ChequDateHijri = getItem.Data.ChequDateHijri,
                        BankId = getItem.Data.BankId,
                        JDescription = getItem.Data.JDescription,
                        StatusId = getItem.Data.StatusId,
                        DocTypeId= getItem.Data.DocTypeId,
                    };
                    var obj = new AccIncomeMasterDtoVW
                    {
                        AccJournalMasterDto = AccJournalMasterDto,

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




                    return Ok(await Result<AccIncomeMasterDtoVW>.SuccessAsync(obj, $""));
                }
                else
                {
                    return Ok(getItem);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccIncomeMasterDtoVW>.FailAsync($"====== Exp in GetByIdForEdit Acc Journal, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(long id)
        {
            try
            {
                var chk = await permission.HasPermission(814, PermissionType.Show);
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


        [HttpGet("GetByReferenceId")]
        public async Task<IActionResult> GetByReferenceId(long id)
        {
            try
            {
                var chk = await permission.HasPermission(814, PermissionType.Show);
                if (!chk)
                {
                    return Ok(await Result.AccessDenied("AccessDenied"));
                }

                if (id <= 0)
                {
                    return Ok(await Result<AccJournalMasterDto>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var getItem = await accServiceManager.AccJournalMasterService.GetOneVW(x => x.JId == id);
                if (getItem.Succeeded)
                {

                    var AccJournalMasterDto = new AccIncomeDto
                    {
                        JId = getItem.Data.JId,
                        JCode = getItem.Data.JCode,
                        PeriodId = getItem.Data.PeriodId,
                        ReferenceNo = getItem.Data.ReferenceNo,
                        CurrencyId = getItem.Data.CurrencyId,
                        ExchangeRate = getItem.Data.ExchangeRate,
                        Amount = getItem.Data.Amount,
                        AmountWrite = getItem.Data.AmountWrite,
                        JDateGregorian = getItem.Data.JDateGregorian,
                        CcId = getItem.Data.CcId,
                        CollectionEmpCode = getItem.Data.CollectionEmpCode,
                        ReferenceCode = getItem.Data.ReferenceCode,
                        PaymentTypeId = getItem.Data.PaymentTypeId,
                        JBian = getItem.Data.JBian,
                        ChequNo = getItem.Data.ChequNo,
                        ChequDateHijri = getItem.Data.ChequDateHijri,
                        BankId = getItem.Data.BankId,
                        JDescription = getItem.Data.JDescription,
                        StatusId = getItem.Data.StatusId,
                        DocTypeId = getItem.Data.DocTypeId,
                    };
                    var obj = new AccIncomeMasterDtoVW
                    {
                        AccJournalMasterDto = AccJournalMasterDto,

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
                                Debit = detail.Credit,
                                Credit = detail.Debit,
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




                    return Ok(await Result<AccIncomeMasterDtoVW>.SuccessAsync(obj, $""));
                }
                else
                {
                    return Ok(getItem);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccIncomeMasterDtoVW>.FailAsync($"====== Exp in GetByIdForEdit Acc Journal, MESSAGE: {ex.Message}"));
            }
        }


        #endregion "transactions_GetById"


    }
}
