using Logix.Application.Common;
using Logix.Application.DTOs.PM;
using Logix.Application.DTOs.PM.Shared;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.PM;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.PM.ViewModel.PmProjectTypeVMFilter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.IdentityModel.Tokens;

namespace Logix.MVC.LogixAPIs.PM
{

    //   الاتفاقيات
    public class PMAgreementController : BasePMApiController
    {
        private readonly IPMServiceManager pMServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IApiDDLHelper ddlHelper;



        public PMAgreementController(IPMServiceManager pMServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IApiDDLHelper ddlHelper)
        {
            this.pMServiceManager = pMServiceManager;
            this.permission = permission;
            this.session = session;
            this.localization = localization;
            this.ddlHelper = ddlHelper;
        }

        #region Index Page


        [HttpPost("Search")]
        public async Task<IActionResult> Search(BindProjectsDto filter)
        {

            try
            {
                var chk = await permission.HasPermission(2022, PermissionType.Show);

                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                filter.Code ??= 0;
                filter.Code2 ??= 0;
                filter.IsSubContract = false;
                filter.BranchId ??= 0;
                filter.TenderStatus ??= 0;
                filter.ParentType ??= 0;
                filter.ProjectType ??= 0;
                filter.Type ??= 0;
                filter.AmountFrom ??= 0;
                filter.AmountTo ??= 0;
                filter.SystemId = 1000;
                filter.EmpId ??= 0;

                if (filter.ProjectType == 0)
                    filter.Type = 0;
                // un used filter properties
                filter.OwnerDeptId ??= 0;
                filter.PaymentType ??= 0;
                filter.ProjectValue ??= 0;
                filter.Iscase ??= 0;
                filter.OwnerDeptId ??= 0;
                filter.StatusId ??= 0;
                filter.Iscase = 0;
                filter.Isletter = false;
                filter.IsActive = false;

                if (string.IsNullOrEmpty(filter.From) || string.IsNullOrEmpty(filter.To))
                {
                    filter.From = "";
                    filter.To = "";
                }
                //   في حال كان موظف مشاريع تظهر فقط المنافسة الخاصة به

                if (session.SalesType == 2)
                    filter.EmpId = session.EmpId;



                var result = await pMServiceManager.PMProjectsService.BindProjects(filter);
                return Ok(result);
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
                var chk = await permission.HasPermission(275, PermissionType.Delete);

                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await pMServiceManager.PMProjectsService.Remove(id);

                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"======= Exp in {this.GetType()}: {ex.Message}"));
            }

        }



        [HttpPost("ChangeStatus")]
        public async Task<IActionResult> ChangeStatus(PmProjectsStatusChangeStatusDto obj)
        {

            try
            {

                var chk = await permission.HasPermission(2022, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                // ملاحظات 

                if (string.IsNullOrEmpty(obj.Note))
                    return Ok(await Result<object>.FailAsync(localization.GetCommonResource("notes")));

                // الحالة
                if (obj.StatusId <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("Status")));


                // الاتفاقية 
                if (obj.Ids.Count <= 0)
                    return Ok(await Result<object>.FailAsync("قم بعملية إختيار المشروع اولاً"));

                obj.StepId = 0;
                obj.CompletionRate = 0;
                var result = await pMServiceManager.PmProjectsStatusService.ChangeStatus(obj);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"======= Exp in {this.GetType()}: {ex.Message}"));

            }

        }


        #endregion

        //#region Edit Page



        //[HttpGet("GetById")]
        //public async Task<IActionResult> Edit(long id)
        //{
        //    try
        //    {

        //        if (id <= 0)
        //            return Ok(await Result<object>.FailAsync(localization.GetMessagesResource("NoIdInUpdate")));

        //        var item = await pMServiceManager.PMProjectsTypeService.GetOneVW(x => x.IsDeleted == false && x.Id == id);

        //        return Ok(item);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(await Result<object>.FailAsync($"Error in {this.GetType()}: {ex.Message}"));
        //    }
        //}


        //[HttpPost("Edit")]
        //public async Task<IActionResult> Edit(PmProjectsTypeEditDto obj)
        //{

        //    try
        //    {

        //        var chk = await permission.HasPermission(2021, PermissionType.Edit);
        //        if (!chk)
        //            return Ok(await Result.AccessDenied("AccessDenied"));
        //        if (obj.Id <= 0)
        //            return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));

        //        //   النوع 

        //        if (obj.ParentId <= 0)
        //            return Ok(await Result.FailAsync($"{localization.GetCommonResource("type")}"));


        //        // اسم التصنيف 
        //        if (string.IsNullOrEmpty(obj.TypeName))
        //            return Ok(await Result<object>.FailAsync(localization.GetPMResource("TypeName")));

        //        var result = await pMServiceManager.PMProjectsTypeService.Update(obj);
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(await Result<string>.FailAsync($"======= Exp in {this.GetType()}: {ex.Message}"));

        //    }

        //}

        //#endregion


        //#region Add Page

        //[HttpPost("Add")]
        //public async Task<IActionResult> Add(PmProjectsTypeDto obj)
        //{

        //    try
        //    {
        //        var chk = await permission.HasPermission(2021, PermissionType.Add);
        //        if (!chk)
        //            return Ok(await Result.AccessDenied("AccessDenied"));
        //        //   النوع 

        //        if (obj.ParentId <= 0)
        //            return Ok(await Result.FailAsync($"{localization.GetCommonResource("type")}"));

        //        // اسم التصنيف 
        //        if (string.IsNullOrEmpty(obj.TypeName))
        //            return Ok(await Result<object>.FailAsync(localization.GetPMResource("TypeName")));

        //        obj.Iscase = false;
        //        obj.FacilityId = session.FacilityId;
        //        obj.SystemId = 9;
        //        var result = await pMServiceManager.PMProjectsTypeService.Add(obj);
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(await Result<string>.FailAsync($"======= Exp in {this.GetType()}: {ex.Message}"));

        //    }

        //}

        //#endregion

    }
}
