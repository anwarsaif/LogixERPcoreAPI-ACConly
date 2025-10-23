using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.HR.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.HR
{
    // انواع الاجازات
    public class HrVacationsTypeApiController : BaseHrApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        public HrVacationsTypeApiController(IHrServiceManager hrServiceManager, IMainServiceManager mainServiceManager, ICurrentData session, IPermissionHelper permission, ILocalizationService localization)
        {
            this.mainServiceManager = mainServiceManager;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.permission = permission;
            this.localization = localization;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var chk = await permission.HasPermission(554, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var items = await hrServiceManager.HrVacationsTypeService.GetAll(e => e.IsDeleted == false);
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    return Ok(items);
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
        [HttpPost("SearchVacationsType")]
        public async Task<IActionResult> GetAllSearch(HrVacationsTypeFilterVM filter)
        {
            var chk = await permission.HasPermission(554, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var items = await hrServiceManager.HrVacationsTypeService.GetAll(e => e.IsDeleted == false
                                && (string.IsNullOrEmpty(filter.VacationTypeName) || (e.VacationTypeName != null && e.VacationTypeName.ToLower().Contains(filter.VacationTypeName.ToLower()) || (e.VacationTypeName2 != null && e.VacationTypeName2.ToLower().Contains(filter.VacationTypeName.ToLower()))
                                ))

                );
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    if (filter == null)
                    {
                        return Ok(items);
                    }

                    res = res.OrderBy(e => e.VacationTypeId);
                    var final = res.ToList();
                    return Ok(await Result<List<HrVacationsTypeDto>>.SuccessAsync(final, ""));
                }

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }


        [HttpPost("GetPagination")]
        public async Task<IActionResult> GetPagination([FromBody] HrVacationsTypeFilterVM filter, int take = Pagination.take, int? lastSeenId = null)
        {
            var chk = await permission.HasPermission(554, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var items = await hrServiceManager.HrVacationsTypeService.GetAllWithPaginationVW(
                    selector: e => e.VacationTypeId,
                    expression: e =>
                        e.IsDeleted == false &&
                        (
                            string.IsNullOrEmpty(filter.VacationTypeName)
                            || (e.VacationTypeName != null && e.VacationTypeName.ToLower().Contains(filter.VacationTypeName.ToLower()))
                            || (e.VacationTypeName2 != null && e.VacationTypeName2.ToLower().Contains(filter.VacationTypeName.ToLower()))
                        ),
                    take: take,
                    lastSeenId: lastSeenId
                );

                if (!items.Succeeded)
                    return Ok(await Result<List<HrVacationsTypeDto>>.FailAsync(items.Status.message));

                if (items.Data == null || !items.Data.Any())
                    return Ok(await Result<List<HrVacationsTypeDto>>.SuccessAsync(new List<HrVacationsTypeDto>()));

                var res = items.Data.OrderBy(x => x.VacationTypeId).ToList();

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
        public async Task<IActionResult> Add(HrVacationsTypeDto obj)
        {
            var chk = await permission.HasPermission(554, PermissionType.Add);
            if (!chk)
            {

                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (!ModelState.IsValid)
            {
                return Ok(await Result<HrVacationsTypeDto>.FailAsync($"يجب ادخال كل البيانات بشكل صحيح"));

            }

            try
            {

                var addRes = await hrServiceManager.HrVacationsTypeService.Add(obj);
                return Ok(addRes);
            }

            catch (Exception ex)
            {
                return Ok(await Result<HrVacationsTypeDto>.FailAsync($"======= Exp in Add HrVacationsTypeDto, MESSAGE: {ex.Message}"));
            }
        }
        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(int Id)
        {
            var chk = await permission.HasPermission(554, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (Id <= 0)
            {
                return Ok(await Result<HrVacationsTypeDto>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
            }

            try
            {
                var getItem = await hrServiceManager.HrVacationsTypeService.GetForUpdate<HrVacationsTypeEditDto>(Id);
                if (getItem.Succeeded && getItem.Data != null)
                {
                    return Ok(getItem);
                }
                return Ok(await Result<HrVacationsTypeDto>.FailAsync("NoIdInUpdate"));
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrVacationsTypeDto>.FailAsync(ex.Message));
            }
        }

        [HttpGet("GetVacationPoliciesByVacationsTypeId")]
        public async Task<IActionResult> GetVacationPoliciesByVacationsTypeId(int Id)
        {
            var chk = await permission.HasPermission(1145, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (Id <= 0)
            {
                return Ok(await Result<HrVacationsTypeDto>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
            }

            try
            {
                var getItem = await hrServiceManager.HrVacationsTypeService.GetForUpdate<HrVacationsTypeEditDto>(Id);
                if (getItem.Succeeded && getItem.Data != null)
                {
                    return Ok(getItem);
                }
                return Ok(await Result<HrVacationsTypeDto>.FailAsync("NoIdInUpdate"));
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrVacationsTypeDto>.FailAsync(ex.Message));
            }
        }

        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(HrVacationsTypeEditDto obj)
        {
            var chk = await permission.HasPermission(554, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (!ModelState.IsValid)
            {
                return Ok(obj);
            }
            try
            {
                var addRes = await hrServiceManager.HrVacationsTypeService.Update(obj);
                if (addRes.Succeeded)
                {
                    return Ok(await Result<HrVacationsTypeEditDto>.SuccessAsync(addRes.Data));
                }

                else
                {
                    return Ok(await Result<HrVacationsTypeEditDto>.FailAsync($"{addRes.Status.message}"));
                }
            }

            catch (Exception ex)
            {
                return Ok(await Result<HrVacationsTypeEditDto>.FailAsync(ex.Message));
            }
        }

        [HttpPost("AddEditVacationPolicies")]
        public async Task<IActionResult> AddEditVacationPolicies(HrVacationsTypeEditVacationPoliciesDto obj)
        {
            var chk = await permission.HasPermission(1145, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (!ModelState.IsValid)
            {
                return Ok(obj);
            }
            try
            {
                var addRes = await hrServiceManager.HrVacationsTypeService.AddEditVacationPolicies(obj);
                if (addRes.Succeeded)
                {
                    return Ok(await Result<HrVacationsTypeEditDto>.SuccessAsync(addRes.Data));
                }

                else
                {
                    return Ok(await Result<HrVacationsTypeEditDto>.FailAsync($"{addRes.Status.message}"));
                }
            }

            catch (Exception ex)
            {
                return Ok(await Result<HrVacationsTypeEditDto>.FailAsync(ex.Message));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(int Id = 0)
        {
            var chk = await permission.HasPermission(554, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (Id == 0)
            {
                return Ok(await Result<object>.FailAsync("Please choose an entity to delete it, there is no id passed"));
            }

            try
            {
                var del = await hrServiceManager.HrVacationsTypeService.Remove(Id);
                if (del.Succeeded)
                {

                    return Ok(await Result<object>.SuccessAsync("Item deleted successfully"));
                }
                return Ok(await Result<object>.FailAsync($"{del.Status.message}"));
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync(ex.Message));
            }
        }

    }


}