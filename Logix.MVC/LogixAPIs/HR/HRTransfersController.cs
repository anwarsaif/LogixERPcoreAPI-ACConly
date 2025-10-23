using DevExpress.CodeParser;
using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.DTOs.Main;
using Logix.Application.Helpers;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.HR.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Result = Logix.Application.Wrapper.Result;

namespace Logix.MVC.LogixAPIs.HR
{
    //  التنقلات
    public class HRTransfersController : BaseHrApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IApiDDLHelper ddlHelper;


        public HRTransfersController(IHrServiceManager hrServiceManager, IMainServiceManager mainServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IApiDDLHelper ddlHelper)
        {
            this.mainServiceManager = mainServiceManager;
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.ddlHelper = ddlHelper;
        }


        #region دوال الشاشة الرئيسية 

        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrTransferFilterDto filter)
        {
            var chk = await permission.HasPermission(494, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied(localization.GetMessagesResource("AccessDenied")));
            }
            try
            {
                var items = await hrServiceManager.HrTransferService.Search(filter);

				return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync(ex.Message));
            }
        }


        [HttpPost("GetPagination")]
        public async Task<IActionResult> GetPagination([FromBody] HrTransferFilterDto filter, int take = 5, long? lastSeenId = null)
        {
            try
            {
                var chk = await permission.HasPermission(494, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var BranchesList = session.Branches.Split(',');
                filter.DeptId ??= 0;
                filter.BranchId ??= 0;
                filter.LocationId ??= 0;
                filter.LocationFromId ??= 0;
                filter.LocationToId ??= 0;
                filter.TransDepartmentFrom ??= 0;
                filter.TransDepartmentTo ??= 0;
                filter.BranchFromId ??= 0;
                filter.BranchToId ??= 0;

                var dateConditions = new List<DateCondition>();
                dateConditions.Add(new DateCondition
                {
                    DatePropertyName = "TransferDate",
                    ComparisonOperator = ComparisonOperator.GreaterThanOrEqual,
                    StartDateString = filter.FromDate ?? ""
                });
                dateConditions.Add(new DateCondition
                {
                    DatePropertyName = "TransferDate",
                    ComparisonOperator = ComparisonOperator.LessThanOrEqual,
                    StartDateString = filter.ToDate ?? ""
                });

                var items = await hrServiceManager.HrTransferService.GetAllWithPaginationVW(selector: e => e.Id,
                expression: e => e.IsDeleted == false &&
                (string.IsNullOrEmpty(filter.EmpId) || e.EmpCode == filter.EmpId) &&
                (string.IsNullOrEmpty(filter.EmpName) || e.EmpName == filter.EmpName) &&
                (filter.DeptId == 0 || e.DeptId == filter.DeptId) &&
                (filter.BranchId == 0 || e.BranchId == filter.BranchId || e.BranchId == filter.BranchFromId) &&
                (BranchesList == null || BranchesList.Contains(e.BranchId.ToString())) &&
                (filter.LocationId == 0 || e.Location == filter.LocationId) &&
                (filter.LocationFromId == 0 || e.TransLocationFrom == filter.LocationFromId) &&
                (filter.LocationToId == 0 || e.TransLocationTo == filter.LocationToId) &&
                (filter.TransDepartmentFrom == 0 || e.TransDepartmentFrom == filter.TransDepartmentFrom) &&
                (filter.TransDepartmentTo == 0 || e.TransDepartmentTo == filter.TransDepartmentTo) &&
                (filter.BranchToId == 0 || e.BranchIdTo == filter.BranchToId),
                    take: take,
                    lastSeenId: lastSeenId,
                   dateConditions: (string.IsNullOrEmpty(filter.FromDate) || string.IsNullOrEmpty(filter.ToDate)) ? null : dateConditions);

                if (!items.Succeeded)
                    return Ok(await Result<List<HrTransfersVw>>.FailAsync(items.Status.message));

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
                return Ok(items.Data);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var chk = await permission.HasPermission(494, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied(localization.GetMessagesResource("AccessDenied")));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await hrServiceManager.HrTransferService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete HR Yearly Allowance DeductionController, MESSAGE: {ex.Message}"));
            }
        }

        #endregion


        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long Id)
        {
            try
            {
                var chk = await permission.HasPermission(494, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied(localization.GetMessagesResource("AccessDenied")));

                if (Id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var item = await hrServiceManager.HrTransferService.GetOneVW(x=>x.Id== Id);
                if (item.Succeeded)
                {

                    if (item.Data != null)
                    {
                        return Ok(item);

                    }
                    else
                    {
                        return Ok(await Result.FailAsync("الرقم الخاص بالنقل غير موجود"));

                    }
                }
                return Ok(await Result.FailAsync(item.Status.message));

            }
            catch (Exception ex)
            {
                return Ok(await Result<HrTransferEditDto>.FailAsync($"====== Exp in HRTransfersController getById, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(HrTransferEditDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(494, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied(localization.GetMessagesResource("AccessDenied")));

                if (!ModelState.IsValid)
                    return Ok(await Result<HrTransferEditDto>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var update = await hrServiceManager.HrTransferService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrTransferEditDto>.FailAsync($"====== Exp in Edit HRDependentsController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Add")]
        public async Task<ActionResult> Add(HrTransferDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(494, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied(localization.GetMessagesResource("AccessDenied")));

                if (!ModelState.IsValid )
                    return Ok(await Result<HrTransferDto>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));
                var add = await hrServiceManager.HrTransferService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add HRYTransferController, MESSAGE: {ex.Message}"));
            }
        }

        #region Transfers All اضافة متعددة 
        [HttpPost("TransfersAllSearch")]
        public async Task<IActionResult> TransfersAllSearch(HrTransferSecondFilterDto filter)
        {
            var chk = await permission.HasPermission(494, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied(localization.GetMessagesResource("AccessDenied")));
            }
            if(filter.TransBranchFrom==null || filter.TransBranchFrom <= 0|| filter.TransDepartmentFrom == null || filter.TransDepartmentFrom <= 0||filter.TransBranchFrom==null|| filter.TransBranchFrom<=0)
            {
                return Ok(await Result<HrEmployeeVw>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

            }
            try
            {
                var BranchesList = session.Branches.Split(',');

                var childDepartments = await hrServiceManager.HrTransferService.HRGetchildeDepartmentFn((long)filter.TransDepartmentFrom);
                var childDepartmentList = childDepartments.Data.ToString().Split(',');

                var items = await hrServiceManager.HrEmployeeService.GetAllVW(e => e.Isdel == false && e.StatusId == 1 &&
                (filter.TransBranchFrom != 0 ? e.BranchId == filter.TransBranchFrom : BranchesList.Contains(e.BranchId.ToString())) &&
                (filter.TransDepartmentFrom != 0 && ( e.DeptId == filter.TransDepartmentFrom || childDepartmentList.Contains(e.DeptId.ToString()))) &&
                (filter.TransLocationFrom != 0 && e.Location == filter.TransLocationFrom)
                );


                if (items.Succeeded)
                {
                    if(items.Data.Any())
                            return Ok(await Result<List<HrEmployeeVw>>.SuccessAsync(items.Data.ToList(), ""));
                    else
                                return Ok(await Result<List<HrEmployeeVw>>.SuccessAsync(items.Data.ToList(), localization.GetResource1("NosearchResult")));
                }
                return Ok(await Result<HrEmployeeVw>.FailAsync(items.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrEmployeeVw>.FailAsync(ex.Message));
            }
        }
        
        [HttpPost("TransfersAllAdd")]
        public async Task<ActionResult> TransfersAllAdd(HrTransfersAllAddDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(494, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied(localization.GetMessagesResource("AccessDenied")));

                if (!ModelState.IsValid)
                    return Ok(await Result<HrTransfersAllAddDto>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));
                var add = await hrServiceManager.HrTransferService.AddMultipleTransfers(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add HRYTransferController, MESSAGE: {ex.Message}"));
            }
        }
        #endregion  اضافة نقل مع تغيير الراتب

        #region Add2 

        [HttpPost("TransfersAdd2")]
        public async Task<ActionResult> TransfersAdd2(HrTransfersAdd2Dto obj)
        {
            try
            {
                var chk = await permission.HasPermission(494, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied(localization.GetMessagesResource("AccessDenied")));

                if (!ModelState.IsValid)
                    return Ok(await Result<HrTransfersAdd2Dto>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));
                var add = await hrServiceManager.HrTransferService.Add2Transfers(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add HRTransferController in function TransfersAdd2, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("AddAllowanceOrDeduction")]
        public async Task<IActionResult> AddAllowanceOrDeduction(HrAllowanceDeductionExtraVM vM)
        {
            var chk = await permission.HasPermission(494, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied(localization.GetMessagesResource("AccessDenied")));
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

        [HttpGet("GetEmpDataByEmpId")]
        public async Task<IActionResult> GetEmpDataByEmpId(string? empId)
        {
            var chk = await permission.HasPermission(494, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied(localization.GetMessagesResource("AccessDenied")));
            }
            if (string.IsNullOrEmpty(empId))
            {
                return Ok(await Result.SuccessAsync(""));

            }
            try
            {
                var empData = await hrServiceManager.HrTransferService.GetEmpDataByEmpId(empId);
                return Ok(empData);


            }
            catch (Exception exp)
            {
                return Ok(await Result<object>.FailAsync($"{exp.Message}"));
            }
        }


        #endregion
    }
}