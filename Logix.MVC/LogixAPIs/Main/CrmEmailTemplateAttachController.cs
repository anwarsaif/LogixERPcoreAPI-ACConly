using Logix.Application.Common;
using Logix.Application.DTOs.CRM;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.Main
{
    public class CrmEmailTemplateAttachController : BaseMainApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly ICrmServiceManager crmServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IFilesHelper filesHelper;

        public CrmEmailTemplateAttachController(IMainServiceManager mainServiceManager,
            ICrmServiceManager crmServiceManager,
            IPermissionHelper permission,
            ILocalizationService localization,
            ICurrentData session,
            IFilesHelper filesHelper)
        {
            this.mainServiceManager = mainServiceManager;
            this.crmServiceManager = crmServiceManager;
            this.permission = permission;
            this.session = session;
            this.localization = localization;
            this.filesHelper = filesHelper;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var items = await crmServiceManager.CrmEmailTemplateAttachService.GetAll(e => e.IsDeleted == false);
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    res = res.OrderBy(e => e.Id);
                    return Ok(await Result<List<CrmEmailTemplateAttachDto>>.SuccessAsync(res.ToList(), items.Status.message));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
    }
}
