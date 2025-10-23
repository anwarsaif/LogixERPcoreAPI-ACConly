using Logix.Application.Common;
using Logix.Application.DTOs.FXA;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.FXA;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.FXA.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;

namespace Logix.MVC.LogixAPIs.FXA
{
    public class FxaAdditionsExclusionController : BaseFxaApiController
    {
        private readonly IFxaServiceManager fxaServiceManager;
        private readonly IAccServiceManager accServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IApiDDLHelper ddlHelper;
        private readonly IWebHostEnvironment env;

        public FxaAdditionsExclusionController(IFxaServiceManager fxaServiceManager,
            IAccServiceManager accServiceManager,
            IPermissionHelper permission,
            ICurrentData session,
            ILocalizationService localization,
            IApiDDLHelper ddlHelper,
            IWebHostEnvironment env)
        {
            this.fxaServiceManager = fxaServiceManager;
            this.accServiceManager = accServiceManager;
            this.permission = permission;
            this.session = session;
            this.localization = localization;
            this.ddlHelper = ddlHelper;
            this.env = env;
        }

        [HttpPost("Search")]
        public async Task<ActionResult> Search(FxaAdditionsExclusionFilterDto filter)
        {
            try
            {
                var chk = await permission.HasPermission(1986, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                filter.Id ??= 0;
                filter.FixedAssetNo ??= 0;
                var items = await fxaServiceManager.FxaAdditionsExclusionService.GetAllVW(a => a.IsDeleted == false
                        && a.FacilityId == session.FacilityId
                        && (filter.Id == 0 || (a.Id == filter.Id))
                        && (filter.FixedAssetNo == 0 || (a.FixedAssetNo == filter.FixedAssetNo))
                        && (string.IsNullOrEmpty(filter.FixedAssetName) || (!string.IsNullOrEmpty(a.FixedAssetName) && a.FixedAssetName.Equals(filter.FixedAssetName)))
                       );

                if (items.Succeeded)
                {
                    var res = items.Data.OrderBy(r => r.Id).AsQueryable();
                    if (!string.IsNullOrEmpty(filter.StartDate) && !string.IsNullOrEmpty(filter.EndDate))
                    {
                        DateTime startDate = DateTime.ParseExact(filter.StartDate ?? "", "yyyy/MM/dd", CultureInfo.InvariantCulture);
                        DateTime endDate = DateTime.ParseExact(filter.EndDate ?? "", "yyyy/MM/dd", CultureInfo.InvariantCulture);

                        res = res.Where(r => !string.IsNullOrEmpty(r.Date1) && DateTime.ParseExact(r.Date1, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= startDate
                            && DateTime.ParseExact(r.Date1, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= endDate);
                    }

                    List<FxaAdditionsExclusionFilterDto> final = new();
                    foreach (var item in res)
                    {
                        final.Add(new FxaAdditionsExclusionFilterDto()
                        {
                            Id = item.Id,
                            Date1 = item.Date1,
                            FixedAssetNo = item.FixedAssetNo,
                            FixedAssetName = item.FixedAssetName,
                            TypeName = item.TypeName,
                            Description = item.Description,
                            CreditOrDebit = item.AdditionsExclusionTypeId == 1 ? item.Credit : item.Debit,
                            AffectAge = item.AffectAge,
                            AffectPriceAsset = item.AffectPriceAsset,
                            AffectInstallment = item.AffectInstallment,
                        });
                    }

                    return Ok(await Result<List<FxaAdditionsExclusionFilterDto>>.SuccessAsync(final));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp Message: {ex.Message}"));
            }
        }


        [HttpPost("Add")]
        public async Task<ActionResult> Add(FxaAdditionsExclusionDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(1986, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var add = await fxaServiceManager.FxaAdditionsExclusionService.Add(obj);
                if (add.Succeeded)
                {
                    //Create QR 
                    var facility = await accServiceManager.AccFacilityService.GetById(session.FacilityId);
                    if (facility.Succeeded)
                    {
                        var invPath = Path.Join(env.WebRootPath, FilesPath.SaleQrPath);
                        var qrResInvoice = QRHelper.GenerateQRforZATCA(invPath, add.Data.Id ?? 0, facility.Data.FacilityName ?? "", facility.Data.VatNumber ?? "", add.Data.Amount ?? 0, 0, add.Data.VatAmount ?? 0, add.Data.Date1 ?? "", (add.Data.Id ?? 0).ToString());
                    }
                }
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("DDLFxaAddExcType")]
        public async Task<IActionResult> DDLFxaAddExcType(int TypeId)
        {
            try
            {
                var list = new SelectList(new List<DDListItem<FxaAdditionsExclusionTypeDto>>());
                list = await ddlHelper.GetAnyLis<FxaAdditionsExclusionType, long>(t => t.IsDeleted == false && t.TypeId == TypeId, "Id", "Name");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }


        [HttpGet("GetByIdForEdit")]
        public async Task<IActionResult> GetByIdForEdit(long id)
        {
            try
            {
                var chk = await permission.HasPermission(1986, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var getItem = await fxaServiceManager.FxaAdditionsExclusionService.GetOneVW(x => x.Id == id
                    && x.IsDeleted == false && x.FacilityId == session.FacilityId);

                if (getItem.Succeeded)
                {
                    var res = getItem.Data;
                    FxaAdditionsExclusionEditDto obj = new()
                    {
                        Id = res.Id,
                        CrdAccountCode = res.AccountTypeId == 1 ? res.CrdAccountCode1 : res.CrdAccountCode,
                        AccountCode = res.AccountCode,
                        AccountTypeId = res.AccountTypeId,
                        Date1 = res.Date1,
                        FxNo = res.FixedAssetNo,
                        AffectAge = res.AffectAge ?? false,
                        AffectAgeDate = res.EndDate,
                        AffectPriceAsset = res.AffectPriceAsset ?? false,
                        AssetPrice = res.AssetPrice,
                        AffectInstallment = res.AffectInstallment ?? false,
                        InstallmentValue = res.InstallmentValue,
                        TypeId = res.TypeId,
                        Description = res.Description,
                        VatRate = res.VatRate,
                        VatAmount = res.VatAmount,
                        Amount = res.AdditionsExclusionTypeId == 1 ? res.Credit : res.Debit,
                        OperationType = res.AdditionsExclusionTypeId ?? 0
                    };

                    //get journal data
                    int docTypeId = res.AdditionsExclusionTypeId == 1 ? 103 : 104;
                    var getJournal = await accServiceManager.AccJournalMasterService.GetOne(j => j.ReferenceNo == id && j.DocTypeId == docTypeId && j.FlagDelete == false);
                    if (getJournal.Succeeded)
                    {
                        obj.JId = getJournal.Data.JId;
                        obj.JCode = getJournal.Data.JCode;
                        obj.PeriodId = getJournal.Data.PeriodId;
                    }

                    return Ok(await Result<FxaAdditionsExclusionEditDto>.SuccessAsync(obj));
                }
                return Ok(getItem);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(FxaAdditionsExclusionEditDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(1986, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var update = await fxaServiceManager.FxaAdditionsExclusionService.Update(obj);
                if (update.Succeeded)
                {
                    //Create QR 
                    var facility = await accServiceManager.AccFacilityService.GetById(session.FacilityId);
                    if (facility.Succeeded)
                    {
                        var invPath = Path.Join(env.WebRootPath, FilesPath.SaleQrPath);
                        var qrResInvoice = QRHelper.GenerateQRforZATCA(invPath,obj.Id ?? 0, facility.Data.FacilityName ?? "", facility.Data.VatNumber ?? "", obj.Amount ?? 0, 0, obj.VatAmount ?? 0, obj.Date1 ?? "", (obj.Id ?? 0).ToString());
                    }
                }
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
                var chk = await permission.HasPermission(1986, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await fxaServiceManager.FxaAdditionsExclusionService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp Message: {ex.Message}"));
            }
        }
    }
}