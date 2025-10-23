using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Logix.Application.Common;
using Logix.Application.DTOs.Main;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.Main;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.Integra
{
    public class IntegraRecordWebhookManagerController : BaseIntegraApiController
    {
        private readonly ICurrentData session;
        private readonly IPermissionHelper permission;
        private readonly ILocalizationService localization;
        private readonly IIntegraServiceManager integraServiceManager;
        private readonly IMainServiceManager mainServiceManager;

        public IntegraRecordWebhookManagerController(
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
        //سجل الويب هوك

        #region "GetAll - Search"
        [HttpPost("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var chk = await permission.HasPermission(1964, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var items = await mainServiceManager.SysRecordWebhookService.GetAll();

                if (!items.Data.Any())
                    return Ok(await Result<List<SysRecordWebhookDto>>.SuccessAsync(localization.GetResource1("NosearchResult")));

                return Ok(await Result<List<SysRecordWebhookDto>>.SuccessAsync(items.Data.ToList(), $"Search Completed {items.Data.Count()}", 200));
            }
            catch (Exception ex)
            {
                return Ok(await Result<List<SysRecordWebhookDto>>.FailAsync($"Error in {nameof(Search)}: {ex.Message}"));
            }
        }

        [HttpPost("Search")]
        public async Task<IActionResult> Search(SysRecordWebhookFilterDto filter)
        {
            try
            {
                var chk = await permission.HasPermission(1964, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                filter.SystemId ??= 0;
                filter.AppId ??= 0;
                filter.ScreenId ??= 0;
                filter.IsSended ??= 0;
                var items = await mainServiceManager.SysRecordWebhookService.GetAllVW(x =>
                    x.IsDeleted == false &&
                    x.FacilityId == session.FacilityId &&
                    (string.IsNullOrEmpty(filter.ErrorCode) || x.ErrorCode.Contains(filter.ErrorCode)) &&
                    (string.IsNullOrEmpty(filter.Name) || x.Name.Contains(filter.Name)) &&
                    (filter.IsSended == 0 || 
                    (filter.IsSended == 1 && x.IsSended == true) || 
                    (filter.IsSended == 2 && x.IsSended == false)) &&
                    (filter.SystemId == 0 || x.SystemId == filter.SystemId) &&
                    (filter.AppId == 0 || x.AppId == filter.AppId) &&
                    (filter.ScreenId == 0 || x.ScreenId == filter.ScreenId) &&
                    (string.IsNullOrEmpty(filter.ReferenceId) || x.ReferenceId.Contains(filter.ReferenceId)));

                if (!items.Data.Any())
                    return Ok(await Result<List<SysRecordWebhookVw>>.SuccessAsync(localization.GetResource1("NosearchResult")));

                return Ok(await Result<List<SysRecordWebhookVw>>.SuccessAsync(items.Data.ToList(), $"Search Completed {items.Data.Count()}", 200));
            }
            catch (Exception ex)
            {
                return Ok(await Result<List<SysRecordWebhookVw>>.FailAsync($"Error in {nameof(Search)}: {ex.Message}"));
            }
        }

        #endregion

        #region "Add - Edit"

        [HttpPost("Add")]
        public async Task<IActionResult> Add(SysRecordWebhookDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(1964, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync(localization.GetMessagesResource("dataRequire")));

                var add = await mainServiceManager.SysRecordWebhookService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(SysRecordWebhookEditDto obj)
        {
            var chk = await permission.HasPermission(1964, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<SysRecordWebhookEditDto>.FailAsync(localization.GetMessagesResource("dataRequire")));
                }

                var Edit = await mainServiceManager.SysRecordWebhookService.Update(obj);
                return Ok(Edit);
            }
            catch (Exception ex)
            {
                return Ok(await Result<SysRecordWebhookEditDto>.FailAsync($"======= Exp in edit: {ex.Message}"));
            }
        }

        #endregion "Add - Edit"

        #region "Delete - DeleteSelectedItems"
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long Id)
        {
            var chk = await permission.HasPermission(1964, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (Id <= 0)
                    return Ok(await Result.FailAsync(localization.GetMessagesResource("NoIdInDelete")));

                var add = await mainServiceManager.SysRecordWebhookService.Remove(Id);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<SysRecordWebhookDto>.FailAsync($"======= Exp in Delete: {ex.Message}"));
            }
        }

        [HttpDelete("DeleteSelectedItems")]
        public async Task<IActionResult> DeleteSelectedItems(string Ids)
        {
            var chk = await permission.HasPermission(1964, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if(string.IsNullOrEmpty(Ids))
                    return Ok(await Result.FailAsync(localization.GetMessagesResource("NoIdInDelete")));

                var items = await mainServiceManager.SysRecordWebhookService.RemoveSelectedItems(Ids);
                if (!items.Data.Any())
                    return Ok(await Result<List<SysRecordWebhookDto>>.SuccessAsync(localization.GetResource1("NosearchResult")));
                
                return Ok(await Result<List<SysRecordWebhookDto>>.SuccessAsync(items.Data.ToList()));
            }
            catch (Exception ex)
            {
                return Ok(await Result<List<SysRecordWebhookDto>>.FailAsync($"======= Exp in Delete: {ex.Message}"));
            }
        }
        #endregion "Delete - DeleteSelectedItems"

        #region "GetByIdForEdit - ProcessDetails"

        [HttpGet("GetByIdForEdit")]
        public async Task<IActionResult> GetByIdForEdit(long id)
        {
            try
            {
                var chk = await permission.HasPermission(1964, PermissionType.Show);
                if (!chk)
                {
                    return Ok(await Result.AccessDenied("AccessDenied"));
                }

                if (id <= 0)
                {
                    return Ok(await Result<SysRecordWebhookEditDto>.FailAsync(localization.GetMessagesResource("InCorrectId")));
                }

                var getItem = await mainServiceManager.SysRecordWebhookService.GetForUpdate<SysRecordWebhookEditDto>(id);
                if (getItem.Succeeded)
                {
                    return Ok(await Result<SysRecordWebhookEditDto>.SuccessAsync(getItem.Data, $""));
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

        [HttpGet("ProcessDetails")]
        public async Task<IActionResult> ProcessDetails(long id)
        {
            try
            {
                var chk = await permission.HasPermission(1964, PermissionType.Show);
                if (!chk)
                {
                    return Ok(await Result.AccessDenied("AcessDenied"));
                }

                if (id <= 0)
                {
                    return Ok(await Result<SysRecordWebhookVw>.FailAsync(localization.GetMessagesResource("NoIdInUpdate")));
                }

                var getItem = await mainServiceManager.SysRecordWebhookService.GetOneVW(s => s.Id == id && s.FacilityId == session.FacilityId && s.IsDeleted == false);
                if (getItem.Succeeded)
                {
                    var obj = getItem.Data;
                    return Ok(await Result<SysRecordWebhookVw>.SuccessAsync(obj, $""));
                }
                else
                {
                    return Ok(getItem);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<SysRecordWebhookVw>.FailAsync($"====== Exp MESSAGE: {ex.Message}"));
            }
        }

        #endregion "GetByIdForEdit - GetById"
    }
}
