using Logix.Application.Common;
using Logix.Application.DTOs.PM;
using Logix.Application.DTOs.PM.Shared;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.PM.ViewModel.PmProjectTypeVMFilter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;

namespace Logix.MVC.LogixAPIs.PM
{
    //   انواع الاتفاقيات
    public class PMAgreementsTypeController : BasePMApiController
    {
        private readonly IPMServiceManager pMServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IWFServiceManager wFServiceManager;



        public PMAgreementsTypeController(IPMServiceManager pMServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IWFServiceManager wFServiceManager)
        {
            this.pMServiceManager = pMServiceManager;
            this.permission = permission;
            this.session = session;
            this.localization = localization;
            this.wFServiceManager = wFServiceManager;
        }

        #region Index Page


        [HttpPost("Search")]
        public async Task<IActionResult> Search(PmProjectTypeSearch filter)
        {
            var chk = await permission.HasPermission(2021, PermissionType.Show);

            if (!chk)
                return Ok(await Result.AccessDenied("AccessDenied"));

            try
            {

                var items = await pMServiceManager.PMProjectsTypeService.GetAllVW(e => e.IsDeleted == false
                && e.SystemId == 1000
                && e.FacilityId == session.FacilityId
                && (string.IsNullOrEmpty(filter.TypeName) || (e.TypeName != null && e.TypeName.Contains(filter.TypeName)))
                );

                if (!items.Succeeded)
                    return Ok(await Result<object>.FailAsync(items.Status.message));
                if (!items.Data.Any())
                    return Ok(await Result<object>.SuccessAsync(new List<object>(), localization.GetResource1("NosearchResult")));

                var res = items.Data.AsQueryable();


                if (!res.Any())
                    return Ok(await Result<object>.SuccessAsync(new List<object>(), localization.GetResource1("NosearchResult")));

                res = res.OrderBy(e => e.Id);


                return Ok(await Result<object>.SuccessAsync(res, ""));
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"======= Exp in {this.GetType()}: {ex.Message}"));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var chk = await permission.HasPermission(1910, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await pMServiceManager.PMProjectsTypeService.Remove(id);

                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"======= Exp in {this.GetType()}: {ex.Message}"));
            }

        }

        #endregion

        #region Edit Page



        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long id)
        {
            try
            {
                // Check permissions
                var check = await permission.HasPermission(2021, PermissionType.Edit);

                if (!check)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetMessagesResource("NoIdInUpdate")));

                var item = await pMServiceManager.PMProjectsTypeService.GetOneVW(x => x.IsDeleted == false && x.Id == id);

                return Ok(item);
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"Error in {this.GetType()}: {ex.Message}"));
            }
        }


        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(PmProjectsTypeEditDto obj)
        {

            try
            {

                var chk = await permission.HasPermission(2021, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (obj.Id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));


                var GetItem = await pMServiceManager.PMProjectsTypeService.GetOneVW(x => x.IsDeleted == false && x.Id == obj.Id);
                if (!GetItem.Succeeded)
                    return Ok(GetItem);
                //  اسم النوع 
                if (string.IsNullOrEmpty(obj.TypeName))
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("TypeName")));
                obj.ParentId = GetItem.Data.ParentId;
                var result = await pMServiceManager.PMProjectsTypeService.Update(obj);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"======= Exp in {this.GetType()}: {ex.Message}"));

            }

        }

        #endregion


        #region Add Page

        [HttpPost("Add")]
        public async Task<IActionResult> Add(PmProjectsTypeDto obj)
        {

            try
            {
                var chk = await permission.HasPermission(2021, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                //  اسم النوع 
                if (string.IsNullOrEmpty(obj.TypeName))
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("TypeName")));

                obj.Iscase = false;
                obj.ParentId = 0;
                obj.FacilityId = session.FacilityId;
                obj.SystemId = 1000;
                var result = await pMServiceManager.PMProjectsTypeService.Add(obj);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"======= Exp in {this.GetType()}: {ex.Message}"));

            }

        }

        #endregion

    }
}
