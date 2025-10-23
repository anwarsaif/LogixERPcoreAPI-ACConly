using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Result = Logix.Application.Wrapper.Result;

namespace Logix.MVC.LogixAPIs.HR
{

    //  تقرير توزيع الرواتب حسب الإدارات
    public class HRPayrollByDepController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;


        public HRPayrollByDepController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
        }
        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrPayrollFilterDto filter)
        {
            var chk = await permission.HasPermission(1013, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                //if (filter.FinancelYear == 0 || filter.FinancelYear == null)
                //{
                //    return Ok(await Result<object>.FailAsync(" يجب اختيار السنة المالية"));
                //}
                //if (!string.IsNullOrEmpty(filter.MsMonth))
                //{
                //    if (int.TryParse(filter.MsMonth, out int month))
                //    {
                //        filter.MsMonth = month.ToString("D2");
                //    }
                //}

                //var BranchesList = session.Branches.Split(',');
                //var items = await hrServiceManager.HrPayrollDService.GetPayrollReports(filter,2);
                //if (items.Succeeded)
                //{
                //    if (items.Data.Count() > 0)
                //    {
                //        return Ok(await Result<List<PayrollAccountingEntryResultDto>>.SuccessAsync(items.Data.ToList(), ""));
                //    }
                //    return Ok(await Result<List<PayrollAccountingEntryResultDto>>.SuccessAsync(items.Data.ToList(), localization.GetResource1("NosearchResult")));
                //}
                //return Ok(await Result<PayrollAccountingEntryResultDto>.FailAsync(items.Status.message));
                var items = await hrServiceManager.HrPayrollDService.PayrollByDeptSearch(filter);
                return Ok(items);

			}
            catch (Exception ex)
            {
                return Ok(await Result<PayrollAccountingEntryResultDto>.FailAsync(ex.Message));
            }
        }
    }
}