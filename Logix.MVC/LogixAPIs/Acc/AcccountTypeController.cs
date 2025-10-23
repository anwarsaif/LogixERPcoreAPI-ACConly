using Logix.Application.Common;
using Logix.Application.DTOs.ACC;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.Acc
{

    public class AcccountTypeController : BaseAccApiController
    {

        private readonly IAccServiceManager accServiceManager;
        private readonly IPermissionHelper permission;
        private readonly IWebHostEnvironment env;
        private readonly IFilesHelper filesHelper;
        private readonly IDDListHelper listHelper;
        private readonly ILocalizationService localization;
        private readonly ICurrentData _session;

        public AcccountTypeController(
            IAccServiceManager accServiceManager,
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
            var chk = await permission.HasPermission(919, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var items = await accServiceManager.AccReferenceTypeService.GetAll(x => x.FlagDelete == false && x.ReferenceTypeId == x.ParentId);
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    res = res.OrderBy(e => e.ReferenceTypeId);
                    return Ok(await Result<List<AccReferenceTypeDto>>.SuccessAsync(res.ToList()));

                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccReferenceTypeDto>.FailAsync($"======= Exp in Acc ReferenceType : {ex.Message}"));

            }
        }

        [HttpGet("GetByParentID")]
        public async Task<IActionResult> GetByParentID(long Id)
        {
            var chk = await permission.HasPermission(919, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var items = await accServiceManager.AccReferenceTypeService.GetAllVW(x => x.FlagDelete == false && x.ParentId == Id);
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    res = res.OrderBy(e => e.ReferenceTypeId);
                    return Ok(await Result<List<AccReferenceTypeVw>>.SuccessAsync(res.ToList()));

                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccReferenceTypeVw>.FailAsync($"======= Exp in Acc ReferenceType  add: {ex.Message}"));
            }
        }

        #endregion "transactions"


        #region "transactions_ADD"
        [HttpPost("Add")]
        public async Task<IActionResult> Add(AccReferenceTypeDto obj)
        {
            var chk = await permission.HasPermission(919, PermissionType.Add);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<AccReferenceTypeDto>.FailAsync(localization.GetMessagesResource("dataRequire")));
                }
                var add = await accServiceManager.AccReferenceTypeService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccReferenceTypeDto>.FailAsync($"======= Exp in Acc ReferenceType  add: {ex.Message}"));
            }
        }
        #endregion "transactions_Add"



    }
}