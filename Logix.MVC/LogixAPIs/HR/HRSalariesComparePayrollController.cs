using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Result = Logix.Application.Wrapper.Result;

namespace Logix.MVC.LogixAPIs.HR
{
    // مقارنة مسير رواتب في شاشة اعداد مسير الرواتب
    public class HRSalariesComparePayrollController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IMainServiceManager mainServiceManager;


        public HRSalariesComparePayrollController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IMainServiceManager mainServiceManager)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.mainServiceManager = mainServiceManager;
        }
        [HttpPost("BINDGRID1")]
        public async Task<IActionResult> BINDGRID1(HrPayrollCompareFilterDto filter)
        {
            try
            {
                //var chk = await permission.HasPermission(1219, PermissionType.Show);
                //if (!chk)
                //{
                //    return Ok(await Result.AccessDenied("AccessDenied"));
                //}
                if (string.IsNullOrEmpty(filter.PreviousMonth) || filter.PreviousMonth == "0" || filter.PreviousMonth == "00")
                {
                    return Ok(await Result<object>.FailAsync("يجب اختيار الشهر السابق"));
                }

                if (string.IsNullOrEmpty(filter.CurrentMonth) || filter.CurrentMonth == "0" || filter.CurrentMonth == "00")
                {
                    return Ok(await Result<object>.FailAsync("يجب اختيار الشهر الحالي"));
                }

                if (filter.FinancialYear <= 0)
                {
                    return Ok(await Result<object>.FailAsync("يجب اختيار السنة المالية"));
                }

                var financelYear = Convert.ToInt32(filter.FinancialYear);
                var currentMonth = filter.CurrentMonth;
                var previousMonth = filter.PreviousMonth;

                var results = await hrServiceManager.HrPreparationSalaryService
                    .GetAllVW(e => (e.MsMonth == currentMonth || e.MsMonth == previousMonth) && e.IsDeleted == false && e.FinancelYear == financelYear);

                if (results == null || !results.Data.Any())
                {
                    return Ok(await Result<object>.SuccessAsync(new List<object>(), "لا توجد نتائج للمقارنة "));
                }

                var groupedResults = results.Data
                    .GroupBy(e => new { e.DeptId, e.Location, e.DepName, e.LocationName, e.FinancelYear })
                    .Select(g => new
                    {
                        DepName = g.Key.DepName,
                        LocationName = g.Key.LocationName,
                        FinancelYear = g.Key.FinancelYear,
                        PervMonth = g.Count(e => e.MsMonth == previousMonth),
                        CurMonth = g.Count(e => e.MsMonth == currentMonth),
                        Difference = g.Count(e => e.MsMonth == previousMonth) - g.Count(e => e.MsMonth == currentMonth)
                    })
                    .ToList();

                if (!groupedResults.Any())
                {
                    return Ok(await Result<object>.SuccessAsync(new List<object>(), "لا توجد نتائج للمقارنة "));
                }

                return Ok(await Result<object>.SuccessAsync(groupedResults, ""));
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync(ex.Message));
            }
        }

        [HttpPost("BINDGRID2")]
        public async Task<IActionResult> BINDGRID2(HrPayrollCompareFilterDto filter)
        {

            try
            {
                //var chk = await permission.HasPermission(1219, PermissionType.Show);
                //if (!chk)
                //{
                //    return Ok(await Result.AccessDenied("AccessDenied"));
                //}
                if (string.IsNullOrEmpty(filter.PreviousMonth) || filter.PreviousMonth == "0" || filter.PreviousMonth == "00")
                {
                    return Ok(await Result<object>.FailAsync($"يجب اختيار الشهر السابق"));
                }
                if (string.IsNullOrEmpty(filter.CurrentMonth) || filter.CurrentMonth == "0" || filter.CurrentMonth == "00")
                {
                    return Ok(await Result<object>.FailAsync($"يجب اختيار الشهر الحالي"));
                }
                if (filter.FinancialYear <= 0)
                {
                    return Ok(await Result<object>.FailAsync($"يجب اختيار السنة المالية"));
                }

                var financelYear = Convert.ToInt32(filter.FinancialYear);

                var hrPreparationSalariesVW = await hrServiceManager.HrPreparationSalaryService
                    .GetAllVW(e => e.MsMonth == filter.PreviousMonth && e.FinancelYear == financelYear && e.IsDeleted == false);

                if (hrPreparationSalariesVW == null || hrPreparationSalariesVW.Data == null || !hrPreparationSalariesVW.Data.Any())
                {
                    return Ok(await Result<object>.SuccessAsync(new List<object>(), "لا يوجد موظفين موجودين في الشهر السابق وغير موجودين في الشهر الحالي"));
                }

                var currentMonthEmpIds = await hrServiceManager.HrPreparationSalaryService
                    .GetAll(e => e.MsMonth == filter.CurrentMonth && e.FinancelYear == financelYear && e.IsDeleted == false);

                var currentMonthIdsList = currentMonthEmpIds.Data.Select(e => e.EmpId).ToList();

                var result = hrPreparationSalariesVW.Data
                    .Where(e => !currentMonthIdsList.Contains(e.EmpId))
                    .Select(e => new
                    {
                        e.EmpId,
                        e.DepName,
                        e.LocationName,
                        e.EmpCode,
                        e.EmpName,
                        e.FinancelYear,
                        e.MsMonth
                    })
                    .ToList();

                if (result.Count() <= 0)
                {
                    return Ok(await Result<object>.SuccessAsync(new List<object>(), "لا يوجد موظفين موجودين في الشهر السابق وغير موجودين في الشهر الحالي"));
                }

                return Ok(await Result<object>.SuccessAsync(result, ""));
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync(ex.Message));
            }
        }

        [HttpPost("BINDGRID3")]
        public async Task<IActionResult> BINDGRID3(HrPayrollCompareFilterDto filter)
        {
            try
            {
                //var chk = await permission.HasPermission(1219, PermissionType.Show);
                //if (!chk)
                //{
                //    return Ok(await Result.AccessDenied("AccessDenied"));
                //}

                if (string.IsNullOrEmpty(filter.PreviousMonth) || filter.PreviousMonth == "0" || filter.PreviousMonth == "00")
                {
                    return Ok(await Result<object>.FailAsync($"يجب اختيار الشهر السابق"));
                }
                if (string.IsNullOrEmpty(filter.CurrentMonth) || filter.CurrentMonth == "0" || filter.CurrentMonth == "00")
                {
                    return Ok(await Result<object>.FailAsync($"يجب اختيار الشهر الحالي"));
                }
                if (filter.FinancialYear <= 0)
                {
                    return Ok(await Result<object>.FailAsync($"يجب اختيار السنة المالية"));
                }

                var financelYear = Convert.ToInt32(filter.FinancialYear);

                var hrPreparationSalariesVW = await hrServiceManager.HrPreparationSalaryService
                    .GetAllVW(e => e.MsMonth == filter.CurrentMonth && e.FinancelYear == financelYear && e.IsDeleted == false);

                if (hrPreparationSalariesVW == null || hrPreparationSalariesVW.Data == null || !hrPreparationSalariesVW.Data.Any())
                {
                    return Ok(await Result<object>.SuccessAsync(new List<object>(), "لا يوجد موظفين موجودين في الشهر الحالي وغير موجودين في الشهر السابق"));
                }

                var previousMonthEmpIds = await hrServiceManager.HrPreparationSalaryService
                    .GetAll(e => e.MsMonth == filter.PreviousMonth && e.FinancelYear == financelYear && e.IsDeleted == false);

                var previousMonthIdsList = previousMonthEmpIds.Data.Select(e => e.EmpId).ToList();

                var result = hrPreparationSalariesVW.Data
                    .Where(e => !previousMonthIdsList.Contains(e.EmpId))
                    .Select(e => new
                    {
                        e.EmpId,
                        e.DepName,
                        e.LocationName,
                        e.EmpCode,
                        e.EmpName,
                        e.FinancelYear,
                        e.MsMonth
                    })
                    .ToList();

                if (result.Count() <= 0)
                {
                    return Ok(await Result<object>.SuccessAsync(new List<object>(), "لا يوجد موظفين موجودين في الشهر الحالي وغير موجودين في الشهر السابق"));
                }

                return Ok(await Result<object>.SuccessAsync(result, ""));
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync(ex.Message));
            }
        }


    }
}