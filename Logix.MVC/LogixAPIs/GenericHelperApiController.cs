using Logix.Application.Common;
using Logix.Application.DTOs.Main;
using Logix.Application.Interfaces.IRepositories;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.Main;
using Logix.Infrastructure.Repositories;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.ViewModelFilter;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace Logix.MVC.LogixAPIs
{
    [Route($"api/{ApiConfig.ApiVersion}/[controller]")]
    [ApiController]
    public class GenericHelperApiController : ControllerBase
    {
        private readonly IMainRepositoryManager mainRepositoryManager;

        public GenericHelperApiController(IMainRepositoryManager mainRepositoryManager)
        {
            this.mainRepositoryManager = mainRepositoryManager;
        }
        [HttpPost("DateDiffDay2")]
        public async Task<IActionResult> DateDiffDay2(DataDiffDayFilterVm filter)
        {
            try
            {
                 DateHelper.Initialize(mainRepositoryManager);
                //var days = await DateHelper.DateDiff_day2(filter.SDate,filter.EDate);
                var days = (DateHelper.StringToDate(filter.EDate) - DateHelper.StringToDate(filter.SDate)).Days;
                return Ok(await Result<int>.SuccessAsync(days));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }



        }
}
