using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.HR
{
    // تقرير بإجمالي الحضور والتأخير والإضافي للموظفين
    public class HRApprovalAbsencesReportController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;

        public HRApprovalAbsencesReportController(IHrServiceManager hrServiceManager, IPermissionHelper permission)
        {
            this.permission = permission;
            this.hrServiceManager = hrServiceManager;
        }

        [HttpPost("Search")]
        public async Task<IActionResult> Search(HRApprovalAbsencesReportFilterDto filter)
        {
            var chk = await permission.HasPermission(1120, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                //if(filter.FromDate == null || filter.ToDate == null)
                //{
                //    return Ok(await Result<object>.FailAsync("من فضلك ادخل تاريخ البداية والنهاية"));
                //}
                var result = await hrServiceManager.HrAbsenceService.HRApprovalAbsencesReport(filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync(ex.Message));
            }
        }

    }
}