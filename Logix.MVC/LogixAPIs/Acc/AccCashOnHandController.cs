using Logix.Application.Common;
using Logix.Application.DTOs.ACC;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;

using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.Acc
{
    public class AccCashOnHandController : BaseAccApiController
    {

        private readonly IAccServiceManager accServiceManager;
        private readonly IPermissionHelper permission;
        private readonly IWebHostEnvironment env;
        private readonly IFilesHelper filesHelper;
        private readonly IDDListHelper listHelper;
        private readonly ILocalizationService localization;
        private readonly ICurrentData _session;

        public AccCashOnHandController(
            IAccServiceManager accServiceManager,
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
            var chk = await permission.HasPermission(332, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var items = await accServiceManager.AccCashOnHandService.GetAll(x => x.IsDeleted == false && x.FacilityId == _session.FacilityId);
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
        public async Task<IActionResult> Search(AccCashOnHandFilterDto filter)
        {
            var chk = await permission.HasPermission(332, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var branchsId = _session.Branches.Split(',');
                filter.BranchId ??= 0;
                var items = await accServiceManager.AccCashOnHandService.GetAll(x => x.IsDeleted == false && x.FacilityId == _session.FacilityId &&
             (string.IsNullOrEmpty(filter.Name) || (x.Name != null && x.Name.Contains(filter.Name))) &&
              (string.IsNullOrEmpty(filter.Name2) || (x.Name2 != null && x.Name2.Contains(filter.Name2))) &&
               (filter.BranchId == 0 || (x.BranchId == filter.BranchId))
                && ((filter.BranchId != 0) || branchsId.Contains(x.BranchId.ToString()))
            );
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();

                    var final = res.ToList();
                    return Ok(await Result<List<AccCashOnHandDto>>.SuccessAsync(final, ""));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccCashOnHandDto>.FailAsync($"======= Exp in Search AccCashOnHand, MESSAGE: {ex.Message}"));
            }
        }

        #endregion "transactions"

        #region "transactions_ADD"
        [HttpPost("Add")]
        public async Task<IActionResult> Add(AccCashOnHandDto obj)
        {
            var chk = await permission.HasPermission(332, PermissionType.Add);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<AccCashOnHandDto>.FailAsync(localization.GetMessagesResource("dataRequire")));
                }
                obj.FacilityId = _session.FacilityId;
                var add = await accServiceManager.AccCashOnHandService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccCashOnHandDto>.FailAsync($"======= Exp in Acc CashOnHand  add: {ex.Message}"));
            }
        }
        #endregion "transactions_Add"


        #region "transactions_Update"
        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(AccCashOnHandEditDto obj)
        {
            var chk = await permission.HasPermission(332, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<AccCashOnHandEditDto>.FailAsync(localization.GetMessagesResource("dataRequire")));
                }

                var Edit = await accServiceManager.AccCashOnHandService.Update(obj);
                return Ok(Edit);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccCashOnHandEditDto>.FailAsync($"======= Exp in Acc CashOnHand  edit: {ex.Message}"));
            }
        }

        #endregion "transactions_Update"

        #region "transactions_Delete"
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long Id)
        {
            var chk = await permission.HasPermission(332, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var add = await accServiceManager.AccCashOnHandService.Remove(Id);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccCashOnHandDto>.FailAsync($"======= Exp in Acc Bank  Delete: {ex.Message}"));
            }
        }
        #endregion "transactions_Delete"

        #region "transactions_GetById"

        [HttpGet("GetByIdForEdit")]
        public async Task<IActionResult> GetByIdForEdit(long id)
        {
            try
            {
                var chk = await permission.HasPermission(332, PermissionType.Show);
                if (!chk)
                {
                    return Ok(await Result.AccessDenied("AccessDenied"));
                }

                if (id <= 0)
                {
                    return Ok(await Result<AccCashOnHandEditDto>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var getItem = await accServiceManager.AccCashOnHandService.GetForUpdate<AccCashOnHandEditDto>(id);
                if (getItem.Succeeded)
                {

                    var acc = await accServiceManager.AccAccountService.GetOneVW(x => x.AccAccountId == getItem.Data.AccAccountId);

                    var obj = new AccCashOnHandEditDto();
                    obj = getItem.Data;

                    if (acc.Data != null)
                    {
                        obj.AccAccountCode = acc.Data.AccAccountCode;
                        if (_session.Language == 1)
                        {
                            obj.AccountName = acc.Data.AccAccountName;
                        }
                        else
                        {
                            obj.AccountName = acc.Data.AccAccountName2;
                        }
                    }
                    var accParent = await accServiceManager.AccAccountService.GetOneVW(x => x.AccAccountId == getItem.Data.AccAccountParentID);

                    if (accParent.Data != null)
                    {

                        obj.AccountParentCode = accParent.Data.AccAccountCode;

                        if (_session.Language == 1)
                        {
                            obj.AccountParentName = accParent.Data.AccAccountName;
                        }
                        else
                        {
                            obj.AccountParentName = accParent.Data.AccAccountName;
                        }
                    }
                    return Ok(await Result<AccCashOnHandEditDto>.SuccessAsync(obj, $""));
                }
                else
                {
                    return Ok(getItem);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccCashOnHandEditDto>.FailAsync($"====== Exp in GetByIdForEdit Acc CashOnHand, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(long id)
        {
            try
            {
                var chk = await permission.HasPermission(332, PermissionType.Show);
                if (!chk)
                {
                    return Ok(await Result.AccessDenied("AcessDenied"));
                }

                if (id <= 0)
                {
                    return Ok(await Result<AccCashOnHandDto>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var getItem = await accServiceManager.AccCashOnHandService.GetOne(s => s.Id == id && s.IsDeleted == false);
                if (getItem.Succeeded)
                {
                    var obj = getItem.Data;
                    return Ok(await Result<AccCashOnHandDto>.SuccessAsync(obj, $""));
                }
                else
                {
                    return Ok(getItem);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccCashOnHandDto>.FailAsync($"====== Exp in GetById Acc CashOnHand, MESSAGE: {ex.Message}"));
            }
        }
        #endregion "transactions_GetById"

    }
}