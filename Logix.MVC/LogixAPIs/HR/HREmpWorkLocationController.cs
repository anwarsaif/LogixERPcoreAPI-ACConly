using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Result = Logix.Application.Wrapper.Result;

namespace Logix.MVC.LogixAPIs.HR
{


    //  تقرير بالمواقع والورديات
    public class HREmpWorkLocationController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IMainServiceManager mainServiceManager;
        public HREmpWorkLocationController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IMainServiceManager mainServiceManager)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.mainServiceManager = mainServiceManager;
        }

        #region IndexPage

        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrEmpWorkLocationFilterDto filter)
        {
            var chk = await permission.HasPermission(710, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var BranchesList = session.Branches.Split(',');
                List<HrEmpWorkLocationFilterDto> resultList = new List<HrEmpWorkLocationFilterDto>();
                var items = await hrServiceManager.HrAttShiftEmployeeMVwService.GetAllVW(e => e.IsDeleted == false && BranchesList.Contains(e.BranchId.ToString()) && e.StatusId == 1 &&
                (string.IsNullOrEmpty(filter.EmpCode) || filter.EmpCode == e.EmpCode) &&
                (string.IsNullOrEmpty(filter.EmpName) || e.EmpName.ToLower().Contains(filter.EmpName) || e.EmpName2.ToLower().Contains(filter.EmpName)) &&
                (filter.Location == 0 || filter.Location == null || filter.Location == e.Location) &&
                (filter.ShitId == 0 || filter.ShitId == null || filter.ShitId == e.ShitId)
  );
                if (items.Succeeded)
                {
                    if (items.Data.Count() > 0)
                    {

                        var res = items.Data.AsQueryable();

                        if (filter.BranchId != null && filter.BranchId > 0)
                        {
                            res = res.Where(c => c.BranchId != null && c.BranchId == filter.BranchId);
                        }

                        if (res.Any())
                        {
                            var getFromSetting = await hrServiceManager.HrSettingService.GetAll();
                            var getFromHRAllowanceDeduction = await hrServiceManager.HrAllowanceDeductionService.GetAll(x => x.IsDeleted == false && x.TypeId == 2 && x.FixedOrTemporary == 1);

                            foreach (var item in res)
                            {
                                decimal TotalAllowance = 0;
                                decimal TotalDeduction = 0;
                                decimal GOSIDeduction = 0;
                                var getTotalAllowance = await hrServiceManager.HrAllowanceDeductionService.GetTotalAllowances((long)item.EmpId);
                                if (getTotalAllowance.Succeeded) TotalAllowance = getTotalAllowance.Data;

                                var getTotalDeduction = await hrServiceManager.HrAllowanceDeductionService.GetTotalDeduction((long)(long)item.EmpId);
                                if (getTotalDeduction.Succeeded) TotalDeduction = getTotalDeduction.Data;

                                var GosiDeductionValue = getFromSetting.Data.Where(x => x.FacilityId == item.FacilityId).Select(x => x.GosiDeduction).FirstOrDefault() ?? 0;
                                var getGosiDeduction = getFromHRAllowanceDeduction.Data.Where(x => x.AdId == GosiDeductionValue && x.EmpId == item.EmpId);

                                foreach (var Gosiitem in getGosiDeduction)
                                {
                                    GOSIDeduction += (Gosiitem.Amount != null ? Gosiitem.Amount.Value : 0);
                                }
                                var newItem = new HrEmpWorkLocationFilterDto
                                {
                                    EmpCode = item.EmpCode,
                                    EmpName = item.EmpName,
                                    BranchName = item.BraName,
                                    BankName = item.BankName,
                                    GroupName = item.Name,
                                    AppointmentDate = item.Doappointment,
                                    LocationName = item.LocationName,
                                    GOSIDeduction = GOSIDeduction,
                                    NetSalary = item.Salary + TotalAllowance - TotalDeduction,
                                };
                                resultList.Add(newItem);
                            }
                            if (resultList.Count > 0) return Ok(await Result<List<HrEmpWorkLocationFilterDto>>.SuccessAsync(resultList));
                            return Ok(await Result<List<HrEmpWorkLocationFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));

                        }
                        return Ok(await Result<List<HrEmpWorkLocationFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));

                    }
                    return Ok(await Result<List<HrEmpWorkLocationFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));
                }
                return Ok(await Result<HrEmpWorkLocationFilterDto>.FailAsync(items.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrEmpWorkLocationFilterDto>.FailAsync(ex.Message));
            }
        }

        #endregion

    }
}