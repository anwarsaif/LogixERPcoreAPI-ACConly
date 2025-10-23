using Logix.Application.Common;
using Logix.Application.DTOs.TS;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Logix.MVC.LogixAPIs.TS
{
    public class TsAppointmentStaffController : BaseTsApiController
    {
        private readonly ITsServiceManager tsServiceManager;
        private readonly IMainServiceManager mainServiceManager;
        private readonly ICurrentData session;
        private readonly IPermissionHelper permission;
        private readonly ILocalizationService localization;

        public TsAppointmentStaffController(
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
                var chk = await permission.HasPermission(1092, PermissionType.Show);
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
                var chk = await permission.HasPermission(1092, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var filter = request.Filter;

                var items = await tsServiceManager.TsAppointmentService.GetAllVW(x => x.IsDeleted == false
                && (string.IsNullOrEmpty(filter.AppDetails) || x.AppDetails.Contains(filter.AppDetails))
                && (session.EmpId == x.ManagerId)
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
                        res = res.Where(s => string.IsNullOrEmpty(s.AppStartDate) || DateTime.ParseExact(s.AppStartDate, "yyyy/MM/dd", CultureInfo.InvariantCulture) == appDate);
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
    }
}
