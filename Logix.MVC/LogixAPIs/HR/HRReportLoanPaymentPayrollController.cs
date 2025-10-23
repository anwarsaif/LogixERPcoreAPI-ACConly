using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Result = Logix.Application.Wrapper.Result;

namespace Logix.MVC.LogixAPIs.HR
{

    //  تقرير بالسلف المخصومة من الراتب
    public class HRReportLoanPaymentPayrollController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IMainServiceManager mainServiceManager;
        public HRReportLoanPaymentPayrollController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IMainServiceManager mainServiceManager)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.mainServiceManager = mainServiceManager;
        }

        #region IndexPage

        [HttpPost("Search")]
        public async Task<IActionResult> Search(HRReportLoanPaymentPayrollFilterDto filter)
        {
            var chk = await permission.HasPermission(1048, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var BranchesList = session.Branches.Split(',');

                List<HRReportLoanPaymentPayrollFilterDto> resultList = new List<HRReportLoanPaymentPayrollFilterDto>();
                var items = await hrServiceManager.HrPayrollDService.GetAllVW(e => e.IsDeleted == false &&
                BranchesList.Contains(e.BranchId.ToString()) &&
                e.Loan > 0 &&
                (string.IsNullOrEmpty(filter.EmpCode) || filter.EmpCode == e.EmpCode) &&
                (string.IsNullOrEmpty(filter.EmpName) || e.EmpName.ToLower().Contains(filter.EmpName) || e.EmpName2.ToLower().Contains(filter.EmpName)) &&
                (filter.LocationId == 0 || filter.LocationId == null || filter.LocationId == e.Location) &&
                (filter.MsMonth == 0 || filter.MsMonth == null || filter.MsMonth == Convert.ToInt32(e.MsMonth))


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

                        if (!string.IsNullOrEmpty(filter.FromDate) && !string.IsNullOrEmpty(filter.ToDate))
                        {
                            res = res.Where(c => (c.MsDate != null && DateHelper.StringToDate(c.MsDate) >= DateHelper.StringToDate(filter.FromDate) && DateHelper.StringToDate(c.MsDate) <= DateHelper.StringToDate(filter.ToDate)));
                        }
                        if (res.Count() >= 0)
                        {

                            foreach (var item in res)
                            {
                                var newItem = new HRReportLoanPaymentPayrollFilterDto
                                {
                                    MsCode = item.MsCode,
                                    MsDate = item.MsDate,
                                    MsMonth=Convert.ToInt32( item.MsMonth),
                                    FinancialYear=item.FinancelYear,
                                    EmpCode = item.EmpCode,
                                    EmpName = item.EmpName,
                                    DepartmentName = item.DepName,
                                    LocationName = item.LocationName,
                                    BranchName=item.BraName,
                                    AmountPaid=item.Loan

                                };
                                resultList.Add(newItem);
                            }
                        }
                        if (resultList.Count > 0) return Ok(await Result<List<HRReportLoanPaymentPayrollFilterDto>>.SuccessAsync(resultList));
                        return Ok(await Result<List<HRReportLoanPaymentPayrollFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));

                    }
                    return Ok(await Result<List<HRReportLoanPaymentPayrollFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));

                }
                return Ok(await Result<List<HRReportLoanPaymentPayrollFilterDto>>.FailAsync(items.Status.message.ToString()));

            }
            catch (Exception ex)
            {
                return Ok(await Result<HRReportLoanPaymentPayrollFilterDto>.FailAsync(ex.Message));
            }
        }

        #endregion

    }
}