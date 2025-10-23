using System.Net.Mail;
using Logix.Application.Common;
using Logix.Application.DTOs.Main;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.Main.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.Main
{
    public class EmailController : BaseMainApiController
    {
        private readonly IPermissionHelper permission;
        private readonly ILocalizationService localization;
        private readonly ICrmServiceManager crmServiceManager;
        private readonly IEmailService emailService;
        private readonly IMainServiceManager mainServiceManager;

        public EmailController(IPermissionHelper permission,
            ILocalizationService localization,
            ICrmServiceManager crmServiceManager,
            IEmailService emailService,
            IMainServiceManager mainServiceManager)
        {
            this.permission = permission;
            this.localization = localization;
            this.crmServiceManager = crmServiceManager;
            this.emailService = emailService;
            this.mainServiceManager = mainServiceManager;
        }


        [HttpPost("SendEmail")]
        public async Task<ActionResult> SendEmail(SendEmailVM obj)
        {
            try
            {
                //var chk = await permission.HasPermission(1002, PermissionType.Show);
                //if (!chk)
                //    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                try
                {
                    var mailAddress = new MailAddress(obj.SendTo ?? ""); // if email is Invalid will cause exception
                    if (!string.IsNullOrEmpty(obj.FilesUrl))
                    {
                        List<string> attachments = new();
                        attachments.AddRange(obj.FilesUrl.Split(','));
                        await emailService.SendEmailWithAttachmentAsync(obj.SendTo ?? "", obj.Subject ?? "", obj.Message ?? "", obj.MailServerId ?? 0, attachments);
                    }
                    else
                        await emailService.SendEmailAsync(obj.SendTo ?? "", obj.Subject ?? "", obj.Message ?? "", obj.MailServerId ?? 0);

                    return Ok(await Result.SuccessAsync());
                }
                catch
                {
                    return Ok(await Result.FailAsync("Invalid email"));
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"=== Exp: {ex.Message}"));
            }
        }

        [HttpGet("GetTemplateData")]
        public async Task<ActionResult> GetTemplateData(long id)
        {
            try
            {
                var item = await crmServiceManager.CrmEmailTemplateService.GetById(id);
                if (item.Succeeded)
                {
                    EmailTemplateVM obj = new()
                    {
                        Subject = item.Data.Subject,
                        Message = item.Data.Message,
                    };

                    // get files
                    var files = await crmServiceManager.CrmEmailTemplateAttachService.GetAll(f => f.TemplateId == id && f.IsDeleted == false);
                    if (files.Succeeded)
                    {
                        List<SysFileDto> TemplateFiles = new();
                        foreach (var file in files.Data)
                        {
                            TemplateFiles.Add(new SysFileDto()
                            {
                                FileName = file.Name,
                                FileUrl = file.FileUrl,
                            });
                        }

                        obj.TemlateFiles = TemplateFiles;
                        return Ok(await Result<EmailTemplateVM>.SuccessAsync(obj));
                    }
                }
                return Ok(item);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"=== Exp: {ex.Message}"));
            }
        }


        [HttpPost("SendEmailToGroup")]
        public async Task<ActionResult> SendEmailToGroup(SendEmailToGroupVM obj)
        {
            // ارسال بريد جماعي
            try
            {
                var chk = await permission.HasPermission(134, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                List<string?> allEmails = new();
                int count = 0;

                // get emails: (when type = 100 => get employees emails, else get from sysCustomer based on custumer type id)
                if (obj.TypeId == 100)
                {
                    var getEmails = await mainServiceManager.InvestEmployeeService.GetAll(e => e.Email,
                            e => e.IsDeleted == false && e.StatusId == 1 && !string.IsNullOrEmpty(e.Email) && e.Email.Length > 5);
                    if (getEmails.Succeeded)
                        allEmails = getEmails.Data.ToList();
                }
                else
                {
                    var getEmails = await mainServiceManager.SysCustomerService.GetAll(e => e.Email,
                            e => e.CusTypeId == obj.TypeId && e.IsDeleted == false && !string.IsNullOrEmpty(e.Email) && e.Email.Length > 5);
                    if (getEmails.Succeeded)
                        allEmails = getEmails.Data.ToList();
                }

                // send emails
                foreach (var email in allEmails)
                {
                    try
                    {
                        var mailAddress = new MailAddress(email ?? ""); // if email is Invalid will cause exception
                        await emailService.SendEmailAsync(email ?? "", obj.Subject ?? "", obj.Message ?? "");
                        ++count;
                    }
                    catch
                    {
                        continue;
                    }
                }

                string msg = localization.GetMainResource("SuccessfullySentToCount");
                msg += " " + count;
                return Ok(await Result.SuccessAsync(msg));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"=== Exp: {ex.Message}"));
            }
        }
    }
}