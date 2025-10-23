using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Helpers;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Result = Logix.Application.Wrapper.Result;

namespace Logix.MVC.LogixAPIs.HR
{

    //  تقرير بإستحقاقات السلف
    public class HRLoanInstallmentReportController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IMainServiceManager mainServiceManager;
        public HRLoanInstallmentReportController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IMainServiceManager mainServiceManager)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.mainServiceManager = mainServiceManager;
        }

        #region IndexPage

        [HttpPost("Search")]
        public async Task<IActionResult> Search(HRLoanInstallmentReportFilterDto filter)
        {
            var chk = await permission.HasPermission(1577, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                filter.IsPaid ??= 2;
                filter.BranchId ??= 0;
                var BranchesList = session.Branches.Split(',');
                List<HRLoanInstallmentReportFilterDto> resultList = new List<HRLoanInstallmentReportFilterDto>();
                var items = await hrServiceManager.HrLoanInstallmentService.GetAllVW(e => e.IsDeleted == false &&
                BranchesList.Contains(e.BranchId.ToString()) &&
                (string.IsNullOrEmpty(filter.EmpCode) || filter.EmpCode == e.EmpCode) &&
                (string.IsNullOrEmpty(filter.EmpName) || e.EmpName != null && (e.EmpName.ToLower().Contains(filter.EmpName))) &&
                (filter.IsPaid == 2 || Convert.ToBoolean(filter.IsPaid) == e.IsPaid) &&
                (filter.BranchId == 0 || filter.BranchId == e.BranchId)
                );
                if (items.Succeeded)
                {
                    if (items.Data.Count() > 0)
                    {

                        var res = items.Data.AsQueryable();

                        if (!string.IsNullOrEmpty(filter.FromDate) && !string.IsNullOrEmpty(filter.ToDate))
                        {
                            res = res.Where(c => (c.DueDate != null && DateHelper.StringToDate(c.DueDate) >= DateHelper.StringToDate(filter.FromDate) && DateHelper.StringToDate(c.DueDate) <= DateHelper.StringToDate(filter.ToDate)));
                        }
                        if (res.Count() >= 0)
                        {

                            foreach (var item in res)
                            {


                                var newItem = new HRLoanInstallmentReportFilterDto
                                {
                                    LoanID = item.LoanId,
                                    LoanDate = item.LoanDate,
                                    EmpCode = item.EmpCode,
                                    EmpName = item.EmpName,
                                    Amount = item.LoanValue,
                                    InstallmentNo = item.IntallmentNo,
                                    DueDate = item.DueDate,
                                    Installment = item.Amount,
                                    PaymentStatus=item.IsPaid
                                };
                                resultList.Add(newItem);
                            }
                        }
                        if (resultList.Count > 0) return Ok(await Result<List<HRLoanInstallmentReportFilterDto>>.SuccessAsync(resultList));
                        return Ok(await Result<List<HRLoanInstallmentReportFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));

                    }
                    return Ok(await Result<List<HRLoanInstallmentReportFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));

                }
                return Ok(await Result<List<HRLoanInstallmentReportFilterDto>>.FailAsync(items.Status.message.ToString()));

            }
            catch (Exception ex)
            {
                return Ok(await Result<HRLoanInstallmentReportFilterDto>.FailAsync(ex.Message));
            }
        }

        #endregion

    }
}