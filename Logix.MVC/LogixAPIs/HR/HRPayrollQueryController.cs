using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using Result = Logix.Application.Wrapper.Result;

namespace Logix.MVC.LogixAPIs.HR
{
    //   استعلام عن راتب موظف
    public class HRPayrollQueryController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly ISysConfigurationHelper sysConfigurationHelper;
       public HRPayrollQueryController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, ISysConfigurationHelper sysConfigurationHelper)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.sysConfigurationHelper = sysConfigurationHelper;
        }
        #region AddPage Business
        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrPayrollQueryFilterDto filter)
        {
            var chk = await permission.HasPermission(339, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
				//var BranchesList = session.Branches.Split(',');
				//var items = await hrServiceManager.HrPayrollDService.GetAllVW(x =>
				//x.IsDeleted == false &&
				//BranchesList.Contains(x.BranchId.ToString()) &&
				//(filter.PayrollTypeId == null || filter.PayrollTypeId == 0 || filter.PayrollTypeId == x.PayrollTypeId) &&
				//(filter.FinancelYear == null || filter.FinancelYear == 0 || filter.FinancelYear == x.FinancelYear) &&
				//(string.IsNullOrEmpty(filter.EmpCode) || filter.EmpCode == x.EmpId.ToString()) &&
				//(string.IsNullOrEmpty(filter.EmpName) || x.EmpName.ToLower().Contains(filter.EmpName.ToLower())) &&
				//(filter.Location == null || filter.Location == 0 || filter.Location == x.Location) &&
				//(filter.DeptId == null || filter.DeptId == 0 || filter.DeptId == x.DeptId) &&
				//(filter.FacilityId == null || filter.FacilityId == 0 || filter.FacilityId == x.FacilityId) &&
				//(filter.FinancelYear == 0 || filter.FinancelYear == null || filter.FinancelYear == x.FinancelYear) &&
				//(filter.PaymentTypeId == 0 || filter.PaymentTypeId == null || filter.PaymentTypeId == x.PaymentTypeId) &&
				//(filter.JobId == 0 || filter.JobId == null || filter.JobId == Convert.ToInt32(x.JobId)) &&
				//(string.IsNullOrEmpty(filter.MsMonth) || x.MsMonth == filter.MsMonth)
				//);
				//if (items.Succeeded)
				//{
				//    if (items.Data.Count() > 0)
				//    {
				//        var res = items.Data.AsEnumerable();
				//        if (!string.IsNullOrEmpty(filter.FromDate))
				//        {
				//            res = res.Where(x => DateHelper.StringToDate(filter.FromDate) <= DateHelper.StringToDate(x.MsDate));
				//        }

				//        if (!string.IsNullOrEmpty(filter.ToDate))
				//        {
				//            res = res.Where(x => DateHelper.StringToDate(filter.ToDate) >= DateHelper.StringToDate(x.MsDate));
				//        }
				//        return Ok(await Result<IEnumerable<HrPayrollDVw>>.SuccessAsync(res, ""));
				//    }
				//    return Ok(await Result<List<HrPayrollDVw>>.SuccessAsync(items.Data.ToList(), localization.GetResource1("NosearchResult")));
				//}
				//return Ok(await Result<HrPayrollDVw>.FailAsync(items.Status.message));
				var items = await hrServiceManager.HrPayrollDService.SearchPayrollQuery(filter);
				return Ok(items);

			}
            catch (Exception ex)
            {
                return Ok(await Result<HrPayrollDVw>.FailAsync(ex.Message));
            }
        }


        #endregion




    }
}