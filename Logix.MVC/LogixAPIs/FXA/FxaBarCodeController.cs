using Logix.Application.Common;
using Logix.Application.DTOs.ACC;
using Logix.Application.DTOs.FXA;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.FXA.ViewModels;
using Logix.MVC.LogixAPIs.FXA.ViewModels.ReportVm;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace Logix.MVC.LogixAPIs.FXA
{
    public class FxaBarCodeController : BaseFxaApiController
    {
        private readonly IFxaServiceManager fxaServiceManager;
        private readonly IAccServiceManager accServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly IWebHostEnvironment env;

        public FxaBarCodeController(IFxaServiceManager fxaServiceManager,
            IAccServiceManager accServiceManager,
            IPermissionHelper permission,
            ICurrentData session,
            IWebHostEnvironment env)
        {
            this.fxaServiceManager = fxaServiceManager;
            this.accServiceManager = accServiceManager;
            this.permission = permission;
            this.session = session;
            this.env = env;
        }

        [HttpPost("Search")]
        public async Task<ActionResult> Search(FxaFixedAssetFilterDto filter)
        {
            try
            {
                var chk = await permission.HasPermission(1425, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var branchsId = session.Branches.Split(',');

                filter.LocationId ??= 0; filter.BranchId ??= 0; filter.StatusId ??= 0; filter.ClassificationId ??= 0; filter.AdditionTypeFilter ??= 0;

                var items = await fxaServiceManager.FxaFixedAssetService.GetAllVW(a => a.IsDeleted == false && a.FacilityId == session.FacilityId
                    && (string.IsNullOrEmpty(filter.Code) || (a.No.ToString() == filter.Code))
                    && (string.IsNullOrEmpty(filter.Name) || (!string.IsNullOrEmpty(a.Name) && a.Name.Contains(filter.Name)))
                    && (filter.LocationId == 0 || (a.LocationId == filter.LocationId))
                    && (filter.BranchId == 0 || (a.BranchId == filter.BranchId))
                    && ((filter.BranchId != 0) || branchsId.Contains(a.BranchId.ToString())) //Exclude any records that its branch not in user banches
                    && (filter.StatusId == 0 || (a.StatusId == filter.StatusId))
                    && (string.IsNullOrEmpty(filter.Description) || (!string.IsNullOrEmpty(a.Description) && a.Description.Contains(filter.Description)))
                );

                if (items.Succeeded)
                {
                    var res = items.Data.OrderBy(r => r.Id).AsQueryable();

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
                            });
                        }
                    }
                    return Ok(await Result<List<FxaFixedAssetVM>>.SuccessAsync(final));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }


        [HttpGet("GetBarCodes")]
        public async Task<ActionResult> GetBarCodes(string assetsIds)
        {
            try
            {
                if (string.IsNullOrEmpty(assetsIds))
                    return Ok(await Result.FailAsync("no assets codes"));

                var getFacility = await accServiceManager.AccFacilityService.GetOne(f => f.FacilityId == session.FacilityId);
                if (getFacility.Succeeded)
                {
                    // get company logo
                    string companyLogo = "/images/" + getFacility.Data.FacilityLogo;

                    // create QR code for facility
                    string qrCode = GenerateQR(getFacility.Data);

                    // create BarCode for each fixed asset
                    var assetIdsArray = assetsIds.Split(',');
                    List<FxaBarCodesVm> reslts = new();
                    foreach (var assetId in assetIdsArray)
                    {
                        //get asset name
                        var getAsset = await fxaServiceManager.FxaFixedAssetService.GetOneVW(a => a.Id.ToString() == assetId && a.FacilityId == session.FacilityId);
                        if (getAsset.Succeeded)
                        {
                            string barCode = GenerateBarcode(getAsset.Data.Code ?? "");

                            reslts.Add(new FxaBarCodesVm()
                            {
                                FxaCode = getAsset.Data.Code,
                                FxaName = getAsset.Data.Name,
                                CompanyLogoUrl = companyLogo,
                                QrCodeUrl = qrCode,
                                BarCodeUrl = barCode
                            });
                        }
                    }

                    return Ok(await Result<List<FxaBarCodesVm>>.SuccessAsync(reslts));
                }
                else
                    return Ok(getFacility);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        private string GenerateQR(AccFacilityDto facilityData)
        {
            try
            {
                int lang = session.Language;
                string qrPath = Path.Join(env.WebRootPath, FilesPath.FixedAssetQrPath);
                string qrName = lang == 1 ? (facilityData.FacilityName ?? "") : (facilityData.FacilityName2 ?? "");
                string qrData = "";

                if (lang == 1)
                    qrData = "اسم الشركة : " + facilityData.FacilityName + " الايميل : " + facilityData.FacilityEmail + " رقم الهاتف : " + facilityData.FacilityPhone;
                if (lang == 2)
                    qrData = "Company Name : " + facilityData.FacilityName2 + " Email : " + facilityData.FacilityEmail + " Phone : " + facilityData.FacilityPhone;

                string addQr = QRHelper.GenerateQR(qrPath, qrName, qrData);
                string qrCode = FilesPath.FixedAssetQrPath + "/" + qrName + ".jpg";
                return qrCode;
            }
            catch
            {
                return "";
            }
        }

        private string GenerateBarcode(string assetCode)
        {
            try
            {
                string barCodeUrl = "";
                string savePath = FilesPath.FixedAssetBarCodePath;
                barCodeUrl = BarCodeHelper.GenerateBarCode(assetCode, savePath);
                return barCodeUrl;
            }
            catch
            {
                return "";
            }
        }
    }
}