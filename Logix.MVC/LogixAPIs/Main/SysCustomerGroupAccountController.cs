using Logix.Application.Common;
using Logix.Application.DTOs.Main;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.Main;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.Main
{
    public class SysCustomerGroupAccountController : BaseMainApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ILocalizationService localization;

        public SysCustomerGroupAccountController(IMainServiceManager mainServiceManager,
            IPermissionHelper permission,
            ILocalizationService localization)
        {
            this.mainServiceManager = mainServiceManager;
            this.permission = permission;
            this.localization = localization;
        }

        private async Task<bool> CheckPermission(int cusTypeId, PermissionType permissionType)
        {
            try
            {
                bool chk = cusTypeId switch
                {
                    // supplier
                    1 => await permission.HasPermission(401, permissionType),
                    // customer
                    2 => await permission.HasPermission(400, permissionType),
                    // contractor
                    3 => await permission.HasPermission(830, permissionType),
                    // other groups
                    12 => await permission.HasPermission(1600, permissionType),
                    _ => false,
                };
                return chk;
            }
            catch
            {
                return false;
            }
        }

        [HttpPost("Search")]
        public async Task<ActionResult> Search(int groupId)
        {
            try
            {
                var items = await mainServiceManager.SysCustomerGroupAccountService.GetAllVW(g => g.IsDeleted == false && g.GroupId == Convert.ToInt64(groupId));
                if (items.Succeeded)
                {
                    var res = items.Data;
                    var final = res.ToList();

                    return Ok(await Result<List<SysCustomerGroupAccountsVw>>.SuccessAsync(final));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpPost("Add")]
        public async Task<ActionResult> Add(SysCustomerGroupAccountDto obj)
        {
            try
            {
                var chk = await CheckPermission(obj.CusTypeId ?? 0, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result<SysCustomerGroupDto>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var add = await mainServiceManager.SysCustomerGroupAccountService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<SysCustomerGroupDto>.FailAsync(ex.Message));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(int cusTypeId, long id)
        {
            try
            {
                var chk = await CheckPermission(cusTypeId, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await mainServiceManager.SysCustomerGroupAccountService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
    }
}
