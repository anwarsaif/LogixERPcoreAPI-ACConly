using Logix.Application.Common;
using Logix.Application.DTOs.FXA;
using Logix.Application.DTOs.Main;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.FXA;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.FXA.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Linq;

namespace Logix.MVC.LogixAPIs.FXA
{
    public class FxaFixedAssetController : BaseFxaApiController
    {
        private readonly IFxaServiceManager fxaServiceManager;
        private readonly IAccServiceManager accServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly ISysConfigurationHelper configurationHelper;

        public FxaFixedAssetController(IFxaServiceManager fxaServiceManager,
            IAccServiceManager accServiceManager,
            IPermissionHelper permission,
            ICurrentData session,
            ILocalizationService localization,
            ISysConfigurationHelper configurationHelper)
        {
            this.fxaServiceManager = fxaServiceManager;
            this.accServiceManager = accServiceManager;
            this.permission = permission;
            this.session = session;
            this.localization = localization;
            this.configurationHelper = configurationHelper;
        }

        //Apps/FixedAssets/Transaction/Fixed_Assets ==> search
        [HttpPost("Search")]
        public async Task<ActionResult> Search(FxaFixedAssetFilterDto filter)
        {
            try
            {
                var chk = await permission.HasPermission(300, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var branchsId = session.Branches.Split(',');
                bool additionType = filter.AdditionTypeFilter == 2; //true if type = 2, otherwhise false

                filter.LocationId ??= 0;
                filter.BranchId ??= 0;
                filter.StatusId ??= 0;
                filter.ClassificationId ??= 0;
                filter.AdditionTypeFilter ??= 0;

                var items = await fxaServiceManager.FxaFixedAssetService.GetAllVW(a => a.IsDeleted == false && a.FacilityId == session.FacilityId
                    && (string.IsNullOrEmpty(filter.Code) || (!string.IsNullOrEmpty(a.Code) && a.Code.Contains(filter.Code)))
                    && (string.IsNullOrEmpty(filter.Name) || (!string.IsNullOrEmpty(a.Name) && a.Name.Contains(filter.Name)))
                    && (filter.LocationId == 0 || (a.LocationId == filter.LocationId))
                    && (filter.BranchId == 0 || (a.BranchId == filter.BranchId))
                    && ((filter.BranchId != 0) || branchsId.Contains(a.BranchId.ToString())) //Exclude any records that its branch not in user banches
                    && (filter.StatusId == 0 || (a.StatusId == filter.StatusId))
                    && (filter.ClassificationId == 0 || (a.ClassificationId == filter.ClassificationId))
                    && (string.IsNullOrEmpty(filter.OhdaEmpCode) || (!string.IsNullOrEmpty(a.OhdaEmpCode) && a.OhdaEmpCode == filter.OhdaEmpCode))
                    && (filter.AdditionTypeFilter == 0 || a.AdditionType == additionType)
                );

                if (items.Succeeded)
                {
                    var res = items.Data.OrderBy(r => r.Id).AsQueryable();
                    if (!string.IsNullOrEmpty(filter.StartDate) && !string.IsNullOrEmpty(filter.EndDate))
                    {
                        DateTime startDate = DateTime.ParseExact(filter.StartDate ?? "", "yyyy/MM/dd", CultureInfo.InvariantCulture);
                        DateTime endDate = DateTime.ParseExact(filter.EndDate ?? "", "yyyy/MM/dd", CultureInfo.InvariantCulture);

                        res = res.Where(r => !string.IsNullOrEmpty(r.PurchaseDate) && DateTime.ParseExact(r.PurchaseDate, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= startDate
                            && DateTime.ParseExact(r.PurchaseDate, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= endDate);
                    }

                    List<FxaFixedAssetVM> final = new();

                    foreach (var item in res)
                    {
                        bool shouldAddItem = (filter.TypeId == null || filter.TypeId == 0 || item.TypeId == filter.TypeId);

                        if (!shouldAddItem)
                        {
                            var typesBasedOnParents = await fxaServiceManager.FxaFixedAssetTypeService.FxaFixedAssetTypeId_DF(filter.TypeId ?? 0);
                            shouldAddItem = typesBasedOnParents.Contains((item.TypeId ?? 0).ToString());
                        }

                        if (shouldAddItem)
                        {
                            final.Add(new FxaFixedAssetVM
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
                                MainAssetName = item.MainAssetName,
                                DeprecMonthlyRate = item.DeprecMonthlyRate,
                                InstallmentValue = item.InstallmentValue
                            });
                        }
                    }
                    return Ok(await Result<List<FxaFixedAssetVM>>.SuccessAsync(final));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Search FxaFixedAssetController, MESSAGE: {ex.Message}"));
            }
        }


        //this action request when page load (page of add & edit asset)
        [HttpGet("GetProperties")]
        public async Task<ActionResult> GetProperties()
        {
            try
            {
                //get properties that indicates if cost center 2,3,4 and 5 is visible or not
                var cc2Visible = await configurationHelper.GetValue(27, session.FacilityId);
                var cc3Visible = await configurationHelper.GetValue(28, session.FacilityId);
                var cc4Visible = await configurationHelper.GetValue(29, session.FacilityId);
                var cc5Visible = await configurationHelper.GetValue(30, session.FacilityId);

                AddAssetProperties obj = new()
                {
                    CC2Visible = cc2Visible == "1",
                    CC3Visible = cc3Visible == "1",
                    CC4Visible = cc4Visible == "1",
                    CC5Visible = cc5Visible == "1",

                    CC1Title = await configurationHelper.GetValue(45, session.FacilityId),
                    CC2Title = await configurationHelper.GetValue(46, session.FacilityId),
                    CC3Title = await configurationHelper.GetValue(47, session.FacilityId),
                    CC4Title = await configurationHelper.GetValue(48, session.FacilityId),
                    CC5Title = await configurationHelper.GetValue(49, session.FacilityId)
                };

                return Ok(await Result<AddAssetProperties>.SuccessAsync(obj));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in GetProperties FxaFixedAssetController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetAssetTypeData")]
        public async Task<ActionResult> GetAssetTypeData(string code)
        {
            //This function called from add page
            try
            {
                var getParentsId = await fxaServiceManager.FxaFixedAssetTypeService.GetAll(t => t.ParentId, t => t.IsDeleted == false);
                var parentsIdArr = getParentsId.Succeeded ? getParentsId.Data : new List<long?>();

                var getItem = await fxaServiceManager.FxaFixedAssetTypeService.GetOneVW(t => t.Code == code
                && t.FacilityId == session.FacilityId && t.IsDeleted == false && !parentsIdArr.Contains(t.Id));

                if (getItem.Succeeded)
                {
                    var item = getItem.Data;
                    FxaFixedAssetTypeVm2 obj = new()
                    {
                        TypeName = item.TypeName,
                        AccountCode = item.AccountCode,
                        AccountCode2 = item.AccountCode2,
                        AccountCode3 = item.AccountCode3,
                        DeprecYearlyRate = item.DeprecYearlyRate,
                        Age = item.Age
                    };

                    return Ok(await Result<FxaFixedAssetTypeVm2>.SuccessAsync(obj));
                }
                return Ok(getItem);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in GetAssetTypeData FxaFixedAssetController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Add")]
        public async Task<ActionResult> Add(FxaFixedAssetDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(300, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var add = await fxaServiceManager.FxaFixedAssetService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add FxaFixedAssetController, MESSAGE: {ex.Message}"));
            }
        }


        [HttpGet("GetByIdForEdit")]
        public async Task<IActionResult> GetByIdForEdit(long id)
        {
            try
            {
                var chkView = await permission.HasPermission(300, PermissionType.View);
                var chkEdit = await permission.HasPermission(300, PermissionType.Edit);
                if (!chkView && !chkEdit)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var getItem = await fxaServiceManager.FxaFixedAssetService.GetOneVW(t => t.Id == id && t.FacilityId == session.FacilityId && t.IsDeleted == false);
                if (getItem.Succeeded)
                {
                    var item = getItem.Data;
                    FxaFixedAssetEditDto obj = BindData(item);

                    if (chkView && !chkEdit)
                        obj.EnableSave = false;

                    return Ok(await Result<FxaFixedAssetEditDto>.SuccessAsync(obj));
                }
                return Ok(getItem);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in GetByIdForEdit FxaFixedAssetController, MESSAGE: {ex.Message}"));
            }
        }

        private FxaFixedAssetEditDto BindData(FxaFixedAssetVw item)
        {
            FxaFixedAssetEditDto obj = new()
            {
                Id = item.Id,
                Code = item.No.ToString(),
                FxCode = item.Code,
                ImgUrl = item.ImgUrl,
                TypeCode = item.TypeCode, // TxtAssetsCode
                TypeName = item.TypeName,
                ClassificationId = item.ClassificationId,
                Name = item.Name,
                Description = item.Description,
                Amount = item.Amount,
                StartDate = item.StartDate,
                EndDate = item.EndDate,
                DeprecMonthlyRate = item.DeprecMonthlyRate,
                DeprecMethod = item.DeprecMethod,
                InstallmentValue = item.InstallmentValue,
                BranchId = item.BranchId,
                StatusId = item.StatusId,
                InitialBalance = item.InitialBalance,

                AccountCode = item.AccAccountCode,
                AccountCode2 = item.AccAccountCode2,
                AccountCode3 = item.AccAccountCode3,
                AccountName = item.AccAccountName,
                AccountName2 = item.AccAccountName2,
                AccountName3 = item.AccAccountName3,

                LocationId = item.LocationId,
                PurchaseDate = item.PurchaseDate,
                PurchaseOrder = item.PurchaseOrder,
                PurchaseAccountCode = item.PurAccAccountCode, // credit account code
                PurchaseAccountName = item.PurAccAccountName, // credit account name

                SupplierCode = item.SupplierCode,
                SupplierName = item.SupplierName,

                CcCode = item.CostCenterCode,
                CcName = item.CostCenterName,

                Annuity = item.Annuity,
                DeprecYearlyRate = item.DeprecYearlyRate,
                LastDepreciationDate = item.LastDepreciationDate,

                EmpCode = item.OhdaEmpCode,
                EmpName = item.OhdaEmpName,
                ScrapValue = item.ScrapValue,

                CcCode2 = item.CostCenterCode2,
                CcName2 = item.CostCenterName2,

                CcCode3 = item.CostCenterCode3,
                CcName3 = item.CostCenterName3,

                CcCode4 = item.CostCenterCode4,
                CcName4 = item.CostCenterName4,

                CcCode5 = item.CostCenterCode5,
                CcName5 = item.CostCenterName5,

                AdditionType = item.AdditionType,

                MainAssetNo = item.MainAssetNo,

                EnableSave = true,
            };

            return obj;
        }

        [HttpGet("FillGridViewsInEdit")]
        public async Task<IActionResult> FillGridViewsInEdit(long id)
        {
            try
            {
                var chk = await permission.HasPermission(300, PermissionType.View);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                FxaFixedAssetEditVM obj = new();

                // GridView1 (get data from FxaTransactionsAsset)
                var getFxaTransAssets = await fxaServiceManager.FxaTransactionsAssetService.GetAllVW(t => t.FixedAssetId == id && t.IsDeleted == false);
                if (getFxaTransAssets.Succeeded)
                {
                    var transAssetsResult = getFxaTransAssets.Data.OrderBy(t => t.TransDate).ToList();
                    decimal gridView1Balance = 0;
                    foreach (var transAsset in transAssetsResult)
                    {
                        gridView1Balance += (transAsset.Credit ?? 0) - (transAsset.Debet ?? 0);
                        obj.FxaTransactionsAssetsVwVms.Add(new FxaTransactionsAssetsVwVm
                        {
                            Id = transAsset.Id,
                            Code = transAsset.Code,
                            TransactionsTypeName = transAsset.TransactionsTypeName,
                            TransDate = transAsset.TransDate,
                            Description = transAsset.Description,
                            Credit = transAsset.Credit,
                            Debet = transAsset.Debet,

                            Balance = gridView1Balance
                        });

                        // those codes are duplicated in FxaFixedAssetService.Update (at begin)
                        // duplicate them to take the ReferenceNo (HidTransactionId) to get old journal for deleting and create new one
                        if (transAsset.TransTypeId == 1 || transAsset.TransTypeId == 2)
                        {
                            obj.HidTransactionId = transAsset.TransactionId;
                        }
                    }

                    var getJournal = await accServiceManager.AccJournalMasterService.GetAll(j => j.JCode, j => j.ReferenceNo != null && j.ReferenceNo == obj.HidTransactionId
                            && j.DocTypeId == 20 && j.FlagDelete == false);
                    if (getJournal.Succeeded)
                    {
                        obj.JCode = getJournal.Data.FirstOrDefault();
                    }

                    // GridView4 (get data from FxaFixedAsset)
                    var mainAssetId = id; var facilityId = session.FacilityId; var branchesId = session.Branches.Split(',');

                    var getFixedAssets = await fxaServiceManager.FxaFixedAssetService.GetAllVW(f => branchesId.Contains(f.BranchId.ToString())
                        && f.FacilityId == facilityId && f.MainAssetId == mainAssetId && f.IsDeleted == false
                    );

                    if (getFixedAssets.Succeeded)
                    {
                        foreach (var fxaItem in getFixedAssets.Data)
                        {
                            obj.FxaFixedAssetVwVms.Add(new FxaFixedAssetVwVm
                            {
                                Id = fxaItem.Id,
                                No = fxaItem.No,
                                Code = fxaItem.Code,
                                Name = fxaItem.Name,
                                TypeName = fxaItem.TypeName,
                                Description = fxaItem.Description,
                                Location = fxaItem.LocationName,
                                Amount = fxaItem.Amount,
                                PurchaseOrder = fxaItem.PurchaseOrder,
                                StartDate = fxaItem.StartDate,
                                EndDate = fxaItem.EndDate,
                            });
                        }

                        // GridView2 (get data from FxaFixedAssetTransfer)
                        var getAssetsTransfer = await fxaServiceManager.FxaFixedAssetTransferService.GetAllVW(t => t.FxaFixedAssetId == id && t.IsDeleted == false);
                        if (getAssetsTransfer.Succeeded)
                        {
                            foreach (var tansferItem in getAssetsTransfer.Data)
                            {
                                obj.FxaFixedAssetTransferVwVms.Add(new FxaFixedAssetTransferVwVm
                                {
                                    Id = tansferItem.Id,
                                    DateTransfer = tansferItem.DateTransfer,
                                    FromLocationName = tansferItem.FromLocationName,
                                    FromEmpName = tansferItem.FromEmpName,
                                    ToLocationName = tansferItem.ToLocationName,
                                    ToEmpName = tansferItem.ToEmpName
                                });
                            }

                            // GridView3 (get data from FxaAdditionsExclusion)
                            var getAdditionsExclusion = await fxaServiceManager.FxaAdditionsExclusionService.GetAllVW(a => a.FixedAssetId == id && a.IsDeleted == false);
                            if (getAdditionsExclusion.Succeeded)
                            {
                                var result = getAdditionsExclusion.Data.OrderBy(r => r.Date1);
                                decimal balance = 0;
                                foreach (var item in result)
                                {
                                    balance += (item.Credit ?? 0) - (item.Debit ?? 0);
                                    obj.FxaAdditionsExclusionVwVms.Add(new FxaAdditionsExclusionVwVm
                                    {
                                        Id = item.Id,
                                        AdditionsExclusionTypeName = item.AdditionsExclusionTypeId == 1 ? "اضافة على الاصل" : "استبعاد من الاصل",
                                        Description = item.Description,
                                        Date1 = item.Date1,
                                        Credit = item.Credit,
                                        Debit = item.Debit,
                                        Balance = balance
                                    });
                                }
                            }
                            else
                            {
                                return Ok(await Result.FailAsync($"{getAdditionsExclusion.Status.message}"));
                            }
                        }
                        else
                        {
                            return Ok(await Result.FailAsync($"{getAssetsTransfer.Status.message}"));
                        }
                    }
                    else
                    {
                        return Ok(await Result.FailAsync($"{getFixedAssets.Status.message}"));
                    }
                }
                else
                {
                    return Ok(await Result.FailAsync($"{getFxaTransAssets.Status.message}"));
                }

                return Ok(await Result<FxaFixedAssetEditVM>.SuccessAsync(obj));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in FillGridViewsInEdit FxaFixedAssetController, MESSAGE: {ex.Message}"));
            }
        }


        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(FxaFixedAssetEditDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(300, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var update = await fxaServiceManager.FxaFixedAssetService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Edit FxaFixedAssetController, MESSAGE: {ex.Message}"));
            }
        }


        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var chk = await permission.HasPermission(300, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await fxaServiceManager.FxaFixedAssetService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result<List<SysExchangeRateDto>>.FailAsync($"====== Exp in Delete FxaFixedAsset, MESSAGE: {ex.Message}"));
            }
        }
    }
}