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
    //  الاجازات التعويضية
    public class HRCompensatoryVacationController : BaseHrApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;

        public HRCompensatoryVacationController(IHrServiceManager hrServiceManager,
            IMainServiceManager mainServiceManager,
            IPermissionHelper permission,
            ICurrentData session,
            ILocalizationService localization)
        {
            this.mainServiceManager = mainServiceManager;
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
        }


        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrVacationsFilterDto filter)
        {
            var chk = await permission.HasPermission(1788, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                filter.BranchId ??= 0; filter.VacationTypeId ??= 0; filter.LocationId ??= 0;
                var BranchesList = session.Branches.Split(',');
                List<HrVacationsFilterDto> resultList = new List<HrVacationsFilterDto>();
                var items = await hrServiceManager.HrCompensatoryVacationService.GetAllVW(e => e.IsDeleted == false
                && BranchesList.Contains(e.BranchId.ToString())
                && (filter.BranchId == 0 || e.BranchId == filter.BranchId)
                && (filter.VacationTypeId == 0 || e.VacationTypeId == filter.VacationTypeId)
                && (filter.LocationId == 0 || e.Location == filter.LocationId)
                //&& (filter.DeptId == 0 || e.DeptId == filter.DeptId)
                && (string.IsNullOrEmpty(filter.EmpCode) || e.EmpCode == filter.EmpCode)
                );
                if (items.Succeeded)
                {
                    var res = items.Data.OrderByDescending(x => x.VacationSdate).AsQueryable();

                    if (!string.IsNullOrEmpty(filter.StartDate) && !string.IsNullOrEmpty(filter.EndDate))
                    {
                        DateTime startDate = DateHelper.StringToDate(filter.StartDate);
                        DateTime endDate = DateHelper.StringToDate(filter.EndDate);
                        res = res.Where(r => r.VacationSdate != null && r.VacationEdate != null &&
                        (DateHelper.StringToDate(r.VacationSdate) >= startDate && DateHelper.StringToDate(r.VacationSdate) <= endDate) &&
                        (DateHelper.StringToDate(r.VacationEdate) >= startDate && DateHelper.StringToDate(r.VacationEdate) <= endDate));
                    }
                    return Ok(await Result<List<HrCompensatoryVacationsVw>>.SuccessAsync(res.ToList(), ""));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrVacationsFilterDto>.FailAsync(ex.Message));
            }
        }
        [HttpPost("GetPagination")]
        public async Task<IActionResult> GetPagination([FromBody] HrVacationsFilterDto filter, int take = Pagination.take, long? lastSeenId = null)
        {
            var chk = await permission.HasPermission(1788, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                filter.BranchId ??= 0;
                filter.VacationTypeId ??= 0;
                filter.LocationId ??= 0;

                var branchesList = session.Branches.Split(',');

                // إعداد شروط التواريخ إذا كانت موجودة
                List<DateCondition>? dateConditions = null;
                if (!string.IsNullOrEmpty(filter.StartDate) && !string.IsNullOrEmpty(filter.EndDate))
                {
                    dateConditions = new List<DateCondition>
        {
            new DateCondition
            {
                DatePropertyName = "VacationSdate",
                ComparisonOperator = ComparisonOperator.GreaterThanOrEqual,
                StartDateString = filter.StartDate
            },
            new DateCondition
            {
                DatePropertyName = "VacationEdate",
                ComparisonOperator = ComparisonOperator.LessThanOrEqual,
                StartDateString = filter.EndDate
            }
        };
                }

                // نفس الشروط من Search لكن مع GetAllWithPaginationVW
                var items = await hrServiceManager.HrCompensatoryVacationService.GetAllWithPaginationVW(
                    selector: e => e.CompensatoryId,
                    expression: e => e.IsDeleted == false
                        && branchesList.Contains(e.BranchId.ToString())
                        && (filter.BranchId == 0 || e.BranchId == filter.BranchId)
                        && (filter.VacationTypeId == 0 || e.VacationTypeId == filter.VacationTypeId)
                        && (filter.LocationId == 0 || e.Location == filter.LocationId)
                        && (string.IsNullOrEmpty(filter.EmpCode) || e.EmpCode == filter.EmpCode),
                    take: take,
                    lastSeenId: lastSeenId,
                    dateConditions: dateConditions
                );

                if (!items.Succeeded)
                    return Ok(await Result<List<HrCompensatoryVacationsVw>>.FailAsync(items.Status.message));

                if (items.Data == null || !items.Data.Any())
                    return Ok(await Result<List<HrCompensatoryVacationsVw>>.SuccessAsync(new List<HrCompensatoryVacationsVw>()));

                var res = items.Data.OrderByDescending(x => x.VacationSdate).ToList();

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
                return Ok(await Result<HrVacationsFilterDto>.FailAsync(ex.Message));
            }


        }


        [HttpPost("Add")]
        public async Task<ActionResult> Add(HrCompensatoryVacationAddDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(1788, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                if (string.IsNullOrEmpty(obj.EmpCode)) return Ok(await Result<object>.FailAsync(localization.GetResource1("EmployeeIsNumber")));

                var add = await hrServiceManager.HrCompensatoryVacationService.AddNewHrCompensatoryVacation(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add Hr Compensatory Vacation Service Controller, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(HrCompensatoryVacationEditDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(1788, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result<HrCompensatoryVacationEditDto>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));
                if (string.IsNullOrEmpty(obj.EmpCode)) return Ok(await Result<object>.FailAsync(localization.GetResource1("EmployeeIsNumber")));

                var update = await hrServiceManager.HrCompensatoryVacationService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrCompensatoryVacationEditDto>.FailAsync($"====== Exp in Hr Compensatory Vacation Controller getById, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long Id)
        {
            try
            {
                var chk = await permission.HasPermission(1788, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var item = await hrServiceManager.HrCompensatoryVacationService.GetOneVW(x => x.CompensatoryId == Id);
                var files = await mainServiceManager.SysFileService.GetAll(x => x.IsDeleted == false && x.TableId == 119 && x.PrimaryKey == Id);

                return Ok(await Result<object>.SuccessAsync(new { data = item.Data, fileDtos = files.Data.ToList() }));
                //return Ok(item);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrNoteEditDto>.FailAsync($"====== Exp in Hr Compensatory Vacation Controller getById, MESSAGE: {ex.Message}"));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var chk = await permission.HasPermission(1788, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await hrServiceManager.HrCompensatoryVacationService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete Hr CompensatoryVacation Controller, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("CompensatoryVacationApprove")]
        public async Task<IActionResult> CompensatoryVacationApprove(long Id)
        {
            try
            {
                var chk = await permission.HasPermission(1788, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var item = await hrServiceManager.HrCompensatoryVacationService.CompensatoryVacationApprove(Id);
                return Ok(item);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrNoteEditDto>.FailAsync($"====== Exp in Hr Compensatory Vacation Controller getById, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("GetDaysClick")]
        public async Task<IActionResult> GetDaysClick(string SDate, string EDate, int VacationTypeId)
        {
            try
            {
                var chk = await permission.HasPermission(1788, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (string.IsNullOrEmpty(SDate) || string.IsNullOrEmpty(EDate))
                    return Ok(await Result.FailAsync("يجب ادخال تاريخ بداية ونهاية الاجازة "));

                var getCount = await hrServiceManager.HrCompensatoryVacationService.GetVacationDaysCount(SDate, EDate, VacationTypeId);
                return Ok(getCount);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp ::::: in  GetDaysClick in HrCompensatoryVacationController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpDelete("DeleteFile")]
        public async Task<IActionResult> DeleteFile(long Id = 0)
        {
            var chk = await permission.HasPermission(1788, PermissionType.Delete);
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
                var del = await mainServiceManager.SysFileService.Remove(Id);

                return Ok(del);
            }
            catch (Exception exp)
            {
                return Ok(await Result.FailAsync($"{exp.Message}"));
            }
        }
    }
}