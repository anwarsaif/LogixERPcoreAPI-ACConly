using Logix.Application.Common;
using Logix.Application.DTOs.Main;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.Main
{
    public class TwoFactorController : BaseMainApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly ISysConfigurationHelper sysConfigurationHelper;
        private readonly ICurrentData currentData;

        public TwoFactorController(IMainServiceManager mainServiceManager,
            ISysConfigurationHelper sysConfigurationHelper,
            ICurrentData currentData)
        {
            this.mainServiceManager = mainServiceManager;
            this.currentData = currentData;
            this.sysConfigurationHelper = sysConfigurationHelper;
        }


        [HttpGet("GetActivationPropertyValue")]
        public async Task<IActionResult> GetActivationPropertyValue()
        {
            try
            {
                var item = await sysConfigurationHelper.GetValue(139, currentData.FacilityId);
                return Ok(await Result<string>.SuccessAsync(item));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("SetActivationPropertyValue")]
        public async Task<IActionResult> SetActivationPropertyValue(string value)
        {
            try
            {
                var item = await mainServiceManager.SysPropertyValueService.SetPropertyValue(139, currentData.FacilityId, value);
                return Ok(await Result<object>.SuccessAsync(item));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }


        [HttpPost("GetUsers")]
        public async Task<ActionResult> GetUsers(SysUserFilterDto filter)
        {
            try
            {
                bool twoFactor = filter.Enable == 1;
                var branchsIds = currentData.Branches.Split(',');
                var items = await mainServiceManager.SysUserService.GetAllVW(u => u.IsDeleted == false
                && u.UserTypeId == 1
                && (filter.Enable == null || (u.TwoFactor != null && u.TwoFactor == twoFactor) || (!twoFactor && u.TwoFactor == null))
                && (string.IsNullOrEmpty(filter.GroupsId) || (!string.IsNullOrEmpty(u.GroupsId) && u.GroupsId.Equals(filter.GroupsId)))
                && (string.IsNullOrEmpty(filter.UserFullname) || (!string.IsNullOrEmpty(u.UserFullname) && u.UserFullname.Contains(filter.UserFullname)))
                && (string.IsNullOrEmpty(filter.EmpCode) || (!string.IsNullOrEmpty(u.EmpCode) && u.EmpCode.Equals(filter.EmpCode)))
                && (filter.FacilityId == null || (u.FacilityId != null && u.FacilityId == filter.FacilityId))
                );
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();

                    if (filter.BranchId > 0)
                        res = res.Where(u => u.UserPkId == filter.BranchId);
                    else
                        res = res.Where(u => branchsIds.Contains(u.UserPkId.ToString()));

                    var final = res.ToList();
                    List<SysUserFilterDto> results = new();
                    foreach (var item in final)
                    {
                        if (currentData.GroupId != 1)
                        {
                            var getSysId = await mainServiceManager.SysGroupService.GetOne(g => g.SystemId, g => g.GroupId == currentData.GroupId);
                            if (getSysId.Succeeded)
                            {
                                var getSysIdByUsrGrp = await mainServiceManager.SysGroupService.GetOne(g => g.SystemId, g => g.GroupId == Convert.ToInt32(item.GroupsId));
                                if (getSysIdByUsrGrp.Succeeded)
                                {
                                    if (getSysId.Data != getSysIdByUsrGrp.Data)
                                        continue;
                                }
                            }
                        }

                        string lastLgn = "";
                        if (item.LastLogin != null)
                        {
                            DateTime currentDate = DateTime.Now;
                            // difference between dates
                            TimeSpan difference = currentDate - item.LastLogin.Value;

                            // difference in days, hours and minutes
                            lastLgn = $"{difference.Days} يوم - {difference.Hours} ساعة - {difference.Minutes} دقيقة";
                        }
                        SysUserFilterDto result = new()
                        {
                            Id = item.UserId,
                            UserFullname = item.UserFullname,
                            BraName = item.BraName,
                            BraName2 = item.BraName2,
                            UserName = item.UserName,
                            UserEmail = item.UserEmail,
                            Mobile = item.Mobile,
                            lastLogin = lastLgn,
                            IsChecked = false
                        };
                        results.Add(result);
                    }
                    return Ok(await Result<List<SysUserFilterDto>>.SuccessAsync(results));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("Activate")]
        public async Task<ActionResult> Activate(string usersId, int twoFactorType)
        {
            try
            {
                var activate = await mainServiceManager.SysUserService.ActivateTwoFactor(usersId, twoFactorType);
                return Ok(activate);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("Deactivate")]
        public async Task<ActionResult> Deactivate(string usersId)
        {
            try
            {
                var deactivate = await mainServiceManager.SysUserService.DeactivateTwoFactor(usersId);
                return Ok(deactivate);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
    }
}