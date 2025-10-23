using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Result = Logix.Application.Wrapper.Result;

namespace Logix.MVC.LogixAPIs.HR
{
    //  تقرير بالرواتب المعتمدة
    public class HRRPPayrollApprovedController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IAccServiceManager accServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IMainServiceManager mainServiceManager;
        public HRRPPayrollApprovedController(IHrServiceManager hrServiceManager, IAccServiceManager accServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IMainServiceManager mainServiceManager)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.mainServiceManager = mainServiceManager;
            this.accServiceManager = accServiceManager;
        }

        #region IndexPage

        [HttpPost("Search")]
        public async Task<IActionResult> Search(HRRPPayrollApprovedFilterDto filter)
        {
            var chk = await permission.HasPermission(1601, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                if (filter.FinancialYear == 0 || filter.FinancialYear == null)
                {
                    return Ok(await Result<HrPayrollFilterDto>.FailAsync(" يجب اختيار السنة المالية"));
                }

                var BranchesList = session.Branches.Split(',');
                List<HRRPPayrollApprovedFilterDto> resultList = new List<HRRPPayrollApprovedFilterDto>();

                // Get filtered payroll data
                var items = await hrServiceManager.HrPayrollService.GetAllVW(e => e.IsDeleted == false &&
                                                                            (e.BranchId == 0 || BranchesList.Contains(e.BranchId.ToString())) &&
                                                                             e.State == 4 &&
                                                                             e.FacilityId == session.FacilityId &&
                                                                             (filter.FinancialYear == null || filter.FinancialYear == 0 || filter.FinancialYear == e.FinancelYear) &&
                                                                             (filter.PayrollType == null || filter.PayrollType == 0 || filter.PayrollType == e.PayrollTypeId) &&
                                                                             (filter.MSCode == null || filter.MSCode == 0 || filter.MSCode == e.MsCode) &&
                                                                             (filter.AppCode == null || filter.AppCode == 0 || filter.AppCode == e.ApplicationCode) &&
                                                                             (filter.Month == null || filter.Month == 0 || Convert.ToInt32(filter.Month) == Convert.ToInt32(e.MsMonth))
                                                                             );

                if (items.Succeeded)
                {
                    if (items.Data.Count() > 0)
                    {
                        // Get all journals (assuming you want all for filtering)
                        var allJournals = await accServiceManager.AccJournalMasterService.GetAll(x => x.FlagDelete == false && x.DocTypeId == 24 && x.ReferenceNo != null);

                        // Get all PayrollDetails (assuming you want all for filtering)
                        var allPayrollDetails = await hrServiceManager.HrPayrollDService.GetAll(x => x.IsDeleted == false);

                        foreach (var payroll in items.Data)
                        {
                            decimal? TotalPayroll = 0;
                            var matchingJournal = allJournals.Data.FirstOrDefault(j => j.ReferenceNo == payroll.MsId);
                            var FilteredPayrollD = allPayrollDetails.Data.Where(X => X.MsId == payroll.MsId);
                            string JCode = "";
                            if (matchingJournal != null)
                            {
                                JCode = matchingJournal.JCode;
                            }
                            TotalPayroll = (FilteredPayrollD.Sum(x => x.Salary) + FilteredPayrollD.Sum(x => x.Allowance) + FilteredPayrollD.Sum(x => x.OverTime) + FilteredPayrollD.Sum(x => x.Commission) + FilteredPayrollD.Sum(x => x.Mandate) - FilteredPayrollD.Sum(x => x.Absence) - FilteredPayrollD.Sum(x => x.Delay) - FilteredPayrollD.Sum(x => x.Loan) - FilteredPayrollD.Sum(x => x.Deduction) - FilteredPayrollD.Sum(x => x.Penalties)) ?? 0;
                            resultList.Add(new HRRPPayrollApprovedFilterDto
                            {
                                JCode = JCode,
                                StatusName = (session.Language == 1) ? payroll.StatusName : payroll.StatusName2,
                                TypeName = (session.Language == 1) ? payroll.TypeName : payroll.TypeName2,
                                TotalPayroll = TotalPayroll,
                                FinancialYear = payroll.FinancelYear,
                                MSMonth = payroll.MsMonth,
                                PayrollNo = payroll.MsCode.ToString(),
                                MSTitle = payroll.MsTitle,
                                MSCode = payroll.MsCode,
                                MSDate = payroll.MsDate,


                            });
                        }

                        if (resultList.Any())
                        {
                            return Ok(await Result<List<HRRPPayrollApprovedFilterDto>>.SuccessAsync(resultList, ""));
                        }
                        return Ok(await Result<List<HRRPPayrollApprovedFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));
                    }
                    return Ok(await Result<List<HRRPPayrollApprovedFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));
                }
                return Ok(await Result<HRRPPayrollApprovedFilterDto>.FailAsync(items.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result<HRRPPayrollApprovedFilterDto>.FailAsync(ex.Message));
            }
        }

        #endregion

    }
}