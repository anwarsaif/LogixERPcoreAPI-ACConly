using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Logix.MVC.LogixAPIs.HR
{
    //  مؤشرات الاداء
    public class HRPerformanceIndicatorsApiController : BaseHrApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly IDDListHelper listHelper;
        public HRPerformanceIndicatorsApiController(IHrServiceManager hrServiceManager, IMainServiceManager mainServiceManager, IPermissionHelper permission, ICurrentData session, IDDListHelper listHelper)
        {
            this.mainServiceManager = mainServiceManager;
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.listHelper = listHelper;   
        }

        [NonAction]
        private void setErrors()
        {
            var errors = new ErrorsHelper(ModelState);
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var chk = await permission.HasPermission(1682, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var items = await hrServiceManager.HrCompetenceService.GetAll(e => e.IsDeleted == false && e.TypeId == 2);
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    res = res.OrderBy(e => e.Id);
                    return Ok(await Result<List<HrCompetenceDto>>.SuccessAsync(res.ToList(), items.Status.message));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrCompetencesVw filter)
        {
            var chk = await permission.HasPermission(1682, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var items = await hrServiceManager.HrCompetenceService.GetAllVW(e => e.IsDeleted == false&&e.TypeId==2);
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    if (filter == null)
                    {
                        return Ok(items);
                    }

                    if (filter.CatId != null && filter.CatId > 0)
                    {
                        res = res.Where(c => c.CatId != null && c.CatId.Equals(filter.CatId));
                    }
                    
                    if (filter.KpiTypeId != null && filter.KpiTypeId > 0)
                    {
                        res = res.Where(c => c.KpiTypeId != null && c.KpiTypeId.Equals(filter.KpiTypeId));
                    }
                    
                    if (filter.MethodId != null && filter.MethodId > 0)
                    {
                        res = res.Where(c => c.MethodId != null && c.MethodId.Equals(filter.MethodId));
                    }
                    if (!string.IsNullOrEmpty(filter.Name))
                    {
                        res = res.Where(c => (c.Name != null && c.Name.Contains(filter.Name)));
                    }
                    if (filter.Score != null && filter.Score!=0)
                    {
                        res = res.Where(s => s.Score != null && s.Score.Equals(filter.Score));
                    }

                    res = res.OrderBy(e => e.Id);
                    var final = res.ToList();
                    return Ok(await Result<List<HrCompetencesVw>>.SuccessAsync(final, ""));
                }

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add(HrCompetenceDto obj)
        {
            var chk = await permission.HasPermission(1682, PermissionType.Add);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            setErrors();
            if (!ModelState.IsValid)
            {
                return Ok(obj);
            }
            try
            {
                obj.TypeId = 2;
                var addRes = await hrServiceManager.HrCompetenceService.Add(obj);
                if (addRes.Succeeded)
                {
                    return Ok(addRes);
                }

                else
                {
                    return Ok(await Result<HrCompetenceDto>.FailAsync(addRes.Status.message));
                }
            }

            catch (Exception ex)
            {
                return Ok(await Result<HrCompetenceDto>.FailAsync(ex.Message));
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long Id)
        {
            setErrors();
            var chk = await permission.HasPermission(1682, PermissionType.Edit);
            if (!chk)
            {
                return Ok(Result<HrCompetenceEditDto>.FailAsync($"Access Denied"));
            }
            if (Id.Equals(null))
            {
                return Ok(Result<HrCompetenceEditDto>.FailAsync($"There is No Id"));
            }

            try
            {
                var getItem = await hrServiceManager.HrCompetenceService.GetForUpdate<HrCompetenceEditDto>(Id);
                if (getItem.Succeeded && getItem.Data != null)
                {
                    return Ok(getItem);
                }
                return Ok(Result<HrCompetenceEditDto>.FailAsync($"No Id InUpdate"));
            }
            catch (Exception exp)
            {
                return Ok(Result<HrCompetenceEditDto>.FailAsync($"EXP in {this.GetType()}, Meesage: {exp.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(HrCompetenceEditDto obj)
        {
            setErrors();
            var chk = await permission.HasPermission(1682, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (!ModelState.IsValid)
            {
                return Ok(obj);
            }
            try
            {
                obj.TypeId = 2;
                var addRes = await hrServiceManager.HrCompetenceService.Update(obj);
                if (addRes.Succeeded)
                {
                    return Ok(addRes);
                }

                else
                {
                    return Ok(await Result.FailAsync($"{addRes.Status.message}"));
                }
            }

            catch (Exception ex)
            {
                return Ok(await Result<HrCompetenceEditDto>.FailAsync($"{ex.Message}"));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long Id = 0)
        {
            setErrors();
            var chk = await permission.HasPermission(1682, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (Id == 0)
            {
                return Ok(await Result.FailAsync("Please choose an entity to delete it, there is no id passed"));
            }

            try
            {
                var del = await hrServiceManager.HrCompetenceService.Remove(Id);
                if (del.Succeeded)
                {

                    return Ok(await Result.SuccessAsync("Item deleted successfully"));
                }
                return Ok(await Result.FailAsync($"{del.Status.message}"));
            }
            catch (Exception exp)
            {
                return Ok(await Result.FailAsync($"{exp.Message}"));
            }
        }

        [HttpGet("DDLCatID")]
        public async Task<IActionResult> DDLCatID()
        {
            try
            {
                var items = await hrServiceManager.HrCompetencesCatagoryService.GetAll(l => l.IsDeleted == false);

                if (items.Succeeded && items.Data != null)
                {
                    var list = listHelper.GetFromList<long>(items.Data.Select(s => new DDListItem<long> { Name = s.Name ?? "no Name", Value = s.Id }), hasDefault: false);

                    return Ok(await Result<SelectList>.SuccessAsync(list));

                }
                return Ok(items);

            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLKpiType")]
        public async Task<IActionResult> DDLKpiType()
        {
            try
            {
                var items = await hrServiceManager.HrKpiTypeService.GetAll(l => l.Isdeleted == false);

                if (items.Succeeded && items.Data != null)
                {
                    var list = listHelper.GetFromList<long>(items.Data.Select(s => new DDListItem<long> { Name = session.Language == 1 ? s.Name?? "" : s.Name2 ??"", Value = s.Id }), hasDefault: false);

                    return Ok(await Result<SelectList>.SuccessAsync(list));

                }
                return Ok(items);

            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

    }
}