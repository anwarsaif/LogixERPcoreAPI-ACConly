using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.HR
{
    //  سجل الحضور والإنصراف

    public class HRRecordAttendanceController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;

        public HRRecordAttendanceController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
        }

        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrRecordAttendanceFilterDto filter)
        {
            var chk = await permission.HasPermission(1556, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var BranchesList = session.Branches.Split(',');
                var items = await hrServiceManager.HrCheckInOutService.GetAllVW(e => BranchesList.Contains(e.BranchId.ToString()) && e.Checktime != null && e.Checktype != null);
                if (items.Succeeded)
                {
                    if (items.Data.Count() > 0)
                    {

                        var res = items.Data.AsQueryable();
                        if (!string.IsNullOrEmpty(filter.EmpCode))
                        {
                            res = res.Where(c => c.EmpCode != null && c.EmpCode == filter.EmpCode);
                        }
                        if (!string.IsNullOrEmpty(filter.EmpName))
                        {
                            res = res.Where(c => (c.EmpName != null && c.EmpName.ToLower().Contains(filter.EmpName.ToLower())));
                        }
                        if (!string.IsNullOrEmpty(filter.FromDate) && !string.IsNullOrEmpty(filter.ToDate))
                        {
                            res = res.Where(r =>
                            (r.Checktime >= DateHelper.StringToDate(filter.FromDate)) &&
                           (r.Checktime <= DateHelper.StringToDate(filter.ToDate))
                           );
                        }
                        if (!string.IsNullOrEmpty(filter.TimeFrom) && !string.IsNullOrEmpty(filter.TimeTo))
                        {

                            res = res.Where(r => r.Checktime.HasValue &&
                            r.Checktime.Value.TimeOfDay >= TimeSpan.Parse(filter.TimeFrom) &&
                            r.Checktime.Value.TimeOfDay <= TimeSpan.Parse(filter.TimeTo));
                        }
                        if (filter.BranchId != null && filter.BranchId > 0)
                        {
                            res = res.Where(c => c.BranchId != null && c.BranchId.Equals(filter.BranchId));
                        }
                        if (filter.Location != null && filter.Location > 0)
                        {
                            res = res.Where(c => c.Location != null && c.Location.Equals(filter.Location));
                        }
                        if (filter.AttType != null && filter.AttType > 0)
                        {
                            res = res.Where(c => c.Checktype != null && c.Checktype.Equals(filter.AttType));
                        }

                        if (res.Count() > 0)
                        {
                            foreach (var item in res)
                            {
                                item.Checktypename = session.Language == 1 ? item.Checktypename : item.Checktypename2;
                            }
                            return Ok(await Result<List<HrCheckInOutVw>>.SuccessAsync(res.ToList(), ""));

                        }
                        return Ok(await Result<List<HrCheckInOutVw>>.SuccessAsync(res.ToList(), localization.GetResource1("NosearchResult")));
                    }
                    return Ok(await Result<List<HrCheckInOutVw>>.SuccessAsync(items.Data.ToList(), localization.GetResource1("NosearchResult")));
                }
                return Ok(await Result<HrRecordAttendanceFilterDto>.FailAsync(items.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrRecordAttendanceFilterDto>.FailAsync(ex.Message));
            }
        }
    }
}