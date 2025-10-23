using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Result = Logix.Application.Wrapper.Result;

namespace Logix.MVC.LogixAPIs.HR
{
    //  تقرير بتاريخ تعيين الموظفين 

    public class HRRPDOAppointmentController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IMainServiceManager mainServiceManager;
        public HRRPDOAppointmentController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IMainServiceManager mainServiceManager)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.mainServiceManager = mainServiceManager;
        }

        #region IndexPage

        [HttpPost("Search")]
        public async Task<IActionResult> Search(DOAppointmentFilterDto filter)
        {
            //List<DOAppointmentFilterDto> results = new List<DOAppointmentFilterDto>();
            var chk = await permission.HasPermission(384, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {

                //var BranchesList = session.Branches.Split(',');

                //var employees = await hrServiceManager.HrEmployeeService.GetAllVW(e => e.IsDeleted == false  && e.Doappointment != null && e.Doappointment != ""&&e.Isdel==false&&e.FacilityId==session.FacilityId&& BranchesList.Contains(e.BranchId.ToString()));
                //var filteredEmployees = employees.Data
                //    .Where(e =>
                //        (string.IsNullOrEmpty(filter.EmpName) || e.EmpName.ToLower().Contains(filter.EmpName.ToLower())) &&
                //        (string.IsNullOrEmpty(filter.empCode) || e.EmpId == filter.empCode) &&
                //        (string.IsNullOrEmpty(filter.From) || string.IsNullOrEmpty(filter.To) || (DateHelper.StringToDate(e.Doappointment) >= DateHelper.StringToDate(filter.From) && DateHelper.StringToDate(e.Doappointment) <= DateHelper.StringToDate(filter.To))) &&
                //        (filter.BranchId == 0 || filter.BranchId == null || e.BranchId == filter.BranchId) &&
                //        (filter.Location == 0 || filter.Location == null || e.Location == filter.Location)&&
                //        (filter.NationalityId == 0 || filter.NationalityId == null || e.NationalityId == filter.NationalityId) &&
                //        (filter.dept == 0 || filter.dept == null || e.DeptId == filter.dept)

                //    );
                //foreach (var item in filteredEmployees)
                //{

                //    var employeeDto = new DOAppointmentFilterDto
                //    {
                //        empCode = item.EmpId,
                //        EmpName = item.EmpName ?? item.EmpName2,
                //        DoAppointment = item.Doappointment,
                //        BranchName= session.Language == 1 ? item.BraName:item.BraName2,
                //        LocationName= session.Language == 1 ? item.LocationName: item.LocationName2,
                //        DeptName= session.Language == 1 ? item.DepName:item.DepName2,
                //        JobName= session.Language == 1 ? item.CatName: item.CatName2,
                //        Nationality=session.Language==1? item.NationalityName: item.NationalityName2,
                        
                //    };

                //    results.Add(employeeDto);
                //}
                //if (results.Count > 0) return Ok(await Result<List<DOAppointmentFilterDto>>.SuccessAsync(results, ""));
                //return Ok(await Result<List<DOAppointmentFilterDto>>.SuccessAsync(results, localization.GetResource1("NosearchResult")));
        var employees = await hrServiceManager.HrEmployeeService.SearchRPDOAppointement(filter);
        return Ok(employees);


            }
            catch (Exception ex)
            {
                return Ok(await Result<DOAppointmentFilterDto>.FailAsync(ex.Message));
            }
        }


        #endregion

    }
}
