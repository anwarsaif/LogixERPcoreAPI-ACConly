using Logix.Application.Common;
using Logix.Application.DTOs.ACC;
using Logix.Application.DTOs.Main;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.Acc
{
    public class ACCCashonhandUsersApiController : BaseAccApiController
    {

        private readonly IAccServiceManager accServiceManager;
        private readonly IMainServiceManager mainServiceManager;
        private readonly IPermissionHelper permission;
        private readonly IWebHostEnvironment env;
        private readonly IFilesHelper filesHelper;
        private readonly IDDListHelper listHelper;
        private readonly ILocalizationService localization;
        private readonly ICurrentData _session;

        public ACCCashonhandUsersApiController(
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

        [HttpGet("GetUsersPermissionAccCashonhand")]
        public async Task<IActionResult> GetUsersPermissionAccCashonhand(long id)
        {
            var chk = await permission.HasPermission(332, PermissionType.Show);
            if (!chk)
                return Ok(await Result.AccessDenied("AccessDenied"));

            if (id <= 0)
                return Ok(await Result<SysUserDto>.FailAsync(localization.GetMessagesResource("NoIdInUpdate")));

            var accCashOnHandResult = await accServiceManager.AccCashOnHandService.GetAllVW(s => s.Id == id && s.IsDeleted == false && s.IsDeleted == false && s.FacilityId == _session.FacilityId);
            if (accCashOnHandResult.Succeeded)
            {
                var usersPermission = accCashOnHandResult.Data.Select(b => b.UsersPermission).FirstOrDefault();

                if (usersPermission != null)
                {
                    var userIds = usersPermission.Split(',');
                    var users = await mainServiceManager.SysUserService
                        .GetAll(x => x.Isdel == false && x.FacilityId == _session.FacilityId && userIds.Contains(x.Id.ToString()));
                    return Ok(users);
                }
            }

            return Ok(await Result<SysUserDto>.FailAsync("No users found."));
        }
        #endregion "transactions"

        #region "transactions_Update"
        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(AccCashOnHandUsersDto obj)
        {
            var chk = await permission.HasPermission(332, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<AccCashOnHandUsersDto>.FailAsync(localization.GetMessagesResource("dataRequire")));
                }

                var Edit = await accServiceManager.AccCashOnHandService.UpdateUsersPermission(obj.ID ?? 0, obj.UsersPermission);
                return Ok(Edit);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccCashOnHandUsersDto>.FailAsync($"======= Exp in AcCash OnHand Users  edit: {ex.Message}"));
            }
        }

        #endregion "transactions_Update"
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long CashId, long Id)
        {
            var chk = await permission.HasPermission(332, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var UsersPermission = await accServiceManager.AccCashOnHandService.GetOne(x => x.UsersPermission, s => s.Id == CashId && s.IsDeleted == false && s.FacilityId == _session.FacilityId);

                var UserList = new List<int>();

                var currentUsers = UsersPermission.Data ?? "";
                var currentUserList = currentUsers.Split(",");
                if (!string.IsNullOrEmpty(currentUsers))
                {
                    foreach (var d in currentUserList)
                    {
                        if (int.Parse(d) != Id)
                        {
                            UserList.Add(int.Parse(d));
                        }
                    }
                }

                var UsersPermissionEdit = string.Join(",", UserList);
                var Edit = await accServiceManager.AccCashOnHandService.UpdateUsersPermission(CashId, UsersPermissionEdit);
                return Ok(Edit);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccCashOnHandUsersDto>.FailAsync($"======= Exp in AcCash OnHand Users Delete: {ex.Message}"));
            }

        }

    }
}