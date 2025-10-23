using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Result = Logix.Application.Wrapper.Result;

namespace Logix.MVC.LogixAPIs.HR
{

    //  تقرير بإنهاء الخدمة للموظفين
    public class HRRPEndOfServiceController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        public HRRPEndOfServiceController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
        }

        #region IndexPage

        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrLeaveFilterDto filter)
        {
            var chk = await permission.HasPermission(1512, PermissionType.Show);
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
                    && (string.IsNullOrEmpty(filter.EmpCode) || e.EmpCode == filter.EmpCode)
                    && (string.IsNullOrEmpty(filter.EmpName) || e.EmpName.ToLower().Contains(filter.EmpName.ToLower()))
                    && (filter.DeptId == 0 || filter.DeptId == e.DeptId)
                    && (filter.Location == 0 || filter.Location == e.Location)
                    && (filter.BranchId == 0 || filter.BranchId == e.BranchId)
                    && (session.FacilityId == e.FacilityId)
                    && (filter.LeaveType == 0 || filter.LeaveType == e.LeaveType)
                    && (BranchesList.Contains(e.BranchId.ToString())));

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
                        return Ok(await Result<List<HrLeaveFilterDto>>.SuccessAsync(new List<HrLeaveFilterDto>(),localization.GetResource1("NosearchResult")));
                    }

                    var result = res.Select(
                        e => new
                        {
                            ReferenceNo = e.Id,
                            EmpCode = e.EmpCode,
                            EmpName = session.Language == 1 ? e.EmpName : e.EmpName2,
                            BranchName = session.Language == 1 ? e.BraName : e.BraName2,
                            DeptName = session.Language == 1 ? e.DepName : e.DepName2,
                            LocationName = session.Language == 1 ? e.LocationName : e.LocationName2,
                            EndOfServiceDate = e.LeaveDate,
                            EndOfServiceReason = session.Language == 1 ? e.TypeName : e.TypeName2,
                            ServiceYears = e.WorkYear,
                        }
                    ).ToList();

                    var groupedResult = res.GroupBy(d => new { d.LeaveType, d.TypeName })
                        .Select(g => new
                        {
                            Cnt = g.Count(),
                            LeaveType = g.Key.LeaveType,
                            TypeName = g.Key.TypeName
                        }).ToList();

                    // Add the additional element
                    groupedResult.Add(new
                    {
                        Cnt = result.Count(),
                        LeaveType = (int?)0,
                        TypeName = "إجمالي انهاء الخدمة"
                    });

                    var finalResult = new
                    {
                        LeaveDetails = result,
                        LeaveSummary = groupedResult
                    };

                    return Ok(await Result<object>.SuccessAsync(finalResult));
                }

                return Ok(await Result<object>.FailAsync(items.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync(ex.Message));
            }
        }

        #endregion

    }
}