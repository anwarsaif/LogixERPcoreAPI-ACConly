using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.HR
{

    // اسناد الموظفين للمجموعات
    public class HROpeningHoursEmployeesController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IApiDDLHelper ddlHelper;


        public HROpeningHoursEmployeesController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IApiDDLHelper ddlHelper)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.ddlHelper = ddlHelper;
        }


        [HttpPost("Add")]
        public async Task<ActionResult> Add(HrAttShiftEmployeeAddDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(551, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));
                if (string.IsNullOrEmpty(obj.BeginDate)) return Ok(await Result.FailAsync($"يجب ادخال من تاريخ"));
                if (string.IsNullOrEmpty(obj.EndDate)) return Ok(await Result.FailAsync($"يجب ادخال الى تاريخ"));
                if (obj.ShitId <= 0) return Ok(await Result.FailAsync($"يجب تحديد المجموعة"));


                var add = await hrServiceManager.HrAttShiftEmployeeService.Assign(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add Hr HROpeningHoursEmployeesController  Controller, MESSAGE: {ex.Message}"));
            }
        }

        // الموظفين المسندين سابقاً
        [HttpPost("Search2")]
        public async Task<IActionResult> Search2(HrAttShiftEmployeeFilterDto filter)
        {
            try
            {
                var chk = await permission.HasPermission(551, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var result = await hrServiceManager.HrAttShiftEmployeeService.Search2(filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in search  HROpeningHoursEmployeesController , MESSAGE: {ex.Message}"));
            }
        }

        // الموظفين المراد اسنادهم للمجموعة
        [HttpPost("Search1")]
        public async Task<IActionResult> Search1(HrAttShiftEmployeeFilterDto filter)
        {
            try
            {
                var chk = await permission.HasPermission(551, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (string.IsNullOrEmpty(filter.BeginDate)) return Ok(await Result.FailAsync($"يجب ادخال من تاريخ"));
                if (string.IsNullOrEmpty(filter.EndDate)) return Ok(await Result.FailAsync($"يجب ادخال الى تاريخ"));
                var result = await hrServiceManager.HrAttShiftEmployeeService.Search1(filter);
                if (result.Data.Count() > 0)
                {
                    var res = result.Data.Select(x=>new
                    {
                        Id=x.Id,
                        EmpId=x.EmpId,  
                        EmpName=x.EmpName  
                    });
                    return Ok(await Result<object>.SuccessAsync(res));
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in search  HROpeningHoursEmployeesController , MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Cancel")]
        public async Task<ActionResult> Cancel(List<long?> entity)
        {
            try
            {
                var chk = await permission.HasPermission(551, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var add = await hrServiceManager.HrAttShiftEmployeeService.Cancel(entity);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add Hr AttLocationEmployee  Controller, MESSAGE: {ex.Message}"));
            }
        }



        [HttpPost("AddUsingExcel")]
        public async Task<ActionResult> AddUsingExcel(IEnumerable<HrAttShiftEmployeeAddFromExcelDto> obj)
        {
            try
            {
                var chk = await permission.HasPermission(551, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));


                var add = await hrServiceManager.HrAttShiftEmployeeService.AssignUsingExcel(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add Hr HROpeningHoursEmployeesController  Controller, MESSAGE: {ex.Message}"));
            }
        }
    }
}