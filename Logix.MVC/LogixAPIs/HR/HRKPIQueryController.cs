using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.Infrastructure.Repositories;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Result = Logix.Application.Wrapper.Result;

namespace Logix.MVC.LogixAPIs.HR
{
    //  استعلام عن تقييم الأداء
    public class HRKPIQueryController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IWFServiceManager wFServiceManager;
        private readonly IMainServiceManager mainServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ILocalizationService localization;
        private readonly ICurrentData session;

        public HRKPIQueryController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ILocalizationService localization, ICurrentData session, IMainServiceManager mainServiceManager, IWFServiceManager wFServiceManager)
        {
            this.permission = permission;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.session = session;
            this.mainServiceManager = mainServiceManager;
            this.wFServiceManager = wFServiceManager;
        }

        #region الصفحة الرئيسية


        [HttpPost("Search")]
        public async Task<IActionResult> Search(HRKpiQueryFilterDto filter)
        {
            var chk = await permission.HasPermission(180, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                filter.ExcludingProbationaryEmployees ??= 0;
                filter.DeptId ??= 0;
                filter.Location ??= 0;
                filter.BranchId ??= 0;
                filter.EvaluationStatus ??= 0;
                filter.PerformanceId ??= 0;
                filter.Status ??= 0;
                if (string.IsNullOrEmpty(filter.EmpCode))
                {
                    filter.EmpCode = null;
                }
                if (string.IsNullOrEmpty(filter.EmpName))
                {
                    filter.EmpName = null;
                }
                if (string.IsNullOrEmpty(filter.FinancialYear))
                {
                    filter.FinancialYear = null;
                }
                var Result = await hrServiceManager.HrKpiService.GetEmployeeKpi(filter);

                return Ok(Result);
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
                var chk = await permission.HasPermission(180, PermissionType.Show);
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
