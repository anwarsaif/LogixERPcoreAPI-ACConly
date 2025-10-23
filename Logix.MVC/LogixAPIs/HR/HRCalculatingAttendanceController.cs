using AutoMapper;
using Logix.Application.Common;
using Logix.Application.DTOs.ACC;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Services;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.HR
{
    //  إعادة أرسال البصمة
    public class HRCalculatingAttendanceController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IMainServiceManager mainServiceManager;
        private readonly ILocalizationService localization;
        private readonly IPermissionHelper permission;
        public HRCalculatingAttendanceController(IHrServiceManager hrServiceManager, ILocalizationService localization, IPermissionHelper permission, IMainServiceManager mainServiceManager)
        {
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.permission = permission;
            this.mainServiceManager = mainServiceManager;
        }
        #region Index Page

        [HttpPost("Resend")]
        public async Task<IActionResult> Resend(HrAttendanceResetDto obj)
        {
            var chk = await permission.HasPermission(2100, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                obj.Branch ??= 0;
                if (string.IsNullOrEmpty(obj.From))
                {
                    return Ok(await Result<object>.FailAsync("يجب ادخال من تاريخ "));

                }
                if (string.IsNullOrEmpty(obj.To))
                {
                    return Ok(await Result<object>.FailAsync("يجب ادخال الى تاريخ "));

                }
                if (string.IsNullOrEmpty(obj.EmpCode)) return Ok(await Result<object>.FailAsync(localization.GetResource1("EmployeeIsNumber")));

                var checkEmpExist = await mainServiceManager.InvestEmployeeService.GetOne(x => x.EmpId == obj.EmpCode && x.IsDeleted == false && x.Isdel == false);
                if (checkEmpExist.Data == null) return Ok(await Result<object>.FailAsync(localization.GetResource1("EmployeeNotFound")));
                obj.EmpId = checkEmpExist.Data.Id;

                var result = await hrServiceManager.HrAttendanceService.HR_Reaset_Attendance_SP(obj);
                return Ok(result);



            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync(ex.Message));
            }
        }

        #endregion

    }
}