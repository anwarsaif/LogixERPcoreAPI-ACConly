using DevExpress.CodeParser;
using DevExpress.DataProcessing.InMemoryDataProcessor;
using DocumentFormat.OpenXml.Office2010.Excel;
using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Helpers;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.Domain.WF;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.HR.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Operations;
using System;

namespace Logix.MVC.LogixAPIs.HR
{

    // طلبات الموارد البشرية 
    public class HRRequestController : BaseHrApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IWFServiceManager wFServiceManager;

        public HRRequestController(IHrServiceManager hrServiceManager, IMainServiceManager mainServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IWFServiceManager wFServiceManager)
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
        public async Task<IActionResult> Search(HrRequestFilterDto filter)
        {
            var chk = await permission.HasPermission(1414, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var BranchesList = session.Branches.Split(',');
                filter.Id ??= 0;
                filter.BranchId ??= 0;
                filter.StatusId ??= 0;
                filter.ApplicationCode ??= 0;

                var items = await hrServiceManager.HrRequestService.GetAllVW(e => e.IsDeleted == false
                && e.FacilityId == session.FacilityId
                && (filter.Id == 0 || e.Id == filter.Id)
                && (filter.BranchId != 0 ? e.BranchId == filter.BranchId : BranchesList.Contains(e.BranchId.ToString()))
                && (filter.StatusId == 0 || filter.StatusId == e.StatusId)
                && (filter.ApplicationCode == 0 || e.ApplicationCode == filter.ApplicationCode)
                );
                if (items.Succeeded)
                {
                    if (items.Data.Count() > 0)
                    {
                        var res = items.Data.AsQueryable();

                        if (!string.IsNullOrEmpty(filter.DateFrom) && !string.IsNullOrEmpty(filter.DateTo))
                        {
                            var fromDate = DateHelper.StringToDate(filter.DateFrom);
                            var toDate = DateHelper.StringToDate(filter.DateTo);

                            res = res.Where(c =>
                                DateHelper.StringToDate(c.ReDate) >= fromDate &&
                                DateHelper.StringToDate(c.ReDate) <= toDate);
                        }

                        return Ok(await Result<IEnumerable<HrRequestVw>>.SuccessAsync(res));
                    }

                    return Ok(await Result<IEnumerable<HrRequestVw>>.SuccessAsync(items.Data , localization.GetResource1("NosearchResult")));

                }

                return Ok(await Result<IEnumerable<HrRequestVw>>.FailAsync(items.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result<IEnumerable<HrRequestVw>>.FailAsync(ex.Message));
            }
        }


        [HttpPost("GetPagination")]
        public async Task<IActionResult> GetPagination([FromBody] HrRequestFilterDto filter, int take = Pagination.take, long? lastSeenId = null)
        {
            var chk = await permission.HasPermission(1414, PermissionType.Show);
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
                    DatePropertyName = "ReDate",
                    ComparisonOperator = ComparisonOperator.GreaterThanOrEqual,
                    StartDateString = filter.DateFrom ?? ""
                });
                dateConditions.Add(new DateCondition
                {
                    DatePropertyName = "ReDate",
                    ComparisonOperator = ComparisonOperator.LessThanOrEqual,
                    StartDateString = filter.DateTo ?? ""
                });

                filter.Id ??= 0;
                filter.BranchId ??= 0;
                filter.StatusId ??= 0;
                filter.ApplicationCode ??= 0;
                var items = await hrServiceManager.HrRequestService.GetAllWithPaginationVW(selector: e => e.Id,
                expression: e =>
                        e.IsDeleted == false
                && e.FacilityId == session.FacilityId
                && (filter.Id == 0 || e.Id == filter.Id)
                && (filter.BranchId != 0 ? e.BranchId == filter.BranchId : BranchesList.Contains(e.BranchId.ToString()))
                && (filter.StatusId == 0 || filter.StatusId == e.StatusId)
                && (filter.ApplicationCode == 0 || e.ApplicationCode == filter.ApplicationCode),
                    take: take,
                    lastSeenId: lastSeenId,
                   dateConditions: (string.IsNullOrEmpty(filter.DateFrom) || string.IsNullOrEmpty(filter.DateTo)) ? null : dateConditions);

                if (!items.Succeeded)
                    return Ok(await Result<List<HrRequestVw>>.FailAsync(items.Status.message));
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

        [HttpDelete("DeleteRequest")]
        public async Task<IActionResult> DeleteRequest(long Id = 0)
        {
            try
            {
                var chk = await permission.HasPermission(1414, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await hrServiceManager.HrRequestService.Remove(Id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete HrRequestController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long Id = 0)
        {
            var chk = await permission.HasPermission(1414, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result<HrEmployeeDto>.FailAsync($"Access Denied"));
            }
            if (Id <= 0)
            {
                return Ok(await Result<HrEmployeeDto>.FailAsync($"There is No Id"));
            }
            try
            {

                var getItem = await hrServiceManager.HrRequestService.GetOneVW(g => g.IsDeleted == false && g.Id == Id);
                if (getItem.Succeeded && getItem.Data != null)
                {
                    var RequestDetails = await hrServiceManager.HrRequestDetailsService.GetAllVW(g => g.IsDeleted == false && g.RequestId == Id);
                    return Ok(await Result<object>.SuccessAsync(new { request = getItem.Data, details = RequestDetails.Data }));
                }
                return Ok(await Result<object>.FailAsync(getItem.Status.message));
            }
            catch (Exception exp)
            {
                return Ok(await Result<object>.FailAsync($"EXP in {this.GetType()}, Meesage: {exp.Message}"));
            }
        }


        #endregion


        #region AddPage

        [HttpPost("Add")]
        public async Task<IActionResult> Add(HrRequestAddDto obj)
        {
            var chk = await permission.HasPermission(1414, PermissionType.Add);
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
                if (obj.RequestDto.Count() <= 0) return Ok(await Result<object>.FailAsync(localization.GetMessagesResource("AddEmployeesFirst")));
                if (obj.RequestType <= 0) return Ok(await Result<object>.FailAsync(localization.GetMessagesResource("AddRequestType")));
                var addRes = await hrServiceManager.HrRequestService.Add(obj);
                return Ok(addRes);
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync(ex.Message));
            }
        }


        #endregion


        #region EditPage


        [HttpDelete("DeleteRequestDetail")]
        public async Task<IActionResult> DeleteRequestDetail(long Id = 0)
        {
            try
            {
                var chk = await permission.HasPermission(1414, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await hrServiceManager.HrRequestDetailsService.Remove(Id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete HrRequestController, MESSAGE: {ex.Message}"));
            }
        }


        [HttpPost("AddEmployeeOnEdit")]
        public async Task<IActionResult> AddEmployeeOnEdit(HrRequestDetailsDto obj)
        {
            var chk = await permission.HasPermission(1414, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {


                if (obj.RequestType <= 0) return Ok(await Result<object>.FailAsync($"قم بإضافة نوع الطلب "));

                var add = await hrServiceManager.HrRequestDetailsService.Add(obj);
                return Ok(add);
            }
            catch (Exception exp)
            {
                return Ok(await Result<object>.FailAsync($"{exp.Message}"));
            }
        }

        [HttpGet("EmpCodeChanged")]
        public async Task<IActionResult> EmpCodeChanged(string empCode)
        {
            try
            {
                var chk = await permission.HasPermission(1414, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (string.IsNullOrEmpty(empCode))
                    return Ok(await Result.FailAsync($"Data Required"));

                var getData = await hrServiceManager.HrEmployeeService.GetOneVW(x => x.Isdel == false && x.IsDeleted == false && x.EmpId == empCode);
                return Ok(getData);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in EmpCodeChanged HrRequestController, MESSAGE: {ex.Message}"));
            }
        }


        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(HrRequestEditDto obj)
        {
            var chk = await permission.HasPermission(1414, PermissionType.Edit);
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
                if (obj.BranchId <= 0) return Ok(await Result<object>.FailAsync($"يجب تحديد الفرع "));
                if (string.IsNullOrEmpty(obj.Subject)) return Ok(await Result<object>.FailAsync($"يجب ادخال عنوان الطلب "));
                if (string.IsNullOrEmpty(obj.ReDate)) return Ok(await Result<object>.FailAsync($"يجب ادخال تاريخ الطلب "));
                obj.StatusId = 1;
                var addRes = await hrServiceManager.HrRequestService.UpdateRequest(obj);
                return Ok(addRes);
            }

            catch (Exception ex)
            {
                return Ok(await Result<HrCompetenceEditDto>.FailAsync($"{ex.Message}"));
            }
        }
        #endregion





        #region Request_View Page

        [HttpGet("View")]
        public async Task<IActionResult> View(long appId = 0, long Id = 0)
        {
            var chk = await permission.HasPermission(1414, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result<HrEmployeeDto>.FailAsync($"Access Denied"));
            }
            if (Id <= 0)
            {
                return Ok(await Result<HrEmployeeDto>.FailAsync($"There is No Id"));
            }
            try
            {
                var getRequestById = await hrServiceManager.HrRequestService.GetOneVW(g => g.IsDeleted == false && g.Id == Id);
                if (getRequestById.Succeeded && getRequestById.Data != null)
                {
                    IEnumerable<WfApplicationsVw> GetMyApplications = Enumerable.Empty<WfApplicationsVw>();
                    var RequestDetails = await hrServiceManager.HrRequestDetailsService.GetAllVW(g => g.IsDeleted == false && g.RequestId == Id);
                    if (appId > 0)
                    {
                        var getApplicationsResult = await wFServiceManager.WfApplicationService.GetAllVW(x => x.IsDeleted == false && x.CreatedBy == session.UserId && x.Id == appId);
                        if (getApplicationsResult.Succeeded && getApplicationsResult.Data != null)
                        {
                            GetMyApplications = getApplicationsResult.Data;
                        }
                    }
                    return Ok(await Result<object>.SuccessAsync(new { request = getRequestById.Data, details = RequestDetails.Data, applications = GetMyApplications }));
                }
                return Ok(await Result<object>.FailAsync(getRequestById.Status.message));
            }
            catch (Exception exp)
            {
                return Ok(await Result<object>.FailAsync($"EXP in {this.GetType()}, Meesage: {exp.Message}"));
            }
        }

        #endregion
    }
}