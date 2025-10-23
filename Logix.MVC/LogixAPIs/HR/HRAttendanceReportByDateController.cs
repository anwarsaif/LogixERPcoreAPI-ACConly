using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.HR
{
    // تقرير حضور وانصراف يومي
    public class HRAttendanceReportByDateController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;

        public HRAttendanceReportByDateController(IHrServiceManager hrServiceManager, IPermissionHelper permission)
        {
            this.permission = permission;
            this.hrServiceManager = hrServiceManager;
        }

        [HttpPost("Search")]
        public async Task<IActionResult> Search(AttendanceSummaryFilter filter)
        {
            var chk = await permission.HasPermission(1310, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var result = await hrServiceManager.HrAttendanceService.GetAttendanceReportByDate(filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync(ex.Message));
            }
        }


        [HttpPost("AddDelay")]
        public async Task<IActionResult> AddDelay(List<AddDelayDto> entities)
        {
            var chk = await permission.HasPermission(1310, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (entities.Count <= 0)
                {
                    return Ok(await Result<object>.FailAsync("يجب تحديد حقل واحد على الأقل"));


                }
                var result = await hrServiceManager.HrAttendanceService.AddDelayFromReport(entities);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync(ex.Message));
            }
        }
    }
}