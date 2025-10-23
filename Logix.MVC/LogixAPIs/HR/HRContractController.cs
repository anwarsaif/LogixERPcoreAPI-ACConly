using AutoMapper;
using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.DTOs.Main;
using Logix.Application.Helpers;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Logix.MVC.LogixAPIs.HR
{
    //   العقود
    public class HRContractController : BaseHrApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IMapper mapper;
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;

        public HRContractController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IMainServiceManager mainServiceManager, IMapper mapper)
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
        public async Task<IActionResult> Search(HrContractFilterDto filter)
        {
            var chk = await permission.HasPermission(1695, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var BranchesList = session.Branches.Split(',');
                List<HrContractesVw> resultList = new List<HrContractesVw>();
                var items = await hrServiceManager.HrContracteService.GetAllVW(e => e.IsDeleted == false && BranchesList.Contains(e.BranchId.ToString()));
                if (items.Succeeded)
                {
                    if (items.Data.Count() > 0)
                    {

                        var res = items.Data.AsQueryable();
                        if (!string.IsNullOrEmpty(filter.EmpCode))
                        {
                            res = res.Where(c => c.EmpCode != null && c.EmpCode == filter.EmpCode);
                        }
                        if (!string.IsNullOrEmpty(filter.EmpName))
                        {
                            res = res.Where(c => (c.EmpName != null && c.EmpName.Contains(filter.EmpName)));
                        }

                        if (filter.DepartmentId != null && filter.DepartmentId > 0)
                        {
                            res = res.Where(c => c.DepartmentId != null && c.DepartmentId.Equals(filter.DepartmentId));
                        }
                        if (filter.BranchId != null && filter.BranchId > 0)
                        {
                            res = res.Where(c => c.BranchId != null && c.BranchId.Equals(filter.BranchId));
                        }
                        if (filter.LocationId != null && filter.LocationId > 0)
                        {
                            res = res.Where(c => c.LocationId != null && c.LocationId.Equals(filter.LocationId));
                        }
                        if (res.Any())
                            return Ok(await Result<List<HrContractesVw>>.SuccessAsync(res.ToList(), ""));
                        return Ok(await Result<List<HrContractesVw>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));
                    }
                    return Ok(await Result<List<HrContractesVw>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));
                }
                return Ok(await Result<HrContractesVw>.FailAsync(items.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrContractesVw>.FailAsync(ex.Message));
            }
        }

        [HttpPost("GetPagination")]
        public async Task<IActionResult> GetPagination([FromBody] HrContractFilterDto filter, int take = 5, long? lastSeenId = null)
        {
            var chk = await permission.HasPermission(1695, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                filter.BranchId ??= 0;
                filter.DepartmentId ??= 0;
                filter.LocationId ??= 0;
                var BranchesList = session.Branches.Split(',');
                var result = await hrServiceManager.HrContracteService.GetAllWithPaginationVW(
                    selector: x => x.Id,
                    expression: e => e.IsDeleted == false
               && (string.IsNullOrEmpty(filter.EmpCode) || e.EmpCode == filter.EmpCode)
               && (e.FacilityId == session.FacilityId)
               && (filter.BranchId != 0 ? e.BranchId == filter.BranchId : BranchesList.Contains(e.BranchId.ToString()))
               && (filter.LocationId == 0 || e.LocationId == filter.LocationId)
               && (filter.DepartmentId == 0 || e.DepartmentId == filter.DepartmentId),
                    take: take,
                    lastSeenId: lastSeenId
                );

                if (!result.Succeeded)
                    return StatusCode(result.Status.code, result.Status.message);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync(ex.Message));
            }
        }
        #endregion

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(long Id)
        {
            try
            {
                var chk = await permission.HasPermission(1695, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var item = await hrServiceManager.HrContracteService.GetOneVW(x => x.Id == Id);

                var contractsDeductionVw = await hrServiceManager.HrContractsDeductionVwService.GetAllVW(x => x.ContractId == Id && x.IsDeleted == false);
                var contractsAllowanceVw = await hrServiceManager.HrContractsAllowanceVwService.GetAllVW(x => x.ContractId == Id && x.IsDeleted == false);

                // 
                var fileDtos = new List<SaveFileDto>();
                var getFiles = await mainServiceManager.SysFileService.GetFilesForUser(Id, 106);

                fileDtos = getFiles.Data ?? new List<SaveFileDto>();
                return Ok(await Result<object>.SuccessAsync(new { data = item.Data, contractsDeductionVw = contractsDeductionVw.Data.ToList(), contractsAllowanceVw = contractsAllowanceVw.Data, fileDtos = fileDtos }));
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrTicketVw>.FailAsync($"====== Exp in Hr  Contract Controller getById, MESSAGE: {ex.Message}"));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var chk = await permission.HasPermission(1695, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await hrServiceManager.HrContracteService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete Hr Contract Controller, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Add")]
        public async Task<ActionResult> Add(HrContracteDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(1695, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var add = await hrServiceManager.HrContracteService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add Hr Contract  Controller, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Add2")]
        public async Task<ActionResult> Add2(HrContracteAdd2Dto obj)
        {
            try
            {
                var chk = await permission.HasPermission(1695, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var add = await hrServiceManager.HrContracteService.Add2(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add Hr Contract  Controller, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Add3")]
        public async Task<ActionResult> Add3(HrContracteAdd3Dto obj)
        {
            try
            {
                var chk = await permission.HasPermission(1695, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var add = await hrServiceManager.HrContracteService.Add3(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add Hr Contract  Controller, MESSAGE: {ex.Message}"));
            }
        }


        [HttpGet("EmpIdChangedAdd3")]
        public async Task<IActionResult> EmpIdChangedAdd3(string EmpId)
        {
            var chk = await permission.HasPermission(1695, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied(localization.GetMessagesResource("AccessDenied")));
            }
            if (string.IsNullOrEmpty(EmpId))
            {
                return Ok(await Result<HrEmployeeVw>.WarningAsync(localization.GetMessagesResource("EmployeeNumberIsRequired")));
            }

            try
            {
                var empData = await hrServiceManager.HrEmployeeService.GetOneVW(i => i.EmpId == EmpId && i.Isdel == false);
                if (empData.Succeeded)
                {
                    if (empData.Data != null)
                    {
                        if (empData.Data.StatusId == 2)
                        {
                            return Ok(await Result.WarningAsync(localization.GetHrResource("EmployeeAlreadyTerminated")));
                        }
                        return Ok(await Result<HrEmployeeVw>.SuccessAsync(empData.Data));
                    }
                    else
                    {
                        return Ok(await Result<HrEmployeeVw>.SuccessAsync(localization.GetResource1("EmployeeNotFound")));
                    }
                }
                return Ok(await Result<HrEmployeeVw>.FailAsync(localization.GetMessagesResource("NoDataWithId")));
            }
            catch (Exception exp)
            {
                return Ok(await Result<HrEmployeeVw>.FailAsync($"{exp.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(HrContracteEditDto obj)
        {
            var chk = await permission.HasPermission(1695, PermissionType.Edit);
            if (!chk)
                return Ok(await Result.AccessDenied("AccessDenied"));

            try
            {

                if (!ModelState.IsValid)
                    return Ok(await Result<HrContracteEditDto>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));
                obj.TypeId = 1;
                var update = await hrServiceManager.HrContracteService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrContracteEditDto>.FailAsync($"====== Exp in Hr Contract Controller getById, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetNewExpiryDate")]
        public async Task<ActionResult> GetNewExpiryDate(string? contractExpiryDateText, int duration = 0, int? durationType = 0)
        {
            DateTime newExpiryDate;

            if (string.IsNullOrEmpty(contractExpiryDateText))
            {
                return Ok(await Result.FailAsync($"هناك خطا في  تاريخ انتهاء العقد"));
            }

            if (duration <= 0)
            {
                return Ok(await Result.FailAsync($"يجب ادخال  مدة التجديد"));
            }

            if (durationType <= 0)
            {
                return Ok(await Result.FailAsync($"يجب تحديد نوع مدة التجديد"));
            }
            try
            {
                var contractExpiryDate = DateHelper.StringToDate(contractExpiryDateText);// DateTime.ParseExact(contractExpiryDateText, "yyyy/MM/dd", CultureInfo.InvariantCulture, DateTimeStyles.None).Date;

                switch (durationType)
                {
                    case 1:
                        int xYear = duration;
                        newExpiryDate = contractExpiryDate.AddYears(xYear).AddDays(-1);
                        break;
                    case 2:
                        int xMonth = duration;
                        newExpiryDate = contractExpiryDate.AddMonths(xMonth);
                        break;
                    case 3:
                        int xWeek = duration;
                        newExpiryDate = contractExpiryDate.AddDays(xWeek * 7);
                        break;
                    case 4:
                        int xDay = duration;
                        newExpiryDate = contractExpiryDate.AddDays(xDay);
                        break;
                    default:
                        return Ok(await Result.FailAsync($"خطأ في  نوع مدة التجديد"));
                }

                return Ok(await Result<string>.SuccessAsync(DateHelper.DateToString(newExpiryDate, CultureInfo.InvariantCulture)));
            }
            catch (Exception ex)
            {

                return Ok(await Result.FailAsync($"exception in GetNewExpiryDate :: the exception is ::{ex.Message.ToString()}"));
            }

        }


        [HttpGet("EmpIdChanged")]
        public async Task<IActionResult> EmpIdChanged(string EmpId)
        {
            var chk = await permission.HasPermission(1695, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied(localization.GetMessagesResource("AccessDenied")));
            }
            if (string.IsNullOrEmpty(EmpId))
            {
                return Ok(await Result<HrEmployeeVw>.WarningAsync(localization.GetMessagesResource("EmployeeNumberIsRequired")));
            }

            try
            {
                var checkEmpId = await hrServiceManager.HrEmployeeService.GetOneVW(i => i.EmpId == EmpId && i.Isdel == false);
                if (checkEmpId.Succeeded)
                {
                    if (checkEmpId.Data != null)
                    {
                        if (checkEmpId.Data.StatusId == 2)
                        {
                            return Ok(await Result.WarningAsync(localization.GetHrResource("EmployeeAlreadyTerminated")));
                        }
                        var Allowance = await hrServiceManager.HrAllowanceVwService.GetAll(x => x.EmpId == checkEmpId.Data.Id && x.IsDeleted == false && x.Status == true && x.FixedOrTemporary == 1 && x.TypeId == 1);
                        var Deduction = await hrServiceManager.HrDeductionVwService.GetAll(x => x.EmpId == checkEmpId.Data.Id && x.IsDeleted == false && x.Status == true && x.FixedOrTemporary == 1 && x.TypeId == 2);

                        return Ok(await Result<object>.SuccessAsync(new { Data = checkEmpId.Data, allowance = Allowance.Data, deduction = Deduction.Data }));
                    }
                    else
                    {
                        return Ok(await Result<HrEmployeeVw>.SuccessAsync(localization.GetResource1("EmployeeNotFound")));
                    }
                }
                return Ok(await Result<HrEmployeeVw>.FailAsync($"{checkEmpId.Status.message}"));
            }
            catch (Exception exp)
            {
                return Ok(await Result<HrEmployeeVw>.FailAsync($"{exp.Message}"));
            }
        }

        [HttpPost("AddSearch")]
        public async Task<IActionResult> AddSearch(HrContractAddFilterDto filter)
        {
            var chk = await permission.HasPermission(1695, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var DateGregorian = DateTime.Now.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);

                var BranchesList = session.Branches.Split(',');
                List<HrContractAddResponseDto> resultList = new List<HrContractAddResponseDto>();
                filter.BranchId ??= 0;
                filter.NationalityId ??= 0;
                filter.LocationId ??= 0;
                filter.ContractTypeID ??= 0;
                filter.JobCategory ??= 0;
                filter.DepartmentId ??= 0;
                var items = await hrServiceManager.HrEmployeeService.GetAllVW(e => e.Isdel == false && e.IsDeleted == false &&
                e.StatusId == 1 &&
                e.ContractExpiryDate != null &&
                e.ContractExpiryDate != "" &&
                (string.IsNullOrEmpty(filter.EmpName) || e.EmpName == filter.EmpName) &&
                (string.IsNullOrEmpty(filter.EmpCode) || e.EmpId == filter.EmpCode) &&
                (filter.BranchId != 0 ? e.BranchId == filter.BranchId : BranchesList.Contains(e.BranchId.ToString())) &&
                (filter.NationalityId == 0 || e.NationalityId == filter.NationalityId) &&
                (filter.LocationId == 0 || e.Location == filter.LocationId) &&
                (filter.JobCategory == 0 || e.JobCatagoriesId == filter.JobCategory) &&
                (filter.ContractTypeID == 0 || e.ContractTypeId == filter.ContractTypeID) &&
                (filter.DepartmentId == 0 || e.DeptId == filter.DepartmentId)
                );
                if (items.Succeeded)
                {
                    if (items.Data.Count() > 0)
                    {

                        var res = items.Data.AsQueryable();
                        if (!string.IsNullOrEmpty(filter.FromDate) && !string.IsNullOrEmpty(filter.ToDate))
                        {
                            res = res.Where(r => r.ContractExpiryDate != null &&
                            (DateHelper.StringToDate(r.ContractExpiryDate) >= DateHelper.StringToDate(filter.FromDate)) &&
                           (DateHelper.StringToDate(r.ContractExpiryDate) <= DateHelper.StringToDate(filter.ToDate))
                           );
                        }

                        if (res.Any())
                        {
                            foreach (var item in res)
                            {
                                decimal TotalAllowance = 0;
                                decimal TotalDeduction = 0;
                                var getTotalAllowance = await hrServiceManager.HrAllowanceDeductionService.GetTotalAllowances(item.Id);
                                if (getTotalAllowance.Succeeded) TotalAllowance = getTotalAllowance.Data;

                                var getTotalDeduction = await hrServiceManager.HrAllowanceDeductionService.GetTotalDeduction(item.Id);
                                if (getTotalDeduction.Succeeded) TotalDeduction = getTotalDeduction.Data;

                                var newItem = new HrContractAddResponseDto
                                {
                                    RemainingDays = (DateHelper.DateDiff_day(DateGregorian, item.ContractExpiryDate)),
                                    NetSalary = item.Salary + TotalAllowance - TotalDeduction,
                                    Deduction = TotalDeduction,
                                    Allowance = TotalAllowance
                                };
                                mapper.Map(item, newItem);
                                resultList.Add(newItem);
                            }
                            resultList.OrderBy(x => DateHelper.StringToDate(x.ContractExpiryDate));

                            return Ok(await Result<List<HrContractAddResponseDto>>.SuccessAsync(resultList));
                        }
                        return Ok(await Result<List<HrContractAddResponseDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));
                    }
                    return Ok(await Result<List<HrContractAddResponseDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));
                }
                return Ok(await Result<HrContractAddResponseDto>.FailAsync(items.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrContractAddResponseDto>.FailAsync(ex.Message));
            }
        }

        [HttpPost("AddSearchPagination")]
        public async Task<IActionResult> AddSearchPagination([FromBody] HrContractAddFilterDto filter, int take = 5, long? lastSeenId = null)
        {
            var chk = await permission.HasPermission(1695, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {

                var DateGregorian = DateTime.Now.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);

                var BranchesList = session.Branches.Split(',');
                List<HrContractAddResponseDto> resultList = new List<HrContractAddResponseDto>();
                filter.BranchId ??= 0;
                filter.NationalityId ??= 0;
                filter.LocationId ??= 0;
                filter.ContractTypeID ??= 0;
                filter.JobCategory ??= 0;
                filter.DepartmentId ??= 0;

                var dateConditions = new List<DateCondition>();
                dateConditions.Add(new DateCondition
                {
                    DatePropertyName = "ContractExpiryDate",
                    ComparisonOperator = ComparisonOperator.GreaterThanOrEqual,
                    StartDateString = filter.FromDate ?? ""
                });
                dateConditions.Add(new DateCondition
                {
                    DatePropertyName = "ContractExpiryDate",
                    ComparisonOperator = ComparisonOperator.LessThanOrEqual,
                    StartDateString = filter.ToDate ?? ""
                });

                var result = await hrServiceManager.HrEmployeeService.GetAllWithPaginationVW(
                    selector: x => x.Id,
                    expression: e => e.Isdel == false && e.IsDeleted == false &&
                e.StatusId == 1 &&
                e.ContractExpiryDate != null &&
                e.ContractExpiryDate != "" &&
                (string.IsNullOrEmpty(filter.EmpName) || e.EmpName == filter.EmpName) &&
                (string.IsNullOrEmpty(filter.EmpCode) || e.EmpId == filter.EmpCode) &&
                (filter.BranchId != 0 ? e.BranchId == filter.BranchId : BranchesList.Contains(e.BranchId.ToString())) &&
                (filter.NationalityId == 0 || e.NationalityId == filter.NationalityId) &&
                (filter.LocationId == 0 || e.Location == filter.LocationId) &&
                (filter.JobCategory == 0 || e.JobCatagoriesId == filter.JobCategory) &&
                (filter.ContractTypeID == 0 || e.ContractTypeId == filter.ContractTypeID) &&
                (filter.DepartmentId == 0 || e.DeptId == filter.DepartmentId),
                    take: take,
                    lastSeenId: lastSeenId,
                    dateConditions: (string.IsNullOrEmpty(filter.FromDate) || string.IsNullOrEmpty(filter.ToDate)) ? null : dateConditions);

                if (result.Succeeded)
                {
                    if (result.Data.Count() > 0)
                    {

                        var res = result.Data.AsQueryable();
                        if (res.Any())
                        {
                            foreach (var item in res)
                            {
                                decimal TotalAllowance = 0;
                                decimal TotalDeduction = 0;
                                var getTotalAllowance = await hrServiceManager.HrAllowanceDeductionService.GetTotalAllowances(item.Id);
                                if (getTotalAllowance.Succeeded) TotalAllowance = getTotalAllowance.Data;

                                var getTotalDeduction = await hrServiceManager.HrAllowanceDeductionService.GetTotalDeduction(item.Id);
                                if (getTotalDeduction.Succeeded) TotalDeduction = getTotalDeduction.Data;

                                var newItem = new HrContractAddResponseDto
                                {
                                    RemainingDays = (DateHelper.DateDiff_day(DateGregorian, item.ContractExpiryDate)),
                                    NetSalary = item.Salary + TotalAllowance - TotalDeduction,
                                    Deduction = TotalDeduction,
                                    Allowance = TotalAllowance
                                };
                                mapper.Map(item, newItem);
                                resultList.Add(newItem);
                            }
                            resultList.OrderBy(x => DateHelper.StringToDate(x.ContractExpiryDate));
                            var paginatedData = new PaginatedResult<object>
                            {
                                Succeeded = result.Succeeded,
                                Data = resultList,
                                Status = result.Status,
                                PaginationInfo = result.PaginationInfo
                            };
                            return Ok(paginatedData);
                        }
                        return Ok(await Result<List<HrContractAddResponseDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));
                    }
                    return Ok(await Result<List<HrContractAddResponseDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));
                }
                if (!result.Succeeded)
                    return StatusCode(result.Status.code, result.Status.message);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync(ex.Message));
            }
        }

    }
}




