using System.Data;
using System.Globalization;
using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IRepositories;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.WF.ViewModels;
using Microsoft.AspNetCore.Mvc;
using NodaTime;

namespace Logix.MVC.LogixAPIs.WF
{
    public class WfDashboardController : BaseWfController
    {
        private readonly IWFServiceManager wfServiceManager;
        private readonly IMainServiceManager mainServiceManager;
        private readonly IHrServiceManager hrServiceManager;
        private readonly ISysConfigurationHelper configurationHelper;
        private readonly IMainRepositoryManager _mainRepositoryManager;
        private readonly ITsServiceManager tsServiceManager;
        private readonly ICurrentData currentData;

        public WfDashboardController(IWFServiceManager wfServiceManager,
            IMainServiceManager mainServiceManager,
            IHrServiceManager hrServiceManager,
            ISysConfigurationHelper configurationHelper,
            IMainRepositoryManager mainRepositoryManager,
            ITsServiceManager tsServiceManager,
            ICurrentData currentData)
        {
            this.wfServiceManager = wfServiceManager;
            this.mainServiceManager = mainServiceManager;
            this.hrServiceManager = hrServiceManager;
            this.configurationHelper = configurationHelper;
            this._mainRepositoryManager = mainRepositoryManager;
            this.tsServiceManager = tsServiceManager;
            this.currentData = currentData;
        }

        #region =============================================== Automation Dashboard ============================================
        [HttpGet("GetAutomationDashboard")]
        public async Task<IActionResult> GetAutomationDashboard()
        {
            // [WF_Dashboard_SP] cmdType 8 and 9

            // first statics (cmdType 8)
            List<AutomationStatisticsVM> statisticslist = new();

            var allApplications = await wfServiceManager.WfApplicationService.GetAllVW(x => x.IsDeleted == false);
            if (allApplications.Succeeded && allApplications.Data.Any())
            {
                var group1 = allApplications.Data.GroupBy(x => new { x.ApplicationsTypeId, x.ApplicationTypeName, x.ApplicationTypeName2 })
                        .Select(g => new
                        {
                            g.Key.ApplicationsTypeId,
                            g.Key.ApplicationTypeName,
                            g.Key.ApplicationTypeName2,
                            Count = g.Count()
                        }).OrderByDescending(x => x.Count).Take(5).ToList();

                foreach (var item in group1)
                {
                    statisticslist.Add(new AutomationStatisticsVM()
                    {
                        Count = item.Count,
                        StatusName = item.ApplicationTypeName,
                        StatusName2 = item.ApplicationTypeName2,
                        Color = "primary",
                        Icon = "icon-bar-chart",
                        Link = "/Apps/Workflow/Applications/Applications_Query?App_Type=" + (item.ApplicationsTypeId ?? 0),
                        StatisticType = 1
                    });
                }

                // second statics (cmdType 9)
                statisticslist.Add(new AutomationStatisticsVM()
                {
                    Count = allApplications.Data.Count(),
                    StatusName = "اجمالي عدد الطلبات",
                    StatusName2 = "Total number of applications",
                    Color = "green",
                    Icon = "icon-bar-chart",
                    Link = "/Apps/Workflow/Applications/Applications_Query?App_Type=" + "0",
                    StatisticType = 2
                });

                // get ids of applications that its status = 5 (approved)
                var approvedIds = await wfServiceManager.WfApplicationsStatusService.GetAll(x => x.ApplicantsId, x => x.StatusId == 5);
                var approvedApps = allApplications.Data.Where(x => approvedIds.Data.Contains(x.ApplicantsId));
                statisticslist.Add(new AutomationStatisticsVM()
                {
                    Count = approvedApps.Count(),
                    StatusName = "اجمالي عدد الطلبات المعتمدة",
                    StatusName2 = "Total number of approved applications",
                    Color = "green",
                    Icon = "icon-bar-chart",
                    Link = "/Apps/Workflow/Applications/Applications_Query?App_Type=" + "0",
                    StatisticType = 2
                });

                return Ok(await Result<List<AutomationStatisticsVM>>.SuccessAsync(statisticslist));
            }
            return Ok(allApplications);
        }
        #endregion ============================================== End Automation Dashboard ==========================================


        #region =============================================== Workflow Dashboard ============================================
        [HttpGet("GetAnnouncement")]
        public async Task<IActionResult> GetAnnouncement()
        {
            // (HR_Self_Service_Dashboard cmdType 21)
            try
            {
                List<AnnouncementVM> announcements = new();
                long userId = currentData.UserId;
                var getAnnouncement = await mainServiceManager.SysAnnouncementService.GetAllVW(x => x.IsDeleted == false && x.IsActive == true
                && (x.LocationId == 2 || x.LocationId == 3)
                && (x.FacilityId == currentData.FacilityId)
                && (x.BranchId == 0 || x.BranchId == currentData.BranchId)
                && (x.DeptId == 0 || x.DeptId == currentData.DeptId)
                && (x.DeptLocationId == 0 || x.DeptLocationId == currentData.LocationId)
                );

                if (getAnnouncement.Succeeded)
                {
                    var currentDate = DateTime.Now;
                    var res = getAnnouncement.Data.Where(x => x.StartDate != null && x.EndDate != null
                     && DateTime.ParseExact(x.StartDate, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= currentDate
                     && DateTime.ParseExact(x.EndDate, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= currentDate);

                    foreach (var item in res)
                    {
                        announcements.Add(new AnnouncementVM { TypeName = item.TypeName, Subject = item.Subject, AttachFile = item.AttachFile, PublishDate = item.PublishDate });
                    }
                    return Ok(await Result<List<AnnouncementVM>>.SuccessAsync(announcements));
                }

                return Ok(await Result.FailAsync());
            }
            catch
            {
                return Ok(await Result.FailAsync());
            }
        }

        [HttpGet("GetApplicationStatics")]
        public async Task<IActionResult> GetApplicationStatics()
        {
            // get applications statics (WF_Dashboard_SP cmdType 6)
            try
            {
                WfApplicationStatisticsVM applicationStatistics = new();
                long userId = currentData.UserId;

                // get wfApplications
                var getApplicationsId = await wfServiceManager.WfApplicationService.GetAll(x => x.IsDeleted == false && x.CreatedBy == userId);
                var getAllApplicationStatus = await wfServiceManager.WfApplicationsStatusService.GetAll();
                if (getApplicationsId.Succeeded && getAllApplicationStatus.Succeeded)
                {
                    List<long?> statusAppsIds = new();
                    var applicationsIdList = getApplicationsId.Data.Select(x => x.Id);
                    // (Accepted, Rejected, and OnTrack) Applications
                    statusAppsIds = getAllApplicationStatus.Data.Where(x => x.NewStatusId == 5).Select(x => x.ApplicationsId).ToList();
                    applicationStatistics.AcceptedCount = applicationsIdList.Where(x => statusAppsIds.Contains(x)).Count();

                    statusAppsIds = getAllApplicationStatus.Data.Where(x => x.NewStatusId == 3).Select(x => x.ApplicationsId).ToList();
                    applicationStatistics.RejectedCount = applicationsIdList.Where(x => statusAppsIds.Contains(x)).Count();

                    statusAppsIds = getAllApplicationStatus.Data.Where(x => (x.NewStatusId == 3 || x.NewStatusId == 5 || x.NewStatusId == 7)).Select(x => x.ApplicationsId).ToList();
                    applicationStatistics.OnTrackCount = applicationsIdList.Where(x => !statusAppsIds.Contains(x)).Count();

                    // OffTrack
                    int offTrackCount = 0;
                    int duration = 0;
                    DateTime createdOn = DateTime.MinValue; // default value
                    List<int> excludeStatus = new() { 3, 7, 2, 5 };
                    var allApplicationsId = getApplicationsId.Data.Where(x => !excludeStatus.Contains(x.StatusId ?? 0)).Select(x => x.Id);

                    foreach (var applicationId in allApplicationsId)
                    {
                        // get WFApplicationsStatus
                        var getApplicationsStatus = await wfServiceManager.WfApplicationsStatusService.GetAll(x => x.ApplicationsId == applicationId && !excludeStatus.Contains(x.NewStatusId ?? 0));
                        if (getApplicationsStatus.Succeeded)
                        {
                            var applicationsStatus = getApplicationsStatus.Data.OrderByDescending(x => x.Id).ToList().Take(1);
                            foreach (var appStatus in applicationsStatus)
                            {
                                // get Duration
                                var getWfSteps = await wfServiceManager.WfStepService.GetAll(x => x.Id == appStatus.StepId);
                                if (getWfSteps.Succeeded)
                                {
                                    if (appStatus.CreatedOn != null)
                                        createdOn = appStatus.CreatedOn.Value;
                                    var stepId = appStatus.StepId;
                                    var wfStep = getWfSteps.Data.FirstOrDefault();
                                    if (wfStep != null)
                                    {
                                        try
                                        {
                                            int minutesFromDays = (wfStep.DurationDays ?? 0) * 60 * 24;
                                            var timeParts = (wfStep.DurationTime ?? "00:00").Split(':');
                                            int totalMinutesFromTime = (int.Parse(timeParts[0]) * 60) + int.Parse(timeParts[1]);
                                            duration = minutesFromDays + totalMinutesFromTime;
                                        }
                                        catch { continue; }
                                    }
                                }
                            }
                        }

                        if (duration > 0 && (DateTime.Now - createdOn).TotalMinutes > duration)
                            ++offTrackCount;
                        duration = 0; // reset duration
                    }

                    applicationStatistics.OffTrackCount = offTrackCount;

                    return Ok(await Result<WfApplicationStatisticsVM>.SuccessAsync(applicationStatistics));
                }
                return Ok(await Result.FailAsync());
            }
            catch
            {
                return Ok(await Result.FailAsync());
            }
        }

        [HttpGet("GetBasicEmpData")]
        public async Task<IActionResult> GetBasicEmpData()
        {
            // (HR_Self_Service_Dashboard cmdType 1)
            try
            {
                var lang = currentData.Language;
                var getEmpData = await hrServiceManager.HrEmployeeService.GetOneVW(x => x.Id == currentData.EmpId && x.IsDeleted == false);
                if (getEmpData.Succeeded)
                {
                    var empData = getEmpData.Data;
                    MyDataVM myData = new()
                    {
                        Id = empData.Id,
                        EmpId = empData.EmpId,
                        EmpName = lang == 1 ? empData.EmpName : empData.EmpName2,
                        Mobile = empData.Mobile,
                        Email = empData.Email,
                        EmpPhoto = empData.EmpPhoto,
                        LocationName = lang == 1 ? empData.LocationName : empData.LocationName2,
                        AttendanceType = empData.AttendanceType,
                        CatName = lang == 1 ? empData.CatName : empData.CatName2,
                        DepName = lang == 1 ? empData.DepName : empData.DepName2,
                    };

                    // get manager
                    var getManagerData = await hrServiceManager.HrEmployeeService.GetOne(x => x.Id == getEmpData.Data.ManagerId && x.IsDeleted == false);
                    if (getManagerData.Succeeded)
                    {
                        myData.ManagerName = lang == 1 ? getManagerData.Data.EmpName : getManagerData.Data.EmpName2;
                    }

                    // Get Job Description
                    var activShowCatagoryName = await configurationHelper.GetValue(376, currentData.FacilityId);
                    if (activShowCatagoryName == "1")
                    {
                        var getJobCat = await mainServiceManager.InvestEmployeeService.GetOne(x => x.JobCatagoriesId, x => x.Id == currentData.EmpId && x.IsDeleted == false);
                        if (getJobCat.Succeeded)
                        {
                            var getJobDescription = await hrServiceManager.HrJobDescriptionService.GetAll(x => x.FileUrl,
                                x => x.IsDeleted == false && x.JobCatId == getJobCat.Data);
                            if (getJobDescription.Succeeded)
                            {
                                myData.LinkJobCatVisible = true;
                                myData.JobCatUrl = getJobDescription.Data.FirstOrDefault();
                            }
                        }
                    }

                    return Ok(await Result<MyDataVM>.SuccessAsync(myData));
                }

                return Ok(await Result.FailAsync());
            }
            catch
            {
                return Ok(await Result.FailAsync());
            }
        }

        [HttpGet("GetUserDataQr")]
        public async Task<IActionResult> GetUserDataQr()
        {
            try
            {
                int enableQr = 0; string pass = ""; string userName = ""; string compnyMember = "";
                compnyMember = await configurationHelper.GetValue(276, currentData.FacilityId);
                if (!string.IsNullOrEmpty(compnyMember))
                {
                    enableQr = 1;
                    var getUserName = await mainServiceManager.SysUserService.GetOne(x => x.UserName, x => x.Id == currentData.UserId);
                    userName = getUserName.Succeeded ? getUserName.Data : ""; ;
                    pass = await mainServiceManager.SysUserService.GetDecryptUserPassword(currentData.UserId);
                }

                return Ok(await Result<object>.SuccessAsync(new { enableQr, pass, userName, compnyMember }));
            }
            catch
            {
                return Ok(await Result.FailAsync());
            }
        }

        [HttpGet("GetBalances")]
        public async Task<IActionResult> GetBalances()
        {
            // (HR_Self_Service_Dashboard cmdType 10)
            try
            {
                List<BalanceVM> balances = new();
                BalanceVM newBalance = new();
                var lang = currentData.Language;
                var currentDate = DateHelper.GetDateGregorianDotNow();

                var vacationTypeId = 1;
                var getVacationType = await configurationHelper.GetValue(94, currentData.FacilityId);
                if (!string.IsNullOrEmpty(getVacationType))
                    vacationTypeId = Convert.ToInt32(getVacationType);

                // Vacation Balance
                var getVacationBalance = await hrServiceManager.HrVacationsService.Vacation_Balance2_FN(currentDate, currentData.EmpId, vacationTypeId);
                BalanceVM VacationBalance = new() { Name = "رصيد الإجازات", Name2 = "Vacation Balance", Balance = getVacationBalance };
                balances.Add(VacationBalance);

                // Tickets Balance
                var getTicketsBalance = await _mainRepositoryManager.DbFunctionsRepository.HR_Ticket_Balance_Fn(currentDate, currentData.EmpId);
                BalanceVM TicketsBalance = new() { Name = "رصيد التذاكر", Name2 = "Tickets Balance", Balance = getTicketsBalance };
                balances.Add(TicketsBalance);

                // Return/Exit Visa
                var getExitReturnBalance = await _mainRepositoryManager.DbFunctionsRepository.HR_Visa_Balance_Fn(currentDate, currentData.EmpId);
                BalanceVM ExitReturnBalance = new() { Name = "رصيد الخروج والعودة", Name2 = "Return/Exit Visa", Balance = getExitReturnBalance };
                balances.Add(ExitReturnBalance);

                return Ok(await Result<List<BalanceVM>>.SuccessAsync(balances));
            }
            catch
            {
                return Ok(await Result.FailAsync());
            }
        }

        [HttpGet("GetTrialExpiryNotifi")]
        public async Task<IActionResult> GetTrialExpiryNotifi()
        {
            // (HR_Self_Service_Dashboard cmdType 22)
            try
            {
                List<TrialNotifiVM> notifications = new();
                TrialNotifiVM notifi = new();
                var lang = currentData.Language;
                var currentDate = DateHelper.GetDateGregorianDotNow();

                var getData = await hrServiceManager.HrEmployeeService.GetAllVW(x => x.IsDeleted == false && x.ManagerId == currentData.EmpId
                && x.StatusId == 1 && x.TrialStatusId != 2);
                if (getData.Succeeded)
                {
                    var res = getData.Data.Where(x => !string.IsNullOrEmpty(x.TrialExpiryDate)
                    && DateTime.ParseExact(x.TrialExpiryDate, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= DateTime.ParseExact(currentDate, "yyyy/MM/dd", CultureInfo.InvariantCulture));

                    foreach (var item in res)
                    {
                        notifications.Add(new TrialNotifiVM()
                        {
                            Id = item.Id,
                            EmpId = item.EmpId,
                            EmpName = lang == 1 ? item.EmpName : item.EmpName2,
                            Doappointment = item.Doappointment,
                            TrialExpiryDate = item.TrialExpiryDate
                        });
                    }

                    return Ok(await Result<List<TrialNotifiVM>>.SuccessAsync(notifications));
                }

                return Ok(await Result.FailAsync());
            }
            catch
            {
                return Ok(await Result.FailAsync());
            }
        }

        [HttpGet("GetNotifications")]
        public async Task<IActionResult> GetNotifications()
        {
            // (HR_Self_Service_Dashboard cmdType 4)
            try
            {
                List<NotificationVM> notifications = new();
                var lang = currentData.Language;

                var getData = await hrServiceManager.HrEmployeeService.GetOneVW(x => x.IsDeleted == false && x.Id == currentData.EmpId);
                if (getData.Succeeded)
                {
                    var res = getData.Data;
                    notifications.Add(new NotificationVM() { Date1 = res.IdExpireDate, Name = lang == 1 ? "تاريخ إنتهاء الهوية" : "ID Expiry Date" });
                    notifications.Add(new NotificationVM() { Date1 = res.ContractExpiryDate, Name = lang == 1 ? "تاريخ إنتهاء العقد" : "Contarct Expiry Date" });
                    notifications.Add(new NotificationVM() { Date1 = res.PassExpireDate, Name = lang == 1 ? "تاريخ إنتهاء جواز السفر" : "Passport Expiry Date" });
                    notifications.Add(new NotificationVM() { Date1 = res.InsuranceDateValidity, Name = lang == 1 ? "تاريخ إنتهاء التأمين الطبي" : "Med Insurance Expiry Date" });

                    return Ok(await Result<List<NotificationVM>>.SuccessAsync(notifications));
                }

                return Ok(await Result.FailAsync());
            }
            catch
            {
                return Ok(await Result.FailAsync());
            }
        }

        [HttpGet("GetSalaryData")]
        public async Task<IActionResult> GetSalaryData()
        {
            // (HR_Self_Service_Dashboard cmdType 12)
            try
            {
                var currentDate = DateHelper.GetDateGregorianDotNow();
                var getData = await _mainRepositoryManager.StoredProceduresRepository.GetSalaryData_WF(currentData.EmpId, currentData.FacilityId, currentDate);
                if (getData.Rows[0] != null)
                {
                    SalaryDataVM result = new()
                    {
                        Attendance = Convert.ToDecimal(getData.Rows[0]["Attendance"]),
                        Salary = Convert.ToDecimal(getData.Rows[0]["Salary"]),
                        Allowance = Convert.ToDecimal(getData.Rows[0]["Allowance"]),
                        Deduction = Convert.ToDecimal(getData.Rows[0]["Deduction"]),
                        ValOverTime = Convert.ToDecimal(getData.Rows[0]["Val_OverTime"]),
                        Loan = Convert.ToDecimal(getData.Rows[0]["Loan"]),
                        Absence = Convert.ToDecimal(getData.Rows[0]["Absence"]),
                        Delay = Convert.ToDecimal(getData.Rows[0]["Delay"]),
                        Penalties = Convert.ToDecimal(getData.Rows[0]["Penalties"])
                    };

                    return Ok(await Result<SalaryDataVM>.SuccessAsync(result));
                }
                else return Ok(await Result.FailAsync());
            }
            catch
            {
                return Ok(await Result.FailAsync());
            }
        }

        [HttpGet("GetPayrolls")]
        public async Task<IActionResult> GetPayrolls()
        {
            // (HR_Self_Service_Dashboard cmdType 6)
            try
            {
                var getData = await hrServiceManager.HrPayrollDService.GetAllVW(x => x.EmpId == currentData.EmpId
                && x.State == 4 && x.PayrollTypeId == 1 && x.IsDeleted == false);
                if (getData.Succeeded)
                {
                    List<PayrollVM> results = new();
                    var items = getData.Data.OrderByDescending(x => x.MsdId).Take(5).ToList();
                    foreach (var item in items)
                    {
                        results.Add(new PayrollVM()
                        {
                            MsId = item.MsId,
                            MsdId = item.MsdId,
                            FinancialYear = item.FinancelYear ?? 0,
                            MsMonth = item.MsMonth,
                            MsMothTxt = item.MsMothTxt,
                            //Total = Convert.ToDecimal(item.Salary + item.Allowance - item.Absence - item.Delay - item.Loan - item.Deduction),
                            // details
                            Salary = item.Salary ?? 0,
                            Allowance = item.Allowance ?? 0,
                            Absence = item.Absence ?? 0,
                            Delay = item.Delay ?? 0,
                            Loan = item.Loan ?? 0,
                            Deduction = item.Deduction ?? 0,
                            Penalties = item.Penalties ?? 0,
                        });
                    }
                    return Ok(await Result<object>.SuccessAsync(new { payrolls = results, empId = currentData.EmpId }));
                }

                return Ok(await Result.FailAsync());
            }
            catch
            {
                return Ok(await Result.FailAsync());
            }
        }

        [HttpGet("GetTasks")]
        public async Task<IActionResult> GetTasks()
        {
            // (HR_Self_Service_Dashboard cmdType 8)
            try
            {
                List<TaskVM> results = new();
                List<int> excludeStatus = new() { 4, 5, 6 };
                var userId = currentData.UserId;
                var currentDate = DateHelper.GetCurrentDateTime();
                var getAllTasks = await tsServiceManager.TsTaskService.GetAllVW(x => !excludeStatus.Contains(x.StatusId ?? 0) && x.Isdel == false);
                if (getAllTasks.Succeeded)
                {
                    var res = getAllTasks.Data.Where(x => !string.IsNullOrEmpty(x.AssigneeToUserId) && x.AssigneeToUserId.Split(',').Contains(userId.ToString()))
                    .OrderBy(x => x.Priority).Take(5).ToList();
                    foreach (var item in res)
                    {
                        if (!string.IsNullOrEmpty(item.SendDate) && !string.IsNullOrEmpty(item.DueDate))
                        {
                            // Calculate days count
                            DateTime sendDate = DateTime.ParseExact(item.SendDate, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                            DateTime dueDate = DateTime.ParseExact(item.DueDate, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                            int cntDays = (dueDate - sendDate).Days;
                            if (cntDays == 0) cntDays = 1;
                            // Calculate days remaining
                            int remainingDays = (currentDate - sendDate).Days;

                            results.Add(new TaskVM()
                            {
                                Id = item.Id,
                                Subject = item.Subject,
                                Presentage = remainingDays * 100 / cntDays
                            });
                        }
                    }

                    return Ok(await Result<List<TaskVM>>.SuccessAsync(results));
                }
                return Ok(await Result.FailAsync());
            }
            catch
            {
                return Ok(await Result.FailAsync());
            }
        }

        [HttpGet("GetDues")]
        public async Task<IActionResult> GetDues()
        {
            // (HR_Self_Service_Dashboard cmdType 13)
            try
            {
                List<DuesVM> results = new();
                var currentDate = DateHelper.GetDateGregorianDotNow();
                var lang = currentData.Language;
                var getData = await _mainRepositoryManager.StoredProceduresRepository.GetDues_WF(currentData.EmpId, currentDate);
                foreach (DataRow row in getData.Rows)
                {
                    results.Add(new DuesVM()
                    {
                        Dues = row["Dues"].ToString() ?? "", // Convert.ToDecimal(row["Dues"]??0),
                        Name = row["Name"].ToString() ?? "",
                        Name2 = row["Name2"].ToString() ?? ""
                    });
                }
                return Ok(await Result<List<DuesVM>>.SuccessAsync(results));
            }
            catch
            {
                return Ok(await Result.FailAsync());
            }
        }

        [HttpGet("GetKpiDegree")]
        public async Task<IActionResult> GetKpiDegree()
        {
            // (HR_Self_Service_Dashboard cmdType 24)
            try
            {
                decimal degreeTotal = 0;
                int count = 0;
                var getKpis = await hrServiceManager.HrKpiService.GetAll(x => x.EmpId == currentData.EmpId && x.IsDeleted == false && x.StatusId == 2);
                if (getKpis.Succeeded)
                {
                    foreach (var item in getKpis.Data)
                    {
                        // get kpi details
                        var getKpiDetails = await hrServiceManager.HrKpiDetaileService.GetAllVW(x => x.EmpId == item.EmpId);
                        var sumDegree = getKpiDetails.Data.Sum(x => x.Degree);
                        var sumScore = getKpiDetails.Data.Sum(x => x.Score);
                        degreeTotal += Convert.ToDecimal(sumDegree / sumScore * 100);
                        ++count;
                    }

                    if (count != 0)
                        degreeTotal = Math.Round(degreeTotal / count, 2);
                }

                return Ok(await Result<decimal>.SuccessAsync(degreeTotal));
            }
            catch
            {
                return Ok(await Result.FailAsync());
            }
        }

        [HttpGet("GetKpiReport")]
        public async Task<IActionResult> GetKpiReport()
        {
            // (HR_Self_Service_Dashboard cmdType 23)
            try
            {
                List<KpiReportVM> results = new();
                var getKpis = await hrServiceManager.HrKpiService.GetAll(x => x.EmpId == currentData.EmpId && x.IsDeleted == false && x.StatusId == 2);
                if (getKpis.Succeeded)
                {
                    var kpis = getKpis.Data.OrderByDescending(x => x.EvaDate).Take(5);
                    foreach (var item in kpis)
                    {
                        // get kpi details
                        var getKpiDetails = await hrServiceManager.HrKpiDetaileService.GetAllVW(x => x.KpiId == item.Id);
                        var sumDegree = getKpiDetails.Data.Sum(x => x.Degree);
                        var sumScore = getKpiDetails.Data.Sum(x => x.Score);
                        decimal degreeTotal = Convert.ToDecimal(sumDegree / sumScore * 100);

                        results.Add(new KpiReportVM()
                        {
                            Id = item.Id ?? 0,
                            EvaDate = item.EvaDate,
                            DegreeTotal = Math.Round(degreeTotal, 2)
                        });
                    }

                    return Ok(await Result<List<KpiReportVM>>.SuccessAsync(results));
                }

                return Ok(await Result.FailAsync());
            }
            catch
            {
                return Ok(await Result.FailAsync());
            }
        }

        [HttpGet("GetGoals")]
        public async Task<IActionResult> GetGoals()
        {
            try
            {
                List<GoalsVM> results = new();
                var getKpis = await hrServiceManager.HrRequestGoalsEmployeeDetailService.GetAllVW(x => x.EmpId == currentData.EmpId && x.IsDeleted == false);
                if (getKpis.Succeeded)
                {
                    var currentYear = DateTime.Now.Year.ToString();
                    var kpis = getKpis.Data.Where(x => !string.IsNullOrEmpty(x.LastRegistrationDate) && x.LastRegistrationDate.Substring(0, 4) == currentYear);
                    foreach (var item in kpis)
                    {
                        results.Add(new GoalsVM()
                        {
                            Id = item.Id,
                            Title = item.Title,
                            Target = item.Target,
                            Weight = item.Weight ?? 0,
                            TargetValue = item.TargetValue ?? 0
                        });
                    }

                    return Ok(await Result<List<GoalsVM>>.SuccessAsync(results));
                }

                return Ok(await Result.FailAsync());
            }
            catch
            {
                return Ok(await Result.FailAsync());
            }
        }

        [HttpGet("GetLoans")]
        public async Task<IActionResult> GetLoans()
        {
            // (HR_Self_Service_Dashboard cmdType 14)
            try
            {
                List<LoanVM> results = new();
                var getLoans = await hrServiceManager.HrLoanService.GetAll(x => x.EmpId == currentData.EmpId.ToString() && x.IsDeleted == false);
                if (getLoans.Succeeded)
                {
                    foreach (var loan in getLoans.Data)
                    {
                        LoanVM obj = new()
                        {
                            LoanValue = loan.LoanValue,
                            InstallmentValue = loan.InstallmentValue,
                            InstallmentCount = loan.InstallmentCount,
                            Note = loan.Note
                        };

                        // get Lloan details & calculate remaining amount
                        var getLoanDetails = await hrServiceManager.HrLoanInstallmentService.GetAll(x => x.LoanId == loan.Id && x.IsDeleted == false);
                        obj.RemainingAmount = getLoanDetails.Data.Where(x => x.IsPaid == false).Sum(x => x.Amount);

                        List<LoanDetailVM> details = new();
                        foreach (var item in getLoanDetails.Data)
                        {
                            details.Add(new LoanDetailVM()
                            {
                                InstallmentNo = item.IntallmentNo,
                                Amount = item.Amount,
                                DueDate = item.DueDate,
                                IsPaid = item.IsPaid ?? false
                            });
                        }
                        obj.Details = details;
                        results.Add(obj);
                    }

                    return Ok(await Result<List<LoanVM>>.SuccessAsync(results));
                }

                return Ok(await Result.FailAsync());
            }
            catch
            {
                return Ok(await Result.FailAsync());
            }
        }

        [HttpGet("GetTeamMembers")]
        public async Task<IActionResult> GetTeamMembers()
        {
            // (HR_Self_Service_Dashboard cmdType 15)
            try
            {
                List<TeamMemberVM> results = new();
                var lang = currentData.Language;
                var getEmployees = await hrServiceManager.HrEmployeeService.GetAllVW(x => x.DeptId == currentData.DeptId && x.Location == currentData.LocationId
                && x.IsDeleted == false && x.StatusId == 1);
                if (getEmployees.Succeeded)
                {
                    foreach (var item in getEmployees.Data)
                    {
                        results.Add(new TeamMemberVM()
                        {
                            EmpName = lang == 1 ? item.EmpName : item.EmpName2,
                            Gender = item.Gender,
                            CatName = lang == 1 ? item.CatName : item.CatName2,
                            EmpPhoto = item.EmpPhoto,
                            StatusName = lang == 1 ? item.StatusName : item.StatusName2
                        });
                    }

                    return Ok(await Result<List<TeamMemberVM>>.SuccessAsync(results));
                }

                return Ok(await Result.FailAsync());
            }
            catch
            {
                return Ok(await Result.FailAsync());
            }
        }

        [HttpGet("GetArchiveFiles")]
        public async Task<IActionResult> GetArchiveFiles()
        {
            // (HR_Self_Service_Dashboard cmdType 3)
            try
            {
                List<ArchiveFileVM> results = new();
                var getFiles = await hrServiceManager.HrArchiveFilesDetailService.GetAllVW(x => x.IsDeleted == false && x.IsDeletedM == false
                && x.EmpId == currentData.EmpId && x.ShowEmp == true);
                if (getFiles.Succeeded)
                {
                    foreach (var item in getFiles.Data)
                    {
                        results.Add(new ArchiveFileVM()
                        {
                            Url = item.Url,
                            Note = item.Note
                        });
                    }

                    return Ok(await Result<List<ArchiveFileVM>>.SuccessAsync(results));
                }

                return Ok(await Result.FailAsync());
            }
            catch
            {
                return Ok(await Result.FailAsync());
            }
        }

        [HttpGet("GetAttendanceReport")]
        public async Task<IActionResult> GetAttendanceReport()
        {
            // (HR_Self_Service_Dashboard cmdType 5)
            try
            {
                List<AttendanceReportVM> result = new();
                var lang = currentData.Language;
                var calendarType = currentData.CalendarType;

                var getData = await hrServiceManager.HrAttendanceService.GetAll(x => x.EmpId == currentData.EmpId && x.IsDeleted == false);
                if (getData.Succeeded)
                {
                    var res = getData.Data
                        .OrderByDescending(x => x.DayDateHijri).ThenByDescending(x => x.TimeIn.HasValue ? x.TimeIn.Value.TimeOfDay : TimeSpan.MinValue)
                        .Take(5).ToList();
                    foreach (var item in res)
                    {
                        var obj = new AttendanceReportVM()
                        {
                            TimeIn = item.TimeIn.HasValue ? item.TimeIn.Value.ToString("HH:mm:ss", CultureInfo.InvariantCulture) : "",
                            TimeOut = item.TimeOut.HasValue ? item.TimeOut.Value.ToString("HH:mm:ss", CultureInfo.InvariantCulture) : ""
                        };
                        if (calendarType == "1")
                            obj.Date = item.DayDateGregorian;
                        else
                        {
                            // convert Gregorian to Hijri
                            DateHelper.Initialize(_mainRepositoryManager);
                            obj.Date = await DateHelper.DateFormattYYYYMMDD_G_H(item.DayDateGregorian ?? "");
                        }

                        result.Add(obj);
                    }

                    return Ok(await Result<List<AttendanceReportVM>>.SuccessAsync(result));
                }

                return Ok(await Result<List<AttendanceReportVM>>.FailAsync());
            }
            catch
            {
                return Ok(await Result<List<AttendanceReportVM>>.FailAsync());
            }
        }

        [HttpGet("GetAttendanceOnline")]
        public async Task<IActionResult> GetAttendanceOnline()
        {
            // (HR_Attendance_SP cmdType 3)
            try
            {
                AttendanceOnlineVM result = new();
                var lang = currentData.Language;
                var calendarType = currentData.CalendarType;
                var currentDate = DateHelper.GetDateGregorianDotNow();
                string Day_Date_Gregorian_G = "";

                if (Convert.ToInt32(calendarType) == 1)
                    Day_Date_Gregorian_G = currentDate;
                else
                    Day_Date_Gregorian_G = await DateHelper.DateGregorian(currentDate);

                int Day_No = (int)DateTime.Now.DayOfWeek + 1; // DayOfWeek start from 0

                var getData = await hrServiceManager.HrAttShiftEmployeeService.GetAllVW(x => x.EmpId == currentData.EmpId && x.DayNo == Day_No && x.IsDeleted == false);
                if (getData.Succeeded)
                {
                    DateTime gDateTime = DateTime.ParseExact(Day_Date_Gregorian_G, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                    var res = getData.Data.Where(x => !string.IsNullOrEmpty(x.BeginDate) && !string.IsNullOrEmpty(x.EndDate)
                        && gDateTime >= DateTime.ParseExact(x.BeginDate, "yyyy/MM/dd", CultureInfo.InvariantCulture)
                        && gDateTime <= DateTime.ParseExact(x.EndDate, "yyyy/MM/dd", CultureInfo.InvariantCulture)).ToList();

                    foreach (var item in res)
                    {
                        result.TimeTableName = item.TimeTableName;
                        result.BeginIn = item.BeginIn.ToString();
                        result.EndIn = item.EndIn.ToString();
                        result.BeginOut = item.BeginOut.ToString();
                        result.EndOut = item.EndOut.ToString();
                        result.OnDutyTime = item.OnDutyTime.ToString();
                        result.OffDutyTime = item.OffDutyTime.ToString();

                        // get HR_Attendances(Time_In, Time_Out)
                        var getAttendance = await hrServiceManager.HrAttendanceService.GetOne(x => x.EmpId == currentData.EmpId && x.IsDeleted == false
                        && x.DayDateGregorian == Day_Date_Gregorian_G && x.TimeTableId == item.TimeTableId);
                        if (getAttendance.Succeeded)
                        {
                            if (getAttendance.Data.TimeIn != null)
                                result.TimeIn = getAttendance.Data.TimeIn.Value.ToString("hh:mm tt");
                            if (getAttendance.Data.TimeOut != null)
                                result.TimeOut = getAttendance.Data.TimeOut.Value.ToString("hh:mm tt");
                        }
                    }

                    // جلب موقع التحضير المسموح به
                    var getAttLocation = await hrServiceManager.HrAttLocationEmployeeService.GetAllVW(x => x.EmpId == currentData.EmpId && x.IsDeleted == false);
                    if (getAttLocation.Succeeded)
                    {
                        var allowLocation = getAttLocation.Data.FirstOrDefault();
                        if (allowLocation != null)
                        {
                            result.Latitude = allowLocation.Latitude;
                            result.Longitude = allowLocation.Longitude;
                            result.LocationName = lang == 1 ? allowLocation.LocationName : allowLocation.LocationName2;
                        }
                    }

                    return Ok(await Result<AttendanceOnlineVM>.SuccessAsync(result));
                }

                return Ok(await Result<AttendanceOnlineVM>.FailAsync());
            }
            catch
            {
                return Ok(await Result<AttendanceOnlineVM>.FailAsync());
            }
        }


        [HttpPost("AddAttendance")]
        public async Task<IActionResult> AddAttendance(AttendanceAddDto obj)
        {
            try
            {
                // TimeIn & TimeOut save only in gregorian (take dataetime.now based on timezone)
                var getTimeNow = await GetDateTime();
                var timeNow = DateTime.ParseExact(getTimeNow, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                // DayDateGregorian come only in gregorian from angular,, and DayDateHijri get it from sysCalendar based on DayDateGregorian
                DateHelper.Initialize(_mainRepositoryManager);
                var hijriDate = await DateHelper.DateFormattYYYYMMDD_G_H(obj.DayDateGregorian ?? "");

                HrAttendanceDto hrAttendanceDto = new()
                {
                    EmpId = currentData.EmpId,
                    DayNo = 0,
                    TimeIn = timeNow, // yyyy-MMy-dd HH:mm:ss.fff
                    TimeOut = timeNow, // // yyyy-MMy-dd HH:mm:ss.fff
                    AttType = obj.AttType,
                    DayDateGregorian = obj.DayDateGregorian,
                    DayDateHijri = hijriDate,
                    CreatedBy = currentData.UserId,
                    LogInBy = "2",
                    LogOutBy = "2",
                    Longitude = obj.Longitude,
                    Latitude = obj.Latitude,
                    LongitudeOut = obj.Longitude,
                    LatitudeOut = obj.Latitude,
                };

                // add
                var add = await _mainRepositoryManager.StoredProceduresRepository.HR_Attendance_SP_CmdType_1(hrAttendanceDto);
                if (!add)
                    return Ok(await Result.FailAsync("حدث خطاء اثناء عملية التحضير"));

                return Ok(await Result<string>.SuccessAsync(timeNow.ToString("hh:mm tt") ?? "")); // to set this value on button of login or logout
            }
            catch
            {
                return Ok(await Result.FailAsync("حدث خطاء اثناء عملية التحضير"));
            }
        }

        private async Task<string> GetDateTime()
        {
            try
            {
                //Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                var activTimeZone = await configurationHelper.GetValue(358, currentData.FacilityId);
                if (activTimeZone == "1")
                {
                    string timeZone = "";
                    var getTimeZoneId = await mainServiceManager.InvestEmployeeService.GetOne(x => x.TimeZoneId, x => x.Id == currentData.EmpId);
                    if (getTimeZoneId.Succeeded && getTimeZoneId.Data > 0)
                        timeZone = await mainServiceManager.InvestEmployeeService.GetTimeZone(getTimeZoneId.Data ?? 0);
                    if (!string.IsNullOrEmpty(timeZone))
                    {
                        Instant utcNow = SystemClock.Instance.GetCurrentInstant();
                        IDateTimeZoneProvider timeZoneProvider = DateTimeZoneProviders.Tzdb;
                        DateTimeZone? riyadhTimeZone = timeZoneProvider.GetZoneOrNull(timeZone);
                        if (riyadhTimeZone != null)
                        {
                            ZonedDateTime riyadhTime = utcNow.InZone(riyadhTimeZone);
                            DateTime dateTime = Convert.ToDateTime(riyadhTime.ToString("yyyy-MM-dd HH:mm:ss", null));
                            return dateTime.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                        }
                    }

                    return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                }
                else
                    return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            }
            catch
            {
                return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            }
        }
        #endregion ============================================== End Workflow Dashboard ==========================================
    }
}
