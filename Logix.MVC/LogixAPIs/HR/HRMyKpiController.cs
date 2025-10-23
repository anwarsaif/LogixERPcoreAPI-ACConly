using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Result = Logix.Application.Wrapper.Result;

namespace Logix.MVC.LogixAPIs.HR
{

    // تقييماتي
    public class HRMyKpiController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IAccServiceManager accServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ILocalizationService localization;
        private readonly ICurrentData session;

        public HRMyKpiController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ILocalizationService localization, ICurrentData session, IAccServiceManager accServiceManager)
        {
            this.permission = permission;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.session = session;
            this.accServiceManager = accServiceManager;
        }

        #region الصفحة الرئيسية


        [HttpPost("Search")]
        public async Task<IActionResult> Search()
        {
            var chk = await permission.HasPermission(338, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var items = await hrServiceManager.HrKpiService.GetAllVW(x => x.IsDeleted == false && x.StatusId == 2 && (x.EmpId == session.EmpId));
                if (items.Succeeded)
                {
                    if (items.Data.Count() > 0)
                    {
                        var KPIRes = items.Data.AsQueryable();

                        if (KPIRes.Count() > 0)
                        {
                            var KpiDetailsItems = await hrServiceManager.HrKpiDetaileService.GetAllVW(x => x.IsDeleted == false);
                            var KpiDetailsData = KpiDetailsItems.Data.AsQueryable();

                            var result = KPIRes.Select(p => new
                            {
                                p.EmpCode,
                                p.EmpName,
                                p.EmpName2,
                                p.EvaDate,
                                p.Id,
                                DegreeTotal = (decimal?)KpiDetailsData
                                    .Where(d => d.KpiId == p.Id)
                                    .GroupBy(d => d.KpiId)
                                    .Select(g => new
                                    {
                                        TotalDegree = g.Sum(d => d.Degree),
                                        TotalScore = g.Sum(d => d.Score)
                                    })
                                    .Select(t => t.TotalScore != 0 ? (t.TotalDegree / t.TotalScore) * 100 : (decimal?)null)
                                    .FirstOrDefault()
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
        #region تفاصيل تقييم الأداء للموظف

        [HttpGet("KPIView")]
        public async Task<IActionResult> KPIView(long Id)
        {
            try
            {
                var chk = await permission.HasPermission(338, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var KpiItem = await hrServiceManager.HrKpiService.GetOneVW(x => x.Id == Id && x.IsDeleted == false);
                if (KpiItem.Succeeded)
                {
                    if (KpiItem.Data == null) return Ok(await Result.FailAsync($"التقييم غير موجود"));

                    var KpiDetailsItems1 = await hrServiceManager.HrKpiDetaileService.GetAllVW(x => x.KpiId == Id && x.TypeId == 1 && x.IsDeleted == false);
                    var KpiDetailsItems2 = await hrServiceManager.HrKpiDetaileService.GetAllVW(x => x.KpiId == Id && x.TypeId == 2 && x.IsDeleted == false);
                    var PerformanceItems = await hrServiceManager.HrPerformanceService.GetAllVW(x => x.Id == KpiItem.Data.PerformanceId && x.IsDeleted == false);


                    return Ok(await Result<object>.SuccessAsync(new { kPIData = KpiItem.Data, KpiDetailsItems1 = KpiDetailsItems1.Data, KpiDetailsItems2 = KpiDetailsItems2.Data, PerformanceItems = PerformanceItems.Data }));
                }
                return Ok(await Result<HrLeaveVw>.FailAsync(KpiItem.Status.message));

            }
            catch (Exception ex)
            {
                return Ok(await Result<HrLeaveVw>.FailAsync($"====== Exp in HRKPIQueryController  KPIView, MESSAGE: {ex.Message}"));
            }
        }
        #endregion

    }
}
