using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.HR
{
    // إشعارات الموظفين
    public class HREmpNotificationOutboxController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;


        public HREmpNotificationOutboxController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
        }

        [HttpGet("Search")]
        public async Task<IActionResult> Search()
        {
            var chk = await permission.HasPermission(1471, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var getdata = await hrServiceManager.HrNotificationService.LoadNotifications("", session.FacilityId);
                return Ok(getdata);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrAttendancesFilterDto>.FailAsync(ex.Message));
            }
        }

        [HttpPost("Add")]
        public async Task<ActionResult> Add(HrNotificationDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(1471, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (string.IsNullOrEmpty(obj.EmpCode)) return Ok(await Result.FailAsync($"يجب ادخال رقم  الموظف"));
                if (string.IsNullOrEmpty(obj.Subject)) return Ok(await Result.FailAsync($"يجب ادخال عنوان الإشعار  "));
                if (string.IsNullOrEmpty(obj.Detailes)) return Ok(await Result.FailAsync($"يجب ادخال محتوى الرسالة  "));
                if (obj.TypeId <= 0) return Ok(await Result.FailAsync($"يجب ادخال نوع الإشعار "));
                if (string.IsNullOrEmpty(obj.NotificationDate)) return Ok(await Result.FailAsync($"يجب ادخال تاريخ الإشعار "));
                var add = await hrServiceManager.HrNotificationService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add  HREmpNotificationOutbox  Controller, MESSAGE: {ex.Message}"));
            }
        }
        [HttpPost("AddReply")]
        public async Task<ActionResult> AddReply(HrNotificationsReplyDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(1471, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (obj.NotificationId <= 0) return Ok(await Result.FailAsync($"حدث خطا في  رقم الاشعار"));
                if (string.IsNullOrEmpty(obj.Reply)) return Ok(await Result.FailAsync($"يجب ادخال الرد  "));
                obj.Source = "outbox";
                var add = await hrServiceManager.HrNotificationService.AddNotificationsReply(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add  HREmpNotificationOutbox  Controller, MESSAGE: {ex.Message}"));
            }
        }
    }
}