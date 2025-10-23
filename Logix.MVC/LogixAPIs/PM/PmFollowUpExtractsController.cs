using Logix.Application.Common;
using Logix.Application.DTOs.Main;
using Logix.Application.DTOs.PM;
using Logix.Application.DTOs.PM.PmProjects;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.Main;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.PM.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Globalization;

namespace Logix.MVC.LogixAPIs.PM
{
    public class PmFollowUpExtractsController : BasePMApiController
    {
        private readonly IPMServiceManager pmServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;

        private readonly ILocalizationService localization;

        public PmFollowUpExtractsController(IPMServiceManager pmServiceManager,
            IPermissionHelper permission,
            ICurrentData session,

               ILocalizationService localization)
        {
            this.pmServiceManager = pmServiceManager;
            this.permission = permission;
            this.session = session;

            this.localization = localization;
        }

        //ProjectsMangement/Extracts/FollowUP_Extract
        [HttpPost("Search")]
        public async Task<IActionResult> Search(PmExtractTransactionFilterDto filter)
        {
            try
            {
                var chk = await permission.HasPermission(1898, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                filter.PaymentTermsId = 1;
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
                            CustomerCode = row["CustomerCode"].ToString(),
                            CustomerName = row["CustomerName"].ToString(),
                            ProjectCode = Convert.ToInt64(row["Project_Code"]),
                            ProjectName = row["Project_Name"].ToString(),
                            Subtotal = Convert.ToDecimal(row["Subtotal"]),
                            ExchangeRate = Convert.ToDecimal(row["Exchange_Rate"]),
                            LastStatusName = row["LastStatus_Name"].ToString(),
                            DateChange = row["Date_Change"].ToString(),
                            DateRemind = row["Date_Remind"].ToString(),
                            LastNote = row["LastNote"].ToString(),
                        });
                    }

                    return Ok(await Result<List<PmExtractTransactionVM>>.SuccessAsync(final));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"======= Exp in FollowUpExtracts PmExtractTransactionController : {ex.Message}"));
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
                    var add = await pmServiceManager.PmExtractTransactionsChangeStatusService.Add(newObj);
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
                return Ok(await Result.FailAsync($"====== Exp in AddComments PmExtractTransactionController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Filter")]
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

            var items = await pmServiceManager.PmExtractTransactionService.GetExtracts(filter);
            return items;
        }

    }


}
