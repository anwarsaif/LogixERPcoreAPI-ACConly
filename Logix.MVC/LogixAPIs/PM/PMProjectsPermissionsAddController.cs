using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.DTOs.PM.PmProjectsStaff;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Services;
using Logix.Application.Wrapper;
using Logix.Domain.PM;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.PM.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;

namespace Logix.MVC.LogixAPIs.PM
{
    // الصلاحيات على المشاريع
    public class PMProjectsPermissionsAddController : BasePMApiController
    {
        private readonly IPMServiceManager pMServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly ISalServiceManager salServiceManager;

        public PMProjectsPermissionsAddController(IPMServiceManager pMServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, ISalServiceManager salServiceManager)
        {
            this.pMServiceManager = pMServiceManager;
            this.permission = permission;
            this.session = session;
            this.localization = localization;
            this.salServiceManager = salServiceManager;
        }


        [HttpPost("Search")]
        public async Task<IActionResult> Search(ProjectsPermissionsFilter filter)
        {
            var chk = await permission.HasPermission(1810, PermissionType.Show);

            if (!chk)
                return Ok(await Result.AccessDenied("AccessDenied"));

            try
            {
                filter.ProjectCodeFrom ??= 0;
                filter.ProjectCodeTo ??= 0;
                var barnchId = session.Branches;

                var BranchesList = session.Branches.Split(',');
                var items = await pMServiceManager.PMProjectsService.GetAllVW(e => e.IsDeleted == false
                && e.SystemId == 5
                && e.IsSubContract == false
                && BranchesList.Contains(e.BranchId.ToString())
                && (string.IsNullOrEmpty(filter.ProjectManagerCode) || (e.EmpId != null && filter.ProjectManagerCode == e.EmpId))
                && (string.IsNullOrEmpty(filter.ProjectManagerName) || (e.EmpName != null && e.EmpName.Contains(filter.ProjectManagerName)))
                && (string.IsNullOrEmpty(filter.ProjectName) || ((e.Name != null && e.Name.Contains(filter.ProjectName)) || (e.Name2 != null && e.Name2.Contains(filter.ProjectName))))
                );
                if (!items.Succeeded)
                    return Ok(await Result<object>.FailAsync(items.Status.message));
                if (!items.Data.Any())
                    return Ok(await Result<object>.SuccessAsync(new List<object>(), localization.GetResource1("NosearchResult")));

                var res = items.Data.AsQueryable();
                res = res.OrderBy(e => e.Id);
                if (filter.ProjectCodeFrom != 0 && filter.ProjectCodeTo == 0)
                {
                    res = res.Where(x => x.Code == filter.ProjectCodeFrom || x.No == filter.ProjectCodeFrom.ToString());
                }
                else if (filter.ProjectCodeTo != 0 && filter.ProjectCodeFrom != 0)
                {
                    res = res.Where(x => x.Code >= filter.ProjectCodeFrom && x.Code <= filter.ProjectCodeTo);
                }
                if (!string.IsNullOrEmpty(filter.FromDate) && !string.IsNullOrEmpty(filter.ToDate))
                {
                    var DateFrom = DateHelper.StringToDate(filter.FromDate);
                    var DateTo = DateHelper.StringToDate(filter.ToDate);
                    res = res.Where(x => DateHelper.StringToDate(x.DateG) >= DateFrom && DateHelper.StringToDate(x.DateG) <= DateTo);
                }
                if (!res.Any())
                    return Ok(await Result<object>.SuccessAsync(new List<object>(), localization.GetResource1("NosearchResult")));
                var ProjectsStaff = await pMServiceManager.PMProjectsStaffService.GetAllVW(e => e.IsDeleted == false);
                var result = res.Select(p => new
                {
                    p.Id,
                    p.Code,
                    p.Name,
                    p.ProjectStart,
                    p.ProjectEnd,
                    ProjectsStaff = ProjectsStaff.Data.Where(x => x.ProjectId == p.Id).ToList()
                }).ToList();

                return Ok(await Result<object>.SuccessAsync(result, ""));

            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"======= Exp in {this.GetType()}: {ex.Message}"));

            }
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add(PMProjectsStaffAddDto obj)
        {

            var chk = await permission.HasPermission(1810, PermissionType.Add);

            if (chk == false)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {

                if (string.IsNullOrEmpty(obj.EmpCode))
                    return Ok(await Result<object>.SuccessAsync(localization.GetResource1("EmployeeIsNumber")));
                
                if (obj.ProjectIds.Count<=0)
                    return Ok(await Result<object>.SuccessAsync("المشروع"));
                
                if (obj.RoleId<=0)
                    return Ok(await Result<object>.SuccessAsync("مستوى الصلاحيات "));

                var add = await pMServiceManager.PMProjectsStaffService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"======= Exp in Wh whunit : {ex.Message}"));

            }

        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {

            var chk = await permission.HasPermission(1810, PermissionType.Delete);
            if (!chk)
                return Ok(await Result.AccessDenied("AccessDenied"));

            try
            {
                var items = await pMServiceManager.PMProjectsStaffService.Remove(id);

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"======= Exp in {this.GetType()}: {ex.Message}"));
            }

        }


    }

}
