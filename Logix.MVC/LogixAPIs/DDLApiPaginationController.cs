using Logix.Application.Common;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;
using Logix.Domain.HR;
using Logix.Domain.Main;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.HR.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Result = Logix.Application.Wrapper.Result;

namespace Logix.MVC.LogixAPIs
{
    [Route($"api/{ApiConfig.ApiVersion}/[controller]")]
    [ApiController]
    public class DDLApiPaginationController : ControllerBase
    {
        private readonly IApiDDLHelper ddlHelper;
        private readonly ILocalizationService localization;
        private readonly ICurrentData session;
        private readonly ISysConfigurationHelper configHelper;
        private readonly IMainServiceManager mainServiceManager;
        private readonly IDDListHelper listHelper;
        private readonly IAccServiceManager accServiceManager;
        private readonly IPMServiceManager pMServiceManager;
        private readonly IWFServiceManager wfServiceManager;
        private readonly IWhServiceManager whServiceManager;
        private readonly IHrServiceManager hrServiceManager;

        public DDLApiPaginationController(IApiDDLHelper ddlHelper,
            ICurrentData session,
            ISysConfigurationHelper configHelper,
            IMainServiceManager mainServiceManager,
            IDDListHelper listHelper,
            IAccServiceManager accServiceManager,
            ILocalizationService localization,
            IPMServiceManager pMServiceManager,
            IWFServiceManager wfServiceManager,
            IWhServiceManager whServiceManager,
            IHrServiceManager hrServiceManager
            )
        {
            this.ddlHelper = ddlHelper;
            this.session = session;
            this.configHelper = configHelper;
            this.mainServiceManager = mainServiceManager;
            this.listHelper = listHelper;
            this.accServiceManager = accServiceManager;
            this.localization = localization;
            this.pMServiceManager = pMServiceManager;
            this.wfServiceManager = wfServiceManager;
            this.whServiceManager = whServiceManager;
            this.hrServiceManager = hrServiceManager;
        }

        #region ======================================== Drop down list From main tables (Sys) ========================================

        [HttpGet("DDLSystemsPagination")]
        public async Task<IActionResult> DDLSystemsPagination(int lang = 1, long lastSeenId = 0, int pageSize = Pagination.DDLpageSize)
        {
            try
            {
                var ddlResult = await listHelper.GetRawListPagination<SysSystem, long>(
                    s => s.Isdel == false,
                    "SystemId",
                    (lang == 1) ? "SystemName" : "SystemName2",
                    lastSeenId,
                    pageSize
                );

                var selectList = new SelectList(ddlResult.Items, "Value", "Name");

                var pagedResult = new DDLDataPagedResult
                {
                    Items = selectList,
                    LastSeenId = ddlResult.LastSeenId
                };

                return Ok(await Result<DDLDataPagedResult>.SuccessAsync(pagedResult, ""));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }



        [HttpGet("DDLScreensPagination")]
        public async Task<IActionResult> DDLScreensPagination(int lang = 1, long lastSeenId = 0, int pageSize = Pagination.DDLpageSize)
        {
            try
            {
                var systemsIds = await mainServiceManager.SysSystemService.GetAll(
                    s => s.SystemId,
                    s => s.Isdel == false);

                if (!systemsIds.Succeeded)
                {
                    return Ok(await Result.FailAsync("فشل في جلب الأنظمة."));
                }

                List<int> systemsIdsArr = new();
                systemsIdsArr.AddRange(systemsIds.Data);
                var ddlResult = await listHelper.GetRawListPagination<SysScreen, long>(
                    s => s.Isdel == false && systemsIdsArr.Contains(s.SystemId ?? 0),
                    "ScreenId",
                    (lang == 1) ? "ScreenName" : "ScreenName2",
                    lastSeenId,
                    pageSize
                );

                var selectList = new SelectList(ddlResult.Items, "Value", "Name");
                var pagedResult = new DDLDataPagedResult
                {
                    Items = selectList,
                    LastSeenId = ddlResult.LastSeenId
                };

                return Ok(await Result<DDLDataPagedResult>.SuccessAsync(pagedResult, ""));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
        [HttpGet("DDLSystemScreensPagination")]
        public async Task<IActionResult> DDLSystemScreensPagination(int systemId, int lang = 1, long lastSeenId = 0, int pageSize = Pagination.DDLpageSize)
        {
            try
            {
                if (systemId <= 0)
                {
                    return Ok(await Result.FailAsync("Invalid System Id"));
                }
                var ddlResult = await listHelper.GetRawListPagination<SysScreen, long>(
                    s => s.SystemId == systemId
                         && s.ParentId != s.ScreenId
                         && s.Isdel == false,
                    "ScreenId",
                    (lang == 1) ? "ScreenName" : "ScreenName2",
                    lastSeenId,
                    pageSize
                );

                var selectList = new SelectList(ddlResult.Items, "Value", "Name");

                var pagedResult = new DDLDataPagedResult
                {
                    Items = selectList,
                    LastSeenId = ddlResult.LastSeenId
                };

                return Ok(await Result<DDLDataPagedResult>.SuccessAsync(pagedResult, ""));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLParentScreensPagination")]
        public async Task<IActionResult> DDLParentScreensPagination(int systemId, int lang = 1, long lastSeenId = 0, int pageSize = Pagination.DDLpageSize)
        {
            try
            {
                if (systemId <= 0)
                {
                    return Ok(await Result.FailAsync("Invalid system Id"));
                }

                var ddlResult = await listHelper.GetRawListPagination<SysScreen, long>(
                    s => s.SystemId == systemId
                         && s.ParentId == s.ScreenId
                         && s.Isdel == false,
                    "ScreenId",
                    (lang == 1) ? "ScreenName" : "ScreenName2",
                    lastSeenId,
                    pageSize
                );
                var selectList = new SelectList(ddlResult.Items, "Value", "Name");
                var pagedResult = new DDLDataPagedResult
                {
                    Items = selectList,
                    LastSeenId = ddlResult.LastSeenId
                };

                return Ok(await Result<DDLDataPagedResult>.SuccessAsync(pagedResult, ""));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }


        [HttpGet("DDLChildScreensPagination")]
        public async Task<IActionResult> DDLChildScreensPagination(long parentId, int lang = 1, long lastSeenId = 0, int pageSize = Pagination.DDLpageSize)
        {
            try
            {
                if (parentId <= 0)
                {
                    return Ok(await Result.FailAsync("Invalid parent Id"));
                }
                var ddlResult = await listHelper.GetRawListPagination<SysScreen, long>(
                    s => s.ParentId != s.ScreenId
                         && s.ParentId == parentId
                         && s.Isdel == false,
                    "ScreenId",
                    (lang == 1) ? "ScreenName" : "ScreenName2",
                    lastSeenId,
                    pageSize
                );

                var selectList = new SelectList(ddlResult.Items, "Value", "Name");

                var pagedResult = new DDLDataPagedResult
                {
                    Items = selectList,
                    LastSeenId = ddlResult.LastSeenId
                };

                return Ok(await Result<DDLDataPagedResult>.SuccessAsync(pagedResult, ""));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLBranchesPagination")]
        public async Task<IActionResult> DDLBranchesPagination(int lang = 1, long lastSeenId = 0, int pageSize = Pagination.DDLpageSize)
        {
            try
            {
                var braList = session.Branches.Split(",").ToList();
                var ddlResult = await listHelper.GetRawListPagination<InvestBranchVw, long>(
                    b => b.Isdel == false
                         && b.FacilityId == session.FacilityId
                         && braList.Contains(b.BranchId.ToString()),
                    "BranchId",
                    (lang == 1) ? "BraName" : "BraName2",
                    lastSeenId,
                    pageSize
                );

                var selectList = new SelectList(ddlResult.Items, "Value", "Name");
                var pagedResult = new DDLDataPagedResult
                {
                    Items = selectList,
                    LastSeenId = ddlResult.LastSeenId
                };

                return Ok(await Result<DDLDataPagedResult>.SuccessAsync(pagedResult, ""));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLBranchesByFacilityIdPagination")]
        public async Task<IActionResult> DDLBranchesByFacilityIdPagination(long facilityId, int lang = 1, int lastSeenId = 0, int pageSize = Pagination.DDLpageSize)
        {
            try
            {
                if (facilityId <= 0)
                {
                    return Ok(await Result.FailAsync("Invalid facility Id"));
                }
                var braList = session.Branches.Split(",").ToList();

                var ddlResult = await listHelper.GetRawListPagination<InvestBranch, int>(
                    s => s.FacilityId == facilityId
                         && s.Isdel == false
                         && braList.Contains(s.BranchId.ToString()),
                    "BranchId",
                    (lang == 1) ? "BraName" : "BraName2",
                    lastSeenId,
                    pageSize
                );

                var selectList = new SelectList(ddlResult.Items, "Value", "Name");

                var pagedResult = new DDLDataPagedResult
                {
                    Items = selectList,
                    LastSeenId = ddlResult.LastSeenId
                };

                return Ok(await Result<DDLDataPagedResult>.SuccessAsync(pagedResult, ""));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
        [HttpGet("DDLMainCategoriesBySysIdPagination")]
        public async Task<IActionResult> DDLMainCategoriesBySysIdPagination(string systemId, int lang = 1, long lastSeenId = 0, int pageSize = Pagination.DDLpageSize)
        {
            try
            {
                if (string.IsNullOrEmpty(systemId) || systemId == "0")
                {
                    return Ok(await Result.FailAsync("Invalid System Id"));
                }

                var ddlResult = await listHelper.GetRawListPagination<SysLookupCategory, long>(
                    s => s.SystemId == systemId && s.Isdel == false,
                    "CatagoriesId",
                    (lang == 1) ? "CatagoriesName" : "CatagoriesName2",
                    lastSeenId,
                    pageSize
                );

                var selectList = new SelectList(ddlResult.Items, "Value", "Name");

                var pagedResult = new DDLDataPagedResult
                {
                    Items = selectList,
                    LastSeenId = ddlResult.LastSeenId
                };

                return Ok(await Result<DDLDataPagedResult>.SuccessAsync(pagedResult, ""));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLLookupCatagoriesPagination")]
        public async Task<IActionResult> DDLLookupCatagoriesPagination(int lang = 1, int lastSeenId = 0, int pageSize = Pagination.DDLpageSize)
        {
            try
            {
                var ddlResult = await listHelper.GetRawListPagination<SysLookupCategory, int>(
                    s => s.Isdel == false,
                    "CatagoriesId",
                    (lang == 1) ? "CatagoriesName" : "CatagoriesName2",
                    lastSeenId,
                    pageSize
                );

                var selectList = new SelectList(ddlResult.Items, "Value", "Name");
                var pagedResult = new DDLDataPagedResult
                {
                    Items = selectList,
                    LastSeenId = ddlResult.LastSeenId
                };

                return Ok(await Result<DDLDataPagedResult>.SuccessAsync(pagedResult, ""));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLCitiesPagination")]
        public async Task<IActionResult> DDLCitiesPagination(int lang = 1, int lastSeenId = 0, int pageSize = Pagination.DDLpageSize)
        {
            try
            {
                var ddlResult = await listHelper.GetRawListPagination<SysCites, int>(
                    b => b.IsDeleted == false,
                    "CityID",
                    (lang == 1) ? "CityName" : "CityName2",
                    lastSeenId,
                    pageSize
                );

                var selectList = new SelectList(ddlResult.Items, "Value", "Name");

                var pagedResult = new DDLDataPagedResult
                {
                    Items = selectList,
                    LastSeenId = ddlResult.LastSeenId
                };

                return Ok(await Result<DDLDataPagedResult>.SuccessAsync(pagedResult, ""));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLCountriesPagination")]
        public async Task<IActionResult> DDLCountriesPagination(int lang = 1, int lastSeenId = 0, int pageSize = Pagination.DDLpageSize)
        {
            try
            {
                var ddlResult = await listHelper.GetRawListPagination<SysCites, int>(
                    b => b.TypeID == 1 && b.IsDeleted == false,
                    "CityID",
                    (lang == 1) ? "CityName" : "CityName2",
                    lastSeenId,
                    pageSize
                );

                var selectList = new SelectList(ddlResult.Items, "Value", "Name");

                var pagedResult = new DDLDataPagedResult
                {
                    Items = selectList,
                    LastSeenId = ddlResult.LastSeenId
                };

                return Ok(await Result<DDLDataPagedResult>.SuccessAsync(pagedResult, ""));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
        [HttpGet("DDLDepartmentCategoriesPagination")]
        public async Task<IActionResult> DDLDepartmentCategoriesPagination(int lang = 1, int lastSeenId = 0, int pageSize = Pagination.DDLpageSize)
        {
            try
            {
                var ddlResult = await listHelper.GetRawListPagination<SysDepartmentCatagory, int>(
                    s => true,
                    "Id",
                    (lang == 1) ? "CatName" : "CatName",
                    lastSeenId,
                    pageSize
                );

                var selectList = new SelectList(ddlResult.Items, "Value", "Name");

                var pagedResult = new DDLDataPagedResult
                {
                    Items = selectList,
                    LastSeenId = ddlResult.LastSeenId
                };

                return Ok(await Result<DDLDataPagedResult>.SuccessAsync(pagedResult, ""));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLDepartmentsParentPagination")]
        public async Task<IActionResult> DDLDepartmentsParentPagination(int lang = 1, int typeId = 0, int lastSeenId = 0, int pageSize = Pagination.DDLpageSize)
        {
            try
            {
                var ddlResult = await listHelper.GetRawListPagination<SysDepartment, int>(
                    d => d.FacilityId == session.FacilityId
                         && d.IsDeleted == false
                         && d.Id != d.ParentId
                         && (typeId != 0 ? d.TypeId == typeId : true),
                    "Id",
                    (lang == 1) ? "Name" : "Name2",
                    lastSeenId,
                    pageSize
                );

                var selectList = new SelectList(ddlResult.Items, "Value", "Name");

                var pagedResult = new DDLDataPagedResult
                {
                    Items = selectList,
                    LastSeenId = ddlResult.LastSeenId
                };

                return Ok(await Result<DDLDataPagedResult>.SuccessAsync(pagedResult, ""));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
        [HttpGet("DDLSysGroupsPagination")]
        public async Task<IActionResult> DDLSysGroupsPagination(int lang = 1, int lastSeenId = 0, int pageSize = Pagination.DDLpageSize)
        {
            try
            {
                var groupIds = new List<int>();
                var getGroupIds = await configHelper.GetValue(105, session.FacilityId);
                if (!string.IsNullOrEmpty(getGroupIds))
                {
                    foreach (var group in getGroupIds.Split(","))
                    {
                        if (int.TryParse(group, out int gid))
                        {
                            groupIds.Add(gid);
                        }
                    }
                }

                var ddlResult = await listHelper.GetRawListPagination<SysGroup, int>(
                    b => b.IsDeleted == false
                         && b.FacilityId == session.FacilityId
                         && groupIds.Contains(b.GroupId),
                    "GroupId",
                    (lang == 1) ? "GroupName" : "GroupName2",
                    lastSeenId,
                    pageSize
                );

                var selectList = new SelectList(ddlResult.Items, "Value", "Name");

                var pagedResult = new DDLDataPagedResult
                {
                    Items = selectList,
                    LastSeenId = ddlResult.LastSeenId
                };

                return Ok(await Result<DDLDataPagedResult>.SuccessAsync(pagedResult, ""));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
        // مستخدمة في شاشة الموظفين /HR/Employee/Employee
        // مستخدمة في شاشة عقد جديد /HR/Contract/Contract_add2

        [HttpGet("DDLDepartmentsPagination")]
        public async Task<IActionResult> DDLDepartmentsPagination(int lang = 1, long lastSeenId = 0, int pageSize = Pagination.DDLpageSize)
        {
            try
            {
                var ddlResult = await listHelper.GetRawListPagination<SysDepartment, long>(
                    d => d.TypeId == 1
                         && d.IsDeleted == false
                         && d.StatusId == 1
                         && (d.IsShare == true || d.FacilityId == session.FacilityId),
                    "Id",
                    (lang == 1) ? "Name" : "Name2",
                    lastSeenId,
                    pageSize
                );

                var selectList = new SelectList(ddlResult.Items, "Value", "Name");

                var pagedResult = new DDLDataPagedResult
                {
                    Items = selectList,
                    LastSeenId = ddlResult.LastSeenId
                };

                return Ok(await Result<DDLDataPagedResult>.SuccessAsync(pagedResult, ""));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        //مستخدمه في شاشة اضافة وظيفة اضافية

        [HttpGet("DDLDepartmentPagination")]
        public async Task<IActionResult> DDLDepartmentPagination(int lang = 1, long lastSeenId = 0, int pageSize = Pagination.DDLpageSize)
        {
            try
            {
                var ddlResult = await listHelper.GetRawListPagination<SysDepartment, long>(
                    d => d.IsDeleted == false
                         && d.TypeId == 1
                         && d.StatusId == 1,
                    "Id",
                    (lang == 1) ? "Name" : "Name2",
                    lastSeenId,
                    pageSize
                );

                var selectList = new SelectList(ddlResult.Items, "Value", "Name");

                var pagedResult = new DDLDataPagedResult
                {
                    Items = selectList,
                    LastSeenId = ddlResult.LastSeenId
                };

                return Ok(await Result<DDLDataPagedResult>.SuccessAsync(pagedResult, ""));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
        [HttpGet("DDLLocationsPagination")]
        public async Task<IActionResult> DDLLocationsPagination(int lang = 1, long lastSeenId = 0, int pageSize = Pagination.DDLpageSize)
        {
            try
            {
                var ddlResult = await listHelper.GetRawListPagination<SysDepartment, long>(
                    d => d.TypeId == 2
                         && d.IsDeleted == false
                         && d.StatusId == 1
                         && (d.IsShare == true || d.FacilityId == session.FacilityId),
                    "Id",
                    (lang == 1) ? "Name" : "Name2",
                    lastSeenId,
                    pageSize
                );

                var selectList = new SelectList(ddlResult.Items, "Value", "Name");

                var pagedResult = new DDLDataPagedResult
                {
                    Items = selectList,
                    LastSeenId = ddlResult.LastSeenId
                };

                return Ok(await Result<DDLDataPagedResult>.SuccessAsync(pagedResult, ""));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLLocationsVwPagination")]
        public async Task<IActionResult> DDLLocationsVwPagination(int lang = 1, long lastSeenId = 0, int pageSize = Pagination.DDLpageSize)
        {
            try
            {
                var ddlResult = await listHelper.GetRawListPagination<SysDepartmentVw, long>(
                    d => d.TypeId == 2
                         && d.IsDeleted == false
                         && d.FacilityId == session.FacilityId,
                    "Id",
                    (lang == 1) ? "Name" : "Name2",
                    lastSeenId,
                    pageSize
                );

                var selectList = new SelectList(ddlResult.Items, "Value", "Name");

                var pagedResult = new DDLDataPagedResult
                {
                    Items = selectList,
                    LastSeenId = ddlResult.LastSeenId
                };

                return Ok(await Result<DDLDataPagedResult>.SuccessAsync(pagedResult, ""));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }


        // مستخدمة في شاشة الموظفين /HR/Employee/Employee
        // مستخدمة في شاشة مباشرة العمل /HR/JoinWork/JoinWork

        [HttpGet("DDLLocationPagination")]
        public async Task<IActionResult> DDLLocationPagination(int lang = 1, long lastSeenId = 0, int pageSize = Pagination.DDLpageSize)
        {
            try
            {

                var ddlResult = await listHelper.GetRawListPagination<SysDepartmentVw, long>(
                    d => d.TypeId == 2
                         && d.IsDeleted == false
                         && d.StatusId == 1
                         && (d.IsShare == true || d.FacilityId == session.FacilityId),
                    "Id",
                    (lang == 1) ? "Name" : "Name2",
                    lastSeenId,
                    pageSize
                );

                var selectList = new SelectList(ddlResult.Items, "Value", "Name");

                var pagedResult = new DDLDataPagedResult
                {
                    Items = selectList,
                    LastSeenId = ddlResult.LastSeenId
                };

                return Ok(await Result<DDLDataPagedResult>.SuccessAsync(pagedResult, ""));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLCustomerTypesPagination")]
        public async Task<IActionResult> DDLCustomerTypesPagination(long lastSeenId = 0, int pageSize = Pagination.DDLpageSize)
        {
            try
            {
                var ddlResult = await listHelper.GetRawListPagination<SysCustomerType, long>(
                    s => true,
                    "TypeId",
                    "CusTypeName",
                    lastSeenId,
                    pageSize
                );

                var selectList = new SelectList(ddlResult.Items, "Value", "Name");

                var pagedResult = new DDLDataPagedResult
                {
                    Items = selectList,
                    LastSeenId = ddlResult.LastSeenId
                };

                return Ok(await Result<DDLDataPagedResult>.SuccessAsync(pagedResult, ""));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
        [HttpGet("DDLCurrencyPagination")]
        public async Task<IActionResult> DDLCurrencyPagination(int lang = 1, int lastSeenId = 0, int pageSize = Pagination.DDLpageSize)
        {
            try
            {
                var ddlResult = await listHelper.GetRawListPagination<SysExchangeRateListVW, int>(
                    d => d.IsDeleted == false,
                    "Id",
                    (lang == 1) ? "Name" : "Name2",
                    lastSeenId,
                    pageSize
                );

                var selectList = new SelectList(ddlResult.Items, "Value", "Name");

                var pagedResult = new DDLDataPagedResult
                {
                    Items = selectList,
                    LastSeenId = ddlResult.LastSeenId
                };

                return Ok(await Result<DDLDataPagedResult>.SuccessAsync(pagedResult, ""));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
        [HttpGet("DDLAnnouncementLocationsPagination")]
        public async Task<IActionResult> DDLAnnouncementLocationsPagination(long lastSeenId = 0, int pageSize = Pagination.DDLpageSize)
        {
            try
            {
                var ddlResult = await listHelper.GetRawListPagination<SysAnnouncementLocationVw, long>(
                    s => s.Isdel == false,
                    "Id",
                    "LocationName",
                    lastSeenId,
                    pageSize
                );

                var selectList = new SelectList(ddlResult.Items, "Value", "Name");

                var pagedResult = new DDLDataPagedResult
                {
                    Items = selectList,
                    LastSeenId = ddlResult.LastSeenId
                };

                return Ok(await Result<DDLDataPagedResult>.SuccessAsync(pagedResult, ""));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLSysTablesPagination")]
        public async Task<IActionResult> DDLSysTablesPagination(int lastSeenId = 0, int pageSize = Pagination.DDLpageSize)
        {
            try
            {
                var ddlResult = await listHelper.GetRawListPagination<SysTable, int>(
                    s => true,
                    "TableId",
                    "TableDescription",
                    lastSeenId,
                    pageSize
                );

                var selectList = new SelectList(ddlResult.Items, "Value", "Name");

                var pagedResult = new DDLDataPagedResult
                {
                    Items = selectList,
                    LastSeenId = ddlResult.LastSeenId
                };

                return Ok(await Result<DDLDataPagedResult>.SuccessAsync(pagedResult, ""));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLSysTableFieldsPagination")]
        public async Task<IActionResult> DDLSysTableFieldsPagination(long tableId, long lastSeenId = 0, int pageSize = Pagination.DDLpageSize)
        {
            try
            {
                var ddlResult = await listHelper.GetRawListPagination<SysTableField, long>(
                    f => f.TableId == tableId,
                    "Id",
                    session.Language == 1 ? "Desc1" : "Desc2",
                    lastSeenId,
                    pageSize
                );

                var selectList = new SelectList(ddlResult.Items, "Value", "Name");

                var pagedResult = new DDLDataPagedResult
                {
                    Items = selectList,
                    LastSeenId = ddlResult.LastSeenId
                };

                return Ok(await Result<DDLDataPagedResult>.SuccessAsync(pagedResult, ""));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
        [HttpGet("DDLDynamicAttributeDataTypePagination")]
        public async Task<IActionResult> DDLDynamicAttributeDataTypePagination(int lang = 1, int lastSeenId = 0, int pageSize = Pagination.DDLpageSize)
        {
            try
            {
                var ddlResult = await listHelper.GetRawListPagination<SysDynamicAttributeDataType, int>(
                    s => true,
                    "Id",
                    (lang == 1) ? "DataTypeCaption" : "DataTypeName",
                    lastSeenId,
                    pageSize
                );

                var selectList = new SelectList(ddlResult.Items, "Value", "Name");

                var pagedResult = new DDLDataPagedResult
                {
                    Items = selectList,
                    LastSeenId = ddlResult.LastSeenId
                };

                return Ok(await Result<DDLDataPagedResult>.SuccessAsync(pagedResult, ""));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLUsersPagination")]
        public async Task<IActionResult> DDLUsersPagination(long lastSeenId = 0, int pageSize = Pagination.DDLpageSize)
        {
            try
            {
                var ddlResult = await listHelper.GetRawListPagination<SysUser, long>(
                    u => u.IsDeleted == false
                         && u.Enable == 1
                         && u.FacilityId == session.FacilityId,
                    "Id",
                    "UserFullname",
                    lastSeenId,
                    pageSize
                );

                var selectList = new SelectList(ddlResult.Items, "Value", "Name");

                var pagedResult = new DDLDataPagedResult
                {
                    Items = selectList,
                    LastSeenId = ddlResult.LastSeenId
                };

                return Ok(await Result<DDLDataPagedResult>.SuccessAsync(pagedResult, ""));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
        [HttpGet("DDLSysGroupPagination")]
        public async Task<IActionResult> DDLSysGroupPagination(int lang = 1, int lastSeenId = 0, int pageSize = Pagination.DDLpageSize)
        {
            try
            {
                var ddlResult = await listHelper.GetRawListPagination<SysGroup, int>(
                    b => b.IsDeleted == false && b.FacilityId == session.FacilityId,
                    "GroupId",
                    lang == 1 ? "GroupName" : "GroupName2",
                    lastSeenId,
                    pageSize
                );

                var selectList = new SelectList(ddlResult.Items, "Value", "Name");

                var pagedResult = new DDLDataPagedResult
                {
                    Items = selectList,
                    LastSeenId = ddlResult.LastSeenId
                };

                return Ok(await Result<DDLDataPagedResult>.SuccessAsync(pagedResult, ""));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLSysExchangeRateListPagination")]
        public async Task<IActionResult> DDLSysExchangeRateListPagination(int lang = 1, int lastSeenId = 0, int pageSize = Pagination.DDLpageSize)
        {
            try
            {
                var ddlResult = await listHelper.GetRawListPagination<SysExchangeRateListVW, int>(
                    d => d.IsDeleted == false,
                    "Id",
                    lang == 1 ? "Name" : "Name2",
                    lastSeenId,
                    pageSize
                );

                var selectList = new SelectList(ddlResult.Items, "Value", "Name");

                var pagedResult = new DDLDataPagedResult
                {
                    Items = selectList,
                    LastSeenId = ddlResult.LastSeenId
                };

                return Ok(await Result<DDLDataPagedResult>.SuccessAsync(pagedResult, ""));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }



        [HttpGet("DrpDepartment3")]
        public async Task<IActionResult> DrpDepartment3(int lang = 1)
        {

            try
            {
                var list = await ddlHelper.GetAnyLis<SysDepartmentVw, long>(x => x.IsDeleted == false && x.TypeId == 1 && (x.IsShare == true || x.FacilityId == session.FacilityId), "Id", lang == 1 ? "Name" : "Name2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
        // مستخدمة في شاشة العقود /HR/Contract/Contract
        [HttpGet("DrpDepartment2")]
        public async Task<IActionResult> DrpDepartment2(int lang = 1)
        {

            try
            {
                var list = await ddlHelper.GetAnyLis<SysDepartmentVw, long>(x => x.IsDeleted == false && x.TypeId == 1 && x.FacilityId == session.FacilityId, "Id", lang == 1 ? "Name" : "Name2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }












        #endregion ========== End lists From main tables (Sys)














        #region  ============================================ Drop down list From HR tables ============================================

        [HttpGet("DDLEmployees")]
        public async Task<IActionResult> DDLEmployees(int lang = 1)
        {
            try
            {
                var list = new SelectList(new List<DDListItem<Logix.Domain.Main.InvestEmployee>>());
                var branchesId = session.Branches.Split(',');

                list = await ddlHelper.GetAnyLis<Logix.Domain.Main.InvestEmployee, long>(u => u.IsDeleted == false
                && u.FacilityId == session.FacilityId && branchesId.Contains(u.BranchId.ToString()),
                "Id", lang == 1 ? "EmpName" : "EmpName2");

                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }


        [HttpGet("GetDDLByCategoryId")]
        public async Task<IActionResult> GetDDLByCategoryId(int id, int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetByCategoryId(id, lang);
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLFacilities")]
        public async Task<IActionResult> DDLFacilities(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<AccFacility, int>(b => b.IsDeleted == false, "FacilityId", lang == 1 ? "FacilityName" : "FacilityName2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
        [HttpGet("DDLJobLevel")]
        public async Task<IActionResult> DDLJobLevel()
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<HrJobLevel, long>(b => b.IsDeleted == false, "Id", "LevelName");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLJobGroups")]
        public async Task<IActionResult> DDLJobGroups()
        {
            try
            {
                var list = new SelectList(new List<DDListItem<HrJobGroups>>());
                list = await ddlHelper.GetAnyLis<HrJobGroups, long>(s => s.IsDeleted == false & s.StatusId == 1 & s.HasSubGroup == false, "Id", (session.Language == 1) ? "Name" : "Name2");
                return Ok(await Result<SelectList>.SuccessAsync(list));

            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
        [HttpGet("DDLSector")]
        public async Task<IActionResult> DDLSector()
        {
            try
            {
                var list = new SelectList(new List<DDListItem<HrSector>>());
                list = await ddlHelper.GetAnyLis<HrSector, long>(s => s.IsDeleted == false & s.FacilityId == session.FacilityId, "Id", (session.Language == 1) ? "Name" : "Name2");
                return Ok(await Result<SelectList>.SuccessAsync(list));

            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }


        [HttpGet("DDLParentID")]
        public async Task<IActionResult> DDLParentID()
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<HrJobGroups, long>(b => b.IsDeleted == false, "Id", "Name");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLHrAttShifts")]
        public async Task<IActionResult> DDLHrAttShifts(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<HrAttShift, long>(d => d.IsDeleted == false, "Id", "Name");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLJobPrograms")]
        public async Task<IActionResult> DDLJobPrograms(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<HrJobProgramVw, long>("ProgramId", "ProgramName");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }


        [HttpGet("DDLSalaryGroups")]
        public async Task<IActionResult> DDLSalaryGroups(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<HrSalaryGroup, long>(d => d.IsDeleted == false, "Id", "Name");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }


        [HttpGet("DDLJobs")]
        public async Task<IActionResult> DDLJobs(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<HrJobVw, long>(d => d.IsDeleted == false && d.StatusId == 1 || d.StatusId == 2, "Id", "JobName");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLNewGradId")]
        public async Task<IActionResult> DDLNewGradId(int levelId)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<HrJobGrade, long>(d => d.IsDeleted == false && levelId != 0 ? d.LevelId == levelId : d.IsDeleted == false, "Id", "GradeName");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLHRJobGrade")]
        public async Task<IActionResult> DDLHRJobGrade(int levelId)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<HrJobGrade, long>(d => d.IsDeleted == false, "Id", "GradeName");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLCurJobId")]
        public async Task<IActionResult> DDLCurJobId()
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<HrJobVw, long>(d => d.IsDeleted == false, "Id", "JobName");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
        // مستخدمة في شاشة الزيادات والعلاوات
        [HttpGet("DDLNewJobId")]
        public async Task<IActionResult> DDLNewJobId(int JobCategoryId = 0)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<HrJobVw, long>(d => d.IsDeleted == false && (d.StatusId == 1 || d.StatusId == 2) && d.JobCatagoriesId == JobCategoryId, "Id", "JobName");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
        // مستخدمة في شاشة الزيادات والعلاوات
        [HttpGet("DDLNewLevelId")]
        public async Task<IActionResult> DDLNewLevelId(int LevelId = 0)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<HrJobGrade, long>(d => d.IsDeleted == false && d.LevelId == LevelId, "Id", "GradeName");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLCompetencescatagories")]
        public async Task<IActionResult> DDLCompetencescatagories(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<HrCompetencesCatagory, long>(d => d.IsDeleted == false, "Id", "Name");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
        [HttpGet("DDLMonthEndDay")]
        public async Task<IActionResult> DDLMonthEndDay(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<MonthDay, string>("DayCode", "DayCode");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLMonthStartDay")]
        public async Task<IActionResult> DDLMonthStartDay()
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<MonthDay, string>("DayCode", "DayCode");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLVacationsCatagories")]
        public async Task<IActionResult> DDLVacationsCatagories(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<HrVacationsCatagory, int>(b => b.IsDeleted == false, "CatId", lang == 1 ? "CatName" : "CatName2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLRateType")]
        public async Task<IActionResult> DDLRateType(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<HrRateTypeVw, int>(b => b.IsDeleted == false, "Id", lang == 1 ? "RateName" : "RateName2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }




        [HttpGet("DDlDisciplinaryCaseID")]
        public async Task<IActionResult> DDlDisciplinaryCaseID(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<HrDisciplinaryCase, long>(d => d.IsDeleted == false, "Id", lang == 1 ? "CaseName" : "CaseName2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDlActionType")]
        public async Task<IActionResult> DDlActionType(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<HrDisciplinaryActionType, long>("Id", lang == 1 ? "ActionName" : "ActionName2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
        [HttpGet("DDLLocationsID")]
        public async Task<IActionResult> DDLLocationsID(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<SysDepartment, long>(d => d.TypeId == 2 && d.IsDeleted == false, "Id", lang == 1 ? "Name" : "Name2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
        //مستخدمه في شاشة اضافة وظيفة اضافية + الموظفين تحت الاجراء

        [HttpGet("DDLLocationProject")]
        public async Task<IActionResult> DDLLocationProject(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<SysDepartment, long>(d => d.IsDeleted == false && d.TypeId == 2 && d.StatusId == 1, "Id", lang == 1 ? "Name" : "Name2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("EmpIdChanged")]
        public async Task<IActionResult> EmpIdChanged(string EmpId)
        {

            if (string.IsNullOrEmpty(EmpId))
            {
                return Ok(await Result<EmpIdChangedVM>.SuccessAsync("there is no id passed"));
            }

            try
            {
                var checkEmpId = await mainServiceManager.InvestEmployeeService.GetOne(i => i.EmpId == EmpId && i.Isdel == false);
                if (checkEmpId.Succeeded)
                {
                    if (checkEmpId.Data != null)
                    {
                        var item = new EmpIdChangedVM
                        {
                            EmpId = checkEmpId.Data.EmpId,
                            EmpName = checkEmpId.Data.EmpName,
                            BankId = checkEmpId.Data.BankId,
                            BranchId = checkEmpId.Data.BranchId,
                            Gender = checkEmpId.Data.Gender,
                            Iban = checkEmpId.Data.Iban,
                            IdNo = checkEmpId.Data.IdNo,
                            NationalityId = checkEmpId.Data.NationalityId,
                        };
                        return Ok(await Result<EmpIdChangedVM>.SuccessAsync(item));
                    }
                    else
                    {
                        return Ok(await Result<EmpIdChangedVM>.SuccessAsync($"There is No Employee with this Id:  {EmpId}"));

                    }
                }
                return Ok(await Result<EmpIdChangedVM>.FailAsync($"{checkEmpId.Status.message}"));
            }
            catch (Exception exp)
            {
                return Ok(await Result<EmpIdChangedVM>.FailAsync($"{exp.Message}"));
            }
        }


        [HttpGet("DDLPolicyID")]
        public async Task<IActionResult> DDLPolicyID(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<HrInsurancePolicy, long>(d => d.IsDeleted == false, "Id", "Name");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }


        [HttpGet("HRDependent")]
        public async Task<IActionResult> HrDependent(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<HrDependent, long>(d => d.IsDeleted == false, "Id", "Name");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }



        [HttpGet("DDLJobCity")]
        public async Task<IActionResult> DDLJobCity(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<SysCites, long>(b => b.IsDeleted == false && b.CountryID == 1, "CityID", lang == 1 ? "CityName" : "CityName2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }


        [HttpGet("DDLVacancy")]
        public async Task<IActionResult> DDLVacancy()
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<HrRecruitmentVacancy, long>(d => d.StatusId == 2 && d.IsDeleted == false, "Id", "VacancyName");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }



        [HttpGet("DDLJobCountry")]
        public async Task<IActionResult> DDLJobCountry(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<SysCountryVw, long>(d => d.Isdel == false, "CountryId", (lang == 1) ? "CountryName" : "CountryName2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }





        [HttpGet("DDlStatusID")]
        public async Task<IActionResult> DDlStatusID()
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<HrVacancyStatusVw, long>(b => b.Isdel == false, "StatusId", "StatusName");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDlVacations")]
        public async Task<IActionResult> DDlVacations(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<HrVacationsTypeVw, int>(b => b.IsDeleted == false, "VacationTypeId", lang == 1 ? "VacationTypeName" : "VacationTypeName2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLVacationsDayType")]
        public async Task<IActionResult> DDLVacationsDayType(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<HrVacationsDayType, int>(b => b.IsDeleted == false, "Id", lang == 1 ? "Name" : "Name2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLTemplateID")]
        public async Task<IActionResult> DDLTemplateID(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<HrKpiTemplate, long>(d => d.IsDeleted == false, "Id", lang == 1 ? "Name" : "Name2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }







        [HttpGet("EmpNameChanged")]
        public async Task<IActionResult> EmpNameChanged(string EmpName)
        {

            if (string.IsNullOrEmpty(EmpName))
            {
                return Ok(await Result<EmpIdChangedVM>.SuccessAsync("there is No Name passed"));
            }

            try
            {
                var checkEmpName = await mainServiceManager.InvestEmployeeService.GetOne(i => i.EmpName == EmpName && i.Isdel == false);
                if (checkEmpName.Succeeded)
                {
                    if (checkEmpName.Data != null)
                    {
                        var item = new EmpIdChangedVM
                        {
                            EmpId = checkEmpName.Data.EmpId,
                            EmpName = checkEmpName.Data.EmpName,
                            BankId = checkEmpName.Data.BankId,
                            BranchId = checkEmpName.Data.BranchId,
                            Gender = checkEmpName.Data.Gender,
                            Iban = checkEmpName.Data.Iban,
                            IdNo = checkEmpName.Data.IdNo,
                            NationalityId = checkEmpName.Data.NationalityId,
                        };
                        return Ok(await Result<EmpIdChangedVM>.SuccessAsync(item));
                    }
                    else
                    {
                        return Ok(await Result<EmpIdChangedVM>.SuccessAsync($"There is No Employee with this Name:  {EmpName}"));

                    }
                }
                return Ok(await Result<EmpIdChangedVM>.FailAsync($"{checkEmpName.Status.message}"));
            }
            catch (Exception exp)
            {
                return Ok(await Result<EmpIdChangedVM>.FailAsync($"{exp.Message}"));
            }
        }



        [HttpGet("DDLDecisionsType")]
        public async Task<IActionResult> DDLDecisionsType()
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<HrDecisionsType, int>(b => b.IsDeleted == false, "Id", "DecType");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLAttLocationsID")]
        public async Task<IActionResult> DDLAttLocationsID()
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<HrAttLocation, int>(d => d.IsDeleted == false, "Id", "LocationName");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }



        [HttpGet("DDLTimeTable")]
        public async Task<IActionResult> DDLTimeTable()
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<HrAttTimeTable, long>(d => d.IsDeleted == false, "Id", "TimeTableName");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }



        [HttpGet("GetEmpIdByCode")]
        public async Task<IActionResult> GetEmpIdByCode(string empCode)
        {

            if (string.IsNullOrEmpty(empCode))
            {
                return Ok(await Result<string>.SuccessAsync("there is empCode  passed"));
            }
            try
            {
                var checkEmpId = await mainServiceManager.InvestEmployeeService.GetOne(e => e.Id, i => i.EmpId == empCode && i.Isdel == false && i.IsDeleted == false);
                if (checkEmpId.Succeeded)
                {
                    if (checkEmpId.Data != null)
                    {

                        return Ok(await Result<long?>.SuccessAsync(checkEmpId.Data));
                    }
                    else
                    {
                        return Ok(await Result<string>.SuccessAsync($"There is No Employee with this Id:  {empCode}"));

                    }
                }
                return Ok(await Result<string>.FailAsync($"{checkEmpId.Status.message}"));
            }
            catch (Exception exp)
            {
                return Ok(await Result<string>.FailAsync($"{exp.Message}"));
            }
        }


        [HttpGet("DDlDisciplinaryCaseIDForNormalAdd")]
        public async Task<IActionResult> DDlDisciplinaryCaseIDForNormalAdd(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<HrDisciplinaryCase, long>(d => d.IsDeleted == false && d.Id == 5, "Id", lang == 1 ? "CaseName" : "CaseName2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDlDisciplinaryCaseIDForIntervalAdd")]
        public async Task<IActionResult> DDlDisciplinaryCaseIDForIntervalAdd(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<HrDisciplinaryCase, long>(d => d.IsDeleted == false && d.Id >= 5, "Id", lang == 1 ? "CaseName" : "CaseName2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }


        [HttpGet("DDLOpeningBalanceType")]
        public async Task<IActionResult> DDLOpeningBalanceType(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<HrOpeningBalanceType, int>(b => b.IsDeleted == false, "TypeId", lang == 1 ? "TypeName" : "TypeName2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }



        [HttpGet("DDLPayrollType1")]
        public async Task<IActionResult> DDLPayrollType1(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<HrPayrollType, int>(x => x.IsDeleted == false, "Id", lang == 1 ? "TypeName" : "TypeName2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLPayrollType2")]
        public async Task<IActionResult> DDLPayrollType2(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<HrPayrollType, int>(x => x.IsDeleted == false && x.Id != 1, "Id", lang == 1 ? "TypeName" : "TypeName2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }


        [HttpGet("DDLTransType")]
        public async Task<IActionResult> DDLTransType()
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<SysTable, int>(x => x.SystemId == "3", "TableId", "TableDescription");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }





        [HttpGet("DDLDepartmentID")]
        public async Task<IActionResult> DDLDepartmentID(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<SysDepartment, long>(d => d.TypeId == 1 && d.IsDeleted == false, "Id", lang == 1 ? "Name" : "Name2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }





        [HttpGet("EvaluationStatus")]
        public async Task<IActionResult> EvaluationStatus(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<HrPerformanceStatusVw, long>(x => x.Isdel == false, "StatusId", lang == 1 ? "StatusName" : "StatusName2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
        [HttpGet("EvaluationFor")]
        public async Task<IActionResult> EvaluationFor(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<HrPerformanceForVw, long>(x => x.Isdel == false, "EvaluationId", lang == 1 ? "EvaluationName" : "EvaluationName2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
        [HttpGet("DDLPerformance")]
        public async Task<IActionResult> DDLPerformance()
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<HrPerformanceVw, long>(x => x.IsDeleted == false, "Id", "Description");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        //  حالة التقييم
        [HttpGet("DDLEvaluationStatus")]
        public async Task<IActionResult> DDLEvaluationStatus()
        {
            try
            {
                List<SelectListItem> lististItems = new List<SelectListItem>();
                lististItems.Insert(0, new SelectListItem
                { Value = "0", Text = localization.GetResource1("All") });
                lististItems.Insert(1, new SelectListItem
                { Value = "1", Text = localization.GetHrResource("New") });
                lististItems.Insert(2, new SelectListItem
                { Value = "2", Text = localization.GetHrResource("Approve") });
                lististItems.Insert(3, new SelectListItem
                { Value = "3", Text = localization.GetHrResource("Reject") });
                return Ok(await Result<object>.SuccessAsync(lististItems, ""));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }

        }

        [HttpGet("DDLEvaluationDone")]
        public async Task<IActionResult> DDLEvaluationDone()
        {
            try
            {
                List<SelectListItem> lististItems = new List<SelectListItem>();
                lististItems.Insert(0, new SelectListItem
                { Value = "0", Text = localization.GetResource1("All") });
                lististItems.Insert(1, new SelectListItem
                { Value = "1", Text = localization.GetHrResource("Evaluationed") });
                lististItems.Insert(2, new SelectListItem
                { Value = "2", Text = localization.GetHrResource("NonEvaluationed") });
                return Ok(await Result<object>.SuccessAsync(lististItems, ""));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }

        }


        [HttpGet("DDLClearanceType")]
        public async Task<IActionResult> DDLClearanceType(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<HrClearanceTypeVw, int>(x => x.IsDeleted == false, "TypeId", lang == 1 ? "TypeName" : "TypeName2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        // احتساب الإضافي من
        [HttpGet("DDLOverTimeFrom")]
        public async Task<IActionResult> DDLOverTimeFrom()
        {
            try
            {
                List<SelectListItem> lististItems = new List<SelectListItem>();
                lististItems.Insert(0, new SelectListItem
                { Value = "1", Text = localization.GetHrResource("BasicSalary") });
                lististItems.Insert(1, new SelectListItem
                { Value = "2", Text = localization.GetHrResource("TotalSalary") });
                lististItems.Insert(2, new SelectListItem
                { Value = "3", Text = localization.GetHrResource("policylabor") });
                lististItems.Insert(3, new SelectListItem
                { Value = "4", Text = localization.GetHrResource("policyCompany") });
                return Ok(await Result<object>.SuccessAsync(lististItems, ""));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        // قراءات ساعات العمل
        [HttpGet("DDLHoursReadings")]
        public async Task<IActionResult> DDLHoursReadings()
        {
            try
            {
                List<SelectListItem> lististItems = new List<SelectListItem>();
                lististItems.Insert(0, new SelectListItem
                { Value = "0", Text = "من الوردية" });
                lististItems.Insert(1, new SelectListItem
                { Value = "1", Text = "من ملف الموظف" });
                lististItems.Insert(2, new SelectListItem
                { Value = "2", Text = "يدوي" });
                return Ok(await Result<object>.SuccessAsync(lististItems, ""));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        // نوع البصمة
        [HttpGet("DDLFingerprintType")]
        public async Task<IActionResult> DDLFingerprintType()
        {
            try
            {
                List<SelectListItem> lististItems = new List<SelectListItem>();
                lististItems.Insert(0, new SelectListItem
                { Value = "0", Text = "الكل" });
                lististItems.Insert(1, new SelectListItem
                { Value = "1", Text = "دخول وخروج" });
                lististItems.Insert(2, new SelectListItem
                { Value = "2", Text = "دخول فقط" });
                return Ok(await Result<object>.SuccessAsync(lististItems, ""));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLProbationTemplate")]
        public async Task<IActionResult> DDLProbationTemplate(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<HrKpiTemplate, long>(d => d.IsDeleted == false && d.TypeId == 2, "Id", lang == 1 ? "Name" : "Name2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLPrimaryTemplate")]
        public async Task<IActionResult> DDLPrimaryTemplate(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<HrKpiTemplate, long>(d => d.IsDeleted == false && d.TypeId == 1, "Id", lang == 1 ? "Name" : "Name2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }


        [HttpGet("DDLFinYear2")]
        public async Task<IActionResult> DDLFinYear2()
        {
            try
            {
                var FinYear = await accServiceManager.AccFinancialYearService.GetAll(x => x.IsDeleted == false && x.FacilityId == session.FacilityId);
                var DrpFinYear = listHelper.GetFromList<long>(FinYear.Data.Select(s => new DDListItem<long> { Name = s.FinYearGregorian.ToString(), Value = s.FinYearGregorian }), hasDefault: false);
                return Ok(await Result<SelectList>.SuccessAsync(DrpFinYear));
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync(ex.Message));
            }
        }



        [HttpGet("DDLRefranceType")]
        public async Task<IActionResult> DDLRefranceType(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<AccReferenceTypeVw, int>(x => x.FlagDelete == false && x.ParentId == 8 && x.ReferenceTypeId != x.ParentId, "ReferenceTypeId", lang == 1 ? "ReferenceTypeName" : "ReferenceTypeName2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync(ex.Message));
            }
        }


        [HttpGet("DDLExpensesType")]
        public async Task<IActionResult> DDLExpensesType(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<HrExpensesTypeVw, long>(x => x.IsDeleted == false && x.FacilityId == session.FacilityId, "Id", lang == 1 ? "Name" : "Name2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLVacancyForRecruitment")]
        public async Task<IActionResult> DDLVacancyForRecruitment()
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<HrRecruitmentVacancy, long>(d => d.IsDeleted == false, "Id", "VacancyName");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLPeriodsDate")]
        public async Task<IActionResult> DDLPeriodsDate(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<AccPeriodDateVws, long>(d => d.PeriodState == 1 && d.FacilityId == session.FacilityId && d.FinYear == session.FinYear, "PeriodId", lang == 1 ? "PeriodDate" : "PeriodDate2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
        [HttpGet("DDLTemplate")]
        public async Task<IActionResult> DDLTemplate()
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<SysTemplate, int>(d => d.IsDeleted == false && d.SystemId == 3 && d.ScreenId == 732, "Id", "Name");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLTimeZone")]
        public async Task<IActionResult> DDLTimeZone()
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<HrTimeZone, int>(d => d.IsDeleted == false, "Id", "DisplayName");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }


        [HttpGet("DDLStructure")]
        public async Task<IActionResult> DDLStructure(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<HrStructureVw, int>(d => d.IsDeleted == false && d.StatusId == 1, "Id", lang == 1 ? "Name" : "Name2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDSubject")]
        public async Task<IActionResult> DDSubject(int lang = 1)
        {
            try
            {
                var SubjectTypeList = await hrServiceManager.HrNotificationsTypeService.GetAll(d => d.SubjectType, d => d.IsDeleted == false && d.FacilityId == session.FacilityId && d.SubjectType != null);
                var list = await ddlHelper.GetAnyLis<SysLookupDataVw, long>(d => d.Isdel == false && d.CatagoriesId == 469 && !SubjectTypeList.Data.Contains((int)d.Code), "Id", lang == 1 ? "Name" : "Name2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
        #endregion


















    }
}
