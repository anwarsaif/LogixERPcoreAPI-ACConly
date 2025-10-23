using AutoMapper;
using Logix.Application.Common;
using Logix.Application.DTOs.ACC;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.HR
{
    //   كشف حساب الموظفين من الى
    public class HREmployeeTransactionsFromToController : BaseHrApiController
    {
        private readonly IAccServiceManager accServiceManager;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IMapper mapper;
        private readonly IPermissionHelper permission;
        public HREmployeeTransactionsFromToController(IAccServiceManager accServiceManager, ICurrentData session, ILocalizationService localization, IMapper mapper, IPermissionHelper permission)
        {
            this.accServiceManager = accServiceManager;
            this.session = session;
            this.localization = localization;
            this.mapper = mapper;
            this.permission = permission;
        }
        #region Index Page

        [HttpPost("Search")]
        public async Task<IActionResult> Search(AccountFromToFilterDto filter)
        {
            var chk = await permission.HasPermission(1056, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                filter.referenceTypeId ??= 0;
                filter.FinancialYear = Convert.ToInt32(session.FinYear);
                filter.FacilityId = session.FacilityId;
                filter.BranchId ??= 0;
                if (string.IsNullOrEmpty(filter.FromDate))
                {
                    return Ok(await Result<object>.FailAsync("يجب ادخال من تاريخ "));

                }
                if (string.IsNullOrEmpty(filter.ToDate))
                {
                    return Ok(await Result<object>.FailAsync("يجب ادخال الى تاريخ "));

                }
                if (string.IsNullOrEmpty(filter.CodeFrom))
                {
                    return Ok(await Result<object>.FailAsync("يجب ادخال من رقم "));

                }
                if (string.IsNullOrEmpty(filter.CodeTo))
                {
                    return Ok(await Result<object>.FailAsync("يجب ادخال الى رقم "));

                }

                var result = await accServiceManager.AccAccountService.GetEmployeeAccountTransactionsFromTo(filter);
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