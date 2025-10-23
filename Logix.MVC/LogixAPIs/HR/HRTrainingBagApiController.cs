using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.HR
{
    public class HRTrainingBagApiController : BaseHrApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;

        public HRTrainingBagApiController(IHrServiceManager hrServiceManager, IMainServiceManager mainServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization)
        {
            this.mainServiceManager = mainServiceManager;
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var chk = await permission.HasPermission(1469, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var items = await hrServiceManager.HrTrainingBagService.GetAllVW(e => e.IsDeleted == false);
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    res = res.OrderBy(e => e.Id);
                    return Ok(await Result<List<HrTrainingBagVw>>.SuccessAsync(res.ToList(), items.Status.message));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
        [HttpPost("SearchTrainingBag")]
        public async Task<IActionResult> GetAllSearch(HrTrainingBagFilterDto filter)
        {
            var chk = await permission.HasPermission(1469, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                filter.TypeId ??= 0;
                var items = await hrServiceManager.HrTrainingBagService.GetAllVW(e => e.IsDeleted == false

                && (filter.TypeId == 0 || e.TypeId == filter.TypeId)
                && (string.IsNullOrEmpty(filter.Name) || (e.Name != null && e.Name == filter.Name || e.Name2 == filter.Name))

                );
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    if (filter == null)
                    {
                        return Ok(items);
                    }
                    res = res.OrderBy(e => e.Id);
                    var final = res.ToList();
                    return Ok(await Result<List<HrTrainingBagVw>>.SuccessAsync(final, ""));
                }

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpPost("GetPagination")]
        public async Task<IActionResult> GetPagination([FromBody] HrTrainingBagFilterDto filter, int take = Pagination.take, int? lastSeenId = null)
        {
            var chk = await permission.HasPermission(1469, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                filter.TypeId ??= 0;

                var items = await hrServiceManager.HrTrainingBagService.GetAllWithPaginationVW(
                    selector: e => e.Id,
                    expression: e =>
                        e.IsDeleted == false &&
                        (filter.TypeId == 0 || e.TypeId == filter.TypeId) &&
                        (string.IsNullOrEmpty(filter.Name) ||
                            (e.Name != null && e.Name.Contains(filter.Name)) ||
                            (e.Name2 != null && e.Name2.Contains(filter.Name))),
                    take: take,
                    lastSeenId: lastSeenId
                );

                if (!items.Succeeded)
                    return Ok(await Result<List<HrTrainingBagVw>>.FailAsync(items.Status.message));

                if (items.Data == null || !items.Data.Any())
                    return Ok(await Result<List<HrTrainingBagVw>>.SuccessAsync(new List<HrTrainingBagVw>()));

                var res = items.Data.OrderBy(x => x.Id).ToList();

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
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add(HrTrainingBagDto obj)
        {
            var chk = await permission.HasPermission(1469, PermissionType.Add);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {

                var addRes = await hrServiceManager.HrTrainingBagService.Add(obj);
                return Ok(addRes);

            }

            catch (Exception ex)
            {
                return Ok(await Result<HrTrainingBagDto>.FailAsync(ex.Message));
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long Id)
        {
            var chk = await permission.HasPermission(1469, PermissionType.Edit);
            if (!chk)
            {
                return Ok(Result<HrTrainingBagEditDto>.FailAsync($"Access Denied"));
            }
            if (Id <= 0)
                return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));


            try
            {
                var getItem = await hrServiceManager.HrTrainingBagService.GetForUpdate<HrTrainingBagEditDto>(Id);
                if (getItem.Succeeded && getItem.Data != null)
                {
                    return Ok(getItem);
                }
                return Ok(Result<HrTrainingBagEditDto>.FailAsync($"No Id InUpdate"));
            }
            catch (Exception exp)
            {
                return Ok(Result<HrTrainingBagEditDto>.FailAsync($"EXP in {this.GetType()}, Meesage: {exp.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(HrTrainingBagEditDto obj)
        {
            var chk = await permission.HasPermission(1469, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var addRes = await hrServiceManager.HrTrainingBagService.Update(obj);

                return Ok(addRes);
            }

            catch (Exception ex)
            {
                return Ok(await Result<HrTrainingBagEditDto>.FailAsync($"{ex.Message}"));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long Id = 0)
        {
            var chk = await permission.HasPermission(1469, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (Id <= 0)
                return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));


            try
            {
                var del = await hrServiceManager.HrTrainingBagService.Remove(Id);
                return Ok(del);

            }
            catch (Exception exp)
            {
                return Ok(await Result.FailAsync($"{exp.Message}"));
            }
        }

    }
}