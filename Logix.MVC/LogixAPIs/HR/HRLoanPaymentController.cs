using DevExpress.CodeParser;
using DevExpress.XtraExport.Helpers;
using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Helpers;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Operations;
using System.Globalization;
using Result = Logix.Application.Wrapper.Result;

namespace Logix.MVC.LogixAPIs.HR
{
    //   سداد السلف
    public class HRLoanPaymentController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IMainServiceManager mainServiceManager;


        public HRLoanPaymentController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization,  IMainServiceManager mainServiceManager)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.mainServiceManager = mainServiceManager;
        }


        #region IndexPage


        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrLoanPaymentFilterDto filter)
        {
            var chk = await permission.HasPermission(1215, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {

                var BranchesList = session.Branches.Split(',');

                filter.PayAmount ??= 0;
                filter.DeptId ??= 0;
                filter.BranchId ??= 0;
                filter.Location ??= 0;
                var items = await hrServiceManager.HrLoanPaymentService.GetAllVW(e => e.IsDeleted == false &&
                (filter.PayAmount == 0 || e.PayAmount == filter.PayAmount) &&
                (string.IsNullOrEmpty(filter.EmpCode) || e.EmpCode == filter.EmpCode) &&
                (string.IsNullOrEmpty(filter.EmpName) || e.EmpName.Contains(filter.EmpName)) &&
                (string.IsNullOrEmpty(filter.VoucherNo) || e.VoucherNo == filter.VoucherNo) &&
                (filter.BranchId != 0 ? e.BranchId == filter.BranchId : BranchesList.Contains(e.BranchId.ToString())) &&
                (filter.DeptId == 0 || e.DeptId == filter.DeptId) &&
                (filter.Location == 0 || e.Location == filter.Location)
                );
                if (items.Succeeded)
                {
                    if (items.Data.Count() > 0)
                    {
                        var res = items.Data.AsQueryable();
                        if (!string.IsNullOrEmpty(filter.FromDate) && !string.IsNullOrEmpty(filter.ToDate))
                        {
                            res = res.Where(x => x.PayDate != null &&
                            DateHelper.StringToDate(x.PayDate) >= DateHelper.StringToDate(filter.FromDate) && DateHelper.StringToDate(x.PayDate) <= DateHelper.StringToDate(filter.ToDate)
                            ).AsQueryable();
                        }
                        if (res.Any())
                            return Ok(await Result<IQueryable<HrLoanPaymentVw>>.SuccessAsync(res, ""));
                        return Ok(await Result<IQueryable<HrLoanPaymentVw>>.SuccessAsync(res, localization.GetResource1("NosearchResult")));
                    }
                    return Ok(await Result<List<HrLoanPaymentVw>>.SuccessAsync(items.Data.ToList(), localization.GetResource1("NosearchResult")));
                }
                return Ok(await Result<HrLoanPaymentVw>.FailAsync(items.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrLoanPaymentVw>.FailAsync(ex.Message));
            }
        }

        [HttpPost("GetPagination")]
        public async Task<IActionResult> GetPagination([FromBody] HrLoanPaymentFilterDto filter, int take = Pagination.take, long? lastSeenId = null)
        {
            var chk = await permission.HasPermission(1215, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {

                var BranchesList = session.Branches.Split(',');

                var dateConditions = new List<DateCondition>();
                dateConditions.Add(new DateCondition
                {
                    DatePropertyName = "PayDate",
                    ComparisonOperator = ComparisonOperator.GreaterThanOrEqual,
                    StartDateString = filter.FromDate ?? ""
                });
                dateConditions.Add(new DateCondition
                {
                    DatePropertyName = "PayDate",
                    ComparisonOperator = ComparisonOperator.LessThanOrEqual,
                    StartDateString = filter.ToDate ?? ""
                });

                filter.PayAmount ??= 0;
                filter.DeptId ??= 0;
                filter.BranchId ??= 0;
                filter.Location ??= 0;
                var items = await hrServiceManager.HrLoanPaymentService.GetAllWithPaginationVW(selector: e => e.Id,
                expression: e =>
                        e.IsDeleted == false &&
                (filter.PayAmount == 0 || e.PayAmount == filter.PayAmount) &&
                (string.IsNullOrEmpty(filter.EmpCode) || e.EmpCode == filter.EmpCode) &&
                (string.IsNullOrEmpty(filter.EmpName) || e.EmpName.Contains(filter.EmpName)) &&
                (string.IsNullOrEmpty(filter.VoucherNo) || e.VoucherNo == filter.VoucherNo) &&
                (filter.BranchId != 0 ? e.BranchId == filter.BranchId : BranchesList.Contains(e.BranchId.ToString())) &&
                (filter.DeptId == 0 || e.DeptId == filter.DeptId) &&
                (filter.Location == 0 || e.Location == filter.Location),
                    take: take,
                    lastSeenId: lastSeenId,
                   dateConditions: (string.IsNullOrEmpty(filter.FromDate) || string.IsNullOrEmpty(filter.ToDate)) ? null : dateConditions);

                if (!items.Succeeded)
                    return Ok(await Result<List<HrLoanPaymentVw>>.FailAsync(items.Status.message));
                if (items.Data.Count() > 0)
                {
                    var res = items.Data.AsQueryable();
                    var lang = session.Language;
                    var paginatedData = new PaginatedResult<object>
                    {
                        Succeeded = items.Succeeded,
                        Data = items.Data,
                        Status = items.Status,
                        PaginationInfo = items.PaginationInfo
                    };
                    return Ok(paginatedData);

                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long Id)
        {
            try
            {
                var chk = await permission.HasPermission(1215, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (Id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await hrServiceManager.HrLoanPaymentService.DeleteHrLoanPayment(Id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete HRLoanPaymentController, MESSAGE: {ex.Message}"));
            }
        }

        #endregion


        #region AddPage Business

        [HttpPost("Add")]
        public async Task<ActionResult> Add(HrLoanPaymentAddDto obj)
        {
            try
            {
                var CurrentDate = DateTime.Now.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);
                var chk = await permission.HasPermission(1215, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result<string>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));
                bool ErrorExsits = false;
                decimal AmountTotal = 0;
                foreach(var item in obj.Details)
                {
                    var TxtAmount = item.NewAmount;
                    var InstallValue = item.Amount;
                    if( !string.IsNullOrEmpty(item.Amount.ToString()))
                    {
                        if(decimal.Parse(TxtAmount.ToString()) != decimal.Parse(InstallValue.ToString()))
                        {
                            ErrorExsits = true;
                        }
                        else
                        {
                            AmountTotal += TxtAmount;
                        }
                    }
                }
                if (ErrorExsits)
                {
                    return Ok(await Result<string>.WarningAsync(localization.GetMessagesResource("AmountNotEqualInstallment")));
                }
                if( AmountTotal != decimal.Parse(obj.PayAmount.ToString()))
                {
                    return Ok(await Result<string>.WarningAsync(localization.GetMessagesResource("LoanValueError2")));
                }                    
                var add = await hrServiceManager.HrLoanPaymentService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"====== Exp in HRLoanPaymentController  Add Method, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetLoanInstallmentbyEmpCode")]
        public async Task<IActionResult> GetLoanInstallmentbyEmpCode(string empCode)
        {
            try
            {
                var chk = await permission.HasPermission(1215, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (string.IsNullOrEmpty(empCode))
                {
                    return Ok(await Result<HrLoanInstallmentVw>.FailAsync($"please choose an employeeCode"));
                }


                var getEmpId = await mainServiceManager.InvestEmployeeService.GetOne(X => X.Isdel == false && X.IsDeleted == false && X.EmpId == empCode);
                if (!getEmpId.Succeeded) return Ok(await Result<HrLoanPaymentVw>.FailAsync($"error:::{getEmpId.Status.message.ToString()}"));

                if (getEmpId.Data.Id == null || getEmpId.Data.Id <= 0) return Ok(await Result<HrLoanInstallmentVw>.FailAsync(localization.GetResource1("EmployeeNotFound")));
                var installmentItems = await hrServiceManager.HrLoanInstallmentService.GetAllVW(x => x.IsDeleted == false && x.IsPaid == false && x.EmpId == getEmpId.Data.Id.ToString()&&x.IsDeletedM==false);
                return Ok(installmentItems);

            }
            catch (Exception ex)
            {
                return Ok(await Result<HrLoanPaymentVw>.FailAsync($"====== Exp in HRLoanPaymentController getById, MESSAGE: {ex.Message}"));
            }
        }

        #endregion

        #region EditPage


        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long Id)
        {
            try
            {
                var chk = await permission.HasPermission(1215, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id <= 0)
                {
                    return Ok(await Result<HrLoanPaymentVw>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var item = await hrServiceManager.HrLoanPaymentService.GetOneVW(x => x.Id == Id);
                if (item.Succeeded)
                {
                    var installmentItems = await hrServiceManager.HrLoanInstallmentPaymentService.GetAllVW(x => x.LoanPaymentId == Id);

                    var getFiles = await mainServiceManager.SysFileService.GetFilesForUser(Id, 157);
                    return Ok(await Result<object>.SuccessAsync(new { LoanData = item.Data, LoanInstallmentPaymentData = installmentItems.Data.ToList(), fileDtos = getFiles.Data }));
                }
                return Ok(item);

            }
            catch (Exception ex)
            {
                return Ok(await Result<HrLoanPaymentVw>.FailAsync($"====== Exp in HRLoanPaymentController getById, MESSAGE: {ex.Message}"));
            }
        }

        #endregion
    }
}