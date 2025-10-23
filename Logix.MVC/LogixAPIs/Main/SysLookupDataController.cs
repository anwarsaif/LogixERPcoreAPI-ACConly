using Logix.Application.Common;
using Logix.Application.DTOs.Main;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.Main;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Logix.MVC.LogixAPIs.Main
{
    public class SysLookupDataController : BaseMainApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IAccServiceManager accServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IApiDDLHelper ddlHelper;

        public SysLookupDataController(
           IMainServiceManager mainServiceManager,
           IAccServiceManager accServiceManager,
           IPermissionHelper permission,
           ILocalizationService localization,
           ICurrentData session,
           IApiDDLHelper ddlHelper)
        {
            this.mainServiceManager = mainServiceManager;
            this.accServiceManager = accServiceManager;
            this.permission = permission;
            this.localization = localization;
            this.session = session;
            this.ddlHelper = ddlHelper;
        }

        [HttpPost("Search")]
        public async Task<ActionResult> Search(SysLookupDataFilterDto filter)
        {
            try
            {
                var chk = await permission.HasPermission(122, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                filter.CatagoriesId ??= 0;
                if (filter.SystemId == "0")
                    filter.SystemId = null;

                var items = await mainServiceManager.SysLookupDataService.GetAllVW(x => x.Isdel == false
                && (string.IsNullOrEmpty(filter.SystemId) || (!string.IsNullOrEmpty(x.SystemId) && x.SystemId.Equals(filter.SystemId)))
                && (filter.CatagoriesId == 0 || x.CatagoriesId == filter.CatagoriesId)
                && (string.IsNullOrEmpty(filter.Name) || (!string.IsNullOrEmpty(x.Name) && x.Name.Contains(filter.Name)) || (!string.IsNullOrEmpty(x.Name2) && x.Name2.ToLower().Contains(filter.Name.ToLower()))));
                
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Add")]
        public async Task<ActionResult> Add(SysLookupDataDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(122, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var add = await mainServiceManager.SysLookupDataService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add SysLookupDataController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(SysLookupDataDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(122, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var update = await mainServiceManager.SysLookupDataService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Edit SysLookupDataController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(long id)
        {
            try
            {
                var chk = await permission.HasPermission(122, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var item = await mainServiceManager.SysLookupDataService.GetOne(c => c.Id == id && c.Isdel == false);
                return Ok(item);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in GetById SysLookupDataController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetByIdForEdit")]
        public async Task<IActionResult> GetByIdForEdit(long id)
        {
            try
            {
                var chk = await permission.HasPermission(122, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var item = await mainServiceManager.SysLookupDataService.GetForUpdate<SysLookupDataDto>(id);
                if (item.Succeeded)
                {
                    var costCenter = await accServiceManager.AccCostCenterService.GetOne(c => c.CcId == item.Data.CcId && c.IsDeleted == false);
                    if (costCenter.Succeeded && costCenter != null)
                    {
                        item.Data.CostCenterCode = costCenter.Data.CostCenterCode;
                        item.Data.CostCenterName = costCenter.Data.CostCenterName;
                    }
                    var account = await accServiceManager.AccAccountService.GetOne(a => a.AccAccountId == item.Data.AccAccountId && a.IsDeleted == false);
                    if (account.Succeeded && account != null)
                    {
                        item.Data.AccountCode = account.Data.AccAccountCode;
                        item.Data.AccountName = account.Data.AccAccountName;
                    }
                }
                return Ok(item);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in GetByIdForEdit SysLookupDataController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var chk = await permission.HasPermission(122, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await mainServiceManager.SysLookupDataService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete SysLookupDataController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetAllCategoriesDdl")]
        public async Task<IActionResult> GetAllCategoriesDdl(string systemId)
        {
            try
            {
                if (!string.IsNullOrEmpty(systemId))
                {
                    var list = new SelectList(new List<DDListItem<SysLookupCategory>>());
                    list = await ddlHelper.GetAnyLis<SysLookupCategory, long>(c => c.Isdel == false && c.SystemId == systemId, "CatagoriesId", (session.Language == 1) ? "CatagoriesName" : "CatagoriesName2");
                    return Ok(await Result<SelectList>.SuccessAsync(list));
                }

                return Ok(await Result.FailAsync("Invalid System Id"));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in GetAllCategoriesDdl SysLookupDataController, MESSAGE: {ex.Message}"));
            }
        }
    }
}
