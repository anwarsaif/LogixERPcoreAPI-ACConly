using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Result = Logix.Application.Wrapper.Result;

namespace Logix.MVC.LogixAPIs.HR
{
    // تقرير بالموظفين حسب البنك
    public class HRRPBankController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IMainServiceManager mainServiceManager;
        public HRRPBankController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IMainServiceManager mainServiceManager)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.mainServiceManager = mainServiceManager;
        }

        #region IndexPage

        [HttpPost("Search")]
        public async Task<IActionResult> Search(RPBankFilterDto filter)
        {
            //List<RPBankFilterDto> results = new List<RPBankFilterDto>();
            var chk = await permission.HasPermission(386, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
        var employees = await hrServiceManager.HrEmployeeService.SearchRPBank(filter);
        return Ok(employees);
        //      var BranchesList = session.Branches.Split(',');

        //      var employees = await hrServiceManager.HrEmployeeService.GetAllVW(e => e.IsDeleted == false &&e.StatusId==1&& e.Isdel == false && BranchesList.Contains(e.BranchId.ToString()));
        //      var filteredEmployees = employees.Data
        //          .Where(e =>
        //              (string.IsNullOrEmpty(filter.EmpName) || e.EmpName.ToLower().Contains(filter.EmpName.ToLower())) &&
        //              (string.IsNullOrEmpty(filter.empCode) || e.EmpId == filter.empCode) &&
        //              (filter.Bank == null || filter.Bank == 0 || filter.Bank == e.BankId) &&
        //              (filter.Branch == null || filter.Branch == 0 || filter.Branch == e.BranchId)
        //);
        //      foreach (var item in filteredEmployees)
        //      {

        //          var employeeDto = new RPBankFilterDto
        //          {
        //              empCode = item.EmpId,
        //              EmpName = item.EmpName ?? item.EmpName2,
        //              IBan = item.Iban,
        //              BankName = item.BankName
        //          };

        //          results.Add(employeeDto);
        //      }
        //      if (results.Count > 0) return Ok(await Result<List<RPBankFilterDto>>.SuccessAsync(results, ""));
        //      return Ok(await Result<List<RPBankFilterDto>>.SuccessAsync(results, localization.GetResource1("NosearchResult")));


      }
            catch (Exception ex)
            {
                return Ok(await Result<RPBankFilterDto>.FailAsync(ex.Message));
            }
        }


        #endregion

    }
}
