using Logix.Application.Common;
using Logix.Application.DTOs.WF;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NuGet.Packaging;

namespace Logix.MVC.LogixAPIs.WF
{
    public class WfAppCommitteeController : BaseWfController
    {
        private readonly IWFServiceManager wfServiceManager;
        private readonly IMainServiceManager mainServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ILocalizationService localization;
        private readonly ICurrentData session;
        private readonly IApiDDLHelper ddlHelper;

        public WfAppCommitteeController(IWFServiceManager wfServiceManager,
            IMainServiceManager mainServiceManager,
            IPermissionHelper permission,
            ILocalizationService localization,
            ICurrentData session,
            IApiDDLHelper ddlHelper)
        {
            this.wfServiceManager = wfServiceManager;
            this.mainServiceManager = mainServiceManager;
            this.permission = permission;
            this.localization = localization;
            this.session = session;
            this.ddlHelper = ddlHelper;
        }

        [HttpPost("Search")]
        public async Task<ActionResult> Search(WfAppCommitteeFilterDto filter)
        {
            try
            {
                var chk = await permission.HasPermission(2207, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                filter.Status ??= 0; // null or 0 => all, 1 => active, 2 => inActive.
                bool isActive = filter.Status == 1;
                var items = await wfServiceManager.WfAppCommitteeService.GetAllVW(x => x.IsDeleted == false
                && (filter.Status == 0 || x.Isactive == isActive)
                && (string.IsNullOrEmpty(filter.Name) || (!string.IsNullOrEmpty(x.Name) && x.Name.Contains(filter.Name)) || (!string.IsNullOrEmpty(x.Name2) && x.Name2.Contains(filter.Name)))
                && (string.IsNullOrEmpty(filter.EmpName) || (!string.IsNullOrEmpty(x.EmpName) && x.EmpName.Contains(filter.EmpName)) || (!string.IsNullOrEmpty(x.EmpName2) && x.EmpName2.Contains(filter.EmpName))));

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetMembers")]
        public async Task<ActionResult> GetMembers(string employeesId)
        {
            try
            {
                var empIdsArray = employeesId.Split(',');
                var branchIdsArray = session.Branches.Split(',');
                var items = await mainServiceManager.InvestEmployeeService.GetAll(x => x.IsDeleted == false && x.StatusId == 1
                && x.FacilityId == session.FacilityId
                && empIdsArray.Contains(x.Id.ToString())
                && branchIdsArray.Contains(x.BranchId.ToString()));

                if (items.Succeeded)
                {
                    List<WfAppCommitteesMemberDto> members = new();
                    foreach (var item in items.Data)
                    {
                        members.Add(new WfAppCommitteesMemberDto()
                        {
                            Id = 0,
                            CommitteeId = 0,
                            EmpId = item.Id,
                            EmpCode = item.EmpId,
                            EmpName = session.Language == 1 ? item.EmpName : item.EmpName2,
                            MemberType = null,
                            IsBoss = false,
                            IsDeleted = false
                        });
                    }

                    return Ok(await Result<List<WfAppCommitteesMemberDto>>.SuccessAsync(members));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("DDLEmployees")]
        public async Task<IActionResult> DDLEmployees()
        {
            try
            {
                var list = new SelectList(new List<DDListItem<Logix.Domain.Main.InvestEmployee>>());
                var branchesId = session.Branches.Split(',');

                list = await ddlHelper.GetAnyLis<Logix.Domain.Main.InvestEmployee, long>(x => x.IsDeleted == false && x.StatusId == 1
                && x.FacilityId == session.FacilityId && branchesId.Contains(x.BranchId.ToString()),
                "Id", session.Language == 1 ? "EmpName" : "EmpName2");

                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpPost("Add")]
        public async Task<ActionResult> Add(WfAppCommitteeAddDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(2207, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                obj.FormationDecision ??= "";
                var add = await wfServiceManager.WfAppCommitteeService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }


        [HttpGet("GetByIdForEdit")]
        public async Task<IActionResult> GetByIdForEdit(long id)
        {
            try
            {
                var chk = await permission.HasPermission(2207, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));

                var item = await wfServiceManager.WfAppCommitteeService.GetForUpdate<WfAppCommitteeEditDto>(id);
                if (item.Succeeded && item.Data != null)
                {
                    // get emp code
                    var getEmpCode = await mainServiceManager.InvestEmployeeService.GetOne(x => x.EmpId, x => x.Id == item.Data.EmpId);
                    item.Data.EmpCode = getEmpCode.Data;

                    // get members
                    var getMembers = await wfServiceManager.WfAppCommitteesMemberService.GetAll(x => x.CommitteeId == item.Data.Id && x.IsDeleted == false);
                    foreach (var member in getMembers.Data)
                    {
                        // get empCode and empName
                        var getEmployee = await mainServiceManager.InvestEmployeeService.GetOne(x => x.Id == member.EmpId);
                        member.EmpCode = getEmployee.Data.EmpId;
                        member.EmpName = session.Language == 1 ? getEmployee.Data.EmpName : getEmployee.Data.EmpName2;
                    }
                    item.Data.MembersDto = getMembers.Data.ToList();

                    // get files
                    var getFiles = await mainServiceManager.SysFileService.GetFilesForUser(item.Data.Id, 146);
                    item.Data.FilesDto = getFiles.Data;
                }
                return Ok(item);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(WfAppCommitteeEditDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(2207, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                obj.FormationDecision ??= "";
                var update = await wfServiceManager.WfAppCommitteeService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }


        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var chk = await permission.HasPermission(2207, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await wfServiceManager.WfAppCommitteeService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }

    }
}
