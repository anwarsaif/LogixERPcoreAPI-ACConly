using Logix.Application.Common;
using Logix.Application.DTOs.Main;
using Logix.Application.DTOs.TS;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Services;
using Logix.Application.Wrapper;
using Logix.Infrastructure.Repositories;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.TS
{
    public class TsCalendarForManagerController : BaseTsApiController
    {
        private readonly ITsServiceManager tsServiceManager;
        private readonly ICurrentData session;
        private readonly IPermissionHelper permission;

        public TsCalendarForManagerController(ITsServiceManager tsServiceManager, ICurrentData session,
            IPermissionHelper permission)
        {
            this.tsServiceManager = tsServiceManager;
            this.session = session;
            this.permission = permission;
        }

        [HttpGet("RetrieveUsersForAdmin")]
        public async Task<IActionResult> RetrieveUsersForAdmin()
        {
            try
            {
                var chk = await permission.HasPermission(1097, PermissionType.Show);

                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                var items = await tsServiceManager.TsAppointmentService.RetrieveUsersForAdmin();

                return Ok(await Result<IEnumerable<SysUserDto>>.SuccessAsync(items, $"Search Completed {items.Count()}", 200));
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        [HttpGet("GetFullUserName")]
        public async Task<IActionResult> GetFullUserName(long userId)
        {
            try
            {
                var chk = await permission.HasPermission(1097, PermissionType.Show);

                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                var items = await tsServiceManager.TsAppointmentService.GetFullUserName(userId);

                return Ok(await Result<SysUserDto>.SuccessAsync(items, $"Search Completed", 200));
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        [HttpPost("SaveEvent")]
        public async Task<IActionResult> SaveEvent(SaveEventDto entity)
        {
            try
            {
                var chk = await permission.HasPermission(1097, PermissionType.Add);

                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                await tsServiceManager.TsAppointmentService.SaveEvent(entity);

                return Ok(200);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        [HttpPost("UpdateDragEvents")]
        public async Task<IActionResult> UpdateDragEvents(long id, string startDate, bool allDay)
        {
            try
            {
                var chk = await permission.HasPermission(1097, PermissionType.Edit);

                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                await tsServiceManager.TsAppointmentService.UpdateDragEvents(id, startDate, allDay);

                return Ok(200);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        [HttpPost("UpdateTitle")]
        public async Task<IActionResult> UpdateTitle(UpdateTitleDto entity)
        {
            try
            {
                var chk = await permission.HasPermission(1097, PermissionType.Edit);

                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                await tsServiceManager.TsAppointmentService.UpdateTitle(entity);

                return Ok(200);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        [HttpPost("EventResizable")]
        public async Task<IActionResult> EventResizable(EventResizableDto entity)
        {
            try
            {
                var chk = await permission.HasPermission(1097, PermissionType.Edit);

                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                await tsServiceManager.TsAppointmentService.EventResizable(entity);

                return Ok(200);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        [HttpGet("RetrieveEventsManager")]
        public async Task<IActionResult> RetrieveEventsManager()
        {
            try
            {
                var chk = await permission.HasPermission(1097, PermissionType.Show);

                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                var SessionEmpID = session.UserId;
                var items = await tsServiceManager.TsAppointmentService.GetAllVW(x=> x.ManagerId == SessionEmpID && x.IsDeleted==false);

                var itemsAsQueryable = items.Data.AsQueryable();

                var tsAppointmentsObject = itemsAsQueryable.AsEnumerable().Select(ts =>
                {
                    TimeSpan? appStartTimes = TimeSpan.TryParse(ts.AppStartTime, out var startTime) ? startTime : (TimeSpan?)null;
                    TimeSpan? appEndTimes = TimeSpan.TryParse(ts.AppEndTime, out var endTime) ? endTime : (TimeSpan?)null; return new
                    {
                        ts.AppId,
                        ts.AppDetails,
                        ts.AppStartDate,
                        AppStartTimes = appStartTimes,
                        ts.AppEndDate,
                        AppEndTimes = appEndTimes,
                        ts.UserId,
                        ts.AppDate,
                        ts.TicketType,
                        ts.IsDeleted,
                        ts.AllDay,
                        ts.AppStartDateAh,
                        ts.AppEndDateAh
                    };
                }).ToList();

                return Ok(await Result<IEnumerable<object>>.SuccessAsync(tsAppointmentsObject, $"Search Completed", 200));
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }
    }
}
