using iText.Commons.Bouncycastle.Asn1.X509;
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

    // إستحقاق الإجازات
    public class HRVacationDueController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        public HRVacationDueController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
        }

        #region IndexPage

        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrVacationDueFilterDto filter)
        {
            var chk = await permission.HasPermission(712, PermissionType.Show);
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
                var DateGregorian = DateTime.Now.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);

                var BranchesList = session.Branches.Split(',');
                List<HrVacationDueFilterDto> resultList = new List<HrVacationDueFilterDto>();
                var items = await hrServiceManager.HrEmployeeService.GetAllVW(e => e.IsDeleted == false && e.Isdel == false && BranchesList.Contains(e.BranchId.ToString()) && e.StatusId == 1 && e.FacilityId == session.FacilityId &&
                (string.IsNullOrEmpty(filter.EmpCode) || filter.EmpCode == e.EmpId) &&
                (string.IsNullOrEmpty(filter.EmpName) || e.EmpName.ToLower().Contains(filter.EmpName) || e.EmpName2.ToLower().Contains(filter.EmpName)) &&
                (filter.LocationId == 0 || filter.LocationId == null || filter.LocationId == e.Location) &&
                (filter.DepartmentId == 0 || filter.DepartmentId == null || filter.DepartmentId == e.DeptId) &&
                (filter.NationalityId == 0 || filter.NationalityId == null || filter.NationalityId == e.NationalityId) &&
                (filter.JobCategory == 0 || filter.JobCategory == null || filter.JobCategory == e.JobCatagoriesId)
                );
                if (items.Succeeded)
                {
                    if (items.Data.Count() > 0)
                    {

                        var res = items.Data.AsQueryable();

                        if (filter.BranchId != null && filter.BranchId > 0)
                        {
                            res = res.Where(c => c.BranchId != null && c.BranchId.Equals(filter.BranchId));
                        }
                        if (res.Count()>0)
                        {
                            foreach (var item in res)
                            {
                                decimal? TotalAllowance = 0;
                                var getTotalAllowance = await hrServiceManager.HrAllowanceDeductionService.GetTotalAllowances((long)item.Id);
                                if (getTotalAllowance.Succeeded) TotalAllowance = getTotalAllowance.Data;

                                decimal VacationBalance = await hrServiceManager.HrVacationsService.Vacation_Balance_FN(filter.CurrentDate, item.Id);
                                var newItem = new HrVacationDueFilterDto
                                {
                                    EmpCode = item.EmpId,
                                    EmpName = item.EmpName??"",
                                    DOAppointment = item.Doappointment??"",
                                    Salary = item.Salary,
                                    NetSalary = item.Salary + TotalAllowance ,
                                    Due = Math.Round(((decimal)VacationBalance * ((decimal)(item.Salary + TotalAllowance ) <= 0 ? 0 : (decimal)(item.Salary + TotalAllowance ) / 30)),2),
                                    VacationBalance = VacationBalance,
                                    VacationDaysYear = item.VacationDaysYear
                                };
                                resultList.Add(newItem);
                            }
                            if (resultList.Count > 0) return Ok(await Result<List<HrVacationDueFilterDto>>.SuccessAsync(resultList));
                            return Ok(await Result<List<HrVacationDueFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));

                        }
                        return Ok(await Result<List<HrVacationDueFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));

                    }
                    return Ok(await Result<List<HrVacationDueFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));
                }
                return Ok(await Result<HrVacationDueFilterDto>.FailAsync(items.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrVacationDueFilterDto>.FailAsync(ex.Message));
            }
        }

        #endregion

    }
}