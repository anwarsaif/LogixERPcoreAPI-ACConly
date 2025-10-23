using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.HR
{
	public class HrJobCategoryController : BaseHrApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;

        public HrJobCategoryController(IHrServiceManager hrServiceManager, IMainServiceManager mainServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization)
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
        public async Task<IActionResult> Search(HrJobCategoryFilterDto filter)
        {
            var chk = await permission.HasPermission(2278, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                List<HrJobCategoryFilterDto> resultList = new List<HrJobCategoryFilterDto>();
                var items = await hrServiceManager.HrJobCategoryService.GetAllVW(x => x.IsDeleted == false);
                if (items.Succeeded)
                {
                    if (items.Data.Count() > 0)
                    {
                        var res = items.Data.AsQueryable();

                        if (!string.IsNullOrEmpty(filter.Name))
                        {
                            res = res.Where(r => r.Name != null && r.Name.Equals(filter.Name));
                        }
                        foreach (var item in res)
                        {
                            var newRecord = new HrJobCategoryFilterDto
							{
                                Id=item.Id,
								Code=item.Code,
								Name =item.Name,   
                                Name2=item.Name2,
								GroupName = item.GroupName,
								GroupName2 = item.GroupName2,
								RefranceCode = item.RefranceCode,
								RefranceName = item.RefranceName,
								StatusName = item.StatusName,
								StatusName2 = item.StatusName2,
                            };
                            resultList.Add(newRecord);
                        }
                        if (resultList.Any())
                            return Ok(await Result<List<HrJobCategoryFilterDto>>.SuccessAsync(resultList, ""));
                        return Ok(await Result<List<HrJobCategoryFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));
                    }
                    return Ok(await Result<List<HrJobCategoryFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));
                }
                return Ok(await Result<HrJobCategoryFilterDto>.FailAsync(items.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrJobCategoryFilterDto>.FailAsync(ex.Message));
            }
        }

        [HttpPost("Add")]
        public async Task<ActionResult> Add(HrJobCategoryDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(2278, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var add = await hrServiceManager.HrJobCategoryService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add Hr Job Grade Controller, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(HrJobCategoryEditDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(2278, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result<HrJobCategoryEditDto>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var update = await hrServiceManager.HrJobCategoryService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrJobCategoryEditDto>.FailAsync($"====== Exp in Edit Hr Job Grade Controller, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long Id)
        {
            try
            {
                var chk = await permission.HasPermission(2278, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var item = await hrServiceManager.HrJobCategoryService.GetOne(x => x.Id == Id);
                return Ok(item);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrJobCategoryEditDto>.FailAsync($"====== Exp in Hr Job Grade Controller getById, MESSAGE: {ex.Message}"));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var chk = await permission.HasPermission(2278, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await hrServiceManager.HrJobCategoryService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete Hr Job Grade Controller, MESSAGE: {ex.Message}"));
            }
        }
    }

}