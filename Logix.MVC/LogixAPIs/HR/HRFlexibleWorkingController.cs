using AutoMapper;
using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.Language;
using System.Globalization;

namespace Logix.MVC.LogixAPIs.HR
{
    //  ترحيل وإعتماد الدوام المرن
    public class HRFlexibleWorkingController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IPermissionHelper permission;
        public HRFlexibleWorkingController(IHrServiceManager hrServiceManager, ICurrentData session, ILocalizationService localization, IPermissionHelper permission)
        {
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.permission = permission;
        }
        #region Index Page

        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrFlexibleWorkingMasterFilterDto filter)
        {
            var chk = await permission.HasPermission(1840, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var BranchesList = session.Branches.Split(',');
                var items = await hrServiceManager.HrFlexibleWorkingMasterService.GetAll(e => e.IsDeleted == false &&
                !string.IsNullOrEmpty(e.DateFrom) &&
                !string.IsNullOrEmpty(e.DateTo) &&
                e.DateTo != "" &&
                (string.IsNullOrEmpty(filter.Code) || filter.Code == e.Code)
                );
                if (items.Succeeded)
                {
                    if (items.Data.Count() > 0)
                    {

                        var res = items.Data.AsQueryable();

                        if (!string.IsNullOrEmpty(filter.DateFrom) && !string.IsNullOrEmpty(filter.DateTo))
                        {
                            var FromDate = DateHelper.StringToDate(filter.DateFrom);
                            var ToDate = DateHelper.StringToDate(filter.DateTo);
                            res = res.Where(r =>
                            (DateHelper.StringToDate(r.DateFrom) >= FromDate && DateHelper.StringToDate(r.DateFrom) <= ToDate) ||
                           (DateHelper.StringToDate(r.DateTo) >= FromDate && DateHelper.StringToDate(r.DateTo) <= ToDate)
                           );
                        }

                        if (res.Any())
                            return Ok(await Result<List<HrFlexibleWorkingMasterDto>>.SuccessAsync(res.ToList(), ""));
                        return Ok(await Result<List<HrFlexibleWorkingMasterDto>>.SuccessAsync(res.ToList(), localization.GetResource1("NosearchResult")));
                    }
                    return Ok(await Result<List<HrFlexibleWorkingMasterDto>>.SuccessAsync(items.Data.ToList(), localization.GetResource1("NosearchResult")));
                }
                return Ok(await Result<HrFlexibleWorkingMasterDto>.FailAsync(items.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync(ex.Message));
            }
        }
        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long Id)
        {
            try
            {
                var chk = await permission.HasPermission(1840, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }
                var Items = await hrServiceManager.HrFlexibleWorkingService.GetAllVW(w => w.IsDeleted == false && w.MasterId == Id);
                if (Items.Data.Count() > 0)
                {
                    List<HrFlexibleWorkingResultDto> res = new List<HrFlexibleWorkingResultDto>();
                    foreach (var item in Items.Data)
                    {
                        var timeInString = item.TimeIn.HasValue ? item.TimeIn.Value.ToString("HH:mm", CultureInfo.InvariantCulture) : null;
                        var timeOutString = item.TimeOut.HasValue ? item.TimeOut.Value.ToString("HH:mm", CultureInfo.InvariantCulture) : null;
                        var actualMinutes = (timeInString != null && timeOutString != null) ? DateHelper.CalculateMinutesDifference(timeInString, timeOutString) : 0;
                        int totalMinutes = (item.TimeIn.HasValue && item.TimeOut.HasValue) ? (int)(item.TimeOut.Value - item.TimeIn.Value).TotalMinutes : 0;
                        var hours = (item.ActualMinute / 60);
                        var Minutes = (item.ActualMinute) - (hours * 60);
                        var newRecord = new HrFlexibleWorkingResultDto
                        {
                            Id = item.Id,
                            Minutes = actualMinutes,
                            ActualHours = $"{(actualMinutes / 60):D2}:{(actualMinutes % 60):D2}",
                            ActualMinute = actualMinutes,
                            DayDateGregorian = item.AttendanceDate,
                            EmpCode = item.EmpCode,
                            EmpName = session.Language == 1 ? item.EmpName : item.EmpName2,
                            DailyWorkingHours = item.DailyWorkingHours,
                            TimeInString = timeInString,
                            TimeOutString = timeOutString,
                            StringMinutes = $"{hours:D2}:{Minutes:D2}"
                        };
                        res.Add(newRecord);

                    }
                    return Ok(await Result<object>.SuccessAsync(res));

                }
                return Ok(Items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrOverTimeMGetByIdDto>.FailAsync($"====== Exp in HRFlexibleWorkingController getById, MESSAGE: {ex.Message}"));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var chk = await permission.HasPermission(1840, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await hrServiceManager.HrFlexibleWorkingMasterService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete HRFlexibleWorkingController, MESSAGE: {ex.Message}"));
            }
        }

        #endregion

        #region AddPage


        [HttpPost("Add")]
        public async Task<ActionResult> Add(HrFlexibleWorkingMasterAddDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(1840, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (string.IsNullOrEmpty(obj.DateFrom))
                    return Ok(await Result<object>.FailAsync($"يجب ادخال  من تاريخ"));
                if (string.IsNullOrEmpty(obj.DateTo))
                    return Ok(await Result<object>.FailAsync($"يجب ادخال الى تاريخ"));
                if (obj.Details.Any(x => (string.IsNullOrEmpty(x.TimeInString) || x.TimeInString == "00:00"))) return Ok(await Result<object>.FailAsync($"تأكد من أوقات الدخول للحقول المحددة"));
                if (obj.Details.Any(x => (string.IsNullOrEmpty(x.TimeOutString) || x.TimeOutString == "00:00"))) return Ok(await Result<object>.FailAsync($"تأكد من أوقات الخروج للحقول المحددة"));
                if (obj.Details.Any(x => (string.IsNullOrEmpty(x.DayDateGregorian)))) return Ok(await Result<object>.FailAsync($"تأكد من التاريخ للحقول المحددة"));

                if (obj.Details.Count() <= 0)
                    return Ok(await Result<object>.FailAsync($"لم يتم اختيار اي حركة لترحيل الدوام المرن  "));
                var add = await hrServiceManager.HrFlexibleWorkingMasterService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add Hr HRFlexibleWorkingController  Controller At Add, MESSAGE: {ex.Message}"));
            }
        }


        [HttpPost("SearchInAdd")]
        public async Task<IActionResult> SearchInAdd(HrFlexibleWorkingMasterFilterDto filter)
        {
            var chk = await permission.HasPermission(1840, PermissionType.Add);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                filter.Branch ??= 0;
                filter.Location ??= 0;
                filter.Dept ??= 0;
                if (filter.Branch == 0)
                {
                    filter.BranchIds = session.Branches;
                }
                else
                {
                    filter.BranchIds = "";
                }
                var result = await hrServiceManager.HrFlexibleWorkingService.SearchInAdd(filter);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync(ex.Message));
            }
        }



        #endregion

        #region EditPage


        [HttpPost("ApproveWork")]
        public async Task<ActionResult> ApproveWork(List<long> Ids)
        {
            try
            {
                var chk = await permission.HasPermission(1840, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (Ids.Count() <= 0)
                    return Ok(await Result<object>.FailAsync($"لم يتم اختيار اي حركة لترحيل الدوام المرن  "));

                var update = await hrServiceManager.HrFlexibleWorkingService.ApproveWork(Ids);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrOverTimeMEditDto>.FailAsync($"====== Exp in Edit HRFlexibleWorkingController ApproveWork, MESSAGE: {ex.Message}"));
            }
        }
        #endregion




    }

}