using DevExpress.CodeParser;
using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Helpers;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.HR.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Operations;

namespace Logix.MVC.LogixAPIs.HR
{
    // ملاحظات على الموظفين 
    public class HRNotesController : BaseHrApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;

        public HRNotesController(IHrServiceManager hrServiceManager, IMainServiceManager mainServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization)
        {
            this.mainServiceManager = mainServiceManager;
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
        }

        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrNoteFilterDto filter)
        {
            var chk = await permission.HasPermission(262, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var BranchesList = session.Branches.Split(',');
                var items = await hrServiceManager.HrNoteService.GetAllVW(e => e.IsDeleted == false &&
                (string.IsNullOrEmpty(filter.EmpCode) || e.EmpCode == filter.EmpCode) &&
                (string.IsNullOrEmpty(filter.EmpName) || e.EmpName == filter.EmpName) &&
                (BranchesList == null || BranchesList.Contains(e.BranchId.ToString())));
                if (items.Succeeded)
                {
                    if (items.Data.Count() > 0)
                    {
                        return Ok(await Result<List<HrNoteVw>>.SuccessAsync(items.Data.ToList(), localization.GetResource1("NosearchResult")));
                    }
                    return Ok(await Result<List<HrNoteVw>>.SuccessAsync(items.Data.ToList(),localization.GetResource1("NosearchResult")));
                }
                return Ok(await Result<HrNoteVw>.FailAsync(items.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrNoteVw>.FailAsync(ex.Message));
            }
        }


        [HttpPost("GetPagination")]
        public async Task<IActionResult> GetPagination([FromBody] HrNoteFilterDto filter, int take = Pagination.take, long? lastSeenId = null)
        {
            var chk = await permission.HasPermission(262, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var BranchesList = session.Branches.Split(',');

                var items = await hrServiceManager.HrNoteService.GetAllWithPaginationVW(selector: e => e.NoteId,
                expression: e => e.IsDeleted == false &&
                (string.IsNullOrEmpty(filter.EmpCode) || e.EmpCode == filter.EmpCode) &&
                (string.IsNullOrEmpty(filter.EmpName) || e.EmpName == filter.EmpName) &&
                (BranchesList == null || BranchesList.Contains(e.BranchId.ToString())),
                    take: take,
                    lastSeenId: lastSeenId);

                if (!items.Succeeded)
                    return Ok(await Result<List<HrNoteVw>>.FailAsync(items.Status.message));

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
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }


        [HttpPost("Add")]
        public async Task<ActionResult> Add(HrNoteDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(262, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var add = await hrServiceManager.HrNoteService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add HRNoteController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(HrNoteEditDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(262, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result<HrNoteEditDto>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var update = await hrServiceManager.HrNoteService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrNoteEditDto>.FailAsync($"====== Exp in Edit HrNoteController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long Id)
        {
            try
            {
                var chk = await permission.HasPermission(262, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var item = await hrServiceManager.HrNoteService.GetOneVW(x=>x.NoteId== Id);
                if (item.Succeeded)
                {
                    if (item.Data != null)
                    {
                        var result = new HrNoteEditDto
                        {
                            EmpId = item.Data.EmpId,
                            NoteId = item.Data.NoteId,
                            EmpCode = item.Data.EmpCode,
                            NoteDate = item.Data.NoteDate,
                            NoteText = item.Data.NoteText,
                        };
                        return Ok(await Result <HrNoteEditDto>.SuccessAsync(result));
                    }
                }
                return Ok(await Result<HrNoteEditDto>.FailAsync(item.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrNoteEditDto>.FailAsync($"====== Exp in HRNoteController getById, MESSAGE: {ex.Message}"));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var chk = await permission.HasPermission(262, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await hrServiceManager.HrNoteService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete HRNotesController, MESSAGE: {ex.Message}"));
            }
        }
    }
}