using Logix.Application.Common;
using Logix.Application.DTOs.ACC;
using Logix.Application.DTOs.Main;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Logix.MVC.LogixAPIs.Main
{
    public class SysPeriodController : BaseMainApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;

        public SysPeriodController(IMainServiceManager mainServiceManager,
            IPermissionHelper permission,
            ILocalizationService localization,
            ICurrentData session)
        {
            this.mainServiceManager = mainServiceManager;
            this.permission = permission;
            this.session = session;
            this.localization = localization;
        }

        [NonAction]
        private async Task<bool> CheckPermission(long sysId, PermissionType permissionType)
        {
            try
            {
                bool chk = false;
                switch (sysId)
                {
                    case 4:
                        chk = await permission.HasPermission(2284, permissionType);
                        break;
                    case 6:
                        chk = await permission.HasPermission(2174, permissionType);
                        break;
                    case 8:
                        chk = await permission.HasPermission(2177, permissionType);
                        break;
                    case 5:
                        chk = await permission.HasPermission(2178, permissionType);
                        break;
                    case 53:
                        chk = await permission.HasPermission(2180, permissionType);
                        break;
                    default:
                        break;
                }
                return chk;
            }
            catch
            {
                return false;
            }
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult> GetAll(long sysId)
        {
            try
            {
                bool chk = await CheckPermission(sysId, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var items = await mainServiceManager.SysPeriodService.GetAll(c => c.IsDeleted == false
                    && c.FacilityId == session.FacilityId && c.SystemId == sysId);
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add(SysPeriodDto obj)
        {
            try
            {
                bool chk = await CheckPermission(obj.SystemId, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync(localization.GetMessagesResource("dataRequire")));

                var chkOverlap = await mainServiceManager.SysPeriodService.IsDateRangeOverlap(obj.Id, obj.StartDate, obj.EndDate, obj.SystemId, session.FacilityId);
                if (!chkOverlap.Succeeded)
                    return Ok(chkOverlap);

                if (chkOverlap.Data > 0)
                    return Ok(await Result.FailAsync("يوجد تاريخ سابق يتداخل مع الفترة الزمنية المدخلة. يرجى اختيار فترة زمنية أخرى"));

                DateTime startDate = DateTime.ParseExact(obj.StartDate, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                DateTime endDate = DateTime.ParseExact(obj.EndDate, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                if (startDate > endDate)
                    return Ok(await Result<SysPeriodDto>.FailAsync(localization.GetMessagesResource("datePeriodStart")));

                var add = await mainServiceManager.SysPeriodService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccPeriodsDto>.FailAsync($"======= Exp: {ex.Message}"));
            }
        }

        [HttpGet("GetByIdForEdit")]
        public async Task<IActionResult> GetByIdForEdit(long id)
        {
            try
            {
                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));

                var item = await mainServiceManager.SysPeriodService.GetForUpdate<SysPeriodEditDto>(id);
                if (item.Succeeded)
                {
                    var chk = await CheckPermission(item.Data.SystemId, PermissionType.Edit);
                    if (!chk)
                        return Ok(await Result.AccessDenied("AccessDenied"));
                }
                
                return Ok(item);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(SysPeriodEditDto obj)
        {
            try
            {
                bool chk = await CheckPermission(obj.SystemId, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync(localization.GetMessagesResource("dataRequire")));

                var chkOverlap = await mainServiceManager.SysPeriodService.IsDateRangeOverlap(obj.Id, obj.StartDate, obj.EndDate, obj.SystemId, obj.FacilityId);
                if (!chkOverlap.Succeeded)
                    return Ok(chkOverlap);

                if (chkOverlap.Data > 0)
                    return Ok(await Result.FailAsync("يوجد تاريخ سابق يتداخل مع الفترة الزمنية المدخلة. يرجى اختيار فترة زمنية أخرى"));

                DateTime startDate = DateTime.ParseExact(obj.StartDate, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                DateTime endDate = DateTime.ParseExact(obj.EndDate, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                if (startDate > endDate)
                    return Ok(await Result<SysPeriodDto>.FailAsync(localization.GetMessagesResource("datePeriodStart")));

                var update = await mainServiceManager.SysPeriodService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id, long sysId)
        {
            try
            {
                bool chk = await CheckPermission(sysId, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await mainServiceManager.SysPeriodService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }
    }
}
