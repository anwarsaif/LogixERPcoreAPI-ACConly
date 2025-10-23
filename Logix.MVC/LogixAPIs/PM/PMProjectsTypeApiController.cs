using Logix.Application.Common;
using Logix.Application.DTOs.PM;
using Logix.Application.DTOs.WH;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Services;
using Logix.Application.Wrapper;
using Logix.Domain.PM;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.PM.ViewModel.PmProjectTypeVMFilter;
using Logix.MVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Globalization;

namespace Logix.MVC.LogixAPIs.PM
{
    public class PMProjectsTypeApiController : BasePMApiController
    {
        private readonly IPMServiceManager pMServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;

        public PMProjectsTypeApiController(IPMServiceManager pMServiceManager, IPermissionHelper permission, ICurrentData session)
        {
            this.pMServiceManager = pMServiceManager;
            this.permission = permission;
            this.session = session;
        }
        /*[HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var chk = await permission.HasPermission(256, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {

                var items = await pMServiceManager.PMProjectsTypeService.GetAll(e => e.IsDeleted == false);
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    res = res.OrderBy(e => e.Id);
                    return Ok(await Result<List<PmProjectsTypeDto>>.SuccessAsync(res.ToList(),items.Status.message));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<List<PmProjectsType>>.FailAsync($"======= Exp in Wh whunit : {ex.Message}"));

            }
        }
        
          */    
        [HttpPost("Search")]
        public async Task<IActionResult> Search(PmProjectTypeSearch entity)
        {
            var chk = await permission.HasPermission(256, PermissionType.Show);
            var chk2 = await permission.HasPermission(410, PermissionType.Show);


            if (chk ==false && chk2 ==false)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                if (entity == null )
                {

                    return Ok(await Result<List<PmProjectsTypeVw>>.FailAsync($"======= Exp in PM entity can not be null"));

                }
                
                var items = await pMServiceManager.PMProjectsTypeService.GetAllVW(e => (e.TypeName??"").Contains( entity.TypeName??"" )
                                        && e.IsDeleted == false &&  e.SystemId == 5 && e.Id==e.ParentId);
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    res = res.OrderBy(e => e.Id);
                    
                    return Ok(await Result<List<PmProjectsTypeVw>>.SuccessAsync(res.ToList(),items.Status.message));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<List<PmProjectsTypeVw>>.FailAsync($"======= Exp in Wh whunit : {ex.Message}"));

            }
        }
        
        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(long id=0)
        {
            var chk = await permission.HasPermission(256, PermissionType.Show);
            var chk2 = await permission.HasPermission(410, PermissionType.Show);


            if (chk ==false && chk2 ==false)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                if (id <= 0 )
                {

                    return Ok(await Result<PmProjectsTypeVw>.FailAsync($"======= Exp in PM entity can not be null"));

                }

                var items = await pMServiceManager.PMProjectsTypeService.GetOneVW(e => e.Id== id);
                                        
              
             
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<PmProjectsTypeVw>.FailAsync($"======= Exp in Wh whunit : {ex.Message}"));

            }
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add(PmProjectsTypeDto entity)
        {

            var chk = await permission.HasPermission(256, PermissionType.Add);
            var chk2 = await permission.HasPermission(410, PermissionType.Add);
            if (chk == false && chk2==false)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                entity.FacilityId = session.FacilityId;
                entity.SystemId = 5;
                var items = await pMServiceManager.PMProjectsTypeService.Add(entity);

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<PmProjectsTypeDto>.FailAsync($"======= Exp in Wh whunit : {ex.Message}"));

            }

        }

        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(PmProjectsTypeEditDto entity)
        {
            var chk = await permission.HasPermission(256, PermissionType.Edit);
            var chk2 = await permission.HasPermission(256, PermissionType.Show);
            var chk3 = await permission.HasPermission(410, PermissionType.Edit);
            var chk4 = await permission.HasPermission(410, PermissionType.Show);
            if((chk==false && chk2==false )&& (chk3==false && chk4 == false)) { 
           
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var items = await pMServiceManager.PMProjectsTypeService.Update(entity);

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<PmProjectsTypeEditDto>.FailAsync($"======= Exp in Wh whunit : {ex.Message}"));

            }

        }
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
           
            var chk = await permission.HasPermission(256, PermissionType.Delete);
            var chk2 = await permission.HasPermission(410, PermissionType.Delete);

            if (chk == false && chk2 == false)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var items = await pMServiceManager.PMProjectsTypeService.Remove(id);

                return Ok(items);
            }
            catch (Exception ex)
            {

                return Ok(await Result<PmProjectsTypeDto>.FailAsync($"======= Exp in Wh whunit delete : {ex.Message}"));
            }

        }   
        
        [HttpGet("GetByParent")]
        public async Task<IActionResult> GetByParent(int parentId)
        {
            var chk = await permission.HasPermission(256, PermissionType.Show);
            var chk2 = await permission.HasPermission(410, PermissionType.Show);
            if (chk == false && chk2 == false)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var items = await pMServiceManager.PMProjectsTypeService.GetAllVW(x=>x.ParentId == parentId && x.IsDeleted == false);

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<List<PmProjectsTypeVw>>.FailAsync($"======= Exp in Wh whunit delete : {ex.Message}"));

            }

        }


        [HttpGet("Filter")]
        public async Task<IActionResult> Filter(string typeName)
        {
            var chk = await permission.HasPermission(256, PermissionType.Show);
            var chk2 = await permission.HasPermission(410, PermissionType.Show);
            if (chk == false && chk2 == false)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var items = await pMServiceManager.PMProjectsTypeService.GetAllVW(x=>x.TypeName.Contains(typeName) && x.IsDeleted == false );

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<List<PmProjectsTypeVw>>.FailAsync($"======= Exp in Wh whunit delete : {ex.Message}"));

            }

        }

        
        [HttpPost("SearchByParent")]
        public async Task<IActionResult> SearchByParent(PmProjectTypeSearch entity)
        {
            var chk = await permission.HasPermission(256, PermissionType.Show);
            var chk2 = await permission.HasPermission(410, PermissionType.Show);
            if (chk == false && chk2 == false)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                if(entity.ParentId <=0) return Ok(await Result<List<PmProjectsTypeVw>>.FailAsync($"يجب اختيار التصنيف الرئيسي"));
                var items = await pMServiceManager.PMProjectsTypeService.GetAllVW(x=>x.TypeName.Contains(entity.TypeName??"") && x.IsDeleted == false && x.ParentId==entity.ParentId&& x.Id != entity.ParentId);

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<List<PmProjectsTypeVw>>.FailAsync($"======= Exp in Wh whunit delete : {ex.Message}"));

            }

        }


    }


}
