using Logix.Application.Common;
using Logix.Application.DTOs.Main;
using Logix.Application.DTOs.PM;
using Logix.Application.DTOs.PM.PmProjects;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Services;
using Logix.Application.Wrapper;
using Logix.Domain.PM;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.PM.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Logix.MVC.LogixAPIs.PM
{

    public class PmProjectPlanController : BasePMApiController
    {
        private readonly IPMServiceManager pmServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;

        public PmProjectPlanController(IPMServiceManager pMServiceManager,
            IPermissionHelper permission,
            ICurrentData session,
            ILocalizationService localization)
        {
            this.pmServiceManager = pMServiceManager;
            this.permission = permission;
            this.session = session;
            this.localization = localization;
        }

        [HttpPost("Search")]
        public async Task<IActionResult> Search(PmProjectPlanFilterDto filter)
        {
            try
            {
                var chk = await permission.HasPermission(1420, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                filter.Id ??= 0; filter.PlanType ??= 0; filter.StatusId ??= 0; filter.ProjectCode ??= 0;
                var items = await pmServiceManager.PMProjectPlanService.GetAllVW(p => p.IsDeleted == false
                    && (filter.Id == 0 || p.Id == filter.Id)
                    && (filter.StatusId == 0 || p.StatusId == filter.StatusId)
                    && (filter.PlanType == 0 || p.PlanType == filter.PlanType)
                    && (filter.ProjectCode == 0 || p.ProjectCode == filter.ProjectCode)
                );
                if (items.Succeeded)
                {
                    var res = items.Data;

                    if (!(string.IsNullOrEmpty(filter.StartDate) && string.IsNullOrEmpty(filter.EndDate)))
                    {
                        DateTime startDate = DateHelper.StringToDate(filter.StartDate);
                        DateTime endDate = DateHelper.StringToDate(filter.EndDate);

                        res = res.Where(r => DateHelper.StringToDate(r.PlanDate) >= startDate && DateHelper.StringToDate(r.PlanDate) <= endDate);
                    }

                    List<PmProjectPlanFilterDto> result = new();
                    foreach (var item in res)
                    {
                        PmProjectPlanFilterDto obj = new()
                        {
                            Id = item.Id,
                            PlanDate = item.PlanDate,
                            Subject = item.Subject,
                            ProjectName = item.ProjectName,
                            StartDate = item.StartDate,
                            EndDate = item.EndDate,
                            PlanTypeName = item.PlanTypeName,
                            StatusName = item.StatusName,
                            Note = item.Note,
                            TypeId = item.TypeId
                        };
                        result.Add(obj);
                    }
                    return Ok(await Result<List<PmProjectPlanFilterDto>>.SuccessAsync(result));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"======= Exp: {ex.Message}"));
            }
        }


        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var chk = await permission.HasPermission(1420, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await pmServiceManager.PMProjectPlanService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }



        [HttpPost("Add")]
        public async Task<IActionResult> Add(PmProjectPlanAddDto entity)
        {
            try
            {
                var chk = await permission.HasPermission(1466, PermissionType.Add);
                if (chk == false)
                {
                    return Ok(await Result.AccessDenied("AccessDenied"));
                }

                var items = await pmServiceManager.PMProjectPlanService.Add(entity);
                return Ok(items);
         
            }
            catch (Exception ex)
            {
                return Ok(await Result<PMProjectsDto>.FailAsync($"======= Exp in  : {ex.Message}"));
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
   
    }



}
