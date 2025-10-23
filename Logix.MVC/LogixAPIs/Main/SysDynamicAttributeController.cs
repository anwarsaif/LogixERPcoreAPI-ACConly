using Logix.Application.Common;
using Logix.Application.DTOs.Main;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.Main;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.Main
{
    public class SysDynamicAttributeController : BaseMainApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ILocalizationService localization;

        public SysDynamicAttributeController(IMainServiceManager mainServiceManager,
            IPermissionHelper permission,
            ILocalizationService localization)
        {
            this.mainServiceManager = mainServiceManager;
            this.permission = permission;
            this.localization = localization;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult> GetAll(SysScreenDto filter)
        {
            try
            {
                var chk = await permission.HasPermission(1004, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var items = await mainServiceManager.SysScreenService.GetAllVW(s => s.Isdel == false);
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpPost("Search")]
        public async Task<ActionResult> Search(SysScreenFilterDto filter)
        {
            try
            {
                var chk = await permission.HasPermission(1004, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                filter.SystemId ??= 0;
                var items = await mainServiceManager.SysScreenService.GetAllVW(s => s.Isdel == false
                && (filter.SystemId == 0 || s.SystemId == filter.SystemId)
                && (string.IsNullOrEmpty(filter.ScreenName) || s.ScreenName!.Contains(filter.ScreenName))
                && (string.IsNullOrEmpty(filter.ScreenName2) || s.ScreenName2!.Contains(filter.ScreenName2))
                );
                if (items.Succeeded)
                {
                    var resList = items.Data.ToList();
                    List<SysScreenFilterDto> final = new List<SysScreenFilterDto>();
                    foreach (var item in resList)
                    {
                        final.Add(new SysScreenFilterDto
                        {
                            ScreenId = item.ScreenId,
                            ScreenName = item.ScreenName,
                            ScreenName2 = item.ScreenName2,
                            SystemId = item.SystemId,
                            SystemName = item.SystemName,
                            SystemName2 = item.SystemName2
                        });
                    }

                    return Ok(await Result<List<SysScreenFilterDto>>.SuccessAsync(final));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpPost("Add")]
        public async Task<ActionResult> Add(SysDynamicAttributeDto entity)
        {
            try
            {
                var chk = await permission.HasPermission(1004, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                entity.StepId ??= 0; entity.TableId ??= 0; entity.MaxLength ??= 0; entity.DefaultValue ??= "";
                var addLicense = await mainServiceManager.SysDynamicAttributeService.Add(entity);
                return Ok(addLicense);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("GetByIdForEdit")]
        public async Task<IActionResult> GetByIdForEdit(long id)
        {
            try
            {
                var chk = await permission.HasPermission(1004, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var item = await mainServiceManager.SysDynamicAttributeService.GetOne(a => a.Id.Equals(id));
                return Ok(item);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in GetByIdForEdit SysDynamicAttributeController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(SysDynamicAttributeEditDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(1004, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                obj.StepId ??= 0; obj.TableId ??= 0; obj.MaxLength ??= 0; obj.DefaultValue ??= "";
                var update = await mainServiceManager.SysDynamicAttributeService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }


        [HttpPost("GetScreenDataForAdd")]
        public async Task<ActionResult> GetScreenDataForAdd(long screenId)
        {
            try
            {
                var chk = await permission.HasPermission(1004, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (screenId <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var items = await mainServiceManager.SysScreenService.GetOne(s => s.ScreenId == screenId && s.Isdel == false);
                if (items.Succeeded)
                {
                    SysDynamicAttributeVM obj = new SysDynamicAttributeVM()
                    {
                        ScreenId = items.Data.ScreenId ?? 0,
                        ScreenName = items.Data.ScreenName ?? "",
                        ScreenName2 = items.Data.ScreenName2 ?? "",
                        SystemId = items.Data.SystemId ?? 0
                    };

                    return Ok(await Result<SysDynamicAttributeVM>.SuccessAsync(obj));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpPost("GetScreenAttributes")]
        public async Task<ActionResult> GetScreenAttributes(long screenId)
        {
            try
            {
                var chk = await permission.HasPermission(1004, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (screenId <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var items = await mainServiceManager.SysDynamicAttributeService.GetAllVW(a => a.IsDeleted == false && a.ScreenId == screenId);
                if (items.Succeeded)
                {
                    var res = items.Data.OrderBy(x => x.SortOrder).ToList();
                    return Ok(await Result<List<SysDynamicAttributesVw>>.SuccessAsync(res));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var chk = await permission.HasPermission(1004, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await mainServiceManager.SysDynamicAttributeService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete SysDynamicAttributeController, MESSAGE: {ex.Message}"));
            }
        }

    }
}
