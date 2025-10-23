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
using Result = Logix.Application.Wrapper.Result;

namespace Logix.MVC.LogixAPIs.HR
{

    //  استحقاق التأمينات
    public class HRGOSIDueController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IMainServiceManager mainServiceManager;
        private readonly IAccServiceManager accServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ILocalizationService localization;
        private readonly ICurrentData session;

        public HRGOSIDueController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ILocalizationService localization, ICurrentData session, IMainServiceManager mainServiceManager, IAccServiceManager accServiceManager)
        {
            this.permission = permission;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.session = session;
            this.mainServiceManager = mainServiceManager;
            this.accServiceManager = accServiceManager;
        }

        #region الصفحة الرئيسية

        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrGosiFilterDto filter)
        {
            var hasPermission = await permission.HasPermission(1001, PermissionType.Show);
            if (!hasPermission)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                if (filter.FinancelYear <= 0 || filter.FinancelYear == null)
                {
                    return Ok(await Result.FailAsync(localization.GetMessagesResource("FinancialYearMustBeSpecified")));
                }

                var userBranchIds = session.Branches.Split(',');
                filter.Month ??= 0;
                filter.BranchId ??= 0;
                filter.FinancelYear ??= 0;

                List<HrGosiFilterDto> gosiResultList = new List<HrGosiFilterDto>();

                var gosiDataResponse = await hrServiceManager.HrGosiService.GetAllVW(x => x.IsDeleted == false
                    && (filter.Month == 0 || x.TMonth == filter.Month.ToString())
                    && (filter.BranchId != 0 ? x.BranchId == filter.BranchId : userBranchIds.Contains(x.BranchId.ToString()))
                    && (filter.FinancelYear == 0 || x.FinancelYear == filter.FinancelYear)
                );

                if (gosiDataResponse.Succeeded)
                {
                    if (gosiDataResponse.Data.Any())
                    {
                        var gosiEmployeeResponse = await hrServiceManager.HrGosiEmployeeService.GetAll(e => e.IsDeleted == false);

                        if (gosiEmployeeResponse.Succeeded && gosiDataResponse.Data.Any())
                        {
                            foreach (var gosiRecord in gosiDataResponse.Data)
                            {
                                var relatedEmployees = gosiEmployeeResponse.Data.Where(x => x.GosiId == gosiRecord.Id);

                                var companyContribution = relatedEmployees.Select(x => x.GosiCompany).Sum() ?? 0;
                                var employeeContribution = relatedEmployees.Select(x => x.GosiEmp).Sum() ?? 0;

                                var dto = new HrGosiFilterDto
                                {
                                    ID = gosiRecord.Id,
                                    TDate = gosiRecord.TDate,
                                    TMonth = gosiRecord.TMonth,
                                    GosiCompany = companyContribution,
                                    GosiEmp = employeeContribution,
                                    FinancelYear = gosiRecord.FinancelYear,
                                    Facility_Name = gosiRecord.FacilityName,
                                    Total = employeeContribution + companyContribution
                                };

                                gosiResultList.Add(dto);
                            }

                            var orderedResult = gosiResultList.OrderByDescending(x => x.ID).ToList();
                            return Ok(await Result<List<HrGosiFilterDto>>.SuccessAsync(orderedResult, ""));
                        }

                        return Ok(await Result<List<HrGosiFilterDto>>.SuccessAsync(gosiResultList, localization.GetResource1("NosearchResult")));
                    }

                    return Ok(await Result<List<HrGosiFilterDto>>.SuccessAsync(gosiResultList, localization.GetResource1("NosearchResult")));
                }

                return Ok(await Result<HrGosiFilterDto>.FailAsync(gosiDataResponse.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrGosiFilterDto>.FailAsync(ex.Message));
            }
        }


        [HttpPost("GetPagination")]
        public async Task<IActionResult> GetPagination([FromBody] HrGosiFilterDto filter, int take = Pagination.take, long? lastSeenId = null)
        {
            var chk = await permission.HasPermission(1001, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {

                if (filter.FinancelYear <= 0 || filter.FinancelYear == null)
                {
                    return Ok(await Result.FailAsync(localization.GetMessagesResource("FinancialYearMustBeSpecified")));
                }

                var userBranchIds = session.Branches.Split(',');
                filter.Month ??= 0;
                filter.BranchId ??= 0;
                filter.FinancelYear ??= 0;

                List<HrGosiFilterDto> gosiResultList = new List<HrGosiFilterDto>();

                var gosiDataResponse = await hrServiceManager.HrGosiService.GetAllWithPaginationVW(selector: x => x.Id,
                expression: x=> x.IsDeleted == false
                    && (filter.Month == 0 || x.TMonth == filter.Month.ToString())
                    && (filter.BranchId != 0 ? x.BranchId == filter.BranchId : userBranchIds.Contains(x.BranchId.ToString()))
                    && (filter.FinancelYear == 0 || x.FinancelYear == filter.FinancelYear),
                    take: take,
                    lastSeenId: lastSeenId);

                if (!gosiDataResponse.Succeeded)
                    return Ok(await Result<List<HrGosiVw>>.FailAsync(gosiDataResponse.Status.message));


                if (gosiDataResponse.Succeeded)
                {
                    if (gosiDataResponse.Data.Any())
                    {
                        var gosiEmployeeResponse = await hrServiceManager.HrGosiEmployeeService.GetAll(e => e.IsDeleted == false);

                        if (gosiEmployeeResponse.Succeeded && gosiDataResponse.Data.Any())
                        {
                            foreach (var gosiRecord in gosiDataResponse.Data)
                            {
                                var relatedEmployees = gosiEmployeeResponse.Data.Where(x => x.GosiId == gosiRecord.Id);

                                var companyContribution = relatedEmployees.Select(x => x.GosiCompany).Sum() ?? 0;
                                var employeeContribution = relatedEmployees.Select(x => x.GosiEmp).Sum() ?? 0;

                                var dto = new HrGosiFilterDto
                                {
                                    ID = gosiRecord.Id,
                                    TDate = gosiRecord.TDate,
                                    TMonth = gosiRecord.TMonth,
                                    GosiCompany = companyContribution,
                                    GosiEmp = employeeContribution,
                                    FinancelYear = gosiRecord.FinancelYear,
                                    Facility_Name = gosiRecord.FacilityName,
                                    Total = employeeContribution + companyContribution
                                };

                                gosiResultList.Add(dto);
                            }

                        }
                    }
                }

                var orderedResult = gosiResultList.OrderByDescending(x => x.ID).ToList();
                var paginatedData = new PaginatedResult<object>
                {
                    Succeeded = gosiDataResponse.Succeeded,
                    Data = orderedResult,
                    Status = gosiDataResponse.Status,
                    PaginationInfo = gosiDataResponse.PaginationInfo
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
            var chk = await permission.HasPermission(1001, PermissionType.Delete);
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
                var del = await hrServiceManager.HrGosiService.Remove(Id);

                return Ok(del);
            }
            catch (Exception exp)
            {
                return Ok(await Result.FailAsync($"{exp.Message}"));
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long Id)
        {
            var chk = await permission.HasPermission(1001, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (Id <= 0) return Ok(await Result.FailAsync(localization.GetMessagesResource("NoIdInUpdate")));


            try
            {
                var GosiData = await hrServiceManager.HrGosiService.GetOne(x => x.Id == Id && x.IsDeleted == false);
                if (GosiData.Data == null)
                {
                    return Ok(await Result<object>.FailAsync(localization.GetMessagesResource("NoItemFoundToEdit")));
                }

                var TxtJCode = string.Empty;
                var fileDtos = new List<SaveFileDto>();

                //  Get JCode By ReferenceNo
                var getJCode = await accServiceManager.AccJournalMasterService.GetOne(x => x.ReferenceNo == Id && x.FlagDelete == false && x.DocTypeId == 37);
                if (getJCode.Data != null)
                {
                    TxtJCode = getJCode.Data.JCode;
                }

                var GosiEmployeeData = await hrServiceManager.HrGosiEmployeeService.GetAllVW(x => x.GosiId == Id && x.IsDeleted == false);
                if (GosiData.Data != null)
                {
                    var getFiles = await mainServiceManager.SysFileService.GetFilesForUser(Id, 84);
                    fileDtos = getFiles.Data ?? new List<SaveFileDto>();
                    return Ok(await Result<object>.SuccessAsync(new { jCode = TxtJCode, GosiData = GosiData, GosiEmployeeData = GosiEmployeeData, Files = fileDtos }));
                }
                return Ok(await Result<object>.FailAsync(localization.GetMessagesResource("NoItemFoundToEdit")));
            }
            catch (Exception exp)
            {
                return Ok(await Result<object>.FailAsync($"{exp.Message}"));
            }
        }

        #endregion

        #region صفحة التعديل

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(HrGosiEditDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(1001, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result<string>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var update = await hrServiceManager.HrGosiService.UpdateGosiEmployee(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrLeaveEditDto>.FailAsync($"====== Exp in  HRGOSIDueController  Edit Method, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("CreateDue")]
        public async Task<ActionResult> CreateDue(HrCreateDueDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(1001, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result<object>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));
                if (obj.Id <= 0)
                    return Ok(await Result<object>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));


                if (obj.tMonth <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetMessagesResource("MonthMustBeSpecified")));

                if (obj.FinancelYear <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetMessagesResource("FinancialYearMustBeSpecified")));

                if (string.IsNullOrEmpty(obj.tDate))
                    return Ok(await Result<object>.FailAsync(localization.GetMessagesResource("DateMustBeSpecified")));

                var update = await hrServiceManager.HrGosiService.CreateDue(obj);
                if (update.Succeeded)
                {
                    var send = await mainServiceManager.SysWebHookService.SendToWebHook(update.Data.JId.ToString(), 1001, session.UserId, session.FacilityId, ProcessType.Added);
                }
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"====== Exp in Edit HR Gois Due  Controller, MESSAGE: {ex.Message}"));
            }
        }
        #endregion

        #region صفحة الاضافة
        [HttpPost("SearchOnAdd")]
        public async Task<IActionResult> SearchOnAdd(EmployeeGosiSearchtDto filter)
        {
            var chk = await permission.HasPermission(1001, PermissionType.Add);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                filter.BranchId ??= 0;
                filter.SalaryGroupId ??= 0;
                filter.DeptId ??= 0;
                filter.Location ??= 0;
                filter.FacilityId = session.FacilityId;
                filter.CmdType = 16;

                if (filter.BranchId != 0)
                {
                    filter.BranchsId = "";
                }
                else
                {
                    filter.BranchsId = session.Branches;
                }

                if (string.IsNullOrEmpty(filter.EmpCode))
                {
                    filter.EmpCode = null;
                }
                if (string.IsNullOrEmpty(filter.EmpName))
                {
                    filter.EmpName = null;
                }

                if (filter.TMonth <= 0)
                {
                    return Ok(await Result<object>.FailAsync(localization.GetMessagesResource("MonthMustBeSpecified")));
                }
                if (filter.FinancelYear <= 0)
                {
                    return Ok(await Result<object>.FailAsync(localization.GetMessagesResource("FinancialYearMustBeSpecified")));
                }

                if (filter.TMonth < 10)
                {
                    filter.MonthCode = "0" + filter.TMonth.ToString();
                }
                else
                {
                    filter.MonthCode = filter.TMonth.ToString();
                }


                var search = await hrServiceManager.HrGosiService.getEmployeeData(filter);
                return Ok(search);

            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync(ex.Message));
            }
        }

        [HttpPost("Add")]
        public async Task<ActionResult> Add(HrGosiAddDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(1001, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result<object>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));
                if (obj.GosiEmp.Count < 1)
                {
                    return Ok(await Result.FailAsync("there is no data"));

                }
                if (obj.GosiEmp.Any(e => string.IsNullOrEmpty(e.emp_ID)))
                {
                    return Ok(await Result.FailAsync("أرقم الموظفين مطلوبة"));

                }
                obj.FinancelYear ??= 0;
                obj.TMonth ??= 0;
                if (obj.TMonth <= 0) return Ok(await Result<object>.FailAsync("يجب تحديد الشهر"));

                if (obj.FinancelYear <= 0) return Ok(await Result<object>.FailAsync("يجب تحديد السنة المالية"));
                if (obj.FinancelYear > DateTime.Now.Year) return Ok(await Result<object>.FailAsync("السنة المالية غير معروفة"));
                if (string.IsNullOrEmpty(obj.TDate)) return Ok(await Result<object>.FailAsync("يجب تحديد التاريخ"));

                var add = await hrServiceManager.HrGosiService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"====== Exp in Add HRGOSIDueController Controller  Add Method, MESSAGE: {ex.Message}"));
            }
        }
        #endregion


    }
}
