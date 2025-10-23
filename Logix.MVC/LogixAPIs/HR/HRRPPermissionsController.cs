using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Result = Logix.Application.Wrapper.Result;

namespace Logix.MVC.LogixAPIs.HR
{

    //  تقرير بالإستئذان
    public class HRRPPermissionsController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IMainServiceManager mainServiceManager;
        public HRRPPermissionsController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IMainServiceManager mainServiceManager)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.mainServiceManager = mainServiceManager;
        }

        #region IndexPage

        [HttpPost("Search")]
        public async Task<IActionResult> Search(HRRPPermissionsFilterDto filter)
        {
            var chk = await permission.HasPermission(771, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {

                var BranchesList = session.Branches.Split(',');
                List<HRRPPermissionsFilterDto> resultList = new List<HRRPPermissionsFilterDto>();
                var items = await hrServiceManager.HrPermissionService.GetAllVW(e => e.IsDeleted == false  &&
                (string.IsNullOrEmpty(filter.EmpCode) || filter.EmpCode == e.EmpCode) &&
                (string.IsNullOrEmpty(filter.EmpName) || e.EmpName.ToLower().Contains(filter.EmpName) || e.EmpName2.ToLower().Contains(filter.EmpName)) &&
                (filter.Type == 0 || filter.Type == null || filter.Type == e.Type) &&
                (filter.Reason == 0 || filter.Reason == null || filter.Reason == e.ReasonLeave) 
                );
                if (items.Succeeded)
                {
                    if (items.Data.Count() > 0)
                    {

                        var res = items.Data.AsQueryable();

                        if (!string.IsNullOrEmpty(filter.FromDate) && !string.IsNullOrEmpty(filter.ToDate))
                        {
                            res = res.Where(c => (c.PermissionDate != null && DateHelper.StringToDate(c.PermissionDate) >= DateHelper.StringToDate(filter.FromDate) && DateHelper.StringToDate(c.PermissionDate) <= DateHelper.StringToDate(filter.ToDate)));
                        }
                        if (res.Any())
                        {
                            foreach (var item in res)
                            {

                                var newItem = new HRRPPermissionsFilterDto
                                {
                                    EmpCode = item.EmpCode,
                                    EmpName = item.EmpName,
                                    PermissionType=item.TypeName,
                                    PermissionDate = item.PermissionDate,
                                    ReasonName=item.ReasonName,
                                    DetailsReason=item.DetailsReason,
                                    ExitTime=item.LeaveingTime,
                                    ReturnTime=item.EstimatedTimeReturn,
                                    ContactNumber = item.ContactNumber,

                                };
                                resultList.Add(newItem);
                            }
                            if (resultList.Count > 0) return Ok(await Result<List<HRRPPermissionsFilterDto>>.SuccessAsync(resultList));
                            return Ok(await Result<List<HRRPPermissionsFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));

                        }
                        return Ok(await Result<List<HRRPPermissionsFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));

                    }
                    return Ok(await Result<List<HRRPPermissionsFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));
                }
                return Ok(await Result<HRRPPermissionsFilterDto>.FailAsync(items.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result<HRRPPermissionsFilterDto>.FailAsync(ex.Message));
            }
        }

        #endregion

    }
}