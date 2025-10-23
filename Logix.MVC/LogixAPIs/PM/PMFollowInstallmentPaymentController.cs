using DocumentFormat.OpenXml.Spreadsheet;
using Logix.Application.Common;
using Logix.Application.DTOs.Main;
using Logix.Application.DTOs.PM;
using Logix.Application.DTOs.PM.Shared;
using Logix.Application.Helpers;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.Main.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using System.Net.Mail;

namespace Logix.MVC.LogixAPIs.PM
{

    //   جدول متابعة سداد الدفعات
    public class PMFollowInstallmentPaymentController : BasePMApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IPMServiceManager pMServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly ICrmServiceManager crmServiceManager;
        private readonly ISendSmsHelper sendSmsHelper;
        private readonly IEmailService emailService;


        public PMFollowInstallmentPaymentController(IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IMainServiceManager mainServiceManager, IPMServiceManager pMServiceManager, ICrmServiceManager crmServiceManager, ISendSmsHelper sendSmsHelper, IEmailService emailService)
        {
            this.permission = permission;
            this.session = session;
            this.localization = localization;
            this.mainServiceManager = mainServiceManager;
            this.pMServiceManager = pMServiceManager;
            this.crmServiceManager = crmServiceManager;
            this.sendSmsHelper = sendSmsHelper;
            this.emailService = emailService;
        }

        #region Index Page


        [HttpPost("Search")]
        public async Task<IActionResult> Search(PaginatedRequest<BindProjectsDto> request)
        {
            var chk = await permission.HasPermission(240, PermissionType.Show);

            if (!chk)
                return Ok(await Result.AccessDenied("AccessDenied"));

            try
            {
                var filter = request.Filter;
                filter.Code ??= 0;
                filter.Type ??= 0;
                filter.StatusId ??= 0;
                filter.paid ??= 0;
                var BranchesList = session.Branches.Split(',');

                var items = await pMServiceManager.PMProjectsInstallmentService.GetAllVW(p => p.IsDeleted == false &&
                        (filter.BranchId == 0 || BranchesList.Contains(p.BranchId.ToString())) &&
                        (filter.ProjectType == 0 || p.ProjectType == filter.ProjectType) &&
                        (filter.Type == 0 || p.Type == filter.Type) &&
                        (filter.Code == 0 || p.ProjectCode == filter.Code) &&
                        (string.IsNullOrEmpty(filter.Name) || (p.Name != null && p.Name.Contains(filter.Name))) &&
                        (string.IsNullOrEmpty(filter.CustomerCode) || p.CustomerCode == filter.CustomerCode) &&
                        (string.IsNullOrEmpty(filter.CustomerName) || p.CustomerName == filter.CustomerName) &&
                        (filter.StatusId == 0 || p.StatusId == filter.StatusId)
                );

                if (!items.Succeeded)
                    return Ok(await Result<object>.FailAsync(items.Status.message));

                if (!items.Data.Any())
                    return Ok(await Result<object>.SuccessAsync(new List<object>(), localization.GetResource1("NosearchResult")));

                var res = items.Data.AsQueryable();

                if (filter.BranchId > 0)
                    res = res.Where(p => p.BranchId == filter.BranchId);



                if (!string.IsNullOrEmpty(filter.From) && !string.IsNullOrEmpty(filter.To))
                {
                    var dateFrom = DateHelper.StringToDate(filter.From);
                    var dateTo = DateHelper.StringToDate(filter.To);
                    res = res.Where(p => DateHelper.StringToDate(p.DateG) >= dateFrom && DateHelper.StringToDate(p.DateG) <= dateTo);
                }

                if (!string.IsNullOrEmpty(filter.InstallmentDateFrom) && !string.IsNullOrEmpty(filter.InstallmentDateTo))
                {
                    var installmentDateFrom = DateHelper.StringToDate(filter.InstallmentDateFrom);
                    var installmentDateTo = DateHelper.StringToDate(filter.InstallmentDateTo);
                    res = res.Where(p => DateHelper.StringToDate(p.Date) >= installmentDateFrom && DateHelper.StringToDate(p.Date) <= installmentDateTo);
                }

                if (filter.paid > 0)
                {
                    if (filter.paid == 1)
                        res = res.Where(p => p.Ispaid == false);
                    if (filter.paid == 2)
                        res = res.Where(p => p.Ispaid == true);
                }

                if (!res.Any())
                    return Ok(await Result<object>.SuccessAsync(new List<object>(), localization.GetResource1("NosearchResult")));

                var projectDtos = res.Select(p => new
                {
                    p.Id,
                    p.Mobile,
                    p.EmailCustomer,
                    p.ProjectCode,
                    ProjectName = p.ProjectsName,
                    Date = p.DateG ?? "",
                    p.ProjectStart,
                    p.ProjectEnd,
                    p.StatusName,
                    p.ProjStepName,
                    p.CompletionRate,
                    p.CustomerCode,
                    p.CustomerName,
                    p.Code,
                    p.Amount,
                    p.LastAction,
                    p.LastUpdateTimePeriod,
                    NetValue = ((p.ProjectValue ?? 0) + (p.ProjectAdditionsValue ?? 0) - (p.DeductionValue ?? 0))
                }).ToList();

                if (request.PageNumber > 0 && request.PageSize > 0)
                {
                    var paginatedData = await PaginatedResult<object>.SuccessAsync(projectDtos, request.PageNumber, request.PageSize);
                    return Ok(paginatedData);
                }

                return Ok(await Result<object>.SuccessAsync(projectDtos, "Search Completed", 200));


            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"Error in {nameof(Search)}: {ex.Message}"));
            }
        }




        [HttpGet("DDLEmailTemplateChanged")]
        public async Task<IActionResult> DDLEmailTemplateChanged(long TemplateId)
        {
            try
            {
                var chk = await permission.HasPermission(240, PermissionType.Show);

                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var result = await crmServiceManager.CrmEmailTemplateService.GetOne(x => x.IsDeleted == false && x.Id == TemplateId);
                return Ok(result);

            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"Error in {this.GetType().Name}.{nameof(DDLEmailTemplateChanged)}: {ex.Message}"));
            }
        }


        //  عند الضغط على زر العرض    
        [HttpGet("ButtonShowClicked")]
        public async Task<IActionResult> ButtonShowClicked(string Id)
        {
            try
            {
                var chk = await permission.HasPermission(240, PermissionType.Show);

                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var result = await pMServiceManager.PMProjectsInstallmentActionService.GetAllVW(x => x.IsDeleted == false && x.InstallmentId == Id);
                return Ok(result);

            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"Error in {this.GetType().Name}.{nameof(DDLEmailTemplateChanged)}: {ex.Message}"));
            }
        }

        [HttpPost("SendSms")]
        public async Task<ActionResult> SendSms(string ReceiverMobiles, string Message)
        {
            try
            {
                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));
                var UserId = (int)session.UserId;
                List<string> mobileList = ReceiverMobiles.Split(',').ToList();
                if (mobileList.Any())
                {
                    foreach (var mobile in mobileList)
                    {
                        var isSend = await sendSmsHelper.SendSms(mobile, Message, false, false, session.FacilityId, UserId);
                        if (!isSend) return Ok(await Result.FailAsync($"{localization.GetMainResource("SendSmsFaild")}" + $"{localization.GetCommonResource("To")}  {mobile}"));
                    }
                    return Ok(await Result.SuccessAsync("تم ارسال الرسالة على جوالات العملاء"));

                }
                return Ok(await Result.FailAsync($"{localization.GetMainResource("SendSmsFaild")}"));

            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"{ex.Message}"));

            }
        }


        [HttpPost("SendEmails")]
        public async Task<ActionResult> SendEmails(string ReceiverEmails, string Message, string Subject)
        {
            try
            {
                try
                {
                    if (!ModelState.IsValid)
                        return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));
                    var UserId = (int)session.UserId;
                    List<string> EmailList = ReceiverEmails.Split(',').ToList();
                    if (EmailList.Any())
                    {
                        foreach (var Email in EmailList)
                        {
                            await emailService.SendEmailAsync(Email ?? "", Subject ?? "", Message ?? "");

                        }
                        return Ok(await Result.SuccessAsync("تم ارسال الرسالة على ايميلات  العملاء"));

                    }
                    return Ok(await Result.FailAsync($"{localization.GetMainResource("SendEmailFaild")}"));

                }
                catch (Exception ex)
                {
                    return Ok(await Result.FailAsync($"{ex.Message}"));

                }

            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }


        [HttpPost("AddComment")]
        public async Task<IActionResult> AddComment(AddCommentDto obj)
        {

            try
            {
                var chk = await permission.HasPermission(240, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (obj.InstallmentIds.Count <= 0)
                    return Ok(await Result<object>.FailAsync("قم بعملية إختيار الدفعة اولاً"));

                var result = await pMServiceManager.PMProjectsInstallmentActionService.AddComment(obj);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"======= Exp in {this.GetType()}: {ex.Message}"));

            }

        }
        #endregion



    }
}
