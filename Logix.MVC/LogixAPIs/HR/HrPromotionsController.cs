using DocumentFormat.OpenXml.Spreadsheet;
using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.HR.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Logix.MVC.LogixAPIs.HR
{
    //  الترقيات
    public class HrPromotionsController : BaseHrApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IApiDDLHelper ddlHelper;


        public HrPromotionsController(IHrServiceManager hrServiceManager, IMainServiceManager mainServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IApiDDLHelper ddlHelper)
        {
            this.mainServiceManager = mainServiceManager;
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.ddlHelper = ddlHelper;
        }

        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrPromotionsFilterDto filter)
        {
            var hasPermission = await permission.HasPermission(2283, PermissionType.Show);
            if (!hasPermission)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var userBranches = session.Branches.Split(',');

                var items = await hrServiceManager.HrIncrementService.GetAllVW(e =>
                    e.IsDeleted == false && e.TransTypeId == 2);

                if (!items.Succeeded)
                    return Ok(await Result<object>.FailAsync(items.Status.message));

                var res = items.Data.AsQueryable();

                if (filter.BranchId != null && filter.BranchId > 0)
                {
                    res = res.Where(r => r.BranchId == filter.BranchId);
                }
                else
                {
                    res = res.Where(r => r.BranchId != null && userBranches.Contains(r.BranchId.ToString()));
                }

                if (!string.IsNullOrEmpty(filter.EmpName))
                {
                    res = res.Where(r => r.EmpName != null && r.EmpName.Contains(filter.EmpName));
                }

                if (!string.IsNullOrEmpty(filter.EmpId))
                {
                    res = res.Where(r => r.EmpCode != null && r.EmpCode == filter.EmpId);
                }

                if (!string.IsNullOrEmpty(filter.FromDate) && !string.IsNullOrEmpty(filter.ToDate))
                {
                    DateTime fromDate = DateHelper.StringToDate(filter.FromDate);
                    DateTime toDate = DateHelper.StringToDate(filter.ToDate);

                    res = res.Where(r => r.StartDate != null && DateHelper.StringToDate(r.StartDate) >= fromDate && DateHelper.StringToDate(r.StartDate) <= toDate);
                }

                if (filter.LocationId != null && filter.LocationId > 0)
                {
                    res = res.Where(r => r.Location == filter.LocationId);
                }

                if (filter.DeptId != null && filter.DeptId > 0)
                {
                    res = res.Where(r => r.DeptId == filter.DeptId);
                }

                var resultList = res
                    .OrderByDescending(r => r.StartDate)
                    .Select(item => new
                    {
                        item.Id,
                        EmpName = session.Language == 1 ? item.EmpName : item.EmpName2,
                        item.EmpCode,
                        DepName = session.Language == 1 ? item.DepName : item.DepName2,
                        LocationName = session.Language == 1 ? item.LocationName : item.LocationName2,
                        BraName = session.Language == 1 ? item.BraName : item.BraName2,
                        item.StartDate,
                        item.LevelName,
                        item.NewLevelName,
                        item.GradeName,
                        item.NewGradeName,
                        CurrentSalary = item.Salary + item.Allowances - item.Deductions,
                        NewSalary = item.NewSalary + item.NewAllowance - item.NewDeduction,
                        Difference = (item.NewSalary + item.NewAllowance - item.NewDeduction) - (item.Salary + item.Allowances - item.Deductions),
                    }).ToList();

                if (resultList.Any())
                    return Ok(await Result<List<object>>.SuccessAsync(resultList.Cast<object>().ToList(), ""));
                //return Ok(await Result<List<object>>.SuccessAsync(resultList, ""));
                else
                    return Ok(await Result<List<object>>.SuccessAsync(resultList.Cast<object>().ToList(), localization.GetResource1("NosearchResult")));
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync(ex.Message));
            }
        }

        [HttpPost("Add")]
        public async Task<ActionResult> Add(HrIncrementDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(2283, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result<HrIncrementDto>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                if (string.IsNullOrEmpty(obj.EmpId.ToString()))
                    return Ok(await Result<HrIncrementDto>.FailAsync($"{localization.GetResource1("EmployeeNotFound")}"));

                //var emp = await mainServiceManager.InvestEmployeeService.GetOne(x=>x.IsDeleted ==  false && x.EmpId == obj.EmpId.ToString());
                //if (emp == null)
                //    return Ok(await Result<HrIncrementDto>.FailAsync($"{localization.GetResource1("EmployeeNotFound")}"));

                var add = await hrServiceManager.HrIncrementService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add HrIncrementController, MESSAGE: {ex.Message}"));
            }
        }


        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(HrIncrementEditDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(2283, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result<HrIncrementEditDto>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var update = await hrServiceManager.HrIncrementService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrIncrementEditDto>.FailAsync($"====== Exp in Edit HrIncrementController, MESSAGE: {ex.Message}"));
            }
        }


        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var chk = await permission.HasPermission(2283, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await hrServiceManager.HrIncrementService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete HR IncrementController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetByIdForEdit")]
        public async Task<IActionResult> GetByIdForEdit(long id)
        {
            try
            {
                var chk = await permission.HasPermission(2283, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync(localization.GetMessagesResource("NoIdInUpdate")));

                var increment = await hrServiceManager.HrIncrementService.GetOneVW(x => x.Id == id && x.IsDeleted == false);
                if (!increment.Succeeded || increment.Data == null)
                    return Ok(await Result.FailAsync(increment.Status.message));

                var empCheck = await hrServiceManager.HrEmployeeService.CHeckEmpInBranch(increment.Data.EmpId);
                if (!empCheck.Succeeded || empCheck.Data == false)
                    return Ok(await Result.FailAsync(localization.GetHrResource("UserNotHasPermissionToEmp")));

                var allowancesDeductions = await hrServiceManager.HrIncrementsAllowanceDeductionService
                    .GetAllVW(x => x.IsDeleted == false && x.IncrementId == id);

                var allowances = allowancesDeductions.Data.Where(x => x.TypeId == 1).ToList();
                var deductions = allowancesDeductions.Data.Where(x => x.TypeId == 2).ToList();

                var curGrad = await hrServiceManager.HrJobGradeService.GetOne(g => g.Id == increment.Data.CurGradId);
                var job = await hrServiceManager.HrJobService.GetOne(j => j.Id == increment.Data.NewJobId);

                string message  = "تم تحديث العلاوة في راتب الموظف ";
                string empName = increment.Data.EmpName; // default

                if (session != null && session.Language == 2)
                {
                    empName = increment.Data.EmpName2;
                }

                var response = new Result<object>
                {
                    Data = new 
                    {
                        Id = increment.Data.Id,
                        GradeName = increment.Data?.GradeName,
                        IncreaseAmount = increment.Data?.IncreaseAmount,
                        IncreaseDate = increment.Data?.IncreaseDate,
                        LevelName = increment.Data?.LevelName,
                        Location = increment.Data?.Location,
                        LocationName = increment.Data?.LocationName,
                        LocationName2 = increment.Data?.LocationName2,
                        NationalityName = increment.Data?.NationalityName,
                        NewAllowance = increment.Data.NewAllowance,
                        NewCatJobId = increment.Data?.NewCatJobId,
                        NewDeduction = increment.Data.NewDeduction,
                        NewGradeName = increment.Data?.NewGradeName,
                        NewGradId = increment.Data?.NewGradId,
                        NewJobId = increment.Data?.NewJobId,
                        NewLevelId = increment.Data?.NewLevelId,
                        NewLevelName = increment.Data?.NewLevelName,
                        NewSalary = increment.Data?.NewSalary,
                        Note = increment.Data?.Note,
                        StartDate = increment.Data?.StartDate,
                        StatusId = increment.Data?.StatusId,
                        DeptId = increment.Data?.DeptId,
                        Doappointment = increment.Data?.Doappointment,
                        EmpCode = increment.Data?.EmpCode,
                        BranchId = increment.Data?.BranchId,
                        CurCatJobId = increment.Data?.CurCatJobId,
                        Allowances = increment.Data.Allowances,
                        Deductions = increment.Data.Deductions,
                        AppId = increment.Data.AppId,
                        ApplyType = increment.Data.ApplyType,
                        BraName = increment.Data.BraName,
                        BraName2 = increment.Data.BraName2,
                        CatName = increment.Data.CatName,
                        CurGradId = increment.Data.CurGradId,
                        CurJobId = increment.Data.CurJobId,
                        CurLevelId = increment.Data.CurLevelId,
                        DecisionDate = increment.Data?.DecisionDate,
                        DecisionNo = increment.Data?.DecisionNo,
                        DepName = increment.Data?.DepName,
                        DepName2 = increment.Data?.DepName2,
                        EmpId = increment.Data.EmpId,
                        EmpName2 = increment.Data.EmpName2,
                        EmpName = empName,
                        Salary = increment.Data.Salary,
                        TransTypeId = increment.Data.TransTypeId,
                        TransTypeName = increment.Data.TransTypeName,
                        TransTypeName2 = increment.Data.TransTypeName2,
                        Message = message,
                    },
                    Status = new Message {
                        code = 200,
                        message = ""
                    },
                    Succeeded = true
                };


                return Ok(response);
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"Exp in GetById: {ex.Message}"));
            }
        }

        [HttpGet("getEmpDataByEmpCode")]
        public async Task<IActionResult> getEmpDataByEmpCode(long empCode)
        {
            var chk = await permission.HasPermission(2283, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (string.IsNullOrEmpty(empCode.ToString()) || empCode.ToString() == "" || empCode == 0)
            {
                return Ok(await Result.FailAsync("You do not Enter Any Value"));
            }

            try
            {
                decimal TotalAllowance = 0;
                decimal TotalDeduction = 0;

                var getEmployeeData = await hrServiceManager.HrEmployeeService.GetOneVW(e => e.EmpId == empCode.ToString() && e.IsDeleted == false && e.Isdel == false);


                if (getEmployeeData.Succeeded)
                {
                    var getTotalAllowance = await hrServiceManager.HrAllowanceDeductionService.GetTotalAllowances(getEmployeeData.Data.Id);
                    if (getTotalAllowance.Succeeded) TotalAllowance = getTotalAllowance.Data;

                    var getTotalDeduction = await hrServiceManager.HrAllowanceDeductionService.GetTotalDeduction(getEmployeeData.Data.Id);
                    if (getTotalDeduction.Succeeded) TotalDeduction = getTotalDeduction.Data;

                    var gradResult = await hrServiceManager.HrJobGradeService.GetOne(e => e.GradeName == getEmployeeData.Data.GradeName);
                    int curGradId = (gradResult.Succeeded && gradResult.Data != null) ? (int) gradResult.Data.Id : 0;

                    var result = new
                    {
                        empCode = empCode,
                        EmpName = session.Language == 1 ? getEmployeeData.Data.EmpName : getEmployeeData.Data.EmpName2,
                        Allowances = TotalAllowance,
                        Deductions = TotalDeduction,
                        CurLevelId = getEmployeeData.Data.LevelId,
                        CurGradName = getEmployeeData.Data.GradeName,
                        CurGradId = curGradId,
                        CurCatJobId = getEmployeeData.Data.JobCatagoriesId,
                        CurJobId = getEmployeeData.Data.JobId,
                        //NetSalary = getEmployeeData.Data.Salary + TotalAllowance - TotalDeduction,
                        Salary = getEmployeeData.Data.Salary
                    };

                    return Ok(await Result<object>.SuccessAsync(result, " "));
                }
                return Ok(await Result.FailAsync(getEmployeeData.Status.message));
            }

            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

    }
}
