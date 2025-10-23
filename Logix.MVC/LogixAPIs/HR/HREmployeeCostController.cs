using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Result = Logix.Application.Wrapper.Result;

namespace Logix.MVC.LogixAPIs.HR
{
    // تكاليف الموظفين
    public class HREmployeeCostController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ILocalizationService localization;
        private readonly ICurrentData session;

        public HREmployeeCostController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ILocalizationService localization, ICurrentData session)
        {
            this.permission = permission;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.session = session;
        }
        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrEmployeeCostFilterDto filter)
        {

            var chk = await permission.HasPermission(1144, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {

                var items = await hrServiceManager.HrEmployeeCostService.Search(filter);
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrEmployeeCostFilterDto>.FailAsync(ex.Message));
            }
        }



        [HttpPost("GetPagination")]
        public async Task<IActionResult> GetPagination([FromBody] HrEmployeeCostFilterDto filter, int take = Pagination.take, int? lastSeenId = null)
        {
            try
            {
                var chk = await permission.HasPermission(1144, PermissionType.Show);
                if (!chk)
                {
                    return Ok(await Result.AccessDenied("AccessDenied"));
                }
                filter.Location ??= 0;
                filter.NationalityId ??= 0;
                filter.DeptId ??= 0;

                var items = await hrServiceManager.HrEmployeeCostService.GetAllWithPaginationVW(
                    selector: e => e.Id,
                    expression: e =>
                        e.IsDeleted == false &&
                        e.FacilityId == session.FacilityId &&
                        (filter.DeptId == 0 || e.DeptId == filter.DeptId) &&
                        (filter.Location == 0 || e.Location == filter.Location) &&
                        (filter.NationalityId == 0 || e.NationalityId == filter.NationalityId) &&
                        (string.IsNullOrEmpty(filter.EmpName) ||
                            (e.EmpName != null && e.EmpName.ToLower().Contains(filter.EmpName.ToLower())) ||
                            (e.EmpName2 != null && e.EmpName2.ToLower().Contains(filter.EmpName.ToLower()))) &&
                        (string.IsNullOrEmpty(filter.EmpCode) || e.EmpCode == filter.EmpCode),
                    take: take,
                    lastSeenId: lastSeenId
                );

                if (!items.Succeeded)
                    return Ok(await Result<List<HrEmployeeCostFilterDto>>.FailAsync(items.Status.message));

                var res = items.Data
                    .Select(item => new HrEmployeeCostFilterDto
                    {
                        Id = item.Id,
                        EmpCode = item.EmpCode,
                        EmpName = session.Language == 1 ? item.EmpName : item.EmpName2,
                        TypeName = session.Language == 1 ? item.TypeName : item.TypeNameEn,
                        CostValue = item.CostValue,
                    }).ToList();

                var paginatedResult = new PaginatedResult<object>
                {
                    Succeeded = true,
                    Data = res,
                    Status = items.Status,
                    PaginationInfo = items.PaginationInfo
                };

                return Ok(paginatedResult);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }


        [HttpGet("EmpIdChanged")]
        public async Task<IActionResult> EmpIdChanged(string? EmpCode)
        {
            var chk = await permission.HasPermission(1144, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (string.IsNullOrEmpty(EmpCode))
            {
                return Ok(await Result.FailAsync(localization.GetResource1("EmployeeIsNumber")));
            }
            try
            {
                var empData = await hrServiceManager.HrEmployeeCostService.GetEmpDataByEmpId(EmpCode);
                return Ok(empData);
            }
            catch (Exception exp)
            {
                return Ok(await Result<object>.FailAsync($"{exp.Message}"));
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long Id)
        {
            var chk = await permission.HasPermission(1144, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (Id <= 0)
                return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));


            try
            {
                var empData = await hrServiceManager.HrEmployeeCostService.GetDataById(Id);
                return Ok(empData);
            }
            catch (Exception exp)
            {
                return Ok(await Result<object>.FailAsync($"{exp.Message}"));
            }
        }


        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(HrEmployeeCostDataDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(1144, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (string.IsNullOrEmpty(obj.EmpCode))
                    return Ok(await Result.FailAsync(localization.GetResource1("EmployeeIsNumber")));

                var update = await hrServiceManager.HrEmployeeCostService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrEmployeeCostDataDto>.FailAsync($"====== Exp in Edit HR HrEmployeeCost  Controller, MESSAGE: {ex.Message}"));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long Id = 0)
        {
            var chk = await permission.HasPermission(1144, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (Id == 0)
            {
                return Ok(await Result.FailAsync($"{localization.GetMessagesResource("ChooseDelete")}"));
            }

            try
            {
                var del = await hrServiceManager.HrEmployeeCostService.Remove(Id);

                return Ok(del);
            }
            catch (Exception exp)
            {
                return Ok(await Result.FailAsync($"{exp.Message}"));
            }
        }
        [HttpPost("Add")]
        public async Task<ActionResult> Add(HrEmployeeCostDataDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(1144, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result<HrEmployeeCostDataDto>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));
                if (string.IsNullOrEmpty(obj.EmpCode))
                    return Ok(await Result.FailAsync(localization.GetResource1("EmployeeIsNumber")));

                var add = await hrServiceManager.HrEmployeeCostService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrEmployeeCostDataDto>.FailAsync($"====== Exp in Add HR  EmployeeCost Controller  Add Method, MESSAGE: {ex.Message}"));
            }
        }

    }
}
