using Logix.Application.Common;
using Logix.Application.DTOs.Main;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.Main;
using Logix.Domain.WF;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Logix.MVC.LogixAPIs.Main
{
    public class SysGroupController : BaseMainApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IWFServiceManager wfServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IApiDDLHelper ddlHelper;

        public SysGroupController(IMainServiceManager mainServiceManager,
            IWFServiceManager wfServiceManager,
            IPermissionHelper permission,
            ILocalizationService localization,
            ICurrentData session,
            IApiDDLHelper ddlHelper)
        {
            this.mainServiceManager = mainServiceManager;
            this.wfServiceManager = wfServiceManager;
            this.permission = permission;
            this.session = session;
            this.localization = localization;
            this.ddlHelper = ddlHelper;
        }

        #region ===================== Basic Functions (Add, Edit, Delete) =================
        [HttpPost("Search")]
        public async Task<ActionResult> Search(SysGroupFilterDto filter)
        {
            try
            {
                var chk = await permission.HasPermission(116, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                filter.SystemId ??= 0;
                var items = await mainServiceManager.SysGroupService.GetAllVW(g => g.FacilityId == session.FacilityId
                && g.IsDel == false
                && (filter.SystemId == 0 || (g.SystemId == filter.SystemId))
                && (string.IsNullOrEmpty(filter.GroupName) || (!string.IsNullOrEmpty(g.GroupName) && g.GroupName.Contains(filter.GroupName)))
                && (string.IsNullOrEmpty(filter.GroupName2) || (!string.IsNullOrEmpty(g.GroupName2) && g.GroupName2.ToLower().Contains(filter.GroupName2)))
                );
                if (items.Succeeded)
                {
                    var res = items.Data;

                    if (session.GroupId != 1)
                    {
                        var userGroup = await mainServiceManager.SysGroupService.GetById(session.GroupId);
                        if (userGroup.Succeeded && userGroup.Data != null)
                        {
                            res = res.Where(g => g.SystemId > 0 && g.SystemId == userGroup.Data.SystemId);
                        }
                        else
                        {
                            return Ok(userGroup);
                        }
                    }
                    var final = res.ToList();

                    List<SysGroupFilterDto> results = new List<SysGroupFilterDto>();
                    foreach (var item in final)
                    {
                        results.Add(new SysGroupFilterDto
                        {
                            GroupId = item.GroupId,
                            GroupName = item.GroupName,
                            GroupName2 = item.GroupName2,
                            SystemId = item.SystemId,
                            SystemName = item.SystemName,
                            SystemName2 = item.SystemName2
                        });
                    }
                    return Ok(await Result<List<SysGroupFilterDto>>.SuccessAsync(results));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in GetAlll SysGroupController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Add")]
        public async Task<ActionResult> Add(SysGroupDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(116, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));
                obj.AppStatusTo ??= ""; obj.AppStatusFrom ??= "";
                var add = await mainServiceManager.SysGroupService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add SysGroupController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("EditBasicData")]
        public async Task<ActionResult> EditBasicData(SysGroupEditDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(116, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var update = await mainServiceManager.SysGroupService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in EditBasicData SysGroupController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetByIdForEdit")]
        public async Task<IActionResult> GetByIdForEdit(int id)
        {
            try
            {
                var chk = await permission.HasPermission(116, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var groupData = await mainServiceManager.SysGroupService.GetForUpdate<SysGroupEditDto>(id);
                if (groupData.Succeeded)
                {
                    var chkHasAutoSys = await mainServiceManager.SysSystemService.GetOne(s => s.SystemId == 43 && s.Isdel == false);
                    if (chkHasAutoSys.Succeeded && chkHasAutoSys.Data != null)
                        groupData.Data.HasAutomationSys = true;

                    return Ok(groupData);
                }
                return Ok(groupData);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in GetByIdForEdit SysGroupController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var chk = await permission.HasPermission(116, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await mainServiceManager.SysGroupService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete SysGroupController, MESSAGE: {ex.Message}"));
            }
        }

        #endregion

        #region =========== Re-fill Tables in Partial Views (screens & forms data) ========

        [HttpGet("RefillTable")]
        public async Task<IActionResult> RefillTable(string? screensId, int groupId, int systemId)
        {
            try
            {
                string[] ScreensArray = Array.Empty<string>();
                if (!string.IsNullOrEmpty(screensId))
                    ScreensArray = screensId.Split(',');
                List<SysScreenPermissionDtoVM> vm = new();
                // نجلب الشاشات 
                var screenInstalled = await mainServiceManager.SysScreenInstalledService.GetAllVW(s => s.SystemId == systemId
                && (string.IsNullOrEmpty(screensId) || ScreensArray.Contains(s.ParentId.ToString())) && s.Isdel == false && s.IsDeleted == false);
                if (screenInstalled.Succeeded)
                {
                    foreach (var item in screenInstalled.Data)
                    {
                        var screenPermission = await mainServiceManager.SysScreenPermissionService.GetOne(p => p.ScreenId == item.ScreenId && p.GroupId == groupId);
                        if (screenPermission.Data != null)
                        {
                            var permission = screenPermission.Data;
                            //نعبي الصلاحيات كما هي،، حسب ما هو بالجدول
                            var SysScreenPermissionDto = new SysScreenPermissionDtoVM
                            {
                                ScreenId = item.ScreenId,
                                ParentId = item.ParentId,
                                GroupId = groupId,
                                PriveId = permission.PriveId,
                                ScreenName = item.ScreenName,
                                ScreenName2 = item.ScreenName2,
                                ScreenShow = permission.ScreenShow ?? false,
                                ScreenAdd = permission.ScreenAdd ?? false,
                                ScreenEdit = permission.ScreenEdit ?? false,
                                ScreenDelete = permission.ScreenDelete ?? false,
                                ScreenPrint = permission.ScreenPrint ?? false,

                                ScreenView = permission.ScreenView ?? false,
                                ScreenApproval = permission.ScreenApproval ?? false,
                                ScreenExport = permission.ScreenExport ?? false,
                                ScreenImport = permission.ScreenImport ?? false,
                                ScreenReject = permission.ScreenReject ?? false,
                            };
                            vm.Add(SysScreenPermissionDto);
                        }
                        else
                        {
                            //اذا الشاشة لم تحفظ في جدول الصلاحيات بعد
                            var SysScreenPermissionDto = new SysScreenPermissionDtoVM
                            {
                                ScreenId = item.ScreenId,
                                ParentId = item.ParentId,
                                GroupId = groupId,
                                ScreenName = item.ScreenName,
                                ScreenShow = false,
                                ScreenAdd = false,
                                ScreenEdit = false,
                                ScreenDelete = false,
                                ScreenPrint = false,
                                ScreenView = false,
                                ScreenApproval = false,
                                ScreenReject = false,
                                ScreenExport = false,
                                ScreenImport = false,
                            };
                            vm.Add(SysScreenPermissionDto);
                        }
                    }
                    return Ok(await Result<List<SysScreenPermissionDtoVM>>.SuccessAsync(vm));
                }

                return Ok(await Result.FailAsync($"{screenInstalled.Status.message}"));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in RefillTable SysGroupController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("RefillTableWithForms")]
        public async Task<IActionResult> RefillTableWithForms(string groupsId, int groupId, int systemId)
        {
            try
            {
                var groupsArray = groupsId.Split(',');
                List<FormsPermissionDtoVM> vm = new List<FormsPermissionDtoVM>();
                int lang = session.Language;
                // نجلب النماذج
                var forms = await wfServiceManager.WfAppTypeService.GetAllVW(f => f.IsDeleted == false && groupsArray.Contains(f.GroupId.ToString()) && f.SystemId == systemId);
                if (forms.Succeeded)
                {
                    foreach (var item in forms.Data)
                    {
                        var obj = new FormsPermissionDtoVM()
                        {
                            GroupId = groupId,
                            FormId = item.Id,
                            FormName = item.Name,
                            FormName2 = item.Name2,
                            FormQery = (!string.IsNullOrEmpty(item.SysGroupQuery)) && item.SysGroupQuery.Split(',').Contains(groupId.ToString()),
                            FormSend = (!string.IsNullOrEmpty(item.SysGroupId)) && item.SysGroupId.Split(',').Contains(groupId.ToString())
                        };

                        vm.Add(obj);
                    }
                    return Ok(await Result<List<FormsPermissionDtoVM>>.SuccessAsync(vm));
                }
                else
                {
                    return Ok(await Result.FailAsync($"{forms.Status.message}"));
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in RefillTableWithForms SysGroupController, MESSAGE: {ex.Message}"));
            }
        }

        #endregion

        #region =========== Edit Permissions on Sceens & on Automation Forms ==============
        [HttpPost("EditPermissions")]
        public async Task<ActionResult> EditPermissions(List<SysScreenPermissionDtoVM> obj)
        {
            try
            {
                var chk = await permission.HasPermission(116, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var update = await mainServiceManager.SysScreenPermissionService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in EditPermissions SysGroupController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("EditFormsPermissions")]
        public async Task<ActionResult> EditFormsPermissions(List<FormsPermissionDtoVM> obj)
        {
            try
            {
                var chk = await permission.HasPermission(116, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));
                //update wfAppType
                var update = await wfServiceManager.WfAppTypeService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in EditFormsPermissions SysGroupController, MESSAGE: {ex.Message}"));
            }
        }

        #endregion

        #region ============== Copying permission from another group ======================
        [HttpPost("Copy")]
        public async Task<IActionResult> Copy(CopyGroupVM obj)
        {
            try
            {
                var chk = await permission.HasPermission(116, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var copy = await mainServiceManager.SysGroupService.Copy(obj);
                return Ok(copy);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Copy SysGroupController, MESSAGE: {ex.Message}"));
            }
        }
        #endregion

        #region =========== Re-fill dropdown lists when change value ======================
        [HttpGet("GetParentScreens")]
        public async Task<IActionResult> GetParentScreens(int systemId)
        {
            try
            {
                if (systemId > 0)
                {
                    var list = new SelectList(new List<DDListItem<SysScreen>>());
                    list = await ddlHelper.GetAnyLis<SysScreen, long>(s => s.SystemId == Convert.ToInt32(systemId) && s.ParentId == s.ScreenId && s.Isdel == false, "ScreenId", (session.Language == 1) ? "ScreenName" : "ScreenName2");
                    return Ok(await Result<SelectList>.SuccessAsync(list));
                }

                return Ok(await Result.FailAsync("Invalid System Id"));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in GetParentScreens SysGroupController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetFormsGroup")]
        public async Task<IActionResult> GetFormsGroup(int systemId)
        {
            try
            {
                if (systemId > 0)
                {
                    var list = new SelectList(new List<DDListItem<WfAppGroup>>());
                    list = await ddlHelper.GetAnyLis<WfAppGroup, long>(s => s.SystemId == Convert.ToInt32(systemId) && s.IsDeleted == false, "Id", (session.Language == 1) ? "Name" : "Name2");
                    return Ok(await Result<SelectList>.SuccessAsync(list));
                }

                return Ok(await Result.FailAsync("Invalid System Id"));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in GetFormsGroup SysGroupController, MESSAGE: {ex.Message}"));
            }
        }
        #endregion

        [HttpGet("DDLWFAppGroup")]
        public async Task<IActionResult> DDLWFAppGroup(long systemId, int lang = 1)
        {
            try
            {
                if (systemId > 0)
                {
                    var list = new SelectList(new List<DDListItem<WfAppGroup>>());
                    list = await ddlHelper.GetAnyLis<WfAppGroup, long>(s => s.SystemId == systemId && s.IsDeleted == false, "Id", lang == 1 ? "Name" : "Name2");
                    return Ok(await Result<SelectList>.SuccessAsync(list));
                }

                return Ok(await Result.FailAsync("Invalid System Id"));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
    }
}
