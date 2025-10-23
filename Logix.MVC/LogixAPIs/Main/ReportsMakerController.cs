using Logix.Application.Common;
using Logix.Application.DTOs.ACC;
using Logix.Application.DTOs.RPT;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.RPT;
using Logix.MVC.Helpers;
using Logix.MVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace Logix.MVC.LogixAPIs.Main
{
    public class ReportsMakerController : BaseMainApiController
    {

        private readonly IRptServiceManager rptServiceManager;
        private readonly IMainServiceManager mainServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ILocalizationService localization;
        private readonly ICurrentData currentData;
        private readonly IDDListHelper listHelper;
        private readonly IApiDDLHelper ddlHelper;

        public ReportsMakerController(IRptServiceManager rptServiceManager,
            IMainServiceManager mainServiceManager,
             IPermissionHelper permission,
            ILocalizationService localization,
            ICurrentData currentData,
             IDDListHelper listHelper,
             IApiDDLHelper ddlHelper
            )
        {
            this.rptServiceManager = rptServiceManager;
            this.mainServiceManager = mainServiceManager;
            this.permission = permission;
            this.localization = localization;
            this.currentData = currentData;
            this.listHelper = listHelper;
            this.ddlHelper = ddlHelper;
        }
        #region "transactions"

        [HttpGet("DDLTable")]
        public async Task<IActionResult> DDLTable(int lang = 1, int sysId = 0)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<RptTable, long>(d => d.SystemId == sysId && d.IsDeleted == false, "Id", lang == 1 ? "Name" : "Name2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpPost("GetSysGroups")]
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
                        var sysgroupVM = new SysGroupVM
                        {
                            GroupId = item.GroupId ?? 0,
                            GroupName = item.GroupName,
                            GroupName2 = item.GroupName2 ?? item.GroupName
                        };
                        list.Add(sysgroupVM);
                    }
                    return Ok(await Result<List<SysGroupVM>>.SuccessAsync(list));
                }
                return Ok(sysGroups);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }

        #endregion   "transactions"

        #region "transactions_ADD"

        [HttpPost("AddExpress")]
        public async Task<IActionResult> AddExpress(RptReportExpressAddDto obj)
        {
            var chk = await permission.HasPermission(922, PermissionType.Add);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<RptReportExpressAddDto>.FailAsync(localization.GetMessagesResource("dataRequire")));
                }

                var add = await rptServiceManager.RptReportService.AddExpress(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<RptReportExpressAddDto>.FailAsync($"======= Exp in Rpt Report Express   add: {ex.Message}"));
            }
        }

        [HttpPost("AddExternalLink")]
        public async Task<IActionResult> AddExternalLink(RptReportExternalLinkAddDto obj)
        {
            var chk = await permission.HasPermission(922, PermissionType.Add);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<RptReportExternalLinkAddDto>.FailAsync(localization.GetMessagesResource("dataRequire")));
                }

                var add = await rptServiceManager.RptReportService.AddExternalLink(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<RptReportExternalLinkAddDto>.FailAsync($"======= Exp in Rpt Externa lLink   add: {ex.Message}"));
            }
        }

        [HttpPost("AddBusinessIntelligence")]
        public async Task<IActionResult> AddBusinessIntelligence(RptReportBusinessIntelligenceAddDto obj)
        {
            var chk = await permission.HasPermission(922, PermissionType.Add);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<RptReportBusinessIntelligenceAddDto>.FailAsync(localization.GetMessagesResource("dataRequire")));
                }

                var add = await rptServiceManager.RptReportService.AddBusinessIntelligence(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<RptReportBusinessIntelligenceAddDto>.FailAsync($"======= Exp in Rpt Business Intelligence add: {ex.Message}"));
            }
        }
        #endregion "transactions_Add"

        #region ==================================== Search & Delete ====================================
        [HttpPost("Search")]
        public async Task<IActionResult> Search(RptReportFilterDto filter)
        {
            try
            {
                var chk = await permission.HasPermission(922, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var items = await rptServiceManager.RptReportService.Search(filter);
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<RptReportsVw>.FailAsync($"======= Exp, MESSAGE: {ex.Message}"));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long Id)
        {
            try
            {
                var chk = await permission.HasPermission(922, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var delete = await rptServiceManager.RptReportService.Remove(Id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccAccountDto>.FailAsync($"======= Exp, MESSAGE: {ex.Message}"));
            }
        }
        #endregion ================================ End Search & Delete =================================

        [HttpPost("UpdateReportGroups")]
        public async Task<IActionResult> UpdateReportGroups(long Id, string? GroupsId)
        {
            try
            {
                var chk = await permission.HasPermission(922, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var update = await rptServiceManager.RptReportService.UpdateReportGroups(Id, GroupsId);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result<RptReportsVw>.FailAsync($"======= Exp, MESSAGE: {ex.Message}"));
            }
        }

        #region "transactions_GetById"

        [HttpGet("GetByIdForEdit")]
        public async Task<IActionResult> GetByIdForEdit(long id)
        {
            try
            {

                var chk = await permission.HasPermission(922, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (id <= 0)
                {
                    return Ok(await Result<RptReportDto>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var getItem = await rptServiceManager.RptReportService.GetForUpdate<RptReportDto>(id);
                if (getItem.Succeeded)
                {
                    var obj = getItem.Data;



                    return Ok(await Result<RptReportDto>.SuccessAsync(obj, $""));
                }
                else
                {
                    return Ok(getItem);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<RptReportDto>.FailAsync($"====== Exp in GetByIdForEdit Reports Maker, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(long id)
        {
            try
            {
                var chk = await permission.HasPermission(922, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                {
                    return Ok(await Result<RptReportDto>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var getItem = await rptServiceManager.RptReportService.GetOne(s => s.Id == id && s.IsDeleted == false);
                if (getItem.Succeeded)
                {
                    var obj = getItem.Data;
                    return Ok(await Result<RptReportDto>.SuccessAsync(obj, $""));
                }
                else
                {
                    return Ok(getItem);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<RptReportDto>.FailAsync($"====== Exp in GetById Reports Maker, MESSAGE: {ex.Message}"));
            }
        }

        #endregion "transactions_GetById"





    }

}
