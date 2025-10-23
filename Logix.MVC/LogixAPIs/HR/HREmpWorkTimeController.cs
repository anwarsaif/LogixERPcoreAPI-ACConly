using AutoMapper;
using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.DTOs.Main;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.Infrastructure.Repositories;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Logix.MVC.LogixAPIs.HR
{

    //   مواعيد الدوام
    public class HREmpWorkTimeController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IMainServiceManager mainServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IMapper mapper;



        public HREmpWorkTimeController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IMapper mapper, IMainServiceManager mainServiceManager)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.mapper = mapper;
            this.mainServiceManager = mainServiceManager;
        }


        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrAttShiftEmployeeMFilterDto filter)
        {
            var chk = await permission.HasPermission(188, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var BranchesList = session.Branches.Split(',');
                if (filter.DDNonAssignedEmployee == 1)
                {


                    var items = await hrServiceManager.HrAttShiftEmployeeMVwService.GetAllVW(e => e.IsDeleted == false && BranchesList.Contains(e.BranchId.ToString()));
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

                            if (filter.BranchId != null && filter.BranchId > 0)
                            {
                                res = res.Where(c => c.BranchId != null && c.BranchId.Equals(filter.BranchId));
                            }
                            if (filter.DeptId != null && filter.DeptId > 0)
                            {
                                res = res.Where(c => c.DeptId != null && c.DeptId.Equals(filter.DeptId));
                            }
                            if (filter.FacilityId != null && filter.FacilityId > 0)
                            {
                                res = res.Where(c => c.FacilityId != null && c.FacilityId.Equals(filter.FacilityId));
                            }
                            if (filter.ShitId != null && filter.ShitId > 0)
                            {
                                res = res.Where(c => c.ShitId != null && c.ShitId.Equals(filter.ShitId));
                            }

                            if (res.Any())
                            {
                                var returnItem = mapper.Map<List<HrAttShiftEmployeeMFilterDto>>(res);
                                return Ok(await Result<List<HrAttShiftEmployeeMFilterDto>>.SuccessAsync(returnItem, ""));

                            }
                            return Ok(await Result<List<HrAttShiftEmployeeMVw>>.SuccessAsync(res.ToList(), localization.GetResource1("NosearchResult")));
                        }
                        return Ok(await Result<List<HrAttShiftEmployeeMVw>>.SuccessAsync(items.Data.ToList(), localization.GetResource1("NosearchResult")));
                    }
                    return Ok(await Result<HrAttShiftEmployeeMVw>.FailAsync(items.Status.message));
                }
               
                
                else if (filter.DDNonAssignedEmployee == 2)
                {

                    var getFromHrAttShiftEmployees = await hrServiceManager.HrAttShiftEmployeeService.GetAll(x => x.IsDeleted == false && (filter.ShitId == 0 || x.ShitId == filter.ShitId));
                    var HrAttShiftEmployeesIdsList = getFromHrAttShiftEmployees.Data.Select(x => x.EmpId).ToList();

                    var items = await hrServiceManager.HrEmployeeService.GetAllVW(e => e.IsDeleted == false && e.Isdel == false && e.StatusId == 1 && !HrAttShiftEmployeesIdsList.Contains(e.Id));
                    if (items.Succeeded)
                    {
                        if (items.Data.Count() > 0)
                        {

                            var res = items.Data.AsQueryable();
                            if (!string.IsNullOrEmpty(filter.EmpCode))
                            {
                                res = res.Where(c => c.EmpId != null && c.EmpId == filter.EmpCode);
                            }
                            if (!string.IsNullOrEmpty(filter.EmpName))
                            {
                                res = res.Where(c => (c.EmpName != null && c.EmpName.Contains(filter.EmpName)));
                            }

                            if (filter.BranchId != null && filter.BranchId > 0)
                            {
                                res = res.Where(c => c.BranchId != null && c.BranchId == filter.BranchId);
                            }
                            if (filter.DeptId != null && filter.DeptId > 0)
                            {
                                res = res.Where(c => c.DeptId != null && c.DeptId == filter.DeptId);
                            }
                            if (filter.FacilityId != null && filter.FacilityId > 0)
                            {
                                res = res.Where(c => c.FacilityId != null && c.FacilityId == filter.FacilityId);
                            }

                            if (res.Count() > 0)
                            {
                                return Ok(await Result<List<HrEmployeeVw>>.SuccessAsync(res.ToList(), ""));

                            }
                            return Ok(await Result<List<HrEmployeeVw>>.SuccessAsync(res.ToList(), localization.GetResource1("NosearchResult")));
                        }
                        return Ok(await Result<List<HrEmployeeVw>>.SuccessAsync(items.Data.ToList(), localization.GetResource1("NosearchResult")));
                    }
                    return Ok(await Result<HrEmployeeVw>.FailAsync(items.Status.message));
                }
                else
                {
                    return Ok(await Result<List<HrEmployeeVw>>.FailAsync("يجب تحديد حالة الموظفين "));
                }

            }
            catch (Exception ex)
            {
                return Ok(await Result<HrAttShiftEmployeeMVw>.FailAsync(ex.Message));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var chk = await permission.HasPermission(188, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await hrServiceManager.HrAttShiftEmployeeService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete Hr HrAttShiftEmployee Controller, MESSAGE: {ex.Message}"));
            }
        }
    }
}