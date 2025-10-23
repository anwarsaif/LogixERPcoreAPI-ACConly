using Logix.Application.Common;
using Logix.Application.DTOs.Integra;
using Logix.Application.DTOs.TS;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Logix.MVC.LogixAPIs.Integra
{
    public class IntegraSystemController : BaseIntegraApiController
    {
        private readonly ICurrentData session;
        private readonly IPermissionHelper permission;
        private readonly ILocalizationService localization;
        private readonly IIntegraServiceManager integraServiceManager;

        public IntegraSystemController(
            ICurrentData session,
            IPermissionHelper permission,
            ILocalizationService localization,
            IIntegraServiceManager integraServiceManager
            )
        {
            this.session = session;
            this.permission = permission;
            this.localization = localization;
            this.integraServiceManager = integraServiceManager;
        }

        #region "GetAll - Search"
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var chk = await permission.HasPermission(1628, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var items = await integraServiceManager.IntegraSystemService.GetAll(x => x.IsDeleted == false && x.FacilityId == session.FacilityId);

                return Ok(await Result<object>.SuccessAsync(items, $"Search Completed {items.Data.Count()}", 200));
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        [HttpPost("Search")]
        public async Task<IActionResult> Search(PaginatedRequest<IntegraSystemFilterDto> request)
        {
            try
            {
                var chk = await permission.HasPermission(1628, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var filter = request.Filter;

                var items = await integraServiceManager.IntegraSystemService.GetAll(x => x.IsDeleted == false
                && x.FacilityId == session.FacilityId
                && (string.IsNullOrEmpty(filter.Name) || x.Name.Contains(filter.Name) || x.Name2.Contains(filter.Name))
                );

                if (!items.Succeeded || items.Data == null)
                {
                    return Ok(await Result<object>.FailAsync("Failed to retrieve data or no data available."));
                }
                
                if (items.Data.Count() == 0)
                    return Ok(await Result<object>.SuccessAsync(new List<object>(), localization.GetResource1("NosearchResult")));

                if (request.PageNumber > 0 && request.PageSize > 0)
                {
                    var paginatedData = await PaginatedResult<object>.SuccessAsync(
                        items.Data.Cast<object>(),
                        request.PageNumber,
                        request.PageSize);
                    return Ok(paginatedData);
                }

                return Ok(await Result<object>.SuccessAsync(items.Data.ToList(), $"Search Completed {items.Data.Count()}", 200));
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"Error in {nameof(Search)}: {ex.Message}"));
            }
        }
        #endregion "GetAll - Search"
    }
}
