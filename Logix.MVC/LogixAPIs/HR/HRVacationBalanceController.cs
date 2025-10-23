using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.HR
{
    //     الرصيد الافتتاحي للإجازات
    public class HRVacationBalanceController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData currentData;
        private readonly ILocalizationService localization;

        public HRVacationBalanceController(IHrServiceManager hrServiceManager,
            IPermissionHelper permission,
            ICurrentData currentData,
            ILocalizationService localization)
        {
            this.permission = permission;
            this.currentData = currentData;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
        }


        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrVacationBalanceFilterDto filter)
        {
            try
            {
                var chk = await permission.HasPermission(175, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var items = await hrServiceManager.HrVacationBalanceService.Search(filter);
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrVacationBalanceFilterDto>.FailAsync(ex.Message));
            }
        }
        [HttpPost("GetPagination")]
        public async Task<IActionResult> GetPagination([FromBody] HrVacationBalanceFilterDto filter, int take = Pagination.take, long? lastSeenId = null)
        {
            try
            {
                var chk = await permission.HasPermission(175, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var branchesList = currentData.Branches.Split(',');


                filter.BranchId ??= 0;
                filter.VacationTypeId ??= 0;
                filter.DeptId ??= 0;
                filter.Location ??= 0;

                var items = await hrServiceManager.HrVacationBalanceService.GetAllWithPaginationVW(
                    selector: e => e.VacBalanceId,
                    expression: e => e.IsDeleted == false
                        && (string.IsNullOrEmpty(filter.EmpCode) || e.EmpCode == filter.EmpCode)
                        && (filter.BranchId == 0 || e.BranchId == filter.BranchId)
                        && (filter.VacationTypeId == 0 || e.VacationTypeId == filter.VacationTypeId)
                        && (filter.DeptId == 0 || e.DeptId == filter.DeptId)
                        && (filter.Location == 0 || e.Location == filter.Location)
                        && branchesList.Contains(e.BranchId.ToString()),
                    take: take,
                    lastSeenId: lastSeenId
                );

                if (!items.Succeeded)
                    return Ok(await Result<List<HrVacationBalanceFilterDto>>.FailAsync(items.Status.message));

                if (items.Data == null || !items.Data.Any())
                    return Ok(await Result<List<HrVacationBalanceFilterDto>>.SuccessAsync(new List<HrVacationBalanceFilterDto>()));

                var resultList = items.Data.Select(item => new HrVacationBalanceFilterDto
                {
                    Id = item.VacBalanceId,
                    EmpCode = item.EmpCode,
                    EmpName = item.EmpName,
                    EmpName2 = item.EmpName2,
                    LocationName = item.LocationName,
                    LocationName2 = item.LocationName2,
                    DepName = item.DepName,
                    DepName2 = item.DepName2,
                    BraName = item.BraName,
                    BraName2 = item.BraName2,
                    StartDate = item.StartDate,
                    VacationBalance = item.VacationBalance,
                    Note = item.Note,
                    VacationTypeName = item.VacationTypeName,
                    VacationTypeName2 = item.VacationTypeName2
                }).ToList();

                var paginatedData = new PaginatedResult<object>
                {
                    Succeeded = items.Succeeded,
                    Data = resultList,
                    Status = items.Status,
                    PaginationInfo = items.PaginationInfo
                };

                return Ok(paginatedData);
            }
            catch (Exception ex)
            {
                return Ok(await Result<List<HrVacationBalanceFilterDto>>.FailAsync(ex.Message));
            }


        }


        // شاشة - تقرير بأرصدة الاجازات
        [HttpPost("AllBalanceSearch")]
        public async Task<IActionResult> AllBalanceSearch(HrVacationBalanceALLSendFilterDto filter)
        {
            try
            {
                var chk = await permission.HasPermission(557, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var items = await hrServiceManager.HrVacationBalanceService.VacationBalanceALL(filter);
                if (items.Succeeded)
                {
                    if (items.Data.Count() > 0)
                    {
                        return Ok(await Result<List<HrVacationBalanceALLFilterDto>>.SuccessAsync(items.Data.ToList(), ""));
                    }
                    return Ok(await Result<List<HrVacationBalanceALLFilterDto>>.SuccessAsync(items.Data.ToList(), localization.GetResource1("NosearchResult")));
                }
                return Ok(await Result<HrVacationBalanceALLFilterDto>.FailAsync(items.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrVacationBalanceALLFilterDto>.FailAsync(ex.Message));
            }
        }

        // شاشة - رصيد إجازة موظف
        [HttpPost("EmpBalanceSearch")]
        public async Task<IActionResult> EmpBalanceSearch(HrVacationEmpBalanceDto filter)
        {
            try
            {
                var chk = await permission.HasPermission(235, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var items = await hrServiceManager.HrVacationBalanceService.VacationEmpBalanceSearch(filter);
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrVacationEmpBalanceDto>.FailAsync(ex.Message));
            }
        }


        [HttpPost("Add")]
        public async Task<ActionResult> Add(HrVacationBalanceDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(175, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var add = await hrServiceManager.HrVacationBalanceService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(HrVacationBalanceEditDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(175, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result<HrVacationBalanceEditDto>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var update = await hrServiceManager.HrVacationBalanceService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrVacationBalanceEditDto>.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long Id)
        {
            try
            {
                var chk = await permission.HasPermission(175, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var item = await hrServiceManager.HrVacationBalanceService.GetOneVW(x => x.VacBalanceId == Id);
                return Ok(item);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrVacationBalanceVw>.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var chk = await permission.HasPermission(175, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await hrServiceManager.HrVacationBalanceService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }
    }
}