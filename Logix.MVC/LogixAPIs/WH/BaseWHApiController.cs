using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.WH
{
    [Route($"api/{ApiConfig.ApiVersion}/WH/[controller]")]
    [ApiController]
    //[EnableCors("CorsPolicy")]
    public abstract class BaseWHApiController : ControllerBase
    {

    }
}
