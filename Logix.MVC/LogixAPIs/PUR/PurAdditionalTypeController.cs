using Logix.Application.Common;
using Logix.Application.DTOs.PUR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Services;
using Logix.Application.Wrapper;
using Logix.Domain.PUR;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.PUR
{
    public class PurAdditionalTypeController : BasePurApiController
    {
        private readonly IPurServiceManager purServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;

        public PurAdditionalTypeController(IPurServiceManager purServiceManager,
            IPermissionHelper permission,
            ICurrentData session,
            ILocalizationService localization)
        {
            this.purServiceManager = purServiceManager;
            this.permission = permission;
            this.session = session;
            this.localization = localization;
        }

        #region "GetAll - Search"
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var chk = await permission.HasPermission(2007, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var items = await purServiceManager.PurAdditionalTypeService.GetAllVW(x => x.IsDeleted == false);
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    res = res.OrderBy(e => e.Code);
                    return Ok(await Result<List<PurAdditionalTypeVw>>.SuccessAsync(res.ToList(), ""));                    
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }
        [HttpPost("Search")]
        public async Task<IActionResult> Search(PurAdditionalTypeFilterDto filter)
        {
            var chk = await permission.HasPermission(2007, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                filter.TypeId ??= 0;
                filter.AccRefTypeId ??= 0;
                filter.Code ??= 0;
                filter.CreditOrDebit ??= 0;
                filter.RateOrAmount ??= 0;
                var items = await purServiceManager.PurAdditionalTypeService.GetAll(x => x.IsDeleted == false
                && x.FacilityId == session.FacilityId
                && (filter.TypeId == 0 || x.TypeId == filter.TypeId)
                && (filter.Code == 0 || x.Code == filter.Code)
                && (string.IsNullOrEmpty(filter.TypeName) || x.TypeName == filter.TypeName)
                && (filter.CreditOrDebit == 0 || x.CreditOrDebit == filter.CreditOrDebit)
                && (filter.RateOrAmount == 0 || x.RateOrAmount == filter.RateOrAmount)
                && (filter.AccRefTypeId == 0 || x.AccRefTypeId == filter.AccRefTypeId)
                );
                if (items.Succeeded)
                {
                    return Ok(await Result<List<PurAdditionalTypeDto>>.SuccessAsync(items.Data.ToList(), ""));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<PurAdditionalTypeDto>.FailAsync($"======= Exp in Search Pur, MESSAGE: {ex.Message}"));
            }
        }
        #endregion "GetAll - Search"

        #region "Add - Edit"
        [HttpPost("Add")]
        public async Task<IActionResult> Add(PurAdditionalTypeDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(2007, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync(localization.GetMessagesResource("dataRequire")));

                var add = await purServiceManager.PurAdditionalTypeService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add, MESSAGE: {ex.Message}"));
            }
        }
        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(PurAdditionalTypeEditDto obj)
        {
            var chk = await permission.HasPermission(2007, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<PurAdditionalTypeEditDto>.FailAsync(localization.GetMessagesResource("dataRequire")));
                }

                var Edit = await purServiceManager.PurAdditionalTypeService.Update(obj);
                if (Edit.Succeeded)
                {
                    return Ok(Edit);
                }
                else
                {
                    return Ok(await Result<PurAdditionalTypeEditDto>.FailAsync(localization.GetResource1("UpdateError")));
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<PurAdditionalTypeEditDto>.FailAsync($"======= Exp in Pur PurDiscountCatalog Schedule  edit: {ex.Message}"));
            }
        }

        #endregion "Add - Edit"

        #region "Delete"
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long Id)
        {
            var chk = await permission.HasPermission(2007, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var removedItem = await purServiceManager.PurAdditionalTypeService.Remove(Id);
                if (removedItem.Succeeded)
                {
                    return Ok(removedItem);
                }
                else
                {
                    return Ok(await Result<PurAdditionalTypeDto>.FailAsync(localization.GetResource1("DeleteFail")));
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<PurAdditionalTypeDto>.FailAsync($"======= Exp in PUR, Delete: {ex.Message}"));
            }
        }
        #endregion "Delete"

        #region "GetById"
        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(long id)
        {
            try
            {
                var chk = await permission.HasPermission(2007, PermissionType.Show);
                if (!chk)
                {
                    return Ok(await Result.AccessDenied("AcessDenied"));
                }

                if (id <= 0)
                {
                    return Ok(await Result<PurAdditionalTypeDto>.FailAsync(localization.GetMessagesResource("InCorrectId")));
                }

                var getItem = await purServiceManager.PurAdditionalTypeService.GetOne(s => s.Id == id && s.IsDeleted == false);
                if (getItem.Succeeded)
                {
                    var obj = getItem.Data;
                    return Ok(await Result<PurAdditionalTypeDto>.SuccessAsync(obj, $""));
                }
                else
                {
                    return Ok(getItem);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<PurAdditionalTypeDto>.FailAsync($"====== Exp in GetById, MESSAGE: {ex.Message}"));
            }
        }
        #endregion
    }
}
