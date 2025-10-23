using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.HR
{
	public class HRJobGroupsController : BaseHrApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;

        public HRJobGroupsController(IHrServiceManager hrServiceManager, IMainServiceManager mainServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization)
        {
            this.mainServiceManager = mainServiceManager;
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
        }

        [NonAction]
        private void setErrors()
        {
            var errors = new ErrorsHelper(ModelState);
        }
        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrJobGroupsFilterDto filter)
        {
            var chk = await permission.HasPermission(2277, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                List<HrJobGroupsFilterDto> resultList = new List<HrJobGroupsFilterDto>();
                var items = await hrServiceManager.HrJobGroupsService.GetAllVW(e => e.IsDeleted == false );
                if (items.Succeeded)
                {
                    if (items.Data.Count() > 0)
                    {
                        var res = items.Data.AsQueryable();

                        if (!string.IsNullOrEmpty(filter.Code))
                        {
                            res = res.Where(r => r.Code != null && r.Code.Equals(filter.Code));
                        }
                        if (!string.IsNullOrEmpty(filter.Name))
                        {
                            res = res.Where(c => (c.Name != null && c.Name == filter.Name));
                        }
                        if (filter.StatusId > 0)
                        {
                            res = res.Where(c => (c.StatusId != null && c.StatusId == filter.StatusId));
                        }
                        foreach (var item in res)
                        {
                            var newRecord = new HrJobGroupsFilterDto
							{
                                Id=item.Id,
								Code=item.Code,
								Name =item.Name,   
                                Name2=item.Name2,
								ParentId = item.ParentId,
								ParentName = item.ParentName,
								ParentName2 = item.ParentName2,
								StatusId = item.StatusId,
								StatusName = item.StatusName,
								StatusName2 = item.StatusName2,
                            };
                            resultList.Add(newRecord);
                        }
                        if (resultList.Any())
                            return Ok(await Result<List<HrJobGroupsFilterDto>>.SuccessAsync(resultList, ""));
                        return Ok(await Result<List<HrJobGroupsFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));
                    }
                    return Ok(await Result<List<HrJobGroupsFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));
                }
                return Ok(await Result<HrJobGroupsFilterDto>.FailAsync(items.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrJobGroupsFilterDto>.FailAsync(ex.Message));
            }
        }

        [HttpPost("Add")]
        public async Task<ActionResult> Add(HrJobGroupsDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(2277, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var add = await hrServiceManager.HrJobGroupsService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add Hr Job Grade Controller, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(HrJobGroupsEditDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(2277, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result<HrJobGroupsEditDto>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var update = await hrServiceManager.HrJobGroupsService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrJobGroupsEditDto>.FailAsync($"====== Exp in Edit Hr Job Grade Controller, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long Id)
        {
            try
            {
                var chk = await permission.HasPermission(2277, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var item = await hrServiceManager.HrJobGroupsService.GetOne(x => x.Id == Id);
                return Ok(item);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrJobGroupsEditDto>.FailAsync($"====== Exp in Hr Job Grade Controller getById, MESSAGE: {ex.Message}"));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var chk = await permission.HasPermission(2277, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await hrServiceManager.HrJobGroupsService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete Hr Job Grade Controller, MESSAGE: {ex.Message}"));
            }
        }
    }

}