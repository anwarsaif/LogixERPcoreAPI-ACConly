using Logix.Application.Common;
using Logix.Application.DTOs.ACC;
using Logix.Application.DTOs.CRM;
using Logix.Application.DTOs.FXA;
using Logix.Application.DTOs.Main;
using Logix.Application.DTOs.WF;
using Logix.Application.Helpers;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;
using Logix.Domain.CRM;
using Logix.Domain.FXA;
using Logix.Domain.HR;
using Logix.Domain.Integra;
using Logix.Domain.Main;
using Logix.Domain.PM;
using Logix.Domain.PUR;
using Logix.Domain.RPT;
using Logix.Domain.SAL;
using Logix.Domain.TS;
using Logix.Domain.WF;
using Logix.Domain.WH;
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
    public class DDLApiController : ControllerBase
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

        public DDLApiController(IApiDDLHelper ddlHelper,
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
        [HttpGet("DDLSystems")]
        public async Task<IActionResult> DDLSystems(int lang = 1)
        {
            try
            {
                var list = new SelectList(new List<DDListItem<SysSystemDto>>());
                list = await ddlHelper.GetAnyLis<SysSystem, long>(s => s.Isdel == false, "SystemId", (lang == 1) ? "SystemName" : "SystemName2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLScreens")]
        public async Task<IActionResult> DDLScreens(int lang = 1)
        {
            try
            {
                var list = new SelectList(new List<DDListItem<SysScreen>>());
                var systemsIds = await mainServiceManager.SysSystemService.GetAll(s => s.SystemId, s => s.Isdel == false);
                if (systemsIds.Succeeded)
                {
                    List<int> systemsIdsArr = new();
                    systemsIdsArr.AddRange(systemsIds.Data);

                    list = await ddlHelper.GetAnyLis<SysScreen, long>(s => s.Isdel == false && systemsIdsArr.Contains(s.SystemId ?? 0), "ScreenId", (lang == 1) ? "ScreenName" : "ScreenName2");
                }

                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLSystemScreens")]
        public async Task<IActionResult> DDLSystemScreens(int systemId, int lang = 1)
        {
            try
            {
                if (systemId > 0)
                {
                    var list = new SelectList(new List<DDListItem<SysScreen>>());
                    list = await ddlHelper.GetAnyLis<SysScreen, long>(s => s.SystemId == systemId && s.ParentId != s.ScreenId && s.Isdel == false, "ScreenId", lang == 1 ? "ScreenName" : "ScreenName2");
                    return Ok(await Result<SelectList>.SuccessAsync(list));
                }

                return Ok(await Result.FailAsync("Invalid System Id"));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLParentScreens")]
        public async Task<IActionResult> DDLParentScreens(int systemId, int lang = 1)
        {
            try
            {
                if (systemId > 0)
                {
                    var list = new SelectList(new List<DDListItem<SysScreen>>());
                    list = await ddlHelper.GetAnyLis<SysScreen, long>(s => s.SystemId == systemId && s.ParentId == s.ScreenId && s.Isdel == false, "ScreenId", lang == 1 ? "ScreenName" : "ScreenName2");
                    return Ok(await Result<SelectList>.SuccessAsync(list));
                }

                return Ok(await Result.FailAsync("Invalid System Id"));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLChildScreens")]
        public async Task<IActionResult> DDLChildScreens(long parentId)
        {
            try
            {
                if (parentId > 0)
                {
                    var list = new SelectList(new List<DDListItem<SysScreen>>());
                    list = await ddlHelper.GetAnyLis<SysScreen, long>(s => s.ParentId != s.ScreenId && s.ParentId == parentId && s.Isdel == false, "ScreenId", (session.Language == 1) ? "ScreenName" : "ScreenName2");
                    return Ok(await Result<SelectList>.SuccessAsync(list));
                }

                return Ok(await Result.FailAsync("Invalid System Id"));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLBranches")]
        public async Task<IActionResult> DDLBranches(int lang = 1)
        {
            try
            {
                var braList = session.Branches.Split(",").ToList();
                var list = await ddlHelper.GetAnyLis<InvestBranchVw, int>(b => b.Isdel == false && b.FacilityId == session.FacilityId
                    && braList.Contains(b.BranchId.ToString()),
                    "BranchId", lang == 1 ? "BraName" : "BraName2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLBranchesByFacilityId")]
        public async Task<IActionResult> DDLBranchesByFacilityId(long facilityId, int lang = 1)
        {
            try
            {
                if (facilityId > 0)
                {
                    var braList = session.Branches.Split(",").ToList();
                    var list = await ddlHelper.GetAnyLis<InvestBranch, int>(s => s.FacilityId == facilityId && s.Isdel == false
                        && braList.Contains(s.BranchId.ToString()),
                        "BranchId", lang == 1 ? "BraName" : "BraName2");
                    return Ok(await Result<SelectList>.SuccessAsync(list));
                }

                return Ok(await Result.FailAsync("Invalid facility Id"));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLMainCategoriesBySysId")]
        public async Task<IActionResult> DDLMainCategoriesBySysId(string systemId)
        {
            try
            {
                if (systemId != "0")
                {
                    var list = new SelectList(new List<DDListItem<SysLookupCategory>>());
                    list = await ddlHelper.GetAnyLis<SysLookupCategory, long>(s => s.SystemId == systemId && s.Isdel == false, "CatagoriesId", (session.Language == 1) ? "CatagoriesName" : "CatagoriesName2");
                    return Ok(await Result<SelectList>.SuccessAsync(list));
                }

                return Ok(await Result.FailAsync("Invalid System Id"));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLLookupCatagories")]
        public async Task<IActionResult> DDLLookupCatagories(int lang = 1)
        {
            try
            {
                var list = new SelectList(new List<DDListItem<SysLookupCategory>>());
                list = await ddlHelper.GetAnyLis<SysLookupCategory, int>(s => s.Isdel == false, "CatagoriesId", (lang == 1) ? "CatagoriesName" : "CatagoriesName2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLCities")]
        public async Task<IActionResult> DDLCities(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<SysCites, int>(b => b.IsDeleted == false, "CityID", lang == 1 ? "CityName" : "CityName2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLCountries")]
        public async Task<IActionResult> DDLCountries(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<SysCites, int>(b => b.TypeID == 1 && b.IsDeleted == false, "CityID", lang == 1 ? "CityName" : "CityName2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLDepartmentCategories")]
        public async Task<IActionResult> DDLDepartmentCategories(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<SysDepartmentCatagory, int>("Id", lang == 1 ? "CatName" : "CatName");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLDepartmentsParent")]
        public async Task<IActionResult> DDLDepartmentsParent(int lang = 1, int typeId = 0)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<SysDepartment, int>(d => d.FacilityId == session.FacilityId && d.IsDeleted == false && d.Id != d.ParentId && (typeId != 0 && d.TypeId == typeId), "Id", lang == 1 ? "Name" : "Name2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLSysGroups")]
        public async Task<IActionResult> DDLSysGroups(int lang = 1)
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
                var list = await ddlHelper.GetAnyLis<SysGroup, int>(b => b.IsDeleted == false && b.FacilityId == session.FacilityId && groupIds.Contains(b.GroupId),
                    "GroupId", lang == 1 ? "GroupName" : "GroupName2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLDepartments")]
        public async Task<IActionResult> DDLDepartments(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<SysDepartment, long>((d => d.TypeId == 1 && d.IsDeleted == false && d.StatusId == 1 && (d.IsShare == true || d.FacilityId == session.FacilityId)), "Id", (lang == 1) ? "Name" : "Name2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
        //مستخدمه في شاشة اضافة وظيفة اضافية
        [HttpGet("DDLDepartment")]
        public async Task<IActionResult> DDLDepartment(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<SysDepartment, long>(d => d.IsDeleted == false && d.TypeId == 1 && d.StatusId == 1, "Id", (lang == 1) ? "Name" : "Name2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLLocations")]
        public async Task<IActionResult> DDLLocations(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<SysDepartment, long>(d => d.TypeId == 2 && d.IsDeleted == false && d.StatusId == 1 && (d.IsShare == true || d.FacilityId == session.FacilityId), "Id", lang == 1 ? "Name" : "Name2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLLocationsVw")]
        public async Task<IActionResult> DDLLocationsVw(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<SysDepartmentVw, long>(d => d.TypeId == 2 && d.IsDeleted == false && (d.FacilityId == session.FacilityId), "Id", lang == 1 ? "Name" : "Name2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        // مستخدمة في شاشة الموظفين /HR/Employee/Employee
        [HttpGet("DDLLocation")]
        public async Task<IActionResult> DDLLocation()
        {
            try
            {
                var lang = session.Language;
                var list = await ddlHelper.GetAnyLis<SysDepartmentVw, long>(d => d.TypeId == 2 && d.IsDeleted == false && d.StatusId == 1 && (d.IsShare == true || d.FacilityId == session.FacilityId), "Id", lang == 1 ? "Name" : "Name2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        // مستخدمة في شاشة مباشرة العمل /HR/JoinWork/JoinWork
        [HttpGet("DDLLocationPagination")]
        public async Task<IActionResult> DDLLocationPagination(int lang = 1, long lastSeenId = 0, int pageSize = Pagination.DDLpageSize)
        {
            try
            {
                var ddlResult = await listHelper.GetRawListPagination<SysDepartmentVw, long>(
                    d => d.TypeId == 2 && d.IsDeleted == false && d.StatusId == 1 && (d.IsShare == true || d.FacilityId == session.FacilityId),
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
        [HttpGet("DrpDepartment")]
        public async Task<IActionResult> DrpDepartment(int lang = 1)
        {

            try
            {
                var list = await ddlHelper.GetAnyLis<SysDepartmentVw, long>(x => x.IsDeleted == false && x.TypeId == 1 && x.StatusId == 1 && (x.IsShare == true || x.FacilityId == session.FacilityId), "Id", lang == 1 ? "Name" : "Name2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
        // مستخدمة في شاشة عقد جديد /HR/Contract/Contract_add2
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

        [HttpGet("DDLCustomerTypes")]
        public async Task<IActionResult> DDLCustomerTypes()
        {
            try
            {
                var list = new SelectList(new List<DDListItem<SysCustomerTypeDto>>());
                list = await ddlHelper.GetAnyLis<SysCustomerType, long>("TypeId", "CusTypeName");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLCurrency")]
        public async Task<IActionResult> DDLCurrency(int lang = 1)
        {
            try
            {
                var list = new SelectList(new List<DDListItem<SysExchangeRateListVW>>());
                list = await ddlHelper.GetAnyLis<SysExchangeRateListVW, int>(d => d.IsDeleted == false, "Id", lang == 1 ? "Name" : "Name2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLAnnouncementLocations")]
        public async Task<IActionResult> DDLAnnouncementLocations()
        {
            try
            {
                var list = new SelectList(new List<DDListItem<SysAnnouncementLocationVw>>());
                list = await ddlHelper.GetAnyLis<SysAnnouncementLocationVw, long>(s => s.Isdel == false, "Id", "LocationName");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLSysTables")]
        public async Task<IActionResult> DDLSysTables()
        {
            try
            {
                var list = new SelectList(new List<DDListItem<SysTable>>());
                list = await ddlHelper.GetAnyLis<SysTable, int>("TableId", "TableDescription");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLSysTableFields")]
        public async Task<IActionResult> DDLSysTableFields(long tableId)
        {
            try
            {
                var list = new SelectList(new List<DDListItem<SysTableField>>());
                list = await ddlHelper.GetAnyLis<SysTableField, long>(f => f.TableId == tableId, "Id", session.Language == 1 ? "Desc1" : "Desc2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLDynamicAttributeDataType")]
        public async Task<IActionResult> DDLDynamicAttributeDataType(int lang = 1)
        {
            try
            {
                var list = new SelectList(new List<DDListItem<SysDynamicAttributeDataType>>());
                list = await ddlHelper.GetAnyLis<SysDynamicAttributeDataType, int>("Id", (lang == 1) ? "DataTypeCaption" : "DataTypeName");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLUsers")]
        public async Task<IActionResult> DDLUsers()
        {
            try
            {
                var list = new SelectList(new List<DDListItem<SysUser>>());
                list = await ddlHelper.GetAnyLis<SysUser, long>(u => u.IsDeleted == false && u.Enable == 1
                && u.FacilityId == session.FacilityId, "Id", "UserFullname");

                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLUserType")]
        public async Task<IActionResult> DDLUserType()
        {
            try
            {
                var list = new SelectList(new List<DDListItem<SysUserTypeDto>>());
                list = await ddlHelper.GetAnyLis<SysUserType, int>("UserTypeId", "UserTypeName");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLUserType2")]
        public async Task<IActionResult> DDLUserType2(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<SysUserType2, int>("Id", lang == 1 ? "Name" : "Name2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLActivityType")]
        public async Task<IActionResult> DDLActivityType()
        {
            try
            {
                // var list = new SelectList(new List<DDListItem<SysActivityType>>());
                var list = await ddlHelper.GetAnyLis<SysActivityType, int>("ActivityTypeID", "ActivityType");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLSysGroup")]
        public async Task<IActionResult> DDLSysGroup(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<SysGroup, int>(b => b.IsDeleted == false && b.FacilityId == session.FacilityId,
                    "GroupId", lang == 1 ? "GroupName" : "GroupName2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLSysExchangeRateList")]
        public async Task<IActionResult> DDLSysExchangeRateList(int lang = 1)
        {
            //Sys_Exchange_Rate_List_VW
            try
            {
                var list = await ddlHelper.GetAnyLis<SysExchangeRateListVW, int>(d => d.IsDeleted == false, "Id", (lang == 1) ? "Name" : "Name2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLSysMailServer")]
        public async Task<IActionResult> DDLSysMailServer()
        {
            try
            {
                var list = new SelectList(new List<DDListItem<SysMailServerDto>>());
                list = await ddlHelper.GetAnyLis<SysMailServer, long>(d => d.IsDeleted == false, "Id", "Description");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLSysMethodTypeApi")]
        public async Task<IActionResult> DDLSysMethodTypeApi()
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<SysMethodTypeApi, long>(d => d.IsDeleted == false, "Id", "Name");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLSysProcessScreenWebHook")]
        public async Task<IActionResult> DDLSysProcessScreenWebHook()
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<SysProcessScreenWebHook, long>(d => d.IsDeleted == false, "Id", "Name");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLSysWebHookAuth")]
        public async Task<IActionResult> DDLSysWebHookAuth()
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<SysWebHookAuth, long>(d => d.IsDeleted == false, "Id", "Name");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        #endregion ========== End lists From main tables (Sys)


        #region ======================================== Drop down list From WF tables (WF) ========================================

        [HttpGet("DDLWfDynamicDataType")]
        public async Task<IActionResult> DDLWfDynamicDataType(int lang = 1)
        {
            try
            {
                var list = new SelectList(new List<DDListItem<WfDynamicAttributeDataTypeDto>>());
                list = await ddlHelper.GetAnyLis<WfDynamicAttributeDataType, int>("Id", lang == 1 ? "DataTypeCaption" : "DataTypeName");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLWfLayoutAttribute")]
        public async Task<IActionResult> DDLWfLayoutAttribute(int lang = 1)
        {
            try
            {
                var list = new SelectList(new List<DDListItem<WfLayoutAttribute>>());
                list = await ddlHelper.GetAnyLis<WfLayoutAttribute, int>(x => x.IsDeleted == false, "Id", lang == 1 ? "Name" : "Name2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLWfLookupType")]
        public async Task<IActionResult> DDLWfLookupType(int lang = 1)
        {
            try
            {
                var list = new SelectList(new List<DDListItem<WfLookupTypeDto>>());
                list = await ddlHelper.GetAnyLis<WfLookupType, long>("Id", (lang == 1) ? "TypeName" : "TypeName2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLWfLookup")]
        public async Task<IActionResult> DDLWfLookup(int lang = 1)
        {
            try
            {
                var list = new SelectList(new List<DDListItem<WfLookUpCatagoryDto>>());
                list = await ddlHelper.GetAnyLis<WfLookUpCatagory, int>(x => x.IsDeleted == false, "CatagoriesId", (lang == 1) ? "CatagoriesName" : "CatagoriesName2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLWfTypeTbl")]
        public async Task<IActionResult> DDLWfTypeTbl()
        {
            try
            {
                var list = new SelectList(new List<DDListItem<WfAppTypeTableDto>>());
                list = await ddlHelper.GetAnyLis<WfAppTypeTable, long>("Id", "Name");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLWfAppGroup")]
        public async Task<IActionResult> DDLWfAppGroup(int lang = 1)
        {
            try
            {
                var list = new SelectList(new List<DDListItem<WfAppGroupDto>>());
                var allGroups = await wfServiceManager.WfAppGroupService.GetAll(x => x.IsDeleted == false);
                if (allGroups.Succeeded && allGroups.Data.Any())
                {
                    var groups = allGroups.Data.OrderBy(x => x.SortNo).ToList();
                    list = listHelper.GetFromList<long>(groups.Select(s => new DDListItem<long> { Name = lang == 1 ? s.Name ?? "" : s.Name2 ?? "", Value = Convert.ToInt64(s.Id) }), hasDefault: false);
                    return Ok(await Result<SelectList>.SuccessAsync(list));
                }
                else
                    return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLWfAppType")]
        public async Task<IActionResult> DDLWfAppType(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<WfAppType, int>(x => x.IsDeleted == false, "Id", lang == 1 ? "Name" : "Name2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLWfStepsType")]
        public async Task<IActionResult> DDLWfStepsType(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<WfStepsType, int>("Id", lang == 1 ? "Name" : "Name2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLWfAppCommittee")]
        public async Task<IActionResult> DDLWfAppCommittee(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<WfAppCommittee, long>(x => x.IsDeleted == false && x.Isactive == true,
                    "Id", lang == 1 ? "Name" : "Name2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }


        [HttpGet("DDLWfVacationType")]
        public async Task<IActionResult> DDLWfVacationType()
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<WfAppType, int>(x => x.Url != null && x.Url.Contains("Vacation"), "Id", "Name");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLWfExpensesType")]
        public async Task<IActionResult> DDLWfExpensesType()
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<WfAppType, int>(x => x.Url != null && x.IsDeleted == false && x.Url.Contains("Expenses"), "Id", "Name");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        #endregion End lists From WF tables (WF)


        #region ======================================== Drop down list From Fixed assets tables =======================================
        [HttpGet("DDLFixedAssetsTypes")]
        public async Task<IActionResult> DDLFixedAssetsTypes()
        {
            try
            {
                var list = new SelectList(new List<DDListItem<FxaFixedAssetTypeDto>>());
                list = await ddlHelper.GetAnyLis<FxaFixedAssetType, long>(t => t.FacilityId == session.FacilityId && t.IsDeleted == false, "Id", "TypeName");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLFixedAssetsParents")]
        public async Task<IActionResult> DDLFixedAssetsParents()
        {
            // all asset types that its parent id = 0
            try
            {
                var list = new SelectList(new List<DDListItem<FxaFixedAssetTypeDto>>());
                list = await ddlHelper.GetAnyLis<FxaFixedAssetType, long>(t => t.ParentId == 0 && t.FacilityId == session.FacilityId && t.IsDeleted == false, "Id", "TypeName");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLDeprecMethods")]
        public async Task<IActionResult> DDLDeprecMethods()
        {
            try
            {
                var list = new SelectList(new List<DDListItem<FxaDepreciationMethod>>());
                list = await ddlHelper.GetAnyLis<FxaDepreciationMethod, long>(t => t.IsDeleted == false, "Id", "Name");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
        #endregion ========================== End lists From Fixed assets tables ==========================


        #region  ============================================ Drop down list From WH tables ============================================
        [HttpGet("DDLWHUnit")]
        public async Task<IActionResult> DDLWHUnit(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<WhUnit, long>(d => d.IsDeleted == false, "UnitId", lang == 1 ? "UnitName" : "UnitName2");
                return Ok(await Result<SelectList>.SuccessAsync(list));

            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLWHTypeID")]
        public async Task<IActionResult> DDLWHTypeID(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<WhTransactionsType, long>(d => d.IsDeleted == false & d.TypeId == 1, "Id", lang == 1 ? "Description" : "Description");
                return Ok(await Result<SelectList>.SuccessAsync(list));

            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLWhAccountType")]
        public async Task<IActionResult> DDLWhAccountType()
        {
            try
            {
                var list = new SelectList(new List<DDListItem<WhAccountType>>());
                list = await ddlHelper.GetAnyLis<WhAccountType, int>("AccountTypeId", "AccountTypeName");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLWHItemsCatagories")]
        public async Task<IActionResult> DDLWHItemsCatagories(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<WhItemsCatagory, long>(d => d.IsDeleted == false & d.FacilityId == session.FacilityId, "CatId", (lang == 1) ? "CatName" : "CatName2");
                return Ok(await Result<SelectList>.SuccessAsync(list));

            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLWhInventorySections")]
        public async Task<IActionResult> DDLWhInventorySections(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<WhInventorySection, long>(d => d.IsDeleted == false, "Id", (lang == 1) ? "Name" : "Name2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLInventories")]
        public async Task<IActionResult> DDLInventories(int lang = 1)
        {
            try
            {
                var branchsId = session.Branches.Split(',');
                var user = session.UserId;

                var list = await ddlHelper.GetAnyLis<WhInventory, long>(x =>
                                                       x.IsDeleted == false &&
                                                       x.StatusId == 1 &&
                                                       branchsId.Contains(x.BranchId.ToString()) &&
                                                       x.FacilityId == session.FacilityId &&
                                                       (x.UsersPermission == null || x.UsersPermission.Contains(user.ToString())),
                                                       "Id", lang == 1 ? "InventoryName" : "InventoryName2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        #endregion ========== End lists From WH tables


        #region  ============================================ Drop down list From CRM tables ============================================
        [HttpGet("DDLCrmEmailTemplate")]
        public async Task<IActionResult> DDLCrmEmailTemplate()
        {
            try
            {
                var list = new SelectList(new List<DDListItem<CrmEmailTemplateDto>>());
                list = await ddlHelper.GetAnyLis<CrmEmailTemplate, long>(t => t.IsDeleted == false, "Id", "Name");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
        #endregion ========== End lists From CRM tables


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

        [HttpGet("DDLIsActive")]
        public async Task<IActionResult> DDLIsActive()
        {
            try
            {
                List<SelectListItem> lististItems = new List<SelectListItem>();
                lististItems.Insert(0, new SelectListItem
                { Value = "1", Text = localization.GetMainResource("Active") });
                lististItems.Insert(1, new SelectListItem
                { Value = "0", Text = localization.GetMainResource("Inactive") });
                return Ok(await Result<object>.SuccessAsync(lististItems, ""));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }

        }

        [HttpGet("DDLJobType")]
        public async Task<IActionResult> DDLJobType()
        {
            try
            {
                List<SelectListItem> lististItems = new List<SelectListItem>();
                lististItems.Insert(0, new SelectListItem
                { Value = "0", Text = localization.GetResource1("All") });
                lististItems.Insert(1, new SelectListItem
                { Value = "1", Text = localization.GetHrResource("Manager") });
                lististItems.Insert(2, new SelectListItem
                { Value = "2", Text = localization.GetHrResource("Emp") });

                return Ok(await Result<object>.SuccessAsync(lististItems, ""));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLApply")]
        public async Task<IActionResult> DDLApply()
        {
            try
            {
                List<SelectListItem> lististItems = new List<SelectListItem>();
                lististItems.Insert(0, new SelectListItem
                { Value = "1", Text = localization.GetHrResource("Apply") });
                lististItems.Insert(1, new SelectListItem
                { Value = "2", Text = localization.GetHrResource("Unapply") });
                return Ok(await Result<object>.SuccessAsync(lististItems, ""));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }

        }
        [HttpGet("DDLMethod")]
        public async Task<IActionResult> DDLMethod()
        {
            try
            {
                List<SelectListItem> lististItems = new List<SelectListItem>();
                lististItems.Insert(0, new SelectListItem
                { Value = "0", Text = localization.GetResource1("All") });
                lististItems.Insert(1, new SelectListItem
                { Value = "1", Text = localization.GetHrResource("CumulativeCalculation") });
                lististItems.Insert(2, new SelectListItem
                { Value = "2", Text = localization.GetHrResource("DecreasingCalculation") });
                return Ok(await Result<object>.SuccessAsync(lististItems, ""));
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


        [HttpGet("RadToDependents")]
        public async Task<IActionResult> RadToDependents()
        {
            try
            {
                List<SelectListItem> lististItems = new List<SelectListItem>();
                lististItems.Insert(0, new SelectListItem
                { Value = "1", Text = localization.GetMainResource("تابع") });
                lististItems.Insert(1, new SelectListItem
                { Value = "0", Text = localization.GetMainResource("الموظف") });
                return Ok(await Result<object>.SuccessAsync(lististItems, ""));
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



        [HttpGet("DDLRadGender")]
        public async Task<IActionResult> DDLRadGender()
        {
            try
            {
                List<SelectListItem> lististItems = new List<SelectListItem>();
                lististItems.Insert(0, new SelectListItem
                { Value = "1", Text = "ذكر " });
                lististItems.Insert(1, new SelectListItem
                { Value = "0", Text = "انثى" });
                lististItems.Insert(2, new SelectListItem
                { Value = "2", Text = "كلاهما" });
                return Ok(await Result<object>.SuccessAsync(lististItems, ""));
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

        [HttpGet("RBLApplicantType")]
        public async Task<IActionResult> RBLApplicantType()
        {
            try
            {
                List<SelectListItem> lististItems = new List<SelectListItem>();
                lististItems.Insert(0, new SelectListItem
                { Value = "1", Text = localization.GetCommonResource("personal") });
                lististItems.Insert(1, new SelectListItem
                { Value = "2", Text = localization.GetCommonResource("administrative") });
                return Ok(await Result<object>.SuccessAsync(lististItems, ""));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }

        }



        [HttpGet("HrClearance")]
        public async Task<IActionResult> HrClearance()
        {
            try
            {
                List<SelectListItem> lististItems = new List<SelectListItem>();
                lististItems.Insert(0, new SelectListItem
                { Value = "0", Text = localization.GetCommonResource("all") });
                lististItems.Insert(1, new SelectListItem
                { Value = "1", Text = localization.GetHrResource("VacCleared") });
                lististItems.Insert(2, new SelectListItem
                { Value = "2", Text = localization.GetHrResource("VacNotCleared") });
                return Ok(await Result<object>.SuccessAsync(lististItems, ""));
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

        [HttpGet("DDLNonAssignedEmployee")]
        public async Task<IActionResult> DDLNonAssignedEmployee()
        {
            try
            {
                List<SelectListItem> lististItems = new List<SelectListItem>();
                lististItems.Insert(0, new SelectListItem
                { Value = "1", Text = localization.GetHrResource("ShiftAssignedEmployees") });
                lististItems.Insert(1, new SelectListItem
                { Value = "2", Text = localization.GetHrResource("NonAssignedEmployeeToShift") });
                return Ok(await Result<object>.SuccessAsync(lististItems, ""));
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



        [HttpGet("DDLAttendanceType")]
        public async Task<IActionResult> DDLAttendanceType()
        {
            try
            {
                List<SelectListItem> lististItems = new List<SelectListItem>();
                lististItems.Insert(0, new SelectListItem
                { Value = "0", Text = localization.GetResource1("Choose") });
                lististItems.Insert(1, new SelectListItem
                { Value = "1", Text = localization.GetSSResources("TimeIn") });
                lististItems.Insert(2, new SelectListItem
                { Value = "2", Text = localization.GetSSResources("TimeOut") });

                return Ok(await Result<object>.SuccessAsync(lististItems, ""));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }


        [HttpGet("DDLStatus")]
        public async Task<IActionResult> DDLStatus()
        {
            try
            {
                List<SelectListItem> lististItems = new List<SelectListItem>();
                lististItems.Insert(0, new SelectListItem
                { Value = "0", Text = localization.GetSSResources("all") });
                lististItems.Insert(1, new SelectListItem
                { Value = "1", Text = localization.GetSSResources("HavePermission") });
                lististItems.Insert(2, new SelectListItem
                { Value = "2", Text = localization.GetSSResources("Absentees") });
                lististItems.Insert(3, new SelectListItem
                { Value = "3", Text = localization.GetSSResources("Attendees") });
                lististItems.Insert(4, new SelectListItem
                { Value = "4", Text = localization.GetSSResources("Latecomers") });
                lististItems.Insert(5, new SelectListItem
                { Value = "5", Text = localization.GetSSResources("NonAssignedEmployeeToShift") });
                return Ok(await Result<object>.SuccessAsync(lististItems, ""));
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

        [HttpGet("DDLMonths")]
        public async Task<IActionResult> DDLMonths()
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<InvestMonth, string>("MonthCode", "MonthName");
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

        [HttpGet("DDLLeaveType")]
        public async Task<IActionResult> DDLLeaveType(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<HrLeaveType, int>(x => x.IsDeleted == false, "TypeId", lang == 1 ? "TypeName" : "TypeName2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLLeaveTypeVw")]
        public async Task<IActionResult> DDLLeaveTypeVw(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<HrLeaveTypeVw, int>(x => x.IsDeleted == false, "TypeId", lang == 1 ? "TypeName" : "TypeName2");
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

        [HttpGet("DDLPermissionType")]
        public async Task<IActionResult> DDLPermissionType(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<HrPermissionTypeVw, int>(x => x.Isdel == false, "TypeId", lang == 1 ? "TypeName" : "TypeName2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }


        [HttpGet("DDLPermissionReason")]
        public async Task<IActionResult> DDLPermissionReason(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<HrPermissionReasonVw, int>(x => x.Isdel == false, "ReasonId", lang == 1 ? "ReasonName" : "ReasonName2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
        [HttpGet("DDLRequestType")]
        public async Task<IActionResult> DDLRequestType(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<HrRequestType, long>(x => x.IsDeleted == false, "Id", lang == 1 ? "RequestName" : "RequestName2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLTransactionType")]
        public async Task<IActionResult> DDLTransactionType(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<HrIncrementType, int>(x => true, "Id", lang == 1 ? "Name" : "Name2");
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

        //  آلية تطبيق الزيادة 
        [HttpGet("DDlApplyType")]
        public async Task<IActionResult> DDlApplyType()
        {
            try
            {
                List<SelectListItem> lististItems = new List<SelectListItem>();
                lististItems.Insert(0, new SelectListItem
                { Value = "0", Text = localization.GetSSResources("all") });
                lististItems.Insert(1, new SelectListItem
                { Value = "1", Text = localization.GetHrResource("ApplyNow") });
                lististItems.Insert(2, new SelectListItem
                { Value = "2", Text = localization.GetSSResources("ApplyOnDate") });

                return Ok(await Result<object>.SuccessAsync(lististItems, ""));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }


        [HttpGet("EvaluationType")]
        public async Task<IActionResult> EvaluationType(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<HrPerformanceTypeVw, long>(x => x.Isdel == false, "TypeId", lang == 1 ? "TypeName" : "TypeName2");
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

        [HttpGet("DDLFieldColumns")]
        public async Task<IActionResult> DDLFieldColumns()
        {
            try
            {
                var result = await mainServiceManager.InvestEmployeeService.DDLFieldColumns();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        
        [HttpGet("DDLYear")]
        public async Task<IActionResult> DDLYear(int num = 10)
        {
            try
            {
                var hijriCalendar = Bahsas.YearHijri(session);
                var currentHijriYear = Convert.ToInt32(hijriCalendar);

                var years = new List<DDListItem<int>>();
                for (int i = currentHijriYear - num; i <= currentHijriYear; i++)
                {
                    years.Add(new DDListItem<int>
                    {
                        Name = i.ToString(),
                        Value = i
                    });
                }

                var drpYear = listHelper.GetFromList<int>(years, hasDefault: false);

                return Ok(await Result<SelectList>.SuccessAsync(drpYear));
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync(ex.Message));
            }
        }
        #endregion


        #region ======================================== Drop down list From ACC tables (ACC) ========================================

        [HttpGet("DDLFinYear")]
        public async Task<IActionResult> DDLFinYear()
        {
            try
            {
                var FinYear = await accServiceManager.AccFinancialYearService.GetAll(x => x.IsDeleted == false && x.FinState == 1 && x.FacilityId == session.FacilityId);
                var DrpFinYear = listHelper.GetFromList<long>(FinYear.Data.Select(s => new DDListItem<long> { Name = s.FinYearGregorian.ToString(), Value = s.FinYear }), hasDefault: false);
                return Ok(await Result<SelectList>.SuccessAsync(DrpFinYear));
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccFinancialYearDto>.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLFinyearFrom")]
        public async Task<IActionResult> DDLFinyearFrom()
        {
            try
            {
                var FinYear = await accServiceManager.AccFinancialYearService.GetAll(x => x.IsDeleted == false && x.FacilityId == session.FacilityId);
                var DrpFinYear = listHelper.GetFromList<long>(FinYear.Data.Select(s => new DDListItem<long> { Name = s.FinYearGregorian.ToString(), Value = s.FinYear }), hasDefault: false);
                return Ok(await Result<SelectList>.SuccessAsync(DrpFinYear));
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccFinancialYearDto>.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLFinyearTo")]
        public async Task<IActionResult> DDLFinyearTo()
        {
            try
            {
                var FinYear = await accServiceManager.AccFinancialYearService.GetAll(x => x.IsDeleted == false && x.FacilityId == session.FacilityId);
                var DrpFinYear = listHelper.GetFromList<long>(FinYear.Data.Select(s => new DDListItem<long> { Name = s.FinYearGregorian.ToString(), Value = s.FinYear }), hasDefault: false);
                return Ok(await Result<SelectList>.SuccessAsync(DrpFinYear));
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccFinancialYearDto>.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLAccountClose")]
        public async Task<IActionResult> DDLAccountClose(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<AccAccountsCloseType, int>("AccountCloseTypeId", lang == 1 ? "AccountCloseTypeName" : "AccountCloseTypeName");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLAccountParent")]
        public async Task<IActionResult> DDLAccountParent(int lang = 1)
        {
            try
            {
                var items = await accServiceManager.AccAccountService.GetAllVW(d => d.FlagDelete == false && d.IsSub == true && d.FacilityId == session.FacilityId);
                var itemsList = items.Data.ToList();
                var hierarchicalList = BindAccountTree(itemsList, lang);

                var ddReferenceTypeList = listHelper.GetFromList<long>(hierarchicalList.Select(s => new DDListItem<long>
                {
                    Name = s.Name,
                    Value = s.Value
                }), hasDefault: false, defaultText: localization.GetResource1("Choose"));

                return Ok(await Result<SelectList>.SuccessAsync(ddReferenceTypeList));

            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        private List<DDListItem<long>> BindAccountTree(List<AccAccountsVw> items, int languageId)
        {
            List<DDListItem<long>> selectList = new List<DDListItem<long>>();

            foreach (var item in items.Where(i => i.AccAccountParentId == i.AccAccountId))
            {
                selectList.Add(new DDListItem<long> { Value = item.AccAccountId, Name = $"{item.AccAccountCode}-{GetAccountName(item, languageId)}" });

                AddChildren(items, selectList, item, languageId, 1);
            }

            return selectList;
        }

        private void AddChildren(List<AccAccountsVw> items, List<DDListItem<long>> selectList, AccAccountsVw parent, int languageId, int level)
        {
            foreach (var child in items.Where(i => i.AccAccountParentId == parent.AccAccountId && i.AccAccountId != i.AccAccountParentId))
            {
                string prefix = new string('-', level * 2);
                selectList.Add(new DDListItem<long> { Value = child.AccAccountId, Name = $"{prefix} {child.AccAccountCode}-{GetAccountName(child, languageId)}" });

                // Recursively add grandchildren
                AddGrandChildren(items, selectList, child, languageId, level + 1);
            }
        }

        private void AddGrandChildren(List<AccAccountsVw> items, List<DDListItem<long>> selectList, AccAccountsVw parent, int languageId, int level)
        {
            foreach (var grandChild in items.Where(i => i.AccAccountParentId == parent.AccAccountId && i.AccAccountId != i.AccAccountParentId))
            {
                string prefix = new string('-', level * 2);
                selectList.Add(new DDListItem<long> { Value = grandChild.AccAccountId, Name = $"{prefix} {grandChild.AccAccountCode}-{GetAccountName(grandChild, languageId)}" });

                // Recursively add great-grandchildren
                AddGrandChildren(items, selectList, grandChild, languageId, level + 1);
            }
        }

        private string GetAccountName(AccAccountsVw account, int languageId)
        {
            if (account.AccAccountId == account.AccAccountParentId)
            {
                return languageId == 1 ? (account.AccAccountName ?? "") : account.AccAccountName2 ?? "";
            }
            else
            {
                return languageId == 1 ? ((account.AccAccountName ?? "")) : (" - " + account.AccAccountName2);
            }
        }

        [HttpGet("DDLAccountGroup")]
        public async Task<IActionResult> DDLAccountGroup(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<AccGroup, int>(d => d.IsDeleted == false && d.FacilityId == session.FacilityId, "AccGroupId", lang == 1 ? "AccGroupName" : "AccGroupName2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }

            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLCostCenter")]
        public async Task<IActionResult> DDLCostCenter(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<AccCostCenter, int>(d => d.IsDeleted == false && d.IsParent == false && d.FacilityId == session.FacilityId, "CcId", lang == 1 ? "CostCenterName" : "CostCenterName2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLCostCenterParent")]
        public async Task<IActionResult> DDLCostCenterParent(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<AccCostCenterVws, int>(d => d.IsParent == true && d.FacilityId == session.FacilityId, "CcId", lang == 1 ? "CostCenterName" : "CostCenterName2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLAccPeriodsPM")]
        public async Task<IActionResult> DDLAccPeriodsPM(int lang = 1)
        {
            //  تم استخدامه في مستخلصات المشاريع
            try
            {
                var allPeriods = await accServiceManager.AccPeriodsService.GetAllVW(x => x.PeriodState == 1 && x.FinYear == session.FinYear && x.FacilityId == session.FacilityId);
                var dePeriods = allPeriods.Data;
                var DDLPeriods = listHelper.GetFromList<long>(dePeriods.Select(s => new DDListItem<long> { Name = lang == 1 ? s.PeriodDate ?? "" : s.PeriodDate2 ?? (s.PeriodDate ?? ""), Value = s.PeriodId }),
                    selectedValue: session.PeriodId, hasDefault: false, defaultText: localization.GetResource1("Choose"));

                return Ok(await Result<SelectList>.SuccessAsync(DDLPeriods));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLReferenceTypeForPM")]
        public async Task<IActionResult> DDLReferenceTypeForPM(int lang = 1)
        {
            try
            {
                List<int> parentIdList = new List<int>
                {
                    1,20,2,4
                };

                var items = await accServiceManager.AccReferenceTypeService.GetAllVW(x => x.FlagDelete == false && parentIdList.Contains(x.ParentId ?? 0));
                var res = items.Data.OrderBy(x => x.ReferenceTypeId).ToList();
                var ddReferenceTypeList = listHelper.GetFromList<long>(items.Data.Select(s => new DDListItem<long>
                {
                    Name = lang == 1 ? ((s.ReferenceTypeName ?? "") + " - " + (s.ParentName ?? "")) : ((s.ReferenceTypeName2) + " - " + (s.ParentName ?? "")),
                    Value = s.ReferenceTypeId
                }), hasDefault: false, defaultText: localization.GetResource1("Choose"));


                return Ok(await Result<SelectList>.SuccessAsync(ddReferenceTypeList));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLReferenceType")]
        public async Task<IActionResult> DDLReferenceType(int lang = 1)
        {
            try
            {
                var items = await accServiceManager.AccReferenceTypeService.GetAllVW(x => x.FlagDelete == false);
                var res = items.Data.OrderBy(x => x.ParentId).ThenBy(x => x.ReferenceTypeId).ToList();
                var ddReferenceTypeList = listHelper.GetFromList<long>(items.Data.Select(s => new DDListItem<long>
                {
                    Name = s.ReferenceTypeId == s.ParentId ? lang == 1 ? ((s.ReferenceTypeName ?? "")) : ((s.ReferenceTypeName2 ?? "")) : lang == 1 ? (" - - " + (s.ReferenceTypeName ?? "")) : (" - - " + (s.ReferenceTypeName2)),
                    Value = s.ReferenceTypeId
                }), hasDefault: false, defaultText: localization.GetResource1("Choose"));

                return Ok(await Result<SelectList>.SuccessAsync(ddReferenceTypeList));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLCostCenterCCNo")]
        public async Task<IActionResult> DDLCostCenterCCNo()
        {
            try
            {

                var CC_Title_1 = await configHelper.GetValue(45, session.FacilityId);
                var CC_Title_2 = await configHelper.GetValue(46, session.FacilityId);
                var CC_Title_3 = await configHelper.GetValue(47, session.FacilityId);
                var CC_Title_4 = await configHelper.GetValue(48, session.FacilityId);
                var CC_Title_5 = await configHelper.GetValue(49, session.FacilityId);
                List<SelectListItem> lististItems = new List<SelectListItem>();
                if (!String.IsNullOrEmpty(CC_Title_1))
                {
                    lististItems.Insert(0, new SelectListItem
                    { Value = "1", Text = CC_Title_1 });
                }
                if (!String.IsNullOrEmpty(CC_Title_2))
                {
                    lististItems.Insert(1, new SelectListItem
                    { Value = "2", Text = CC_Title_2 });
                }
                if (!String.IsNullOrEmpty(CC_Title_3))
                {
                    lististItems.Insert(2, new SelectListItem
                    { Value = "3", Text = CC_Title_3 });
                }
                if (!String.IsNullOrEmpty(CC_Title_4))
                {
                    lististItems.Insert(3, new SelectListItem
                    { Value = "4", Text = CC_Title_4 });
                }
                if (!String.IsNullOrEmpty(CC_Title_5))
                {
                    lististItems.Insert(4, new SelectListItem
                    { Value = "5", Text = CC_Title_5 });
                }

                return Ok(await Result<object>.SuccessAsync(lististItems, ""));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLAccPeriods")]
        public async Task<IActionResult> DDLAccPeriods(int lang = 1)
        {
            try
            {
                var list = new SelectList(new List<DDListItem<AccPeriodDateVws>>());
                list = await ddlHelper.GetAnyLis<AccPeriodDateVws, long>(p => p.PeriodState == 1 && p.FinYear == session.FinYear
                        && p.FacilityId == session.FacilityId && p.FlagDelete == false, "PeriodId", (lang == 1) ? "PeriodDate" : "PeriodDate2");
                return Ok(await Result<SelectList>.SuccessAsync(list));

            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLDocType")]
        public async Task<IActionResult> DDLDocType(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<AccDocumentTypeListVw, int>("DocTypeId", lang == 1 ? "DocTypeName" : "DocTypeName2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }

            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
        [HttpGet("DDLPaymentType")]
        public async Task<IActionResult> DDLPaymentType(int lang = 1)
        {
            try
            {
                var PaymentType = await configHelper.GetValue(104, session.FacilityId);
                var list = await ddlHelper.GetAnyLis<AccPaymentType, int>(d => d.FlagDelete == false && ((d.PaymentTypeId != 0) || PaymentType.Contains(d.PaymentTypeId.ToString())), "PaymentTypeId", lang == 1 ? "PaymentTypeName" : "PaymentTypeName2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }

            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }


        [HttpGet("DDLCashOnhand")]

        public async Task<Result<SelectList>> DDLCashOnhand(int lang = 1)
        {
            try
            {
                // استخراج بيانات الفروع والمستخدم واللغة من الجلسة
                var branchsId = session.Branches?.Split(',') ?? Array.Empty<string>();
                var user = session.UserId;

                // جلب البيانات من الخدمة
                var items = await accServiceManager.AccCashOnHandListVwService.GetAll(
                    x => x.IsDeleted == false &&
                         x.FacilityId == session.FacilityId &&
                          (x.BranchId == 0 || branchsId.Contains(x.BranchId.ToString()))
                );

                // التحقق من نجاح العملية
                if (items.Succeeded && items.Data != null)
                {

                    var filteredItems = items.Data.Where(x => !string.IsNullOrEmpty(x.UsersPermission) && x.UsersPermission.Split(',').Contains(user.ToString()));
                    // إنشاء القائمة المحددة
                    var ddCashOnHandList = listHelper.GetFromList<long>(
                        filteredItems.Select(s => new DDListItem<long>
                        {
                            Name = lang == 1 ? s.Name : s.Name2,
                            Value = s.AccAccountId ?? 0,
                        }),
                        hasDefault: false,
                        defaultText: localization.GetResource1("Choose")
                    );

                    // إرجاع النتيجة بنجاح
                    return await Result<SelectList>.SuccessAsync(ddCashOnHandList);
                }
                else
                {
                    // إرجاع رسالة خطأ في حالة عدم وجود بيانات أو فشل العملية
                    return await Result<SelectList>.FailAsync("No data found or operation failed.");
                }
            }
            catch (Exception ex)
            {
                // تسجيل الخطأ وإرجاع رسالة خطأ مناسبة
                return await Result<SelectList>.FailAsync($"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("DDLDocTypeTransfergeneral")]
        public async Task<IActionResult> DDLDocTypeTransfergeneral(int lang = 1)
        {
            try
            {
                string Posting_By_User_Doc_Type = "1";
                Posting_By_User_Doc_Type = await configHelper.GetValue(20, session.FacilityId);
                string Acc_Posting = "1";
                Acc_Posting = await mainServiceManager.SysUserService.GetUserPosting(session.UserId) ?? "1";

                var list = await ddlHelper.GetAnyLis<AccDocumentTypeListVw, int>(x => x.FlagDelete == false && Posting_By_User_Doc_Type.Equals("2") || Acc_Posting.Contains(x.DocTypeId.ToString()), "DocTypeId", lang == 1 ? "DocTypeName" : "DocTypeName2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }

            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLPettyCashExpensesType")]

        public async Task<IActionResult> DDLPettyCashExpensesType(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<AccPettyCashExpensesType, int>("Id", lang == 1 ? "Name" : "Name2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        #endregion  End lists From ACC tables (ACC)


        #region  ======================================== Drop down list From PM tables (PM) ========================================

        [HttpGet("DDLProjectsType")]
        public async Task<IActionResult> DDLProjectsType(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<PmProjectsTypeVw, long>(d => d.IsDeleted == false && d.Id == d.ParentId && d.SystemId == 5, "Id", lang == 1 ? "TypeName" : "TypeName");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLProjects")]
        public async Task<IActionResult> DDLProjects(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<PmProjectsVw, long>(d => d.IsDeleted == false && d.StatusId == 1 && d.IsSubContract == false && d.FacilityId == session.FacilityId, "Id", lang == 1 ? "TypeName" : "TypeName");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLProjectsType2")]
        public async Task<IActionResult> DDLProjectsType2(int? ParentId)
        {
            try
            {
                var lang = session.Language;
                var list = await ddlHelper.GetAnyLis<PmProjectsTypeVw, long>(d => d.IsDeleted == false && d.ParentId == ParentId && d.Id != ParentId && d.SystemId == 5, "Id", lang == 1 ? "TypeName" : "TypeName");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLProjectsStages")]
        public async Task<IActionResult> DDLProjectsStages(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<PmProjectsStagesVw, long>(d => d.IsDeleted == false && (d.Id == d.ParentId || d.ParentId == null || d.ParentId == 0) && d.FacilityId == session.FacilityId, "Id", lang == 1 ? "Name" : "Name2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }


        [HttpGet("DDLProjectStatus")]
        public async Task<IActionResult> DDLProjectStatus(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<PmProjectStatusVw, long>(d => d.Isdel == false, "Id", lang == 1 ? "StatusName" : "StatusName2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLPmDurationType")]
        public async Task<IActionResult> DDLPmDurationType(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<PmDurationTypeVw, long>(d => d.Isdel == false, "Id", lang == 1 ? "DurationTypeName" : "DurationTypeName2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLPMProjectItems")]
        public async Task<IActionResult> DDLPMProjectItems(long? ProjectCode)
        {
            try
            {
                var lang = session.Language;
                var project = await pMServiceManager.PMProjectsService.GetOne(x => x.Code == ProjectCode && x.IsDeleted == false && x.IsSubContract == false);
                if (project != null && project.Succeeded == true)
                {
                    var list = await ddlHelper.GetAnyLis<PMProjectsItemsVw, long>(d => d.IsDeleted == false && d.ProjectId == project.Data.Id, "Id", lang == 1 ? "ItemName" : "ItemName");
                    return Ok(await Result<SelectList>.SuccessAsync(list));
                }
                var listTemp = await ddlHelper.GetAnyLis<PMProjectsItemsVw, long>(d => 1 == 2, "Id", lang == 1 ? "ItemName" : "ItemName");
                return Ok(await Result<SelectList>.SuccessAsync(listTemp));

            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
        [HttpGet("DDLPMProjectItemsById")]
        public async Task<IActionResult> DDLPMProjectItemsById(long? ProjectId)
        {
            try
            {
                var lang = session.Language;
                //var project = await pMServiceManager.PMProjectsService.GetOne(x => x.Code == ProjectCode && x.IsDeleted == false && x.IsSubContract == false);
                if (ProjectId != null)
                {
                    var list = await ddlHelper.GetAnyLis<PMProjectsItemsVw, long>(d => d.IsDeleted == false && d.ProjectId == ProjectId, "Id", lang == 1 ? "ItemName" : "ItemName");
                    return Ok(await Result<SelectList>.SuccessAsync(list));
                }
                var listTemp = await ddlHelper.GetAnyLis<PMProjectsItemsVw, long>(d => 1 == 2, "Id", lang == 1 ? "ItemName" : "ItemName");
                return Ok(await Result<SelectList>.SuccessAsync(listTemp));

            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLExtractTransactionsStatus")]
        public async Task<IActionResult> DDLExtractTransactionsStatus(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<PmExtractTransactionsStatus, int>(d => true, "Id", lang == 1 ? "StatusName" : "StatusName2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLPmRiskEffect")]
        public async Task<IActionResult> DDLPmRiskEffect(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<PmRiskEffect, long>(d => d.IsDeleted == false, "Id", lang == 1 ? "Value" : "Value");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLTypeExtract")]
        public async Task<IActionResult> DDLTypeExtract(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<PmExtractTransactionsType, long>(d => true, "Id", lang == 1 ? "TransactionType" : "TransactionType");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLExtractAdditional")]
        public async Task<IActionResult> DDLExtractAdditional(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<PmExtractAdditionalType, long>(d => d.FacilityId == session.FacilityId && d.IsDeleted == false, "Id", lang == 1 ? "Name" : "Name");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLPmRiskImpact")]
        public async Task<IActionResult> DDLPmRiskImpact(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<PmRiskImpact, long>(d => d.IsDeleted == false, "Id", lang == 1 ? "Value" : "Value");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLPmExtractTransactionsStatus")]
        public async Task<IActionResult> DDLPmExtractTransactionsStatus(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<PmExtractTransactionsStatus, long>(d => true, "Id", lang == 1 ? "StatusName" : "StatusName2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        //  الإدارة في شاشة ميثاق مشروع جديد
        [HttpGet("DDLProjectOwner")]
        public async Task<IActionResult> DDLProjectOwner(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<SysDepartment, int>(b => (b.IsDeleted == false && b.FacilityId == session.FacilityId && b.StatusId == 1 && b.TypeId == 1), "Id", lang == 1 ? "Name" : "Name2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
        //   المسؤول   في شاشة إعداد خطة المشروع
        [HttpGet("DDLEmpTask")]
        public async Task<IActionResult> DDLEmpTask()
        {
            try
            {
                var list = new SelectList(new List<DDListItem<SysUser>>());
                list = await ddlHelper.GetAnyLis<SysUser, long>(u => u.Isdel == false && u.IsDeleted == false && u.Enable == 1 && u.FacilityId == session.FacilityId, "Id", "UserFullname");

                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLEmpTaskByUserType")]
        public async Task<IActionResult> DDLEmpTaskByUserType()
        {
            try
            {
                var list = new SelectList(new List<DDListItem<SysUser>>());
                var property = await mainServiceManager.SysPropertyValueService.GetByProperty(369, session.FacilityId);
                if (property != null && property.Data.PropertyValue == "1")
                {
                    list = await ddlHelper.GetAnyLis<SysUser, long>(u => u.Isdel == false && u.Enable == 1 && u.UserTypeId == 1, "Id", "UserFullname");
                }
                else
                {
                    list = await ddlHelper.GetAnyLis<SysUser, long>(u => u.Isdel == false && u.Enable == 1 && u.UserTypeId == 1 && u.FacilityId == session.FacilityId, "Id", "UserFullname");
                }

                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }


        [HttpGet("DDLResourcesDept")]
        public async Task<IActionResult> DDLResourcesDept(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<SysDepartment, long>((d => d.TypeId == 1 && d.IsDeleted == false && d.StatusId == 1 && d.FacilityId == session.FacilityId), "Id", (lang == 1) ? "Name" : "Name2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }


        [HttpGet("DDLResourcesManager")]
        public async Task<IActionResult> DDLResourcesManager(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<HrEmployeeVw, long>((d => d.Isdel == false && d.IsDeleted == false && d.StatusId != 10 && d.FacilityId == session.FacilityId), "Id", (lang == 1) ? "EmpName" : "EmpName2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLAgreementProjectsType")]
        public async Task<IActionResult> DDLAgreementProjectsType(int lang = 1)
        {
            try
            {
                var FacilityId = session.FacilityId;
                var list = await ddlHelper.GetAnyLis<PmProjectsType, long>(d => d.IsDeleted == false && d.Id == d.ParentId && d.SystemId == 9 && d.Iscase == false && d.FacilityId == FacilityId, "Id", lang == 1 ? "TypeName" : "TypeName");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLAgreementProjectsType2")]
        public async Task<IActionResult> DDLAgreementProjectsType2(int? ParentId)
        {
            try
            {
                var lang = session.Language;
                var list = await ddlHelper.GetAnyLis<PmProjectsType, long>(d => d.IsDeleted == false && d.ParentId == ParentId && d.Id != ParentId, "Id", lang == 1 ? "TypeName" : "TypeName");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLProjectsListVw")]
        public async Task<IActionResult> DDLProjectsListVw()
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<PmProjectsListVw, long>(d => d.IsDeleted == false && d.StatusId == 1 && d.IsSubContract == false && d.FacilityId == session.FacilityId, "Id", "Name");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        #endregion End lists From PM tables (PM)

        #region  ======================================== Drop down list From Sal tables (SAL) ========================================

        [HttpGet("DDLDiscountType")]
        public async Task<IActionResult> DDLDiscountType(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<SalDiscountType, long>("Id", lang == 1 ? "Name" : "Name");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLSalPaymentType")]
        public async Task<IActionResult> DDLSalPaymentType(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<SalPaymentTerm, long>(d => d.IsDeleted == false, "Id", lang == 1 ? "PaymentTerms" : "PaymentTerms2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        #endregion End lists From Sal tables (SAL)

        #region  ======================================== Drop down list From PUR tables (PUR) ========================================

        [HttpGet("DDLACCReferenceType")]
        public async Task<IActionResult> DDLACCReferenceType(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<AccReferenceTypeVw, int>(
                    x => x.FlagDelete == false && (new[] { 1, 20, 2, 3 }.Contains(x.ParentId ?? 0)),
                    "ReferenceTypeId",
                    lang == 1 ? "ParentName + ' - ' + ReferenceTypeName" : "ParentName + ' - ' + ReferenceTypeName2"
                );
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLSysCustomerGroup")]
        public async Task<IActionResult> DDLSysCustomerGroup(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<SysCustomerGroup, int>(
                    x => x.IsDeleted == false && x.CusTypeId == 1 && x.FacilityId == session.FacilityId,
                    "Id", lang == 1 ? "Name" : "Name");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLPurPaymentTerm")]
        public async Task<IActionResult> DDLPurPaymentTerm(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<PurPaymentTerm, long>(d => d.IsDeleted == false, "Id", lang == 1 ? "PaymentTerms" : "PaymentTerms2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
        #endregion End lists From PUR tables (PUR)

        #region  ======================================== Drop down list From TS tables (TS) ========================================
        [HttpGet("DDLTaskProject")]
        public async Task<IActionResult> DDLTaskProject(string? projectId)
        {
            try
            {
                SelectList list;
                projectId ??= "0";
                long projectID = Convert.ToInt64(projectId);
                if (projectID == 0)
                    list = await ddlHelper.GetAnyLis<TsTasksVw, long>(d => d.Isdel == false && d.UserId == session.UserId, "Id", "Subject");
                else
                    list = await ddlHelper.GetAnyLis<TsTasksVw, long>(d => d.Isdel == false && d.UserId == session.UserId && d.ProjectId == projectID, "Id", "Subject");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLTaskParent")]
        public async Task<IActionResult> DDLTaskParent(string? projectId)
        {
            try
            {
                SelectList list;
                projectId ??= "0";
                long projectID = Convert.ToInt64(projectId);
                if (projectID == 0)
                    list = await ddlHelper.GetAnyLis<TsTasksVw, long>(d => d.Isdel == false && d.UserId == session.UserId, "Id", "Subject");
                else
                    list = await ddlHelper.GetAnyLis<TsTasksVw, long>(d => d.Isdel == false && d.UserId == session.UserId && d.ProjectId == projectID, "Id", "Subject");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
        #endregion End lists From TS tables (TS)

        #region  ======================================== Drop down list From Integra tables (Integra) ========================================
        [HttpGet("DDLIntegraSystemId")]
        public async Task<IActionResult> DDLIntegraSystemId(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<IntegraSystem, long>(d => d.IsDeleted == false, "Id", lang == 1 ? "Name" : "Name2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLIntegraSystemField")]
        public async Task<IActionResult> DDLIntegraSystemField(long tableId)
        {
            try
            {
                var lang = session.Language;
                var list = await ddlHelper.GetAnyLis<IntegraField, long>(d => d.TableId == tableId, "Id", lang == 1 ? "Name" : "Name2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
        #endregion End lists From TS tables (TS)

        #region  ======================================== Drop down list From RPT tables (RPT) ========================================

        [HttpGet("DDLOperator")]
        public async Task<IActionResult> DDLOperator(int lang = 1)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<RptOperator, long>(d => d.IsDeleted == false, "Id", lang == 1 ? "OperatorName" : "OperatorName");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("DDLTable")]
        public async Task<IActionResult> DDLTable(int lang = 1, int sysId = 0)
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<RptTable, long>(d => d.IsDeleted == false && sysId == sysId, "Id", lang == 1 ? "Name" : "Name2");
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }


        #endregion End lists From RPT tables  (RPT)


    }
}
