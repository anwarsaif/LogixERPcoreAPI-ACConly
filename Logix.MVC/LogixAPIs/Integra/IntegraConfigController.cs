using Logix.Application.Common;
using Logix.Application.DTOs.Integra;
using Logix.Application.DTOs.TS;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Services;
using Logix.Application.Wrapper;
using Logix.Domain.Integra;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.Integra
{
    public class IntegraConfigController : BaseIntegraApiController
    {
        private readonly ICurrentData session;
        private readonly IPermissionHelper permission;
        private readonly ILocalizationService localization;
        private readonly IIntegraServiceManager integraServiceManager;

        public IntegraConfigController(
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

        [HttpPost("Search")]
        public async Task<IActionResult> Search(PaginatedRequest<IntegraPropertyFilterDto> request)
        {
            try
            {
                var chk = await permission.HasPermission(1628, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                var filter = request.Filter;

                if (filter.IntegraSystemId < 0)
                    return Ok(await Result.FailAsync(localization.GetMessagesResource("InCorrectSystemId")));

                var items = await integraServiceManager.IntegraPropertyValueService.GetIntegraConfig(filter);

                if (!items.Any())
                    return Ok(await Result<object>.SuccessAsync(new List<object>(), localization.GetResource1("NosearchResult")));

                if (request.PageNumber > 0 && request.PageSize > 0)
                {
                    var paginatedData = await PaginatedResult<object>.SuccessAsync(
                        items.Cast<object>(),
                        request.PageNumber,
                        request.PageSize);
                    return Ok(paginatedData);
                }

                return Ok(await Result<object>.SuccessAsync(items.ToList(), $"Search Completed {items.Count()}", 200));
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"Error in {nameof(Search)}: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(long PropertyId, long SystemId, string PropertyValue, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return Ok(await Result.FailAsync(localization.GetMessagesResource("dataRequire")));

            if (PropertyId <= 0)
                return Ok(await Result.FailAsync(localization.GetMessagesResource("InCorrectPropertyId")));

            if (SystemId <= 0)
                return Ok(await Result.FailAsync(localization.GetMessagesResource("InCorrectSystemId")));

            try
            {
                var hasPermission = await permission.HasPermission(1628, PermissionType.Edit);
                if (!hasPermission)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var updateResult = await integraServiceManager.IntegraPropertyValueService
                    .UpdatePropertyValue(PropertyId, PropertyValue, SystemId, cancellationToken);

                if (!updateResult.Succeeded)
                    return BadRequest(updateResult);

                return Ok(updateResult);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Edit IntegraPropertyValuesController, MESSAGE: {ex.Message}"));
            }
        }

    }
}
