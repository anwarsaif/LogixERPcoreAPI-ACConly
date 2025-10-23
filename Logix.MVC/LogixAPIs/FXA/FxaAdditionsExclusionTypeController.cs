using Logix.Application.Common;
using Logix.Application.DTOs.FXA;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.FXA;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.FXA
{
    public class FxaAdditionsExclusionTypeController : BaseFxaApiController
    {
        private readonly IFxaServiceManager fxaServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ILocalizationService localization;

        public FxaAdditionsExclusionTypeController(IFxaServiceManager fxaServiceManager,
            IPermissionHelper permission,
            ILocalizationService localization)
        {
            this.fxaServiceManager = fxaServiceManager;
            this.permission = permission;
            this.localization = localization;
        }

        [HttpPost("Search")]
        public async Task<ActionResult> Search(FxaAdditionsExclusionTypeFilterDto filter)
        {
            try
            {
                var chk = await permission.HasPermission(1985, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                filter.Id ??= 0;
                filter.TypeId ??= 0;

                var items = await fxaServiceManager.FxaAdditionsExclusionTypeService.GetAll(t => t.IsDeleted == false
                    && (filter.Id == 0 || (t.Id == filter.Id))
                    && (filter.TypeId == 0 || (t.TypeId != null && t.TypeId == filter.TypeId))
                    && (string.IsNullOrEmpty(filter.Name) || (!string.IsNullOrEmpty(t.Name) && t.Name.Contains(filter.Name)))
                 );

                if (items.Succeeded)
                {
                    var res = items.Data.OrderBy(t => t.Id).ToList();
                    List<FxaAdditionsExclusionTypeFilterDto> final = new();

                    foreach (var item in res)
                    {
                        final.Add(new FxaAdditionsExclusionTypeFilterDto
                        {
                            Id = item.Id,
                            Name = item.Name,
                            TypeId = item.TypeId,
                            TypeName = item.TypeId == 1 ? localization.GetAccResource("Additions") : (item.TypeId == 2 ? localization.GetAccResource("Exclusions") : "-")
                        });
                    }
                    return Ok(await Result<List<FxaAdditionsExclusionTypeFilterDto>>.SuccessAsync(final));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Search FxaAdditionsExclusionTypeController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Add")]
        public async Task<ActionResult> Add(FxaAdditionsExclusionTypeDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(1985, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var add = await fxaServiceManager.FxaAdditionsExclusionTypeService.Add(obj);
                return add.Succeeded ? Ok(await Result<long>.SuccessAsync(add.Data.Id??0)) : Ok(await Result.FailAsync(add.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add FxaAdditionsExclusionTypeController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetByIdForEdit")]
        public async Task<IActionResult> GetByIdForEdit(long id)
        {
            try
            {
                var chk = await permission.HasPermission(1985, PermissionType.View);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var getItem = await fxaServiceManager.FxaAdditionsExclusionTypeService.GetForUpdate<FxaAdditionsExclusionTypeDto>(id);
                return Ok(getItem);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in GetByIdForEdit FxaAdditionsExclusionTypeController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(FxaAdditionsExclusionTypeDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(1985, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var update = await fxaServiceManager.FxaAdditionsExclusionTypeService.Update(obj);
                return update.Succeeded ? Ok(await Result.SuccessAsync()) : Ok(await Result.FailAsync(update.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Edit FxaAdditionsExclusionTypeController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var chk = await permission.HasPermission(1985, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await fxaServiceManager.FxaAdditionsExclusionTypeService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete FxaAdditionsExclusionTypeController, MESSAGE: {ex.Message}"));
            }
        }
    }
}