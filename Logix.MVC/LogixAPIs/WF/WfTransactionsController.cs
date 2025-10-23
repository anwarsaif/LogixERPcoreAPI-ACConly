using DocumentFormat.OpenXml.Spreadsheet;
using Logix.Application.Common;
using Logix.Application.DTOs.Main;
using Logix.Application.DTOs.WF.ViewModels;
using Logix.Application.Helpers;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Services;
using Logix.Application.Wrapper;
using Logix.Domain.WF;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.WF.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph.Models.Security;
using IWorkflowHelper = Logix.Application.Helpers.IWorkflowHelper;

namespace Logix.MVC.LogixAPIs.WF
{
    public class WfTransactionsController : BaseWfController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly ICurrentData session;
        private readonly IPermissionHelper permission;
        private readonly ILocalizationService localization;
        private readonly IWFServiceManager wfServiceManager;
        private readonly IWorkflowHelper workflowHelper;
        public WfTransactionsController(
            IMainServiceManager mainServiceManager,
            IWFServiceManager wfServiceManager,
            ICurrentData session,
            IPermissionHelper permission,
            IWorkflowHelper IWorkflowHelper,
            ILocalizationService localization
            )
        {
            this.mainServiceManager = mainServiceManager;
            this.workflowHelper = IWorkflowHelper;
            this.wfServiceManager = wfServiceManager;
            this.session = session;
            this.permission = permission;
            this.localization = localization;
        }
        [HttpGet("GetScreenWorkflowByScreen")]
        public async Task<ActionResult> GetScreenWorkflowByScreen(long screenId)
        {
            /// <summary>Gets a Workflow for a Specific Screen Given ScreenId</summary>
            try
            {
                var items = await mainServiceManager.SysScreenWorkflowService.GetScreenWorkflowByScreen(screenId);
                if (!items.Succeeded || items.Data == null)
                {
                    return Ok(await Result<object>.FailAsync(items.Status.message));
                }
                return Ok(await Result<object>.SuccessAsync(items.Data.ToList()));
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"Error in {nameof(GetScreenWorkflowByScreen)}: {ex.Message}"));
            }

        }

        [HttpGet("GetAttributesForScreen")]
        public async Task<ActionResult> GetAttributesForScreen(long screenId, long? appTypeId, int? stepId = null)
        {
            /// <summary>Gets a list of Dynamic Attributes for a Specific ScreenId AppTypeId and StepId</summary>
            try
            {
                var items = await mainServiceManager.SysScreenWorkflowService.GetAttributes(screenId, appTypeId, stepId);
                if (!items.Succeeded || items.Data == null)
                {
                    return Ok(await Result<object>.FailAsync(items.Status.message));
                }
                return Ok(await Result<object>.SuccessAsync(items.Data.ToList()));
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"Error in {nameof(GetAttributes)}: {ex.Message}"));
            }

        }
        [HttpGet("GetAttributes")]
        public async Task<ActionResult> GetAttributes(long appTypeId, long stepId)
        {
            /// <summary>Gets a list of Dynamic Attributes for a Specific AppType and StepId</summary>
            try
            {
                var items = await wfServiceManager.WfDynamicAttributeService.GetAllVW(x => x.AppTypeId == appTypeId
                && (stepId == 0 || x.StepId == stepId) && x.IsDeleted == false);
                if (items.Succeeded && items.Data.Any())
                {
                    var res = items.Data.OrderBy(x => x.SortOrder).ToList();
                    return Ok(await Result<List<WfDynamicAttributesVw>>.SuccessAsync(res));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }
        [HttpGet("GetAttributeValues")]
        public async Task<ActionResult> GetAttributeValues(long screenId, long appId, long? appTypeId = null)
        {
            /// <summary>Gets a list of Dynamic Attributes of an application type along with their latest values, layout details, and control definitions.</summary>
            try
            {
                var items = await mainServiceManager.SysScreenWorkflowService.GetAttributeValues(screenId, appId, appTypeId);
                if (!items.Succeeded || items.Data == null)
                {
                    return Ok(await Result<object>.FailAsync(items.Status.message));
                }
                return Ok(await Result<object>.SuccessAsync(items.Data.ToList()));
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"Error in {nameof(GetAttributeValues)}: {ex.Message}"));
            }

        }
        
        [HttpGet("GetTableData")]
        public async Task<ActionResult> GetTableData(long tableId, long stepId)
        {
            /// <summary>Gets a list of Dynamic Attributes Tables for a Specific TableId and StepId</summary>
            try
            {
                DynamicAttributesTableVm obj = new();

                var tableName = await wfServiceManager.WfAppTypeTableService.GetOne(x => x.Name, x => x.Id == tableId && x.IsDeleted == false);
                if (tableName.Succeeded)
                    obj.TableName = tableName.Data ?? "";

                var items = await wfServiceManager.WfDynamicAttributesTableService.GetAllVW(x => x.TableId == tableId
                && (stepId == 0 || x.StepId == stepId) && x.IsDeleted == false);
                if (items.Succeeded && items.Data.Any())
                {
                    var res = items.Data.OrderBy(x => x.SortOrder).ToList();
                    obj.TableAttributes = res;
                }
                return Ok(await Result<DynamicAttributesTableVm>.SuccessAsync(obj));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }
        [HttpGet("GetDefaultValue")]
        public async Task<ActionResult> GetDefaultValue(string defaultValue, long? empId, string currDate, long? facilityId = 1, long? appId = null, long? finYear = null)
        {
            try
            {
                var items = await mainServiceManager.SysScreenWorkflowService.GetDefaultValue(defaultValue, empId, currDate, facilityId, appId, finYear);
                if (!items.Succeeded || items.Data == null)
                {
                    return Ok(await Result<object>.FailAsync(items.Status.message));
                }
                return Ok(await Result<object>.SuccessAsync(items.Data));
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"Error in {nameof(GetDefaultValue)}: {ex.Message}"));
            }

        }

        [HttpGet("Send")]
        public async Task<ActionResult> Send(long Applicants_ID, long Screen_ID, int App_Type_ID = 0, long Alternative_Emp_ID = 0, string AppSubject = "", long? CustomerId = 0)
        {
            /// <summary>
            /// This Action Adds Application, Sends Emails to Related Parties, and Creates Notifications According to Notification Table and Returns the Added ApplicationId
            /// 0 if Error Occurs
            /// </summary>
            try
            {
                var item = await wfServiceManager.WfApplicationService.Send(Applicants_ID, Screen_ID, App_Type_ID, Alternative_Emp_ID, AppSubject, CustomerId);
                if (!item.Succeeded || item.Data == 0)
                {
                    return Ok(await Result<object>.FailAsync(item.Status.message));
                }
                return Ok(await Result<object>.SuccessAsync(item.Data));
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"Error in {nameof(Send)}: {ex.Message}"));
            }

        }

        [HttpGet("GetStaticTransactions")]
        public async Task<ActionResult> GetStaticTransactions()
        {
            var staticData = new List<WfTransactionVm>{
            new() {
                Id = 1,
                SenderName = "أحمد علي",
                ReceiverName = "محمد حسن",
                StepName = "مراجعة أولية",
                StatusName = "تمت الموافقة",
                Comment = "تمت المراجعة من قبل القسم المالي",
                CreatedOn = DateTime.Now.AddDays(-2)
            },
            new() {
                Id = 2,
                SenderName = "سعاد فهد",
                ReceiverName = "خالد السبيعي",
                StepName = "اعتماد نهائي",
                StatusName = "بانتظار الرد",
                Comment = "بانتظار رد اللجنة",
                CreatedOn = DateTime.Now.AddDays(-1)
            }};

            return Ok(await Result<List<WfTransactionVm>>.SuccessAsync(staticData));
        }

        [HttpGet("GetStaticData")]
        public async Task<ActionResult> GetStaticData()
        {
            var data = new List<WfStaticTransactionVm>{
            new() {
                FromUserName = "علي أحمد",
                ToUserName = "نورة محمد",
                StepName = "مراجعة الطلب",
                StatusName = "بانتظار الرد",
                Comment = "جاري التحقق من البيانات",
                CreatedOn = DateTime.Now.AddDays(-3)
            },
            new() {
                FromUserName = "فهد السبيعي",
                ToUserName = "أمل العتيبي",
                StepName = "اعتماد الطلب",
                StatusName = "تم الاعتماد",
                Comment = "تم إرسال الطلب للإدارة",
                CreatedOn = DateTime.Now.AddDays(-1)
            }};

            return Ok(await Result<List<WfStaticTransactionVm>>.SuccessAsync(data));
        }
        #region ==================================




        #endregion=================================
    }
}
