using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.TS
{
    [Route($"api/{ApiConfig.ApiVersion}/Ts/[controller]")]
    [ApiController]
    public class BaseTsApiController : ControllerBase
    {
    }
}
