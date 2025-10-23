using Logix.Application.Common;
using Logix.Application.DTOs.ACC;
using Logix.Application.DTOs.Main;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;
using Logix.Domain.Main;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Logix.MVC.LogixAPIs.Main
{

    public class SysCustomerGroupController : BaseMainApiController
    {
        private readonly ICurrentData session;
        private readonly IMainServiceManager mainServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ILocalizationService localization;
        private readonly IApiDDLHelper ddlHelper;

        public SysCustomerGroupController(IMainServiceManager mainServiceManager,
            IPermissionHelper permission,
            ILocalizationService localization,
            ICurrentData session,
            IApiDDLHelper ddlHelper)
        {
            this.session = session;
            this.mainServiceManager = mainServiceManager;
            this.permission = permission;
            this.localization = localization;
            this.ddlHelper = ddlHelper;
        }

        private async Task<bool> CheckPermission(int cusTypeId, PermissionType permissionType)
        {
            try
            {
                bool chk = cusTypeId switch
                {
                    // supplier
                    1 => await permission.HasPermission(401, permissionType),
                    // customer
                    2 => await permission.HasPermission(400, permissionType),
                    // contractor
                    3 => await permission.HasPermission(830, permissionType),
                    // donor
                    4 => await permission.HasPermission(927, permissionType),
                    // membership
                    6 => await permission.HasPermission(928, permissionType),
                    // other groups
                    12 => await permission.HasPermission(1600, permissionType),
                    _ => false,
                };
                return chk;
            }
            catch
            {
                return false;
            }
        }

        [HttpPost("Search")]
        public async Task<ActionResult> Search(SysCustomerGroupFilterDto filter)
        {
            try
            {
                filter.CusTypeId ??= 0;
                var chk = await CheckPermission(filter.CusTypeId ?? 0, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var items = await mainServiceManager.SysCustomerGroupService.GetAll(g => g.IsDeleted == false && g.FacilityId == session.FacilityId
                && (filter.CusTypeId == 0 || g.CusTypeId == filter.CusTypeId)
                && (string.IsNullOrEmpty(filter.Name) || g.Name!.Contains(filter.Name)));
                if (items.Succeeded)
                {
                    var res = items.Data.OrderBy(c => c.CusTypeId).AsQueryable();
                    var final = res.ToList();

                    var getAllCusTypes = await mainServiceManager.SysCustomerTypeService.GetAll();
                    var allCusTypes = getAllCusTypes.Data;

                    foreach (var item in final)
                    {
                        var getCusType = allCusTypes.Where(c => c.TypeId == item.CusTypeId).FirstOrDefault();
                        if (getCusType != null)
                            item.CusTypeName = getCusType.CusTypeName;
                    }

                    return Ok(await Result<List<SysCustomerGroupDto>>.SuccessAsync(final));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpPost("Add")]
        public async Task<ActionResult> Add(SysCustomerGroupDto obj)
        {
            try
            {
                obj.CusTypeId ??= 0;
                var chk = await CheckPermission(obj.CusTypeId ?? 0, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result<SysCustomerGroupDto>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var add = await mainServiceManager.SysCustomerGroupService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<SysCustomerGroupDto>.FailAsync(ex.Message));
            }
        }

        [HttpGet("GetByIdForEdit")]
        public async Task<IActionResult> GetByIdForEdit(int id)
        {
            try
            {
                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));

                var item = await mainServiceManager.SysCustomerGroupService.GetForUpdate<SysCustomerGroupEditDto>(id);
                if (item.Succeeded)
                {
                    var chk = await CheckPermission(item.Data.CusTypeId ?? 0, PermissionType.Edit);
                    if (!chk)
                        return Ok(await Result.AccessDenied("AccessDenied"));
                }
                return Ok(item);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(SysCustomerGroupEditDto obj)
        {
            try
            {
                var chk = await CheckPermission(obj.CusTypeId ?? 0, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var update = await mainServiceManager.SysCustomerGroupService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(int cusTypeId, int id)
        {
            try
            {
                var chk = await CheckPermission(cusTypeId, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await mainServiceManager.SysCustomerGroupService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete SysCustomerGroup, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetOtherCustGroups")]
        public async Task<IActionResult> GetOtherCustGroups(int groupId)
        {
            try
            {
                var list = new SelectList(new List<DDListItem<SysCustomerGroupDto>>());

                var getCusTypeId = await mainServiceManager.SysCustomerGroupService.GetOne(g => g.CusTypeId, g => g.Id == groupId);
                if (getCusTypeId.Succeeded && getCusTypeId.Data > 0)
                {
                    list = await ddlHelper.GetAnyLis<SysCustomerGroup, long>(g => g.CusTypeId == getCusTypeId.Data && g.IsDeleted == false, "Id", "Name");
                    return Ok(await Result<SelectList>.SuccessAsync(list));
                }
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result<SysCustomerGroupDto>.FailAsync(ex.Message));
            }
        }


        [HttpGet("GetAccRefTypes")]
        public async Task<IActionResult> GetAccRefTypes(int groupId)
        {
            try
            {
                var list = new SelectList(new List<DDListItem<AccReferenceTypeDto>>());

                var getCusTypeId = await mainServiceManager.SysCustomerGroupService.GetOne(g => g.CusTypeId, g => g.Id == groupId && g.FacilityId == session.FacilityId);
                if (getCusTypeId.Succeeded && getCusTypeId.Data > 0)
                {
                    var parentId = 0;
                    if (getCusTypeId.Data == 1)
                        parentId = 3;
                    else if (getCusTypeId.Data == 2)
                        parentId = 2;
                    else if (getCusTypeId.Data == 3)
                        parentId = 20;
                    else if (getCusTypeId.Data == 4)
                        parentId = 18;
                    else if (getCusTypeId.Data == 6)
                        parentId = 19;
                    else if (getCusTypeId.Data == 7)
                        parentId = 33;
                    else if (getCusTypeId.Data == 10)
                        parentId = 50;
                    else if (getCusTypeId.Data == 11)
                        parentId = 51;
                    else
                        return Ok(await Result<SelectList>.SuccessAsync(list));

                    //select Reference_Type_ID from Sys_Customer_Group_Accounts where IsDeleted = 0 and Group_ID = @ID
                    var currentAccForGroup = await mainServiceManager.SysCustomerGroupAccountService.GetAll(a => a.ReferenceTypeId, a => a.IsDeleted == false && a.GroupId == Convert.ToInt64(groupId));
                    if (currentAccForGroup.Succeeded)
                    {
                        List<string> currentAccRefIds = new List<string>();
                        foreach (var item in currentAccForGroup.Data)
                        {
                            currentAccRefIds.Add(item.ToString());
                        }

                        //get all account reference
                        list = await ddlHelper.GetAnyLis<AccReferenceType, long>(a => a.FlagDelete == false && a.ParentId == parentId && a.ParentId != a.ReferenceTypeId && !currentAccRefIds.Contains(a.ReferenceTypeId.ToString()), "ReferenceTypeId", (session.Language == 1) ? "ReferenceTypeName" : "ReferenceTypeName2");
                    }

                    return Ok(await Result<SelectList>.SuccessAsync(list));
                }
                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result<SysCustomerGroupDto>.FailAsync($"====== Exp in GetAccRefTypes SysCustomerGroup, MESSAGE: {ex.Message}"));
            }
        }
    }
}
