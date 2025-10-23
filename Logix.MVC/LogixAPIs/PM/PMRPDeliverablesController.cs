using Logix.Application.Common;
using Logix.Application.DTOs.PM;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.PM;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;

namespace Logix.MVC.LogixAPIs.PM
{
    /// بنك المخرجات
    public class PMRPDeliverablesController : BasePMApiController
    {
        private readonly IPMServiceManager pMServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ILocalizationService localization;



        public PMRPDeliverablesController(IPMServiceManager pMServiceManager, IPermissionHelper permission, ILocalizationService localization)
        {
            this.pMServiceManager = pMServiceManager;
            this.permission = permission;
            this.localization = localization;

        }

        #region Index Page


        [HttpPost("Search")]
        public async Task<IActionResult> Search(PmProjectsDeliverableFilterDto filter)
        {
            var chk = await permission.HasPermission(2089, PermissionType.Show);

            if (!chk)
                return Ok(await Result.AccessDenied("AccessDenied"));

            try
            {
                filter.ProjectCode ??= 0;

                var items = await pMServiceManager.PmProjectsDeliverableService.GetAllVW(e => e.IsDeleted == false && e.DeliveryDate != null
                && ((filter.ProjectCode == 0) || ((e.ProjectCode == filter.ProjectCode)))
                && (string.IsNullOrEmpty(filter.ProjectName) || ((e.ProjectName != null && e.ProjectName.Contains(filter.ProjectName))))
                && (string.IsNullOrEmpty(filter.Name) || ((e.Name != null && e.Name.Contains(filter.Name))))
                && (string.IsNullOrEmpty(filter.CustomerCode) || ((e.CustomerCode != null && e.CustomerCode.Contains(filter.CustomerCode))))
                && (string.IsNullOrEmpty(filter.CustomerName) || ((e.CustomerName != null && e.CustomerName.Contains(filter.CustomerName))))
                && (string.IsNullOrEmpty(filter.ItemName) || ((e.ItemName != null && e.ItemName.Contains(filter.ItemName))))
                && (string.IsNullOrEmpty(filter.Note) || ((e.Note != null && e.Note.Contains(filter.Note))))

                );

                if (!items.Succeeded)
                    return Ok(await Result<object>.FailAsync(items.Status.message));
                if (!items.Data.Any())
                    return Ok(await Result<object>.SuccessAsync(new List<object>(), localization.GetResource1("NosearchResult")));

                var res = items.Data.AsQueryable();

                if (!string.IsNullOrEmpty(filter.From) && !string.IsNullOrEmpty(filter.To))
                {
                    var DateFrom = DateHelper.StringToDate(filter.From);
                    var DateTo = DateHelper.StringToDate(filter.To);
                    res = res.Where(x => x.DeliveryDate != null && x.DeliveryDate != "" && DateHelper.StringToDate(x.DeliveryDate) >= DateFrom && DateHelper.StringToDate(x.DeliveryDate) <= DateTo);
                }

                if (!res.Any())
                    return Ok(await Result<object>.SuccessAsync(new List<object>(), localization.GetResource1("NosearchResult")));


                var result = res.Select(g => new
                {
                    g.Id,
                    g.ProjectCode,
                    g.ProjectName,
                    g.CustomerCode,
                    g.CustomerName,
                    g.Name,
                    g.Type,
                    g.Qty,
                    g.DeliveryDate,
                    g.StatusName,
                    g.Note,
                    g.FilePdf,
                })
                    .ToList();

                return Ok(await Result<object>.SuccessAsync(result, ""));
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"======= Exp in {this.GetType()}: {ex.Message}"));
            }
        }


        #endregion



    }
}
