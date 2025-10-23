using Microsoft.AspNetCore.Mvc;
using Logix.MVC.Helpers;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Common;
using Logix.Application.Wrapper;
using Logix.Application.DTOs.HR;

namespace Logix.MVC.LogixAPIs.HR
{
    //   مؤشرات أداء إنهاء الخدمات 
    public class HRLeaveDashboardController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;

        public HRLeaveDashboardController(IHrServiceManager hrServiceManager,
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
        public async Task<IActionResult> Search(HrLeaveDashboardFilterDto filter)
        {
            var chk = await permission.HasPermission(1502, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                filter.DeptId ??= 0;
                filter.Location ??= 0;
                filter.BranchId ??= 0;
                filter.LeaveType ??= 0;

                var BranchesList = session.Branches.Split(',');

                var items = await hrServiceManager.HrLeaveService.GetAllVW(e => e.IsDeleted == false
                    && BranchesList.Contains(e.BranchId.ToString())
                    && (filter.DeptId == 0 || filter.DeptId == e.DeptId)
                    && (filter.Location == 0 || filter.Location == e.Location)
                    && (session.FacilityId == e.FacilityId)
                    && (filter.LeaveType == 0 || filter.LeaveType == e.LeaveType));

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

                        res = res.Where(d => !string.IsNullOrEmpty(d.LeaveDate)
                            && DateHelper.StringToDate(d.LeaveDate) >= fromDate
                            && DateHelper.StringToDate(d.LeaveDate) <= toDate);
                    }

                    if (res.Count() <= 0)
                    {
                        return Ok(await Result<List<HrLeaveDashboardFilterDto>>.SuccessAsync(new List<HrLeaveDashboardFilterDto>(),localization.GetResource1("NosearchResult")));
                    }

                    var leaveTypeGroups = res.GroupBy(d => new { d.LeaveType, d.TypeName })
                        .Select(g => new 
                        {
                            Cnt = g.Count(),
                            LeaveType = g.Key.LeaveType,
                            LeaveTypeName = g.Key.TypeName,
                            Icon = "icon-user",
                            Url = "",
                            Color = g.Key.LeaveType == 1 ? "primary" : (g.Key.LeaveType == 2 ? "red" : "green")
                        }).ToList();

                    var locationGroups = res.GroupBy(d => new { d.LocationName, d.LocationName2 })
                        .Select(g => new
                        {
                            Cnt = g.Count(),
                            LocationName = g.Key.LocationName,
                            LocationName2 = g.Key.LocationName2
                        }).ToList();

                    var branchGroups = res.GroupBy(d => new { d.BraName, d.BraName2 })
                        .Select(g => new
                        {
                            Cnt = g.Count(),
                            BraName = g.Key.BraName,
                            BraName2 = g.Key.BraName2
                        }).ToList();

                    var departmentGroups = res.GroupBy(d => new { d.DepName, d.DepName2 })
                        .Select(g => new
                        {
                            Cnt = g.Count(),
                            DepName = g.Key.DepName,
                            DepName2 = g.Key.DepName2
                        }).ToList();

                    var result = new
                    {
                        LeaveTypeGroups = leaveTypeGroups,
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
