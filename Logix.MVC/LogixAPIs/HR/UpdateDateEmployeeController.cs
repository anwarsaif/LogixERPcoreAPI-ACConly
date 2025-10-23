

using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.HR
{
    public class UpdateDateEmployeeController : BaseHrApiController
    {

        private readonly IPermissionHelper permission;
        private readonly IMainServiceManager mainServiceManager;

        public UpdateDateEmployeeController(IPermissionHelper permission, IMainServiceManager mainServiceManager )
        {
            this.permission = permission;
            this.mainServiceManager = mainServiceManager;
        }
        //[HttpPost("Search")]
        //public async Task<IActionResult> Search()
        //{
        //    var chk = await permission.HasPermission(578, PermissionType.Import);
        //    if (!chk)
        //    {
        //        return Ok(await Result.AccessDenied("AccessDenied"));
        //    }
        //    try
        //    {
        //        var items = await mainServiceManager.InvestEmployeeService.DDLFieldColumns();

        //        return Ok(items);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(await Result<object>.FailAsync(ex.Message));

        //    }
        //}
    }
}
