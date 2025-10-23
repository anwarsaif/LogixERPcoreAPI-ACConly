using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.HR
{
    [Route($"api/{ApiConfig.ApiVersion}/HR/[controller]")]
    [ApiController]
    public class BaseHrApiController : ControllerBase
    {

    }
}
