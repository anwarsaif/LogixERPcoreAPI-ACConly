using Logix.Application.Common;
using Logix.Application.DTOs.Main;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.Main
{

    public class SysCurrencyController : BaseMainApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ILocalizationService localization;

        public SysCurrencyController(IMainServiceManager mainServiceManager,
            IPermissionHelper permission,
            ILocalizationService localization)
        {
            this.mainServiceManager = mainServiceManager;
            this.permission = permission;
            this.localization = localization;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult> GetAll()
        {
            try
            {
                var chk = await permission.HasPermission(404, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var items = await mainServiceManager.SysCurrencyService.GetAll(c => c.IsDeleted == false);
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Search")]
        public async Task<ActionResult> Search(SysCurrencyFilterDto filter)
        {
            try
            {
                var chk = await permission.HasPermission(404, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var items = await mainServiceManager.SysCurrencyService.GetAll(x => x.IsDeleted == false
                && (string.IsNullOrEmpty(filter.Code) || x.Code.ToLower().Contains(filter.Code.ToLower()))
                && (string.IsNullOrEmpty(filter.Name) || x.Name.Contains(filter.Name) || x.Name2.ToLower().Contains(filter.Name.ToLower())));

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Add")]
        public async Task<ActionResult> Add(SysCurrencyDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(404, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var add = await mainServiceManager.SysCurrencyService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(SysCurrencyEditDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(404, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var update = await mainServiceManager.SysCurrencyService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var chk = await permission.HasPermission(404, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var item = await mainServiceManager.SysCurrencyService.GetOne(c => c.Id == id && c.IsDeleted == false);
                return Ok(item);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetByIdForEdit")]
        public async Task<IActionResult> GetByIdForEdit(int id)
        {
            try
            {
                var chk = await permission.HasPermission(404, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var item = await mainServiceManager.SysCurrencyService.GetForUpdate<SysCurrencyEditDto>(id);
                if (item.Succeeded)
                {
                    var obj = item.Data;
                    return Ok(await Result<SysCurrencyEditDto>.SuccessAsync(obj, $""));
                }
                else
                {
                    return Ok(item);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var chk = await permission.HasPermission(400, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                if (id == 1)
                    return Ok(await Result.FailAsync($"{localization.GetMainResource("CantDeleteThePrimaryCurrency")}"));

                var delete = await mainServiceManager.SysCurrencyService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }


        [HttpGet("GetDefaultCurrency")]
        public async Task<IActionResult> GetDefaultCurrency()
        {
            try
            {
                var dataCurrency = await mainServiceManager.SysCurrencyService.GetAll(x => x.IsDeleted == false);
                var DefaultCurrency = dataCurrency.Data.OrderBy(S => S.Id).FirstOrDefault();

                if (DefaultCurrency != null)
                {
                    var obj = DefaultCurrency;
                    return Ok(await Result<SysCurrencyDto>.SuccessAsync(obj, $""));
                }
                else
                {
                    return Ok(DefaultCurrency);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<SysCurrencyDto>.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }
    }
}
