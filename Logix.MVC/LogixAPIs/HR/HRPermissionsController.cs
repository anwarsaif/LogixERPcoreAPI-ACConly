using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using Result = Logix.Application.Wrapper.Result;

namespace Logix.MVC.LogixAPIs.HR
{
    // الاستئذان
    public class HRPermissionsController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;


        public HRPermissionsController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
        }


        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrPermissionFilterDto filter)
        {
            var chk = await permission.HasPermission(260, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                //List<HrPermissionFilterDto> resultList = new List<HrPermissionFilterDto>();
                //var items = await hrServiceManager.HrPermissionService.GetAllVW(e => e.IsDeleted == false && e.FacilityId == session.FacilityId);
                //if (items.Succeeded)
                //{
                //    if (items.Data.Any())
                //    {
                //        var res = items.Data.AsQueryable();

                //        if (!string.IsNullOrEmpty(filter.EmpName))
                //        {
                //            res = res.Where(r => r.EmpName != null && r.EmpName.Contains(filter.EmpName));
                //        }
                //        if (filter.EmpId > 0 && filter.EmpId != null)
                //        {
                //            res = res.Where(r => r.EmpCode != null && r.EmpCode == filter.EmpId.ToString());
                //        }
                //        if (filter.LocationId != null && filter.LocationId > 0)
                //        {
                //            res = res.Where(c => c.Location != null && c.Location == filter.LocationId);
                //        }
                //        if (filter.TypeId != null && filter.TypeId > 0)
                //        {
                //            res = res.Where(c => c.Type != null && c.Type == filter.TypeId);
                //        }
                //        if (filter.BranchId != null && filter.BranchId > 0)
                //        {
                //            res = res.Where(c => c.BranchId != null && c.BranchId == filter.BranchId);
                //        }
                //        if (filter.ReasonLeave != null && filter.ReasonLeave > 0)
                //        {
                //            res = res.Where(c => c.ReasonLeave != null && c.ReasonLeave == filter.ReasonLeave);
                //        }
                //        if (filter.DeptId != null && filter.DeptId > 0)
                //        {
                //            res = res.Where(c => c.DeptId != null && c.DeptId == filter.DeptId);
                //        }

                //        if (!string.IsNullOrEmpty(filter.FromDate) && !string.IsNullOrEmpty(filter.ToDate))
                //        {
                //            res = res.Where(r => r.PermissionDate != null && DateHelper.StringToDate(r.PermissionDate) >= DateHelper.StringToDate(filter.FromDate) && DateHelper.StringToDate(r.PermissionDate) <= DateHelper.StringToDate(filter.ToDate));
                //        }

                //        if (!string.IsNullOrEmpty(filter.FromTime) && !string.IsNullOrEmpty(filter.ToTime))
                //        {
                //            res = res.Where(r => r.LeaveingTime != null && TimeSpan.Parse(r.LeaveingTime) >= TimeSpan.Parse(filter.FromTime) && TimeSpan.Parse(r.LeaveingTime) <= TimeSpan.Parse(filter.ToTime));
                //        }

                //        foreach (var item in res)
                //        {
                //            var newRecord = new HrPermissionFilterDto
                //            {
                //                Id = item.Id,
                //                Empcode = item.EmpCode,
                //                EmpName = item.EmpName,
                //                LocName = item.LocationName,
                //                DepName = item.DepName,
                //                BraName = item.BraName,
                //                PermissionDate = item.PermissionDate,
                //                TypeName = item.TypeName,
                //                reasonName = item.ReasonName,
                //                LeaveingTime = item.LeaveingTime,
                //                EstimatedTimeReturn = item.EstimatedTimeReturn,
                //                TimeDifference = item.TimeDifference,
                //                ContactNumber = item.ContactNumber,

                //            };
                //            resultList.Add(newRecord);
                //        }
                //        if (resultList.Any())
                //            return Ok(await Result<List<HrPermissionFilterDto>>.SuccessAsync(resultList, ""));
                //        return Ok(await Result<List<HrPermissionFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));
                //    }
                //    return Ok(await Result<List<HrPermissionFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));
                //}
                //return Ok(await Result<HrPermissionFilterDto>.FailAsync(items.Status.message));
                var items = await hrServiceManager.HrPermissionService.Search(filter);
                return Ok(items);

			}
            catch (Exception ex)
            {
                return Ok(await Result<HrPermissionFilterDto>.FailAsync(ex.Message));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var chk = await permission.HasPermission(260, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await hrServiceManager.HrPermissionService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete HR Permissions Controller, MESSAGE: {ex.Message}"));
            }
        }


        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long Id)
        {
            try
            {
                var chk = await permission.HasPermission(260, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id <= 0)
                {
                    return Ok(await Result<HrPermissionsVw>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var item = await hrServiceManager.HrPermissionService.GetOneVW(x => x.Id == Id);
                if (item.Succeeded)
                {
                    return Ok(item);
                }
                return Ok(await Result<HrPermissionsVw>.FailAsync(item.Status.message));

            }
            catch (Exception ex)
            {
                return Ok(await Result<HrPermissionsVw>.FailAsync($"====== Exp in HR Holiday Controller getById, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(HrPermissionEditDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(260, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result<HrPermissionEditDto>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));
                var update = await hrServiceManager.HrPermissionService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrPermissionEditDto>.FailAsync($"====== Exp in Edit HR Permission  Controller, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Add")]
        public async Task<ActionResult> Add(HrPermissionDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(260, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result<HrPermissionDto>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));
                if (obj.Type<=0)
                    return Ok(await Result<HrPermissionDto>.FailAsync($"يجب اختيار النوع "));
                                if (obj.ReasonLeave <= 0)
                    return Ok(await Result<HrPermissionDto>.FailAsync($"يجب اختيار السبب "));


                var add = await hrServiceManager.HrPermissionService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrPermissionDto>.FailAsync($"====== Exp in Add HR   Permission Controller  Add Method, MESSAGE: {ex.Message}"));
            }
        }

    }
}