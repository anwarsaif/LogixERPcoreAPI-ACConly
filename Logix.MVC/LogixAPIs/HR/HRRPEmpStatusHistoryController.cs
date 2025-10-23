using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using Result = Logix.Application.Wrapper.Result;

namespace Logix.MVC.LogixAPIs.HR
{

    //  تقرير بتغيرات حالة الموظف
    public class HRRPEmpStatusHistoryController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IMainServiceManager mainServiceManager;
        public HRRPEmpStatusHistoryController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IMainServiceManager mainServiceManager)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.mainServiceManager = mainServiceManager;
        }

        #region IndexPage

        [HttpPost("Search")]
        public async Task<IActionResult> Search(HRRPEmpStatusHistoryFilterDto filter)
        {
            //List<HRRPEmpStatusHistoryFilterDto> results = new List<HRRPEmpStatusHistoryFilterDto>();
            var chk = await permission.HasPermission(383, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                //var BranchesList = session.Branches.Split(',');

                //var employees = await hrServiceManager.HrEmpStatusHistoryService.GetAllVW(e => e.IsDeleted == false && e.StatusId == 1 && BranchesList.Contains(e.BranchId.ToString()));
                //var filteredEmployees = employees.Data.Where(e =>
                //        (string.IsNullOrEmpty(filter.EmpName) || e.EmpName.ToLower().Contains(filter.EmpName.ToLower())) &&
                //        (string.IsNullOrEmpty(filter.empCode) || e.EmpId == filter.empCode) &&
                //        (filter.StatusId == 0 || filter.StatusId == null || e.StatusId == filter.StatusId)

                //    );
                //foreach (var item in filteredEmployees)
                //{

                //    var employeeDto = new HRRPEmpStatusHistoryFilterDto
                //    {
                //        empCode = item.EmpId,
                //        EmpName = item.EmpName ,
                //        UserName=item.UserFullname,
                //        OldStatus=item.StatusOldName,
                //        NewStatus=item.StatusNewName,
                //        Reason=item.Note,
                //        Tdate=item.CreatedOn.Value.ToString("yyyy/MM/dd",CultureInfo.InvariantCulture)
                //    };

                //    results.Add(employeeDto);
                //}
                //if (results.Count > 0) return Ok(await Result<List<HRRPEmpStatusHistoryFilterDto>>.SuccessAsync(results, ""));
                //return Ok(await Result<List<HRRPEmpStatusHistoryFilterDto>>.SuccessAsync(results, localization.GetResource1("NosearchResult")));
                var employees = await hrServiceManager.HrEmpStatusHistoryService.Search(filter);
				return Ok(employees);


			}
            catch (Exception ex)
            {
                return Ok(await Result<HRRPEmpStatusHistoryFilterDto>.FailAsync(ex.Message));
            }
        }

        #endregion

    }
}