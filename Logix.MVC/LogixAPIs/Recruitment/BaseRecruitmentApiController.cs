using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.Recruitment
{
    [Route($"api/{ApiConfig.ApiVersion}/Recruitment/[controller]")]
    [ApiController]
    public class BaseRecruitmentApiController : ControllerBase
    {

    }
}
