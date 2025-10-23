using Logix.Application.Common;
using Logix.Application.DTOs.Main;
using Logix.Application.DTOs.PM;
using Logix.Application.DTOs.PM.PmProjects;
using Logix.Application.DTOs.PM.PmProjectsStaff;
using Logix.Application.DTOs.PM.Shared;
using Logix.Application.DTOs.WF;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.PM;
using Logix.Domain.WF;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;

namespace Logix.MVC.LogixAPIs.PM
{
    //  المشاريع حسب الإدارة
    public class PMProjectsByDept2Controller : BasePMApiController
    {
        private readonly IPMServiceManager pMServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;

        private readonly ISysConfigurationHelper sysConfigurationHelper;
        private readonly IWFServiceManager wFServiceManager;


        public PMProjectsByDept2Controller(IPMServiceManager pMServiceManager, IPermissionHelper permission, ICurrentData session , ISysConfigurationHelper sysConfigurationHelper, IWFServiceManager wFServiceManager)
        {
            this.pMServiceManager = pMServiceManager;
            this.permission = permission;
            this.session = session;

            this.sysConfigurationHelper = sysConfigurationHelper;
            this.wFServiceManager = wFServiceManager;
        }

        #region Index Page


        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var chk = await permission.HasPermission(1740, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var FacilityId = session.FacilityId;

                // Fetch Property_Value1
                string propertyValue1 = await sysConfigurationHelper.GetValue(259, 1) ?? "0";

                // Fetch Projects
                var Getprojects = await pMServiceManager.PMProjectsService.GetAllVW(p => p.IsDeleted == false && p.IsSubContract == false && p.FacilityId == FacilityId);
                var projects = Getprojects.Succeeded && Getprojects.Data != null ? Getprojects.Data.AsQueryable() : Enumerable.Empty<PmProjectsVw>().AsQueryable();

                // Fetch Extract Transactions
                var GetExtractTransactions = await pMServiceManager.PmDraftExtractTransactionService.GetAllVW(p => p.IsDeleted == false);
                var ExtractTransactions = GetExtractTransactions.Succeeded && GetExtractTransactions.Data != null ? GetExtractTransactions.Data.AsQueryable() : Enumerable.Empty<PmExtractTransactionsVw>().AsQueryable();

                // Fetch Applications Status
                var GetApplicationsStatus = await wFServiceManager.WfApplicationsStatusService.GetAll(a => a.NewStatusId == 5);
                var ApplicationsStatus = GetApplicationsStatus.Succeeded && GetApplicationsStatus.Data != null ? GetApplicationsStatus.Data.AsQueryable() : Enumerable.Empty<WfApplicationsStatusDto>().AsQueryable();

                // Fetch all project staff
                var AllStaff = await pMServiceManager.PMProjectsStaffService.GetAll(staff => staff.IsDeleted == false && staff.RoleId.HasValue);
                var Staff = AllStaff.Data.AsQueryable();

                // Prepare combined data for each department
                var projectSummary = projects
                    .GroupBy(p => new { p.OwnerDeptId, p.OwnerDeptName, p.OwnerDeptName2 })
                    .Select(g => new
                    {
                        OwnerDeptId = g.Key.OwnerDeptId,
                        OwnerDeptName = g.Key.OwnerDeptName,
                        OwnerDeptName2 = g.Key.OwnerDeptName2,
                        Cnt = g.Count(),
                        EstimatedCost = g.Sum(p => ExtractTransactions
                            .Where(e => e.ProjectId == p.Id &&
                                        (propertyValue1 == "0" ||
                                         (propertyValue1 == "1" && ApplicationsStatus.Any(a => a.ApplicationsId == e.AppId))))
                            .Sum(e => (decimal?)e.Total) ?? 0),
                        ProjectValue = g.Sum(p => p.ProjectValue ?? 0),

                        NotStarted = g.Count(p => p.StatusId == 1),
                        Completed = g.Count(p => p.StatusId == 2),
                        Plan = g.Count(p => p.StatusId == 3 || p.StatusId == 6),
                        Stumbling = g.Count(p => p.StatusId == 5),
                        Late = g.Count(p => p.StatusId == 4),
                        Plan3 = g.Count(p => p.StatusId == 3),
                        Plan6 = g.Count(p => p.StatusId == 6)
                    })
                    .ToList();

                return Ok(await Result<object>.SuccessAsync(projectSummary, "Project summary and statistics fetched successfully"));
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"Error fetching  data, MESSAGE: {ex.Message}"));
            }
        }

        #endregion


    }
}
