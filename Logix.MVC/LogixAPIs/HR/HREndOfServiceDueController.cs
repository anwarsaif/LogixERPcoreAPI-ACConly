using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using Result = Logix.Application.Wrapper.Result;

namespace Logix.MVC.LogixAPIs.HR
{

    /// استحقاق نهاية الخدمة
    public class HREndOfServiceDueController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IMainServiceManager mainServiceManager;
        public HREndOfServiceDueController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IMainServiceManager mainServiceManager)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.mainServiceManager = mainServiceManager;
        }

        #region IndexPage

        //[HttpPost("Search")]
        //public async Task<IActionResult> Search(HREndOfServiceDueFilterDto filter)
        //{
        //    var chk = await permission.HasPermission(713, PermissionType.Show);
        //    if (!chk)
        //    {
        //        return Ok(await Result.AccessDenied("AccessDenied"));
        //    }
        //    if (string.IsNullOrEmpty(filter.CurrentDate))
        //    {
        //        return Ok(await Result<object>.FailAsync("يجب ادخال الى تاريخ"));

        //    }
        //    if (filter.LeaveType <= 0)
        //    {
        //        return Ok(await Result<object>.FailAsync("سبب انهاء الخدمة"));

        //    }
        //    try
        //    {
        //        filter.LocationId ??= 0;
        //        filter.DepartmentId ??= 0;
        //        filter.NationalityId ??= 0;
        //        filter.JobCategory ??= 0;
        //        var BranchesList = session.Branches.Split(',');
        //        List<HREndOfServiceDueFilterDto> resultList = new List<HREndOfServiceDueFilterDto>();
        //        var items = await hrServiceManager.HrEmployeeService.GetAllVW(e => e.IsDeleted == false && e.Isdel == false && BranchesList.Contains(e.BranchId.ToString()) && e.StatusId == 1 && e.FacilityId == session.FacilityId &&
        //        (string.IsNullOrEmpty(filter.EmpCode) || filter.EmpCode == e.EmpId) &&
        //        (string.IsNullOrEmpty(filter.EmpName) || (e.EmpName != null && e.EmpName.ToLower().Contains(filter.EmpName)) || (e.EmpName2 != null && e.EmpName2.ToLower().Contains(filter.EmpName))) &&
        //        (filter.LocationId == 0 || filter.LocationId == null || filter.LocationId == e.Location) &&
        //        (filter.DepartmentId == 0 || filter.DepartmentId == null || filter.DepartmentId == e.DeptId) &&
        //        (filter.NationalityId == 0 || filter.NationalityId == null || filter.NationalityId == e.NationalityId) &&
        //        (filter.JobCategory == 0 || filter.JobCategory == null || filter.JobCategory == e.JobCatagoriesId)
        //        );
        //        if (items.Succeeded)
        //        {
        //            if (items.Data.Count() > 0)
        //            {

        //                var res = items.Data.AsQueryable();

        //                if (filter.BranchId != null && filter.BranchId > 0)
        //                {
        //                    res = res.Where(c => c.BranchId != null && c.BranchId.Equals(filter.BranchId));
        //                }
        //                if (res.Count() > 0)
        //                {
        //                    var getAllAllowance = await hrServiceManager.HrAllowanceDeductionService.GetAllVW(e => e.IsDeleted == false && e.TypeId == 1 && e.FixedOrTemporary == 1);

        //                    foreach (var item in res)
        //                    {
        //                        decimal? TotalAllowance = 0;
        //                        var getTotalAllowance = getAllAllowance.Data.Where(e => e.EmpId == item.Id);

        //                        foreach (var Allowanceitem in getTotalAllowance)
        //                        {
        //                            TotalAllowance += (Allowanceitem.Amount != null ? Allowanceitem.Amount.Value : 0);
        //                        }
        //                        decimal EndServiceDue = await hrServiceManager.HrLeaveService.HR_End_Service_Due(filter.CurrentDate, item.Id, filter.LeaveType);
        //                        var newItem = new HREndOfServiceDueFilterDto
        //                        {
        //                            EmpCode = item.EmpId,
        //                            EmpName = item.EmpName ?? "",
        //                            DOAppointment = item.Doappointment ?? "",
        //                            Salary = item.Salary,
        //                            NetSalary = item.Salary + TotalAllowance,
        //                            EndServiceDue = EndServiceDue,
        //                        };
        //                        resultList.Add(newItem);
        //                    }
        //                    if (resultList.Count > 0) return Ok(await Result<List<HREndOfServiceDueFilterDto>>.SuccessAsync(resultList));
        //                    return Ok(await Result<List<HREndOfServiceDueFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));

        //                }
        //                return Ok(await Result<List<HREndOfServiceDueFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));

        //            }
        //            return Ok(await Result<List<HREndOfServiceDueFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));
        //        }
        //        return Ok(await Result<HREndOfServiceDueFilterDto>.FailAsync(items.Status.message));
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(await Result<HrVacationDueFilterDto>.FailAsync(ex.Message));
        //    }
        //}

        [HttpPost("Search")]
        public async Task<IActionResult> Search(HREndOfServiceDueFilterDto filter)
        {
            var chk = await permission.HasPermission(713, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (string.IsNullOrEmpty(filter.CurrentDate))
            {
                return Ok(await Result<object>.FailAsync("يجب ادخال الى تاريخ"));
            }
            try
            {
                List<HREndOfServiceDueFilterDto> resultList = new List<HREndOfServiceDueFilterDto>();
                filter.LocationId ??= 0;
                filter.DepartmentId ??= 0;
                filter.NationalityId ??= 0;
                filter.JobCategory ??= 0;
                filter.LeaveType ??= 0;

                var BranchesList = session.Branches.Split(',');
                var items = await hrServiceManager.HrEmployeeService.GetAllVW(e =>
                    e.IsDeleted == false &&
                    e.Isdel == false &&
                    e.Doappointment != null &&
                    BranchesList.Contains(e.BranchId.ToString()) &&
                    e.StatusId == 1 &&
                    e.FacilityId == session.FacilityId &&
                    (string.IsNullOrEmpty(filter.EmpCode) || filter.EmpCode == e.EmpId) &&
                    (string.IsNullOrEmpty(filter.EmpName) || (e.EmpName != null && e.EmpName.ToLower().Contains(filter.EmpName)) || (e.EmpName2 != null && e.EmpName2.ToLower().Contains(filter.EmpName))) &&
                    (filter.LocationId == 0 || filter.LocationId == e.Location) &&
                    (filter.DepartmentId == 0 || filter.DepartmentId == e.DeptId) &&
                    (filter.NationalityId == 0 || filter.NationalityId == e.NationalityId) &&
                    (filter.JobCategory == 0 || filter.JobCategory == e.JobCatagoriesId) &&
                    (filter.BranchId == 0 || filter.BranchId == e.BranchId)
                );

                if (!items.Succeeded)
                    return Ok(await Result<object>.FailAsync(items.Status.message));

                if (!items.Data.Any())
                    return Ok(await Result<List<HREndOfServiceDueFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));

                var res = items.Data.AsQueryable();
                var getAllLoanInstallment = await hrServiceManager.HrLoanInstallmentService.GetAll(x => x.IsDeleted == false);
                var AllLoanInstallment = getAllLoanInstallment.Data.AsQueryable();
                var LoanInstallmentIdsList = AllLoanInstallment.Select(x => x.LoanId).ToList();
                var getAllLoan = await hrServiceManager.HrLoanService.GetAll(x => x.IsDeleted == false);
                var AllLoan = getAllLoan.Data.AsQueryable();

                var getAllAllowanceDeduction = await hrServiceManager.HrAllowanceDeductionService.GetAllVW(e =>
                    e.IsDeleted == false &&
                    e.Status == true &&
                    e.FixedOrTemporary == 1);

                foreach (var item in res)
                {
                    decimal previousDue = 0;
                    decimal? totalAllowance = getAllAllowanceDeduction.Data
                        .Where(a => a.EmpId == item.Id && a.TypeId == 1)
                        .Sum(a => a.Amount);

                    decimal? totalDeduction = getAllAllowanceDeduction.Data
                        .Where(a => a.EmpId == item.Id && a.TypeId == 2)
                        .Sum(a => a.Amount);
                    var TotalAllowanceDeduction = totalAllowance + totalDeduction;
                    decimal totalSalary = (item.Salary + totalAllowance) ?? 0;
                    decimal EndServiceDue = await hrServiceManager.HrLeaveService.HR_End_Service_Due(filter.CurrentDate, item.Id, (int)filter.LeaveType);

                    int DaysCount = DateHelper.GetCountDays(DateHelper.StringToDate(item.Doappointment), DateHelper.StringToDate(filter.CurrentDate));
                    int MonthsCount = DateHelper.GetCountMonths(DateHelper.StringToDate(item.Doappointment), DateHelper.StringToDate(filter.CurrentDate));
                    int YearsCount = DateHelper.GetCountYears(DateHelper.StringToDate(item.Doappointment), DateHelper.StringToDate(filter.CurrentDate));

                    previousDue = AllLoan.Where(x => Convert.ToInt64(x.EmpId) == item.Id && !LoanInstallmentIdsList.Contains(x.Id)).Sum(x => x.LoanValue) ?? 0;
                    decimal firstFiveYears = 0;
                    firstFiveYears = (YearsCount - 5 >= 0) ?
                        ((totalSalary / 2) * 5) :
                        ((totalSalary / 2) * YearsCount);
                    decimal remainingYears = 0;
                    remainingYears = (YearsCount - 5 >= 0) ?
                        (YearsCount - 5) * totalSalary : 0;
                    decimal forMonths = 0;
                    forMonths = (YearsCount - 5 >= 0) ?
                       ((decimal)MonthsCount / 12 * totalSalary) :
                       ((decimal)MonthsCount / 12 * (totalSalary / 2));

                    decimal forDays = 0;
                    forDays = (YearsCount - 5 >= 0) ?
                         (((decimal)DaysCount / 360) * totalSalary) :
                         (((decimal)DaysCount / 360) * (totalSalary / 2));
                    var loanValueInCurrentDate = AllLoan.Where(x => Convert.ToInt64(x.EmpId) == item.Id && x.LoanDate == filter.CurrentDate && !LoanInstallmentIdsList.Contains(x.Id)).Sum(x => x.LoanValue) ?? 0;
                    decimal net = 0;
                    net = firstFiveYears + remainingYears + forMonths + forDays - loanValueInCurrentDate;

                    resultList.Add(new HREndOfServiceDueFilterDto
                    {
                        EmpCode = item.EmpId,
                        EmpName = item.EmpName,
                        BranchName = item.BraName,
                        Salary = item.Salary,
                        TotalSalary = Math.Round(totalSalary, 2),
                        FirstFiveYears = Math.Round(firstFiveYears, 2),
                        RemainingYears = Math.Round(remainingYears, 2),
                        ForMonths = Math.Round(forMonths, 2),
                        ForDays = Math.Round(forDays, 2),
                        NetSalary = Math.Round(net, 2),
                        EndServiceDue = EndServiceDue,
                        YearsCount = YearsCount,
                        DaysCount = DaysCount,
                        MonthsCount = MonthsCount,
                        previousDue = previousDue,
                        TotalAllowanceDeduction = TotalAllowanceDeduction,
                        DOAppointment = item.Doappointment,
                        PDate = filter.CurrentDate,

                    });
                }

                var sortedList = resultList
                    .OrderBy(r => r.EmpCode)
                    .ToList();

                return Ok(await Result<List<HREndOfServiceDueFilterDto>>.SuccessAsync(sortedList));
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"Error occurred while processing the request: {ex.Message}"));
            }
        }

        #endregion

    }
}