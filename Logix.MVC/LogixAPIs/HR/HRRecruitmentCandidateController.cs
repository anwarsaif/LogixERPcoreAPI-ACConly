using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.HR
{

    // المرشحين
    public class HRRecruitmentCandidateController : BaseHrApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;

        public HRRecruitmentCandidateController(IHrServiceManager hrServiceManager, IMainServiceManager mainServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization)
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
        public async Task<IActionResult> Search(HrRecruitmentCandidateFilterDto filter)
        {
            var chk = await permission.HasPermission(573, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {

                var items = await hrServiceManager.HrRecruitmentCandidateService.Search(filter);
                return Ok(items);

            }
            catch (Exception ex)
            {
                return Ok(await Result<HrJobLevelFilterDto>.FailAsync(ex.Message));
            }
        }

        [HttpPost("GetPagination")]
        public async Task<IActionResult> GetPagination(HrRecruitmentCandidateFilterDto filter, int take = Pagination.take, long? lastSeenId = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var chk = await permission.HasPermission(573, PermissionType.Show);
                if (!chk)
                {
                    return Ok(await Result.AccessDenied("AccessDenied"));
                }
                var items = await hrServiceManager.HrRecruitmentCandidateService.GetAllWithPaginationVW(
                    selector: x => x.Id,
                    expression: e => e.IsDeleted == false
                                && e.FacilityId == session.FacilityId
                                && (string.IsNullOrEmpty(filter.Name) || (e.Name != null && e.Name.Contains(filter.Name)))
                                && (filter.VacancyId == null || filter.VacancyId == 0 || e.VacancyId == filter.VacancyId)
                                && (filter.NationalityId == null || filter.NationalityId == 0 || e.NationalityId == filter.NationalityId)
                                && (filter.Gender == null || filter.Gender == 0 || e.Gender == filter.Gender)
                                && (filter.QualificationId == null || filter.QualificationId == 0 || e.QualificationId == filter.QualificationId)
                                && (filter.SpecializationId == null || filter.SpecializationId == 0 || e.SpecializationId == filter.SpecializationId)
                                && (filter.MaritalStatus == null || filter.MaritalStatus == 0 || e.MaritalStatus == filter.MaritalStatus)
                                && (filter.YearOfExp == null || filter.YearOfExp == 0 || e.YearOfExp == filter.YearOfExp)
                                && (string.IsNullOrEmpty(filter.RangeExperience) || (e.RangeExperience != null && e.RangeExperience.Contains(filter.RangeExperience)))
                                && (string.IsNullOrEmpty(filter.YearGraduation) || (e.YearGraduation != null && e.YearGraduation.Contains(filter.YearGraduation))),
                    take: take,
                    lastSeenId: lastSeenId
                );

                if (!items.Succeeded)
                    return StatusCode(items.Status.code, items.Status.message);

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync(ex.Message));
            }
        }

        [HttpPost("Add")]
        public async Task<ActionResult> Add(HrRecruitmentCandidateDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(573, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var add = await hrServiceManager.HrRecruitmentCandidateService.RecruitmentCandidateAdd(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add Hr Recruitment Candidate Controller, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(HrRecruitmentCandidateEditDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(573, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result<HrRecruitmentCandidateEditDto>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var update = await hrServiceManager.HrRecruitmentCandidateService.RecruitmentCandidateEdit(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrRecruitmentCandidateEditDto>.FailAsync($"====== Exp in Edit Hr Recruitment Candidate Controller, MESSAGE: {ex.Message}"));
            }
        }


        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var chk = await permission.HasPermission(573, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await hrServiceManager.HrRecruitmentCandidateService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete Hr Recruitment Candidate Controller, MESSAGE: {ex.Message}"));
            }
        }
        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long Id)
        {
            HrRecruitmentCandidateGetByIdDto result = new HrRecruitmentCandidateGetByIdDto();
            List<HrRecruitmentCandidateKpiDVw> listOfItems = new List<HrRecruitmentCandidateKpiDVw>();
            try
            {
                var chk = await permission.HasPermission(573, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }
                var item = await hrServiceManager.HrRecruitmentCandidateService.GetOneVW(x => x.Id == Id);
                if (!item.Succeeded) return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                result.candidateData = item.Data;
                var GetAppIDKPIByCandidateID = await hrServiceManager.HrRecruitmentCandidateKpiService.GetAllVW(x => x.CandidateId == Id);
                if (GetAppIDKPIByCandidateID.Data.Any())
                {
                    var AppId = GetAppIDKPIByCandidateID.Data?.Any() == true ? GetAppIDKPIByCandidateID.Data.Max(x => x.AppId) : 0;
                    if (AppId != 0)
                    {
                        var GetHRRecruitmentCandidateKPIByAppID = await hrServiceManager.HrRecruitmentCandidateKpiService.GetOneVW(X => X.AppId == AppId);
                        result.EvaDate = GetHRRecruitmentCandidateKPIByAppID.Data.EvaDate;
                        result.TemName = GetHRRecruitmentCandidateKPIByAppID.Data.TemName;
                        result.CandidateName = GetHRRecruitmentCandidateKPIByAppID.Data.CandidateName;
                        result.VacancyName = GetHRRecruitmentCandidateKPIByAppID.Data.VacancyName;
                        var GetHRRecruitmentCandidateKPID = await hrServiceManager.HrRecruitmentCandidateKpiDService.GetAllVW(x => x.KpiId == GetHRRecruitmentCandidateKPIByAppID.Data.Id);
                        foreach (var newCandidateKPID in GetHRRecruitmentCandidateKPID.Data)
                        {

                            listOfItems.Add(newCandidateKPID);
                        }
                        result.allDetails = listOfItems.ToList();
                    }
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrRecruitmentCandidateDto>.FailAsync($"====== Exp in Hr Recruitment Candidate Controller getById, MESSAGE: {ex.Message}"));
            }
        }


        [HttpGet("GetHRRecruitmentCandidateForAcceptance")]
        public async Task<IActionResult> GetHRRecruitmentCandidateForAcceptance(long Id)
        {
            try
            {
                var chk = await permission.HasPermission(573, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var item = await hrServiceManager.HrRecruitmentCandidateService.GetOneVW(x => x.Id == Id);
                return Ok(item);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrNoteEditDto>.FailAsync($"====== Exp in Hr Job Grade Controller getById, MESSAGE: {ex.Message}"));
            }
        }
    }
}