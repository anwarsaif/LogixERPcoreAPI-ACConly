using Logix.Application.Common;
using Logix.Application.DTOs.Main;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.Main;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Logix.MVC.LogixAPIs.Main
{
    public class AnnouncementApiController : BaseMainApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ILocalizationService localization;

        public AnnouncementApiController(
            IMainServiceManager mainServiceManager,
            IPermissionHelper permission,
            ILocalizationService localization
            )
        {
            this.mainServiceManager = mainServiceManager;
            this.permission = permission;
            this.localization = localization;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var chk = await permission.HasPermission(632, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var items = await mainServiceManager.SysAnnouncementService.GetAllVW(a => a.IsDeleted == false);
                return Ok(items.Data);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in GetAll AnnouncementApiController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Search")]
        public async Task<ActionResult> Search(SysAnnouncementFilterDto filter)
        {
            try
            {
                var chk = await permission.HasPermission(632, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                filter.Type ??= 0; filter.LocationId ??= 0; filter.BranchId ??= 0; filter.DeptLocationId ??= 0; filter.DeptId ??= 0;

                var items = await mainServiceManager.SysAnnouncementService.GetAllVW(s => s.IsDeleted == false
                && (filter.IsActive == null || s.IsActive == filter.IsActive)
                && (string.IsNullOrEmpty(filter.Subject) || (!string.IsNullOrEmpty(s.Subject) && s.Subject.Contains(filter.Subject)))
                && (filter.Type == 0 || s.Type == filter.Type)
                && (filter.LocationId == 0 || s.LocationId == filter.LocationId)
                && (filter.BranchId == 0 || s.BranchId == filter.BranchId)
                && (filter.DeptLocationId == 0 || s.DeptLocationId == filter.DeptLocationId)
                && (filter.DeptId == 0 || s.DeptId == filter.DeptId)
                );
                if (items.Succeeded)
                {
                    var res = items.Data;
                    
                    if (!string.IsNullOrEmpty(filter.StartDate) || !string.IsNullOrEmpty(filter.EndDate))
                    {
                        var startDate = DateHelper.StringToDate(filter.StartDate);
                        var endDate = DateHelper.StringToDate(filter.EndDate);

                        res = res.Where(s => !string.IsNullOrEmpty(s.StartDate) && DateHelper.StringToDate(s.StartDate) >= startDate && DateHelper.StringToDate(s.StartDate) <= endDate);
                    }

                    var final = res.ToList();
                    return Ok(await Result<List<SysAnnouncementVw>>.SuccessAsync(final));
                }
                else
                {
                    return Ok(items);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Search AnnouncementApiController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add(SysAnnouncementDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(632, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));


                var add = await mainServiceManager.SysAnnouncementService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add AnnouncementApi, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(long id)
        {
            try
            {
                var chk = await permission.HasPermission(632, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));

                var getAnnounce = await mainServiceManager.SysAnnouncementService.GetOne(a => a.Id == id);
                return Ok(getAnnounce);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in GetById AnnouncementApi, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetByIdForEdit")]
        public async Task<IActionResult> GetByIdForEdit(long id)
        {
            try
            {
                var chk = await permission.HasPermission(632, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var getAnnounce = await mainServiceManager.SysAnnouncementService.GetForUpdate<SysAnnouncementEditDto>(id);
                return Ok(getAnnounce);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in GetByIdForEdit AnnouncementApi, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(SysAnnouncementEditDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(632, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var update = await mainServiceManager.SysAnnouncementService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Edit AnnouncementApi, MESSAGE: {ex.Message}"));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var chk = await permission.HasPermission(632, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));
                }

                var delete = await mainServiceManager.SysAnnouncementService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete AnnouncementApi, MESSAGE: {ex.Message}"));
            }
        }
    }
}
