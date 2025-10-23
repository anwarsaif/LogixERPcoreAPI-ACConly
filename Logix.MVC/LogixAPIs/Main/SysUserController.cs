using System.DirectoryServices;
using System.Runtime.InteropServices;
using Logix.Application.Common;
using Logix.Application.DTOs.Main;
using Logix.Application.Helpers;
using Logix.Application.Interfaces.IRepositories;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.Main;
using Logix.MVC.Helpers;
using Logix.MVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Logix.MVC.LogixAPIs.Main
{
    public class SysUserController : BaseMainApiController
    {
        private readonly IMainRepositoryManager _mainRepositoryManager;
        private readonly IMainServiceManager mainServiceManager;
        private readonly IHrServiceManager hrServiceManager;
        private readonly ISalServiceManager salServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly ISysConfigurationHelper configurationHelper;
        private readonly IApiDDLHelper ddlHelper;
        private readonly IDDListHelper listHelper;
        private readonly ISendSmsHelper sendSmsHelper;
        private readonly IEmailAppHelper emailAppHelper;

        public SysUserController(IMainServiceManager mainServiceManager,
            IMainRepositoryManager _mainRepositoryManager,
            IHrServiceManager hrServiceManager,
            ISalServiceManager salServiceManager,
            IPermissionHelper permission,
            ILocalizationService localization,
            ICurrentData session,
            ISysConfigurationHelper configurationHelper,
            IApiDDLHelper ddlHelper,
            IDDListHelper listHelper, ISendSmsHelper sendSmsHelper, IEmailAppHelper emailAppHelper)

        {
            this._mainRepositoryManager = _mainRepositoryManager;
            this.mainServiceManager = mainServiceManager;
            this.hrServiceManager = hrServiceManager;
            this.salServiceManager = salServiceManager;
            this.permission = permission;
            this.session = session;
            this.localization = localization;
            this.configurationHelper = configurationHelper;
            this.ddlHelper = ddlHelper;
            this.listHelper = listHelper;
            this.sendSmsHelper = sendSmsHelper;
            this.emailAppHelper = emailAppHelper;
        }

        #region ============================ Search & Get ===============================
        [HttpPost("Search")]
        public async Task<ActionResult> Search(SysUserFilterDto filter)
        {
            try
            {
                var chk = await permission.HasPermission(25, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                //var branchsIds = session.Branches.Split(',');
                //var items = await mainServiceManager.SysUserService.GetAllVW(u => u.IsDeleted == false
                //&& u.UserTypeId == 1
                //&& (filter.Enable == null || (u.Enable != null && u.Enable == filter.Enable))
                //&& (string.IsNullOrEmpty(filter.GroupsId) || (!string.IsNullOrEmpty(u.GroupsId) && u.GroupsId.Equals(filter.GroupsId)))
                //&& (string.IsNullOrEmpty(filter.UserFullname) || (!string.IsNullOrEmpty(u.UserFullname) && u.UserFullname.Contains(filter.UserFullname)))
                //&& (string.IsNullOrEmpty(filter.EmpCode) || (!string.IsNullOrEmpty(u.EmpCode) && u.EmpCode.Equals(filter.EmpCode)))
                //&& (filter.FacilityId == null || (u.FacilityId != null && u.FacilityId == filter.FacilityId))
                //&& (filter.UserTypeId == null || (u.UserTypeId != null && u.UserTypeId == filter.UserTypeId))
                //&& (string.IsNullOrEmpty(filter.UserName) || (!string.IsNullOrEmpty(u.UserName) && u.UserName.Contains(filter.UserName)))
                //);
                //if (items.Succeeded)
                //{
                //    var res = items.Data.AsQueryable();

                //    if (filter.BranchId > 0)
                //        res = res.Where(u => u.UserPkId == filter.BranchId);
                //    else
                //        res = res.Where(u => branchsIds.Contains(u.UserPkId.ToString()));

                //    var final = res.ToList();
                //    List<SysUserFilterDto> results = new List<SysUserFilterDto>();
                //    foreach (var item in final)
                //    {
                //        if (session.GroupId != 1)
                //        {
                //            var getSysId = await mainServiceManager.SysGroupService.GetOne(g => g.SystemId, g => g.GroupId == session.GroupId);
                //            if (getSysId.Succeeded)
                //            {
                //                var getSysIdByUsrGrp = await mainServiceManager.SysGroupService.GetOne(g => g.SystemId, g => g.GroupId == Convert.ToInt32(item.GroupsId));
                //                if (getSysIdByUsrGrp.Succeeded)
                //                {
                //                    if (getSysId.Data != getSysIdByUsrGrp.Data)
                //                        continue;
                //                }
                //            }
                //        }

                //        string lastLgn = "";
                //        if (item.LastLogin != null)
                //        {
                //            DateTime currentDate = DateTime.Now;
                //            // difference between dates
                //            TimeSpan difference = currentDate - item.LastLogin.Value;

                //            // difference in days, hours and minutes
                //            lastLgn = $"{difference.Days} يوم - {difference.Hours} ساعة - {difference.Minutes} دقيقة";
                //        }
                //        SysUserFilterDto result = new SysUserFilterDto()
                //        {
                //            Id = item.UserId,
                //            UserFullname = item.UserFullname,
                //            UserEmail = item.UserEmail,
                //            UserName = item.UserName,
                //            BraName = item.BraName,
                //            BraName2 = item.BraName2,
                //            EnableChkbx = item.Enable == 1 ? true : false,
                //            GroupsId = item.GroupsId,
                //            GroupName = item.GroupName,
                //            lastLogin = lastLgn
                //        };
                //        results.Add(result);
                //    }
                //    return Ok(await Result<List<SysUserFilterDto>>.SuccessAsync(results));
                //}
                //return Ok(items);

                var items = await mainServiceManager.SysUserService.Search(filter);
                return Ok(items);

            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Search SysUsersController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetByIdForEdit")]
        public async Task<IActionResult> GetByIdForEdit(long id)
        {
            try
            {
                var chk = await permission.HasPermission(25, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }
                var item = await mainServiceManager.SysUserService.GetForUpdate<SysUserEditDto>(id);
                item.Data.StringUserPassword = await _mainRepositoryManager.StoredProceduresRepository.GetDecryptUserPassword(id);

                if (item.Data.EmpId > 0)
                {
                    var getEmp = await mainServiceManager.InvestEmployeeService.GetOne(e => e.Id == item.Data.EmpId && e.IsDeleted == false);
                    if (getEmp.Succeeded && getEmp.Data != null)
                    {
                        item.Data.EmpCode = getEmp.Data.EmpId;
                    }
                }

                return Ok(item);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in GetByIdForEdit SysUsersController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetLoginTime")]
        public async Task<IActionResult> GetLoginTime(long id)
        {
            try
            {
                var chk = await permission.HasPermission(25, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }
                var item = await mainServiceManager.SysUserService.GetForUpdate<SysUserEditDto>(id);

                string empCode = "";
                if (item.Data.EmpId > 0)
                {
                    var getEmp = await mainServiceManager.InvestEmployeeService.GetOne(e => e.Id == item.Data.EmpId && e.IsDeleted == false);
                    if (getEmp.Succeeded && getEmp.Data != null)
                    {
                        empCode = getEmp.Data.EmpId ?? "";
                    }
                }

                SysUserLoginTimeDto obj = new SysUserLoginTimeDto
                {
                    Id = item.Data.Id,
                    Ips = item.Data.Ips,
                    TimeFrom = item.Data.TimeFrom.ToString(),
                    TimeTo = item.Data.TimeTo.ToString(),
                    UserName = item.Data.UserName,
                    EmpCode = empCode
                };

                return Ok(await Result<SysUserLoginTimeDto>.SuccessAsync(obj));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in GetByIdForEdit SysUsersController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetUserNameAndCode")]
        public async Task<IActionResult> GetUserNameAndCode(long id)
        {
            try
            {
                var chk = await permission.HasPermission(25, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }
                var item = await mainServiceManager.SysUserService.GetForUpdate<SysUserEditDto>(id);

                string empCode = "";
                if (item.Data.EmpId > 0)
                {
                    var getEmp = await mainServiceManager.InvestEmployeeService.GetOne(e => e.Id == item.Data.EmpId && e.IsDeleted == false);
                    if (getEmp.Succeeded && getEmp.Data != null)
                    {
                        empCode = getEmp.Data.EmpId ?? "";
                    }
                }

                UserNameAndEmpCodeDto obj = new UserNameAndEmpCodeDto
                {
                    UserId = item.Data.Id,
                    UserName = item.Data.UserName,
                    EmpCode = empCode
                };

                return Ok(await Result<UserNameAndEmpCodeDto>.SuccessAsync(obj));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in GetUserNameAndCode SysUsersController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetScreenProperties")]
        public async Task<IActionResult> GetScreenProperties(long screenId, long userId)
        {
            try
            {
                List<SysPropertyVM> result = new();
                var getProperty = await mainServiceManager.SysScreenPropertyService.GetAll(p => p.IsDeleted == false
                    && screenId == 0 || p.ScreenId == screenId);

                var getPermissions = await mainServiceManager.SysScreenPermissionPropertyService.GetAll(p => p.IsDeleted == false && p.UserId == userId);

                if (getProperty.Succeeded && getPermissions.Succeeded)
                {
                    var allPermissions = getPermissions.Data.ToList();
                    foreach (var item in getProperty.Data)
                    {
                        SysPropertyVM obj = new()
                        {
                            PermissionId = 0,
                            PropertyId = item.Id,
                            UserId = userId,
                            PropertyName = item.PropertyName,
                            Allow = false,
                            Value = ""
                        };
                        var getPermissionProperty = allPermissions.Where(p => p.PropertyId == item.Id).FirstOrDefault();
                        if (getPermissionProperty != null && getPermissionProperty.Id > 0)
                        {
                            obj.PermissionId = getPermissionProperty.Id;
                            obj.Allow = getPermissionProperty.Allow;
                            obj.Value = getPermissionProperty.Value;
                        }

                        result.Add(obj);
                    }
                }

                return Ok(await Result<List<SysPropertyVM>>.SuccessAsync(result));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("UpdateScreenPropertyPermission")]
        public async Task<IActionResult> UpdateScreenPropertyPermission(SysPropertyVM obj)
        {
            try
            {
                var update = await mainServiceManager.SysScreenPermissionPropertyService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }

        [HttpDelete("DeleteProperty")]
        public async Task<IActionResult> DeleteProperty(long id)
        {
            try
            {
                //var chk = await permission.HasPermission(25, PermissionType.Delete);
                //if (!chk)
                //    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.SuccessAsync());

                var delete = await mainServiceManager.SysScreenPermissionPropertyService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("Package")]
        public async Task<ActionResult> Package()
        {
            try
            {
                var chk = await permission.HasPermission(25, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var items = await mainServiceManager.SysPackagesPropertyValueService.GetAll(p => p.IsDeleted == false);
                if (items.Succeeded)
                {
                    var res = items.Data;

                    List<PackageVM> results = new List<PackageVM>();
                    foreach (var item in res)
                    {
                        switch (item.PropertyId)
                        {
                            case 1:
                                results.Add(new PackageVM { Proprty = "عدد المستخدمين التشغليين", Quantity = Convert.ToInt64(item.PropertyValue), Used = await GetCountEnableUser(1) });
                                break;
                            case 2:
                                results.Add(new PackageVM { Proprty = "عدد الفروع", Quantity = Convert.ToInt64(item.PropertyValue), Used = await GetCountBranch() });
                                break;
                            case 3:
                                results.Add(new PackageVM { Proprty = "عدد نقاط البيع اون لاين", Quantity = Convert.ToInt64(item.PropertyValue), Used = await GetCountPOS(1) });
                                break;
                            case 4:
                                results.Add(new PackageVM { Proprty = "عدد نقاط البيع اوفلاين", Quantity = Convert.ToInt64(item.PropertyValue), Used = await GetCountPOS(2) });
                                break;
                            case 5:
                                results.Add(new PackageVM { Proprty = "عدد المستخدمين الذاتيين", Quantity = Convert.ToInt64(item.PropertyValue), Used = await GetCountEnableUser(2) });
                                break;
                        }
                    }
                    return Ok(await Result<List<PackageVM>>.SuccessAsync(results));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Package SysUsersController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetPackageId")]
        public async Task<ActionResult> GetPackageId()
        {
            try
            {
                var packageId = await mainServiceManager.SysUserService.GetPackageId();
                return Ok(packageId);
            }
            catch
            {
                return Ok(await Result<long>.SuccessAsync(0));
            }
        }

        [HttpGet("GetEmpByEmail")]
        public async Task<IActionResult> GetEmpByEmail(long id)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in GetByIdForEdit SysUsersController, MESSAGE: {ex.Message}"));
            }
        }
        #endregion ============================ End =======================================


        #region ========================== Basic Actions ================================
        [HttpPost("Add")]
        public async Task<ActionResult> Add(SysUserDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(25, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                obj.UserTypeId = 1;

                //check if username is already exist
                var chkUserName = await mainServiceManager.SysUserService.GetAll(u => u.Id, u => u.Isdel == false && u.UserName.Equals(obj.UserName));
                if (chkUserName.Succeeded)
                {
                    if (chkUserName.Data.Any())
                        return Ok(await Result.FailAsync("اسم المستخدم موجود مسبقا"));

                    //check if emp code is correct
                    var chkEmpCode = await mainServiceManager.InvestEmployeeService.GetOne(e => e.Id, e => e.IsDeleted == false && e.EmpId == obj.EmpCode);
                    if (chkEmpCode.Succeeded)
                    {
                        if (chkEmpCode.Data != null && chkEmpCode.Data > 0)
                        {
                            obj.EmpId = Convert.ToInt32(chkEmpCode.Data);
                            var getEmpName = await mainServiceManager.InvestEmployeeService.GetOne(e => e.EmpName, e => e.Id == obj.EmpId);
                            if (getEmpName.Succeeded)
                                obj.UserFullname = getEmpName.Data;
                        }
                        else
                            return Ok(await Result.FailAsync("الموظف غير موجود في قائمة الموظفين"));

                        //check if can add new user
                        var chkAddNewUser = await AdditionAllowed(obj.UserType2Id ?? 0, obj.FacilityId ?? 0);
                        if (!chkAddNewUser)
                            return Ok(await Result.FailAsync("لقد وصلت الى الحد الاقصى من المستخدمين"));


                        var add = await mainServiceManager.SysUserService.Add(obj);
                        return Ok(add);
                    }
                    else
                        return Ok(await Result.FailAsync(chkEmpCode.Status.message));
                }
                else
                    return Ok(await Result.FailAsync(chkUserName.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add SysUsersController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("AddActive")]
        public async Task<ActionResult> AddActive(SysUserDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(25, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                //check emp
                var chkEmp = await hrServiceManager.HrEmployeeService.GetOne(e => e.Id, e => e.Email == obj.Email && e.IsDeleted == false);
                if (chkEmp.Succeeded)
                {
                    if (chkEmp.Data > 0)
                    {
                        return Ok(await Result.FailAsync(" هذا الموظف موجود مسبقا"));
                    }
                    else
                    {
                        //add new employee
                        InvestEmployeeDto newEmp = new InvestEmployeeDto
                        {
                            EmpName = obj.UserFullname,
                            Email = obj.Email,
                            StatusId = 10,
                            BranchId = 1
                        };
                        var addNewEmp = await mainServiceManager.InvestEmployeeService.Add(newEmp);
                        if (addNewEmp.Succeeded)
                        {
                            obj.EmpId = Convert.ToInt32(addNewEmp.Data.Id);
                        }
                    }

                    //check if username is already exist
                    var chkUserName = await mainServiceManager.SysUserService.GetAll(u => u.Id, u => u.Isdel == false && u.UserName.Equals(obj.UserName));
                    if (chkUserName.Succeeded)
                    {
                        if (chkUserName.Data.Any())
                            return Ok(await Result.FailAsync("اسم المستخدم موجود مسبقا"));

                        //var add = await mainServiceManager.SysUserService.Add(obj);
                        return Ok(/*add*/);

                    }
                    else
                        return Ok(await Result.FailAsync(chkUserName.Status.message));
                }
                else
                    return Ok(await Result.FailAsync(chkEmp.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add SysUsersController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(SysUserEditDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(25, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                obj.UserTypeId = 1;
                //check if emp code is correct
                var chkEmpCode = await mainServiceManager.InvestEmployeeService.GetOne(e => e.Id, e => e.IsDeleted == false && e.EmpId == obj.EmpCode);
                if (chkEmpCode.Succeeded)
                {
                    if (chkEmpCode.Data != null && chkEmpCode.Data > 0)
                    {
                        obj.EmpId = Convert.ToInt32(chkEmpCode.Data);
                        var getEmpName = await mainServiceManager.InvestEmployeeService.GetOne(e => e.EmpName, e => e.Id == obj.EmpId);
                        if (getEmpName.Succeeded)
                            obj.UserFullname = getEmpName.Data;

                        if (!await AdditionAllowedInEdit(obj))
                            return Ok(await Result.FailAsync("لقد وصلت الى الحد الاقصى من المستخدمين"));

                        var update = await mainServiceManager.SysUserService.Update(obj);
                        return Ok(update);
                    }
                    else
                        return Ok(await Result.FailAsync("الموظف غير موجود في قائمة الموظفين"));
                }
                else
                    return Ok(await Result.FailAsync(chkEmpCode.Status.message));

            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Edit SysUsersController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var chk = await permission.HasPermission(25, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                if (id == 1)
                    return Ok(await Result.FailAsync($"{localization.GetMainResource("UserNotDeletable")}"));

                var delete = await mainServiceManager.SysUserService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete SysUsersController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("UpdateLoginTime")]
        public async Task<ActionResult> UpdateLoginTime(SysUserLoginTimeDto obj)
        {
            try
            {
                //var chk = await permission.HasPermission(25, PermissionType.Edit);
                //if (!chk)
                //    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var update = await mainServiceManager.SysUserService.UpdateUserLoginTime(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in LoginTime SysUsersController, MESSAGE: {ex.Message}"));
            }
        }
        #endregion ============================ End =======================================


        #region ========================= Additional Actions ============================
        [HttpPost("ChkHasActvDirectory")]
        public async Task<ActionResult> ChkHasActvDirectory()
        {
            try
            {
                var chk = await permission.HasPermission(25, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var result = await configurationHelper.GetValue(168, session.FacilityId);
                return Ok(await Result<string>.SuccessAsync(result));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in ChkHasActvDirectory SysUsersController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("GetBranches")]
        public async Task<IActionResult> GetBranches()
        {
            try
            {
                var branches = await mainServiceManager.InvestBranchService.GetAll(b => b.Isdel == false);
                if (branches.Succeeded)
                {
                    List<BranchesVM> list = new List<BranchesVM>();
                    foreach (var item in branches.Data)
                    {
                        var sysgroupVM = new BranchesVM { BranchId = item.BranchId, BraName = item.BraName, BraName2 = item.BraName2 };
                        list.Add(sysgroupVM);
                    }
                    return Ok(await Result<List<BranchesVM>>.SuccessAsync(list));
                }
                return Ok(branches);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in GetSysGroups SysPoliciesProcedureController, MESSAGE: {ex.Message}"));
            }
        }
        #endregion ============================ End =======================================


        #region ======================= NonAction Actions ============================
        [NonAction]
        public async Task<bool> AdditionAllowed(int UserType2Id, long FacilityId)
        {
            if (UserType2Id == 1)
            {
                var getProperty1 = await mainServiceManager.SysPackagesPropertyValueService.GetOne(p => p.PropertyValue, p => p.IsDeleted == false && p.PropertyId == 1);
                var countUser = await GetCountEnableUser(UserType2Id);

                bool property1 = getProperty1.Data != null ? true : false;
                if (property1 == false)
                    return true;
                else if (Convert.ToInt32(getProperty1.Data) > countUser)
                    return true;
                else
                    return false;
            }
            if (UserType2Id == 2)
            {
                var getProperty1 = await mainServiceManager.SysPackagesPropertyValueService.GetOne(p => p.PropertyValue, p => p.IsDeleted == false && p.PropertyId == 5);
                var countUser = await GetCountEnableUser(UserType2Id);

                bool property1 = getProperty1.Data != null ? true : false;
                if (property1 == false)
                    return true;
                else if (Convert.ToInt32(getProperty1.Data) > countUser)
                    return true;
                else
                    return false;
            }
            return false;
        }

        [NonAction]
        public async Task<bool> AdditionAllowedInEdit(SysUserEditDto obj)
        {
            if (obj.Enable == 2 && obj.UserType2Id == obj.PreviousUserType2Id)
                return true;

            if (obj.Enable == obj.PreviousUserType2Id && obj.UserType2Id == obj.PreviousUserType2Id)
                return true;

            if (obj.PreviousUserType2Id == 1)
            {
                var getProperty1 = await mainServiceManager.SysPackagesPropertyValueService.GetOne(p => p.PropertyValue, p => p.IsDeleted == false && p.PropertyId == 1);
                var countUser = await GetCountEnableUser(obj.UserType2Id ?? 0);

                bool property1 = getProperty1.Data != null ? true : false;
                if (property1 == false)
                    return true;
                else if (Convert.ToInt32(getProperty1.Data) > countUser)
                    return true;
                else
                    return false;
            }

            if (obj.PreviousUserType2Id == 2)
            {
                var getProperty1 = await mainServiceManager.SysPackagesPropertyValueService.GetOne(p => p.PropertyValue, p => p.IsDeleted == false && p.PropertyId == 5);
                var countUser = await GetCountEnableUser(obj.UserType2Id ?? 0);

                bool property1 = getProperty1.Data != null ? true : false;
                if (property1 == false)
                    return true;
                else if (Convert.ToInt32(getProperty1.Data) > countUser)
                    return true;
                else
                    return false;
            }

            return false;
        }
        [NonAction]

        public async Task<int> GetCountEnableUser(int Type)
        {
            var countUser = await mainServiceManager.SysUserService.GetAll(u => u.Isdel == false && u.Enable == 1 && u.UserTypeId == 1 && u.UserType2Id == Type);
            return countUser.Data.Count();
        }

        [NonAction]
        public async Task<int> GetCountBranch()
        {
            var countBranch = await mainServiceManager.InvestBranchService.GetAll(u => u.Isdel == false);
            return countBranch.Data.Count();
        }

        [NonAction]
        public async Task<int> GetCountPOS(int posType)
        {
            var countPOS = await salServiceManager.SalPosSettingService.GetAll(p => p.IsDeleted == false && p.PosType == posType);
            return countPOS.Data.Count();
        }
        #endregion ============================ End =======================================


        #region ============================= DDL ===================================

        [HttpGet("DDLCustomerGroups")]
        public async Task<IActionResult> DDLCustomerGroups()
        {
            try
            {
                //var list = new SelectList(new List<DDListItem<SysTable>>());
                var list = await ddlHelper.GetAnyLis<SysCustomerGroup, int>(g => g.CusTypeId == 2 && g.IsDeleted == false, "Id", "Name");
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
                //var list = await ddlHelper.GetAnyLis<SysUser, long>(u => u.IsDeleted == false, "Id", "UserName" + "|" + "UserFullname");

                var users = await mainServiceManager.SysUserService.GetAll(u => u.IsDeleted == false);
                var list = listHelper.GetFromList<long>(users.Data.Select(s => new DDListItem<long> { Name = s.UserName + " | " + s.UserFullname, Value = s.Id }), hasDefault: false);

                return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        #endregion ============================ End ====================================


        #region ============================= change-password ===================================


        [HttpGet("GetUserChangePassword")]
        public async Task<IActionResult> GetUserChangePassword()
        {
            try
            {
                // استدعاء الخدمة للحصول على بيانات المستخدم
                var getItem = await mainServiceManager.SysUserService.GetOneVW(x => x.UserId == session.UserId);
                if (getItem.Succeeded)
                {
                    // تحويل البيانات إلى الكائن DTO
                    var obj = new SysUserchangepasswordDto
                    {
                        UserName = getItem.Data.UserName,
                        EmpName = getItem.Data.EmpName,
                        Email = getItem.Data.UserEmail,
                        TwoFactor = getItem.Data.TwoFactor,
                        TwoFactorType = getItem.Data.TwoFactorType
                    };

                    // معالجة خاصية Two-Factor
                    await ProcessTwoFactorAsync(obj);

                    // إرجاع النتيجة
                    return Ok(await Result<SysUserchangepasswordDto>.SuccessAsync(obj, ""));
                }
                else
                {
                    return Ok(getItem);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<SysUserchangepasswordDto>.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }

        private async Task ProcessTwoFactorAsync(SysUserchangepasswordDto obj)
        {
            // الحصول على الإعدادات الحالية من النظام
            string active = await configurationHelper.GetValue(139, session.FacilityId);

            if (!string.IsNullOrEmpty(active) && active != "0")
            {
                obj.TowfactorVisible = true;

                switch (active)
                {
                    case "3": // SMS and Email
                        HandleTwoFactor(obj, includeSms: true, includeEmail: true);
                        break;

                    case "1": // SMS
                        HandleTwoFactor(obj, includeSms: true, includeEmail: false);
                        break;

                    case "2": // Email
                        HandleTwoFactor(obj, includeSms: false, includeEmail: true);
                        break;

                    default:
                        DisableTwoFactor(obj);
                        break;
                }
            }
            else
            {
                obj.ChecKFactorType = false;
                obj.TrOtp2 = false;
                obj.RadioTypeOtp = 0;
                obj.TowfactorVisible = false;
            }
        }

        private void HandleTwoFactor(SysUserchangepasswordDto obj, bool includeSms, bool includeEmail)
        {
            bool isTwoFactorEnabled = obj.TwoFactor ?? false;
            if (isTwoFactorEnabled)

            {
                obj.ChecKFactorType = true;
                obj.TrOtp2 = true;

                if (obj.TwoFactorType == 1 && includeSms)
                {
                    obj.RadioTypeOtp = 1;
                }
                else if (obj.TwoFactorType == 2 && includeEmail)
                {
                    obj.RadioTypeOtp = 2;
                }
                else
                {
                    obj.ChecKFactorType = false;
                    obj.TrOtp2 = false;
                }
            }
            else
            {
                obj.RadioTypeOtp = 0;

            }


        }

        private void DisableTwoFactor(SysUserchangepasswordDto obj)
        {
            obj.ChecKFactorType = false;
            obj.TrOtp2 = false;
            obj.RadioTypeOtp = 0;
        }


        [HttpPost("ChangePassword")]
        public async Task<ActionResult> ChangePassword(SysUserchangepasswordAddDto obj)
        {
            try
            {
                // التحقق من صحة المدخلات
                if (obj == null || obj.UserPassword == null || string.IsNullOrEmpty(obj.NewPassword))
                {

                    return Ok(await Result.FailAsync($"{localization.GetMainResource("PasswordDoesNotMatch")}"));
                }

                // تحويل كلمة المرور الحالية إلى نص


                // التحقق من كلمة المرور الحالية
                var isPasswordCorrect = await mainServiceManager.SysUserService.CheckPassword(session.UserId, obj.NewPassword);

                if (isPasswordCorrect.HasValue && isPasswordCorrect.Value > 0)
                {
                    // تغيير كلمة المرور إلى الجديدة
                    var result = await mainServiceManager.SysUserService.ChangePassword(session.UserId, obj.NewPassword, obj.Email);

                    if (result)
                    {

                        return Ok(await Result.SuccessAsync($"{localization.GetMainResource("UpdateSuccess")}"));
                    }
                    else
                    {
                        return Ok(await Result.FailAsync($"{localization.GetMainResource("UpdateError")}"));
                    }
                }
                else
                {
                    return Ok(await Result.FailAsync($"{localization.GetMainResource("UpdateError")}"));
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"Error in ChangePassword: {ex.Message}"));
            }
        }

        [HttpPost("Verificationstep")]
        public async Task<ActionResult> Verificationstep(SysUserchangepasswordVerificationDto obj)
        {
            try
            {
                if (obj.ChecKFactorType)
                {
                    if (obj.RadioTypeOtp != -1)
                    {
                        var otpResult = await OTP(obj.RadioTypeOtp);
                        if (otpResult)
                        {
                            obj.DIVOtpCodeContainer = true;
                            return Ok(await Result.SuccessAsync("OTP sent successfully."));
                        }
                        else
                        {
                            return Ok(await Result.FailAsync("Failed to send OTP."));
                        }
                    }
                    else
                    {
                        return Ok(await Result.FailAsync("Invalid OTP type."));
                    }
                }
                else
                {
                    var disableResult = await DisableTwoFactor(session.UserId);
                    if (disableResult)
                    {
                        return Ok(await Result.SuccessAsync("Two-factor authentication disabled."));
                    }
                    else
                    {
                        return Ok(await Result.FailAsync("Failed to disable two-factor authentication."));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"Error in Verificationstep: {ex.Message}"));
            }
        }

        private async Task<bool> OTP(int radioTypeOtp)
        {
            try
            {
                var user = await mainServiceManager.SysUserService.GetOneVW(x => x.UserId == session.UserId);
                if (user.Succeeded && user.Data != null)
                {
                    string otp = GenerateOTP(); // Generate a 4-digit OTP.
                    var updateResult = await mainServiceManager.SysUserService.UpdateOTP(otp, session.UserId);

                    if (!updateResult)
                        return false;

                    if (radioTypeOtp == 1)
                    {
                        return await SendSMSOTP(otp, user.Data.Mobile);
                    }
                    else if (radioTypeOtp == 2)
                    {
                        await SendEmailOTP(otp, user.Data.UserEmail);
                        return true;
                    }
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        [NonAction]
        public string GenerateOTP(int length = 4)
        {
            const string numbers = "0123456789";
            var random = new Random();
            var otp = new char[length];

            for (int i = 0; i < length; i++)
            {
                otp[i] = numbers[random.Next(numbers.Length)];
            }

            return new string(otp);
        }

        [NonAction]
        public async Task<bool> SendSMSOTP(string code, string mobile)
        {
            try
            {
                var message = $"Logix Two-Factor-Auth Code: {code}";
                await sendSmsHelper.SendSms(mobile, message, false);
                return true;
            }
            catch
            {
                return false;
            }
        }

        [NonAction]
        public async Task SendEmailOTP(string code, string email)
        {
            try
            {
                var subject = "Logix Two-Factor-Auth Code";
                var message = $"Please verify the following code in order to login to Logix:\nLogix Two-Factor-Auth Code: {code}";
                await emailAppHelper.SendEmailAsync(email, subject, message);
            }
            catch (Exception ex)
            {
                // Log exception if needed
            }
        }

        [NonAction]
        public async Task<bool> DisableTwoFactor(long userId)
        {
            try
            {
                var user = await mainServiceManager.SysUserService.UpdateTwoFactor(userId);
                if (user.Succeeded)
                {

                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }



        #endregion ============================ change-password ====================================




        #region ================================اضافة مستخدم من الأكتيف ديركتوري===========================================



        [HttpPost("ActiveDirectorySearch")]
        public async Task<ActionResult> ActiveDirectorySearch(LdapLoginRequest request)
        {

            var result = GetUserLogin(request.Ldap, request.ActiveUserName, request.userName, request.pass);

            if (result.IsValidUser)
            {
                return Ok(await Result<object>.SuccessAsync(new { EmpName = result.empName, EmpEmail = result.Email }));
            }
            else
            {
                return Ok(await Result<object>.FailAsync("المستخدم غير موجود في الاكتف ديركتري"));
            }
        }


        [NonAction]
        private LdapLoginResult GetUserLogin(string? ldap, string activeUserName, string userName, string password)
        {
            DirectoryEntry? entry = null;
            var result = new LdapLoginResult();

            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    entry = new DirectoryEntry(ldap, activeUserName, password);
                    var search = new DirectorySearcher(entry)
                    {
                        Filter = $"(CN={userName})"
                    };
                    var searchResult = search.FindOne();

                    if (searchResult == null)
                    {
                        result.IsValidUser = false;
                    }
                    else
                    {
                        var user = searchResult.GetDirectoryEntry();
                        result.IsValidUser = true;
                        result.empName = user.Properties["name"]?.Value?.ToString();
                        result.Email = user.Properties["email"]?.Value?.ToString();

                    }
                }
                else
                {
                    throw new PlatformNotSupportedException("This method is only supported on Windows.");
                }
            }
            catch (Exception ex)
            {
                result.IsValidUser = false;
                result.Exception = ex.Message;
            }
            finally
            {
                entry?.Dispose();
            }

            return result;
        }

        [HttpPost("AddActiveDirectoryUser")]
        public async Task<ActionResult> AddActiveDirectoryUser(ActiveDirectoryUserAddDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(25, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var add = await mainServiceManager.SysUserService.AddActiveDirectoryUser(obj);
                return Ok(add);

            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in AddActiveDirectoryUser SysUsersController, MESSAGE: {ex.Message}"));
            }
        }

        #endregion
    }
}