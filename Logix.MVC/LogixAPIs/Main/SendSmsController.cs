using Logix.Application.Common;
using Logix.Application.DTOs.Main;
using Logix.Application.Helpers;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.Main.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.Main
{
    public class SendSmsController : BaseMainApiController
    {
        private readonly ISendSmsHelper sendSmsHelper;
        private readonly ILocalizationService localization;
        private readonly IMainServiceManager mainServiceManager;
        private readonly ICurrentData session;
        private readonly ISysConfigurationHelper configurationHelper;

        public SendSmsController(ISendSmsHelper sendSmsHelper,
            ILocalizationService localization,
            IMainServiceManager mainServiceManager,
            ICurrentData session,
            ISysConfigurationHelper configurationHelper)
        {
            this.sendSmsHelper = sendSmsHelper;
            this.localization = localization;
            this.mainServiceManager = mainServiceManager;
            this.session = session;
            this.configurationHelper = configurationHelper;
        }

        [HttpPost("Send")]
        public async Task<ActionResult> Send(SendSmsVM obj)
        {
            try
            {
                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var isSend = await sendSmsHelper.SendSms(obj.ReceiverMobile, obj.Message, obj.IsRepeat, obj.IsArabic, obj.FacilityId ?? 0, obj.UserId ?? 0);
                if (isSend)
                    return Ok(await Result.SuccessAsync());
                else
                    return Ok(await Result.FailAsync($"{localization.GetMainResource("SendSmsFaild")}"));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }


        [HttpPost("SendToGroup")]
        public async Task<ActionResult> SendToGroup(SendSmsToGroupVM obj)
        {
            try
            {
                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                obj.BranchId ??= 0; obj.GroupId ??= 0;

                // get mobiles
                string phoneNumbers = "";
                List<string?> allMobiles = new();
                if (obj.GroupId == 3)
                {
                    var branchesId = session.Branches.Split(',');
                    var getMobiles = await mainServiceManager.InvestEmployeeService.GetAll(e => e.Mobile,
                            e => e.IsDeleted == false && e.StatusId == 1
                            && (obj.BranchId == 0 || e.BranchId == obj.BranchId) && branchesId.Contains(e.BranchId.ToString()));

                    if (getMobiles.Succeeded)
                        allMobiles = getMobiles.Data.GroupBy(x => x).Select(group => group.Key).ToList();
                }
                else
                {
                    var getMobiles = await mainServiceManager.SysCustomerService.GetAll(e => e.Mobile,
                            e => e.CusTypeId == obj.GroupId && e.IsDeleted == false && e.FacilityId == session.FacilityId
                            && (obj.BranchId == 0 || e.BranchId == obj.BranchId));

                    if (getMobiles.Succeeded)
                        allMobiles = getMobiles.Data.GroupBy(x => x).Select(group => group.Key).ToList();
                }

                foreach (var mobile in allMobiles)
                {
                    if (!string.IsNullOrEmpty(mobile) && mobile.Length == 10 && mobile.All(char.IsDigit))
                    {
                        string formatMobile = "966" + mobile.Substring(1);

                        if (!string.IsNullOrEmpty(phoneNumbers))
                            phoneNumbers += "," + formatMobile;
                        else
                            phoneNumbers += formatMobile;
                    }
                }

                var isSend = await sendSmsHelper.SendSms(phoneNumbers, obj.Message);
                if (isSend)
                    return Ok(await Result.SuccessAsync());
                else
                    return Ok(await Result.FailAsync($"{localization.GetMainResource("SendSmsFaild")}"));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }
    }


}