using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.DTOs.Main;
using Logix.Application.Interfaces.IRepositories;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.Infrastructure.Repositories;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.HR.ViewModel;
using Microsoft.AspNetCore.Mvc;
using AllowanceVM = Logix.Application.DTOs.HR.AllowanceVM;
using DeductionVM = Logix.Application.DTOs.HR.DeductionVM;

namespace Logix.MVC.LogixAPIs.HR
{
    //   الوظائف الاضافية 
    public class HREmployeeSubController : BaseHrApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IAccServiceManager accServiceManager;

        public HREmployeeSubController(IHrServiceManager hrServiceManager, IMainServiceManager mainServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IAccServiceManager accServiceManager)
        {
            this.mainServiceManager = mainServiceManager;
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.accServiceManager = accServiceManager;
        }

        [HttpPost("Search")]
        public async Task<IActionResult> Search(EmployeeSubFilterDto filter)
        {
            var chk = await permission.HasPermission(763, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var resultList = await hrServiceManager.HrEmployeeService.SearchEmployeeSub(filter);
                if (resultList.Succeeded)
                {
                    return Ok(await Result<List<HrEmployeeVw>>.SuccessAsync(resultList.Data));
                }

                return Ok(await Result<HrEmployeeDto>.FailAsync(resultList.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }


        [HttpPost("GetPagination")]
        public async Task<IActionResult> GetPagination([FromBody] EmployeeSubFilterDto filter, int take = Pagination.take, long? lastSeenId = null)
        {
            var chk = await permission.HasPermission(763, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {

                var BranchesList = session.Branches.Split(',');
                filter.BranchId ??= 0;
                filter.JobType ??= 0;
                filter.DeptId ??= 0;
                filter.NationalityId ??= 0;
                filter.JobCatagoriesId ??= 0;
                filter.StatusId ??= 0;
                filter.LocationId ??= 0;
                filter.SponsorsId ??= 0;
                filter.FacilityId ??= 0;
                filter.ParentId ??= 0;
                // Get child department IDs if DeptId filter is specified
                List<long> childDeptIds = null;
                if (filter.DeptId != 0)
                {
                    var childDeptIdsString = await hrServiceManager.HrEmployeeService.GetchildDepartment(filter.DeptId.Value);
                    if (childDeptIdsString != null)
                    {
                        childDeptIds = childDeptIdsString.Data
                            .Select(long.Parse)
                            .ToList();
                    }
                }

                var items = await hrServiceManager.HrEmployeeService.GetAllWithPaginationVW(selector: e => e.Id,
                expression: e => e.IsDeleted == false && e.IsSub == true
                   && (filter.BranchId != 0 ? e.BranchId == filter.BranchId : BranchesList.Contains(e.BranchId.ToString()))
                   && (filter.JobType == 0 || e.JobType == filter.JobType)
                   && (filter.DeptId == 0 || (e.DeptId == filter.DeptId || (childDeptIds != null && childDeptIds.Contains(e.DeptId.Value))))
                   && (filter.NationalityId == 0 || e.NationalityId == filter.NationalityId)
                   && (filter.JobCatagoriesId == 0 || e.JobCatagoriesId == filter.JobCatagoriesId)
                   && (filter.StatusId == 0 || e.StatusId == filter.StatusId)
                   && (string.IsNullOrEmpty(filter.EmpName) || e.EmpName.ToLower().Contains(filter.EmpName.ToLower()) || e.EmpName2.ToLower().Contains(filter.EmpName.ToLower()))
                   && (string.IsNullOrEmpty(filter.EmpId) || e.EmpId == filter.EmpId)
                   && (string.IsNullOrEmpty(filter.IdNo) || e.IdNo == filter.IdNo)
                   && (string.IsNullOrEmpty(filter.PassportNo) || e.PassportNo == filter.PassportNo)
                   && (string.IsNullOrEmpty(filter.EntryNo) || e.EntryNo == filter.EntryNo)
                   && (filter.LocationId == 0 || e.Location == filter.LocationId)
                   && (filter.SponsorsId == 0 || e.SponsorsId == filter.SponsorsId)
                   && (filter.FacilityId == 0 || e.FacilityId == filter.FacilityId)
                   && (filter.ParentId == 0 || e.ParentId == filter.ParentId)
                   && (string.IsNullOrEmpty(filter.CostCenterCode) || e.CostCenterCode == filter.CostCenterCode)
                   && (string.IsNullOrEmpty(filter.CostCenterName) || filter.CostCenterName.Contains(e.CostCenterName)),
                    take: take,
                    lastSeenId: lastSeenId);

                if (!items.Succeeded)
                    return Ok(await Result<List<HrEmployeeVw>>.FailAsync(items.Status.message));

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

        [HttpPost("Add")]
        public async Task<IActionResult> Add(EmployeeSubDto obj)
        {
            var chk = await permission.HasPermission(763, PermissionType.Add);
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

                var addRes = await mainServiceManager.InvestEmployeeService.AddEmployeeSub(obj);
                return Ok(addRes);

            }
            catch (Exception ex)
            {
                return Ok(await Result<EmployeeSubDto>.FailAsync(ex.Message));
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long Id)
        {
            var chk = await permission.HasPermission(763, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result<HrEmployeeDto>.FailAsync($"Access Denied"));
            }
            if (Id.Equals(null))
            {
                return Ok(await Result<HrEmployeeDto>.FailAsync($"There is No Id"));
            }
            try
            {
                List<AllowanceVM>? allowance = new List<AllowanceVM>();
                List<DeductionVM>? deduction = new List<DeductionVM>();
                var hrEmployeeDto = new EmployeeSubDto();
                long? ShiftId = 0;
                string ParentCode = string.Empty;
                string CostCenterCode = string.Empty;

                var getItem = await hrServiceManager.HrEmployeeService.GetOneVW(g => g.IsDeleted == false && g.Id == Id);
                if (getItem.Succeeded && getItem.Data != null)
                {
                    var getAllowance = await hrServiceManager.HrAllowanceVwService.GetAll(e => e.EmpId == getItem.Data.Id && e.IsDeleted == false && e.TypeId == 1 && e.FixedOrTemporary == 1 && e.Status == true);
                    var getDeduction = await hrServiceManager.HrDeductionVwService.GetAll(e => e.EmpId == getItem.Data.Id && e.IsDeleted == false && e.TypeId == 2 && e.FixedOrTemporary == 1 && e.Status == true);
                    var gethrAttShiftEmp = await hrServiceManager.HrAttShiftEmployeeService.GetOne(e => e.EmpId == getItem.Data.Id);
                    if (getAllowance.Data.Any())
                    {
                        foreach (var item in getAllowance.Data)
                        {
                            var newRecord = new AllowanceVM
                            {
                                AdId = item.AdId,
                                Id = item.Id,
                                AllowanceAmount = item.Amount,
                                AllowanceRate = item.Rate,
                            };
                            allowance.Add(newRecord);
                        }
                    }
                    if (getDeduction.Data.Any())
                    {
                        foreach (var item in getDeduction.Data)
                        {
                            var newRecord = new DeductionVM
                            {
                                AdId = item.AdId,
                                Id = item.Id,
                                DeductionAmount = item.Amount,
                                DeductionRate = item.Rate,
                            };
                            deduction.Add(newRecord);
                        }

                    }
                    if (gethrAttShiftEmp.Succeeded && gethrAttShiftEmp.Data != null)
                    {
                        ShiftId = gethrAttShiftEmp.Data.ShitId;
                    }


                    var getParentCode = await mainServiceManager.InvestEmployeeService.GetOne(g => g.Id == getItem.Data.ParentId);
                    if (getParentCode.Succeeded && getParentCode.Data != null)
                    {
                        ParentCode = getParentCode.Data.EmpId ?? "";

                    }

                    var getCostCenterCode = await accServiceManager.AccCostCenterService.GetOne(e => e.CcId == getItem.Data.CcId && e.FacilityId == session.FacilityId && e.IsActive == true && e.IsDeleted == false);
                    if (getCostCenterCode.Succeeded && getCostCenterCode.Data != null)
                    {
                        CostCenterCode = getCostCenterCode.Data.CostCenterCode ?? "";

                    }
                    var empData = new EmployeeSubDto
                    {
                        allowance = allowance,
                        deduction = deduction,
                        JobID = getItem.Data.EmpId,
                        EmpName = getItem.Data.EmpName,
                        EmpName2 = getItem.Data.EmpName2,
                        IdNo = getItem.Data.IdNo,
                        BranchId = getItem.Data.BranchId,
                        NationalityId = getItem.Data.NationalityId,
                        Salary = getItem.Data.Salary,
                        SalaryGroupId = getItem.Data.SalaryGroupId,
                        BankId = getItem.Data.BankId,
                        Iban = getItem.Data.Iban,
                        JobCatagoriesId = getItem.Data.JobCatagoriesId,
                        Doappointment = getItem.Data.Doappointment,
                        DeptId = getItem.Data.DeptId,
                        Location = getItem.Data.Location,
                        ManagerId = getItem.Data.ManagerId,
                        DailyWorkingHours = getItem.Data.DailyWorkingHours,
                        ContractDate = getItem.Data.ContractData,
                        ContractExpiryDate = getItem.Data.ContractExpiryDate,
                        ShitID = ShiftId,
                        ManagerCode = getItem.Data.ManagerCode,
                        Gender = getItem.Data.Gender,
                        ParentId = getItem.Data.ParentId,
                        ProgramId = getItem.Data.ProgramId,
                        FacilityId = getItem.Data.FacilityId,
                        MaritalStatus = getItem.Data.MaritalStatus,
                        ParentCode = ParentCode,
                        CostCenterCode = CostCenterCode
                    };

                    return Ok(await Result<EmployeeSubDto>.SuccessAsync(empData));
                }
                return Ok(await Result<EmployeeSubDto>.FailAsync(getItem.Status.message));
            }
            catch (Exception exp)
            {
                return Ok(await Result<HrEmployeeDto>.FailAsync($"EXP in {this.GetType()}, Meesage: {exp.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(EmployeeSubDto obj)
        {
            var chk = await permission.HasPermission(763, PermissionType.Edit);
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
                var addRes = await mainServiceManager.InvestEmployeeService.UpdateEmployeeSub(obj);
                return Ok(addRes);

            }

            catch (Exception ex)
            {
                return Ok(await Result<HrCompetenceEditDto>.FailAsync($"{ex.Message}"));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long Id = 0)
        {
            var chk = await permission.HasPermission(763, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (Id <= 0)
            {
                return Ok(await Result<object>.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));
            }

            try
            {
                var del = await mainServiceManager.InvestEmployeeService.Remove(Id);
                if (del.Succeeded)
                {

                    return Ok(await Result<object>.SuccessAsync("Item deleted successfully"));
                }
                return Ok(await Result<object>.FailAsync($"{del.Status.message}"));
            }
            catch (Exception exp)
            {
                return Ok(await Result<object>.FailAsync($"{exp.Message}"));
            }
        }

        [HttpGet("DeleteAllowanceOrDeduction")]
        public async Task<IActionResult> DeleteAllowanceOrDeduction(long id = 0)
        {
            var chk = await permission.HasPermission(763, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var del = await hrServiceManager.HrAllowanceDeductionService.Remove(id);
                return Ok(del);


            }
            catch (Exception exp)
            {
                return Ok(await Result<object>.FailAsync($"{exp.Message}"));
            }
        }


        [HttpPost("AddAllowanceOrDeductionOnEdit")]
        public async Task<IActionResult> AddAllowanceOrDeductionOnEdit(HrAllowanceDeductionExtraVM vM)
        {
            var chk = await permission.HasPermission(763, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var add = await hrServiceManager.HrAllowanceDeductionService.AddOneEdit(vM);
                return Ok(add);
            }
            catch (Exception exp)
            {
                return Ok(await Result<object>.FailAsync($"{exp.Message}"));
            }
        }



        [HttpGet("EmpIdChanged")]
        public async Task<IActionResult> EmpIdChanged(string EmpId)
        {
            var chk = await permission.HasPermission(763, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (string.IsNullOrEmpty(EmpId))
            {
                return Ok(await Result<EmpIdChangedVM>.SuccessAsync("there is no id passed"));
            }

            try
            {
                var checkEmpId = await mainServiceManager.InvestEmployeeService.GetOne(i => i.EmpId == EmpId && i.Isdel == false);
                if (checkEmpId.Succeeded)
                {
                    if (checkEmpId.Data != null)
                    {
                        var item = new EmpIdChangedVM
                        {
                            EmpId = checkEmpId.Data.EmpId,
                            EmpName = checkEmpId.Data.EmpName,
                            BankId = checkEmpId.Data.BankId,
                            BranchId = checkEmpId.Data.BranchId,
                            Gender = checkEmpId.Data.Gender,
                            Iban = checkEmpId.Data.Iban,
                            IdNo = checkEmpId.Data.IdNo,
                            NationalityId = checkEmpId.Data.NationalityId,
                            //Salary = checkEmpId.Data.Salary,
                            //SalaryGroupId = checkEmpId.Data.SalaryGroupId,
                        };
                        return Ok(await Result<EmpIdChangedVM>.SuccessAsync(item));

                    }
                    else
                    {
                        return Ok(await Result<EmpIdChangedVM>.SuccessAsync($"There is No Employee with this Id:  {EmpId}"));

                    }
                }
                return Ok(await Result<EmpIdChangedVM>.FailAsync($"{checkEmpId.Status.message}"));
            }
            catch (Exception exp)
            {
                return Ok(await Result<EmpIdChangedVM>.FailAsync($"{exp.Message}"));
            }
        }

        [HttpGet("EmpManagerIdChanged")]
        public async Task<IActionResult> EmpManagerIdChanged(string EmpId)
        {
            var chk = await permission.HasPermission(763, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (string.IsNullOrEmpty(EmpId))
            {
                return Ok(await Result<EmpManagerIdChangedVM>.SuccessAsync("there is no id passed"));
            }

            try
            {
                var checkEmpId = await mainServiceManager.InvestEmployeeService.GetOne(i => i.EmpId == EmpId && i.Isdel == false);
                if (checkEmpId.Succeeded)
                {
                    if (checkEmpId.Data != null)
                    {
                        var item = new EmpManagerIdChangedVM
                        {
                            EmpId = checkEmpId.Data.EmpId,
                            EmpName = checkEmpId.Data.EmpName,
                        };
                        return Ok(await Result<EmpManagerIdChangedVM>.SuccessAsync(item));

                    }
                    else
                    {
                        return Ok(await Result<EmpManagerIdChangedVM>.SuccessAsync($"There is No Employee of this Id{EmpId}"));

                    }
                }
                return Ok(await Result<EmpManagerIdChangedVM>.FailAsync($"{checkEmpId.Status.message}"));
            }
            catch (Exception exp)
            {
                return Ok(await Result<EmpManagerIdChangedVM>.FailAsync($"{exp.Message}"));
            }
        }
    }
}