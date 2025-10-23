using Logix.Application.Common;
using Logix.Application.DTOs.PM;
using Logix.Application.DTOs.PM.PmProjectsStage;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.PM;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.PM.ViewModel.PmProjectsStageVMFilter;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace Logix.MVC.LogixAPIs.PM
{
    public class PMProjectsStageApiController : BasePMApiController
    {
        private readonly IPMServiceManager pMServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;

        public PMProjectsStageApiController(IPMServiceManager pMServiceManager, IPermissionHelper permission, ICurrentData session)
        {
            this.pMServiceManager = pMServiceManager;
            this.permission = permission;
            this.session = session;
        }
 
        [HttpPost("Search")]
        public async Task<IActionResult> Search(PmProjectsStageFilter filter)
        {
            var chk = await permission.HasPermission(1932, PermissionType.Show);

            if (!chk )
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {

                var items = await pMServiceManager.PMProjectsStageService.GetAllVW(e =>
                e.IsDeleted == false
               && (e.Name ?? "").Contains(filter.Name ?? "")
                 
                 && e.FacilityId == session.FacilityId
                 &&e.ParentId ==e.Id

                 );
               

                
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    res = res.OrderBy(e => e.Id);
                    if (filter.Id!=0 )
                    {
                        res = res.Where(res=>res.Id==filter.Id);
                    }
                    return Ok(await Result<List<PmProjectsStagesVw>>.SuccessAsync(res.ToList(),items.Status.message));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<List<PmProjectsStagesVw>>.FailAsync($"======= Exp in Wh whunit : {ex.Message}"));

            }
        }
   /*          [HttpGet("GetAllParentVM")]
        public async Task<IActionResult> GetAllParentVM()
        {
            var chk = await permission.HasPermission(1932, PermissionType.Show);

            if (!chk )
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {

                var items = await pMServiceManager.PMProjectsStageService.GetAllVW(e => e.IsDeleted == false && e.Id == e.ParentId  && e.FacilityId == session.FacilityId);
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    res = res.OrderBy(e => e.Id);
                    return Ok(await Result<List<PmProjectsStagesVw>>.SuccessAsync(res.ToList(),items.Status.message));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<List<PmProjectsStagesVw>>.FailAsync($"======= Exp in Wh whunit : {ex.Message}"));

            }
        }
*/
        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(long id)
        {
            var chk = await permission.HasPermission(1932, PermissionType.Show);

            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var items = await pMServiceManager.PMProjectsStageService.GetOneVW(x =>x.Id==id&& x.FacilityId == session.FacilityId && x.IsDeleted == false);

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<PmProjectsStagesVw>.FailAsync($"======= Exp in Wh whunit delete : {ex.Message}"));
            }
        }


        [HttpPost("Add")]
        public async Task<IActionResult> Add(PmProjectsStageAddDto entity)
        {

            var chk = await permission.HasPermission(1932, PermissionType.Add);
        
            if (!chk )
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                entity.FacilityId = session.FacilityId;
             
                    
                var items = await pMServiceManager.PMProjectsStageService.Add(entity);

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<PmProjectsTypeDto>.FailAsync($"======= Exp in Wh whunit : {ex.Message}"));

            }

        } 
  /*      [HttpPost("AddSubStage")]
        public async Task<IActionResult> AddSubStage(PmProjectsStageAddDto entity)
        {

            var chk = await permission.HasPermission(1932, PermissionType.Add);
        
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                entity.FacilityId = session.FacilityId;
             
                var items = await pMServiceManager.PMProjectsStageService.Add(entity);

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<PmProjectsTypeDto>.FailAsync($"======= Exp in Wh whunit : {ex.Message}"));

            }

        }
*/


        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(PmProjectsStageEditDto entity)
        {
            var chk = await permission.HasPermission(1932, PermissionType.Edit);
     
            if(!chk) { 
           
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var items = await pMServiceManager.PMProjectsStageService.Update(entity);

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<PmProjectsStageEditDto>.FailAsync($"======= Exp in Wh whunit : {ex.Message}"));

            }

        }
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
           
            var chk = await permission.HasPermission(1932, PermissionType.Delete);
        
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var items = await pMServiceManager.PMProjectsStageService.Remove(id);

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<PmProjectsStageDto>.FailAsync($"======= Exp in Wh whunit delete : {ex.Message}"));
            }

        }   
        
/* 
        [HttpPost("FilterStage")]
        public async Task<IActionResult> FilterStage(PmProjectsStageFilter filter)
        {
            var chk = await permission.HasPermission(1932, PermissionType.Show);
         
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
             
                var items = await pMServiceManager.PMProjectsStageService.GetAllVW(x=>x.IsDeleted == false );


                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    if (filter.Id != 0)
                    {
                        res = res.Where(x => x.Id == filter.Id);

                    }    
                    if (filter.Name is null)
                    {
                        res = res.Where(x => x.Name.Contains(filter.Name)|| x.Name2.Contains(filter.Name));

                    }
                }

                    return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<List<PmProjectsStagesVw>>.FailAsync($"======= Exp in Wh whunit delete : {ex.Message}"));

            }
             
        } */
        [HttpPost("SearchByParent")]
        public async Task<IActionResult> SearchByParent(PmProjectsSubStageFilter filter)
        {
            var chk = await permission.HasPermission(1932, PermissionType.Show);
         
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var items = await pMServiceManager.PMProjectsStageService.GetAllVW(x=>x.IsDeleted == false && x.Id!=x.ParentId &&x.ParentId ==filter.ParentId && x.FacilityId==session.FacilityId );


                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    if (filter.Id != 0)
                    {
                        res = res.Where(x => x.Id == filter.Id);

                    } 
                    if (filter.ParentId != 0)
                    {
                        res = res.Where(x => x.ParentId == filter.ParentId);

                    }    
                    if (filter.Name is null)
                    {
                        res = res.Where(x => x.Name.Contains(filter.Name)|| x.Name2.Contains(filter.Name));

                    }

                    return Ok(await Result<List<PmProjectsStagesVw>>.SuccessAsync(res.ToList(), items.Status.message));

                }

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<List<PmProjectsStagesVw>>.FailAsync($"======= Exp in Wh whunit delete : {ex.Message}"));

            }

        }



    }


}
