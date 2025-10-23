using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.HR
{

    // رابط التوظيف الخارجي
    public class HRListOfJobController : BaseHrApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;

        public HRListOfJobController(IHrServiceManager hrServiceManager, IMainServiceManager mainServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization)
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
            var chk = await permission.HasPermission(1016, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var items = await hrServiceManager.HrRecruitmentVacancyService.GetAllHRRecruitmentVacancy();

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrRecruitmentVacancyVwDto>.FailAsync(ex.Message));
            }
        }
        [HttpPost("Add")]
        public async Task<IActionResult> Add(HrRecruitmentCandidateDto obj)
        {
            var chk = await permission.HasPermission(1016, PermissionType.Add);
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
                var addRes = await hrServiceManager.HrRecruitmentCandidateService.Add(obj);
                return Ok(addRes);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrRecruitmentCandidateDto>.FailAsync(ex.Message));
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long Id)
        {
            try
            {
                var chk = await permission.HasPermission(1016, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id <= 0)
                {
                    return Ok(await Result<HrRecruitmentVacancy>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var item = await hrServiceManager.HrRecruitmentVacancyService.GetOne(x => x.Id == Id);
                return Ok(item);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrRecruitmentVacancy>.FailAsync($"====== Exp in HR Recruitment vacancy Controller getById, MESSAGE: {ex.Message}"));
            }
        }

    }
}