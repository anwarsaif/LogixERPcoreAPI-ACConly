using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.HR
{
    public class HROccasionsApiController : BaseHrApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly IDDListHelper listHelper;
        private readonly ILocalizationService localization;

        public HROccasionsApiController(IHrServiceManager hrServiceManager, IMainServiceManager mainServiceManager, IPermissionHelper permission, ICurrentData session, IDDListHelper listHelper, ILocalizationService localization)
        {
            this.mainServiceManager = mainServiceManager;
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.listHelper = listHelper;
            this.localization = localization;
        }



        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrNotificationsSettingFilterDto filter)
        {
            var chk = await permission.HasPermission(1472, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {

                filter.Id ??= 0;
                filter.IsActive ??= 0;
                var items = await hrServiceManager.HrNotificationsSettingService.GetAllVW(e =>
                    e.IsDeleted == false &&
                    e.FacilityId == session.FacilityId &&
                    (filter.Id == 0 || e.Id == filter.Id)
                && (string.IsNullOrEmpty(filter.Subject) || (e.Subject != null && e.Subject.Contains(filter.Subject)))
                && (!filter.IsActive.HasValue || filter.IsActive == 0 || e.IsActive == (filter.IsActive.Value == 1))
                );

                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable().OrderBy(e => e.Id).ToList();
                    return Ok(await Result<List<HrNotificationsSettingVw>>.SuccessAsync(res, ""));
                }

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
        [HttpPost("GetPagination")]
        public async Task<IActionResult> GetPagination([FromBody] HrNotificationsSettingFilterDto filter, int take = Pagination.take, int? lastSeenId = null)
        {
            var chk = await permission.HasPermission(1472, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                filter.Id ??= 0;
                filter.IsActive ??= 0;

                var items = await hrServiceManager.HrNotificationsSettingService.GetAllWithPaginationVW(
                    selector: e => e.Id,
                    expression: e =>
                        e.IsDeleted == false &&
                    e.FacilityId == session.FacilityId &&
                    (filter.Id == 0 || e.Id == filter.Id)
                && (string.IsNullOrEmpty(filter.Subject) || (e.Subject != null && e.Subject.Contains(filter.Subject)))
                && (!filter.IsActive.HasValue || filter.IsActive == 0 || e.IsActive == (filter.IsActive.Value == 1))


                            ,
                    take: take,
                    lastSeenId: lastSeenId
                );

                if (!items.Succeeded)
                    return Ok(await Result<List<HrNotificationsSettingVw>>.FailAsync(items.Status.message));

                if (items.Data == null || !items.Data.Any())
                    return Ok(await Result<List<HrNotificationsSettingVw>>.SuccessAsync(new List<HrNotificationsSettingVw>()));

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
        public async Task<IActionResult> Add(HrNotificationsSettingDto obj)
        {
            var chk = await permission.HasPermission(1472, PermissionType.Add);
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
                var addRes = await hrServiceManager.HrNotificationsSettingService.Add(obj);
                if (addRes.Succeeded)
                {
                    return Ok(addRes);
                }

                else
                {
                    return Ok(await Result<HrNotificationsSettingDto>.FailAsync(addRes.Status.message));
                }
            }

            catch (Exception ex)
            {
                return Ok(await Result<HrNotificationsSettingDto>.FailAsync(ex.Message));
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long Id)
        {
            var chk = await permission.HasPermission(1472, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (Id.Equals(null))
            {
                return Ok(Result<HrNotificationsSettingEditDto>.FailAsync(localization.GetMessagesResource("NoIdInUpdate")));
            }

            try
            {
                var getItem = await hrServiceManager.HrNotificationsSettingService.GetForUpdate<HrNotificationsSettingEditDto>(Id);
                if (getItem.Succeeded && getItem.Data != null)
                {
                    var a = getItem.Data.IsActive;
                    if (a == true)
                    {
                        getItem.Data.IsActiveInt = 1;
                    }
                    else if (a == false)
                    {
                        getItem.Data.IsActiveInt = 2;
                    }

                    return Ok(getItem);
                }

                return Ok(Result<HrNotificationsSettingEditDto>.FailAsync(localization.GetMessagesResource("NoIdInUpdate")));
            }
            catch (Exception exp)
            {
                return Ok(Result<HrNotificationsSettingEditDto>.FailAsync($"EXP in {this.GetType()}, Meesage: {exp.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(HrNotificationsSettingEditDto obj)
        {
            var chk = await permission.HasPermission(1472, PermissionType.Edit);
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
                var addRes = await hrServiceManager.HrNotificationsSettingService.Update(obj);
                if (addRes.Succeeded)
                {
                    return Ok(addRes);
                }

                else
                {
                    return Ok(await Result.FailAsync($"{addRes.Status.message}"));
                }
            }

            catch (Exception ex)
            {
                return Ok(await Result<HrNotificationsSettingEditDto>.FailAsync($"{ex.Message}"));
            }
        }


        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long Id = 0)
        {

            var chk = await permission.HasPermission(535, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }


            try
            {
                var del = await hrServiceManager.HrNotificationsSettingService.Remove(Id);
                if (del.Succeeded)
                {

                    return Ok(del);
                }
                return Ok(await Result.FailAsync($"{del.Status.message}"));
            }
            catch (Exception exp)
            {
                return Ok(await Result.FailAsync($"{exp.Message}"));
            }
        }

    }
}