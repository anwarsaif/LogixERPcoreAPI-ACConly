using Logix.Application.Common;
using Logix.Application.DTOs.Main;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.Main;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.Integra
{
    public class IntegraWebhookManagerAuthController : BaseIntegraApiController
    {
        private readonly ICurrentData session;
        private readonly IPermissionHelper permission;
        private readonly ILocalizationService localization;
        private readonly IIntegraServiceManager integraServiceManager;
        private readonly IMainServiceManager mainServiceManager;

        public IntegraWebhookManagerAuthController(
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
        //مصادقة الويب هوك

        #region "GetAll - Search"
        [HttpPost("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var chk = await permission.HasPermission(2127, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var items = await mainServiceManager.SysWebHookAuthService.GetAll();

                if (!items.Data.Any())
                    return Ok(await Result<List<SysWebHookAuthDto>>.SuccessAsync(localization.GetResource1("NosearchResult")));

                return Ok(await Result<List<SysWebHookAuthDto>>.SuccessAsync(items.Data.ToList(), $"Search Completed {items.Data.Count()}", 200));
            }
            catch (Exception ex)
            {
                return Ok(await Result<List<SysWebHookAuthDto>>.FailAsync($"Error in {nameof(Search)}: {ex.Message}"));
            }
        }

        [HttpPost("Search")]
        public async Task<IActionResult> Search(SysWebHookAuthFilterDto filter)
        {
            try
            {
                var chk = await permission.HasPermission(2127, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                filter.AppId ??= 0;
                var items = await mainServiceManager.SysWebHookAuthService.GetAllVW(x =>
                    x.IsDeleted == false &&
                    x.FacilityId == session.FacilityId &&
                    (string.IsNullOrEmpty(filter.Name) || x.Name.Contains(filter.Name)) &&
                    (string.IsNullOrEmpty(filter.Description) || x.Description.Contains(filter.Description)) &&
                    (filter.AppId == 0 || x.AppId == filter.AppId));

                if (!items.Data.Any())
                    return Ok(await Result<List<SysWebHookAuthVw>>.SuccessAsync(localization.GetResource1("NosearchResult")));

                return Ok(await Result<List<SysWebHookAuthVw>>.SuccessAsync(items.Data.ToList(), $"Search Completed {items.Data.Count()}", 200));
            }
            catch (Exception ex)
            {
                return Ok(await Result<List<SysWebHookAuthVw>>.FailAsync($"Error in {nameof(Search)}: {ex.Message}"));
            }
        }

        #endregion

        #region "Add - Edit"
        //إضافة مصادقة ويب هووك

        [HttpPost("Add")]
        public async Task<IActionResult> Add(SysWebHookAuthDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(2127, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync(localization.GetMessagesResource("dataRequire")));

                var add = await mainServiceManager.SysWebHookAuthService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(SysWebHookAuthEditDto obj)
        {
            var chk = await permission.HasPermission(2127, PermissionType.Edit);
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

                var Edit = await mainServiceManager.SysWebHookAuthService.Update(obj);
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
            var chk = await permission.HasPermission(2127, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var add = await mainServiceManager.SysWebHookAuthService.Remove(Id);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<SysWebHookAuthDto>.FailAsync($"======= Exp in Delete: {ex.Message}"));
            }
        }
        #endregion "Delete"

        #region "GetByIdForEdit - GetById"

        [HttpGet("GetByIdForEdit")]
        public async Task<IActionResult> GetByIdForEdit(long id)
        {
            try
            {
                var chk = await permission.HasPermission(2127, PermissionType.Show);
                if (!chk)
                {
                    return Ok(await Result.AccessDenied("AccessDenied"));
                }

                if (id <= 0)
                {
                    return Ok(await Result<SysWebHookEditDto>.FailAsync(localization.GetMessagesResource("InCorrectId")));
                }

                var getItem = await mainServiceManager.SysWebHookAuthService.GetForUpdate<SysWebHookEditDto>(id);
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
                var chk = await permission.HasPermission(2127, PermissionType.Show);
                if (!chk)
                {
                    return Ok(await Result.AccessDenied("AcessDenied"));
                }

                if (id <= 0)
                {
                    return Ok(await Result<SysWebHookAuthDto>.FailAsync(localization.GetMessagesResource("NoIdInUpdate")));
                }

                var getItem = await mainServiceManager.SysWebHookAuthService.GetOne(s => s.Id == id && s.IsDeleted == false);
                if (getItem.Succeeded)
                {
                    var obj = getItem.Data;
                    return Ok(await Result<SysWebHookAuthDto>.SuccessAsync(obj, $""));
                }
                else
                {
                    return Ok(getItem);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<SysWebHookAuthDto>.FailAsync($"====== Exp MESSAGE: {ex.Message}"));
            }
        }

        #endregion "GetByIdForEdit - GetById"
    }
}
