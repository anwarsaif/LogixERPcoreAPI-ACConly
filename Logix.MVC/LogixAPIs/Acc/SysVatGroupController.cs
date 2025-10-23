using Logix.Application.Common;
using Logix.Application.DTOs.Main;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.Main;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.Acc
{
    public class SysVatGroupController : BaseAccApiController
    {

        private readonly IAccServiceManager accServiceManager;
        private readonly IMainServiceManager mainServiceManager;
        private readonly IPermissionHelper permission;
        private readonly IWebHostEnvironment env;
        private readonly IFilesHelper filesHelper;
        private readonly IDDListHelper listHelper;
        private readonly ILocalizationService localization;
        private readonly ICurrentData _session;

        public SysVatGroupController(
            IAccServiceManager accServiceManager,
           IMainServiceManager mainServiceManager,

            IPermissionHelper permission,
             IWebHostEnvironment env,
            IHttpContextAccessor httpContextAccessor,
            IFilesHelper filesHelper,
            IDDListHelper listHelper,
             ILocalizationService localization
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
            this._session = session;
        }



        #region "transactions"


        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var chk = await permission.HasPermission(765, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var items = await mainServiceManager.SysVatGroupService.GetAllVW(x => x.IsDeleted == false && x.FacilityId == _session.FacilityId);
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    res = res.OrderBy(e => e.VatId);
                    return Ok(await Result<List<SysVatGroupVw>>.SuccessAsync(res.ToList(), ""));

                }
                return Ok(items);

            }
            catch (Exception ex)
            {
                return Ok(await Result<SysVatGroupDto>.FailAsync($"======= Exp in Search SysVATGroup, MESSAGE: {ex.Message}"));

            }
        }
        [HttpPost("Search")]
        public async Task<IActionResult> Search(SysVatGroupFilterDto filter)
        {
            var chk = await permission.HasPermission(765, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                filter.VatRate ??= 0;
                var items = await mainServiceManager.SysVatGroupService.GetAllVW(c => c.IsDeleted == false && c.FacilityId == _session.FacilityId

                && (string.IsNullOrEmpty(filter.VatName) || (c.VatName != null && c.VatName.Contains(filter.VatName))) &&
              (filter.VatRate == 0 || c.VatRate == filter.VatRate)


                );
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();

                    var final = res.ToList();
                    return Ok(await Result<List<SysVatGroupVw>>.SuccessAsync(final, ""));
                }
                return Ok(items);

            }
            catch (Exception ex)
            {
                return Ok(await Result<SysVatGroupVw>.FailAsync($"======= Exp in Search SysVATGroupVW, MESSAGE: {ex.Message}"));
            }
        }
        #endregion "transactions"

        #region "transactions_ADD"
        [HttpPost("Add")]
        public async Task<IActionResult> Add(SysVatGroupDto obj)
        {
            var chk = await permission.HasPermission(765, PermissionType.Add);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<SysVatGroupDto>.FailAsync(localization.GetMessagesResource("dataRequire")));
                }


                var add = await mainServiceManager.SysVatGroupService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<SysVatGroupDto>.FailAsync($"======= Exp in Acc SysVatGroupDto  add: {ex.Message}"));
            }
        }
        #endregion "transactions_Add"

        #region "transactions_Update"

        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(SysVatGroupEditDto obj)
        {
            var chk = await permission.HasPermission(765, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<SysVatGroupEditDto>.FailAsync(localization.GetMessagesResource("dataRequire")));
                }


                var Edit = await mainServiceManager.SysVatGroupService.Update(obj);
                return Ok(Edit);
            }
            catch (Exception ex)
            {
                return Ok(await Result<SysVatGroupEditDto>.FailAsync($"======= Exp in Acc Group  edit: {ex.Message}"));
            }
        }

        #endregion "transactions_Update"


        #region "transactions_Delete"
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long Id)
        {
            var chk = await permission.HasPermission(765, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var add = await mainServiceManager.SysVatGroupService.Remove(Id);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<SysVatGroupDto>.FailAsync($"======= Exp in Acc VatGroup  Delete: {ex.Message}"));
            }
        }
        #endregion "transactions_Delete"

        #region "transactions_GetById"

        [HttpGet("GetByIdForEdit")]
        public async Task<IActionResult> GetByIdForEdit(long id)
        {
            try
            {
                var chk = await permission.HasPermission(765, PermissionType.Show);
                if (!chk)
                {
                    return Ok(await Result.AccessDenied("AccessDenied"));
                }

                if (id <= 0)
                {
                    return Ok(await Result<SysVatGroupEditDto>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var getItem = await mainServiceManager.SysVatGroupService.GetForUpdate<SysVatGroupEditDto>(id);
                if (getItem.Succeeded)
                {

                    //========================حساب المبيعات
                    var acc = await accServiceManager.AccAccountService.GetOne(x => x.AccAccountId == getItem.Data.SalesVatAccountId);

                    var obj = new SysVatGroupEditDto();
                    obj = getItem.Data;
                    if (acc.Data != null)
                    {
                        obj.SalesVatAccountCode = acc.Data.AccAccountCode;
                        if (_session.Language == 1)
                        {
                            obj.SalesVatAccountName = acc.Data.AccAccountName;
                        }
                        else
                        {
                            obj.SalesVatAccountName = acc.Data.AccAccountName2;
                        }
                    }
                    //========================حساب المشتريات
                    var accpuh = await accServiceManager.AccAccountService.GetOne(x => x.AccAccountId == getItem.Data.PurchasesVatAccountId);

                    if (accpuh.Data != null)
                    {
                        obj.PurchasesVatAccountCode = accpuh.Data.AccAccountCode;
                        if (_session.Language == 1)
                        {
                            obj.PurchasesVatAccountName = accpuh.Data.AccAccountName;
                        }
                        else
                        {
                            obj.PurchasesVatAccountName = accpuh.Data.AccAccountName2;
                        }

                    }
                    return Ok(await Result<SysVatGroupEditDto>.SuccessAsync(obj, $""));
                }
                else
                {
                    return Ok(getItem);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<SysVatGroupEditDto>.FailAsync($"====== Exp in GetByIdForEdit  Sys VatGroup, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(long id)
        {
            try
            {
                var chk = await permission.HasPermission(765, PermissionType.Show);
                if (!chk)
                {
                    return Ok(await Result.AccessDenied("AcessDenied"));
                }

                if (id <= 0)
                {
                    return Ok(await Result<SysVatGroupDto>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var getItem = await mainServiceManager.SysVatGroupService.GetOne(s => s.VatId == id && s.IsDeleted == false);
                if (getItem.Succeeded)
                {
                    var obj = getItem.Data;
                    return Ok(await Result<SysVatGroupDto>.SuccessAsync(obj, $""));
                }
                else
                {
                    return Ok(getItem);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<SysVatGroupDto>.FailAsync($"====== Exp in GetById Sys VatGroup, MESSAGE: {ex.Message}"));
            }
        }
        #endregion "transactions_GetById"

    }
}
