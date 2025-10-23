using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.DTOs.Main;
using Logix.Application.Helpers;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.HR.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.HR
{
    //  مباشرة عمل
    public class HRJoinWorkController : BaseHrApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;

        public HRJoinWorkController(IHrServiceManager hrServiceManager, IMainServiceManager mainServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization)
        {
            this.mainServiceManager = mainServiceManager;
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
        }

        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrDirectJobFilterDto filter)
        {
            var chk = await permission.HasPermission(434, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var items = await hrServiceManager.HrDirectJobService.Search(filter);
                return Ok(items);

            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        [HttpPost("GetPagination")]
        public async Task<IActionResult> GetPagination([FromBody] HrDirectJobFilterDto filter, int take = Pagination.take, long? lastSeenId = null)
        {
            var chk = await permission.HasPermission(434, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {

                var BranchesList = session.Branches.Split(',');

                var dateConditions = new List<DateCondition>();
                dateConditions.Add(new DateCondition
                {
                    DatePropertyName = "DateDirect",
                    ComparisonOperator = ComparisonOperator.GreaterThanOrEqual,
                    StartDateString = filter.From ?? ""
                });
                dateConditions.Add(new DateCondition
                {
                    DatePropertyName = "DateDirect",
                    ComparisonOperator = ComparisonOperator.LessThanOrEqual,
                    StartDateString = filter.To ?? ""
                });


                filter.BranchId ??= 0;
                filter.LocationId ??= 0;
                filter.DeptId ??= 0;
                var items = await hrServiceManager.HrDirectJobService.GetAllWithPaginationVW(selector: e => e.Id,
                expression: e =>
                        e.IsDeleted == false &&
                (string.IsNullOrEmpty(filter.EmpId) || e.EmpCode == filter.EmpId) &&
                (string.IsNullOrEmpty(filter.EmpName) || e.EmpName.Contains(filter.EmpName)) &&
                (filter.BranchId == 0 || e.BranchId == filter.BranchId) &&
                (filter.LocationId == 0 || e.Location == filter.LocationId) &&
                (filter.DeptId == 0 || e.DeptId == filter.DeptId) &&
                (BranchesList == null || BranchesList.Contains(e.BranchId.ToString())),
                    take: take,
                    lastSeenId: lastSeenId,
                   dateConditions: (string.IsNullOrEmpty(filter.From) || string.IsNullOrEmpty(filter.To)) ? null : dateConditions);

                if (!items.Succeeded)
                    return Ok(await Result<List<HrDirectJobVw>>.FailAsync(items.Status.message));
                if (items.Data.Count() > 0)
                {
                    var res = items.Data.AsQueryable();
                    var lang = session.Language;
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

        [HttpPost("Add")]
        public async Task<ActionResult> Add(HrDirectJobDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(434, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var add = await hrServiceManager.HrDirectJobService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add HRJoinWorkController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var chk = await permission.HasPermission(434, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await hrServiceManager.HrDirectJobService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete HRJoinWorkController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(HrDirectJobEditDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(434, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result<HrDirectJobEditDto>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var update = await hrServiceManager.HrDirectJobService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrDirectJobEditDto>.FailAsync($"====== Exp in Edit HrDirectJobController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long Id)
        {
            try
            {
                var chk = await permission.HasPermission(434, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var item = await hrServiceManager.HrDirectJobService.GetOneVW(x => x.Id == Id);
                if (!item.Succeeded)
                    return Ok(item);

                // الملفات
                var fileDtos = new List<SaveFileDto>();
                var getFiles = await mainServiceManager.SysFileService.GetFilesForUser(Id, 152);

                var entity = item.Data as HrDirectJobVw;

                var response = new
                {
                    entity.Id,
                    entity.EmpId,
                    entity.Date1,
                    entity.DateDirect,
                    entity.Note,
                    entity.IsDeleted,
                    entity.EmpName,
                    entity.EmpCode,
                    entity.TypeId,
                    entity.TypeName,
                    entity.VacationId,
                    entity.VacationTypeName,
                    entity.VacationSdate,
                    entity.VacationEdate,
                    entity.VacationAccountDay,
                    entity.BranchId,
                    entity.BraName,
                    entity.DeptId,
                    entity.Location,
                    entity.DepName,
                    entity.LocationName,
                    entity.DepName2,
                    entity.LocationName2,
                    entity.BraName2,
                    entity.EmpName2,
                    fileDtos = getFiles
                };

                return Ok(await Result<object>.SuccessAsync(response));

            }
            catch (Exception ex)
            {
                return Ok(await Result<HrDirectJobEditDto>.FailAsync($"====== Exp in HRJoinWorkController getById, MESSAGE: {ex.Message}"));
            }
        }
        [HttpGet("EmpCodeChanged")]
        public async Task<IActionResult> EmpCodeChanged(string EmpId)
        {

            try
            {
                var chk = await permission.HasPermission(434, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (string.IsNullOrEmpty(EmpId))
                    return Ok(await Result<object>.SuccessAsync(localization.GetResource1("EmployeeIsNumber")));

                var checkEmpId = await mainServiceManager.InvestEmployeeService.GetOne(i => i.EmpId == EmpId && i.Isdel == false);
                if (!checkEmpId.Succeeded || checkEmpId.Data == null)
                    return Ok(await Result<HrVacationsVw>.FailAsync(localization.GetResource1("EmployeeNotFound")));

                var getvacationsData = await hrServiceManager.HrVacationsService.GetAllVW(x => x.IsDeleted == false && x.NeedJoinRequest == true && (x.VacationRdate == null || x.VacationRdate == "") && x.EmpId == checkEmpId.Data.Id);
                if (!getvacationsData.Succeeded)
                    return Ok(await Result<object>.FailAsync(getvacationsData.Status.message));

                if (!getvacationsData.Data.Any())
                    return Ok(await Result<List<HrVacationsVw>>.SuccessAsync(getvacationsData.Data.ToList(), localization.GetResource1("NosearchResult")));

                var res = getvacationsData.Data.AsQueryable();

                res = res.Where(x => !string.IsNullOrEmpty(x.VacationEdate)).OrderByDescending(x => DateHelper.StringToDate(x.VacationEdate));

                return Ok(await Result<List<HrVacationsVw>>.SuccessAsync(res.ToList(), ""));

            }
            catch (Exception exp)
            {
                return Ok(await Result<HrVacationsVw>.FailAsync($"{exp.Message}"));
            }
        }
    }
}