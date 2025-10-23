using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Result = Logix.Application.Wrapper.Result;

namespace Logix.MVC.LogixAPIs.HR
{

    //  تقرير بالانتداب
    public class HRRPMandateController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IMainServiceManager mainServiceManager;


        public HRRPMandateController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IMainServiceManager mainServiceManager)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.mainServiceManager = mainServiceManager;
        }


        #region IndexPage

        [HttpPost("Search")]
        public async Task<IActionResult> Search(HRRPMandateFilterDto filter)
        {
            var chk = await permission.HasPermission(1507, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                filter.Location ??= 0;
                filter.TypeId ??= 0;
                filter.FromLocation ??= 0;
                filter.ToLocation ??= 0;
                filter.JobCatagoriesId ??= 0;
                filter.DeptId ??= 0;
                List<HRRPMandateFilterDto> resultList = new List<HRRPMandateFilterDto>();

                var items = await hrServiceManager.HrMandateService.GetAllVW(e => e.IsDeleted == false &&
               (filter.BranchId == 0 || filter.BranchId == e.BranchId) &&
                (string.IsNullOrEmpty(filter.EmpName) || (e.EmpName != null && e.EmpName.ToLower().Contains(filter.EmpName.ToLower())) || (e.EmpName2 != null && e.EmpName2.ToLower().Contains(filter.EmpName.ToLower()))) &&
                (string.IsNullOrEmpty(filter.EmpCode) || e.EmpCode == filter.EmpCode) &&
                (filter.JobCatagoriesId == 0 || filter.JobCatagoriesId == e.JobCatagoriesId) &&
                (filter.DeptId == 0 || filter.DeptId == e.DeptId) &&
                (filter.FromLocation == 0 || filter.FromLocation == Convert.ToInt32(e.FromLocation)) &&
                (filter.ToLocation == 0 || filter.ToLocation == Convert.ToInt32(e.ToLocation)) &&
                (filter.TypeId == 0 || filter.TypeId == e.TypeId) &&
                (filter.Location == 0 || filter.Location == e.Location)
                );

                if (!items.Succeeded)
                    return Ok(await Result<object>.FailAsync(items.Status.message));


                if (!items.Data.Any())
                    return Ok(await Result<List<HRRPMandateFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));

                var res = items.Data.AsQueryable();

                if (!string.IsNullOrEmpty(filter.FromDate) && !string.IsNullOrEmpty(filter.ToDate))
                {
                    res = res.Where(c => (c.FromDate != null && DateHelper.StringToDate(c.FromDate) >= DateHelper.StringToDate(filter.FromDate) && DateHelper.StringToDate(c.FromDate) <= DateHelper.StringToDate(filter.ToDate)));
                }
                foreach (var item in res)
                {
                    var newItem = new HRRPMandateFilterDto
                    {
                        Id = item.Id,
                        EmpCode = item.EmpCode,
                        EmpName = item.EmpName,
                        FromDate = item.FromDate,
                        ToDate = item.ToDate,
                        NumberOfNights = item.NoOfNight,
                        FromLocationName = item.FromLocation,
                        ToLocationName = item.ToLocation,
                        ActualExpenses = item.ActualExpenses ?? 0,
                        TypeId = item.TypeId,
                    };
                    resultList.Add(newItem);
                }

                if (resultList.Count > 0)
                {
                    var statistic = new List<object>
                    {
                        new {Cnt = resultList.Count,Description = "عدد الانتدابات"},
                        new {Cnt = resultList.Where(x=>x.TypeId==1).Count(),Description = "الانتدابات الداخلي"},
                        new {Cnt = resultList.Where(x=>x.TypeId==2).Count(),Description = "الانتدابات الخارجي"},
                        new {Cnt = resultList.Sum(x=>x.ActualExpenses),Description = "إجمالي المصاريف"},
                        new {Cnt = resultList.Where(x=>x.TypeId==1).Sum(x=>x.ActualExpenses),Description = "إجمالي المصاريف الداخلي"},
                        new {Cnt = resultList.Where(x=>x.TypeId==2).Sum(x=>x.ActualExpenses),Description = "إجمالي المصاريف الخارجي"},
                    };
                    return Ok(await Result<object>.SuccessAsync(new { Data = resultList, Statistics = statistic }));

                }
                return Ok(await Result<List<object>>.SuccessAsync(new List<object>(), localization.GetResource1("NosearchResult")));
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync(ex.Message));
            }
        }

        #endregion

    }
}