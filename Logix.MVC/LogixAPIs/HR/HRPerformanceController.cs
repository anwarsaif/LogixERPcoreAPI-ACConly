using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Services;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.HR.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.HR
{
    //   فترة التقيم
    public class HRPerformanceController : BaseHrApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IWFServiceManager wFServiceManager;

        public HRPerformanceController(IHrServiceManager hrServiceManager, IMainServiceManager mainServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IWFServiceManager wFServiceManager)
        {
            this.mainServiceManager = mainServiceManager;
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.wFServiceManager = wFServiceManager;
        }

        #region Main Page



        [HttpPost("Search")]

        public async Task<IActionResult> Search(HrPerformanceFilterDto filter)
        {
            var chk = await permission.HasPermission(997, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                List<HrPerformanceFilterDto> resultList = new List<HrPerformanceFilterDto>();
                long FacilityId = session.FacilityId;
                filter.StatusId ??= 0;
                filter.TypeId ??= 0;

                var items = await hrServiceManager.HrPerformanceService.GetAllVW(x => x.IsDeleted == false && x.DueDate != null && x.DueDate != ""
                && (x.FacilityId == FacilityId)
                && (filter.TypeId == 0 || filter.TypeId == x.TypeId)
                && (filter.StatusId == 0 || filter.StatusId == x.StatusId)

                );
                if (items.Succeeded)
                {
                    if (items.Data.Count() > 0)
                    {
                        var res = items.Data.AsQueryable();
                        if (!string.IsNullOrEmpty(filter.FromDate) && !string.IsNullOrEmpty(filter.ToDate))
                        {
                            res = res.Where(c => DateHelper.StringToDate(c.DueDate) >= DateHelper.StringToDate(filter.FromDate) && DateHelper.StringToDate(c.DueDate) <= DateHelper.StringToDate(filter.ToDate));
                        }
                        if (res.Count() > 0)
                        {
                            var getKPIData = await hrServiceManager.HrKpiService.GetAllVW(x => x.IsDeleted == false);
                            foreach (var item in res)
                            {

                                var newRow = new HrPerformanceFilterDto
                                {
                                    Id = item.Id,
                                    EvaluatedCount = getKPIData.Data.Count(x => x.PerformanceId == item.Id),
                                    StartDate = item.StartDate ?? "",
                                    EndDate = item.EndDate ?? "",
                                    StatusName = item.StatusName ?? "",
                                    StatusName2 = item.StatusName2 ?? "",
                                    DepartmentsIds = item.DepartmentsIds,
                                    Description = item.Description ?? "",
                                    DueDate = item.DueDate ?? "",
                                    EvaluationName = item.EvaluationName ?? "",
                                    EvaluationName2 = item.EvaluationName2 ?? "",
                                    EvaluationFor = item.EvaluationFor,
                                    JobsCatIds = item.JobsCatIds,
                                    TypeName = item.TypeName ?? "",
                                    TypeName2 = item.TypeName2 ?? "",
                                };
                                resultList.Add(newRow);
                            }
                            return Ok(await Result<object>.SuccessAsync(resultList, ""));

                        }
                        return Ok(await Result<object>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));

                    }

                    return Ok(await Result<object>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));

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
                var chk = await permission.HasPermission(997, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await hrServiceManager.HrPerformanceService.Remove(Id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete HrPerformanceController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long Id = 0)
        {
            var chk = await permission.HasPermission(997, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result<object>.FailAsync($"Access Denied"));
            }
            if (Id <= 0)
            {
                return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
            }
            try
            {
                var result = await hrServiceManager.HrPerformanceService.GetOneVW(I => I.IsDeleted == false && I.Id == Id);
                return Ok(result);
            }
            catch (Exception exp)
            {
                return Ok(await Result<object>.FailAsync($"EXP in {this.GetType()}, Meesage: {exp.Message}"));
            }
        }


        #endregion


        #region AddPage

        [HttpPost("Add")]
        public async Task<IActionResult> Add(HrPerformanceDto obj)
        {
            var chk = await permission.HasPermission(997, PermissionType.Add);
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
                if (obj.EvaluationFor <= 0)
                {
                    return Ok(await Result<object>.FailAsync($"ادخل الموظفين المراد تقييمهم  "));

                }
                if (obj.TypeId <= 0)
                {
                    return Ok(await Result<object>.FailAsync($"ادخل النوع  "));

                }

                if (string.IsNullOrEmpty(obj.StartDate))
                {
                    return Ok(await Result<object>.FailAsync($"ادخل من تاريخ "));
                }

                if (string.IsNullOrEmpty(obj.EndDate))
                {
                    return Ok(await Result<object>.FailAsync($"ادخل الى تاريخ "));
                }

                if (string.IsNullOrEmpty(obj.Description))
                {
                    return Ok(await Result<object>.FailAsync($"ادخل الوصف "));
                }
                if (string.IsNullOrEmpty(obj.DueDate))
                {
                    return Ok(await Result<object>.FailAsync($"ادخل \r\nأخر موعد للتقييم "));
                }



                var addRes = await hrServiceManager.HrPerformanceService.Add(obj);
                return Ok(addRes);
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync(ex.Message));
            }
        }

        #endregion


        #region EditPage

        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(HrPerformanceEditDto obj)
        {
            var chk = await permission.HasPermission(997, PermissionType.Add);
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
                if (obj.EvaluationFor <= 0)
                {
                    return Ok(await Result<object>.FailAsync($"ادخل الموظفين المراد تقييمهم  "));

                }
                if (obj.TypeId <= 0)
                {
                    return Ok(await Result<object>.FailAsync($"ادخل النوع  "));

                }

                if (string.IsNullOrEmpty(obj.StartDate))
                {
                    return Ok(await Result<object>.FailAsync($"ادخل من تاريخ "));
                }

                if (string.IsNullOrEmpty(obj.EndDate))
                {
                    return Ok(await Result<object>.FailAsync($"ادخل الى تاريخ "));
                }

                if (string.IsNullOrEmpty(obj.Description))
                {
                    return Ok(await Result<object>.FailAsync($"ادخل الوصف "));
                }
                if (string.IsNullOrEmpty(obj.DueDate))
                {
                    return Ok(await Result<object>.FailAsync($"ادخل \r\nأخر موعد للتقييم "));
                }


                var addRes = await hrServiceManager.HrPerformanceService.Update(obj);
                return Ok(addRes);
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync(ex.Message));
            }
        }


        #endregion

        #region SharedFunctions
        //  ارسال اشعار  ل  لتقيم  الموظفين

        [HttpGet("SendNotificationsEvaluation")]
        public async Task<IActionResult> SendNotificationsEvaluation(long Id = 0)
        {
            var chk = await permission.HasPermission(997, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result<object>.FailAsync($"Access Denied"));
            }
            if (Id <= 0)
            {
                return Ok(await Result.FailAsync($"قم بعملية الحفظ اولاً"));
            }
            try
            {
                var result = await hrServiceManager.HrPerformanceService.SendNotificationsEvaluation(Id);
                return Ok(result);
            }
            catch (Exception exp)
            {
                return Ok(await Result<object>.FailAsync($"EXP in {this.GetType()}, Meesage: {exp.Message}"));
            }
        }


        // إرسال إشعار بإضافة مؤشرات أداء الموظف

        [HttpGet("SendEmployeePerformanceIndicators")]
        public async Task<IActionResult> SendEmployeePerformanceIndicators(long Id = 0)
        {
            var chk = await permission.HasPermission(997, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result<object>.FailAsync($"Access Denied"));
            }
            if (Id <= 0)
            {
                return Ok(await Result.FailAsync($"قم بعملية الحفظ اولاً"));
            }
            try
            {
                var result = await hrServiceManager.HrPerformanceService.SendEmployeePerformanceIndicators(Id);
                return Ok(result);
            }
            catch (Exception exp)
            {
                return Ok(await Result<object>.FailAsync($"EXP in {this.GetType()}, Meesage: {exp.Message}"));
            }
        }
        #endregion
    }
}