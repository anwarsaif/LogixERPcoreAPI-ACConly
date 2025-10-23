using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.HR
{

    //   مستويات الوظائف
    public class HRJobLevelController : BaseHrApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;

        public HRJobLevelController(IHrServiceManager hrServiceManager, IMainServiceManager mainServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization)
        {
            this.mainServiceManager = mainServiceManager;
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
        }


        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrJobLevelFilterDto filter)
        {
            var chk = await permission.HasPermission(572, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                List<HrJobLevelFilterDto> resultList = new List<HrJobLevelFilterDto>();
                var items = await hrServiceManager.HrJobLevelService.GetAll(e => e.IsDeleted == false);
                if (items.Succeeded)
                {
                    if (items.Data.Count() > 0)
                    {
                        var res = items.Data.AsQueryable();

                        if (!string.IsNullOrEmpty(filter.LevelName))
                        {
                            res = res.Where(r => r.LevelName != null && r.LevelName.Contains(filter.LevelName));
                        }
                        foreach (var item in res)
                        {
                            var newRecord = new HrJobLevelFilterDto
                            {
                                Id = item.Id,
                                LevelName = item.LevelName,
                                MaxSalary = item.MaxSalary,
                                MinSalary = item.MinSalary,
                                RatePerNight = item.RatePerNight,
                                TransportAmount = item.TransportAmount,
                                GroupId = item.GroupId
                            };
                            resultList.Add(newRecord);
                        }
                        if (resultList.Any())
                            return Ok(await Result<List<HrJobLevelFilterDto>>.SuccessAsync(resultList, ""));
                        return Ok(await Result<List<HrJobLevelFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));
                    }
                    return Ok(await Result<List<HrJobLevelFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));
                }
                return Ok(await Result<HrJobLevelFilterDto>.FailAsync(items.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrJobLevelFilterDto>.FailAsync(ex.Message));
            }
        }

        [HttpPost("GetPagination")]
        public async Task<IActionResult> GetPagination(HrJobLevelFilterDto filter, int take = Pagination.take, long? lastSeenId = null)
        {
            var chk = await permission.HasPermission(572, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var items = await hrServiceManager.HrJobLevelService.GetAllWithPaginationVW(
                    selector: x => x.Id,
                    expression: e => e.IsDeleted == false
                                && (string.IsNullOrEmpty(filter.LevelName) || e.LevelName.Contains(filter.LevelName)),
                    take: take,
                    lastSeenId: lastSeenId
                );

                if (!items.Succeeded)
                    return StatusCode(items.Status.code, items.Status.message);

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrJobLevelFilterDto>.FailAsync(ex.Message));
            }
        }


        [HttpPost("Add")]
        public async Task<ActionResult> Add(HrJobLevelDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(572, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var add = await hrServiceManager.HrJobLevelService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add Hr Job Level Controller, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(HrJobLevelEditDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(572, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result<HrJobLevelEditDto>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var update = await hrServiceManager.HrJobLevelService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrJobLevelEditDto>.FailAsync($"====== Exp in Edit Hr Job Level Controller, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long Id)
        {
            try
            {
                var chk = await permission.HasPermission(572, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var item = await hrServiceManager.HrJobLevelService.GetOne(x => x.Id == Id);
                return Ok(item);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrNoteEditDto>.FailAsync($"====== Exp in Hr Job Level Controller getById, MESSAGE: {ex.Message}"));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var chk = await permission.HasPermission(572, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await hrServiceManager.HrJobLevelService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete Hr Job Level Controller, MESSAGE: {ex.Message}"));
            }
        }
    }
}