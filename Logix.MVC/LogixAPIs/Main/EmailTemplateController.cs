using Logix.Application.Common;
using Logix.Application.DTOs.CRM;
using Logix.Application.DTOs.Main;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.Main
{
    public class EmailTemplateController : BaseMainApiController
    {
        private readonly ICrmServiceManager crmServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ILocalizationService localization;

        public EmailTemplateController(ICrmServiceManager crmServiceManager,
            IPermissionHelper permission,
            ILocalizationService localization)
        {
            this.crmServiceManager = crmServiceManager;
            this.permission = permission;
            this.localization = localization;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult> GetAll()
        {
            try
            {
                var chk = await permission.HasPermission(902, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var items = await crmServiceManager.CrmEmailTemplateService.GetAll(c => c.IsDeleted == false);
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in GetAlll CrmEmailTemplatesController, MESSAGE: {ex.Message}"));
            }
        }


        [HttpPost("Search")]
        public async Task<ActionResult> Search(CrmEmailTemplateFilterDto filter)
        {
            try
            {
                var chk = await permission.HasPermission(902, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                var items = await crmServiceManager.CrmEmailTemplateService.GetAll(t => t.IsDeleted == false);
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();

                    if (!string.IsNullOrEmpty(filter.Name))
                        res = res.Where(f => f.Name != null && f.Name.Contains(filter.Name));

                    var final = res.ToList();

                    return Ok(await Result<List<CrmEmailTemplateDto>>.SuccessAsync(final));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Search CrmEmailTemplatesController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Add")]
        public async Task<ActionResult> Add(CrmEmailTemplateDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(902, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var add = await crmServiceManager.CrmEmailTemplateService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("GetByIdForEdit")]
        public async Task<IActionResult> GetByIdForEdit(long id)
        {
            try
            {
                var chk = await permission.HasPermission(902, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));

                var item = await crmServiceManager.CrmEmailTemplateService.GetForUpdate<CrmEmailTemplateEditDto>(id);
                if (item.Succeeded)
                {
                    List<SysFileDto>? Files = new();
                    var getAttachements = await crmServiceManager.CrmEmailTemplateAttachService.GetAll(x => x.TemplateId == item.Data.Id
                    && x.IsDeleted == false);
                    foreach (var attach in getAttachements.Data)
                    {
                        Files.Add(new()
                        {
                            Id = attach.Id,
                            FileName = attach.Name,
                            FileUrl = attach.FileUrl
                        });
                    }

                    item.Data.Files = Files;
                }
                return Ok(item);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }


        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(CrmEmailTemplateEditDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(902, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var update = await crmServiceManager.CrmEmailTemplateService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Edit CrmEmailTemplatesController, MESSAGE: {ex.Message}"));
            }
        }


        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var chk = await permission.HasPermission(902, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await crmServiceManager.CrmEmailTemplateService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete CrmEmailTemplatesController, MESSAGE: {ex.Message}"));
            }
        }
    }
}