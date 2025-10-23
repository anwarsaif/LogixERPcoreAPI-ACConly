using Humanizer;
using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Helpers;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Linq;
using Result = Logix.Application.Wrapper.Result;

namespace Logix.MVC.LogixAPIs.HR
{

    // تقرير الحضور والإنصراف الشهري
    public class HRAttendanceMonthly2Controller : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IMainServiceManager mainServiceManager;


        public HRAttendanceMonthly2Controller(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IMainServiceManager mainServiceManager)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.mainServiceManager = mainServiceManager;
        }


        #region IndexPage



        [HttpPost("Search")]
        public async Task<IActionResult> Search(HRAttendanceMonthly2FilterDto filter)
        {
            List<HRAttendanceMonthly2FilterDto> results = new List<HRAttendanceMonthly2FilterDto>();
            var chk = await permission.HasPermission(1846, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (string.IsNullOrEmpty(filter.From) || string.IsNullOrEmpty(filter.To))
                {
                    return Ok(await Result<object>.FailAsync("يجب إدخال تاريخ بداية ونهاية التقرير"));
                }

                int diff = (DateHelper.StringToDate(filter.To) - DateHelper.StringToDate(filter.From)).Days;
                if (!(diff <= 30))
                {
                    return Ok(await Result<object>.FailAsync("أقصى مدة يعمل بها التقرير شهر كامل"));
                }

                filter.BranchId ??= 0;
                filter.Location ??= 0;
                var BranchesList = session.Branches.Split(',');
                var employees = await hrServiceManager.HrEmployeeService.GetAllVW(e => e.IsDeleted == false && e.Isdel == false && e.FacilityId == session.FacilityId && e.StatusId == 1);
                var filteredData = employees.Data
                    .Where(e =>
                        (string.IsNullOrEmpty(filter.EmpName) || e.EmpName.ToLower().Contains(filter.EmpName.ToLower())) &&
                        (string.IsNullOrEmpty(filter.empCode) || e.EmpId == filter.empCode) &&
                        (filter.BranchId == 0 || e.BranchId == filter.BranchId) &&
                        (filter.Location == 0 || e.Location == filter.Location)
                    ).ToList();

                var empIds = filteredData.Select(e => e.Id).ToList();
                var startDate = DateHelper.StringToDate(filter.From);
                var endDate = DateHelper.StringToDate(filter.To);

                // Fetch all necessary data
                var permissions = await hrServiceManager.HrPermissionService.GetAllVW(p => p.IsDeleted == false && empIds.Contains((long)p.EmpId) && p.PermissionDate != null && p.PermissionDate != "");
                var attendances = await hrServiceManager.HrAttendanceService.GetAllVW(a => a.IsDeleted == false && empIds.Contains((long)a.EmpId) && a.DayDateGregorian != null && a.DayDateGregorian != "");
                var vacations = await hrServiceManager.HrVacationsService.GetAllVW(v => v.IsDeleted == false && empIds.Contains((long)v.EmpId) && v.VacationSdate != null && v.VacationSdate != "" && v.VacationEdate != null && v.VacationEdate != "");
                var mandates = await hrServiceManager.HrMandateService.GetAllVW(m => m.IsDeleted == false && empIds.Contains((long)m.EmpId) && m.FromDate != null && m.FromDate != "" && m.ToDate != null && m.ToDate != "");
                var holidays = await hrServiceManager.HrHolidayService.GetAll(h => h.IsDeleted == false && h.HolidayDateFrom != null && h.HolidayDateFrom != "" && h.HolidayDateTo != null && h.HolidayDateTo != "");
                var assignments = await hrServiceManager.HrAssignmenService.GetAllVW(a => a.IsDeleted == false && empIds.Contains((long)a.EmpId) && a.FromDate != null && a.FromDate != "" && a.ToDate != null && a.ToDate != "");

                //var filteredPermissions = permissions.Data.Where(p => DateHelper.StringToDate1(p.PermissionDate) >= startDate && DateHelper.StringToDate1(p.PermissionDate) <= endDate);
                //var filteredAttendances = attendances.Data.Where(a => startDate >= DateHelper.StringToDate1(a.DayDateGregorian) && startDate <= DateHelper.StringToDate1(a.DayDateGregorian));
                //var filteredVacations = vacations.Data.Where(v => startDate >= DateHelper.StringToDate1(v.VacationSdate) && startDate <= DateHelper.StringToDate1(v.VacationEdate));
                //var filteredMandates = mandates.Data.Where(m => startDate >= DateHelper.StringToDate1(m.FromDate) && startDate <= DateHelper.StringToDate1(m.ToDate));
                //var filteredHolidays = holidays.Data.Where(h => startDate >= DateHelper.StringToDate1(h.HolidayDateFrom) && startDate <= DateHelper.StringToDate1(h.HolidayDateTo));
                //var filteredAssignments = assignments.Data.Where(a => startDate >= DateHelper.StringToDate1(a.FromDate) && startDate <= DateHelper.StringToDate1(a.ToDate));
                var filteredPermissions = permissions.Data.Where(p => DateHelper.StringToDate(p.PermissionDate) >= startDate && DateHelper.StringToDate(p.PermissionDate) <= endDate);
                var filteredAttendances = attendances.Data.Where(a => DateHelper.StringToDate(a.DayDateGregorian) >= startDate && DateHelper.StringToDate(a.DayDateGregorian) <= endDate);
                var filteredVacations = vacations.Data.Where(v => DateHelper.StringToDate(v.VacationSdate) <= endDate && DateHelper.StringToDate(v.VacationEdate) >= startDate);
                var filteredMandates = mandates.Data.Where(m => DateHelper.StringToDate(m.FromDate) <= endDate && DateHelper.StringToDate(m.ToDate) >= startDate);
                var filteredHolidays = holidays.Data.Where(h => DateHelper.StringToDate(h.HolidayDateFrom) <= endDate && DateHelper.StringToDate(h.HolidayDateTo) >= startDate);
                var filteredAssignments = assignments.Data.Where(a => DateHelper.StringToDate(a.FromDate) <= endDate && DateHelper.StringToDate(a.ToDate) >= startDate);

                foreach (var item in filteredData)
                {
                    var attendanceInfos = new List<AttendanceInfo>();
                    var currentDate = startDate;

                    while (currentDate <= endDate)
                    {
                        var checkAttendance = "A"; // Default value

                        // الإستئذانات
                        if (filteredPermissions.Any(p => p.EmpCode == item.EmpId && DateHelper.StringToDate(p.PermissionDate) == currentDate))
                        {
                            checkAttendance = "P";
                        }
                        // التشيك على الدوام الرسمي للموظف
                        if (filteredAttendances.Any(a => a.EmpCode == item.EmpId && DateHelper.StringToDate(a.DayDateGregorian) == currentDate))
                        {
                            checkAttendance = "T";
                        }

                        // التشيك على الإجازات
                        if (filteredVacations.Any(v => v.EmpCode == item.EmpId && DateHelper.StringToDate(v.VacationSdate) <= currentDate && DateHelper.StringToDate(v.VacationEdate) >= currentDate))
                        {
                            checkAttendance = "V";
                        }

                        // انتداب
                        if (filteredMandates.Any(m => m.EmpCode == item.EmpId && DateHelper.StringToDate(m.FromDate) <= currentDate && DateHelper.StringToDate(m.ToDate) >= currentDate))
                        {
                            checkAttendance = "BT";
                        }

                        // العطل الرسمية
                        if (filteredHolidays.Any(h => DateHelper.StringToDate(h.HolidayDateFrom) <= currentDate && DateHelper.StringToDate(h.HolidayDateTo) >= currentDate))
                        {
                            checkAttendance = "H";
                        }

                        // مهمات العمل
                        if (filteredAssignments.Any(a => a.EmpCode == item.EmpId && DateHelper.StringToDate(a.FromDate) <= currentDate && DateHelper.StringToDate(a.ToDate) >= currentDate))
                        {
                            checkAttendance = "As";
                        }

                        attendanceInfos.Add(new AttendanceInfo
                        {
                            DayName = session.Language == 1 ? currentDate.ToString("dddd", new CultureInfo("ar-SA")) : currentDate.ToString("dddd", new CultureInfo("en-US")),
                            IsPreesent = checkAttendance
                        });

                        currentDate = currentDate.AddDays(1);
                    }

                    var employeeDto = new HRAttendanceMonthly2FilterDto
                    {
                        empCode = item.EmpId,
                        EmpName = item.EmpName ?? item.EmpName2,
                        JobName = item.CatName ?? item.CatName2,
                        Info = attendanceInfos
                    };

                    results.Add(employeeDto);
                }

                if (results.Count > 0)
                    return Ok(await Result<List<HRAttendanceMonthly2FilterDto>>.SuccessAsync(results, ""));
                else
                    return Ok(await Result<List<HRAttendanceMonthly2FilterDto>>.SuccessAsync(results, localization.GetResource1("NosearchResult")));
            }
            catch (Exception ex)
            {
                return Ok(await Result<HRAttendanceMonthly2FilterDto>.FailAsync(ex.Message));
            }
        }



        #endregion

    }
}
