using Logix.Application.Common;
using Logix.Application.DTOs.PM;
using Logix.Application.DTOs.PM.PmProjectsStaff;
using Logix.Application.DTOs.PM.Shared;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;

namespace Logix.MVC.LogixAPIs.PM
{
    ///  إسناد فريق عمل المشاريع

    public class PMEmployeesAssigningController : BasePMApiController
    {
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IPMServiceManager pMServiceManager;



        public PMEmployeesAssigningController(IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IPMServiceManager pMServiceManager)
        {
            this.permission = permission;
            this.session = session;
            this.localization = localization;
            this.pMServiceManager = pMServiceManager;
        }

        #region Index Page

        [HttpPost("Search")]
        public async Task<IActionResult> Search(PaginatedRequest<BindProjectsDto> request)
        {

            try
            {
                var filter = request.Filter;

                var chk = await permission.HasPermission(1897, PermissionType.Show);

                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                filter.Code ??= 0;
                filter.Code2 ??= 0;
                filter.BranchId ??= 0;
                filter.TenderStatus ??= 0;
                filter.ParentType ??= 0;
                filter.ProjectType ??= 0;
                filter.Type ??= 0;
                filter.AmountFrom ??= 0;
                filter.AmountTo ??= 0;
                filter.SystemId = 5;
                filter.EmpId ??= 0;

                if (filter.ProjectType == 0)
                    filter.Type = 0;
                filter.OwnerDeptId ??= 0;
                filter.PaymentType ??= 0;
                filter.ProjectValue ??= 0;
                filter.Iscase ??= 0;
                filter.OwnerDeptId ??= 0;
                filter.StatusId ??= 0;
                filter.Iscase = 0;
                filter.Isletter = false;
                filter.IsActive = false;

                if (string.IsNullOrEmpty(filter.From) || string.IsNullOrEmpty(filter.To))
                {
                    filter.From = "";
                    filter.To = "";
                }
                //   في حال كان موظف مشاريع تظهر فقط المنافسة الخاصة به

                if (session.SalesType == 2)
                    filter.EmpId = session.EmpId;

                if (filter.IsSubContract == true)
                {
                    filter.ParentType = 2;
                }


                var result = await pMServiceManager.PMProjectsService.BindProjects(filter);
                if (request.PageNumber > 0 && request.PageSize > 0)
                {
                    var paginatedData = await PaginatedResult<object>.SuccessAsync(
                        result.Data.ToList(),
                        request.PageNumber,
                        request.PageSize);
                    return Ok(paginatedData);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"======= Exp in {this.GetType()}: {ex.Message}"));
            }
        }

        [HttpPost("Assign")]
        public async Task<IActionResult> Assign(EmployeeAssignDto obj)
        {

            try
            {

                var chk = await permission.HasPermission(1897, PermissionType.Show);

                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (obj.EmpId <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetHrResource("emp")));

                if (obj.ProjectsId.Count() <= 0)
                    return Ok(await Result<object>.FailAsync($"{localization.GetPMResource("ProjectNo")}"));


                var result = await pMServiceManager.PMProjectsStaffService.AssignEmployeeToProjecy(obj);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"Error in {this.GetType().Name}.{nameof(Assign)}: {ex.Message}"));

            }

        }
        [HttpPost("UnAssign")]
        public async Task<IActionResult> UnAssign(EmployeeAssignDto obj)
        {

            try
            {

                var chk = await permission.HasPermission(1897, PermissionType.Show);

                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (obj.EmpId <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetHrResource("emp")));

                if (obj.ProjectsId.Count() <= 0)
                    return Ok(await Result<object>.FailAsync($"{localization.GetPMResource("ProjectNo")}"));


                var result = await pMServiceManager.PMProjectsStaffService.UnAssignEmployeeToProjecy(obj);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"Error in {this.GetType().Name}.{nameof(UnAssign)}: {ex.Message}"));

            }

        }


        #endregion


    }
}
