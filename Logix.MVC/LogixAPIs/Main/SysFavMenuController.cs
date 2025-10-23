using Logix.Application.Common;
using Logix.Application.DTOs.Main;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Logix.MVC.LogixAPIs.Main
{
    public class SysFavMenuController : BaseMainApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;

        public SysFavMenuController(IMainServiceManager mainServiceManager,
            IPermissionHelper permission,
            ILocalizationService localization,
            ICurrentData session)
        {
            this.mainServiceManager = mainServiceManager;
            this.permission = permission;
            this.session = session;
            this.localization = localization;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult> GetAll()
        {
            try
            {
                var items = await mainServiceManager.SysFavMenuService.GetAll(f => f.UserId != null && f.UserId == session.UserId);
                if (items.Succeeded)
                {
                    var res = items.Data.OrderBy(x => x.SortNo).AsQueryable();
                    var results = res.ToList();
                    return Ok(await Result<List<SysFavMenuDto>>.SuccessAsync(results));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in GetAll SysFavMenuController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetByIdForEdit")]
        public async Task<IActionResult> GetByIdForEdit(long id)
        {
            try
            {
                if (id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var item = await mainServiceManager.SysFavMenuService.GetForUpdate<SysFavMenuDto>(id);
                if (item.Succeeded)
                {
                    //if user modify the id in browser url, the retreived data may be a favorite screen for another user
                    if (item.Data.UserId == session.UserId)
                        return Ok(item);
                    else
                        return Ok(await Result.FailAsync("Invalid id"));
                }
                else
                    return Ok(item);

            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in GetByIdForEdit SysFavMenuController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(SysFavMenuDto obj)
        {
            try
            {
                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var update = await mainServiceManager.SysFavMenuService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Edit SysFavMenuController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await mainServiceManager.SysFavMenuService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete SysFavMenuController, MESSAGE: {ex.Message}"));
            }
        }
    }
}