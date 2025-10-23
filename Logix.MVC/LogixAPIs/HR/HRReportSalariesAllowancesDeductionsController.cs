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

    //  تقرير برواتب الموظفين والبدلات والحسميات
    public class HRReportSalariesAllowancesDeductionsController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IMainServiceManager mainServiceManager;
        public HRReportSalariesAllowancesDeductionsController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IMainServiceManager mainServiceManager)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.mainServiceManager = mainServiceManager;
        }

        #region IndexPage

        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrStaffSalariesAllowancesDeductionsFilterDto filter)
        {
            var chk = await permission.HasPermission(411, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
        //var BranchesList = session.Branches.Split(',');
        //List<HrStaffSalariesAllowancesDeductionsFilterDto> resultList = new List<HrStaffSalariesAllowancesDeductionsFilterDto>();
        //var items = await hrServiceManager.HrEmployeeService.GetAllVW(e => e.IsDeleted == false && e.Isdel == false && BranchesList.Contains(e.BranchId.ToString()) &&
        //(string.IsNullOrEmpty(filter.EmpCode) || filter.EmpCode == e.EmpId) &&
        //(string.IsNullOrEmpty(filter.EmpName) || e.EmpName.ToLower().Contains(filter.EmpName) || e.EmpName2.ToLower().Contains(filter.EmpName)) &&
        //(filter.LocationId == 0 || filter.LocationId == null || filter.LocationId == e.Location) &&
        //(filter.Status == 0 || filter.Status == null || filter.Status == e.StatusId) &&
        //(filter.DepartmentId == 0 || filter.DepartmentId == null || filter.DepartmentId == e.DeptId) &&
        //(filter.NationalityId == 0 || filter.NationalityId == null || filter.NationalityId == e.NationalityId) &&
        //(filter.JobCategory == 0 || filter.JobCategory == null || filter.JobCategory == e.JobCatagoriesId) &&
        //(filter.JobType == null || filter.JobType == 0 || filter.JobType == e.JobType) &&
        //(filter.SalaryGroup == null || filter.SalaryGroup == 0 || filter.SalaryGroup == e.SalaryGroupId) &&
        //(string.IsNullOrEmpty(filter.IdNo) || e.IdNo == filter.IdNo) &&
        //(string.IsNullOrEmpty(filter.PassId) || e.PassportNo == filter.PassId) &&
        //(string.IsNullOrEmpty(filter.EntryNo) || e.EntryNo == filter.EntryNo)
        //);
        //if (items.Succeeded)
        //{
        //    if (items.Data.Count() > 0)
        //    {

        //        var res = items.Data.AsQueryable();

        //        if (filter.BranchId != null && filter.BranchId > 0)
        //        {
        //            res = res.Where(c => c.BranchId != null && c.BranchId == filter.BranchId);
        //        }

        //        if (res.Any())
        //        {
        //            foreach (var item in res)
        //            {
        //                decimal TotalAllowance = 0;
        //                decimal TotalDeduction = 0;
        //                var getTotalAllowance = await hrServiceManager.HrAllowanceDeductionService.GetTotalAllowances(item.Id);
        //                if (getTotalAllowance.Succeeded) TotalAllowance = getTotalAllowance.Data;

        //                var getTotalDeduction = await hrServiceManager.HrAllowanceDeductionService.GetTotalDeduction(item.Id);
        //                if (getTotalDeduction.Succeeded) TotalDeduction = getTotalDeduction.Data;
        //                var newItem = new HrStaffSalariesAllowancesDeductionsFilterDto
        //                {
        //                    EmpCode = item.EmpId,
        //                    EmpName = (session.Language == 1) ? item.EmpName:item.EmpName2,
        //                    NationalityName = (session.Language == 1) ? item.NationalityName:item.NationalityName2,
        //                    BranchName = (session.Language == 1) ? item.BraName:item.BraName2,
        //                    DepartmentName = (session.Language == 1) ? item.DepName:item.DepName2,
        //                    LocationName = (session.Language == 1) ? item.LocationName:item.LocationName2,
        //                    Salary = item.Salary,
        //                    NetSalary = (item.Salary + TotalAllowance - TotalDeduction),
        //                    Deduction = TotalDeduction,
        //                    Allowance = TotalAllowance,
        //                    CatName = (session.Language == 1) ? item.CatName : item.CatName2
        //                };
        //                resultList.Add(newItem);
        //            }
        //            if (resultList.Count > 0) return Ok(await Result<List<HrStaffSalariesAllowancesDeductionsFilterDto>>.SuccessAsync(resultList));
        //            return Ok(await Result<List<HrStaffSalariesAllowancesDeductionsFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));

        //        }
        //        return Ok(await Result<List<HrStaffSalariesAllowancesDeductionsFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));

        //    }
        //    return Ok(await Result<List<HrStaffSalariesAllowancesDeductionsFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));
        //}
        //return Ok(await Result<HrStaffSalariesAllowancesDeductionsFilterDto>.FailAsync(items.Status.message));
        var items = await hrServiceManager.HrEmployeeService.SearchHrStaffSalariesAllowancesDeductions(filter);
        return Ok(items);
      }
            catch (Exception ex)
            {
                return Ok(await Result<HrStaffSalariesAllowancesDeductionsFilterDto>.FailAsync(ex.Message));
            }
        }

        #endregion

    }
}
