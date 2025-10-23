using AutoMapper;
using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.DTOs.HR.EmployeeDto;
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
    //   قائمة الموظفين
    public class HrEmployeeApiController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IMainServiceManager mainServiceManager;
        private readonly IPermissionHelper permission;
        private readonly IDDListHelper listHelper;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IMapper mapper;
        private readonly ISysConfigurationAppHelper sysConfigurationAppHelper;
        private readonly IAccServiceManager accServiceManager;
        private readonly ISysConfigurationHelper configHelper;
        public HrEmployeeApiController(IHrServiceManager hrServiceManager,
            IMainServiceManager mainServiceManager,
            IPermissionHelper permission,
            IDDListHelper listHelper,
            ILocalizationService localization,
            ICurrentData session,
            ISysConfigurationHelper configHelper,
            IMapper _mapper,
            ISysConfigurationAppHelper sysConfigurationAppHelpe,
            IAccServiceManager accServiceManager
            )
        {
            this.hrServiceManager = hrServiceManager;
            this.mainServiceManager = mainServiceManager;
            this.permission = permission;
            this.listHelper = listHelper;
            this.session = session;
            this.localization = localization;
            this.configHelper = configHelper;
            this.mapper = _mapper;
            this.sysConfigurationAppHelper = sysConfigurationAppHelpe;
            this.accServiceManager = accServiceManager;
        }

        [HttpGet("GetPropertyValues")]
        public async Task<IActionResult> GetPropertyValues()
        {
            List<long> propertyIds = new() { 469, 284, 410, 418, 454, 306, 456, 27, 28, 29, 30, 409 };

            // جلب البيانات
            var properties = await mainServiceManager.SysPropertyValueService.GetAll(
                x => propertyIds.Contains(x.PropertyId ?? 0) && x.FacilityId == session.FacilityId
            );

            // تأكد أن Data ليس null
            var data = properties?.Data ?? new List<SysPropertyValueDto>();

            // إنشاء قاموس باستخدام LINQ بحت
            var valueDict = propertyIds
                .ToDictionary(
                    id => id,
                    id => data
                        .Where(p => p.PropertyId == id)
                        .Select(p => string.IsNullOrEmpty(p.PropertyValue) ? "0" : p.PropertyValue)
                        .FirstOrDefault() ?? "0"
                );

            return Ok(await Result<object>.SuccessAsync(valueDict));
        }

        [HttpPost("Search")]
        public async Task<IActionResult> Search(EmployeeFilterDto filter)
        {
            var chk = await permission.HasPermission(28, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {

                var items = await hrServiceManager.HrEmployeeService.Search(filter);

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync(ex.Message));

            }
        }

        [HttpPost("GetPagination")]
        public async Task<IActionResult> GetPagination([FromBody] EmployeeFilterDto filter, int take = Pagination.take, long? lastSeenId = null)
        {
            var chk = await permission.HasPermission(28, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                filter.BranchId ??= 0;
                filter.DeptId ??= 0;
                filter.NationalityId ??= 0;
                filter.JobType ??= 0;
                filter.Status ??= 0;
                filter.JobCatagoriesId ??= 0;
                filter.NationalityId ??= 0;
                filter.Protection ??= 0;
                filter.ContractType ??= 0;
                filter.Degree ??= 0;
                filter.Level ??= 0;
                filter.FacilityId ??= 0;
                filter.SponsorsID ??= 0;
                filter.Location ??= 0;


                // عرف المتغير خارج if
                List<string> childDepartment = new List<string>();

                if (filter.DeptId != 0)
                {
                    var departmentes = await mainServiceManager.SysDepartmentService
                        .GetchildDepartment(filter.DeptId ?? 0);

                    if (departmentes?.Data != null)
                        childDepartment = departmentes.Data;
                }

                var BranchesList = session.Branches.Split(',');

                var result = await hrServiceManager.HrEmployeeService.GetAllWithPaginationVW(
                    selector: x => x.Id,
                    expression: e =>
                        e.IsDeleted == false &&
                        e.IsSub == false &&
                        (string.IsNullOrEmpty(filter.EmpName) ||
                         e.EmpName.ToLower().Contains(filter.EmpName.ToLower()) ||
                         e.EmpName2.ToLower().Contains(filter.EmpName.ToLower())) &&
                        (string.IsNullOrEmpty(filter.EmpCode) || e.EmpId == filter.EmpCode) &&
                        (filter.JobType == 0 || filter.JobType == e.JobType) &&
                        (filter.Status == 0 || filter.Status == e.StatusId) &&
                        (filter.JobCatagoriesId == 0 || filter.JobCatagoriesId == e.JobCatagoriesId) &&
                        (filter.NationalityId == 0 || filter.NationalityId == e.NationalityId) &&
                        (filter.DeptId == 0 || childDepartment.Contains(e.DeptId.ToString())) &&
                        (string.IsNullOrEmpty(filter.IdNo) || e.IdNo == filter.IdNo) &&
                        (string.IsNullOrEmpty(filter.PassId) || e.PassportNo == filter.PassId) &&
                        (string.IsNullOrEmpty(filter.EntryNo) || e.EntryNo == filter.EntryNo) &&
                        (filter.Location == 0 || e.Location == filter.Location) &&
                        (filter.SponsorsID == 0 || filter.SponsorsID == e.SponsorsId) &&
                        (filter.FacilityId == 0 || filter.FacilityId == e.FacilityId) &&
                        (filter.Level == 0 || filter.Level == e.LevelId) &&
                        (filter.Degree == 0 || filter.Degree == e.DegreeId) &&
                        (filter.ContractType == 0 || filter.ContractType == e.ContractTypeId) &&
                        (filter.Protection == 0 || filter.Protection == e.WagesProtection) &&
                        (string.IsNullOrEmpty(filter.EmpCode2) || e.EmpCode2 == filter.EmpCode2)
                    &&
                    (filter.BranchId != 0
                        ? e.BranchId == filter.BranchId
                        : BranchesList.Contains(e.BranchId.ToString())
                    )
                    ,
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

        [HttpGet("GetEmployeeById")]
        public async Task<IActionResult> GetEmployeeById(long Id)
        {
            var chk = await permission.HasPermission(28, PermissionType.Show);
            if (!chk)
            {
                return BadRequest(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var item = await hrServiceManager.HrEmployeeService.GetOneVW(e => e.Id == Id && e.IsDeleted == false && e.IsSub == false);

                var managerCode = await mainServiceManager.InvestEmployeeService.GetOne(m => m.Id == item.Data.ManagerId && m.Isdel == false && m.IsDeleted == false);
                if (managerCode != null && managerCode.Data != null)
                {
                    item.Data.ManagerCode = managerCode.Data.EmpId;
                }
                if (item.Succeeded)
                {
                    if (item.Data != null)
                    {
                        //التشيك هل الموظف من ضمن صلاحيات الفروع المتاحة للمستخدم ام لا

                        var CHeckEmpInBranch = await hrServiceManager.HrEmployeeService.CHeckEmpInBranch(item.Data.Id);

                        if (CHeckEmpInBranch.Succeeded && CHeckEmpInBranch.Data == false)
                        {
                            return Ok(await Result<object>.FailAsync(CHeckEmpInBranch.Status.message));

                        }
                        decimal TotalAllowance = 0;
                        decimal TotalDeduction = 0;

                        ///////////////////////////
                        var result = mapper.Map<EmpDataForEditDto>(item.Data);
                        result.HrEmployeeVw = item.Data;
                        if (!string.IsNullOrEmpty(item.Data.Doappointment))
                        {
                            int daysCount = (DateTime.Now - DateHelper.StringToDate(item.Data.Doappointment)).Days;
                            int MonthsCount = ((DateTime.Now.Year - DateHelper.StringToDate(item.Data.Doappointment).Year) * 12 + (DateTime.Now.Month - DateHelper.StringToDate(item.Data.Doappointment).Month));
                            int YearsCount = DateTime.Now.Year - DateHelper.StringToDate(item.Data.Doappointment).Year;
                            if (DateTime.Now.Month < DateHelper.StringToDate(item.Data.Doappointment).Month || (DateTime.Now.Month == DateHelper.StringToDate(item.Data.Doappointment).Month && DateTime.Now.Day < DateHelper.StringToDate(item.Data.Doappointment).Day))
                            {
                                YearsCount--;
                            }
                            result.DaysServes = daysCount;
                            result.MonthServes = MonthsCount;
                            result.YearServes = YearsCount;
                        }

                        var LastworkingDay = await hrServiceManager.HrLeaveService.GetOneVW(x => x.EmpCode == item.Data.EmpId); var getTotalAllowance = await hrServiceManager.HrAllowanceDeductionService.GetAll(e => e.EmpId == item.Data.Id && e.IsDeleted == false && e.TypeId == 1 && e.FixedOrTemporary == 1);
                        if (LastworkingDay.Succeeded && LastworkingDay.Data != null)
                        {
                            result.LastworkingDay = LastworkingDay.Data.LastWorkingDay;
                        }
                        else
                        {
                            result.LastworkingDay = "";
                        }

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
                        var employee = await mainServiceManager.InvestEmployeeService.GetOne(x => x.Id == Id);
                        //if (employee?.Data.ParentId == null)
                        //    return null;
                        if (employee.Data != null && employee.Succeeded)
                        {
                            var parent = await mainServiceManager.InvestEmployeeService.GetOne(x => x.Id == employee.Data.ParentId);

                            var ParentCode = "";
                            if (parent.Data != null && parent.Succeeded)
                            {
                                if (!string.IsNullOrEmpty(parent.Data.EmpId))
                                {
                                    ParentCode = parent?.Data.EmpId;
                                    result.HrEmployeeVw.ParentId = Convert.ToInt64(ParentCode);
                                }
                            }
                        }
                        var totalSalary = TotalAllowance + item.Data.Salary;
                        var netSalary = TotalAllowance + item.Data.Salary - TotalDeduction;
                        result.TotalSalary = totalSalary;
                        result.NetSalary = netSalary;
                        if (!string.IsNullOrEmpty(result.HrEmployeeVw.BirthDate))
                        {
                            var birthDate = DateTime.ParseExact(result.HrEmployeeVw.BirthDate, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                            var now = DateTime.Today;
                            result.Age = now.Year - birthDate.Year;

                            if (birthDate > now.AddYears(-result.Age ?? 0))
                                result.Age--;
                        }
                        else
                        {
                            result.Age = null;
                        }

                        if (result.HrEmployeeVw.CcId != null && result.HrEmployeeVw.CcId > 0)
                        {
                            var costCenterCode = await accServiceManager.AccCostCenterService.GetOne(x => x.CcId == result.HrEmployeeVw.CcId);
                            if (!string.IsNullOrEmpty(costCenterCode.Data.CostCenterCode))
                            {
                                result.HrEmployeeVw.CcId = Convert.ToInt64(costCenterCode.Data.CostCenterCode);
                            }
                        }
                        if (result.HrEmployeeVw.CcId2 != null && result.HrEmployeeVw.CcId2 > 0)
                        {
                            var costCenterCode2 = await accServiceManager.AccCostCenterService.GetOne(x => x.CcId == result.HrEmployeeVw.CcId2);
                            if (!string.IsNullOrEmpty(costCenterCode2.Data.CostCenterCode))
                            {
                                result.HrEmployeeVw.CcId2 = Convert.ToInt64(costCenterCode2.Data.CostCenterCode);
                            }
                        }
                        if (result.HrEmployeeVw.CcId3 != null && result.HrEmployeeVw.CcId3 > 0)
                        {
                            var costCenterCode3 = await accServiceManager.AccCostCenterService.GetOne(x => x.CcId == result.HrEmployeeVw.CcId3);
                            if (!string.IsNullOrEmpty(costCenterCode3.Data.CostCenterCode))
                            {
                                result.HrEmployeeVw.CcId3 = Convert.ToInt64(costCenterCode3.Data.CostCenterCode);
                            }
                        }
                        if (result.HrEmployeeVw.CcId4 != null && result.HrEmployeeVw.CcId4 > 0)
                        {
                            var costCenterCode4 = await accServiceManager.AccCostCenterService.GetOne(x => x.CcId == result.HrEmployeeVw.CcId4);
                            if (!string.IsNullOrEmpty(costCenterCode4.Data.CostCenterCode))
                            {
                                result.HrEmployeeVw.CcId4 = Convert.ToInt64(costCenterCode4.Data.CostCenterCode);
                            }
                        }
                        if (result.HrEmployeeVw.CcId5 != null && result.HrEmployeeVw.CcId5 > 0)
                        {
                            var costCenterCode5 = await accServiceManager.AccCostCenterService.GetOne(x => x.CcId == result.HrEmployeeVw.CcId5);
                            if (!string.IsNullOrEmpty(costCenterCode5.Data.CostCenterCode))
                            {
                                result.HrEmployeeVw.CcId5 = Convert.ToInt64(costCenterCode5.Data.CostCenterCode);
                            }
                        }

                        var getAllowance = await hrServiceManager.HrAllowanceVwService.GetAllVW(e => e.EmpId == item.Data.Id && e.IsDeleted == false && e.Status == true && e.TypeId == 1 && e.FixedOrTemporary == 1);
                        var getDeduction = await hrServiceManager.HrDeductionVwService.GetAllVW(e => e.EmpId == item.Data.Id && e.IsDeleted == false && e.Status == true && e.TypeId == 2 && e.FixedOrTemporary == 1);
                        result.SalryAllownce = getAllowance.Data.ToList();
                        result.SalryDeduction = getDeduction.Data.ToList();
                        result.NetSalary = item.Data.Salary + TotalAllowance - TotalDeduction;

                        List<long> propertyIds = new() { 469, 284, 410, 418, 454, 306, 456, 27, 28, 29, 30, 409 };
                        var properties = await mainServiceManager.SysPropertyValueService.GetAll(x => propertyIds.Contains(x.PropertyId ?? 0)
                            && x.FacilityId == session.FacilityId);

                        //result.Properties = properties.Data.ToDictionary(p => p.PropertyId ?? 0, p => p.PropertyValue ?? "");
                        var valueDict = properties.Data
                            .Where(p => p.PropertyId.HasValue)
                            .ToDictionary(p => p.PropertyId.Value, p => p.PropertyValue ?? "");

                        // Ensure all propertyIds exist in final result
                        result.Properties = propertyIds.ToDictionary(
                            id => id,
                            id => valueDict.ContainsKey(id) ? valueDict[id] : ""
                        );
                        var dependents = await hrServiceManager.HrDependentService.GetAllVW(x => x.IsDeleted == false && x.EmpId == Id);
                        result.Dependents = dependents?.Data?.ToList() ?? new List<HrDependentsVw>();

                        var archives = await hrServiceManager.HrArchiveFilesDetailService.GetAllVW(x => x.IsDeleted == false && x.EmpId == Id);
                        result.Archives = archives.Data.ToList();

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

        [HttpGet("GetEmployeeByEmpId")]
        public async Task<IActionResult> GetEmployeeByEmpId(string empId)
        {
            var chk = await permission.HasPermission(28, PermissionType.Show);
            if (!chk)
            {
                return BadRequest(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var item = await hrServiceManager.HrEmployeeService.GetOne(e => e.EmpId == empId && e.IsDeleted == false && e.IsSub == false);
                if (item.Succeeded)
                {
                    return Ok(item);
                }

                return Ok(item);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }




        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var chk = await permission.HasPermission(28, PermissionType.Show);
            if (!chk)
            {
                return BadRequest(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var items = await hrServiceManager.HrEmployeeService.GetAllVW(e => e.IsDeleted == false && e.IsSub == false);
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    res = res.OrderBy(e => e.EmpId);
                    return Ok(items);
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }




        #region //======================== Edit Employee Apis =====================

        [HttpPost("EditEmployeeMainInfo")]
        public async Task<IActionResult> EditEmployeeMainInfo(EmployeeMainInfoDto obj)
        {
            var chk = await permission.HasPermission(28, PermissionType.Edit);
            if (!chk)
            {
                return BadRequest(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<EmployeeMainInfoDto>.FailAsync($"يجب ادخال كل البيانات بشكل صحيح"));
                }
                obj.VisaNo = obj.VisaNo;
                var edit = await mainServiceManager.InvestEmployeeService.UpdateMain(obj);
                return Ok(edit);
            }
            catch (Exception ex)
            {
                return Ok(await Result<EmployeeMainInfoDto>.FailAsync($"======= Exp in Hr Employee edit: {ex.Message}"));
            }
        }

        [HttpPost("EditEmployeeContactInfo")]
        public async Task<IActionResult> EditEmployeeContactInfo(EmployeeContactInfoDto obj)
        {
            var chk = await permission.HasPermission(28, PermissionType.Edit);
            if (!chk)
            {
                return BadRequest(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<EmployeeContactInfoDto>.FailAsync($"يجب ادخال كل البيانات بشكل صحيح"));
                }

                var edit = await mainServiceManager.InvestEmployeeService.UpdateContact(obj);
                return Ok(edit);
            }
            catch (Exception ex)
            {
                return Ok(await Result<EmployeeContactInfoDto>.FailAsync($"======= Exp in Hr Employee edit: {ex.Message}"));
            }
        }

        [HttpPost("EditEmployeeAdditionalProps")]
        public async Task<IActionResult> EditEmployeeAdditionalProps(EmployeeAdditionalPropsDto obj)
        {
            var chk = await permission.HasPermission(28, PermissionType.Edit);
            if (!chk)
            {
                return BadRequest(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<EmployeeAdditionalPropsDto>.FailAsync($"يجب ادخال كل البيانات بشكل صحيح"));
                }

                var edit = await mainServiceManager.InvestEmployeeService.UpdateAdditionalProps(obj);
                return Ok(edit);
            }
            catch (Exception ex)
            {
                return Ok(await Result<EmployeeAdditionalPropsDto>.FailAsync($"======= Exp in Hr Employee edit: {ex.Message}"));
            }
        }

        [HttpPost("EditEmployeeContractInfo")]
        public async Task<IActionResult> EditEmployeeContractInfo(EmployeeContractInfoDto obj)
        {
            var chk = await permission.HasPermission(28, PermissionType.Edit);
            if (!chk)
            {
                return BadRequest(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<EmployeeContractInfoDto>.FailAsync($"يجب ادخال كل البيانات بشكل صحيح"));
                }
                var edit = await mainServiceManager.InvestEmployeeService.UpdateContract(obj);
                return Ok(edit);
            }
            catch (Exception ex)
            {
                return Ok(await Result<EmployeeContractInfoDto>.FailAsync($"======= Exp in Hr Employee edit: {ex.Message}"));
            }
        }

        [HttpPost("EditEmployeeDependents")]
        public async Task<IActionResult> EditEmployeeDependents(EmployeeDependentsDto obj)
        {
            var chk = await permission.HasPermission(28, PermissionType.Edit);
            if (!chk)
            {
                return BadRequest(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<EmployeeDependentsDto>.FailAsync($"يجب ادخال كل البيانات بشكل صحيح"));
                }

                var edit = await mainServiceManager.InvestEmployeeService.UpdateDependents(obj);
                return Ok(edit);
            }
            catch (Exception ex)
            {
                return Ok(await Result<EmployeeDependentsDto>.FailAsync($"======= Exp in Hr Employee edit: {ex.Message}"));
            }
        }

        [HttpPost("EditEmployeeFollowers")]
        public async Task<IActionResult> EditEmployeeFollowers(EmployeeFollowersDto obj)
        {
            var chk = await permission.HasPermission(28, PermissionType.Edit);
            if (!chk)
            {
                return BadRequest(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<EmployeeFollowersDto>.FailAsync($"يجب ادخال كل البيانات بشكل صحيح"));
                }

                var edit = await mainServiceManager.InvestEmployeeService.UpdateFollowers(obj);
                return Ok(edit);
            }
            catch (Exception ex)
            {
                return Ok(await Result<EmployeeFollowersDto>.FailAsync($"======= Exp in Hr Employee edit: {ex.Message}"));
            }
        }

        [HttpPost("EditEmployeeJobInfo")]
        public async Task<IActionResult> EditEmployeeJobInfo(EmployeeJobInfoDto obj)
        {
            var chk = await permission.HasPermission(28, PermissionType.Edit);
            if (!chk)
            {
                return BadRequest(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<EmployeeJobInfoDto>.FailAsync($"يجب ادخال كل البيانات بشكل صحيح"));
                }

                var edit = await mainServiceManager.InvestEmployeeService.UpdateJob(obj);
                return Ok(edit);
            }
            catch (Exception ex)
            {
                return Ok(await Result<EmployeeJobInfoDto>.FailAsync($"======= Exp in Hr Employee edit: {ex.Message}"));
            }
        }


        [HttpPost("EditEmployeeMedicalInsurance")]
        public async Task<IActionResult> EditEmployeeMedicalInsurance(EmployeeMedicalInsuranceDto obj)
        {
            var chk = await permission.HasPermission(28, PermissionType.Edit);
            if (!chk)
            {
                return BadRequest(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<EmployeeMedicalInsuranceDto>.FailAsync($"يجب ادخال كل البيانات بشكل صحيح"));
                }

                var edit = await mainServiceManager.InvestEmployeeService.UpdateMedicalInsurance(obj);
                return Ok(edit);
            }
            catch (Exception ex)
            {
                return Ok(await Result<EmployeeMedicalInsuranceDto>.FailAsync($"======= Exp in Hr Employee edit: {ex.Message}"));
            }
        }

        [HttpPost("EditEmployeePreparing")]
        public async Task<IActionResult> EditEmployeePreparing(EmployeePreparingDto obj)
        {
            var chk = await permission.HasPermission(28, PermissionType.Edit);
            if (!chk)
            {
                return BadRequest(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<EmployeePreparingDto>.FailAsync($"يجب ادخال كل البيانات بشكل صحيح"));
                }

                var edit = await mainServiceManager.InvestEmployeeService.UpdatePreparing(obj);
                return Ok(edit);
            }
            catch (Exception ex)
            {
                return Ok(await Result<EmployeePreparingDto>.FailAsync($"======= Exp in Hr Employee edit: {ex.Message}"));
            }
        }


        [HttpPost("EditEmployeeSalaryInfo")]
        public async Task<IActionResult> EditEmployeeSalaryInfo(EmployeeSalaryInfoDto obj)
        {
            var chk = await permission.HasPermission(28, PermissionType.Edit);
            if (!chk)
            {
                return BadRequest(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<EmployeeSalaryInfoDto>.FailAsync($"يجب ادخال كل البيانات بشكل صحيح"));
                }

                var edit = await mainServiceManager.InvestEmployeeService.UpdateSalary(obj);
                return Ok(edit);
            }
            catch (Exception ex)
            {
                return Ok(await Result<EmployeeSalaryInfoDto>.FailAsync($"======= Exp in Hr Employee edit: {ex.Message}"));
            }
        }


        [HttpPost("EditEmployeeSocialInsurance")]
        public async Task<IActionResult> EditEmployeeSocialInsurance(EmployeeSocialInsuranceDto obj)
        {
            var chk = await permission.HasPermission(28, PermissionType.Edit);
            if (!chk)
            {
                return BadRequest(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<EmployeeSocialInsuranceDto>.FailAsync($"يجب ادخال كل البيانات بشكل صحيح"));
                }

                var edit = await mainServiceManager.InvestEmployeeService.UpdateSocialInsurance(obj);
                return Ok(edit);
            }
            catch (Exception ex)
            {
                return Ok(await Result<EmployeeSocialInsuranceDto>.FailAsync($"======= Exp in Hr Employee edit: {ex.Message}"));
            }
        }

        [HttpPost("EditEmployeeTravelInfo")]
        public async Task<IActionResult> EditEmployeeTravelInfo(EmployeeTravelInfoDto obj)
        {
            var chk = await permission.HasPermission(28, PermissionType.Edit);
            if (!chk)
            {
                return BadRequest(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<EmployeeTravelInfoDto>.FailAsync($"يجب ادخال كل البيانات بشكل صحيح"));
                }

                var edit = await mainServiceManager.InvestEmployeeService.UpdateTravel(obj);
                return Ok(edit);
            }
            catch (Exception ex)
            {
                return Ok(await Result<EmployeeTravelInfoDto>.FailAsync($"======= Exp in Hr Employee edit: {ex.Message}"));
            }
        }


        [HttpPost("EditConnectAccounts")]
        public async Task<IActionResult> EditConnectAccounts(AccountConnectDto obj)
        {
            var chk = await permission.HasPermission(28, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<AccountConnectDto>.FailAsync($"يجب ادخال كل البيانات بشكل صحيح"));
                }
                if (obj.FacilityId <= 0)
                {
                    return Ok(await Result<AccountConnectDto>.FailAsync($"يجب  تحديد الشركة "));
                }
                if (obj.SalaryGroupId <= 0)
                {
                    return Ok(await Result<AccountConnectDto>.FailAsync($"يجب  تحديد مجموعة الرواتب "));
                }
                var edit = await mainServiceManager.InvestEmployeeService.UpdateConnectAccounts(obj);
                return Ok(edit);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccountConnectDto>.FailAsync($"======= Exp in Hr Employee edit: {ex.Message}"));
            }
        }



        [HttpPost("ChangeEmployeeImage")]
        public async Task<IActionResult> ChangeEmployeeImage(ChangeEmployeeImageDto obj)
        {
            var chk = await permission.HasPermission(28, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<object>.FailAsync($"يجب ادخال كل البيانات بشكل صحيح"));
                }
                if (string.IsNullOrEmpty(obj.empCode))
                {
                    return Ok(await Result<object>.FailAsync($"يجب  تحديد الموظف "));
                }

                var edit = await mainServiceManager.InvestEmployeeService.ChangeEmployeeImage(obj);
                return Ok(edit);
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"======= Exp in Hr Employee edit: {ex.Message}"));
            }
        }














        #endregion



        [HttpPost("AddEmployeeFast")]
        public async Task<IActionResult> FastAdd(EmployeeFastAddDto obj)
        {
            var chk = await permission.HasPermission(28, PermissionType.Add);
            if (!chk)
            {
                return BadRequest(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<EmployeeFastAddDto>.FailAsync($"يجب ادخال كل البيانات بشكل صحيح"));
                }

                //chek if not auto numbering
                if (obj.AutoNumbering == false && string.IsNullOrEmpty(obj.EmpId))
                {
                    return Ok(await Result<EmployeeFastAddDto>.FailAsync($"ادخل رقم الموظف"));
                }

                var add = await mainServiceManager.InvestEmployeeService.FastAdd(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<EmployeeFastAddDto>.FailAsync($"======= Exp in Hr Employee fast add: {ex.Message}"));
            }
        }

        [HttpPost("DeleteEmployee")]
        public async Task<IActionResult> DeleteEmployee(long Id)
        {
            var chk = await permission.HasPermission(28, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (Id <= 0)
            {
                return Ok(await Result<object>.FailAsync($"Emp Id Is Required"));

            }
            try
            {
                var del = await mainServiceManager.InvestEmployeeService.Remove(Id);
                return Ok(del);
            }
            catch (Exception ex)
            {
                return Ok(await Result<EmployeeFastAddDto>.FailAsync($"======= Exp in Hr Employee DeleteEmployee: {ex.Message}"));
            }
        }

        [HttpPost("AddEmployee")]
        public async Task<IActionResult> Add(InvestEmployeeAddDto obj)
        {
            var chk = await permission.HasPermission(28, PermissionType.Add);
            if (!chk)
            {
                return BadRequest(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<InvestEmployeeDto>.FailAsync(localization.GetMessagesResource("dataRequire")));
                }



                var add = await mainServiceManager.InvestEmployeeService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<InvestEmployeeDto>.FailAsync($"======= Exp in Hr Employee fast add: {ex.Message}"));
            }
        }


        [HttpGet("GetByIdForAddCandidate")]
        public async Task<IActionResult> GetByIdForAddCandidate(long Id)
        {
            try
            {
                var chk = await permission.HasPermission(28, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var item = await hrServiceManager.HrRecruitmentCandidateService.GetOneVW(x => x.IsDeleted == false && x.Id == Id);
                return Ok(item);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrCostTypeEditDto>.FailAsync($"====== Exp in cost type getById, MESSAGE: {ex.Message}"));
            }
        }

    }
}
