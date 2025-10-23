using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using Result = Logix.Application.Wrapper.Result;

namespace Logix.MVC.LogixAPIs.HR
{

    // تقرير بالسلف
    public class HRLoanReportController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IMainServiceManager mainServiceManager;


        public HRLoanReportController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IMainServiceManager mainServiceManager)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.mainServiceManager = mainServiceManager;
        }


        #region IndexPage

        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrLoanFilterDto filter)
        {
            List<HrLoanFilterDto> results = new List<HrLoanFilterDto>();
            var chk = await permission.HasPermission(261, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
        //var BranchesList = session.Branches.Split(',');
        //var getLoanInstallment = await hrServiceManager.HrLoanInstallmentService.GetAll(e => e.IsDeleted == false);

        //var loanInstallments = getLoanInstallment.Data.ToList();

        //var getFromLoanVws = await hrServiceManager.HrLoanService.GetAllVW(e => e.IsDeleted == false && e.StatusId != 2 &&
        //    (filter.DeptId == null || filter.DeptId == 0 || filter.DeptId == e.DeptId) &&
        //    (BranchesList.Contains(e.BranchId.ToString())) &&
        //    (filter.Location == null || filter.Location == 0 || filter.Location == e.Location) &&
        //    (filter.Type == null || filter.Type == 0 || filter.Type == e.Type));

        //if (getFromLoanVws.Succeeded)
        //{
        //    if (getFromLoanVws.Data.Count() > 0)
        //    {
        //        var res = getFromLoanVws.Data.AsEnumerable();

        //        if (filter.BranchId > 0)
        //        {
        //            res = res.Where(x => x.BranchId == filter.BranchId);
        //        }
        //        if (!string.IsNullOrEmpty(filter.From) && !string.IsNullOrEmpty(filter.To))
        //        {
        //            res = res.Where(x => x.LoanDate != null &&
        //                                 DateHelper.StringToDate(x.LoanDate) >= DateHelper.StringToDate(filter.From) &&
        //                                 DateHelper.StringToDate(x.LoanDate) <= DateHelper.StringToDate(filter.To));
        //        }

        //        res = res.Where(e =>
        //            (filter.LoanNature == null || filter.LoanNature == 0 ||
        //             (filter.LoanNature == 1 && loanInstallments.Any(x => x.LoanId == e.Id && x.IsDeleted == false)) ||
        //             (filter.LoanNature == 2 && !loanInstallments.Any(x => x.LoanId == e.Id && x.IsDeleted == false))) &&
        //            (filter.LoanStatus == null || filter.LoanStatus == 0 ||
        //             (filter.LoanStatus == 1 &&
        //              loanInstallments.Where(x => x.LoanId == e.Id && x.IsPaid == false && x.IsDeleted == false).Sum(x => x.Amount) > 0) ||
        //             (filter.LoanStatus == 2 &&
        //              loanInstallments.Where(x => x.LoanId == e.Id && x.IsPaid == false && x.IsDeleted == false).Sum(x => x.Amount) == 0)) &&
        //            (string.IsNullOrEmpty(filter.EmpCode) || filter.EmpCode == e.EmpCode) &&
        //            (string.IsNullOrEmpty(filter.EmpName) || e.EmpName.ToLower().Contains(filter.EmpName.ToLower())));

        //        if (res.Any())
        //        {
        //            foreach (var item in res)
        //            {
        //                var paidInstallments = loanInstallments
        //                    .Where(x => x.LoanId == item.Id && x.IsPaid == true && x.IsDeleted == false)
        //                    .Sum(x => x.Amount);

        //                var remainingAmount = (item.LoanValue ?? 0) - paidInstallments;

        //                var singleItem = new HrLoanFilterDto
        //                {
        //                    LoanDate = item.LoanDate,
        //                    LoanValue = item.LoanValue,
        //                    InstallmentValue = item.InstallmentValue,
        //                    InstallmentCount = item.InstallmentCount,
        //                    BraName = item.BraName,
        //                    DepName = item.DepName,
        //                    LocationName = item.LocationName,
        //                    Note = item.Note,
        //                    EmpName = item.EmpName,
        //                    EndDatePayment = item.EndDatePayment,
        //                    StartDatePayment = item.StartDatePayment,
        //                    EmpCode = item.EmpCode,
        //                    RemainingAmount = remainingAmount
        //                };

        //                results.Add(singleItem);
        //            }

        //            return Ok(await Result<List<HrLoanFilterDto>>.SuccessAsync(results, ""));
        //        }

        //        return Ok(await Result<List<HrLoanFilterDto>>.SuccessAsync(results, localization.GetResource1("NosearchResult")));
        //    }

        //    return Ok(await Result<List<HrLoanFilterDto>>.SuccessAsync(results, localization.GetResource1("NosearchResult")));
        //}

        //return Ok(await Result<HrLoanPaymentVw>.FailAsync(getFromLoanVws.Status.message));
                  var getFromLoanVws = await hrServiceManager.HrLoanService.Search(filter);
                  return Ok(getFromLoanVws);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrLoanPaymentVw>.FailAsync(ex.Message));
            }
        }

        #endregion

    }
}
