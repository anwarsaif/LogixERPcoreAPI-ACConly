using Logix.Application.Common;
using Logix.Application.DTOs.Main;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.Main;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.Main
{
    public class SysFilesDocumentController : BaseMainApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;

        public SysFilesDocumentController(IMainServiceManager mainServiceManager,
            IPermissionHelper permission,
            ICurrentData session,
        ILocalizationService localization
            )
        {
            this.mainServiceManager = mainServiceManager;
            this.permission = permission;
            this.session = session;
            this.localization = localization;
        }

        [HttpPost("Search")]
        public async Task<ActionResult> Search(SysFilesDocumentFilterDto filter)
        {
            try
            {
                var chk = await permission.HasPermission(1793, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var items = await mainServiceManager.SysFilesDocumentService.GetAllVW(d => d.IsDeleted == false
                    && (filter.FacilityId == null || (d.FacilityId != null && d.FacilityId == filter.FacilityId))
                    && (filter.SystemId == null || (d.SystemId != null && d.SystemId == filter.SystemId))
                    && (filter.ScreenId == null || (d.ScreenId != null && d.ScreenId == filter.ScreenId))
                );

                if (items.Succeeded)
                {
                    var res = items.Data.ToList();
                    return Ok(await Result<List<SysFilesDocumentVw>>.SuccessAsync(res));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Search SysFilesDocumentController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Add")]
        public async Task<ActionResult> Add(SysFilesDocumentDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(1793, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var add = await mainServiceManager.SysFilesDocumentService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add SysFilesDocumentController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(SysFilesDocumentDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(1793, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var update = await mainServiceManager.SysFilesDocumentService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Edit SysFilesDocumentController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var chk = await permission.HasPermission(1793, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await mainServiceManager.SysFilesDocumentService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete SysFilesDocumentController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(long id)
        {
            try
            {
                var chk = await permission.HasPermission(1793, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var item = await mainServiceManager.SysFilesDocumentService.GetOne(t => t.Id == id && t.IsDeleted == false);
                return Ok(item);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in GetById SysFilesDocumentController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetByIdForEdit")]
        public async Task<IActionResult> GetByIdForEdit(long id)
        {
            try
            {
                var chk = await permission.HasPermission(1793, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var item = await mainServiceManager.SysFilesDocumentService.GetForUpdate<SysFilesDocumentDto>(id);
                return Ok(item);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in GetByIdForEdit SysFilesDocumentController, MESSAGE: {ex.Message}"));
            }
        }

    }
}