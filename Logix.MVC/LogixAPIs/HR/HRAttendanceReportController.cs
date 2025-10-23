using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Logix.MVC.LogixAPIs.HR
{
    //   تقرير بالحضور والانصراف
    public class HRAttendanceReportController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;

        public HRAttendanceReportController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
        }

        [HttpPost("Search")]
        public async Task<IActionResult> Search(HRAttendanceReportFilterDto filter)
        {
            var chk = await permission.HasPermission(189, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                //var BranchesList = session.Branches.Split(',');


                //if (string.IsNullOrEmpty(filter.EmpCode))
                //{
                //    filter.EmpCode = null;
                //}
                //if (string.IsNullOrEmpty(filter.EmpName))
                //{
                //    filter.EmpName = null;
                //}

                //if (filter.BranchId != null && filter.BranchId > 0)
                //{
                //    filter.BranchsId = "";

                //}

                //else
                //{
                //    filter.BranchId = 0;
                //    filter.BranchsId = session.Branches;
                //}
                //if (filter.ShitId <= 0)
                //{
                //    filter.ShitId = 0;

                //}
                //filter.TimeTableId = 0;

                //if (filter.DeptId == 0 || filter.DeptId == null)
                //{
                //    filter.DeptId = null;
                //}

                //if (filter.StatusId == 0 || filter.StatusId == null)
                //{
                //    filter.StatusId = null;
                //}
                //if (filter.Location == 0 || filter.Location == null)
                //{
                //    filter.Location = null;
                //}
                //if (filter.SponsorsId == 0 || filter.SponsorsId == null)
                //{
                //    filter.SponsorsId = null;
                //}
                //if (filter.AttendanceType == 0 || filter.AttendanceType == null)
                //{
                //    filter.AttendanceType = null;
                //}
                //if (!string.IsNullOrEmpty(filter.DayDateGregorian) && !string.IsNullOrEmpty(filter.DayDateGregorian2))
                //{
                //    filter.DayDateGregorian = filter.DayDateGregorian;
                //    filter.DayDateGregorian2 = filter.DayDateGregorian2;
                //}
                //else
                //{
                //    filter.DayDateGregorian = null;
                //    filter.DayDateGregorian2 = null;
                //}
                //filter.ManagerId = 0;

                var items = await hrServiceManager.HrAttendanceService.getHR_Attendance_Report_SP(filter);
                if (items.Succeeded)
                {
                    if (items.Data.Count() > 0)
                    {
                        //foreach (var item in items.Data)
                        //{
                        //    if (item.Delay > 0)
                        //    {
                        //        item.DelayAsString = (item.Delay / 60).ToString() + ":" + (item.Delay - ((item.Delay / 60) * 60)).ToString();
                        //    }
                        //    else
                        //    {
                        //        item.DelayAsString = "";
                        //    }

                        //    if (item.LeaveEarly > 0)
                        //    {
                        //        item.LeaveEarlyAsString = (item.LeaveEarly / 60).ToString() + ":" + (item.LeaveEarly - ((item.LeaveEarly / 60) * 60)).ToString();
                        //    }
                        //    else
                        //    {
                        //        item.LeaveEarlyString = "";
                        //    }
                        //}
                        return Ok(await Result<List<HRAttendanceReportDto>>.SuccessAsync(items.Data.ToList(), ""));
                    }
                    return Ok(await Result<List<HRAttendanceReportDto>>.SuccessAsync(items.Data.ToList(), localization.GetResource1("NosearchResult")));
                }
                return Ok(await Result<HRAttendanceReportDto>.FailAsync(items.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result<HRAttendanceReportDto>.FailAsync(ex.Message));
            }
        }
    }
}