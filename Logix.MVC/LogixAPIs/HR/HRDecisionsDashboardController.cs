using Microsoft.AspNetCore.Mvc;
using Logix.MVC.Helpers;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Common;
using Logix.Application.Wrapper;
using Logix.Application.DTOs.HR;

namespace Logix.MVC.LogixAPIs.HR
{
    // مؤشر أداء القرارات
    public class HRDecisionsDashboardController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;

        public HRDecisionsDashboardController(IHrServiceManager hrServiceManager,
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
        public async Task<IActionResult> Search(HrDecisionsDashboardFilterDto filter)
        {
            var chk = await permission.HasPermission(1503, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                filter.DeptId ??= 0;
                filter.Location ??= 0;
                filter.BranchId ??= 0;
                var BranchesList = session.Branches.Split(',');

                var items = await hrServiceManager.HrDecisionService.GetAllVW(e => !e.IsDeleted
                    && BranchesList.Contains(e.BranchId.ToString())
                    && (filter.DeptId == 0 || filter.DeptId == e.DeptId)
                    && (filter.Location == 0 || filter.Location == e.Location));

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

                        res = res.Where(d => !string.IsNullOrEmpty(d.DecDate)
                            && DateHelper.StringToDate(d.DecDate) >= fromDate
                            && DateHelper.StringToDate(d.DecDate) <= toDate);
                    }

                    if (res.Count() <= 0)
                    {
                        return Ok(await Result<List<object>>.SuccessAsync(new List<object>(), localization.GetResource1("NosearchResult")));
                    }

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

                    var decisionTypeGroups = res.GroupBy(d => new { d.DecType, d.DecTypeName, d.DecTypeName2 })
                        .Select(g => new
                        {
                            Cnt = g.Count(),
                            DecType = g.Key.DecType,
                            DecTypeName = g.Key.DecTypeName,
                            DecTypeName2 = g.Key.DecTypeName2,
                            Icon = "icon-user",
                            Url = "",
                            Color = g.Key.DecType == 1 ? "primary" : (g.Key.DecType == 2 ? "red" : "green")
                        }).ToList();

                    var result = new
                    {
                        LocationGroups = locationGroups,
                        BranchGroups = branchGroups,
                        DepartmentGroups = departmentGroups,
                        DecisionTypeGroups = decisionTypeGroups
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

        //[HttpPost("Search")]
        //public async Task<IActionResult> Search(HrDecisionsDashboardFilterDto filter)
        //{
        //    var chk = await permission.HasPermission(1503, PermissionType.Show);
        //    if (!chk)
        //    {
        //        return Ok(await Result.AccessDenied("AccessDenied"));
        //    }
        //    try
        //    {
        //        filter.DeptId ??= 0;
        //        filter.Location ??= 0;
        //        filter.BranchId ??= 0;
        //        var BranchesList = session.Branches.Split(',');

        //        var items = await hrServiceManager.HrDecisionService.GetAllVW(e => !e.IsDeleted
        //            && BranchesList.Contains(e.BranchId.ToString())
        //            && (filter.DeptId == 0 || filter.DeptId == e.DeptId)
        //            && (filter.Location == 0 || filter.Location == e.Location));

        //        if (items.Succeeded)
        //        {
        //            var res = items.Data.AsQueryable();

        //            if (filter.BranchId > 0)
        //            {
        //                res = res.Where(c => c.BranchId != null && c.BranchId == filter.BranchId);
        //            }
        //            if (!string.IsNullOrEmpty(filter.FromDate) && !string.IsNullOrEmpty(filter.ToDate))
        //            {
        //                var fromDate = DateHelper.StringToDate1(filter.FromDate);
        //                var toDate = DateHelper.StringToDate1(filter.ToDate);

        //                res = res.Where(d => !string.IsNullOrEmpty(d.DecDate)
        //                    && DateHelper.StringToDate1(d.DecDate) >= fromDate
        //                    && DateHelper.StringToDate1(d.DecDate) <= toDate);
        //            }

        //            var result = res.GroupBy(d => new { d.DecType, d.DecTypeName, d.DecTypeName2 })
        //                .Select(g => new HrDecisionsDashboardFilterDto
        //                {
        //                    Cnt = g.Count(),
        //                    DecType = g.Key.DecType,
        //                    DecTypeName = g.Key.DecTypeName,
        //                    DecTypeName2 = g.Key.DecTypeName2,
        //                    Icon = "icon-user",
        //                    Url = "",
        //                    Color = g.Key.DecType == 1 ? "primary" : (g.Key.DecType == 2 ? "red" : "green")
        //                }).ToList();

        //            return Ok(await Result<List<HrDecisionsDashboardFilterDto>>.SuccessAsync(result, result.Count > 0 ? "" : localization.GetResource1("NosearchResult")));
        //        }

        //        return Ok(await Result<object>.FailAsync(items.Status.message));
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(await Result<object>.FailAsync(ex.Message));
        //    }
        //}

        //[HttpPost("SearchByDepartment")]
        //public async Task<IActionResult> SearchByDepartment(HrDecisionsDashboardFilterDto filter)
        //{
        //    var chk = await permission.HasPermission(1503, PermissionType.Show);
        //    if (!chk)
        //    {
        //        return Ok(await Result.AccessDenied("AccessDenied"));
        //    }
        //    try
        //    {
        //        filter.DeptId ??= 0;
        //        filter.Location ??= 0;
        //        filter.BranchId ??= 0;
        //        var BranchesList = session.Branches.Split(',');

        //        var items = await hrServiceManager.HrDecisionService.GetAllVW(e => !e.IsDeleted
        //            && BranchesList.Contains(e.BranchId.ToString())
        //            && (filter.DeptId == 0 || filter.DeptId == e.DeptId)
        //            && (filter.Location == 0 || filter.Location == e.Location));

        //        if (items.Succeeded)
        //        {
        //            var res = items.Data.AsQueryable();

        //            if (filter.BranchId > 0)
        //            {
        //                res = res.Where(c => c.BranchId != null && c.BranchId == filter.BranchId);
        //            }
        //            if (!string.IsNullOrEmpty(filter.FromDate) && !string.IsNullOrEmpty(filter.ToDate))
        //            {
        //                var fromDate = DateHelper.StringToDate1(filter.FromDate);
        //                var toDate = DateHelper.StringToDate1(filter.ToDate);

        //                res = res.Where(d => !string.IsNullOrEmpty(d.DecDate)
        //                    && DateHelper.StringToDate1(d.DecDate) >= fromDate
        //                    && DateHelper.StringToDate1(d.DecDate) <= toDate);
        //            }

        //            var result = res.GroupBy(d => new { d.DepName, d.DepName2 })
        //                .Select(g => new HrDecisionsDashboardFilterDto
        //                {
        //                    Cnt = g.Count(),
        //                    DepName = g.Key.DepName,
        //                    DepName2 = g.Key.DepName2
        //                }).ToList();

        //            return Ok(await Result<List<HrDecisionsDashboardFilterDto>>.SuccessAsync(result, result.Count > 0 ? "" : localization.GetResource1("NosearchResult")));
        //        }

        //        return Ok(await Result<object>.FailAsync(items.Status.message));
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(await Result<object>.FailAsync(ex.Message));
        //    }
        //}
        //[HttpPost("SearchByBranch")]
        //public async Task<IActionResult> SearchByBranch(HrDecisionsDashboardFilterDto filter)
        //{
        //    var chk = await permission.HasPermission(1503, PermissionType.Show);
        //    if (!chk)
        //    {
        //        return Ok(await Result.AccessDenied("AccessDenied"));
        //    }
        //    try
        //    {
        //        filter.DeptId ??= 0;
        //        filter.Location ??= 0;
        //        filter.BranchId ??= 0;
        //        var BranchesList = session.Branches.Split(',');

        //        var items = await hrServiceManager.HrDecisionService.GetAllVW(e => !e.IsDeleted
        //            && BranchesList.Contains(e.BranchId.ToString())
        //            && (filter.DeptId == 0 || filter.DeptId == e.DeptId)
        //            && (filter.Location == 0 || filter.Location == e.Location));

        //        if (items.Succeeded)
        //        {
        //            var res = items.Data.AsQueryable();

        //            if (filter.BranchId > 0)
        //            {
        //                res = res.Where(c => c.BranchId != null && c.BranchId == filter.BranchId);
        //            }
        //            if (!string.IsNullOrEmpty(filter.FromDate) && !string.IsNullOrEmpty(filter.ToDate))
        //            {
        //                var fromDate = DateHelper.StringToDate1(filter.FromDate);
        //                var toDate = DateHelper.StringToDate1(filter.ToDate);

        //                res = res.Where(d => !string.IsNullOrEmpty(d.DecDate)
        //                    && DateHelper.StringToDate1(d.DecDate) >= fromDate
        //                    && DateHelper.StringToDate1(d.DecDate) <= toDate);
        //            }

        //            var result = res.GroupBy(d => new { d.BraName, d.BraName2 })
        //                .Select(g => new HrDecisionsDashboardFilterDto
        //                {
        //                    Cnt = g.Count(),
        //                    BraName = g.Key.BraName,
        //                    BraName2 = g.Key.BraName2
        //                }).ToList();

        //            return Ok(await Result<List<HrDecisionsDashboardFilterDto>>.SuccessAsync(result, result.Count > 0 ? "" : localization.GetResource1("NosearchResult")));
        //        }

        //        return Ok(await Result<object>.FailAsync(items.Status.message));
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(await Result<object>.FailAsync(ex.Message));
        //    }
        //}
        //[HttpPost("SearchByLocation")]
        //public async Task<IActionResult> SearchByLocation(HrDecisionsDashboardFilterDto filter)
        //{
        //    var chk = await permission.HasPermission(1503, PermissionType.Show);
        //    if (!chk)
        //    {
        //        return Ok(await Result.AccessDenied("AccessDenied"));
        //    }
        //    try
        //    {
        //        filter.DeptId ??= 0;
        //        filter.Location ??= 0;
        //        filter.BranchId ??= 0;
        //        var BranchesList = session.Branches.Split(',');

        //        var items = await hrServiceManager.HrDecisionService.GetAllVW(e => !e.IsDeleted
        //            && BranchesList.Contains(e.BranchId.ToString())
        //            && (filter.DeptId == 0 || filter.DeptId == e.DeptId)
        //            && (filter.Location == 0 || filter.Location == e.Location));

        //        if (items.Succeeded)
        //        {
        //            var res = items.Data.AsQueryable();

        //            if (filter.BranchId > 0)
        //            {
        //                res = res.Where(c => c.BranchId != null && c.BranchId == filter.BranchId);
        //            }
        //            if (!string.IsNullOrEmpty(filter.FromDate) && !string.IsNullOrEmpty(filter.ToDate))
        //            {
        //                var fromDate = DateHelper.StringToDate1(filter.FromDate);
        //                var toDate = DateHelper.StringToDate1(filter.ToDate);

        //                res = res.Where(d => !string.IsNullOrEmpty(d.DecDate)
        //                    && DateHelper.StringToDate1(d.DecDate) >= fromDate
        //                    && DateHelper.StringToDate1(d.DecDate) <= toDate);
        //            }

        //            var result = res.GroupBy(d => new { d.LocationName, d.LocationName2 })
        //                .Select(g => new HrDecisionsDashboardFilterDto
        //                {
        //                    Cnt = g.Count(),
        //                    LocationName = g.Key.LocationName,
        //                    LocationName2 = g.Key.LocationName2
        //                }).ToList();

        //            return Ok(await Result<List<HrDecisionsDashboardFilterDto>>.SuccessAsync(result, result.Count > 0 ? "" : localization.GetResource1("NosearchResult")));
        //        }

        //        return Ok(await Result<object>.FailAsync(items.Status.message));
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(await Result<object>.FailAsync(ex.Message));
        //    }
        //}
        //[HttpPost("SearchAll")]
        //public async Task<IActionResult> SearchAll(HrDecisionsDashboardFilterDto filter)
        //{
        //    var chk = await permission.HasPermission(1503, PermissionType.Show);
        //    if (!chk)
        //    {
        //        return Ok(await Result.AccessDenied("AccessDenied"));
        //    }
        //    try
        //    {
        //        filter.DeptId ??= 0;
        //        filter.Location ??= 0;
        //        filter.BranchId ??= 0;
        //        var BranchesList = session.Branches.Split(',');

        //        var items = await hrServiceManager.HrDecisionService.GetAllVW(e => !e.IsDeleted
        //            && BranchesList.Contains(e.BranchId.ToString())
        //            && (filter.DeptId == 0 || filter.DeptId == e.DeptId)
        //            && (filter.Location == 0 || filter.Location == e.Location));

        //        if (items.Succeeded)
        //        {
        //            var res = items.Data.AsQueryable();

        //            if (filter.BranchId > 0)
        //            {
        //                res = res.Where(c => c.BranchId != null && c.BranchId == filter.BranchId);
        //            }
        //            if (!string.IsNullOrEmpty(filter.FromDate) && !string.IsNullOrEmpty(filter.ToDate))
        //            {
        //                var fromDate = DateHelper.StringToDate1(filter.FromDate);
        //                var toDate = DateHelper.StringToDate1(filter.ToDate);

        //                res = res.Where(d => !string.IsNullOrEmpty(d.DecDate)
        //                    && DateHelper.StringToDate1(d.DecDate) >= fromDate
        //                    && DateHelper.StringToDate1(d.DecDate) <= toDate);
        //            }
        //            if(res.Count()<=0) return Ok(await Result<List<HrDecisionsDashboardFilterDto>>.SuccessAsync(localization.GetResource1("NosearchResult")));

        //            var locationGroups = res.GroupBy(d => new { d.LocationName, d.LocationName2 })
        //                .Select(g => new
        //                {
        //                    Cnt = g.Count(),
        //                    LocationName = g.Key.LocationName,
        //                    LocationName2 = g.Key.LocationName2
        //                }).ToList();

        //            var branchGroups = res.GroupBy(d => new { d.BraName, d.BraName2 })
        //                .Select(g => new
        //                {
        //                    Cnt = g.Count(),
        //                    BraName = g.Key.BraName,
        //                    BraName2 = g.Key.BraName2
        //                }).ToList();

        //            var departmentGroups = res.GroupBy(d => new { d.DepName, d.DepName2 })
        //                .Select(g => new
        //                {
        //                    Cnt = g.Count(),
        //                    DepName = g.Key.DepName,
        //                    DepName2 = g.Key.DepName2
        //                }).ToList();

        //            var decisionTypeGroups = res.GroupBy(d => new { d.DecType, d.DecTypeName, d.DecTypeName2 })
        //                .Select(g => new
        //                {
        //                    Cnt = g.Count(),
        //                    DecType = g.Key.DecType,
        //                    DecTypeName = g.Key.DecTypeName,
        //                    DecTypeName2 = g.Key.DecTypeName2,
        //                    Icon = "icon-user",
        //                    Url = "",
        //                    Color = g.Key.DecType == 1 ? "primary" : (g.Key.DecType == 2 ? "red" : "green")
        //                }).ToList();

        //            var result = new
        //            {
        //                LocationGroups = locationGroups,
        //                BranchGroups = branchGroups,
        //                DepartmentGroups = departmentGroups,
        //                DecisionTypeGroups = decisionTypeGroups
        //            };

        //            return Ok(await Result<object>.SuccessAsync(result));
        //        }

        //        return Ok(await Result<object>.FailAsync(items.Status.message));
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(await Result<object>.FailAsync(ex.Message));
        //    }
        //}

    }
}
