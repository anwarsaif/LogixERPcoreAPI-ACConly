using Logix.Application.Common;
using Logix.Application.DTOs.Main;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.Main
{
    public class SysCustomersFilesSettingController : BaseMainApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly ICurrentData currentData;
        private readonly ILocalizationService localization;

        public SysCustomersFilesSettingController(
            IMainServiceManager mainServiceManager,
            ICurrentData currentData,
            ILocalizationService localization)
        {
            this.mainServiceManager = mainServiceManager;
            this.currentData = currentData;
            this.localization = localization;
        }

        [HttpGet("Bind")]
        public async Task<IActionResult> Bind(int customerTypeId)
        {
            try
            {
                List<SysCustomersFilesSettingDto> result = new();
                // get file types from lookup
                var getLookup = await mainServiceManager.SysLookupDataService.GetAll(x => x.CatagoriesId == 344 && x.Isdel == false);
                if (getLookup.Succeeded)
                {
                    foreach (var item in getLookup.Data)
                    {
                        var getSetting = await mainServiceManager.SysCustomersFilesSettingService.GetOneVW(x => x.FileTypeId == item.Code
                        && x.CustomerTypeId == customerTypeId && x.FacilityId == currentData.FacilityId);

                        SysCustomersFilesSettingDto obj = new()
                        {
                            CustomerTypeId = customerTypeId,
                            FileTypeId = Convert.ToInt32(item.Code),
                            FileTypeName = item.Name,
                            FileTypeName2 = item.Name2 ?? item.Name,
                            IsRequired = false,
                            RequiresAuthorization = false
                        };

                        if (getSetting.Data != null)
                        {
                            obj.Id = getSetting.Data.Id;
                            obj.CustomerTypeId = getSetting.Data.CustomerTypeId;
                            obj.CustomerTypeName = getSetting.Data.CustomerTypeName;
                            obj.IsRequired = getSetting.Data.IsRequired;
                            obj.RequiresAuthorization = getSetting.Data.RequiresAuthorization;
                            obj.IsDeleted = getSetting.Data.IsDeleted;
                        }

                        result.Add(obj);
                    }
                }
                return Ok(await Result<List<SysCustomersFilesSettingDto>>.SuccessAsync(result));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(SysCustomersFilesSettingDto obj)
        {
            try
            {
                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var update = await mainServiceManager.SysCustomersFilesSettingService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }
    }
}
