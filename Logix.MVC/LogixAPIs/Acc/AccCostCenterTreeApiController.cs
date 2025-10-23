using Logix.Application.Common;
using Logix.Application.DTOs.ACC;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.Acc.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.Acc
{
    public class AccCostCenterTreeApiController : BaseAccApiController
    {

        private readonly IAccServiceManager accServiceManager;
        private readonly IPermissionHelper permission;
        private readonly IWebHostEnvironment env;
        private readonly IFilesHelper filesHelper;
        private readonly IDDListHelper listHelper;
        private readonly ILocalizationService localization;
        private readonly ISysConfigurationHelper configurationHelper;
        private readonly ICurrentData _session;

        public AccCostCenterTreeApiController(
            IAccServiceManager accServiceManager,
            IPermissionHelper permission,
             IWebHostEnvironment env,
            IHttpContextAccessor httpContextAccessor,
            IFilesHelper filesHelper,
            IDDListHelper listHelper,
             ILocalizationService localization
             , ISysConfigurationHelper configurationHelper
            , ICurrentData session
            )
        {
            this.accServiceManager = accServiceManager;
            this.permission = permission;
            this.env = env;
            this.filesHelper = filesHelper;
            this.listHelper = listHelper;
            this.localization = localization;
            this.configurationHelper = configurationHelper;
            this._session = session;
        }







        #region "CostCentertTree"

        [HttpGet("CostCentertTree")]

        public async Task<IActionResult> GetCostCentertTree()
        {
            int lang = _session.Language;
            var chk = await permission.HasPermission(658, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));

            }


            List<ACCCostCentertVM> ACCCostCentertVMs = new List<ACCCostCentertVM>();
            var ACCCostCenterts = await accServiceManager.AccCostCenterService.GetAll(x => x.IsDeleted == false && x.FacilityId == _session.FacilityId);
            ACCCostCentertVMs = ACCCostCenterts.Data.Select(a => new ACCCostCentertVM { CcId = a.CcId, CostCenterName = lang == 1 ? a.CostCenterName : a.CostCenterName2 ?? a.CostCenterName, CcParentId = a.CcParentId == 0 ? a.CcId : a.CcParentId }).ToList();

            List<ACCCostCentertVM> mainCostCenterts = ACCCostCentertVMs
                .Where(a => a.CcParentId == a.CcId || a.CcParentId == null)
                // .Where(a => a.CcParentId != null)

                .ToList();

            List<ACCCostCenterNode> CostCenterWithChildren = new List<ACCCostCenterNode>();
            foreach (var mainCostCenter in mainCostCenterts)
            {
                var CostCenterData = new ACCCostCenterNode
                {

                    CcId = mainCostCenter.CcId,
                    CostCenterName = mainCostCenter.CostCenterName,
                    CostCenterName2 = mainCostCenter.CostCenterName2,
                    Icon = "jstree-folder" // Replace with the CSS class for the desired icon
                };

                CostCenterData.Children = await GetCostCenterWithChildren(mainCostCenter.CcId);

                CostCenterWithChildren.Add(CostCenterData);
            }


            return Ok(await Result<List<ACCCostCenterNode>>.SuccessAsync(CostCenterWithChildren, ""));

        }
        [NonAction]

        private async Task<List<ACCCostCenterNode>> GetCostCenterWithChildren(long CcId)
        {
            int lang = _session.Language;
            List<ACCCostCentertVM> CostCenterVMs = new List<ACCCostCentertVM>();
            var CostCenters = await accServiceManager.AccCostCenterService.GetAll(x => x.IsDeleted == false && x.FacilityId == _session.FacilityId);
            CostCenterVMs = CostCenters.Data.Select(a => new ACCCostCentertVM { CcId = a.CcId, CostCenterName = lang == 1 ? a.CostCenterName : a.CostCenterName2 ?? a.CostCenterName, CcParentId = a.CcParentId }).ToList();
            List<ACCCostCentertVM> childCostCenter = CostCenterVMs
                .Where(a => a.CcParentId == CcId && a.CcParentId != a.CcId)
                .ToList();

            List<ACCCostCenterNode> CostCenterNodes = new List<ACCCostCenterNode>();
            foreach (var childCostCenters in childCostCenter)
            {
                var CostCenterNode = new ACCCostCenterNode
                {
                    CcId = childCostCenters.CcId,
                    CostCenterName = childCostCenters.CostCenterName,
                    CostCenterName2 = childCostCenters.CostCenterName2,
                    Icon = "jstree-folder" // Replace with the CSS class for the desired icon
                };

                CostCenterNode.Children = await GetCostCenterWithChildren(childCostCenters.CcId);

                CostCenterNodes.Add(CostCenterNode);
            }

            return CostCenterNodes;
        }

        #endregion "CostCentertTree"

        #region "transactions_GetById"
        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(long id)
        {
            try
            {
                var chk = await permission.HasPermission(658, PermissionType.Show);
                if (!chk)
                {
                    return Ok(await Result.AccessDenied("AccessDenied"));
                }

                if (id <= 0)
                {
                    return Ok(await Result<AccCostCenterEditDto>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var getItem = await accServiceManager.AccCostCenterService.GetForUpdate<AccCostCenterEditDto>(id);
                if (getItem.Succeeded)
                {
                    var obj = getItem.Data;



                    return Ok(await Result<AccCostCenterEditDto>.SuccessAsync(obj, $""));
                }
                else
                {
                    return Ok(getItem);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccCostCenterEditDto>.FailAsync($"====== Exp in GetByIdForEdit Acc CostCenter, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetByload")]
        public async Task<IActionResult> GetByload()
        {
            try
            {
                var chk = await permission.HasPermission(658, PermissionType.Show);
                if (!chk)
                {
                    return Ok(await Result.AccessDenied("AccessDenied"));
                }


                var dataTemp = await accServiceManager.AccCostCenterService.GetAll(x => x.IsDeleted == false && x.FacilityId == _session.FacilityId);
                var data = dataTemp.Data.OrderBy(S => S.CcId).FirstOrDefault();
                var getItem = await accServiceManager.AccCostCenterService.GetForUpdate<AccCostCenterEditDto>(data.CcId);
                if (getItem.Succeeded)
                {
                    var obj = getItem.Data;



                    return Ok(await Result<AccCostCenterEditDto>.SuccessAsync(obj, $""));
                }
                else
                {
                    return Ok(getItem);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccCostCenterEditDto>.FailAsync($"====== Exp in GetByload Acc CostCenter , MESSAGE: {ex.Message}"));
            }
        }

        #endregion "transactions_GetById"


        #region "transactions_Update"

        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(AccCostCenterEditDto obj)
        {
            var chk = await permission.HasPermission(658, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<AccCostCenterEditDto>.FailAsync(localization.GetMessagesResource("dataRequire")));
                }




                obj.FacilityId = _session.FacilityId;
                var Edit = await accServiceManager.AccCostCenterService.Update(obj);
                return Ok(Edit);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccCostCenterEditDto>.FailAsync($"======= Exp in Acc CostCenter edit: {ex.Message}"));
            }
        }

        #endregion "transactions_Update"

        #region "transactions_Delete"


        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long Id)
        {
            var chk = await permission.HasPermission(658, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var ch = await accServiceManager.AccCostCenterService.GetAll(x => x.CcParentId == Id);
                if (ch.Succeeded && ch.Data.Count() > 0)
                {
                    return Ok(await Result<AccCostCenterDto>.FailAsync(localization.GetAccResource("chkCostCenterParentId")));


                }
                var add = await accServiceManager.AccCostCenterService.Remove(Id);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccCostCenterDto>.FailAsync($"======= Exp in Acc CostCenter  Delete: {ex.Message}"));
            }
        }
        #endregion "transactions_Delete"
    }
}