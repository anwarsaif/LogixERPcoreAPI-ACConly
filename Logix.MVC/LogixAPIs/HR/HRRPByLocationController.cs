using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Result = Logix.Application.Wrapper.Result;

namespace Logix.MVC.LogixAPIs.HR
{
    //  تقرير إجمالي بالموظفين حسب الموقع
    public class HRRPByLocationController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IMainServiceManager mainServiceManager;


        public HRRPByLocationController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IMainServiceManager mainServiceManager)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.mainServiceManager = mainServiceManager;
        }


        #region IndexPage

        [HttpPost("Search")]
        public async Task<IActionResult> Search(HRRPByLocationFilterDto filter)
        {
            var chk = await permission.HasPermission(731, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var BranchesList = session.Branches.Split(',');

                List<HRRPByLocationFilterDto> resultList = new List<HRRPByLocationFilterDto>();

                var items = await hrServiceManager.HrEmployeeService.GetAllVW(e => e.IsDeleted == false
               && BranchesList.Contains(e.BranchId.ToString())

               && (filter.JobCatagoriesId == null || filter.JobCatagoriesId == 0 || filter.JobCatagoriesId == e.JobCatagoriesId)
               && (filter.Status == null || filter.Status == 0 || filter.Status == e.StatusId)
               && (filter.NationalityId == null || filter.NationalityId == 0 || filter.NationalityId == e.NationalityId)
               && (filter.Location == null || filter.Location == 0 || filter.Location == e.Location)
                );
                if (items.Succeeded)
                {
                    if (items.Data.Count() > 0)
                    {

                        var res = items.Data.AsQueryable();
                        if (filter.BranchId != null && filter.BranchId > 0)
                        {
                            res = res.Where(x => x.BranchId == filter.BranchId);
                        }
                        if (res.Count() > 0)
                        {
                            var DistinctLocations = res.GroupBy(location => location.LocationName)
  .Select(group => new { LocationName = group.Key, Count = group.Count() });

                            foreach (var item in DistinctLocations)
                            {
                                var newItem = new HRRPByLocationFilterDto
                                {
                                    EmployeeCount = item.Count,
                                    LocationName = item.LocationName ?? "",

                                };
                                resultList.Add(newItem);
                            }
                            if (resultList.Count > 0) return Ok(await Result<List<HRRPByLocationFilterDto>>.SuccessAsync(resultList));
                            return Ok(await Result<List<HRRPByLocationFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));
                        }
                        return Ok(await Result<List<HRRPByLocationFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));


                    }
                    return Ok(await Result<List<HRRPByLocationFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));

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