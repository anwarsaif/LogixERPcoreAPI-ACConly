using Logix.Application.Common;
using Logix.Application.DTOs.PM;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.PM.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Logix.MVC.LogixAPIs.PM
{
    public class PmReportsController : BasePMApiController
    {
        private readonly IPMServiceManager pmServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;

        public PmReportsController(IPMServiceManager pmServiceManager,
            IPermissionHelper permission,
            ICurrentData session,
               ILocalizationService localization)
        {
            this.pmServiceManager = pmServiceManager;
            this.permission = permission;
            this.session = session;
            this.localization = localization;
        }

        [HttpPost("RpExtract")]
        public async Task<IActionResult> RpExtract(PmExtractTransactionFilterDto filter)
        {
            try
            {
                var chk = await permission.HasPermission(940, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

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

                filter.PaymentTermsId = 1;

                var items = await pmServiceManager.PmExtractTransactionService.GetExtracts(filter);

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
                            BraName = row["BRA_NAME"].ToString(),
                            Total = Convert.ToDecimal(row["Total"]),
                            Vat = Convert.ToDecimal(row["Vat"]),
                            DiscountAmount = Convert.ToDecimal(row["Discount_Amount"]),
                            Subtotal = Convert.ToDecimal(row["Subtotal"]),

                            StartDate = row["StartDate"].ToString(),
                            EndDate = row["EndDate"].ToString()
                        });
                    }

                    return Ok(await Result<List<PmExtractTransactionVM>>.SuccessAsync(final));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"======= Exp in RpExtract ReportsController : {ex.Message}"));
            }
        }
   
    
    }
}