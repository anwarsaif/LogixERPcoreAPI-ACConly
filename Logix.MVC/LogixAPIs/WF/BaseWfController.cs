using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.WF
{
    [Route($"api/{ApiConfig.ApiVersion}/WF/[controller]")]
    [ApiController]
    public abstract class BaseWfController : ControllerBase
    {
        // Both Automation and Workflow Systems Will Request API From This Folder (WF).
    }
}