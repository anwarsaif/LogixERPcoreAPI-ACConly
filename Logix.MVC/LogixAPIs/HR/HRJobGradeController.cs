using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.HR
{
    // درجات الوظائف 
    public class HRJobGradeController : BaseHrApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;

        public HRJobGradeController(IHrServiceManager hrServiceManager, IMainServiceManager mainServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization)
        {
            this.mainServiceManager = mainServiceManager;
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
        }


        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrJobGradeFilterDto filter, int take = Pagination.take, long? lastSeenId = null)
        {
            var chk = await permission.HasPermission(571, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var items = await hrServiceManager.HrJobGradeService.GetAllWithPaginationVW(
                    selector: x => x.Id,
                    expression: e => e.IsDeleted == false
                                && (string.IsNullOrEmpty(filter.GradeName) || e.GradeName == filter.GradeName)
                                && (string.IsNullOrEmpty(filter.GradeNo) || e.GradeNo == filter.GradeNo)
                                && (filter.LevelId == 0 || e.LevelId == filter.LevelId),
                    take: take,
                    lastSeenId: lastSeenId
                );

                if (!items.Succeeded)
                    return StatusCode(items.Status.code, items.Status.message);

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrJobGradeFilterDto>.FailAsync(ex.Message));
            }
        }

        [HttpPost("GetPagination")]
        public async Task<IActionResult> GetPagination(HrJobGradeFilterDto filter, int take = Pagination.take, long? lastSeenId = null)
        {
            var chk = await permission.HasPermission(571, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var items = await hrServiceManager.HrJobGradeService.GetAllWithPaginationVW(
                    selector: x => x.Id,
                    expression: e => e.IsDeleted == false
                                && (string.IsNullOrEmpty(filter.GradeName) || e.GradeName == filter.GradeName)
                                && (string.IsNullOrEmpty(filter.GradeNo) || e.GradeNo == filter.GradeNo)
                                && (filter.LevelId == 0 || e.LevelId == filter.LevelId),
                    take: take,
                    lastSeenId: lastSeenId
                );

                if (!items.Succeeded)
                    return StatusCode(items.Status.code, items.Status.message);

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrJobGradeFilterDto>.FailAsync(ex.Message));
            }
        }
        [HttpPost("Add")]
        public async Task<ActionResult> Add(HrJobGradeDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(571, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var add = await hrServiceManager.HrJobGradeService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add Hr Job Grade Controller, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(HrJobGradeEditDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(571, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result<HrJobGradeEditDto>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var update = await hrServiceManager.HrJobGradeService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrJobGradeEditDto>.FailAsync($"====== Exp in Edit Hr Job Grade Controller, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long Id)
        {
            try
            {
                var chk = await permission.HasPermission(571, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var item = await hrServiceManager.HrJobGradeService.GetOne(x => x.Id == Id);
                return Ok(item);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrNoteEditDto>.FailAsync($"====== Exp in Hr Job Grade Controller getById, MESSAGE: {ex.Message}"));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var chk = await permission.HasPermission(571, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await hrServiceManager.HrJobGradeService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete Hr Job Grade Controller, MESSAGE: {ex.Message}"));
            }
        }
    }

}