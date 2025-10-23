using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace Logix.MVC.LogixAPIs.HR
{
    public class HRSalaryGroupApiController : BaseHrApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IAccServiceManager accServiceManager;
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly IApiDDLHelper ddlHelper;
        private readonly ILocalizationService localization;
        private readonly IDDListHelper listHelper;


        public HRSalaryGroupApiController(IHrServiceManager hrServiceManager, IMainServiceManager mainServiceManager, IPermissionHelper permission, ICurrentData session, IAccServiceManager accServiceManager, IApiDDLHelper ddlHelper, IDDListHelper listHelper, ILocalizationService localization)
        {
            this.mainServiceManager = mainServiceManager;
            this.permission = permission;
            this.session = session;
            this.accServiceManager = accServiceManager;
            this.hrServiceManager = hrServiceManager;
            this.ddlHelper = ddlHelper;
            this.listHelper = listHelper;
            this.localization = localization;
        }

        #region Index Page


        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var chk = await permission.HasPermission(409, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var items = await hrServiceManager.HrSalaryGroupService.GetAll(e => e.IsDeleted == false && e.FacilityId == session.FacilityId);
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    res = res.OrderBy(e => e.Id);
                    return Ok(await Result<List<HrSalaryGroupDto>>.SuccessAsync(res.ToList(), items.Status.message));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }



        [HttpPost("GetPagination")]
        public async Task<IActionResult> GetPagination([FromBody] HrSalaryGroupFilterDto filter, int take = Pagination.take, long? lastSeenId = null)
        {
            var chk = await permission.HasPermission(409, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var items = await hrServiceManager.HrSalaryGroupService.GetAllWithPaginationVW(
                    selector: e => e.Id,
                    expression: e => e.IsDeleted == false && e.FacilityId == session.FacilityId
                    && (string.IsNullOrEmpty(filter.Name) || e.Name == filter.Name)
                    ,
                    take: take,
                    lastSeenId: lastSeenId
                );

                if (!items.Succeeded)
                    return Ok(await Result<List<HrSalaryGroupDto>>.FailAsync(items.Status.message));

                if (items.Data == null || !items.Data.Any())
                    return Ok(await Result<List<HrSalaryGroupDto>>.SuccessAsync(new List<HrSalaryGroupDto>()));

                var res = items.Data.OrderBy(x => x.Id).ToList();

                var paginatedData = new PaginatedResult<object>
                {
                    Succeeded = items.Succeeded,
                    Data = res,
                    Status = items.Status,
                    PaginationInfo = items.PaginationInfo
                };

                return Ok(paginatedData);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }


        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long Id = 0)
        {
            var chk = await permission.HasPermission(409, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (Id <= 0)
                return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));


            try
            {
                var del = await hrServiceManager.HrSalaryGroupService.Remove(Id);
                return Ok(del);

            }
            catch (Exception exp)
            {
                return Ok(await Result.FailAsync($"{exp.Message}"));
            }
        }


        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long Id)
        {
            var chk = await permission.HasPermission(409, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result<HrSalaryGroupEditDto>.FailAsync($"Access Denied"));
            }

            if (Id <= 0)
                return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));


            try
            {
                var result = new HrSalaryGroupEditDto();
                var Item = await hrServiceManager.HrSalaryGroupService.GetOneVW(x => x.IsDeleted == false && x.Id == Id);
                if (!Item.Succeeded)
                    return Ok(await Result<object>.FailAsync(Item.Status.message));

                if (Item.Data == null)
                    return Ok(await Result<HrSalaryGroupEditDto>.FailAsync(localization.GetMessagesResource("NoItemFoundToEdit")));
                result.Id = Item.Data.Id;
                result.Name = Item.Data.Name;
                result.AccountAllowancesCode = Item.Data.AccountAllowancesCode;
                result.AccountDeductionCode = Item.Data.AccountDeductionCode;
                result.AccountOverTimeCode = Item.Data.AccountOverTimeCode;
                result.AccountSalaryCode = Item.Data.AccountSalaryCode;
                result.AccountDueSalaryCode = Item.Data.AccountDueSalaryCode;
                result.AccountLoanCode = Item.Data.AccountLoanCode;
                result.AccountOhadCode = Item.Data.AccountOhadCode;
                result.AccountTicketsCode = Item.Data.AccountTicketsCode;
                result.AccountVacationSalaryCode = Item.Data.AccountVacationSalaryCode;
                result.AccountEndServiceCode = Item.Data.AccountEndServiceCode;
                result.AccountDueEndServiceCode = Item.Data.AccountDueEndServiceCode;
                result.AccountDueVacationCode = Item.Data.AccountDueVacationCode;
                result.AccountDueTicketsCode = Item.Data.AccountDueTicketsCode;
                result.AccountDueGosiCode = Item.Data.AccountDueGosiCode;
                result.AccountGosiCode = Item.Data.AccountGosiCode;
                result.FacilityId = Item.Data.FacilityId;
                result.AccountMandateCode = Item.Data.AccountMandateCode;
                result.AccountDueMandateCode = Item.Data.AccountDueMandateCode;
                result.AccountCommissionCode = Item.Data.AccountCommissionCode;
                result.AccountDueCommissionCode = Item.Data.AccountDueCommissionCode;
                result.AccountMedicalInsuranceCode = Item.Data.AccountMedicalInsuranceCode;
                result.AccountPrepaidExpensesCode = Item.Data.AccountPrepaidExpensesCode;
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////
                var AllDeductions = await hrServiceManager.HrSalaryGroupDeductionVwService.GetAllVW(e => e.IsDeleted == false && e.GroupId == Id && e.TypeId == 2);
                result.Deductions = AllDeductions.Data.ToList();
                var AllAllowances = await hrServiceManager.HrSalaryGroupAllowanceVwService.GetAllVW(e => e.IsDeleted == false && e.GroupId == Id && e.TypeId == 1);
                result.Allowances = AllAllowances.Data.ToList();
                return Ok(await Result<HrSalaryGroupEditDto>.SuccessAsync(result, ""));

            }
            catch (Exception exp)
            {
                return Ok(await Result<HrSalaryGroupEditDto>.FailAsync($"EXP in {this.GetType()}, Meesage: {exp.Message}"));
            }
        }

        #endregion


        #region Add  Page

        [HttpPost("Add")]
        public async Task<IActionResult> Add(HrSalaryGroupDto obj)
        {
            var chk = await permission.HasPermission(409, PermissionType.Add);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                if (string.IsNullOrEmpty(obj.Name))
                    return Ok(await Result<object>.FailAsync($"{localization.GetHrResource("GroupName")}"));


                if (obj.FacilityId <= 0)
                    return Ok(await Result<object>.FailAsync($"{localization.GetHrResource("Company")}"));

                var addRes = await hrServiceManager.HrSalaryGroupService.Add(obj);
                return Ok(addRes);

            }

            catch (Exception ex)
            {
                return Ok(await Result<HrSalaryGroupDto>.FailAsync(ex.Message));
            }
        }



        #endregion


        #region Edit  Page

        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(HrSalaryGroupEditDto obj)
        {
            var chk = await permission.HasPermission(409, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                if (string.IsNullOrEmpty(obj.Name))
                    return Ok(await Result<object>.FailAsync($"{localization.GetHrResource("GroupName")}"));


                if (obj.FacilityId <= 0)
                    return Ok(await Result<object>.FailAsync($"{localization.GetHrResource("Company")}"));

                if (obj.Id <= 0)
                    return Ok(await Result<object>.FailAsync($"{localization.GetHrResource("Group")}"));

                var Edit = await hrServiceManager.HrSalaryGroupService.Update(obj);
                return Ok(Edit);

            }

            catch (Exception ex)
            {
                return Ok(await Result<HrSalaryGroupEditDto>.FailAsync($"{ex.Message}"));
            }
        }


        // this funnction to remove both deduction and allowance
        [HttpDelete("DeleteSalaryGroupAllowanceOrDeduction")]
        public async Task<IActionResult> DeleteSalaryGroupAllowanceOrDeduction(long Id = 0)
        {
            var chk = await permission.HasPermission(409, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (Id <= 0)
                return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));


            try
            {
                var del = await hrServiceManager.HrSalaryGroupAccountService.Remove(Id);

                return Ok(del);

            }
            catch (Exception exp)
            {
                return Ok(await Result.FailAsync($"{exp.Message}"));
            }
        }

        // this funnction to Add both deduction and allowance

        [HttpPost("AddSalaryGroupAllowanceOrDeduction")]
        public async Task<IActionResult> AddSalaryGroupAllowanceOrDeduction(HrSalaryGroupAccountDto obj)
        {
            var chk = await permission.HasPermission(409, PermissionType.Add);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                if (string.IsNullOrEmpty(obj.AccountDueCode))
                    return Ok(await Result<object>.FailAsync($"قم بإدخال حساب المستحق اولاً"));
                if (string.IsNullOrEmpty(obj.AccountExpCode))
                    return Ok(await Result<object>.FailAsync($"قم بإدخال حساب المصروف اولاً"));
                var addRes = await hrServiceManager.HrSalaryGroupAccountService.Add(obj);
                return Ok(addRes);
            }

            catch (Exception ex)
            {
                return Ok(await Result<HrSalaryGroupAccountDto>.FailAsync(ex.Message));
            }
        }



        //display all deduction
        [HttpGet("GetAllDeductionForGroup")]
        public async Task<IActionResult> GetAllDeductionForGroup(long groupId = 0)
        {
            var chk = await permission.HasPermission(409, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (groupId == 0)
            {
                return Ok(await Result.FailAsync("there is no groupId passed"));
            }

            try
            {
                var items = await hrServiceManager.HrSalaryGroupDeductionVwService.GetAllVW(e => e.IsDeleted == false && e.GroupId == groupId && e.TypeId == 2);
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();

                    return Ok(await Result<List<HrSalaryGroupDeductionVw>>.SuccessAsync(res.ToList(), items.Status.message));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
        //display all allowances
        [HttpGet("GetAllAllowancesForGroup")]
        public async Task<IActionResult> GetAllAllowancesForGroup(long groupId = 0)
        {
            var chk = await permission.HasPermission(409, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (groupId == 0)
            {
                return Ok(await Result.FailAsync("there is no groupId passed"));
            }

            try
            {
                var items = await hrServiceManager.HrSalaryGroupAllowanceVwService.GetAllVW(e => e.IsDeleted == false && e.GroupId == groupId && e.TypeId == 1);
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();

                    return Ok(await Result<List<HrSalaryGroupAllowanceVw>>.SuccessAsync(res.ToList(), items.Status.message));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        #endregion


        #region --------  ربط مجموعة الموظفين بأنواع الحسابات -------



        [HttpPost("AddSalaryGroupRefrance")]
        public async Task<ActionResult> AddSalaryGroupRefrance(HrSalaryGroupRefranceDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(409, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (obj.ReferenceTypeId <= 0)
                    return Ok(await Result<object>.FailAsync($"قم بإدخال نوع الحساب"));

                if (string.IsNullOrEmpty(obj.AccountCode))
                    return Ok(await Result<object>.FailAsync($"قم بإدخال رقم الحساب"));
                var getSalaryGroupRefranc = await hrServiceManager.HrSalaryGroupRefranceService.GetAll(e => e.IsDeleted == false && e.GroupId == obj.GroupId && e.ReferenceTypeId == obj.ReferenceTypeId);
                if (getSalaryGroupRefranc.Data.Any())
                    return Ok(await Result<object>.FailAsync($" نوع الحساب مربوط  مسبقا"));


                var add = await hrServiceManager.HrSalaryGroupRefranceService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add Hr SalaryGroupRefrance Controller, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetAllSalaryGroupReferences")]
        public async Task<IActionResult> GetAllSalaryGroupReferences(long groupId = 0)
        {
            var chk = await permission.HasPermission(409, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (groupId <= 0)
            {
                return Ok(await Result.FailAsync("Please choose an entity, there is no id passed"));
            }
            try
            {
                var items = await hrServiceManager.HrSalaryGroupRefranceService.GetAllVW(e => e.IsDeleted == false && e.GroupId == groupId);
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    if (res.Any())
                        res = res.OrderBy(e => e.Id);
                    return Ok(await Result<List<HrSalaryGroupRefranceVw>>.SuccessAsync(res.ToList(), items.Status.message));

                }
                return Ok(await Result<List<HrSalaryGroupRefranceVw>>.FailAsync(items.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpDelete("DeleteSalaryGroupRefrance")]
        public async Task<IActionResult> DeleteSalaryGroupRefrance(long Id = 0)
        {
            var chk = await permission.HasPermission(409, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (Id <= 0)
                return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));


            try
            {
                var del = await hrServiceManager.HrSalaryGroupRefranceService.Remove(Id);

                return Ok(del);
            }
            catch (Exception exp)
            {
                return Ok(await Result.FailAsync($"{exp.Message}"));
            }
        }

        [HttpGet("DDLAccountType")]
        public async Task<IActionResult> DDLAccountType(long groupId = 0)
        {
            var lang = session.Language;
            var chk = await permission.HasPermission(409, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (groupId == 0)
            {
                return Ok(await Result.FailAsync("Please choose an entity to delete it, there is no id passed"));
            }

            try
            {
                var getSalaryGroupRefranc = await hrServiceManager.HrSalaryGroupRefranceService.GetAll(e => e.IsDeleted == false && e.GroupId == groupId);
                var SalaryGroupRefrancTypeId = getSalaryGroupRefranc.Data.Select(x => x.ReferenceTypeId).ToList();
                if (getSalaryGroupRefranc.Succeeded)
                {
                    var getACCReferenceType = await accServiceManager.AccReferenceTypeService.GetAll(r => r.FlagDelete == false && r.ParentId == 8 && r.ParentId != r.ReferenceTypeId && !SalaryGroupRefrancTypeId.Contains(r.ReferenceTypeId));
                    var res = getACCReferenceType.Data.AsQueryable();
                    var list = listHelper.GetFromList<long>(getACCReferenceType.Data.Select(s => new DDListItem<long> { Name = lang == 1 ? s.ReferenceTypeName ?? "" : s.ReferenceTypeName2 ?? "", Value = s.ReferenceTypeId }), hasDefault: false);

                    return Ok(await Result<SelectList>.SuccessAsync(list));
                }
                return Ok(await Result<HrSalaryGroupRefranceDto>.SuccessAsync(getSalaryGroupRefranc.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLSalaryGroups")]
        public async Task<IActionResult> DDLSalaryGroups()
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<HrSalaryGroup, long>(d => d.IsDeleted == false && d.FacilityId == session.FacilityId, "Id", "Name");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }



        #endregion




        [HttpGet("DDLAllSalayGroupAccountsAllowance")]
        public async Task<IActionResult> DDLAllSalayGroupAccountsAllowance(long groupId = 0)
        {
            var lang = session.Language;
            var chk = await permission.HasPermission(409, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (groupId <= 0)
            {
                return Ok(await Result.FailAsync("Please choose an Group to get its DDL, there is no id passed"));
            }

            try
            {
                var getSalaryGroupAccounts = await hrServiceManager.HrSalaryGroupAccountService.GetAll(e => e.IsDeleted == false && e.GroupId == groupId && e.TypeId == 1);
                var getSalaryGroupAccountsAdId = getSalaryGroupAccounts.Data.Select(x => x.AdId).ToList();
                if (getSalaryGroupAccounts.Succeeded)
                {
                    var getSysLookUp = await mainServiceManager.SysLookupDataService.GetAll(r => r.Isdel == false && r.CatagoriesId == 20 && r.Code != null && !getSalaryGroupAccountsAdId.Contains((int)r.Code));
                    var res = getSysLookUp.Data.AsQueryable();
                    var list = listHelper.GetFromList<long>(getSysLookUp.Data.Select(s => new DDListItem<long> { Name = lang == 1 ? s.Name ?? "" : s.Name2 ?? "", Value = Convert.ToInt32(s.Code) }), hasDefault: false);

                    return Ok(await Result<SelectList>.SuccessAsync(list, "DDL All Allowance"));
                }
                return Ok(await Result<HrSalaryGroupAccountDto>.SuccessAsync(getSalaryGroupAccounts.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLAllSalayGroupAccountsDeduction")]
        public async Task<IActionResult> DDLAllSalayGroupAccountsDeduction(long groupId = 0)
        {
            var lang = session.Language;
            var chk = await permission.HasPermission(409, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (groupId <= 0)
            {
                return Ok(await Result.FailAsync("Please choose an Group to get its DDL, there is no id passed"));
            }

            try
            {
                var getSalaryGroupAccounts = await hrServiceManager.HrSalaryGroupAccountService.GetAll(e => e.IsDeleted == false && e.GroupId == groupId && e.TypeId == 2);
                var getSalaryGroupAccountsAdId = getSalaryGroupAccounts.Data.Select(x => x.AdId).ToList();
                if (getSalaryGroupAccounts.Succeeded)
                {
                    var getSysLookUp = await mainServiceManager.SysLookupDataService.GetAll(r => r.Isdel == false && r.CatagoriesId == 21 && r.Code != null && !getSalaryGroupAccountsAdId.Contains((int)r.Code));
                    var res = getSysLookUp.Data.AsQueryable();
                    var list = listHelper.GetFromList<long>(getSysLookUp.Data.Select(s => new DDListItem<long> { Name = lang == 1 ? s.Name ?? "" : s.Name2 ?? "", Value = Convert.ToInt32(s.Code) }), hasDefault: false);

                    return Ok(await Result<SelectList>.SuccessAsync(list, "DDL All Deduction"));
                }
                return Ok(await Result<HrSalaryGroupAccountDto>.SuccessAsync(getSalaryGroupAccounts.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }






    }
}