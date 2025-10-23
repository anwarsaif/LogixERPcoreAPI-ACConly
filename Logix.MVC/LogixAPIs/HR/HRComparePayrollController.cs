using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Result = Logix.Application.Wrapper.Result;

namespace Logix.MVC.LogixAPIs.HR
{
    //   مقارنة الرواتب
    public class HRComparePayrollController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IMainServiceManager mainServiceManager;


        public HRComparePayrollController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IMainServiceManager mainServiceManager)
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
            var chk = await permission.HasPermission(1219, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (string.IsNullOrEmpty(filter.PreviousMonth) || filter.PreviousMonth == "0" || filter.PreviousMonth == "00")
                {
                    return Ok(await Result<object>.FailAsync($"  يجب اختيار الشهر السابق"));

                }
                if (string.IsNullOrEmpty(filter.CurrentMonth) || filter.CurrentMonth == "0" || filter.CurrentMonth == "00")
                {
                    return Ok(await Result<object>.FailAsync($"  يجب اختيار الشهر الحالي"));

                }

                if (filter.FinancialYear <= 0)
                {
                    return Ok(await Result<object>.FailAsync($"  يجب اختيار السنة المالية"));

                }
                var getData = await hrServiceManager.HrPayrollDService.PayrollCompare(filter, 1);
                if (getData.Data.Count() > 0)
                {
                    return Ok(await Result<object>.SuccessAsync(getData.Data.ToList()));
                }
                else
                {
                    return Ok(await Result<object>.SuccessAsync(getData.Data.ToList(), "لا توجد نتائج للمقارنة "));
                }

            }
            catch (Exception ex)
            {
                return Ok(await Result<PayrollAccountingEntryResultDto>.FailAsync(ex.Message));
            }
        }

        [HttpPost("BINDGRID2")]
        public async Task<IActionResult> BINDGRID2(HrPayrollCompareFilterDto filter)
        {
            var chk = await permission.HasPermission(1219, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (string.IsNullOrEmpty(filter.PreviousMonth) || filter.PreviousMonth == "0" || filter.PreviousMonth == "00")
                {
                    return Ok(await Result<object>.FailAsync($"  يجب اختيار الشهر السابق"));

                }
                if (string.IsNullOrEmpty(filter.CurrentMonth) || filter.CurrentMonth == "0" || filter.CurrentMonth == "00")
                {
                    return Ok(await Result<object>.FailAsync($"  يجب اختيار الشهر الحالي"));

                }

                if (filter.FinancialYear <= 0)
                {
                    return Ok(await Result<object>.FailAsync($"  يجب اختيار السنة المالية"));

                }

                var getData = await hrServiceManager.HrPayrollDService.PayrollCompare(filter, 2);
                if (getData.Data.Count() > 0)
                {
                    return Ok(await Result<object>.SuccessAsync(getData.Data.ToList()));
                }
                else
                {
                    return Ok(await Result<object>.SuccessAsync(getData.Data.ToList(), "لا يوجد موظفين موجودين في الشهر السابق وغير موجودين في الشهر الحالي"));
                }

            }
            catch (Exception ex)
            {
                return Ok(await Result<PayrollAccountingEntryResultDto>.FailAsync(ex.Message));
            }
        }


        [HttpPost("BINDGRID3")]
        public async Task<IActionResult> BINDGRID3(HrPayrollCompareFilterDto filter)
        {
            var chk = await permission.HasPermission(1219, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (string.IsNullOrEmpty(filter.PreviousMonth) || filter.PreviousMonth == "0" || filter.PreviousMonth == "00")
                {
                    return Ok(await Result<object>.FailAsync($"  يجب اختيار الشهر السابق"));

                }
                if (string.IsNullOrEmpty(filter.CurrentMonth) || filter.CurrentMonth == "0" || filter.CurrentMonth == "00")
                {
                    return Ok(await Result<object>.FailAsync($"  يجب اختيار الشهر الحالي"));

                }

                if (filter.FinancialYear <= 0)
                {
                    return Ok(await Result<object>.FailAsync($"  يجب اختيار السنة المالية"));

                }
                var getData = await hrServiceManager.HrPayrollDService.PayrollCompare(filter, 3);
                if (getData.Data.Count() > 0)
                {
                    return Ok(await Result<object>.SuccessAsync(getData.Data.ToList()));
                }
                else
                {
                    return Ok(await Result<object>.SuccessAsync(getData.Data.ToList(), "لا يوجد موظفين موجودين في الشهر الحالي وغير موجودين في الشهر السابق"));
                }

            }
            catch (Exception ex)
            {
                return Ok(await Result<PayrollAccountingEntryResultDto>.FailAsync(ex.Message));
            }
        }

        [HttpPost("BINDGRID4")]
        public async Task<IActionResult> BINDGRID4(HrPayrollCompareFilterDto filter)
        {
            var chk = await permission.HasPermission(1219, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (string.IsNullOrEmpty(filter.PreviousMonth) || filter.PreviousMonth == "0" || filter.PreviousMonth == "00")
                {
                    return Ok(await Result<object>.FailAsync($"  يجب اختيار الشهر السابق"));

                }
                if (string.IsNullOrEmpty(filter.CurrentMonth) || filter.CurrentMonth == "0" || filter.CurrentMonth == "00")
                {
                    return Ok(await Result<object>.FailAsync($"  يجب اختيار الشهر الحالي"));

                }

                if (filter.FinancialYear <= 0)
                {
                    return Ok(await Result<object>.FailAsync($"  يجب اختيار السنة المالية"));

                }
                var getData = await hrServiceManager.HrPayrollDService.PayrollCompare(filter, 4);
                if (getData.Data.Count() > 0)
                {
                    return Ok(await Result<object>.SuccessAsync(getData.Data.ToList()));
                }
                else
                {
                    return Ok(await Result<object>.SuccessAsync(getData.Data.ToList(), "لا يوجد راتب موظفين يختلف  على الشهر  السابق"));
                }

            }
            catch (Exception ex)
            {
                return Ok(await Result<PayrollAccountingEntryResultDto>.FailAsync(ex.Message));
            }
        }

    }
}