using AutoMapper;
using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Result = Logix.Application.Wrapper.Result;

namespace Logix.MVC.LogixAPIs.HR
{
    // اعداد السلف
    public class HRPreparationSalariesLoanController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IMainServiceManager mainServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;



        public HRPreparationSalariesLoanController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IMainServiceManager mainServiceManager)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;

            this.mainServiceManager = mainServiceManager;
        }

        #region IndexPage


        [HttpPost("Search")]
        public async Task<IActionResult> Search(HRPreparationSalariesLoanFilterDto filter)
        {
            var chk = await permission.HasPermission(651, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (filter.FinancelYear == 0 || filter.FinancelYear == null)
                {
                    return Ok(await Result<HrPreparationSalariesFilterDto>.FailAsync(" يجب اختيار السنة المالية"));

                }
                filter.FinYear =Convert.ToInt32( session.FinYear);
                filter.CMDType = 1;
                var items = await hrServiceManager.HrPayrollService.getHR_Preparation_Salaries_Loan_SP(filter);
                if (items.Succeeded)
                {
                    if (items.Data.Count() > 0)
                    {
                            return Ok(await Result<List<HRPreparationSalariesLoanDto>>.SuccessAsync(items.Data.ToList(), ""));
                    }
                    return Ok(await Result<List<HRPreparationSalariesLoanDto>>.SuccessAsync(items.Data.ToList(), localization.GetResource1("NosearchResult")));
                }
                return Ok(await Result<HRPreparationSalariesLoanDto>.FailAsync(items.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrPreparationSalariesVw>.FailAsync(ex.Message));
            }
        }

        #endregion


        #region AddPage Business

        [HttpPost("Add")]
        public async Task<ActionResult> Add(List<HRPreparationSalariesLoanAddDto> obj)
        {
            try
            {
                var chk = await permission.HasPermission(651, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result<string>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));
                var add = await hrServiceManager.HrPreparationSalaryService.PreparationSalariesLoanAdd(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"====== Exp in Add HR   PreparationCommision Controller  Add Method, MESSAGE: {ex.Message}"));
            }
        }


        #endregion

    }
}