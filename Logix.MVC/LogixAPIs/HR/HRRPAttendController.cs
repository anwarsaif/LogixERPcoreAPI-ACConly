using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Result = Logix.Application.Wrapper.Result;

namespace Logix.MVC.LogixAPIs.HR
{

    //  تقرير بالموظفين المستبعدين من التحضير
    public class HRRPAttendController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IMainServiceManager mainServiceManager;
        public HRRPAttendController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IMainServiceManager mainServiceManager)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.mainServiceManager = mainServiceManager;
        }

        #region IndexPage

        [HttpPost("Search")]
        public async Task<IActionResult> Search(RPAttendFilterDto filter)
        {
            //List<RPAttendFilterDto> results = new List<RPAttendFilterDto>();
            var chk = await permission.HasPermission(385, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
        //      var BranchesList = session.Branches.Split(',');

        //      var employees = await hrServiceManager.HrEmployeeService.GetAllVW(e => e.IsDeleted == false && e.Isdel == false && e.StatusId == 1&&e.ExcludeAttend==true && e.FacilityId == session.FacilityId );
        //      var filteredEmployees = employees.Data
        //          .Where(e =>
        //              (string.IsNullOrEmpty(filter.EmpName) || e.EmpName.ToLower().Contains(filter.EmpName.ToLower())) &&
        //              (string.IsNullOrEmpty(filter.empCode) || e.EmpId == filter.empCode) 
        //);
        //      foreach (var item in filteredEmployees)
        //      {

        //          var employeeDto = new RPAttendFilterDto
        //          {
        //              empCode = item.EmpId,
        //              EmpName = item.EmpName ?? item.EmpName2,
        //              Id = item.Id
        //          };

        //          results.Add(employeeDto);
        //      }
        //      if (results.Count > 0) return Ok(await Result<List<RPAttendFilterDto>>.SuccessAsync(results, ""));
        //      return Ok(await Result<List<RPAttendFilterDto>>.SuccessAsync(results, localization.GetResource1("NosearchResult")));
        var employees = await hrServiceManager.HrEmployeeService.SearchRPAttend(filter);
                return Ok(employees);

      }
            catch (Exception ex)
            {
                return Ok(await Result<RPAttendFilterDto>.FailAsync(ex.Message));
            }
        }


        #endregion

    }
}
