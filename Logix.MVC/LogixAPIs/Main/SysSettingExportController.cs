using Logix.Application.Common;
using Logix.Application.DTOs.Main;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.Main.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.Main
{
    public class SysSettingExportController : BaseMainApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ILocalizationService localization;

        public SysSettingExportController(IMainServiceManager mainServiceManager,
            IPermissionHelper permission,
            ILocalizationService localization)
        {
            this.mainServiceManager = mainServiceManager;
            this.permission = permission;
            this.localization = localization;
        }

        [HttpPost("Search")]
        public async Task<ActionResult> Search(SysSettingExportFilterDto filter)
        {
            try
            {
                var chk = await permission.HasPermission(1841, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var items = await mainServiceManager.SysSettingExportService.GetAllVW(d => d.IsDeleted == false
                    && (filter.FacilityId == null || (d.FacilityId != null && d.FacilityId == filter.FacilityId))
                    && (filter.SystemId == null || (d.SystemId != null && d.SystemId == filter.SystemId))
                    && (filter.ScreenId == null || (d.ScreenId != null && d.ScreenId == filter.ScreenId))
                );

                if (items.Succeeded)
                {
                    var res = items.Data.ToList();
                    List<SysSettingExportVM> final = new();
                    foreach (var item in res)
                    {
                        final.Add(new SysSettingExportVM
                        {
                            Id = item.Id,
                            SystemName = item.SystemName,
                            ScreenName = item.ScreenName,
                            Name = item.Name,
                            Name2 = item.Name2,
                            TypeName = item.Type == 1 ? "Excel" : item.Type == 2 ? "CSV" : "-",
                            Query = item.Query,
                            Type = item.Type
                        });
                    }
                    return Ok(await Result<List<SysSettingExportVM>>.SuccessAsync(final));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Search SysSettingExportController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Add")]
        public async Task<ActionResult> Add(SysSettingExportDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(1841, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var add = await mainServiceManager.SysSettingExportService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add SysSettingExportController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(SysSettingExportDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(1841, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var update = await mainServiceManager.SysSettingExportService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Edit SysSettingExportController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var chk = await permission.HasPermission(1841, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await mainServiceManager.SysSettingExportService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete SysSettingExportController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetByIdForEdit")]
        public async Task<IActionResult> GetByIdForEdit(long id)
        {
            try
            {
                var chk = await permission.HasPermission(1841, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var item = await mainServiceManager.SysSettingExportService.GetForUpdate<SysSettingExportDto>(id);
                return Ok(item);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in GetByIdForEdit SysSettingExportController, MESSAGE: {ex.Message}"));
            }
        }
    }
}