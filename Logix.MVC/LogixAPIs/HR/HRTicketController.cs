using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Helpers;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.HR
{
    //   التذاكر 
    public class HRTicketController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IMainServiceManager mainServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ILocalizationService localization;

        public HRTicketController(IHrServiceManager hrServiceManager,
            IMainServiceManager mainServiceManager,
            IPermissionHelper permission,
            ILocalizationService localization)
        {
            this.hrServiceManager = hrServiceManager;
            this.mainServiceManager = mainServiceManager;
            this.permission = permission;
            this.localization = localization;
        }


        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrTicketFilterDto filter)
        {
            try
            {
                var chk = await permission.HasPermission(594, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                filter.IsBillable ??= 0; filter.DeptId ??= 0; filter.LocationId ??= 0; filter.BranchId ??= 0;
                var items = await hrServiceManager.HrTicketService.GetAllVW(x => x.IsDeleted == false
                && (string.IsNullOrEmpty(filter.EmpCode) || x.EmpCode == filter.EmpCode)
                && (string.IsNullOrEmpty(filter.TicketNo) || (x.TicketNo != null && x.TicketNo.Contains(filter.TicketNo)))
                && (filter.IsBillable == 0 || (x.IsBillable == filter.IsBillable))
                && (filter.DeptId == 0 || (x.DeptId == filter.DeptId))
                && (filter.LocationId == 0 || (x.Location == filter.LocationId))
                && (filter.BranchId == 0 || (x.BranchId == filter.BranchId)));

                if (items.Succeeded)
                {
                    var res = items.Data.OrderBy(x => x.EmpId).AsQueryable();
                    if (!string.IsNullOrEmpty(filter.FromDate) && !string.IsNullOrEmpty(filter.ToDate))
                    {
                        DateTime fromDate = DateHelper.StringToDate(filter.FromDate);
                        DateTime toDate = DateHelper.StringToDate(filter.ToDate);
                        res = res.Where(r => r.TicketDate != null &&
                        (DateHelper.StringToDate(r.TicketDate) >= fromDate) && (DateHelper.StringToDate(r.TicketDate) <= toDate));
                    }
                    return Ok(await Result<List<HrTicketVw>>.SuccessAsync([.. res], ""));
                }
                return Ok(await Result<HrTicketFilterDto>.FailAsync(items.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrVacationsFilterDto>.FailAsync(ex.Message));
            }
        }

        [HttpPost("GetPagination")]
        public async Task<IActionResult> GetPagination([FromBody] HrTicketFilterDto filter, int take = Pagination.take, long? lastSeenId = null)
        {
            try
            {
                var chk = await permission.HasPermission(594, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));


                filter.IsBillable ??= 0;
                filter.DeptId ??= 0;
                filter.LocationId ??= 0;
                filter.BranchId ??= 0;

                // إعداد شروط التواريخ
                List<DateCondition>? dateConditions = null;
                if (!string.IsNullOrEmpty(filter.FromDate) && !string.IsNullOrEmpty(filter.ToDate))
                {
                    dateConditions = new List<DateCondition>
        {
            new DateCondition
            {
                DatePropertyName = "TicketDate",
                ComparisonOperator = ComparisonOperator.GreaterThanOrEqual,
                StartDateString = filter.FromDate
            },
            new DateCondition
            {
                DatePropertyName = "TicketDate",
                ComparisonOperator = ComparisonOperator.LessThanOrEqual,
                StartDateString = filter.ToDate
            }
        };
                }

                var items = await hrServiceManager.HrTicketService.GetAllWithPaginationVW(
                    selector: e => e.Id,
                    expression: x => x.IsDeleted == false
                        && (string.IsNullOrEmpty(filter.EmpCode) || x.EmpCode == filter.EmpCode)
                        && (string.IsNullOrEmpty(filter.TicketNo) || (x.TicketNo != null && x.TicketNo.Contains(filter.TicketNo)))
                        && (filter.IsBillable == 0 || x.IsBillable == filter.IsBillable)
                        && (filter.DeptId == 0 || x.DeptId == filter.DeptId)
                        && (filter.LocationId == 0 || x.Location == filter.LocationId)
                        && (filter.BranchId == 0 || x.BranchId == filter.BranchId),
                    take: take,
                    lastSeenId: lastSeenId,
                    dateConditions: dateConditions
                );

                if (!items.Succeeded)
                    return Ok(await Result<List<HrTicketVw>>.FailAsync(items.Status.message));

                if (items.Data == null || !items.Data.Any())
                    return Ok(await Result<List<HrTicketVw>>.SuccessAsync(new List<HrTicketVw>()));

                var res = items.Data.OrderBy(x => x.EmpId).ToList();

                var paginatedData = new PaginatedResult<object>
                {
                    Succeeded = items.Succeeded,
                    Data = res,
                    Status = items.Status,
                    PaginationInfo = items.PaginationInfo
                };

                return Ok(paginatedData);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrTicketFilterDto>.FailAsync(ex.Message));
            }


        }


        [HttpPost("Add")]
        public async Task<ActionResult> Add(HrTicketDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(594, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (string.IsNullOrEmpty(obj.EmpCode))
                    return Ok(await Result<object>.FailAsync(localization.GetResource1("EmployeeIsNumber")));

                if (string.IsNullOrEmpty(obj.TicketDate))
                    return Ok(await Result<object>.FailAsync(localization.GetCommonResource("Tdate")));

                if (string.IsNullOrEmpty(obj.TicketNo) || obj.TicketNo == "0")
                    return Ok(await Result<object>.FailAsync(localization.GetHrResource("TicketNo")));

                if (obj.TicketCount <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetHrResource("NoOfTickets")));

                if (obj.TicketAmount <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetHrResource("TicketAmount")));

                if (obj.IsBillable <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetHrResource("TicketNo")));

                if (obj.Way <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetHrResource("TripType")));

                if (obj.Cabins <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetHrResource("TripClass")));

                if (string.IsNullOrEmpty(obj.Purpose))
                    return Ok(await Result<object>.FailAsync(localization.GetHrResource("TicketPurpose")));

                var add = await hrServiceManager.HrTicketService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(HrTicketEditDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(594, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (obj.Id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));

                if (string.IsNullOrEmpty(obj.EmpCode))
                    return Ok(await Result<object>.FailAsync(localization.GetResource1("EmployeeIsNumber")));

                if (string.IsNullOrEmpty(obj.TicketDate))
                    return Ok(await Result<object>.FailAsync(localization.GetCommonResource("Tdate")));

                if (string.IsNullOrEmpty(obj.TicketNo) || obj.TicketNo == "0")
                    return Ok(await Result<object>.FailAsync(localization.GetHrResource("TicketNo")));

                if (obj.TicketCount <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetHrResource("NoOfTickets")));

                if (obj.TicketAmount <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetHrResource("TicketAmount")));

                if (obj.IsBillable <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetHrResource("TicketNo")));

                if (obj.Way <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetHrResource("TripType")));

                if (obj.Cabins <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetHrResource("TripClass")));

                if (string.IsNullOrEmpty(obj.Purpose))
                    return Ok(await Result<object>.FailAsync(localization.GetHrResource("TicketPurpose")));

                var update = await hrServiceManager.HrTicketService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrTicketEditDto>.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long Id)
        {
            try
            {
                var chk = await permission.HasPermission(594, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));

                var item = await hrServiceManager.HrTicketService.GetOneVW(x => x.Id == Id);
                if (item.Succeeded == false)
                {
                    return Ok(item);
                }

                var fileDtos = await mainServiceManager.SysFileService.GetAll(x => x.IsDeleted == false && x.TableId == 165 && x.PrimaryKey == Id);
                var result = new
                {
                    item.Data,
                    fileDtos = fileDtos.Data,
                };
                return Ok(await Result<object>.SuccessAsync(result));
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrTicketVw>.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var chk = await permission.HasPermission(594, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await hrServiceManager.HrTicketService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }
    }
}