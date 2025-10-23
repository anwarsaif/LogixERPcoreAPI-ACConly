using AutoMapper;
using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Helpers;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.HR
{
    //  انواع القرارات
    public class HRDecisionsTypeController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IApiDDLHelper ddlHelper;
        private readonly IMapper mapper;
        private readonly IMainServiceManager mainServiceManager;

        public HRDecisionsTypeController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IApiDDLHelper ddlHelper, IMapper mapper,IMainServiceManager mainServiceManager)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.ddlHelper = ddlHelper;
            this.mapper = mapper;
            this.mainServiceManager = mainServiceManager;
        }


        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrDecisionsTypeFilterDto filter)
        {
            var chk = await permission.HasPermission(2011, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                List<HrDecisionsTypeFilterDto> resultList = new List<HrDecisionsTypeFilterDto>();
                filter.RefTypeID    ??= 0;
                var items = await hrServiceManager.HrDecisionsTypeService.GetAll(e =>
                    e.IsDeleted == false &&
                    (string.IsNullOrEmpty(filter.DecType) || e.DecType.Contains(filter.DecType)) &&
                    (filter.RefTypeID == 0 || e.RefTypeId == filter.RefTypeID));

                if (!items.Succeeded)
                    return Ok(await Result<HrDecisionsTypeFilterDto>.FailAsync(items.Status.message));

                var lookups = await mainServiceManager.SysLookupDataService.GetAll(e => e.CatagoriesId == 559);
                var lookupMap = lookups.Data.ToDictionary(l => l.Code, l => l.Name);

                foreach (var item in items.Data)
                {
                    resultList.Add(new HrDecisionsTypeFilterDto
                    {
                        Id = item.Id,
                        DecType = item.DecType,
                        RefTypeID = item.RefTypeId,
                        RefTypeName = item.RefTypeId != null && lookupMap.ContainsKey(item.RefTypeId)
                            ? lookupMap[item.RefTypeId]
                            : null
                    });
                }

                if (resultList.Any())
                    return Ok(await Result<List<HrDecisionsTypeFilterDto>>.SuccessAsync(resultList, ""));
                else
                    return Ok(await Result<List<HrDecisionsTypeFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrDecisionsTypeFilterDto>.FailAsync(ex.Message));
            }
        }

        [HttpPost("GetPagination")]
        public async Task<IActionResult> GetPagination([FromBody] HrDecisionsTypeFilterDto filter, int take = 5, long? lastSeenId = null)
        {
            try
            {
                var chk = await permission.HasPermission(2011, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                List<HrDecisionsTypeFilterDto> resultList = new List<HrDecisionsTypeFilterDto>();

                var BranchesList = session.Branches.Split(',');
                filter.RefTypeID ??= 0;

                var items = await hrServiceManager.HrDecisionsTypeService.GetAllWithPagination(selector: e => e.Id,
                expression: e =>
                    e.IsDeleted == false &&
                    (string.IsNullOrEmpty(filter.DecType) || e.DecType.Contains(filter.DecType)) &&
                    (filter.RefTypeID == 0 || e.RefTypeId == filter.RefTypeID),
                    take: take,
                    lastSeenId: lastSeenId);

                if (!items.Succeeded)
                    return Ok(await Result<List<HrDecisionsTypeDto>>.FailAsync(items.Status.message));


                var lookups = await mainServiceManager.SysLookupDataService.GetAll(e => e.CatagoriesId == 559);
                var lookupMap = lookups.Data.ToDictionary(l => l.Code, l => l.Name);

                foreach (var item in items.Data)
                {
                    resultList.Add(new HrDecisionsTypeFilterDto
                    {
                        Id = item.Id,
                        DecType = item.DecType,
                        RefTypeID = item.RefTypeId,
                        RefTypeName = item.RefTypeId != null && lookupMap.ContainsKey(item.RefTypeId)
                            ? lookupMap[item.RefTypeId]
                            : null
                    });
                }

                var paginatedData = new PaginatedResult<List<HrDecisionsTypeFilterDto>>
                {
                    Succeeded = items.Succeeded,
                    Data = resultList,
                    Status = items.Status,
                    PaginationInfo = items.PaginationInfo
                };
                return Ok(paginatedData);

            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        [HttpPost("Add")]
        public async Task<ActionResult> Add(HrDecisionsTypeDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(2011, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var add = await hrServiceManager.HrDecisionsTypeService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add Hr Decisions Type  Controller, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(HrDecisionsTypeEditDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(2011, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result<HrDecisionsTypeEditDto>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var update = await hrServiceManager.HrDecisionsTypeService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrDecisionsTypeEditDto>.FailAsync($"====== Exp in Hr Decisions Type Controller getById, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long Id)
        {
            try
            {
                var chk = await permission.HasPermission(2011, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var item = await hrServiceManager.HrDecisionsTypeService.GetOne(x => x.Id == Id&&x.IsDeleted==false);
                if (item.Succeeded)
                {
                    HrDecisionsTypeGetByIdDto returnedItem = new HrDecisionsTypeGetByIdDto();
                    if (item.Data != null)
                    {
                        //returnedItem = mapper.Map<HrDecisionsTypeGetByIdDto>(item.Data);
                        returnedItem.Note = item.Data.Note;
                        returnedItem.Id = item.Data.Id;
                        returnedItem.DecType = item.Data.DecType;
                        returnedItem.RefTypeId = item.Data.RefTypeId;
                        //returnedItem.CatagoriesId = item.Data.CatagoriesId;
                        var getDecisionTypeEmployee = await hrServiceManager.HrDecisionsTypeEmployeeService.GetAllVW(x => x.DecisionsTypeId == item.Data.Id&&x.IsDeleted==false);
                        if (getDecisionTypeEmployee.Data != null)
                        {
                            var empData = getDecisionTypeEmployee.Data.ToList();
                            returnedItem.HrDecisionsTypeEmployee = empData;
                        }
                        return Ok(await Result<HrDecisionsTypeGetByIdDto>.SuccessAsync(returnedItem));

                    }
                    return Ok(await Result<HrDecisionsTypeGetByIdDto>.FailAsync("the Record Is Not Found"));

                }
                return Ok(await Result<HrDecisionsTypeGetByIdDto>.FailAsync(item.Status.message));

            }
            catch (Exception ex)
            {
                return Ok(await Result<HrDecisionsTypeGetByIdDto>.FailAsync($"====== Exp in Hr Decision Controller getById, MESSAGE: {ex.Message}"));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var chk = await permission.HasPermission(2011, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await hrServiceManager.HrDecisionsTypeService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete Hr DecisionType Controller, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("AddDecisionsTypeEmployee")]
        public async Task<ActionResult> AddDecisionsTypeEmployee(HrDecisionsTypeEmployeeDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(2011, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var add = await hrServiceManager.HrDecisionsTypeEmployeeService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add Hr Decision Types  Controller, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("DeleteDecisionsTypeEmployee")]
        public async Task<IActionResult> DeleteDecisionsTypeEmployee(long id)
        {
            try
            {
                var chk = await permission.HasPermission(2011, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await hrServiceManager.HrDecisionsTypeEmployeeService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete Hr DecisionType Controller, MESSAGE: {ex.Message}"));
            }
        }
    }
}