using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Result = Logix.Application.Wrapper.Result;

namespace Logix.MVC.LogixAPIs.HR
{

    // مقارنة  الرواتب  حسب الفرع
    public class HRComparePayrollByBranchController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IMainServiceManager mainServiceManager;


        public HRComparePayrollByBranchController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IMainServiceManager mainServiceManager)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.mainServiceManager = mainServiceManager;
        }

        [HttpPost("Compare")]
        public async Task<IActionResult> Compare(HrPayrollCompareFilterDto filter)
        {
            var chk = await permission.HasPermission(1426, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                //if (string.IsNullOrEmpty(filter.PreviousMonth) || filter.PreviousMonth == "0" || filter.PreviousMonth == "00")
                //{
                //    return Ok(await Result<object>.FailAsync($"  يجب اختيار الشهر السابق"));

                //}
                //if (string.IsNullOrEmpty(filter.CurrentMonth) || filter.CurrentMonth == "0" || filter.CurrentMonth == "00")
                //{
                //    return Ok(await Result<object>.FailAsync($"  يجب اختيار الشهر الحالي"));

                //}
                //if (filter.BranchId <= 0)
                //{
                //    return Ok(await Result<object>.FailAsync($"  يجب اختيار الفرع"));

                //}
                //if (filter.FinancialYear <= 0)
                //{
                //    return Ok(await Result<object>.FailAsync($"  يجب اختيار السنة المالية"));

                //}

                //var getData = await hrServiceManager.HrPayrollDService.PayrollCompare(filter, 5);
                //if (getData.Data.Count() > 0)
                //{
                //    return Ok(await Result<object>.SuccessAsync(getData.Data.ToList()));
                //}
                //else
                //{
                //    return Ok(await Result<object>.SuccessAsync(getData.Data.ToList(), "لا توجد نتائج للمقارنة "));
                //}
                var items = await hrServiceManager.HrPayrollDService.SearchComperByBranch(filter);
                return Ok(items);


			}
            catch (Exception ex)
            {
                return Ok(await Result<PayrollAccountingEntryResultDto>.FailAsync(ex.Message));
            }
        }
    }
}