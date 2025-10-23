using Logix.Application.Common;
using Logix.Application.DTOs.FXA;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Services.HR;
using Logix.Application.Wrapper;
using Logix.Domain.FXA;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;

namespace Logix.MVC.LogixAPIs.FXA
{
    public class FxaTransactionsPaymentController : BaseFxaApiController
    {
        private readonly IFxaServiceManager fxaServiceManager;
        private readonly IAccServiceManager accServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IApiDDLHelper ddlHelper;
        private readonly IWebHostEnvironment env;

        public FxaTransactionsPaymentController(IFxaServiceManager fxaServiceManager,
            IAccServiceManager accServiceManager,
            IPermissionHelper permission,
            ICurrentData session,
            ILocalizationService localization,
            IApiDDLHelper ddlHelper,
            IWebHostEnvironment env)
        {
            this.fxaServiceManager = fxaServiceManager;
            this.accServiceManager = accServiceManager;
            this.permission = permission;
            this.session = session;
            this.localization = localization;
            this.ddlHelper = ddlHelper;
            this.env = env;
        }

		[HttpPost("Search")]
		public async Task<ActionResult> Search(FxaTransactionsPaymentFilterDto filter)
		{
			try
			{
				var chk = await permission.HasPermission(1986, PermissionType.Show);
				if (!chk)
					return Ok(await Result.AccessDenied("AccessDenied"));

				filter.Id ??= 0;
				filter.No ??= 0;

                var items = await fxaServiceManager.FxaTransactionsPaymentService.GetAll(a => a.IsDeleted == false
                    //&& a.FacilityId == session.FacilityId
                    && (filter.Id == 0 || (a.Id == filter.Id))
                    && (filter.No == 0 || (a.No == filter.No))
                    && (string.IsNullOrEmpty(filter.Code) || (!string.IsNullOrEmpty(a.Code) && a.Code.Equals(filter.Code)))
                    && (filter.BranchId == 0 || a.BranchId == filter.BranchId)
                    && (filter.BranchId == 0 || session.BranchId == filter.BranchId)
                    && (filter.TransactionId == null || a.TransactionId == filter.TransactionId)
                    && (filter.BankId == null || a.BankId == filter.BankId));

				if (!items.Succeeded)
					return Ok(items);

				var res = items.Data.OrderBy(r => r.Id).AsQueryable();

				if (!string.IsNullOrEmpty(filter.PaymentDate) && !string.IsNullOrEmpty(filter.PaymentDate2))
				{
					DateTime startDate = DateTime.ParseExact(filter.PaymentDate ?? "", "yyyy/MM/dd", CultureInfo.InvariantCulture);
					DateTime endDate = DateTime.ParseExact(filter.PaymentDate2 ?? "", "yyyy/MM/dd", CultureInfo.InvariantCulture);

					res = res.Where(r => !string.IsNullOrEmpty(r.PaymentDate)
						&& DateTime.ParseExact(r.PaymentDate, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= startDate
						&& DateTime.ParseExact(r.PaymentDate, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= endDate);
				}

				var final = res.Select(item => new FxaTransactionsPaymentFilterDto()
				{
					Id = item.Id,
					No = item.No,
					Code = item.Code,
					BranchId = item.BranchId,
					TransactionId = item.TransactionId,
					PaymentDate = item.PaymentDate,
					PaymentDate2 = item.PaymentDate2,
					AmountReceived = item.AmountReceived,
					//PaymentMethodId = item.PaymentMethodId,
					BankId = item.BankId,
					//BankReference = item.BankReference,
					//DaysLate = item.DaysLate,
					//IsDeleted = item.IsDeleted,
					//AccountId = item.AccountId,
					//Jid = item.Jid,
					//CurrencyId = item.CurrencyId,
					//ExchangeRate = item.ExchangeRate
				}).ToList();

				return Ok(await Result<List<FxaTransactionsPaymentFilterDto>>.SuccessAsync(final));
			}
			catch (Exception ex)
			{
				return Ok(await Result.FailAsync($"Error: {ex.Message}"));
			}
		}

		[HttpPost("Add")]
		public async Task<ActionResult> Add(FxaTransactionsPaymentDto model)
		{
			try
			{
				var chk = await permission.HasPermission(1986, PermissionType.Add);
				if (!chk)
					return Ok(await Result.AccessDenied("AccessDenied"));

				// التحقق من أن الفترة المحاسبية غير مغلقة
				var periodCheck = await accServiceManager.AccPeriodsService.CheckDateInPeriod(session.FacilityId, model.PaymentDate);
				if (periodCheck)
					return Ok(await Result.FailAsync("الفترة المحاسبية مغلقة"));

				var entity = new FxaTransactionsPaymentDto
				{
					No = model.No ?? 0,
					Code = model.Code,
					BranchId = model.BranchId,
					TransactionId = model.TransactionId,
					PaymentDate = model.PaymentDate,
					PaymentDate2 = model.PaymentDate2,
					AmountReceived = model.AmountReceived ?? 0,
					PaymentMethodId = model.PaymentMethodId,
					BankId = model.BankId,
					BankReference = model.BankReference,
					DaysLate = model.DaysLate ?? 0,
					IsDeleted = false,
					AccountId = model.AccountId,
					Jid = model.Jid,
					CurrencyId = model.CurrencyId,
					ExchangeRate = model.ExchangeRate ?? 1,
				};

				var result = await fxaServiceManager.FxaTransactionsPaymentService.Add(entity);
				return Ok(result);
			}
			catch (Exception ex)
			{
				return Ok(await Result.FailAsync($"Error: {ex.Message}"));
			}
		}

		[HttpPost("Update")]
		public async Task<ActionResult> Update(FxaTransactionsPaymentEditDto model)
		{
			try
			{
				var chk = await permission.HasPermission(1986, PermissionType.Edit);
				if (!chk)
					return Ok(await Result.AccessDenied("AccessDenied"));

				var existing = await fxaServiceManager.FxaTransactionsPaymentService.Update(model);
				//if (!existing.Succeeded)
				//	return Ok(existing);

				//existing.Data.No = model.No ?? existing.Data.No;
				//existing.Data.Code = model.Code ?? existing.Data.Code;
				//existing.Data.BranchId = model.BranchId ?? existing.Data.BranchId;
				//existing.Data.TransactionId = model.TransactionId ?? existing.Data.TransactionId;
				//existing.Data.PaymentDate = model.PaymentDate ?? existing.Data.PaymentDate;
				//existing.Data.PaymentDate2 = model.PaymentDate2 ?? existing.Data.PaymentDate2;
				//existing.Data.AmountReceived = model.AmountReceived ?? existing.Data.AmountReceived;
				//existing.Data.PaymentMethodId = model.PaymentMethodId ?? existing.Data.PaymentMethodId;
				//existing.Data.BankId = model.BankId ?? existing.Data.BankId;
				//existing.Data.BankReference = model.BankReference ?? existing.Data.BankReference;
				//existing.Data.DaysLate = model.DaysLate ?? existing.Data.DaysLate;
				//existing.Data.AccountId = model.AccountId ?? existing.Data.AccountId;
				//existing.Data.Jid = model.Jid ?? existing.Data.Jid;
				//existing.Data.CurrencyId = model.CurrencyId ?? existing.Data.CurrencyId;
				//existing.Data.ExchangeRate = model.ExchangeRate ?? existing.Data.ExchangeRate;

				//var result = await fxaServiceManager.FxaTransactionsPaymentService.Update(existing.Data);
				return Ok(existing);
			}
			catch (Exception ex)
			{
				return Ok(await Result.FailAsync($"Error: {ex.Message}"));
			}
		}

		


		//[HttpPost("Add")]
  //      public async Task<ActionResult> Add(FxaTransactionsPaymentDto obj)
  //      {
  //          try
  //          {
  //              var chk = await permission.HasPermission(1986, PermissionType.Add);
  //              if (!chk)
  //                  return Ok(await Result.AccessDenied("AccessDenied"));

  //              if (!ModelState.IsValid)
  //                  return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

  //              var add = await fxaServiceManager.FxaTransactionsPaymentService.Add(obj);
  //              //if (add.Succeeded)
  //              //{
  //              //    //Create QR 
  //              //    var facility = await accServiceManager.AccFacilityService.GetById(session.FacilityId);
  //              //    if (facility.Succeeded)
  //              //    {
  //              //        var invPath = Path.Join(env.WebRootPath, FilesPath.SaleQrPath);
  //              //        var qrResInvoice = QRHelper.GenerateQRforZATCA(invPath, add.Data.Id ?? 0, facility.Data.FacilityName ?? "", facility.Data.VatNumber ?? "", add.Data.Amount ?? 0, 0, add.Data.VatAmount ?? 0, add.Data.Date1 ?? "", (add.Data.Id ?? 0).ToString());
  //              //    }
  //              //}
  //              return Ok(add);
  //          }
  //          catch (Exception ex)
  //          {
  //              return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
  //          }
  //      }

        [HttpGet("DDLFxaAddExcType")]
        public async Task<IActionResult> DDLFxaAddExcType(int TypeId)
        {
            try
            {
                var list = new SelectList(new List<DDListItem<FxaAdditionsExclusionTypeDto>>());
                list = await ddlHelper.GetAnyLis<FxaAdditionsExclusionType, long>(t => t.IsDeleted == false && t.TypeId == TypeId, "Id", "Name");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }


        [HttpGet("GetByIdForEdit")]
        public async Task<IActionResult> GetByIdForEdit(long id)
        {
            try
            {
                var chk = await permission.HasPermission(1986, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var getItem = await fxaServiceManager.FxaTransactionsPaymentService.GetOneVW(x => x.Id == id
                    && x.IsDeleted == false);

                //if (getItem.Succeeded)
                //{
                //    var res = getItem.Data;
                //    FxaTransactionsPaymentEditDto obj = new()
                //    {
                //        Id = res.Id,
                //        CrdAccountCode = res.AccountTypeId == 1 ? res.CrdAccountCode1 : res.CrdAccountCode,
                //        AccountCode = res.AccountCode,
                //        AccountTypeId = res.AccountTypeId,
                //        Date1 = res.Date1,
                //        FxNo = res.FixedAssetNo,
                //        AffectAge = res.AffectAge ?? false,
                //        AffectAgeDate = res.EndDate,
                //        AffectPriceAsset = res.AffectPriceAsset ?? false,
                //        AssetPrice = res.AssetPrice,
                //        AffectInstallment = res.AffectInstallment ?? false,
                //        InstallmentValue = res.InstallmentValue,
                //        TypeId = res.TypeId,
                //        Description = res.Description,
                //        VatRate = res.VatRate,
                //        VatAmount = res.VatAmount,
                //        Amount = res.AdditionsExclusionTypeId == 1 ? res.Credit : res.Debit,
                //        OperationType = res.AdditionsExclusionTypeId ?? 0
                //    };

                //    //get journal data
                //    int docTypeId = res.AdditionsExclusionTypeId == 1 ? 103 : 104;
                //    var getJournal = await accServiceManager.AccJournalMasterService.GetOne(j => j.ReferenceNo == id && j.DocTypeId == docTypeId && j.FlagDelete == false);
                //    if (getJournal.Succeeded)
                //    {
                //        obj.JId = getJournal.Data.JId;
                //        obj.JCode = getJournal.Data.JCode;
                //        obj.PeriodId = getJournal.Data.PeriodId;
                //    }

                //    return Ok(await Result<FxaTransactionsPaymentEditDto>.SuccessAsync(obj));
                //}
                return Ok(getItem);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(FxaTransactionsPaymentEditDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(1986, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var update = await fxaServiceManager.FxaTransactionsPaymentService.Update(obj);
                //if (update.Succeeded)
                //{
                //    //Create QR 
                //    var facility = await accServiceManager.AccFacilityService.GetById(session.FacilityId);
                //    if (facility.Succeeded)
                //    {
                //        var invPath = Path.Join(env.WebRootPath, FilesPath.SaleQrPath);
                //        var qrResInvoice = QRHelper.GenerateQRforZATCA(invPath,obj.Id ?? 0, facility.Data.FacilityName ?? "", facility.Data.VatNumber ?? "", obj.Amount ?? 0, 0, obj.VatAmount ?? 0, obj.Date1 ?? "", (obj.Id ?? 0).ToString());
                //    }
                //}
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }


        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var chk = await permission.HasPermission(1986, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await fxaServiceManager.FxaTransactionsPaymentService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp Message: {ex.Message}"));
            }
        }
    }
}