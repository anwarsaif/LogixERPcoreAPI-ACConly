using System.Data;
using EInvoiceKSADemo.Helpers.Models;
using EInvoiceKSADemo.Helpers.Zatca.Models;
using Logix.Application.Common;
using Logix.Application.DTOs.Main;
using Logix.Application.Helpers.Sal;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.Main;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.Sales.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Logix.MVC.LogixAPIs.Sales
{
    public class ReportToZatcaController : BaseSalesApiController
    {
        private readonly ICurrentData session;
        private readonly IMvcSession mvcSession;
        private readonly IApiDDLHelper ddlHelper;
        private readonly ISysConfigurationHelper configurationHelper;
        private readonly ILocalizationService localization;
        private readonly IMainServiceManager mainServiceManager;
        private readonly IZatcaHelper zatcaHelper;

        public ReportToZatcaController(ICurrentData session,
            IMvcSession mvcSession,
            IApiDDLHelper ddlHelper,
            ISysConfigurationHelper configurationHelper,
            ILocalizationService localization,
            IMainServiceManager mainServiceManager,
            IZatcaHelper zatcaHelper)
        {
            this.session = session;
            this.mvcSession = mvcSession;
            this.ddlHelper = ddlHelper;
            this.configurationHelper = configurationHelper;
            this.localization = localization;
            this.mainServiceManager = mainServiceManager;
            this.zatcaHelper = zatcaHelper;
        }

        [HttpPost("Search")]
        public async Task<IActionResult> Search(ZatcaInvoiceFilterDto filter)
        {
            try
            {
                var lang = session.Language;
                var result = new List<object>();
                var dt = new DataTable();

                filter.BranchId ??= 0; filter.InvoiceStatus ??= 0; filter.InvoiceType ??= 0; filter.PaymentTermsId ??= 0;

                string Environment = await configurationHelper.GetValue(336, session.FacilityId);
                if (string.IsNullOrEmpty(Environment))
                    return Ok(await Result.FailAsync(localization.GetCommonResource("EnvironmentError")));
                else if (Environment == "1")
                {
                    var items = await mainServiceManager.SysZatcaInvoiceTransactionService.GetTransactions(filter);
                    if (items.Succeeded)
                        dt = items.Data;
                    else
                        return Ok(await Result.FailAsync(items.Status.message));
                }
                else if (Environment == "0")
                {
                    var items = await mainServiceManager.SysZatcaInvoiceTransactionsSimulationService.GetTransactions_Simulation(filter);
                    if (items.Succeeded)
                        dt = items.Data;
                    else
                        return Ok(await Result.FailAsync(items.Status.message));
                }

                foreach (DataRow row in dt.Rows)
                {
                    string isReportedToZatcaState = "";
                    string color = "";
                    if (row.IsNull("IsReportedToZatca") || Convert.ToBoolean(row["IsReportedToZatca"]) == false)
                    {
                        color = "red";
                        isReportedToZatcaState = lang == 1 ? "غير مرحلة" : "Not reported";
                    }
                    else
                    {
                        color = "green";
                        isReportedToZatcaState = lang == 1 ? "مرحلة" : "Reported";
                    }

                    result.Add(new
                    {
                        IsSelected = false, // for checkbox
                        InvoiceId = Convert.ToInt64(row["ID"]),
                        BranchId = !row.IsNull("BRANCH_ID") ? Convert.ToInt64(row["BRANCH_ID"]) : 0,
                        Code = row["Code"].ToString(),
                        Date1 = !row.IsNull("Date1") ? row["Date1"].ToString() : "",
                        PaymentTerms = lang == 1 ? row["Payment_Terms"].ToString() : row["Payment_Terms2"].ToString(),
                        CustomerName = lang == 1 ? row["CustomerName"].ToString() : row["CustomerName2"].ToString(),
                        Total = !row.IsNull("Total") ? Convert.ToDecimal(row["Total"]) : 0,
                        DiscountAmount = !row.IsNull("Discount_Amount") ? Convert.ToDecimal(row["Discount_Amount"]) : 0,
                        VatAmount = !row.IsNull("VAT_Amount") ? Convert.ToDecimal(row["VAT_Amount"]) : 0,
                        NewsubTotal = !row.IsNull("NewsubTotal") ? Convert.ToDecimal(row["NewsubTotal"]) : 0,
                        BraName = lang == 1 ? row["BRA_NAME"].ToString() : row["BRA_NAME2"].ToString(),

                        IsReportedToZatcaState = isReportedToZatcaState,
                        Color = color
                    });
                }

                return Ok(await Result<List<object>>.SuccessAsync(result));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpPost("Send")]
        public async Task<IActionResult> Send(InvoicesToZatcaVm obj)
        {
            try
            {
                string getEnvironment = await configurationHelper.GetValue(336, session.FacilityId);
                if (string.IsNullOrEmpty(getEnvironment))
                    return Ok(await Result.FailAsync(localization.GetCommonResource("EnvironmentError")));

                long FacilityId = session.FacilityId;
                long UserId = session.UserId;
                int Environment = Convert.ToInt32(getEnvironment);
                string msg = "";

                // initiate session to keep the data about result, to use it in message that will return to frontend
                mvcSession.AddData<InvoicesReportingResult>("InvoicesReportingResult", new InvoicesReportingResult());
                mvcSession.AddData<bool>("hasError", false); // determine if message be as success(green) or error (red)

                switch (obj.InvoiceType)
                {
                    case 1:
                    case 3:
                        msg = await ReportSellTransaction(obj.Invoices, FacilityId, UserId, obj.InvoiceType, obj.SendAll, Environment);
                        break;
                    case 2:
                        msg = await ReportSellTransactionDiscount(obj.Invoices, FacilityId, UserId, obj.InvoiceType, obj.SendAll, Environment);
                        break;
                    case 4:
                    case 15:
                        msg = await ReportPM_Extract_Transactions(obj.Invoices, FacilityId, UserId, obj.InvoiceType, obj.SendAll, Environment);
                        break;
                    case 5:
                        msg = await ReportPM_Extract_TransactionsCredit_Note(obj.Invoices, FacilityId, UserId, obj.InvoiceType, obj.SendAll, Environment);
                        break;
                    case 6:
                        msg = await ReportRE_Transactions(obj.Invoices, FacilityId, UserId, obj.InvoiceType, obj.SendAll, Environment);
                        break;
                    case 8:
                        msg = await ReportRE_Transactions(obj.Invoices, FacilityId, UserId, obj.InvoiceType, obj.SendAll, Environment);
                        break;
                    case 9:
                        msg = await ReportPM_Transactions(obj.Invoices, FacilityId, UserId, obj.InvoiceType, obj.SendAll, Environment);
                        break;
                    case 10:
                        msg = await ReportPM_Transactions(obj.Invoices, FacilityId, UserId, obj.InvoiceType, obj.SendAll, Environment);
                        break;
                    case 11:
                        break;
                    case 13:
                        break;
                    case 14:
                        break;
                    case 16:
                        break;
                    case 17:
                        break;
                    case 18:
                        break;
                    default:
                        return Ok(await Result.FailAsync("Invalid invoice type"));
                }

                if (obj.SendAll)
                {
                    InvoicesReportingResult InvoicesReportingResult = mvcSession.GetData<InvoicesReportingResult>("InvoicesReportingResult");
                    string successMsg = "Total invoices :" + InvoicesReportingResult.InvoicesCount
                        + " , Invoices reported :" + InvoicesReportingResult.Reported + " , Invoices not accepted :" + InvoicesReportingResult.ReportedNotIcepted
                        + " , Invoices not reported have problems :" + InvoicesReportingResult.NotReported;
                    return Ok(await Result.SuccessAsync(successMsg));
                }
                else
                {
                    bool hasError= mvcSession.GetData<bool>("hasError");
                    return hasError ? Ok(await Result.FailAsync(msg)) : Ok(await Result.SuccessAsync());
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }


        [NonAction]
        private async Task<string> ReportSellTransaction(List<InvoicesToZatca> Invoices, long FacilityId, long UserId, int InvoiceAccordingTypeId, bool SendAll, int Environment)
        {
            HttpContext context = this.HttpContext;
            string labMsg = "";
            foreach (var invoice in Invoices)
            {
                await zatcaHelper.Initiate(FacilityId, invoice.BranchId, context);
                ZatcaInvoiceReportResult ZatcaInvoiceReportResult = await zatcaHelper.ReportSellTransaction(invoice.InvoiceId, FacilityId, UserId, InvoiceAccordingTypeId, invoice.BranchId);
                CheckResult(ZatcaInvoiceReportResult, invoice.Code, SendAll, ref labMsg);
            }

            return labMsg;
        }

        [NonAction]
        private async Task<string> ReportSellTransactionDiscount(List<InvoicesToZatca> Invoices, long FacilityId, long UserId, int InvoiceAccordingTypeId, bool SendAll, int Environment)
        {
            HttpContext context = this.HttpContext;
            string labMsg = "";
            foreach (var invoice in Invoices)
            {
                await zatcaHelper.Initiate(FacilityId, invoice.BranchId);
                ZatcaInvoiceReportResult ZatcaInvoiceReportResult = await zatcaHelper.ReportSellTransactionDiscount(invoice.InvoiceId, FacilityId, UserId, InvoiceAccordingTypeId, invoice.BranchId);
                CheckResult(ZatcaInvoiceReportResult, invoice.Code, SendAll, ref labMsg);
            }

            return labMsg;
        }

        [NonAction]
        private async Task<string> ReportPM_Extract_Transactions(List<InvoicesToZatca> Invoices, long FacilityId, long UserId, int InvoiceAccordingTypeId, bool SendAll, int Environment)
        {
            HttpContext context = this.HttpContext;
            string labMsg = "";
            foreach (var invoice in Invoices)
            {
                await zatcaHelper.Initiate(FacilityId, invoice.BranchId, context);
                ZatcaInvoiceReportResult ZatcaInvoiceReportResult = await zatcaHelper.ReportPM_Extract_Transactions(invoice.InvoiceId, FacilityId, UserId, InvoiceAccordingTypeId, invoice.BranchId);
                CheckResult(ZatcaInvoiceReportResult, invoice.Code, SendAll, ref labMsg);
            }

            return labMsg;
        }

        [NonAction]
        private async Task<string> ReportPM_Extract_TransactionsCredit_Note(List<InvoicesToZatca> Invoices, long FacilityId, long UserId, int InvoiceAccordingTypeId, bool SendAll, int Environment)
        {
            HttpContext context = this.HttpContext;
            string labMsg = "";
            foreach (var invoice in Invoices)
            {
                await zatcaHelper.Initiate(FacilityId, invoice.BranchId, context);
                ZatcaInvoiceReportResult ZatcaInvoiceReportResult = await zatcaHelper.ReportPM_Extract_TransactionsCredit_Note(invoice.InvoiceId, FacilityId, UserId, InvoiceAccordingTypeId, invoice.BranchId);
                CheckResult(ZatcaInvoiceReportResult, invoice.Code, SendAll, ref labMsg);
            }

            return labMsg;
        }

        [NonAction]
        private async Task<string> ReportRE_Transactions(List<InvoicesToZatca> Invoices, long FacilityId, long UserId, int InvoiceAccordingTypeId, bool SendAll, int Environment)
        {
            HttpContext context = this.HttpContext;
            string labMsg = "";
            foreach (var invoice in Invoices)
            {
                await zatcaHelper.Initiate(FacilityId, invoice.BranchId, context);
                ZatcaInvoiceReportResult ZatcaInvoiceReportResult = await zatcaHelper.ReportRE_Transactions(invoice.InvoiceId, FacilityId, UserId, InvoiceAccordingTypeId, invoice.BranchId);
                CheckResult(ZatcaInvoiceReportResult, invoice.Code, SendAll, ref labMsg);
            }

            return labMsg;
        }

        [NonAction]
        private async Task<string> ReportPM_Transactions(List<InvoicesToZatca> Invoices, long FacilityId, long UserId, int InvoiceAccordingTypeId, bool SendAll, int Environment)
        {
            HttpContext context = this.HttpContext;
            string labMsg = "";
            foreach (var invoice in Invoices)
            {
                await zatcaHelper.Initiate(FacilityId, invoice.BranchId, context);
                ZatcaInvoiceReportResult ZatcaInvoiceReportResult = await zatcaHelper.ReportPM_Transactions(invoice.InvoiceId, FacilityId, UserId, InvoiceAccordingTypeId, invoice.BranchId);
                CheckResult(ZatcaInvoiceReportResult, invoice.Code, SendAll, ref labMsg);
            }

            return labMsg;
        }


        [NonAction]
        private void CheckResult(ZatcaInvoiceReportResult ZatcaInvoiceReportResult, string Code, bool SendAll, ref string labMsg)
        {
            if (SendAll)
            {
                InvoicesReportingResult InvoicesReportingResult = mvcSession.GetData<InvoicesReportingResult>("InvoicesReportingResult");
                InvoicesReportingResult.InvoicesCount = InvoicesReportingResult.InvoicesCount + 1;
                if (ZatcaInvoiceReportResult.Success == false && ZatcaInvoiceReportResult.Data != null)
                {
                    if (!string.IsNullOrEmpty(ZatcaInvoiceReportResult.Data.ReportingResult))
                        InvoicesReportingResult.ReportedNotIcepted = InvoicesReportingResult.ReportedNotIcepted + 1;
                }
                else if (ZatcaInvoiceReportResult.Success == false)
                    InvoicesReportingResult.NotReported = InvoicesReportingResult.NotReported + 1;
                else
                    InvoicesReportingResult.Reported = InvoicesReportingResult.Reported + 1;

                mvcSession.AddData<InvoicesReportingResult>("InvoicesReportingResult", InvoicesReportingResult);
            }
            else
            {
                if (ZatcaInvoiceReportResult.Success == false && ZatcaInvoiceReportResult.Data != null)
                {
                    if (!string.IsNullOrEmpty(ZatcaInvoiceReportResult.Data.ReportingResult))
                    {
                        string ErrorMessages = "Invoicecode = " + Code;
                        int counter = 0;
                        foreach (ValidationResultMessage ValidationResults in ZatcaInvoiceReportResult.Data.ErrorMessages)
                        {
                            counter += 1;
                            ErrorMessages += "     ErrorMessages " + counter + ": (" + ValidationResults.Message + ")";
                        }
                        mvcSession.AddData<bool>("hasError", true);
                        labMsg = ErrorMessages;
                    }
                    return;
                }
                else if (ZatcaInvoiceReportResult.Success == false)
                {
                    mvcSession.AddData<bool>("hasError", true);
                    labMsg = ZatcaInvoiceReportResult.Message + " Invoicecode = " + Code;
                    return;
                }
                else
                {
                    labMsg = localization.GetCommonResource("OperationSucces");
                }
            }
        }

        #region ================================ DDL ================================
        [HttpGet("DDLSysInvoiceType")]
        public async Task<IActionResult> DDLSysInvoiceType(int lang = 1)
        {
            try
            {
                var mySystem = await mainServiceManager.SysSystemService.GetAll(x => x.SystemId, x => x.Isdel == false && x.ShowInHome == true);
                var sysIds = mySystem.Data.ToList();
                var sysInvoiceType = new SelectList(new List<DDListItem<SysInvoiceAccordingType>>());
                sysInvoiceType = await ddlHelper.GetAnyLis<SysInvoiceAccordingType, long>(x => x.IsDeleted == false && sysIds.Contains((int)(x.SystemId ?? 0)),
                    "Id", lang == 1 ? "Name" : "Name2");

                return Ok(await Result<SelectList>.SuccessAsync(sysInvoiceType));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLZatcaInvoiceType")]
        public async Task<IActionResult> DDLZatcaInvoiceType(int lang = 1)
        {
            try
            {
                var zatcaInvoiceType = new SelectList(new List<DDListItem<SysZatcaInvoiceType>>());
                zatcaInvoiceType = await ddlHelper.GetAnyLis<SysZatcaInvoiceType, long>(x => x.IsDeleted == false,
                    "Id", lang == 1 ? "Name" : "Name2");

                return Ok(await Result<SelectList>.SuccessAsync(zatcaInvoiceType));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        #endregion ================================ End DDL ================================
    }
}
