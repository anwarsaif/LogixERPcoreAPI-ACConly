using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.Integra
{
    [Route($"api/{ApiConfig.ApiVersion}/Integra/[controller]")]
    [ApiController]
    public class BaseIntegraApiController : ControllerBase
    {
    }
}
