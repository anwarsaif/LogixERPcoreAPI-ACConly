using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.DTOs.Main;
using Logix.Application.Helpers;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Result = Logix.Application.Wrapper.Result;

namespace Logix.MVC.LogixAPIs.HR
{
    // حسميات وبدلات أخرى
    public class HRDeductionAllowanceOtherController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IMainServiceManager mainServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ILocalizationService localization;
        private readonly ICurrentData session;

        public HRDeductionAllowanceOtherController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ILocalizationService localization, ICurrentData session, IMainServiceManager mainServiceManager)
        {
            this.permission = permission;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.session = session;
            this.mainServiceManager = mainServiceManager;
        }

        #region الصفحة الرئيسية



        [HttpPost("Search")]

        public async Task<IActionResult> Search(HrAllowanceDeductionOtherFilterDto filter)
        {
            var chk = await permission.HasPermission(178, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {

                var items = await hrServiceManager.HrAllowanceDeductionService.Search(filter);
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrAllowanceDeductionVw>.FailAsync(ex.Message));
            }
        }

        [HttpPost("GetPagination")]
        public async Task<IActionResult> GetPagination([FromBody] HrAllowanceDeductionOtherFilterDto filter, int take = Pagination.take, long? lastSeenId = null)
        {
            var chk = await permission.HasPermission(601, PermissionType.Show);
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
                    StartDateString = filter.From ?? ""
                });
                dateConditions.Add(new DateCondition
                {
                    DatePropertyName = "DueDate",
                    ComparisonOperator = ComparisonOperator.LessThanOrEqual,
                    StartDateString = filter.To ?? ""
                });
                filter.DeptId ??= 0;
                filter.Location ??= 0;
                filter.Amount ??= 0;
                filter.ADID ??= 0;
                filter.Type ??= 0;
                filter.BranchId ??= 0;

                var items = await hrServiceManager.HrAllowanceDeductionService.GetAllWithPaginationVW(selector: e => e.Id,
                expression: e => e.IsDeleted == false && e.FixedOrTemporary == 2 
                && BranchesList.Contains(e.BranchId.ToString())
                && (filter.DeptId == 0 || filter.DeptId == e.DeptId)
                && (filter.Location == 0 || filter.Location == e.Location)
                && (filter.Amount == 0 || filter.Amount == e.Amount)
                && (filter.ADID == 0 || filter.ADID == e.AdId)
                && (filter.Type == 0 || filter.Type == e.TypeId)
                && (string.IsNullOrEmpty(filter.EmpName) || e.EmpName.ToLower().Contains(filter.EmpName.ToLower()) || e.EmpName2.ToLower().Contains(filter.EmpName.ToLower()))
               && (string.IsNullOrEmpty(filter.EmpCode) || e.EmpCode == filter.EmpCode)
               && (filter.BranchId == 0 || BranchesList.Contains(e.BranchId.ToString())),
                    take: take,
                    lastSeenId: lastSeenId,
                   dateConditions: (string.IsNullOrEmpty(filter.From) || string.IsNullOrEmpty(filter.To)) ? null : dateConditions);

                if (!items.Succeeded)
                    return Ok(await Result<List<HrAllowanceDeductionVw>>.FailAsync(items.Status.message));

                var paginatedData = new PaginatedResult<object>
                {
                    Succeeded = items.Succeeded,
                    Data = items.Data,
                    Status = items.Status,
                    PaginationInfo = items.PaginationInfo
                };
                return Ok(paginatedData);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long Id = 0)
        {
            var chk = await permission.HasPermission(178, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (Id == 0)
            {
                return Ok(await Result.FailAsync($"{localization.GetMessagesResource("ChooseDelete")}"));
            }

            try
            {
                var del = await hrServiceManager.HrAllowanceDeductionService.RemoveOtherDeductionAllowance(Id);

                return Ok(del);
            }
            catch (Exception exp)
            {
                return Ok(await Result.FailAsync($"{exp.Message}"));
            }
        }

        [HttpDelete("DeleteList")]
        public async Task<IActionResult> DeleteList(List<long> Ids)
        {
            var chk = await permission.HasPermission(178, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (Ids.Count() <= 0)
            {
                return Ok(await Result.FailAsync($"{localization.GetMessagesResource("ChooseDelete")}"));
            }

            try
            {
                var del = await hrServiceManager.HrAllowanceDeductionService.RemoveOtherDeductionAllowance(Ids);
                return Ok(del);
            }
            catch (Exception exp)
            {
                return Ok(await Result.FailAsync($"{exp.Message}"));
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long? Id)
        {
            var chk = await permission.HasPermission(178, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (Id <= 0)
            {
                return Ok(await Result.FailAsync("Id is Required"));
            }

            try
            {
                var result = await hrServiceManager.HrAllowanceDeductionService.GetOneVW(x => x.IsDeleted == false && x.FixedOrTemporary == 2 && x.Status == true && x.Id == Id);
                var files = await mainServiceManager.SysFileService.GetAll(x => x.IsDeleted == false && x.PrimaryKey == Id && x.TableId == 49);
                return Ok(await Result<object>.SuccessAsync(new { data = result.Data, fileDtos = files }, ""));
                //return Ok(result);
            }
            catch (Exception exp)
            {
                return Ok(await Result<object>.FailAsync($"{exp.Message}"));
            }
        }

        #endregion


        #region صفحة التعديل

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(HrOtherAllowanceDeductionEditDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(178, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));



                var update = await hrServiceManager.HrAllowanceDeductionService.EditOtherAllowanceDeduction(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrHolidayEditDto>.FailAsync($"====== Exp in Edit HR HrEmployeeCost  Controller, MESSAGE: {ex.Message}"));
            }
        }

        #endregion


        #region صفحة الاضافة

        [HttpPost("Add")]
        public async Task<ActionResult> Add(HrOtherAllowanceDeductionAddDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(178, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result<HrAllowanceDeductionDto>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));
                if (string.IsNullOrEmpty(obj.EmpCode))
                {
                    return Ok(await Result.WarningAsync(localization.GetResource1("EmployeeNotFound")));

                }


                var add = await hrServiceManager.HrAllowanceDeductionService.AddOtherAllowanceDeduction(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrAllowanceDeductionDto>.FailAsync($"====== Exp in Add   HrAllowanceDeductionDto Controller  Add Method, MESSAGE: {ex.Message}"));
            }
        }

        //[HttpGet("EmpCodeChanged")]
        //public async Task<IActionResult> EmpCodeChanged(string? EmpCode)
        //{
        //    var chk = await permission.HasPermission(178, PermissionType.Delete);
        //    if (!chk)
        //    {
        //        return Ok(await Result.AccessDenied("AccessDenied"));
        //    }
        //    if (string.IsNullOrEmpty(EmpCode))
        //    {
        //        return Ok(await Result.WarningAsync("Emp Code is Required"));
        //    }
        //    try
        //    {
        //        decimal TotalAllowance = 0;
        //        decimal TotalDeduction = 0;
        //        var checkEmpId = await mainServiceManager.InvestEmployeeService.GetOne(i => i.EmpId == EmpCode && i.Isdel == false && i.IsDeleted == false);
        //        if (checkEmpId.Data == null)
        //        {
        //            return Ok(await Result<object>.WarningAsync($"{localization.GetResource1("EmployeeNotFound")}"));
        //        }

        //        var getTotalAllowance = await hrServiceManager.HrAllowanceDeductionService.GetTotalAllowances((long)checkEmpId.Data.Id);
        //        if (getTotalAllowance.Succeeded) TotalAllowance = getTotalAllowance.Data;

        //        var getTotalDeduction = await hrServiceManager.HrAllowanceDeductionService.GetTotalDeduction((long)checkEmpId.Data.Id);
        //        if (getTotalDeduction.Succeeded) TotalDeduction = getTotalDeduction.Data;
        //        var getEmployeeData = await hrServiceManager.HrEmployeeService.GetOneVW(e => e.Id == checkEmpId.Data.Id);

        //        return Ok(await Result<object>.SuccessAsync(new { Salary = getEmployeeData.Data.Salary, NetSalary = getEmployeeData.Data.Salary + TotalAllowance - TotalDeduction, Allowance = TotalAllowance, Deduction = TotalDeduction }));
        //    }
        //    catch (Exception exp)
        //    {
        //        return Ok(await Result<object>.FailAsync($"{exp.Message}"));
        //    }
        //}
        [HttpGet("EmpCodeChanged")]
        public async Task<IActionResult> EmpCodeChanged(string? EmpCode)
        {
            var chk = await permission.HasPermission(178, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (string.IsNullOrEmpty(EmpCode))
            {
                return Ok(await Result.WarningAsync("Emp Code is Required"));
            }
            try
            {
                decimal TotalAllowance = 0;
                decimal TotalDeduction = 0;

                var employeeResult = await hrServiceManager.HrEmployeeService.GetEmpByCode(EmpCode, session.FacilityId);
                if (!employeeResult.Succeeded || employeeResult.Data == null)
                {
                    // في حال لم يتم العثور على الموظف
                    return Ok(await Result<object>.WarningAsync(localization.GetResource1("EmployeeNotFound")));
                }


                var getTotalAllowance = await hrServiceManager.HrAllowanceDeductionService.GetTotalAllowances((long)employeeResult.Data.Id);
                if (getTotalAllowance.Succeeded) TotalAllowance = getTotalAllowance.Data;

                var getTotalDeduction = await hrServiceManager.HrAllowanceDeductionService.GetTotalDeduction((long)employeeResult.Data.Id);
                if (getTotalDeduction.Succeeded) TotalDeduction = getTotalDeduction.Data;
                var getEmployeeData = await hrServiceManager.HrEmployeeService.GetOneVW(e => e.Id == employeeResult.Data.Id);

                return Ok(await Result<object>.SuccessAsync(new { Salary = getEmployeeData.Data.Salary, NetSalary = getEmployeeData.Data.Salary + TotalAllowance - TotalDeduction, Allowance = TotalAllowance, Deduction = TotalDeduction }));
            }
            catch (Exception exp)
            {
                return Ok(await Result<object>.FailAsync($"{exp.Message}"));
            }
        }
        #endregion

        #region  اضافة - حسميات أو بدلات متعددة


        [HttpPost("SearchOnMultiAdd")]
        //public async Task<IActionResult> SearchOnMultiAdd(HrAllowanceDeductionOtherFilterDto filter)
        //{
        //    var chk = await permission.HasPermission(178, PermissionType.Show);
        //    if (!chk)
        //        return Ok(await Result.AccessDenied("AccessDenied"));



        //    int typeId = (filter.Type == 20) ? 1 : 2;

        //    var BranchesList = session.Branches.Split(',');
        //    filter.BranchId ??= 0;

        //    var employeeys = await hrServiceManager.HrEmployeeService.GetAllVW(x =>
        //        x.Isdel == false &&
        //        x.IsDeleted == false &&
        //        x.StatusId == 1 &&
        //        ((filter.BranchId != 0 && x.BranchId == filter.BranchId) || BranchesList.Contains(x.BranchId.ToString())) &&
        //        (string.IsNullOrEmpty(filter.EmpName) || x.EmpName.ToLower().Contains(filter.EmpName.ToLower()) || x.EmpName2.ToLower().Contains(filter.EmpName.ToLower())) &&
        //        (string.IsNullOrEmpty(filter.EmpCode) || x.EmpId == filter.EmpCode) &&
        //        (filter.DeptId == null || filter.DeptId == 0 || x.DeptId == filter.DeptId) &&
        //        (filter.categoryId == null || filter.categoryId == 0 || x.CatagoriesId == filter.categoryId)
        //    );

        //    if (!employeeys.Succeeded)
        //        return Ok(await Result<HrAllowanceDeductionOtherFilterDto>.FailAsync(employeeys.Status.message));

        //    var res = employeeys.Data.AsQueryable();

        //    if (filter.BranchId != null && filter.BranchId > 0)
        //        res = res.Where(c => c.BranchId == filter.BranchId);

        //    var items = await hrServiceManager.HrAllowanceDeductionService.GetAll(e =>
        //        e.IsDeleted == false &&
        //        e.FixedOrTemporary == 2 &&
        //        e.Status == true &&
        //        (e.AdId == filter.ADID) &&
        //        (e.TypeId == typeId) &&
        //        (string.IsNullOrEmpty(filter.DueDate) || e.DueDate == filter.DueDate)
        //    );

        //    if (!items.Succeeded)
        //        return Ok(await Result<List<HrAllowanceDeductionOtherFilterDto>>.SuccessAsync(new(), localization.GetResource1("NosearchResult")));

        //    var resultList = new List<HrAllowanceDeductionOtherFilterDto>();

        //    foreach (var employee in res)
        //    {
        //        decimal? valueFromDB = items.Data
        //            .Where(x => x.EmpId == employee.Id)
        //            .Select(x => x.Amount)
        //            .Sum();

        //        var dto = new HrAllowanceDeductionOtherFilterDto
        //        {
        //            Id = employee.Id,
        //            EmpCode = employee.EmpId,
        //            EmpName = employee.EmpName ?? "",
        //            EmpName2 = employee.EmpName2 ?? "",
        //            DeptName = employee.DepName,
        //            CatName = employee.CatName,
        //            Amount = valueFromDB
        //        };

        //        // Optional override logic from VB.NET (manual value)
        //        if (filter.Amount != null && filter.Amount > 0)
        //        {
        //            dto.Amount = filter.Amount;
        //        }

        //        resultList.Add(dto);
        //    }

        //    return Ok(await Result<List<HrAllowanceDeductionOtherFilterDto>>.SuccessAsync(resultList, resultList.Any() ? "" : localization.GetResource1("NosearchResult")));
        //}
        public async Task<IActionResult> SearchOnMultiAdd(HrAllowanceDeductionOtherFilterDto filter)
        {
            // تحقق من الصلاحيات
            if (!await permission.HasPermission(178, PermissionType.Show))
                return Ok(await Result.AccessDenied("AccessDenied"));

            // تحديد النوع
            int typeId = filter.Type == 20 ? 1 : 2;

            // إعداد قائمة الفروع
            var branchesList = session.Branches.Split(',');
            filter.BranchId ??= 0;

            // استعلام الموظفين
            var employeesResponse = await hrServiceManager.HrEmployeeService.GetAllVW(e =>
                e.Isdel == false &&
                e.IsDeleted == false &&
                e.StatusId == 1 &&
                ((filter.BranchId != 0 && e.BranchId == filter.BranchId) || branchesList.Contains(e.BranchId.ToString())) &&
                (string.IsNullOrEmpty(filter.EmpName) ||
                    e.EmpName.ToLower().Contains(filter.EmpName.ToLower()) ||
                    e.EmpName2.ToLower().Contains(filter.EmpName.ToLower())) &&
                (string.IsNullOrEmpty(filter.EmpCode) || e.EmpId == filter.EmpCode) &&
                (filter.DeptId == null || filter.DeptId == 0 || e.DeptId == filter.DeptId) &&
                (filter.categoryId == null || filter.categoryId == 0 || e.CatagoriesId == filter.categoryId)
            );

            if (!employeesResponse.Succeeded)
                return Ok(await Result<HrAllowanceDeductionOtherFilterDto>.FailAsync(employeesResponse.Status.message));

            // تحويل النتائج إلى Queryable
            var employees = employeesResponse.Data.AsQueryable();



            // استعلام البدلات والخصومات
            var allowancesResponse = await hrServiceManager.HrAllowanceDeductionService.GetAll(a =>
                a.IsDeleted == false &&
                a.FixedOrTemporary == 2 &&
                a.Status == true &&
                a.AdId == filter.ADID &&
                a.TypeId == typeId &&
                (string.IsNullOrEmpty(filter.DueDate) || a.DueDate == filter.DueDate)
            );

            if (!allowancesResponse.Succeeded)
                return Ok(await Result<List<HrAllowanceDeductionOtherFilterDto>>
                    .SuccessAsync(new(), localization.GetResource1("NosearchResult")));

            // تجهيز النتائج النهائية
            var resultList = employees.Select(employee => new HrAllowanceDeductionOtherFilterDto
            {
                Id = employee.Id,
                EmpCode = employee.EmpId,
                EmpName = employee.EmpName ?? "",
                EmpName2 = employee.EmpName2 ?? "",
                DeptName = employee.DepName,
                CatName = employee.CatName,
                Amount = filter.Amount > 0
           ? filter.Amount
           : allowancesResponse.Data.Where(x => x.EmpId == employee.Id).Sum(x => x.Amount)
            }).ToList();


            // إعادة النتيجة
            var message = resultList.Any() ? "" : localization.GetResource1("NosearchResult");
            return Ok(await Result<List<HrAllowanceDeductionOtherFilterDto>>.SuccessAsync(resultList, message));
        }


        [HttpPost("AddOnMultiAdd")]
        public async Task<ActionResult> AddOnMultiAdd(HrOtherAllowanceDeductionMultiAddDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(178, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result<HrAllowanceDeductionDto>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));
                if (obj.dataDto.Count() < 1)
                {
                    return Ok(await Result.FailAsync("there is no data"));

                }

                var add = await hrServiceManager.HrAllowanceDeductionService.MultiAddOtherAllowanceDeduction(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrAllowanceDeductionDto>.FailAsync($"====== Exp in Add   HrAllowanceDeduction Controller  Add Method, MESSAGE: {ex.Message}"));
            }
        }

        #endregion



        #region   اضافة - حسميات أو بدلات أخرى لفترة

        [HttpPost("AddOnIntervalAdd")]
        public async Task<ActionResult> AddOnIntervalAdd(HrOtherAllowanceDeductionIntervalAddDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(178, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result<HrAllowanceDeductionDto>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                //if (string.IsNullOrEmpty(obj.FromDate))
                //{
                //    return Ok(await Result.FailAsync("يجب ادخال من  تاريخ "));

                //}
                //if (string.IsNullOrEmpty(obj.ToDate))
                //{
                //    return Ok(await Result.FailAsync("يجب ادخال الى  تاريخ "));

                //}
                //if (obj.TypeId <= 0)
                //{
                //    return Ok(await Result.FailAsync("يجب ادخال الفئة "));

                //}
                //if (obj.AdId <= 0)
                //{
                //    return Ok(await Result.FailAsync("يجب ادخال النوع"));

                //}
                //if (obj.Amount <= 0 || obj.Amount == null)
                //{
                //    return Ok(await Result.FailAsync("يجب ادخال المبلغ"));

                //}
                //if ((DateHelper.StringToDate(obj.ToDate) <= (DateHelper.StringToDate(obj.FromDate))))
                //{
                //    return Ok(await Result.FailAsync("يجب التأكد من تاريخ بداية ونهاية الفترة"));

                //}
                var add = await hrServiceManager.HrAllowanceDeductionService.IntervalAddOtherAllowanceDeduction(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrAllowanceDeductionDto>.FailAsync($"====== Exp in Add   HrAllowanceDeduction Controller  Add Method, MESSAGE: {ex.Message}"));
            }
        }

        #endregion

        #region  سحب البدلات والحسميات من الإكسل

        [HttpPost("AddFromExcel")]
        public async Task<ActionResult> AddFromExcel(List<HrOtherAllowanceDeductionAddFromExcelDto> obj)
        {
            try
            {
                var chk = await permission.HasPermission(178, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result<string>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));
                if (obj.Count <= 0) return Ok(await Result.FailAsync($"يجب تعبئة الملف "));

                if (obj.Any(d => string.IsNullOrEmpty(d.EmpCode)))
                {
                    return Ok(await Result.FailAsync("يجب ادخال  ارقم الموظفين"));

                }
                if (obj.Any(d => string.IsNullOrEmpty(d.DueDate)))
                {
                    return Ok(await Result.FailAsync("يجب ادخال التواريخ"));

                }
                if (obj.Any(d => d.TypeId <= 0 || d.TypeId == null))
                {
                    return Ok(await Result.FailAsync("يجب ادخال الفئات "));

                }
                if (obj.Any(d => d.AdId <= 0 || d.AdId == null))
                {
                    return Ok(await Result.FailAsync("يجب ادخال الانوع"));

                }
                if (obj.Any(d => d.Amount <= 0 || d.Amount == null))
                {
                    return Ok(await Result.FailAsync("يجب ادخال المبالغ"));

                }
                var add = await hrServiceManager.HrAllowanceDeductionService.AddOtherAllowanceDeductionFromExcel(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"====== Exp in Add   HrAllowanceDeductionDto Controller  AddUsingExcel Method, MESSAGE: {ex.Message}"));
            }
        }
        #endregion

    }
}
