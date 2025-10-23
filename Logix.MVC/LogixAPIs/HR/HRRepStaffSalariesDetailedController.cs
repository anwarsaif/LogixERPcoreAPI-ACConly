using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Result = Logix.Application.Wrapper.Result;

namespace Logix.MVC.LogixAPIs.HR
{
    // تقرير رواتب الموظفين تفصيلي
    public class HRRepStaffSalariesDetailedController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IAccServiceManager accServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ILocalizationService localization;
        private readonly ICurrentData session;

        public HRRepStaffSalariesDetailedController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ILocalizationService localization, ICurrentData session, IAccServiceManager accServiceManager)
        {
            this.permission = permission;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.session = session;
            this.accServiceManager = accServiceManager;
        }

        #region الصفحة الرئيسية



        [HttpPost("Search")]

        public async Task<IActionResult> Search(HrRepStaffSalariesDetailedFilterDto filter)
        {
            var chk = await permission.HasPermission(1023, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                List<HrRepStaffSalariesDetailedFilterDto> resultList = new List<HrRepStaffSalariesDetailedFilterDto>();


                filter.FacilityId ??= 0;
                filter.StatusId ??= 0;
                filter.BranchId ??= 0;
                filter.DeptId ??= 0;
                filter.Location ??= 0;
                filter.NationalityId ??= 0;
                filter.WagesProtection ??= 0;
                filter.ContractTypeID ??= 0;
                var BranchesList = session.Branches.Split(',');

                var items = await hrServiceManager.HrEmployeeService.GetAllVW(x => x.IsDeleted == false && x.Isdel == false
                && BranchesList.Contains(x.BranchId.ToString())
                && (filter.FacilityId == 0 || filter.FacilityId == x.FacilityId)
                && (filter.DeptId == 0 || filter.DeptId == x.DeptId)
                && (filter.Location == 0 || filter.Location == x.Location)
                && (filter.StatusId == 0 || filter.StatusId == x.StatusId)
                && (filter.ContractTypeID == 0 || filter.ContractTypeID == x.ContractTypeId)
                && (filter.NationalityId == 0 || filter.NationalityId == x.NationalityId)
                && (filter.WagesProtection == 0 || filter.WagesProtection == x.WagesProtection)
                && (string.IsNullOrEmpty(filter.EmpCode) || filter.EmpCode == x.EmpId)
                && (string.IsNullOrEmpty(filter.EmpName) || x.EmpName.ToLower().Contains(filter.EmpName.ToLower()))

                );
                if (items.Succeeded)
                {
                    if (items.Data.Count() > 0)
                    {
                        var res = items.Data.AsQueryable();
                        if (filter.BranchId > 0)
                        {
                            res = res.Where(c => c.BranchId != null && c.BranchId == filter.BranchId);
                        }
                        if (res.Count() > 0)
                        {

                            var getAllAllowancesAndDeductions = await hrServiceManager.HrAllowanceDeductionService.GetAllVW(x => x.IsDeleted == false && x.FixedOrTemporary == 1);

                            foreach (var item in res)
                            {
                                decimal TotalAllowance = 0;
                                decimal TotalDeduction = 0;
                                List<DeductionItem>? Deductions = new List<DeductionItem>();
                                List<AllowanceItem>? Allowances = new List<AllowanceItem>();
                                var newResultList = new HrRepStaffSalariesDetailedFilterDto();
                                newResultList.EmpCode = item.EmpId ?? "";
                                newResultList.EmpName = item.EmpName ?? "";
                                newResultList.NationalityName = item.NationalityName ?? "";
                                newResultList.FacilityName = item.FacilityName ?? "";
                                newResultList.DepartmentName = item.DepName ?? "";
                                newResultList.LocationName = item.LocationName ?? "";
                                newResultList.DoAppointment = item.Doappointment ?? "";
                                newResultList.Salary = item.Salary ?? 0;

                                var allAllowances = getAllAllowancesAndDeductions.Data.Where(x => x.TypeId == 1 && x.EmpId == item.Id);
                                foreach (var singleAllowance in allAllowances)
                                {
                                    var newAllowanceItem = new AllowanceItem
                                    {
                                        Amount = (singleAllowance.Amount != null ? singleAllowance.Amount.Value : 0),
                                        Name = singleAllowance.Name ?? ""
                                    };
                                    Allowances.Add(newAllowanceItem);
                                    TotalAllowance += (singleAllowance.Amount != null ? singleAllowance.Amount.Value : 0);

                                }
                                newResultList.Allowances = Allowances;

                                var allDeductions = getAllAllowancesAndDeductions.Data.Where(x => x.TypeId == 2 && x.EmpId == item.Id);
                                foreach (var singleDeduction in allDeductions)
                                {
                                    var newDeductionItem = new DeductionItem
                                    {
                                        Amount = (singleDeduction.Amount != null ? singleDeduction.Amount.Value : 0),
                                        Name = singleDeduction.Name ?? ""
                                    };
                                    Deductions.Add(newDeductionItem);
                                    TotalDeduction += (singleDeduction.Amount != null ? singleDeduction.Amount.Value : 0);

                                }
                                newResultList.Deductions = Deductions;
                                newResultList.TotalSalary = item.Salary + TotalAllowance - TotalDeduction;
                                resultList.Add(newResultList);
                            }
                            return Ok(await Result<object>.SuccessAsync(resultList, ""));

                        }
                        return Ok(await Result<object>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));

                    }

                    return Ok(await Result<object>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));

                }

                return Ok(await Result<object>.FailAsync(items.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync(ex.Message));
            }
        }
        #endregion








    }
}
