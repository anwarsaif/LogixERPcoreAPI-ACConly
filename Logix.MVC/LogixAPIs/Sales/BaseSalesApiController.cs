using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.Sales
{
    [Route($"api/{ApiConfig.ApiVersion}/Sales/[controller]")]
    [ApiController]
  //  [EnableCors("CorsPolicy")]
    public abstract class BaseSalesApiController : ControllerBase
    {
      
    }


}
