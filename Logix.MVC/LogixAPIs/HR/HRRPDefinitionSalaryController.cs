using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Result = Logix.Application.Wrapper.Result;

namespace Logix.MVC.LogixAPIs.HR
{

    //  تقرير طباعة تعريف موظف
    public class HRRPDefinitionSalaryController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        public HRRPDefinitionSalaryController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
        }

        #region IndexPage

        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrRPDefinitionSalaryFilterDto filter)
        {
            var chk = await permission.HasPermission(1918, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var BranchesList = session.Branches.Split(',');
                List<HrRPDefinitionSalaryFilterDto> resultList = new List<HrRPDefinitionSalaryFilterDto>();
                var items = await hrServiceManager.HrDefinitionSalaryEmpService.GetAll(e => e.IsDeleted == false && e.FacilityId == session.FacilityId &&
                (string.IsNullOrEmpty(filter.EmpCode) || filter.EmpCode == e.EmpId)
                && (string.IsNullOrEmpty(filter.EmpName) || e.EmpName.ToLower().Contains(filter.EmpName))
                && (filter.Sample == 0 || filter.Sample == e.DefinitionTypeId)
 );
                if (items.Succeeded)
                {
                    if (items.Data.Count() > 0)
                    {

                        var res = items.Data.AsQueryable();


                        if (!string.IsNullOrEmpty(filter.FromDate) && !string.IsNullOrEmpty(filter.ToDate))
                        {
                            res = res.Where(c => (c.DefinitionDate != null && c.DefinitionDate != "" && DateHelper.StringToDate(c.DefinitionDate) >= DateHelper.StringToDate(filter.FromDate) && DateHelper.StringToDate(c.DefinitionDate) <= DateHelper.StringToDate(filter.ToDate)));
                        }
                        if (res.Count() > 0)
                        {
                            foreach (var item in res)
                            {
                                var newItem = new HrRPDefinitionSalaryFilterDto
                                {
                                    EmpCode = item.EmpId,
                                    EmpName = item.EmpName,
                                    LocationName = item.LocationName,
                                    DepartmentName = item.DepName,
                                    Date = item.DefinitionDate,
                                    FileURL = item.DefinitionUrl,
                                    SampleName = item.DefinitionTypeName,
                                    SendToName=item.DefinitionSendToName

                                };
                                resultList.Add(newItem);
                            }
                            if (resultList.Count > 0) return Ok(await Result<List<HrRPDefinitionSalaryFilterDto>>.SuccessAsync(resultList));
                            return Ok(await Result<List<HrRPDefinitionSalaryFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));

                        }
                        return Ok(await Result<List<HrRPDefinitionSalaryFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));
                    }
                    return Ok(await Result<List<HrRPDefinitionSalaryFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));
                }
                return Ok(await Result<HrRPDefinitionSalaryFilterDto>.FailAsync(items.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrRPDefinitionSalaryFilterDto>.FailAsync(ex.Message));
            }
        }

        #endregion

    }
}