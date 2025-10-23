using Logix.Application.Common;
using Logix.Application.DTOs.ACC;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.Acc
{
    public class AccAccountsLevelController : BaseAccApiController
    {

        private readonly IAccServiceManager accServiceManager;
        private readonly IPermissionHelper permission;
        private readonly IWebHostEnvironment env;
        private readonly IFilesHelper filesHelper;
        private readonly IDDListHelper listHelper;
        private readonly ILocalizationService localization;
        private readonly ISysConfigurationHelper configurationHelper;
        private readonly ICurrentData _session;

        public AccAccountsLevelController(
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
        #region "transactions"


        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var chk = await permission.HasPermission(1149, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var items = await accServiceManager.AccAccountsLevelService.GetAll();
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    res = res.OrderBy(e => e.LevelId);
                    return Ok(items);
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        #endregion "transactions"



        #region "transactions_Update"
        [HttpPost("Edit")]
        public async Task<IActionResult> Updatedigit(long LevelId, int NoOfDigit, string Color)
        {
            var chk = await permission.HasPermission(1149, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<AccAccountsLevelEditDto>.FailAsync(localization.GetMessagesResource("dataRequire")));
                }



                var Edit = await accServiceManager.AccAccountsLevelService.Updatedigit(LevelId, NoOfDigit, Color);
                return Ok(Edit);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccAccountsLevelEditDto>.FailAsync($"======= Exp in Acc Accounts Level edit: {ex.Message}"));
            }
        }

        #endregion "transactions_Update"
    }
}
