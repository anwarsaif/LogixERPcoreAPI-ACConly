using iText.Commons.Bouncycastle.Asn1.X509;
using Logix.Application.Common;
using Logix.Application.DTOs.FXA;
using Logix.Application.DTOs.Main;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Services;
using Logix.Application.Wrapper;
using Logix.Domain.FXA;
using Logix.Domain.Main;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.FXA.ViewModels.ReportVm;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Operations;
using System.Globalization;

namespace Logix.MVC.LogixAPIs.FXA
{
    public class FxaReportsController : BaseFxaApiController
    {
        private readonly IFxaServiceManager fxaServiceManager;
        private readonly IAccServiceManager accServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IApiDDLHelper ddlHelper;

        public FxaReportsController(IFxaServiceManager fxaServiceManager,
            IAccServiceManager accServiceManager,
            IPermissionHelper permission,
            ICurrentData session,
            ILocalizationService localization,
            IApiDDLHelper ddlHelper)
        {
            this.fxaServiceManager = fxaServiceManager;
            this.accServiceManager = accServiceManager;
            this.permission = permission;
            this.session = session;
            this.localization = localization;
            this.ddlHelper = ddlHelper;
        }

        #region ================================================== Asset purchased & saled (exclude) ==================================================

        [HttpPost("GetPurchaseAssetReport")]
        public async Task<ActionResult> GetPurchaseAssetReport(PurchaseAssetReportFilter filter)
        {
            // [FXA_RPFixedAsset_SP] @CMDTYPE = 5 (DB :Logix_2024_07_22)
            try
            {
                var chk = await permission.HasPermission(1239, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                //if number property is null => convert it to 0
                filter.FxNo ??= 0; filter.LocationId ??= 0; filter.BranchId ??= 0; filter.StatusId ??= 0; filter.TypeId ??= 0;

                var items = await fxaServiceManager.FxaFixedAssetService.GetAllVW(x => x.IsDeleted == false && x.FacilityId == session.FacilityId
                    && (filter.FxNo == 0 || x.No == filter.FxNo)
                    && (string.IsNullOrEmpty(filter.FxName) || (x.Name != null && x.Name.ToLower().Contains(filter.FxName.ToLower())))
                    && (filter.LocationId == 0 || x.LocationId == filter.LocationId)
                    && (filter.BranchId == 0 || x.BranchId == filter.BranchId)
                    && (filter.StatusId == 0 || x.StatusId == filter.StatusId)
                    && (filter.TypeId == 0 || x.TypeId == filter.TypeId)
                    && (string.IsNullOrEmpty(filter.Description) || (!string.IsNullOrEmpty(x.Description) && x.Description.Contains(filter.Description)))
                );

                if (items.Succeeded)
                {
                    var res = items.Data.OrderBy(r => r.No).AsQueryable();
                    if (!string.IsNullOrEmpty(filter.StartDate) && !string.IsNullOrEmpty(filter.EndDate))
                    {
                        DateTime startDate = DateTime.ParseExact(filter.StartDate ?? "", "yyyy/MM/dd", CultureInfo.InvariantCulture);
                        DateTime endDate = DateTime.ParseExact(filter.EndDate ?? "", "yyyy/MM/dd", CultureInfo.InvariantCulture);

                        res = res.Where(r => !string.IsNullOrEmpty(r.PurchaseDate) && DateTime.ParseExact(r.PurchaseDate, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= startDate
                            && DateTime.ParseExact(r.PurchaseDate, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= endDate);
                    }

                    List<PurchaseAssetReportVm> final = new();
                    foreach (var item in res)
                    {
                        PurchaseAssetReportVm obj = new()
                        {
                            No = item.No,
                            Name = item.Name,
                            BraName = item.BraName,
                            TypeName = item.TypeName,
                            LocationName = item.LocationName,
                            AccAccountCode = item.AccAccountCode,
                            AccAccountName = item.AccAccountName,
                            PurchaseDate = item.PurchaseDate,
                            Amount = item.Amount,
                            Description = item.Description,
                        };

                        //get journal code
                        var getJournal = await accServiceManager.AccJournalMasterService.GetOne(j => j.JCode,
                                j => j.ReferenceNo == item.Id && j.DocTypeId == 20 && j.FlagDelete == false);
                        if (getJournal.Succeeded)
                            obj.JCode = getJournal.Data;

                        final.Add(obj);
                    }

                    return Ok(await Result<List<PurchaseAssetReportVm>>.SuccessAsync(final));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp Message: {ex.Message}"));
            }
        }

        [HttpPost("GetSaleAssetReport")]
        public async Task<ActionResult> GetSaleAssetReport(SaleAssetReportFilter filter)
        {
            // [FXA_RPFixedAsset_SP] @CMDTYPE = 6 (DB :Logix_2024_07_22)
            try
            {
                var chk = await permission.HasPermission(1240, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                //if number property is null => convert it to 0
                filter.LocationId ??= 0; filter.BranchId ??= 0;

                var items = await fxaServiceManager.FxaTransactionService.GetAllVW(x => x.IsDeleted == false && x.FacilityId == session.FacilityId
                    && (filter.LocationId == 0 || x.LocationId == filter.LocationId)
                    && (filter.BranchId == 0 || x.BranchId == filter.BranchId)
                );

                if (items.Succeeded)
                {
                    var res = items.Data.OrderBy(r => r.No).AsQueryable();
                    if (!string.IsNullOrEmpty(filter.StartDate) && !string.IsNullOrEmpty(filter.EndDate))
                    {
                        DateTime startDate = DateTime.ParseExact(filter.StartDate ?? "", "yyyy/MM/dd", CultureInfo.InvariantCulture);
                        DateTime endDate = DateTime.ParseExact(filter.EndDate ?? "", "yyyy/MM/dd", CultureInfo.InvariantCulture);

                        res = res.Where(r => !string.IsNullOrEmpty(r.PurchaseDate) && DateTime.ParseExact(r.PurchaseDate, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= startDate
                            && DateTime.ParseExact(r.PurchaseDate, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= endDate);
                    }

                    List<SaleAssetReportVm> final = new();
                    foreach (var item in res)
                    {
                        bool matchCode = true;
                        bool matchName = true;

                        if (!string.IsNullOrEmpty(filter.FxCode))
                        {
                            var chkCode = await fxaServiceManager.FxaTransactionsAssetService.GetAllVW(x => x.TransactionId == item.Id
                                && x.FxCode == filter.FxCode && x.IsDeleted == false);

                            if (chkCode.Succeeded)
                            {
                                if (!(chkCode.Data.Any()))
                                    matchCode = false;
                            }
                            else
                                return Ok(await Result.FailAsync(chkCode.Status.message));
                        }

                        if (!string.IsNullOrEmpty(filter.FxName))
                        {
                            var chkName = await fxaServiceManager.FxaTransactionsAssetService.GetAllVW(x => x.TransactionId == item.Id && x.IsDeleted == false
                                 && (x.FxName != null && x.FxName.ToLower().Contains(filter.FxName.ToLower())));

                            if (chkName.Succeeded)
                            {
                                if (!(chkName.Data.Any()))
                                    matchName = false;
                            }
                            else
                                return Ok(await Result.FailAsync(chkName.Status.message));
                        }

                        if (matchCode && matchName)
                        {
                            SaleAssetReportVm obj = new()
                            {
                                FxNo = item.FxNo,
                                FxName = item.FxName,
                                AccAccountCode = item.AccAccountCode,
                                AccAccountName = item.AccAccountName,
                                TransDate = item.PurchaseDate,
                                Total = item.Total,
                            };

                            //get journal code
                            var getJournal = await accServiceManager.AccJournalMasterService.GetOne(j => j.JCode,
                                    j => j.ReferenceNo == item.Id && j.DocTypeId == 20 && j.FlagDelete == false);
                            if (getJournal.Succeeded)
                                obj.JCode = getJournal.Data;

                            final.Add(obj);
                        }
                    }

                    return Ok(await Result<List<SaleAssetReportVm>>.SuccessAsync(final));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp Message: {ex.Message}"));
            }
        }

        #endregion ================================================ End Asset purchased & saled (exclude) ================================================


        #region ====================================================== Asset deprecaiation report ======================================================
        [HttpPost("GetDepreciationReport")]
        public async Task<ActionResult> GetDepreciationReport(FxaDeprecReportFilter filter)
        {
            // تقرير بالاصول
            try
            {
                var chk = await permission.HasPermission(1239, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (string.IsNullOrEmpty(filter.StartDate) || string.IsNullOrEmpty(filter.EndDate))
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var result = await GetDepreciationData(filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp Message: {ex.Message}"));
            }
        }

        [HttpPost("GetDepreciationReport3")]
        public async Task<ActionResult> GetDepreciationReport3(FxaDeprecReportFilter filter)
        {
            // تقرير بالاصول حسب المستوى
            try
            {
                var chk = await permission.HasPermission(2151, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (string.IsNullOrEmpty(filter.StartDate) || string.IsNullOrEmpty(filter.EndDate))
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var result = await GetDepreciationData(filter);
                // get father and grand type name
                if (result.Succeeded)
                {
                    var res = result.Data;
                    foreach (var item in res)
                    {
                        long fatherTypeId = 0; string fatherTypeName = "";
                        long grandTypeId = 0; string grandTypeName = "";

                        var getFatherType = await fxaServiceManager.FxaFixedAssetTypeService.GetOne(t => t.Id == item.TypeId);
                        if (getFatherType.Succeeded)
                        {
                            fatherTypeId = getFatherType.Data.ParentId ?? 0;
                            fatherTypeName = getFatherType.Data.TypeName ?? "";
                        }

                        if (fatherTypeId > 0)
                        {
                            var getGrandType = await fxaServiceManager.FxaFixedAssetTypeService.GetOne(t => t.Id == fatherTypeId);
                            if (getGrandType.Succeeded)
                            {
                                grandTypeId = getGrandType.Data.ParentId ?? 0;
                                grandTypeName = getGrandType.Data.TypeName ?? "";
                            }
                        }

                        //if no parent
                        if (fatherTypeId == 0)
                            fatherTypeName = item.TypeName ?? "";

                        if (grandTypeId == 0)
                            grandTypeName = fatherTypeName;

                        item.FatherTypeName = fatherTypeName;
                        item.GrandTypeName = grandTypeName;
                    }

                    return Ok(await Result<List<FxaDeprecData>>.SuccessAsync(res));
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp Message: {ex.Message}"));
            }
        }

        private async Task<IResult<List<FxaDeprecData>>> GetDepreciationData(FxaDeprecReportFilter filter)
        {
            try
            {
                // [FXA_FixedAsset_SP] @CMDTYPE = 14 (DB :Logix_2024_07_22)
                // this function used in (GetDepreciationReport, GetDepreciationReport3)

                DateTime startDate = DateTime.ParseExact(filter.StartDate ?? "", "yyyy/MM/dd", CultureInfo.InvariantCulture);
                DateTime endDate = DateTime.ParseExact(filter.EndDate ?? "", "yyyy/MM/dd", CultureInfo.InvariantCulture);

                var branchsId = session.Branches.Split(',');
                bool additionType = filter.AdditionTypeFilter == 2; //true if type = 2, otherwhise false

                //if number property is null => convert it to 0
                filter.LocationId ??= 0; filter.BranchId ??= 0; filter.StatusId ??= 0; filter.ClassificationId ??= 0; filter.AdditionTypeFilter ??= 0;

                var getFxaIds = await fxaServiceManager.FxaTransactionsAssetService.GetAllVW(f => f.TransTypeId == 4 && f.IsDeleted == false);
                if (!getFxaIds.Succeeded)
                    return await Result<List<FxaDeprecData>>.FailAsync(getFxaIds.Status.message);

                var fxaIdsResult = getFxaIds.Data;
                fxaIdsResult = fxaIdsResult.Where(r => !string.IsNullOrEmpty(r.TransDate) && DateTime.ParseExact(r.TransDate, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= startDate
                       && DateTime.ParseExact(r.TransDate, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= endDate);

                var fxaIds = getFxaIds.Data.Select(x => x.FixedAssetId ?? 0).ToArray();
                List<int> acceptStatus = new() { 0, 1, 2 };

                var items = await fxaServiceManager.FxaFixedAssetService.GetAllVW(a => a.IsDeleted == false && a.FacilityId == session.FacilityId
                    && (string.IsNullOrEmpty(filter.Code) || (!string.IsNullOrEmpty(a.Code) && a.Code == filter.Code))
                    && (string.IsNullOrEmpty(filter.Name) || (!string.IsNullOrEmpty(a.Name) && a.Name.Contains(filter.Name)))
                    && (filter.LocationId == 0 || (a.LocationId == filter.LocationId))
                    && (filter.BranchId == 0 || (a.BranchId == filter.BranchId))
                    && (filter.ClassificationId == 0 || (a.ClassificationId == filter.ClassificationId))
                    && ((filter.BranchId != 0) || branchsId.Contains(a.BranchId.ToString())) //Exclude any records that its branch not in user banches
                    && (filter.StatusId == 0 || (a.StatusId == filter.StatusId))
                    && (acceptStatus.Contains(filter.StatusId ?? 0) && (a.StatusId == 1 || fxaIds.Contains(a.Id)))
                    && (filter.AdditionTypeFilter == 0 || a.AdditionType == additionType)
                    && (string.IsNullOrEmpty(filter.Description) || (!string.IsNullOrEmpty(a.Description) && a.Description.Contains(filter.Description)))
                );

                if (items.Succeeded)
                {
                    var res = items.Data;

                    List<FxaDeprecData> final = new();

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
                            FxaDeprecData obj = new()
                            {
                                Id = item.Id,
                                PurchaseDate = item.PurchaseDate,
                                No = item.No,
                                Code = item.Code,
                                Name = item.Name,
                                Description = item.Description,
                                LocationName = item.LocationName,
                                TypeId = item.TypeId,
                                TypeName = item.TypeName,
                                ClassificationName = item.ClassificationId == 1 ? "اصول" : (item.ClassificationId == 2 ? "اصول غير ملموسة" : ""),
                                AdditionType = item.AdditionType,
                                MainAssetName = item.MainAssetName,
                                TypeCode = item.TypeCode,
                                StartDate = item.StartDate,
                                EndDate = item.EndDate,
                                DeprecMethodName = item.DeprecMethodName,
                                DeprecMonthlyRate = item.DeprecMonthlyRate,
                                InstallmentValue = item.InstallmentValue,
                            };

                            var getTransAssetVw = await fxaServiceManager.FxaTransactionsAssetService.GetAllVW(f => f.FixedAssetId == item.Id
                                && f.IsDeleted == false && f.IsDeletedFx == false);
                            if (!getTransAssetVw.Succeeded)
                                return await Result<List<FxaDeprecData>>.FailAsync(getTransAssetVw.Status.message);

                            var TransAssetVwData = getTransAssetVw.Data.AsQueryable();

                            // get DiscardTbl (outer apply)
                            var DiscardTbl = TransAssetVwData.Where(x => x.TransTypeId == 4).OrderBy(x => x.TransDate);
                            string DiscardDate = DiscardTbl.Select(x => x.TransDate ?? "").FirstOrDefault() ?? "";
                            decimal DiscardAmount = DiscardTbl.Select(x => x.Total ?? 0).FirstOrDefault();

                            decimal amount = 0; decimal sumCredit = 0; decimal sumDebit = 0;

                            if (DateTime.ParseExact(item.PurchaseDate ?? "", "yyyy/MM/dd", CultureInfo.InvariantCulture) < startDate)
                                amount = item.Amount ?? 0;

                            sumCredit = TransAssetVwData
                                .Where(x => x.TransTypeId == 8 && DateTime.ParseExact(x.TransDate ?? "", "yyyy/MM/dd", CultureInfo.InvariantCulture) < startDate)
                                .Sum(x => x.Credit ?? 0);

                            sumDebit = TransAssetVwData
                                .Where(x => x.TransTypeId == 8 && DateTime.ParseExact(x.TransDate ?? "", "yyyy/MM/dd", CultureInfo.InvariantCulture) < startDate)
                                .Sum(x => x.Debet ?? 0);

                            obj.Amount = amount + sumCredit - sumDebit;
                            amount = 0; sumCredit = 0; sumDebit = 0;

                            // Get Additions
                            List<int> myTransTypeId = new() { 1, 2, 3, 8 };
                            obj.Additions = TransAssetVwData
                                .Where(x => myTransTypeId.Contains(x.TransTypeId ?? 0)
                                    && DateTime.ParseExact(x.TransDate ?? "", "yyyy/MM/dd", CultureInfo.InvariantCulture) >= startDate
                                    && DateTime.ParseExact(x.TransDate ?? "", "yyyy/MM/dd", CultureInfo.InvariantCulture) <= endDate)
                                .Sum(x => x.Credit ?? 0);

                            // Get Discards
                            if (!string.IsNullOrEmpty(DiscardDate) && DateTime.ParseExact(DiscardDate, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= startDate
                                    && DateTime.ParseExact(DiscardDate, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= endDate
                               )
                                amount = item.Amount ?? 0;

                            myTransTypeId = new() { 1, 2, 3, 8 };
                            sumDebit = TransAssetVwData
                               .Where(x => myTransTypeId.Contains(x.TransTypeId ?? 0)
                                    && DateTime.ParseExact(x.TransDate ?? "", "yyyy/MM/dd", CultureInfo.InvariantCulture) >= startDate
                                    && DateTime.ParseExact(x.TransDate ?? "", "yyyy/MM/dd", CultureInfo.InvariantCulture) <= endDate)
                               .Sum(x => x.Debet ?? 0);

                            obj.Discards = amount + sumDebit;
                            amount = 0; sumDebit = 0;

                            // Get DepreciationOld
                            sumDebit = TransAssetVwData
                               .Where(x => x.TransTypeId == 5
                                    && DateTime.ParseExact(x.TransDate ?? "", "yyyy/MM/dd", CultureInfo.InvariantCulture) < startDate)
                               .Sum(x => x.Debet ?? 0);

                            obj.DepreciationOld = sumDebit;
                            sumDebit = 0;

                            // Get DepreciationNow 
                            sumDebit = TransAssetVwData
                               .Where(x => x.TransTypeId == 5
                                    && DateTime.ParseExact(x.TransDate ?? "", "yyyy/MM/dd", CultureInfo.InvariantCulture) >= startDate
                                    && DateTime.ParseExact(x.TransDate ?? "", "yyyy/MM/dd", CultureInfo.InvariantCulture) <= endDate)
                               .Sum(x => x.Debet ?? 0);

                            obj.DepreciationNow = sumDebit;
                            sumDebit = 0;

                            // Get DepreciateDiscardation
                            if (obj.Discards > 0)
                                obj.DepreciateDiscardation = obj.DepreciationOld + obj.DepreciationNow;

                            // Get ProfitAndLoss
                            var getRevaluation = await fxaServiceManager.FxaTransactionsRevaluationService.GetAllVW(r => r.FixedAssetId == item.Id
                            && r.TransTypeId == 8 && r.IsDeleted == false);

                            var revaluationData = getRevaluation.Data.AsQueryable();
                            decimal sumProfit_Loss = revaluationData
                                .Where(x => DateTime.ParseExact(x.TransDate ?? "", "yyyy/MM/dd", CultureInfo.InvariantCulture) >= startDate
                                    && DateTime.ParseExact(x.TransDate ?? "", "yyyy/MM/dd", CultureInfo.InvariantCulture) <= endDate)
                                .Sum(x => x.ProfitAndLoss ?? 0);
                            obj.ProfitAndLoss = sumProfit_Loss;


                            final.Add(obj);
                        }
                    }

                    return await Result<List<FxaDeprecData>>.SuccessAsync(final);
                }

                return await Result<List<FxaDeprecData>>.FailAsync(items.Status.message);
            }
            catch (Exception ex)
            {
                return await Result<List<FxaDeprecData>>.FailAsync(ex.Message);
            }
        }


        [HttpPost("GetDepreciationReport2")]
        public async Task<ActionResult> GetDepreciationReport2(FxaDeprec2ReportFilter filter)
        {
            // تقرير بالاصول 2
            // [FXA_FixedAsset_SP2] @CMDTYPE = 1 (DB :Logix_2024_07_22)
            try
            {
                var chk = await permission.HasPermission(2008, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                //if number property is null => convert it to 0
                filter.BranchId ??= 0; filter.No ??= 0; filter.TypeId ??= 0; filter.TypeId2 ??= 0; filter.TypeId3 ??= 0;
                filter.ClassificationId ??= 0; filter.StatusId ??= 0; filter.LocationId ??= 0;

                var branchsId = session.Branches.Split(',');
                List<int> acceptStatus = new() { 0, 1, 2 };

                var items = await fxaServiceManager.FxaFixedAssetService.GetAllVW2(a => a.IsDeleted == false && a.FacilityId == session.FacilityId
                    && (filter.No == 0 || (a.No == filter.No) || (a.No.ToString() == filter.Code))
                    && (string.IsNullOrEmpty(filter.Code) || (!string.IsNullOrEmpty(a.Code) && a.Code == filter.Code) || (a.No.ToString() == filter.Code))
                    && (string.IsNullOrEmpty(filter.Name) || (!string.IsNullOrEmpty(a.Name) && a.Name.Contains(filter.Name)))
                    && (filter.LocationId == 0 || (a.LocationId == filter.LocationId))
                    && (filter.BranchId == 0 || (a.BranchId == filter.BranchId))
                    && ((filter.BranchId != 0) || branchsId.Contains(a.BranchId.ToString()))
                    && (filter.ClassificationId == 0 || (a.ClassificationId == filter.ClassificationId))
                    && (filter.StatusId == 0 || (a.StatusId == filter.StatusId))
                    && (acceptStatus.Contains(a.StatusId ?? 0) && a.StatusId == 1)
                    && (string.IsNullOrEmpty(filter.Description) || (!string.IsNullOrEmpty(a.Description) && a.Description.Contains(filter.Description)))

                    && (
                        (filter.TypeId != 0 || filter.TypeId2 != 0)
                        || (
                            filter.TypeId3 == 0
                            || a.TypeId == filter.TypeId3
                            || filter.TypeId2 == filter.TypeId3
                            || a.TypeId3 == filter.TypeId3
                            || a.ParentId == filter.TypeId3
                            || a.ParentId2 == filter.TypeId3
                            || a.ParentId3 == filter.TypeId3
                        )
                    )

                    && (
                        (filter.TypeId != 0)
                        || (
                            filter.TypeId2 == 0
                            || a.TypeId2 == filter.TypeId2
                            || a.ParentId == filter.TypeId2
                            || a.ParentId2 == filter.TypeId2
                            || a.ParentId3 == filter.TypeId2
                        )
                    )

                    && (
                            filter.TypeId == 0
                            || a.TypeId == filter.TypeId
                            || a.ParentId == filter.TypeId
                            || a.ParentId2 == filter.TypeId
                            || a.ParentId3 == filter.TypeId
                    )
                );

                if (items.Succeeded)
                {
                    var res = items.Data;
                    if (!string.IsNullOrEmpty(filter.StartDate) && !string.IsNullOrEmpty(filter.EndDate))
                    {
                        DateTime startDate = DateTime.ParseExact(filter.StartDate ?? "", "yyyy/MM/dd", CultureInfo.InvariantCulture);
                        DateTime endDate = DateTime.ParseExact(filter.EndDate ?? "", "yyyy/MM/dd", CultureInfo.InvariantCulture);

                        res = res.Where(r => !string.IsNullOrEmpty(r.PurchaseDate) && DateTime.ParseExact(r.PurchaseDate, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= startDate
                            && DateTime.ParseExact(r.PurchaseDate, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= endDate);
                    }

                    List<FxaDeprec2ReportVm> final = new();
                    foreach (var item in res)
                    {
                        final.Add(new FxaDeprec2ReportVm()
                        {
                            No = item.No,
                            Code = item.Code,
                            Name = item.Name,
                            TypeName3 = item.TypeName3,
                            TypeName2 = item.TypeName2,
                            TypeName = item.TypeName,
                            ClassificationName = item.ClassificationName,
                            Description = item.Description,
                            BraName = item.BraName,
                            LocationName = item.LocationName,
                            Amount = item.Amount,
                            PurchaseDate = item.PurchaseDate
                        });
                    }

                    return Ok(await Result<List<FxaDeprec2ReportVm>>.SuccessAsync(final));
                }

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp Message: {ex.Message}"));
            }
        }


        [HttpPost("GetDepreciationReportByCategory")]
        public async Task<ActionResult> GetDepreciationReportByCategory(FxaDeprecByCategoryFilter filter)
        {
            // تقرير بالاصول حسب الفئات
            // [FXA_RPFixedAsset_SP] @CMDTYPE = 2 (DB :Logix_2024_07_22)
            try
            {
                var chk = await permission.HasPermission(968, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (string.IsNullOrEmpty(filter.StartDate) || string.IsNullOrEmpty(filter.EndDate))
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                DateTime startDate = DateTime.ParseExact(filter.StartDate ?? "", "yyyy/MM/dd", CultureInfo.InvariantCulture);
                DateTime endDate = DateTime.ParseExact(filter.EndDate ?? "", "yyyy/MM/dd", CultureInfo.InvariantCulture);

                //if number property is null => convert it to 0
                filter.LocationId ??= 0; filter.BranchId ??= 0; filter.TypeId ??= 0; filter.StatusId ??= 0;

                var getFxaIds = await fxaServiceManager.FxaTransactionsAssetService.GetAllVW(f => f.TransTypeId == 4 && f.IsDeleted == false);
                if (!getFxaIds.Succeeded)
                    return Ok(getFxaIds);

                var fxaIdsResult = getFxaIds.Data;
                fxaIdsResult = fxaIdsResult.Where(r => !string.IsNullOrEmpty(r.TransDate) && DateTime.ParseExact(r.TransDate, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= startDate
                       && DateTime.ParseExact(r.TransDate, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= endDate);

                var fxaIds = getFxaIds.Data.Select(x => x.FixedAssetId ?? 0).ToArray();
                List<int> acceptStatus = new() { 0, 1, 2 };

                var items = await fxaServiceManager.FxaFixedAssetService.GetAllVW(a => a.IsDeleted == false && a.FacilityId == session.FacilityId
                    && (string.IsNullOrEmpty(filter.Code) || (!string.IsNullOrEmpty(a.Code) && a.Code == filter.Code))
                    && (string.IsNullOrEmpty(filter.Name) || (!string.IsNullOrEmpty(a.Name) && a.Name.Contains(filter.Name)))
                    && (filter.LocationId == 0 || (a.LocationId == filter.LocationId))
                    && (filter.BranchId == 0 || (a.BranchId == filter.BranchId))
                    && (filter.TypeId == 0 || (a.TypeId == filter.TypeId))
                    && (filter.StatusId == 0 || (a.StatusId == filter.StatusId))
                    && (acceptStatus.Contains(filter.StatusId ?? 0) && (a.StatusId == 1 || fxaIds.Contains(a.Id)))
                    && (string.IsNullOrEmpty(filter.Description) || (!string.IsNullOrEmpty(a.Description) && a.Description.Contains(filter.Description)))
                );

                if (items.Succeeded)
                {
                    var res = items.Data;
                    List<FxaDeprecByCategoryData> final = new();

                    foreach (var item in res)
                    {
                        FxaDeprecByCategoryData obj = new()
                        {
                            Id = item.Id,
                            PurchaseDate = item.PurchaseDate,
                            No = item.No,
                            Code = item.Code,
                            Name = item.Name,
                            Description = item.Description,
                            LocationName = item.LocationName,
                            TypeCode = item.TypeCode,
                            TypeName = item.TypeName,
                            AdditionType = item.AdditionType,
                            MainAssetName = item.MainAssetName,
                            StartDate = item.StartDate,
                            EndDate = item.EndDate,
                            DeprecMethodName = item.DeprecMethodName,
                            DeprecMonthlyRate = item.DeprecMonthlyRate,
                            InstallmentValue = item.InstallmentValue,

                            AccountCode = item.AccAccountCode,
                            AccountCode2 = item.AccAccountCode2,
                            AccountCode3 = item.AccAccountCode3,

                            AccountName = item.AccAccountName,
                            AccountName2 = item.AccAccountName2,
                            AccountName3 = item.AccAccountName3,
                        };

                        var getTransAssetVw = await fxaServiceManager.FxaTransactionsAssetService.GetAllVW(f => f.FixedAssetId == item.Id
                            && f.IsDeleted == false && f.IsDeletedFx == false);
                        if (!getTransAssetVw.Succeeded)
                            return Ok(getTransAssetVw);

                        var TransAssetVwData = getTransAssetVw.Data.AsQueryable();

                        // get DiscardTbl (outer apply)
                        var DiscardTbl = TransAssetVwData.Where(x => x.TransTypeId == 4).OrderBy(x => x.TransDate);
                        string DiscardDate = DiscardTbl.Select(x => x.TransDate ?? "").FirstOrDefault() ?? "";
                        decimal DiscardAmount = DiscardTbl.Select(x => x.Total ?? 0).FirstOrDefault();

                        decimal amount = 0; decimal sumCredit = 0; decimal sumDebit = 0;

                        if (DateTime.ParseExact(item.PurchaseDate ?? "", "yyyy/MM/dd", CultureInfo.InvariantCulture) < startDate)
                            amount = item.Amount ?? 0;

                        sumCredit = TransAssetVwData
                            .Where(x => x.TransTypeId == 8 && DateTime.ParseExact(x.TransDate ?? "", "yyyy/MM/dd", CultureInfo.InvariantCulture) < startDate)
                            .Sum(x => x.Credit ?? 0);

                        sumDebit = TransAssetVwData
                            .Where(x => x.TransTypeId == 8 && DateTime.ParseExact(x.TransDate ?? "", "yyyy/MM/dd", CultureInfo.InvariantCulture) < startDate)
                            .Sum(x => x.Debet ?? 0);

                        obj.Amount = amount + sumCredit - sumDebit;
                        amount = 0; sumCredit = 0; sumDebit = 0;

                        // Get Additions
                        List<int> myTransTypeId = new() { 1, 2, 3, 8 };
                        obj.Additions = TransAssetVwData
                            .Where(x => myTransTypeId.Contains(x.TransTypeId ?? 0)
                                && DateTime.ParseExact(x.TransDate ?? "", "yyyy/MM/dd", CultureInfo.InvariantCulture) >= startDate
                                && DateTime.ParseExact(x.TransDate ?? "", "yyyy/MM/dd", CultureInfo.InvariantCulture) <= endDate)
                            .Sum(x => x.Credit ?? 0);

                        // Get Discards
                        if (!string.IsNullOrEmpty(DiscardDate) && DateTime.ParseExact(DiscardDate, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= startDate
                                && DateTime.ParseExact(DiscardDate, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= endDate
                           )
                            amount = item.Amount ?? 0;

                        myTransTypeId = new List<int> { 1, 2, 3, 8 };
                        sumDebit = TransAssetVwData
                           .Where(x => myTransTypeId.Contains(x.TransTypeId ?? 0)
                                && DateTime.ParseExact(x.TransDate ?? "", "yyyy/MM/dd", CultureInfo.InvariantCulture) >= startDate
                                && DateTime.ParseExact(x.TransDate ?? "", "yyyy/MM/dd", CultureInfo.InvariantCulture) <= endDate)
                           .Sum(x => x.Debet ?? 0);

                        obj.Discards = amount + sumDebit;
                        amount = 0; sumDebit = 0;

                        // Get DepreciationOld
                        sumDebit = TransAssetVwData
                           .Where(x => x.TransTypeId == 5
                                && DateTime.ParseExact(x.TransDate ?? "", "yyyy/MM/dd", CultureInfo.InvariantCulture) < startDate)
                           .Sum(x => x.Debet ?? 0);

                        obj.DepreciationOld = sumDebit;
                        sumDebit = 0;

                        // Get DepreciationNow 
                        sumDebit = TransAssetVwData
                           .Where(x => x.TransTypeId == 5
                                && DateTime.ParseExact(x.TransDate ?? "", "yyyy/MM/dd", CultureInfo.InvariantCulture) >= startDate
                                && DateTime.ParseExact(x.TransDate ?? "", "yyyy/MM/dd", CultureInfo.InvariantCulture) <= endDate)
                           .Sum(x => x.Debet ?? 0);

                        obj.DepreciationNow = sumDebit;
                        sumDebit = 0;

                        // Get DepreciateDiscardation
                        if (obj.Discards > 0)
                            obj.DepreciateDiscardation = obj.DepreciationOld + obj.DepreciationNow;

                        // Get ProfitAndLoss
                        var getRevaluation = await fxaServiceManager.FxaTransactionsRevaluationService.GetAllVW(r => r.FixedAssetId == item.Id
                        && r.TransTypeId == 8 && r.IsDeleted == false);

                        var revaluationData = getRevaluation.Data.AsQueryable();
                        decimal sumProfit_Loss = revaluationData
                            .Where(x => DateTime.ParseExact(x.TransDate ?? "", "yyyy/MM/dd", CultureInfo.InvariantCulture) >= startDate
                                && DateTime.ParseExact(x.TransDate ?? "", "yyyy/MM/dd", CultureInfo.InvariantCulture) <= endDate)
                            .Sum(x => x.ProfitAndLoss ?? 0);
                        obj.ProfitAndLoss = sumProfit_Loss;


                        final.Add(obj);
                    }

                    var groupedData = final
                        .GroupBy(x => new { x.TypeCode, x.TypeName, x.AccountCode, x.AccountName, x.AccountCode2, x.AccountName2, x.AccountCode3, x.AccountName3 })
                        .Select(g => new
                        {
                            TypeCode = g.Key.TypeCode,
                            TypeName = g.Key.TypeName,

                            AccountCode = g.Select(x => x.AccountCode).FirstOrDefault(),
                            AccountCode2 = g.Select(x => x.AccountCode2).FirstOrDefault(),
                            AccountCode3 = g.Select(x => x.AccountCode3).FirstOrDefault(),

                            AccountName = g.Select(x => x.AccountName).FirstOrDefault(),
                            AccountName2 = g.Select(x => x.AccountName2).FirstOrDefault(),
                            AccountName3 = g.Select(x => x.AccountName3).FirstOrDefault(),

                            Amount = g.Sum(x => x.Amount),
                            Additions = g.Sum(x => x.Additions),
                            Discards = g.Sum(x => x.Discards),
                            DepreciationOld = g.Sum(x => x.DepreciationOld),
                            DepreciationNow = g.Sum(x => x.DepreciationNow),
                            DepreciateDiscardation = g.Sum(x => x.DepreciateDiscardation),
                            ProfitAndLoss = g.Sum(x => x.ProfitAndLoss)
                        }).ToList();

                    List<FxaDeprecByCategoryReportVm> results = new();
                    foreach (var item in groupedData)
                    {
                        results.Add(new FxaDeprecByCategoryReportVm()
                        {
                            TypeCode = item.TypeCode,
                            TypeName = item.TypeName,
                            Amount = item.Amount,
                            Additions = item.Additions,
                            Discards = item.Discards,
                            DepreciationOld = item.DepreciationOld,
                            DepreciationNow = item.DepreciationNow,
                            DepreciateDiscardation = item.DepreciateDiscardation,
                            ProfitAndLoss = item.ProfitAndLoss,
                            
                            AccountCode = item.AccountCode,
                            AccountCode2 = item.AccountCode2,
                            AccountCode3 = item.AccountCode3,

                            AccountName = item.AccountName,
                            AccountName2 = item.AccountName2,
                            AccountName3 = item.AccountName3,
                        });
                    }

                    return Ok(await Result<List<FxaDeprecByCategoryReportVm>>.SuccessAsync(results));
                }

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp Message: {ex.Message}"));
            }
        }
        #endregion ================================================ End Asset deprecaiation report ================================================


        #region ====================================================== Asset types DDL ======================================================
        [HttpGet("DDLAssetType2")]
        public async Task<IActionResult> DDLAssetType2(long typeId)
        {
            try
            {
                if (typeId > 0)
                {
                    var list = new SelectList(new List<DDListItem<FxaFixedAssetTypeDto>>());
                    list = await ddlHelper.GetAnyLis<FxaFixedAssetType, long>(t => t.ParentId == typeId && t.IsDeleted == false && t.FacilityId == session.FacilityId,
                        "Id", "TypeName");
                    return Ok(await Result<SelectList>.SuccessAsync(list));
                }

                return Ok(await Result.FailAsync("Invalid type Id"));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLAssetType3")]
        public async Task<IActionResult> DDLAssetType3(long typeId)
        {
            // this function used in RP_Depreciation3
            try
            {
                if (typeId > 0)
                {
                    var list = new SelectList(new List<DDListItem<FxaFixedAssetTypeDto>>());
                    var typesBasedOnParents = await fxaServiceManager.FxaFixedAssetTypeService.FxaFixedAssetTypeId_DF((int)typeId);
                    if (!string.IsNullOrEmpty(typesBasedOnParents))
                    {
                        var idsArray = typesBasedOnParents.Split(',');
                        list = await ddlHelper.GetAnyLis<FxaFixedAssetType, long>(t => t.IsDeleted == false && t.FacilityId == session.FacilityId && idsArray.Contains(t.Id.ToString()),
                            "Id", "TypeName");
                    }

                    return Ok(await Result<SelectList>.SuccessAsync(list));
                }

                return Ok(await Result.FailAsync("Invalid type Id"));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
        #endregion =================================================== End Asset types DDL ===================================================

    }
}