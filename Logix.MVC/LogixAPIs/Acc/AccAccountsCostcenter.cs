using Logix.Application.Common;
using Logix.Application.DTOs.ACC;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;


namespace Logix.MVC.LogixAPIs.Acc
{
    public class AccAccountsCostcenter : BaseAccApiController
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

        public AccAccountsCostcenter(
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



        [HttpPost("Search")]
        public async Task<IActionResult> Search(AccAccountsCostcenterFilterDto filter)
        {
            var chk = await permission.HasPermission(357, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var items = await accServiceManager.AccAccountsCostcenterVWService.GetAllVW(c => c.IsDeleted == false);
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    if (filter == null)
                    {
                        return Ok(items);
                    }




                    var final = res.ToList();
                    return Ok(await Result<List<AccAccountsCostcenterVw>>.SuccessAsync(final, ""));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccAccountsCostcenterVw>.FailAsync($"======= Exp in Search AccAccountsCostcenterVw, MESSAGE: {ex.Message}"));
            }
        }
        #endregion "transactions"

        #region "transactions_ADD"
        [HttpPost("Add")]
        public async Task<IActionResult> Add(AccAccountsCostcenterDto obj)
        {
            var chk = await permission.HasPermission(357, PermissionType.Add);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<AccAccountsCostcenterDto>.FailAsync(localization.GetMessagesResource("dataRequire")));
                }


                var add = await accServiceManager.AccAccountsCostcenterService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccAccountsCostcenterDto>.FailAsync($"======= Exp in Acc Accounts Costcenter  add: {ex.Message}"));
            }
        }
        #endregion "transactions_Add"

        #region "transactions_Update"
        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(AccAccountsCostcenterEditDto obj)
        {
            var chk = await permission.HasPermission(357, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<AccAccountsCostcenterEditDto>.FailAsync(localization.GetMessagesResource("dataRequire")));
                }


                var Edit = await accServiceManager.AccAccountsCostcenterService.Update(obj);

                return Ok(Edit);


            }
            catch (Exception ex)
            {
                return Ok(await Result<AccAccountsCostcenterEditDto>.FailAsync($"======= Exp in AccAccounts Costcenter  edit: {ex.Message}"));
            }
        }

        #endregion "transactions_Update"

        #region "transactions_Delete"
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long Id)
        {
            var chk = await permission.HasPermission(357, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {


                var Delete = await accServiceManager.AccAccountsCostcenterService.Remove(Id);
                return Ok(Delete);


            }
            catch (Exception ex)
            {
                return Ok(await Result<AccAccountsCostcenterDto>.FailAsync($"======= Exp in Accounts Costcenter  Delete: {ex.Message}"));
            }
        }

        #endregion "transactions_Delete"
        #region "transactions_GetById"

        [HttpGet("GetByIdForEdit")]
        public async Task<IActionResult> GetByIdForEdit(long id)
        {
            try
            {
                var chk = await permission.HasPermission(357, PermissionType.Show);
                if (!chk)
                {
                    return Ok(await Result.AccessDenied("AccessDenied"));
                }

                if (id <= 0)
                {
                    return Ok(await Result<AccAccountsCostcenterEditDto>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var getItem = await accServiceManager.AccAccountsCostcenterService.GetForUpdate<AccAccountsCostcenterEditDto>(id);
                if (getItem.Succeeded)
                {
                    var obj = getItem.Data;



                    return Ok(await Result<AccAccountsCostcenterEditDto>.SuccessAsync(obj, $""));
                }
                else
                {
                    return Ok(getItem);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccAccountsCostcenterEditDto>.FailAsync($"====== Exp in GetByIdForEdit Accounts Costcenter, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(long id)
        {
            try
            {
                var chk = await permission.HasPermission(357, PermissionType.Show);
                if (!chk)
                {
                    return Ok(await Result.AccessDenied("AcessDenied"));
                }

                if (id <= 0)
                {
                    return Ok(await Result<AccAccountsCostcenterDto>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var getItem = await accServiceManager.AccAccountsCostcenterService.GetOne(s => s.Id == id && s.IsDeleted == false);
                if (getItem.Succeeded)
                {
                    var obj = getItem.Data;
                    return Ok(await Result<AccAccountsCostcenterDto>.SuccessAsync(obj, $""));
                }
                else
                {
                    return Ok(getItem);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccAccountsCostcenterDto>.FailAsync($"====== Exp in GetById Accounts Costcenter, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetByAccountsId")]
        public async Task<IActionResult> GetByAccountsId(long id)
        {
            try
            {
                var chk = await permission.HasPermission(357, PermissionType.Show);
                if (!chk)
                {
                    return Ok(await Result.AccessDenied("AccessDenied"));
                }

                if (id <= 0)
                {
                    return Ok(await Result<AccAccountsCostcenterVw>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var getItem = await accServiceManager.AccAccountsCostcenterVWService.GetAll(x => x.AccAccountId == id && x.IsDeleted == false);
                if (getItem.Succeeded)
                {
                    var obj = getItem.Data;



                    return Ok(await Result<List<AccAccountsCostcenterVw>>.SuccessAsync(obj.ToList(), ""));

                }
                else
                {
                    return Ok(getItem);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccAccountsCostcenterVw>.FailAsync($"====== Exp in GetByAccountsId Accounts Costcenter, MESSAGE: {ex.Message}"));
            }
        }
        #endregion "transactions_GetById"

    }
}

