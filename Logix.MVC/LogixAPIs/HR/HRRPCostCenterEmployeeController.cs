using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Result = Logix.Application.Wrapper.Result;

namespace Logix.MVC.LogixAPIs.HR
{
    //  تقرير بمراكز تكلفة الموظفين
    public class HRRPCostCenterEmployeeController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IMainServiceManager mainServiceManager;
        public HRRPCostCenterEmployeeController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IMainServiceManager mainServiceManager)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.mainServiceManager = mainServiceManager;
        }

        #region IndexPage

        [HttpPost("Search")]
        public async Task<IActionResult> Search(RPCostCenterEmployeeFilterDto filter)
        {
            var chk = await permission.HasPermission(1560, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var BranchesList = session.Branches.Split(',');
                List<RPCostCenterEmployeeFilterDto> resultList = new List<RPCostCenterEmployeeFilterDto>();
                var items = await hrServiceManager.HrEmployeeService.GetAllVW(e => e.IsDeleted == false && e.Isdel == false && BranchesList.Contains(e.BranchId.ToString()) && e.IsSub==false &&
                (string.IsNullOrEmpty(filter.EmpCode) || filter.EmpCode == e.EmpId) &&
                (string.IsNullOrEmpty(filter.EmpName) || e.EmpName.ToLower().Contains(filter.EmpName) || e.EmpName2.ToLower().Contains(filter.EmpName)) &&
                (filter.StatusId == 0 || filter.StatusId == null || filter.StatusId == e.StatusId) &&
                (string.IsNullOrEmpty(filter.CostCenterCode) || filter.CostCenterCode == e.CostCenterCode) &&
                (string.IsNullOrEmpty(filter.CostCenterName) || e.CostCenterName.ToLower().Contains(filter.CostCenterName) ) 

                );
                if (items.Succeeded)
                {
                    if (items.Data.Count() > 0)
                    {

                        var res = items.Data.AsQueryable();

                        if (res.Any())
                        {
                            var getSalaryGroups = await hrServiceManager.HrSalaryGroupService.GetAll(x => x.IsDeleted == false);

                            foreach (var item in res)
                            {

                                string SalaryGroupName = getSalaryGroups.Data.Where(x => x.Id == item.SalaryGroupId).Select(x => x.Name).FirstOrDefault();
                                
                                
                                var newItem = new RPCostCenterEmployeeFilterDto
                                {
                                    EmpCode = item.EmpId,
                                    IdNo = item.IdNo,
                                    EmpName = item.EmpName,
                                    LocationName = item.LocationName,
                                    CostCenterCode=item.CostCenterCode,
                                    CostCenterName=item.CostCenterName,
                                    SalaryGroupName = SalaryGroupName,
                                };
                                resultList.Add(newItem);
                            }
                            if (resultList.Count > 0) return Ok(await Result<List<RPCostCenterEmployeeFilterDto>>.SuccessAsync(resultList));
                            return Ok(await Result<List<RPCostCenterEmployeeFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));

                        }
                        return Ok(await Result<List<RPCostCenterEmployeeFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));

                    }
                    return Ok(await Result<List<RPCostCenterEmployeeFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));
                }
                return Ok(await Result<RPCostCenterEmployeeFilterDto>.FailAsync(items.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result<RPCostCenterEmployeeFilterDto>.FailAsync(ex.Message));
            }
        }

        #endregion

    }
}