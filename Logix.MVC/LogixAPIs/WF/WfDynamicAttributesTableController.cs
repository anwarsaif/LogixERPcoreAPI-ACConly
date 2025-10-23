using Logix.Application.Common;
using Logix.Application.DTOs.WF;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.LogixAPIs.WF.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.WF
{
    public class WfDynamicAttributesTableController : BaseWfController
    {
        private readonly IWFServiceManager wfServiceManager;
        private readonly ILocalizationService localization;

        public WfDynamicAttributesTableController(IWFServiceManager wfServiceManager,
            ILocalizationService localization)
        {
            this.wfServiceManager = wfServiceManager;
            this.localization = localization;
        }

        [HttpGet("GetTableData")]
        public async Task<ActionResult> GetTableData(long tableId, long stepId)
        {
            try
            {
                DynamicAttributesTableVm obj = new();

                var tableName = await wfServiceManager.WfAppTypeTableService.GetOne(x => x.Name, x => x.Id == tableId && x.IsDeleted == false);
                if (tableName.Succeeded)
                    obj.TableName = tableName.Data ?? "";

                var items = await wfServiceManager.WfDynamicAttributesTableService.GetAllVW(x => x.TableId == tableId
                && (stepId == 0 || x.StepId == stepId) && x.IsDeleted == false);
                if (items.Succeeded && items.Data.Any())
                {
                    var res = items.Data.OrderBy(x => x.SortOrder).ToList();
                    obj.TableAttributes = res;
                }
                return Ok(await Result<DynamicAttributesTableVm>.SuccessAsync(obj));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }
        //[HttpGet("GetDynamicAttributesAppType")]
        //public async Task<ActionResult> GetDynamicAttributesAppType(long tableId, long stepId)
        //{
        //    try
        //    {
                

        //        var items = await wfServiceManager.WfDynamicAttributesTableService.GetAll(x => x.TableId == tableId
        //        && (stepId == 0 || x.StepId == stepId) && x.IsDeleted == false);
        //        if (items.Succeeded && items.Data.Any())
        //        {
        //            var res = items.Data.OrderBy(x => x.SortOrder).ToList();
                    
        //        }
        //        return Ok(await Result<DynamicAttributesTableVm>.SuccessAsync(obj));
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
        //    }
        //}

        
        [HttpPost("Add")]
        public async Task<ActionResult> Add(WfDynamicAttributesTableDto obj)
        {
            try
            {
                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                obj.DataTypeId ??= 0; obj.LookUpType ??= 0; obj.LookUpCatagoriesId ??= 0; obj.SysLookUpCatagoriesId ??= 0;
                obj.TableId ??= 0; obj.MaxLength ??= 0; obj.LookUpSql ??= "";

                var add = await wfServiceManager.WfDynamicAttributesTableService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }


        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(WfDynamicAttributesTableEditDto obj)
        {
            try
            {
                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                obj.DataTypeId ??= 0; obj.LookUpType ??= 0; obj.LookUpCatagoriesId ??= 0; obj.SysLookUpCatagoriesId ??= 0;
                obj.TableId ??= 0; obj.MaxLength ??= 0; obj.LookUpSql ??= "";

                var update = await wfServiceManager.WfDynamicAttributesTableService.Update(obj);
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

                var delete = await wfServiceManager.WfDynamicAttributesTableService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }
    }
}
