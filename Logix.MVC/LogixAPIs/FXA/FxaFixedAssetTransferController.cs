using Logix.Application.Common;
using Logix.Application.DTOs.FXA;
using Logix.Application.DTOs.Main;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.FXA.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Logix.MVC.LogixAPIs.FXA
{
    public class FxaFixedAssetTransferController : BaseFxaApiController
    {
        private readonly IFxaServiceManager fxaServiceManager;
        private readonly IMainServiceManager mainServiceManager;
        private readonly IAccServiceManager accServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;

        public FxaFixedAssetTransferController(IFxaServiceManager fxaServiceManager,
            IMainServiceManager mainServiceManager,
            IAccServiceManager accServiceManager,
            IPermissionHelper permission,
            ICurrentData session,
            ILocalizationService localization)
        {
            this.fxaServiceManager = fxaServiceManager;
            this.mainServiceManager = mainServiceManager;
            this.accServiceManager = accServiceManager;
            this.permission = permission;
            this.session = session;
            this.localization = localization;
        }

        #region ============================================= Search & Delete =============================================
        [HttpPost("Search")]
        public async Task<ActionResult> Search(FxaFixedAssetTransferFilterDto filter)
        {
            try
            {
                var chk = await permission.HasPermission(799, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                //make null number = 0
                filter.FromLocationId ??= 0;
                filter.ToLocationId ??= 0;
                filter.FxaFixedAssetId ??= 0;

                var items = await fxaServiceManager.FxaFixedAssetTransferService.GetAllVW(t => t.IsDeleted == false
                    && (string.IsNullOrEmpty(filter.Code) || t.Code == filter.Code)
                    && (string.IsNullOrEmpty(filter.Name) || (!string.IsNullOrEmpty(t.Name) && t.Name.ToLower().Contains(filter.Name.ToLower())))
                    && (filter.FromLocationId == 0 || t.FromLocationId == filter.FromLocationId)
                    && (filter.ToLocationId == 0 || t.ToLocationId == filter.ToLocationId)
                    && (string.IsNullOrEmpty(filter.FromEmpCode) || (!string.IsNullOrEmpty(t.FromEmpCode) && t.FromEmpCode.Contains(filter.FromEmpCode)))
                    && (string.IsNullOrEmpty(filter.ToEmpCode) || (!string.IsNullOrEmpty(t.ToEmpCode) && t.ToEmpCode.Contains(filter.ToEmpCode)))
                //&& (filter.FxaFixedAssetId == 0 || t.FxaFixedAssetId == filter.FxaFixedAssetId)
                );

                if (items.Succeeded)
                {
                    var res = items.Data.OrderBy(r => r.Id).AsQueryable();
                    if (!string.IsNullOrEmpty(filter.StartDate) && !string.IsNullOrEmpty(filter.EndDate))
                    {
                        DateTime startDate = DateTime.ParseExact(filter.StartDate ?? "", "yyyy/MM/dd", CultureInfo.InvariantCulture);
                        DateTime endDate = DateTime.ParseExact(filter.EndDate ?? "", "yyyy/MM/dd", CultureInfo.InvariantCulture);

                        res = res.Where(r => !string.IsNullOrEmpty(r.DateTransfer) && DateTime.ParseExact(r.DateTransfer, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= startDate
                            && DateTime.ParseExact(r.DateTransfer, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= endDate);
                    }

                    List<FxaFixedAssetTransferSearchVM> final = new();

                    foreach (var item in res)
                    {
                        final.Add(new FxaFixedAssetTransferSearchVM
                        {
                            Id = item.Id,
                            Code = item.Code,
                            Name = item.Name,
                            DateTransfer = item.DateTransfer,
                            FromEmpName = item.FromEmpName,
                            ToEmpName = item.ToEmpName,
                            FromLocationName = item.FromLocationName,
                            ToLocationName = item.ToLocationName,
                        });
                    }
                    return Ok(await Result<List<FxaFixedAssetTransferSearchVM>>.SuccessAsync(final));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Search FxaFixedAssetTransferController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var chk = await permission.HasPermission(799, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await fxaServiceManager.FxaFixedAssetTransferService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result<List<SysExchangeRateDto>>.FailAsync($"====== Exp in Delete FxaFixedAsset, MESSAGE: {ex.Message}"));
            }
        }

        #endregion ========================================= End Search & Delete ============================================

        #region =================================================== Add ===================================================
        [HttpGet("GetAssetByNo")]
        public async Task<IActionResult> GetAssetByNo(long no, long facilityId)
        {
            //This function called from add and edit page
            //This function used to get data of asset that user need to transfer (when user write number of asset in asset popup)
            try
            {
                facilityId = facilityId == 0 ? session.FacilityId : facilityId;

                var getItem = await fxaServiceManager.FxaFixedAssetService.GetOneVW(f => f.No == no && f.FacilityId == facilityId
                        && f.StatusId == 1 && f.IsDeleted == false);

                if (getItem.Succeeded)
                {
                    var item = getItem.Data;
                    FixedAssetDataForTransferVm obj = new()
                    {
                        FromBranchId = item.BranchId,
                        FromFacilityId = item.FacilityId,
                        CcCode = item.CostCenterCode,
                        CcName = item.CostCenterName,
                        FromEmpCode = item.OhdaEmpCode,
                        FromEmpName = item.OhdaEmpName,
                        FromLocationId = item.LocationId
                    };

                    return Ok(await Result<FixedAssetDataForTransferVm>.SuccessAsync(obj));
                }
                return Ok(getItem);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in GetAssetByNo FxaFixedAssetTransferController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Add")]
        public async Task<ActionResult> Add(FxaFixedAssetTransferDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(799, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var add = await fxaServiceManager.FxaFixedAssetTransferService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add FxaFixedAssetTransferController, MESSAGE: {ex.Message}"));
            }
        }
        #endregion =============================================== End Add =================================================

        #region =================================================== Edit ===================================================
        [HttpGet("GetByIdForEdit")]
        public async Task<IActionResult> GetByIdForEdit(long id)
        {
            try
            {
                var chk = await permission.HasPermission(799, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var getItem = await fxaServiceManager.FxaFixedAssetTransferService.GetOneVW(t => t.Id == id && t.IsDeleted == false);
                if (getItem.Succeeded)
                {
                    var item = getItem.Data;
                    FxaFixedAssetTransferEditDto obj = new()
                    {
                        Id = item.Id,
                        FxaFixedAssetNo = item.No,
                        FxaFixedAssetName = item.Name,
                        DateTransfer = item.DateTransfer,
                        FromFacilityId = item.FromFacilityId,
                        Note = item.Note,
                        ToFacilityId = item.ToFacilityId,
                        FromBranchId = item.FromBranchId,
                        ToBranchId = item.ToBranchId,
                        FromCcCode = item.CostCenterCode,
                        FromCcName = item.CostCenterName,
                        ToCcCode = item.CostCenterCode2,
                        ToCcName = item.CostCenterName2,
                        FromEmpCode = item.FromEmpCode,
                        FromEmpName = item.FromEmpName,
                        ToEmpCode = item.ToEmpCode,
                        ToEmpName = item.ToEmpName,
                        FromLocationId = item.FromLocationId,
                        ToLocationId = item.ToLocationId,
                    };

                    return Ok(await Result<FxaFixedAssetTransferEditDto>.SuccessAsync(obj));
                }
                return Ok(getItem);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in GetByIdForEdit FxaFixedAssetTransferController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(FxaFixedAssetTransferEditDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(799, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var update = await fxaServiceManager.FxaFixedAssetTransferService.Update(obj);
                return update.Succeeded ? Ok(await Result.SuccessAsync()) : Ok(await Result.FailAsync(update.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Edit FxaFixedAssetTransferController, MESSAGE: {ex.Message}"));
            }
        }
        #endregion =============================================== End Edit ==================================================

        #region ================================= Transfer assets from Employee to another =================================
        [HttpGet("GetAssetsByEmpCode")]
        public async Task<IActionResult> GetAssetsByEmpCode(string empCode, long facilityId)
        {
            try
            {
                facilityId = facilityId == 0 ? session.FacilityId : facilityId;

                long empId = 0;
                var getEmpId = await mainServiceManager.InvestEmployeeService.GetOne(e => e.Id, e => !string.IsNullOrEmpty(e.EmpId) && e.EmpId == empCode && e.IsDeleted == false);
                if (!getEmpId.Succeeded)
                    return Ok(getEmpId);

                empId = getEmpId.Data ?? 0;

                var getEmpAssets = await fxaServiceManager.FxaFixedAssetService.GetAllVW(t => t.OhdaEmpId == empId && t.StatusId == 1 && t.FacilityId == facilityId);
                if (getEmpAssets.Succeeded)
                {
                    var items = getEmpAssets.Data;

                    List<EmployeeFixedAssetsVM> final = new();
                    foreach (var item in items)
                    {
                        final.Add(new EmployeeFixedAssetsVM()
                        {
                            Id = item.Id,
                            No = item.No,
                            Code = item.Code,
                            Name = item.Name,
                            Description = item.Description,
                            Location = item.LocationName,
                            Amount = item.Amount,
                            StartDate = item.StartDate,
                            EndDate = item.EndDate,
                            DeprecMethodName = item.DeprecMethodName,
                            DeprecMonthlyRate = item.DeprecMonthlyRate,
                            InstallmentValue = item.InstallmentValue,

                            FacilityId = item.FacilityId,
                            BranchId = item.BranchId,
                            CostCenterCode = item.CostCenterCode,
                            LocationId = item.LocationId,
                            IsSelected = false
                        });
                    }
                    return Ok(await Result<List<EmployeeFixedAssetsVM>>.SuccessAsync(final));
                }
                return Ok(getEmpAssets);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in GetAssetsByEmpCode FxaFixedAssetTransferController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Add2")]
        public async Task<ActionResult> Add2(FxaFixedAssetTransferDto2 obj)
        {
            //this function used to transfer assets from an employee to another employee
            try
            {
                var chk = await permission.HasPermission(799, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var add = await fxaServiceManager.FxaFixedAssetTransferService.Add2(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add2 FxaFixedAssetTransferController, MESSAGE: {ex.Message}"));
            }
        }
        #endregion ==================================== End Transfer ==============================================

    }
}