//using Logix.Domain.PUR;
using Logix.Application.Common;
using Logix.Application.DTOs.ACC;
using Logix.Application.DTOs.PUR;
using Logix.Application.DTOs.SAL;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Services;
using Logix.Application.Wrapper;
using Logix.Domain.PUR;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
//using ZXing;

namespace Logix.MVC.LogixAPIs.PUR
{
    public class PurExpenseController : BasePurApiController
    {
        private readonly IPurServiceManager purServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ILocalizationService localization;

        public PurExpenseController(IPurServiceManager purServiceManager,
            IPermissionHelper permission,
            ICurrentData CurrentData,
            ILocalizationService localization)
        {
            this.purServiceManager = purServiceManager;
            this.permission = permission;
            this.localization = localization;
        }
        #region "Get - Search"
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var chk = await permission.HasPermission(819, PermissionType.Show);
            if (!chk)
                return Ok(await Result.AccessDenied("AccessDenied"));
            try
            {
                var items = await purServiceManager.PurExpenseService.GetAllVW(e=>e.IsDeleted == false);
                if (items.Succeeded)
                {
                    return Ok(await Result<List<PurExpensesVw>>.SuccessAsync(items.Data.ToList(), ""));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in GetAll PurExpenseController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Search")]
        public async Task<ActionResult> Search(PurExpenseFilterDto filter)
        {
            var chk = await permission.HasPermission(819, PermissionType.Show);
            if (!chk)
                return Ok(await Result.AccessDenied("AccessDenied"));

            try
            {
                filter.TypeId ??= 0;
                filter.MethodCalculation ??= 0;
                filter.MethodDistribution ??= 0;
                var items = await purServiceManager.PurExpenseService.GetAll(x => x.IsDeleted == false
                && (filter.TypeId == 0 || x.TypeId == filter.TypeId)
                && (filter.MethodCalculation == 0 || x.MethodCalculation == filter.MethodCalculation)
                && (filter.MethodDistribution == 0 || x.MethodDistribution == filter.MethodDistribution)
                && (string.IsNullOrEmpty(filter.Name) || x.Name == filter.Name)
                && (string.IsNullOrEmpty(filter.Name2) || x.Name2 == filter.Name2)
                );
                if ((items.Succeeded))
                {
                    return Ok(await Result<List<PurExpenseDto>>.SuccessAsync(items.Data.ToList(), ""));
                }
                return Ok(items);   
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Search PurExpenseController, MESSAGE: {ex.Message}"));
            }
        }
        #endregion

        #region "Add - Edit"
        [HttpPost("Add")]
        public async Task<IActionResult> Add (PurExpenseDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(819, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if(!ModelState.IsValid)
                    return Ok(await Result.FailAsync(localization.GetMessagesResource("dataRequire")));

                var add = await purServiceManager.PurExpenseService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add PurExpenseController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(PurExpenseEditDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(819, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var update = await purServiceManager.PurExpenseService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in EditBasicData SalAdditionalTypeController, MESSAGE: {ex.Message}"));
            }
        }
        #endregion

        #region "Delete"
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var chk = await permission.HasPermission(819, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync(localization.GetMessagesResource("NoIdInDelete")));

                var delete = await purServiceManager.PurExpenseService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete Hr OpeningBalanceService Controller, MESSAGE: {ex.Message}"));
            }
        }
        #endregion

        #region "GetById"
        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(long id)
        {
            try
            {
                var chk = await permission.HasPermission(819, PermissionType.Show);
                if (!chk)
                {
                    return Ok(await Result.AccessDenied("AcessDenied"));
                }

                if (id <= 0)
                {
                    return Ok(await Result<PurExpenseDto>.FailAsync(localization.GetMessagesResource("InCorrectId")));
                }

                var getItem = await purServiceManager.PurExpenseService.GetOne(s => s.Id == id && s.IsDeleted == false);
                if (getItem.Succeeded)
                {
                    var obj = getItem.Data;
                    return Ok(await Result<PurExpenseDto>.SuccessAsync(obj, $""));
                }
                else
                {
                    return Ok(getItem);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<PurExpenseDto>.FailAsync($"====== Exp in GetById, MESSAGE: {ex.Message}"));
            }
        }
        #endregion
    }
}
