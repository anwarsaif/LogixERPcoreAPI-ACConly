using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.HR
{
    public class HrEvaluationAnnualIncreaseConfigApiController : BaseHrApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly IDDListHelper listHelper;
        private readonly ILocalizationService localization;

        public HrEvaluationAnnualIncreaseConfigApiController(IHrServiceManager hrServiceManager, IDDListHelper listHelper, IMainServiceManager mainServiceManager, ICurrentData session, IPermissionHelper permission, ILocalizationService localization)
        {
            this.mainServiceManager = mainServiceManager;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.permission = permission;
            this.listHelper = listHelper;
            this.localization = localization;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var chk = await permission.HasPermission(1686, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var items = await hrServiceManager.HrEvaluationAnnualIncreaseConfigService.GetAll(e => e.IsDeleted == false && e.FacilityId == session.FacilityId);
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    res = res.OrderBy(e => e.Id);
                    return Ok(await Result<List<HrEvaluationAnnualIncreaseConfigDto>>.SuccessAsync(res.ToList(), items.Status.message));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add(HrEvaluationAnnualIncreaseConfigDto obj)
        {
            var chk = await permission.HasPermission(1686, PermissionType.Add);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            if (!ModelState.IsValid)
            {
                return Ok(await Result<HrEvaluationAnnualIncreaseConfigDto>.FailAsync(localization.GetMessagesResource("dataRequire")));
            }
            try
            {
                var addRes = await hrServiceManager.HrEvaluationAnnualIncreaseConfigService.Add(obj);
                return Ok(addRes);
            }

            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long Id)
        {
            var chk = await permission.HasPermission(1686, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (Id.Equals(null))
            {
                return Ok(await Result.FailAsync(localization.GetMessagesResource("dataRequire")));
            }

            try
            {
                var getItem = await hrServiceManager.HrEvaluationAnnualIncreaseConfigService.GetForUpdate<HrEvaluationAnnualIncreaseConfigEditDto>(Id);
                if (getItem.Succeeded && getItem.Data != null)
                {
                    return Ok(getItem);
                }
                return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));

            }
        }

        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(HrEvaluationAnnualIncreaseConfigEditDto obj)
        {

            var chk = await permission.HasPermission(1686, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (!ModelState.IsValid)
            {
                return Ok(await Result<HrEvaluationAnnualIncreaseConfigEditDto>.FailAsync(localization.GetMessagesResource("dataRequire")));

            }
            try
            {
                var addRes = await hrServiceManager.HrEvaluationAnnualIncreaseConfigService.Update(obj);
                if (addRes.Succeeded)
                {
                    return Ok(addRes);
                }

                else
                {
                    return Ok(await Result<HrEvaluationAnnualIncreaseConfigEditDto>.FailAsync($"{addRes.Status.message}"));
                }
            }

            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));

            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long Id = 0)
        {

            var chk = await permission.HasPermission(1686, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (Id <= 0)
            {
                return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));
            }
            try
            {
                var del = await hrServiceManager.HrEvaluationAnnualIncreaseConfigService.Remove(Id);
                return Ok(del);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));

            }
        }

    }

}