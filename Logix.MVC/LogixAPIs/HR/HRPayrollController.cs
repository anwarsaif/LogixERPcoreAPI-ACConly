using System.Globalization;
using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.DTOs.Main;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
//using Result = Logix.Application.Wrapper.Result;

namespace Logix.MVC.LogixAPIs.HR
{
    //   مسير الرواتب تلقائي
    public class HRPayrollController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly ISysConfigurationHelper sysConfigurationHelper;
        private readonly IMainServiceManager mainServiceManager;


        public HRPayrollController(IHrServiceManager hrServiceManager,
            IPermissionHelper permission,
            ICurrentData session,
            ILocalizationService localization,
            ISysConfigurationHelper sysConfigurationHelper,
            IMainServiceManager mainServiceManager)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.sysConfigurationHelper = sysConfigurationHelper;
            this.mainServiceManager = mainServiceManager;
        }

        #region ================================ Search & Delete ================================
        [HttpGet("GetCurrentYear")]
        public async Task<IActionResult> GetCurrentYear()
        {
            try
            {
                var CurrentYear = DateHelper.YearHijri(session.CalendarType);
                return Ok(await Result<object>.SuccessAsync(new { CurrentYear }));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrPayrollFilterDto filter)
        {
            try
            {
                var chk = await permission.HasPermission(168, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (filter.FinancelYear == 0 || filter.FinancelYear == null)
                    return Ok(await Result<HrPayrollFilterDto>.FailAsync(" يجب اختيار السنة المالية"));

                var BranchesList = session.Branches.Split(',');
                long facilityId = session.FacilityId;
                filter.PayrollTypeId ??= 0; filter.FinancelYear ??= 0; filter.MsCode ??= 0; filter.ApplicationCode ??= 0;

                var items = await hrServiceManager.HrPayrollService.GetAllVW(e => e.IsDeleted == false
                && (e.BranchId == 0 || BranchesList.Contains(e.BranchId.ToString()))
                && (string.IsNullOrEmpty(filter.MsMonth) || filter.MsMonth == e.MsMonth)
                && (filter.PayrollTypeId == 0 || filter.PayrollTypeId == e.PayrollTypeId)
                && (filter.FinancelYear == 0 || filter.FinancelYear == e.FinancelYear)
                && (filter.MsCode == 0 || filter.MsCode == e.MsCode)
                && (filter.ApplicationCode == 0 || filter.ApplicationCode == e.ApplicationCode)
                && (facilityId == 1 || facilityId == e.FacilityId));

                if (items.Succeeded)
                {
                    var res = items.Data.OrderByDescending(x => x.MsId).ToList();
                    return Ok(await Result<List<HrPayrollVw>>.SuccessAsync(res, ""));
                }
                return Ok(await Result.FailAsync(items.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long Id)
        {
            try
            {
                var chk = await permission.HasPermission(168, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await hrServiceManager.HrPayrollService.Remove(Id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete HR Payroll Controller, MESSAGE: {ex.Message}"));
            }
        }
        #endregion ============================ End Search & Delete =============================


        #region ====================================== Add ======================================
        [HttpGet("GetMonthDescription")]
        public async Task<IActionResult> GetMonthDescription(int Year, string MonthCode)
        {
            try
            {
                var culuture = new CultureInfo("en-US"); string format = "yyyy/MM/dd";
                var items = await hrServiceManager.HrPayrollTransactionTypeValueService.GetAllVW(x => x.FacilityId == 1);
                if (items.Succeeded)
                {
                    List<object> results = new();
                    int MonthStartDay = 1; int MonthEndDay = 1;
                    var getSetting = await hrServiceManager.HrSettingService.GetAll(x => x.FacilityId == 1);
                    var setting = getSetting.Data.FirstOrDefault();
                    if (setting != null)
                    {
                        MonthStartDay = Convert.ToInt32(setting.MonthStartDay);
                        MonthEndDay = Convert.ToInt32(setting.MonthEndDay);
                    }

                    // when value = 1 => set basic start & end date (from 01 to last day of month)
                    string defaultDescription = "";
                    DateTime defaultStartDate = DateHelper.StringToDate($"{Year}/{MonthCode}/01");
                    DateTime defaultEndtDate = defaultStartDate.AddMonths(1).AddDays(-1);
                    defaultDescription = $"بداية الشهر من: {defaultStartDate.ToString(format, culuture)}" +
                        $" إلى: {defaultEndtDate.ToString(format, culuture)}";

                    // when value = 2 => calculate start & end date based on HrSetting
                    int year = Year; int month = Convert.ToInt32(MonthCode);
                    if (month == 1 && MonthStartDay != 1)
                    {
                        year -= 1;
                        month = 12;
                    }
                    else
                    {
                        if (MonthStartDay != 1)
                            month -= 1;
                    }

                    string settingDescription = "";
                    DateTime settingStartDate = DateHelper.StringToDate($"{year}/{month:00}/01").AddDays(MonthStartDay - 1);
                    DateTime settingEndtDate = DateHelper.StringToDate($"{Year}/{MonthCode:00}/01").AddDays(MonthEndDay - 1);
                    settingDescription = $"بداية الشهر في الاعدادات من: {settingStartDate.ToString(format, culuture)}" +
                        $" إلى: {settingEndtDate.ToString(format, culuture)}";

                    foreach (var item in items.Data)
                    {
                        if (item.Value == 1) // شهر كامل
                            results.Add(new { item.Name, item.Name2, Description = defaultDescription });
                        else if (item.Value == 2) // بناءً على الإعدادات
                            results.Add(new { item.Name, item.Name2, Description = settingDescription });
                        else
                            results.Add(new { item.Name, item.Name2, Description = "لا يوجد شرح لهذه القيمة" });
                    }

                    return Ok(await Result<object>.SuccessAsync(results));
                }

                return Ok(await Result<object>.SuccessAsync(new { items }));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpPost("SearchInAdd")]
        public async Task<IActionResult> SearchInAdd(HRPayrollCreateSpFilterDto filter)
        {
            try
            {
                var chk = await permission.HasPermission(168, PermissionType.Show);
                if (!chk)
                {
                    return Ok(await Result.AccessDenied("AccessDenied"));
                }

                List<HRPayrollCreate2SpDto> result = new();
                filter.BRANCHID ??= 0; filter.FacilityID ??= 0; filter.Location ??= 0; filter.DeptID ??= 0;
                filter.SalaryGroupID ??= 0; filter.PaymentTypeID ??= 0; filter.WagesProtection ??= 0; filter.SponsorsID ??= 0;
                filter.Branches = session.Branches;
                if (string.IsNullOrEmpty(filter.FinancelYear))
                    return Ok(await Result.FailAsync(" يجب اختيار السنة المالية"));
                if (string.IsNullOrEmpty(filter.MSMonth))
                    return Ok(await Result.FailAsync(" يجب اختيار الشهر"));

                DateTime monthEndDate = new DateTime(Convert.ToInt32(filter.FinancelYear), Convert.ToInt32(filter.MSMonth), 1).AddMonths(1).AddDays(-1);
                var monthEnd = monthEndDate.Day.ToString("00");

                var propertyId = await sysConfigurationHelper.GetValue(178, session.FacilityId);
                if (propertyId == "1")
                {
                    if (filter.BRANCHID <= 0)
                    {
                        return Ok(await Result.FailAsync("اختر الفرع"));
                    }
                }

                // null is the default value in stored procedure parameters
                if (filter.SalaryGroupID <= 0)
                    filter.SalaryGroupID = null;

                if (string.IsNullOrEmpty(filter.ContractTypeID) || filter.ContractTypeID.Length <= 0 || filter.ContractTypeID == "null")
                    filter.ContractTypeID = null;

                if (filter.PaymentTypeID <= 0)
                    filter.PaymentTypeID = null;

                if (filter.SponsorsID <= 0)
                    filter.SponsorsID = null;

                if (filter.WagesProtection <= 0)
                    filter.WagesProtection = null;

                var items = await hrServiceManager.HrPayrollService.getHR_Payroll_Create_Sp(filter);
                if (items.Succeeded)
                {
                    if (items.Data.Count() > 0)
                    {
                        var sDate = DateHelper.StringToDate($"{filter.FinancelYear}/{filter.MSMonth}/01");
                        var eDate = DateHelper.StringToDate($"{filter.FinancelYear}/{filter.MSMonth}/{monthEnd}");

                        // get Increments
                        var getIncerementsAll = await hrServiceManager.HrIncrementService.GetAll(x => x.IsDeleted == false && x.ApplyType == 2);
                        if (!getIncerementsAll.Succeeded)
                            return Ok(await Result.FailAsync(getIncerementsAll.Status.message));

                        var filteredIncrements = getIncerementsAll.Data.Where(x => !string.IsNullOrEmpty(x.StartDate)
                            && DateHelper.StringToDate(x.StartDate) >= sDate && DateHelper.StringToDate(x.StartDate) <= eDate).ToList();

                        // get Contractes (Raises)
                        var getRaisesAll = await hrServiceManager.HrContracteService.GetAll(x => x.IsDeleted == false && x.ApplyType == 2);
                        if (!getRaisesAll.Succeeded)
                            return Ok(await Result.FailAsync(getRaisesAll.Status.message));

                        var filteredRaises = getRaisesAll.Data.Where(x => !string.IsNullOrEmpty(x.StartDate)
                            && DateHelper.StringToDate(x.StartDate) >= sDate && DateHelper.StringToDate(x.StartDate) <= eDate).ToList();

                        string Daily_Working_hours = "";
                        string Emp_Have_Increements = "";
                        string Emp_Have_Raises = "";
                        string Emp_JoinWork = "";

                        foreach (var item in items.Data)
                        {
                            if (item.Daily_Working_hours == null || Convert.ToDecimal(item.Daily_Working_hours) <= 0)
                            {
                                Daily_Working_hours += item.Emp_ID + ",";
                                continue;
                            }

                            var empIncrements = filteredIncrements.Where(x => x.EmpId == item.ID).ToList();
                            if (empIncrements.Any())
                            {
                                Emp_Have_Increements += item.Emp_ID + ",";
                            }

                            var empRaises = filteredRaises.Where(x => x.EmpId == item.ID).ToList();
                            if (empRaises.Any())
                            {
                                Emp_Have_Raises += item.Emp_ID + ",";
                            }
                            result.Add(item);
                        }

                        if (Emp_Have_Increements.Length > 0)
                            Emp_Have_Increements = $"{localization.GetHrResource("increasesNote")} \n {Emp_Have_Increements.TrimEnd(',')}";

                        if (Daily_Working_hours.Length > 0)
                            Daily_Working_hours = $"{localization.GetResource1("EmpNotHaveWorkHoursEntered")} \n {Daily_Working_hours.TrimEnd(',')}";

                        if (Emp_Have_Raises.Length > 0)
                            Emp_Have_Raises = $"{localization.GetHrResource("RaisesNote")} \n {Emp_Have_Raises.TrimEnd(',')}";

                        var Check_JoinWork_Date = "0";
                        Check_JoinWork_Date = await sysConfigurationHelper.GetValue(137, Convert.ToInt64(filter.FacilityID));
                        if (Check_JoinWork_Date == "1")
                        {
                            var checkEmpJoinWork = await hrServiceManager.HrPayrollService.CheckJoinWorkForPayroll(filter);
                            if (checkEmpJoinWork.Succeeded)
                            {
                                if (!string.IsNullOrEmpty(checkEmpJoinWork.Data))
                                {
                                    Emp_JoinWork = $"{localization.GetHrResource("NoVacationEffectiveDateNotice")} \n {checkEmpJoinWork.Data.TrimEnd(',')}";
                                }
                            }
                        }

                        return Ok(await Result<object>.SuccessAsync(new
                        {
                            data = result,
                            EmpHaveIncreements = Emp_Have_Increements,
                            DailyWorkingHours = Daily_Working_hours,
                            EmpHaveRaises = Emp_Have_Raises,
                            JoinWork = Emp_JoinWork
                        }, ""));
                    }
                    return Ok(await Result<List<HRPayrollCreate2SpDto>>.SuccessAsync(items.Data.ToList(), localization.GetResource1("NosearchResult")));
                }
                return Ok(await Result.FailAsync(items.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpPost("Add")]
        public async Task<ActionResult> Add(HrPayrollAddDto obj)
        {
            try
            {
                var CurrentDate = DateTime.Now.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);
                var chk = await permission.HasPermission(168, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result<string>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));
                if (obj.SpDtos.Count <= 0)
                    return Ok(await Result<string>.FailAsync("يجب توافر بيانات المسير "));
                if (obj.FinancelYear <= 0)
                    return Ok(await Result<string>.FailAsync("يجب تحديد السنة المالية "));


                if (obj.FacilityId <= 0 || obj.FacilityId == null) obj.FacilityId = (int?)session.FacilityId;
                obj.PayrollTypeId = 1;
                obj.State = 1;
                obj.MsDate = CurrentDate;
                var add = await hrServiceManager.HrPayrollService.AddNewAutomaticPayroll1(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"====== Exp in Add HR   Payroll Controller  Add Method, MESSAGE: {ex.Message}"));
            }
        }
        #endregion ================================== End Add ===================================




        #region EditPage


        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long Id)
        {
            try
            {
                var result = new HrPayrollEditDto();
                result.fileDtos = new List<SaveFileDto>();

                var chk = await permission.HasPermission(168, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id <= 0)
                {
                    return Ok(await Result<object>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var item = await hrServiceManager.HrPayrollService.GetOneVW(x => x.MsId == Id);
                var details = await hrServiceManager.HrPayrollDService.GetAllVW(x => x.MsId == Id && x.IsDeleted == false);
                var notes = await hrServiceManager.HrPayrollNoteService.GetAllVW(x => x.MsId == Id);
                if (item.Succeeded == false || item.Data == null) return Ok(item);
                result.MsId = item.Data.MsId;
                result.MsTitle = item.Data.MsTitle;
                result.DueDate = item.Data.DueDate;
                result.PaymentDate = item.Data.PaymentDate;
                result.FacilityId = item.Data.FacilityId;
                result.FinancelYear = item.Data.FinancelYear;
                result.MsMonth = item.Data.MsMonth;

                if (details.Succeeded && details.Data != null)
                {
                    result.payrollDVw = details.Data.OrderBy(x => x.EmpCode).ToList();
                }
                if (notes.Succeeded && notes.Data != null)
                {
                    result.notes = notes.Data.ToList();
                }

                var GetSysFiles = await mainServiceManager.SysFileService.GetAll(x => x.IsDeleted == false && x.PrimaryKey == Id && x.TableId == 37);
                if (GetSysFiles.Data != null)
                {
                    foreach (var file in GetSysFiles.Data)
                    {
                        var singleFile = new SaveFileDto
                        {
                            Id = file.Id,
                            FileName = file.FileName ?? "",
                            FileURL = file.FileUrl,
                            IsDeleted = file.IsDeleted,
                            FileDate = file.FileDate
                        };
                        result.fileDtos.Add(singleFile);
                    }
                }

                return Ok(await Result<object>.SuccessAsync(result, "", 200));

            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"====== Exp in HR Manual Payroll Controller getById, MESSAGE: {ex.Message}"));
            }
        }
        [HttpPost("SearchInEdit")]
        public async Task<IActionResult> SearchInEdit(HrPayrollFilter2Dto filter)
        {
            List<HrPayrollFilter2ResultDto> result = new List<HrPayrollFilter2ResultDto>();

            var chk = await permission.HasPermission(168, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                //filter.FinancelYear ??= 0;
                //filter.FinancelYear = 0;
                filter.FacilityId ??= 0;
                filter.DeptId ??= 0;
                filter.BranchId ??= 0;
                filter.Location ??= 0;
                filter.SponsorsId ??= 0;
                filter.WagesProtection ??= 0;
                filter.NationalityId ??= 0;
                filter.SalaryGroupId ??= 0;
                filter.PayrollTypeId ??= 0;
                var BranchesList = session.Branches.Split(',');
                if (filter.MsId <= 0)
                {
                    return Ok(await Result<object>.FailAsync("رقم المسير مطلوب"));

                }
                //  هنا يجب ارسال رقم المسير والسنة المالية والشهر للحصول على بيانات صحيحة
                var items = await hrServiceManager.HrPayrollDService.GetAllVW(e =>
                e.IsDeleted == false &&
                (filter.MsId == 0 || e.MsId == filter.MsId) &&
                //(filter.FinancelYear == 0 || filter.FinancelYear == e.FinancelYear) &&
                (filter.FacilityId == 0 || e.FacilityId == filter.FacilityId) &&
                (filter.DeptId == 0 || e.DeptId == filter.DeptId) &&
                (filter.BranchId == 0 || e.BranchId == filter.BranchId) &&
                (filter.Location == 0 || e.Location == filter.Location) &&
                (filter.SponsorsId == 0 || e.SponsorsId == filter.SponsorsId) &&
                (filter.NationalityId == 0 || e.NationalityId == filter.NationalityId) &&
                (filter.WagesProtection == 0 || e.WagesProtection == filter.WagesProtection) &&
                (filter.SalaryGroupId == 0 || e.SalaryGroupId == filter.SalaryGroupId) &&
                (string.IsNullOrEmpty(filter.EmpCode) || e.EmpCode == filter.EmpCode) &&
                (string.IsNullOrEmpty(filter.EmpName) || e.EmpName == filter.EmpName) &&
                (filter.PayrollTypeId == 0 || e.PayrollTypeId == filter.PayrollTypeId) &&
                (string.IsNullOrEmpty(filter.MsMonth) || e.MsMonth == filter.MsMonth));
                if (items.Succeeded)
                {
                    if (items.Data.Count() > 0)
                    {
                        var res = items.Data.ToList();
                        if (filter.BranchId > 0)
                        {
                            res = res.Where(x => x.BranchId == filter.BranchId).ToList();
                        }

                        if (res.Any())
                        {
                            foreach (var item in res)
                            {
                                decimal GosiDeduction = 0;
                                var SelectGosi = await hrServiceManager.HrPayrollAllowanceDeductionService.GetAllVW(x => x.Debit > 0 && x.IsDeleted == false && x.AdId == 1 && x.EmpCode == item.EmpCode && x.MsId == filter.MsId);

                                if (SelectGosi.Data.Count() > 0)
                                    GosiDeduction = SelectGosi.Data.FirstOrDefault().AdValue ?? 0;

                                var newRecord = new HrPayrollFilter2ResultDto
                                {
                                    EmpCode = item.EmpCode,
                                    Emp_ID = item.EmpCode,
                                    Emp_Name = (session.Language == 1) ? item.EmpName : item.EmpName2,
                                    BraName = (session.Language == 1) ? item.BraName : item.BraName2,
                                    Dep_Name = (session.Language == 1) ? item.DepName : item.DepName2,
                                    Location_Name = (session.Language == 1) ? item.LocationName : item.LocationName2,
                                    Salary = item.Salary,
                                    Allowance = item.Allowance,
                                    H_OverTime = item.HOverTime,
                                    OverTime = item.OverTime,
                                    Commission = item.Commission,
                                    Mandate = item.Mandate,
                                    //Total = item.Salary + item.Allowance + item.OverTime + item.Commission + item.Mandate,
                                    Total = item.BasicSalary + item.AllowanceOrignal,
                                    Attendance = item.Attendance,
                                    Absence = item.Absence,
                                    Delay = item.Delay,
                                    Loan = item.Loan,
                                    Penalties = item.Penalties,
                                    Deduction = item.Deduction,
                                    TotalDeductions = item.Absence + item.Delay + item.Loan + item.Deduction + item.Penalties,
                                    Net = item.Net,
                                    CostCenterCode = item.CostCenterCode,
                                    Bank_ID = item.BankId,
                                    Bank_Name = item.BankName,
                                    BasicSalary = item.BasicSalary,
                                    GosiDeduction = GosiDeduction
                                };
                                result.Add(newRecord);
                            }

                            return Ok(await Result<List<HrPayrollFilter2ResultDto>>.SuccessAsync(result, ""));

                        }
                        return Ok(await Result<List<HrPayrollFilter2ResultDto>>.SuccessAsync(result, localization.GetResource1("NosearchResult")));
                    }
                    return Ok(await Result<List<HrPayrollFilter2ResultDto>>.SuccessAsync(result, localization.GetResource1("NosearchResult")));
                }
                return Ok(await Result<HrPayrollFilter2ResultDto>.FailAsync(items.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrPayrollFilter2ResultDto>.FailAsync(ex.Message));
            }
        }

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(HrPayrollEditDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(168, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result<object>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));
                if (obj.MsId <= 0)
                    return Ok(await Result<object>.FailAsync("يجب تحديد رقم المسير"));
                if (string.IsNullOrEmpty(obj.MsTitle))
                    return Ok(await Result<object>.FailAsync("يجب تحديد عنوان المسير"));
                if (string.IsNullOrEmpty(obj.DueDate))
                    return Ok(await Result<object>.FailAsync("يجب ادخال  تاريخ الاستحقاق"));
                if (string.IsNullOrEmpty(obj.PaymentDate))
                    return Ok(await Result<object>.FailAsync("يجب ادخال تاريخ الدفع"));



                var update = await hrServiceManager.HrPayrollService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrPayrollEditDto>.FailAsync($"====== Exp in Edit HR HrPayroll  Controller, MESSAGE: {ex.Message}"));
            }
        }

        /// <summary>
        /// هذه الدالة تستخدم لجلب الملاحظات على المسير وذلك عند الانتقال الى واجهة تعديل مسير
        /// </summary>
        /// <param name="Id">رقم المسير</param>
        /// <returns></returns>
        [HttpGet("GetPayrollNotes")]
        public async Task<IActionResult> GetPayrollNotes(long Id)
        {
            try
            {
                var chk = await permission.HasPermission(168, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id <= 0)
                {
                    return Ok(await Result<HrHolidayVw>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var item = await hrServiceManager.HrPayrollNoteService.GetAllVW(x => x.MsId == Id);
                return Ok(item);

            }
            catch (Exception ex)
            {
                return Ok(await Result<HrHolidayVw>.FailAsync($"====== Exp in HR Manual Payroll Controller getById, MESSAGE: {ex.Message}"));
            }
        }
        #endregion
        //كل شاشات ال view ال 3 عرض مدير موارد - عرض مدير مالية - عرض مدير عام بتستخدم هذه الشاشة
        [HttpPost("GetPayrollDataForEdit")]
        public async Task<IActionResult> GetPayrollDataForEdit(HrPayrollFilter2Dto filter)
        {
            List<HrPayrollFilter2ResultDto> result = new List<HrPayrollFilter2ResultDto>();

            var chk = await permission.HasPermission(168, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var hrPayroll = new HrPayrollDto();
                long appId = 0;
                if (filter.MsId != null && filter.MsId != 0)
                {
                    var hrPayrollResult = await hrServiceManager.HrPayrollService.GetOne(x => x.MsId == filter.MsId && x.IsDeleted == false);
                    if (hrPayrollResult != null && hrPayrollResult.Data != null)
                    {
                        hrPayroll = hrPayrollResult.Data;
                    }
                }
                else if (filter.Id != null && filter.Id != 0)
                {
                    appId = filter.Id ?? 0;
                    var hrPayrollResult = await hrServiceManager.HrPayrollService.GetOne(x => x.AppId == appId && x.IsDeleted == false && x.AppId != 0);
                    if (hrPayrollResult != null && hrPayrollResult.Data != null)
                    {
                        hrPayroll = hrPayrollResult.Data;
                    }
                    filter.MsId = hrPayroll.MsId;
                }
                //var BranchesList = session.Branches.Split(',');
                var filesDto = await mainServiceManager.SysFileService.GetAll(x => x.IsDeleted == false && x.PrimaryKey == filter.MsId && x.TableId == 37);
                var hrPayrollNote = await hrServiceManager.HrPayrollNoteService.GetAllVW(x => x.MsId == filter.MsId);

                if (filter.PayrollTypeId != 1)
                {
                    //Payment_Due
                    filter.BranchId = 0;
                    filter.DeptId = 0;
                    filter.Location = 0;
                    filter.SponsorsId = 0;
                    filter.FacilityId = 0;
                    filter.NationalityId = 0;
                    filter.PaymentTypeId = 0;
                    filter.WagesProtection = 0;

                    if (filter.MsId <= 0)
                    {
                        return Ok(await Result<object>.FailAsync("رقم المسير مطلوب"));

                    }
                    //  هنا يجب ارسال رقم المسير والسنة المالية والشهر للحصول على بيانات صحيحة
                    var items = await hrServiceManager.HrPayrollDService.GetAllVW(e =>
                    e.IsDeleted == false &&
                    (filter.MsId == 0 || e.MsId == filter.MsId) &&
                    (filter.DeptId == 0 || e.DeptId == filter.DeptId) &&
                    (filter.Location == 0 || e.Location == filter.Location) &&
                    (filter.BranchId == 0 || e.BranchId == filter.BranchId) &&
                    //(BranchesList == null || BranchesList.Contains(e.BranchId.ToString())) &&
                    (string.IsNullOrEmpty(filter.EmpCode) || e.EmpCode == filter.EmpCode) &&
                    (string.IsNullOrEmpty(filter.EmpName) || e.EmpName == filter.EmpName) &&
                    (filter.FacilityId == 0 || e.FacilityId == filter.FacilityId) &&
                    (filter.SponsorsId == 0 || e.SponsorsId == filter.SponsorsId) &&
                    (filter.NationalityId == 0 || (filter.NationalityId == 1 && e.NationalityId == 1) || (filter.NationalityId == 2 && e.NationalityId != 1)) &&
                    (filter.PaymentTypeId == 0 || e.PaymentTypeId == filter.PaymentTypeId) &&
                    (filter.WagesProtection == 0 || e.WagesProtection == filter.WagesProtection) &&
                    (filter.NationalityId == 0 || (filter.NationalityId == 1 && e.NationalityId == 13) || (filter.NationalityId == 2 && e.NationalityId != 13)) &&
                    (filter.SalaryGroupId == 0 || e.SalaryGroupId == filter.SalaryGroupId) &&
                    (string.IsNullOrEmpty(filter.CostCenterCode) || e.CostCenterCode == filter.CostCenterCode));
                    if (items.Succeeded)
                    {
                        if (items.Data.Count() > 0)
                        {
                            var res = items.Data.ToList();
                            res = res.OrderBy(r => r.EmpCode).ToList();


                            if (res.Any())
                            {
                                foreach (var item in res)
                                {
                                    var newRecord = new HrPayrollFilter2ResultDto
                                    {
                                        EmpCode = item.EmpCode,
                                        Emp_ID = item.EmpCode,
                                        Emp_Name = (session.Language == 1) ? item.EmpName : item.EmpName2,
                                        BraName = (session.Language == 1) ? item.BraName : item.BraName2,
                                        Dep_Name = (session.Language == 1) ? item.DepName : item.DepName2,
                                        Location_Name = (session.Language == 1) ? item.LocationName : item.LocationName2,
                                        Salary = item.Salary,
                                        Allowance = item.Allowance,
                                        H_OverTime = item.HOverTime,
                                        OverTime = item.OverTime,
                                        Commission = item.Commission,
                                        Mandate = item.Mandate,
                                        //Total = item.Salary + item.Allowance + item.OverTime + item.Commission + item.Mandate,
                                        Total = item.BasicSalary + item.AllowanceOrignal,
                                        Attendance = item.Attendance,
                                        Absence = item.Absence,
                                        Delay = item.Delay,
                                        Loan = item.Loan,
                                        Penalties = item.Penalties,
                                        Deduction = item.Deduction,
                                        TotalDeductions = item.Absence + item.Delay + item.Loan + item.Deduction + item.Penalties,
                                        Net = item.Net,
                                        CostCenterCode = item.CostCenterCode,
                                        Bank_ID = item.BankId,
                                        Bank_Name = item.BankName,
                                        BasicSalary = item.BasicSalary,
                                        AllowanceOrignal = item.AllowanceOrignal
                                    };
                                    result.Add(newRecord);
                                }

                                return Ok(await Result<object>.SuccessAsync(new { hrPayroll = hrPayroll, hrPayrolldList = result, hrPayrollNotes = hrPayrollNote.Data, filesDto = filesDto.Data }, ""));

                            }
                            return Ok(await Result<List<HrPayrollFilter2ResultDto>>.SuccessAsync(result, localization.GetResource1("NosearchResult")));
                        }
                        return Ok(await Result<List<HrPayrollFilter2ResultDto>>.SuccessAsync(result, localization.GetResource1("NosearchResult")));
                    }
                }
                else
                {
                    //Payroll
                    filter.MsId ??= 0;
                    filter.FacilityId ??= 0;
                    filter.DeptId ??= 0;
                    filter.BranchId ??= 0;
                    filter.Location ??= 0;
                    filter.SponsorsId ??= 0;
                    filter.WagesProtection ??= 0;
                    filter.NationalityId ??= 0;
                    filter.SalaryGroupId ??= 0;
                    filter.PayrollTypeId ??= 0;
                    filter.PaymentTypeId ??= 0;

                    if (filter.MsId <= 0)
                    {
                        return Ok(await Result<object>.FailAsync("رقم المسير مطلوب"));

                    }
                    //  هنا يجب ارسال رقم المسير والسنة المالية والشهر للحصول على بيانات صحيحة
                    var items = await hrServiceManager.HrPayrollDService.GetAllVW(e =>
                    e.IsDeleted == false &&
                    (filter.MsId == 0 || e.MsId == filter.MsId) &&
                    (filter.DeptId == 0 || e.DeptId == filter.DeptId) &&
                    (filter.Location == 0 || e.Location == filter.Location) &&
                    (filter.BranchId == 0 || e.BranchId == filter.BranchId) &&
                    //(BranchesList == null || BranchesList.Contains(e.BranchId.ToString())) &&
                    (string.IsNullOrEmpty(filter.EmpCode) || e.EmpCode == filter.EmpCode) &&
                    (string.IsNullOrEmpty(filter.EmpName) || e.EmpName == filter.EmpName) &&
                    (filter.FacilityId == 0 || e.FacilityId == filter.FacilityId) &&
                    (filter.SponsorsId == 0 || e.SponsorsId == filter.SponsorsId) &&
                    (filter.NationalityId == 0 || (filter.NationalityId == 1 && e.NationalityId == 1) || (filter.NationalityId == 2 && e.NationalityId != 1)) &&
                    (filter.PaymentTypeId == 0 || e.PaymentTypeId == filter.PaymentTypeId) &&
                    (filter.WagesProtection == 0 || e.WagesProtection == filter.WagesProtection) &&
                    (filter.NationalityId == 0 || (filter.NationalityId == 1 && e.NationalityId == 13) || (filter.NationalityId == 2 && e.NationalityId != 13)) &&
                    (filter.SalaryGroupId == 0 || e.SalaryGroupId == filter.SalaryGroupId) &&
                    //(filter.PayrollTypeId == 0 || e.PayrollTypeId == filter.PayrollTypeId) &&
                    (string.IsNullOrEmpty(filter.CostCenterCode) || e.CostCenterCode == filter.CostCenterCode));
                    if (items.Succeeded)
                    {
                        if (items.Data.Count() > 0)
                        {
                            var res = items.Data.ToList();
                            res = res.OrderBy(r => r.EmpCode).ToList();

                            if (res.Any())
                            {
                                foreach (var item in res)
                                {
                                    decimal GosiDeduction = 0;
                                    var SelectGosi = await hrServiceManager.HrPayrollAllowanceDeductionService.GetOneVW(x => x.Debit > 0 && x.IsDeleted == false && x.AdId == 1 && x.EmpCode == item.EmpCode && x.MsId == filter.MsId);
                                    if (SelectGosi != null && SelectGosi.Data != null)
                                    {
                                        GosiDeduction = SelectGosi.Data.AdValue ?? 0;
                                    }

                                    var newRecord = new HrPayrollFilter2ResultDto
                                    {
                                        EmpCode = item.EmpCode,
                                        Emp_ID = item.EmpCode,
                                        Emp_Name = (session.Language == 1) ? item.EmpName : item.EmpName2,
                                        BraName = (session.Language == 1) ? item.BraName : item.BraName2,
                                        Dep_Name = (session.Language == 1) ? item.DepName : item.DepName2,
                                        Location_Name = (session.Language == 1) ? item.LocationName : item.LocationName2,
                                        Salary = item.Salary,
                                        Allowance = item.Allowance,
                                        H_OverTime = item.HOverTime,
                                        OverTime = item.OverTime,
                                        Commission = item.Commission,
                                        Mandate = item.Mandate,
                                        //Total = item.Salary + item.Allowance + item.OverTime + item.Commission + item.Mandate,
                                        Total = item.BasicSalary + item.AllowanceOrignal,
                                        Attendance = item.Attendance,
                                        Absence = item.Absence,
                                        Delay = item.Delay,
                                        Loan = item.Loan,
                                        Penalties = item.Penalties,
                                        Deduction = item.Deduction,
                                        TotalDeductions = item.Absence + item.Delay + item.Loan + item.Deduction + item.Penalties,
                                        Net = item.Net,
                                        CostCenterCode = item.CostCenterCode,
                                        Bank_ID = item.BankId,
                                        Bank_Name = item.BankName,
                                        BasicSalary = item.BasicSalary,
                                        GosiDeduction = GosiDeduction,
                                        Note = item.Note,
                                        AllowanceOrignal = item.AllowanceOrignal
                                    };
                                    result.Add(newRecord);
                                }

                                return Ok(await Result<object>.SuccessAsync(new { hrPayroll = hrPayroll, hrPayrolldList = result, hrPayrollNotes = hrPayrollNote.Data, filesDto = filesDto.Data }, ""));
                            }
                            return Ok(await Result<List<HrPayrollFilter2ResultDto>>.SuccessAsync(result, localization.GetResource1("NosearchResult")));
                        }
                        return Ok(await Result<List<HrPayrollFilter2ResultDto>>.SuccessAsync(result, localization.GetResource1("NosearchResult")));
                    }
                }

                return Ok(await Result<HrPayrollFilter2ResultDto>.FailAsync());
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrPayrollFilter2ResultDto>.FailAsync(ex.Message));
            }
        }
    }
}