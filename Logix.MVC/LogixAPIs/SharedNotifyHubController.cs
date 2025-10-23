using Logix.Application.Common;
using Logix.Application.Helpers.SignalHelper;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.Main.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Globalization;

namespace Logix.MVC.LogixAPIs
{
    [Route($"api/{ApiConfig.ApiVersion}/[controller]")]
    [ApiController]

    public class SharedNotifyHubController : ControllerBase
    {
        private readonly IMainServiceManager _mainServiceManager;
        private readonly ICurrentData currentData;
        private readonly IHubContext<NotifyHub> _hub;

        public SharedNotifyHubController(
            IMainServiceManager mainServiceManager,
             ICurrentData currentData,
            IHubContext<NotifyHub> hub)
        {
            _mainServiceManager = mainServiceManager;
            this.currentData = currentData;
            _hub = hub;
        }

        /// <param name="userId">معرف المستخدم (اختياري). إذا تم تمريره سيتم البث إلى مجموعة المستخدم عبر SignalR.</param>

        [HttpGet("GetUserNotifications")]
        public async Task<IActionResult> GetUserNotifications([FromQuery] string? userId = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var getNotifs = await _mainServiceManager.SysNotificationService.GetTopVw();
                if (getNotifs.Succeeded && getNotifs.Data != null && getNotifs.Data.Any())
                {
                    var result = getNotifs.Data.Select(item => new UserNotificationsVM
                    {
                        Id = item.Id,
                        MsgTxt = item.MsgTxt,
                        Url = item.Url,
                        UserFullname = item.UserFullname,
                        CreatedOn = item.CreatedOn != null ? item.CreatedOn.Value.ToString("yyyy/MM/dd hh:mm:ss tt", CultureInfo.InvariantCulture) : string.Empty,
                    }).ToList();

                    if (!string.IsNullOrWhiteSpace(userId))
                    {
                        await _hub.Clients.Group($"User_{userId}").SendAsync("GetUserNotifications", result, cancellationToken);
                    }

                    return Ok(await Result<List<UserNotificationsVM>>.SuccessAsync(result));
                }
                return Ok(getNotifs);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("GetNotificationshub")]
        public async Task<IActionResult> GetUserNotifications(long? userId)
        {
            try
            {
                var getNotifs = await _mainServiceManager.SysNotificationService.GetAllVW(x => x.UserId == userId);
                if (getNotifs.Succeeded)
                {
                    List<UserNotificationsVM> result = new();
                    foreach (var item in getNotifs.Data)
                    {
                        result.Add(new UserNotificationsVM
                        {
                            Id = item.Id,
                            MsgTxt = item.MsgTxt,
                            Url = item.Url,
                            UserFullname = item.UserFullname,
                            CreatedOn = item.CreatedOn != null ? item.CreatedOn.Value.ToString("yyyy/MM/dd hh:mm:ss tt", CultureInfo.InvariantCulture) : "",
                        });
                    }

                    await _hub.Clients.All.SendAsync(SignalNOTIFICATION.NOTIFICATION_CHANNEL + "/" + currentData.UserId);
                    return Ok(await Result<List<UserNotificationsVM>>.SuccessAsync(result));
                }
                return Ok(getNotifs);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }


    }
}
