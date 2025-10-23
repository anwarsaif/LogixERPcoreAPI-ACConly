using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.HR
{
    //  طلبات التوظيف
    public class HRRecruitmentVacancyController : BaseHrApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;

        public HRRecruitmentVacancyController(IHrServiceManager hrServiceManager, IMainServiceManager mainServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization)
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
        public async Task<IActionResult> Search(HrRecruitmentVacancyFilterDto filter)
        {
            var chk = await permission.HasPermission(574, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                filter.StatusId ??= 0;
                filter.JobId ??= 0;
                var items = await hrServiceManager.HrRecruitmentVacancyService.SearchHRRecruitmentVacancy(filter);

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrRecruitmentVacancyVwDto>.FailAsync(ex.Message));
            }
        }
        [HttpPost("Add")]
        public async Task<IActionResult> Add(HrRecruitmentVacancyDto obj)
        {
            var chk = await permission.HasPermission(574, PermissionType.Add);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (obj.BranchId <= 0 || obj.FacilityId <= 0 || obj.LocationId <= 0 || obj.DeptId <= 0 || obj.JobId <= 0)
            {
                return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));
            }
            try
            {
                obj.SalaryFrom ??= 0;
                obj.SalaryTo ??= 0;
                obj.AgeFrom ??= 0;
                obj.AgeTo ??= 0;
                if (string.IsNullOrEmpty(obj.Experience)) obj.Experience = "";
                obj.QualificationId ??= 0;
                var addRes = await hrServiceManager.HrRecruitmentVacancyService.Add(obj);
                return Ok(addRes);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrRecruitmentVacancyDto>.FailAsync(ex.Message));
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long Id)
        {
            try
            {
                var chk = await permission.HasPermission(574, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id <= 0)
                {
                    return Ok(await Result<HrRecruitmentVacancyEditDto>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var item = await hrServiceManager.HrRecruitmentVacancyService.GetOne(x => x.Id == Id);
                return Ok(item);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrRecruitmentVacancyEditDto>.FailAsync($"====== Exp in HR Recruitment Vacancy Controller getById, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(HrRecruitmentVacancyEditDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(574, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid || obj.BranchId <= 0 || obj.FacilityId <= 0 || obj.LocationId <= 0 || obj.DeptId <= 0 || obj.JobId <= 0)
                {
                    return Ok(await Result<HrRecruitmentVacancyEditDto>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));
                }

                var update = await hrServiceManager.HrRecruitmentVacancyService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrRecruitmentVacancyEditDto>.FailAsync($"====== Exp in Edit Hr Recruitment Vacancy Controller, MESSAGE: {ex.Message}"));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long Id = 0)
        {
            setErrors();
            var chk = await permission.HasPermission(574, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (Id <= 0)
            {
                return Ok(await Result<HrRecruitmentVacancyDto>.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));
            }

            try
            {
                var del = await hrServiceManager.HrRecruitmentVacancyService.Remove(Id);

                return Ok(del);
            }
            catch (Exception exp)
            {
                return Ok(await Result<HrRecruitmentVacancyDto>.FailAsync($"{exp.Message}"));
            }
        }
    }
}