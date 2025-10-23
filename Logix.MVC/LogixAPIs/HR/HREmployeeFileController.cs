using Microsoft.AspNetCore.Mvc;
using Logix.MVC.Helpers;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Common;
using Logix.Application.DTOs.Main;
using Logix.Application.Wrapper;
using Logix.Application.DTOs.HR.EmployeeDto;
using Logix.Domain.HR;
using AutoMapper;
using Logix.Application.Helpers;
using Logix.Application.DTOs.HR;

namespace Logix.MVC.LogixAPIs.HR
{

    //ملف الموظف  
    public class HREmployeeFileController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;

        public HREmployeeFileController(IHrServiceManager hrServiceManager,
            IPermissionHelper permission,
            ILocalizationService localization,
            ICurrentData session
            )
        {
            this.hrServiceManager = hrServiceManager;
            this.permission = permission;
            this.session = session;
            this.localization = localization;
        }

        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrEmployeeFileFilterDto filter)
        {
            var chk = await permission.HasPermission(198, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
        // List<HrEmployeeFileFilterDto> result = new List<HrEmployeeFileFilterDto>();
        // filter.DeptId ??= 0;
        // filter.Location ??= 0;
        // filter.BranchId ??= 0;
        // filter.Status ??= 0;
        // filter.JobCatagoriesId ??= 0;
        // filter.JobType ??= 0;
        // filter.NationalityId ??= 0;
        // filter.SponsorsID ??= 0;
        // var BranchesList = session.Branches.Split(',');

        // var items = await hrServiceManager.HrEmployeeService.GetAllVW(e => e.IsDeleted == false && e.Isdel == false
        //&& BranchesList.Contains(e.BranchId.ToString())
        //&& (string.IsNullOrEmpty(filter.EmpName) || e.EmpName.ToLower().Contains(filter.EmpName.ToLower()) || e.EmpName2.ToLower().Contains(filter.EmpName.ToLower()))
        //&& (string.IsNullOrEmpty(filter.EmpCode) || e.EmpId == filter.EmpCode)
        //&& (filter.JobType == 0 || filter.JobType == e.JobType)
        //&& (filter.JobCatagoriesId == 0 || filter.JobCatagoriesId == e.JobCatagoriesId)
        //&& (filter.Status == 0 || filter.Status == e.StatusId)
        //&& (filter.NationalityId == 0 || filter.NationalityId == e.NationalityId)
        //&& (filter.DeptId == 0 || filter.DeptId == e.DeptId)
        //&& (string.IsNullOrEmpty(filter.IdNo) || e.IdNo == filter.IdNo)
        //&& (string.IsNullOrEmpty(filter.PassId) || e.PassportNo == filter.PassId)
        //&& (string.IsNullOrEmpty(filter.EntryNo) || e.EntryNo == filter.EntryNo)
        //&& (filter.Location == 0 || filter.Location == e.Location)
        //&& (filter.SponsorsID == 0 || filter.SponsorsID == e.SponsorsId)
        // );
        // if (items.Succeeded)
        // {
        //     if (items.Data.Count() > 0)
        //     {
        //         var res = items.Data.AsQueryable();
        //         if (filter.BranchId > 0)
        //         {
        //             res = res.Where(c => c.BranchId != null && c.BranchId == filter.BranchId);
        //         }
        //         if (res.Count() > 0)
        //         {
        //             foreach (var item in res)
        //             {
        //                 var singleRecord = new HrEmployeeFileFilterDto
        //                 {
        //                     Id=item.Id,
        //                     EmpCode = item.EmpId,
        //                     EmpName = item.EmpName ?? "",
        //                     EmpName2 = item.EmpName2 ?? "",
        //                     IdNo = item.IdNo,
        //                     DeptName = (session.Language == 1) ? item.DepName : item.DepName2,
        //                     CatName = (session.Language == 1) ? item.CatName : item.CatName2,
        //                     StatusName = (session.Language == 1) ? item.StatusName : item.StatusName2,
        //                 };
        //                 result.Add(singleRecord);
        //             }
        //             return Ok(await Result<List<HrEmployeeFileFilterDto>>.SuccessAsync(result));

        //         }
        //         return Ok(await Result<List<HrEmployeeFileFilterDto>>.SuccessAsync(result, localization.GetResource1("NosearchResult")));
        //     }
        //     return Ok(await Result<List<HrEmployeeFileFilterDto>>.SuccessAsync(result, localization.GetResource1("NosearchResult")));

        // }

        // return Ok(await Result<object>.FailAsync(items.Status.message));
                var items = await hrServiceManager.HrEmployeeService.SearchEmployeeFile(filter);
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync(ex.Message));

            }
        }

    }
}
