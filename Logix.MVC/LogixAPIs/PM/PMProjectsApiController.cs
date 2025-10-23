using AutoMapper;
using Logix.Application.Common;
using Logix.Application.DTOs.Common;
using Logix.Application.DTOs.PM;
using Logix.Application.DTOs.PM.PmProjects;
using Logix.Application.DTOs.PM.PmProjectsStaff;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Services;
using Logix.Application.Wrapper;
using Logix.Domain.PM;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Logix.MVC.LogixAPIs.PM
{
    public class PMProjectsApiController : BasePMApiController
    {
        private readonly IPMServiceManager pMServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ITsServiceManager tsServiceManager;
        private readonly IMainServiceManager mainServiceManager;
        private readonly IMapper mapper;

        public PMProjectsApiController(IPMServiceManager pMServiceManager, 
            IPermissionHelper permission, 
            ICurrentData session,
            ITsServiceManager tsServiceManager,
            IMainServiceManager mainServiceManager,
            IMapper mapper
            )
        {
            this.pMServiceManager = pMServiceManager;
            this.permission = permission;
            this.session = session;
            this.tsServiceManager = tsServiceManager;
            this.mainServiceManager = mainServiceManager;
            this.mapper = mapper;
        }
   
        [HttpPost("Search")]
        public async Task<IActionResult> Search(PmProjectSearchFilterVm filter)
        {
            var chk = await permission.HasPermission(240, PermissionType.Show);
            var chk2 = await permission.HasPermission(378, PermissionType.Show);
          
            if (chk ==false && chk2==false )
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {


               // var tempVm = await pMServiceManager.PMProjectsService.GetAllVW(x=>x.IsDeleted==false);
               var tempVm = await pMServiceManager.PMProjectsService.GetAllQuaryVW(filter);

                return Ok(tempVm);
            }
            catch (Exception ex)
            {
                return Ok(await Result<List<PmProjectsVw>>.FailAsync($"======= Exp in Wh whunit : {ex.Message}"));

            }
        }
          [HttpPost("SearchByEmp")]
        public async Task<IActionResult> SearchByEmp(PmProjectSearchFilterVm filter)
        {
            var chk = await permission.HasPermission(795, PermissionType.Show);
          
            if (chk ==false )
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {


               var tempVm = await pMServiceManager.PMProjectsService.GetAllQuaryVWByEmp(filter);

           
                return Ok(tempVm);
            }
            catch (Exception ex)
            {
                return Ok(await Result<List<PmProjectsVw>>.FailAsync($"======= Exp in Wh whunit : {ex.Message}"));

            }
        }
   
       [HttpPost("Add")]
        public async Task<IActionResult> Add(PMProjectsAddDto entity)
        {

            var chk = await permission.HasPermission(240, PermissionType.Add);
          var chk2 = await permission.HasPermission(378, PermissionType.Add);
         
            if (chk == false && chk2==false )
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
              
                var items = await pMServiceManager.PMProjectsService.Add(entity);

                return Ok(items);
            }
            catch (Exception ex)    
            {
                return Ok(await Result<PMProjectsDto>.FailAsync($"======= Exp in  : {ex.Message}"));

            }

        }
       [HttpPost("Edit")]
        public async Task<IActionResult> Edit(PMProjectsEditDto entity)
        {
            var chk = await permission.HasPermission(240, PermissionType.Edit);
            var chk2 = await permission.HasPermission(378, PermissionType.Edit);

            if (chk == false && chk2 == false)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

         
            try
            {
              
                var items = await pMServiceManager.PMProjectsService.Update(entity);

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<PMProjectsDto>.FailAsync($"======= Exp in  : {ex.Message}"));

            }

        }


        [HttpGet("GetByIdForEdit")]
        public async Task<IActionResult> GetByIdForEdit(long Id)
        {
            var chk = await permission.HasPermission(899, PermissionType.Show);

            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if(Id <= 0)
            {
                return Ok(await Result<PMProjectsDto>.FailAsync($"not found this project"));

            }
            try
            {

                var items = await pMServiceManager.PMProjectsService.GetForEditById(Id);


                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<PMProjectsDto>.FailAsync($"======= Exp in  : {ex.Message}"));

            }
        }

        #region task 
        [HttpPost("AddProjectTask")]
        public async Task<IActionResult> AddProjectTask(PMProjectTaskDto entity)
        {

            var chk = await permission.HasPermission(256, PermissionType.Add);

            if (chk == false)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {

                var items = await pMServiceManager.PMProjectsService.AddProjectTask(entity);

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<PMProjectTaskDto>.FailAsync($"======= Exp in  : {ex.Message}"));

            }

        }
        [HttpGet("DeleteProjectTask")]
        public async Task<IActionResult> DeleteProjectTask(long Id)
        {

            var chk = await permission.HasPermission(256, PermissionType.Delete);

            if (chk == false)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var items = await tsServiceManager.TsTaskService.Remove(Id);

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<PMProjectTaskDto>.FailAsync($"======= Exp in Wh whunit delete : {ex.Message}"));
            }

        }
        #endregion  end task

        #region Stokeholder 
        [HttpPost("AddProjectStokeholder")]
        public async Task<IActionResult> AddProjectStokeholder(PMProjectsStokeholderDto entity)
        {

            var chk = await permission.HasPermission(256, PermissionType.Add);

            if (chk == false)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                entity.InternalOrExternal = 1;
                var items = await pMServiceManager.PMProjectsStokeholderService.Add(entity);

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<PMProjectsStokeholderDto>.FailAsync($"======= Exp in  : {ex.Message}"));

            }

        }
        [HttpGet("DeleteProjectStokeholder")]
        public async Task<IActionResult> DeleteProjectStokeholder(long Id)
        {

            var chk = await permission.HasPermission(256, PermissionType.Delete);

            if (chk == false)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var items = await pMServiceManager.PMProjectsStokeholderService.Remove(Id);

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<PMProjectsStokeholderDto>.FailAsync($"======= Exp in Wh whunit delete : {ex.Message}"));
            }

        }
        #endregion  end Stokeholder

        #region AssigneeToUser 
        [HttpPost("AddAssigneeToUser")]
        public async Task<IActionResult> AddAssigneeToUser(PMProjectsStaffAddDto entity)
        { 

            var chk = await permission.HasPermission(256, PermissionType.Add);

            if (chk == false)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
           
                var items = await pMServiceManager.PMProjectsStaffService.Add(entity);

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<PMProjectsStaffAddDto>.FailAsync($"======= Exp in  : {ex.Message}"));

            }

        }
        [HttpGet("DeleteAssigneeToUser")]
        public async Task<IActionResult> DeleteAssigneeToUser(long ProjectId,long EmpId)
        {

            var chk = await permission.HasPermission(256, PermissionType.Delete);

            if (chk == false)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {

                var items = await pMServiceManager.PMProjectsStaffService.RemoveByProject(ProjectId,EmpId);

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<PMProjectsStaffAddDto>.FailAsync($"======= Exp in Wh whunit delete : {ex.Message}"));
            }

        }
        #endregion  end AssigneeToUser

        #region AddProjectItem 
        [HttpPost("AddProjectItem")]
        public async Task<IActionResult> AddProjectItem(PMProjectsItemDto entity)
        {

            var chk = await permission.HasPermission(256, PermissionType.Add);

            if (chk == false)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                if (entity.Id > 0)
                {
                    // mapper.Map<PmProjectsInstallmentEditDto>(entity);
                    var itemEdit = await pMServiceManager.PMProjectsItemService.Update(mapper.Map<PMProjectsItemEditDto>(entity));
                    return Ok(itemEdit);
                }
                entity.ItemId = 0;
                entity.Total = 0;

                var items = await pMServiceManager.PMProjectsItemService.Add(entity);

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<PMProjectsItemDto>.FailAsync($"======= Exp in  : {ex.Message}"));

            }

        }
        [HttpGet("DeleteProjectItem")]
        public async Task<IActionResult> DeleteProjectItem(long Id)
        {

            var chk = await permission.HasPermission(256, PermissionType.Delete);

            if (chk == false)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var items = await pMServiceManager.PMProjectsItemService.Remove(Id);

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<PMProjectsItemDto>.FailAsync($"======= Exp in Wh whunit delete : {ex.Message}"));
            }

        }
        #endregion  end AddProjectItem

        #region  ProjectsInstallment
        [HttpPost("AddProjectsInstallment")]
        public async Task<IActionResult> AddProjectsInstallment(PMProjectsInstallmentDto entity)
        {

            var chk = await permission.HasPermission(256, PermissionType.Add);

            if (chk == false)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                if (entity.Id > 0)
                {
                   // mapper.Map<PmProjectsInstallmentEditDto>(entity);
                    var itemEdit = await pMServiceManager.PMProjectsInstallmentService.Update(mapper.Map<PmProjectsInstallmentEditDto>(entity));
                    return Ok(itemEdit);
                }
                var items = await pMServiceManager.PMProjectsInstallmentService.Add(entity);

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<PMProjectsItemDto>.FailAsync($"======= Exp in  : {ex.Message}"));

            }

        }
        [HttpGet("DeleteProjectsInstallment")]
        public async Task<IActionResult> DeleteProjectsInstallment(long Id)
        {

            var chk = await permission.HasPermission(256, PermissionType.Delete);

            if (chk == false)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var items = await pMServiceManager.PMProjectsInstallmentService.Remove(Id);

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<PMProjectsItemDto>.FailAsync($"======= Exp in Wh whunit delete : {ex.Message}"));
            }

        }
        #endregion  end AddProjectItem
        #region  ProjectsAddDeduc الاضافات والخصومات  
        [HttpPost("AddProjectsAddDeduc")]
        public async Task<IActionResult> AddProjectsAddDeduc(PmProjectsAddDeducDto entity)
        {
            entity.Debit = 0;

            var chk = await permission.HasPermission(256, PermissionType.Add);

            if (chk == false)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                
                var items = await pMServiceManager.PMProjectsAddDeducService.Add(entity);

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<PMProjectsItemDto>.FailAsync($"======= Exp in  : {ex.Message}"));

            }

        }     
        [HttpPost("AddDeducProjects")]
        public async Task<IActionResult> AddDeducProjects(PmProjectsAddDeducDto entity)
        {
            //  الخصومات
            entity.Credit = 0;

            var chk = await permission.HasPermission(256, PermissionType.Add);

            if (chk == false)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                
                var items = await pMServiceManager.PMProjectsAddDeducService.Add(entity);

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<PMProjectsItemDto>.FailAsync($"======= Exp in  : {ex.Message}"));

            }

        }


        [HttpGet("DeleteProjectsAddDeduc")]
        public async Task<IActionResult> DeleteProjectsAddDeduc(long Id)
        {

            var chk = await permission.HasPermission(256, PermissionType.Delete);

            if (chk == false)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var items = await pMServiceManager.PMProjectsAddDeducService.Remove(Id);

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<PMProjectsItemDto>.FailAsync($"======= Exp in Wh whunit delete : {ex.Message}"));
            }

        }
        #endregion  end AddProjectItem

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {

            var chk = await permission.HasPermission(256, PermissionType.Delete);

            if (chk == false )
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var items = await pMServiceManager.PMProjectsService.Remove(id);

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<PMProjectsDto>.FailAsync($"======= Exp in Wh whunit delete : {ex.Message}"));
            }

        }   
       

    }


}
