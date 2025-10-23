using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.DTOs.Main;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Result = Logix.Application.Wrapper.Result;

namespace Logix.MVC.LogixAPIs.HR
{

    //  تقرير بالموظفين
    public class HRRPEmployeeController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IMainServiceManager mainServiceManager;


        public HRRPEmployeeController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IMainServiceManager mainServiceManager)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.mainServiceManager = mainServiceManager;
        }


        #region IndexPage

        [HttpPost("Search")]
        public async Task<IActionResult> Search(HRRPEmployeeFilterDto filter)
        {
            var chk = await permission.HasPermission(1919, PermissionType.Show);
            if (!chk)
            {
                return BadRequest(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                List<HRRPEmployeeFilterDto> resultList = new List<HRRPEmployeeFilterDto>();

                var items = await hrServiceManager.HrEmployeeService.GetAllVW(e => e.IsDeleted == false && e.IsSub == false && e.IsDeleted == false
               && (filter.BranchId == null || filter.BranchId == 0 || filter.BranchId == e.BranchId)
               && (string.IsNullOrEmpty(filter.EmpName) || e.EmpName.ToLower().Contains(filter.EmpName.ToLower()) || e.EmpName2.ToLower().Contains(filter.EmpName.ToLower()))
               && (string.IsNullOrEmpty(filter.EmpCode) || e.EmpId == filter.EmpCode)
               && (filter.JobType == null || filter.JobType == 0 || filter.JobType == e.JobType)
               && (filter.JobCatagoriesId == null || filter.JobCatagoriesId == 0 || filter.JobCatagoriesId == e.JobCatagoriesId)
               && (filter.Status == null || filter.Status == 0 || filter.Status == e.StatusId)
               && (filter.NationalityId == null || filter.NationalityId == 0 || filter.NationalityId == e.NationalityId)
               && (filter.DeptId == null || filter.DeptId == 0 || filter.DeptId == e.DeptId)
               && (string.IsNullOrEmpty(filter.IdNo) || e.IdNo == filter.IdNo)
               && (string.IsNullOrEmpty(filter.PassId) || e.PassportNo == filter.PassId)
               && (string.IsNullOrEmpty(filter.EntryNo) || e.EntryNo == filter.EntryNo)
               && (filter.Location == null || filter.Location == 0 || filter.Location == e.Location)
               && (filter.SponsorsID == null || filter.SponsorsID == 0 || filter.SponsorsID == e.SponsorsId)
               && (filter.FacilityId == null || filter.FacilityId == 0 || filter.FacilityId == e.FacilityId)
               && (filter.Level == null || filter.Level == 0 || filter.Level == e.LevelId)
               && (filter.Degree == null || filter.Degree == 0 || filter.Degree == e.DegreeId)
               && (filter.ContractType == null || filter.ContractType == 0 || filter.ContractType == e.ContractTypeId)
               && (filter.Protection == null || filter.Protection == 0 || filter.Protection == e.WagesProtection)
               && (string.IsNullOrEmpty(filter.EmpCode2) || e.EmpCode2 == filter.EmpCode2)
                );
                if (items.Succeeded)
                {
                    if (items.Data.Count() > 0)
                    {

                        var res = items.Data.AsQueryable();

                        if (res.Count() > 0)
                        {
                            var getFromLeave = await hrServiceManager.HrLeaveService.GetAllVW(x => x.IsDeleted == false);
                            foreach (var item in res)
                            {
                                string? getLeaveDate = getFromLeave.Data.Where(x => x.EmpId == item.Id).Select(x => x.LeaveDate).FirstOrDefault();
                                var newItem = new HRRPEmployeeFilterDto
                                {
                                    EmpCode = item.EmpId,
                                    EmpName = item.EmpName,
                                    EmpPhoto = item.EmpPhoto ?? "",
                                    EmpName2 = item.EmpName2 ?? "",
                                    BranchName = item.BraName ?? "",
                                    DeptName = item.DepName ?? "",
                                    IdNo = item.IdNo,
                                    LocationName = item.LocationName ?? "",
                                    Catname = item.CatName ?? "",
                                    DOAppointment = item.Doappointment ?? "",
                                    ContractexpiryDate = item.ContractExpiryDate ?? "",
                                    LeaveDate =string.IsNullOrEmpty(getLeaveDate)? "تحت الخدمة ": getLeaveDate,
                                    StatusName = item.StatusName ?? ""
                                };
                                resultList.Add(newItem);
                                getLeaveDate = "";
                            }

                            if (resultList.Count > 0) return Ok(await Result<List<HRRPEmployeeFilterDto>>.SuccessAsync(resultList));
                            return Ok(await Result<List<HRRPEmployeeFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));

                        }
                        return Ok(await Result<List<HRRPEmployeeFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));

                    }
                    return Ok(await Result<List<HRRPEmployeeFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));

                }

                return Ok(await Result<object>.FailAsync(items.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync(ex.Message));

            }
        }

        #endregion

    }
}