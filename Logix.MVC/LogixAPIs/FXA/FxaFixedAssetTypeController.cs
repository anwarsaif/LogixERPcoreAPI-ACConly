using Logix.Application.Common;
using Logix.Application.DTOs.FXA;
using Logix.Application.DTOs.Main;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Services;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.FXA.ViewModels;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Logix.MVC.LogixAPIs.FXA
{
    public class FxaFixedAssetTypeController : BaseFxaApiController
    {
        private readonly IFxaServiceManager fxaServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;

        public FxaFixedAssetTypeController(IFxaServiceManager fxaServiceManager,
            IPermissionHelper permission,
            ICurrentData session,
            ILocalizationService localization)
        {
            this.fxaServiceManager = fxaServiceManager;
            this.permission = permission;
            this.session = session;
            this.localization = localization;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult> GetAll()
        {
            try
            {
                var chk = await permission.HasPermission(901, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var items = await fxaServiceManager.FxaFixedAssetTypeService.GetAllVW(t => t.IsDeleted == false && t.FacilityId == session.FacilityId);

                if (items.Succeeded)
                {
                    var data = items.Data;
                    List<FxaFixedAssetTypeVm> final = new();
                    foreach (var item in data)
                    {
                        var obj = new FxaFixedAssetTypeVm
                        {
                            Id = item.Id,
                            Code = item.Code,
                            TypeName = item.TypeName,
                            DeprecYearlyRate = item.DeprecYearlyRate,
                            Age = item.Age,
                            AccountCode = item.AccountCode,
                            AccountName = item.AccountName,
                            AccountCode2 = item.AccountCode2,
                            AccountName2 = item.AccountName2,
                            AccountCode3 = item.AccountCode3,
                            AccountName3 = item.AccountName3
                        };

                        if (item.ParentId != 0)
                        {
                            var getMainAssetName = await fxaServiceManager.FxaFixedAssetTypeService.GetOne(t => t.TypeName, t => t.Id == item.ParentId);
                            if (getMainAssetName.Succeeded)
                                obj.MainAssetName = getMainAssetName.Data;
                        }
                        else
                            obj.MainAssetName = "رئيسي";

                        final.Add(obj);
                    }
                    return Ok(await Result<List<FxaFixedAssetTypeVm>>.SuccessAsync(final));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Search FxaFixedAssetTypeController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Search")]
        public async Task<ActionResult> Search(FxaFixedAssetTypeFilterDto filter)
        {
            try
            {
                var chk = await permission.HasPermission(901, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                filter.ParentId ??= 0;
                filter.DeprecYearlyRate ??= 0;
                filter.Age ??= 0;

                var items = await fxaServiceManager.FxaFixedAssetTypeService.GetAllVW(t => t.IsDeleted == false && t.FacilityId == session.FacilityId
                 && (string.IsNullOrEmpty(filter.Code) || (!string.IsNullOrEmpty(t.Code) && t.Code.Equals(filter.Code)))
                 && (string.IsNullOrEmpty(filter.TypeName) || (!string.IsNullOrEmpty(t.TypeName) && t.TypeName.Contains(filter.TypeName)))
                 && (filter.ParentId == 0 || (t.ParentId == filter.ParentId))
                 && (filter.DeprecYearlyRate == 0 || (t.DeprecYearlyRate == filter.DeprecYearlyRate))
                 && (filter.Age == 0 || (t.Age == filter.Age))
                 && (string.IsNullOrEmpty(filter.AccountCode) || (!string.IsNullOrEmpty(t.AccountCode) && t.AccountCode.Contains(filter.AccountCode)))
                 && (string.IsNullOrEmpty(filter.AccountCode2) || (!string.IsNullOrEmpty(t.AccountCode2) && t.AccountCode2.Contains(filter.AccountCode2)))
                 && (string.IsNullOrEmpty(filter.AccountCode3) || (!string.IsNullOrEmpty(t.AccountCode3) && t.AccountCode3.Contains(filter.AccountCode3)))
                 );

                if (items.Succeeded)
                {
                    var data = items.Data;
                    List<FxaFixedAssetTypeVm> final = new();
                    foreach (var item in data)
                    {
                        var obj = new FxaFixedAssetTypeVm
                        {
                            Id = item.Id,
                            Code = item.Code,
                            TypeName = item.TypeName,
                            DeprecYearlyRate = item.DeprecYearlyRate,
                            Age = item.Age,
                            AccountCode = item.AccountCode,
                            AccountName = item.AccountName,
                            AccountCode2 = item.AccountCode2,
                            AccountName2 = item.AccountName2,
                            AccountCode3 = item.AccountCode3,
                            AccountName3 = item.AccountName3
                        };

                        if (item.ParentId != 0)
                        {
                            var getMainAssetName = await fxaServiceManager.FxaFixedAssetTypeService.GetOne(t => t.TypeName, t => t.Id == item.ParentId);
                            if (getMainAssetName.Succeeded)
                                obj.MainAssetName = getMainAssetName.Data;
                        }
                        else
                            obj.MainAssetName = "رئيسي";

                        final.Add(obj);
                    }
                    return Ok(await Result<List<FxaFixedAssetTypeVm>>.SuccessAsync(final));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Search FxaFixedAssetTypeController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Add")]
        public async Task<ActionResult> Add(FxaFixedAssetTypeDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(901, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                obj.ParentId ??= 0;
                var add = await fxaServiceManager.FxaFixedAssetTypeService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add FxaFixedAssetTypeController, MESSAGE: {ex.Message}"));
            }
        }


        [HttpGet("GetByIdForEdit")]
        public async Task<IActionResult> GetByIdForEdit(long id)
        {
            try
            {
                var chk = await permission.HasPermission(901, PermissionType.View);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var getItem = await fxaServiceManager.FxaFixedAssetTypeService.GetOneVW(t => t.Id == id);
                if (getItem.Succeeded)
                {
                    var item = getItem.Data;
                    FxaFixedAssetTypeEditDto obj = new()
                    {
                        Id = item.Id,
                        Code = item.Code,
                        TypeName = item.TypeName,
                        Age = item.Age,
                        DeprecYearlyRate = item.DeprecYearlyRate,
                        Note = item.Note,
                        ParentId = item.ParentId,

                        AccountCode = item.AccountCode,
                        AccountName = item.AccountName,

                        AccountCode2 = item.AccountCode2,
                        AccountName2 = item.AccountName2,

                        AccountCode3 = item.AccountCode3,
                        AccountName3 = item.AccountName3
                    };

                    return Ok(await Result<FxaFixedAssetTypeEditDto>.SuccessAsync(obj));
                }
                return Ok(getItem);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in GetByIdForEdit FxaFixedAssetTypeController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(FxaFixedAssetTypeEditDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(901, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var update = await fxaServiceManager.FxaFixedAssetTypeService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Edit FxaFixedAssetTypeController, MESSAGE: {ex.Message}"));
            }
        }


        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var chk = await permission.HasPermission(901, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await fxaServiceManager.FxaFixedAssetTypeService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete FxaFixedAssetTypeController, MESSAGE: {ex.Message}"));
            }
        }
    }
}