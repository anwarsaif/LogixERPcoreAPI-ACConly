using Logix.Application.Common;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.Acc.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.Acc
{
    public class AccAccountsTreeController : BaseAccApiController
    {

        private readonly IAccServiceManager accServiceManager;
        private readonly IPermissionHelper permission;
        private readonly IWebHostEnvironment env;
        private readonly IFilesHelper filesHelper;
        private readonly IDDListHelper listHelper;
        private readonly ILocalizationService localization;
        private readonly ISysConfigurationHelper configurationHelper;
        private readonly ICurrentData _session;

        public AccAccountsTreeController(
            IAccServiceManager accServiceManager,
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
            this.permission = permission;
            this.env = env;
            this.filesHelper = filesHelper;
            this.listHelper = listHelper;
            this.localization = localization;
            this.configurationHelper = configurationHelper;
            this._session = session;
        }







        #region "AccountsTree"

        [HttpGet("AccountsTree")]
        public async Task<IActionResult> GetAccountsTree()
        {
            int lang = _session.Language;
            var chk = await permission.HasPermission(61, PermissionType.Show);
            if (!chk)
            {
                return Ok(Result.AccessDenied("AccessDenied"));
            }

            List<AccountVM> accountVMs = new List<AccountVM>();
            var accounts = await accServiceManager.AccAccountService.GetAll(x => x.IsDeleted == false && x.FacilityId == _session.FacilityId);
            accountVMs = accounts.Data.Select(a => new AccountVM { AccAccountId = a.AccAccountId, AccAccountName = lang == 1 ? a.AccAccountName : a.AccAccountName2 ?? a.AccAccountName, AccAccountParentId = a.AccAccountParentId }).ToList();

            List<AccountVM> mainAccounts = accountVMs
                .Where(a => a.AccAccountParentId == a.AccAccountId || a.AccAccountParentId == null)
                .ToList();

            List<AccountApiNode> accountsWithChildren = new List<AccountApiNode>();
            foreach (var mainAccount in mainAccounts)
            {
                var accountData = new AccountApiNode
                {
                    AccountId = mainAccount.AccAccountId,
                    AccountName = mainAccount.AccAccountName,
                    AccountName2 = mainAccount.AccAccountName2,
                    Icon = "jstree-folder" // Replace with the CSS class for the desired icon
                };

                accountData.Children = await GetAccountsWithChildrenRecursive(mainAccount.AccAccountId);

                accountsWithChildren.Add(accountData);
            }

            return Ok(await Result<List<AccountApiNode>>.SuccessAsync(accountsWithChildren, ""));

        }

        [NonAction]

        private async Task<List<AccountApiNode>> GetAccountsWithChildrenRecursive(long accountId)
        {
            int lang = _session.Language;
            List<AccountVM> accountVMs = new List<AccountVM>();
            var accounts = await accServiceManager.AccAccountService.GetAll(x => x.IsDeleted == false && x.FacilityId == _session.FacilityId);
            accountVMs = accounts.Data.Select(a => new AccountVM { AccAccountId = a.AccAccountId, AccAccountName = lang == 1 ? a.AccAccountName : a.AccAccountName2 ?? a.AccAccountName, AccAccountParentId = a.AccAccountParentId }).ToList();
            List<AccountVM> childAccounts = accountVMs
                .Where(a => a.AccAccountParentId == accountId && a.AccAccountParentId != a.AccAccountId)
                .ToList();

            List<AccountApiNode> accountNodes = new List<AccountApiNode>();
            foreach (var childAccount in childAccounts)
            {
                var accountNode = new AccountApiNode
                {
                    AccountId = childAccount.AccAccountId,
                    AccountName = childAccount.AccAccountName,
                    AccountName2 = childAccount.AccAccountName2,
                    Icon = "jstree-folder" // Replace with the CSS class for the desired icon
                };

                accountNode.Children = await GetAccountsWithChildrenRecursive(childAccount.AccAccountId);

                accountNodes.Add(accountNode);
            }

            return accountNodes;
        }

        #endregion "AccountsTree"





    }
}