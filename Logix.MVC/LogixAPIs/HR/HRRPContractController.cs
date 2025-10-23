using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.HR.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using Result = Logix.Application.Wrapper.Result;

namespace Logix.MVC.LogixAPIs.HR
{

    //  تقرير بانتهاء عقود الموظفين
    public class HRRPContractController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IMainServiceManager mainServiceManager;
        public HRRPContractController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IMainServiceManager mainServiceManager)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.mainServiceManager = mainServiceManager;
        }

        #region IndexPage

        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrRPContractFilterDto filter)
        {
            var chk = await permission.HasPermission(382, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
               var items = await hrServiceManager.HrEmployeeService.SearchRPContract(filter);
               return Ok(items);

                //var BranchesList = session.Branches.Split(',');
                //List<HrRPContractFilterDto> resultList = new List<HrRPContractFilterDto>();
                //var items = await hrServiceManager.HrEmployeeService.GetAllVW(e => e.IsDeleted == false && BranchesList.Contains(e.BranchId.ToString()) && e.StatusId == 1 && e.ContractExpiryDate != null &&
                //e.ContractExpiryDate != "" &&
                //(string.IsNullOrEmpty(filter.EmpCode) || filter.EmpCode == e.EmpId) &&
                //(string.IsNullOrEmpty(filter.DOAppointment) || filter.DOAppointment == e.Doappointment) &&
                //(string.IsNullOrEmpty(filter.EmpName) || e.EmpName.ToLower().Contains(filter.EmpName) || e.EmpName2.ToLower().Contains(filter.EmpName)) &&
                //(filter.LocationId == 0 || filter.LocationId == null || filter.LocationId == e.Location) &&
                //(filter.DepartmentId == 0 || filter.DepartmentId == null || filter.DepartmentId == e.DeptId) &&
                //(filter.NationalityId == 0 || filter.NationalityId == null || filter.NationalityId == e.NationalityId) &&
                //(filter.JobCategory == 0 || filter.JobCategory == null || filter.JobCategory == e.JobCatagoriesId) &&
                //(filter.ContractTypeID == 0 || filter.ContractTypeID == null || filter.ContractTypeID == e.ContractTypeId)
                //);
                //if (items.Succeeded)
                //{
                //    if (items.Data.Count() > 0)
                //    {

        //        var res = items.Data.AsQueryable();

        //        if (filter.BranchId != null && filter.BranchId > 0)
        //        {
        //            res = res.Where(c => c.BranchId != null && c.BranchId==filter.BranchId);
        //        }
        //        if (!string.IsNullOrEmpty(filter.FromDate) && !string.IsNullOrEmpty(filter.ToDate))
        //        {
        //            res = res.Where(c => (c.ContractExpiryDate != null && DateHelper.StringToDate(c.ContractExpiryDate) >= DateHelper.StringToDate(filter.FromDate) && DateHelper.StringToDate(c.ContractExpiryDate) <= DateHelper.StringToDate(filter.ToDate)));
        //        }
        //        if (res.Any())
        //        {
        //            foreach (var item in res)
        //            {
        //                decimal TotalAllowance = 0;
        //                decimal TotalDeduction = 0;
        //                var getTotalAllowance = await hrServiceManager.HrAllowanceDeductionService.GetTotalAllowances(item.Id);
        //                if (getTotalAllowance.Succeeded) TotalAllowance = getTotalAllowance.Data;

        //                var getTotalDeduction = await hrServiceManager.HrAllowanceDeductionService.GetTotalDeduction(item.Id);
        //                if (getTotalDeduction.Succeeded) TotalDeduction = getTotalDeduction.Data;
        //                var newItem = new HrRPContractFilterDto
        //                {
        //                    EmpCode = item.EmpId,
        //                    EmpName = item.EmpName,
        //                    IdNo = item.IdNo,
        //                    DOAppointment = item.Doappointment,
        //                    ContractExpiryDate = item.ContractExpiryDate,
        //                    RemainingDays = (DateHelper.StringToDate(item.ContractExpiryDate) - DateTime.Now).Days,
        //                    NationalityName = item.NationalityName,
        //                    BranchName = item.BraName,
        //                    DepartmentName = item.DepName,
        //                    LocationName = item.LocationName,
        //                    Salary = item.Salary,
        //                    NetSalary = item.Salary + TotalAllowance - TotalDeduction,
        //                    Deduction = TotalDeduction,
        //                    Allowance = TotalAllowance
        //                };
        //                resultList.Add(newItem);
        //            }
        //            if (resultList.Count > 0) return Ok(await Result<List<HrRPContractFilterDto>>.SuccessAsync(resultList));
        //            return Ok(await Result<List<HrRPContractFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));

        //        }
        //        return Ok(await Result<List<HrRPContractFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));

        //    }
        //    return Ok(await Result<List<HrRPContractFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));
        //}
        //return Ok(await Result<HrRPContractFilterDto>.FailAsync(items.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrRPContractFilterDto>.FailAsync(ex.Message));
            }
        }

        [HttpPost("UpdateContractExpair")]
        public async Task<IActionResult> UpdateContractExpair(List<string> EmpCodes, string? toDate = "")
        {
            var chk = await permission.HasPermission(382, PermissionType.Edit);
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

                var UpdateItem = await mainServiceManager.InvestEmployeeService.UpdateContractExpair(EmpCodes, toDate);
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
