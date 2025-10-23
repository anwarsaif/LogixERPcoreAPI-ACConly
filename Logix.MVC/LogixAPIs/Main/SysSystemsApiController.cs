using Logix.Application.Common;
using Logix.Application.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.Main
{
   [Authorize]
    public class SysSystemsApiController : BaseMainApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly ICurrentData session;

        public SysSystemsApiController(IMainServiceManager mainServiceManager, ICurrentData session)
        {
            this.mainServiceManager = mainServiceManager;
            this.session = session;
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var systems = await mainServiceManager.SysSystemService.GetAll();
                if (systems.Succeeded)
                {

                    var sysList = systems.Data.Where(s => s.Isdel == false && s.ShowInHome == true).OrderBy(s => s.SysSort).ToList();
                    foreach (var sys in sysList)
                    {
                        if (sys.IsCore)
                        {
                            var url = $"/{sys.Area}/{sys.Controller}/{sys.Action}";
                            sys.DefaultPage = url;
                        }
                        else
                        {
                            var url = $"{session.OldBaseUrl}{sys.Folder}{sys.DefaultPage}";
                            sys.DefaultPage = url;
                        }
                    }

                    return Ok(sysList);
                }
                return BadRequest("No data Found");
            }
            catch (Exception exp)
            {
                return BadRequest(exp.Message);
            }
        }
    }
}
