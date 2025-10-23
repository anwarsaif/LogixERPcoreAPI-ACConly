using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.DTOs.Main;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.HR
{

    //  الكفاءات
    public class HrCompetenceApiController : BaseHrApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        public HrCompetenceApiController(IHrServiceManager hrServiceManager, IMainServiceManager mainServiceManager, IPermissionHelper permission, ICurrentData session)
        {
            this.mainServiceManager = mainServiceManager;
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
        }


        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var chk = await permission.HasPermission(535, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var items = await hrServiceManager.HrCompetenceService.GetAll(e => e.IsDeleted == false&&e.TypeId==1);
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    res = res.OrderBy(e => e.Id);
                    return Ok(await Result<List<HrCompetenceDto>>.SuccessAsync(res.ToList(),items.Status.message ));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
        [HttpPost("SearchCompetence")]
        public async Task<IActionResult> GetAllSearch(HrCompetencesVw filter)
        {
            var chk = await permission.HasPermission(535, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var items = await hrServiceManager.HrCompetenceService.GetAllVW(e => e.IsDeleted == false&&e.TypeId==1);
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
                    if (!string.IsNullOrEmpty(filter.Name))
                    {
                        res = res.Where(c => (c.Name != null && c.Name.Contains(filter.Name)));
                    }
                    if (filter.Score != null )
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
            var chk = await permission.HasPermission(535, PermissionType.Add);
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
                obj.TypeId = 1;
                obj.KpiTypeId = 0;
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
            var chk = await permission.HasPermission(535, PermissionType.Edit);
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
                var getItem = await hrServiceManager.HrCompetenceService.GetOne(x=>x.Id==Id&&x.IsDeleted==false);
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
            var chk = await permission.HasPermission(535, PermissionType.Edit);
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
                obj.TypeId = 1;
                obj.KpiTypeId = 0;
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
            var chk = await permission.HasPermission(535, PermissionType.Delete);
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

                    return Ok(del);
                
            }
            catch (Exception exp)
            {
                return Ok(await Result.FailAsync($"{exp.Message}"));
            }
        }


    }

}