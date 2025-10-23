using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Logix.MVC.LogixAPIs.HR
{

    // إحصائيات ببيانات الموظفين
    public class HRFormReportDashboardController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IMainServiceManager mainServiceManager;
        private readonly ICurrentData session;
        private readonly IPermissionHelper permission;
        private readonly ILocalizationService localization;

        public HRFormReportDashboardController(IPermissionHelper permission, IHrServiceManager hrServiceManager, ICurrentData session, IMainServiceManager mainServiceManager, ILocalizationService localization)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.mainServiceManager = mainServiceManager;
            this.localization = localization;
        }


        [HttpPost("GetChartsData")]
        public async Task<IActionResult> GetPayrollReports(HrDashboardDto filter)
        {
            var chk = await permission.HasPermission(913, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                filter.DeptId ??= 0;
                filter.LocationId ??= 0;
                filter.BranchId ??= 0;
                var BranchesList = session.Branches.Split(',');


                var GetAllEmployees = await hrServiceManager.HrEmployeeService.GetAllVW(e => e.IsDeleted == false && e.FacilityId == session.FacilityId && BranchesList.Contains(e.BranchId.ToString())
                            && (filter.BranchId == 0 || e.BranchId == filter.BranchId)
                            && (filter.LocationId == 0 || e.Location == filter.LocationId)
                            && (filter.DeptId == 0 || e.DeptId == filter.DeptId)
                            );
                if (GetAllEmployees.Data.Count()<1)
                {
                    return Ok(await Result<List<HrDashboardDto>>.SuccessAsync(new List<HrDashboardDto>(), localization.GetResource1("NosearchResult")));
                }
                var res = GetAllEmployees.Data.AsQueryable();

                var EmployeesByDepartment = res.Where(e => e.StatusId == 1).GroupBy(d => new { d.DepName, d.DepName2 })
                    .Select(g => new { Cnt = g.Count(), Name2 = g.Key.DepName2, Name = g.Key.DepName, }).OrderByDescending(x => x.Cnt).ToList();

                var EmployeesByLocation = res.Where(e => e.StatusId == 1).GroupBy(d => new { d.LocationName, d.LocationName2 })
                    .Select(g => new { Cnt = g.Count(), LocationName = g.Key.LocationName, LocationName2 = g.Key.LocationName2 }).OrderByDescending(x => x.Cnt).ToList();

                var EmployeesByBranch = res.Where(e => e.StatusId == 1).GroupBy(d => new { d.BraName, d.BraName2 })
                    .Select(g => new { Cnt = g.Count(), BraName = g.Key.BraName, BraName2 = g.Key.BraName2 }).OrderByDescending(x => x.Cnt).ToList();

                var EmployeesByStatus = res.GroupBy(d => new { d.StatusName, d.StatusName2 })
                    .Select(g => new
                    { Cnt = g.Count(), StatusName = g.Key.StatusName, StatusName2 = g.Key.StatusName2 }).OrderByDescending(x => x.Cnt).ToList();


                var EmployeesByCat = res.Where(e => e.StatusId == 1).GroupBy(d => new { d.CatName, d.CatName2 })
                    .Select(g => new { Cnt = g.Count(), CatName = g.Key.CatName, CatName2 = g.Key.CatName2 }).OrderByDescending(x => x.Cnt).ToList();


                var EmployeesByNationality = res.Where(e => e.StatusId == 1).GroupBy(d => new { d.NationalityName, d.NationalityName2 })
                    .Select(g => new { Cnt = g.Count(), NationalityName = g.Key.NationalityName, NationalityName2 = g.Key.NationalityName2 }).OrderByDescending(x => x.Cnt).ToList();


                var EmployeesByMaritalStatus = res.Where(e => e.StatusId == 1).GroupBy(d => new { d.MaritalStatusName, d.MaritalStatusName2 })
                    .Select(g => new { Cnt = g.Count(), MaritalStatusName = g.Key.MaritalStatusName, MaritalStatusName2 = g.Key.MaritalStatusName2 }).OrderByDescending(x => x.Cnt).ToList();

                var EmployeesByBank = res.Where(e => e.StatusId == 1).GroupBy(d => new { d.BankName, d.BankName2 }).Select(g => new { Cnt = g.Count(), BankName = g.Key.BankName, BankName2 = g.Key.BankName2 }).OrderByDescending(x => x.Cnt).ToList();
                var result = new
                {
                    EmployeesByDepartment = EmployeesByDepartment,
                    EmployeesByLocation = EmployeesByLocation,
                    EmployeesByBranch = EmployeesByBranch,
                    EmployeesByStatus = EmployeesByStatus,
                    EmployeesByCat = EmployeesByCat,
                    EmployeesByNationality = EmployeesByNationality,
                    EmployeesByMaritalStatus = EmployeesByMaritalStatus,
                    EmployeesByBank = EmployeesByBank,
                };
                return Ok(await Result<object>.SuccessAsync(result));


            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync(ex.Message));
            }
        }


    }
}