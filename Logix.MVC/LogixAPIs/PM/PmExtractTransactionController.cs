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
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Globalization;

namespace Logix.MVC.LogixAPIs.PM
{
    public class PmExtractTransactionController : BasePMApiController
    {
        private readonly IPMServiceManager pmServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly IAccServiceManager accServiceManager;
        private readonly ILocalizationService localization;

        public PmExtractTransactionController(IPMServiceManager pmServiceManager,
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
                var chk = await permission.HasPermission(294, PermissionType.Show);
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
                            BraName = row["BRA_NAME"].ToString(),
                            Total = Convert.ToDecimal(row["Total"]),
                            Vat = Convert.ToDecimal(row["Vat"]),
                            DiscountAmount = Convert.ToDecimal(row["Discount_Amount"]),
                            Subtotal = Convert.ToDecimal(row["Subtotal"]),
                            CurrencyName = row["Currency_Name"].ToString(),
                            ExchangeRate = Convert.ToDecimal(row["Exchange_Rate"])
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

            var chk = await permission.HasPermission(256, PermissionType.Add);

            if (chk == false)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {

                var items = await pmServiceManager.PmExtractTransactionService.Add(entity);

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

            var chk = await permission.HasPermission(256, PermissionType.Edit);

            if (chk == false)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {

                var items = await pmServiceManager.PmExtractTransactionService.Update(entity);

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<PMProjectsDto>.FailAsync($"======= Exp in  : {ex.Message}"));

            }

        }
        [HttpPost("CreateInvoice")]
        public async Task<IActionResult> CreateInvoice(PmExtractTransactionInvoiceCreateDto entity)
        {

            var chk = await permission.HasPermission(256, PermissionType.Edit);

            if (chk == false)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {

                var items = await pmServiceManager.PmExtractTransactionService.CreateInvoice(entity);

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<PMProjectsDto>.FailAsync($"======= Exp in  : {ex.Message}"));

            }

        }
        [HttpPost("CreateACC")]
        public async Task<IActionResult> CreateACC(PmExtractTransactionACCCreateDto entity)
        {

            var chk = await permission.HasPermission(72, PermissionType.Add);

            if (chk == false)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {

                var items = await pmServiceManager.PmExtractTransactionService.CreateACC(entity);

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
                var chk = await permission.HasPermission(294, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await pmServiceManager.PmExtractTransactionService.Remove(id, 31);
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
                var result = await pmServiceManager.PmExtractTransactionService.GetProjectExtractInfo(Code);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<PMProjectExtractInfoDto>.FailAsync($"====== Exp in Delete PmExtractTransactionController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetPMProjectsItemsByPMCodeItemCode")]
        public async Task<IActionResult> GetPMProjectsItemsByPMCodeItemCode(long ProjectCode, string ProjectItemCode)
        {
            try
            {
                if (ProjectCode == 0)
                    return Ok(await Result<PMExtractItemDto>.FailAsync($"اختر المشروع"));

                int PaymentTermsId = 1;
                PMExtractItemDto pMExtractItemDto = new PMExtractItemDto();
                var chk = await permission.HasPermission(294, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var projectsItemVMResult = await pmServiceManager.PMProjectsItemService.GetOneVW(x => x.ProjectCode == ProjectCode && x.ItemCode == ProjectItemCode && x.IsDeleted == false);
                var projectsItemVM = projectsItemVMResult.Data;
                var pmExtractTransactionsProductVMResult = await pmServiceManager.PmExtractTransactionsProductService.GetAllVW(x => x.IsDeletedM == false && x.PItemsId == projectsItemVM.Id && x.PaymentTermsId == PaymentTermsId);
                var pmExtractTransactionsProductVM = pmExtractTransactionsProductVMResult.Data;
                // ------- 
                pMExtractItemDto.AmountPrevious = pmExtractTransactionsProductVM.Sum(x => x.AmountRate);
                pMExtractItemDto.QtyPrevious = pmExtractTransactionsProductVM.Sum(x => x.Qty);
                pMExtractItemDto.RateAll = pmExtractTransactionsProductVM.Sum(x => x.Rate);

                var SumAmountResult = await pmServiceManager.PmExtractTransactionsProductService.GetAllVW
                        (x => x.IsDeletedM == false && x.ItemCode == projectsItemVM.ItemCode
                         && x.PItemsId != projectsItemVM.Id
                        && x.PaymentTermsId == PaymentTermsId);
                if (SumAmountResult.Data != null)
                {
                    pMExtractItemDto.SumAmount = SumAmountResult.Data.Sum(x => x.AmountRate);
                }
                pMExtractItemDto.Total = projectsItemVM.Total;
                pMExtractItemDto.Id = projectsItemVM.Id;
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
                return Ok(await Result<PMExtractItemDto>.FailAsync($"====== Exp in Delete PmExtractTransactionController, MESSAGE: {ex.Message}"));
            }
        }
      
           
        [HttpGet("GetByIdForEdit")]
        public async Task<IActionResult> GetByIdForEdit(long Id)
        {
            var chk = await permission.HasPermission(294, PermissionType.Show);
            var chk2 = await permission.HasPermission(294, PermissionType.Edit);

            if (chk == false && chk2 == false)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (Id <= 0)
            {
                return Ok(await Result<PMProjectsDto>.FailAsync($"not found this extract"));

            }
            try
            {

                var items = await pmServiceManager.PmExtractTransactionService.GetForEditById(Id);

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<PMProjectsDto>.FailAsync($"======= Exp in  : {ex.Message}"));

            }
        }

        #endregion ================================== End Extracts ===================================



        #region ================================= Draft Extracts =================================
        //ProjectsMangement/Extracts/Draft_Extract
        [HttpPost("DraftExtractSearch")]
        public async Task<IActionResult> DraftExtractSearch(PmExtractTransactionFilterDto filter)
        {
            try
            {
                var chk = await permission.HasPermission(1973, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                filter.PaymentTermsId = 6;
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
                            BraName = row["BRA_NAME"].ToString(),
                            Total = Convert.ToDecimal(row["Total"]),
                            Vat = Convert.ToDecimal(row["Vat"]),
                            DiscountAmount = Convert.ToDecimal(row["Discount_Amount"]),
                            Subtotal = Convert.ToDecimal(row["Subtotal"]),
                            CurrencyName = row["Currency_Name"].ToString(),
                            ExchangeRate = Convert.ToDecimal(row["Exchange_Rate"]),
                        });
                    }

                    return Ok(await Result<List<PmExtractTransactionVM>>.SuccessAsync(final));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"======= Exp in DraftExtractSearch PmExtractTransactionController : {ex.Message}"));
            }
        }

        #endregion ============================ End Draft Extracts ================================


        #region =============================== Follow Up Extracts ===============================
        //ProjectsMangement/Extracts/FollowUP_Extract
        [HttpPost("FollowUpExtracts")]
        public async Task<IActionResult> FollowUpExtracts(PmExtractTransactionFilterDto filter)
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
        #endregion ============================= End Follow Up Extracts ==============================

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


        [HttpGet("GetAllPMProjectsItemsByProjectCode")]
        public async Task<IActionResult> GetAllPMProjectsItemsByProjectCode(long projectCode)
        {
            try
            {
                if (projectCode == 0)
                    return Ok(await Result<IEnumerable<PMExtractItemDto>>.FailAsync("اختر المشروع"));

                int paymentTermsId = 1;
                var hasPermission = await permission.HasPermission(294, PermissionType.Add);
                if (!hasPermission)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                // Fetch all project items
                var projectsItemsResult = await pmServiceManager.PMProjectsItemService.GetAllVW(
                    x => x.ProjectCode == projectCode && x.IsDeleted==false);

                var projectsItems = projectsItemsResult.Data;

                if (projectsItems == null || !projectsItems.Any())
                    return Ok(await Result<IEnumerable<PMExtractItemDto>>.FailAsync("No project items found."));

                var pmExtractItemsDtos = new List<PMExtractItemDto>();

                foreach (var item in projectsItems)
                {
                    var pmExtractTransactionsProductVMResult = await pmServiceManager.PmExtractTransactionsProductService.GetAllVW(
                        x => x.IsDeletedM==false && x.PItemsId == item.Id && x.PaymentTermsId == paymentTermsId);

                    var pmExtractTransactionsProductVM = pmExtractTransactionsProductVMResult.Data;

                    var pMExtractItemDto = new PMExtractItemDto
                    {
                        AmountPrevious = pmExtractTransactionsProductVM.Sum(x => x.AmountRate),
                        QtyPrevious = pmExtractTransactionsProductVM.Sum(x => x.Qty),
                        RateAll = pmExtractTransactionsProductVM.Sum(x => x.Rate),
                        Total = item.Total,
                        Id = item.Id,
                        ItemName = item.ItemName,
                        ItemId = item.ItemId,
                        ProjectId = item.ProjectId,
                        UnitId = item.UnitId,
                        UnitName = item.UnitName,
                        Qty = item.Qty,
                        Price = item.Price,
                        ItemCode = item.ItemCode,
                        Note = item.Note,
                        QtyApprove = item.Qty,
                        PriceApprove = item.Total
                    };

                    // Calculate the sum of amounts for items with the same item code but different IDs
                    var sumAmountResult = await pmServiceManager.PmExtractTransactionsProductService.GetAllVW(
                        x => x.IsDeletedM==false && x.ItemCode == item.ItemCode && x.PItemsId != item.Id && x.PaymentTermsId == paymentTermsId);

                    if (sumAmountResult.Data != null)
                    {
                        pMExtractItemDto.SumAmount = sumAmountResult.Data.Sum(x => x.AmountRate);
                    }

                    pmExtractItemsDtos.Add(pMExtractItemDto);
                }

                return Ok(Result<IEnumerable<PMExtractItemDto>>.Success(pmExtractItemsDtos));
            }
            catch (Exception ex)
            {
                return Ok(await Result<IEnumerable<PMExtractItemDto>>.FailAsync($"====== Exp in GetAllPMProjectsItemsByProjectCode, MESSAGE: {ex.Message}"));
            }
        }
    }


}
