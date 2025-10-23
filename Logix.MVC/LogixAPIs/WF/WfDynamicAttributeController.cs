using Logix.Application.Common;
using Logix.Application.DTOs.WF;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.WF;
using Logix.MVC.LogixAPIs.WF.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.WF
{
    public class WfDynamicAttributeController : BaseWfController
    {
        private readonly IWFServiceManager wfServiceManager;
        private readonly ILocalizationService localization;
        private readonly ICurrentData session;

        public WfDynamicAttributeController(IWFServiceManager wfServiceManager,
            ILocalizationService localization,
            ICurrentData session)
        {
            this.wfServiceManager = wfServiceManager;
            this.localization = localization;
            this.session = session;
        }

        [HttpGet("GetAppTypeData")]
        public async Task<ActionResult> GetAppTypeData(long appTypeId, long stepId)
        {
            try
            {
                int lang = session.Language;
                DynamicAttributeVm obj = new();

                var appTypeData = await wfServiceManager.WfAppTypeService.GetOne(x => x.Id == appTypeId && x.IsDeleted == false);
                if (appTypeData.Succeeded)
                {
                    obj.AppTypeName = lang == 1 ? appTypeData.Data.Name : (appTypeData.Data.Name2 ?? appTypeData.Data.Name);
                    obj.AppTypeUrl = appTypeData.Data.Url ?? "";
                    if (obj.AppTypeUrl == ".")
                        obj.AppTypeUrl = "/Apps/Workflow/Applications/Application_Add?App_Type_ID=" + appTypeId;
                    else
                        obj.AppTypeUrl = obj.AppTypeUrl + "_Add?App_Type_ID=" + appTypeId;
                }

                if (stepId > 0)
                {
                    var stepData = await wfServiceManager.WfStepService.GetOne(x => x.Id == stepId && x.IsDeleted == false);
                    if (stepData.Succeeded)
                    {
                        obj.StepName = lang == 1 ? stepData.Data.StepName : (stepData.Data.StepName2 ?? stepData.Data.StepName);
                    }
                }
                else
                {
                    obj.StepName = lang == 1 ? "مقدم الطلب" : "Applicant";
                }
                return Ok(await Result<DynamicAttributeVm>.SuccessAsync(obj));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetAttributes")]
        public async Task<ActionResult> GetAttributes(long appTypeId, long stepId)
        {
            try
            {
                var items = await wfServiceManager.WfDynamicAttributeService.GetAllVW(x => x.AppTypeId == appTypeId
                && (stepId == 0 || x.StepId == stepId) && x.IsDeleted == false);
                if (items.Succeeded && items.Data.Any())
                {
                    var res = items.Data.OrderBy(x => x.SortOrder).ToList();
                    return Ok(await Result<List<WfDynamicAttributesVw>>.SuccessAsync(res));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }


        [HttpPost("Add")]
        public async Task<ActionResult> Add(WfDynamicAttributeDto obj)
        {
            try
            {
                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                obj.DataTypeId ??= 0; obj.LookUpType ??= 0; obj.LookUpCatagoriesId ??= 0; obj.SysLookUpCatagoriesId ??= 0;
                obj.AppTypeId ??= 0; obj.StepId ??= 0; obj.TableId ??= 0; obj.MaxLength ??= 0; obj.LookUpSql ??= "";

                var add = await wfServiceManager.WfDynamicAttributeService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(WfDynamicAttributeEditDto obj)
        {
            try
            {
                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                obj.DataTypeId ??= 0; obj.LookUpType ??= 0; obj.LookUpCatagoriesId ??= 0; obj.SysLookUpCatagoriesId ??= 0;
                obj.AppTypeId ??= 0; obj.StepId ??= 0; obj.TableId ??= 0; obj.MaxLength ??= 0; obj.LookUpSql ??= "";

                var update = await wfServiceManager.WfDynamicAttributeService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }


        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await wfServiceManager.WfDynamicAttributeService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }
    }
}
