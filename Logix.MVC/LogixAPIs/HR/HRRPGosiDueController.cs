using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Result = Logix.Application.Wrapper.Result;

namespace Logix.MVC.LogixAPIs.HR
{

    //  تقرير استحقاق التأمينات
    public class HRRPGosiDueController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IAccServiceManager accServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ILocalizationService localization;
        private readonly ICurrentData session;

        public HRRPGosiDueController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ILocalizationService localization, ICurrentData session, IAccServiceManager accServiceManager)
        {
            this.permission = permission;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.session = session;
            this.accServiceManager = accServiceManager;
        }

        #region الصفحة الرئيسية



        [HttpPost("Search")]

        public async Task<IActionResult> Search(HrRPGosiDueFilterDto filter)
        {
            var chk = await permission.HasPermission(1193, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                long? CostCenterId = 0;
                filter.FacilityId ??= 0;
                filter.StatusId ??= 0;
                filter.BranchId ??= 0;
                filter.DeptId ??= 0;
                filter.Location ??= 0;
                var BranchesList = session.Branches.Split(',');

                if (!string.IsNullOrEmpty(filter.CostCenterCode))
                {
                    var getCostCenterByCode = await accServiceManager.AccCostCenterService.GetOne(c => c.CcId, c => c.CostCenterCode == filter.CostCenterCode && c.IsDeleted == false && c.FacilityId == session.FacilityId && c.IsActive == true);
                    if (getCostCenterByCode.Succeeded)
                        CostCenterId = getCostCenterByCode.Data;
                }
                var items = await hrServiceManager.HrGosiEmployeeService.GetAllVW(x => x.IsDeleted == false && x.TDate != null && x.TDate != ""
                && BranchesList.Contains(x.BranchId.ToString())
                && (filter.FacilityId == 0 || filter.FacilityId == x.FacilityId)
                && (filter.DeptId == 0 || filter.DeptId == x.DeptId)
                && (filter.Location == 0 || filter.Location == x.Location)
                && (filter.StatusId == 0 || filter.StatusId == x.StatusId)
                && (filter.BranchId == 0 || filter.BranchId == x.BranchId)
                && (CostCenterId == 0 || CostCenterId == x.CcId)
                && (string.IsNullOrEmpty(filter.EmpCode) || filter.EmpCode == x.EmpCode)
                && (string.IsNullOrEmpty(filter.EmpName) || (x.EmpName != null && x.EmpName.ToLower().Contains(filter.EmpName.ToLower())))
                && (string.IsNullOrEmpty(filter.CostCenterName) || (x.CostCenterName != null && x.CostCenterName.ToLower().Contains(filter.CostCenterName.ToLower())))
                );

                if (!items.Succeeded)
                    return Ok(await Result<object>.FailAsync(items.Status.message));

                if (items.Data.Count() < 1)
                    return Ok(await Result<List<HrRPGosiDueFilterDto>>.SuccessAsync(new List<HrRPGosiDueFilterDto>(), "", 200));

                var res = items.Data.AsQueryable();

                if (!string.IsNullOrEmpty(filter.FromDate) && !string.IsNullOrEmpty(filter.ToDate))
                {
                    res = res.Where(c => DateHelper.StringToDate(c.TDate) >= DateHelper.StringToDate(filter.FromDate) && DateHelper.StringToDate(c.TDate) <= DateHelper.StringToDate(filter.ToDate));
                }
              
                if (res.Count() < 1)
                    return Ok(await Result<List<HrRPGosiDueFilterDto>>.SuccessAsync(new List<HrRPGosiDueFilterDto>(),"",200));


                var result = res
                    .GroupBy(x => new
                    {
                        x.EmpId,
                        x.EmpCode,
                        x.EmpName,
                        x.EmpName2,
                        x.CostCenterCode,
                        x.CostCenterName,
                        x.StatusName,
                        x.GosiTypeName,
                        x.IdNo,
                        x.BasicSalary,
                        x.HousingAllowance,
                        x.OtherAllowance,
                        x.TotalSalary,
                        x.DepName,
                        x.DepName2,
                        x.LocationName,
                        x.LocationName2
                    })
                    .Select(g => new HrRPGosiDueFilterDto
                    {
                        EmpId = g.Key.EmpId,
                        EmpCode = g.Key.EmpCode,
                        EmpName = g.Key.EmpName,
                        EmpName2 = g.Key.EmpName2,
                        CostCenterCode = g.Key.CostCenterCode,
                        CostCenterName = g.Key.CostCenterName,
                        StatusName = g.Key.StatusName,
                        GosiTypeName = g.Key.GosiTypeName,
                        IdNo = g.Key.IdNo,
                        BasicSalary = g.Key.BasicSalary,
                        HousingAllowance = g.Key.HousingAllowance,
                        OtherAllowance = g.Key.OtherAllowance,
                        TotalSalary = g.Key.TotalSalary,
                        DepName = g.Key.DepName,
                        DepName2 = g.Key.DepName2,
                        LocationName = g.Key.LocationName,
                        LocationName2 = g.Key.LocationName2,
                        GosiEmp = g.Sum(x => x.GosiEmp),
                        GosiCompany = g.Sum(x => x.GosiCompany),
                        Total = g.Sum(x => x.GosiEmp + x.GosiCompany)
                    })
                    .ToList();
                return Ok(await Result<List<HrRPGosiDueFilterDto>>.SuccessAsync(result,"",200));
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync(ex.Message));
            }
        }

        #endregion

    }
}
