using DocumentFormat.OpenXml.Spreadsheet;
using Logix.Application.Common;
using Logix.Application.DTOs.Main;
using Logix.Application.DTOs.SAL;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Services;
using Logix.Application.Wrapper;
using Logix.Domain.Main;
using Logix.Domain.PM;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;

namespace Logix.MVC.LogixAPIs.PM
{
    //   ربط الفواتير بالمشروع
    public class SalFilterDto
    {
        public string CustomerCode { get; set; } = null!;
    }
    public class PMUpdateprojectInvoicesController : BasePMApiController
    {
        private readonly ISalServiceManager salServiceManager;
        private readonly IMainServiceManager mainServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IApiDDLHelper ddlHelper;



        public PMUpdateprojectInvoicesController(IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IMainServiceManager mainServiceManager, ISalServiceManager salServiceManager, IApiDDLHelper ddlHelper)
        {
            this.permission = permission;
            this.session = session;
            this.localization = localization;
            this.salServiceManager = salServiceManager;
            this.mainServiceManager = mainServiceManager;
            this.ddlHelper = ddlHelper;
        }

        #region Index Page

        [HttpPost("Search")]
        public async Task<IActionResult> Search(PaginatedRequest<SalFilterDto> request)
        {
            var chk = await permission.HasPermission(1183, PermissionType.Add);

            if (!chk)
                return Ok(await Result.AccessDenied("AccessDenied"));

            try
            {
                var filter = request.Filter;
                var items = await salServiceManager.SalTransactionService.GetAllVW(e => e.IsDeleted == false
                && e.TransTypeId == 1
                && e.CustomerCode == filter.CustomerCode
                );

                if (!items.Succeeded)
                    return Ok(await Result<object>.FailAsync(items.Status.message));

                if (!items.Data.Any())
                    return Ok(await Result<object>.SuccessAsync(new List<object>(), localization.GetResource1("NosearchResult")));

                var result = items.Data.Select(x =>
                new
                {
                    x.Id,
                    x.Code,
                    x.NewSubtotal,
                    x.Date1,
                    x.DocumentNote,
                    x.ProjectCode,
                    x.ProjectName,
                });
                if (request.PageNumber > 0 && request.PageSize > 0)
                {
                    var paginatedData = await PaginatedResult<object>.SuccessAsync(
                        result,
                        request.PageNumber,
                        request.PageSize);
                    return Ok(paginatedData);
                }

                return Ok(await Result<object>.SuccessAsync(result.ToList(), "Search Completed", 200));
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"Error in {nameof(Search)}: {ex.Message}"));
            }
        }


        [HttpGet("BindCustomerProjects")]
        public async Task<IActionResult> BindCustomerProjects(string CustomerCode)
        {
            try
            {
                var chk = await permission.HasPermission(1183, PermissionType.Add);

                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var list = new SelectList(new List<DDListItem<SysUser>>());

                // Validate Customer Code
                if (string.IsNullOrEmpty(CustomerCode))
                    return Ok(await Result<object>.FailAsync(localization.GetMessagesResource("NoIdInUpdate")));

                // Fetch Customer
                var item = await mainServiceManager.SysCustomerService.GetOneVW(x =>
                    x.IsDeleted == false &&
                    x.Code == CustomerCode &&
                    x.CusTypeId == 2 &&
                    x.FacilityId == session.FacilityId);

                if (!item.Succeeded)
                    return Ok(await Result<object>.FailAsync(item.Status.message));

                if (item.Data == null)
                    return Ok(await Result<object>.FailAsync(session.Language == 1 ? "العميل غير موجود" : "Customer not exist"));

                list = await ddlHelper.GetAnyLis<PmProjectsEditVw, long>(x => x.IsDeleted == false && x.StatusId == 1 && x.CustomerId == item.Data.Id, "Id", "Name");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"Error in {this.GetType()}: {ex.Message}"));
            }
        }


        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(UpdateProjectSALTransactionsDto obj)
        {

            try
            {

                var chk = await permission.HasPermission(1183, PermissionType.Add);

                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (obj.ProjectId <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("ProjectNo")));

                if (obj.InvoiceIds.Count() <= 0)
                    return Ok(await Result<object>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));


                var result = await salServiceManager.SalTransactionService.UpdateProjectSALTransactions(obj);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"Error in {this.GetType().Name}.{nameof(Edit)}: {ex.Message}"));

            }

        }


        #endregion


    }
}
