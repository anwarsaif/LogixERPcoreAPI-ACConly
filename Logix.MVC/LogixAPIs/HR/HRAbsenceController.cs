using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Helpers;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.HR.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.HR
{
    // الغياب 
    public class HRAbsenceController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IMainServiceManager mainServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;


        public HRAbsenceController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IMainServiceManager mainServiceManager)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.mainServiceManager = mainServiceManager;
        }
        #region الشاشة الرئيسيية
        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrAbsenceFilterDto filter)
        {
            var chk = await permission.HasPermission(170, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var items = await hrServiceManager.HrAbsenceService.Search(filter);
                return Ok(items);

			}
            catch (Exception ex)
            {
                return Ok(await Result<List<HrAbsenceFilterDto>>.FailAsync(ex.Message));
            }
        }

        [HttpPost("GetPagination")]
        public async Task<IActionResult> GetPagination([FromBody] HrAbsenceFilterDto filter, int take = 5, long? lastSeenId = null)
        {
            try
            {
                var chk = await permission.HasPermission(170, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var BranchesList = session.Branches.Split(',');
                filter.BranchId ??= 0;
                filter.DeptId ??= 0;
                filter.Location ??= 0;
                filter.StatusId ??= 0;

                var dateConditions = new List<DateCondition>();
                dateConditions.Add(new DateCondition
                {
                    DatePropertyName = "AbsenceDate",
                    ComparisonOperator = ComparisonOperator.GreaterThanOrEqual,
                    StartDateString = filter.FromDate ?? ""
                });
                dateConditions.Add(new DateCondition
                {
                    DatePropertyName = "AbsenceDate",
                    ComparisonOperator = ComparisonOperator.LessThanOrEqual,
                    StartDateString = filter.ToDate ?? ""
                });

                var items = await hrServiceManager.HrAbsenceService.GetAllWithPaginationVW(selector: x => x.Id,
                expression: x => x.IsDeleted == false
                && (filter.BranchId != 0 ? x.BranchId == filter.BranchId : BranchesList.Contains(x.BranchId.ToString()))
                && (string.IsNullOrEmpty(filter.EmpCode) || x.EmpCode == filter.EmpCode)
                && (string.IsNullOrEmpty(filter.EmpName) || x.EmpName.ToLower().Contains(filter.EmpName.ToLower()) || x.EmpName2.ToLower().Contains(filter.EmpName.ToLower()))
                && (filter.DeptId == 0 || filter.DeptId == x.DeptId)
                && (filter.Location == 0 || filter.Location == x.Location)
                && (filter.StatusId == 0 || filter.StatusId == x.StatusId),
                    take: take,
                    lastSeenId: lastSeenId,
                   dateConditions: (string.IsNullOrEmpty(filter.FromDate) || string.IsNullOrEmpty(filter.ToDate)) ? null : dateConditions);

                if (!items.Succeeded)
                    return Ok(await Result<List<HrAbsenceVw>>.FailAsync(items.Status.message));

                var paginatedData = new PaginatedResult<object>
                {
                    Succeeded = items.Succeeded,
                    Data = items.Data,
                    Status = items.Status,
                    PaginationInfo = items.PaginationInfo
                };
                return Ok(paginatedData);

            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long AbsenceId)
        {
            try
            {
                var chk = await permission.HasPermission(170, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (AbsenceId <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));
                var delete = await hrServiceManager.HrAbsenceService.NewRemove(AbsenceId);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete Hr Absence Controller, MESSAGE: {ex.Message}"));
            }
        }

        [HttpDelete("DeleteGroup")]
        public async Task<IActionResult> DeleteGroup(List<long> AbsenceIds)
        {
            try
            {
                var chk = await permission.HasPermission(170, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!AbsenceIds.Any())
                    return Ok(await Result.FailAsync(localization.GetMessagesResource("SelectRecordsTobeDeleted")));

                var delete = await hrServiceManager.HrAbsenceService.Remove(AbsenceIds);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete Hr Absence Controller, MESSAGE: {ex.Message}"));
            }
        }


        #endregion

        #region شاشة غياب يوم جديد

        //////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// /
        /// هذا الكود يتم استدعاءه عند التغيير في نوع المخالفة ويشترط توفر جميع الباراميترات
        /// </summary>
        /// <param name="EmpCode">كود الموظف </param>
        /// <param name="AbsenceDate">تاريخ الغياب </param>
        /// <param name="DisciplinaryCaseID">نوع المخالفة </param>
        /// <returns></returns>
        /// 

        [HttpGet("DisciplinaryCaseIDChanged")]
        public async Task<IActionResult> DisciplinaryCaseIDChanged(string EmpCode, string AbsenceDate, int DisciplinaryCaseID)
        {

            if (string.IsNullOrEmpty(EmpCode))
            {
                return Ok(await Result<string>.SuccessAsync("You Should Select Employee"));
            }
            if (string.IsNullOrEmpty(AbsenceDate))
            {
                return Ok(await Result<string>.SuccessAsync("يجب تحديد تاريخ الغياب "));
            }

            try
            {
                var getItem = await hrServiceManager.HrAbsenceService.DisciplinaryCaseIDChanged(EmpCode, AbsenceDate, DisciplinaryCaseID);
                return Ok(getItem);

            }

            catch (Exception exp)
            {
                return Ok(await Result<EmpIdChangedVM>.FailAsync($"{exp.Message}"));
            }
        }

        [HttpPost("Add")]
        public async Task<ActionResult> Add(HrAbsenceAddDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(170, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (string.IsNullOrEmpty(obj.EmpCode)) return Ok(await Result.FailAsync($"يجب ادخال رقم  الموظف"));
                if (string.IsNullOrEmpty(obj.AbsenceDate)) return Ok(await Result.FailAsync($"يجب ادخال التاريخ "));
                if (obj.Type <= 0) return Ok(await Result.FailAsync($"يجب ادخال  سبب الغياب  "));
                if (obj.ApplyPlenties)
                {
                    if (obj.DisciplinaryCaseId <= 0) return Ok(await Result.FailAsync($"يجب ادخال نوع المخالفة "));

                }
                var add = await hrServiceManager.HrAbsenceService.AddSingleAbsence(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add Hr Absence  Controller, MESSAGE: {ex.Message}"));
            }
        }


        #endregion

        #region this Region is for page of name Add From Excel (اضافة من الاكسل)

        [HttpPost("AddAbsenceFromExcel")]
        public async Task<ActionResult> AddAbsenceFromExcel(List<HrAbsenceAddDto> obj)
        {
            try
            {
                var chk = await permission.HasPermission(170, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));
                if (!obj.Any())
                {
                    return Ok(await Result.FailAsync("لا يوجد بيانات في ملف الإكسل "));

                }
                foreach (var item in obj)
                {
                    if (string.IsNullOrEmpty(item.EmpCode))
                        return Ok(await Result.FailAsync("يجب ادخال رقم الموظف"));
                }
                foreach (var item in obj)
                {
                    if (string.IsNullOrEmpty(item.AbsenceDate))
                        return Ok(await Result.FailAsync($"يجب ادخال التاريخ  للموظف رقم :{item.EmpCode} "));
                }
                foreach (var item in obj)
                {
                    if (item.Type != 1 && item.Type != 2)
                        return Ok(await Result.FailAsync($"يجب ادخال نوع الغياب  للموظف رقم :{item.EmpCode} "));
                }
                foreach (var item in obj)
                {
                    if (item.ApplyPlenties&&item.DisciplinaryCaseId<=0)
                        return Ok(await Result.FailAsync($"يجب ادخال رقم المخالفة  للموظف رقم :{item.EmpCode} "));
                }
                var add = await hrServiceManager.HrAbsenceService.AddAbsenceFromExcel(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add Hr Absence Controller in function AddAbsenceFromExcel, MESSAGE: {ex.Message}"));
            }
        }

        #endregion

        #region this region for page of name Absence_NotAttendance (تغيب الموظفين الغير حاضرين)


        [HttpPost("AddAbsenceForNotAttendance")]
        public async Task<ActionResult> AddAbsenceForNotAttendance(AbsenceNotAttendanceDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(170, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                obj.CMDTYPE = 10;
                if (string.IsNullOrEmpty(obj.FromDate)) return Ok(await Result.FailAsync($"يجب ادخال من تاريخ"));
                if (string.IsNullOrEmpty(obj.ToDate)) return Ok(await Result.FailAsync($"يجب ادخال الى تاريخ "));
                var add = await hrServiceManager.HrAbsenceService.AbsenceNotAttendance(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add Hr Absence  Controller, MESSAGE: {ex.Message}"));
            }
        }
        #endregion

        #region this region is for page of name Absence_Add2 (غياب فترة جديد)
        [HttpPost("AbsenceForNewInterval")]
        public async Task<ActionResult> AbsenceForNewInterval(HrAbsenceAddDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(170, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (string.IsNullOrEmpty(obj.EmpCode)) return Ok(await Result.FailAsync(localization.GetResource1("EmployeeNotFound")));
                if (string.IsNullOrEmpty(obj.AbsenceDate)) return Ok(await Result.FailAsync(localization.GetMessagesResource("RequiredFromDate")));
                if (string.IsNullOrEmpty(obj.ToDate)) return Ok(await Result.FailAsync(localization.GetMessagesResource("RequiredToDate")));
                if (obj.Type <= 0) return Ok(await Result.FailAsync(localization.GetMessagesResource("RequiredReasonAbsence")));
                if (obj.DaysCount <= 0) return Ok(await Result.FailAsync(localization.GetMessagesResource("RequiredAbsenceDays")));
                if (DateHelper.StringToDate(obj.ToDate) < DateHelper.StringToDate(obj.AbsenceDate)) return Ok(await Result.FailAsync(localization.GetMessagesResource("StartdateGreaterThanEnddate")));
                if (((DateHelper.StringToDate(obj.ToDate) - DateHelper.StringToDate(obj.AbsenceDate)).Days) + 1 != obj.DaysCount) return Ok(await Result.FailAsync(localization.GetMessagesResource("DifferenceBWNStartEndDates")));
                if (obj.ApplyPlenties)
                {
                    if (obj.DisciplinaryCaseId <= 0) return Ok(await Result.FailAsync(localization.GetMessagesResource("RequiredDisciplinaryCaseId")));
                }
                var add = await hrServiceManager.HrAbsenceService.AbsenceForNewInterval(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add Hr Absence  Controller, MESSAGE: {ex.Message}"));
            }
        }
        [HttpGet("CountDays")]
        public async Task<ActionResult> CountDays(string SDate, string EDate)
        {
            try
            {
                if(string.IsNullOrEmpty(SDate)) return Ok(await Result.FailAsync(localization.GetMessagesResource("RequiredFromDate")));
                if (string.IsNullOrEmpty(EDate)) return Ok(await Result.FailAsync(localization.GetMessagesResource("RequiredToDate")));
                DateTime startDate =DateHelper.StringToDate(SDate);
                DateTime endDate = DateHelper.StringToDate(EDate);

                TimeSpan duration = endDate - startDate;
                int daysCount = duration.Days + 1;

                return Ok(await Result<int>.SuccessAsync(daysCount,""));        

            }
            catch (Exception ex)
            {

                return Ok(await Result.FailAsync($"====== Exp in Add Hr Absence  Controller, MESSAGE: {ex.Message}"));
                ;
            }
          
        }


        #endregion

        #region this region is for page of name Absence_Add3 (غياب متعدد)
        [HttpPost("MultiAbsenceAdd")]
        public async Task<ActionResult> MultiAbsenceAdd(HrMultiAbsenceAddDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(170, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (!obj.EmpCode.Any() || obj.EmpCode.All(string.IsNullOrEmpty))
                {
                    return Ok(await Result.FailAsync("يجب تحديد موظفين"));
                }
                if (string.IsNullOrEmpty(obj.AbsenceDate)) return Ok(await Result.FailAsync($" يجب ادخال  تاريخ الغياب  "));
                if (obj.Type <= 0) return Ok(await Result.FailAsync($"يجب ادخال  سبب الغياب  "));
                if (obj.ApplyPlenties)
                {
                    if (obj.DisciplinaryCaseId <= 0) return Ok(await Result.FailAsync($"يجب ادخال نوع المخالفة "));
                }
                var add = await hrServiceManager.HrAbsenceService.MultiAbsenceAdd(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add Hr Absence  Controller, MESSAGE: {ex.Message}"));
            }
        }


        [HttpPost("MultiAbsenceSearch")]
        public async Task<IActionResult> MultiAbsenceSearch(HrMultiAbsenceAddFilterDto filter)
        {
            var chk = await permission.HasPermission(170, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                List<HrMultiAbsenceAddFilterDto> result = new List<HrMultiAbsenceAddFilterDto>();
                var BranchesList = session.Branches.Split(',');
                var items = await hrServiceManager.HrEmployeeService.GetAllVW(x => x.IsDeleted == false && x.Isdel == false && BranchesList.Contains(x.BranchId.ToString())
                && (string.IsNullOrEmpty(filter.EmpCode) || x.EmpId == filter.EmpCode)
                && (string.IsNullOrEmpty(filter.EmpName) || x.EmpName.ToLower().Contains(filter.EmpName.ToLower()) || x.EmpName2.ToLower().Contains(filter.EmpName.ToLower()))
                && (filter.DeptId == null || filter.DeptId == 0 || filter.DeptId == x.DeptId)
                && (filter.Location == null || filter.Location == 0 || filter.Location == x.Location)
                );
                if (items.Succeeded)
                {
                    if (items.Data.Count() > 0)
                    {

                        var res = items.Data.AsQueryable();

                        if (filter.BranchId != null && filter.BranchId > 0)
                        {
                            res = res.Where(c => c.BranchId != null && c.BranchId.Equals(filter.BranchId));
                        }


                        if (res.Any())
                        {

                            foreach (var item in res)
                            {
                                var newItem = new HrMultiAbsenceAddFilterDto
                                {
                                    EmpCode = item.EmpId,
                                    EmpName = item.EmpName,
                                    EmpNameEn = item.EmpName2,
                                    DeptName = session.Language == 1 ? item.DepName : item.DepName2,
                                    IDNumber = item.IdNo,
                                    CatName = session.Language == 1 ? item.CatName : item.CatName2

                                };
                                result.Add(newItem);
                            }
                            return Ok(await Result<List<HrMultiAbsenceAddFilterDto>>.SuccessAsync(result, ""));

                        }
                        return Ok(await Result<List<HrMultiAbsenceAddFilterDto>>.SuccessAsync(result, localization.GetResource1("NosearchResult")));
                    }
                    return Ok(await Result<List<HrMultiAbsenceAddFilterDto>>.SuccessAsync(result, localization.GetResource1("NosearchResult")));
                }
                return Ok(await Result<List<HrMultiAbsenceAddFilterDto>>.SuccessAsync(result, localization.GetResource1("NosearchResult")));
            }
            catch (Exception ex)
            {
                return Ok(await Result<List<HrMultiAbsenceAddFilterDto>>.FailAsync(ex.Message));
            }
        }
        #endregion
    }
}