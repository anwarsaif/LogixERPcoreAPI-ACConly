using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Result = Logix.Application.Wrapper.Result;

namespace Logix.MVC.LogixAPIs.HR
{
    // تقرير بالقرارات
    public class HRRPDecisionsController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IMainServiceManager mainServiceManager;
        public HRRPDecisionsController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IMainServiceManager mainServiceManager)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.mainServiceManager = mainServiceManager;
        }

        #region IndexPage

        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrRPDecisionsFilterDto filter)
        {
            var chk = await permission.HasPermission(1511, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {

                List<HrRPDecisionsFilterDto> resultList = new List<HrRPDecisionsFilterDto>();
                var items = await hrServiceManager.HrDecisionService.GetAllVW(e => e.IsDeleted == false &&
                (string.IsNullOrEmpty(filter.EmpCode) || filter.EmpCode == e.EmpCode) &&
                (string.IsNullOrEmpty(filter.EmpName) || e.EmpName.ToLower().Contains(filter.EmpName) || e.EmpName2.ToLower().Contains(filter.EmpName)) &&
                (filter.LocationId == 0 || filter.LocationId == null || filter.LocationId == e.Location) &&
                (filter.DepartmentId == 0 || filter.DepartmentId == null || filter.DepartmentId == e.DeptId) &&
                (filter.DecisionCode == 0 || filter.DecisionCode == null || filter.DecisionCode == e.DecCode) &&
                (filter.DecisionTytpe == 0 || filter.DecisionTytpe == null || filter.DecisionTytpe == e.DecType) &&
                (filter.DecisionTytpe == 0 || filter.DecisionTytpe == null || filter.DecisionTytpe == e.DecType)&&
                (filter.BranchId == 0 || filter.BranchId == null || filter.BranchId == e.BranchId)

                );
                if (items.Succeeded)
                {
                    if (items.Data.Count() > 0)
                    {

                        var res = items.Data.AsQueryable();
                        if (!string.IsNullOrEmpty(filter.FromDate) && !string.IsNullOrEmpty(filter.ToDate))
                        {
                            res = res.Where(c => (c.DecDate != null && DateHelper.StringToDate(c.DecDate) >= DateHelper.StringToDate(filter.FromDate) && DateHelper.StringToDate(c.DecDate) <= DateHelper.StringToDate(filter.ToDate)));
                        }
                        if (res.Count() >= 0)
                        {

                            foreach (var item in res)
                            {
                                var newItem = new HrRPDecisionsFilterDto
                                {
                                    EmpCode = item.EmpCode,
                                    EmpName = item.EmpName,
                                    DepartmentName = item.DepName,
                                    LocationName = item.LocationName,
                                    DecisionDate = item.DecDate,
                                    BranchName = item.BraName,
                                    DecisionTytpeName = item.DecTypeName,
                                    DecisionCode=item.DecCode
                                };
                                resultList.Add(newItem);
                            }
                        }
                        if (resultList.Count > 0) return Ok(await Result<List<HrRPDecisionsFilterDto>>.SuccessAsync(resultList));
                        return Ok(await Result<List<HrRPDecisionsFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));

                    }
                    return Ok(await Result<List<HrRPDecisionsFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));

                }
                return Ok(await Result<List<HrRPDecisionsFilterDto>>.FailAsync(items.Status.message.ToString()));

            }
            catch (Exception ex)
            {
                return Ok(await Result<HrRPDecisionsFilterDto>.FailAsync(ex.Message));
            }
        }

        #endregion

    }
}