using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.HR.ViewModel;
using Microsoft.AspNetCore.Mvc;
using AllowanceVM = Logix.Application.DTOs.HR.AllowanceVM;
using DeductionVM = Logix.Application.DTOs.HR.DeductionVM;

namespace Logix.MVC.LogixAPIs.HR
{
    //  حالة الموظفين
    public class HREmployeeStatusController : BaseHrApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;

        public HREmployeeStatusController(IHrServiceManager hrServiceManager, IMainServiceManager mainServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization)
        {
            this.mainServiceManager = mainServiceManager;
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
        }
        [HttpPost("Search")]
        public async Task<IActionResult> Search(EmployeeSubFilterDto filter)
        {
            var chk = await permission.HasPermission(611, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var BranchesList = session.Branches.Split(',');
                filter.BranchId ??= 0;
                filter.JobType ??= 0;
                filter.JobCatagoriesId ??= 0;
                filter.StatusId ??= 0;
                filter.NationalityId ??= 0;
                filter.DeptId ??= 0;
                filter.LocationId ??= 0;
                filter.FacilityId ??= 0;
                filter.SponsorsId ??= 0;
                var items = await hrServiceManager.HrEmployeeService.GetAllVW(e => /*e.IsDeleted == false &&*/ e.Isdel == false 
                && (filter.BranchId != 0 ? e.BranchId == filter.BranchId : BranchesList.Contains(e.BranchId.ToString()))
                && (string.IsNullOrEmpty(filter.EmpId) || e.EmpId == filter.EmpId)
                && (string.IsNullOrEmpty(filter.EmpName) || e.EmpName.Contains(filter.EmpName))
                && (filter.JobType == 0 || e.JobType == filter.JobType)
                && (filter.JobCatagoriesId == 0 || e.JobCatagoriesId == filter.JobCatagoriesId)
                && (filter.StatusId == 0 || e.StatusId == filter.StatusId)
                && (filter.NationalityId == 0 || e.NationalityId == filter.NationalityId)
                && (filter.DeptId == 0 || e.DeptId == filter.DeptId)
                && (filter.LocationId == 0 || e.Location == filter.LocationId)
                && (filter.FacilityId == 0 || e.FacilityId == filter.FacilityId)
                && (filter.SponsorsId == 0 || e.SponsorsId == filter.SponsorsId)
                && (string.IsNullOrEmpty(filter.PassportNo) || e.PassportNo.Contains(filter.PassportNo))
                && (string.IsNullOrEmpty(filter.IdNo) || e.IdNo.Contains(filter.IdNo))
                && (string.IsNullOrEmpty(filter.EntryNo) || e.EntryNo.Contains(filter.EntryNo))
                );
                if (items.Succeeded)
                {
                    if (items.Data.Count() > 0)
                    {
                        return Ok(await Result<List<HrEmployeeVw>>.SuccessAsync(items.Data.ToList(), ""));
                    }

                    return Ok(await Result<List<HrEmployeeVw>>.SuccessAsync(items.Data.ToList(), localization.GetResource1("NosearchResult")));

                }

                return Ok(await Result<HrEmployeeVw>.FailAsync(items.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        [HttpPost("GetPagination")]
        public async Task<IActionResult> GetPagination([FromBody] EmployeeSubFilterDto filter, int take = Pagination.take, long? lastSeenId = null)
        {
            var chk = await permission.HasPermission(611, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var BranchesList = session.Branches.Split(',');
                filter.BranchId ??= 0;
                filter.JobType ??= 0;
                filter.JobCatagoriesId ??= 0;
                filter.StatusId ??= 0;
                filter.NationalityId ??= 0;
                filter.DeptId ??= 0;
                filter.LocationId ??= 0;
                filter.FacilityId ??= 0;
                filter.SponsorsId ??= 0;

                var items = await hrServiceManager.HrEmployeeService.GetAllWithPaginationVW(selector: e => e.Id,
                expression: e => /*e.IsDeleted == false &&*/ e.Isdel == false
                && (filter.BranchId != 0 ? e.BranchId == filter.BranchId : BranchesList.Contains(e.BranchId.ToString()))
                && (string.IsNullOrEmpty(filter.EmpId) || e.EmpId == filter.EmpId)
                && (string.IsNullOrEmpty(filter.EmpName) || e.EmpName.Contains(filter.EmpName))
                && (filter.JobType == 0 || e.JobType == filter.JobType)
                && (filter.JobCatagoriesId == 0 || e.JobCatagoriesId == filter.JobCatagoriesId)
                && (filter.StatusId == 0 || e.StatusId == filter.StatusId)
                && (filter.NationalityId == 0 || e.NationalityId == filter.NationalityId)
                && (filter.DeptId == 0 || e.DeptId == filter.DeptId)
                && (filter.LocationId == 0 || e.Location == filter.LocationId)
                && (filter.FacilityId == 0 || e.FacilityId == filter.FacilityId)
                && (filter.SponsorsId == 0 || e.SponsorsId == filter.SponsorsId)
                && (string.IsNullOrEmpty(filter.PassportNo) || e.PassportNo.Contains(filter.PassportNo))
                && (string.IsNullOrEmpty(filter.IdNo) || e.IdNo.Contains(filter.IdNo))
                && (string.IsNullOrEmpty(filter.EntryNo) || e.EntryNo.Contains(filter.EntryNo)),
                    take: take,
                    lastSeenId: lastSeenId);

                if (!items.Succeeded)
                    return Ok(await Result<List<HrEmployeeVw>>.FailAsync(items.Status.message));
                
                var paginatedData = new PaginatedResult<object>
                {
                    Succeeded = items.Succeeded,
                    Data = items.Data,
                    Status = items.Status,
                    PaginationInfo = items.PaginationInfo
                };
                return Ok(paginatedData);
                
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        [HttpPost("ChanhgeStatus")]
        public async Task<IActionResult> ChanhgeStatus(int StatusId, List<string> employeesId,
    [FromQuery] string? Note)
        {
            var chk = await permission.HasPermission(611, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (!employeesId.Any() || StatusId <= 0)
            {
                return Ok(Result.SuccessAsync());
            }


            try
            {
                var getItem = await mainServiceManager.InvestEmployeeService.ChangeEmployeesStatus(StatusId, employeesId,Note);

                return Ok(getItem);

            }
            catch (Exception exp)
            {
                return Ok(Result<bool>.FailAsync($"EXP in {this.GetType()}, Meesage: {exp.Message}"));
            }
        }

    }
}