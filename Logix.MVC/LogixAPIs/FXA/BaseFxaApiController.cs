using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.FXA
{
    [Route($"api/{ApiConfig.ApiVersion}/FXA/[controller]")]
    [ApiController]
    public abstract class BaseFxaApiController : ControllerBase
    {
        
    }
}