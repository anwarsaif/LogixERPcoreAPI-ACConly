using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Infrastructure.Repositories;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Result = Logix.Application.Wrapper.Result;

namespace Logix.MVC.LogixAPIs.HR
{
    // مؤشرات اهداف الموظفين
    public class HRHEmpGoalIndicatorsController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IMainServiceManager mainServiceManager;
        private readonly IAccServiceManager accServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ILocalizationService localization;
        private readonly ICurrentData session;

        public HRHEmpGoalIndicatorsController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ILocalizationService localization, ICurrentData session, IMainServiceManager mainServiceManager, IAccServiceManager accServiceManager)
        {
            this.permission = permission;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.session = session;
            this.mainServiceManager = mainServiceManager;
            this.accServiceManager = accServiceManager;
        }

        #region الصفحة الرئيسية



        [HttpPost("Search")]

        public async Task<IActionResult> Search(int? EvaluationPeriod, int? KPITemplat)
        {
            var chk = await permission.HasPermission(2062, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                List<EmployeeGoalIndicatorFilterDto> resultList = new List<EmployeeGoalIndicatorFilterDto>();
                EvaluationPeriod ??= 0;
                KPITemplat ??= 0;
                var items = await hrServiceManager.HrEmpGoalIndicatorService.GetAll(x => x.IsDeleted == false
                && (EvaluationPeriod == 0 || EvaluationPeriod == x.PeriodId)
                && (KPITemplat == 0 || KPITemplat == x.KpiTemplatesId)

                );
                if (items.Succeeded)
                {
                    if (items.Data.Count() > 0)
                    {
                        var res = items.Data.AsQueryable();

                        var HrPerformanceItems = await hrServiceManager.HrPerformanceService.GetAll(e => e.IsDeleted == false);
                        var KPITemplatesItems = await hrServiceManager.HrKpiTemplateService.GetAll(e => e.IsDeleted == false);



                        if (res.Count() > 0)
                        {
                            foreach (var item in items.Data)
                            {

                                var Performance = HrPerformanceItems.Data.Where(x => x.Id == item.PeriodId).FirstOrDefault();
                                var Templates = KPITemplatesItems.Data.Where(x => x.Id == item.KpiTemplatesId).FirstOrDefault();
                                var newItem = new EmployeeGoalIndicatorFilterDto
                                {
                                    Id = item.Id,
                                    KpiTemplateName = Templates.Name ?? "",
                                    EvaluationPeriod = "فترة التقييم من " + Performance.StartDate + "  الى  " + Performance.EndDate,
                                };
                                resultList.Add(newItem);
                            }

                            return Ok(await Result<List<EmployeeGoalIndicatorFilterDto>>.SuccessAsync(resultList, ""));

                        }
                        return Ok(await Result<List<EmployeeGoalIndicatorFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));




                    }
                    return Ok(await Result<List<EmployeeGoalIndicatorFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));

                }

                return Ok(await Result<HrGosiFilterDto>.FailAsync(items.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrGosiFilterDto>.FailAsync(ex.Message));
            }
        }


        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long Id = 0)
        {
            var chk = await permission.HasPermission(2062, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (Id == 0)
            {
                return Ok(await Result.FailAsync($"{localization.GetMessagesResource("ChooseDelete")}"));
            }

            try
            {
                var del = await hrServiceManager.HrEmpGoalIndicatorService.Remove(Id);

                return Ok(del);
            }
            catch (Exception exp)
            {
                return Ok(await Result.FailAsync($"{exp.Message}"));
            }
        }



        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long? Id)
        {
            var chk = await permission.HasPermission(2062, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (Id <= 0)
                return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));


            try
            {
                HrEmpGoalIndicatorAddDto result = new HrEmpGoalIndicatorAddDto();
                result.Competence = new List<GoalIndicatorsCompetenceDto>();
                result.Employee = new List<GoalIndicatorsEmployeeDto>();

                var GetEmpGoalIndicators = await hrServiceManager.HrEmpGoalIndicatorService.GetOne(x => x.Id == Id && x.IsDeleted == false);
                if (!GetEmpGoalIndicators.Succeeded)
                    return Ok(await Result<HrEmpGoalIndicatorAddDto>.FailAsync($"{GetEmpGoalIndicators.Status.message}"));

                if (GetEmpGoalIndicators.Data == null)
                    return Ok(await Result<HrEmpGoalIndicatorAddDto>.FailAsync("موشر الأداء غير موجود"));
                result.KpiTemplatesId = GetEmpGoalIndicators.Data.KpiTemplatesId;
                result.PeriodId = GetEmpGoalIndicators.Data.PeriodId;
                result.Id = GetEmpGoalIndicators.Data.Id;
                ////////////////////////////////////////////////////////////
                var GetEmpGoalIndicatorsEmployee = await hrServiceManager.HrEmpGoalIndicatorsEmployeeService.GetAllVW(x => x.GoalIndicatorsId == Id && x.IsDeleted == false);

                if (GetEmpGoalIndicatorsEmployee.Data != null)
                {
                    foreach (var item in GetEmpGoalIndicatorsEmployee.Data)
                    {
                        var newEmp = new GoalIndicatorsEmployeeDto
                        {

                            Id = item.Id,
                            EmpCode = item.EmpCode ?? "",
                            EmpName = item.EmpName ?? "",
                            IsDeleted = item.IsDeleted,

                        };
                        result.Employee.Add(newEmp);
                    }
                }

                var GetEmpGoalIndicatorsCompetence = await hrServiceManager.HrEmpGoalIndicatorsCompetenceService.GetAll(x => x.GoalIndicatorsId == Id && x.IsDeleted == false);


                if (GetEmpGoalIndicatorsCompetence.Data != null)
                {
                    foreach (var item in GetEmpGoalIndicatorsCompetence.Data)
                    {
                        var newComptence = new GoalIndicatorsCompetenceDto
                        {
                            Target = Convert.ToInt32(item.Target),
                            CompetencesId = item.CompetencesId,
                            Id = item.Id,
                            Note = item.Note ?? "",
                            Weight = Convert.ToInt32(item.Weight),
                            IsDeleted = item.IsDeleted,
                        };
                        result.Competence.Add(newComptence);
                    }
                }

                return Ok(await Result<HrEmpGoalIndicatorAddDto>.SuccessAsync(result, "", 200));


            }
            catch (Exception exp)
            {
                return Ok(await Result<HrEmpGoalIndicatorAddDto>.FailAsync($"{exp.Message}"));
            }
        }

        #endregion

        #region صفحة التعديل


        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(HrEmpGoalIndicatorAddDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(2062, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));


                if (obj.Employee.Where(x => x.IsDeleted == false).Count() < 1)
                    return Ok(await Result.FailAsync("قم باضافة الموظفين اولاً"));


                if (obj.Employee.Where(x => x.IsDeleted == false).GroupBy(e => e.EmpCode).Any(g => g.Count() > 1))
                    return Ok(await Result.FailAsync("لا يمكنك تكرار الموظفين"));

                if (obj.Competence.Where(x => x.IsDeleted == false).Count() < 1)
                    return Ok(await Result.FailAsync("قم باضافة المؤشرات اولاً"));
                if (obj.Competence.Where(x => x.IsDeleted == false).GroupBy(e => e.CompetencesId).Any(g => g.Count() > 1))
                    return Ok(await Result.FailAsync("لا يمكنك تكرار المؤشر"));

                if (obj.Employee.Any(e => string.IsNullOrEmpty(e.EmpCode)))
                    return Ok(await Result.FailAsync("أرقم الموظفين مطلوبة"));

                if (obj.PeriodId <= 0)
                    return Ok(await Result.FailAsync("قم بتحديد فترة التقيم "));

                if (obj.KpiTemplatesId <= 0)
                    return Ok(await Result.FailAsync("قم بتحديد نموذج التقييم "));

                if (obj.Competence.Any(e => e.Weight <= 0))
                    return Ok(await Result.FailAsync(" يجب ادخال الوزن لكل كفاءة"));

                if (obj.Competence.Any(e => e.Target <= 0))
                    return Ok(await Result.FailAsync(" يجب ادخال القيمة المستهدفة لكل كفاءة"));

                var add = await hrServiceManager.HrEmpGoalIndicatorService.Edit(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"====== Exp in    HRHEmpGoalIndicators Controller  edit Method, MESSAGE: {ex.Message}"));
            }
        }

        #endregion


        #region صفحة الاضافة

        // الشاشة تستدعى في أكثر من مكان
        [HttpGet("AddGetMethod")]
        public async Task<IActionResult> AddGetMethod(long? Id = 0)
        {
            var chk = await permission.HasPermission(2062, PermissionType.Add);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (Id <= 0)
            {
                return Ok(await Result<object>.SuccessAsync(null));
            }

            try
            {
                List<HrEmpGoalIndicatorsCompetenceResultDto> IndicatorsCompetence = new List<HrEmpGoalIndicatorsCompetenceResultDto>();
                // GetEmpGoalIndicators
                var GetEmpGoalIndicators = await hrServiceManager.HrEmpGoalIndicatorService.GetOne(x => x.Id == Id && x.IsDeleted == false);
                if (GetEmpGoalIndicators.Succeeded)
                {
                    if (GetEmpGoalIndicators.Data == null)
                    {
                        return Ok(await Result<object>.FailAsync("موشر الأداء غير موجود"));


                    }
                    var GetEmpGoalIndicatorsEmployee = await hrServiceManager.HrEmpGoalIndicatorsEmployeeService.GetAllVW(x => x.GoalIndicatorsId == Id && x.IsDeleted == false);
                    var GetEmpGoalIndicatorsCompetence = await hrServiceManager.HrEmpGoalIndicatorsCompetenceService.GetAllVW(x => x.GoalIndicatorsId == Id && x.IsDeleted == false);
                    if (GetEmpGoalIndicatorsCompetence.Data != null)
                    {
                        var GetComptences = await hrServiceManager.HrCompetenceService.GetAll(x => x.IsDeleted == false);
                        foreach (var item in GetEmpGoalIndicatorsCompetence.Data)
                        {
                            var newComptence = new HrEmpGoalIndicatorsCompetenceResultDto
                            {
                                Target = item.Target,
                                CompetencesId = item.CompetencesId,
                                Id = item.Id,
                                Note = item.Note ?? "",
                                GoalIndicatorsId = item.GoalIndicatorsId,
                                Weight = item.Weight,
                                CompetencesName = GetComptences.Data.Where(x => x.Id == item.CompetencesId).Select(x => x.Name).FirstOrDefault()
                            };
                            IndicatorsCompetence.Add(newComptence);
                        }
                    }

                    return Ok(await Result<object>.SuccessAsync(new { GetEmpGoalIndicators = GetEmpGoalIndicators.Data, GetEmpGoalIndicatorsEmployee = GetEmpGoalIndicatorsEmployee.Data.ToList(), GetEmpGoalIndicatorsComptences = IndicatorsCompetence }));
                }
                else
                {
                    return Ok(await Result<object>.FailAsync($"{GetEmpGoalIndicators.Status.message}"));

                }
            }
            catch (Exception exp)
            {
                return Ok(await Result<object>.FailAsync($"{exp.Message}"));
            }
        }


        [HttpPost("Add")]
        public async Task<ActionResult> Add(HrEmpGoalIndicatorAddDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(2062, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (obj.Employee.Count < 1)
                    return Ok(await Result.FailAsync("قم باضافة الموظفين اولاً"));

                if (obj.Competence.Count < 1)
                    return Ok(await Result.FailAsync("قم باضافة المؤشرات اولاً"));
                if (obj.Competence.GroupBy(e => e.CompetencesId).Any(g => g.Count() > 1))
                    return Ok(await Result.FailAsync("لا يمكنك تكرار المؤشر"));

                if (obj.Employee.Any(e => string.IsNullOrEmpty(e.EmpCode)))
                    return Ok(await Result.FailAsync("أرقم الموظفين مطلوبة"));

                if (obj.Employee.GroupBy(e => e.EmpCode).Any(g => g.Count() > 1))
                    return Ok(await Result.FailAsync("لا يمكنك تكرار الموظفين"));

                if (obj.PeriodId <= 0)
                    return Ok(await Result.FailAsync("قم بتحديد فترة التقيم "));

                if (obj.KpiTemplatesId <= 0)
                    return Ok(await Result.FailAsync("قم بتحديد نموذج التقييم "));

                if (obj.Competence.Any(e => e.Weight <= 0))
                    return Ok(await Result.FailAsync(" يجب ادخال الوزن لكل كفاءة"));

                if (obj.Competence.Any(e => e.Target <= 0))
                    return Ok(await Result.FailAsync(" يجب ادخال القيمة المستهدفة لكل كفاءة"));

                var add = await hrServiceManager.HrEmpGoalIndicatorService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"====== Exp in Add   HRHEmpGoalIndicators Controller  Add Method, MESSAGE: {ex.Message}"));
            }
        }


        [HttpPost("AddEmployee")]
        public async Task<ActionResult> AddEmployee(GoalIndicatorsEmployeeDto obj)
        {
            try
            {
                obj.TemplateId ??= 0;
                var chk = await permission.HasPermission(2062, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (string.IsNullOrEmpty(obj.EmpCode))
                    return Ok(await Result.FailAsync(localization.GetResource1("EmployeeIsNumber")));

                var CheckEmpExist = await mainServiceManager.InvestEmployeeService.GetOne(x => x.EmpId == obj.EmpCode && x.IsDeleted == false && x.Isdel == false);
                if (CheckEmpExist.Data == null) return Ok(await Result<object>.FailAsync(localization.GetResource1("EmployeeNotFound")));
                var result = await hrServiceManager.HrEmpGoalIndicatorService.AddEmployee(CheckEmpExist.Data.Id, obj.TemplateId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"====== Exp in Add   HRHEmpGoalIndicators Controller  Add Method, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("AddKpi")]
        public async Task<IActionResult> AddKpi(HrCompetenceDto obj)
        {
            var chk = await permission.HasPermission(2062, PermissionType.Add);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                obj.TypeId = 2;
                var addRes = await hrServiceManager.HrCompetenceService.Add(obj);
                return Ok(addRes);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrCompetenceDto>.FailAsync(ex.Message));
            }
        }

        #endregion

    }
}
