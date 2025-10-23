using Logix.Application.Common;
using Logix.Application.DTOs.Main;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.Main;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.HR.ViewModel;
using Logix.MVC.LogixAPIs.Main.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Logix.MVC.LogixAPIs.Main
{
    public class SysCitiesController : BaseMainApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ILocalizationService localization;
        private readonly ICurrentData session;
        private readonly IApiDDLHelper ddlHelper;

        public SysCitiesController(IMainServiceManager mainServiceManager,
            IPermissionHelper permission,
            ILocalizationService localization,
            ICurrentData session,
            IApiDDLHelper ddlHelper)
        {
            this.mainServiceManager = mainServiceManager;
            this.permission = permission;
            this.localization = localization;
            this.session = session;
            this.ddlHelper = ddlHelper;
        }

        [HttpPost("GetCitiesList")]
        public async Task<IActionResult> GetCitiesList()
        {
            try
            {
                var chk = await permission.HasPermission(797, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                List<SysCitiesVM> citiesVMs = new List<SysCitiesVM>();
                var items = await mainServiceManager.SysCitesService.GetAll(c => c.IsDeleted == false);
                if (items.Succeeded)
                {
                    foreach (var item in items.Data)
                    {
                        citiesVMs.Add(new SysCitiesVM
                        {
                            CityID = item.CityID,
                            CityName = item.CityName,
                            ParentID = item.ParentID
                        });
                    }
                    return Ok(await Result<List<SysCitiesVM>>.SuccessAsync(citiesVMs));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"======= Exp in GetCitiesList SysCitiesController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Add")]
        public async Task<ActionResult> Add(SysCitesDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(797, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var add = await mainServiceManager.SysCitesService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add SysCitesController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(SysCitesEditDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(797, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var update = await mainServiceManager.SysCitesService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Edit SysCitesController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var chk = await permission.HasPermission(797, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await mainServiceManager.SysCitesService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete SysCitesController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(long id)
        {
            try
            {
                var chk = await permission.HasPermission(797, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var item = await mainServiceManager.SysCitesService.GetOne(t => t.CityID == id && t.IsDeleted == false);
                return Ok(item);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in GetById SysCitesController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetByIdForEdit")]
        public async Task<IActionResult> GetByIdForEdit(long id)
        {
            try
            {
                var chk = await permission.HasPermission(797, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var item = await mainServiceManager.SysCitesService.GetForUpdate<SysCitesEditDto>(id);
                return Ok(item);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in GetByIdForEdit SysCitesController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetCitiesByCountry")]
        public async Task<IActionResult> GetCitiesByCountry(int countryId)
        {
            try
            {
                int lang = session.Language;
                var list = await ddlHelper.GetAnyLis<SysCites, int>(b => b.CountryID == countryId || b.CityID == countryId && b.IsDeleted == false, "CityID", lang == 2 ? "CityName2" : "CityName");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
    }
}
