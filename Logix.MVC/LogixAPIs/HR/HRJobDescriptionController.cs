using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.HR
{
    //  الوصف الوظيفي
    public class HRJobDescriptionController : BaseHrApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;

        public HRJobDescriptionController(IHrServiceManager hrServiceManager, IMainServiceManager mainServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization)
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
        [HttpPost("getAll")]
        public async Task<IActionResult> getAll()
        {
            var chk = await permission.HasPermission(1000, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var items = await hrServiceManager.HrJobDescriptionService.GetAll(e => e.IsDeleted == false );

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result <HrJobDescriptionDto>.FailAsync(ex.Message));
            }
        }
        [HttpPost("Add")]
        public async Task<IActionResult> Add(HrJobDescriptionDto obj)
        {
            var chk = await permission.HasPermission(1000, PermissionType.Add);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            setErrors();
            if (!ModelState.IsValid)
            {
                return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));
            }
            try
            {
                var addRes = await hrServiceManager.HrJobDescriptionService.Add(obj);
                return Ok(addRes);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrJobDescriptionDto>.FailAsync(ex.Message));
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long Id)
        {
            try
            {
                var chk = await permission.HasPermission(1000, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id <= 0)
                {
                    return Ok(await Result<HrJobDescriptionEditDto>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var item = await hrServiceManager.HrJobDescriptionService.GetOne(x => x.Id == Id);
                return Ok(item);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrJobDescriptionEditDto>.FailAsync($"====== Exp in HR job Description Controller getById, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(HrJobDescriptionEditDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(1000, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                {
                    return Ok(await Result<HrJobDescriptionEditDto>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));
                }

                var update = await hrServiceManager.HrJobDescriptionService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrJobDescriptionEditDto>.FailAsync($"====== Exp in Edit Hr job Description Controller, MESSAGE: {ex.Message}"));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long Id = 0)
        {
            setErrors();
            var chk = await permission.HasPermission(1000, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (Id <= 0)
            {
                return Ok(await Result<HrJobDescriptionDto>.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));
            }

            try
            {
                var del = await hrServiceManager.HrJobDescriptionService.Remove(Id);
                if (del.Succeeded)
                {

                    return Ok(await Result<HrJobDescriptionDto>.SuccessAsync("Item deleted successfully"));
                }
                return Ok(await Result<HrJobDescriptionDto>.FailAsync($"{del.Status.message}"));
            }
            catch (Exception exp)
            {
                return Ok(await Result<HrJobDescriptionDto>.FailAsync($"{exp.Message}"));
            }
        }
    }
}