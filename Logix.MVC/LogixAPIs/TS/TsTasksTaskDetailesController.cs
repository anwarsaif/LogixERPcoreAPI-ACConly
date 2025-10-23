using Logix.Application.Common;
using Logix.Application.DTOs.Main;
using Logix.Application.DTOs.PM;
using Logix.Application.DTOs.PUR;
using Logix.Application.DTOs.TS;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Services;
using Logix.Application.Wrapper;
using Logix.Domain.PUR;
using Logix.Domain.TS;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Logix.MVC.LogixAPIs.TS
{
    public class TsTasksTaskDetailesController : BaseTsApiController
    {
        private readonly IPermissionHelper permission;
        private readonly ITsServiceManager tsServiceManager;
        private readonly IPMServiceManager pmServiceManager;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IMainServiceManager mainServiceManager;

        public TsTasksTaskDetailesController(
            IPermissionHelper permission,
            ITsServiceManager tsServiceManager,
            IPMServiceManager pmServiceManager,
            ICurrentData session,
            ILocalizationService localization,
            IMainServiceManager mainServiceManager)
        {
            this.permission = permission;
            this.tsServiceManager = tsServiceManager;
            this.pmServiceManager = pmServiceManager;
            this.session = session;
            this.localization = localization;
            this.mainServiceManager = mainServiceManager;
        }

        #region "Edit"
        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(TsTaskEditDto obj)
        {
            if (!(await permission.HasPermission(243, PermissionType.Edit)) && !(await permission.HasPermission(956, PermissionType.Edit)))
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<TsTaskEditDto>.FailAsync(localization.GetMessagesResource("dataRequire")));
                }

                var Edit = await tsServiceManager.TsTaskService.Update(obj);
                return Ok(Edit);
            }
            catch (Exception ex)
            {
                return Ok(await Result<TsTaskEditDto>.FailAsync($"======= Exp in edit: {ex.Message}"));
            }
        }
        #endregion "Edit"

        #region "ApproveTask"
        [HttpPost("ApproveTask")]
        public async Task<IActionResult> ApproveTask(TsTaskDetailsEditDto obj)
        {
            if (!(await permission.HasPermission(243, PermissionType.Edit)) && !(await permission.HasPermission(956, PermissionType.Edit)))
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<TsTaskDetailsEditDto>.FailAsync(localization.GetMessagesResource("dataRequire")));
                }
                var Edit = await tsServiceManager.TsTaskService.ApproveTask(obj);
                if (Edit.Succeeded)
                {
                    return Ok(Edit);
                }
                else
                {
                    return Ok(await Result<TsTaskDetailsEditDto>.FailAsync(localization.GetResource1("UpdateError")));
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<TsTaskDetailsEditDto>.FailAsync($"======= Exp in PurItemsPriceM edit: {ex.Message}"));
            }
        }
        #endregion "ApproveTask"

        #region "GetByIdForEdit"

        [HttpGet("GetByIdForEdit")]
        public async Task<IActionResult> GetByIdForEdit(long id)
        {
            try
            {
                if (!(await permission.HasPermission(243, PermissionType.Edit)) && !(await permission.HasPermission(956, PermissionType.Edit)))
                {
                    return Ok(await Result.AccessDenied("AccessDenied"));
                }

                if (id <= 0)
                {
                    return Ok(await Result<TsTaskDetailsVwDto>.FailAsync(localization.GetMessagesResource("InCorrectId")));
                }
                var getItem = await tsServiceManager.TsTaskService.GetOneVW(x=>x.Id == id);
                if (getItem.Succeeded)
                {
                    var obj = new TsTaskDetailsVwDto();
                    if (!string.IsNullOrEmpty(getItem.Data.AssigneeToGroupId))
                    {
                        var groupId = getItem.Data.AssigneeToGroupId.Split(',');
                        var AssigneeToGroupId = await mainServiceManager.SysGroupService.GetAll(
                        x => x.IsDeleted == false && groupId.Contains(x.GroupId.ToString()));
                        
                        if (AssigneeToGroupId?.Data != null)
                        {
                            obj.AssigneeToGroupId = AssigneeToGroupId.Data.Select(g => new SysGroupIdNameDto
                            {
                                GroupId = g.GroupId,
                                GroupName = g.GroupName,
                                GroupName2 = g.GroupName,

                            }).ToList();
                        }
                    }

                    if (!string.IsNullOrEmpty(getItem.Data.AssigneeToUserId))
                    {
                        var userId = getItem.Data.AssigneeToUserId.Split(',');
                        var AssigneeToUserId = await mainServiceManager.SysUserService.GetAll(
                        x => x.IsDeleted ==false && userId.Contains(x.Id.ToString()));
                        
                        if (AssigneeToUserId?.Data != null)
                        {
                            obj.AssigneeToUserId = AssigneeToUserId.Data.Select(g => new UserIdNameDto
                            {
                                UserId = g.Id,
                                UserName = g.UserFullname
                            }).ToList();
                        }
                    }
                    obj.Task = getItem.Data;
                    var taskDetails = await tsServiceManager.TsTasksResponseService.GetAllVW(x => x.Isdel == false && x.TaskId == id);
                    obj.TaskResponses = taskDetails.Data?.ToList();
                    var files = await pmServiceManager.PMProjectsFileService.GetAllVW(x => x.IsDeleted == false && x.TaskId == id);
                    obj.Files = files.Data?.ToList();
                    return Ok(await Result<TsTaskDetailsVwDto>.SuccessAsync(obj, $""));
                }
                return Ok(getItem);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }

        #endregion "GetByIdForEdit"

        #region "AddFileTask"
        [HttpPost("AddFileTask")]
        public async Task<IActionResult> AddFileTask(PmProjectsFileDto obj)
        {
            if (!(await permission.HasPermission(243, PermissionType.Edit)) || !(await permission.HasPermission(956, PermissionType.Edit)))
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<PmProjectsFileDto>.FailAsync(localization.GetMessagesResource("dataRequire")));
                }
                obj.ProjectId = 0;
                obj.ParentId = 0;
                obj.CopyNo = 0;
                var Edit = await pmServiceManager.PMProjectsFileService.Add(obj);
                if (Edit.Succeeded)
                {
                    return Ok(Edit);
                }
                else
                {
                    return Ok(await Result<PmProjectsFileDto>.FailAsync(localization.GetResource1("UpdateError")));
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<PmProjectsFileDto>.FailAsync($"======= Exp in PurItemsPriceM edit: {ex.Message}"));
            }
        }
        #endregion "AddFileTask"
    }
}
