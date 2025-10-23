using DevExpress.CodeParser;
using Logix.Application.Common;
using Logix.Application.DTOs.Main;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.Main;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.Integra
{
    public class IntegraRecordWebhookManagerAuthController : Controller
    {
        private readonly ICurrentData session;
        private readonly IPermissionHelper permission;
        private readonly ILocalizationService localization;
        private readonly IIntegraServiceManager integraServiceManager;
        private readonly IMainServiceManager mainServiceManager;

        public IntegraRecordWebhookManagerAuthController(
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
        //سجل مصادقة الويب هوك  - Webhook Auth سجل

        #region "GetAll - Search"
        [HttpPost("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var chk = await permission.HasPermission(2128, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var items = await mainServiceManager.SysRecordWebHookAuthService.GetAllVW();

                if (!items.Data.Any())
                    return Ok(await Result<List<SysRecordWebhookAuthVw>>.SuccessAsync(localization.GetResource1("NosearchResult")));

                return Ok(await Result<List<SysRecordWebhookAuthVw>>.SuccessAsync(items.Data.ToList(), $"Search Completed {items.Data.Count()}", 200));
            }
            catch (Exception ex)
            {
                return Ok(await Result<List<SysRecordWebhookAuthVw>>.FailAsync($"Error in {nameof(Search)}: {ex.Message}"));
            }
        }

        [HttpPost("Search")]
        public async Task<IActionResult> Search(SysRecordWebhookAuthFilterDto filter)
        {
            try
            {
                var chk = await permission.HasPermission(2128, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                filter.AppId ??= 0;
                filter.IsSended ??= 0;

                if (filter.IsSended == 0)
                    filter.IsSended = 0;
                else if (filter.IsSended == 1)
                    filter.IsSended = 1;
                else if (filter.IsSended == 2)
                    filter.IsSended = 2;

                var items = await mainServiceManager.SysRecordWebHookAuthService.GetAllVW(x =>
                    x.IsDeleted == false &&
                    x.FacilityId == session.FacilityId &&
                    (string.IsNullOrEmpty(filter.Name) || x.Name.Contains(filter.Name)) &&
                    (string.IsNullOrEmpty(filter.ErrorCode) || x.ErrorCode.Contains(filter.ErrorCode)) &&
                    (filter.IsSended == 0 ||
                    (filter.IsSended == 1 && x.IsSended == true) ||
                    (filter.IsSended == 2 && x.IsSended == false)) &&
                    (filter.AppId == 0 || x.AppId == filter.AppId));

                if (!items.Data.Any())
                    return Ok(await Result<List<SysRecordWebhookAuthVw>>.SuccessAsync(localization.GetResource1("NosearchResult")));

                return Ok(await Result<List<SysRecordWebhookAuthVw>>.SuccessAsync(items.Data.ToList(), $"Search Completed {items.Data.Count()}", 200));
            }
            catch (Exception ex)
            {
                return Ok(await Result<List<SysRecordWebhookAuthVw>>.FailAsync($"Error in {nameof(Search)}: {ex.Message}"));
            }
        }

        #endregion

        #region "Add - Edit"

        [HttpPost("Add")]
        public async Task<IActionResult> Add(SysRecordWebhookAuthDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(2128, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync(localization.GetMessagesResource("dataRequire")));

                var add = await mainServiceManager.SysRecordWebHookAuthService.Add(obj);
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
    }
}
