using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.Acc
{
    [Route($"api/{ApiConfig.ApiVersion}/Acc/[controller]")]
    [ApiController]
    public abstract class BaseAccApiController : ControllerBase
    {
       
    }
}
