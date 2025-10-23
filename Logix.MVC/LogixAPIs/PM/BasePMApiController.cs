using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.PM
{
    [Route($"api/{ApiConfig.ApiVersion}/PM/[controller]")]
    [ApiController]
    public abstract class BasePMApiController : ControllerBase
    {
    /*    [HttpGet("Test")]
        public IActionResult Test()
        {
            return Ok("This Is a BasePMApi Test Success");
        }*/
    }
}
