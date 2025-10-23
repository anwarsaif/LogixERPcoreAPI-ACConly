using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Linq;
using Result = Logix.Application.Wrapper.Result;

namespace Logix.MVC.LogixAPIs.HR
{
    //  ملفات الاكسل شيت
    public class HRPreparationSalariesShowWizardController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        public HRPreparationSalariesShowWizardController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
        }

        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrShowWizardFilterDto filter)
        {
            var chk = await permission.HasPermission(428, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {

                var BranchesList = session.Branches.Split(',');

                var items = await hrServiceManager.HrPreparationSalaryService.GetAllVW(e => e.IsDeleted == false &&
                !string.IsNullOrEmpty(e.PackageNo) &&
                (filter.FinancelYear == null || filter.FinancelYear == 0 || filter.FinancelYear == e.FinancelYear) &&
                (filter.DeptId == null || filter.DeptId == 0 || filter.DeptId == e.DeptId) &&
                (filter.Location == null || filter.Location == 0 || filter.Location == e.Location) &&
                (string.IsNullOrEmpty(filter.MsMonth) || Convert.ToInt32(filter.MsMonth) == Convert.ToInt32(e.MsMonth)));
                if (items.Succeeded)
                {
                    if (items.Data.Count() > 0)
                    {
                        var result = items.Data
                            .GroupBy(x => new { x.UserFullname, x.DepName, x.LocationName, x.FinancelYear, x.MsMonth, x.PackageNo })
                            .OrderBy(group => group.Key.PackageNo);
                        return Ok(await Result<List<HrPreparationSalariesVw>>.SuccessAsync(items.Data.ToList(), ""));
                    }
                    return Ok(await Result<List<HrPreparationSalariesVw>>.SuccessAsync(items.Data.ToList(), localization.GetResource1("NosearchResult")));
                }
                return Ok(await Result<HrPreparationSalariesVw>.FailAsync(items.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrPreparationSalariesVw>.FailAsync(ex.Message));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long PackageId)
        {
            try
            {
                var chk = await permission.HasPermission(428, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (PackageId <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));
                var delete = await hrServiceManager.HrPreparationSalaryService.RemoveByPackage(PackageId.ToString());
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete HRPreparationSalariesShowWizard Controller , MESSAGE: {ex.Message}"));
            }
        }
    }
}