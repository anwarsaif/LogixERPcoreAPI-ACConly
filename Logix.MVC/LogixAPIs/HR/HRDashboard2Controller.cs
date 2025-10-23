using iText.Commons.Bouncycastle.Asn1.X509;
using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.DTOs.Main;
using Logix.Application.DTOs.RPT;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Versioning;
using System.Globalization;
using System.Linq;

namespace Logix.MVC.LogixAPIs.HR
{
    // إحصائيات بالحركات للموظفين
    public class HRDashboard2Controller : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IMainServiceManager mainServiceManager;
        private readonly ICurrentData session;
        private readonly IPermissionHelper permission;
        private readonly ILocalizationService localization;

        public HRDashboard2Controller(IPermissionHelper permission, IHrServiceManager hrServiceManager, ICurrentData session, IMainServiceManager mainServiceManager, ILocalizationService localization)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.mainServiceManager = mainServiceManager;
            this.localization = localization;   
        }

        [HttpPost("GetStatistics")]
        public async Task<IActionResult> GetStatistics(HrDashboardDto filter)
        {
            var chk = await permission.HasPermission(918, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var BranchesList = session.Branches.Split(',');
                {
                    filter.DeptId ??= 0;
                    filter.LocationId ??= 0;
                    filter.BranchId ??= 0;

                    var employees = await hrServiceManager.HrEmployeeService.GetAllVW(e => e.IsDeleted == false && e.FacilityId == session.FacilityId && BranchesList.Contains(e.BranchId.ToString()) && e.StatusId == 1
                                && (filter.BranchId == 0 || e.BranchId == filter.BranchId)
                                && (filter.LocationId == 0 || e.Location == filter.LocationId)
                                && (filter.DeptId == 0 || e.DeptId == filter.DeptId));

                    var onVacationsResult = await hrServiceManager.HrVacationsService.GetAllVW(v => v.IsDeleted == false && BranchesList.Contains(v.BranchId.ToString())
                                    && (filter.BranchId == 0 || v.BranchId == filter.BranchId)
                                    && (filter.LocationId == 0 || v.Location == filter.LocationId)
                                    && (filter.DeptId == 0 || v.DeptId == filter.DeptId)
                                    && (v.VacationSdate != null && v.VacationEdate != null)
    );
                    var onVacations = onVacationsResult.Data.AsQueryable();
                    if (!string.IsNullOrEmpty(filter.StartDate) && !string.IsNullOrEmpty(filter.EndDate))
                    {
                        var StartDate = DateHelper.StringToDate(filter.StartDate);
                        var EndDate = DateHelper.StringToDate(filter.EndDate);
                        onVacations = onVacations.Where(r =>
                        (DateHelper.StringToDate(r.VacationSdate) >= StartDate &&
                       (DateHelper.StringToDate(r.VacationEdate) <= StartDate)
                       ));
                    }
                    var absencesResult = await hrServiceManager.HrAbsenceService.GetAllVW(a => !a.IsDeleted && BranchesList.Contains(a.BranchId.ToString())
                                    && (filter.BranchId == 0 || a.BranchId == filter.BranchId)
                                    && (filter.LocationId == 0 || a.Location == filter.LocationId)
                                    && (filter.DeptId == 0 || a.DeptId == filter.DeptId)
                                    && (a.AbsenceDate != null)

                                    );



                    var absences = absencesResult.Data.AsQueryable();
                    if (!string.IsNullOrEmpty(filter.StartDate) && !string.IsNullOrEmpty(filter.EndDate))
                    {
                        var StartDate = DateHelper.StringToDate(filter.StartDate);
                        var EndDate = DateHelper.StringToDate(filter.EndDate);
                        absences = absences.Where(r =>
                        (DateHelper.StringToDate(r.AbsenceDate) >= StartDate &&
                       (DateHelper.StringToDate(r.AbsenceDate) <= EndDate)
                       ));
                    }
                    var presentResult = await hrServiceManager.HrAttendanceService.GetAllVW(a => a.IsDeleted == false && BranchesList.Contains(a.BranchId.ToString())
                                    && (filter.BranchId == 0 || a.BranchId == filter.BranchId)
                                    && (filter.LocationId == 0 || a.Location == filter.LocationId)
                                    && (filter.DeptId == 0 || a.DeptId == filter.DeptId)
                                    && (a.DayDateGregorian != null));


                    var present = presentResult.Data.AsQueryable();
                    if (!string.IsNullOrEmpty(filter.StartDate) && !string.IsNullOrEmpty(filter.EndDate))
                    {
                        var StartDate = DateHelper.StringToDate(filter.StartDate);
                        var EndDate = DateHelper.StringToDate(filter.EndDate);
                        present = present.Where(r =>
                        (DateHelper.StringToDate(r.DayDateGregorian) >= StartDate &&
                       (DateHelper.StringToDate(r.DayDateGregorian) <= EndDate)
                       ));
                    }
                    var joining = employees.Data.AsQueryable();
                    if (!string.IsNullOrEmpty(filter.StartDate) && !string.IsNullOrEmpty(filter.EndDate))
                    {
                        var StartDate = DateHelper.StringToDate(filter.StartDate);
                        var EndDate = DateHelper.StringToDate(filter.EndDate);
                        joining = joining.Where(r => DateHelper.StringToDate(r.Doappointment) >= StartDate && DateHelper.StringToDate(r.Doappointment) <= EndDate);
                    }

                    ///
                    var LeaveResult = await hrServiceManager.HrLeaveService.GetAllVW(a => a.IsDeleted == false && BranchesList.Contains(a.BranchId.ToString())
                    && (filter.BranchId == 0 || a.BranchId == filter.BranchId)
                    && (filter.LocationId == 0 || a.Location == filter.LocationId)
                    && (filter.DeptId == 0 || a.DeptId == filter.DeptId)
                    && (a.LeaveDate != null));


                    var Leave = LeaveResult.Data.AsQueryable();
                    if (!string.IsNullOrEmpty(filter.StartDate) && !string.IsNullOrEmpty(filter.EndDate))
                    {
                        var StartDate = DateHelper.StringToDate(filter.StartDate);
                        var EndDate = DateHelper.StringToDate(filter.EndDate);
                        Leave = Leave.Where(r =>
                        (DateHelper.StringToDate(r.LeaveDate) >= StartDate &&
                       (DateHelper.StringToDate(r.LeaveDate) <= EndDate)
                       ));
                    }

                    ////////////
                    var DisciplinaryCaseActionResult = await hrServiceManager.HrDisciplinaryCaseActionService.GetAllVW(a => a.IsDeleted == false && BranchesList.Contains(a.BranchId.ToString())
                    && (filter.BranchId == 0 || a.BranchId == filter.BranchId)
                    && (filter.LocationId == 0 || a.Location == filter.LocationId)
                    && (filter.DeptId == 0 || a.DeptId == filter.DeptId)
                    && (a.DueDate != null));


                    var DisciplinaryCaseAction = DisciplinaryCaseActionResult.Data.AsQueryable();
                    if (!string.IsNullOrEmpty(filter.StartDate) && !string.IsNullOrEmpty(filter.EndDate))
                    {
                        var StartDate = DateHelper.StringToDate(filter.StartDate);
                        var EndDate = DateHelper.StringToDate(filter.EndDate);
                        DisciplinaryCaseAction = DisciplinaryCaseAction.Where(r =>
                        (DateHelper.StringToDate(r.DueDate) >= StartDate &&
                       (DateHelper.StringToDate(r.DueDate) <= EndDate)
                       ));
                    }

                    ////////////
                    var HrPayrollDResult = await hrServiceManager.HrPayrollDService.GetAllVW(a => a.IsDeleted == false && BranchesList.Contains(a.BranchId.ToString())
                    && (filter.BranchId == 0 || a.BranchId == filter.BranchId)
                    && (filter.LocationId == 0 || a.Location == filter.LocationId)
                    && (filter.DeptId == 0 || a.DeptId == filter.DeptId)
                    && (a.FinancelYear != null && a.MsMonth != null));


                    var HrPayrollD = HrPayrollDResult.Data.AsQueryable();
                    if (!string.IsNullOrEmpty(filter.StartDate) && !string.IsNullOrEmpty(filter.EndDate))
                    {
                        var StartDate = DateHelper.StringToDate(filter.StartDate);
                        var EndDate = DateHelper.StringToDate(filter.EndDate);
                        HrPayrollD = HrPayrollD.Where(r =>
                        (DateHelper.StringToDate(r.FinancelYear + "/" + r.MsMonth + "/15") >= StartDate &&
                       (DateHelper.StringToDate(r.FinancelYear + "/" + r.MsMonth + "/15") <= EndDate)
                       ));
                    }

                    ////////////
                    var ContractResult = await hrServiceManager.HrContracteService.GetAllVW(a => a.IsDeleted == false && BranchesList.Contains(a.BranchId.ToString())
                    && (filter.BranchId == 0 || a.BranchId == filter.BranchId)
                    && (filter.LocationId == 0 || a.LocationId == filter.LocationId)
                    && (filter.DeptId == 0 || a.DepartmentId == filter.DeptId)
                    && (a.ContractExpiryDate != null && a.NewContractExpiryDate != null));


                    var Contract = ContractResult.Data.AsQueryable();
                    if (!string.IsNullOrEmpty(filter.StartDate) && !string.IsNullOrEmpty(filter.EndDate))
                    {
                        var StartDate = DateHelper.StringToDate(filter.StartDate);
                        var EndDate = DateHelper.StringToDate(filter.EndDate);
                        Contract = Contract.Where(r =>
                        (DateHelper.StringToDate(r.ContractExpiryDate) >= StartDate && (DateHelper.StringToDate(r.ContractExpiryDate) <= EndDate)) ||
                        (DateHelper.StringToDate(r.NewContractExpiryDate) >= StartDate && (DateHelper.StringToDate(r.NewContractExpiryDate) <= EndDate))
                       );
                    }

                    ////////////
                    var TransferResult = await hrServiceManager.HrTransferService.GetAllVW(a => a.IsDeleted == false && BranchesList.Contains(a.BranchId.ToString())
                    && (filter.BranchId == 0 || a.BranchId == filter.BranchId)
                    && (filter.LocationId == 0 || a.Location == filter.LocationId)
                    && (filter.DeptId == 0 || a.DeptId == filter.DeptId)
                    && (a.TransferDate != null));


                    var Transfer = TransferResult.Data.AsQueryable();
                    if (!string.IsNullOrEmpty(filter.StartDate) && !string.IsNullOrEmpty(filter.EndDate))
                    {
                        var StartDate = DateHelper.StringToDate(filter.StartDate);
                        var EndDate = DateHelper.StringToDate(filter.EndDate);
                        Transfer = Transfer.Where(r =>
                        (DateHelper.StringToDate(r.TransferDate) >= StartDate && (DateHelper.StringToDate(r.TransferDate) <= EndDate))
                       );
                    }

                    ////////////
                    var OverTimeMResult = await hrServiceManager.HrOverTimeMService.GetAllVW(a => a.IsDeleted == false && BranchesList.Contains(a.BranchId.ToString())
                    && (filter.BranchId == 0 || a.BranchId == filter.BranchId)
                    && (filter.LocationId == 0 || a.Location == filter.LocationId)
                    && (filter.DeptId == 0 || a.DeptId == filter.DeptId)
                    && (a.DateFrom != null && a.DateTo != null));


                    var OverTimeM = OverTimeMResult.Data.AsQueryable();
                    if (!string.IsNullOrEmpty(filter.StartDate) && !string.IsNullOrEmpty(filter.EndDate))
                    {
                        var StartDate = DateHelper.StringToDate(filter.StartDate);
                        var EndDate = DateHelper.StringToDate(filter.EndDate);
                        OverTimeM = OverTimeM.Where(r =>
                        (DateHelper.StringToDate(r.DateFrom) >= StartDate && (DateHelper.StringToDate(r.DateFrom) <= EndDate)) ||
                        (DateHelper.StringToDate(r.DateTo) >= StartDate && (DateHelper.StringToDate(r.DateTo) <= EndDate))
                       );
                    }

                    ////////////
                    var AllowanceDeductionResult = await hrServiceManager.HrAllowanceDeductionService.GetAllVW(a => a.IsDeleted == false && BranchesList.Contains(a.BranchId.ToString())
                    && (filter.BranchId == 0 || a.BranchId == filter.BranchId)
                    && (filter.LocationId == 0 || a.Location == filter.LocationId)
                    && (filter.DeptId == 0 || a.DeptId == filter.DeptId)
                    && (a.DueDate != null));


                    var AllowanceDeduction = AllowanceDeductionResult.Data.AsQueryable();
                    if (!string.IsNullOrEmpty(filter.StartDate) && !string.IsNullOrEmpty(filter.EndDate))
                    {
                        var StartDate = DateHelper.StringToDate(filter.StartDate);
                        var EndDate = DateHelper.StringToDate(filter.EndDate);
                        AllowanceDeduction = AllowanceDeduction.Where(r =>
                        (DateHelper.StringToDate(r.DueDate) >= StartDate && (DateHelper.StringToDate(r.DueDate) <= EndDate))
                       );
                    }
                    ////////////

                    var IncrementsResult = await hrServiceManager.HrIncrementService.GetAllVW(a => a.IsDeleted == false && BranchesList.Contains(a.BranchId.ToString())
                    && (filter.BranchId == 0 || a.BranchId == filter.BranchId)
                    && (filter.LocationId == 0 || a.Location == filter.LocationId)
                    && (filter.DeptId == 0 || a.DeptId == filter.DeptId)
                    && (a.IncreaseDate != null)
                    && (a.TransTypeId == 1 || a.TransTypeId == 2)
                    );


                    var Increments = IncrementsResult.Data.AsQueryable();
                    if (!string.IsNullOrEmpty(filter.StartDate) && !string.IsNullOrEmpty(filter.EndDate))
                    {
                        var StartDate = DateHelper.StringToDate(filter.StartDate);
                        var EndDate = DateHelper.StringToDate(filter.EndDate);
                        Increments = Increments.Where(r =>
                        (DateHelper.StringToDate(r.IncreaseDate) >= StartDate && (DateHelper.StringToDate(r.IncreaseDate) <= EndDate))
                       );
                    }

                    ////////////

                    var PayrollResult = await hrServiceManager.HrPayrollService.GetAllVW(a => a.IsDeleted == false && BranchesList.Contains(a.BranchId.ToString())
                    && (filter.BranchId == 0 || a.BranchId == filter.BranchId)
                    && (filter.LocationId == 0 || a.Location == filter.LocationId)
                    && (filter.DeptId == 0 || a.DeptId == filter.DeptId)
                    && (a.MsDate != null)
                    && (a.PayrollTypeId == 1)
                    && (a.State == 2)
                    );


                    var Payroll = PayrollResult.Data.AsQueryable();
                    if (!string.IsNullOrEmpty(filter.StartDate) && !string.IsNullOrEmpty(filter.EndDate))
                    {
                        var StartDate = DateHelper.StringToDate(filter.StartDate);
                        var EndDate = DateHelper.StringToDate(filter.EndDate);
                        Payroll = Payroll.Where(r =>
                        (DateHelper.StringToDate(r.MsDate) >= StartDate && (DateHelper.StringToDate(r.MsDate) <= EndDate))
                       );
                    }
                    ////////////
                    var ApprovalVacations = onVacationsResult.Data.AsQueryable();
                    ApprovalVacations = ApprovalVacations.Where(x => x.StatusId == 4);
                    if (!string.IsNullOrEmpty(filter.StartDate) && !string.IsNullOrEmpty(filter.EndDate))
                    {
                        var StartDate = DateHelper.StringToDate(filter.StartDate);
                        var EndDate = DateHelper.StringToDate(filter.EndDate);
                        ApprovalVacations = ApprovalVacations.Where(r =>
                        (DateHelper.StringToDate(r.VacationSdate) >= StartDate && (DateHelper.StringToDate(r.VacationSdate) <= StartDate)) ||
                        (DateHelper.StringToDate(r.VacationEdate) >= StartDate && (DateHelper.StringToDate(r.VacationEdate) <= StartDate))

                        );
                    }

                    ////////////
                    var FixingEmployeeSalaryResult = await hrServiceManager.HrFixingEmployeeSalaryService.GetAllVW(a => a.IsDeleted == false && BranchesList.Contains(a.BranchId.ToString())
                    && (filter.BranchId == 0 || a.BranchId == filter.BranchId)
                    && (filter.LocationId == 0 || a.Location == filter.LocationId)
                    && (filter.DeptId == 0 || a.DeptId == filter.DeptId)
                    && (a.FixingDate != null));


                    var FixingEmployeeSalary = FixingEmployeeSalaryResult.Data.AsQueryable();
                    if (!string.IsNullOrEmpty(filter.StartDate) && !string.IsNullOrEmpty(filter.EndDate))
                    {
                        var StartDate = DateHelper.StringToDate(filter.StartDate);
                        var EndDate = DateHelper.StringToDate(filter.EndDate);
                        FixingEmployeeSalary = FixingEmployeeSalary.Where(r =>
                        (DateHelper.StringToDate(r.FixingDate) >= StartDate && (DateHelper.StringToDate(r.FixingDate) <= EndDate))
                       );
                    }



                    ////////////
                    var GetHrTableIds = await mainServiceManager.SysTableService.GetAll(x => x.TableId, x => x.SystemId == "3");
                    var HrTablesList = GetHrTableIds.Data.ToList();
                    var FilesResult = await mainServiceManager.SysFileService.GetAll(a => a.IsDeleted == false && HrTablesList.Contains((int)a.TableId)
                    && (a.FileDate != null));


                    var Files = FilesResult.Data.AsQueryable();
                    if (!string.IsNullOrEmpty(filter.StartDate) && !string.IsNullOrEmpty(filter.EndDate))
                    {
                        var StartDate = DateHelper.StringToDate(filter.StartDate);
                        var EndDate = DateHelper.StringToDate(filter.EndDate);
                        Files = Files.Where(r =>
                        (DateHelper.StringToDate(r.FileDate) >= StartDate && (DateHelper.StringToDate(r.FileDate) <= EndDate))
                       );
                    }


                    var results = new List<HrDashboardDto>
        {
            new HrDashboardDto
            {
                Cnt =  employees.Data.Count(),
                Name = "عدد الموظفين",
                Name2 = "Employee",
                Color = "primary",
                Icon = "icon-user",
                Url = ""
            },
            new HrDashboardDto
            {
                Cnt =  onVacations.Count(),
                Name = "عدد المجازين",
                Name2 = "On Leave",
                Color = "yellow",
                Icon = "icon-user",
                Url = ""
            },
            new HrDashboardDto
            {
                Cnt =  absences.Count(),
                Name = "عدد الغياب",
                Name2 = "Absence",
                Color = "red",
                Icon = "icon-user",
                Url = ""
            },
            new HrDashboardDto
            {
                Cnt = present.Count(),
                Name = "عدد الحضور",
                Name2 = "Present",
                Color = "green",
                Icon = "icon-user",
                Url = ""
            },
            new HrDashboardDto
            {
                Cnt = joining.Count(),
                Name = "الذين تم توظيفهم خلال الفترة",
                Name2 = "Joining This Period",
                Color = "green",
                Icon = "icon-user",
                Url = ""
            },
            new HrDashboardDto
            {
                Cnt = Leave.Count(),
                Name = "الذين تم فصلهم خلال الفترة",
                Name2 = "Exists This Period",
                Color = "red",
                Icon = "icon-user",
                Url = ""
            },
            new HrDashboardDto
            {
                Cnt = DisciplinaryCaseAction.Count(),
                Name = "عدد الجزاءات خلال الفترة",
                Name2 = "Penalties This Period",
                Color = "yellow",
                Icon = "icon-user",
                Url = ""
            },
            new HrDashboardDto
            {
                Cnt = HrPayrollD.Sum(X=>X.Net),
                Name = "إجمالي الرواتب خلال الفترة",
                Name2 = "Salary This Period",
                Color = "yellow",
                Icon = "icon-user",
                Url = ""
            },
            new HrDashboardDto
            {
                Cnt = Contract.Count(),
                Name = "الذين تم تجديد عقودهم خلال الفترة",
                Name2 = "Contracts In This Period",
                Color = "green",
                Icon = "icon-user",
                Url = ""
            },
            new HrDashboardDto
            {
                Cnt = Transfer.Count(),
                Name = "عدد التنقلات بالنظام خلال الفترة",
                Name2 = "Transfers In This Period",
                Color = "yellow",
                Icon = "icon-user",
                Url = ""
            },
            new HrDashboardDto
            {
                Cnt = OverTimeM.Count(),
                Name = "عدد عمليات العمل الإضافي خلال الفترة",
                Name2 = "Over Time In This Period",
                Color = "primary",
                Icon = "icon-user",
                Url = ""
            },
            new HrDashboardDto
            {
                Cnt = AllowanceDeduction.Count(),
                Name = " عدد عمليات البدلات والحسميات خلال الفترة",
                Name2 = "Allowance And Deduction In This Period",
                Color = "red",
                Icon = "icon-user",
                Url = ""
            },
            new HrDashboardDto
            {
                Cnt = Increments.Count(),
                Name = " عدد العلاوات والترقيات خلال الفترة",
                Name2 = "Bonus And Increments In This Period",
                Color = "green",
                Icon = "icon-user",
                Url = ""
            },
            new HrDashboardDto
            {
                Cnt = Payroll.Count(),
                Name = " مسيرات الرواتب الي اعتمدت خلال الفترة",
                Name2 = "Payroll In This Period",
                Color = "yellow",
                Icon = "icon-user",
                Url = ""
            },
            new HrDashboardDto
            {
                Cnt = Leave.Count(),
                Name = "عدد المخالصات خلال الفترة ",
                Name2 = "End Of Service In This Period",
                Color = "red",
                Icon = "icon-user",
                Url = ""
            },
            new HrDashboardDto
            {
                Cnt = ApprovalVacations.Count(),
                Name = "عدد الموافقات على الاجازات خلال الفترة",
                Name2 = "Vacations Approval In This Period",
                Color = "green",
                Icon = "icon-user",
                Url = ""
            },
            new HrDashboardDto
            {
                Cnt = FixingEmployeeSalary.Count(),
                Name = "عدد الخطابات الي طلبت خلال الفترة",
                Name2 = "Letters In This Period",
                Color = "red",
                Icon = "icon-user",
                Url = ""
            },
            new HrDashboardDto
            {
                Cnt = Files.Count(),
                Name = "عدد الملفات الي ارشفت خلال الفترة",
                Name2 = "Files Archived In This Period",
                Color = "yellow",
                Icon = "icon-user",
                Url = ""
            }

        };

                    return Ok(await Result<List<HrDashboardDto>>.SuccessAsync(results));
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync(ex.Message));
            }
        }



        [HttpPost("GetChartsData")]
        public async Task<IActionResult> GetPayrollReports(HrDashboardDto filter)
        {
            var chk = await permission.HasPermission(918, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                filter.DeptId ??= 0;
                filter.LocationId ??= 0;
                filter.BranchId ??= 0;
                var branchesList = session.Branches.Split(',');

                var results = await hrServiceManager.HrPayrollDService.GetPayrollReportsForHrDashboard2(filter);
                if (results.Data != null) return Ok(await Result<HrDashboard2ResultDto>.SuccessAsync(results.Data, ""));
                return Ok(await Result<HrDashboard2ResultDto>.SuccessAsync(results.Data, localization.GetResource1("NosearchResult")));

            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync(ex.Message));
            }
        }


    }
}