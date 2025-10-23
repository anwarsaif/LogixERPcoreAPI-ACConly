using Logix.Application.Common;
using Logix.Application.DTOs.Main;
using Logix.Application.DTOs.PM;
using Logix.Application.DTOs.PM.PmProjects;
using Logix.Application.DTOs.PM.Shared;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Org.BouncyCastle.Asn1.Ocsp;

namespace Logix.MVC.LogixAPIs.PM
{

    // ربط المستخلصات في الاوراق المالية
    public class PMPaymentFinancialPaperController : BasePMApiController
    {
        private readonly IPMServiceManager pMServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IWFServiceManager wFServiceManager;
        private readonly IMainServiceManager mainServiceManager;


        public PMPaymentFinancialPaperController(IPMServiceManager pMServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IWFServiceManager wFServiceManager, IMainServiceManager mainServiceManager)
        {
            this.pMServiceManager = pMServiceManager;
            this.permission = permission;
            this.session = session;
            this.localization = localization;
            this.wFServiceManager = wFServiceManager;
            this.mainServiceManager = mainServiceManager;
        }

        #region Index Page


        [HttpPost("Search")]
        public async Task<IActionResult> Search(PaginatedRequest<PmExtractTransactionFilterDto> request)
        {

            try
            {

                var chk = await permission.HasPermission(2004, PermissionType.Show);

                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var filter = request.Filter;

                var result = await pMServiceManager.PmExtractTransactionService.GetExtractTransactionsForPaymentFinancialPaper(filter);
                if (request.PageNumber > 0 && request.PageSize > 0)
                {
                    var paginatedData = await PaginatedResult<object>.SuccessAsync(
                    result.Data.ToList(),
                    request.PageNumber,
                        request.PageSize);
                    return Ok(paginatedData);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"======= Exp in {this.GetType()}: {ex.Message}"));
            }
        }


        #endregion

        #region Edit Page

        //   مدفوعات العميل  
        [HttpGet("GetCustomerPayment")]
        public async Task<IActionResult> GetCustomerPayment(string CustomerCode)
        {
            try
            {
                var chk = await permission.HasPermission(2004, PermissionType.Show);

                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var result = await pMServiceManager.PmExtractTransactionService.GetCustomerPayment(CustomerCode);
                return Ok(result);

            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"Error in {this.GetType().Name}.{nameof(GetCustomerPayment)}: {ex.Message}"));
            }
        }


        //  تسديد
        [HttpPost("Pay")]
        public async Task<IActionResult> Pay(List<CustomerPayment> objects)
        {
            try
            {
                var chk = await permission.HasPermission(2004, PermissionType.Show);

                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                var result = await pMServiceManager.PmExtractTransactionService.Pay(objects);
                return Ok(result);

            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"Error in {this.GetType().Name}.{nameof(Pay)}: {ex.Message}"));
            }
        }
        #endregion


    }
}
