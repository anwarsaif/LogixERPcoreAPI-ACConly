using Logix.Application.Common;
using Logix.Application.DTOs.Integra;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.Integra
{
    public class IntegraMappingController : BaseIntegraApiController
    {
        private readonly ICurrentData session;
        private readonly IPermissionHelper permission;
        private readonly ILocalizationService localization;
        private readonly IIntegraServiceManager integraServiceManager;

        public IntegraMappingController(
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
        [HttpPost("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var chk = await permission.HasPermission(1629, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                
                var items = await integraServiceManager.IntegraTablesService.GetAll();

                if (!items.Data.Any())
                    return Ok(await Result<List<IntegraTableDto>>.SuccessAsync(localization.GetResource1("NosearchResult")));

                return Ok(await Result<List<IntegraTableDto>>.SuccessAsync(items.Data.ToList(), $"Search Completed {items.Data.Count()}", 200));
            }
            catch (Exception ex)
            {
                return Ok(await Result<List<IntegraTableDto>>.FailAsync($"Error in {nameof(Search)}: {ex.Message}"));
            }
        }

        [HttpPost("Search")]
        public async Task<IActionResult> Search(string? tableName)
        {
            try
            {
                var chk = await permission.HasPermission(1629, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                
                var items = await integraServiceManager.IntegraTablesService.GetAll(x=> string.IsNullOrEmpty(tableName) || x.Name.Contains(tableName) || x.Name2.Contains(tableName));

                if (!items.Data.Any())
                    return Ok(await Result<List<IntegraTableDto>>.SuccessAsync(localization.GetResource1("NosearchResult")));

                return Ok(await Result<List<IntegraTableDto>>.SuccessAsync(items.Data.ToList(), $"Search Completed {items.Data.Count()}", 200));
            }
            catch (Exception ex)
            {
                return Ok(await Result<List<IntegraTableDto>>.FailAsync($"Error in {nameof(Search)}: {ex.Message}"));
            }
        }

        #endregion


        //[HttpPost("Edit")]
        //public async Task<ActionResult> Edit(IntegraPropertyValueEditDto obj)
        //{
        //    try
        //    {
        //        var chk = await permission.HasPermission(1628, PermissionType.Edit);
        //        if (!chk)
        //            return Ok(await Result.AccessDenied("AccessDenied"));

        //        if (!ModelState.IsValid)
        //            return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

        //        if (obj.PropertyId <= 0)
        //            return Ok(await Result.FailAsync(localization.GetMessagesResource("InCorrectPropertyId")));

        //        if (obj.IntegraSystemId <= 0)
        //            return Ok(await Result.FailAsync(localization.GetMessagesResource("InCorrectSystemId")));

        //        var update = await integraServiceManager.IntegraPropertyValueService.UpdatePropertyValue(obj.PropertyId ?? 0, obj.PropertyValue ?? "", obj.IntegraSystemId ?? 0);
        //        return Ok(update);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(await Result.FailAsync($"====== Exp in Edit IntegraPropertyValuesController, MESSAGE: {ex.Message}"));
        //    }
        //}
    }
}
