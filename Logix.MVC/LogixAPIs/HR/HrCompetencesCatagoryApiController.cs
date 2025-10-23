using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.DTOs.Main;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Logix.MVC.LogixAPIs.HR
{
    //  انواع الكفاءات
    public class HrCompetencesCatagoryApiController : BaseHrApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        public HrCompetencesCatagoryApiController(IHrServiceManager hrServiceManager, IMainServiceManager mainServiceManager, ICurrentData session, IPermissionHelper permission)
        {
            this.mainServiceManager = mainServiceManager;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.permission = permission;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var chk = await permission.HasPermission(534, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var items = await hrServiceManager.HrCompetencesCatagoryService.GetAll(e => e.IsDeleted == false);
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    res = res.OrderBy(e => e.Id);
                    return Ok(await Result<List<HrCompetencesCatagoryDto>>.SuccessAsync(res.ToList(), items.Status.message));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                 return Ok(await Result.FailAsync(ex.Message));
            }
        }
        [HttpPost("SearchCompetenceCatagory")]
        public async Task<IActionResult> GetAllSearch(HrCompetencesCatagoryDto filter)
        {
            var chk = await permission.HasPermission(534, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var items = await hrServiceManager.HrCompetencesCatagoryService.GetAll(e => e.IsDeleted == false);
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    if (filter == null)
                    {
                        return Ok(items);
                    }
                    if (!string.IsNullOrEmpty(filter.Name))
                    {
                        res = res.Where(c => (c.Name != null && c.Name.Contains(filter.Name)));
                    }

                    res = res.OrderBy(e => e.Id);
                    var final = res.ToList();
                    return Ok(await Result<List<HrCompetencesCatagoryDto>>.SuccessAsync(final, ""));
                }

                return Ok(items);
            }
            catch (Exception ex)
            {
                 return Ok(await Result.FailAsync(ex.Message));
            }
        }
        [HttpPost("Add")]
        public async Task<IActionResult> Add(HrCompetencesCatagoryDto obj)
        {
            var chk = await permission.HasPermission(534, PermissionType.Add);
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
                var addRes = await hrServiceManager.HrCompetencesCatagoryService.Add(obj);
                if (addRes.Succeeded)
                {
                    return Ok(addRes);
                }

                else
                {
                    return Ok(obj);
                }
            }

            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long  Id)
        {
            var chk = await permission.HasPermission(534, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (Id.Equals(null))
            {
                return Ok(await Result.FailAsync("No Id In Update"));;
            }

            try
            {
                var getItem = await hrServiceManager.HrCompetencesCatagoryService.GetForUpdate<HrCompetencesCatagoryEditDto>(Id);
                if (getItem.Succeeded && getItem.Data != null)
                {
                    return Ok(getItem);
                }
                return Ok(await Result.FailAsync("No Id In Update"));;
            }
            catch (Exception ex)
            {
                 return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(HrCompetencesCatagoryEditDto obj)
        {
            var chk = await permission.HasPermission(534, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (!ModelState.IsValid)
            {
                return Ok(await Result<HrCompetencesCatagoryEditDto>.SuccessAsync(obj));
            }
            try
            {
                var addRes = await hrServiceManager.HrCompetencesCatagoryService.Update(obj);
                if (addRes.Succeeded)
                {
                    return Ok(addRes);
                }

                else
                {
                    return Ok(await Result.FailAsync(addRes.Status.message));
                }
            }

            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long Id=0)
        {
            var chk = await permission.HasPermission(534, PermissionType.Delete);
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
                var del = await hrServiceManager.HrCompetencesCatagoryService.Remove(Id);
                return Ok(del);
            }
            catch (Exception ex)
            {
                 return Ok(await Result.FailAsync(ex.Message));
            }
        }

    }

}