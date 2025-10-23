using DocumentFormat.OpenXml.ExtendedProperties;
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

    //  تغيير المدير المباشر
    public class HRTransferManagerToManagerController : BaseHrApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;

        public HRTransferManagerToManagerController(IHrServiceManager hrServiceManager, IMainServiceManager mainServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization)
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
            var chk = await permission.HasPermission(831, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var BranchesList = session.Branches.Split(',');
                filter.BranchId ??= 0;
                filter.DeptId ??= 0;
                filter.JobCatagoriesId ??= 0;
                filter.LocationId ??= 0;
                filter.NationalityId ??= 0;
                var managerId = 0L;
                if (!string.IsNullOrEmpty(filter.ManagerId))
                {
                    var checkManager = await mainServiceManager.InvestEmployeeService.GetManagerId(filter.ManagerId);
                    if (checkManager == 0)
                    {
                        return Ok(await Result<List<HrEmployeeVw>>.FailAsync(localization.GetResource1("EmployeeNotFound")));
                    }
                    managerId = checkManager;
                }
                var manager2Id = 0L;
                if (!string.IsNullOrEmpty(filter.Manager2Id))
                {
                    var checkManager = await mainServiceManager.InvestEmployeeService.GetManagerId(filter.Manager2Id);
                    if (checkManager == 0)
                    {
                        return Ok(await Result<List<HrEmployeeVw>>.FailAsync(localization.GetResource1("EmployeeNotFound")));
                    }
                    manager2Id = checkManager;
                }
                var manager3Id = 0L;
                if (!string.IsNullOrEmpty(filter.Manager3Id))
                {
                    var checkManager = await mainServiceManager.InvestEmployeeService.GetManagerId(filter.Manager3Id);
                    if (checkManager == 0)
                    {
                        return Ok(await Result<List<HrEmployeeVw>>.FailAsync(localization.GetResource1("EmployeeNotFound")));
                    }
                    manager3Id = checkManager;
                }
                var items = await hrServiceManager.HrEmployeeService.GetAllVW(e => e.IsDeleted == false && e.Isdel == false && e.StatusId == 1 &&
                (filter.BranchId != 0 ? e.BranchId == filter.BranchId : BranchesList.Contains(e.BranchId.ToString())) &&
                (filter.DeptId == 0 || e.DeptId == filter.DeptId) &&
                (string.IsNullOrEmpty(filter.EmpId) || e.EmpId == filter.EmpId) &&
                (string.IsNullOrEmpty(filter.EmpName) || e.EmpName.Contains(filter.EmpName)) &&
                (filter.LocationId == 0 || e.Location == filter.LocationId) &&
                (managerId == 0 || e.ManagerId == managerId) &&
                (manager2Id == 0 || e.Manager2Id == manager2Id) &&
                (manager3Id == 0 || e.Manager3Id == manager3Id) &&
                (filter.NationalityId == 0 || e.NationalityId == filter.NationalityId) &&
                (string.IsNullOrEmpty(filter.IdNo) || e.IdNo.Contains(filter.IdNo))
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
            var chk = await permission.HasPermission(831, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var BranchesList = session.Branches.Split(',');
                filter.BranchId ??= 0;
                filter.DeptId ??= 0;
                filter.JobCatagoriesId ??= 0;
                filter.LocationId ??= 0;
                filter.NationalityId ??= 0;
                var managerId = 0L;

                if (!string.IsNullOrEmpty(filter.ManagerId))
                {
                    var checkManager = await mainServiceManager.InvestEmployeeService.GetManagerId(filter.ManagerId);
                    if (checkManager == 0)
                    {
                        return Ok(await Result<List<HrEmployeeVw>>.FailAsync(localization.GetResource1("EmployeeNotFound")));
                    }
                    managerId = checkManager;
                }
                var manager2Id = 0L;
                if (!string.IsNullOrEmpty(filter.Manager2Id))
                {
                    var checkManager = await mainServiceManager.InvestEmployeeService.GetManagerId(filter.Manager2Id);
                    if (checkManager == 0)
                    {
                        return Ok(await Result<List<HrEmployeeVw>>.FailAsync(localization.GetResource1("EmployeeNotFound")));
                    }
                    manager2Id = checkManager;
                }
                var manager3Id = 0L;
                if (!string.IsNullOrEmpty(filter.Manager3Id))
                {
                    var checkManager = await mainServiceManager.InvestEmployeeService.GetManagerId(filter.Manager3Id);
                    if (checkManager == 0)
                    {
                        return Ok(await Result<List<HrEmployeeVw>>.FailAsync(localization.GetResource1("EmployeeNotFound")));
                    }
                    manager3Id = checkManager;
                }

                var items = await hrServiceManager.HrEmployeeService.GetAllWithPaginationVW(selector: e => e.Id,
                expression: e => e.IsDeleted == false && e.Isdel == false && e.StatusId == 1 &&
                (filter.BranchId != 0 ? e.BranchId == filter.BranchId : BranchesList.Contains(e.BranchId.ToString())) &&
                (filter.DeptId == 0 || e.DeptId == filter.DeptId) &&
                (string.IsNullOrEmpty(filter.EmpId) || e.EmpId == filter.EmpId) &&
                (string.IsNullOrEmpty(filter.EmpName) || e.EmpName.Contains(filter.EmpName)) &&
                (filter.LocationId == 0 || e.Location == filter.LocationId) &&
                (managerId == 0 || e.ManagerId == managerId) &&
                (manager2Id == 0 || e.Manager2Id == manager2Id) &&
                (manager3Id == 0 || e.Manager3Id == manager3Id) &&
                (filter.NationalityId == 0 || e.NationalityId == filter.NationalityId) &&
                (string.IsNullOrEmpty(filter.IdNo) || e.IdNo.Contains(filter.IdNo)),
                    take: take,
                    lastSeenId: lastSeenId);

                if (!items.Succeeded)
                    return Ok(await Result<List<HrEmployeeVw>>.FailAsync(items.Status.message));

                if (items.Data.Count() > 0)
                {
                    var paginatedData = new PaginatedResult<object>
                    {
                        Succeeded = items.Succeeded,
                        Data = items.Data,
                        Status = items.Status,
                        PaginationInfo = items.PaginationInfo
                    };
                    return Ok(paginatedData);
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        [HttpGet("ManagerIdChanged")]
        public async Task<IActionResult> ManagerIdChanged(string EmpId)
        {
            var chk = await permission.HasPermission(831, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (string.IsNullOrEmpty(EmpId))
            {
                return Ok(await Result<EmpManagerIdChangedVM>.SuccessAsync("there is no id passed"));
            }

            try
            {
                var checkEmpId = await mainServiceManager.InvestEmployeeService.GetOne(i => i.EmpId == EmpId && i.Isdel == false);
                if (checkEmpId.Succeeded)
                {
                    if (checkEmpId.Data != null)
                    {
                        var item = new EmpManagerIdChangedVM
                        {
                            EmpId = checkEmpId.Data.EmpId,
                            EmpName = checkEmpId.Data.EmpName,
                        };
                        return Ok(await Result<EmpManagerIdChangedVM>.SuccessAsync(item));

                    }
                    else
                    {
                        return Ok(await Result<EmpManagerIdChangedVM>.SuccessAsync(localization.GetResource1("EmployeeNotFound")));

                    }
                }
                return Ok(await Result<EmpManagerIdChangedVM>.FailAsync($"{checkEmpId.Status.message}"));
            }
            catch (Exception exp)
            {
                return Ok(await Result<EmpManagerIdChangedVM>.FailAsync($"{exp.Message}"));
            }
        }


        [HttpPost("AssignManager1")]
        public async Task<IActionResult> AssignManager1(string empCode, List<string> employeesCodes)
        {
            var chk = await permission.HasPermission(831, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (string.IsNullOrEmpty(empCode))
            {
                return Ok(await Result<EmpManagerIdChangedVM>.SuccessAsync(localization.GetMessagesResource("DirectManagerNotFound")));
            }
            if (!employeesCodes.Any())
            {
                return Ok(await Result<EmpManagerIdChangedVM>.SuccessAsync(localization.GetMessagesResource("PleaseSelectAtLeastOneEmployee")));
            }

            try
            {
                var checkEmpId = await mainServiceManager.InvestEmployeeService.ChangeEmployeeManager1(empCode, employeesCodes);

                return Ok(checkEmpId);
            }
            catch (Exception exp)
            {
                return Ok(await Result<EmpManagerIdChangedVM>.FailAsync($"{exp.Message}"));
            }
        }

        [HttpPost("AssignManager2")]
        public async Task<IActionResult> AssignManager2(string empCode, List<string> employeesCodes)
        {
            var chk = await permission.HasPermission(831, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (string.IsNullOrEmpty(empCode))
            {
                return Ok(await Result<EmpManagerIdChangedVM>.SuccessAsync("there is no id passed"));
            }
            if (!employeesCodes.Any())
            {
                return Ok(await Result<EmpManagerIdChangedVM>.SuccessAsync("there is no Employee Selected"));
            }

            try
            {
                var checkEmpId = await mainServiceManager.InvestEmployeeService.ChangeEmployeeManager2(empCode, employeesCodes);

                return Ok(checkEmpId);
            }
            catch (Exception exp)
            {
                return Ok(await Result<EmpManagerIdChangedVM>.FailAsync($"{exp.Message}"));
            }
        }

        [HttpPost("AssignManager3")]
        public async Task<IActionResult> AssignManager3(string empCode, List<string> employeesCodes)
        {
            var chk = await permission.HasPermission(831, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (string.IsNullOrEmpty(empCode))
            {
                return Ok(await Result<EmpManagerIdChangedVM>.SuccessAsync("there is no id passed"));
            }
            if (!employeesCodes.Any())
            {
                return Ok(await Result<EmpManagerIdChangedVM>.SuccessAsync("there is no Employee Selected"));
            }

            try
            {
                var checkEmpId = await mainServiceManager.InvestEmployeeService.ChangeEmployeeManager3(empCode, employeesCodes);

                return Ok(checkEmpId);
            }
            catch (Exception exp)
            {
                return Ok(await Result<EmpManagerIdChangedVM>.FailAsync($"{exp.Message}"));
            }
        }

    }
}