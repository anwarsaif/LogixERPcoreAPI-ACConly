using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.DTOs.Main;
using Logix.Application.Helpers;
using Logix.Application.Interfaces.IRepositories;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.Infrastructure.Repositories;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.HR
{
    //الموظفين تحت الاجراء
    public class HREmployeeBendingController : BaseHrApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly IDDListHelper listHelper;
        private readonly ILocalizationService localization;

        public HREmployeeBendingController(IHrServiceManager hrServiceManager, IMainServiceManager mainServiceManager, IPermissionHelper permission, ICurrentData session, IDDListHelper listHelper, ILocalizationService localization)
        {
            this.mainServiceManager = mainServiceManager;
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.listHelper = listHelper;
            this.localization = localization;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var chk = await permission.HasPermission(601, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                List<HrEmployeeBendingVM> EmployeeBendingList = new List<HrEmployeeBendingVM>();

                var BranchesList = session.Branches.Split(',');
                var getAllEmployees = await hrServiceManager.HrEmployeeService.GetAllVW(x => x.Isdel == false && x.IsDeleted == false && x.StatusId == 10 && BranchesList.Contains(x.BranchId.ToString()));
                if (getAllEmployees.Succeeded && getAllEmployees.Data.Any())
                {
                    var acivityItems = await mainServiceManager.SysActivityLogService.GetAll(a => a.TableId == 7);

                    if (acivityItems.Succeeded && acivityItems.Data.Any())
                    {
                        var acivityItemsTablePrimarykey = acivityItems.Data.Select(a => a.TablePrimarykey).ToList();
                        var allEmployee = getAllEmployees.Data.Where(e => acivityItemsTablePrimarykey.Contains(e.Id));

                        if (allEmployee.Any())
                        {
                            foreach (var item in allEmployee)
                            {
                                var hrEmployeeBendingVw = new HrEmployeeBendingVM
                                {
                                    Id = item.Id,
                                    EmpId = item.EmpId,
                                    EmpName = item.EmpName,
                                    EmpName2 = item.EmpName2,
                                    BraName = item.BraName,
                                    ByName = item.ByName,
                                    ContractDate = item.ContractData,
                                    ContractExpiryDate = item.ContractExpiryDate,
                                    IdNo = item.IdNo,
                                    DepName = item.DepName,
                                    LocationName = item.LocationName,
                                    ReasonStatus = item.ReasonStatus,
                                    StatusName = item.StatusName,
                                    theDate = item.CreatedOn == null ? item.ModifiedOn.ToString() : item.CreatedOn.ToString()
                                };
                                EmployeeBendingList.Add(hrEmployeeBendingVw);

                            }
                        }
                    }


                }
                return Ok(await Result<List<HrEmployeeBendingVM>>.SuccessAsync(EmployeeBendingList.ToList()));

            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
        
        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrEmployeeBendingFilterVM filter)
        {
            var chk = await permission.HasPermission(601, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var getAllEmployees = await hrServiceManager.HrEmployeeService.EmployeeBendingSearch(filter);

                return Ok(getAllEmployees);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpPost("GetPagination")]
        public async Task<IActionResult> GetPagination([FromBody] HrEmployeeBendingFilterVM filter, int take = Pagination.take, long? lastSeenId = null)
        {
            var chk = await permission.HasPermission(601, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {

                var EmployeeBendingList = new List<HrEmployeeBendingVM>();
                var BranchesList = session.Branches.Split(',');

                filter.BranchId ??= 0;
                filter.JobTypeId ??= 0;
                filter.DeptId ??= 0;
                filter.NationalityId ??= 0;
                filter.JobCatagoriesId ??= 0;
                filter.SponsorsID ??= 0;
                filter.FacilityId ??= 0;
                filter.LocationProjectId ??= 0;
                var items = await hrServiceManager.HrEmployeeService.GetAllWithPaginationVW(selector: x => x.Id,
                expression: x => x.Isdel == false && x.IsDeleted == false && x.StatusId == 10
                      && (filter.JobTypeId == 0 || x.JobId == filter.JobTypeId)
                      && (filter.BranchId != 0 ? x.BranchId == filter.BranchId : BranchesList.Contains(x.BranchId.ToString()))
                      && (string.IsNullOrEmpty(filter.EmpId) || x.EmpId == filter.EmpId)
                      && (string.IsNullOrEmpty(filter.EmpName) || x.EmpName.ToLower().Contains(filter.EmpName.ToLower()) || x.EmpName2.ToLower().Contains(filter.EmpName.ToLower()))
                      && (filter.NationalityId == 0 || x.NationalityId == filter.NationalityId)
                      && (filter.JobCatagoriesId == 0 || x.JobCatagoriesId == filter.JobCatagoriesId)
                      && (filter.DeptId == 0 || x.DeptId == filter.DeptId)
                      && (string.IsNullOrEmpty(filter.IdNo) || x.IdNo == filter.IdNo)
                      && (filter.LocationProjectId == 0 || x.Location == filter.LocationProjectId)
                      && (filter.SponsorsID == 0 || x.SponsorsId == filter.SponsorsID)
                      && (filter.FacilityId == 0 || x.FacilityId == filter.FacilityId)
                      && (string.IsNullOrEmpty(filter.BorderID) || (x.EntryNo != null && x.EntryNo.ToLower().Contains(filter.BorderID.ToLower())))
                      && (string.IsNullOrEmpty(filter.PassportID) || (x.PassportNo != null && x.PassportNo.ToLower().Contains(filter.PassportID.ToLower()))),
                    take: take,
                    lastSeenId: lastSeenId);

                if (!items.Succeeded)
                    return Ok(await Result<List<HrEmployeeBendingVM>>.FailAsync(items.Status.message));

                if (items == null || !items.Data.Any())
                    return  Ok(await Result<List<HrEmployeeBendingVM>>.SuccessAsync(EmployeeBendingList));

                var acivityItems = await mainServiceManager.SysActivityLogService.GetAll(a => a.TableId == 7);

                var res = items.Data.AsQueryable();

                EmployeeBendingList = res.Select(item => new HrEmployeeBendingVM
                {
                    Id = item.Id,
                    EmpId = item.EmpId,
                    EmpName = item.EmpName,
                    EmpName2 = item.EmpName2,
                    BraName = item.BraName,
                    ByName = item.ByName,
                    ContractDate = item.ContractData,
                    ContractExpiryDate = item.ContractExpiryDate,
                    IdNo = item.IdNo,
                    DepName = item.DepName,
                    LocationName = item.LocationName,
                    ReasonStatus = item.ReasonStatus,
                    StatusName = item.StatusName,
                    theDate = item.CreatedOn == null ? item.ModifiedOn.ToString() : item.CreatedOn.ToString(),
                    ActivityLogId = acivityItems.Equals(item.Id) ? acivityItems.Data.FirstOrDefault(a => a.TablePrimarykey == item.Id).ActivityLogId : null
                }).ToList();
                
                var paginatedData = new PaginatedResult<object>
                {
                    Succeeded = items.Succeeded,
                    Data = EmployeeBendingList.ToList(),
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

        [HttpPost("ChangeStatus")]
        public async Task<IActionResult> ChangeStatus(int StatusId, List<string> employeesId)
        {
            var chk = await permission.HasPermission(601, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (!employeesId.Any() || StatusId <= 0)
            {
                return Ok(Result.SuccessAsync());
            }

            try
            {
                var getItem = await mainServiceManager.InvestEmployeeService.ChangeEmployeesStatus(StatusId, employeesId, null);

                return Ok(getItem);

            }
            catch (Exception exp)
            {
                return Ok(Result<bool>.FailAsync($"EXP in {this.GetType()}, Meesage: {exp.Message}"));
            }
        }
    }
}