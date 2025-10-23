using Logix.Application.Common;
using Logix.Application.DTOs.ACC;
using Logix.Application.DTOs.Main;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Services;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;
using Logix.Domain.Main;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.Acc
{
    public class AccChequeBookApiController : BaseAccApiController
    {

    private readonly IAccServiceManager accServiceManager;
    private readonly IPermissionHelper permission;
    private readonly IWebHostEnvironment env;
    private readonly IFilesHelper filesHelper;
    private readonly IDDListHelper listHelper;
    private readonly ILocalizationService localization;
    private readonly ICurrentData _session;

    public AccChequeBookApiController(
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
            var chk = await permission.HasPermission(63, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var items = await accServiceManager.AccBankChequeBookService.GetAll(x=>x.IsDeleted==false);
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
        [HttpGet("GetByBankAll")]
        public async Task<IActionResult> GetByBankAll(long BankId)
        {
            var chk = await permission.HasPermission(63, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var items = await accServiceManager.AccBankChequeBookService.GetAll(x=>x.BankId== BankId);
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
        #endregion "transactions"

        #region "transactions_ADD"
        [HttpPost("Add")]
        public async Task<IActionResult> Add(AccBankChequeBookDto obj)
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
                    return Ok(await Result<AccBankChequeBookDto>.FailAsync(localization.GetMessagesResource("dataRequire")));
                }
                var add = await accServiceManager.AccBankChequeBookService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccBankChequeBookDto>.FailAsync($"======= Exp in Acc Bank Cheque Book  add: {ex.Message}"));
            }
        }
        #endregion "transactions_Add"
        #region "transactions_Update"
        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(AccBankChequeBookEditDto obj)
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
                    return Ok(await Result<AccBankChequeBookEditDto>.FailAsync(localization.GetMessagesResource("dataRequire")));
                }

                var Edit = await accServiceManager.AccBankChequeBookService.Update(obj);
                return Ok(Edit);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccBankChequeBookEditDto>.FailAsync($"======= Exp in Acc Bank Cheque Book  edit: {ex.Message}"));
            }
        }

        #endregion "transactions_Update"

        #region "transactions_Delete"

        [HttpPost("Delete")]
        public async Task<IActionResult> Delete(int Id)
        {
            var chk = await permission.HasPermission(63, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var add = await accServiceManager.AccBankChequeBookService.Remove(Id);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccBankChequeBookDto>.FailAsync($"======= Exp in Acc Bank Cheque Book  Delete: {ex.Message}"));
            }
        }
        #endregion "transactions_Delete"

        #region "transactions_GetById"

        [HttpGet("GetByIdForEdit")]
        public async Task<IActionResult> GetByIdForEdit(int id)
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
                    return Ok(await Result<AccBankChequeBookEditDto>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var getItem = await accServiceManager.AccBankChequeBookService.GetForUpdate<AccBankChequeBookEditDto>(id);
                if (getItem.Succeeded)
                {
                    var obj = getItem.Data;



                    return Ok(await Result<AccBankChequeBookEditDto>.SuccessAsync(obj, $""));
                }
                else
                {
                    return Ok(getItem);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccBankChequeBookEditDto>.FailAsync($"====== Exp in GetByIdForEdit Acc Bank Cheque Book, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(int id)
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
                    return Ok(await Result<AccBankChequeBookDto>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var getItem = await accServiceManager.AccBankChequeBookService.GetOne(s => s.Id == id && s.IsDeleted == false);
                if (getItem.Succeeded)
                {
                    var obj = getItem.Data;
                    return Ok(await Result<AccBankChequeBookDto>.SuccessAsync(obj, $""));
                }
                else
                {
                    return Ok(getItem);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccBankChequeBookDto>.FailAsync($"====== Exp in GetById Acc Bank Cheque Book , MESSAGE: {ex.Message}"));
            }
        }
        #endregion "transactions_GetById"
    }
}
