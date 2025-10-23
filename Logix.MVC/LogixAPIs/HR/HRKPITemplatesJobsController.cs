using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.HR
{
    // ربط نماذج التقييم مع المسمى الوظيفي
    public class HRKPITemplatesJobsController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;

        public HRKPITemplatesJobsController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
        }

        #region Main Page



        [HttpGet("GetAll")]

        public async Task<IActionResult> GetAll()
        {
            var chk = await permission.HasPermission(2099, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var items = await hrServiceManager.HrKpiTemplatesJobService.GetAllVW(x => x.IsDeleted == false && x.FacilityId == session.FacilityId);
                if (items.Succeeded)
                {
                    if (items.Data.Count() > 0)
                    {
                        return Ok(await Result<object>.SuccessAsync(items.Data.ToList(), ""));
                    }
                    return Ok(await Result<object>.SuccessAsync(items.Data.ToList(), localization.GetResource1("NosearchResult")));
                }
                return Ok(await Result<object>.FailAsync(items.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync(ex.Message));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long Id = 0)
        {
            try
            {
                var chk = await permission.HasPermission(2099, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await hrServiceManager.HrKpiTemplatesJobService.Remove(Id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete HRKPITemplatesJobsController, MESSAGE: {ex.Message}"));
            }
        }


        [HttpPost("Add")]
        public async Task<IActionResult> Add(HrKpiTemplatesJobDto obj)
        {
            var chk = await permission.HasPermission(2099, PermissionType.Add);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (!ModelState.IsValid)
            {
                return Ok(obj);
            }
            try
            {
                obj.CatJobId ??= 0;
                obj.TemplateId ??= 0;
                if (obj.CatJobId <= 0)
                    return Ok(await Result<object>.FailAsync($"يجب نحديد المسمى الوظيفي "));
                if (obj.TemplateId <= 0)
                    return Ok(await Result<object>.FailAsync($"  يجب نحديد نموذج التقييم"));

                var addRes = await hrServiceManager.HrKpiTemplatesJobService.Add(obj);
                return Ok(addRes);
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync(ex.Message));
            }
        }
        #endregion




    }
}