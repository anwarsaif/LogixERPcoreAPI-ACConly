using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.HR
{
    // رفع بيانات الحضور والانصراف
    public class HRCheckingStaffController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;


        public HRCheckingStaffController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session)
        {
            this.permission = permission;
            this.hrServiceManager = hrServiceManager;
            this.session = session;
        }

        [HttpPost("Search")]
        public async Task<IActionResult> Search(HRAttendanceCheckingStaffFilterDto filter)
        {
            var chk = await permission.HasPermission(1562, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                //filter.DeptId ??= 0;
                //if (string.IsNullOrEmpty(filter.Date))
                //{
                //    return Ok(await Result<object>.FailAsync("يجب ادخال التاريخ "));
                //}

                var items = await hrServiceManager.HrAttendanceService.GetEmployeesForUploadAttendances(filter);

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HRAttendanceCheckingStaffFilterDto>.FailAsync(ex.Message));
            }
        }


        [HttpPost("UploadAttendance")]
        public async Task<IActionResult> UploadAttendance(HRAttendanceUploadDto obj)
        {
            var chk = await permission.HasPermission(1562, PermissionType.Add);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (string.IsNullOrEmpty(obj.Date))
                {
                    return Ok(await Result<object>.FailAsync("يجب ادخال التاريخ "));
                }
                if (obj.Data.Any(o => string.IsNullOrEmpty(o.empCode)))
                {
                    return Ok(await Result<object>.FailAsync("يجب ادخال ارقام الموظفين كاملة "));
                }
                if (obj.Data.Any(o => o.Permission <= 0 && o.Absence <= 0 && o.NormalVacation <= 0))
                {
                    return Ok(await Result<object>.FailAsync("يجب تحديد خيار واحد على الأقل إما الغياب أو استئذان أو اجازة اعتيادية"));
                }
                if (obj.Data.Any(o => (o.Absence == 1 && o.Permission == 1 && o.NormalVacation == 1) ||(o.Absence == 1 && o.Permission == 1) ||(o.NormalVacation == 1 && o.Absence == 1) ||(o.Permission == 1 && o.Absence == 1)))
                {
                    return Ok(await Result<object>.FailAsync(" يجب إختيار خيار واحد فقط"));
                }
                var items = await hrServiceManager.HrAttendanceService.UploadAttendances(obj);

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HRAttendanceCheckingStaffFilterDto>.FailAsync(ex.Message));
            }
        }

    }
}