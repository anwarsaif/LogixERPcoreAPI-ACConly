using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.HR.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Result = Logix.Application.Wrapper.Result;

namespace Logix.MVC.LogixAPIs.HR
{
    // العطل الرسمية
    public class HRHolidayController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;


        public HRHolidayController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
        }


        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrHolidayFilterDto filter)
        {
            var chk = await permission.HasPermission(260, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                List<HrHolidayFilterDto> resultList = new List<HrHolidayFilterDto>();
                var items = await hrServiceManager.HrHolidayService.GetAllVW(e => e.IsDeleted == false
                && e.FacilityId == session.FacilityId
                && (string.IsNullOrEmpty(filter.HolidayName) || (e.HolidayName != null && e.HolidayName.Contains(filter.HolidayName)))
                );

                if (!items.Succeeded)
                    return Ok(await Result<object>.FailAsync(items.Status.message));

                if (!items.Data.Any())
                    return Ok(await Result<object>.SuccessAsync(new List<object>(), localization.GetResource1("NosearchResult")));
                var res = items.Data.AsQueryable();

                foreach (var item in res)
                {
                    var newRecord = new HrHolidayFilterDto
                    {
                        HolidayName = item.HolidayName,
                        HolidayId = item.HolidayId,
                        HolidayDateFrom = item.HolidayDateFrom,
                        HolidayDateTo = item.HolidayDateTo,
                    };
                    resultList.Add(newRecord);
                }
                return Ok(await Result<List<HrHolidayFilterDto>>.SuccessAsync(resultList, ""));

            }
            catch (Exception ex)
            {
                return Ok(await Result<HrTransferFilterDto>.FailAsync(ex.Message));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long Holidayid)
        {
            try
            {
                var chk = await permission.HasPermission(260, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Holidayid <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await hrServiceManager.HrHolidayService.Remove(Holidayid);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete HR Assignments Controller, MESSAGE: {ex.Message}"));
            }
        }


        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long HolidayId)
        {
            try
            {
                var chk = await permission.HasPermission(260, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (HolidayId <= 0)
                {
                    return Ok(await Result<HrHolidayVw>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var item = await hrServiceManager.HrHolidayService.GetOneVW(x => x.HolidayId == HolidayId);
                if (item.Succeeded)
                {
                    return Ok(item);
                }
                return Ok(await Result<HrHolidayVw>.FailAsync(item.Status.message));

            }
            catch (Exception ex)
            {
                return Ok(await Result<HrHolidayVw>.FailAsync($"====== Exp in HR Holiday Controller getById, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(HrHolidayEditDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(260, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result<HrHolidayEditDto>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));
                if (obj.HolidayDateFrom == null || obj.HolidayDateFrom == "")
                    return Ok(await Result<HrHolidayEditDto>.FailAsync("Holiday Date From  is Required"));
                if (obj.HolidayDateTo == null || obj.HolidayDateTo == "")
                    return Ok(await Result<HrHolidayEditDto>.FailAsync("Holiday Date To   is Required"));


                var update = await hrServiceManager.HrHolidayService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrHolidayEditDto>.FailAsync($"====== Exp in Edit HR Holiday  Controller, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Add")]
        public async Task<ActionResult> Add(HrHolidayDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(260, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result<HrHolidayDto>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));
                if (obj.HolidayDateFrom == null || obj.HolidayDateFrom == "")
                    return Ok(await Result<HrHolidayDto>.FailAsync("Holiday Date From  is Required"));
                if (obj.HolidayDateTo == null || obj.HolidayDateTo == "")
                    return Ok(await Result<HrHolidayDto>.FailAsync("Holiday Date To   is Required"));

                var add = await hrServiceManager.HrHolidayService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrHolidayDto>.FailAsync($"====== Exp in Add HR   Holiday Controller  Add Method, MESSAGE: {ex.Message}"));
            }
        }

    }
}