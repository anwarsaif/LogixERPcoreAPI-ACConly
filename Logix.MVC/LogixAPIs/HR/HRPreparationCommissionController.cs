using AutoMapper;
using iText.Commons.Bouncycastle.Asn1.X509;
using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Result = Logix.Application.Wrapper.Result;

namespace Logix.MVC.LogixAPIs.HR
{
    // إعداد مسير عمولات
    public class HRPreparationCommissionController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IMainServiceManager mainServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly ISysConfigurationHelper sysConfigurationHelper;
        private readonly IMapper mapper;


        public HRPreparationCommissionController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, ISysConfigurationHelper sysConfigurationHelper, IMapper mapper, IMainServiceManager mainServiceManager)
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
            var chk = await permission.HasPermission(556, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                filter.Location ??= 0;
                filter.DeptId ??= 0;
                filter.FinancelYear ??= 0;
                if (filter.FinancelYear <= 0 )
                {
                    return Ok(await Result<HrPreparationSalariesFilterDto>.FailAsync(" يجب اختيار السنة المالية"));

                }
                List<HrPreparationSalariesFilterDto> resultList = new List<HrPreparationSalariesFilterDto>();
                var items = await hrServiceManager.HrPreparationSalaryService.GetAllVW(e => e.IsDeleted == false &&
                e.PayrollTypeId == 2 &&
                ( filter.FinancelYear == e.FinancelYear) &&
                (filter.Location == 0 || filter.Location == e.Location) &&
                (filter.DeptId == 0 || filter.DeptId == e.DeptId) &&
                (string.IsNullOrEmpty(filter.EmpCode) || filter.EmpCode == e.EmpCode) &&
                (string.IsNullOrEmpty(filter.EmpName) || filter.EmpName == e.EmpName) &&
                (string.IsNullOrEmpty(filter.MsMonth) || filter.MsMonth == "00" || filter.MsMonth == "0" || Convert.ToInt32(filter.MsMonth) == Convert.ToInt32(e.MsMonth)));
                if (items.Succeeded)
                {
                    if (items.Data.Count() > 0)
                    {
                        var res = items.Data.AsQueryable();
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
                var chk = await permission.HasPermission(556, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (Id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await hrServiceManager.HrPreparationSalaryService.Remove(Id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete HR PreparationCommision Controller, MESSAGE: {ex.Message}"));
            }
        }

        [HttpDelete("DeleteList")]
        public async Task<IActionResult> DeleteList(List<long> Id)
        {
            try
            {
                var chk = await permission.HasPermission(556, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (Id.Count <= 0)
                    return Ok(await Result.FailAsync($"الرجاء تحديد العناصر المراد حذفها"));

                var delete = await hrServiceManager.HrPreparationSalaryService.Remove(Id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete HR PreparationCommision Controller, MESSAGE: {ex.Message}"));
            }
        }
        #endregion


        #region AddPage Business

        [HttpPost("Add")]
        public async Task<ActionResult> Add(HrPreparationSalaryDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(556, PermissionType.Add);
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
                if (string.IsNullOrEmpty(obj.MsDate))
                    return Ok(await Result<HrPreparationSalaryEditDto>.FailAsync($"يرجى ادخال  التاريخ"));

                if (string.IsNullOrEmpty(obj.DaysOfmonth))
                    obj.DaysOfmonth = "0";
                var add = await hrServiceManager.HrPreparationSalaryService.PreparationCommissionAdd(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"====== Exp in Add HR   PreparationCommision Controller  Add Method, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("AddUsingExcel")]
        public async Task<ActionResult> AddUsingExcel(List<HrPreparationSalaryDto> obj)
        {
            try
            {
                var YearTest = "";
                var MonthTest = "";

                var chk = await permission.HasPermission(556, PermissionType.Add);
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
                    if (string.IsNullOrEmpty(item.DaysOfmonth))
                        item.DaysOfmonth = "0";
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
            var chk = await permission.HasPermission(556, PermissionType.Add);
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
                var empData = await hrServiceManager.HrEmployeeCostService.GetEmpDataByEmpId(EmpCode);
                return Ok(empData);


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
                var chk = await permission.HasPermission(556, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id <= 0)
                {
                    return Ok(await Result<HrHolidayVw>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var item = await hrServiceManager.HrPreparationSalaryService.GetOneVW(x => x.Id == Id);
                return Ok(item);

            }
            catch (Exception ex)
            {
                return Ok(await Result<HrPreparationSalaryDto>.FailAsync($"====== Exp in HR PreparationSalary Controller getById, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(HrPreparationCommissionUpdateDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(556, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result<HrPreparationSalaryEditDto>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                if (string.IsNullOrEmpty(obj.EmpCode))
                    return Ok(await Result<HrPreparationSalaryEditDto>.FailAsync($"يرجى ادخال رقم الموظف"));

                if (string.IsNullOrEmpty(obj.MsDate))
                    return Ok(await Result<HrPreparationSalaryEditDto>.FailAsync($"يرجى ادخال  التاريخ"));

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