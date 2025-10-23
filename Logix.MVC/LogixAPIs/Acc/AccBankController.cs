using Logix.Application.Common;
using Logix.Application.DTOs.ACC;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.Acc
{
    public class AccBankController : BaseAccApiController
    {

        private readonly IAccServiceManager accServiceManager;
        private readonly IMainServiceManager mainServiceManager;
        private readonly IPermissionHelper permission;
        private readonly IWebHostEnvironment env;
        private readonly IFilesHelper filesHelper;
        private readonly IDDListHelper listHelper;
        private readonly ILocalizationService localization;
        private readonly ICurrentData _session;

        public AccBankController(
            IAccServiceManager accServiceManager,
              IMainServiceManager mainServiceManager,
            IPermissionHelper permission,
             IWebHostEnvironment env,
            IHttpContextAccessor httpContextAccessor,
            IFilesHelper filesHelper,
            IDDListHelper listHelper,
             ILocalizationService localization
            , ICurrentData session
            )
        {
            this.accServiceManager = accServiceManager;
            this.mainServiceManager = mainServiceManager;
            this.permission = permission;
            this.env = env;
            this.filesHelper = filesHelper;
            this.listHelper = listHelper;
            this.localization = localization;
            this._session = session;
        }



        #region "transactions"



        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var chk = await permission.HasPermission(63, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var items = await accServiceManager.AccBankService.GetAllVW(x => x.FlagDelete == false && x.FacilityId == _session.FacilityId);
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    res = res.OrderBy(e => e.BankId);
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
        public async Task<IActionResult> Search(AccBankFilterDto filter)
        {
            var chk = await permission.HasPermission(63, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {

                var branchsId = _session.Branches.Split(',');
                filter.BranchId ??= 0;
                filter.Bank ??= 0;
                filter.StatusId ??= 0;
                var items = await accServiceManager.AccBankService.GetAllVW(x => x.FlagDelete == false && x.FacilityId == _session.FacilityId &&
             (filter.Bank == 0 || x.Bank == filter.Bank) &&
             (string.IsNullOrEmpty(filter.BankName) || (x.BankName != null && x.BankName.Contains(filter.BankName))) &&
              (string.IsNullOrEmpty(filter.BankName2) || (x.BankName2 != null && x.BankName2.Contains(filter.BankName2))) &&
              (string.IsNullOrEmpty(filter.BankAccountNo) || (x.BankAccountNo != null && x.BankAccountNo.Contains(filter.BankAccountNo))) &&
               (string.IsNullOrEmpty(filter.Iban) || (x.Iban != null && x.Iban.Contains(filter.Iban)))
           && (filter.StatusId == 0 || x.StatusId == filter.StatusId)

             && (filter.BranchId == 0 || (x.BranchId == filter.BranchId))
                        && ((filter.BranchId != 0) || branchsId.Contains(x.BranchId.ToString()))
            );
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();

                    var final = res.ToList();
                    return Ok(await Result<List<AccBankVw>>.SuccessAsync(final, ""));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccBankVw>.FailAsync($"======= Exp in Search AccBankVw, MESSAGE: {ex.Message}"));
            }
        }
        #endregion "transactions"

        #region "transactions_ADD"
        [HttpPost("Add")]
        public async Task<IActionResult> Add(AccBankDto obj)
        {
            var chk = await permission.HasPermission(63, PermissionType.Add);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<AccBankDto>.FailAsync(localization.GetMessagesResource("dataRequire")));
                }

                var add = await accServiceManager.AccBankService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccBankDto>.FailAsync($"======= Exp in Acc Group  add: {ex.Message}"));
            }
        }
        #endregion "transactions_Add"

        #region "transactions_Update"
        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(AccBankEditDto obj)
        {
            var chk = await permission.HasPermission(63, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<AccBankEditDto>.FailAsync(localization.GetMessagesResource("dataRequire")));
                }

                var Edit = await accServiceManager.AccBankService.Update(obj);
                return Ok(Edit);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccBankEditDto>.FailAsync($"======= Exp in Acc AccBank  edit: {ex.Message}"));
            }
        }

        #endregion "transactions_Update"

        #region "transactions_Delete"
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long Id)
        {
            var chk = await permission.HasPermission(63, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var add = await accServiceManager.AccBankService.Remove(Id);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccBankDto>.FailAsync($"======= Exp in Acc Bank  Delete: {ex.Message}"));
            }
        }
        #endregion "transactions_Delete"

        #region "transactions_GetById"

        [HttpGet("GetByIdForEdit")]
        public async Task<IActionResult> GetByIdForEdit(long id)
        {
            try
            {
                var chk = await permission.HasPermission(63, PermissionType.Show);
                if (!chk)
                {
                    return Ok(await Result.AccessDenied("AccessDenied"));
                }

                if (id <= 0)
                {
                    return Ok(await Result<AccBankEditDto>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var getItem = await accServiceManager.AccBankService.GetForUpdate<AccBankEditDto>(id);
                if (getItem.Succeeded)
                {


                    var acc = await accServiceManager.AccAccountService.GetOne(x => x.AccAccountId == getItem.Data.AccAccountId);

                    var obj = new AccBankEditDto();
                    obj = getItem.Data;
                    if (acc.Data != null)
                    {
                        obj.AccountCode = acc.Data.AccAccountCode;
                        if (_session.Language == 1)
                        {
                            obj.AccountName = acc.Data.AccAccountName;
                        }
                        else
                        {
                            obj.AccountName = acc.Data.AccAccountName2;
                        }
                    }


                    return Ok(await Result<AccBankEditDto>.SuccessAsync(obj, $""));
                }
                else
                {
                    return Ok(getItem);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccBankEditDto>.FailAsync($"====== Exp in GetByIdForEdit Acc Bank, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(long id)
        {
            try
            {
                var chk = await permission.HasPermission(63, PermissionType.Show);
                if (!chk)
                {
                    return Ok(await Result.AccessDenied("AcessDenied"));
                }

                if (id <= 0)
                {
                    return Ok(await Result<AccBankDto>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var getItem = await accServiceManager.AccBankService.GetOne(s => s.BankId == id && s.IsDeleted == false);
                if (getItem.Succeeded)
                {
                    var obj = getItem.Data;
                    return Ok(await Result<AccBankDto>.SuccessAsync(obj, $""));
                }
                else
                {
                    return Ok(getItem);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccBankDto>.FailAsync($"====== Exp in GetById Acc Bank, MESSAGE: {ex.Message}"));
            }
        }




        #endregion "transactions_GetById"

    }
}