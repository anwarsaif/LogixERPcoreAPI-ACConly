using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.DTOs.Main;
using Logix.Application.DTOs.RPT;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Services;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Linq;

namespace Logix.MVC.LogixAPIs.HR
{
    public class HrDashboardApiController : BaseHrApiController
    {
        private readonly IAccServiceManager accServiceManager;
        private readonly IMainServiceManager mainServiceManager;
        private readonly IHrServiceManager hrServiceManager;
        private readonly ICurrentData session;
        private readonly int _SystemId = 3;

        public HrDashboardApiController(IAccServiceManager accServiceManager, IHrServiceManager hrServiceManager, IMainServiceManager mainServiceManager, ICurrentData session)
        {
            this.accServiceManager = accServiceManager;
            this.mainServiceManager = mainServiceManager;
            this.session = session;
            this.hrServiceManager= hrServiceManager;    
        }
 
        [HttpGet("GetHrReports")]
        public async Task<IActionResult> GetAccReports()
        {
            try
            {
                IEnumerable<RptReportDto> reportsList = new List<RptReportDto>();
                var getReports = await accServiceManager.AccDashboardService.GetReports(_SystemId, session.GroupId.ToString());
                return Ok(getReports);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetHrStatistics")]
        public async Task<IActionResult> GetHrStatistics()
        {
            var DateGregorian = DateTime.Now.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);

            try
            {
                var BranchesList = session.Branches.Split(',');
                List<HrStatisticsVM> statisticList = new List<HrStatisticsVM>();
                var getAllEmployees = await hrServiceManager.HrEmployeeService.GetAll(x=>x.Isdel==false&&x.IsDeleted==false&&x.IsSub==false&&x.FacilityId==session.FacilityId&& BranchesList.Contains(x.BranchId.ToString()));

                if (getAllEmployees.Succeeded)
                {
                    var AllActiveEmployee = getAllEmployees.Data.Where(x => x.StatusId == 1).ToList();
                    var ActiveEmployee = new HrStatisticsVM
                    {
                        Color="green",
                        Count= AllActiveEmployee.Count(),
                        Icon= "icon-bar-chart",
                        StatisticType=1,
                        StatusName= "عدد الموظفين على راس العمل ",
                        StatusName2 = "Active employees",
                        Route = "Apps/HR/Employee/Employee?Status_ID=1"

                    };
                    statisticList.Add(ActiveEmployee);
                    var AllPendingEmployee = getAllEmployees.Data.Where(x => x.StatusId == 10).ToList();
                    var PendingEmployee = new HrStatisticsVM
                    {
                        Color = "red",
                        Count = AllPendingEmployee.Count(),
                        Icon = "icon-bar-chart",
                        StatisticType = 1,
                        StatusName = "عدد الموظفين تحت الإجراء ",
                        StatusName2 = "N'Pending employees",
                        Route = "Apps/HR/Employee/Employee?Status_ID=10"

                    };
                    statisticList.Add(PendingEmployee);


                    // the begin of second report data
                    // all present statistic
                    var getAllAttendance = await hrServiceManager.HrAttendanceService.GetAll(x => x.IsDeleted == false && x.DayDateGregorian == DateGregorian);
                   if(getAllAttendance.Succeeded)
                    {
                        var EmpIdList = getAllAttendance.Data.Select(e => e.EmpId).ToList();
                        var AllPresentEmployees = AllActiveEmployee.Where(a=> EmpIdList.Contains(a.Id));
                        var PresentEmployees = new HrStatisticsVM
                        {
                            Color = "green",
                            Count = AllPresentEmployees.Count(),
                            Icon = "icon-bar-chart",
                            StatisticType = 2,
                            StatusName = "الحضور",
                            StatusName2 = "Attendance",
                            Route = "Apps/HR/FormReport/Attendance/AttendanceReport_Days?Type_Id=1"

                        };
                        statisticList.Add(PresentEmployees);
                    }


              

                }

                ////////////////////////all Late statistics////////////////////////////////////////////////////////////////////////////////
                // Get all employees
                var getAllEmployeesForLateAttendance = await hrServiceManager.HrEmployeeService.GetAll(x =>
                    x.Isdel == false &&
                    x.IsDeleted == false &&
                    x.IsSub == false &&
                    x.StatusId == 1 &&
                    x.FacilityId == session.FacilityId &&
                    BranchesList.Contains(x.BranchId.ToString()) &&
                    x.ExcludeAttend == false);

                if (getAllEmployeesForLateAttendance.Succeeded)
                {
                    // Get all attendances for the specified date
                    var getAllAttendancesForLateAttendance = await hrServiceManager.HrAttendanceService.GetAll(a =>
                        a.IsDeleted == false &&
                        a.DayDateGregorian == DateGregorian);

                    if (getAllAttendancesForLateAttendance.Succeeded)
                    {
                        // Extract relevant information from attendance data
                        var attendanceData = getAllAttendancesForLateAttendance.Data.ToList();

                        // Fetch HrAttTimeTable data for the attendances
                        var timeTableIds = attendanceData.Select(a => a.TimeTableId).Distinct().ToList();
                        var getAllTimeTables = await hrServiceManager.HrAttTimeTableService.GetAll(t => timeTableIds.Contains(t.Id));

                        if (getAllTimeTables.Succeeded)
                        {
                            // Extract relevant information from time table data
                            var timeTableData = getAllTimeTables.Data.ToDictionary(t => t.Id);

                            // Filter attendances based on the condition
                            var lateAttendanceData = attendanceData
                                .Where(a => a.TimeTableId.HasValue &&
                                            timeTableData.ContainsKey(a.TimeTableId.Value) &&
                                            a.TimeIn > a.DefTimeIn.Value.AddMinutes((double)timeTableData[a.TimeTableId.Value].LateTimeM))
                                .Select(a => new { Id = a.EmpId })
                                .ToList();

                            // Filter employees who have late attendance based on the extracted data
                            var employeesWithLateAttendance = getAllEmployeesForLateAttendance.Data
                                .Where(e => lateAttendanceData.Any(a => a.Id == e.Id))
                                .ToList();

                            // Calculate and add the statistics to your list
                            var employeesWithLateAttendanceStatistic = new HrStatisticsVM
                            {
                                Color = "yellow",
                                Count = employeesWithLateAttendance.Count(),
                                Icon = "icon-bar-chart",
                                StatisticType = 2,
                                StatusName = "المتأخرين",
                                StatusName2 = "Late",
                                Route = "Apps/HR/FormReport/Attendance/AttendanceReport_Days?Type_Id=8"

                            };

                            statisticList.Add(employeesWithLateAttendanceStatistic);
                        }
                    }
                }

                ////////////////////////all Absence statistics////////////////////////////////////////////////////////////////////////////////
                // Get all employees
                var getAllEmployeesForAbsence = await hrServiceManager.HrEmployeeService.GetAll(x =>
                    x.Isdel == false &&
                    x.IsDeleted == false &&
                    x.IsSub == false &&
                    x.StatusId == 1 &&
                    x.FacilityId == session.FacilityId &&
                    BranchesList.Contains(x.BranchId.ToString()) &&
                    x.ExcludeAttend == false);

                if (getAllEmployeesForAbsence.Succeeded)
                {
                    // Get all attendances, vacations, mandates, and shift employees without date conditions
                    var getAllAttendancesForAbsence = await hrServiceManager.HrAttendanceService.GetAll(a =>
                        a.IsDeleted == false);
                    var getAllVacationsForAbsence = await hrServiceManager.HrVacationsService.GetAll(v =>
                        v.IsDeleted == false);
                    var getAllMandatesForAbsence = await hrServiceManager.HrMandateService.GetAll(m =>
                        m.IsDeleted == false);
                    var getAllShiftEmployeesForAbsence = await hrServiceManager.HrAttShiftEmployeeService.GetAll(se =>
                        se.IsDeleted == false);

                    if (getAllAttendancesForAbsence.Succeeded &&
                        getAllVacationsForAbsence.Succeeded &&
                        getAllMandatesForAbsence.Succeeded &&
                        getAllShiftEmployeesForAbsence.Succeeded)
                    {
                        // Extract relevant information from each data source
                        var attendanceData = getAllAttendancesForAbsence.Data.ToList();
                        var vacationData = getAllVacationsForAbsence.Data.ToList();
                        var mandateData = getAllMandatesForAbsence.Data.ToList();
                        var shiftEmployeeData = getAllShiftEmployeesForAbsence.Data.ToList();

                        // Filter data based on the date conditions
                     
                        var dateFilteredAttendanceData = attendanceData
                            .Where(a => a.DayDateGregorian != null && a.DayDateGregorian == DateGregorian)
                            .ToList();

                        var dateFilteredVacationData = vacationData
                            .Where(v => v.VacationSdate != null && v.VacationEdate != null && DateHelper.StringToDate(DateGregorian) >= DateHelper.StringToDate(v.VacationSdate) &&
                                        DateHelper.StringToDate(DateGregorian) <= DateHelper.StringToDate(v.VacationEdate))
                            .ToList();

                        var dateFilteredMandateData = mandateData
                            .Where(m => m.FromDate != null && m.ToDate != null && DateHelper.StringToDate(DateGregorian) >= DateHelper.StringToDate(m.FromDate) &&
                                        DateHelper.StringToDate(DateGregorian) <= DateHelper.StringToDate(m.ToDate))
                            .ToList();

                        var dateFilteredShiftEmployeeData = shiftEmployeeData
                            .Where(se => se.BeginDate != null && se.EndDate != null && DateHelper.StringToDate(DateGregorian) >= DateHelper.StringToDate(se.BeginDate) &&
                                         DateHelper.StringToDate(DateGregorian) <= DateHelper.StringToDate(se.EndDate))
                            .ToList();

                        // Extract relevant IDs for each data source
                        var attendanceIds = dateFilteredAttendanceData.Select(a => a.EmpId).ToList();
                        var vacationIds = dateFilteredVacationData.Select(v => v.EmpId).ToList();
                        var mandateIds = dateFilteredMandateData.Select(m => m.EmpId).ToList();
                        var shiftEmployeeIds = dateFilteredShiftEmployeeData.Select(se => se.EmpId).ToList();

                        // Filter employees based on absence conditions
                        var employeesWithAbsence = getAllEmployeesForAbsence.Data
                            .Where(e => !attendanceIds.Contains(e.Id) &&
                                        !vacationIds.Contains(e.Id) &&
                                        !mandateIds.Contains(e.Id) &&
                                        shiftEmployeeIds.Contains(e.Id))
                            .ToList();

                        // Calculate and add the statistics to your list
                        var employeesWithAbsenceStatistic = new HrStatisticsVM
                        {
                            Color = "red",
                            Count = employeesWithAbsence.Count(),
                            Icon = "icon-bar-chart",
                            StatisticType = 2,
                            StatusName = "الغياب",
                            StatusName2 = "Absence",
                            Route = "Apps/HR/FormReport/Attendance/AttendanceReport_Days?Type_Id=2"

                        };

                        statisticList.Add(employeesWithAbsenceStatistic);
                    }
                }

                //////////////////////all Vacation statistic/////////////////////////////////////////////

                // Get all  employees
                var getAllEmployeeForVacation = await hrServiceManager.HrEmployeeService.GetAll(x => x.Isdel == false && x.IsDeleted == false && x.IsSub == false && x.FacilityId == session.FacilityId && BranchesList.Contains(x.BranchId.ToString()));
                // Get all vacations
                var getAllVacations = await hrServiceManager.HrVacationsService.GetAll(e => e.IsDeleted == false);
                if (getAllEmployeeForVacation.Succeeded && getAllVacations.Succeeded)
                {
                    // Extract relevant information from vacation data
                    var vacationData = getAllVacations.Data.Select(e => new { Id = e.EmpId, StartDate = e.VacationSdate, EndDate = e.VacationEdate }).ToList();

                    // Filter vacations that intersect with the current date
                    var currentVacations = vacationData.Where(e =>e.EndDate != null && e.StartDate != null && DateGregorian != null &&  DateHelper.StringToDate(e.EndDate) >= DateHelper.StringToDate(DateGregorian) && DateHelper.StringToDate(e.StartDate) <= DateHelper.StringToDate(DateGregorian)).ToList();

                    // Get statistics related to employees on vacation
                    var employeesOnVacation = getAllEmployeeForVacation.Data.Where(a => currentVacations.Any(b => b.Id == a.Id)).ToList();

                    // Calculate and add the statistics to your list
                    var employeesOnVacationStatistic = new HrStatisticsVM
                    {
                        Color = "primary",
                        Count = employeesOnVacation.Count(),
                        Icon = "icon-bar-chart",
                        StatisticType = 2,
                        StatusName = "المجازين",
                        StatusName2 = "Vacation",
                        Route = "Apps/HR/FormReport/Attendance/AttendanceReport_Days?Type_Id=3"


                    };

                    statisticList.Add(employeesOnVacationStatistic);
                }

                //////////////////// all excluded from preparation ///////////////////////////////////
                var getAllExcluded = getAllEmployees.Data.Where(x => x.StatusId == 1 && x.ExcludeAttend == true && !getAllEmployees.Data.Any(e => e.ExcludeAttend == false && e.Id == x.Id)).ToList();
                var ExcludedEmployees = new HrStatisticsVM
                {
                    Color = "primary",
                    Count = getAllExcluded.Count(),
                    Icon = "icon-bar-chart",
                    StatisticType = 2,
                    StatusName = "المستبعدين من التحضير",
                    StatusName2 = "Those excluded from preparation",
                    Route= "Apps/HR/FormReport/RPAttend"
                };
                statisticList.Add(ExcludedEmployees);
               
                return Ok(statisticList);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

    }

}