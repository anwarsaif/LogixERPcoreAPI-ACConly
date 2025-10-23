using Logix.Application.Common;
using Logix.Application.DTOs.WF;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.WF
{
    public class WfLookUpDataController : BaseWfController
    {
        private readonly IWFServiceManager wfServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ILocalizationService localization;

        public WfLookUpDataController(IWFServiceManager wfServiceManager,
            IPermissionHelper permission,
            ILocalizationService localization)
        {
            this.wfServiceManager = wfServiceManager;
            this.permission = permission;
            this.localization = localization;
        }

        [HttpPost("Search")]
        public async Task<ActionResult> Search(WfLookupDataFilterDto filter)
        {
            try
            {
                var chk = await permission.HasPermission(301, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                filter.CatagoriesId ??= 0;
                var items = await wfServiceManager.WfLookupDataService.GetAllVW(x => x.Isdel == false
                && (filter.CatagoriesId == 0 || x.CatagoriesId == filter.CatagoriesId)
                && (
                        string.IsNullOrEmpty(filter.Name)
                        || (!string.IsNullOrEmpty(x.Name) && x.Name.Contains(filter.Name))
                        || (!string.IsNullOrEmpty(x.Name2) && x.Name2.Contains(filter.Name))
                    ));

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Add")]
        public async Task<ActionResult> Add(WfLookupDataDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(301, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                obj.Code ??= 0;obj.SortNo ??= 0;
                var add = await wfServiceManager.WfLookupDataService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }


        [HttpGet("GetByIdForEdit")]
        public async Task<IActionResult> GetByIdForEdit(long id)
        {
            try
            {
                var chkEdit = await permission.HasPermission(301, PermissionType.Edit);
                var chkView = await permission.HasPermission(301, PermissionType.View);
                if (!chkEdit && !chkView)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));

                var item = await wfServiceManager.WfLookupDataService.GetForUpdate<WfLookupDataDto>(id);
                return Ok(item);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(WfLookupDataDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(301, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                obj.SortNo ??= 0;
                var update = await wfServiceManager.WfLookupDataService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }


        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var chk = await permission.HasPermission(301, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await wfServiceManager.WfLookupDataService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }
    }
}
