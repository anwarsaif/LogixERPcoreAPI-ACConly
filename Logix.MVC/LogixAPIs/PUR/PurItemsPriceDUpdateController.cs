using Logix.Application.Common;
using Logix.Application.DTOs.PUR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Services;
using Logix.Application.Wrapper;
using Logix.Infrastructure.Repositories;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.PUR
{
    public class PurItemsPriceDUpdateController : BasePurApiController
    {
        private readonly IPurServiceManager purServiceManager;
        private readonly IWhServiceManager whServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ILocalizationService localization;
        private readonly ICurrentData session;

        public PurItemsPriceDUpdateController(IPurServiceManager purServiceManager,
            IWhServiceManager whServiceManager,
            IPermissionHelper permission,
            ICurrentData CurrentData,
            ILocalizationService localization,
            ICurrentData session)
        {
            this.purServiceManager = purServiceManager;
            this.whServiceManager = whServiceManager;
            this.permission = permission;
            this.localization = localization;
            this.session = session;
        }

        [HttpPost("Search")]
        public async Task<ActionResult> Search(PurItemsPriceMFilterDto filter)
        {
            try
            {
                var chk = await permission.HasPermission(2034, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                filter.Id ??= 0;
                filter.BranchId ??= 0;
                filter.CatId ??= 0;
                var lang = session.Language;
                var items = await purServiceManager.PurItemsPriceDService.GetAllVW(x => x.IsDeleted == false
                && (string.IsNullOrEmpty(filter.ItemName) || x.ItemName == filter.ItemName)
                && (session.FacilityId == 0 || x.FacilityId == session.FacilityId)
                && (filter.BranchId == 0 || x.BranchId == filter.BranchId)
                && (string.IsNullOrEmpty(filter.ItemCode) || x.ItemCode == filter.ItemCode)
                );
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Search SalItemsPriceMController, MESSAGE: {ex.Message}"));
            }
        }
        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(PurItemsPriceDUpdateListDto obj)
        {
            var chk = await permission.HasPermission(2034, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<PurItemsPriceDUpdateListDto>.FailAsync(localization.GetMessagesResource("dataRequire")));
                }

                var Edit = await purServiceManager.PurItemsPriceDService.Update(obj);
                return Ok(Edit);
            }
            catch (Exception ex)
            {
                return Ok(await Result<PurItemsPriceDUpdateListDto>.FailAsync($"======= Exp in PurItemsPriceM Schedule  edit: {ex.Message}"));
            }
        }
       
    }
}
