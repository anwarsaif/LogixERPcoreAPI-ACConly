using Logix.Application.Common;
using Logix.Application.DTOs.TS;
using Logix.Application.Helpers;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Logix.MVC.LogixAPIs.TS
{
    public class TsAppointmentController : BaseTsApiController
    {
        private readonly ITsServiceManager tsServiceManager;
        private readonly IMainServiceManager mainServiceManager;
        private readonly ICurrentData session;
        private readonly IPermissionHelper permission;
        private readonly ILocalizationService localization;

        public TsAppointmentController(
            ITsServiceManager tsServiceManager,
            IMainServiceManager mainServiceManager,
            ICurrentData session,
            IPermissionHelper permission,
            ILocalizationService localization
            )
        {
            this.tsServiceManager = tsServiceManager;
            this.mainServiceManager = mainServiceManager;
            this.session = session;
            this.permission = permission;
            this.localization = localization;
        }
        #region "GetAll - Search"
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var chk = await permission.HasPermission(254, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var items = await tsServiceManager.TsAppointmentService.GetAll(x => x.IsDeleted == false);

                return Ok(await Result<object>.SuccessAsync(items, $"Search Completed {items.Data.Count()}", 200));
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        [HttpPost("Search")]
        public async Task<IActionResult> Search(PaginatedRequest<TsAppointmentFilterDto> request)
        {
            try
            {
                var chk = await permission.HasPermission(254, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var filter = request.Filter;

                filter.UserId ??= 0;
                var items = await tsServiceManager.TsAppointmentService.GetAllVW(x => x.IsDeleted == false
                && x.FacilityId == session.FacilityId
                && (string.IsNullOrEmpty(filter.AppDetails) || x.AppDetails.Contains(filter.AppDetails))
                && (filter.UserId == 0 || filter.UserId == x.UserId)
                );

                if (!items.Succeeded || items.Data == null)
                {
                    return Ok(await Result<object>.FailAsync("Failed to retrieve data or no data available."));
                }

                var res = items.Data.AsQueryable();
                if (items.Succeeded)
                {
                    if (!string.IsNullOrEmpty(filter.AppDate))
                    {
                        DateTime appDate = DateTime.ParseExact(filter.AppDate, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                        res = res.Where(s => string.IsNullOrEmpty(s.AppDate) || DateTime.ParseExact(s.AppDate, "yyyy/MM/dd", CultureInfo.InvariantCulture) == appDate);
                    }
                }

                if (!res.Any())
                    return Ok(await Result<object>.SuccessAsync(new List<object>(), localization.GetResource1("NosearchResult")));
                
                if (request.PageNumber > 0 && request.PageSize > 0)
                {
                    var paginatedData = await PaginatedResult<object>.SuccessAsync(
                        res.Cast<object>(),
                        request.PageNumber,
                        request.PageSize);
                    return Ok(paginatedData);
                }

                return Ok(await Result<object>.SuccessAsync(res.ToList(), $"Search Completed {res.Count()}", 200));
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"Error in {nameof(Search)}: {ex.Message}"));
            }
        }
        #endregion "GetAll - Search"

        #region "Add - Edit"

        [HttpPost("Add")]
        public async Task<IActionResult> Add(TsAppointmentDto obj)
        {
            try
            {
                if (!await permission.HasPermission(254, PermissionType.Add))
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync(localization.GetMessagesResource("dataRequire")));

                var add = await tsServiceManager.TsAppointmentService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(TsAppointmentEditDto obj)
        {
            if (!(await permission.HasPermission(254, PermissionType.Edit) && await permission.HasPermission(254, PermissionType.Show))
                && !(await permission.HasPermission(1091, PermissionType.Edit) && await permission.HasPermission(1091, PermissionType.Show)))
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<TsAppointmentEditDto>.FailAsync(localization.GetMessagesResource("dataRequire")));
                }

                var Edit = await tsServiceManager.TsAppointmentService.Update(obj);
                return Ok(Edit);
            }
            catch (Exception ex)
            {
                return Ok(await Result<TsAppointmentEditDto>.FailAsync($"======= Exp in edit: {ex.Message}"));
            }
        }
        #endregion "Add - Edit"

        #region "Delete"
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long Id)
        {
            var chk = await permission.HasPermission(254, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var del = await tsServiceManager.TsAppointmentService.Remove(Id);
                return Ok(del);
            }
            catch (Exception ex)
            {
                return Ok(await Result<TsAppointmentDto>.FailAsync($"======= Exp in Delete: {ex.Message}"));
            }
        }
        #endregion "Delete"

        #region "GetByIdForEdit - GetById"

        [HttpGet("GetByIdForEdit")]
        public async Task<IActionResult> GetByIdForEdit(long id)
        {
            try
            {
                if (!(await permission.HasPermission(254, PermissionType.Edit) && await permission.HasPermission(254, PermissionType.Show))
                && !(await permission.HasPermission(1091, PermissionType.Edit) && await permission.HasPermission(1091, PermissionType.Show)))
                {
                    return Ok(await Result.AccessDenied("AccessDenied"));
                }

                if (id <= 0)
                {
                    return Ok(await Result<TsAppointmentEditDto>.FailAsync(localization.GetMessagesResource("InCorrectId")));
                }

                var getItem = await tsServiceManager.TsAppointmentService.GetForUpdate<TsAppointmentEditDto>(id);
                if (getItem.Succeeded)
                {
                    return Ok(await Result<TsAppointmentEditDto>.SuccessAsync(getItem.Data, $""));
                }
                else
                {
                    return Ok(getItem);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(long id)
        {
            try
            {
                if (!(await permission.HasPermission(254, PermissionType.Show))
                && !(await permission.HasPermission(1091, PermissionType.Show)))
                {
                    return Ok(await Result.AccessDenied("AcessDenied"));
                }

                if (id <= 0)
                {
                    return Ok(await Result<TsAppointmentDto>.FailAsync(localization.GetMessagesResource("NoIdInUpdate")));
                }

                var getItem = await tsServiceManager.TsAppointmentService.GetOne(s => s.AppId == id && s.IsDeleted == false);
                if (getItem.Succeeded)
                {
                    var obj = getItem.Data;
                    return Ok(await Result<TsAppointmentDto>.SuccessAsync(obj, $""));
                }
                else
                {
                    return Ok(getItem);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<TsAppointmentDto>.FailAsync($"====== Exp MESSAGE: {ex.Message}"));
            }
        }
        #endregion "GetByIdForEdit - GetById"
    }
}
