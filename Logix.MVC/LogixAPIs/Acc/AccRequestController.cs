using Logix.Application.Common;
using Logix.Application.DTOs.ACC;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.Acc
{
    public class AccRequestController : BaseAccApiController
    {

        private readonly IAccServiceManager accServiceManager;
        private readonly IMainServiceManager mainServiceManager;
        private readonly IPermissionHelper permission;
        private readonly IWebHostEnvironment env;
        private readonly IFilesHelper filesHelper;
        private readonly IDDListHelper listHelper;
        private readonly ILocalizationService localization;
        private readonly ISysConfigurationHelper configurationHelper;
        private readonly ICurrentData _session;

        public AccRequestController(
            IAccServiceManager accServiceManager,
            IMainServiceManager mainServiceManager,
            IPermissionHelper permission,
             IWebHostEnvironment env,
            IHttpContextAccessor httpContextAccessor,
            IFilesHelper filesHelper,
            IDDListHelper listHelper,
             ILocalizationService localization
             , ISysConfigurationHelper configurationHelper
            , ICurrentData session
            )
        {
            this.accServiceManager = accServiceManager;
            this.mainServiceManager = mainServiceManager;
            this.permission = permission;
            this.env = env;
            this.filesHelper = filesHelper;
            this.listHelper = listHelper;
            this.localization = localization;
            this.configurationHelper = configurationHelper;
            this._session = session;
        }
        #region "transactions"





        [HttpPost("Search")]
        public async Task<IActionResult> Search(AccRequestFilterDto filter)
        {
            var chk = await permission.HasPermission(641, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {

                var items = await accServiceManager.AccRequestService.Search(filter);
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccRequestVw>.FailAsync($"======= Exp in Search RequestVw, MESSAGE: {ex.Message}"));
            }
        }


        #endregion "transactions"

        #region "transactions_ADD"
        [HttpPost("Add")]
        public async Task<IActionResult> Add(AccRequestDto obj)
        {
            var chk = await permission.HasPermission(641, PermissionType.Add);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<AccRequestDto>.FailAsync(localization.GetMessagesResource("dataRequire")));
                }


                var add = await accServiceManager.AccRequestService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccRequestDto>.FailAsync($"======= Exp in Acc Request  add: {ex.Message}"));
            }
        }

        #endregion "transactions_Add"

        #region "transactions_Update"
        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(AccRequestEditDto obj)
        {
            var chk = await permission.HasPermission(641, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<AccRequestEditDto>.FailAsync(localization.GetMessagesResource("dataRequire")));
                }



                var Edit = await accServiceManager.AccRequestService.Update(obj);
                return Ok(Edit);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccRequestEditDto>.FailAsync($"======= Exp in Acc Request edit: {ex.Message}"));
            }
        }

        #endregion "transactions_Update"

        #region "transactions_Delete"
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long Id)
        {
            var chk = await permission.HasPermission(641, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var delete = await accServiceManager.AccRequestService.Remove(Id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccRequestPaymentDto>.FailAsync($"======= Exp in Acc Request  Delete: {ex.Message}"));
            }
        }
        #endregion "transactions_Delete"

        #region "transactions_GetById"

        [HttpGet("GetByIdForEdit")]
        public async Task<IActionResult> GetByIdForEdit(long id)
        {
            try
            {
                var chk = await permission.HasPermission(641, PermissionType.Show);
                if (!chk)
                {
                    return Ok(await Result.AccessDenied("AccessDenied"));
                }

                if (id <= 0)
                {
                    return Ok(await Result<AccRequestVw>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var getItem = await accServiceManager.AccRequestService.GetOneVW(s => s.Id == id && s.IsDeleted == false);

                if (getItem.Succeeded)
                {

                    var obj = getItem.Data;
                    var files = await mainServiceManager.SysFileService.GetAll(x => x.IsDeleted == false && x.TableId == 21 && x.PrimaryKey == id);
                    //obj.FileDtos = files.Data;
                    return Ok(await Result<AccRequestVw>.SuccessAsync(obj, $""));
                }
                else
                {
                    return Ok(getItem);
                }



            }
            catch (Exception ex)
            {
                return Ok(await Result<AccRequestVw>.FailAsync($"====== Exp in GetByIdForEdit Acc Request, MESSAGE: {ex.Message}"));
            }
        }
        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(long id)
        {
            try
            {
                var chk = await permission.HasPermission(641, PermissionType.Show);
                if (!chk)
                {
                    return Ok(await Result.AccessDenied("AcessDenied"));
                }

                if (id <= 0)
                {
                    return Ok(await Result<AccRequestDto>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var getItem = await accServiceManager.AccRequestService.GetOne(s => s.Id == id && s.IsDeleted == false);
                if (getItem.Succeeded)
                {
                    var obj = getItem.Data;
                    return Ok(await Result<AccRequestDto>.SuccessAsync(obj, $""));
                }
                else
                {
                    return Ok(getItem);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccRequestDto>.FailAsync($"====== Exp in GetById Acc Request, MESSAGE: {ex.Message}"));
            }
        }


        #endregion "transactions_GetById"

        #region "transactions_InformationBank"

        [HttpGet("GetAccType")]
        public async Task<IActionResult> GetAccType(int ReferenceTypeId)
        {
            var referenceTypeData = await accServiceManager.AccReferenceTypeService.GetOne(s => s.ParentId, x => x.FlagDelete == false && x.ReferenceTypeId == ReferenceTypeId);
            var referenceType = referenceTypeData.Data ?? 0;

            return Ok(await Result<int>.SuccessAsync(referenceType));
        }

        private async Task<int> GetACCReferenceType(int ReferenceTypeId)
        {
            var ReferenceTypedata = await accServiceManager.AccReferenceTypeService.GetOne(s => s.ParentId, x => x.FlagDelete == false && x.ReferenceTypeId == ReferenceTypeId);
            var ReferenceType = ReferenceTypedata.Data ?? 0;

            return ReferenceType;
        }

        [HttpGet("GetInformationBank")]
        public async Task<IActionResult> GetInformationBank(int accountType, string code)
        {
            try
            {
                accountType = await GetACCReferenceType(accountType);

                var obj = new InformationBankDto();

                if (accountType == 2 || accountType == 3)
                {
                    if (accountType == 3)
                    {
                        accountType = 1;
                    }

                    var InformationBank = await mainServiceManager.SysCustomerService.GetOne(x => x.IsDeleted == false && x.Code == code && x.CusTypeId == accountType);

                    if (InformationBank.Succeeded)
                    {
                        obj = new InformationBankDto()
                        {
                            Id = InformationBank.Data.Id,
                            BankId = InformationBank.Data.BankId,
                            BankAccount = InformationBank.Data.BankAccount,
                            IdNo = InformationBank.Data.IdNo,
                            Name = InformationBank.Data.Name,
                            Name2 = InformationBank.Data.Name2,
                        };

                        return Ok(await Result<InformationBankDto>.SuccessAsync(obj, ""));
                    }
                }

                if (accountType == 8)
                {
                    var InformationBank = await mainServiceManager.InvestEmployeeService.GetOne(x => x.IsDeleted == false && x.EmpId == code);

                    if (InformationBank.Succeeded)
                    {
                        obj = new InformationBankDto()
                        {
                            Id = InformationBank.Data.Id,
                            BankId = InformationBank.Data.BankId,
                            BankAccount = InformationBank.Data.AccountNo,
                            IdNo = InformationBank.Data.IdNo,
                            Name = InformationBank.Data.EmpName,
                            Name2 = InformationBank.Data.EmpName2,
                        };

                        return Ok(await Result<InformationBankDto>.SuccessAsync(obj, ""));
                    }
                }

                return Ok(obj);
            }
            catch (Exception ex)
            {
                return Ok(await Result<InformationBankDto>.FailAsync($"====== Exp in Information Bank  Acc Request, MESSAGE: {ex.Message}"));

            }

        }

        #endregion "transactions_InformationBank"

        [HttpGet("GetRequestByAppCode")]
        public async Task<IActionResult> GetRequestByAppCode(long AppCode)
        {
            try
            {
                var obj = new AccRequestPopDto();
                string Code = "";
                string Name = "";
                var LORequest = await accServiceManager.AccRequestService.GetOneVW(x =>
                    x.IsDeleted == false &&
                    x.FacilityId == _session.FacilityId &&
                    x.TransTypeId == 1 &&
                    x.StatusId == 1 &&
                    (x.AppCode != null && x.AppCode.Equals(AppCode))

                );

                if (LORequest.Succeeded && LORequest.Data != null)
                {

                    var AccReque = await accServiceManager.AccRequestService.GetAll(a => a.RefranceId == LORequest.Data.Id && a.TransTypeId == 2);
                    if (AccReque.Succeeded && AccReque.Data != null && AccReque.Data.Count() == 0)
                    {
                        if (LORequest.Data.ReferenceTypeId == 1)
                        {
                            Code = LORequest.Data.AccountCode;
                            Name = LORequest.Data.AccountName;
                        }
                        else
                        {
                            Code = LORequest.Data.Code;
                            Name = LORequest.Data.Name;
                        }
                        obj = new AccRequestPopDto
                        {
                            AppCode = LORequest.Data.AppCode,
                            AppDate = LORequest.Data.AppDate,
                            //DepName = LORequest.Data.DepName,
                            Amount = LORequest.Data.Amount,
                            TypeId = LORequest.Data.TypeId,
                            DepId = LORequest.Data.DepId,
                            Status2Id = LORequest.Data.Status2Id,
                            Amountwrite = LORequest.Data.Amountwrite,
                            ReferenceTypeId = LORequest.Data.ReferenceTypeId,
                            AccAccountCode = Code,
                            AccAccountName = Name,
                            CostCenterCode = LORequest.Data.CostCenterCode,
                            CostCenterName = LORequest.Data.CostCenterName,
                            BranchId = LORequest.Data.BranchId,
                            IdNo = LORequest.Data.IdNo,
                            Iban = LORequest.Data.Iban,
                            CustomerName = LORequest.Data.CustomerName,
                            CustomerCont = LORequest.Data.CustomerCont,
                            BankId = LORequest.Data.BankId,
                            AppId = LORequest.Data.Id,
                            CcId = LORequest.Data.CcId,

                            Description = LORequest.Data.Description,
                        };
                        return Ok(await Result<AccRequestPopDto>.SuccessAsync(obj, $""));

                    }
                    return Ok(obj);

                }

                return Ok(obj);

            }
            catch (Exception exp)
            {
                return Ok(await Result<AccRequestPopDto>.FailAsync($"Exception Message: {exp.Message}"));
            }
        }


        [HttpGet("SelectACCJournalMaster")]
        public async Task<IActionResult> SelectACCJournalMaster(long JID)
        {
            try
            {

                var RequestJID = await accServiceManager.AccJournalMasterService.GetAll(x =>

                    x.FacilityId == _session.FacilityId &&
                    x.JId == JID
                );

                if (RequestJID.Succeeded)
                {
                    return Ok(await Result<AccJournalMasterDto>.SuccessAsync(RequestJID.Data.FirstOrDefault(), $""));
                }
                else
                {
                    return Ok(await Result<AccJournalMasterDto>.FailAsync($"لا تتوفر بيانات"));
                }


            }
            catch (Exception exp)
            {
                return Ok(await Result<AccJournalMasterDto>.FailAsync($"Exception Message: {exp.Message}"));
            }
        }
        [HttpGet("GetAccRequestByRefranceID")]
        public async Task<IActionResult> GetAccRequestByRefranceID(long id)
        {
            try
            {


                if (id <= 0)
                {
                    return Ok(await Result<AccRequestVw>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var getItem = await accServiceManager.AccRequestService.GetAllVW(x => x.RefranceId == id && x.TransTypeId == 2 && x.IsDeleted == false);
                if (getItem.Succeeded)
                {



                    return Ok(await Result<AccRequestVw>.SuccessAsync(getItem.Data.FirstOrDefault(), $""));
                }
                else
                {
                    return Ok(getItem);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccRequestVw>.FailAsync($"====== Exp in Get AccRequest By RefranceID Acc Request, MESSAGE: {ex.Message}"));
            }
        }




    }
}

