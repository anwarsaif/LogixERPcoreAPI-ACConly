using Logix.Application.Common;
using Logix.Application.DTOs.TS;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Logix.MVC.LogixAPIs.TS
{
    public class TsMyTaskBoardController : BaseTsApiController
    {
        private readonly ITsServiceManager tsServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;

        public TsMyTaskBoardController(
            ITsServiceManager tsServiceManager,
            IPermissionHelper permission,
            ICurrentData session,
            ILocalizationService localization
            )
        {
            this.tsServiceManager = tsServiceManager;
            this.permission = permission;
            this.session = session;
            this.localization = localization;
        }
        #region "GetAll"

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var chk = await permission.HasPermission(1131, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var tasksStatus = await tsServiceManager.TsTaskService.GetAllTsTaskStatusVw(x => x.Isdel == false);
                var statisticsDtos = new List<TsMyTsBoardStatisticsDto>();
                foreach( var task in tasksStatus )
                {
                    var tasks = await tsServiceManager.TsTaskService.GetAll(x =>
                        x.Isdel ==false 
                        //&& x.StatusId != 4 && x.StatusId != 5 && x.StatusId != 6 
                        && x.StatusId == task.StatusId // Exclude Status IDs 4, 5, and 6
                    );
                    var ts = tasks?.Data?.Where(n =>
                        !string.IsNullOrEmpty(n.AssigneeToUserId) && // Check if AssigneeToUserId is not null or empty
                        n.AssigneeToUserId.Split(',').Contains(session.UserId.ToString()) // Check if UserId exists
                    ) ?? Enumerable.Empty<TsTaskDto>(); // Fallback to an empty enumerable if tasks.Data is null

                    statisticsDtos.Add(new TsMyTsBoardStatisticsDto
                    {
                        StatusId = task.StatusId,
                        StatusName = task.StatusName,
                        StatusName2 = task.StatusName2,
                        Color = task.Color,
                        Count = ts.Count(),
                        Icon = task.Icon,
                        tasks = ts.ToList(),
                    });
                }
                return Ok(await Result<object>.SuccessAsync(statisticsDtos, $"Search Completed {statisticsDtos.Count()}", 200));
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        #endregion "GetAll"

        #region "UpdateStatus"

        [HttpPost("UpdateStatus")]
        public async Task<IActionResult> UpdateStatus(TsTaskIdAndStatusDto obj)
        {
            if (!await permission.HasPermission(1131, PermissionType.Edit))
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<TsTaskIdAndStatusDto>.FailAsync(localization.GetMessagesResource("dataRequire")));
                }

                var Edit = await tsServiceManager.TsTaskService.UpdateStatus(obj);
                return Ok(Edit);
            }
            catch (Exception ex)
            {
                return Ok(await Result<TsTaskIdAndStatusDto>.FailAsync($"======= Exp in edit: {ex.Message}"));
            }
        }

        #endregion "UpdateStatus"
    }
}
