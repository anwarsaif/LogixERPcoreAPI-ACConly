using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.HR.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Logix.MVC.LogixAPIs.HR
{
    //   تحديث التأمينات
    public class HrInsuranceUpdateApiController : BaseHrApiController
    {
        private readonly IPermissionHelper permission;
        private readonly ILocalizationService localization;
        private readonly ICurrentData session;
        private readonly IMainServiceManager mainServiceManager;


        public HrInsuranceUpdateApiController(IMainServiceManager mainServiceManager, ICurrentData session, IPermissionHelper permission,ILocalizationService localization)
        {
            this.session = session;
            this.permission = permission;
            this.localization = localization;
            this.mainServiceManager= mainServiceManager;
        }


        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(string? Tdate)
        {
            var chk = await permission.HasPermission(909, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (string.IsNullOrEmpty(Tdate))
            {
                return Ok( await Result.WarningAsync(localization.GetMessagesResource("DateIsRequired")));
            }
            try
            {
                var addRes = await mainServiceManager.InvestEmployeeService.InsuranceUpdate(Tdate);
                return Ok(addRes);
            }

            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
    
    }

}