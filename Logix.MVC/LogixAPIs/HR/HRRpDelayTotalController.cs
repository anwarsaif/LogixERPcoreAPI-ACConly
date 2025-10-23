using Microsoft.AspNetCore.Mvc;
using Logix.MVC.Helpers;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Common;
using Logix.Application.Wrapper;
using Logix.Application.DTOs.HR;

namespace Logix.MVC.LogixAPIs.HR
{
    // تقرير تجمعي بالتاخرات خلال فترة
    public class HRRpDelayTotalController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;

        public HRRpDelayTotalController(IHrServiceManager hrServiceManager,
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
        public async Task<IActionResult> Search(HrDelayTotalFilterDto filter)
        {
            var chk = await permission.HasPermission(1630, PermissionType.Show);
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

                var items = await hrServiceManager.HrDelayService.GetAllVW(e => e.IsDeleted == false
                    && BranchesList.Contains(e.BranchId.ToString())
                    && (filter.DeptId == 0 || filter.DeptId == e.DeptId)
                    && (filter.Location == 0 || filter.Location == e.Location)
                    && (string.IsNullOrEmpty(filter.EmpName) || e.EmpName.ToLower().Contains(filter.EmpName.ToLower()))
                    && (string.IsNullOrEmpty(filter.EmpCode) || e.EmpCode == filter.EmpCode)
                );

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

                        res = res.Where(d => !string.IsNullOrEmpty(d.DelayDate)
                            && DateHelper.StringToDate(d.DelayDate) >= fromDate
                            && DateHelper.StringToDate(d.DelayDate) <= toDate);
                    }

                    if (res.Count() <= 0)
                    {
                        return Ok(await Result<List<object>>.SuccessAsync(new List<object>(), localization.GetResource1("NosearchResult")));
                    }

                    var result = res.GroupBy(d => new { d.EmpCode, d.EmpName, d.EmpId })
                        .Select(g => new
                        {
                            g.Key.EmpCode,
                            g.Key.EmpName,
                            g.Key.EmpId,
                            TotalDelay = g.Sum(d => d.DelayTime.HasValue ? d.DelayTime.Value.TotalMinutes : 0),
                        }).ToList()
                        .Select(x => new
                        {
                            x.EmpCode,
                            x.EmpName,
                            Times = $"{(int)(x.TotalDelay / 60):00}:{(int)(x.TotalDelay % 60):00}"
                        }).ToList();

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
