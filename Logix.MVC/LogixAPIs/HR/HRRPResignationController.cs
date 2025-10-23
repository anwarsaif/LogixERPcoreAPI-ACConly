using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using Result = Logix.Application.Wrapper.Result;

namespace Logix.MVC.LogixAPIs.HR
{
    //  تقرير باستقالة موظفين
    public class HRRPResignationController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        public HRRPResignationController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
        }

        #region IndexPage

        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrRPResignationFilterDto filter)
        {
            var chk = await permission.HasPermission(387, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var BranchesList = session.Branches.Split(',');
                List<HrRPResignationFilterDto> resultList = new List<HrRPResignationFilterDto>();
                var items = await hrServiceManager.HrLeaveService.GetAllVW(e => e.IsDeleted == false && BranchesList.Contains(e.BranchId.ToString()) &&
                (string.IsNullOrEmpty(filter.EmpCode) || filter.EmpCode == e.EmpCode) &&
                (string.IsNullOrEmpty(filter.EmpName) || e.EmpName.ToLower().Contains(filter.EmpName) ) &&
                (filter.LocationId == 0 || filter.LocationId == null || filter.LocationId == e.Location) &&
                (filter.NationalityId == 0 || filter.NationalityId == null || filter.NationalityId == e.NationalityId) &&
                (filter.JobCategory == 0 || filter.JobCategory == null || filter.JobCategory == e.JobCatagoriesId) &&
                (filter.FacilityId == 0 || filter.FacilityId == null || filter.FacilityId == e.FacilityId) &&
                (filter.LeaveReason == 0 || filter.LeaveReason == null || filter.LeaveReason == e.LeaveType)
                );
                if (items.Succeeded)
                {
                    if (items.Data.Count() > 0)
                    {

                        var res = items.Data.AsQueryable();

                        if (filter.BranchId != null && filter.BranchId > 0)
                        {
                            res = res.Where(c => c.BranchId != null && c.BranchId.Equals(filter.BranchId));
                        }
                        if (!string.IsNullOrEmpty(filter.FromDate) && !string.IsNullOrEmpty(filter.ToDate))
                        {
                            res = res.Where(c => (c.LeaveDate != null && DateHelper.StringToDate(c.LeaveDate) >= DateHelper.StringToDate(filter.FromDate) && DateHelper.StringToDate(c.LeaveDate) <= DateHelper.StringToDate(filter.ToDate)));
                        }
                        if (res.Any())
                        {
                            foreach (var item in res)
                            {
                                var newItem = new HrRPResignationFilterDto
                                {
                                    EmpCode = item.EmpCode,
                                    EmpName = item.EmpName,
                                    DOAppointment = item.Doappointment,
                                    FacilityName = item.FacilityName,
                                    CatName = item.CatName,
                                    NationalityName = item.NationalityName,
                                    LocationName = item.LocationName,
                                    BranchName = item.BraName,
                                    DepartmentName = item.DepName,
                                    EndOfServiceDate = item.LeaveDate,
                                    EndOfServiceReason = item.TypeName,
                                    Note = item.Note

                                };
                                resultList.Add(newItem);
                            }
                            if (resultList.Count > 0) return Ok(await Result<List<HrRPResignationFilterDto>>.SuccessAsync(resultList));
                            return Ok(await Result<List<HrRPResignationFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));

                        }
                        return Ok(await Result<List<HrRPResignationFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));
                    }
                    return Ok(await Result<List<HrRPResignationFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));
                }
                return Ok(await Result<HrRPResignationFilterDto>.FailAsync(items.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrRPResignationFilterDto>.FailAsync(ex.Message));
            }
        }

        #endregion

    }
}