using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.PUR
{
    [Route($"api/{ApiConfig.ApiVersion}/Pur/[controller]")]
    [ApiController]
    public class BasePurApiController : ControllerBase
    {
    }
}
