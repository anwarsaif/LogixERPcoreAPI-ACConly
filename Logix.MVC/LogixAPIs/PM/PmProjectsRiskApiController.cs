using AutoMapper;
using Logix.Application.Common;
using Logix.Application.DTOs.Main;
using Logix.Application.DTOs.PM;
using Logix.Application.DTOs.PM.PmProjects;
using Logix.Application.DTOs.PM.PmProjectsStaff;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.PM;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.PM
{
    public class PmProjectsRiskApiController : BasePMApiController
    {
        private readonly IPMServiceManager pmServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly IMainServiceManager mainServiceManager;
        private readonly IMapper mapper;

        public PmProjectsRiskApiController(IPMServiceManager pMServiceManager,
            IPermissionHelper permission,
            ICurrentData session,
            IMainServiceManager mainServiceManager,
            IMapper mapper
            )
        {
            this.pmServiceManager = pMServiceManager;
            this.permission = permission;
            this.session = session;
            this.mainServiceManager = mainServiceManager;
            this.mapper = mapper;
        }

        [HttpPost("Search")]
        public async Task<IActionResult> Search(PmProjectsRiskFilterDto filter)
        {
            var chk = await permission.HasPermission(1466, PermissionType.Show);


            if (chk == false)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {


                // var tempVm = await pMServiceManager.PMProjectsService.GetAllVW(x=>x.IsDeleted==false);
                var tempVm = await pmServiceManager.PMProjectsRiskService.GetAllQuaryVW(filter);

                return Ok(tempVm);
            }
            catch (Exception ex)
            {
                return Ok(await Result<List<PmProjectsRisksVw>>.FailAsync($"======= Exp in Wh whunit : {ex.Message}"));

            }
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add(PmProjectsRiskAddDto entity)
        {

            var chk = await permission.HasPermission(1466, PermissionType.Add);


            if (chk == false)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {

                var items = await pmServiceManager.PMProjectsRiskService.Add(entity);

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<PMProjectsDto>.FailAsync($"======= Exp in  : {ex.Message}"));

            }

        }
        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(PmProjectsRiskEditDto entity)
        {
            var chk = await permission.HasPermission(1466, PermissionType.Edit);


            if (chk == false)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }


            try
            {

                var items = await pmServiceManager.PMProjectsRiskService.Update(entity);

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
            var chk = await permission.HasPermission(1466, PermissionType.Show);

            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (Id <= 0)
            {
                return Ok(await Result<PmProjectsRiskEditDto>.FailAsync($"not found this project"));

            }
            try
            {
                var responce = await pmServiceManager.PMProjectsRiskService.GetOneVW(x => x.Id == Id);
                //var items = await pMServiceManager.PMProjectsService.GetForUpdate<PmProjectsRiskEditDto>(Id);
                /*  if (items.Succeeded)
                  {

                  }
  */
                if (responce != null && responce.Succeeded == true)
                {
                    var temnp = responce.Data;
                    if (temnp != null)
                    {
                        var editObject = new PmProjectsRiskEditDto
                        {
                            Id = temnp.Id,
                            ActionsPlans = temnp.ActionsPlans,
                            Date = temnp.Date,
                            Description = temnp.Description,
                            Details = temnp.Details,
                            Effect = temnp.Effect,
                            EmpCode = temnp.EmpCode,
                            EmpName = temnp.EmpName,
                            Impact = temnp.Impact,
                            PlannedDate = temnp.PlannedDate,
                            ProjectCode = temnp.ProjectCode,
                            ProjectId = temnp.ProjectId,
                            ProjectName = temnp.ProjectName,
                            RequiredSupport = temnp.RequiredSupport,
                            RiskRate = temnp.RiskRate,
                            Significance = temnp.Significance,
                            StatusId = temnp.StatusId,
                            TypeId = temnp.TypeId,

                        };
                        var fileList = await mainServiceManager.SysFileService.GetAll(x => x.TableId == 77 && x.PrimaryKey == editObject.Id && x.FacilityId == session.FacilityId && x.IsDeleted == false);

                        if (fileList != null && fileList.Succeeded)
                        {
                            foreach (var file in fileList.Data)
                            {
                                editObject.FileList.Add(new SaveFileDto
                                {
                                    Id = file.Id,
                                    FileDate = file.FileDate,
                                    FileName = file.FileName ?? "",
                                    FileURL = file.FileUrl ?? "",
                                    IsDeleted = file.IsDeleted,

                                });
                            }

                          
                        }
                        return Ok(Result<PmProjectsRiskEditDto>.Success(editObject));
                    }




                }


                return Ok(responce);
            }
            catch (Exception ex)
            {
                return Ok(await Result<PmProjectsRiskEditDto>.FailAsync($"======= Exp in  : {ex.Message}"));

            }
        }


        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long Id)
        {

            var chk = await permission.HasPermission(1466, PermissionType.Delete);

            if (chk == false)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var items = await pmServiceManager.PMProjectsRiskService.Remove(Id);

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<PMProjectsDto>.FailAsync($"======= Exp in Wh whunit delete : {ex.Message}"));
            }

        }

        [HttpGet("GetPMProjectsByCode")]
        public async Task<IActionResult> GetPMProjectsByCode(long ProjectCode)
        {
            try
            {
                return Ok(await pmServiceManager.PMProjectsService.GetPMProjectsByCode(ProjectCode, session.FacilityId));

            }
            catch (Exception ex)
            {
                return Ok(await Result<PMProjectsSimpleInfoDto>.FailAsync($"======= Exp in  : {ex.Message}"));
            }

        }
        [HttpGet("GetAddInit")]
        public async Task<IActionResult> GetAddInit()
        {
            try
            {
                return Ok(await pmServiceManager.PMProjectsService.GetPMProjectsByCode(2,session.FacilityId));

            }
            catch (Exception ex)
            {
                return Ok(await Result<PMProjectsSimpleInfoDto>.FailAsync($"======= Exp in  : {ex.Message}"));
            }

        }

        /*   [HttpGet("GetEffectWithColor")]
           public async Task<IActionResult> GetEffectWithColor()
           {
               try
               {
                   return Ok(await pMServiceManager.PM(2));

               }
               catch (Exception ex)
               {
                   return Ok(await Result<PMProjectsSimpleInfoDto>.FailAsync($"======= Exp in  : {ex.Message}"));
               }

           }*/




    }


}
