using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Services;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Result = Logix.Application.Wrapper.Result;

namespace Logix.MVC.LogixAPIs.HR
{

    //  اعتماد مسير الرواتب من قبل الإدارة المالية
    public class HRPayrollFinController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IMainServiceManager mainServiceManager;

        public HRPayrollFinController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IMainServiceManager mainServiceManager)
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
            var chk = await permission.HasPermission(297, PermissionType.Show);
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
                (e.State == 2 || e.State == 7) &&
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
                var chk = await permission.HasPermission(297, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id <= 0)
                {
                    return Ok(await Result<HrHolidayVw>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var item = await hrServiceManager.HrPayrollService.GetOne(x => x.MsId == Id);
                var files = await mainServiceManager.SysFileService.GetAll(x => x.IsDeleted == false && x.PrimaryKey == Id && x.TableId == 37);
                return Ok(await Result<object>.SuccessAsync(new { data = item.Data, fileDtos = files }, ""));
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrHolidayVw>.FailAsync($"====== Exp in HR Manual Payroll Controller getById, MESSAGE: {ex.Message}"));
            }
        }
        [HttpGet("GetPayrollNotes")]
        public async Task<IActionResult> GetPayrollNotes(long Id)
        {
            try
            {
                var chk = await permission.HasPermission(297, PermissionType.Edit);
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

        [HttpPost("SearchInEdit")]
        public async Task<IActionResult> SearchInEdit(HrPayrollFilter2Dto filter)
        {
            var chk = await permission.HasPermission(297, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var BranchesList = session.Branches.Split(',');
                if (filter.MsId <= 0)
                {
                    return Ok(await Result<object>.FailAsync("رقم المسير مطلوب"));

                }
                //  هنا يجب ارسال رقم المسير والسنة المالية والشهر للحصول على بيانات صحيحة
                var items = await hrServiceManager.HrPayrollDService.GetAllVW(e =>
                e.IsDeleted == false &&
                (filter.MsId == 0 || filter.MsId == e.MsId) &&
                (filter.FinancelYear == 0 || filter.FinancelYear == e.FinancelYear) &&
                (filter.FacilityId == 0 || filter.FacilityId == e.FacilityId) &&
                (filter.DeptId == 0 || filter.DeptId == e.DeptId) &&
                (filter.BranchId == 0 || filter.BranchId == e.BranchId) &&
                (filter.Location == 0 || filter.Location == e.Location) &&
                (filter.SponsorsId == 0 || filter.SponsorsId == e.SponsorsId) &&
                (filter.NationalityId == 0 || filter.NationalityId == e.NationalityId) &&
                (filter.WagesProtection == 0 || filter.WagesProtection == e.WagesProtection) &&
                (filter.SalaryGroupId == 0 || filter.SalaryGroupId == e.SalaryGroupId) &&
                (string.IsNullOrEmpty(filter.EmpCode) || filter.EmpCode == e.EmpCode) &&
                (string.IsNullOrEmpty(filter.EmpName) || filter.EmpName == e.EmpName) &&
                (filter.PayrollTypeId == 0 || filter.PayrollTypeId == e.PayrollTypeId) &&
                (string.IsNullOrEmpty(filter.MsMonth) || filter.MsMonth == e.MsMonth));
                if (items.Succeeded)
                {
                    if (items.Data.Count() > 0)
                    {
                        var res = items.Data.ToList();
                        if (filter.BranchId > 0)
                        {
                            res = res.Where(x => x.BranchId == filter.BranchId).ToList();
                        }

                        if (res.Any())
                            return Ok(await Result<List<HrPayrollDVw>>.SuccessAsync(res, ""));
                        return Ok(await Result<List<HrPayrollDVw>>.SuccessAsync(res, localization.GetResource1("NosearchResult")));
                    }
                    return Ok(await Result<List<HrPayrollDVw>>.SuccessAsync(items.Data.ToList(), localization.GetResource1("NosearchResult")));
                }
                return Ok(await Result<HrPayrollDVw>.FailAsync(items.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrPayrollDVw>.FailAsync(ex.Message));
            }
        }
        [HttpPost("AApprove")]
        public async Task<IActionResult> AApprove(ApproveRejectPayrollDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(297, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (obj.MsId <= 0)
                {
                    return Ok(await Result<object>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var item = await hrServiceManager.HrPayrollService.ChangeStatusPayroll(obj, 3);
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
                var chk = await permission.HasPermission(297, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (obj.MsId <= 0)
                {
                    return Ok(await Result<object>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var item = await hrServiceManager.HrPayrollService.ChangeStatusPayroll(obj, 6);
                return Ok(item);

            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"====== Exp in HR Payroll GM Controller   Reject, MESSAGE: {ex.Message}"));
            }
        }

    }

}