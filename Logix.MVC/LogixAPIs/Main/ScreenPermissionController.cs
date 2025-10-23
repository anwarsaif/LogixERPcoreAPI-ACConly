using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.Main
{
    public class ScreenPermissionController : BaseMainApiController
    {
        private readonly IPermissionHelper permission;

        public ScreenPermissionController(IPermissionHelper permission)
        {
            this.permission = permission;
        }

        [HttpGet("HasPermission")]
        public async Task<IActionResult> GetEmployeeById(long screenId, int permissionType)
        {
            try
            {
                if (permissionType >= 1 && permissionType <= 5)
                {
                    PermissionType type = PermissionType.Show;
                    switch (permissionType)
                    {
                        case 1: type = PermissionType.Add; break;
                        case 2: type = PermissionType.Edit; break;
                        case 3: type = PermissionType.Delete; break;
                        case 4: type = PermissionType.Show; break;
                        case 5: type = PermissionType.Print; break;
                    }
                    var chk = await permission.HasPermission(screenId, type);
                    return Ok(chk); 
                }
                else
                {
                    return BadRequest("PermissionType must be between 1 to 5 only");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

 


    }
}
