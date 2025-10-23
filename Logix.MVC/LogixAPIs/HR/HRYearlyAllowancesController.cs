using DevExpress.Pdf.Xmp;
using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.DTOs.Main;
using Logix.Application.Helpers;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Services;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.HR.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualBasic;
using System.Reflection.Emit;
using ZXing;
using Result = Logix.Application.Wrapper.Result;

namespace Logix.MVC.LogixAPIs.HR
{

    //   بدلات  سنوية
    public class HRYearlyAllowancesController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IApiDDLHelper ddlHelper;


        public HRYearlyAllowancesController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IApiDDLHelper ddlHelper)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.ddlHelper = ddlHelper;
        }

        [HttpPost("GetPagination")]
        public async Task<IActionResult> GetPagination([FromBody] HrAllowanceDeductionFilterDto filter, int take = Pagination.take, long? lastSeenId = null)
        {
            var chk = await permission.HasPermission(575, PermissionType.Show);
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
                    DatePropertyName = "DueDate",
                    ComparisonOperator = ComparisonOperator.GreaterThanOrEqual,
                    StartDateString = filter.FromDate ?? ""
                });
                dateConditions.Add(new DateCondition
                {
                    DatePropertyName = "DueDate",
                    ComparisonOperator = ComparisonOperator.LessThanOrEqual,
                    StartDateString = filter.ToDate ?? ""
                });
                var items = await hrServiceManager.HrAllowanceDeductionService.GetAllWithPaginationVW(selector: e => e.Id,
                expression: e =>
                        e.IsDeleted == false && e.Status == true && e.FixedOrTemporary > 2 &&
                        (string.IsNullOrEmpty(filter.EmpCode) || (!string.IsNullOrEmpty(e.EmpCode) && Convert.ToInt64(e.EmpCode) == Convert.ToInt64(filter.EmpCode))) &&
                        (string.IsNullOrEmpty(filter.EmpName) || e.EmpName.ToLower().Contains(filter.EmpName.ToLower()) || e.EmpName2.ToLower().Contains(filter.EmpName.ToLower())),
                    take: take,
                    lastSeenId: lastSeenId,
                   dateConditions: (string.IsNullOrEmpty(filter.FromDate) || string.IsNullOrEmpty(filter.ToDate)) ? null : dateConditions);

                if (!items.Succeeded)
                    return Ok(await Result<List<HrLeaveVw>>.FailAsync(items.Status.message));
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
        [HttpPost("Add")]
        public async Task<ActionResult> Add(HrAllowanceDeductionDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(575, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid || obj.FixedOrTemporary <= 0)
                    return Ok(await Result<HrAllowanceDeductionDto>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));
                if (string.IsNullOrEmpty(obj.ContractDate))
                    return Ok(await Result<HrAllowanceDeductionDto>.FailAsync($"تاريخ العقد مطلوب"));
                if (obj.Amount <= 0)
                    return Ok(await Result<HrAllowanceDeductionDto>.FailAsync($"يجب ادخال المبلغ"));



                var add = await hrServiceManager.HrAllowanceDeductionService.AddYearlyAllowanceDeduction(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add HRYearlySalaryController, MESSAGE: {ex.Message}"));
            }
        }


        [HttpGet("EmpIdChanged")]
        public async Task<IActionResult> EmpIdChanged(string EmpId)
        {
            try
            {
                var chk = await permission.HasPermission(575, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (string.IsNullOrEmpty(EmpId))
                {
                    return Ok(await Result<HrAllowanceDeductionAutoDataDto>.SuccessAsync(""));
                }

                decimal TotalAllowance = 0;
                decimal TotalDeduction = 0;
                var getEmployees = await hrServiceManager.HrEmployeeService.GetOneVW(e => e.EmpId == EmpId);
                if (getEmployees.Succeeded)
                {
                    if (getEmployees.Data != null)
                    {
                        var getTotalAllowance = await hrServiceManager.HrAllowanceDeductionService.GetTotalAllowances(getEmployees.Data.Id);
                        if (getTotalAllowance.Succeeded) TotalAllowance = getTotalAllowance.Data;
                        var getTotalDeduction = await hrServiceManager.HrAllowanceDeductionService.GetTotalDeduction(getEmployees.Data.Id);
                        if (getTotalDeduction.Succeeded) TotalDeduction = getTotalDeduction.Data;

                        var result = new HrAllowanceDeductionAutoDataDto
                        {

                            allowanceAmount = TotalAllowance,
                            deductionAmount = TotalDeduction,
                            salary = getEmployees.Data.Salary,
                            totalSalary = getEmployees.Data.Salary + (TotalAllowance - TotalDeduction),
                            EmpName = getEmployees.Data.EmpName,
                            EmpId = EmpId


                        };
                        return Ok(await Result<HrAllowanceDeductionAutoDataDto>.SuccessAsync(result, ""));

                    }
                    else
                    {
                        return Ok(await Result<HrAllowanceDeductionAutoDataDto>.SuccessAsync(""));

                    }

                }
                return Ok(await Result<HrAllowanceDeductionAutoDataDto>.FailAsync(getEmployees.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrAllowanceDeductionAutoDataDto>.FailAsync($"====== Exp in HRNoteController getById, MESSAGE: {ex.Message}"));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var chk = await permission.HasPermission(575, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await hrServiceManager.HrAllowanceDeductionService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete HR Yearly Allowance DeductionController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetRadioButtons")]
        public async Task<IActionResult> GetRadioButtons()
        {
            try
            {
                List<DDLItem<int>> resultList = new List<DDLItem<int>>();
                var chk = await permission.HasPermission(575, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var result = new DDLItem<int>()
                {
                    Name = localization.GetHrResource("Allownce"),
                    Value = 20
                };
                resultList.Add(result);
                var result2 = new DDLItem<int>()
                {
                    Name = localization.GetHrResource("Deduction"),
                    Value = 21
                };
                resultList.Add(result2);

                return Ok(await Result<List<DDLItem<int>>>.SuccessAsync(resultList, ""));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Allowance DeductionController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("DDLAllowanceDeductionTempOrFix")]
        public async Task<IActionResult> DDLAllowanceDeductionTempOrFix()
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<HrAllowanceDeductionTempOrFix, int>(d => d.Id > 2, "Id", "TypeName");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
    }
}