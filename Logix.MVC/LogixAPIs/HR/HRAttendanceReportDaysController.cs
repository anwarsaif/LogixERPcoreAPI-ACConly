using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.HR
{

    //  تقرير الحضور والانصراف اليومي
    public class HRAttendanceReportDaysController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;


        public HRAttendanceReportDaysController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session)
        {
            this.permission = permission;
            this.hrServiceManager = hrServiceManager;
            this.session = session; 
        }

        [HttpPost("Search")]
        public async Task<IActionResult> Search(HRAttendanceReport6FilterSP filter)
        {
            var chk = await permission.HasPermission(1559, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                //filter.Workinghours ??= 0;
                //filter.BranchId ??= 0;
                //if (filter.Workinghours <= 0)
                //{
                //    return Ok(await Result<object>.FailAsync("يجب ادخال  قراءة ساعة العمل"));
                //}
                //if (string.IsNullOrEmpty(filter.EmpCode))
                //{
                //    filter.EmpCode = null;
                //}
                //if (string.IsNullOrEmpty(filter.EmpName))
                //{
                //    filter.EmpName = "";
                //}
                //if (filter.Location <= 0 || filter.Location == null)
                //{
                //    filter.Location = 0;
                //}
                //if (filter.BranchId <= 0)
                //{
                //    filter.BranchId=0;
                //    filter.BranchsId = session.Branches;
                //}
                //else
                //{
                //    filter.BranchsId = "";
                //}
                //if (string.IsNullOrEmpty(filter.From)|| string.IsNullOrEmpty(filter.To))
                //{
                //    filter.From = "";
                //    filter.To = "";
                //}
                var items = await hrServiceManager.HrAttendanceService.HR_Attendance_Report6_SP(filter);

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HRAttendanceReport4Dto>.FailAsync(ex.Message));
            }
        }

    }
}