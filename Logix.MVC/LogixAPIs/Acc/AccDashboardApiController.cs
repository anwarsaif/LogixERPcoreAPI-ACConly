using Logix.Application.Common;
using Logix.Application.DTOs.ACC;
using Logix.Application.DTOs.Main;
using Logix.Application.DTOs.RPT;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.Acc
{
    public class AccDashboardApiController : BaseAccApiController
    {
        private readonly IAccServiceManager accServiceManager;
        private readonly IMainServiceManager mainServiceManager;
        private readonly ICurrentData session;
        private readonly int _SystemId = 2;

        public AccDashboardApiController(IAccServiceManager accServiceManager, IMainServiceManager mainServiceManager, ICurrentData session)
        {
            this.accServiceManager = accServiceManager;
            this.mainServiceManager = mainServiceManager;
            this.session = session;
        }

        [HttpGet("GetAccStatistics")]
        public async Task<IActionResult> GetAccStatistics()
        {
            try
            {
                IEnumerable<AccStatisticsDto> statisticsList = new List<AccStatisticsDto>();
                var getStatistics = await accServiceManager.AccDashboardService.GetStatistics(session.FacilityId, session.FinYear);
                return Ok(getStatistics);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetAccReports")]
        public async Task<IActionResult> GetAccReports()
        {
            try
            {
                IEnumerable<RptReportDto> reportsList = new List<RptReportDto>();
                var getReports = await accServiceManager.AccDashboardService.GetReports(_SystemId, session.GroupId.ToString());
                return Ok(getReports);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
