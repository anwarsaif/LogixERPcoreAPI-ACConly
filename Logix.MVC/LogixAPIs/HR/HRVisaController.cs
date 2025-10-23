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
    // التأشيرات
    public class HRVisaController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IMainServiceManager mainServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ILocalizationService localization;

        public HRVisaController(IHrServiceManager hrServiceManager,
            IMainServiceManager mainServiceManager,
            IPermissionHelper permission,
            ILocalizationService localization)
        {
            this.permission = permission;
            this.hrServiceManager = hrServiceManager;
            this.mainServiceManager = mainServiceManager;
            this.localization = localization;
        }


        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrVisaFilterDto filter)
        {
            try
            {
                var chk = await permission.HasPermission(595, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                filter.VisaType ??= 0; filter.VisaDays ??= 0; filter.DeptId ??= 0; filter.Location ??= 0; filter.BranchId ??= 0; filter.DateType ??= 0;
                var items = await hrServiceManager.HrVisaService.GetAllVW(e => e.IsDeleted == false
                && (string.IsNullOrEmpty(filter.EmpCode) || e.EmpCode == filter.EmpCode)
                && (filter.VisaType == 0 || e.VisaType == filter.VisaType)
                && (filter.VisaDays == 0 || e.VisaDays == filter.VisaDays)
                && (filter.DeptId == 0 || e.DeptId == filter.DeptId)
                && (filter.Location == 0 || e.Location == filter.Location)
                && (filter.BranchId == 0 || e.BranchId == filter.BranchId));
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();

                    if (!string.IsNullOrEmpty(filter.FromDate) && !string.IsNullOrEmpty(filter.ToDate))
                    {
                        DateTime fromDate = DateHelper.StringToDate(filter.FromDate);
                        DateTime toDate = DateHelper.StringToDate(filter.ToDate);
                        switch (filter.DateType)
                        {
                            case 1:
                                res = res.Where(r => r.VisaDate != null &&
                                (DateHelper.StringToDate(r.VisaDate) >= fromDate) && (DateHelper.StringToDate(r.VisaDate) <= toDate));
                                break;
                            case 2:
                                res = res.Where(r => r.StartDate != null &&
                                (DateHelper.StringToDate(r.StartDate) >= fromDate) && (DateHelper.StringToDate(r.StartDate) <= toDate));
                                break;
                            case 3:
                                res = res.Where(r => r.EndDate != null &&
                                (DateHelper.StringToDate(r.EndDate) >= fromDate) && (DateHelper.StringToDate(r.EndDate) <= toDate));
                                break;
                            default:
                                break;
                        }
                    }
                    res = res.OrderBy(x => x.EmpId);
                    return Ok(await Result<List<HrVisaVw>>.SuccessAsync([.. res], ""));
                }
                return Ok(await Result<HrVisaFilterDto>.FailAsync(items.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrVisaFilterDto>.FailAsync(ex.Message));
            }
        }
        [HttpPost("GetPagination")]
        public async Task<IActionResult> GetPagination([FromBody] HrVisaFilterDto filter, int take = Pagination.take, long? lastSeenId = null)
        {
            try
            {
                var chk = await permission.HasPermission(595, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                filter.VisaType ??= 0;
                filter.VisaDays ??= 0;
                filter.DeptId ??= 0;
                filter.Location ??= 0;
                filter.BranchId ??= 0;
                filter.DateType ??= 0;

                // إعداد شروط التواريخ حسب نوع التاريخ المطلوب
                List<DateCondition>? dateConditions = null;
                if (!string.IsNullOrEmpty(filter.FromDate) && !string.IsNullOrEmpty(filter.ToDate))
                {
                    string? dateField = filter.DateType switch
                    {
                        1 => "VisaDate",
                        2 => "StartDate",
                        3 => "EndDate",
                        _ => null
                    };

                    if (!string.IsNullOrEmpty(dateField))
                    {
                        dateConditions = new List<DateCondition>
            {
                new DateCondition
                {
                    DatePropertyName = dateField,
                    ComparisonOperator = ComparisonOperator.GreaterThanOrEqual,
                    StartDateString = filter.FromDate
                },
                new DateCondition
                {
                    DatePropertyName = dateField,
                    ComparisonOperator = ComparisonOperator.LessThanOrEqual,
                    StartDateString = filter.ToDate
                }
            };
                    }
                }

                var items = await hrServiceManager.HrVisaService.GetAllWithPaginationVW(
                    selector: e => e.Id,
                    expression: e => e.IsDeleted == false
                        && (string.IsNullOrEmpty(filter.EmpCode) || e.EmpCode == filter.EmpCode)
                        && (filter.VisaType == 0 || e.VisaType == filter.VisaType)
                        && (filter.VisaDays == 0 || e.VisaDays == filter.VisaDays)
                        && (filter.DeptId == 0 || e.DeptId == filter.DeptId)
                        && (filter.Location == 0 || e.Location == filter.Location)
                        && (filter.BranchId == 0 || e.BranchId == filter.BranchId),
                    take: take,
                    lastSeenId: lastSeenId,
                    dateConditions: dateConditions
                );

                if (!items.Succeeded)
                    return Ok(await Result<List<HrVisaVw>>.FailAsync(items.Status.message));

                if (items.Data == null || !items.Data.Any())
                    return Ok(await Result<List<HrVisaVw>>.SuccessAsync(new List<HrVisaVw>()));

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
                return Ok(await Result<HrVisaFilterDto>.FailAsync(ex.Message));
            }


        }

        [HttpPost("Add")]
        public async Task<ActionResult> Add(HrVisaDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(595, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid || obj.VisaType <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var add = await hrServiceManager.HrVisaService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(HrVisaEditDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(595, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid || obj.VisaType <= 0)
                    return Ok(await Result<HrVisaEditDto>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var update = await hrServiceManager.HrVisaService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrVisaEditDto>.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long Id)
        {
            try
            {
                var chk = await permission.HasPermission(595, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var item = await hrServiceManager.HrVisaService.GetForUpdate<HrVisaEditDto>(Id);
                if (item.Succeeded == false)
                {
                    return Ok(item);
                }

                var fileDtos = await mainServiceManager.SysFileService.GetAll(x => x.IsDeleted == false && x.TableId == 164 && x.PrimaryKey == Id);
                var getEmp = await mainServiceManager.InvestEmployeeService.GetOne(x => x.EmpId, x => x.Id == item.Data.EmpId);
                item.Data.EmpCode = getEmp.Data;
                return Ok(await Result<object>.SuccessAsync(new { item.Data, fileDtos = fileDtos.Data }));
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
                var chk = await permission.HasPermission(595, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await hrServiceManager.HrVisaService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }
    }
}