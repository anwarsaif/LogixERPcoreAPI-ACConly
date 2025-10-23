using Logix.Application.Common;
using Logix.Application.DTOs.PM;
using Logix.Application.DTOs.PM.PmProjects;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Services;
using Logix.Application.Wrapper;
using Logix.Domain.PM;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;

namespace Logix.MVC.LogixAPIs.PM
{
    public class PMSharedController : BasePMApiController
    {
        private readonly IPMServiceManager pMServiceManager;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;



        public PMSharedController(IPMServiceManager pMServiceManager, ICurrentData session, ILocalizationService localization)
        {
            this.pMServiceManager = pMServiceManager;
            this.session = session;
            this.localization = localization;
        }

        [HttpGet("GetPMProjectsByCode")]
        public async Task<IActionResult> GetPMProjectsByCode(long ProjectCode)
        {
            try
            {
                return Ok(await pMServiceManager.PMProjectsService.GetPMProjectsByCode(ProjectCode, session.FacilityId));

            }
            catch (Exception ex)
            {
                return Ok(await Result<PMProjectsSimpleInfoDto>.FailAsync($"======= Exp in  : {ex.Message}"));
            }

        }



        [HttpGet("GetAllDeliverableForProject")]
        public async Task<IActionResult> GetAllDeliverableForProject(long Code)
        {
            if (Code == 0)
            {
                return Ok(await Result<ProjectsDeliverablePopUpDto>.SuccessAsync("there is no id passed"));
            }
            try
            {
                var getProject = await pMServiceManager.PMProjectsService.GetOneVW(f => f.Code == Code && f.IsDeleted == false);
                if (!getProject.Succeeded)
                    return Ok(await Result<ProjectsDeliverablePopUpDto>.FailAsync($"{getProject.Status.message}"));

                if (getProject.Data == null) return Ok(await Result<List<ProjectsDeliverablePopUpDto>>.SuccessAsync($"{localization.GetPMResource("projectisnotinprojectlist")}"));

                var GetAllProjectDeliverable = await pMServiceManager.PmProjectsDeliverableService.GetAll(x => x.IsDeleted == false && x.ProjectId == getProject.Data.Id);

                if (!GetAllProjectDeliverable.Succeeded)
                    return Ok(await Result<List<ProjectsDeliverablePopUpDto>>.FailAsync($"{GetAllProjectDeliverable.Status.message}"));

                if (GetAllProjectDeliverable.Data == null) return Ok(await Result<List<ProjectsDeliverablePopUpDto>>.SuccessAsync(new List<ProjectsDeliverablePopUpDto>()));

                var DeliverableIds = GetAllProjectDeliverable.Data.Select(x => x.Id).ToList();


                var TransactionsDetailsResult = await pMServiceManager.PmDeliverableTransactionsDetailService
                    .GetAllVW(d => d.IsDeleted == false && d.ProjectId == getProject.Data.Id && d.DeliverableId.HasValue && DeliverableIds.Contains(d.DeliverableId.Value));

                // If the result retrieval failed, set it to an empty collection instead of exiting
                var transactionDetailsData = TransactionsDetailsResult.Succeeded ? TransactionsDetailsResult.Data : Enumerable.Empty<PmDeliverableTransactionsDetailsVw>();

                var result = GetAllProjectDeliverable.Data.Select(x =>
                    new ProjectsDeliverablePopUpDto
                    {
                        // DeliverableId
                        Id = x.Id,
                        Name = x.Name,
                        ApproveQty = x.Qty,
                        PreviousQty = transactionDetailsData.Where(d => d.DeliverableId == x.Id).Sum(x => x.Qty) ?? 0,
                    }
                    ).ToList();
                return Ok(await Result<List<ProjectsDeliverablePopUpDto>>.SuccessAsync(result));


            }
            catch (Exception exp)
            {
                return Ok(await Result<ProjectsDeliverablePopUpDto>.FailAsync($"{exp.Message}"));
            }
        }

    }
}
