using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.HR
{

    //  ملخص الحضور والإنصراف خلال فترة
    public class HRAttendanceTotalFromToController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;


        public HRAttendanceTotalFromToController(IHrServiceManager hrServiceManager, IPermissionHelper permission)
        {
            this.permission = permission;
            this.hrServiceManager = hrServiceManager;
        }

        [HttpPost("Search")]
        public async Task<IActionResult> Search(HRAttendanceTotalReportFilterDto filter)
        {
            var chk = await permission.HasPermission(1559, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                //if (string.IsNullOrEmpty(filter.empCode))
                //{
                //    filter.empCode = "";
                //}
                //if (string.IsNullOrEmpty(filter.EmpName))
                //{
                //    filter.EmpName = "";
                //}
                //if (filter.Location <= 0 || filter.Location == null)
                //{
                //    filter.Location = 0;
                //}

                //if (string.IsNullOrEmpty(filter.From))
                //{
                //    filter.From = "";
                //}
                //if (string.IsNullOrEmpty(filter.To))
                //{
                //    filter.To = "";
                //}
                var items = await hrServiceManager.HrAttendanceService.HR_Attendance_TotalReportNew_SP(filter);

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HRAttendanceReport4Dto>.FailAsync(ex.Message));
            }
        }

    }
}