using Logix.Application.Common;
using Logix.Application.DTOs.Main;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.Main
{
    public class SysLookupCategoryController : BaseMainApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IPermissionHelper permission;
        private readonly IWebHostEnvironment env;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;

        public SysLookupCategoryController(
           IMainServiceManager mainServiceManager,
           IPermissionHelper permission,
           IWebHostEnvironment env,
           IDDListHelper listHelper,
           ICurrentData session,
           ILocalizationService localization)
        {
            this.mainServiceManager = mainServiceManager;
            this.permission = permission;
            this.env = env;
            this.session = session;
            this.localization = localization;
        }

        [HttpPost("Search")]
        public async Task<ActionResult> Search(SysLookupCategoryDto filter)
        {
            try
            {
                var chk = await permission.HasPermission(121, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var items = await mainServiceManager.SysLookupCategoryService.GetAll(c => c.Isdel == false);
                if (items.Succeeded)
                {
                    var res = items.Data.OrderBy(c => c.CatagoriesId).AsQueryable();

                    if (filter == null)
                    {
                        return Ok(items);
                    }

                    if (Convert.ToInt32(filter.SystemId) > 0)
                        res = res.Where(c => c.SystemId != null && c.SystemId.Equals(filter.SystemId));

                    if (!string.IsNullOrEmpty(filter.CatagoriesName))
                        res = res.Where(c => (c.CatagoriesName != null && c.CatagoriesName.Contains(filter.CatagoriesName) || (c.CatagoriesName2 != null && c.CatagoriesName2.ToLower().Contains(filter.CatagoriesName.ToLower()))));

                    var final = res.ToList();

                    return Ok(await Result<List<SysLookupCategoryDto>>.SuccessAsync(final));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Search SysLookupCategoryController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Add")]
        public async Task<ActionResult> Add(SysLookupCategoryDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(121, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var add = await mainServiceManager.SysLookupCategoryService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add SysLookupCategoryController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(SysLookupCategoryEditDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(121, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var update = await mainServiceManager.SysLookupCategoryService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Edit SysLookupCategoryController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(long id)
        {
            try
            {
                var chk = await permission.HasPermission(121, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var item = await mainServiceManager.SysLookupCategoryService.GetOne(c => c.CatagoriesId == id && c.Isdel == false);
                return Ok(item);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in GetById SysLookupCategoryController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetByIdForEdit")]
        public async Task<IActionResult> GetByIdForEdit(long id)
        {
            try
            {
                var chk = await permission.HasPermission(121, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var item = await mainServiceManager.SysLookupCategoryService.GetForUpdate<SysLookupCategoryEditDto>(id);
                if (item.Succeeded)
                {
                    if (item.Data.IsEditable == false)
                        return Ok(await Result.FailAsync("لا يمكنك تعديل هذا العنصر"));
                }
                return Ok(item);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in GetByIdForEdit SysLookupCategoryController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var chk = await permission.HasPermission(121, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await mainServiceManager.SysLookupCategoryService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete SysLookupCategoryController, MESSAGE: {ex.Message}"));
            }
        }
    
    }
}
