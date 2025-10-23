using Logix.Application.Common;
using Logix.Application.DTOs.ACC;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.Acc
{
    public class AccTypesExpensesController : BaseAccApiController
    {

        private readonly IAccServiceManager accServiceManager;
        private readonly IMainServiceManager mainServiceManager;
        private readonly IPermissionHelper permission;
        private readonly IWebHostEnvironment env;
        private readonly IFilesHelper filesHelper;
        private readonly IDDListHelper listHelper;
        private readonly ILocalizationService localization;
        private readonly ICurrentData _session;

        public AccTypesExpensesController(
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
            var chk = await permission.HasPermission(1715, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var items = await accServiceManager.AccPettyCashExpensesTypeService.GetAllVW(x => x.IsDeleted == false && x.FacilityId == _session.FacilityId);
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    res = res.OrderBy(e => e.Id);
                    return Ok(await Result<List<AccPettyCashExpensesTypeVw>>.SuccessAsync(res.ToList(), ""));

                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccPettyCashExpensesTypeVw>.FailAsync($"======= Exp in Search AccBankVw, MESSAGE: {ex.Message}"));
            }
        }
        [HttpPost("Search")]
        public async Task<IActionResult> Search(AccPettyCashExpensesTypeFilterDto filter)
        {
            var chk = await permission.HasPermission(1715, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var items = await accServiceManager.AccPettyCashExpensesTypeService.GetAllVW(x => x.IsDeleted == false && x.FacilityId == _session.FacilityId
                && (string.IsNullOrEmpty(filter.Name) || (x.Name != null && x.Name.Contains(filter.Name))) &&
              (string.IsNullOrEmpty(filter.Name2) || (x.Name2 != null && x.Name2.Contains(filter.Name2)))

                );
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();

                    var final = res.ToList();
                    return Ok(await Result<List<AccPettyCashExpensesTypeVw>>.SuccessAsync(final, ""));

                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccPettyCashExpensesTypeVw>.FailAsync($"======= Exp in Search AccBankVw, MESSAGE: {ex.Message}"));
            }
        }
        #endregion "transactions"

        #region "transactions_ADD"
        [HttpPost("Add")]
        public async Task<IActionResult> Add(AccPettyCashExpensesTypeDto obj)
        {
            var chk = await permission.HasPermission(1715, PermissionType.Add);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<AccPettyCashExpensesTypeDto>.FailAsync(localization.GetMessagesResource("dataRequire")));
                }

                var add = await accServiceManager.AccPettyCashExpensesTypeService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccPettyCashExpensesTypeDto>.FailAsync($"======= Exp in Acc Acc PettyCash ExpensesType   add: {ex.Message}"));
            }
        }
        #endregion "transactions_Add"

        #region "transactions_Update"

        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(AccPettyCashExpensesTypeEditDto obj)
        {
            var chk = await permission.HasPermission(1715, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<AccPettyCashExpensesTypeEditDto>.FailAsync(localization.GetMessagesResource("dataRequire")));
                }



                var Edit = await accServiceManager.AccPettyCashExpensesTypeService.Update(obj);
                return Ok(Edit);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccPettyCashExpensesTypeEditDto>.FailAsync($"======= Exp in Acc PettyCash ExpensesType  edit: {ex.Message}"));
            }
        }

        #endregion "transactions_Update"

        #region "transactions_Delete"

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long Id)
        {
            var chk = await permission.HasPermission(1715, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var add = await accServiceManager.AccPettyCashExpensesTypeService.Remove(Id);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccPettyCashExpensesTypeDto>.FailAsync($"======= Exp in Acc PettyCash Expenses Type Delete: {ex.Message}"));
            }
        }
        #endregion "transactions_Delete"


        #region "transactions_GetById"

        [HttpGet("GetByIdForEdit")]
        public async Task<IActionResult> GetByIdForEdit(long id)
        {
            try
            {
                var chk = await permission.HasPermission(1715, PermissionType.Show);
                if (!chk)
                {
                    return Ok(await Result.AccessDenied("AccessDenied"));
                }

                if (id <= 0)
                {
                    return Ok(await Result<AccPettyCashExpensesTypeEditDto>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var getItem = await accServiceManager.AccPettyCashExpensesTypeService.GetForUpdate<AccPettyCashExpensesTypeEditDto>(id);
                if (getItem.Succeeded)
                {

                    var obj = new AccPettyCashExpensesTypeEditDto();
                    obj = getItem.Data;
                    var acc = await accServiceManager.AccAccountService.GetOne(x => x.AccAccountId == getItem.Data.AccAccountId);
                    if (acc.Data != null)
                    {
                        obj.AccAccountCode = acc.Data.AccAccountCode;
                        if (_session.Language == 1)
                        {
                            obj.AccAccountName = acc.Data.AccAccountName;
                        }
                        else
                        {
                            obj.AccAccountName = acc.Data.AccAccountName2;
                        }
                    }
                    return Ok(await Result<AccPettyCashExpensesTypeEditDto>.SuccessAsync(obj, $""));
                }
                else
                {
                    return Ok(getItem);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccPettyCashExpensesTypeEditDto>.FailAsync($"====== Exp in GetByIdForEdit  Acc PettyCash ExpensesType , MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(long id)
        {
            try
            {
                var chk = await permission.HasPermission(1715, PermissionType.Show);
                if (!chk)
                {
                    return Ok(await Result.AccessDenied("AcessDenied"));
                }

                if (id <= 0)
                {
                    return Ok(await Result<AccPettyCashExpensesTypeDto>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var getItem = await accServiceManager.AccPettyCashExpensesTypeService.GetOne(s => s.Id == id && s.IsDeleted == false);
                if (getItem.Succeeded)
                {
                    var obj = getItem.Data;
                    return Ok(await Result<AccPettyCashExpensesTypeDto>.SuccessAsync(obj, $""));
                }
                else
                {
                    return Ok(getItem);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccPettyCashExpensesTypeDto>.FailAsync($"====== Exp in GetById Acc PettyCash ExpensesType, MESSAGE: {ex.Message}"));
            }
        }
        #endregion "transactions_GetById"
    }
}