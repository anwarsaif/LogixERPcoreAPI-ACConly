using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Result = Logix.Application.Wrapper.Result;

namespace Logix.MVC.LogixAPIs.HR
{
    //   تقرير تقييمات الأداء
    public class HRRepKPIController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IMainServiceManager mainServiceManager;
        public HRRepKPIController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IMainServiceManager mainServiceManager)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.mainServiceManager = mainServiceManager;
        }

        #region IndexPage

        [HttpPost("Search")]
        public async Task<IActionResult> Search(HRRepKPIFilterDto filter)
        {
            var chk = await permission.HasPermission(1684, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {

				//var BranchesList = session.Branches.Split(',');
				//List<HRRepKPIFilterDto> resultList = new List<HRRepKPIFilterDto>();
				//var items = await hrServiceManager.HrKpiService.GetAllVW(e => e.IsDeleted == false && BranchesList.Contains(e.BranchId.ToString()) &&
				//(string.IsNullOrEmpty(filter.EmpCode) || filter.EmpCode == e.EmpCode) &&
				//(string.IsNullOrEmpty(filter.Achievements) || e.Achievements.Contains(filter.Achievements)) &&
				//(string.IsNullOrEmpty(filter.Recommendations) || e.Recommendations.Contains(filter.Recommendations)) &&
				//(string.IsNullOrEmpty(filter.SuggestedTraining) || e.SuggestedTraining.Contains(filter.SuggestedTraining)) &&
				//(string.IsNullOrEmpty(filter.StrengthsPoints) || e.StrengthsPoints.Contains(filter.StrengthsPoints)) &&
				//(string.IsNullOrEmpty(filter.WeaknessesPoints) || e.WeaknessesPoints.Contains(filter.WeaknessesPoints)) &&
				//(string.IsNullOrEmpty(filter.EmpName) || e.EmpName.ToLower().Contains(filter.EmpName) || e.EmpName2.ToLower().Contains(filter.EmpName)) &&
				//(filter.LocationId == 0 || filter.LocationId == null || filter.LocationId == e.Location) &&
				//(filter.Status == 0 || filter.Status == null || filter.Status == e.StatusId) &&
				//(filter.Type == 0 || filter.Type == null || filter.Type == e.TypeId) &&
				//(filter.DepartmentId == 0 || filter.DepartmentId == null || filter.DepartmentId == e.DeptId)
				//);
				//if (items.Succeeded)
				//{
				//    if (items.Data.Count() > 0)
				//    {

				//        var res = items.Data.AsQueryable();

				//        if (filter.BranchId != null && filter.BranchId > 0)
				//        {
				//            res = res.Where(c => c.BranchId != null && c.BranchId == filter.BranchId);
				//        }

				//        if (filter.Month != null && filter.Month > 0)
				//        {
				//            res = res.Where(c => c.EvaDate != null && Convert.ToInt32(c.EvaDate.Substring(5, 2)) == filter.Month);
				//        }
				//        if (res.Any())
				//        {
				//            var getKPIDetailes = await hrServiceManager.HrKpiDetaileService.GetAllVW(x => x.IsDeleted == false);
				//            foreach (var item in res)
				//            {
				//                var getKpiForItem = getKPIDetailes.Data.Where(x => x.KpiId == item.Id);
				//                var sumDegrees = getKpiForItem.Sum(d => d.Degree);
				//                var sumScores = getKpiForItem.Sum(d => d.Score);
				//                decimal? TotalDegree = 0;
				//                if (sumScores != 0)
				//                {
				//                    TotalDegree = sumDegrees / sumScores * 100;
				//                }

				//                var newItem = new HRRepKPIFilterDto
				//                {
				//                    EmpCode = item.EmpCode,
				//                    EmpName = item.EmpName,
				//                    DegreeTotal = TotalDegree,
				//                    TemName = item.TemName,
				//                    Achievements = item.Achievements,
				//                    EvaDate = item.EvaDate,
				//                    Recommendations = item.Recommendations,
				//                    SuggestedTraining = item.SuggestedTraining,
				//                    StrengthsPoints = item.StrengthsPoints,
				//                    WeaknessesPoints = item.WeaknessesPoints,
				//                };
				//                resultList.Add(newItem);
				//            }
				//            if (resultList.Count > 0) return Ok(await Result<List<HRRepKPIFilterDto>>.SuccessAsync(resultList));
				//            return Ok(await Result<List<HRRepKPIFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));

				//        }
				//        return Ok(await Result<List<HRRepKPIFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));

				//    }
				//    return Ok(await Result<List<HRRepKPIFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));
				//}
				//return Ok(await Result<HRRepKPIFilterDto>.FailAsync(items.Status.message));
				var items = await hrServiceManager.HrKpiService.Search(filter);
				return Ok(items);

			}
            catch (Exception ex)
            {
                return Ok(await Result<HRRepKPIFilterDto>.FailAsync(ex.Message));
            }
        }

        #endregion

    }
}