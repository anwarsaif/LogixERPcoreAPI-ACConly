using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Polly;

namespace Logix.MVC.LogixAPIs.HR
{

    //اسناد الموظفين للمواقع
    public class HRLocationEmployeeController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;


        public HRLocationEmployeeController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
        }


        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrAttLocationEmployeeFilterDto filter)
        {
            var chk = await permission.HasPermission(553, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var BranchesList = session.Branches.Split(',');
                var items = await hrServiceManager.HrAttLocationEmployeeService.GetAllVW(e => e.IsDeleted == false && BranchesList.Contains(e.BranchId.ToString()));
                if (items.Succeeded)
                {
                    if (items.Data.Count() > 0)
                    {

                        var res = items.Data.AsQueryable();
                        if (!string.IsNullOrEmpty(filter.EmpCode))
                        {
                            res = res.Where(c => c.EmpCode != null && c.EmpCode == filter.EmpCode);
                        }
                        if (!string.IsNullOrEmpty(filter.EmpName))
                        {
                            res = res.Where(c => (c.EmpName != null && c.EmpName.Contains(filter.EmpName)));
                        }

                        if (filter.BranchId != null && filter.BranchId > 0)
                        {
                            res = res.Where(c => c.BranchId != null && c.BranchId.Equals(filter.BranchId));
                        }
                        if (filter.LocationId != null && filter.LocationId > 0)
                        {
                            res = res.Where(c => c.LocationId != null && c.LocationId.Equals(filter.LocationId));
                        }

                        if (res.Any())
                            return Ok(await Result<List<HrAttLocationEmployeeVw>>.SuccessAsync(res.ToList(), ""));
                        return Ok(await Result<List<HrAttLocationEmployeeVw>>.SuccessAsync(new List<HrAttLocationEmployeeVw>(), localization.GetResource1("NosearchResult")));
                    }
                    return Ok(await Result<List<HrAttLocationEmployeeVw>>.SuccessAsync(items.Data.ToList(), localization.GetResource1("NosearchResult")));
                }
                return Ok(await Result<HrAttLocationEmployeeVw>.FailAsync(items.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrAttLocationEmployeeVw>.FailAsync(ex.Message));
            }
        }

        [HttpPost("Add")]
        public async Task<ActionResult> Add(HrAttLocationEmployeeDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(553, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));
                if (string.IsNullOrEmpty(obj.BeginDate)) return Ok(await Result.FailAsync($"يجب ادخال من تاريخ"));
                if (string.IsNullOrEmpty(obj.EndDate)) return Ok(await Result.FailAsync($"يجب ادخال الى تاريخ"));
                if (obj.LocationId <= 0) return Ok(await Result.FailAsync($"يجب تحديد الموقع"));


                var add = await hrServiceManager.HrAttLocationEmployeeService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add Hr AttLocationEmployee  Controller, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(HrAttLocationEmployeeEditeDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(553, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result<HrAttLocationEmployeeEditeDto>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var update = await hrServiceManager.HrAttLocationEmployeeService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrAttLocationEmployeeEditeDto>.FailAsync($"====== Exp in Hr LocationEmployee Controller getById, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long Id)
        {
            try
            {
                var chk = await permission.HasPermission(553, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var item = await hrServiceManager.HrAttLocationEmployeeService.GetOneVW(x => x.Id == Id);
                return Ok(item);

            }
            catch (Exception ex)
            {
                return Ok(await Result<HrDecisionsVw>.FailAsync($"====== Exp in Hr LocationEmployee Controller getById, MESSAGE: {ex.Message}"));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var chk = await permission.HasPermission(553, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await hrServiceManager.HrAttLocationEmployeeService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete Hr LocationEmployee Controller, MESSAGE: {ex.Message}"));
            }
        }
        [HttpPost("Search2")]


        public async Task<IActionResult> Search2(HrAttLocationEmployeeFilterDto filter)
        {
            var chk = await permission.HasPermission(553, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                if (filter.LocationId <= 0)
                {
                    return Ok(await Result<object>.FailAsync("The LocationId is Required"));
                }
                filter.JobCatagoriesId ??= 0;
                filter.BranchId ??= 0;
                filter.DeptId ??= 0;
                filter.Location ??= 0;

                var branchesList = session.Branches.Split(',');

                var getEmployees = await hrServiceManager.HrEmployeeService.GetAllVW(e =>
                    e.IsDeleted == false &&
                    e.StatusId == 1 &&
                    e.FacilityId == session.FacilityId &&
                    branchesList.Contains(e.BranchId.ToString()) &&
                    (string.IsNullOrEmpty(filter.EmpCode) || e.EmpId == filter.EmpCode) &&
                    (string.IsNullOrEmpty(filter.EmpName) || (e.EmpName != null && e.EmpName.Contains(filter.EmpName))) &&
                    (filter.BranchId == 0 || e.BranchId == filter.BranchId) &&
                    (filter.JobCatagoriesId == 0 || e.JobCatagoriesId == filter.JobCatagoriesId) &&
                    (filter.DeptId == 0 || e.DeptId == filter.DeptId) &&
                    (filter.Location == 0 || e.Location == filter.Location)
                );

                // Filter out items present in HR_Att_Location_Employee
                var items = await hrServiceManager.HrAttLocationEmployeeService.GetAll(e => e.IsDeleted == false && e.LocationId == filter.LocationId);
                var itemsIds = items.Data.Select(x => x.EmpId).ToList();
                var result = getEmployees.Data
                    .Where(x => !itemsIds.Contains(x.Id))
                    .Select(x => new
                    {
                        x.EmpName,
                        x.EmpName2,
                        x.EmpId,
                        x.Id
                    }).OrderBy(x=>Convert.ToInt64(x.EmpId));

                if (result.Any())
                {
                    return Ok(await Result<object>.SuccessAsync(result.ToList(), ""));
                }
                else
                {
                    return Ok(await Result<object>.SuccessAsync(new List<object>(), localization.GetResource1("NosearchResult")));
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync(ex.Message));
            }
        }

        [HttpPost("Search1")]
        public async Task<IActionResult> Search1(HrAttLocationEmployeeFilterDto filter)
        {
            try
            {

                var chk = await permission.HasPermission(553, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (filter.LocationId <= 0)
                {
                    return Ok(await Result<object>.FailAsync("The LocationId is Required"));
                }
                var result = await hrServiceManager.HrEmployeeLocationVwService.Search(filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in search Hr LocationEmployee Controller, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Cancel")]
        public async Task<ActionResult> Cancel(HrAttLocationEmployeeCancelDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(553, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var add = await hrServiceManager.HrAttLocationEmployeeService.Cancel(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add Hr AttLocationEmployee  Controller, MESSAGE: {ex.Message}"));
            }
        }
    }
}
