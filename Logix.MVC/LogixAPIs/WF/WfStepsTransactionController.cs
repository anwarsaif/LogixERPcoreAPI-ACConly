using Logix.Application.Common;
using Logix.Application.DTOs.WF;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.WF;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.WF.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Logix.MVC.LogixAPIs.WF
{
    public class WfStepsTransactionController : BaseWfController
    {
        private readonly IWFServiceManager wfServiceManager;
        private readonly IMainServiceManager mainServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ILocalizationService localization;
        private readonly IApiDDLHelper ddlHelper;
        private readonly ICurrentData session;

        public WfStepsTransactionController(IWFServiceManager wfServiceManager,
            IMainServiceManager mainServiceManager,
            IPermissionHelper permission,
            ILocalizationService localization,
            IApiDDLHelper ddlHelper,
            ICurrentData session)
        {
            this.wfServiceManager = wfServiceManager;
            this.permission = permission;
            this.localization = localization;
            this.ddlHelper = ddlHelper;
            this.session = session;
            this.mainServiceManager = mainServiceManager;
        }

        [HttpPost("Search")]
        public async Task<ActionResult> Search(WfStepsTransactionFilterDto filter)
        {
            try
            {
                var chk = await permission.HasPermission(268, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                filter.AppTypeId ??= 0;
                var items = await wfServiceManager.WfStepsTransactionService.GetAllVW(x => x.IsDeleted == false
                && (filter.AppTypeId == 0 || x.AppTypeId == filter.AppTypeId));

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Add")]
        public async Task<ActionResult> Add(WfStepsTransactionDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(268, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var add = await wfServiceManager.WfStepsTransactionService.Add(obj);
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
                var chk = await permission.HasPermission(268, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));

                var item = await wfServiceManager.WfStepsTransactionService.GetForUpdate<WfStepsTransactionEditDto>(id);
                return Ok(item);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(WfStepsTransactionEditDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(268, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var update = await wfServiceManager.WfStepsTransactionService.Update(obj);
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
                var chk = await permission.HasPermission(268, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await wfServiceManager.WfStepsTransactionService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }


        [HttpGet("DDLWfStep")]
        public async Task<IActionResult> DDLWfAppType(int appTypeId)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<WfStep, int>(x => x.IsDeleted == false && x.AppTypeId == appTypeId,
                    "Id", session.Language == 1 ? "StepName" : "StepName2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("GetStatusAndGroups")]
        public async Task<IActionResult> GetStatusAndGroups()
        {
            try
            {
                AddStageVm obj = new();
                // get wfStatus
                var wfStatus = await wfServiceManager.WfStatusService.GetAll(x => x.IsDeleted == false);
                if (wfStatus.Succeeded)
                {
                    foreach (var item in wfStatus.Data)
                    {
                        obj.StatusChkBoxVms.Add(new WfStatusChkBoxVm()
                        {
                            Id = item.Id,
                            StatusName = session.Language == 1 ? item.StatusName : item.StatusName2
                        });
                    }
                }

                // get sysGroups
                var sysGroups = await mainServiceManager.SysGroupService.GetAll(g => g.IsDeleted == false);
                if (sysGroups.Succeeded)
                {
                    foreach (var item in sysGroups.Data)
                    {
                        obj.SysGroupChkBoxVms.Add(new SysGroupChkBoxVm()
                        {
                            GroupId = item.GroupId ?? 0,
                            GroupName = item.GroupName
                        });
                    }
                }

                return Ok(await Result<AddStageVm>.SuccessAsync(obj));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }
    }
}
