using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
namespace Logix.MVC.LogixAPIs.Main
{
    [Route($"api/{ApiConfig.ApiVersion}/Main/[controller]")]
    [ApiController]
    public abstract class BaseMainApiController : ControllerBase
    {
        //[HttpGet("Test")]
        //public IActionResult Test()
        //{
        //    return Ok("this is BaseMainApi index");
        //}
    }
}
