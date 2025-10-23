using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.HR
{

    // تقرير بالدخول والخروج للموظف
    public class HRCheckInOutController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;

        public HRCheckInOutController(IHrServiceManager hrServiceManager, IPermissionHelper permission)
        {
            this.permission = permission;
            this.hrServiceManager = hrServiceManager;
        }

        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrCheckInOutFilterDto filter)
        {
            var chk = await permission.HasPermission(1844, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var result = await hrServiceManager.HrActualAttendanceService.Search(filter);
                return Ok(result);
				//if (filter.ReportType == 1)
				//{
				//    var result = await hrServiceManager.HrActualAttendanceService.GetAttendanceDetailsReportForEmployee(filter);
				//    return Ok(result);

				//}
				//else if (filter.ReportType == 2)
				//{
				//    var result = await hrServiceManager.HrActualAttendanceService.GetAttendanceTotalReportForEmployee(filter);
				//    return Ok(result);

				//}
				//else
				//{
				//    return Ok(await Result<object>.FailAsync("يجب اختيار نوع التقرير اما  تفصيلي او اجمالي"));

				//}
			}
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync(ex.Message));
            }
        }

        [HttpPost("UpdateCheckInOut")]
        public async Task<IActionResult> UpdateCheckInOut(HrUpdateCheckINout Obj)
        {
            var chk = await permission.HasPermission(1844, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {

                var result = await hrServiceManager.HrCheckInOutService.UpdateCheckInOut(Obj);
                return Ok(result);


            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync(ex.Message));
            }
        }
    }
}