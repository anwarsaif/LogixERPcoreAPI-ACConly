using Logix.Application.Common;
using Logix.Application.DTOs.RPT;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs
{
    // هذا الكلاس للدالة العامه على مستوى النظام كامل 
    [Route($"api/{ApiConfig.ApiVersion}/[controller]")]
    [ApiController]
    public class MainSharedController : ControllerBase
    {
        private readonly IPMServiceManager pmServiceManager;
        private readonly IRptServiceManager rptServiceManager;
        private readonly IMainServiceManager mainServiceManager;
        private readonly ISysConfigurationHelper configurationHelper;
        private readonly ICurrentData session;

        public MainSharedController(IPMServiceManager pMServiceManager,
            IRptServiceManager rptServiceManager,
            IMainServiceManager mainServiceManager,
            ISysConfigurationHelper configurationHelper,
            ICurrentData session)
        {
            this.pmServiceManager = pMServiceManager;
            this.rptServiceManager = rptServiceManager;
            this.mainServiceManager = mainServiceManager;
            this.configurationHelper = configurationHelper;
            this.session = session;
        }

        // دالة جلب فئات الاصناف
        [HttpGet("BindCategoriesTree")]
        public async Task<IActionResult> BindCategoriesTree()
        {
            try
            {
                var result = await pmServiceManager.PMSharedService.GetCategoriesTree();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"Error fetching  data, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetCustomReport")]
        public async Task<IActionResult> GetCustomReport(long screenId, int screenType)
        {
            // RPT_Custom_Reports_SP, CMDTYPE = 5
            try
            {
                string UsrPrmissionPrpty = await configurationHelper.GetValue(345, session.FacilityId);

                var result = await rptServiceManager.RptCustomReportService.GetAll(x => x.ScreenId == screenId && x.IsDeleted == false && x.Active == true);
                if (result.Succeeded)
                {
                    var getUser = await mainServiceManager.SysUserService.GetOne(x => x.Id == session.UserId);
                    var user = getUser.Data;
                    var reports = result.Data.Where(x => x.ReportType != 2
                    || (x.ReportType == 2 && (x.FacilityId == 0 || x.FacilityId == session.FacilityId)
                            && (!string.IsNullOrEmpty(UsrPrmissionPrpty) && UsrPrmissionPrpty != "0" && UsrPrmissionPrpty.Split(',').Contains(user.Id.ToString())
                                || (!string.IsNullOrEmpty(x.GoupsAccess) && x.GoupsAccess != "0" && x.GoupsAccess.Contains(user.GroupsId ?? "0"))
                                || (!string.IsNullOrEmpty(x.GoupsPermissionEdit) && x.GoupsPermissionEdit != "0" && x.GoupsPermissionEdit.Contains(user.GroupsId ?? "0"))
                                || (!string.IsNullOrEmpty(x.UsersAccess) && x.UsersAccess != "0" && x.UsersAccess.Contains(user.Id.ToString()))
                                || (!string.IsNullOrEmpty(x.UsersPermissionEdit) && x.UsersPermissionEdit != "0" && x.UsersPermissionEdit.Contains(user.Id.ToString()))
                                || ((string.IsNullOrEmpty(x.GoupsAccess) || x.GoupsAccess == "0") && (string.IsNullOrEmpty(x.UsersAccess) || x.UsersAccess == "0"))
                                )));

                    return Ok(await Result<List<RptCustomReportDto>>.SuccessAsync(reports.ToList()));

                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"Error fetching  data, MESSAGE: {ex.Message}"));
            }
        }
    }
}