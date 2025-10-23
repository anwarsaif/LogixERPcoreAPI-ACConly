using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Helpers;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.HR.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Logix.MVC.LogixAPIs.HR
{
    //  الاجازات
    public class HRVacationsController : BaseHrApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData currentData;
        private readonly ILocalizationService localization;
        private readonly ISysConfigurationHelper configurationHelper;

        public HRVacationsController(IHrServiceManager hrServiceManager, IMainServiceManager mainServiceManager,
            IPermissionHelper permission,
            ICurrentData currentData,
            ILocalizationService localization,
            ISysConfigurationHelper configurationHelper)
        {
            this.mainServiceManager = mainServiceManager;
            this.permission = permission;
            this.currentData = currentData;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.configurationHelper = configurationHelper;
        }


        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrVacationsFilterDto filter)
        {

            var chk = await permission.HasPermission(169, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var items = await hrServiceManager.HrVacationsService.Search(filter);
                return Ok(items);

            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }


        [HttpPost("GetPagination")]
        public async Task<IActionResult> GetPagination([FromBody] HrVacationsFilterDto filter, int take = Pagination.take, long? lastSeenId = null)
        {
            try
            {
                var chk = await permission.HasPermission(169, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                filter.VacationTypeId ??= 0; filter.DeptId ??= 0; filter.LocationId ??= 0; filter.BranchId ??= 0;
                filter.TransTypeId ??= 0; filter.ClearnceId ??= 0;
                var BranchesList = currentData.Branches.Split(',');

                var getFromClearnce = await hrServiceManager.HrClearanceService.GetAll(x => x.VacationId, X => X.IsDeleted == false);
                var deptList = new List<string>();
                if (filter.DeptId > 0)
                {
                    var getDeptList = await mainServiceManager.SysDepartmentService.GetchildDepartment((long)filter.DeptId);
                    deptList = getDeptList.Data;
                }
                var dateConditions = new List<DateCondition>
                {
                    new() {
                        DatePropertyName = "VacationSdate",
                        ComparisonOperator = ComparisonOperator.GreaterThanOrEqual,
                        StartDateString = filter.StartDate ?? ""
                    },
                    new() {
                        DatePropertyName = "VacationEdate",
                        ComparisonOperator = ComparisonOperator.LessThanOrEqual,
                        StartDateString = filter.EndDate ?? ""
                    }
                };

                var items = await hrServiceManager.HrVacationsService.GetAllWithPaginationVW(selector: x => x.VacationId,
                    expression: e => e.IsDeleted == false
                && (filter.BranchId == 0 || filter.BranchId == e.BranchId)
                && BranchesList.Contains(e.BranchId.ToString())
                && (filter.VacationTypeId == 0 || filter.VacationTypeId == e.VacationTypeId)
                && (string.IsNullOrEmpty(filter.EmpCode) || filter.EmpCode == e.EmpCode)
                && (filter.DeptId == 0 || filter.DeptId == e.DeptId || deptList.Contains((e.DeptId ?? 0).ToString()))
                && (filter.LocationId == 0 || filter.LocationId == e.Location)
                && (
                        filter.ClearnceId == 0
                        || (filter.ClearnceId == 1 && getFromClearnce.Data.Contains(e.VacationId))
                        || (filter.ClearnceId == 2 && !getFromClearnce.Data.Contains(e.VacationId))
                   )
                && (filter.TransTypeId == 0 || filter.TransTypeId == e.TransTypeId),
                    take: take,
                    lastSeenId: lastSeenId,
                   dateConditions: (string.IsNullOrEmpty(filter.StartDate) || string.IsNullOrEmpty(filter.EndDate)) ? null : dateConditions);

                if (!items.Succeeded)
                    return StatusCode(items.Status.code, items.Status.message);

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        [HttpGet("OnLoad")]
        public async Task<IActionResult> OnLoad()
        {
            try
            {
                bool showDayType = true;
                var DefPaytype = await configurationHelper.GetValue(301, currentData.FacilityId);
                if (DefPaytype != "1") showDayType = false;

                var getSetting = await hrServiceManager.HrSettingService.GetAll(x => x.LeaveDeduction,
                    x => x.FacilityId == currentData.FacilityId);
                int? leaveDeduction = getSetting.Data.FirstOrDefault();

                return Ok(await Result<object>.SuccessAsync(new { ShowDayType = showDayType, LeaveDeduction = leaveDeduction }));
            }
            catch (Exception exp)
            {
                return Ok(await Result<object>.FailAsync($"{exp.Message}"));
            }
        }

        [HttpGet("OnEmpCodeChange")]
        public async Task<IActionResult> OnEmpCodeChange(string EmpCode, string? StartDate, string? EndDate, int? VacationTypeId)
        {
            try
            {
                string dateNow = DateTime.Now.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);
                string currentDate = "";
                if (string.IsNullOrEmpty(StartDate))
                    StartDate = dateNow;
                if (string.IsNullOrEmpty(EndDate))
                    EndDate = dateNow;

                var ReportDate = await configurationHelper.GetValue(263, currentData.FacilityId);
                if (ReportDate == "2") currentDate = StartDate;
                else if (ReportDate == "3") currentDate = EndDate;
                else currentDate = dateNow;

                long empId = 0;
                var getEmpId = await mainServiceManager.InvestEmployeeService.GetOne(x => x.Id,
                    x => x.EmpId == EmpCode && x.Isdel == false && x.IsDeleted == false);
                empId = getEmpId.Data ?? 0;

                var getBalance = await hrServiceManager.HrVacationsService.Vacation_Balance_FN(currentDate, empId);
                bool chkBalance = false;
                var getVacationType = await hrServiceManager.HrVacationsTypeService.GetOne(i => i.ValidateBalance, i => i.VacationTypeId == VacationTypeId && i.IsDeleted == false);
                chkBalance = getVacationType.Data ?? false;

                decimal balance = 0;
                if (chkBalance)
                    balance = getBalance;

                return Ok(await Result<object>.SuccessAsync(new { Balance = balance }));
            }
            catch (Exception exp)
            {
                return Ok(await Result<object>.FailAsync($"{exp.Message}"));
            }
        }

        [HttpGet("VacationTypeIdChange")]
        public async Task<IActionResult> VacationTypeIdChange(string EmpCode, int VacationTypeId, string? StartDate, string? EndDate)
        {
            try
            {
                if (string.IsNullOrEmpty(EmpCode))
                    return Ok(await Result<EmpIdChangedVM>.SuccessAsync(localization.GetResource1("EmployeeIsNumber")));

                var DateGregorian = DateTime.Now.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);
                if (string.IsNullOrEmpty(StartDate))
                    StartDate = DateGregorian;
                if (string.IsNullOrEmpty(EndDate))
                    EndDate = DateGregorian;

                long empId = 0;
                var getEmpId = await mainServiceManager.InvestEmployeeService.GetOne(x => x.Id,
                    x => x.EmpId == EmpCode && x.Isdel == false && x.IsDeleted == false);
                empId = getEmpId.Data ?? 0;

                var getBalance = await hrServiceManager.HrVacationsService.Vacation_Balance2_FN(DateGregorian, empId, VacationTypeId);
                // get totalSalary & salary_insurance_wage
                decimal totalSalary = 0; decimal salaryInsuranceWage = 0;
                var getEmpData = await hrServiceManager.HrEmployeeService.GetOneVW(i => i.EmpId == EmpCode && i.IsDeleted == false && i.Isdel == false);
                salaryInsuranceWage = getEmpData.Data.SalaryInsuranceWage ?? 0;
                var getAllowanceAmount = await hrServiceManager.HrAllowanceDeductionService.GetAll(i => i.Amount,
                    i => i.EmpId == empId && i.IsDeleted == false && i.TypeId == 1 && i.FixedOrTemporary == 1);
                totalSalary = (getEmpData.Data.Salary ?? 0) + (getAllowanceAmount.Data.Sum() ?? 0);

                var getVacationType = await hrServiceManager.HrVacationsTypeService.GetOne(x => x.VacationTypeId == VacationTypeId);
                var vacationType = getVacationType.Data;
                int catId = vacationType.CatId ?? 0;

                var totalSlaryType = await configurationHelper.GetValue(396, currentData.FacilityId);
                if (totalSlaryType == "1" && catId == 2)
                    totalSalary = Math.Round(salaryInsuranceWage, 2);

                decimal balance = 0;
                var ReportDate = await configurationHelper.GetValue(263, currentData.FacilityId);
                if (ReportDate == "2") balance = await GetVacationBalance(empId, VacationTypeId, StartDate);
                else if (ReportDate == "3") balance = await GetVacationBalance(empId, VacationTypeId, EndDate);
                else balance = await GetVacationBalance(empId, VacationTypeId, DateGregorian);

                bool isSalaryChk = false; bool enableSalaryChk = true;
                if (vacationType.DeductionDays == 1)
                {
                    isSalaryChk = true;
                    enableSalaryChk = false;
                }

                decimal deductionDay = 0; bool pnlDeduction = false;
                if (vacationType.DeductionDays > 0 && vacationType.DeductionDays < 1)
                {
                    pnlDeduction = true;
                    deductionDay = vacationType.DeductionDays ?? 0;
                }
                bool ChkNeedJoinRequest = vacationType.NeedJoinRequest ?? false;

                return Ok(await Result<object>.SuccessAsync(new
                {
                    TotalSalary = totalSalary,
                    Balance = balance,
                    IsSalaryChk = isSalaryChk,
                    EnableChkSalary = enableSalaryChk,
                    PnlDeductionVisible = pnlDeduction,
                    DeductionDay = deductionDay,
                    NeedJoinRequest = ChkNeedJoinRequest
                }));
            }
            catch (Exception exp)
            {
                return Ok(await Result<object>.FailAsync($"{exp.Message}"));
            }
        }

        [NonAction]
        public async Task<decimal> GetVacationBalance(long EmpId, int VacationTypeId, string CurrDate)
        {
            try
            {
                var getBalance = await hrServiceManager.HrVacationsService.Vacation_Balance2_FN(CurrDate, EmpId, VacationTypeId);
                bool chkBalance = false;
                var getVacationType = await hrServiceManager.HrVacationsTypeService.GetOne(i => i.ValidateBalance, i => i.VacationTypeId == VacationTypeId && i.IsDeleted == false);
                chkBalance = getVacationType.Data ?? false;

                decimal balance = 0;
                if (chkBalance) balance = getBalance;
                else
                {
                    if (VacationTypeId == 1) balance = getBalance;
                    else balance = 0;
                }
                return balance;
            }
            catch
            {
                return 0;
            }
        }

        [HttpPost("GetDays")]
        public async Task<IActionResult> GetDays(GetDaysClickDto obj)
        {
            try
            {
                decimal No_OF_Days = await hrServiceManager.HrVacationsService.GetCountDays(obj.SDate ?? "", obj.EDate ?? "", obj.VacationTypeId);
                var getValue = await hrServiceManager.HrVacationsDayTypeService.GetOne(x => x.Value, x => x.Id == obj.VacationsDayType);
                decimal Vacations_Day_Type_Value = getValue.Data ?? 0;
                No_OF_Days = No_OF_Days * Vacations_Day_Type_Value;
                decimal VacationAccountDay = No_OF_Days;
                decimal DeductionAmount = Math.Round(No_OF_Days * ((obj.DeductionDays ?? 0) * ((obj.TotalSlary ?? 0) / 30)), 2);

                long empId = 0;
                var getEmpId = await mainServiceManager.InvestEmployeeService.GetOne(x => x.Id,
                    x => x.EmpId == obj.EmpCode && x.Isdel == false && x.IsDeleted == false);
                empId = getEmpId.Data ?? 0;

                decimal balance = 0;
                var ReportDate = await configurationHelper.GetValue(263, currentData.FacilityId);
                if (ReportDate == "2") balance = await GetVacationBalance(empId, obj.VacationTypeId, obj.SDate ?? "");
                else if (ReportDate == "3") balance = await GetVacationBalance(empId, obj.VacationTypeId, obj.EDate ?? "");
                else balance = await GetVacationBalance(empId, obj.VacationTypeId, DateTime.Now.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture));

                return Ok(await Result<object>.SuccessAsync(new
                {
                    VacationAccountDay,
                    Balance = balance,
                    DeductionAmount
                }));
            }
            catch (Exception exp)
            {
                return Ok(await Result<object>.FailAsync($"{exp.Message}"));
            }
        }

        [HttpPost("Add")]
        public async Task<IActionResult> AddVacations(HrVacationsDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(169, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                obj.VacationTypeId ??= 0; obj.VacationsDayTypeId ??= 1;

                if (string.IsNullOrEmpty(obj.EmpCode))
                {
                    return Ok(await Result<HrVacationsDto>.FailAsync(localization.GetResource1("EmployeeIsNumber")));
                }
                if (string.IsNullOrEmpty(obj.VacationEdate))
                {
                    return Ok(await Result<HrVacationsDto>.FailAsync($"يجب ادخال الى تاريخ "));
                }
                if (string.IsNullOrEmpty(obj.VacationSdate))
                {
                    return Ok(await Result<HrVacationsDto>.FailAsync($"يجب ادخال من تاريخ "));
                }
                if (obj.VacationTypeId <= 0)
                {
                    return Ok(await Result<HrVacationsDto>.FailAsync($"يجب ادخال النوع"));
                }

                var addRes = await hrServiceManager.HrVacationsService.AddVacations(obj);
                return Ok(addRes);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrVacationsDto>.FailAsync(ex.Message));
            }
        }


        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long? Id)
        {
            try
            {
                var chkEdit = await permission.HasPermission(169, PermissionType.Edit);
                var chkView = await permission.HasPermission(169, PermissionType.View);
                if (!chkEdit && !chkView)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id <= 0)
                    return Ok(await Result.FailAsync("Id is Required"));

                var getData = await hrServiceManager.HrVacationsService.GetAllVW(x => x.VacationId == Id && x.IsDeleted == false);
                return Ok(getData);
            }
            catch (Exception exp)
            {
                return Ok(await Result<object>.FailAsync($"{exp.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(HrVacationsEditDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(169, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result<HrVacationsEditDto>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var EditRes = await hrServiceManager.HrVacationsService.EditVacations(obj);
                return Ok(EditRes);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrVacationsEditDto>.FailAsync(ex.Message));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long Id = 0)
        {
            try
            {
                var chk = await permission.HasPermission(169, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id == 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("ChooseDelete")}"));

                var del = await hrServiceManager.HrVacationsService.Remove(Id);
                return Ok(del);
            }
            catch (Exception exp)
            {
                return Ok(await Result.FailAsync($"{exp.Message}"));
            }
        }

        /// <summary>
        /// لمعرفه هل نجعل زر تصفية مستحقات الإجازة متاح ام ملغي وذلك عن طريق البحث عن تصفية الاجازة
        /// </summary>
        /// <param name="Id">رقم الاجازة</param>
        /// <returns></returns>
        [HttpGet("ChkClearanceBtn")]
        public async Task<IActionResult> ChkClearanceBtn(long VacationId)
        {
            try
            {
                if (VacationId <= 0)
                    return Ok(await Result.FailAsync("Id is Required"));

                bool ShowClearanceBtn = true; long ClearanceId = 0;
                var getData = await hrServiceManager.HrClearanceService.GetAllVW(v => v.VacationId == VacationId && v.IsDeleted == false);
                if (getData.Data.Any())
                {
                    ShowClearanceBtn = false;
                    var clearanceData = getData.Data.FirstOrDefault();
                    ClearanceId = clearanceData != null ? clearanceData.Id : 0;
                }
                return Ok(await Result<object>.SuccessAsync(new { ShowClearanceBtn, ClearanceId }, ""));
            }
            catch (Exception exp)
            {
                return Ok(await Result<object>.FailAsync($"{exp.Message}"));
            }
        }


        #region ================================ Vacations Report ================================
        [HttpPost("VacationsReport")]
        public async Task<IActionResult> VacationsReport(HrVacationsFilterDto filter)
        {
            try
            {
                var chk = await permission.HasPermission(195, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var res = await hrServiceManager.HrVacationsService.VacationReportSearch(filter);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpPost("GetVacationReportPagination")]
        public async Task<IActionResult> GetVacationReportPagination([FromBody] HrVacationsFilterDto filter, int take = Pagination.take, long? lastSeenId = null)
        {
            try
            {
                if (!await permission.HasPermission(195, PermissionType.Show))
                    return Ok(await Result.AccessDenied("AccessDenied"));
                var grouped = await hrServiceManager.HrVacationsService.GetVacationReportPaginationGrouped(filter, take, lastSeenId);
                return Ok(grouped);

            }
            catch (Exception ex)
            {
                return Ok(await Result<List<HrVacationsFilterDto>>.FailAsync(ex.Message));
            }
        }

        #endregion ============================ End Vacation Report =============================

        #region ============================ Employee Vacations Report ============================
        [HttpPost("EmpVacationsReport")]
        public async Task<IActionResult> EmpVacationsReport(HrRPVacationEmployeeFilterDto filter)
        {
            try
            {
                var chk = await permission.HasPermission(907, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var items = await hrServiceManager.HrVacationsService.HRRVacationEmployeeSearch(filter);
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpPost("EmpVacationsReportPagination")]
        public async Task<IActionResult> EmpVacationsReportPagination([FromBody] HrRPVacationEmployeeFilterDto filter, int take = Pagination.take, long? lastSeenId = null, CancellationToken cancellationToken = default)
        {
            try
            {
                // تحقق الصلاحيات (إذا بدك)
                var chk = await permission.HasPermission(907, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var branchesList = currentData.Branches.Split(',');

                filter.BranchId ??= 0;
                filter.DeptId ??= 0;
                filter.LocationId ??= 0;
                filter.VacationTypeId ??= 0;

                // الأقسام الفرعية
                List<int> deptList = new();
                if (filter.DeptId > 0)
                {
                    var getDeptList = await mainServiceManager.SysDepartmentService.GetchildDepartment((long)filter.DeptId);
                    if (getDeptList.Data.Any())
                        deptList = getDeptList.Data.Where(x => !string.IsNullOrEmpty(x)).Select(x => Convert.ToInt32(x)).ToList();
                }

                // شروط التاريخ (نفس اللي عاملينه في GetPagination)
                List<DateCondition>? dateConditions = null;
                if (!string.IsNullOrEmpty(filter.StartDate) && !string.IsNullOrEmpty(filter.EndDate))
                {
                    dateConditions = new List<DateCondition>
            {
                new() {
                    DatePropertyName = "VacationSdate",
                    ComparisonOperator = ComparisonOperator.GreaterThanOrEqual,
                    StartDateString = filter.StartDate
                },
                new() {
                    DatePropertyName = "VacationEdate",
                    ComparisonOperator = ComparisonOperator.LessThanOrEqual,
                    StartDateString = filter.EndDate
                }
            };
                }
                // جلب البيانات باستخدام السيرفس مباشرة مع Pagination
                var items = await hrServiceManager.HrVacationsService.GetAllWithPaginationVW(
                    selector: x => x.VacationId,
                    expression: e => e.IsDeleted == false
                           && ((filter.BranchId == 0 && branchesList.Contains(e.BranchId.ToString())) || (filter.BranchId > 0 && e.BranchId == filter.BranchId))
                && (string.IsNullOrEmpty(filter.EmpCode) || e.EmpCode == filter.EmpCode)
                && (filter.VacationTypeId == 0 || filter.VacationTypeId == e.VacationTypeId)
                && (filter.LocationId == 0 || filter.LocationId == e.Location)
                && (filter.DeptId == 0 || filter.DeptId == e.DeptId || deptList.Contains(e.DeptId ?? 0)),
                    take: take,
                    lastSeenId: lastSeenId,
                    dateConditions: dateConditions
                );


                if (!items.Succeeded)
                    return Ok(await Result<List<HrVacationsVw>>.FailAsync(items.Status.message));

                // تحويل النتائج إلى DTO
                var resultList = items.Data.Select(item => new HrRPVacationEmployeeFilterDto
                {
                    VacationId = item.VacationId,
                    EmpCode = item.EmpCode,
                    EmpName = item.EmpName,
                    EmpName2 = item.EmpName2,
                    BraName = item.BraName,
                    BraName2 = item.BraName2,
                    DeptName = item.DepName,
                    DeptName2 = item.DepName2,
                    LocationName = item.LocationName,
                    LocationName2 = item.LocationName2,
                    VacationTypeName = item.VacationTypeName,
                    VacationTypeName2 = item.VacationTypeName2,
                    StartDate = item.VacationSdate,
                    EndDate = item.VacationEdate,
                    VacationAccountDay = item.VacationAccountDay,
                    IsSalary = item.IsSalary,
                    VacationAlternetiveEmpNo = item.AlternativeEmpCode,
                    VacationAlternetiveEmpName = item.AlternativeEmpName,
                    Note = item.Note
                }).ToList();

                var paginatedData = new PaginatedResult<object>
                {
                    Succeeded = true,
                    Data = resultList,
                    Status = items.Status,
                    PaginationInfo = items.PaginationInfo
                };

                return Ok(paginatedData);
            }
            catch (Exception ex)
            {
                return Ok(await Result<List<HrRPVacationEmployeeFilterDto>>.FailAsync(ex.Message));
            }
        }

        #endregion ========================= End Employee Vacations Report ========================
    }
}