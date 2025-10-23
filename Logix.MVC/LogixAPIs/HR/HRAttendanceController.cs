using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.Infrastructure.Repositories;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.HR
{

    // التحضير  
    public class HRAttendanceController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;


        public HRAttendanceController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
        }

        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrAttendancesFilterDto filter)
        {
            var chk = await permission.HasPermission(589, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var searchResult = await hrServiceManager.HrAttendanceService.AttendanceSearch(filter);
                return Ok(searchResult);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrAttendancesFilterDto>.FailAsync(ex.Message));
            }
        }

        [HttpPost("Add")]
        public async Task<ActionResult> Add(HrAttendanceDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(589, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (string.IsNullOrEmpty(obj.EmpCode)) return Ok(await Result.FailAsync($"يجب ادخال رقم  الموظف"));
                if (string.IsNullOrEmpty(obj.TxtDate)) return Ok(await Result.FailAsync($"يجب ادخال التاريخ "));
                if (obj.AttType <= 0) return Ok(await Result.FailAsync($"يجب ادخال نوع  التحضير "));
                var add = await hrServiceManager.HrAttendanceService.HR_Attendance_SP_CmdType_1(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add Hr Attendance  Controller, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(HrAttendanceEditDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(589, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result<HrAttendanceEditDto>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));
                if (string.IsNullOrEmpty(obj.DayDateGregorian)) return Ok(await Result.FailAsync($"يجب ادخال التاريخ "));
                if (string.IsNullOrEmpty(obj.TimeInString)) return Ok(await Result.FailAsync($"يجب ادخال وقت الدخول "));
                if (string.IsNullOrEmpty(obj.TimeOutString)) return Ok(await Result.FailAsync($"يجب ادخال وقت الخروج "));

                var update = await hrServiceManager.HrAttendanceService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrAttendanceEditDto>.FailAsync($"====== Exp in Hr Hr Attendance  Controller getById, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long AttendanceId)
        {
            try
            {
                var chk = await permission.HasPermission(589, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (AttendanceId <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var item = await hrServiceManager.HrAttendanceService.GetOneVW(x => x.AttendanceId == AttendanceId);
                return Ok(item);

            }
            catch (Exception ex)
            {
                return Ok(await Result<HrDecisionsVw>.FailAsync($"====== Exp in Hr Attendance Controller getById, MESSAGE: {ex.Message}"));
            }
        }



        #region this Region is for page of name Attendance_Add2 (تحضير متعدد)


        [HttpPost("SearchForMultiAdd")]
        public async Task<ActionResult> SearchForMultiAdd(HRAddMultiAttendanceFilterDto filter)
        {
            try
            {
                var chk = await permission.HasPermission(589, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result<HRAddMultiAttendanceDto>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));
                filter.BranchId ??= 0;
                filter.Location ??= 0;
                filter.DeptId ??= 0;
                filter.TimeTableId ??= 0;
                if (string.IsNullOrEmpty(filter.EmpCode))
                {
                    filter.EmpCode = null;
                }
                if (string.IsNullOrEmpty(filter.DayDateGregorian))
                {
                    return Ok(await Result<HRAddMultiAttendanceDto>.FailAsync($"DayDateGregorian IS Required"));
                }
                if (filter.AttendanceType<=0)
                {
                    return Ok(await Result<HRAddMultiAttendanceDto>.FailAsync($"attendanceType IS Required"));
                }
                var result = await hrServiceManager.HrAttendanceService.AttendanceSearchForMultiAdd(filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HRAddMultiAttendanceDto>.FailAsync($"====== Exp in Hr Hr Attendance  Controller SearchForMultiAdd, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("MultiAdd")]
        public async Task<ActionResult> MultiAdd(List<HrMultiAddDto> obj)
        {
            try
            {
                var chk = await permission.HasPermission(589, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                foreach (var item in obj)
                {
                    if (string.IsNullOrEmpty(item.EmpCode))
                        return Ok(await Result.FailAsync("يجب ادخال رقم الموظف"));
                }
                foreach (var item in obj)
                {
                    if (string.IsNullOrEmpty(item.TimeText))
                        return Ok(await Result.FailAsync($"يجب ادخال الوقت للموظف رقم :{item.EmpCode} "));
                }
                foreach (var item in obj)
                {
                    if (string.IsNullOrEmpty(item.TxtDate))
                        return Ok(await Result.FailAsync($"يجب ادخال التاريخ  للموظف رقم :{item.EmpCode} "));
                }
                foreach (var item in obj)
                {
                    if (item.AttType != 1 && item.AttType != 2)
                        return Ok(await Result.FailAsync($"يجب ادخال نوع التحضير  للموظف رقم :{item.EmpCode} "));
                }
                var add = await hrServiceManager.HrAttendanceService.MultiAdd(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add Hr Attendance  Controller, MESSAGE: {ex.Message}"));
            }
        }

        #endregion



        #region this Region is for page of name Add From Excel (اضافة من الاكسل)

        [HttpPost("AddAttendanceFromExcel")]
        public async Task<ActionResult> AddAttendanceFromExcel(List<AddAttendanceFromExcelDto> obj)
        {
            try
            {
                var chk = await permission.HasPermission(589, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));
                foreach (var item in obj)
                {
                    if (string.IsNullOrEmpty(item.EmpCode))
                        return Ok(await Result.FailAsync("يجب ادخال رقم الموظف"));
                }
                foreach (var item in obj)
                {
                    if (string.IsNullOrEmpty(item.TimeText))
                        return Ok(await Result.FailAsync($"يجب ادخال الوقت للموظف رقم :{item.EmpCode} "));
                }
                foreach (var item in obj)
                {
                    if (string.IsNullOrEmpty(item.TxtDate))
                        return Ok(await Result.FailAsync($"يجب ادخال التاريخ  للموظف رقم :{item.EmpCode} "));
                }
                foreach (var item in obj)
                {
                    if (item.AttType != 1 && item.AttType != 2)
                        return Ok(await Result.FailAsync($"يجب ادخال نوع التحضير  للموظف رقم :{item.EmpCode} "));
                }
                var add = await hrServiceManager.HrAttendanceService.AddAttendanceFromExcel(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add Hr Attendance Controller in function AddAttendanceFromExcel, MESSAGE: {ex.Message}"));
            }
        }

        #endregion
    }
}