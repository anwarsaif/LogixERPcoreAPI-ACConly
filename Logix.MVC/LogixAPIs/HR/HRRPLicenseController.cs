using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Result = Logix.Application.Wrapper.Result;

namespace Logix.MVC.LogixAPIs.HR
{

    //  تقرير بإنتهاء رخص الموظفين
    public class HRRPLicenseController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IMainServiceManager mainServiceManager;


        public HRRPLicenseController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IMainServiceManager mainServiceManager)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.mainServiceManager = mainServiceManager;
        }


        #region IndexPage

        [HttpPost("Search")]
        public async Task<IActionResult> Search(HRRPlicenseFilterDto filter)
        {
            List<HRRPlicenseFilterDto> results = new List<HRRPlicenseFilterDto>();
            var chk = await permission.HasPermission(488, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                filter.BranchId ??= 0;
                filter.LicenseType ??= 0;
                var BranchesList = session.Branches.Split(',');
                var employees = await hrServiceManager.HrLicenseService.GetAllVW(e => e.IsDeleted == false && e.ExpiryDate != null && BranchesList.Contains(e.BranchId.ToString()));
                var filteredData = employees.Data
                    .Where(e =>
                        (string.IsNullOrEmpty(filter.EmpName) || (e.EmpName != null && e.EmpName.ToLower().Contains(filter.EmpName.ToLower()))) &&
                        (string.IsNullOrEmpty(filter.empCode) || e.EmpCode == filter.empCode) &&
                        (filter.BranchId == 0 || e.BranchId == filter.BranchId) &&
                        (filter.LicenseType == 0 || e.LicenseType == filter.LicenseType)

                    );
                if (!string.IsNullOrEmpty(filter.From) && (!string.IsNullOrEmpty(filter.To)))
                {
                    var StartDate = DateHelper.StringToDate(filter.From);
                    var EndDate = DateHelper.StringToDate(filter.To);
                    filteredData = filteredData.Where(x => DateHelper.StringToDate(x.ExpiryDate) >= StartDate
                    && DateHelper.StringToDate(x.ExpiryDate) <= EndDate);
                }
                foreach (var item in filteredData)
                {
                    var remainingDays = (DateHelper.StringToDate(item.ExpiryDate) - DateTime.Now).Days;

                    var employeeDto = new HRRPlicenseFilterDto
                    {
                        Id = item.Id,
                        LicenseTypeName = item.LicenseTypeName,
                        empCode = item.EmpCode,
                        EmpName = item.EmpName ?? item.EmpName2,
                        ExpiryDate = item.ExpiryDate,
                        RemainingDays = remainingDays,
                    };

                    results.Add(employeeDto);
                }
                if (results.Count > 0) return Ok(await Result<List<HRRPlicenseFilterDto>>.SuccessAsync(results, ""));
                return Ok(await Result<List<HRRPlicenseFilterDto>>.SuccessAsync(results, localization.GetResource1("NosearchResult")));


            }
            catch (Exception ex)
            {
                return Ok(await Result<HRRPlicenseFilterDto>.FailAsync(ex.Message));
            }
        }

        #endregion

    }
}