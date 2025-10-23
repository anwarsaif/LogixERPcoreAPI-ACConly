using Logix.Application.Common;
using Logix.Application.DTOs.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Logix.MVC.LogixAPIs.HR
{
    public class HrSettingApiController : BaseHrApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly ILocalizationService localization;
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly IDDListHelper listHelper;
        private readonly IApiDDLHelper ddlHelper;
        public HrSettingApiController(IHrServiceManager hrServiceManager, IDDListHelper listHelper, IMainServiceManager mainServiceManager, ICurrentData session, IPermissionHelper permission, IApiDDLHelper ddlHelper, ILocalizationService localization)
        {
            this.mainServiceManager = mainServiceManager;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.permission = permission;
            this.listHelper = listHelper;
            this.ddlHelper = ddlHelper;
            this.localization = localization;   
        }
        [NonAction]
        private void setErrors()
        {
            var errors = new ErrorsHelper(ModelState);
        }
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            setErrors();
            var chk = await permission.HasPermission(663, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.FailAsync("AccessDenied"));
            }
            try
            {
                var getItem = await hrServiceManager.HrSettingService.GetOne(i=>i.FacilityId== session.FacilityId);
                var hrVaues = await hrServiceManager.HrPayrollTransactionTypeValueService.GetAll(i=>i.FacilityId== session.FacilityId);
                getItem.Data.hrPayrollTransactionTypeValues = hrVaues.Data.ToList();
                if (!getItem.Succeeded && getItem.Data==null)
                {
                    var ob = new HrSettingDto
                    {
                        Id = 0,
                        AppAbsenceDisciplinary = 2,
                        ApplyAttDelay = 2,
                        ApplyAttDisciplinary = 2,
                        BadalatAllowance = 1,
                        HousingAllowance = 1,
                        MobileDeduction = 3,
                        GosiDeduction = 1,
                        BonusesAllowance = 1,
                        FoodAllowance = 1,
                        HousingDeduction = 10,
                        LeaveBenefitsAllowance = 16,
                        LeaveDeduction = 13,
                        MororDeduction = 2,
                        MobileAllowance = 5,
                        MonthStartDay   =null,
                        MonthEndDay =null,
                        OtherDeduction = 4,
                        PenaltiesDeduction =4 ,
                        OverTime = null,
                        PrevMonthAllowance =7 ,
                        TicketAllowance = 17,
                        TransportAllowance = 2,
                        UpdetDepLocExl = 1,
                        VacationDueAllowance= 15,
                        FacilityId = session.FacilityId,
                    };  
                    return Ok(await Result<HrSettingDto>.SuccessAsync( ob));
                }
                return Ok(getItem);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"{ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(HrSettingDto obj)
        {
            setErrors();
            var chk = await permission.HasPermission(663, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (!ModelState.IsValid)
            {
                return Ok(obj);
            }
            try
            {
                var getFacilitySettings =await hrServiceManager.HrSettingService.GetOne(s=>s.FacilityId==obj.FacilityId);
                if (getFacilitySettings.Data == null)
                {
                    var addRes = await hrServiceManager.HrSettingService.Add(obj);
                    if (addRes.Succeeded)
                    {
                        return Ok(await Result<HrSettingDto>.SuccessAsync(addRes.Data, ""));
                    }

                    else
                    {
                        return Ok(await Result.FailAsync($"{addRes.Status.message}"));
                    }
                }
                var EditRes = await hrServiceManager.HrSettingService.Update(obj);
                if (EditRes.Succeeded)
                {
                    return Ok(await Result<HrSettingDto>.SuccessAsync(EditRes.Data, ""));
                }

                else
                {
                    return Ok(await Result.FailAsync($"{EditRes.Status.message}"));
                }
            }

            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"{ex.Message}"));
            }
        }

        [HttpGet("DDlallowance")]
        public async Task<IActionResult> DDlallowance()
        {
            try
            {
                var list = await mainServiceManager.SysLookupDataService.GetDataByCategory(20);

                if (list.Succeeded && list.Data != null)
                {
                    return Ok(await Result<object>.SuccessAsync(list, ""));

                }
                return Ok(list);

            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
        [HttpGet("DDlDeduction")]
        public async Task<IActionResult> DDlDeduction()
        {
            try
            {
                var list = await mainServiceManager.SysLookupDataService.GetDataByCategory(21);

                if (list.Succeeded && list.Data != null)
                {
                    return Ok(await Result<object>.SuccessAsync(list, ""));

                }
                return Ok(list);

            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

    }

}