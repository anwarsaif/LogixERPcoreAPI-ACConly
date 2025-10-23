using Logix.Application.Common;
using Logix.Application.DTOs.Main;
using Logix.Application.DTOs.RPT;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.Main;
using Logix.Domain.RPT;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Logix.MVC.LogixAPIs.Main
{
    public class CurrentUsersFilter
    {
        public int? UserTypeId { get; set; }
        public string? UserFullName { get; set; }
    }

    public class SysNotificationVm
    {
        public long Id { get; set; }
        public long? TableId { get; set; }
        public long? UserId { get; set; }
        public string? MsgTxt { get; set; }
        public bool? IsRead { get; set; }
        public DateTime? ReadDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public long? ActivityLogId { get; set; }
        [StringLength(50)]
        public string? UserFullname { get; set; }
        [StringLength(200)]
        public string? TableDescription { get; set; }
        public string? ScreenUrl { get; set; }
        public long? TablePrimarykey { get; set; }
        public string? Url { get; set; }
        public string? Date1 { get; set; }
    }

    public class ReportsController : BaseMainApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData currentData;
        private readonly IRptServiceManager rptServiceManager;
        private readonly IApiDDLHelper ddlHelper;

        public ReportsController(IMainServiceManager mainServiceManager,
            IPermissionHelper permission,
            ICurrentData currentData,
            IRptServiceManager rptServiceManager,
            IApiDDLHelper ddlHelper
            )
        {
            this.mainServiceManager = mainServiceManager;
            this.permission = permission;
            this.currentData = currentData;
            this.rptServiceManager = rptServiceManager;
            this.ddlHelper = ddlHelper;
        }

        //عداد الدخول
        [HttpPost("LoginCounter")]
        public async Task<ActionResult> LoginCounter(SysUserLogTimeSearchVm filter)
        {
            try
            {
                var chk = await permission.HasPermission(101, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var items = await mainServiceManager.SysUserLogTimeService.GetAllVW(a => (
                    filter.UserTypeId == null || (a.UserTypeId != null && a.UserTypeId == filter.UserTypeId))
                    && (string.IsNullOrEmpty(filter.LoginTime) || (a.LoginTime != null && a.LoginTime.Value.Date >= DateTime.ParseExact(filter.LoginTime, "yyyy/MM/dd", CultureInfo.InvariantCulture)))
                    && (string.IsNullOrEmpty(filter.LogoutTime) || (a.LoginTime != null && a.LoginTime.Value.Date <= DateTime.ParseExact(filter.LogoutTime, "yyyy/MM/dd", CultureInfo.InvariantCulture)))
                    );
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();

                    var x = res.GroupBy(l => l.UserFullname).Select(g => new
                    {
                        USER_FULLNAME = g.Key,
                        Num_of_entries = g.Count()
                    }).ToList();

                    List<SysUserLogTimeVm> results = new List<SysUserLogTimeVm>();
                    foreach (var item in x)
                    {
                        SysUserLogTimeVm result = new SysUserLogTimeVm()
                        {
                            UserFullName = item.USER_FULLNAME,
                            NumOfEntries = item.Num_of_entries
                        };
                        results.Add(result);
                    }
                    return Ok(await Result<List<SysUserLogTimeVm>>.SuccessAsync(results));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in LoginCounter ReportsController, MESSAGE: {ex.Message}"));
            }
        }

        //المتواجدون حاليا
        [HttpPost("CurrentUsers")]
        public async Task<IActionResult> CurrentUsers(CurrentUsersFilter filter)
        {
            try
            {
                var chk = await permission.HasPermission(92, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var items = await mainServiceManager.SysUserLogTimeService.GetAllVW(u => u.Offline == false
                 && (filter.UserTypeId == null || (u.UserTypeId != null && u.UserTypeId == filter.UserTypeId)));
                if (items.Succeeded)
                {
                    var res = items.Data.DistinctBy(u => u.UserFullname);

                    var final = res.ToList();
                    List<CurrentUsersFilter> result = new();
                    foreach (var item in final)
                    {
                        result.Add(new CurrentUsersFilter { UserFullName = item.UserFullname });
                    }
                    return Ok(await Result<List<CurrentUsersFilter>>.SuccessAsync(result));
                }

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in CurrentUsers ReportsController, MESSAGE: {ex.Message}"));
            }
        }

        //العمليات التي تمت على النظام
        [HttpPost("GetAllActivityByTransaction")]
        public async Task<ActionResult> GetAllActivityByTransaction(SysActivityLogFilterDto filter)
        {
            try
            {
                var chk = await permission.HasPermission(1100, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var items = await mainServiceManager.SysActivityLogService.GetAllVW(a => a.ActivityTypeId > 0
                && (string.IsNullOrEmpty(filter.ActivityDateFrom) || (a.ActivityDate != null && a.ActivityDate.Value.Date >= DateTime.ParseExact(filter.ActivityDateFrom, "yyyy/MM/dd", CultureInfo.InvariantCulture)))
                && (string.IsNullOrEmpty(filter.ActivityDateTo) || (a.ActivityDate != null && a.ActivityDate.Value.Date <= DateTime.ParseExact(filter.ActivityDateTo, "yyyy/MM/dd", CultureInfo.InvariantCulture)))
                && (filter.ActivityTypeId == null || (a.ActivityTypeId != null && a.ActivityTypeId == filter.ActivityTypeId))
                && (filter.ScreenId == null || (a.ScreenId != null && a.ScreenId == filter.ScreenId))
                && (filter.TablePrimarykey == null || (a.TablePrimarykey != null && a.TablePrimarykey == filter.TablePrimarykey))
                && (string.IsNullOrEmpty(filter.UserId) || !string.IsNullOrEmpty(a.UserId) && a.UserId.Equals(filter.UserId))
               );
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();

                    var groupRes = res.GroupBy(l => new { l.ScreenName, l.ScreenName2 }).Select(g => new
                    {
                        ScreenName = g.Key.ScreenName,
                        ScreenName2 = g.Key.ScreenName2,
                        TranscationCount = g.Count()
                    }).ToList();

                    List<SysActivityLogVM> results = new List<SysActivityLogVM>();
                    foreach (var item in groupRes)
                    {
                        SysActivityLogVM result = new SysActivityLogVM()
                        {
                            ScreenName = item.ScreenName,
                            ScreenName2 = item.ScreenName2,
                            TranscationCount = item.TranscationCount
                        };
                        results.Add(result);
                    }

                    return Ok(await Result<List<SysActivityLogVM>>.SuccessAsync(results));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in GetAllActivityByTransaction ReportsController, MESSAGE: {ex.Message}"));
            }
        }

        //تحركات المستخدمين
        [HttpPost("GetAllActivityLog")]
        public async Task<ActionResult> GetAllActivityLog(SysActivityLogFilterDto filter)
        {
            try
            {
                var chk = await permission.HasPermission(100, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var items = await mainServiceManager.SysActivityLogService.GetAllVW(a => a.ActivityTypeId > 0
                && (string.IsNullOrEmpty(filter.ActivityDateFrom) || (a.ActivityDate != null && a.ActivityDate.Value.Date >= DateTime.ParseExact(filter.ActivityDateFrom, "yyyy/MM/dd", CultureInfo.InvariantCulture)))
                && (string.IsNullOrEmpty(filter.ActivityDateTo) || (a.ActivityDate != null && a.ActivityDate.Value.Date <= DateTime.ParseExact(filter.ActivityDateTo, "yyyy/MM/dd", CultureInfo.InvariantCulture)))
                && (filter.ActivityTypeId == null || (a.ActivityTypeId != null && a.ActivityTypeId == filter.ActivityTypeId))
                && (filter.ScreenId == null || (a.ScreenId != null && a.ScreenId == filter.ScreenId))
                && (filter.TablePrimarykey == null || (a.TablePrimarykey != null && a.TablePrimarykey == filter.TablePrimarykey))
                && (string.IsNullOrEmpty(filter.UserId) || !string.IsNullOrEmpty(a.UserId) && a.UserId.Equals(filter.UserId))
               );

                if (items.Succeeded)
                {
                    var res = items.Data.ToList();

                    List<SysActivityLogVM> results = new();
                    foreach (var item in res)
                    {
                        SysActivityLogVM result = new SysActivityLogVM()
                        {
                            ScreenName = item.ScreenName,
                            ScreenName2 = item.ScreenName2,
                            ActivityType = item.ActivityType,
                            UserFullname = item.UserFullname,
                            ActivityDate = item.ActivityDate != null ? item.ActivityDate.Value.ToString("dd/MM/yyyy h:mm:ss tt", CultureInfo.InvariantCulture) : "",
                            TablePrimarykey = item.TablePrimarykey
                        };
                        results.Add(result);
                    }
                    return Ok(await Result<List<SysActivityLogVM>>.SuccessAsync(results));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in GetAllActivityLog ReportsController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("GetUserPermissionRp")]
        public async Task<ActionResult> GetUserPermissionRp(UserPermissionSearchVm filter)
        {
            try
            {
                var chk = await permission.HasPermission(1238, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var getReport = await mainServiceManager.SysScreenPermissionService.GetUserPermissionReport(filter);
                return Ok(getReport);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in GetUserPermissionRp ReportsController, MESSAGE: {ex.Message}"));
            }
        }
        //--------------------------
        [HttpPost("GetUserPermissionRpPagination")]

        public async Task<ActionResult> GetUserPermissionRpPagination([FromBody] UserPermissionSearchVm filter, int take = Pagination.take, int? lastSeenId = null)
        {
            try
            {
                var chk = await permission.HasPermission(1238, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var getReport = await mainServiceManager.SysScreenPermissionService.GetUserPermissionReportAsync(filter, take, lastSeenId);
                return Ok(getReport);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in GetUserPermissionRpPagination ReportsController, MESSAGE: {ex.Message}"));
            }
        }
        ///----------------------------------------------
        [HttpPost("GetUserTrackingRp")]
        public async Task<ActionResult> GetUserTrackingRp(SysUserTrackingFilterDto filter)
        {
            try
            {
                var chk = await permission.HasPermission(1537, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                //var getReport = await mainServiceManager.SysUserTrackingService.GetAllVW(a =>
                //    (string.IsNullOrEmpty(filter.ActivityDateFrom) || (a.CreatedOn != null && a.CreatedOn.Value.Date >= DateTime.ParseExact(filter.ActivityDateFrom, "yyyy/MM/dd", CultureInfo.InvariantCulture)))
                //    && (string.IsNullOrEmpty(filter.ActivityDateTo) || (a.CreatedOn != null && a.CreatedOn.Value.Date <= DateTime.ParseExact(filter.ActivityDateTo, "yyyy/MM/dd", CultureInfo.InvariantCulture)))
                //    && (filter.UserId == null || (a.UserId != null && a.UserId == filter.UserId))
                //);

                //if (getReport.Succeeded)
                //{
                //    var res = getReport.Data.ToList();

                //    List<SysUserTrackingVm> results = new();
                //    foreach (var item in res)
                //    {
                //        SysUserTrackingVm result = new SysUserTrackingVm()
                //        {
                //            Url = item.Url,
                //            UserFullName = item.UserFullname,
                //            Date = item.CreatedOn != null ? item.CreatedOn.Value.ToString("yyyy/MM/dd hh:mm:ss tt", CultureInfo.InvariantCulture) : "",
                //        };
                //        results.Add(result);
                //    }
                //    return Ok(await Result<List<SysUserTrackingVm>>.SuccessAsync(results));
                //}
                var getReport = await mainServiceManager.SysUserTrackingService.GetUserTrackingRp(filter);
                return Ok(getReport);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in GetUserTrackingRp ReportsController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("GetUserNotificationsRP")]
        public async Task<ActionResult> GetUserNotificationsRP(SysNotificationfilterDto filter)
        {
            try
            {
                var userId = currentData.UserId;
                var statusId = filter.StatusID ??= 2;

                var notifications = await mainServiceManager.SysNotificationService.GetAllVW(a =>
                    a.UserId != null &&
                    a.UserId == userId &&
                    (statusId == 2 || statusId == 1 ? a.IsRead == true : a.IsRead == false)
                );

                if (notifications.Succeeded)
                {
                    IEnumerable<SysNotificationsVw> res = notifications.Data.ToList(); ;

                    if (!string.IsNullOrEmpty(filter.DateFrom))
                    {
                        DateTime startDate = DateTime.ParseExact(filter.DateFrom, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                        res = res.Where(s => !s.CreatedOn.HasValue || s.CreatedOn >= startDate);
                    }

                    if (!string.IsNullOrEmpty(filter.DateTo))
                    {
                        DateTime endDate = DateTime.ParseExact(filter.DateTo, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                        res = res.Where(s => !s.CreatedOn.HasValue || s.CreatedOn <= endDate);
                    }

                    List<SysNotificationVm> results = new();
                    foreach (var item in res)
                    {
                        SysNotificationVm result = new SysNotificationVm()
                        {
                            UserId = item.UserId,
                            MsgTxt = item.MsgTxt,
                            IsRead = item.IsRead,
                            ReadDate = item.ReadDate,
                            CreatedBy = item.CreatedBy,
                            CreatedOn = item.CreatedOn,
                            ActivityLogId = item.ActivityLogId,
                            UserFullname = item.UserFullname,
                            Url = item.Url,
                            Date1 = item.CreatedOn != null ? item.CreatedOn.Value.ToString("yyyy/MM/dd hh:mm:ss tt", CultureInfo.InvariantCulture) : "",
                        };
                        results.Add(result);
                    }
                    return Ok(await Result<List<SysNotificationVm>>.SuccessAsync(results));
                }

                return Ok(notifications);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in GetUserNotificationsRP ReportsController, MESSAGE: {ex.Message}"));
            }
        }

        // تقرير بالحقول والجداول
        [HttpPost("GetAllRptFields")]
        public async Task<IActionResult> GetAllRptFields(RptFieldFilterDto filter)
        {
            try
            {
                var chk = await permission.HasPermission(925, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var items = await rptServiceManager.RptFieldService.Search(filter);
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(Result.FailAsync(ex.Message));
            }
        }

        [HttpPost("GetUsersLogins")]
        public async Task<IActionResult> GetUsersLogins(SysUsersLoginsVm filter)
        {
            try
            {
                var chk = await permission.HasPermission(2148, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var items = await mainServiceManager.SysUserService.GetUsersLoginsRpt(filter);
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(Result.FailAsync(ex.Message));
            }
        }

        [HttpPost("GetUsersPermissions")]
        public async Task<IActionResult> GetUsersPermissions(SysUsersLoginsVm filter)
        {
            try
            {
                var chk = await permission.HasPermission(2149, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var items = await mainServiceManager.SysUserService.GetUsersPermissionsRpt(filter);
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(Result.FailAsync(ex.Message));
            }
        }

        #region ================================= DDL =================================
        [HttpGet("DDLRptTables")]
        public async Task<IActionResult> DDLRptTables(int systemId = 0, int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<RptTable, long>(s => s.SystemId == systemId && s.IsDeleted == false,
                    "Id", (lang == 1) ? "Name" : "Name2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLRptFields")]
        public async Task<IActionResult> DDLRptFields(int tableId = 0, int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<RptField, long>(s => s.TableId == tableId && s.IsDeleted == false,
                    "Id", (lang == 1) ? "Name" : "Name2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
        #endregion ============================= End DDL ==============================
    }
}