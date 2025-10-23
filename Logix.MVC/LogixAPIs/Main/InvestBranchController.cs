using Logix.Application.Common;
using Logix.Application.DTOs.ACC;
using Logix.Application.DTOs.Main;
using Logix.Application.DTOs.WA;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.Main
{
    public class InvestBranchController : BaseMainApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IAccServiceManager accServiceManager;
        private readonly IPermissionHelper permission;
        private readonly IWaServiceManager waServiceManager;
        private readonly ILocalizationService localization;

        public InvestBranchController(IMainServiceManager mainServiceManager,
            IAccServiceManager accServiceManager,
            IPermissionHelper permission,
            IWaServiceManager waServiceManager,
            ILocalizationService localization)
        {
            this.mainServiceManager = mainServiceManager;
            this.accServiceManager = accServiceManager;
            this.permission = permission;
            this.waServiceManager = waServiceManager;
            this.localization = localization;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult> GetAll()
        {
            try
            {
                var chk = await permission.HasPermission(1, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var items = await mainServiceManager.InvestBranchService.GetAll(c => c.Isdel == false);
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in GetAlll InvestBranchController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Search")]
        public async Task<ActionResult> Search(/*DataTableRequestModel request*/ InvestBranchFilterDto filter)
        {
            try
            {
                var chk = await permission.HasPermission(1, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                //var items = await mainServiceManager.InvestBranchService.GetAllVW();
                //if (items.Succeeded)
                //{
                //    var query = items.Data.Where(b => b.Isdel == false).OrderBy(s => s.BranchId).AsQueryable();

                //    var totalRecords = query.Count();

                //    // Apply pagination
                //    var filteredRecords = query.Skip(request.start).Take(request.length).ToList();

                //    // Prepare the response
                //    var response = new DataTableResponseModel<InvestBranchVw>
                //    {
                //        draw = request.draw,
                //        recordsTotal = totalRecords,
                //        recordsFiltered = totalRecords,
                //        data = filteredRecords
                //    };

                //    return Ok(await Result<DataTableResponseModel<InvestBranchVw>>.SuccessAsync(response, ""));
                //}

                filter.FacilityId ??= 0;
                var items = await mainServiceManager.InvestBranchService.GetAll(c => c.Isdel == false
                && (filter.FacilityId == 0 || c.FacilityId == filter.FacilityId)
                && (string.IsNullOrEmpty(filter.BraName) || c.BraName!.Contains(filter.BraName) || c.BraName2!.ToLower().Contains(filter.BraName.ToLower())));
                
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Search InvestBranchController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Add")]
        public async Task<ActionResult> Add(InvestBranchDto obj)
        {
            var chk = await permission.HasPermission(1, PermissionType.Add);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (!ModelState.IsValid)
            {
                return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));
            }

            if (obj.Auto == false && string.IsNullOrEmpty(obj.BranchCode))
            {
                return Ok(await Result.FailAsync($"{localization.GetMainResource("BranchCode")}"));
            }
            try
            {
                if (!string.IsNullOrEmpty(obj.CostCenterCode))
                {
                    var getCostCenter = await accServiceManager.AccCostCenterService.GetAll(a => a.CostCenterCode == obj.CostCenterCode && a.IsDeleted == false);
                    if (getCostCenter.Succeeded && getCostCenter.Data.Any())
                    {
                        obj.CcId = (int)getCostCenter.Data.Single().CcId;
                    }
                    else
                    {
                        return Ok(await Result.FailAsync($"{localization.GetResource1("CostCenterNotExsists")}"));
                    }
                }

                var add = await mainServiceManager.InvestBranchService.Add(obj);
                if (add.Succeeded)
                {
                    var waResult = await waServiceManager.WhatsappBusinessService.SendWhatsappMessage(
                        new WhatsappBusinessDataSendDto
                        {
                            DocumentUrl = "https://www.pdf995.com/samples/pdf.pdf",
                            RecipientPhoneNumber = "967775699645",
                            DataMessage = add.Data,
                            HasDocument = true,
                            //WaTemplateMessageValue = add.Data,
                        }, 1);
                }

                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add InvestBranch, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var chk = await permission.HasPermission(1, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var obj = new InvestBranchDto();
                var getBranch = await mainServiceManager.InvestBranchService.GetOne(b => b.BranchId == id && b.Isdel == false);
                if (getBranch.Succeeded)
                {
                    obj = getBranch.Data;
                    if (obj.CcId != 0)
                    {
                        //get costCenterCode by costCenterId to display it in view
                        var getCostCenter = await accServiceManager.AccCostCenterService.GetAll(a => a.CcId == obj.CcId);
                        if (getCostCenter.Succeeded && getCostCenter.Data != null)
                        {
                            obj.CostCenterCode = getCostCenter.Data.Single().CostCenterCode;
                            obj.CostCenterName = getCostCenter.Data.Single().CostCenterName;
                        }
                    }

                    return Ok(await Result<InvestBranchDto>.SuccessAsync(obj));
                }
                else
                {
                    return Ok(getBranch);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in GetById InvestBranch, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetByIdForEdit")]
        public async Task<IActionResult> GetByIdForEdit(int id)
        {
            try
            {
                var chk = await permission.HasPermission(1, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var obj = new InvestBranchDto();
                var getBranch = await mainServiceManager.InvestBranchService.GetForUpdate<InvestBranchDto>(id);
                if (getBranch.Succeeded)
                {
                    obj = getBranch.Data;
                    if (obj.CcId != 0)
                    {
                        //get costCenterCode by costCenterId to display it in view
                        var getCostCenter = await accServiceManager.AccCostCenterService.GetAll(a => a.CcId == obj.CcId);
                        if (getCostCenter.Succeeded && getCostCenter.Data != null)
                        {
                            obj.CostCenterCode = getCostCenter.Data.Single().CostCenterCode;
                            obj.CostCenterName = getCostCenter.Data.Single().CostCenterName;
                        }
                    }

                    return Ok(await Result<InvestBranchDto>.SuccessAsync(obj));
                }
                else
                {
                    return Ok(getBranch);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in GetByIdForEdit InvestBranch, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(InvestBranchDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(1, PermissionType.Edit);
                if (!chk)
                {
                    return Ok(await Result.AccessDenied("AccessDenied"));
                }
                if (!ModelState.IsValid)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));
                }

                if (!string.IsNullOrEmpty(obj.CostCenterCode))
                {
                    var getCostCenter = await accServiceManager.AccCostCenterService.GetAll(a => a.CostCenterCode == obj.CostCenterCode);
                    if (getCostCenter.Succeeded && getCostCenter.Data.Any())
                    {
                        obj.CcId = (int)getCostCenter.Data.Single().CcId;
                    }
                    else
                    {
                        return Ok(await Result.FailAsync($"{localization.GetResource1("CostCenterNotExsists")}"));
                    }
                }
                else
                {
                    obj.CcId = 0;
                }

                var update = await mainServiceManager.InvestBranchService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Edit InvestBranch, MESSAGE: {ex.Message}"));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var chk = await permission.HasPermission(1, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));
                }

                var delete = await mainServiceManager.InvestBranchService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete InvestBranch, MESSAGE: {ex.Message}"));
            }
        }

        //ربط الحسابات بالفروع

        [HttpGet("GetBranchAccount")]
        public async Task<ActionResult> BranchAccount(int branchId)
        {
            try
            {
                var chk = await permission.HasPermission(1, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (branchId <= 0)
                {
                    return Ok(await Result<List<AccBranchAccountsVwsDto>>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }
                var branch = await mainServiceManager.InvestBranchService.GetById(branchId);
                if (!branch.Succeeded)
                {
                    return Ok(await Result<List<AccBranchAccountsVwsDto>>.FailAsync($"{branch.Status.message}"));
                }

                //get all Acc_Branch_Account_Type
                var AccBranchAccTypes = await accServiceManager.AccBranchAccountTypeService.GetAll(t => t.IsDeleted == false);

                //get list from Acc_Branch_Account_Vw where Br_Acc_Type_ID = id of Acc_Branch_Account_Type
                var AllDataFromView = await accServiceManager.AccBranchAccountService.GetAllVW();
                List<AccBranchAccountsVwsDto> BranchAccountList = new List<AccBranchAccountsVwsDto>();
                foreach (var item in AccBranchAccTypes.Data)
                {
                    var singleItem = AllDataFromView.Data.Where(s => s.BranchId == branchId && s.BrAccTypeId == item.Id).ToList();
                    if (singleItem.Count() == 0)
                    {
                        //that means this branch has no account with type of item.Id
                        //we save some initial data, to show all types of account but with empty AccAccountCode 
                        AccBranchAccountsVwsDto record = new AccBranchAccountsVwsDto();
                        record.BranchId = branchId;
                        record.BrAccTypeId = item.Id;
                        record.Name = item.Name;
                        record.Name2 = item.Name2;
                        singleItem.Add(record);
                    }
                    BranchAccountList.AddRange(singleItem);
                }
                //return view with this list of AccVW
                //return View(BranchAccountList);
                return Ok(await Result<List<AccBranchAccountsVwsDto>>.SuccessAsync(BranchAccountList));
            }
            catch (Exception ex)
            {
                return Ok(await Result<List<AccBranchAccountsVwsDto>>.FailAsync($"====== Exp in Get Accounts of Branch, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("UpdateBranchAccount")]
        public async Task<ActionResult> BranchAccount(List<AccBranchAccountsVwsDto> list)
        {
            try
            {
                var update = await accServiceManager.AccBranchAccountService.Update(list);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result<List<AccBranchAccountsVwsDto>>.FailAsync($"====== Exp in Save Accounts of Branch, MESSAGE: {ex.Message}"));
            }
        }
    }
}
