using Logix.Application.Common;
using Logix.Application.DTOs.Main;
using Logix.Application.DTOs.TS;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.TS
{
    public class TsTasksCompletedController : BaseTsApiController
    {
        private readonly IPermissionHelper permission;
        private readonly ITsServiceManager tsServiceManager;
        private readonly IPMServiceManager pmServiceManager;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IMainServiceManager mainServiceManager;

        public TsTasksCompletedController(
            IPermissionHelper permission,
            ITsServiceManager tsServiceManager,
            IPMServiceManager pmServiceManager,
            ICurrentData session,
            ILocalizationService localization,
            IMainServiceManager mainServiceManager)
        {
            this.permission = permission;
            this.tsServiceManager = tsServiceManager;
            this.pmServiceManager = pmServiceManager;
            this.session = session;
            this.localization = localization;
            this.mainServiceManager = mainServiceManager;
        }


        #region "MarkAsCompletedTask"
        [HttpPost("MarkAsCompletedTask")]
        public async Task<IActionResult> MarkAsCompletedTask(TsCompeletedTaskDto obj)
        {
            if (!(await permission.HasPermission(243, PermissionType.Edit)) || !(await permission.HasPermission(956, PermissionType.Edit)))
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<TsCompeletedTaskDto>.FailAsync(localization.GetMessagesResource("dataRequire")));
                }
                var Edit = await tsServiceManager.TsTaskService.MarkAsCompletedTask(obj);
                if (Edit.Succeeded)
                {
                    return Ok(Edit);
                }
                else
                {
                    return Ok(await Result<TsCompeletedTaskDto>.FailAsync(localization.GetResource1("UpdateError")));
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<TsCompeletedTaskDto>.FailAsync($"======= Exp in PurItemsPriceM edit: {ex.Message}"));
            }
        }
        #endregion "ApproveTask"


        #region "Add"
        [HttpPost("Add")]
        public async Task<IActionResult> Add(TsCompeletedTaskAddDto obj)
        {

            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<object>.FailAsync(localization.GetMessagesResource("dataRequire")));
                }
                var result = await tsServiceManager.TsTaskService.CompletedTaskAdd(obj);
                return Ok(result);

            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"Error in {nameof(Add)}: {ex.Message}"));
            }
        }


        #endregion "Add"

    }
}