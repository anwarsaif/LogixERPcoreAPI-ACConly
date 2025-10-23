using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IRepositories;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Interfaces.IServices.HR;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.HR.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using NuGet.Common;
using System.Globalization;

namespace Logix.MVC.LogixAPIs.HR
{

    // تقرير بالتأمين 
    public class RPInsuranceEmpController : BaseHrApiController
    {
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IHrServiceManager hrServiceManager;

        public RPInsuranceEmpController(IPermissionHelper permission, ILocalizationService localization, ICurrentData session, IHrServiceManager hrServiceManager)
        {
            this.permission = permission;
            this.session = session;
            this.localization = localization;
            this.hrServiceManager = hrServiceManager;
        }

        [HttpPost("Search")]

        public async Task<IActionResult> Search(HrInsuranceEmpfilterRPDto filter)
        {
            List<HrInsuranceEmpResulteDto> InsuranceList = new List<HrInsuranceEmpResulteDto>();
            filter.PolicyId ??= 0;
            filter.DeptId ??= 0;
            filter.Location ??= 0;
            filter.InsuranceType ??= 0;
            filter.StatusId ??= 0;
            filter.ClassId ??= 0;
            filter.BranchId ??= 0;
            var chk = await permission.HasPermission(1254, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var BranchesList = session.Branches.Split(',');

                var items = await hrServiceManager.HrInsuranceEmpService.GetAllVW(e => e.IsDeleted == false &&
                BranchesList.Contains(e.BranchId.ToString()) &&
                (filter.PolicyId == 0 || e.PolicyId == filter.PolicyId) &&
                (filter.DeptId == 0 || e.DeptId == filter.DeptId) &&
                (filter.Location == 0 || e.Location == filter.Location) &&
                (filter.InsuranceType == 0 || e.InsuranceType == filter.InsuranceType) &&
                (filter.StatusId == 0 || e.StatusId == filter.StatusId) &&
                (filter.ClassId == 0 || e.ClassId == filter.ClassId) &&
                (filter.BranchId == 0 || e.BranchId == filter.BranchId) &&
                (string.IsNullOrEmpty(filter.EmpCode) || e.EmpCode == filter.EmpCode)
                );

                if (!items.Succeeded)
                {
                    return Ok(await Result.FailAsync(items.Status.message));

                }


                var res = items.Data.AsQueryable();
                var RefranceInsEmpID = await hrServiceManager.HrInsuranceEmpService.GetAll(x => x.RefranceInsEmpId);
                res = res.Where(x => !RefranceInsEmpID.Data.Contains(x.Id));

                if (!string.IsNullOrEmpty(filter.StartDate) && (!string.IsNullOrEmpty(filter.EndDate)))
                {
                    var StartDate = DateHelper.StringToDate(filter.StartDate);
                    var EndDate = DateHelper.StringToDate(filter.EndDate);

                    res = res.Where(x =>
                        DateHelper.StringToDate(x.CreatedOn.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture)) >= StartDate &&
                        DateHelper.StringToDate(x.CreatedOn.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture)) <= EndDate);
                }

                if (res.Any())
                {
                    foreach (var item in res)
                    {
                        var newRow = new HrInsuranceEmpResulteDto
                        {
                            EmpCode = item.EmpCode,
                            EmpName = item.EmpName,
                            CreatedOn = item.CreatedOn.ToString("yyyy/mm/dd", CultureInfo.InvariantCulture),
                            DependentName = item.DependentName,
                            DepName = item.DepName,
                            LocationName = item.LocationName,
                            StatusName = item.StatusName,
                            PolicyCode = item.PolicyCode,
                            ClassName = item.ClassName,
                            Amount = item.Amount,
                        };
                        InsuranceList.Add(newRow);
                    }
                    if (InsuranceList.Count > 0)
                        return Ok(await Result<List<HrInsuranceEmpResulteDto>>.SuccessAsync(InsuranceList, ""));
                    return Ok(await Result<List<object>>.SuccessAsync(new List<object>(), localization.GetResource1("NosearchResult")));

                }
                return Ok(await Result<List<object>>.SuccessAsync(new List<object>(), localization.GetResource1("NosearchResult")));



            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }


    }
}