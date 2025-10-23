using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Result = Logix.Application.Wrapper.Result;

namespace Logix.MVC.LogixAPIs.HR
{
    //  تقرير المخالفات والجزاءت
    public class HRRPDisciplinaryCaseActionController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IMainServiceManager mainServiceManager;
        public HRRPDisciplinaryCaseActionController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IMainServiceManager mainServiceManager)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.mainServiceManager = mainServiceManager;
        }

        #region IndexPage

        [HttpPost("Search")]
        public async Task<IActionResult> Search(HRRPDisciplinaryCaseActionFilterDto filter)
        {
            var chk = await permission.HasPermission(1510, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                filter.LocationId ??= 0;
                filter.DepartmentId ??= 0;
                filter.ActionType ??= 0;
                filter.DisciplinaryCaseID ??= 0;
                var BranchesList = session.Branches.Split(',');

                var items = await hrServiceManager.HrDisciplinaryCaseActionService.GetAllVW(e =>
                    e.IsDeleted == false &&
                    BranchesList.Contains(e.BranchId.ToString()) &&
                    (string.IsNullOrEmpty(filter.EmpCode) || filter.EmpCode == e.EmpCode) &&
                    (string.IsNullOrEmpty(filter.EmpName) ||
                        (e.EmpName != null && e.EmpName.ToLower().Contains(filter.EmpName)) ||
                        (e.EmpName2 != null && e.EmpName2.ToLower().Contains(filter.EmpName))) &&
                    (filter.LocationId == 0 || filter.LocationId == e.Location) &&
                    (filter.DepartmentId == 0 || filter.DepartmentId == e.DeptId) &&
                    (filter.ActionType == 0 || filter.ActionType == e.ActionType) &&
                    (filter.DisciplinaryCaseID == 0 || filter.DisciplinaryCaseID == e.DisciplinaryCaseId) &&
                    (filter.BranchId == 0 || filter.BranchId == e.BranchId)
                );

                if (!items.Succeeded)
                    return Ok(await Result<object>.FailAsync(items.Status.message));

                var res = items.Data.AsQueryable();

                if (!string.IsNullOrEmpty(filter.FromDate) && !string.IsNullOrEmpty(filter.ToDate))
                {
                    res = res.Where(c =>
                        c.DueDate != null &&
                        DateHelper.StringToDate(c.DueDate) >= DateHelper.StringToDate(filter.FromDate) &&
                        DateHelper.StringToDate(c.DueDate) <= DateHelper.StringToDate(filter.ToDate)
                    );
                }
                if (!res.Any())
                {
                    var statistic = new List<object>
                    {
                        new {Cnt = 0,CaseName = "إجمالي المخالفات والجزاءت"}
                    };
                    return Ok(await Result<object>.SuccessAsync(statistic, localization.GetResource1("NosearchResult")));
                }


                var resultList = res.Select(item => new HRRPDisciplinaryCaseActionFilterDto
                {
                    EmpCode = item.EmpCode,
                    EmpName = item.EmpName,
                    DueDate = item.DueDate,
                    CaseName = item.CaseName,
                    ActionName = item.ActionName,
                    NoOfRepeat = item.CountRept,
                    DeductionRate = item.DeductedRate,
                    DeductionAmount = item.DeductedAmount
                }).ToList();

                var groupedResult = res.GroupBy(d => new { d.CaseName, d.DisciplinaryCaseId })
                    .Select(g => new
                    {
                        Cnt = g.Count(),
                        CaseName = g.Key.CaseName
                    }).ToList();

                var statistics = new
                {
                    Cnt = resultList.Count(),
                    CaseName = "إجمالي المخالفات والجزاءت"
                };
                groupedResult.Insert(0, statistics);

                return Ok(await Result<object>.SuccessAsync(new { Data = resultList, Statistics = groupedResult }));
            }
            catch (Exception ex)
            {
                // Consider logging the exception
                return Ok(await Result<HRRPDisciplinaryCaseActionFilterDto>.FailAsync(ex.Message));
            }
        }

        #endregion

    }
}