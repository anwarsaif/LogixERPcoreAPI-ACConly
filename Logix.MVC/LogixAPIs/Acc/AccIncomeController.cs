using Logix.Application.Common;
using Logix.Application.DTOs.ACC;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Operations;
using System.Globalization;

namespace Logix.MVC.LogixAPIs.Acc
{
    public class AccIncomeController : BaseAccApiController
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

        public AccIncomeController(
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
            var chk = await permission.HasPermission(72, PermissionType.Show);
            var chk2 = await permission.HasPermission(462, PermissionType.Show);
            var chk3 = await permission.HasPermission(248, PermissionType.Show);
            var chk4 = await permission.HasPermission(503, PermissionType.Show);
            if (!chk && !chk2 && !chk3 && !chk4)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var items = await accServiceManager.AccJournalMasterService.GetAllVW(x => x.FlagDelete == false && x.FacilityId == _session.FacilityId && x.DocTypeId == 1 && x.FinYear == _session.FinYear);
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
            var chk = await permission.HasPermission(72, PermissionType.Show);
            var chk2 = await permission.HasPermission(462, PermissionType.Show);
            var chk3 = await permission.HasPermission(248, PermissionType.Show);
            var chk4 = await permission.HasPermission(503, PermissionType.Show);
            if (!chk && !chk2 && !chk3 && !chk4)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                filter.PaymentTypeId ??= 0;
                filter.StatusId ??= 0;
                filter.InsertUserId ??= 0;
                filter.BranchId ??= 0;
                filter.PeriodId ??= 0;

                long fromParsed, toParsed;
                var branchsId = _session.Branches.Split(',');
                var items = await accServiceManager.AccJournalMasterService.GetAllVW(x => x.FlagDelete == false
                   && x.FacilityId == _session.FacilityId && x.DocTypeId == 1 && x.FinYear == _session.FinYear
                && (filter.InsertUserId == 0 || x.InsertUserId == filter.InsertUserId)
                && (filter.PeriodId == 0 || x.PeriodId == filter.PeriodId)
                && (filter.Credit == 0 || x.Amount == filter.Credit)
                && (string.IsNullOrEmpty(filter.ReferenceCode) || (x.ReferenceCode != null && x.ReferenceCode.Contains(filter.ReferenceCode)))
                 && ((filter.BranchId !=0 ? x.BranchId == filter.BranchId : branchsId.Contains(x.BranchId.ToString())))
                 && (string.IsNullOrEmpty(filter.CollectionEmpCode) || (x.CollectionEmpCode != null && x.CollectionEmpCode.Contains(filter.CollectionEmpCode)))
                 && (filter.StatusId == 0 || (x.StatusId == filter.StatusId))
                 && (filter.PaymentTypeId == 0 || (x.PaymentTypeId == filter.PaymentTypeId))
                 && (string.IsNullOrEmpty(filter.ReferenceNoFrom) ||
                (x.ReferenceNo != null && long.TryParse(filter.ReferenceNoFrom, out fromParsed) && long.TryParse(filter.ReferenceNoTo, out toParsed) &&
                 x.ReferenceNo >= fromParsed && x.ReferenceNo <= toParsed))
                );
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    if (filter == null)
                    {
                        return Ok(items.Data.Where(x => x.FlagDelete == false));
                    }
                    if (!string.IsNullOrEmpty(filter.CostCenterCode)  || !string.IsNullOrEmpty(filter.AccountCode) || !string.IsNullOrEmpty(filter.AccountName) || !string.IsNullOrEmpty(filter.Description))
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
                        res = res.Where(s => string.IsNullOrEmpty(s.JDateGregorian) || DateTime.ParseExact(s.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= startDate);
                    }

                    if (!string.IsNullOrEmpty(filter.JDateGregorian2))
                    {
                        DateTime endDate = DateTime.ParseExact(filter.JDateGregorian2, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                        res = res.Where(s => string.IsNullOrEmpty(s.JDateGregorian) || DateTime.ParseExact(s.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= endDate);
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


        [HttpGet("GetBookSerial")]
        public async Task<IActionResult> GetBookSerial(long branchId)
        {
            var ReferenceCode = await accServiceManager.AccJournalMasterService.GetBookSerial(_session.FacilityId, branchId, 1);


            return Ok(await Result<int>.SuccessAsync(ReferenceCode));
        }
        #endregion "transactions"

        #region "transactions_ADD"
        [HttpPost("Add")]
        public async Task<IActionResult> Add(AccIncomeDto obj)
        {
            var chk = await permission.HasPermission(72, PermissionType.Add);
            var chk2 = await permission.HasPermission(462, PermissionType.Add);
            var chk3 = await permission.HasPermission(248, PermissionType.Add);
            var chk4 = await permission.HasPermission(503, PermissionType.Add);
            if (!chk && !chk2 && !chk3 && !chk4)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<AccIncomeDto>.FailAsync(localization.GetMessagesResource("dataRequire")));
                }
                var add = await accServiceManager.AccJournalMasterService.AddIncome(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccIncomeDto>.FailAsync($"======= Exp in Acc Journal add: {ex.Message}"));
            }
        }
        #endregion "transactions_Add"

        #region "transactions_Update"

        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(AccIncomeMasterEditDtoVW obj)
        {
            var chk = await permission.HasPermission(72, PermissionType.Edit);
            var chk2 = await permission.HasPermission(462, PermissionType.Edit);
            var chk3 = await permission.HasPermission(248, PermissionType.Edit);
            var chk4 = await permission.HasPermission(503, PermissionType.Edit);
            if (!chk && !chk2 && !chk3 && !chk4)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<AccIncomeMasterEditDtoVW>.FailAsync(localization.GetMessagesResource("dataRequire")));
                }
                obj.AccJournalMasterDto.DocTypeId = 1;
                var Edit = await accServiceManager.AccJournalMasterService.UpdateIncome(obj);
                return Ok(Edit);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccIncomeMasterEditDtoVW>.FailAsync($"======= Exp in Acc Journal  edit: {ex.Message}"));
            }
        }

        #endregion "transactions_Update"

        #region "transactions_Delete"
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long Id)
        {
            var chk = await permission.HasPermission(72, PermissionType.Delete);
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
                var chk = await permission.HasPermission(72, PermissionType.Show);
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
                    var DetailesDebitor = await accServiceManager.AccJournalDetaileService.SelectACCJournalDetailesDebitor(id);

                    var AccJournalMasterDto = new AccIncomeDto
                    {
                        JId = getItem.Data.JId,
                        JCode = getItem.Data.JCode,
                        PeriodId = getItem.Data.PeriodId,
                        ReferenceNo = getItem.Data.ReferenceNo,
                        CurrencyId = getItem.Data.CurrencyId,
                        ExchangeRate = getItem.Data.ExchangeRate,
                        Amount = DetailesDebitor.Data.Amount,
                        AmountWrite = DetailesDebitor.Data.AmountWrite,
                        JDateGregorian = getItem.Data.JDateGregorian,
                        CcId = getItem.Data.CcId,
                        CollectionEmpCode = getItem.Data.CollectionEmpCode,
                        ReferenceCode = getItem.Data.ReferenceCode,
                        PaymentTypeId = getItem.Data.PaymentTypeId,
                        cashOnhandId = DetailesDebitor.Data.AccAccountId,
                        JBian = getItem.Data.JBian,
                        ChequNo = getItem.Data.ChequNo,
                        ChequDateHijri = getItem.Data.ChequDateHijri,
                        BankId = getItem.Data.BankId,
                        JDescription = getItem.Data.JDescription,
                        StatusId = getItem.Data.StatusId,
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
                var chk = await permission.HasPermission(72, PermissionType.Show);
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


        #region "Reports Income "

        [HttpPost("RepIncomeSearch")]

        public async Task<IActionResult> RepIncomeSearch(AccJournalMasterfilterDto filter)
        {
            var chk = await permission.HasPermission(76, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                //filter.PaymentTypeId ??= 0;
                //filter.StatusId ??= 0;
                //filter.InsertUserId ??= 0;
                //filter.BranchId ??= 0;
                //filter.PeriodId ??= 0;

                //long fromParsed, toParsed;
                //var branchsId = _session.Branches.Split(',');
                //var items = await accServiceManager.AccJournalMasterService.GetAllVW(x => x.FlagDelete == false
                //   && x.FacilityId == _session.FacilityId && x.DocTypeId == 1 && x.FinYear == _session.FinYear
                //&& (filter.InsertUserId == 0 || x.InsertUserId==filter.InsertUserId)
                //&& (filter.PeriodId == 0 || x.PeriodId==filter.PeriodId)
                //&& (filter.Credit == 0 || x.Amount == filter.Credit)
                //&& (string.IsNullOrEmpty(filter.ReferenceCode) || (x.ReferenceCode != null && x.ReferenceCode.Contains(filter.ReferenceCode)))
                // && ((filter.BranchId !=0 ? x.BranchId == filter.BranchId : branchsId.Contains(x.BranchId.ToString())))

                // && (string.IsNullOrEmpty(filter.CollectionEmpCode) || (x.CollectionEmpCode != null && x.CollectionEmpCode.Contains(filter.CollectionEmpCode)))
                // && (filter.StatusId == 0 || (x.StatusId == filter.StatusId))
                // && (filter.PaymentTypeId == 0 || (x.PaymentTypeId == filter.PaymentTypeId))
                // && (string.IsNullOrEmpty(filter.ReferenceNoFrom) ||
                //(x.ReferenceNo != null &&long.TryParse(filter.ReferenceNoFrom, out fromParsed) && long.TryParse(filter.ReferenceNoTo, out toParsed) &&
                // x.ReferenceNo >= fromParsed && x.ReferenceNo <= toParsed))
                //);
                //if (items.Succeeded)
                //{
                //    var res = items.Data.AsQueryable();

                //    if (!string.IsNullOrEmpty(filter.CostCenterCode) || !string.IsNullOrEmpty(filter.AccountCode) || !string.IsNullOrEmpty(filter.AccountName) || !string.IsNullOrEmpty(filter.Description))
                //    {
                //        var details = await accServiceManager.AccJournalDetaileService.GetAllVW(x => x.FlagDelete == false);
                //        if (!string.IsNullOrEmpty(filter.CostCenterCode))
                //        {

                //            res = res.Where(s => details.Data.Any(d => d.JId == s.JId && d.CostCenterCode != null && d.CostCenterCode == filter.CostCenterCode));

                //        }
                //        if (!string.IsNullOrEmpty(filter.CostcenterName))
                //        {
                //            res = res.Where(s => details.Data.Any(d => d.JId == s.JId && d.CostCenterName != null && d.CostCenterName.Contains(filter.CostcenterName)));

                //        }


                //        if (!string.IsNullOrEmpty(filter.AccountCode))
                //        {
                //            res = res.Where(s => details.Data.Any(d => d.JId == s.JId && d.AccAccountCode != null && d.AccAccountCode == filter.AccountCode));

                //        }
                //        if (!string.IsNullOrEmpty(filter.AccountName))
                //        {
                //            res = res.Where(s => details.Data.Any(d => d.JId == s.JId && d.AccAccountName != null && d.AccAccountName.Contains(filter.AccountName)));

                //        }

                //        if (!string.IsNullOrEmpty(filter.Description))
                //        {
                //            res = res.Where(s => details.Data.Any(d => d.JId == s.JId && d.Description.IndexOf(filter.Description, StringComparison.OrdinalIgnoreCase) >= 0));
                //        }

                //    }

                //    if (!string.IsNullOrEmpty(filter.JDateGregorian))
                //    {
                //        DateTime startDate = DateTime.ParseExact(filter.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                //        res = res.Where(s => string.IsNullOrEmpty(s.JDateGregorian) || DateTime.ParseExact(s.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= startDate);
                //    }

                //    if (!string.IsNullOrEmpty(filter.JDateGregorian2))
                //    {
                //        DateTime endDate = DateTime.ParseExact(filter.JDateGregorian2, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                //        res = res.Where(s => string.IsNullOrEmpty(s.JDateGregorian) || DateTime.ParseExact(s.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= endDate);
                //    }





                //    var final = res.ToList();
                //    return Ok(await Result<List<AccJournalMasterVw>>.SuccessAsync(final, ""));
                //}
                var items = await accServiceManager.AccJournalMasterService.RepIncomeSearch(filter);
				return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccJournalMasterVw>.FailAsync($"======= Exp in Search AccJournalMasterVw, MESSAGE: {ex.Message}"));
            }
        }

        #endregion "Reports Income "

        #region "Reports Income Details"



        [HttpPost("RepIncomeDetailsSearch")]
        public async Task<IActionResult> RepIncomeDetailsSearch(AccJournalDetailefilterDto filter)
        {
            var chk = await permission.HasPermission(1747, PermissionType.Show);

            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {     

                filter.PaymentTypeId ??= 0;
                filter.StatusId ??= 0;
                filter.BranchId ??= 0;
                filter.PeriodId ??= 0;
                filter.ReferenceTypeId ??= 0;
                filter.Amount ??= 0;
                long fromParsed, toParsed;
                var branchsId = _session.Branches.Split(',');
                var items = await accServiceManager.AccJournalDetaileService.GetAllVW(x => x.MFlagDelete == false
                   && x.FacilityId == _session.FacilityId && x.DocTypeId == 1 && x.FinYear == _session.FinYear
                     && (string.IsNullOrEmpty(filter.CostCenterCode) || (x.CostCenterCode != null && x.CostCenterCode.Contains(filter.CostCenterCode)))
                     && (filter.ReferenceTypeId == 0 || x.ReferenceTypeId == filter.ReferenceTypeId)
                     && (string.IsNullOrEmpty(filter.CollectionEmpCode) || (x.CollectionEmpCode != null && x.CollectionEmpCode.Contains(filter.CollectionEmpCode))) 
                     && (filter.PeriodId == 0 || x.PeriodId == filter.PeriodId)
                     && (filter.Amount == 0 || x.Amount == filter.Amount)
                     && (string.IsNullOrEmpty(filter.AccountCode) || (x.AccAccountCode != null && x.AccAccountCode.Contains(filter.AccountCode)))
                     && (string.IsNullOrEmpty(filter.Description) || (x.Description != null && x.Description.Contains(filter.Description)))
                     //&& (filter.BranchId !=0 ? x.MBranchId == filter.BranchId : branchsId.Contains(x.BranchId.ToString()))
                     && (filter.StatusId == 0 || (x.MStatusId == filter.StatusId))
                     && (filter.PaymentTypeId == 0 || (x.PaymentTypeId == filter.PaymentTypeId))
                     && (string.IsNullOrEmpty(filter.ReferenceNoFrom) ||
                         (x.MReferenceNo != null && long.TryParse(filter.ReferenceNoFrom, out fromParsed) && long.TryParse(filter.ReferenceNoTo, out toParsed) &&
                       x.MReferenceNo >= fromParsed && x.MReferenceNo <= toParsed))

                     );

                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                  

                    if (!string.IsNullOrEmpty(filter.JDateGregorian))
                    {
                        DateTime startDate = DateTime.ParseExact(filter.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                        res = res.Where(s => string.IsNullOrEmpty(s.JDateGregorian) || DateTime.ParseExact(s.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= startDate);
                    }

                    if (!string.IsNullOrEmpty(filter.JDateGregorian2))
                    {
                        DateTime endDate = DateTime.ParseExact(filter.JDateGregorian2, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                        res = res.Where(s => string.IsNullOrEmpty(s.JDateGregorian) || DateTime.ParseExact(s.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= endDate);
                    }

                    var final = res.ToList();
                    return Ok(await Result<List<AccJournalDetailesVw>>.SuccessAsync(final, ""));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccJournalDetailesVw>.FailAsync($"======= Exp in Search Acc Journal Detailes Vw, MESSAGE: {ex.Message}"));
            }
        }
        #endregion "Reports Income Details""



    }
}
