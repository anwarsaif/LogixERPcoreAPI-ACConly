using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Result = Logix.Application.Wrapper.Result;

namespace Logix.MVC.LogixAPIs.HR
{

    //  تقرير التأمينات الاجتماعية
    public class HRRPGosiController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IAccServiceManager accServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ILocalizationService localization;
        private readonly ICurrentData session;

        public HRRPGosiController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ILocalizationService localization, ICurrentData session, IAccServiceManager accServiceManager)
        {
            this.permission = permission;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.session = session;
            this.accServiceManager = accServiceManager;
        }

        #region الصفحة الرئيسية



        [HttpPost("Search")]

        public async Task<IActionResult> Search(HrEmployeeGosiReportFilterDto filter)
        {
            var chk = await permission.HasPermission(924, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                filter.BranchId ??= 0;
                filter.Location ??= 0;
                filter.SalaryGroupId ??= 0;
                if (filter.FacilityId<=0)
                {
                    return Ok(await Result<object>.FailAsync("يجب تحديد الشركه"));
                }
                if (filter.BranchId == 0)
                {
                    filter.BranchIds = session.Branches;
                }
                else
                {
                    filter.BranchIds = "";

                }

                var items = await hrServiceManager.HrGosiService.GetEmployeeGosiReportInf(filter);

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync(ex.Message));
            }
        }

        #endregion

    }
}
