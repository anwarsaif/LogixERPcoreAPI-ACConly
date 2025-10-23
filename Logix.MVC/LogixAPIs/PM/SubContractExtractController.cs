using Logix.Application.Common;
using Logix.Application.DTOs.Main;
using Logix.Application.DTOs.PM;
using Logix.Application.DTOs.PM.PmProjects;
using Logix.Application.DTOs.PM.SubContract;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Services;
using Logix.Application.Wrapper;
using Logix.Domain.Main;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.PM.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Globalization;

namespace Logix.MVC.LogixAPIs.PM
{
    public class SubContractExtractController : BasePMApiController
    {
        private readonly IPMServiceManager pMServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;

        public SubContractExtractController(IPMServiceManager pMServiceManager,
            IPermissionHelper permission,
            ICurrentData session,
               ILocalizationService localization)
        {
            this.pMServiceManager = pMServiceManager;
            this.permission = permission;
            this.session = session;
            this.localization = localization;
        }

        #region ============================ Follow Up Extracts SubContrac ============================
        //ProjectsMangement/Extracts_SubContract/FollowUP_Extract_SubC
        [HttpPost("FollowUpExtracts")]
        public async Task<IActionResult> FollowUpExtracts(PmExtractTransactionFilterDto filter)
        {
            try
            {
                var chk = await permission.HasPermission(1896, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                filter.PaymentTermsId = 2;
                var items = await Filter(filter);

                if (items.Succeeded)
                {
                    var res = items.Data;

                    List<PmExtractTransactionVM> final = new();
                    foreach (DataRow row in res.Rows)
                    {
                        final.Add(new PmExtractTransactionVM
                        {
                            Id = Convert.ToInt64(row["ID"]),
                            Code = row["Code"].ToString(),
                            Date1 = row["Date1"].ToString(),
                            StartDate = row["StartDate"].ToString(),
                            EndDate = row["EndDate"].ToString(),
                            CustomerCode = row["CustomerCode"].ToString(),
                            CustomerName = row["CustomerName"].ToString(),
                            ProjectCode = Convert.ToInt64(row["Project_Code"]),
                            ProjectName = row["Project_Name"].ToString(),
                            Subtotal = Convert.ToDecimal(row["Subtotal"]),
                            CurrencyName = row["Currency_Name"].ToString(),
                            LastStatusName = row["LastStatus_Name"].ToString(),
                            DateChange = row["Date_Change"].ToString(),
                            DateRemind = row["Date_Remind"].ToString(),
                            LastNote = row["LastNote"].ToString()
                        });
                    }

                    return Ok(await Result<List<PmExtractTransactionVM>>.SuccessAsync(final));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"======= Exp in FollowUpExtracts SubContractExtractController : {ex.Message}"));
            }
        }

        //this action request from FollowUP_Extract page,, selected chkbox from grid view and press add from operations
        //selected rows must save its id in SelectedIds in 
        [HttpPost("AddComments")]
        public async Task<ActionResult> AddComments(PmExtractTransactionsChangeStatusDto obj)
        {
            try
            {
                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                if (string.IsNullOrEmpty(obj.SelectedIds))
                    return Ok(await Result.FailAsync($"{localization.GetPMResource("selectingProcessextract")}"));

                var selctedIdsarr = obj.SelectedIds.Split(',');
                int count = 0;
                foreach (var id in selctedIdsarr)
                {
                    PmExtractTransactionsChangeStatusDto newObj = new()
                    {
                        TransactionId = Convert.ToInt64(id),
                        Description = obj.Description,
                        DateChange = DateTime.Now.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture),
                        DateRemind = obj.DateRemind,
                        StatusId = obj.StatusId
                    };
                    var add = await pMServiceManager.PmExtractTransactionsChangeStatusService.Add(newObj);
                    if (add.Succeeded) ++count;
                }
                if (count > 0)
                {
                    string msg = localization.GetPMResource("Statusextract");
                    msg += " " + count.ToString();
                    return Ok(await Result<string>.SuccessAsync(msg));
                }
                else
                    return Ok(await Result.FailAsync($"{localization.GetPMResource("selectingProcessextract")}"));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in AddComments SubContractExtractController, MESSAGE: {ex.Message}"));
            }
        }
        #endregion ========================== End Follow Up Extracts SubContrac =========================

        //ProjectsMangement/Extracts_SubContract/Extract_SubC
        [HttpPost("Search")]
        public async Task<IActionResult> Search(PmExtractTransactionFilterDto filter)
        {
            try
            {
                var chk = await permission.HasPermission(989, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                filter.PaymentTermsId = 2;
                var items = await Filter(filter);

                if (items.Succeeded)
                {
                    var res = items.Data;

                    List<PmExtractTransactionVM> final = new();
                    foreach (DataRow row in res.Rows)
                    {
                        final.Add(new PmExtractTransactionVM
                        {
                            Id = Convert.ToInt64(row["ID"]),
                            Code = row["Code"].ToString(),
                            Date1 = row["Date1"].ToString(),
                            StartDate = row["StartDate"].ToString(),
                            EndDate = row["EndDate"].ToString(),
                            CustomerCode = row["CustomerCode"].ToString(),
                            CustomerName = row["CustomerName"].ToString(),
                            ProjectCode = Convert.ToInt64(row["Project_Code"]),
                            ProjectName = row["Project_Name"].ToString(),
                            BraName = row["BRA_NAME"].ToString(),
                            Total = Convert.ToDecimal(row["Total"]),
                            Vat = Convert.ToDecimal(row["Vat"]),
                            DiscountAmount = Convert.ToDecimal(row["Discount_Amount"]),
                            Subtotal = Convert.ToDecimal(row["Subtotal"]),
                            CurrencyName = row["Currency_Name"].ToString(),
                            ExchangeRate = Convert.ToDecimal(row["Exchange_Rate"]),
                            ValueInLocalCurrency = Math.Round(Convert.ToDecimal(row["Subtotal"]) * Convert.ToDecimal(row["Exchange_Rate"]), 2)
                        });
                    }

                    return Ok(await Result<List<PmExtractTransactionVM>>.SuccessAsync(final));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"======= Exp in Search SubContractExtractController : {ex.Message}"));
            }
        }

        //[HttpPost("Add")]

        [HttpPost("Add")]
        public async Task<IActionResult> Add(PMSubContractAddDto entity)
        {

            var chk = await permission.HasPermission(256, PermissionType.Add);

            if (chk == false)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {

                var items = await pMServiceManager.PMSubContractService.Add(entity);

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<PMProjectsDto>.FailAsync($"======= Exp in  : {ex.Message}"));

            }

        }
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var chk = await permission.HasPermission(989, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await pMServiceManager.PmExtractTransactionService.Remove(id, 58);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result<List<SysExchangeRateDto>>.FailAsync($"====== Exp in Delete SubContractExtractController, MESSAGE: {ex.Message}"));
            }
        }


        //this function is shared,, used with any search function
        private async Task<IResult<DataTable>> Filter(PmExtractTransactionFilterDto filter)
        {
            //convert null number to zero
            filter.TransTypeId ??= 0;
            filter.BranchId ??= 0;
            filter.PaymentTermsId ??= 0;
            filter.ProjectId ??= 0;
            filter.StatusId ??= 0;
            filter.UserType ??= 0;

            if (filter.Total == 0) filter.Total = null;

            //convert empty string to null
            if (string.IsNullOrEmpty(filter.Code)) filter.Code = null;
            if (string.IsNullOrEmpty(filter.CustomerCode)) filter.CustomerCode = null;
            if (string.IsNullOrEmpty(filter.CustomerName)) filter.CustomerName = null;
            filter.ProjectCode ??= 0;
            if (string.IsNullOrEmpty(filter.ProjectName)) filter.ProjectName = null;
            if (string.IsNullOrEmpty(filter.ParentProjectCode)) filter.ParentProjectCode = null;
            if (string.IsNullOrEmpty(filter.ProjectManagerCode)) filter.ProjectManagerCode = null;
            if (string.IsNullOrEmpty(filter.ProjectManagerName)) filter.ProjectManagerName = null;
            if (string.IsNullOrEmpty(filter.InvCode)) filter.InvCode = null;
            if (string.IsNullOrEmpty(filter.ItemCode)) filter.ItemCode = null;

            if (string.IsNullOrEmpty(filter.StartDate) || string.IsNullOrEmpty(filter.EndDate))
                filter.StartDate = null;

            if (filter.BranchId == 0)
                filter.BranchsId = session.Branches;

            if (session.SalesType == 2)
            {
                filter.UserType = 2;
                filter.EmpId = session.EmpId;
            }

            var items = await pMServiceManager.PmExtractTransactionService.GetExtracts(filter);
            return items;
        }
    }
}