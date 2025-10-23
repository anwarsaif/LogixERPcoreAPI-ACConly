using Logix.Application.Common;
using Logix.Application.DTOs.Main;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.Main;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Logix.MVC.LogixAPIs.HR
{
    public class SysDepartmentApiController : BaseHrApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;

        private readonly IAccServiceManager accServiceManager;
        private readonly IPMServiceManager pmServiceManager;
        private readonly IHrServiceManager hrServiceManager;
        private readonly IApiDDLHelper ddlHelper;

        public SysDepartmentApiController(IMainServiceManager mainServiceManager,
            IPermissionHelper permission,
            ILocalizationService localization,
            ICurrentData session,
            IAccServiceManager accServiceManager,
            IPMServiceManager pmServiceManager,
            IHrServiceManager hrServiceManager,
            IApiDDLHelper ddlHelper
            )
        {
            this.mainServiceManager = mainServiceManager;
            this.permission = permission;
            this.session = session;
            this.localization = localization;

            this.accServiceManager = accServiceManager;
            this.pmServiceManager = pmServiceManager;
            this.hrServiceManager = hrServiceManager;
            this.ddlHelper = ddlHelper;
        }


        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var chk = await permission.HasPermission(293, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var items = await mainServiceManager.SysDepartmentService.GetAllVW(c => c.IsDeleted == false);
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<InvestEmployeeDto>.FailAsync($"======= Exp in Search SysDepartmentVw, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Search")]
        public async Task<IActionResult> Search(SysDepartmentFilterDto filter)
        {
            var hasPermission = await permission.HasPermission(293, PermissionType.Show);
            if (!hasPermission)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var branchesList = session.Branches?.Split(',') ?? Array.Empty<string>();

                filter.TypeId ??= 0;
                filter.StatusId ??= 0;
                filter.CatId ??= 0;
                filter.StructureId ??= 0;
                filter.ProjectId ??= 0;
                filter.CustomerId ??= 0;

                var items = await mainServiceManager.SysDepartmentService.GetAllVW(c =>
                    c.IsDeleted == false &&
                    c.FacilityId == session.FacilityId &&
                    (branchesList.Length == 0 || branchesList.Contains(c.BranchId.ToString())) &&
                    (filter.TypeId == 0 || c.TypeId == filter.TypeId) &&
                    (string.IsNullOrEmpty(filter.Name) ||
                        (c.Name != null && c.Name.Contains(filter.Name)) ||
                        (c.Name2 != null && c.Name2.ToLower().Contains(filter.Name.ToLower()))) &&
                    (filter.StatusId == 0 || c.StatusId == filter.StatusId) &&
                    (filter.CatId == 0 || c.CatId == filter.CatId) &&
                    (filter.StructureId == 0 || c.StructureId == filter.StructureId)
                );

                if (!items.Succeeded)
                {
                    return Ok(await Result<object>.FailAsync(items.Status.message));
                }

                if (!items.Data.Any())
                {
                    return Ok(await Result<List<SysDepartmentVw>>
                        .SuccessAsync(new List<SysDepartmentVw>(), localization.GetResource1("NosearchResult")));
                }

                return Ok(await Result<List<SysDepartmentVw>>
                    .SuccessAsync(items.Data.ToList(), ""));
            }
            catch (Exception ex)
            {
                return Ok(await Result<SysDepartmentVw>
                    .FailAsync($"======= Exp in Search SysDepartmentVw, MESSAGE: {ex.Message}"));
            }
        }
        [HttpPost("GetPagination")]
        public async Task<IActionResult> GetPagination([FromBody] SysDepartmentFilterDto filter, int take = Pagination.take, int? lastSeenId = null)
        {
            var chk = await permission.HasPermission(293, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            var branchesList = session.Branches?.Split(',') ?? Array.Empty<string>();
            filter.TypeId ??= 0;
            filter.StatusId ??= 0;
            filter.CatId ??= 0;
            filter.StructureId ??= 0;
            filter.ProjectId ??= 0;
            filter.CustomerId ??= 0;
            try
            {
                var items = await mainServiceManager.SysDepartmentService.GetAllWithPaginationVW(
                    selector: e => e.Id,
                    expression: c => c.IsDeleted == false &&
 c.FacilityId == session.FacilityId &&
 (branchesList.Length == 0 || branchesList.Contains(c.BranchId.ToString())) &&
 (filter.TypeId == 0 || c.TypeId == filter.TypeId) &&
 (string.IsNullOrEmpty(filter.Name) ||
     (c.Name != null && c.Name.Contains(filter.Name)) ||
     (c.Name2 != null && c.Name2.ToLower().Contains(filter.Name.ToLower()))) &&
 (filter.StatusId == 0 || c.StatusId == filter.StatusId) &&
 (filter.CatId == 0 || c.CatId == filter.CatId) &&
 (filter.StructureId == 0 || c.StructureId == filter.StructureId),
                    take: take,
                    lastSeenId: lastSeenId
                );

                if (!items.Succeeded)
                    return Ok(await Result<List<SysDepartmentVw>>.FailAsync(items.Status.message));

                if (items.Data == null || !items.Data.Any())
                    return Ok(await Result<List<SysDepartmentVw>>.SuccessAsync(new List<SysDepartmentVw>()));

                var res = items.Data.OrderBy(x => x.TypeId).ToList();

                var paginatedData = new PaginatedResult<object>
                {
                    Succeeded = items.Succeeded,
                    Data = res,
                    Status = items.Status,
                    PaginationInfo = items.PaginationInfo
                };

                return Ok(paginatedData);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add(SysDepartmentDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(293, PermissionType.Add);
                if (!chk)
                {
                    return Ok(await Result.AccessDenied("AccessDenied"));
                }
                //   نوع الادارة / الموقع
                if (obj.TypeId <= 0) return Ok(await Result<object>.FailAsync(localization.GetHrResource("DeptLocationType")));


                //    اسم الإداره/الموقع بالعربي
                if (string.IsNullOrEmpty(obj.Name)) return Ok(await Result<object>.FailAsync(localization.GetHrResource("DeptLocationArName")));

                //   اسم الإداره/الموقع بالانجليزي

                if (string.IsNullOrEmpty(obj.Name2)) return Ok(await Result<object>.FailAsync(localization.GetHrResource("DeptLocationEnName")));


                var add = await mainServiceManager.SysDepartmentService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<SysDepartmentDto>.FailAsync($"======= Exp in Add SysDepartmentDto, MESSAGE: {ex.Message}"));
            }
        }


        [HttpGet("GetByIdForEdit")]
        public async Task<IActionResult> GetByIdForEdit(long id)
        {
            try
            {
                var chk = await permission.HasPermission(293, PermissionType.Show);
                if (!chk)
                {
                    return Ok(await Result.AccessDenied("AccessDenied"));
                }
                if (id <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetMessagesResource("NoIdInUpdate")));

                var getItem = await mainServiceManager.SysDepartmentService.GetOneVW(s => s.Id == id && s.IsDeleted == false);
                return Ok(getItem);

            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"====== Exp in GetById SysDepartmentDto, MESSAGE: {ex.Message}"));
            }
        }


        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(SysDepartmentEditDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(293, PermissionType.Edit);
                if (!chk)
                {
                    return Ok(await Result.AccessDenied("AccessDenied"));
                }
                //   نوع الادارة / الموقع
                if (obj.TypeId <= 0) return Ok(await Result<object>.FailAsync(localization.GetHrResource("DeptLocationType")));


                //    اسم الإداره/الموقع بالعربي
                if (string.IsNullOrEmpty(obj.Name)) return Ok(await Result<object>.FailAsync(localization.GetHrResource("DeptLocationArName")));

                //   اسم الإداره/الموقع بالانجليزي

                if (string.IsNullOrEmpty(obj.Name2)) return Ok(await Result<object>.FailAsync(localization.GetHrResource("DeptLocationEnName")));



                var update = await mainServiceManager.SysDepartmentService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result<SysDepartmentDto>.FailAsync($"======= Exp in Edit SysDepartmentDto, MESSAGE: {ex.Message}"));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long Id)
        {
            var chk = await permission.HasPermission(28, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var delete = await mainServiceManager.SysDepartmentService.Remove(Id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result<EmployeeFastAddDto>.FailAsync($"======= Exp in Delete SysDepartmentDto, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("DDLDepartmentTypeChanged")]
        public async Task<IActionResult> DDLDepartmentTypeChanged(int TypeId, int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<SysDepartment, int>(d => d.FacilityId == session.FacilityId && d.IsDeleted == false && d.TypeId == TypeId && d.Id != d.ParentId, "Id", lang == 2 ? "Name2" : "Name");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }




        [HttpGet("GetOrganizationChartData")]
        public async Task<IActionResult> GetOrganizationChartData()
        {
            long facilityId = session.FacilityId;

            var departments = await mainServiceManager.SysDepartmentService
                .GetAllVW(x =>
                    x.TypeId == 1 &&
                    x.IsDeleted == false &&
                    x.FacilityId == facilityId
                );

            var result = departments.Data.Select(x => new
            {
                DepId = x.Id,
                EmpName = x.EmpName,
                EmpName2 = x.EmpName2,
                EmpPhoto = x.EmpPhoto?.Replace("~", "") ?? string.Empty,
                DepName = x.Name,
                DepName2 = x.Name2,
                ParentId = x.ParentId,
                EmpCode = x.EmpCode
            }).ToList();
            return Ok(await Result<object>.SuccessAsync(result, ""));
        }

    }

}
