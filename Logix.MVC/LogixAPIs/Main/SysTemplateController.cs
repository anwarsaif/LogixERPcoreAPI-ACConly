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
    public class SysTemplateController : BaseMainApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IPermissionHelper permission;
        private readonly IApiDDLHelper ddlHelper;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IFilesHelper filesHelper;

        public SysTemplateController(IMainServiceManager mainServiceManager,
            IPermissionHelper permission,
            IApiDDLHelper ddlHelper,
            ILocalizationService localization,
            ICurrentData session,
            IFilesHelper filesHelper)
        {
            this.mainServiceManager = mainServiceManager;
            this.permission = permission;
            this.ddlHelper = ddlHelper;
            this.session = session;
            this.localization = localization;
            this.filesHelper = filesHelper;
        }

        [HttpPost("Search")]
        public async Task<ActionResult> Search(SysTemplateFilterDto filter)
        {
            try
            {
                var chk = await permission.HasPermission(399, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                var items = await mainServiceManager.SysTemplateService.GetAllVW(t => t.IsDeleted == false);
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();

                    if (!string.IsNullOrEmpty(filter.Name))
                        res = res.Where(t => !string.IsNullOrEmpty(t.Name) && t.Name.Contains(filter.Name));

                    if (filter.SystemId > 0)
                        res = res.Where(t => t.SystemId != null && t.SystemId.Equals(filter.SystemId));

                    if (filter.ScreenId > 0)
                        res = res.Where(t => t.ScreenId != null && t.ScreenId.Equals(filter.ScreenId));

                    var final = res.ToList();

                    return Ok(await Result<List<SysTemplateVw>>.SuccessAsync(final));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Search SysTemplateController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Add")]
        public async Task<ActionResult> Add(SysTemplateDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(399, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                //if type is text, so the details field is required
                if (obj.TypeId == 1 && string.IsNullOrEmpty(obj.Detailes))
                    return Ok(await Result.FailAsync($"{localization.GetSALResource("Details")}"));

                var add = await mainServiceManager.SysTemplateService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add SysTemplateController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(SysTemplateEditDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(399, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                if (obj.TypeId == 1 && string.IsNullOrEmpty(obj.Detailes))
                    return Ok(await Result.FailAsync($"{localization.GetSALResource("Details")}"));

                var update = await mainServiceManager.SysTemplateService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Edit SysTemplateController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var chk = await permission.HasPermission(399, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await mainServiceManager.SysTemplateService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete SysTemplateController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var chk = await permission.HasPermission(399, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var item = await mainServiceManager.SysTemplateService.GetOne(t => t.Id == id && t.IsDeleted == false);
                return Ok(item);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in GetById SysTemplateController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetByIdForEdit")]
        public async Task<IActionResult> GetByIdForEdit(int id)
        {
            try
            {
                var chk = await permission.HasPermission(399, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var item = await mainServiceManager.SysTemplateService.GetForUpdate<SysTemplateEditDto>(id);
                return Ok(item);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in GetByIdForEdit SysTemplateController, MESSAGE: {ex.Message}"));
            }
        }


        [HttpGet("GetScreens")]
        public async Task<IActionResult> GetScreens(int systemId)
        {
            try
            {
                if (systemId > 0)
                {
                    var list = new SelectList(new List<DDListItem<SysScreen>>());
                    list = await ddlHelper.GetAnyLis<SysScreen, long>(s => s.SystemId == Convert.ToInt32(systemId) && s.ParentId != s.ScreenId && s.Isdel == false, "ScreenId", (session.Language == 1) ? "ScreenName" : "ScreenName2");
                    return Ok(await Result<SelectList>.SuccessAsync(list));
                }

                return Ok(await Result.FailAsync("Invalid System Id"));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in GetScreens SysTemplateController, MESSAGE: {ex.Message}"));
            }
        }
    }
}
