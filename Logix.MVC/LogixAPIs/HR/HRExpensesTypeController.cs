using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.HR
{

    //   انواع مصروفات الموظفين
    public class HRExpensesTypeController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IAccServiceManager accServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;

        public HRExpensesTypeController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IAccServiceManager accServiceManager)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.accServiceManager = accServiceManager;
        }

        #region Index Page

        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrExpensesTypeFilterDto filter)
        {
            var chk = await permission.HasPermission(2106, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {

                var items = await hrServiceManager.HrExpensesTypeService.GetAllVW(e => e.IsDeleted == false
                    && e.FacilityId == session.FacilityId
                    && (string.IsNullOrEmpty(filter.Name) || ((e.Name != null && e.Name.ToLower().Contains(filter.Name.ToLower())) || (e.Name2 != null && e.Name2.ToLower().Contains(filter.Name.ToLower()))))
                );

                if (items.Succeeded)
                {
                    if (items.Data.Count() > 0)
                    {
                        var res = items.Data.AsQueryable();
                        return Ok(await Result<List<HrExpensesTypeVw>>.SuccessAsync(res.ToList(), ""));
                    }
                    return Ok(await Result<List<object>>.SuccessAsync(new List<object>(), localization.GetResource1("NosearchResult")));
                }
                return Ok(await Result<object>.FailAsync(items.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync(ex.Message));
            }
        }


        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var chk = await permission.HasPermission(2106, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await hrServiceManager.HrExpensesTypeService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete HRExpensesTypeController  Controller, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long Id)
        {
            try
            {
                var chk = await permission.HasPermission(2106, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var item = await hrServiceManager.HrExpensesTypeService.GetOneVW(x => x.Id == Id && x.IsDeleted == false);

                return Ok(item);

            }
            catch (Exception ex)
            {
                return Ok(await Result<HrLeaveVw>.FailAsync($"====== Exp in HRExpensesTypeController  getById, MESSAGE: {ex.Message}"));
            }
        }



        #endregion


        #region Add Page


        [HttpPost("Add")]
        public async Task<ActionResult> Add(HrExpensesTypeDto obj)
        {
            try
            {
                var FacilityId = session.FacilityId;
                var chk = await permission.HasPermission(2106, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (string.IsNullOrEmpty(obj.Name))
                    return Ok(await Result<object>.FailAsync($" يجب ادخال  اسم نوع المصروف عربي   "));
                if (string.IsNullOrEmpty(obj.AccountExpCode))
                    return Ok(await Result<object>.FailAsync($"يجب ادخال  حساب المصروف "));
                if (string.IsNullOrEmpty(obj.AccountDueCode))
                    return Ok(await Result<object>.FailAsync($"يجب ادخال  حساب الاستحقاق "));
                if (string.IsNullOrEmpty(obj.AccountPaidAdvanceCode))
                    return Ok(await Result<object>.FailAsync($"يجب ادخال  حساب المدفوع مقدماُ "));
                obj.VatRate = (obj.VatRate != null) ? obj.VatRate : 0;
                obj.Amount = (obj.Amount != null) ? obj.Amount : 0;
                obj.AppTypeIds = (string.IsNullOrEmpty(obj.AppTypeIds)) ? "0" : obj.AppTypeIds;
                obj.FacilityId = FacilityId;

                //  فحص  حساب الاستحقاق
                var checkAccountExist = await accServiceManager.AccAccountService.GetOne(x => x.AccAccountCode == obj.AccountDueCode && x.FacilityId == FacilityId);
                if (checkAccountExist.Data == null) return Ok(await Result<object>.FailAsync($"حساب الاستحقاق   غير موجود "));
                obj.AccountDueId = checkAccountExist.Data.AccAccountId;

                //  فحص  حساب المصروف
                var checkAccountExpenseExist = await accServiceManager.AccAccountService.GetOne(x => x.AccAccountCode == obj.AccountExpCode && x.FacilityId == FacilityId);
                if (checkAccountExpenseExist.Data == null) return Ok(await Result<object>.FailAsync($"حساب المصروف   غير موجود "));
                obj.AccountExpId = checkAccountExpenseExist.Data.AccAccountId;

                //  فحص  حساب المدفوع مقدماُ
                var checkAccountPaidAdvanceExist = await accServiceManager.AccAccountService.GetOne(x => x.AccAccountCode == obj.AccountPaidAdvanceCode && x.FacilityId == FacilityId);
                if (checkAccountPaidAdvanceExist.Data == null) return Ok(await Result<object>.FailAsync($"حساب  المدفوع مقدماُ غير موجود "));
                obj.AccountPaidAdvanceId = checkAccountPaidAdvanceExist.Data.AccAccountId;


                var add = await hrServiceManager.HrExpensesTypeService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add HRExpensesTypeController  Controller, MESSAGE: {ex.Message}"));
            }
        }



        #endregion

        #region Edit Page

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(HrExpensesTypeEditDto obj)
        {
            try
            {
                var FacilityId = session.FacilityId;

                var chk = await permission.HasPermission(2106, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (string.IsNullOrEmpty(obj.Name))
                    return Ok(await Result<object>.FailAsync($" يجب ادخال  اسم نوع المصروف عربي   "));
                if (string.IsNullOrEmpty(obj.AccountExpCode))
                    return Ok(await Result<object>.FailAsync($"يجب ادخال  حساب المصروف "));
                if (string.IsNullOrEmpty(obj.AccountDueCode))
                    return Ok(await Result<object>.FailAsync($"يجب ادخال  حساب الاستحقاق "));
                if (string.IsNullOrEmpty(obj.AccountPaidAdvanceCode))
                    return Ok(await Result<object>.FailAsync($"يجب ادخال  حساب المدفوع مقدماُ "));
                obj.VatRate = (obj.VatRate != null) ? obj.VatRate : 0;
                obj.Amount = (obj.Amount != null) ? obj.Amount : 0;
                obj.AppTypeIds = (string.IsNullOrEmpty(obj.AppTypeIds)) ? "0" : obj.AppTypeIds;

                //  فحص  حساب الاستحقاق
                var checkAccountExist = await accServiceManager.AccAccountService.GetOne(x => x.AccAccountCode == obj.AccountDueCode && x.FacilityId == FacilityId);
                if (checkAccountExist.Data == null) return Ok(await Result<object>.FailAsync($"حساب الاستحقاق   غير موجود "));
                obj.AccountDueId = checkAccountExist.Data.AccAccountId;

                //  فحص  حساب المصروف
                var checkAccountExpenseExist = await accServiceManager.AccAccountService.GetOne(x => x.AccAccountCode == obj.AccountExpCode && x.FacilityId == FacilityId);
                if (checkAccountExpenseExist.Data == null) return Ok(await Result<object>.FailAsync($"حساب المصروف   غير موجود "));
                obj.AccountExpId = checkAccountExpenseExist.Data.AccAccountId;

                //  فحص  حساب المدفوع مقدماُ
                var checkAccountPaidAdvanceExist = await accServiceManager.AccAccountService.GetOne(x => x.AccAccountCode == obj.AccountPaidAdvanceCode && x.FacilityId == FacilityId);
                if (checkAccountPaidAdvanceExist.Data == null) return Ok(await Result<object>.FailAsync($"حساب  المدفوع مقدماُ غير موجود "));
                obj.AccountPaidAdvanceId = checkAccountPaidAdvanceExist.Data.AccAccountId;


                var update = await hrServiceManager.HrExpensesTypeService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrMandateLocationMasterEditDto>.FailAsync($"====== Exp in HRExpensesTypeController Controller Edit, MESSAGE: {ex.Message}"));
            }
        }

        #endregion

    }

}