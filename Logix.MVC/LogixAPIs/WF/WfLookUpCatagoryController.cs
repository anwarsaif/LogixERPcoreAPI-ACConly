using Logix.Application.Common;
using Logix.Application.DTOs.WF;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.WF
{
    public class WfLookUpCatagoryController : BaseWfController
    {
        private readonly IWFServiceManager wfServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ILocalizationService localization;

        public WfLookUpCatagoryController(IWFServiceManager wfServiceManager,
            IPermissionHelper permission,
            ILocalizationService localization)
        {
            this.wfServiceManager = wfServiceManager;
            this.permission = permission;
            this.localization = localization;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult> GetAll()
        {
            try
            {
                var chk = await permission.HasPermission(302, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var items = await wfServiceManager.WfLookUpCatagoryService.GetAll(x => x.IsDeleted == false);
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Add")]
        public async Task<ActionResult> Add(WfLookUpCatagoryDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(302, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var add = await wfServiceManager.WfLookUpCatagoryService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }


        [HttpGet("GetByIdForEdit")]
        public async Task<IActionResult> GetByIdForEdit(int id)
        {
            try
            {
                var chk = await permission.HasPermission(302, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));

                var item = await wfServiceManager.WfLookUpCatagoryService.GetForUpdate<WfLookUpCatagoryDto>(id);
                return Ok(item);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(WfLookUpCatagoryDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(302, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var update = await wfServiceManager.WfLookUpCatagoryService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }


        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var chk = await permission.HasPermission(302, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await wfServiceManager.WfLookUpCatagoryService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }

    }
}
