using AutoMapper;
using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IRepositories;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using Result = Logix.Application.Wrapper.Result;

namespace Logix.MVC.LogixAPIs.HR
{

    //  إعداد مسير الرواتب
    public class HRPreparationSalariesController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IMainServiceManager mainServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly ISysConfigurationHelper sysConfigurationHelper;
        private readonly IMapper mapper;


        public HRPreparationSalariesController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, ISysConfigurationHelper sysConfigurationHelper, IMapper mapper, IMainServiceManager mainServiceManager)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.sysConfigurationHelper = sysConfigurationHelper;
            this.mapper = mapper;
            this.mainServiceManager = mainServiceManager;
        }

        #region IndexPage


        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrPreparationSalariesFilterDto filter)
        {
            var chk = await permission.HasPermission(274, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (filter.FinancelYear == 0 || filter.FinancelYear == null)
                {
                    return Ok(await Result<HrPreparationSalariesFilterDto>.FailAsync(" يجب اختيار السنة المالية"));

                }
                List<HrPreparationSalariesFilterDto> resultList = new List<HrPreparationSalariesFilterDto>();
                var items = await hrServiceManager.HrPreparationSalaryService.GetAllVW(e => e.IsDeleted == false &&
                e.PayrollTypeId == 1 &&
                (filter.FinancelYear == null || filter.FinancelYear == 0 || filter.FinancelYear == e.FinancelYear) &&
                (filter.Location == null || filter.Location == 0 || filter.Location == e.Location) &&
                (filter.DeptId == null || filter.DeptId == 0 || filter.DeptId == e.DeptId) &&
                (string.IsNullOrEmpty(filter.EmpCode) || filter.EmpCode == e.EmpCode) &&
                (string.IsNullOrEmpty(filter.EmpName) || filter.EmpName == e.EmpName) &&
                (string.IsNullOrEmpty(filter.MsMonth) || filter.MsMonth == "00" || filter.MsMonth == "0" || Convert.ToInt32(filter.MsMonth) == Convert.ToInt32(e.MsMonth)));
                if (items.Succeeded)
                {
                    if (items.Data.Count() > 0)
                    {
                        var res = items.Data.AsQueryable();
                        if (session.FacilityId != 1)
                        {
                            res = res.Where(x => x.FacilityId == session.FacilityId).AsQueryable();
                        }
                        foreach (var item in res)
                        {
                            var newRecord = new HrPreparationSalariesFilterDto
                            {
                                Id = item.Id,
                                EmpCode = item.EmpCode,
                                EmpName = item.EmpName,
                                DepName = item.DepName,
                                LocationName = item.LocationName,
                                FinancelYear = item.FinancelYear,
                                MsMonth = item.MsMonth,
                                NetSalary = item.DueDayWork + item.AllowanceOther + item.ExtraTime + item.DuePrevMonth - item.Absence - item.Delay - item.Loan - item.DeductionOther - item.Penalties,


                            };
                            resultList.Add(newRecord);
                        }
                        if (resultList.Any())
                            return Ok(await Result<List<HrPreparationSalariesFilterDto>>.SuccessAsync(resultList, ""));
                        return Ok(await Result<List<HrPreparationSalariesFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));
                    }
                    return Ok(await Result<List<HrPreparationSalariesFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));
                }
                return Ok(await Result<HrPreparationSalariesFilterDto>.FailAsync(items.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrPreparationSalariesFilterDto>.FailAsync(ex.Message));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long Id)
        {
            try
            {
                var chk = await permission.HasPermission(274, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (Id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await hrServiceManager.HrPreparationSalaryService.Remove(Id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete HR PreparationSalaries Controller, MESSAGE: {ex.Message}"));
            }
        }

        [HttpDelete("DeleteList")]
        public async Task<IActionResult> DeleteList(List<long> Id)
        {
            try
            {
                var chk = await permission.HasPermission(274, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (Id.Count <= 0)
                    return Ok(await Result.FailAsync($"الرجاء تحديد العناصر المراد حذفها"));

                var delete = await hrServiceManager.HrPreparationSalaryService.Remove(Id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete HR PreparationSalaries Controller, MESSAGE: {ex.Message}"));
            }
        }
        #endregion


        #region AddPage Business

        [HttpPost("Add")]
        public async Task<ActionResult> Add(HrPreparationSalaryDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(274, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result<string>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));
                if (obj.FinancelYear <= 0)
                    return Ok(await Result<string>.FailAsync("يجب تحديد السنة المالية "));

                if (string.IsNullOrEmpty(obj.MsMonth))
                    return Ok(await Result<string>.FailAsync("يجب تحديد الشهر  "));
                if (string.IsNullOrEmpty(obj.EmpCode))
                    return Ok(await Result<string>.FailAsync("الرجاء ادخال رقم الموظف    "));
                obj.PayrollTypeId = 1;
                var add = await hrServiceManager.HrPreparationSalaryService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"====== Exp in Add HR   Preparation Controller  Add Method, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("AddUsingExcel")]
        public async Task<ActionResult> AddUsingExcel(List<HrPreparationSalaryDto> obj)
        {
            try
            {
                var YearTest = "";
                var MonthTest = "";

                var chk = await permission.HasPermission(274, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result<string>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));
                foreach (var item in obj)
                {
                    if (item.FinancelYear <= 0)
                    {
                        YearTest += item.EmpCode + ",";
                    }

                }
                foreach (var item in obj)
                {
                    if (string.IsNullOrEmpty(item.MsMonth))
                        MonthTest += item.EmpCode + ",";
                }

                if (!string.IsNullOrEmpty(YearTest))
                {
                    return Ok(await Result<string>.FailAsync($"يجب تحديد السنة المالية للموظفين  :  {YearTest.TrimEnd(',')}"));

                }
                if (!string.IsNullOrEmpty(MonthTest))
                {
                    return Ok(await Result<string>.FailAsync($"يجب تحديد الشهر  للموظفين   {MonthTest.TrimEnd(',')}"));

                }
                foreach (var item in obj)
                {
                    if (string.IsNullOrEmpty(item.EmpCode))
                        return Ok(await Result<string>.FailAsync("الرجاء ادخال رقم الموظف    "));
                }
                var add = await hrServiceManager.HrPreparationSalaryService.AddUsingExcel(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"====== Exp in Add HR   Preparation Salaries Controller  Add Method, MESSAGE: {ex.Message}"));
            }
        }
        [HttpGet("EmpIdChanged")]
        public async Task<IActionResult> EmpIdChanged(string? EmpCode)
        {
            var chk = await permission.HasPermission(274, PermissionType.Add);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (string.IsNullOrEmpty(EmpCode))
            {
                return Ok(await Result.SuccessAsync(""));

            }
            try
            {
                decimal allowanceAmount = 0;
                decimal deductionAmount = 0;
                decimal totalSalary = 0;
                int countDayWork = 30;
                var empData = await hrServiceManager.HrEmployeeService
                    .GetOneVW(x => x.EmpId == EmpCode && x.Isdel == false && x.IsDeleted == false);

                if (empData?.Data != null)
                {
                    var empId = empData.Data.Id;

                    var allowances = await hrServiceManager.HrAllowanceDeductionService
                        .GetAll(x => x.EmpId == empId && x.IsDeleted == false && x.TypeId == 1 && x.FixedOrTemporary == 1);

                    allowanceAmount = allowances?.Data?.Sum(x => x.Amount) ?? 0;

                    var deductions = await hrServiceManager.HrAllowanceDeductionService
                        .GetAll(x => x.EmpId == empId && x.IsDeleted == false && x.TypeId == 2 && x.FixedOrTemporary == 1);

                    deductionAmount = deductions?.Data?.Sum(x => x.Amount) ?? 0;
                    var salary = empData.Data.Salary ?? 0;
                    totalSalary = salary + allowanceAmount - deductionAmount;

                }

                var result = new AddPreparationSalariesDto
                {
                    HrEmployee = empData.Data,
                    allowanceAmount = allowanceAmount,
                    deductionAmount = deductionAmount,
                    countDayWork = countDayWork,
                    totalSalary = totalSalary
                };

                return Ok(await Result<object>.SuccessAsync(result));



            }
            catch (Exception exp)
            {
                return Ok(await Result<object>.FailAsync($"{exp.Message}"));
            }
        }
        #endregion




        #region EditPage


        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long Id)
        {
            try
            {
                var chk = await permission.HasPermission(274, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (Id <= 0)
                {
                    return Ok(await Result<object>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }
                HrPreparationSalariesGetByIdDto result = new HrPreparationSalariesGetByIdDto();
                var item = await hrServiceManager.HrPreparationSalaryService.GetOneVW(x => x.Id == Id);
                if (item.Succeeded && item.Data != null)
                {
                    result.SalaryData = mapper.Map<HrPreparationSalariesDataDto>(item.Data);
                    var getSalaryMethod = await sysConfigurationHelper.GetValue(79, session.FacilityId);
                    if (getSalaryMethod == "2")
                    {
                        var GetSysMonth = await hrServiceManager.InvestMonthService.GetOne(x => x.MonthCode == item.Data.MsMonth);
                        result.SalaryData.DaysOfmonth = GetSysMonth.Data.DaysOfMonth.ToString();
                        result.SalaryData.salaryMethod = 2;
                    }
                    else
                    {
                        result.SalaryData.DaysOfmonth = "30";
                        result.SalaryData.salaryMethod = 1;
                    }
                    var AllowancedeductionItem = await hrServiceManager.HrPsAllowanceVwService.GetAllVW(x => x.PsId == Id && x.IsDeleted == false);
                    var allowance = AllowancedeductionItem.Data.Where(x => x.TypeId == 1).ToList();
                    //result.allowance = allowance;
                    //var deduction = AllowancedeductionItem.Data.Where(x => x.TypeId == 2).ToList();
                    //result.deduction = deduction;

                    return Ok(await Result<object>.SuccessAsync(result));
                }
                return Ok(await Result<object>.FailAsync(item.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"====== Exp in HR preparation Salaries Payroll Controller getById, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetDaysOfMonthByCode")]
        public async Task<IActionResult> GetDaysOfMonthByCode(string monthCode)
        {
            try
            {
                var chk = await permission.HasPermission(274, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (string.IsNullOrEmpty(monthCode))
                {
                    return Ok(await Result<object>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }
                var item = await hrServiceManager.InvestMonthService.GetOne(x => x.MonthCode == monthCode);
                if (item.Succeeded && item.Data != null)
                {
                    return Ok(item);
                }
                return Ok(await Result<object>.FailAsync(item.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"====== Exp in HR preparation Salaries Payroll Controller getById, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(HrPreparationSalaryEditDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(274, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result<HrPreparationSalaryEditDto>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));
                if (obj.Id <= 0)
                    return Ok(await Result<HrPreparationSalaryEditDto>.FailAsync($"رقم التحضير غير صالح "));

                if (string.IsNullOrEmpty(obj.EmpCode))
                    return Ok(await Result<HrPreparationSalaryEditDto>.FailAsync($"رقم الموظف غير صالح "));

                var update = await hrServiceManager.HrPreparationSalaryService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrPreparationSalaryEditDto>.FailAsync($"====== Exp in Hr Preparation Controller getById, MESSAGE: {ex.Message}"));
            }
        }
        #endregion
    }
}