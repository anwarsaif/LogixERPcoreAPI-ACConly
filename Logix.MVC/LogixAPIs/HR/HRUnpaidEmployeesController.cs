using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Ocsp;
using Result = Logix.Application.Wrapper.Result;

namespace Logix.MVC.LogixAPIs.HR
{

    //   الموظفين الذين لم يصدر لهم رواتب
    public class HRUnpaidEmployeesController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IMainServiceManager mainServiceManager;


        public HRUnpaidEmployeesController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IMainServiceManager mainServiceManager)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.mainServiceManager = mainServiceManager;
        }
        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrUnpaidEmployeesFilter filter)
        {
            var chk = await permission.HasPermission(2075, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
				// if (filter.FinancelYear <= 0)
				// {
				//     return Ok(await Result<object>.FailAsync(" يجب اختيار السنة المالية"));
				// }
				// if (string.IsNullOrEmpty(filter.MsMonth))
				// {
				//     return Ok(await Result<object>.FailAsync(" يجب اختيار الشهر "));
				// }
				// if (!string.IsNullOrEmpty(filter.MsMonth))
				// {
				//     if (int.TryParse(filter.MsMonth, out int month))
				//     {
				//         filter.MsMonth = month.ToString("D2");
				//     }
				// }
				// filter.SponsorId ??= 0;
				// filter.StatusId ??= 0;
				// filter.NationalityId ??= 0;
				// filter.JobCatagoriesId ??= 0;
				// filter.Location ??= 0;
				// filter.FacilityId ??= 0;
				// filter.ContractType ??= 0;
				// filter.WagesProtection ??= 0;
				// var BranchesList = session.Branches.Split(',');
				// var PayrolDetails = await hrServiceManager.HrPayrollDService.GetAllVW(x => x.IsDeleted == false && x.PayrollTypeId == 1 && x.MsMonth == filter.MsMonth && x.FinancelYear == filter.FinancelYear);
				// var empIds = PayrolDetails.Data.Select(x => x.EmpId);
				// var deptList = await mainServiceManager.SysDepartmentService.GetchildDepartment((long)filter.DeptId);

				// var EmpData = await hrServiceManager.HrEmployeeService.GetAllVW(x =>
				// x.IsDeleted == false
				// && x.IsSub == false
				// && x.Isdel == false
				// && BranchesList.Contains(x.BranchId.ToString())
				// && (filter.NationalityId == 0 || filter.NationalityId == x.NationalityId)
				// && (filter.JobCatagoriesId == 0 || filter.JobCatagoriesId == x.JobCatagoriesId)
				// && (filter.StatusId == 0 || filter.StatusId == x.StatusId)
				// && (string.IsNullOrEmpty(filter.EmpName) || x.EmpName.ToLower().Contains(filter.EmpName) || x.EmpName2.ToLower().Contains(filter.EmpName))
				// && (string.IsNullOrEmpty(filter.EmpId) || x.EmpId == filter.EmpId)
				// && (string.IsNullOrEmpty(filter.IdNo) || filter.IdNo == x.IdNo)
				// && (filter.SponsorId == 0 || filter.SponsorId == x.SponsorsId)
				// && (filter.Location == 0 || filter.Location == x.Location)
				// && (filter.FacilityId == 0 || filter.FacilityId == x.FacilityId)
				// && (filter.ContractType == 0 || filter.ContractType == x.ContractTypeId)
				// && (filter.WagesProtection == 0 || filter.WagesProtection == x.WagesProtection)
				// && (string.IsNullOrEmpty(filter.EmpCode2) || filter.EmpCode2 == x.EmpCode2)
				//&& !empIds.Contains(x.Id)
				// );
				// var result = EmpData.Data.Where(x => x.Doappointment != null && DateHelper.StringToDate(x.Doappointment) < DateHelper.StringToDate($"{filter.FinancelYear}/{filter.MsMonth}/{x.Doappointment.Substring(8, 2)}")).AsQueryable();
				// if (filter.BranchId > 0)
				// {
				//     result = result.Where(x => x.BranchId == filter.BranchId);
				// }
				// if (filter.DeptId > 0)
				// {
				//     result = result.Where(x => (x.DeptId == filter.DeptId || deptList.Data.Contains(x.DeptId.ToString())));
				// }
				// if (result.Count() > 0)
				// {
				//     var projectedResult = result.Select(x => new
				//     {
				//         x.EmpId,
				//         x.EmpName,    
				//         x.EmpName2,   
				//         DepName = session.Language == 1 ? x.DepName : x.DepName2,  
				//         BraName = session.Language == 1 ? x.BraName : x.BraName2,  
				//         LocationName = session.Language == 1 ? x.LocationName : x.LocationName2,  
				//         CatName = session.Language == 1 ? x.CatName : x.CatName2, 
				//         StatusName = session.Language == 1 ? x.StatusName : x.StatusName2, 
				//         x.IdNo,
				//         x.EmpPhoto,
				//         x.Doappointment,
				//         x.ContractExpiryDate,
				//     }).ToList();

				//     return Ok(await Result<object>.SuccessAsync(projectedResult));


				// }
				// return Ok(await Result<object>.SuccessAsync(result.ToList(), localization.GetResource1("NosearchResult")));


				var EmpData = await hrServiceManager.HrEmployeeService.UnpaidEmployeesSearch(filter);
				return Ok(EmpData);


			}
            catch (Exception ex)
            {
                return Ok(await Result<PayrollAccountingEntryResultDto>.FailAsync(ex.Message));
            }
        }

    }
}