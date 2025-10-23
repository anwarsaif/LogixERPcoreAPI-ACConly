using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Helpers;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Logix.MVC.LogixAPIs.HR
{
    // تصفية مستحقات إجازة
    public class HRClearanceController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IMainServiceManager mainServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData currentData;
        private readonly ILocalizationService localization;

        public HRClearanceController(IHrServiceManager hrServiceManager,
            IMainServiceManager mainServiceManager,
            IPermissionHelper permission,
            ICurrentData currentData,
            ILocalizationService localization)
        {
            this.hrServiceManager = hrServiceManager;
            this.mainServiceManager = mainServiceManager;
            this.permission = permission;
            this.currentData = currentData;
            this.localization = localization;
        }

        #region ================================ Search & Delete ================================
        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrClearanceFilterDto filter)
        {
            try
            {
                var chk = await permission.HasPermission(433, PermissionType.Show);
                var chk2 = await permission.HasPermission(838, PermissionType.Show);
                if (!chk && !chk2)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var items = await hrServiceManager.HrClearanceService.Search(filter);
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
        [HttpPost("GetPagination")]
        public async Task<IActionResult> GetPagination([FromBody] HrClearanceFilterDto filter, int take = Pagination.take, long? lastSeenId = null)
        {
            try
            {
                var chk = await permission.HasPermission(433, PermissionType.Show);
                var chk2 = await permission.HasPermission(838, PermissionType.Show);
                if (!chk && !chk2)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                filter.BranchId ??= 0; filter.DeptId ??= 0; filter.LocationId ??= 0; filter.ClearanceType ??= 0;
                var BranchesList = currentData.Branches.Split(',').Select(int.Parse).ToList();

                var dateConditions = new List<DateCondition>
                {
                    new() {
                        DatePropertyName = "DateC",
                        ComparisonOperator = ComparisonOperator.GreaterThanOrEqual,
                        StartDateString = filter.FromDate ?? ""
                    },
                    new() {
                        DatePropertyName = "DateC",
                        ComparisonOperator = ComparisonOperator.LessThanOrEqual,
                        StartDateString = filter.ToDate ?? ""
                    }
                };

                var items = await hrServiceManager.HrClearanceService.GetAllWithPaginationVW(selector: x => x.Id,
                    expression: e => e.IsDeleted == false
               && ((filter.BranchId == 0 && BranchesList.Contains(e.BranchId ?? 0)) || (filter.BranchId > 0 && e.BranchId == filter.BranchId))
                && (filter.ClearanceType == 0 || e.ClearanceType == filter.ClearanceType)
                && (string.IsNullOrEmpty(filter.empCode) || e.EmpCode == filter.empCode)
                && (filter.DeptId == 0 || e.DeptId == filter.DeptId)
                && (filter.LocationId == 0 || e.Location == filter.LocationId)

                ,
                    take: take,
                    lastSeenId: lastSeenId,
                   dateConditions: (string.IsNullOrEmpty(filter.FromDate) || string.IsNullOrEmpty(filter.ToDate)) ? null : dateConditions);

                if (!items.Succeeded)
                    return StatusCode(items.Status.code, items.Status.message);

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var chk = await permission.HasPermission(433, PermissionType.Delete);
                var chk2 = await permission.HasPermission(838, PermissionType.Delete);
                if (!chk && !chk2)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await hrServiceManager.HrClearanceService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }
        #endregion ============================ End Search & Delete =============================


        #region ====================================== Add ======================================
        [HttpPost("Add")]
        public async Task<ActionResult> Add(HrClearanceAddDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(433, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var add = await hrServiceManager.HrClearanceService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }


        [HttpPost("GetDataBtnClicked")]
        public async Task<ActionResult> GetDataBtnClicked(string EmpCode, string LastWorkingDate, int ClearanceTypeId)
        {
            try
            {
                var chk = await permission.HasPermission(433, PermissionType.Add);
                var chk2 = await permission.HasPermission(838, PermissionType.Add);
                if (!chk && !chk2)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (string.IsNullOrEmpty(EmpCode) || string.IsNullOrEmpty(LastWorkingDate) || ClearanceTypeId <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var GetData = await hrServiceManager.HrClearanceService.GetData(EmpCode, LastWorkingDate, ClearanceTypeId);
                return Ok(GetData);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"Exp, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetVacationData")]
        public async Task<IActionResult> GetVacationData(long Id)
        {
            try
            {
                var getVacation = await hrServiceManager.HrVacationsService.GetOneVW(x => x.VacationId == Id);
                if (!getVacation.Succeeded)
                    return Ok(await Result.FailAsync(getVacation.Status.message));

                return Ok(await Result<object>.SuccessAsync(new
                {
                    getVacation.Data.EmpCode,
                    getVacation.Data.VacationAccountDay,
                    getVacation.Data.VacationSdate,
                    getVacation.Data.VacationEdate
                }));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("GetAllowances")]
        public async Task<IActionResult> GetAllowances(long EmpId, long DaysCnt)
        {
            try
            {
                var x = await hrServiceManager.HrSettingService.GetOne(x => x.HousingAllowance, x => x.FacilityId == currentData.FacilityId);
                int housingId = x.Succeeded ? Convert.ToInt32(x.Data) : 0;

                var getAllowances = await hrServiceManager.HrAllowanceVwService.GetAll(x => x.EmpId == EmpId && x.TypeId == 1
                && x.IsDeleted == false && x.Status == true && x.FixedOrTemporary == 1);
                if (!getAllowances.Succeeded)
                    return Ok(await Result.FailAsync(getAllowances.Status.message));

                decimal AmountLong = 0;
                List<object> allowances = new();
                foreach (var item in getAllowances.Data)
                {
                    if (!decimal.TryParse(item.Amount.ToString(), out AmountLong))
                        AmountLong = 0;

                    allowances.Add(new
                    {
                        item.Id,
                        item.TypeId,
                        item.AdId,
                        item.Rate,
                        item.Amount,
                        item.Name,
                        NewAmount = Math.Round((AmountLong / 30) * DaysCnt, 2),
                        IsDeleted = false,
                        IsNew = false
                    });
                }

                return Ok(await Result<object>.SuccessAsync(new { allowances, housingId }));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
        #endregion ================================== End Add ===================================


        #region ====================================== Edit =====================================
        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long Id)
        {
            try
            {
                var chk = await permission.HasPermission(433, PermissionType.Edit);
                var chkView = await permission.HasPermission(433, PermissionType.View);
                if (!chk && !chkView)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));

                var item = await hrServiceManager.HrClearanceService.GetOneVW(x => x.Id == Id);
                if (item.Succeeded)
                {
                    var CHeckEmpInBranch = await hrServiceManager.HrEmployeeService.CHeckEmpInBranch(item.Data.EmpId);
                    if (CHeckEmpInBranch.Succeeded && CHeckEmpInBranch.Data == false)
                        return Ok(await Result.FailAsync(CHeckEmpInBranch.Status.message));

                    if (item.Data.PayrollId > 0)
                    {
                        var getPayPayroll = await hrServiceManager.HrPayrollService.GetOne(x => x.MsId == item.Data.PayrollId && x.IsDeleted == false);
                        if (getPayPayroll.Data != null)
                        {
                            //  يجب الغاء زر تحويل الى مسير تصفية في صفحة التعديل
                            return Ok(await Result<object>.SuccessAsync(new { item.Data, getPayPayroll.Data.MsCode }));
                        }
                    }
                    return Ok(await Result<object>.SuccessAsync(new { item.Data, MsCode = 0 }));
                }
                return Ok(await Result.FailAsync(item.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetAllowanceDeduction")]
        public async Task<IActionResult> GetAllowanceDeduction(long ClearanceId, long EmpId)
        {
            try
            {
                var x = await hrServiceManager.HrSettingService.GetOne(x => x.HousingAllowance, x => x.FacilityId == currentData.FacilityId);
                int housingId = x.Succeeded ? Convert.ToInt32(x.Data) : 0;

                var getAllowances = await hrServiceManager.HrClearanceAllowanceDeductionService.GetAllVW(x => x.ClearanceId == ClearanceId
                 && x.EmpId == EmpId && x.IsDeleted == false);
                if (!getAllowances.Succeeded)
                    return Ok(await Result.FailAsync(getAllowances.Status.message));

                List<object> allowances = new();
                foreach (var item in getAllowances.Data)
                {
                    allowances.Add(new
                    {
                        item.Id,
                        item.TypeId,
                        item.AdId,
                        item.Rate,
                        item.Amount,
                        item.Name,
                        item.NewAmount,
                        IsDeleted = false,
                        IsNew = false
                    });
                }

                return Ok(await Result<object>.SuccessAsync(new { allowances, housingId }));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(HrClearanceEditDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(433, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var update = await hrServiceManager.HrClearanceService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrLeaveEditDto>.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("PayrollTransfer")]
        public async Task<IActionResult> PayrollTransfer(HrClearancePayrollTransferDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(433, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (obj.Id <= 0)
                    return Ok(await Result<object>.FailAsync($"رقم التصفية"));

                var transfer = await hrServiceManager.HrClearanceService.PayrollTransfer(obj);
                return Ok(transfer);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }
        #endregion ================================== End Edit ==================================



        #region =================================== Clearance 2 =================================
        [HttpGet("OnLoad2")]
        public async Task<IActionResult> OnLoad2()
        {
            try
            {
                decimal HDOverTime = 0;
                var getOverTime = await hrServiceManager.HrSettingService.GetOne(x => x.OverTime, x => x.FacilityId == currentData.FacilityId);
                if (getOverTime.Succeeded)
                    HDOverTime = getOverTime.Data ?? 0;

                var CurrentYear = DateHelper.YearHijri(currentData.CalendarType);

                return Ok(await Result<object>.SuccessAsync(new
                {
                    HDOverTime,
                    CurrentYear
                }));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("ChkAddSalary")]
        public async Task<IActionResult> ChkAddSalary(string EmpCode, int Year, int Month)
        {
            try
            {
                string strMonth = Month.ToString("00");
                var getEmpId = await mainServiceManager.InvestEmployeeService.GetOne(x => x.Id,
                    x => x.EmpId == EmpCode && x.Isdel == false);
                if (!getEmpId.Succeeded)
                    return Ok(await Result.FailAsync(localization.GetResource1("EmployeeNotFound")));

                long empId = getEmpId.Data ?? 0;

                var chkMonth = await hrServiceManager.HrClearanceMonthService.GetAllVW(x => x.EmpId == empId
                && x.FinancelYear == Year && x.MsMonth == strMonth && x.IsDeleted == false);
                if (chkMonth.Data.Any())
                    return Ok(await Result.FailAsync("الشهر موجود مسبقاً"));

                var chkPayroll = await hrServiceManager.HrPayrollDService.GetAllVW(x => x.EmpId == empId && x.PayrollTypeId == 1
                && x.FinancelYear == Year && x.MsMonth == strMonth && x.IsDeleted == false);
                if (chkPayroll.Data.Any())
                    return Ok(await Result.FailAsync(" لم يتمكن من عملية الإضافة وذلك بسبب وجود  مسير رواتب لهذا الموظف في نفس الشهر "));

                return Ok(await Result.SuccessAsync());
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("GetPayrollClearance")]
        public async Task<IActionResult> GetPayrollClearance(string EmpCode)
        {
            try
            {
                var getData = await hrServiceManager.HrClearanceService.HR_Payroll_Clearance_Sp(EmpCode, "");
                if (getData.Succeeded)
                {
                    List<HrClearanceMonthVm> results = new();
                    foreach (DataRow row in getData.Data.Rows)
                    {
                        HrClearanceMonthVm obj = new()
                        {
                            FinancialYear = Convert.ToInt32(row["FinancelYear"]),
                            CountDayWork = Convert.ToInt32(row["Count_Day_Work"]),
                            DayAbsence = Convert.ToInt32(row["Day_Absence"]),
                            MDelay = Convert.ToInt64(row["M_Delay"]),
                            DayPrevMonth = Convert.ToInt32(row["Day_PrevMonth"]),
                            Note = Convert.ToString(row["Note"]),
                            Month = Convert.ToString(row["MS_Month"]),
                            MsDate = Convert.ToString(row["MS_Date"]),
                            Salary = Convert.ToDecimal(row["Salary"]),
                            Allowance = Convert.ToDecimal(row["allowance"]),
                            Deduction = Convert.ToDecimal(row["Deduction"]),
                            HExtraTime = Convert.ToDecimal(row["H_Extra_time"]),
                            Absence = Convert.ToDecimal(row["Absence"]),
                            Delay = Convert.ToDecimal(row["Delay"]),
                            Loan = Convert.ToDecimal(row["Loan"]),
                            DeductionOther = Convert.ToDecimal(row["Deduction_Other"]),
                            ExtraTime = Convert.ToDecimal(row["Extra_time"]),
                            AllowancesOther = Convert.ToDecimal(row["allowance_Other"]),
                            DueDayWork = Convert.ToDecimal(row["Due_Day_Work"]),
                            PrevMonth = Convert.ToDecimal(row["Due_PrevMonth"]),
                            Commission = Convert.ToDecimal(row["Commission"]),
                            Penalties = Convert.ToDecimal(row["Penalties"])
                        };

                        decimal allowance = obj.DueDayWork ?? 0 + obj.ExtraTime ?? 0 + obj.PrevMonth ?? 0 + obj.Commission ?? 0 + obj.AllowancesOther ?? 0;
                        decimal deduction = obj.Absence ?? 0 + obj.Delay ?? 0 + obj.Loan ?? 0 + obj.DeductionOther ?? 0 + obj.Penalties ?? 0;
                        obj.Total = allowance - deduction;

                        results.Add(obj);
                    }

                    return Ok(await Result<List<HrClearanceMonthVm>>.SuccessAsync(results));
                }

                return Ok(await Result.FailAsync(getData.Status.message));
            }
            catch
            {
                return Ok(await Result.FailAsync());
            }
        }

        [HttpPost("Add2")]
        public async Task<ActionResult> Add2(HrClearanceAddDto2 obj)
        {
            try
            {
                var chk = await permission.HasPermission(838, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var add = await hrServiceManager.HrClearanceService.Add2(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }

        // Edit page
        [HttpGet("GetById2")]
        public async Task<IActionResult> GetById2(long Id)
        {
            try
            {
                var chk = await permission.HasPermission(838, PermissionType.Edit);
                var chkView = await permission.HasPermission(838, PermissionType.View);
                if (!chk && !chkView)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));

                var item = await hrServiceManager.HrClearanceService.GetOneVW(x => x.Id == Id);
                if (item.Succeeded)
                {
                    var CHeckEmpInBranch = await hrServiceManager.HrEmployeeService.CHeckEmpInBranch(item.Data.EmpId);
                    if (CHeckEmpInBranch.Succeeded && CHeckEmpInBranch.Data == false)
                        return Ok(await Result.FailAsync(CHeckEmpInBranch.Status.message));

                    decimal HDOverTime = 0;
                    var getOverTime = await hrServiceManager.HrSettingService.GetOne(x => x.OverTime, x => x.FacilityId == currentData.FacilityId);
                    if (getOverTime.Succeeded)
                        HDOverTime = getOverTime.Data ?? 0;

                    var CurrentYear = DateHelper.YearHijri(currentData.CalendarType);

                    decimal Daily_Working_hours = 0;
                    var GetData = await hrServiceManager.HrClearanceService.GetData(item.Data.EmpCode, item.Data.DateC ?? "", 1);
                    if (GetData.Succeeded) Daily_Working_hours = GetData.Data.Daily_Working_hours ?? 0;

                    return Ok(await Result<object>.SuccessAsync(new { item.Data, HDOverTime, CurrentYear, Daily_Working_hours }));
                }
                return Ok(await Result.FailAsync(item.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetClearanceMonths")]
        public async Task<IActionResult> GetClearanceMonths(long ClearanceId)
        {
            try
            {
                var items = await hrServiceManager.HrClearanceMonthService.GetAll(x => x.ClearanceId == ClearanceId && x.IsDeleted == false);
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("AddClearanceMonth")]
        public async Task<ActionResult> AddClearanceMonth(HrClearanceMonthVm obj)
        {
            try
            {
                var chk = await permission.HasPermission(838, PermissionType.Edit);
                var chkView = await permission.HasPermission(838, PermissionType.View);
                if (!chk && !chkView)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var getEmpId = await mainServiceManager.InvestEmployeeService.GetOne(x => x.Id,
                    x => x.EmpId == obj.EmpCode && x.Isdel == false);
                if (!getEmpId.Succeeded)
                    return Ok(await Result.FailAsync(localization.GetResource1("EmployeeNotFound")));

                long empId = getEmpId.Data ?? 0;

                var chkMonth = await hrServiceManager.HrClearanceMonthService.GetAllVW(x => x.EmpId == empId
                && x.FinancelYear == obj.FinancialYear && x.MsMonth == obj.Month && x.IsDeleted == false);
                if (chkMonth.Data.Any())
                    return Ok(await Result.FailAsync("الشهر موجود مسبقاً"));

                var chkPayroll = await hrServiceManager.HrPayrollDService.GetAllVW(x => x.EmpId == empId && x.PayrollTypeId == 1
                && x.FinancelYear == obj.FinancialYear && x.MsMonth == obj.Month && x.IsDeleted == false);
                if (chkPayroll.Data.Any())
                    return Ok(await Result.FailAsync(" لم يتمكن من عملية الإضافة وذلك بسبب وجود  مسير رواتب لهذا الموظف في نفس الشهر "));

                HrClearanceMonthDto item = new()
                {
                    Id = 0,
                    FacilityId = Convert.ToInt32(currentData.FacilityId),
                    ClearanceId = obj.ClearanceId,
                    Absence = obj.Absence,
                    Allowance = obj.Allowance,
                    AllowanceOther = obj.AllowancesOther,
                    Commission = obj.Commission,
                    CountDayWork = obj.CountDayWork ?? 0,
                    DayAbsence = obj.DayAbsence ?? 0,
                    DayPrevMonth = obj.DayPrevMonth ?? 0,
                    Deduction = obj.Deduction,
                    DeductionOther = obj.DeductionOther,
                    Delay = obj.Delay,
                    DueDayWork = obj.DueDayWork,
                    DuePrevMonth = obj.PrevMonth,
                    FinancelYear = obj.FinancialYear ?? 0,
                    HExtraTime = obj.HExtraTime,
                    Loan = obj.Loan,
                    MsDate = obj.MsDate,
                    MsMonth = obj.Month,
                    MDelay = obj.MDelay ?? 0,
                    Note = obj.Note,
                    Penalties = obj.Penalties,
                    Salary = obj.Salary,
                    ExtraTime = obj.ExtraTime,
                };
                var add = await hrServiceManager.HrClearanceMonthService.Add(item);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("DeleteClearanceMonth")]
        public async Task<IActionResult> DeleteClearanceMonth(long Id, long ClearanceId)
        {
            try
            {
                var chk = await permission.HasPermission(838, PermissionType.Edit);
                var chkView = await permission.HasPermission(838, PermissionType.View);
                if (!chk && !chkView)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var delete = await hrServiceManager.HrClearanceMonthService.Remove(Id);
                if (delete.Succeeded)
                {
                    var items = await hrServiceManager.HrClearanceMonthService.GetAll(x => x.ClearanceId == ClearanceId && x.IsDeleted == false);
                    return Ok(items);
                }
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit2")]
        public async Task<ActionResult> Edit2(HrClearanceAddDto2 obj)
        {
            try
            {
                var chk = await permission.HasPermission(838, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var edit = await hrServiceManager.HrClearanceService.Edit2(obj);
                return Ok(edit);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }

        #endregion =============================== End Clearance 2 ==============================
    }
}