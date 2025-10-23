using Logix.Application.Common;
using Logix.Application.DTOs.Main;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Logix.MVC.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.Main
{
    public class SysPoliciesProcedureController : BaseMainApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ILocalizationService localization;

        public SysPoliciesProcedureController(IMainServiceManager mainServiceManager,
            IPermissionHelper permission,
            ILocalizationService localization)
        {
            this.mainServiceManager = mainServiceManager;
            this.permission = permission;
            this.localization = localization;
        }

        [HttpPost("Search")]
        public async Task<IActionResult> Search(SysPoliciesProcedureFilterDto filter)
        {
            try
            {
                var chk = await permission.HasPermission(934, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                filter.TypeId ??= 0;
                var items = await mainServiceManager.SysPoliciesProcedureService.GetAllVW(c => c.IsDeleted == false
                && (filter.TypeId == 0 || (c.TypeId != null && c.TypeId == filter.TypeId))
                && (string.IsNullOrEmpty(filter.Name) || (!string.IsNullOrEmpty(c.Name) && c.Name.Contains(filter.Name)) || (!string.IsNullOrEmpty(c.Name2) && c.Name2.Contains(filter.Name))));
                
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Search SysPoliciesProcedureController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("GetSysGroups")]
        public async Task<IActionResult> GetSysGroups()
        {
            try
            {
                var sysGroups = await mainServiceManager.SysGroupService.GetAll(g => g.IsDeleted == false);
                if (sysGroups.Succeeded)
                {
                    List<SysGroupVM> list = new List<SysGroupVM>();
                    foreach (var item in sysGroups.Data)
                    {
                        var sysgroupVM = new SysGroupVM { GroupId = item.GroupId ?? 0, GroupName = item.GroupName };
                        list.Add(sysgroupVM);
                    }
                    return Ok(await Result<List<SysGroupVM>>.SuccessAsync(list));
                }
                return Ok(sysGroups);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in GetSysGroups SysPoliciesProcedureController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Add")]
        public async Task<ActionResult> Add(SysPoliciesProcedureDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(934, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var add = await mainServiceManager.SysPoliciesProcedureService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add SysPoliciesProcedureController, MESSAGE: {ex.Message}"));
            }
        }


        [HttpGet("GetByIdForEdit")]
        public async Task<IActionResult> GetByIdForEdit(long id)
        {
            try
            {
                var chk = await permission.HasPermission(934, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var getItem = await mainServiceManager.SysPoliciesProcedureService.GetForUpdate<SysPoliciesProcedureEditDto>(id);
                return Ok(getItem);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in GetByIdForEdit SysPoliciesProcedureController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(SysPoliciesProcedureEditDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(934, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var update = await mainServiceManager.SysPoliciesProcedureService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Edit SysPoliciesProcedureController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var chk = await permission.HasPermission(934, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await mainServiceManager.SysPoliciesProcedureService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete SysPoliciesProcedureController, MESSAGE: {ex.Message}"));
            }
        }
    }
}