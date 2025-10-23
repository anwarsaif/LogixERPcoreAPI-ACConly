using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Logix.MVC.LogixAPIs.HR
{
    //  تقرير بالحضور والإنصراف حسب الموظف
    public class HRAttendanceReportForEmpController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IMainServiceManager mainServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;

        public HRAttendanceReportForEmpController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IMainServiceManager mainServiceManager)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.mainServiceManager = mainServiceManager;
        }

        [HttpPost("Search")]
        public async Task<IActionResult> Search(HRAttendanceReport5FilterDto filter)
        {
            var chk = await permission.HasPermission(1467, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                //filter.CalendarType = Convert.ToInt32(session.CalendarType);
                //filter.Language = Convert.ToInt32(session.Language);
                //if (string.IsNullOrEmpty(filter.From) || string.IsNullOrEmpty(filter.To))
                //{
                //    filter.From = null;
                //    filter.To = null;
                //}
                //if (!string.IsNullOrEmpty(filter.EmpCode))
                //{

                //    var getEmpId = await mainServiceManager.InvestEmployeeService.GetOne(x => x.Isdel == false && x.IsDeleted == false && x.EmpId == filter.EmpCode);
                //    if (getEmpId.Data != null)
                //    {
                //        filter.EmpId = getEmpId.Data.Id;
                //    }
                //}


                //var items = await hrServiceManager.HrAttendanceService.getHR_Attendance_Report5_SP(filter);
                //if (items.Succeeded)
                //{
                //    if (items.Data.Count() > 0)
                //    {
                //        return Ok(await Result<List<HRAttendanceReport5Dto>>.SuccessAsync(items.Data.ToList(), ""));
                //    }
                //    return Ok(await Result<List<HRAttendanceReport5Dto>>.SuccessAsync(items.Data.ToList(), localization.GetResource1("NosearchResult")));
                //}
                //return Ok(await Result<HRAttendanceReport5Dto>.FailAsync(items.Status.message));
                var items = await hrServiceManager.HrAttendanceService.Search(filter);
                return Ok(items);

			}
            catch (Exception ex)
            {
                return Ok(await Result<HRAttendanceReport5Dto>.FailAsync(ex.Message));
            }
        }
    }
}