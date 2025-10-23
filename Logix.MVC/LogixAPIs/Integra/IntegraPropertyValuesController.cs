using Logix.Application.Common;
using Logix.Application.DTOs.Integra;
using Logix.Application.DTOs.Main;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Services;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.Integra
{
    public class IntegraPropertyValuesController : BaseIntegraApiController
    {
        private readonly IIntegraServiceManager integraServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly ISysConfigurationHelper configurationHelper;

        public IntegraPropertyValuesController(IIntegraServiceManager integraServiceManager,
            IPermissionHelper permission,
            ICurrentData session,
            ILocalizationService localization
             , ISysConfigurationHelper configurationHelper

            )
        {
            this.integraServiceManager = integraServiceManager;
            this.permission = permission;
            this.session = session;
            this.localization = localization;
            this.configurationHelper = configurationHelper;
        }

        [HttpPost("Search")]
        public async Task<ActionResult> Search(IntegraPropertyValueFilterDto filter)
        {
            try
            {
                var chk = await permission.HasPermission(1628, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                // to get only properties of systems that the customer has
                var allSystems = await integraServiceManager.IntegraSystemService.GetAll(s => s.IsDeleted == false);
                if (allSystems.Succeeded)
                {
                    var items = await integraServiceManager.IntegraPropertyValueService.GetAllVW(p => allSystems.Data.Select(x=>x.Id).Contains(p.IntegraSystemId ?? 0) && p.FacilityId == session.FacilityId);
                    if (items.Succeeded)
                    {
                        var res = items.Data.AsQueryable();

                        if (!string.IsNullOrEmpty(filter.PropertyName))
                            res = res.Where(t => !string.IsNullOrEmpty(t.Name) && t.Name.Contains(filter.PropertyName));

                        if (filter.IntegraSystemId > 0)
                            res = res.Where(t => t.IntegraSystemId != null && t.IntegraSystemId.Equals(filter.IntegraSystemId));

                        var resList = res.ToList();
                        List<IntegraPropertyValueFilterDto> final = new List<IntegraPropertyValueFilterDto>();
                        foreach (var item in resList)
                        {
                            final.Add(new IntegraPropertyValueFilterDto
                            {
                                PropertyId = item.Id,
                                PropertyCode = item.Code,
                                PropertyValue = item.PropertyValue,
                                PropertyName = item.Name,
                                PropertyName2 = item.Name2,
                                IntegraSystemId = item.IntegraSystemId,
                                IntegraSystemName = session.Language == 1 ? allSystems.Data.AsQueryable().FirstOrDefault(x => x.Id == item.IntegraSystemId)?.Name : allSystems.Data.AsQueryable().FirstOrDefault(x => x.Id == item.IntegraSystemId)?.Name2,
                                Description = item.Description,
                            });
                        }
                        return Ok(await Result<List<IntegraPropertyValueFilterDto>>.SuccessAsync(final));
                    }
                    return Ok(items);
                }
                return Ok(allSystems);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Search IntegraPropertyValuesController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Add")]
        public async Task<ActionResult> Add(IntegraPropertyValueDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(1628, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var add = await integraServiceManager.IntegraPropertyValueService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add IntegraPropertyValuesController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(IntegraPropertyValueDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(1628, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var update = await integraServiceManager.IntegraPropertyValueService.UpdatePropertyValue(obj.Id, obj.PropertyValue ?? "", obj.IntegraSystemId ?? 0);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Edit IntegraPropertyValuesController, MESSAGE: {ex.Message}"));
            }
        }


        [HttpGet("GetByIdForEdit")]
        public async Task<IActionResult> GetByIdForEdit(long id)
        {
            try
            {
                var chk = await permission.HasPermission(1628, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var item = await integraServiceManager.IntegraPropertyValueService.GetForUpdate<IntegraPropertyValueDto>(id);
                return Ok(item);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in GetByIdForEdit IntegraPropertyValuesController, MESSAGE: {ex.Message}"));
            }
        }

    }
}
