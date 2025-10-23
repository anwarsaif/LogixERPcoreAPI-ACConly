using Logix.Application.Common;
using Logix.Application.DTOs.WF;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.WF
{
    public class WfApplicationsAssignesReplyController : BaseWfController
    {
        private readonly IWFServiceManager wfServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ILocalizationService localization;

        public WfApplicationsAssignesReplyController(IWFServiceManager wfServiceManager,
            IPermissionHelper permission,
            ILocalizationService localization)
        {
            this.wfServiceManager = wfServiceManager;
            this.permission = permission;
            this.localization = localization;
        }

        //[HttpPost("Search")]
        //public async Task<ActionResult> Search(WfStepFilterDto filter)
        //{
        //    try
        //    {
        //        var chk = await permission.HasPermission(267, PermissionType.Show);
        //        if (!chk)
        //            return Ok(await Result.AccessDenied("AccessDenied"));

        //        filter.AppTypeId ??= 0;
        //        var items = await wfServiceManager.WfStepService.GetAllVW(x => x.IsDeleted == false
        //        && (filter.AppTypeId == 0 || x.AppTypeId == filter.AppTypeId)
        //        && (string.IsNullOrEmpty(filter.StepName) || (!string.IsNullOrEmpty(x.StepName) && x.StepName.Contains(filter.StepName)))
        //        && (string.IsNullOrEmpty(filter.StepName2) || (!string.IsNullOrEmpty(x.StepName2) && x.StepName2.Contains(filter.StepName2))));

        //        return Ok(items);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
        //    }
        //}

        [HttpPost("Add")]
        public async Task<ActionResult> Add(WfApplicationsAssignesReplyDto obj)
        {
            try
            {
                //var chk = await permission.HasPermission(267, PermissionType.Add);
                //if (!chk)
                //    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                //obj.LevelNo ??= 0; obj.CommitteeType ??= 0; obj.CommitteeId ??= 0; obj.DecisionsType ??= 0;
                var add = await wfServiceManager.WfApplicationsAssignesReplyService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }
        [HttpPost("ReplyAssignes")]
        public async Task<ActionResult> ReplyAssignes(WfApplicationsAssignesReplyDto obj)
        {
            try
            {
                //var chk = await permission.HasPermission(267, PermissionType.Add);
                //if (!chk)
                //    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                //obj.LevelNo ??= 0; obj.CommitteeType ??= 0; obj.CommitteeId ??= 0; obj.DecisionsType ??= 0;
                var add = await wfServiceManager.WfApplicationsAssignesReplyService.ReplyAssignes(obj);
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
                //var chk = await permission.HasPermission(267, PermissionType.Edit);
                //if (!chk)
                //    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));

                var item = await wfServiceManager.WfApplicationsAssignesReplyService.GetForUpdate<WfApplicationsAssignesReplyEditDto>(id);
                return Ok(item);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(WfApplicationsAssignesReplyEditDto obj)
        {
            try
            {
                //var chk = await permission.HasPermission(267, PermissionType.Edit);
                //if (!chk)
                //    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                //obj.LevelNo ??= 0; obj.CommitteeType ??= 0; obj.CommitteeId ??= 0; obj.DecisionsType ??= 0;
                var update = await wfServiceManager.WfApplicationsAssignesReplyService.Update(obj);
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
                //var chk = await permission.HasPermission(267, PermissionType.Delete);
                //if (!chk)
                //    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await wfServiceManager.WfApplicationsAssignesReplyService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }
    }
}
