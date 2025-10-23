using Logix.Application.Common;
using Logix.Application.DTOs.WF;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Services;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Logix.MVC.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Logix.MVC.LogixAPIs.WF
{
    public class WfAppTypeController : BaseWfController
    {
        private readonly IWFServiceManager wfServiceManager;
        private readonly IMainServiceManager mainServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ILocalizationService localization;
        private readonly ICurrentData session;
        private readonly IDDListHelper listHelper;

        public WfAppTypeController(IWFServiceManager wfServiceManager,
            IPermissionHelper permission,
            ILocalizationService localization,
            ICurrentData session,
            IDDListHelper listHelper,
            IMainServiceManager mainServiceManager)
        {
            this.wfServiceManager = wfServiceManager;
            this.permission = permission;
            this.localization = localization;
            this.session = session;
            this.listHelper = listHelper;
            this.mainServiceManager = mainServiceManager;
        }

        [HttpPost("Search")]
        public async Task<ActionResult> Search(WfAppTypeFilterDto filter)
        {
            try
            {
                var chk = await permission.HasPermission(266, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                filter.SystemId ??= 0; filter.GroupId ??= 0;
                var items = await wfServiceManager.WfAppTypeService.GetAllVW(x => x.IsDeleted == false
                && (filter.SystemId == 0 || x.SystemId == filter.SystemId)
                && (filter.GroupId == 0 || x.GroupId == filter.GroupId)
                && (string.IsNullOrEmpty(filter.Name) || (!string.IsNullOrEmpty(x.Name) && x.Name.Contains(filter.Name)))
                && (string.IsNullOrEmpty(filter.Name2) || (!string.IsNullOrEmpty(x.Name2) && x.Name2.Contains(filter.Name2))));

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Add")]
        public async Task<ActionResult> Add(WfAppTypeDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(266, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var add = await wfServiceManager.WfAppTypeService.Add(obj);
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
                var chk = await permission.HasPermission(266, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));

                var item = await wfServiceManager.WfAppTypeService.GetForUpdate<WfAppTypeEditDto>(id);
                return Ok(item);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(WfAppTypeEditDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(266, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var update = await wfServiceManager.WfAppTypeService.Update(obj);
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
                var chk = await permission.HasPermission(266, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await wfServiceManager.WfAppTypeService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("DDLAppGroups")]
        public async Task<IActionResult> DDLAppGroups(long systemId)
        {
            try
            {
                int lang = session.Language;
                var list = new SelectList(new List<DDListItem<WfAppGroupDto>>());
                var allGroups = await wfServiceManager.WfAppGroupService.GetAll(x => x.IsDeleted == false && (systemId==0 || x.SystemId == systemId));
                if (allGroups.Succeeded && allGroups.Data.Any())
                {
                    var groups = allGroups.Data.OrderBy(x => x.SortNo).ToList();
                    list = listHelper.GetFromList<long>(groups.Select(s => new DDListItem<long> { Name = lang == 1 ? s.Name ?? "" : s.Name2 ?? "", Value = Convert.ToInt64(s.Id) }), hasDefault: false);
                    return Ok(await Result<SelectList>.SuccessAsync(list));
                }
                else
                    return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("GetSysGroups")]
        public async Task<IActionResult> GetSysGroups()
        {
            try
            {
                var sysGroups = await mainServiceManager.SysGroupService.GetAll(g => g.IsDeleted == false);
                if (sysGroups.Succeeded)
                {
                    List<SysGroupVM> list = new();
                    foreach (var item in sysGroups.Data)
                    {
                        var sysgroupVM = new SysGroupVM { GroupId = item.GroupId ?? 0, GroupName = item.GroupName };
                        list.Add(sysgroupVM);
                    }
                    return Ok(await Result<List<SysGroupVM>>.SuccessAsync(list));
                }
                return Ok(sysGroups);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp MESSAGE: {ex.Message}"));
            }
        }

    }
}
