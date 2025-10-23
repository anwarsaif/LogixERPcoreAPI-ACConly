using Logix.Application.Common;
using Logix.Application.DTOs.Main;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.Main;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Logix.MVC.LogixAPIs.Main
{
    public class SysPropertyValuesController : BaseMainApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IApiDDLHelper ddlHelper;

        public SysPropertyValuesController(IMainServiceManager mainServiceManager,
            IPermissionHelper permission,
            ICurrentData session,
            ILocalizationService localization,
            IApiDDLHelper ddlHelper
            )
        {
            this.mainServiceManager = mainServiceManager;
            this.permission = permission;
            this.session = session;
            this.localization = localization;
            this.ddlHelper = ddlHelper;
        }

        [HttpPost("Search")]
        public async Task<ActionResult> Search(SysPropertyValueFilterDto filter)
        {
            try
            {
                var chk = await permission.HasPermission(821, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                filter.PropertyId ??= 0; filter.SystemId ??= 0; filter.ClassificationsId ??= 0;
                long facilityId = session.FacilityId;

                // to get only properties of systems that the customer has
                var allSystems = await mainServiceManager.SysSystemService.GetAll(s => s.SystemId, s => s.Isdel == false);
                if (allSystems.Succeeded)
                {
                    var properties = await mainServiceManager.SysPropertyService.GetAllVW(p => allSystems.Data.Contains(p.SystemId ?? 0)
                    && (filter.PropertyId == 0 || (p.Id == filter.PropertyId))
                    && (string.IsNullOrEmpty(filter.PropertyName) || (!string.IsNullOrEmpty(p.PropertyName) && p.PropertyName.Contains(filter.PropertyName)))
                    && (filter.SystemId == 0 || (p.SystemId == filter.SystemId))
                    && (filter.ClassificationsId == 0 || (p.ClassificationsId == filter.ClassificationsId))
                    && (filter.IsRequired == false || (p.IsRequired == filter.IsRequired))
                    );

                    if (properties.Succeeded)
                    {
                        List<SysPropertyValueFilterDto> final = new();

                        foreach (var item in properties.Data)
                        {
                            long valueId = 0; string propertyValue = "";
                            var getValue = await mainServiceManager.SysPropertyValueService.GetOne(x => x.PropertyId == item.Id && x.FacilityId == facilityId);
                            if (getValue.Succeeded && getValue.Data.Id > 0)
                            {
                                valueId = getValue.Data.Id;
                                propertyValue = getValue.Data.PropertyValue ?? "";
                            }

                            final.Add(new SysPropertyValueFilterDto
                            {
                                Id = valueId,
                                PropertyCode = item.PropertyCode,
                                PropertyId = item.Id,
                                PropertyValue = propertyValue,
                                PropertyName = item.PropertyName,
                                SystemId = item.SystemId,
                                SystemName = item.SystemName,
                                SystemName2 =  item.SystemName2,
                                Description = item.Description,
                            });
                        }

                        if (filter.IsEmptyValue)
                        {
                            final = final.Where(x => string.IsNullOrEmpty(x.PropertyValue)).ToList();
                        }

                        return Ok(await Result<List<SysPropertyValueFilterDto>>.SuccessAsync(final));
                    }
                    return Ok(properties);
                }
                return Ok(allSystems);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }
        
        [HttpPost("Add")]
        public async Task<ActionResult> Add(SysPropertyValueDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(821, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var add = await mainServiceManager.SysPropertyValueService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(SysPropertyValueDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(821, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var update = await mainServiceManager.SysPropertyValueService.UpdatePropertyValue(obj.Id, obj.PropertyValue ?? "");
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }


        [HttpGet("GetByIdForEdit")]
        public async Task<IActionResult> GetByIdForEdit(long id)
        {
            try
            {
                var chk = await permission.HasPermission(821, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var item = await mainServiceManager.SysPropertyValueService.GetForUpdate<SysPropertyValueDto>(id);
                return Ok(item);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in GetByIdForEdit SysPropertyValuesController, MESSAGE: {ex.Message}"));
            }
        }


        [HttpGet("DDLPropertyClassification")]
        public async Task<IActionResult> DDLPropertyClassification()
        {
            try
            {
                var list = new SelectList(new List<DDListItem<SysPropertyClassification>>());
                list = await ddlHelper.GetAnyLis<SysPropertyClassification, long>(s => s.IsDeleted == false, "Id", (session.Language == 1) ? "Name" : "Name2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
    }
}
