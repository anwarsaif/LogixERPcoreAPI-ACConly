using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Helpers;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using Result = Logix.Application.Wrapper.Result;

namespace Logix.MVC.LogixAPIs.HR
{

    // تقرير انتهاء الهوية للموظفين
    public class HREmpIDExpireReportController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IMainServiceManager mainServiceManager;
        public HREmpIDExpireReportController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IMainServiceManager mainServiceManager)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.mainServiceManager = mainServiceManager;
        }

        #region IndexPage

        [HttpPost("Search")]
        public async Task<IActionResult> Search(HREmpIDExpireReportFilterDto filter)
        {
            var chk = await permission.HasPermission(258, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
        //var BranchesList = session.Branches.Split(',');

        //List<HREmpIDExpireReportFilterDto> resultList = new List<HREmpIDExpireReportFilterDto>();
        //var items = await hrServiceManager.HrEmployeeService.GetAll(e => e.IsDeleted == false && e.StatusId == 1 && e.Isdel == false && e.IdExpireDate != null && e.IdExpireDate != "" &&
        //BranchesList.Contains(e.BranchId.ToString()) &&
        //session.FacilityId == session.FacilityId &&
        //(string.IsNullOrEmpty(filter.EmpCode) || filter.EmpCode == e.EmpId) &&
        //(string.IsNullOrEmpty(filter.EmpName) || (e.EmpName != null && e.EmpName.ToLower().Contains(filter.EmpName)) || (e.EmpName2 != null && e.EmpName2.ToLower().Contains(filter.EmpName))) &&
        //(filter.LocationId == 0 || filter.LocationId == null || filter.LocationId == e.Location) &&
        //(filter.NationalityId == 0 || filter.NationalityId == null || filter.NationalityId == e.NationalityId)


        //);
        //if (items.Succeeded)
        //{
        //    if (items.Data.Count() > 0)
        //    {

        //        var res = items.Data.AsQueryable();
        //        if (filter.BranchId != null && filter.BranchId > 0)
        //        {
        //            res = res.Where(c => c.BranchId != null && c.BranchId == filter.BranchId);
        //        }

        //        if (!string.IsNullOrEmpty(filter.FromDate) && !string.IsNullOrEmpty(filter.ToDate))
        //        {
        //            res = res.Where(c => (c.IdExpireDate != null && DateHelper.StringToDate(c.IdExpireDate) >= DateHelper.StringToDate(filter.FromDate) && DateHelper.StringToDate(c.IdExpireDate) <= DateHelper.StringToDate(filter.ToDate)));
        //        }
        //        if (res.Count() >= 0)
        //        {
        //            foreach (var item in res)
        //            {
        //                int? remainingDays = 0;
        //                var isHijri = Bahsas.IsHijri(item.IdExpireDate, session);
        //                if (isHijri)
        //                {
        //                    var getEqualDate = Bahsas.HijriToGreg(item.IdExpireDate);
        //                    remainingDays = (DateHelper.StringToDate(getEqualDate) - DateTime.Now).Days;
        //                }
        //                else
        //                {
        //                    remainingDays = (DateHelper.StringToDate(item.IdExpireDate) - DateTime.Now).Days;
        //                }

        //                var newItem = new HREmpIDExpireReportFilterDto
        //                {
        //                    EmpCode = item.EmpId,
        //                    EmpName = item.EmpName,
        //                    IdNo = item.IdNo,
        //                    IDExpireDate = item.IdExpireDate,
        //                    RemainingDays = remainingDays,

        //                };
        //                resultList.Add(newItem);
        //            }
        //        }
        //        if (resultList.Count > 0)
        //            return Ok(await Result<List<HREmpIDExpireReportFilterDto>>.SuccessAsync(resultList));
        //        return Ok(await Result<List<HREmpIDExpireReportFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));

        //    }
        //    return Ok(await Result<List<HREmpIDExpireReportFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));

        //}
        //return Ok(await Result<List<HREmpIDExpireReportFilterDto>>.FailAsync(items.Status.message.ToString()));
               var items = await hrServiceManager.HrEmployeeService.SearchEmpIDExpireReport(filter);
                  return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HREmpIDExpireReportFilterDto>.FailAsync(ex.Message));
            }
        }

        [HttpPost("ExpireUpdate")]
        public async Task<IActionResult> ExpireUpdate(HREmpIDExpireUpdateDto obj)
        {
            var chk = await permission.HasPermission(258, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (obj.Duration <= 0)
                    return Ok(await Result<object>.FailAsync("يجب اختيار مدة التجديد"));
                if (obj.EmpCode.Count < 1)
                    return Ok(await Result<object>.FailAsync("لم يتم اختيار اي موظف للتحديث "));
                if (obj.EmpCode.Any(x => string.IsNullOrEmpty(x)))
                    return Ok(await Result<object>.FailAsync("يجب ادخال ارقام الموظفين لجميع العناصر المحددة"));
                var update = await mainServiceManager.InvestEmployeeService.UpdateIDExpair(obj);

                return Ok(update);

            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync(ex.Message));
            }
        }

        #endregion

    }
}
