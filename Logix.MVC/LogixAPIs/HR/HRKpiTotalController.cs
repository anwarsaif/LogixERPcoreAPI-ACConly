using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Infrastructure.Repositories;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Result = Logix.Application.Wrapper.Result;

namespace Logix.MVC.LogixAPIs.HR
{

    // تقرير اجمالي بالتقييمات
    public class HRKpiTotalController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IAccServiceManager accServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ILocalizationService localization;
        private readonly ICurrentData session;

        public HRKpiTotalController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ILocalizationService localization, ICurrentData session, IAccServiceManager accServiceManager)
        {
            this.permission = permission;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.session = session;
            this.accServiceManager = accServiceManager;
        }

        #region الصفحة الرئيسية


        [HttpPost("Search")]
        public async Task<IActionResult> Search(HRKpiTotalFilterDto filter, CancellationToken cancellationToken)
        {
            var chk = await permission.HasPermission(933, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                filter.DeptId ??= 0;
                filter.Location ??= 0;
                filter.BranchId ??= 0;
                filter.PerformanceId ??= 0;

                var BranchesList = session.Branches.Split(',');

                var items = await hrServiceManager.HrKpiService.GetAllVW(x => x.IsDeleted == false
                    && (x.TypeId == 1)
                    && (!string.IsNullOrEmpty(x.EvaDate))
                    && (filter.PerformanceId == 0 || x.PerformanceId == filter.PerformanceId)
                    && (filter.DeptId == 0 || x.DeptId == filter.DeptId)
                    && (filter.Location == 0 || x.Location == filter.Location)
                    && (x.StatusId == 2)
                    && (x.FacilityId == session.FacilityId)
                    && (BranchesList.Contains(x.BranchId.ToString()))
                    && (string.IsNullOrEmpty(filter.EmpName) || x.EmpName.ToLower().Contains(filter.EmpName) || x.EmpName2.ToLower().Contains(filter.EmpName))
                    && (string.IsNullOrEmpty(filter.EmpCode) || x.EmpCode == filter.EmpCode)
                );
                if (items.Succeeded)
                {
                    if (items.Data.Count() > 0)
                    {
                        var KPIRes = items.Data.AsQueryable();

                        if (!string.IsNullOrEmpty(filter.FromDate) && !string.IsNullOrEmpty(filter.ToDate))
                        {
                            KPIRes = KPIRes.Where(c => DateHelper.StringToDate(c.EvaDate) >= DateHelper.StringToDate(filter.FromDate) && DateHelper.StringToDate(c.EvaDate) <= DateHelper.StringToDate(filter.ToDate));
                        }
                        if (filter.BranchId > 0)
                        {
                            KPIRes = KPIRes.Where(c => c.BranchId != null && c.BranchId == filter.BranchId);
                        }
                        if (KPIRes.Count() > 0)
                        {

                            var KpiDetailsItems = await hrServiceManager.HrKpiDetaileService.GetAllVW(x => x.IsDeleted == false);
                            var KpiDetailsData = KpiDetailsItems.Data.AsQueryable();
                            var result = KPIRes.Select(p => new
                            {
                                p.EmpCode,
                                p.EmpName,
                                p.EmpName2,
                                DegreeTotal = KpiDetailsData.Where(d => d.KpiId == p.Id).Sum(d => (d.Degree / d.Score * 100) * (d.Weight / 100))
                            }).ToList();
                            return Ok(await Result<object>.SuccessAsync(result.ToList(), ""));

                        }
                        return Ok(await Result<List<HRKpiTotalFilterDto>>.SuccessAsync(new List<HRKpiTotalFilterDto>(), localization.GetResource1("NosearchResult")));

                    }

                    return Ok(await Result<List<HRKpiTotalFilterDto>>.SuccessAsync(new List<HRKpiTotalFilterDto>(), localization.GetResource1("NosearchResult")));

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
