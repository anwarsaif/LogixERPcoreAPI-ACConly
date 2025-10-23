using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Operations;

namespace Logix.MVC.LogixAPIs.HR
{

    //تكاليف الموظفين خلال فترة العقد
    public class HRRepStaffSalariesContractPeriodController : BaseHrApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;

        public HRRepStaffSalariesContractPeriodController(IHrServiceManager hrServiceManager, IMainServiceManager mainServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization)
        {
            this.mainServiceManager = mainServiceManager;
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
        }

        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrEmployeeCostReportFilterDto filter)
        {
            var chk = await permission.HasPermission(1249, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                string EmpCode = "0";
                string Location = "0";
                string Nationality = "0";
                string Department = "0";
                string StatusId = "0";
                string BranchId = "0";
                string? EmpName = "0";
                filter.Location ??= 0;
                filter.Department ??= 0;
                filter.Nationality ??= 0;
                filter.BranchId ??= 0;
                filter.StatusId ??= 0;

                if (!string.IsNullOrEmpty(filter.EmpName))
                    EmpName = filter.EmpName;
                if (!string.IsNullOrEmpty(filter.EmpCode))
                    EmpCode = filter.EmpCode;
                if (filter.Location > 0)
                    Location = filter.Location.Value.ToString();

                if (filter.Department > 0)
                    Department = filter.Department.Value.ToString();

                if (filter.Nationality > 0)
                    Nationality = filter.Nationality.Value.ToString();

                if (filter.StatusId > 0)
                    StatusId = filter.StatusId.Value.ToString();

                if (filter.BranchId <= 0)
                {
                    filter.BranchsId = session.Branches;
                }
                else
                {
                    BranchId = filter.BranchId.Value.ToString();
                    filter.BranchsId = null;

                }
                var items = await hrServiceManager.HrEmployeeCostService.GetRPEmployeeContract(EmpCode, EmpName, Nationality, Department, Location, session.Language, BranchId, filter.BranchsId, StatusId);
                if (items.Succeeded)
                {
                    if (items.Data.Any())
                        return Ok(await Result<IEnumerable<dynamic>>.SuccessAsync(items.Data));
                    return Ok(await Result<IEnumerable<dynamic>>.SuccessAsync(items.Data, localization.GetResource1("NosearchResult")));


                }

                return Ok(await Result<object>.FailAsync(items.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync(ex.Message));
            }
        }
    }
}