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

    //  إعداد رسائل التهنئة
    public class HRNotificationsTypeApiController : BaseHrApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly IDDListHelper listHelper;
        private readonly ILocalizationService localization;
        public HRNotificationsTypeApiController(IHrServiceManager hrServiceManager, IMainServiceManager mainServiceManager, IPermissionHelper permission, ICurrentData session, IDDListHelper listHelper, ILocalizationService localization)
        {
            this.mainServiceManager = mainServiceManager;
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.listHelper = listHelper;
            this.localization = localization;
        }



        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrNotificationsTypeFilterDto filter)
        {
            var chk = await permission.HasPermission(1473, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                filter.Id ??= 0;
                filter.IsActive ??= 0;
                var items = await hrServiceManager.HrNotificationsTypeService.GetAllVW(e =>
                    e.IsDeleted == false &&
                    e.FacilityId == session.FacilityId &&
                    (filter.Id == 0 || e.Id == filter.Id)
                && (string.IsNullOrEmpty(filter.MsgSubject) || (e.MsgSubject != null && e.MsgSubject.Contains(filter.MsgSubject)))
                && (!filter.IsActive.HasValue || filter.IsActive == 0 || e.IsActive == (filter.IsActive.Value == 1))
                );

                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable().OrderBy(e => e.Id).ToList();
                    return Ok(await Result<List<HrNotificationsTypeVw>>.SuccessAsync(res, ""));
                }

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
        [HttpPost("GetPagination")]
        public async Task<IActionResult> GetPagination([FromBody] HrNotificationsTypeFilterDto filter, int take = Pagination.take, int? lastSeenId = null)
        {
            var chk = await permission.HasPermission(1473, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                filter.Id ??= 0;
                filter.IsActive ??= 0;

                var items = await hrServiceManager.HrNotificationsTypeService.GetAllWithPaginationVW(
                    selector: e => e.Id,
                    expression: e =>
                        e.IsDeleted == false &&
                    e.FacilityId == session.FacilityId &&
                    (filter.Id == 0 || e.Id == filter.Id)
                && (string.IsNullOrEmpty(filter.MsgSubject) || (e.MsgSubject != null && e.MsgSubject.Contains(filter.MsgSubject)))
                && (!filter.IsActive.HasValue || filter.IsActive == 0 || e.IsActive == (filter.IsActive.Value == 1))


                            ,
                    take: take,
                    lastSeenId: lastSeenId
                );

                if (!items.Succeeded)
                    return Ok(await Result<List<HrNotificationsTypeVw>>.FailAsync(items.Status.message));

                if (items.Data == null || !items.Data.Any())
                    return Ok(await Result<List<HrNotificationsTypeVw>>.SuccessAsync(new List<HrNotificationsTypeVw>()));

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
        public async Task<IActionResult> Add(HrNotificationsTypeDto obj)
        {

            try
            {
                var chk = await permission.HasPermission(1473, PermissionType.Add);
                if (!chk)
                {
                    return Ok(await Result.AccessDenied("AccessDenied"));
                }
                if (obj.SubjectType <= 0)
                    return Ok(await Result<object>.FailAsync($"{localization.GetHrResource("MessageTitle")}"));

                if (string.IsNullOrEmpty(obj.Detailes))
                    return Ok(await Result.FailAsync($"{localization.GetHrResource("MessageContent")}"));

                var addRes = await hrServiceManager.HrNotificationsTypeService.Add(obj);
                return Ok(addRes);
            }

            catch (Exception ex)
            {
                return Ok(await Result<HrNotificationsTypeDto>.FailAsync(ex.Message));
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long Id)
        {
            var chk = await permission.HasPermission(1473, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (Id.Equals(null))
            {
                return Ok(Result<HrNotificationsTypeEditDto>.FailAsync(localization.GetMessagesResource("NoIdInUpdate")));
            }

            try
            {
                var getItem = await hrServiceManager.HrNotificationsTypeService.GetForUpdate<HrNotificationsTypeEditDto>(Id);


                if (getItem.Succeeded && getItem.Data != null)
                {
                    getItem.Data.SubjectType ??= 0;
                    var getList = await mainServiceManager.SysLookupDataService.GetAllVW(d => d.Isdel == false && d.Code == getItem.Data.SubjectType && d.CatagoriesId == 469);
                    var NameLookup = session.Language == 1 ? getList.Data.Select(s => s.Name).FirstOrDefault() : getList.Data.Select(s => s.Name).FirstOrDefault();
                    getItem.Data.NameLookup = NameLookup;
                    return Ok(getItem);
                }
                return Ok(Result<HrNotificationsTypeEditDto>.FailAsync(localization.GetMessagesResource("NoIdInUpdate")));
            }
            catch (Exception exp)
            {
                return Ok(Result<HrNotificationsTypeEditDto>.FailAsync($"EXP in {this.GetType()}, Meesage: {exp.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(HrNotificationsTypeEditDto obj)
        {
            var chk = await permission.HasPermission(1473, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                if (obj.SubjectType <= 0)
                    return Ok(await Result<object>.FailAsync($"{localization.GetHrResource("MessageTitle")}"));

                if (string.IsNullOrEmpty(obj.Detailes))
                    return Ok(await Result.FailAsync($"{localization.GetHrResource("MessageContent")}"));

                var UpdateRes = await hrServiceManager.HrNotificationsTypeService.Update(obj);
                return Ok(UpdateRes);


            }

            catch (Exception ex)
            {
                return Ok(await Result<HrNotificationsTypeEditDto>.FailAsync($"{ex.Message}"));
            }
        }

        [HttpGet("DDSubjectForEdit")]
        public async Task<IActionResult> DDSubjectForEdit(long typeId = 0)
        {
            try
            {
                var getList = await mainServiceManager.SysLookupDataService.GetAllVW(d => d.Isdel == false && d.Code == typeId && d.CatagoriesId == 469);
                if (getList.Succeeded && getList.Data != null)
                {
                    var list = getList.Data.Select(s => new { Id = s.Code, Name1 = s.Name, Name2 = s.Name2 });
                    return Ok(await Result<object>.SuccessAsync(list, "قائمة"));
                }

                return Ok(await Result<SysLookupDataDto>.SuccessAsync(getList.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDSubjectForAdd")]
        public async Task<IActionResult> DDSubjectForAdd()
        {
            var lang = session.Language;

            try
            {
                var getallSubjectsTypes = await hrServiceManager.HrNotificationsTypeService.GetAll(n => n.IsDeleted == false && n.FacilityId == session.FacilityId);
                var getallSubjectsTypesId = getallSubjectsTypes.Data.Select(x => x.SubjectType).ToList();

                if (getallSubjectsTypes.Succeeded && getallSubjectsTypes.Data != null)
                {
                    var getfromLookUp = await mainServiceManager.SysLookupDataService.GetAllVW(d => d.Isdel == false && d.CatagoriesId == 469 && d.Code != null && !getallSubjectsTypesId.Contains((int)(d.Code)));
                    var list = listHelper.GetFromList<long>(getfromLookUp.Data.Select(s => new DDListItem<long> { Name = lang == 1 ? s.Name ?? "" : s.Name2 ?? "", Value = (long)s.Code }), hasDefault: false);

                    return Ok(await Result<object>.SuccessAsync(list, "قائمة"));
                }

                return Ok(await Result<HrNotificationsTypeDto>.SuccessAsync(getallSubjectsTypes.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }



    }
}