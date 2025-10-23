using Logix.Application.Common;
using Logix.Application.DTOs.Integra;
using Logix.Application.DTOs.Main;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.Main;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.Integra
{
    public class IntegraWebhookManagerController : BaseIntegraApiController
    {
        private readonly ICurrentData session;
        private readonly IPermissionHelper permission;
        private readonly ILocalizationService localization;
        private readonly IIntegraServiceManager integraServiceManager;
        private readonly IMainServiceManager mainServiceManager;

        public IntegraWebhookManagerController(
            ICurrentData session,
            IPermissionHelper permission,
            ILocalizationService localization,
            IIntegraServiceManager integraServiceManager,
            IMainServiceManager mainServiceManager
            )
        {
            this.session = session;
            this.permission = permission;
            this.localization = localization;
            this.integraServiceManager = integraServiceManager;
            this.mainServiceManager = mainServiceManager;
        }
        //الويب هوك

        #region "GetAll - Search"
        [HttpPost("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var chk = await permission.HasPermission(1963, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var items = await mainServiceManager.SysWebHookService.GetAll();

                if (!items.Data.Any())
                    return Ok(await Result<List<SysWebHookDto>>.SuccessAsync(localization.GetResource1("NosearchResult")));

                return Ok(await Result<List<SysWebHookDto>>.SuccessAsync(items.Data.ToList(), $"Search Completed {items.Data.Count()}", 200));
            }
            catch (Exception ex)
            {
                return Ok(await Result<List<SysWebHookDto>>.FailAsync($"Error in {nameof(Search)}: {ex.Message}"));
            }
        }

        [HttpPost("Search")]
        public async Task<IActionResult> Search(SysWebHookFilterDto filter)
        {
            try
            {
                var chk = await permission.HasPermission(1963, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                filter.SystemId ??= 0;
                filter.AppId ??= 0;
                filter.ScreenId ??= 0;
                var items = await mainServiceManager.SysWebHookService.GetAllVW(x =>
                    x.IsDeleted == false &&
                    x.FacilityId == session.FacilityId &&
                    (string.IsNullOrEmpty(filter.Name) || x.Name.Contains(filter.Name)) &&
                    (string.IsNullOrEmpty(filter.Description) || x.Description.Contains(filter.Description)) &&
                    (filter.SystemId == 0 || x.SystemId == filter.SystemId) &&
                    (filter.AppId == 0 || x.AppId == filter.AppId) && 
                    (filter.ScreenId == 0 || x.ScreenId == filter.ScreenId));

                if (!items.Data.Any())
                    return Ok(await Result<List<SysWebHookVw>>.SuccessAsync(localization.GetResource1("NosearchResult")));

                return Ok(await Result<List<SysWebHookVw>>.SuccessAsync(items.Data.ToList(), $"Search Completed {items.Data.Count()}", 200));
            }
            catch (Exception ex)
            {
                return Ok(await Result<List<SysWebHookVw>>.FailAsync($"Error in {nameof(Search)}: {ex.Message}"));
            }
        }

        #endregion

        #region "Add - Edit"

        [HttpPost("Add")]
        public async Task<IActionResult> Add(SysWebHookDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(1963, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync(localization.GetMessagesResource("dataRequire")));

                var add = await mainServiceManager.SysWebHookService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(SysWebHookEditDto obj)
        {
            var chk = await permission.HasPermission(1963, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<SysWebHookEditDto>.FailAsync(localization.GetMessagesResource("dataRequire")));
                }

                var Edit = await mainServiceManager.SysWebHookService.Update(obj);
                return Ok(Edit);
            }
            catch (Exception ex)
            {
                return Ok(await Result<SysWebHookEditDto>.FailAsync($"======= Exp in edit: {ex.Message}"));
            }
        }

        #endregion "Add - Edit"

        #region "Delete"
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long Id)
        {
            var chk = await permission.HasPermission(1963, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var add = await mainServiceManager.SysWebHookService.Remove(Id);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<SysWebHookDto>.FailAsync($"======= Exp in Delete: {ex.Message}"));
            }
        }
        #endregion "Delete"

        #region "GetByIdForEdit - GetById"

        [HttpGet("GetByIdForEdit")]
        public async Task<IActionResult> GetByIdForEdit(long id)
        {
            try
            {
                var chk = await permission.HasPermission(1963, PermissionType.Show);
                if (!chk)
                {
                    return Ok(await Result.AccessDenied("AccessDenied"));
                }

                if (id <= 0)
                {
                    return Ok(await Result<SysWebHookEditDto>.FailAsync(localization.GetMessagesResource("InCorrectId")));
                }

                var getItem = await mainServiceManager.SysWebHookService.GetForUpdate<SysWebHookEditDto>(id);
                if (getItem.Succeeded)
                {
                    return Ok(await Result<SysWebHookEditDto>.SuccessAsync(getItem.Data, $""));
                }
                else
                {
                    return Ok(getItem);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(long id)
        {
            try
            {
                var chk = await permission.HasPermission(1963, PermissionType.Show);
                if (!chk)
                {
                    return Ok(await Result.AccessDenied("AcessDenied"));
                }

                if (id <= 0)
                {
                    return Ok(await Result<SysWebHookDto>.FailAsync(localization.GetMessagesResource("NoIdInUpdate")));
                }

                var getItem = await mainServiceManager.SysWebHookService.GetOne(s => s.Id == id && s.IsDeleted == false);
                if (getItem.Succeeded)
                {
                    var obj = getItem.Data;
                    return Ok(await Result<SysWebHookDto>.SuccessAsync(obj, $""));
                }
                else
                {
                    return Ok(getItem);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<SysWebHookDto>.FailAsync($"====== Exp MESSAGE: {ex.Message}"));
            }
        }

        #endregion "GetByIdForEdit - GetById"
    }
}
