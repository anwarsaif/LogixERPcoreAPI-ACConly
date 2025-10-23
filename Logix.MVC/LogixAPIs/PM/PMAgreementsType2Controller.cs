using Logix.Application.Common;
using Logix.Application.DTOs.PM;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.PM;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.PM.ViewModel.PmProjectTypeVMFilter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;

namespace Logix.MVC.LogixAPIs.PM
{

    //  تصنيف المنافسة
    public class PMAgreementsType2Controller : BasePMApiController
    {
        private readonly IPMServiceManager pMServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IApiDDLHelper ddlHelper;



        public PMAgreementsType2Controller(IPMServiceManager pMServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IApiDDLHelper ddlHelper)
        {
            this.pMServiceManager = pMServiceManager;
            this.permission = permission;
            this.session = session;
            this.localization = localization;
            this.ddlHelper = ddlHelper;
        }

        #region Index Page


        [HttpPost("Search")]
        public async Task<IActionResult> Search(PmProjectTypeSearch filter)
        {

            try
            {
                filter.ParentId ??= 0;
                var items = await pMServiceManager.PMProjectsTypeService.GetAllVW(e => e.IsDeleted == false
                && e.ParentId != e.Id
                && e.FacilityId == session.FacilityId
                && (string.IsNullOrEmpty(filter.TypeName) || (e.TypeName != null && e.TypeName.Contains(filter.TypeName)))
                );
                if (!items.Succeeded)
                    return Ok(await Result<object>.FailAsync(items.Status.message));
                if (!items.Data.Any())
                    return Ok(await Result<object>.SuccessAsync(new List<object>(), localization.GetResource1("NosearchResult")));

                var res = items.Data.AsQueryable();
                if (filter.ParentId != 0)
                {
                    res=res.Where(x => x.ParentId == filter.ParentId);  
                }

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
       
        [HttpGet("DDLType")]
        public async Task<IActionResult> DDLType(long? TypeId)
        {
            try
            {
                var lang = session.Language;
                var FacilityId = session.FacilityId;
                var list = await ddlHelper.GetAnyLis<PmProjectsType, long>(d => d.IsDeleted == false && d.Id == d.ParentId && d.SystemId == 1000&&d.FacilityId==FacilityId, "Id", lang == 2 ? "TypeName" : "TypeName");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
       
        
        #endregion

        #region Edit Page



        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long id)
        {
            try
            {

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

                //   النوع 

                if (obj.ParentId <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetCommonResource("type")}"));


                // اسم التصنيف 
                if (string.IsNullOrEmpty(obj.TypeName))
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("TypeName")));

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
                //   النوع 

                if (obj.ParentId <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetCommonResource("type")}"));

                // اسم التصنيف 
                if (string.IsNullOrEmpty(obj.TypeName))
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("TypeName")));

                obj.Iscase = false;
                obj.FacilityId = session.FacilityId;
                obj.SystemId = 9;
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
