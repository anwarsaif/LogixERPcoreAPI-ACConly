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

    //  اعتماد مسير الرواتب من قبل مدير الموارد البشرية
    public class HRPayrollHrController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IMainServiceManager mainServiceManager;

        public HRPayrollHrController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IMainServiceManager mainServiceManager)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.mainServiceManager = mainServiceManager;
        }
        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrPayrollFilterDto filter)
        {
            var chk = await permission.HasPermission(296, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (filter.FinancelYear == 0 || filter.FinancelYear == null)
                {
                    return Ok(await Result<object>.FailAsync(" يجب اختيار السنة المالية"));
                }
                var BranchesList = session.Branches.Split(',');
                var items = await hrServiceManager.HrPayrollService.GetAllVW(e => e.IsDeleted == false &&
                (e.State == 1 || e.State == 6) &&
               (e.BranchId == 0 | BranchesList.Contains(e.BranchId.ToString())) &&
               (filter.FinancelYear == null || filter.FinancelYear == 0 || filter.FinancelYear == e.FinancelYear) &&
               (filter.PayrollTypeId == null || filter.PayrollTypeId == 0 || filter.PayrollTypeId == e.PayrollTypeId) &&
               (filter.MsCode == null || filter.MsCode == 0 || filter.MsCode == e.MsCode) &&
               (filter.ApplicationCode == null || filter.ApplicationCode == 0 || filter.ApplicationCode == e.ApplicationCode) &&
               (string.IsNullOrEmpty(filter.MsMonth) || e.MsMonth == filter.MsMonth)
                );
                if (items.Succeeded)
                {
                    if (items.Data.Count() > 0)
                    {
                        var res = items.Data.AsQueryable().ToList();
                        if (session.FacilityId != 1)
                        {
                            res = res.Where(x => x.FacilityId == session.FacilityId).ToList();
                        }
                        return Ok(await Result<List<HrPayrollVw>>.SuccessAsync(res, ""));
                    }
                    return Ok(await Result<List<HrPayrollVw>>.SuccessAsync(items.Data.ToList(), localization.GetResource1("NosearchResult")));
                }
                return Ok(await Result<HrPayrollVw>.FailAsync(items.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrPayrollVw>.FailAsync(ex.Message));
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long Id)
        {
            try
            {
                var chk = await permission.HasPermission(296, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id <= 0)
                {
                    return Ok(await Result<object>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var item = await hrServiceManager.HrPayrollService.GetOne(x => x.MsId == Id);
                var files = await mainServiceManager.SysFileService.GetAll(x => x.IsDeleted == false && x.PrimaryKey == Id && x.TableId == 37);
                return Ok(await Result<object>.SuccessAsync(new { data = item.Data, fileDtos = files }, ""));

            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"====== Exp in HR Manual Payroll Controller getById, MESSAGE: {ex.Message}"));
            }
        }
        [HttpGet("GetPayrollNotes")]
        public async Task<IActionResult> GetPayrollNotes(long Id)
        {
            try
            {
                var chk = await permission.HasPermission(296, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id <= 0)
                {
                    return Ok(await Result<HrHolidayVw>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var item = await hrServiceManager.HrPayrollNoteService.GetAllVW(x => x.MsId == Id);
                return Ok(item);

            }
            catch (Exception ex)
            {
                return Ok(await Result<HrHolidayVw>.FailAsync($"====== Exp in HR Manual Payroll Controller getById, MESSAGE: {ex.Message}"));
            }
        }
        [HttpPost("Approve")]
        public async Task<IActionResult> Approve(ApproveRejectPayrollDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(296, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (obj.MsId <= 0)
                {
                    return Ok(await Result<object>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var item = await hrServiceManager.HrPayrollService.ChangeStatusPayroll(obj, 2);
                return Ok(item);

            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"====== Exp in HR Payroll GM Controller AApprove, MESSAGE: {ex.Message}"));
            }
        }
        [HttpPost("Reject")]
        public async Task<IActionResult> Reject(ApproveRejectPayrollDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(296, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (obj.MsId <= 0)
                {
                    return Ok(await Result<object>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var item = await hrServiceManager.HrPayrollService.ChangeStatusPayroll(obj, 5);
                return Ok(item);

            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"====== Exp in HR Payroll GM Controller   Reject, MESSAGE: {ex.Message}"));
            }
        }

    }

}