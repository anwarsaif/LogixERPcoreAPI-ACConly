using AutoMapper;
using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.DTOs.Main;
using Logix.Application.Helpers;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.Infrastructure.Repositories;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using Result = Logix.Application.Wrapper.Result;

namespace Logix.MVC.LogixAPIs.HR
{

    //   السلف
    public class HRLoanController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IMainServiceManager mainServiceManager;
        private readonly IMapper mapper;

        public HRLoanController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IMainServiceManager mainServiceManager, IMapper mapper)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.mainServiceManager = mainServiceManager;
            this.mapper = mapper;
        }


        #region IndexPage

        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrLoanFilterDto filter)
        {
            var chk = await permission.HasPermission(171, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {

                var items = await hrServiceManager.HrLoanService.Search(filter);
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrLoanPaymentVw>.FailAsync(ex.Message));
            }
        }

        [HttpPost("GetPagination")]
        public async Task<IActionResult> GetPagination([FromBody] HrLoanFilterDto filter, int take = Pagination.take, long? lastSeenId = null)
        {
            var chk = await permission.HasPermission(171, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {

                var resultList = new List<HrLoanFilterDto>();
                var branchesList = session.Branches.Split(',');

                var dateConditions = new List<DateCondition>();
                dateConditions.Add(new DateCondition
                {
                    DatePropertyName = "LoanDate",
                    ComparisonOperator = ComparisonOperator.GreaterThanOrEqual,
                    StartDateString = filter.From ?? ""
                });
                dateConditions.Add(new DateCondition
                {
                    DatePropertyName = "LoanDate",
                    ComparisonOperator = ComparisonOperator.LessThanOrEqual,
                    StartDateString = filter.To ?? ""
                });

                filter.DeptId ??= 0;
                filter.BranchId ??= 0;
                filter.Type ??= 0;
                filter.Location ??= 0;
                var items = await hrServiceManager.HrLoanService.GetAllWithPaginationVW(selector: e => e.Id,
                expression: e => e.IsDeleted == false &&
                    (filter.DeptId == 0 || filter.DeptId == e.DeptId) &&
                      (filter.BranchId != 0
                            ? e.BranchId == filter.BranchId
                            : branchesList.Contains(e.BranchId.ToString())) &&
                    (filter.Location == 0 || filter.Location == e.Location) &&
                    (filter.Type == 0 || filter.Type == e.Type) &&
                    (string.IsNullOrEmpty(filter.EmpCode) || filter.EmpCode == e.EmpCode) &&
                    (string.IsNullOrEmpty(filter.EmpName) || e.EmpName.ToLower().Contains(filter.EmpName.ToLower())),
                    take: take,
                    lastSeenId: lastSeenId,
                    dateConditions: (string.IsNullOrEmpty(filter.From) || string.IsNullOrEmpty(filter.To)) ? null : dateConditions);

                if (!items.Succeeded)
                    return Ok(await Result<List<HrLoanFilterDto>>.FailAsync(items.Status.message));

                if (items == null || !items.Data.Any())
                    return Ok(await Result<List<HrLoanFilterDto>>.SuccessAsync(resultList));

                var res = items.Data.AsQueryable();

                // الأقساط المدفوعة
                var installments = await hrServiceManager.HrLoanInstallmentService
                    .GetAll(x => x.IsDeleted == false && x.IsPaid == true);

                var paidInstallments = installments.Data
                    .GroupBy(x => x.LoanId)
                    .Select(g => new { LoanId = g.Key, PaidAmount = g.Sum(x => x.Amount) })
                    .ToDictionary(x => x.LoanId, x => x.PaidAmount);

                // تجهيز النتيجة
                var resList = res.ToList();
                if (resList.Any())
                {
                    resultList = resList.Select(item =>
                    {
                        var paidAmount = paidInstallments.TryGetValue(item.Id, out var val) ? val : 0;
                        var remainingAmount = (item.LoanValue ?? 0) - paidAmount;

                        return new HrLoanFilterDto
                        {
                            Id = item.Id,
                            LoanDate = item.LoanDate,
                            LoanValue = item.LoanValue,
                            InstallmentValue = item.InstallmentValue,
                            InstallmentCount = item.InstallmentCount,
                            BraName = item.BraName,
                            DepName = item.DepName,
                            EmpName = item.EmpName,
                            LocationName = item.LocationName,
                            BraName2 = item.BraName2,
                            DepName2 = item.DepName2,
                            EmpName2 = item.EmpName2,
                            LocationName2 = item.LocationName2,
                            Note = item.Note,
                            EndDatePayment = item.EndDatePayment,
                            StartDatePayment = item.StartDatePayment,
                            EmpCode = item.EmpCode,
                            RemainingAmount = remainingAmount
                        };
                    }).ToList();
                }
                if (items.Data.Count() > 0)
                {
                    var lang = session.Language;
                    var paginatedData = new PaginatedResult<object>
                    {
                        Succeeded = items.Succeeded,
                        Data = resultList.ToList(),
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
                var chk = await permission.HasPermission(171, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (Id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await hrServiceManager.HrLoanService.DeleteHrLoan(Id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete HRLoanController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetEmployeeById")]
        public async Task<IActionResult> GetEmployeeById(string EmpId)
        {
            var chk = await permission.HasPermission(28, PermissionType.Show);
            if (!chk)
            {
                return BadRequest(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var item = await hrServiceManager.HrEmployeeService.GetOneVW(e => e.EmpId == EmpId && e.IsDeleted == false);


                if (item.Succeeded)
                {
                    if (item.Data != null)
                    {
                        decimal TotalAllowance = 0;
                        decimal TotalDeduction = 0;

                        ///////////////////////////
                        var result = mapper.Map<EmpDataForEditDto>(item.Data);
                        result.HrEmployeeVw = item.Data;


                        var getTotalAllowance = await hrServiceManager.HrAllowanceDeductionService.GetAll(e => e.EmpId == item.Data.Id && e.IsDeleted == false && e.TypeId == 1 && e.FixedOrTemporary == 1);

                        if (getTotalAllowance.Succeeded)
                        {
                            foreach (var item1 in getTotalAllowance.Data)
                            {
                                TotalAllowance += (item1.Amount != null ? item1.Amount.Value : 0);
                            }
                        }
                        var getTotalDeduction = await hrServiceManager.HrAllowanceDeductionService.GetAll(e => e.EmpId == item.Data.Id && e.IsDeleted == false && e.TypeId == 2 && e.FixedOrTemporary == 1);
                        if (getTotalDeduction.Succeeded)
                        {
                            foreach (var item2 in getTotalDeduction.Data)
                            {
                                TotalDeduction += (item2.Amount != null ? item2.Amount.Value : 0);
                            }
                        }
                        var totalSalary = TotalAllowance + item.Data.Salary;
                        var netSalary = TotalAllowance + item.Data.Salary - TotalDeduction;
                        result.TotalSalary = totalSalary;
                        result.NetSalary = netSalary;

                        return Ok(await Result<object>.SuccessAsync(result));
                    }
                    else
                    {
                        return Ok(await Result<object>.FailAsync(localization.GetResource1("EmployeeNotFound")));

                    }
                }

                return Ok(item);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion


        #region AddPage Business

        [HttpPost("Add")]
        public async Task<ActionResult> Add(HrLoanDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(171, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (!ModelState.IsValid)
                    return Ok(await Result<string>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));



                var add = await hrServiceManager.HrLoanService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"====== Exp in HRLoanPaymentController  Add Method, MESSAGE: {ex.Message}"));
            }
        }




        [HttpPost("Add4")]
        public async Task<ActionResult> Add4(HrLoan4Dto obj)
        {
            try
            {
                var chk = await permission.HasPermission(171, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (!ModelState.IsValid)
                    return Ok(await Result<string>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                if (obj.Type <= 0)
                    return Ok(await Result<string>.FailAsync(localization.GetMessagesResource("TypeIsRequired")));

                if (string.IsNullOrEmpty(obj.EmpId))
                    return Ok(await Result<string>.FailAsync(localization.GetMessagesResource("EmployeeNameIsRequired")));

                if (string.IsNullOrEmpty(obj.LoanDate))
                    return Ok(await Result<string>.FailAsync(localization.GetMessagesResource("LoanDateIsRequired")));

                if (string.IsNullOrEmpty(obj.LoanValue.ToString()) || obj.LoanValue <= 0)
                    return Ok(await Result<string>.FailAsync(localization.GetMessagesResource("AmountIsRequired")));

                if (string.IsNullOrEmpty(obj.InstallmentCount.ToString()) || obj.InstallmentCount <= 0)
                    return Ok(await Result<string>.FailAsync(localization.GetMessagesResource("InstallmentCountIsRequired")));

                var add = await hrServiceManager.HrLoanService.Add4(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"====== Exp in HRLoanController  Add Method, MESSAGE: {ex.Message}"));
            }
        }

        #endregion

        #region EditPage

        [HttpDelete("DeleteInstallment")]
        public async Task<IActionResult> DeleteInstallment(long Id)
        {
            try
            {
                var chk = await permission.HasPermission(171, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (Id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await hrServiceManager.HrLoanInstallmentService.Remove(Id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete installment  HRLoanController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetLoanDataById")]
        public async Task<IActionResult> GetLoanDataById(long id)
        {
            var chk = await permission.HasPermission(171, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            var result = await hrServiceManager.HrLoanService.GetLoanWithRemainingAmount(id);
            var files = await mainServiceManager.SysFileService.GetAll(x => x.IsDeleted == false && x.PrimaryKey == id && x.TableId == 138);
            if (!result.Succeeded)
            {
                return Ok(await Result<HrLoanDto>.FailAsync(result.Status.message));
            }

            return Ok(await Result<object>.SuccessAsync(new { data = result.Data, fileDtos = files.Data }, ""));
        }

        [HttpGet("GetLoanInstallmentDataById")]
        public async Task<IActionResult> GetLoanInstallmentDataById(long id)
        {
            var hasPermission = await permission.HasPermission(171, PermissionType.Show);
            if (!hasPermission)
                return Ok(await Result.AccessDenied("AccessDenied"));

            // Get loan and installments
            var hrLoan = await hrServiceManager.HrLoanService.GetOneVW(x => x.Id == id && x.IsDeleted == false);
            if (hrLoan.Data == null)
                return Ok(await Result<object>.FailAsync(localization.GetMessagesResource("LoanNotFound")));

            var hrLoanIns = await hrServiceManager.HrLoanInstallmentService.GetAllVW(x => x.LoanId == id && x.IsDeleted == false);
            if (!hrLoanIns.Succeeded)
                return Ok(await Result<HrLoanInstallmentVw>.FailAsync(hrLoanIns.Status.message));

            var empData = await mainServiceManager.InvestEmployeeService.GetOne(x => x.EmpId == hrLoan.Data.EmpCode && x.IsDeleted == false);
            if (empData.Data == null)
                return Ok(await Result<object>.FailAsync(localization.GetMessagesResource("EmployeeNotFound")));

            var totalAllowance = await hrServiceManager.HrAllowanceDeductionService.GetTotalAllowances(empData.Data.Id ?? 0);
            var salary = (empData.Data.Salary + totalAllowance.Data) ?? 0;

            var resultList = new List<object>();

            foreach (var row in hrLoanIns.Data)
            {
                // Get sum of same month installments (excluding current loan id)
                var sameMonthInput = new HrLoanInstallmentNotLoanIdDto
                {
                    fromDate = row.DueDate,
                    empId = empData.Data.Id,
                    LoanId = row.LoanId
                };
                var sameMonthInstallments = await hrServiceManager.HrLoanService.GetSumInstallmentLoanNotLoanId(sameMonthInput);

                var installmentValue = row.Amount ?? 0;
                var installmentTotal = (sameMonthInstallments + installmentValue);

                double installmentRate = 0;
                if (salary > 0)
                    installmentRate = Math.Round((double)installmentTotal / (double)salary * 100, 2);

                resultList.Add(new
                {
                    Installment_ID = row.Id,
                    IsDeleted = 0,
                    Installment_No = row.IntallmentNo,
                    Installment_Value = installmentValue,
                    Installment_Date = row.DueDate,
                    IsPaid = row.IsPaid == true ? 1 : 0,
                    Recepit_No = row.RecepitNo,
                    Recepit_Date = row.RecepitDate,
                    Installment_Same_Month = sameMonthInstallments,
                    Installment_Total = installmentTotal,
                    Installment_Rate = installmentRate
                });
            }

            return Ok(await Result<object>.SuccessAsync(resultList));
        }
        /// <summary>
        /// تستخدم لتعديل تاريخ قسط فقط
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>

        [HttpPost("EditInstallment")]
        public async Task<ActionResult> EditInstallment(HrLoanInstallmentEditDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(171, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result<string>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                if (string.IsNullOrEmpty(obj.DueDate))
                    return Ok(await Result<string>.FailAsync(localization.GetMessagesResource("DueDateIsRequired")));
                var getLoanInstallment = await hrServiceManager.HrLoanInstallmentService.GetAll(e => e.IsDeleted == false && e.IsPaid == true && e.LoanId == obj.LoanId);
                if (DateHelper.StringToDate(obj.DueDate) < DateTime.Now)
                {
                    return Ok(await Result<string>.FailAsync(localization.GetMessagesResource("DueDateIsInvalid")));

                }
                if (getLoanInstallment.Data.Count() > 0)
                {
                    return Ok(await Result<string>.FailAsync(localization.GetMessagesResource("EditFailedPaidInstallments")));
                }
                var add = await hrServiceManager.HrLoanInstallmentService.Update(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"====== Exp in HRLoanPaymentController  Add Method, MESSAGE: {ex.Message}"));
            }
        }
        [HttpPost("EditLoan")]
        public async Task<ActionResult> EditLoan(HrEditLoanDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(171, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result<string>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));


                var add = await hrServiceManager.HrLoanService.Update(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"====== Exp in HRLoanPaymentController  Add Method, MESSAGE: {ex.Message}"));
            }
        }




        [HttpPost("ReSchedule")]
        public async Task<ActionResult> ReSchedule(InstallmentScheduleDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(171, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result<string>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                decimal Installment_V = 0;
                var property = await mainServiceManager.SysPropertyValueService.GetByProperty(55, session.FacilityId);
                if (!string.IsNullOrEmpty(property.Data.PropertyValue))
                {
                    Installment_V = Convert.ToDecimal(property.Data.PropertyValue);
                    if (Installment_V != 0)
                    {
                        return Ok(await Result<string>.FailAsync(localization.GetMessagesResource("RecorBelExcAllowPremPercentage"), 450));
                    }
                }

                if (string.IsNullOrEmpty(obj.SDatePyment))
                    return Ok(await Result<string>.FailAsync(localization.GetMessagesResource("FirstInstallmentDateIsRequired")));

                if (string.IsNullOrEmpty(obj.empCode.ToString()))
                    return Ok(await Result<string>.FailAsync(localization.GetMessagesResource("EmployeeNumberIsRequired")));
                if (string.IsNullOrEmpty(obj.Installmentcount.ToString()) || obj.Installmentcount <= 0)
                    return Ok(await Result<string>.FailAsync(localization.GetMessagesResource("InstallmentCountIsRequired")));

                var add = await hrServiceManager.HrLoanService.ReScheduleLoan(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"====== Exp in HRLoanController  ReSchedule Method, MESSAGE: {ex.Message}"));
            }
        }

        #endregion
        [HttpPost("GetInstallmentsByEmpCode")]
        public async Task<IActionResult> GetInstallmentsByEmpCode(HrLoanInstallmentInputDto obj)
        {
            var hasPermission = await permission.HasPermission(171, PermissionType.Show);
            if (!hasPermission)
                return Ok(await Result.AccessDenied("AccessDenied"));

            if (!DateTime.TryParseExact(obj.fromDate, "yyyy/MM/dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var startDate))
            {
                return Ok(await Result<string>.FailAsync("Invalid fromDate or toDate format. Expected format: yyyy/MM/dd"));
            }
            var empId = await mainServiceManager.InvestEmployeeService.GetOne(x => x.EmpId == obj.empCode && x.IsDeleted == false);

            var result = await hrServiceManager.HrLoanInstallmentService
                .GetAllVW(x => x.EmpId == empId.Data.Id.ToString()
                            && x.IsDeleted == false
                            && x.IsDeletedM == false
                            && x.IsPaid == false);
            if (!result.Succeeded)
                return Ok(await Result<HrLoanDto>.FailAsync(result.Status.message));
            string monthYear = startDate.ToString("MM/yyyy", CultureInfo.InvariantCulture);

            decimal x = obj.installmentcount ?? 0;
            int y = (int)Math.Floor(x);
            int NO = 1;
            decimal s = Convert.ToDecimal(obj.installmentcount) - y;
            var totalAllowance = await hrServiceManager.HrAllowanceDeductionService.GetTotalAllowances(empId.Data.Id ?? 0);
            var totalSalary = (empId.Data.Salary + totalAllowance.Data) ?? 0;
            var dueDate = startDate.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);

            List<HrLoanInstallmentSummaryDto> installmentList = new List<HrLoanInstallmentSummaryDto>();
            for (int i = 0; i < y; i++)
            {
                var currentDueDate = startDate.AddMonths(i);
                string dueDateStr = currentDueDate.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);
                string targetMonth = currentDueDate.ToString("MM/yyyy", CultureInfo.InvariantCulture);

                var getHrLoanInstallmentDto = new HrLoanInstallmentSummaryDto
                {
                    IntallmentNo = NO++,
                    DueDate = dueDateStr,
                    Amount = obj.installmentValue
                };

                getHrLoanInstallmentDto.SameMonthInstallments = result.Data
                    .Where(c =>
                        !string.IsNullOrWhiteSpace(c.DueDate) &&
                        DateTime.TryParseExact(c.DueDate, "yyyy/MM/dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDueDate) &&
                        parsedDueDate.ToString("MM/yyyy", CultureInfo.InvariantCulture) == targetMonth)
                    .Sum(c => c.Amount);

                var installmentTotal = (getHrLoanInstallmentDto.SameMonthInstallments + obj.installmentValue) ?? 0;

                if (totalSalary > 0)
                {
                    getHrLoanInstallmentDto.Rate = Math.Round(installmentTotal / totalSalary * 100, 2);
                }
                else
                {
                    return Ok(await Result<string>.FailAsync(localization.GetMessagesResource("DetermineEmpSalary")));
                }

                installmentList.Add(getHrLoanInstallmentDto);
            }

            if (s > 0)
            {
                var currentDueDate = startDate.AddMonths(y);
                string dueDateStr = currentDueDate.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);
                string targetMonth = currentDueDate.ToString("MM/yyyy", CultureInfo.InvariantCulture);
                var getHrLoanInstallmentDto = new HrLoanInstallmentSummaryDto();
                getHrLoanInstallmentDto.Amount = Math.Round((Convert.ToDecimal(obj.installmentValue) * s), 2);
                getHrLoanInstallmentDto.IntallmentNo = NO;
                getHrLoanInstallmentDto.DueDate = dueDateStr;

                getHrLoanInstallmentDto.SameMonthInstallments = result.Data
                    .Where(c =>
                        !string.IsNullOrWhiteSpace(c.DueDate) &&
                        DateTime.TryParseExact(c.DueDate, "yyyy/MM/dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDueDate) &&
                        parsedDueDate.ToString("MM/yyyy", CultureInfo.InvariantCulture) == targetMonth)
                    .Sum(c => c.Amount);
                var installmentTotal = (getHrLoanInstallmentDto.SameMonthInstallments + getHrLoanInstallmentDto.Amount) ?? 0;
                if (totalSalary > 0)
                {
                    getHrLoanInstallmentDto.Rate = Math.Round(installmentTotal / totalSalary * 100, 2);
                }
                else
                {
                    getHrLoanInstallmentDto.Rate = 0;

                    return Ok(await Result<string>.FailAsync(localization.GetMessagesResource("DetermineEmpSalary")));

                }

                installmentList.Add(getHrLoanInstallmentDto);

            }

            return Ok(await Result<object>.SuccessAsync(installmentList));
        }

        [HttpPost("GetEmpMonthlyInstallmentRate")]
        public async Task<IActionResult> GetEmpMonthlyInstallmentRate(HrLoanInstallmentInput2Dto obj)
        {
            var hasPermission = await permission.HasPermission(171, PermissionType.Show);
            if (!hasPermission)
                return Ok(await Result.AccessDenied("AccessDenied"));

            if (!DateTime.TryParseExact(obj.fromDate, "yyyy/MM/dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var startDate))
            {
                return Ok(await Result<string>.FailAsync("Invalid fromDate or toDate format. Expected format: yyyy/MM/dd"));
            }
            var empId = await mainServiceManager.InvestEmployeeService.GetOne(x => x.EmpId == obj.empCode && x.IsDeleted == false);

            var result = await hrServiceManager.HrLoanInstallmentService
                .GetAllVW(x => x.EmpId == empId.Data.Id.ToString()
                            && x.IsDeleted == false
                            && x.IsDeletedM == false
                            && x.IsPaid == false);
            if (!result.Succeeded)
                return Ok(await Result<HrLoanDto>.FailAsync(result.Status.message));
            string monthYear = startDate.ToString("MM/yyyy", CultureInfo.InvariantCulture);

            int NO = 1;
            var totalAllowance = await hrServiceManager.HrAllowanceDeductionService.GetTotalAllowances(empId.Data.Id ?? 0);
            var totalSalary = (empId.Data.Salary + totalAllowance.Data) ?? 0;
            var dueDate = startDate.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);

            List<HrLoanInstallmentSummaryDto> installmentList = new List<HrLoanInstallmentSummaryDto>();
            string dueDateStr = startDate.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);
            string targetMonth = startDate.ToString("MM/yyyy", CultureInfo.InvariantCulture);

            var getHrLoanInstallmentDto = new HrLoanInstallmentSummaryDto
            {
                IntallmentNo = NO++,
                DueDate = dueDateStr,
                Amount = obj.installmentValue
            };

            getHrLoanInstallmentDto.SameMonthInstallments = result.Data
                .Where(c =>
                    !string.IsNullOrWhiteSpace(c.DueDate) &&
                    DateTime.TryParseExact(c.DueDate, "yyyy/MM/dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDueDate) &&
                    parsedDueDate.ToString("MM/yyyy", CultureInfo.InvariantCulture) == targetMonth)
                .Sum(c => c.Amount);

            var installmentTotal = (getHrLoanInstallmentDto.SameMonthInstallments + obj.installmentValue) ?? 0;

            if (totalSalary > 0)
            {
                getHrLoanInstallmentDto.Rate = Math.Round(installmentTotal / totalSalary * 100, 2);
            }
            else
            {
                return Ok(await Result<string>.FailAsync(localization.GetMessagesResource("DetermineEmpSalary")));
            }

            installmentList.Add(getHrLoanInstallmentDto);

            return Ok(await Result<object>.SuccessAsync(installmentList));
        }

    }
}