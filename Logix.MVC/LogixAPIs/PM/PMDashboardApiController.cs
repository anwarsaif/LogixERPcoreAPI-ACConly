using Microsoft.AspNetCore.Mvc;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Common;
using Logix.Application.Wrapper;
using Logix.Application.DTOs.PM;
using Logix.Application.DTOs.Main;
using Logix.MVC.Helpers;

namespace Logix.MVC.LogixAPIs.PM
{
    public class PMDashboardApiController : BasePMApiController
    {
        private readonly IPMServiceManager ipmServiceManager;
        private readonly IMainServiceManager mainServiceManager;    
        private readonly IPermissionHelper permission;

        public PMDashboardApiController(
            IPMServiceManager ipmServiceManager,
            IMainServiceManager mainServiceManager,
            IPermissionHelper permission
            
            )
        {
            this.ipmServiceManager = ipmServiceManager; 
            this.mainServiceManager = mainServiceManager;
            this.permission = permission;
        }

        [HttpGet("GetPMProjects")]
        public async Task<IActionResult> GetPMProjects()
        {
            try
            {
                var getProjects = await ipmServiceManager.PMProjectsService.GetAll();
                if (getProjects.Succeeded)
                {
                   
                    return Ok(await Result<object>.SuccessAsync( getProjects.Data,"kkkkkk",200));
                }

                return Ok(getProjects);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetStatistics")]
        public async Task<IActionResult> GetStatistics()
        {
            try
            {
                var Items = new List<PmStatisticsDto>();
                var AllStatus = await ipmServiceManager.PmProjectStatusVwService.GetAllVW(x => x.Isdel == false);
                if (AllStatus.Succeeded)
                {
                    foreach (var item in AllStatus.Data)
                    {
                        int count = 0;
                        var projects = await ipmServiceManager.PMProjectsService.GetAll(x => x.IsDeleted == false && x.StatusId == item.Id&&x.ParentId==0);
                        count = projects.Data.Count();
                        var PmStatisticsDto = new PmStatisticsDto
                        {
                            Color = item.Color,
                            Icon = item.Icon,
                            StatusId = (int)item.Id,
                            StatusName = item.StatusName,
                            StatusName2 = item.StatusName2,
                            Count = count
                        };
                        Items.Add(PmStatisticsDto);
                    }

                    return Ok(await Result<object>.SuccessAsync(Items, "all projects status", 200));

                }
                return Ok(Items);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }


    }
}
