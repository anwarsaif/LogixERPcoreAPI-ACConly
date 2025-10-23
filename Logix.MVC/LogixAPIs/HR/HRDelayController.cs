using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.HR.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Result = Logix.Application.Wrapper.Result;

namespace Logix.MVC.LogixAPIs.HR
{

    //   التأخيرات
    public class HRDelayController : BaseHrApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IApiDDLHelper ddlHelper;


        public HRDelayController(IHrServiceManager hrServiceManager, IMainServiceManager mainServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IApiDDLHelper ddlHelper)
        {
            this.mainServiceManager = mainServiceManager;
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.ddlHelper = ddlHelper;
        }



        [HttpPost("Search")]


        public async Task<IActionResult> Search(HrDelayFilterDto filter)
        {
            var chk = await permission.HasPermission(177, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                //var BranchesList = session.Branches.Split(',');

                //List<HrDelayFilterDto> resultList = new List<HrDelayFilterDto>();
                //var items = await hrServiceManager.HrDelayService.GetAllVW(e => e.IsDeleted == false && BranchesList.Contains(e.BranchId.ToString()) && e.FacilityId == session.FacilityId);
                //if (items.Succeeded)
                //{
                //    if (items.Data.Count() > 0)
                //    {
                //        var res = items.Data.AsQueryable();

                //        if (!string.IsNullOrEmpty(filter.EmpCode))
                //        {
                //            res = res.Where(r => r.EmpCode != null && r.EmpCode == filter.EmpCode);
                //        }
                //        if (!string.IsNullOrEmpty(filter.EmpName))
                //        {
                //            res = res.Where(c => (c.EmpName != null && c.EmpName.ToLower().Contains(filter.EmpName.ToLower())));
                //        }
                //        if (filter.LocationId != null && filter.LocationId > 0)
                //        {
                //            res = res.Where(c => c.Location != null && c.Location == filter.LocationId);
                //        }
                //        if (filter.TypeId != null && filter.TypeId > 0)
                //        {
                //            res = res.Where(c => c.TypeId != null && c.TypeId == filter.TypeId);
                //        }
                //        if (filter.BranchId != null && filter.BranchId > 0)
                //        {
                //            res = res.Where(c => c.BranchId != null && c.BranchId == filter.BranchId);
                //        }

                //        if (!string.IsNullOrEmpty(filter.FromDate) && !string.IsNullOrEmpty(filter.ToDate))
                //        {
                //            var fromDate = DateHelper.StringToDate(filter.FromDate);
                //            var toDate = DateHelper.StringToDate(filter.ToDate);
                //            res = res.Where(r => r.DelayDate != null && DateHelper.StringToDate(r.DelayDate) >= fromDate && DateHelper.StringToDate(r.DelayDate) <= toDate);
                //        }


                //        foreach (var item in res)
                //        {
                //            var delayValue = await hrServiceManager.HrPolicyService.ApplyPoliciesAsync(session.FacilityId, 2, item.EmpId);

                //            // Ensure item.DelayTime is TimeSpan? and convert it to decimal if needed
                //            var delayTimeInMinutes = item.DelayTime.HasValue ? (decimal)item.DelayTime.Value.TotalMinutes : 0m;

                //            // Assuming DailyWorkingHours is a decimal
                //            var dailyWorkingHours = item.DailyWorkingHours ?? 1m; // Ensure non-zero

                //            var delayTime = delayTimeInMinutes * (delayValue / 30m / dailyWorkingHours / 60m);

                //            var newRecord = new HrDelayFilterDto
                //            {
                //                Id = item.Id,
                //                EmpName = item.EmpName,
                //                EmpId = item.EmpId,
                //                EmpCode = item.EmpCode,
                //                Note = item.Note,
                //                DeptName = item.DepName,
                //                LocationName = item.LocationName,
                //                DelayDate = item.DelayDate,
                //                DelayTime = item.DelayTime.HasValue ? item.DelayTime.Value.ToString(@"hh\:mm\:ss") : "00:00:00",
                //                DelayDuration = delayTime.ToString("F2"),
                //                TypeName = (item.TypeId == 2) ? "تقصير" : (item.TypeId == 1 ? "تاخير" : ""),
                //            };

                //            resultList.Add(newRecord);
                //        }


                //        if (resultList.Count() > 0)
                //            return Ok(await Result<List<HrDelayFilterDto>>.SuccessAsync(resultList, ""));

                //        return Ok(await Result<List<HrDelayFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));
                //    }

                //    return Ok(await Result<List<HrDelayFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));
                //}

                //return Ok(await Result<HrDelayFilterDto>.FailAsync(items.Status.message));
                var items = await hrServiceManager.HrDelayService.Search(filter);
                return Ok(items);

			}
            catch (Exception ex)
            {
                return Ok(await Result<HrDelayFilterDto>.FailAsync(ex.Message));
            }
        }


        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var chk = await permission.HasPermission(177, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await hrServiceManager.HrDelayService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete HR delay Controller, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("DeleteAllSelected")]
        public async Task<IActionResult> DeleteAllSelected(List<long> id)
        {
            try
            {
                var chk = await permission.HasPermission(177, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!id.Any())
                    return Ok(await Result.FailAsync($"قم بتحديد السجلات المراد حذفها"));

                var delete = await hrServiceManager.HrDelayService.DeleteAllSelected(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete HR delay Controller, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Add")]
        public async Task<ActionResult> Add(HrDelayAddDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(177, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result<HrDelayDto>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));
                if (string.IsNullOrEmpty(obj.DelayTimeString) || obj.DelayTimeString == "00:00")
                {
                    return Ok(await Result<HrDelayDto>.FailAsync("    مدة التأخير مطلوب "));

                }
                if (obj.TypeId <= 0)
                    return Ok(await Result<HrDelayDto>.FailAsync($"TypeId is Required"));
                if (string.IsNullOrEmpty(obj.EmpCode)) return Ok(await Result<HrDelayDto>.FailAsync($"EmpCode is Required"));
                if (string.IsNullOrEmpty(obj.DelayDate)) return Ok(await Result<HrDelayDto>.FailAsync($"DelayDate is Required"));

                var add = await hrServiceManager.HrDelayService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrDelayDto>.FailAsync($"====== Exp in Add HR   Delay Controller  Add Method, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("DelayNonCheckout")]
        public async Task<ActionResult> DelayNonCheckout(HrDelayNonCheckoutDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(177, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result<HrDelayNonCheckoutDto>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var add = await hrServiceManager.HrDelayService.DelayNonCheckout(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrDelayNonCheckoutDto>.FailAsync($"====== Exp in Add HR   Delay Controller  Add Method, MESSAGE: {ex.Message}"));
            }
        }



        [HttpGet("DDLType")]
        public async Task<IActionResult> DDLType()
        {
            try
            {
                List<SelectListItem> lististItems = new List<SelectListItem>();
                lististItems.Insert(0, new SelectListItem
                { Value = "0", Text = "الكل" });
                lististItems.Insert(1, new SelectListItem
                { Value = "1", Text = "تأخير" });
                lististItems.Insert(2, new SelectListItem
                { Value = "2", Text = "تقصير" });
                return Ok(await Result<object>.SuccessAsync(lististItems, ""));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }

        }
    }
}