using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Logix.MVC.LogixAPIs.HR
{
    public class HRCardTemplateApiController : BaseHrApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly IDDListHelper listHelper;

        public HRCardTemplateApiController(IHrServiceManager hrServiceManager, IMainServiceManager mainServiceManager, IPermissionHelper permission, ICurrentData session, IDDListHelper listHelper)
        {
            this.mainServiceManager = mainServiceManager;
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.listHelper = listHelper;
        }


        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var chk = await permission.HasPermission(1478, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var items = await hrServiceManager.HrCardTemplateService.GetAll(e => e.IsDeleted == false && e.Status == 1 && e.FacilityId == session.FacilityId);
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    res = res.OrderBy(e => e.Id);
                    return Ok(await Result<List<HrCardTemplateDto>>.SuccessAsync(res.ToList(), items.Status.message));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
        [HttpPost("SearchTemplate")]
        public async Task<IActionResult> GetAllSearch(HrCardTemplateFilterDto filter)
        {
            var chk = await permission.HasPermission(1478, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var items = await hrServiceManager.HrCardTemplateService.GetAll(e => e.IsDeleted == false
                && (e.Status == null || e.Status == 1) && e.FacilityId == session.FacilityId
                 && (string.IsNullOrEmpty(filter.Name) ||
                        (e.Name != null && e.Name.Contains(filter.Name)))


                );
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    res = res.OrderBy(e => e.Id);
                    var final = res.ToList();
                    return Ok(await Result<List<HrCardTemplateDto>>.SuccessAsync(final, ""));
                }

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }


        [HttpPost("GetPagination")]
        public async Task<IActionResult> GetPagination([FromBody] HrCardTemplateFilterDto filter, int take = Pagination.take, int? lastSeenId = null)
        {
            var chk = await permission.HasPermission(1478, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {

                var items = await hrServiceManager.HrCardTemplateService.GetAllWithPaginationVW(
                    selector: e => e.Id,
                    expression: e =>
                     e.IsDeleted == false && (e.Status == null || e.Status == 1) && e.FacilityId == session.FacilityId
                     && (string.IsNullOrEmpty(filter.Name) ||
                     (e.Name != null && e.Name.Contains(filter.Name)))
                            ,
                    take: take,
                    lastSeenId: lastSeenId
                );

                if (!items.Succeeded)
                    return Ok(await Result<List<HrCardTemplateDto>>.FailAsync(items.Status.message));

                if (items.Data == null || !items.Data.Any())
                    return Ok(await Result<List<HrCardTemplateDto>>.SuccessAsync(new List<HrCardTemplateDto>()));

                var res = items.Data.OrderBy(x => x.Id).ToList();

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
        public async Task<IActionResult> Add(HrCardTemplateDto obj)
        {
            var chk = await permission.HasPermission(1478, PermissionType.Add);
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
                var addRes = await hrServiceManager.HrCardTemplateService.Add(obj);
                if (addRes.Succeeded)
                {
                    return Ok(addRes);
                }

                else
                {
                    return Ok(await Result<HrCardTemplateDto>.FailAsync(addRes.Status.message));
                }
            }

            catch (Exception ex)
            {
                return Ok(await Result<HrCardTemplateDto>.FailAsync(ex.Message));
            }
        }

        [HttpGet("PreviewById")]
        public async Task<IActionResult> PreviewById(long Id)
        {

            var chk = await permission.HasPermission(1478, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result<HrCardTemplateDto>.FailAsync($"Access Denied"));
            }
            if (Id.Equals(null))
            {
                return Ok(Result<HrCardTemplateDto>.FailAsync($"There is No Id Passed"));
            }

            try
            {
                var items = await hrServiceManager.HrCardTemplateService.GetOne(t => t.Id == Id && t.IsDeleted == false && t.Status == 1 && t.FacilityId == session.FacilityId);
                if (items.Succeeded && items.Data != null)
                {
                    return Ok(items);
                }
                return Ok(await Result<HrCardTemplateDto>.FailAsync(items.Status.message));
            }
            catch (Exception exp)
            {
                return Ok(Result<HrCardTemplateDto>.FailAsync($"EXP in {this.GetType()}, Meesage: {exp.Message}"));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long Id = 0)
        {
            var chk = await permission.HasPermission(1478, PermissionType.Delete);
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
                var del = await hrServiceManager.HrCardTemplateService.Remove(Id);
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

        [HttpGet("ChangeStatus")]
        public async Task<IActionResult> ChangeStatus(long Id)
        {

            var chk = await permission.HasPermission(1478, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (Id.Equals(null))
            {
                return Ok(await Result<HrCardTemplateEditDto>.FailAsync($"There is No Id"));
            }

            try
            {
                var getItem = await hrServiceManager.HrCardTemplateService.GetForUpdate<HrCardTemplateEditDto>(Id);
                if (getItem.Succeeded && getItem.Data != null)
                {
                    var status = getItem.Data.Status == 1 ? 0 : 1;
                    var ChangeStatus = await hrServiceManager.HrCardTemplateService.UpdateTemplateStatus(getItem.Data.Id, status);
                    if (ChangeStatus.Succeeded)
                    {
                        return Ok(ChangeStatus);
                    }

                    else
                    {
                        return Ok(await Result<HrCardTemplateEditDto>.FailAsync());
                    }
                }
                return Ok(await Result<HrCompetenceEditDto>.FailAsync($"there is no record with this Id: {Id}"));
            }
            catch (Exception exp)
            {
                return Ok(await Result<HrCompetenceEditDto>.FailAsync($"EXP in {this.GetType()}, Meesage: {exp.Message}"));
            }
        }


        [HttpGet("DDLTemplates")]
        public async Task<IActionResult> DDLTemplates()
        {
            try
            {
                var getHrCardsTemplate = await hrServiceManager.HrCardTemplateService.GetAll(l => l.IsDeleted == false && l.Status == 1 && l.FacilityId == session.FacilityId);

                if (getHrCardsTemplate.Succeeded && getHrCardsTemplate.Data != null)
                {
                    var list = listHelper.GetFromList<long>(getHrCardsTemplate.Data.Select(s => new DDListItem<long> { Name = s.Name ?? "no Name", Value = s.Id }), hasDefault: false);

                    return Ok(await Result<SelectList>.SuccessAsync(list));

                }
                return Ok(getHrCardsTemplate);

            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }



    }

}