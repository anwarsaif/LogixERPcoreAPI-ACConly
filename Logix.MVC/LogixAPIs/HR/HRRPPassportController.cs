using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.DTOs.Main;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Result = Logix.Application.Wrapper.Result;

namespace Logix.MVC.LogixAPIs.HR
{

    //  تقرير بانتهاء جوازات سفر الموظفين
    public class HRRPPassportController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IMainServiceManager mainServiceManager;


        public HRRPPassportController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IMainServiceManager mainServiceManager)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.mainServiceManager = mainServiceManager;
        }


        #region IndexPage

        [HttpPost("Search")]
        public async Task<IActionResult> Search(RPPassportFilterDto filter)
        {
            List<RPPassportFilterDto> results = new List<RPPassportFilterDto>();
            var chk = await permission.HasPermission(381, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
              var employees = await hrServiceManager.HrEmployeeService.SearchRPPassport(filter);
                return Ok(employees);
        //var BranchesList = session.Branches.Split(',');
        //var employees = await hrServiceManager.HrEmployeeService.GetAll(e => e.IsDeleted == false && e.StatusId == 1 && e.PassExpireDate != null && e.PassExpireDate != "" && BranchesList.Contains(e.BranchId.ToString()));
        //var filteredEmployees = employees.Data
        //    .Where(e =>
        //        (string.IsNullOrEmpty(filter.EmpName) || e.EmpName.ToLower().Contains(filter.EmpName.ToLower())) &&
        //        (string.IsNullOrEmpty(filter.empCode) || e.EmpId == filter.empCode) &&
        //        (string.IsNullOrEmpty(filter.From) || string.IsNullOrEmpty(filter.To) || (DateHelper.StringToDate(e.PassExpireDate) >= DateHelper.StringToDate(filter.From) && DateHelper.StringToDate(e.PassExpireDate) <= DateHelper.StringToDate(filter.To))) &&
        //        (filter.BranchId == 0 || filter.BranchId == null || e.BranchId == filter.BranchId) &&
        //        (filter.Location == 0 || filter.Location == null || e.Location == filter.Location)

        //    );
        //foreach (var item in filteredEmployees)
        //{
        //    var remainingDays = (DateHelper.StringToDate(item.PassExpireDate)- DateTime.Now).Days;

        //    var employeeDto = new RPPassportFilterDto
        //    {
        //        empCode = item.EmpId,
        //        EmpName = item.EmpName ?? item.EmpName2,
        //        PassExpireDate = item.PassExpireDate,
        //        RemainingDays = remainingDays,
        //    };

        //    results.Add(employeeDto);
        //}
        //if (results.Count > 0) return Ok(await Result<List<RPPassportFilterDto>>.SuccessAsync(results, ""));
        //return Ok(await Result<List<RPPassportFilterDto>>.SuccessAsync(results, localization.GetResource1("NosearchResult")));


      }
      catch (Exception ex)
            {
                return Ok(await Result<RPPassportFilterDto>.FailAsync(ex.Message));
            }
        }

        #endregion

    }
}
