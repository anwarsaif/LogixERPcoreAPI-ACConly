using System.ComponentModel.DataAnnotations;
using Logix.Application.Common;
using Logix.Application.DTOs.Main;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.Main
{
    public class EmployeeSearchDto
    {
        public long? Id { get; set; }
        [StringLength(50)]
        public string? EmpId { get; set; }
        [StringLength(250)]
        public string? EmpName { get; set; }
        public int? BranchId { get; set; }
        [StringLength(250)]
        public string? EmpName2 { get; set; }
        public string? BraName { get; set; }
        public string? BraName2 { get; set; }
    }

    public class EmployeeController : BaseMainApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IMainServiceManager mainServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;

        public EmployeeController(IHrServiceManager hrServiceManager,
            IMainServiceManager mainServiceManager,
            IPermissionHelper permission,
            ICurrentData session,
            ILocalizationService localization)
        {
            this.hrServiceManager = hrServiceManager;
            this.mainServiceManager = mainServiceManager;
            this.permission = permission;
            this.session = session;
            this.localization = localization;
        }

        [HttpPost("Search")]
        public async Task<ActionResult> Search(EmployeeSearchDto filter)
        {
            try
            {
                var chk = await permission.HasPermission(959, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var items = await hrServiceManager.HrEmployeeService.GetAllVW(e => e.IsDeleted == false
                && e.IsSub == false
                && (string.IsNullOrEmpty(filter.EmpName) || (!string.IsNullOrEmpty(e.EmpName) && e.EmpName.Contains(filter.EmpName))
                    || (!string.IsNullOrEmpty(e.EmpName) && !string.IsNullOrEmpty(e.EmpName2) && e.EmpName2.Contains(filter.EmpName)))
                && (string.IsNullOrEmpty(filter.EmpId) || (!string.IsNullOrEmpty(e.EmpId) && e.EmpId.Equals(filter.EmpId)))
                && (filter.BranchId == null || (e.BranchId != null && e.BranchId == filter.BranchId))
                );
                if (items.Succeeded)
                {
                    var res = items.Data.OrderBy(e => e.EmpId).AsQueryable();

                    if (!(filter.BranchId > 0))
                    {
                        //get branchsId from session,, this field in sys_use save like that "1,2,5,..."
                        var branchsIds = session.Branches.Split(',');
                        res = res.Where(r => r.BranchId != null && branchsIds.Contains(r.BranchId.ToString()));
                    }

                    var final = res.ToList();
                    List<EmployeeSearchDto> results = new List<EmployeeSearchDto>();
                    foreach (var item in final)
                    {
                        EmployeeSearchDto result = new()
                        {
                            Id = item.Id,
                            EmpId = item.EmpId,
                            EmpName = item.EmpName,
                            EmpName2 = item.EmpName2,
                            BraName = item.BraName,
                            BraName2 = item.BraName2
                        };
                        results.Add(result);
                    }
                    return Ok(await Result<List<EmployeeSearchDto>>.SuccessAsync(results));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Search EmployeeController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Add")]
        public async Task<ActionResult> Add(InvestEmployeeDto2 obj)
        {
            try
            {
                var chk = await permission.HasPermission(959, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                //chek if not auto numbering
                if (obj.AutoNumbering == false && string.IsNullOrEmpty(obj.EmpId))
                {
                    return Ok(await Result.FailAsync($"{localization.GetMainResource("PleaseEnterEmployeeNumFirst")}"));
                }

                var add = await mainServiceManager.InvestEmployeeService.AddFromMainSys(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add EmployeeController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetByIdForEdit")]
        public async Task<IActionResult> GetByIdForEdit(long id)
        {
            try
            {
                var chk = await permission.HasPermission(959, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var item = await mainServiceManager.InvestEmployeeService.GetForUpdate<InvestEmployeeDto2>(id);
                return Ok(item);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in GetByIdForEdit EmployeeController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(InvestEmployeeDto2 obj)
        {
            try
            {
                var chk = await permission.HasPermission(959, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var update = await mainServiceManager.InvestEmployeeService.UpdateFromMainSys(obj);
                return Ok(update);

            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Edit EmployeeController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var chk = await permission.HasPermission(959, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await mainServiceManager.InvestEmployeeService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete EmployeeController, MESSAGE: {ex.Message}"));
            }
        }

    }
}
