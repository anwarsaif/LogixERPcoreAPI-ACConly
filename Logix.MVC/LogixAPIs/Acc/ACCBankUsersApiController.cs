using Autofac.Core;
using Logix.Application.Common;
using Logix.Application.DTOs.ACC;
using Logix.Application.DTOs.Main;
using Logix.Application.DTOs.OPM;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using NuGet.Packaging;

namespace Logix.MVC.LogixAPIs.Acc
{
    
    public class ACCBankUsersApiController : BaseAccApiController
    {

        private readonly IAccServiceManager accServiceManager;
        private readonly IMainServiceManager mainServiceManager;
        private readonly IPermissionHelper permission;
        private readonly IWebHostEnvironment env;
        private readonly IFilesHelper filesHelper;
        private readonly IDDListHelper listHelper;
        private readonly ILocalizationService localization;
        private readonly ICurrentData _session;

        public ACCBankUsersApiController(
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

        [HttpGet("GetUsersPermissionACCBank")]
        public async Task<IActionResult> GetUsersPermissionACCBank(long id)
        {
            var chk = await permission.HasPermission(63, PermissionType.Show);
            if (!chk)
                return Ok(await Result.AccessDenied("AccessDenied"));

            if (id <= 0)
                return Ok(await Result<SysUserDto>.FailAsync(localization.GetMessagesResource("NoIdInUpdate")));

            var accBanksResult = await accServiceManager.AccBankService.GetAll(s => s.BankId == id && s.IsDeleted == false&&s.FacilityId==_session.FacilityId);
            if (accBanksResult.Succeeded)
            {
                var usersPermission = accBanksResult.Data.Select(b => b.UsersPermission).FirstOrDefault();

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
        public async Task<IActionResult> Edit(ACCBankUsersDto obj)
        {
            var chk = await permission.HasPermission(63, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<ACCBankUsersDto>.FailAsync(localization.GetMessagesResource("dataRequire")));
                }

                var Edit = await accServiceManager.AccBankService.UpdateUsersPermission(obj.BankID??0, obj.UsersPermission);
                return Ok(Edit);
            }
            catch (Exception ex)
            {
                return Ok(await Result<ACCBankUsersDto>.FailAsync($"======= Exp in Acc Bank User edit: {ex.Message}"));
            }
        }

        #endregion "transactions_Update"
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long BankId, long Id)
        {
            var chk = await permission.HasPermission(63, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var UsersPermission = await accServiceManager.AccBankService.GetOne(x=>x.UsersPermission,s => s.BankId == BankId && s.IsDeleted == false&&s.FacilityId==_session.FacilityId);

                var UserList =new  List<int>();
             
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
                var Edit = await accServiceManager.AccBankService.UpdateUsersPermission(BankId, UsersPermissionEdit);
                return Ok(Edit);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccBankDto>.FailAsync($"======= Exp in Acc Bank User Delete: {ex.Message}"));
            }
        
    }
    }
}