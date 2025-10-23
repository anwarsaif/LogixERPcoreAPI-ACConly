using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.HR.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.HR
{

    //  تقييم المرشحين
    public class HRCandidatesKPIController : BaseHrApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;

        public HRCandidatesKPIController(IHrServiceManager hrServiceManager, IMainServiceManager mainServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization)
        {
            this.mainServiceManager = mainServiceManager;
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
        }

        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrRecruitmentCandidateKpiFilterDto filter)
        {
            var chk = await permission.HasPermission(579, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var items = await hrServiceManager.HrRecruitmentCandidateKpiService.SearchHrRecruitmentCandidateKp(filter);

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrRecruitmentCandidateKpiFilterDto>.FailAsync(ex.Message));
            }
        }
        [HttpPost("GetPagination")]
        public async Task<IActionResult> GetPagination(HrRecruitmentCandidateKpiFilterDto filter, int take = Pagination.take, long? lastSeenId = null)
        {
            var chk = await permission.HasPermission(579, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                filter.CandidateId ??= 0;
                filter.KpiTemId ??= 0;

                var items = await hrServiceManager.HrRecruitmentCandidateKpiService.GetAllWithPaginationVW(
                    selector: x => x.Id,
                    expression: e => e.IsDeleted == false
                                && (filter.CandidateId == 0 || e.CandidateId == filter.CandidateId)
                                && (filter.KpiTemId == 0 || e.KpiTemId == filter.KpiTemId)
                                && (string.IsNullOrEmpty(filter.CandidateName) || (e.CandidateName != null && e.CandidateName.Contains(filter.CandidateName)))
                                && (string.IsNullOrEmpty(filter.EvaDate) || (e.EvaDate != null && DateHelper.StringToDate(e.EvaDate) >= DateHelper.StringToDate(filter.EvaDate)))
                                && (string.IsNullOrEmpty(filter.EvaDateTo) || (e.EvaDate != null && DateHelper.StringToDate(e.EvaDate) <= DateHelper.StringToDate(filter.EvaDateTo))),
                    take: take,
                    lastSeenId: lastSeenId
                );

                if (!items.Succeeded)
                    return StatusCode(items.Status.code, items.Status.message);

                decimal? Eva_Degree = 0;
                decimal? Eva_Value = 0;
                List<HrRecruitmentCandidateKpiFilterDto> result = new List<HrRecruitmentCandidateKpiFilterDto>();
                foreach (var item in items.Data)
                {
                    var getAllKpiDetailsData = await hrServiceManager.HrRecruitmentCandidateKpiDService.GetAllVW(x => x.IsDeleted == false && x.KpiId == item.Id);
                    Eva_Degree = getAllKpiDetailsData.Data.Sum(x => x.Degree);
                    Eva_Value = getAllKpiDetailsData.Data.Sum(x => x.Degree * x.Weight);
                    var newRecord = new HrRecruitmentCandidateKpiFilterDto
                    {
                        Id = item.Id,
                        EvaDegree = Eva_Degree,
                        CandidateId = item.Id,
                        CandidateName = item.CandidateName,
                        EvaDate = item.EvaDate,
                        EvaValue = Eva_Value,
                        KpiTemId = item.KpiTemId,
                        TemName = item.TemName,
                        VacancyName = item.VacancyName
                    };
                    result.Add(newRecord);

                }
                var paginatedData = new PaginatedResult<object>
                {
                    Succeeded = items.Succeeded,
                    Data = result,
                    Status = items.Status,
                    PaginationInfo = items.PaginationInfo
                };

                return Ok(paginatedData);

            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync(ex.Message));
            }
        }

        [HttpPost("Add")]
        public async Task<ActionResult> Add(HrCandidateKPIDtoForOperations obj)
        {
            try
            {
                var chk = await permission.HasPermission(579, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var add = await hrServiceManager.HrRecruitmentCandidateKpiService.AddNewCandidateKpi(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in HR Candidates KPI Controller  Add Function, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(HrCandidateKPIDtoForOperations obj)
        {
            try
            {
                var chk = await permission.HasPermission(579, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result<HrCandidateKPIDtoForOperations>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var update = await hrServiceManager.HrRecruitmentCandidateKpiService.UpdateCandidateKpi(obj);
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
            List<HrRecruitmentCandidateKpiDVw> result = new List<HrRecruitmentCandidateKpiDVw>();
            var finalResult = new HrCandidateKPIDtoForGetById();

            try
            {
                var chk = await permission.HasPermission(579, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var item = await hrServiceManager.HrRecruitmentCandidateKpiService.GetOneVW(x => x.Id == Id);
                if (item.Succeeded)
                {
                    if (item.Data != null)
                    {
                        finalResult.CandidateId = item.Data.CandidateId;
                        finalResult.Id = (int?)item.Data.Id;
                        finalResult.CandidateName = item.Data.CandidateName;
                        finalResult.KpiTemId = item.Data.KpiTemId;
                        finalResult.EvaDate = item.Data.EvaDate;
                        finalResult.VacancyName = item.Data.VacancyName;
                        var getKpiDItems = await hrServiceManager.HrRecruitmentCandidateKpiDService.GetAllVW(x => x.KpiId == Id);
                        if (getKpiDItems != null)
                        {
                            foreach (var singlItem in getKpiDItems.Data)
                            {

                                var newDetails = new HrRecruitmentCandidateKpiDVw
                                {
                                    Id = singlItem.Id,
                                    Degree = singlItem.Degree,
                                    KpiId = singlItem.KpiId,
                                    CandidateId = singlItem.CandidateId,
                                    CandidateName = singlItem.CandidateName,
                                    VacancyName = singlItem.VacancyName,
                                    Subject = singlItem.Subject,
                                    Description = singlItem.Description,
                                    Score = singlItem.Score,
                                    Weight = singlItem.Weight,


                                };
                                result.Add(newDetails);
                            }
                            finalResult.KpiDVw = result;
                        }

                        return Ok(await Result<HrCandidateKPIDtoForGetById>.SuccessAsync(finalResult));

                    }
                    else
                    {
                        return Ok(await Result<HrRecruitmentCandidateKpiVw>.FailAsync(item.Status.message));

                    }
                }

                return Ok(await Result<HrRecruitmentCandidateKpiVw>.FailAsync(item.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrNoteEditDto>.FailAsync($"====== Exp in Hr Candidate Kpi Controller getById, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var chk = await permission.HasPermission(579, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await hrServiceManager.HrRecruitmentCandidateKpiService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete Hr Recruitment Candidate Kpi Controller, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("EmpIdChanged")]
        public async Task<IActionResult> EmpIdChanged(string EmpId)
        {
            var chk = await permission.HasPermission(579, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (string.IsNullOrEmpty(EmpId))
            {
                return Ok(await Result<EmpIdChangedVM>.SuccessAsync(localization.GetResource1("EmployeeIsNumber")));
            }

            try
            {
                var checkEmpId = await mainServiceManager.InvestEmployeeService.GetOne(i => i.EmpId == EmpId && i.Isdel == false);
                if (checkEmpId.Succeeded)
                {
                    if (checkEmpId.Data != null)
                    {
                        var item = new EmpIdChangedVM
                        {
                            EmpId = checkEmpId.Data.EmpId,
                            EmpName = checkEmpId.Data.EmpName,
                            EmpName2 = checkEmpId.Data.EmpName2,
                        };
                        return Ok(await Result<EmpIdChangedVM>.SuccessAsync(item));

                    }
                    else
                    {
                        return Ok(await Result<EmpIdChangedVM>.SuccessAsync(localization.GetResource1("EmployeeNotFound")));

                    }
                }
                return Ok(await Result<EmpIdChangedVM>.FailAsync($"{checkEmpId.Status.message}"));
            }
            catch (Exception exp)
            {
                return Ok(await Result<EmpIdChangedVM>.FailAsync($"{exp.Message}"));
            }
        }

        [HttpGet("CandidateIdChanged")]
        public async Task<IActionResult> CandidateIdChanged(long Id)
        {
            var chk = await permission.HasPermission(579, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (Id <= 0)
            {
                return Ok(await Result<string>.SuccessAsync("there is No Cadidate with this Id"));
            }
            try
            {
                var checkCandidateId = await hrServiceManager.HrRecruitmentCandidateService.GetOne(i => i.Id == Id);
                if (checkCandidateId.Succeeded)
                {
                    if (checkCandidateId.Data != null)
                    {
                        return Ok(await Result<string>.SuccessAsync(checkCandidateId.Data.Name ?? ""));

                    }
                    else
                    {
                        return Ok(await Result<string>.SuccessAsync(localization.GetResource1("EmployeeNotFound")));

                    }
                }
                return Ok(await Result<string>.FailAsync($"{checkCandidateId.Status.message}"));
            }
            catch (Exception exp)
            {
                return Ok(await Result<string>.FailAsync($"{exp.Message}"));
            }
        }
        [HttpGet("TemplateIDChanged")]
        public async Task<IActionResult> TemplateIDChanged(long Id)
        {
            var chk = await permission.HasPermission(579, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (Id <= 0)
            {
                return Ok(await Result<string>.SuccessAsync("there is No Cadidate with this Id"));
            }
            try
            {
                var getData = await hrServiceManager.HrKpiTemplatesCompetenceService.GetAll(x => x.TemplateId == Id && x.IsDeleted == false);
                if (getData.Succeeded)
                {
                    if (getData.Data != null)
                    {
                        return Ok(await Result<IEnumerable<HrKpiTemplatesCompetenceDto>>.SuccessAsync(getData.Data));

                    }
                }
                return Ok(await Result<HrKpiTemplatesCompetenceDto>.FailAsync($"{getData.Status.message}"));
            }
            catch (Exception exp)
            {
                return Ok(await Result<string>.FailAsync($"{exp.Message}"));
            }
        }
    }

}