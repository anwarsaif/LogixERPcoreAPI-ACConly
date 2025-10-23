using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Services;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using Result = Logix.Application.Wrapper.Result;

namespace Logix.MVC.LogixAPIs.HR
{

    //  تقرير بانتهاء التأمينات الطبية
    public class HRRPMedicalInsuranceController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IMainServiceManager mainServiceManager;
        public HRRPMedicalInsuranceController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IMainServiceManager mainServiceManager)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.mainServiceManager = mainServiceManager;
        }

        #region IndexPage

        [HttpPost("Search")]
        public async Task<IActionResult> Search(RPMedicalInsuranceFilterDto filter)
        {
            //List<RPMedicalInsuranceFilterDto> results = new List<RPMedicalInsuranceFilterDto>();
            var chk = await permission.HasPermission(383, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
        var employees = await hrServiceManager.HrEmployeeService.SearchRPMedicalInsurance(filter);
        return Ok(employees);
        //var employees = await hrServiceManager.HrEmployeeService.GetAllVW(e => e.IsDeleted == false && e.StatusId == 1 && e.InsuranceDateValidity != null && e.InsuranceDateValidity != "" );
        //var filteredEmployees = employees.Data
        //    .Where(e =>
        //        (string.IsNullOrEmpty(filter.EmpName) || e.EmpName.ToLower().Contains(filter.EmpName.ToLower())) &&
        //        (string.IsNullOrEmpty(filter.empCode) || e.EmpId == filter.empCode) &&
        //        (string.IsNullOrEmpty(filter.From) || string.IsNullOrEmpty(filter.To) || (DateHelper.StringToDate(e.InsuranceDateValidity) >= DateHelper.StringToDate(filter.From) && DateHelper.StringToDate(e.InsuranceDateValidity) <= DateHelper.StringToDate(filter.To))) &&
        //        (filter.BranchId == 0 || filter.BranchId == null || e.BranchId == filter.BranchId) &&
        //        (filter.Location == 0 || filter.Location == null || e.Location == filter.Location)

        //    );
        //foreach (var item in filteredEmployees)
        //{
        //    var remainingDays = (DateHelper.StringToDate(item.InsuranceDateValidity) - DateTime.Now).Days;

        //    var employeeDto = new RPMedicalInsuranceFilterDto
        //    {
        //        empCode = item.EmpId,
        //        EmpName = item.EmpName ?? item.EmpName2,
        //        InuranceExpireDate = item.InsuranceDateValidity,
        //        RemainingDays = remainingDays,
        //    };

        //    results.Add(employeeDto);
        //}
        //if (results.Count > 0) return Ok(await Result<List<RPMedicalInsuranceFilterDto>>.SuccessAsync(results, ""));
        //return Ok(await Result<List<RPMedicalInsuranceFilterDto>>.SuccessAsync(results, localization.GetResource1("NosearchResult")));


      }
      catch (Exception ex)
            {
                return Ok(await Result<RPMedicalInsuranceFilterDto>.FailAsync(ex.Message));
            }
        }

        [HttpPost("UpdateMedicalInsuranceExpair")]
        public async Task<IActionResult> UpdateMedicalInsuranceExpair(List<string> EmpCodes, string ?toDate = "")
        {
            var chk = await permission.HasPermission(383, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (EmpCodes.Count() <= 0)
            {
                return Ok(await Result<object>.SuccessAsync("لم يتم اختيار اي موظف للتحديث "));
            }

            try
            {

                var UpdateItem = await mainServiceManager.InvestEmployeeService.UpdateMedicalInsuranceExpair(EmpCodes, toDate);
                return Ok(UpdateItem);
            }
            catch (Exception exp)
            {
                return Ok(await Result<object>.FailAsync($"{exp.Message}"));
            }
        }

        #endregion

    }
}
