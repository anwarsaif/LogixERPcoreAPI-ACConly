using AutoMapper;
using Logix.Application.Common;
using Logix.Application.DTOs.ACC;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Logix.MVC.LogixAPIs.HR
{

    //   كشف حساب موظف
    public class HRAccountTransactionsController : BaseHrApiController
    {
        private readonly IAccServiceManager accServiceManager;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IMapper mapper;
        private readonly IPermissionHelper permission;
        public HRAccountTransactionsController(IAccServiceManager accServiceManager, ICurrentData session, ILocalizationService localization, IMapper mapper, IPermissionHelper permission)
        {
            this.accServiceManager = accServiceManager;
            this.session = session;
            this.localization = localization;
            this.mapper = mapper;
            this.permission = permission;
        }
        #region Index Page

        [HttpPost("Search")]
        public async Task<IActionResult> Search(AccountBalanceSheetFilterDto filter)
        {
            var chk = await permission.HasPermission(421, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
        //filter.referenceTypeId ??= 0;
        //filter.FacilityId ??= 0;
        //if (string.IsNullOrEmpty(filter.EmpCode))
        //{
        //    return Ok(await Result<object>.FailAsync(localization.GetResource1("EmployeeIsNumber")));

        //}
        //if (filter.FacilityId <= 0)
        //{
        //    filter.FacilityId = session.FacilityId;
        //}
        //if (filter.chkAllYear == true)
        //{
        //    var result = await accServiceManager.AccAccountService.GetEmployeeAccountTransactionsForAllYears(filter);
        //    return Ok(result);

        //}
        //else
        //{
        //    var result = await accServiceManager.AccAccountService.GetEmployeeAccountTransactionsForCurrentYear(filter);
        //    return Ok(result);


        //}
        var result = await accServiceManager.AccAccountService.AccountTransactionsSearch(filter);
        return Ok(result);
      }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync(ex.Message));
            }
        }

        #endregion

    }
}
