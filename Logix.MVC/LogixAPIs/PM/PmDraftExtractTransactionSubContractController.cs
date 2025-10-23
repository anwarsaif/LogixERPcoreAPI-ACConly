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

namespace Logix.MVC.LogixAPIs.PM
{
    public class PmDraftExtractTransactionSubContractController : BasePMApiController
    {
        private readonly IPMServiceManager pmServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly IAccServiceManager accServiceManager;
        private readonly ILocalizationService localization;

        public PmDraftExtractTransactionSubContractController(IPMServiceManager pmServiceManager,
            IPermissionHelper permission,
            ICurrentData session,
               IAccServiceManager accServiceManager,
               ILocalizationService localization)
        {
            this.pmServiceManager = pmServiceManager;
            this.permission = permission;
            this.session = session;
            this.accServiceManager = accServiceManager;
            this.localization = localization;
        }
        #region ==================================== Extracts ====================================
        //ProjectsMangement/Extracts/Extract
        [HttpPost("Search")]
        public async Task<IActionResult> Search(PmExtractTransactionFilterDto filter)
        {
            try
            {
                var chk = await permission.HasPermission(2064, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                filter.PaymentTermsId =7;
                var items = await Filter(filter);

                if (items.Succeeded)
                {
                    var res = items.Data;
                    List<PmExtractTransactionVM> final = new();
                    foreach (DataRow row in res.Rows)
                    {
                        Console.WriteLine(row);
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
                            CurrencyName = row["Currency_Name"].ToString(),
                            ExchangeRate = Convert.ToDecimal(row["Exchange_Rate"]),
                            StartDate = row["StartDate"].ToString(),
                            EndDate = row["EndDate"].ToString(),
                            ValueInLocalCurrency = Math.Round(Convert.ToDecimal(row["Subtotal"])* Convert.ToDecimal(row["Exchange_Rate"]),2),

                          
                        });
                    }

                    return Ok(await Result<List<PmExtractTransactionVM>>.SuccessAsync(final));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"======= Exp in Search PmExtractTransactionController : {ex.Message}"));
            }
        }

        //[HttpPost("Add")]

        [HttpPost("Add")]
        public async Task<IActionResult> Add(PmExtractTransactionAddDto entity)
        {

            var chk = await permission.HasPermission(2064, PermissionType.Add);

            if (chk == false)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {

                var items = await pmServiceManager.PmDraftExtractTransactionSubContractService.Add(entity);

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<PMProjectsDto>.FailAsync($"======= Exp in  : {ex.Message}"));

            }

        }
        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(PmExtractTransactionEditPostDto entity)
        {

            var chk = await permission.HasPermission(2064, PermissionType.Edit);

            if (chk == false)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {

                var items = await pmServiceManager.PmDraftExtractTransactionSubContractService.Update(entity);

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
                var chk = await permission.HasPermission(2064, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await pmServiceManager.PmDraftExtractTransactionSubContractService.Remove(id);
                return Ok(delete);  
            }
            catch (Exception ex)
            {
                return Ok(await Result<List<SysExchangeRateDto>>.FailAsync($"====== Exp in Delete PmExtractTransactionController, MESSAGE: {ex.Message}"));
            }
        }
       
        
        [HttpGet("GetProjectExtractInfoByProjectCode")]
        public async Task<IActionResult> GetProjectExtractInfoByProjectCode(long Code)
        {
            try
            {
                var chk = await permission.HasPermission(294, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
/*
                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));
*/
                var result = await pmServiceManager.PmExtractTransactionSubContractService.GetProjectExtractInfo(Code);
                return Ok(result);  
            }
            catch (Exception ex)
            {
                return Ok(await Result<PMProjectExtractInfoDto>.FailAsync($"====== Exp in Delete PmExtractTransactionController, MESSAGE: {ex.Message}"));
            }
        }
             
        [HttpGet("GetPMProjectsItemsByPMCodeItemCode")]
        public async Task<IActionResult> GetPMProjectsItemsByPMCodeItemCode(long ProjectCode ,string ProjectItemCode)
        {
            try
            {
                if(ProjectCode==0)
                return Ok(await Result<PMExtractItemDto>.FailAsync(localization.GetMessagesResource("SelectProjectContract")));

                int PaymentTermsId = 2;
                PMExtractItemDto pMExtractItemDto = new PMExtractItemDto(); 
               var chk = await permission.HasPermission(989, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var projectsItemVMResult = await pmServiceManager.PMProjectsItemService.GetOneVW(x=>x.ProjectCode== ProjectCode && x.ItemCode== ProjectItemCode && x.IsDeleted==false );
                var projectsItemVM = projectsItemVMResult.Data;
                var pmExtractTransactionsProductVMResult = await pmServiceManager.PmExtractTransactionsProductService.GetAllVW(x=>x.IsDeletedM==false && x.PItemsId==projectsItemVM.Id && x.PaymentTermsId == PaymentTermsId);
                var pmExtractTransactionsProductVM = pmExtractTransactionsProductVMResult.Data;
                // ------- 
                pMExtractItemDto.AmountPrevious = pmExtractTransactionsProductVM.Sum(x=>x.AmountRate);
                pMExtractItemDto.QtyPrevious = pmExtractTransactionsProductVM.Sum(x=>x.Qty);
                pMExtractItemDto.RateAll = pmExtractTransactionsProductVM.Sum(x=>x.Rate);

              var SumAmountResult =  await pmServiceManager.PmExtractTransactionsProductService.GetAllVW
                      (x => x.IsDeletedM == false && x.ItemCode == projectsItemVM.ItemCode 
                       && x.PItemsId != projectsItemVM.Id
                      && x.PaymentTermsId == PaymentTermsId);
                if(SumAmountResult.Data != null)
                {
                    pMExtractItemDto.SumAmount = SumAmountResult.Data.Sum(x=>x.AmountRate);
                }
                pMExtractItemDto.Total= projectsItemVM.Total;
                pMExtractItemDto.Id= projectsItemVM.Id;
                pMExtractItemDto.ItemName = projectsItemVM.ItemName;
                pMExtractItemDto.ItemId = projectsItemVM.ItemId;
                pMExtractItemDto.ProjectId = projectsItemVM.ProjectId;
                pMExtractItemDto.UnitId = projectsItemVM.UnitId;
                pMExtractItemDto.UnitName = projectsItemVM.UnitName;
                pMExtractItemDto.Qty = projectsItemVM.Qty;
                pMExtractItemDto.Price = projectsItemVM.Price;
                pMExtractItemDto.ItemCode = projectsItemVM.ItemCode;
                pMExtractItemDto.Note = projectsItemVM.Note;
                pMExtractItemDto.QtyApprove = projectsItemVM.Qty;
                pMExtractItemDto.PriceApprove = projectsItemVM.Total;

                return Ok(Result<PMExtractItemDto>.Success(pMExtractItemDto));
            }
            catch (Exception ex)
            {
                return Ok(await Result<PMExtractItemDto>.FailAsync($"exp NotFound"));
               // return Ok(await Result<PMExtractItemDto>.FailAsync($"====== Exp in Delete PmExtractTransactionController, MESSAGE: {ex.Message}"));
            }
        }
        [HttpGet("GetByIdForEdit")]
        public async Task<IActionResult> GetByIdForEdit(long Id)
        {
               var chk = await permission.HasPermission(989, PermissionType.Show);
            var chk2 = await permission.HasPermission(989, PermissionType.Edit);

            if (chk==false &&  chk2==false)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }  
            if (Id <= 0)
            {
                return Ok(await Result<PMProjectsDto>.FailAsync($"not found this extract"));

            }
            try
            {

                var items = await pmServiceManager.PmDraftExtractTransactionSubContractService.GetForEditById(Id);
               
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<PMProjectsDto>.FailAsync($"======= Exp in  : {ex.Message}"));

            }
        }

        #endregion ================================== End Extracts ===================================


        [HttpPost("Filter")]
        //this function is shared,, used with any search function
        private async Task<IResult<DataTable>> Filter(PmExtractTransactionFilterDto filter)
        {
            //convert null number to zero
            filter.TransTypeId ??= 0;
            filter.BranchId ??= 0;
            filter.PaymentTermsId ??=0;
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
           // if (string.IsNullOrEmpty(filter.ParentProjectCode)) filter.ParentProjectCode = null;
            if (string.IsNullOrEmpty(filter.ProjectManagerCode)) filter.ProjectManagerCode = null;
            if (string.IsNullOrEmpty(filter.ProjectManagerName)) filter.ProjectManagerName = null;
            if (string.IsNullOrEmpty(filter.InvCode)) filter.InvCode = null;
            if (string.IsNullOrEmpty(filter.ItemCode)) filter.ItemCode = null;

            if (string.IsNullOrEmpty(filter.StartDate) || string.IsNullOrEmpty(filter.EndDate))
            {
                filter.StartDate = null;
                filter.EndDate = null;

            }
               

            if (filter.BranchId == 0)
                filter.BranchsId = session.Branches;

            if (session.SalesType == 2)
            {
                filter.UserType = 2;
                filter.EmpId = session.EmpId;
            }

            var items = await pmServiceManager.PmDraftExtractTransactionSubContractService.GetExtracts(filter);
            return items;
        }



    }




}
