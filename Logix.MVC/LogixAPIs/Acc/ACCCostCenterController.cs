using Logix.Application.Common;
using Logix.Application.DTOs.ACC;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.Acc
{
    public class ACCCostCenterController : BaseAccApiController
    {

        private readonly IAccServiceManager accServiceManager;
        private readonly IPermissionHelper permission;
        private readonly IWebHostEnvironment env;
        private readonly IFilesHelper filesHelper;
        private readonly IDDListHelper listHelper;
        private readonly ILocalizationService localization;
        private readonly ICurrentData _currentData;

        public ACCCostCenterController(
            IAccServiceManager accServiceManager,
            IPermissionHelper permission,
             IWebHostEnvironment env,
            IHttpContextAccessor httpContextAccessor,
            IFilesHelper filesHelper,
            IDDListHelper listHelper,
             ILocalizationService localization
            , ICurrentData _currentData
            )
        {
            this.accServiceManager = accServiceManager;
            this.permission = permission;
            this.env = env;
            this.filesHelper = filesHelper;
            this.listHelper = listHelper;
            this.localization = localization;
            this._currentData = _currentData;
        }



        #region "transactions"

        [HttpPost("Search")]
        public async Task<IActionResult> Search(AccCostCenterFilterDto filter)
        {
            var chk = await permission.HasPermission(658, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var items = await accServiceManager.AccCostCenterService.Search(filter);

                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable().DistinctBy(x => x.CostCenterCode);


                    var final = res.ToList();
                    return Ok(await Result<List<AccCostCenterVws>>.SuccessAsync(final, ""));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccCostCenterVws>.FailAsync($"======= Exp in Search AccCostCenterVws, MESSAGE: {ex.Message}"));
            }
        }
        #endregion "transactions"

        #region "transactions_ADD"
        [HttpPost("Add")]
        public async Task<IActionResult> Add(AccCostCenterDto obj)
        {
            var chk = await permission.HasPermission(658, PermissionType.Add);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<AccCostCenterDto>.FailAsync(localization.GetMessagesResource("dataRequire")));
                }


                var add = await accServiceManager.AccCostCenterService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccCostCenterDto>.FailAsync($"======= Exp in Acc AccCostCenter add: {ex.Message}"));
            }
        }
        #endregion "transactions_Add"

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
                var Edit = await accServiceManager.AccCostCenterService.Update(obj);
                return Ok(Edit);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccCostCenterEditDto>.FailAsync($"======= Exp in Acc Group  edit: {ex.Message}"));
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

                var add = await accServiceManager.AccCostCenterService.Remove(Id);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccCostCenterDto>.FailAsync($"======= Exp in Acc CostCenter  Delete: {ex.Message}"));
            }
        }
        #endregion "transactions_Delete"

        #region "transactions_GetById"

        [HttpGet("GetByIdForEdit")]
        public async Task<IActionResult> GetByIdForEdit(long id)
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

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(long id)
        {
            try
            {
                var chk = await permission.HasPermission(658, PermissionType.Show);
                if (!chk)
                {
                    return Ok(await Result.AccessDenied("AcessDenied"));
                }

                if (id <= 0)
                {
                    return Ok(await Result<AccCostCenterDto>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var getItem = await accServiceManager.AccCostCenterService.GetOne(s => s.CcId == id && s.IsDeleted == false);
                if (getItem.Succeeded)
                {
                    var obj = getItem.Data;
                    return Ok(await Result<AccCostCenterDto>.SuccessAsync(obj, $""));
                }
                else
                {
                    return Ok(getItem);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccCostCenterDto>.FailAsync($"====== Exp in GetById Acc CostCenter MESSAGE: {ex.Message}"));
            }
        }


        #endregion "transactions_GetById"


    }
}
