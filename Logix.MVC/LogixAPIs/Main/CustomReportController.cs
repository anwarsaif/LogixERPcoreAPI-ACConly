using Logix.Application.Common;
using Logix.Application.DTOs.RPT;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.RPT;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Logix.MVC.LogixAPIs.Main
{
    public class CustomReportController : BaseMainApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IRptServiceManager rptServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ILocalizationService localization;
        private readonly ICurrentData session;
        private readonly IFilesHelper filesHelper;
        private readonly IWebHostEnvironment env;
        private readonly IApiDDLHelper ddlHelper;

        public CustomReportController(IRptServiceManager rptServiceManager,
            IMainServiceManager mainServiceManager,
            IPermissionHelper permission,
            ILocalizationService localization,
            ICurrentData session,
            IFilesHelper filesHelper,
            IWebHostEnvironment env,
            IApiDDLHelper ddlHelper)
        {
            this.mainServiceManager = mainServiceManager;
            this.rptServiceManager = rptServiceManager;
            this.permission = permission;
            this.localization = localization;
            this.session = session;
            this.filesHelper = filesHelper;
            this.env = env;
            this.ddlHelper = ddlHelper;
        }

        [HttpPost("GetAll")]
        public async Task<ActionResult> GetAll(RptCustomReportFilterDto filter)
        {
            try
            {
                var chk = await permission.HasPermission(1042, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var items = await rptServiceManager.RptCustomReportService.GetAll(r => r.IsDeleted == false);
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();

                    if (!string.IsNullOrEmpty(filter.Name))
                        res = res.Where(r => r.Name != null && r.Name.Contains(filter.Name));
                    if (!string.IsNullOrEmpty(filter.Name2))
                        res = res.Where(r => r.Name2 != null && r.Name2.ToLower().Contains(filter.Name2.ToLower()));

                    var final = res.ToList();

                    return Ok(await Result<List<RptCustomReportDto>>.SuccessAsync(final));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in GetAlll CustomReportController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Search")]
        public async Task<ActionResult> Search(RptCustomReportFilterDto filter)
        {
            try
            {
                var chk = await permission.HasPermission(1042, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                long facilityId = session.FacilityId;
                filter.SystemId ??= 0; filter.ParentId ??= 0; filter.ScreenId ??= 0; filter.Name2 = (filter.Name2 ?? "").ToLower();

                List<long?> screenIds = new();
                if (filter.ScreenId > 0)
                {
                    screenIds.Add(filter.ScreenId);
                }
                else if (filter.ParentId > 0)
                {
                    var getScreens = await mainServiceManager.SysScreenService.GetAll(x => x.ScreenId, x => x.ParentId == filter.ParentId);
                    screenIds = getScreens.Data.ToList();
                }
                else if (filter.SystemId > 0)
                {
                    var getScreens = await mainServiceManager.SysScreenService.GetAll(x => x.ScreenId, x => x.SystemId == filter.SystemId);
                    screenIds = getScreens.Data.ToList();
                }

                var items = await rptServiceManager.RptCustomReportService.GetAll(x => x.IsDeleted == false
                && (x.FacilityId == facilityId || x.FacilityId == 0 || x.FacilityId == null)
                && (screenIds.Count == 0 || screenIds.Contains(x.ScreenId))
                && (string.IsNullOrEmpty(filter.Name) || (!string.IsNullOrEmpty(x.Name) && x.Name.Contains(filter.Name)))
                && (string.IsNullOrEmpty(filter.Name2) || (!string.IsNullOrEmpty(x.Name2) && x.Name2.ToLower().Contains(filter.Name2))));

                if (items.Succeeded)
                {
                    foreach (var item in items.Data)
                    {
                        var getScreen = await mainServiceManager.SysScreenService.GetOne(x => x.ScreenId == item.ScreenId);
                        if (getScreen.Succeeded)
                        {
                            var getSystem = await mainServiceManager.SysSystemService.GetOne(x => x.SystemId == getScreen.Data.SystemId);
                            if (getSystem.Succeeded)
                            {
                                item.ScreenName = getScreen.Data.ScreenName;
                                item.ScreenName2 = getScreen.Data.ScreenName2;
                                item.SystemName = getSystem.Data.SystemName;
                                item.SystemName2 = getSystem.Data.SystemName2;
                                item.Folder = getSystem.Data.Folder;

                                string screenUrl = getScreen.Data.ScreenUrl ?? "";
                                if (screenUrl.StartsWith('/'))
                                    item.ScreenUrl = screenUrl;
                                else
                                    item.ScreenUrl = $"{getSystem.Data.Folder}/{screenUrl}";
                            }
                        }
                    }
                }

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Add")]
        public async Task<ActionResult> Add(RptCustomReportDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(1042, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                if (string.IsNullOrEmpty(obj.UsersPermissionEdit)) obj.UsersPermissionEdit = "0";
                if (string.IsNullOrEmpty(obj.GoupsPermissionEdit)) obj.GoupsPermissionEdit = "0";
                if (string.IsNullOrEmpty(obj.UsersAccess)) obj.UsersAccess = "0";
                if (string.IsNullOrEmpty(obj.GoupsAccess)) obj.GoupsAccess = "0";

                var add = await rptServiceManager.RptCustomReportService.Add(obj);
                return Ok(add);

            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add CustomReportController, MESSAGE: {ex.Message}"));
            }
        }


        [HttpGet("GetByIdForEdit")]
        public async Task<IActionResult> GetByIdForEdit(long id)
        {
            try
            {
                var chk = await permission.HasPermission(1042, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var getReport = await rptServiceManager.RptCustomReportService.GetForUpdate<RptCustomReportEditDto>(id);
                if (getReport.Succeeded)
                {
                    var obj = new RptCustomReportEditDto();
                    obj = getReport.Data;

                    //get systemId & parentId to be selected in select lists
                    var screen = await mainServiceManager.SysScreenService.GetById(obj.ScreenId);
                    if (screen != null)
                    {
                        obj.SystemId = screen.Data.SystemId;
                        obj.ParentId = screen.Data.ParentId;
                    }
                    return Ok(Result<RptCustomReportEditDto>.SuccessAsync((obj)));
                }

                return Ok(getReport);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in GetByIdForEdit CustomReportController, MESSAGE: {ex.Message}"));
            }
        }


        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(RptCustomReportEditDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(1042, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var update = await rptServiceManager.RptCustomReportService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Edit SysTemplateController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var chk = await permission.HasPermission(1042, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await rptServiceManager.RptCustomReportService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete CustomReportController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("DDLReports")]
        public async Task<IActionResult> DDLReports(long screenId)
        {
            try
            {
                var list = new SelectList(new List<DDListItem<RptCustomReportDto>>());
                list = await ddlHelper.GetAnyLis<RptCustomReport, long>(x => x.IsDeleted == false && x.ReportType == 2 && x.IsMain == true && x.ScreenId == screenId
                    , "ReportId", (session.Language == 1) ? "Name" : "Name2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        //GetParentScreens, GetChildScreens => move to DDLApiController (DDLParentScreens, DDLChildScreens)

    }
}
