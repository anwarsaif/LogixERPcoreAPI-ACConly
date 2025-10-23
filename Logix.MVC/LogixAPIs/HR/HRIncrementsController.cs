using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Helpers;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Services.HR;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.Domain.WF;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.HR.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Logix.MVC.LogixAPIs.HR
{

    //  العلاوات / الزيادات
    public class HRIncrementsController : BaseHrApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IWFServiceManager wFServiceManager;

        public HRIncrementsController(IHrServiceManager hrServiceManager, IMainServiceManager mainServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IWFServiceManager wFServiceManager)
        {
            this.mainServiceManager = mainServiceManager;
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.wFServiceManager = wFServiceManager;
        }

        #region Main Page


        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrIncrementFilterDto filter)
        {
            var chk = await permission.HasPermission(273, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var items = await hrServiceManager.HrIncrementService.Search(filter);
				return Ok(items);

			}
            catch (Exception ex)
            {
                return Ok(await Result<HrIncrementsVw>.FailAsync(ex.Message));
            }
        }

        [HttpPost("GetPagination")]
        public async Task<IActionResult> GetPagination([FromBody] HrIncrementFilterDto filter, int take = Pagination.take, long? lastSeenId = null)
        {
            var chk = await permission.HasPermission(273, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {

                var dateConditions = new List<DateCondition>();
                dateConditions.Add(new DateCondition
                {
                    DatePropertyName = "StartDate",
                    ComparisonOperator = ComparisonOperator.GreaterThanOrEqual,
                    StartDateString = filter.From ?? ""
                });
                dateConditions.Add(new DateCondition
                {
                    DatePropertyName = "StartDate",
                    ComparisonOperator = ComparisonOperator.LessThanOrEqual,
                    StartDateString = filter.To ?? ""
                });

                var BranchesList = session.Branches.Split(',');
                filter.BranchId ??= 0;
                filter.DeptId ??= 0;
                filter.Location ??= 0;
                filter.TransactionType ??= 0;

                var items = await hrServiceManager.HrIncrementService.GetAllWithPaginationVW(selector: e => e.Id,
                expression: e => e.IsDeleted == false 
                && (filter.BranchId != 0 ? e.BranchId == filter.BranchId : BranchesList.Contains(e.BranchId.ToString()))
                && (filter.DeptId == 0 || e.DeptId == filter.DeptId)
                && (filter.Location == 0 || e.Location == filter.Location)
                && (filter.TransactionType == 0 || e.TransTypeId == filter.TransactionType)
                && (string.IsNullOrEmpty(filter.EmpName) || e.EmpName == filter.EmpName)
                && (string.IsNullOrEmpty(filter.EmpCode) || e.EmpCode == filter.EmpCode),
                    take: take,
                    lastSeenId: lastSeenId,
                   dateConditions: (string.IsNullOrEmpty(filter.From) || string.IsNullOrEmpty(filter.To)) ? null : dateConditions);

                if (!items.Succeeded)
                    return Ok(await Result<List<HrIncrementsVw>>.FailAsync(items.Status.message));

                if (items.Data.Count() > 0)
                {
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
        public async Task<IActionResult> Delete(long Id = 0)
        {
            try
            {
                var chk = await permission.HasPermission(273, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await hrServiceManager.HrIncrementService.Remove(Id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete HrIncrementsController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long Id = 0)
        {
            var chk = await permission.HasPermission(273, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result<object>.FailAsync($"Access Denied"));
            }
            if (Id <= 0)
            {
                return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
            }
            try
            {
                var Message = string.Empty;
                var BtnApplyNowVisibile = true;

                var result = await hrServiceManager.HrIncrementService.GetOneVW(I => I.IsDeleted == false && I.Id == Id);
                if (result.Data != null)
                {
                    var CHeckEmpInBranch = await hrServiceManager.HrEmployeeService.CHeckEmpInBranch(result.Data.EmpId);
                    if (CHeckEmpInBranch.Succeeded && CHeckEmpInBranch.Data == false)
                    {
                        return Ok(await Result<object>.FailAsync(CHeckEmpInBranch.Status.message));

                    }
                    if (result.Data.ApplyType == 1)
                    {
                        Message = localization.GetMessagesResource("IncrementUpdatedInEmployeeSalary");
                        BtnApplyNowVisibile = false;
                    }
                    
                    //var allIncrements = await hrServiceManager.HrIncrementsAllowanceDeductionService.GetAllVW(x => x.IsDeleted == false&&x.IncrementId==Id);
                    var IncrementAllowance = await hrServiceManager.HrIncrementsAllowanceVwService.GetAll(x => x.IncrementId == Id && x.IsDeleted == false);
                    var IncrementDeduction = await hrServiceManager.HrIncrementsDeductionVwService.GetAll(x => x.IncrementId == Id && x.IsDeleted == false);

                    return Ok(await Result<object>.SuccessAsync(new { Data = result.Data, allowance = IncrementAllowance, deduction = IncrementDeduction, Message = Message, BtnApplyNowVisibile = BtnApplyNowVisibile }));

                }
                return Ok(await Result<object>.FailAsync(result.Status.message));


            }
            catch (Exception exp)
            {
                return Ok(await Result<object>.FailAsync($"EXP in {this.GetType()}, Meesage: {exp.Message}"));
            }
        }


        [HttpGet("GetSalaryByHrJobGradeId")]
        public async Task<IActionResult> GetSalaryByHrJobGradeId(long JobGradeId = 0)
        {
            var chk = await permission.HasPermission(273, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result<object>.FailAsync($"Access Denied"));
            }
            if (JobGradeId <= 0)
            {
                return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
            }
            try
            {
                var result = await hrServiceManager.HrJobGradeService.GetOne(I => I.Id == JobGradeId);
                if (result.Data != null)
                {
                    return Ok(await Result<object>.SuccessAsync(result.Data.Salary));

                }
                return Ok(await Result<object>.FailAsync(result.Status.message));


            }
            catch (Exception exp)
            {
                return Ok(await Result<object>.FailAsync($"EXP in {this.GetType()}, Meesage: {exp.Message}"));
            }
        }


        #endregion


        #region AddPage

        [HttpPost("Add")]
        public async Task<IActionResult> Add(HrIncrementsAddDto obj)
        {
            var chk = await permission.HasPermission(273, PermissionType.Add);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (!ModelState.IsValid)
            {
                return Ok(obj);
            }
            try
            {
                if (obj.TransTypeId <= 0)
                {
                    return Ok(await Result<object>.FailAsync($"ادخل نوع العملية "));

                }
                if (obj.TransTypeId == 2)
                {
                    if (obj.NewJobId <= 0)
                    {
                        return Ok(await Result<object>.FailAsync($"ادخل كود الوظيفة الجديدة"));
                    }
                }
                if (string.IsNullOrEmpty(obj.EmpCode))
                {
                    return Ok(await Result<object>.FailAsync($"ادخل كود الموظف "));
                }
                if (obj.ApplyType <= 0)
                {
                    return Ok(await Result<object>.FailAsync($"ادخل آلية تطبيق الزيادة "));
                }
                if (obj.IncreaseAmount <= 0)
                {
                    return Ok(await Result<object>.FailAsync($"ادخل مقدار الزيادة "));
                }
                if (obj.allowancesList.Count() > 0)
                {
                    if (obj.allowancesList.Any(x => x.AdId <= 0)) return Ok(await Result<object>.FailAsync($"يجب تحديد النوع لجميع البدلات"));
                    if (obj.allowancesList.Any(x => x.Amount < 0)) return Ok(await Result<object>.FailAsync($"يجب تحديد المبلغ لجميع البدلات"));
                    if (obj.allowancesList.Any(x => x.NewAmount < 0)) return Ok(await Result<object>.FailAsync($"يجب تحديد المبلغ الجديد لجميع البدلات"));
                }
                if (obj.deductionsList.Count() > 0)
                {
                    if (obj.deductionsList.Any(x => x.AdId <= 0)) return Ok(await Result<object>.FailAsync($"يجب تحديد النوع لجميع الحسميات"));
                    if (obj.deductionsList.Any(x => x.Amount < 0)) return Ok(await Result<object>.FailAsync($"يجب تحديد المبلغ لجميع الحسميات"));
                    if (obj.deductionsList.Any(x => x.NewAmount < 0)) return Ok(await Result<object>.FailAsync($"يجب تحديد المبلغ الجديد الحسميات"));
                }
                var addRes = await hrServiceManager.HrIncrementService.Add(obj);
                return Ok(addRes);
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync(ex.Message));
            }
        }


        [HttpGet("EmpCodeChanged")]
        public async Task<IActionResult> EmpCodeChanged(string empCode)
        {
            try
            {
                var chk = await permission.HasPermission(273, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (string.IsNullOrEmpty(empCode))
                    return Ok(await Result.FailAsync($"Data Required"));

                var result = new EmpAutoDataVM();
                List<HrAllowanceDeductionVw> allowances = new List<HrAllowanceDeductionVw>();
                List<HrAllowanceDeductionVw> deductions = new List<HrAllowanceDeductionVw>();
                decimal TotalAllowance = 0;
                decimal TotalDeduction = 0;
                var getEmployeeData = await hrServiceManager.HrEmployeeService.GetOneVW(e => e.EmpId == empCode);
                if (getEmployeeData != null && getEmployeeData.Data != null)
                {
                    var CHeckEmpInBranch = await hrServiceManager.HrEmployeeService.CHeckEmpInBranch(getEmployeeData.Data.Id);
                    if (CHeckEmpInBranch.Succeeded && CHeckEmpInBranch.Data == false)
                    {
                        return Ok(await Result<object>.FailAsync(CHeckEmpInBranch.Status.message));

                    }
                    result.EmpName = getEmployeeData.Data.EmpName;
                    result.Salary = getEmployeeData.Data.Salary;
                    if (getEmployeeData.Data.LevelId == null) result.LevelId = 0; else result.LevelId = getEmployeeData.Data.LevelId;
                    if (getEmployeeData.Data.DegreeId == null) result.DegreeId = 0; else result.DegreeId = getEmployeeData.Data.DegreeId;
                    if (getEmployeeData.Data.JobCatagoriesId == null) result.JobCatagoriesId = 0; else result.JobCatagoriesId = getEmployeeData.Data.JobCatagoriesId;
                    if (getEmployeeData.Data.JobId == null) result.JobId = 0; else result.JobId = getEmployeeData.Data.JobId;
                    ;
                    var getTotalAllowance = await hrServiceManager.HrAllowanceDeductionService.GetAllVW(e => e.EmpId == getEmployeeData.Data.Id && e.IsDeleted == false && e.TypeId == 1 && e.FixedOrTemporary == 1);
                    if (getTotalAllowance.Succeeded)
                    {
                        foreach (var item in getTotalAllowance.Data)
                        {

                            allowances.Add(item);
                            TotalAllowance += (item.Amount != null ? item.Amount.Value : 0);
                        }
                    }
                    result.SalryAllownce = TotalAllowance;
                    result.allowances = allowances;
                    var getTotalDeduction = await hrServiceManager.HrAllowanceDeductionService.GetAllVW(e => e.EmpId == getEmployeeData.Data.Id && e.IsDeleted == false && e.TypeId == 2 && e.FixedOrTemporary == 1);
                    if (getTotalDeduction.Succeeded)
                    {
                        foreach (var item in getTotalDeduction.Data)
                        {
                            deductions.Add(item);

                            TotalDeduction += (item.Amount != null ? item.Amount.Value : 0);
                        }
                    }
                    result.SalryDeduction = TotalDeduction;
                    result.deduction = deductions;
                    result.NetSalary = getEmployeeData.Data.Salary + TotalAllowance - TotalDeduction;
                    return Ok(await Result<EmpAutoDataVM>.SuccessAsync(result, ""));

                }
                else
                {
                    return Ok(await Result<object>.FailAsync(localization.GetResource1("EmployeeNotFound")));

                }
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in EmpCodeChanged HrRequestController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("DeleteAllowanceOrDeduction")]
        public async Task<IActionResult> DeleteAllowanceOrDeduction(long Id = 0)
        {
            var chk = await permission.HasPermission(273, PermissionType.Add);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            if (Id <= 0)
                return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

            try
            {
                var del = await hrServiceManager.HrAllowanceDeductionService.Remove(Id);
                return Ok(del);

            }
            catch (Exception exp)
            {
                return Ok(await Result<object>.FailAsync($"{exp.Message}"));
            }
        }

        #endregion


        #region EditPage

        [HttpPost("ApplyIncrement")]
        public async Task<IActionResult> ApplyIncrement(long IncrementId, int TransTypeID)
        {
            var chk = await permission.HasPermission(273, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                if (IncrementId <= 0) return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                if (TransTypeID <= 0) return Ok(await Result.FailAsync($"يجب اختيار نوع العملية "));

                var addRes = await hrServiceManager.HrIncrementService.ApplyIncrement(IncrementId, TransTypeID);
                return Ok(addRes);
            }

            catch (Exception ex)
            {
                return Ok(await Result<HrCompetenceEditDto>.FailAsync($"{ex.Message}"));
            }
        }
        #endregion


        #region Increments_ByGrade  (العلاوة السنوية حسب الدرجة الوظيفية)




        [HttpPost("IncrementsByGradeSearch")]
        public async Task<IActionResult> IncrementsByGradeSearch(IncrementsBothFilterDto filter)
        {
            var chk = await permission.HasPermission(273, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if(string.IsNullOrEmpty(filter.ToDate))
                    return Ok(await Result<object>.FailAsync("يجب ادخال الحقل الى تاريخ"));

                if (filter.BranchId <= 0)
                {
                    filter.BranchId = 0;
                    filter.Branches = session.Branches;
                }
                else
                {
                    filter.Branches = null;
                }
                filter.CMDTYPE =1;
                filter.AnnaulIncreaseMethod = 1;
                filter.Location ??= 0;
                filter.Nationality ??= 0;
                var BranchesList = session.Branches.Split(',');
                var result = await hrServiceManager.HrIncrementService.IncrementsEvaluationsSearch(filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrIncrementsVw>.FailAsync(ex.Message));
            }
        }


        [HttpPost("MakeApproveByGrade")]
        public async Task<IActionResult> MakeApproveByGrade(List<MakeApproveDto> objects)
        {
            var chk = await permission.HasPermission(273, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result<object>.FailAsync($"Access Denied"));
            }
            if (objects.Count <= 0)
            {
                return Ok(await Result.FailAsync($"يجب تحديد صف واحد على الأقل"));
            }
            try
            {
                var result = await hrServiceManager.HrIncrementService.MakeApprove(objects);
                return Ok(result);
            }
            catch (Exception exp)
            {
                return Ok(await Result<object>.FailAsync($"EXP in {this.GetType()}, Meesage: {exp.Message}"));
            }
        }


        #endregion

        #region Increments_ByEvaluation  (العلاوة السنوية حسب التقييم)


        [HttpPost("IncrementsByEvaluationSearch")]
        public async Task<IActionResult> IncrementsByEvaluationSearch(IncrementsBothFilterDto filter)
        {
            var chk = await permission.HasPermission(273, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (string.IsNullOrEmpty(filter.ToDate))
                    return Ok(await Result<object>.FailAsync("يجب ادخال الحقل الى تاريخ"));
                if (string.IsNullOrEmpty(filter.EvaluationFromDate))
                    return Ok(await Result<object>.FailAsync("يجب ادخال الحقل تاريخ التقييم من "));
                if (string.IsNullOrEmpty(filter.EvaluationToDate))
                    return Ok(await Result<object>.FailAsync("يجب ادخال الحقل تاريخ التقييم الى "));

                if (filter.BranchId <= 0)
                {
                    filter.BranchId = 0;
                    filter.Branches = session.Branches;
                }
                else
                {
                    filter.Branches = null;
                }
                filter.CMDTYPE = 1;
                filter.AnnaulIncreaseMethod =2;
                filter.Location ??= 0;
                filter.Nationality ??= 0;
                var BranchesList = session.Branches.Split(',');
                var result = await hrServiceManager.HrIncrementService.IncrementsEvaluationsSearch(filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrIncrementsVw>.FailAsync(ex.Message));
            }
        }


        [HttpPost("MakeApproveByEvaluation")]
        public async Task<IActionResult> MakeApproveByEvaluation(List<MakeApproveDto> objects)
        {
            var chk = await permission.HasPermission(273, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result<object>.FailAsync($"Access Denied"));
            }
            if (objects.Count <= 0)
            {
                return Ok(await Result.FailAsync($"يجب تحديد صف واحد على الأقل"));
            }
            try
            {
                var result = await hrServiceManager.HrIncrementService.MakeApprove(objects);
                return Ok(result);
            }
            catch (Exception exp)
            {
                return Ok(await Result<object>.FailAsync($"EXP in {this.GetType()}, Meesage: {exp.Message}"));
            }
        }

        #endregion


    }
}