using Logix.Application.Services.Main;
using Logix.Application.Services;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Common;
using Logix.Application.Interfaces.IServices.Main;
using Microsoft.AspNetCore.Cors;
using System.Globalization;

namespace Logix.MVC.LogixAPIs
{

    public class PermissionPostVM
    {
        public int ScreenId { get; set; }
    }

    [Route($"api/{ApiConfig.ApiVersion}/[controller]")]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        private readonly IPermissionHelper permission;

        public PermissionController(IPermissionHelper permission)
        {
            this.permission = permission;
        }

        [HttpPost("GetAllScreenPermission")]
        public async Task<ActionResult> ScreenAllTPermission(PermissionPostVM entity)
        {
            try
            {
                return Ok(await permission.ScreenAllTPermission(entity.ScreenId));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"{ex.Message}"));
            }
        }
    }
}