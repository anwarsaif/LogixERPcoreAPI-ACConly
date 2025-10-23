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
    // تقرير بالموقوفين رواتبهم
    public class HRRPOFSalariesController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IMainServiceManager mainServiceManager;


        public HRRPOFSalariesController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IMainServiceManager mainServiceManager)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.mainServiceManager = mainServiceManager;
        }


        #region IndexPage

        [HttpPost("Search")]
        public async Task<IActionResult> Search(HRRPOFSalarieFilterDto filter)
        {
            var chk = await permission.HasPermission(923, PermissionType.Show);
            if (!chk)
            {
                return BadRequest(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var BranchesList = session.Branches.Split(',');

                List<HRRPOFSalarieFilterDto> resultList = new List<HRRPOFSalarieFilterDto>();

                var items = await hrServiceManager.HrEmployeeService.GetAllVW(e => e.IsDeleted == false && e.Isdel == false && BranchesList.Contains(e.BranchId.ToString()) &&
                (string.IsNullOrEmpty(filter.EmpName) || e.EmpName.ToLower().Contains(filter.EmpName.ToLower()) || e.EmpName2.ToLower().Contains(filter.EmpName.ToLower())) &&
                (e.StopSalary == true && e.StopDateSalary != "" && !string.IsNullOrEmpty(e.StopDateSalary)) &&
                (string.IsNullOrEmpty(filter.EmpCode) || e.EmpId == filter.EmpCode)
                );
                if (items.Succeeded)
                {
                    if (items.Data.Count() > 0)
                    {

                        var res = items.Data.AsQueryable();

                        if (res.Count() >= 0)
                        {

                            if (filter.BranchId != null && filter.BranchId > 0)
                            {
                                res = res.Where(c => c.BranchId != null && c.BranchId==filter.BranchId);
                            }
                            if (!string.IsNullOrEmpty(filter.FromDate) && !string.IsNullOrEmpty(filter.ToDate))
                            {
                                res = res.Where(c => (c.StopDateSalary != null && DateHelper.StringToDate(c.StopDateSalary) >= DateHelper.StringToDate(filter.FromDate) && DateHelper.StringToDate(c.StopDateSalary) <= DateHelper.StringToDate(filter.ToDate)));
                            }
                            var getFromSyslookupData = await mainServiceManager.SysLookupDataService.GetAll(x => x.Isdel == false && x.CatagoriesId == 13);
                            foreach (var item in res)
                            {
                                string? StopSalaryName = getFromSyslookupData.Data.Where(x=>x.Code==item.StopSalaryCode).Select(x=>x.Name).FirstOrDefault();
                                var newItem = new HRRPOFSalarieFilterDto
                                {
                                    EmpCode = item.EmpId,
                                    EmpName = item.EmpName,
                                    StopSalaryName = StopSalaryName ?? "",
                                    StopSalaryDate=item.StopDateSalary
                                };
                                resultList.Add(newItem);
                            }
                        }
                        if (resultList.Count > 0) return Ok(await Result<List<HRRPOFSalarieFilterDto>>.SuccessAsync(resultList));
                        return Ok(await Result<List<HRRPOFSalarieFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));

                    }
                    return Ok(await Result<List<HRRPOFSalarieFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));

                }

                return Ok(await Result<HRRPOFSalarieFilterDto>.FailAsync(items.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result<HRRPOFSalarieFilterDto>.FailAsync(ex.Message));
            }
        }

        #endregion

    }
}