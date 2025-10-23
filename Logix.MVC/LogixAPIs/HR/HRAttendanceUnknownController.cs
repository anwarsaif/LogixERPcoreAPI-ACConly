using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.HR
{

    //  البصمات الغير معروفة
    public class HRAttendanceUnknownController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;

        public HRAttendanceUnknownController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
        }

        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrAttendanceUnknownFilterDto filter)
        {
            var chk = await permission.HasPermission(1557, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
				//var BranchesList = session.Branches.Split(',');
				//List<HrAttendanceUnknownFilterDto> resultList = new List<HrAttendanceUnknownFilterDto>();
				//var items = await hrServiceManager.HrCheckInOutService.GetAllVW(e => e.IsSend == false && BranchesList.Contains(e.BranchId.ToString())&&e.Checktime!=null&&e.Checktype!=null);
				//if (items.Succeeded)
				//{
				//    if (items.Data.Count() > 0)
				//    {

				//        var res = items.Data.AsQueryable();
				//        if (!string.IsNullOrEmpty(filter.EmpCode))
				//        {
				//            res = res.Where(c => c.EmpCode != null && c.EmpCode == filter.EmpCode);
				//        }
				//        if (!string.IsNullOrEmpty(filter.EmpName))
				//        {
				//            res = res.Where(c => (c.EmpName != null && c.EmpName.ToLower().Contains(filter.EmpName.ToLower())));
				//        }
				//        if (!string.IsNullOrEmpty(filter.FromDate) && !string.IsNullOrEmpty(filter.ToDate))
				//        {
				//            res = res.Where(r => 
				//            (r.Checktime >= DateHelper.StringToDate(filter.FromDate)) &&
				//           (r.Checktime <= DateHelper.StringToDate(filter.ToDate))
				//           );
				//        }
				//        if (!string.IsNullOrEmpty(filter.TimeFrom) && !string.IsNullOrEmpty(filter.TimeTo))
				//        {
				//            filter.TimeFrom += ":00";
				//            filter.TimeTo += ":00";
				//            res = res.Where(r => r.Checktime.HasValue&&
				//            r.Checktime.Value.TimeOfDay >= TimeSpan.Parse(filter.TimeFrom) &&
				//            r.Checktime.Value.TimeOfDay <= TimeSpan.Parse(filter.TimeTo));
				//        }
				//        if (filter.BranchId != null && filter.BranchId > 0)
				//        {
				//            res = res.Where(c => c.BranchId != null && c.BranchId.Equals(filter.BranchId));
				//        }
				//        if (filter.Location != null && filter.Location > 0)
				//        {
				//            res = res.Where(c => c.Location != null && c.Location.Equals(filter.Location));
				//        }

				//        foreach (var item in res)
				//        {
				//            var newRecord = new HrAttendanceUnknownFilterDto
				//            {

				//                Id = item.Id,
				//                EmpCode = item.EmpCode,
				//                EmpName =session.Language==1? item.EmpName: item.EmpName2,
				//                DayName = session.Language == 1 ? item.DayName : item.DayName2,
				//                Checktime =item.Checktime,
				//                TimeText = item.Checktime.Value.TimeOfDay.ToString()??"00:00",
				//                CheckTypeName =( item.Checktype == 1) ? localization.GetHrResource("EntryTransaction") : localization.GetHrResource("ExitTransaction"),
				//            };
				//            resultList.Add(newRecord);
				//        }
				//        if (resultList.Any())
				//            return Ok(await Result<List<HrAttendanceUnknownFilterDto>>.SuccessAsync(resultList, ""));
				//        return Ok(await Result<List<HrAttendanceUnknownFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));
				//    }
				//    return Ok(await Result<List<HrAttendanceUnknownFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));
				//}
				//return Ok(await Result<HrAttendanceUnknownFilterDto>.FailAsync(items.Status.message));
				var items = await hrServiceManager.HrCheckInOutService.Search(filter);
				return Ok(items);

			}
            catch (Exception ex)
            {
                return Ok(await Result<HrAttendanceUnknownFilterDto>.FailAsync(ex.Message));
            }
        }



        [HttpGet("ChangeType")]
        public async Task<IActionResult> ChangeType(long Id)
        {
            try
            {
                var chk = await permission.HasPermission(1557, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id <= 0 )
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));
                }

                var item = await hrServiceManager.HrCheckInOutService.ChangeType(Id);
                return Ok(item);
            }
            catch (Exception ex)
            {
                return Ok(await Result<bool>.FailAsync($"====== Exp in HRAttendanceUnknownController ChangeStatus, MESSAGE: {ex.Message}"));
            }
        }


    }
}