using Microsoft.AspNetCore.Mvc;
using Logix.MVC.Helpers;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Common;
using Logix.Application.Wrapper;
using Logix.Application.DTOs.HR;

namespace Logix.MVC.LogixAPIs.HR
{
    //  مؤشرات أداء الإنتدابات
    public class HRMandateDashboardController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;

        public HRMandateDashboardController(IHrServiceManager hrServiceManager,
            IPermissionHelper permission,
            ILocalizationService localization,
            ICurrentData session
            )
        {
            this.hrServiceManager = hrServiceManager;
            this.permission = permission;
            this.session = session;
            this.localization = localization;
        }
        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrMandateDashboardFilterDto filter)
        {
            var chk = await permission.HasPermission(1501, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                filter.DeptId ??= 0;
                filter.Location ??= 0;
                filter.BranchId ??= 0;
                filter.TypeId ??= 0;
                filter.JobCatagoryID ??= 0;

                var BranchesList = session.Branches.Split(',');

                var items = await hrServiceManager.HrMandateService.GetAllVW(e => e.IsDeleted == false
                    && BranchesList.Contains(e.BranchId.ToString())
                    && (filter.DeptId == 0 || filter.DeptId == e.DeptId)
                    && (filter.Location == 0 || filter.Location == e.Location)
                    && (filter.JobCatagoryID == 0 || filter.JobCatagoryID == e.JobCatagoriesId)
                    && (filter.TypeId == 0 || filter.TypeId == e.TypeId));

                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();

                    if (filter.BranchId > 0)
                    {
                        res = res.Where(c => c.BranchId != null && c.BranchId == filter.BranchId);
                    }

                    if (!string.IsNullOrEmpty(filter.FromDate) && !string.IsNullOrEmpty(filter.ToDate))
                    {
                        var fromDate = DateHelper.StringToDate(filter.FromDate);
                        var toDate = DateHelper.StringToDate(filter.ToDate);

                        res = res.Where(d => !string.IsNullOrEmpty(d.FromDate)
                            && DateHelper.StringToDate(d.FromDate) >= fromDate
                            && DateHelper.StringToDate(d.FromDate) <= toDate);
                    }

                    if (res.Count() <= 0)
                    {
                        return Ok(await Result<List<object>>.SuccessAsync(new List<object>(), localization.GetResource1("NosearchResult")));
                    }

                    var MandateTypeGroups = res.GroupBy(d => new { d.TypeId, d.TypeName })
                        .Select(g => new
                        {
                            Cnt = g.Count(),
                            LeaveType = g.Key.TypeId,
                            LeaveTypeName = g.Key.TypeName,
                            Icon = "icon-user",
                            Url = "",
                            Color = g.Key.TypeId == 1 ? "primary" : (g.Key.TypeId == 2 ? "red" : "green"),
                            TotalActualExpenses = g.Sum(x => x.ActualExpenses)

                        }).ToList();

                    var locationGroups = res.GroupBy(d => new { d.LocationName, d.LocationName2 })
                        .Select(g => new
                        {
                            Cnt = g.Count(),
                            LocationName = g.Key.LocationName,
                            LocationName2 = g.Key.LocationName2,
                            TotalActualExpenses = g.Sum(x => x.ActualExpenses)

                        }).ToList();

                    var branchGroups = res.GroupBy(d => new { d.BraName, d.BraName2 })
                        .Select(g => new
                        {
                            Cnt = g.Count(),
                            BraName = g.Key.BraName,
                            BraName2 = g.Key.BraName2,
                            TotalActualExpenses=g.Sum(x=>x.ActualExpenses)
                        }).ToList();

                    var departmentGroups = res.GroupBy(d => new { d.DepName, d.DepName2 })
                        .Select(g => new
                        {
                            Cnt = g.Count(),
                            DepName = g.Key.DepName,
                            DepName2 = g.Key.DepName2,
                            TotalActualExpenses = g.Sum(x => x.ActualExpenses)

                        }).ToList();

                    var result = new
                    {
                        MandateTypeGroups = MandateTypeGroups,
                        LocationGroups = locationGroups,
                        BranchGroups = branchGroups,
                        DepartmentGroups = departmentGroups
                    };

                    return Ok(await Result<object>.SuccessAsync(result));
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
