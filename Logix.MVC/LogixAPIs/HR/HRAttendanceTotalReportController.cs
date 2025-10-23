using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.HR
{
    // تقرير بإجمالي الحضور والتأخير والإضافي للموظفين
    public class HRAttendanceTotalReportController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;

        public HRAttendanceTotalReportController(IHrServiceManager hrServiceManager, IPermissionHelper permission)
        {
            this.permission = permission;
            this.hrServiceManager = hrServiceManager;
        }

        [HttpPost("Search")]
        public async Task<IActionResult> Search(HRAttendanceTotalReportSPFilterDto filter)
        {
            var chk = await permission.HasPermission(652, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var result = await hrServiceManager.HrAttendanceService.HR_Attendance_TotalReport_SP(filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync(ex.Message));
            }
        }

    }
}