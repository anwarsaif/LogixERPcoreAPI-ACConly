using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using Result = Logix.Application.Wrapper.Result;

namespace Logix.MVC.LogixAPIs.HR
{
    //  طباعة ملف تبادل الراجحي
    public class HRPayrollReciprocalController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly ISysConfigurationHelper sysConfigurationHelper;


        public HRPayrollReciprocalController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, ISysConfigurationHelper sysConfigurationHelper)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.sysConfigurationHelper = sysConfigurationHelper;
        }
        #region IndexPage


        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrPayrollFilterDto filter)
        {
            var chk = await permission.HasPermission(872, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (filter.FinancelYear == 0 || filter.FinancelYear == null)
                {
                    return Ok(await Result<HrPayrollFilterDto>.FailAsync(" يجب اختيار السنة المالية"));

                }
                List<HrPayrollFilterDto> resultList = new List<HrPayrollFilterDto>();
                var items = await hrServiceManager.HrPayrollService.GetAllVW(e => e.IsDeleted == false &&
                (filter.FinancelYear == null || filter.FinancelYear == 0 || filter.FinancelYear == e.FinancelYear) &&
                (filter.PayrollTypeId == null || filter.PayrollTypeId == 0 || filter.PayrollTypeId == e.PayrollTypeId) &&
                (string.IsNullOrEmpty(filter.MsMonth) || Convert.ToInt32(filter.MsMonth) == Convert.ToInt32(e.MsMonth)));
                if (items.Succeeded)
                {
                    if (items.Data.Count() > 0)
                    {
                        var res = items.Data.AsQueryable();
                        if (session.FacilityId != 1)
                        {
                            res = res.Where(x => x.FacilityId == session.FacilityId).AsQueryable();
                        }
                        foreach (var item in res)
                        {
                            var newRecord = new HrPayrollFilterDto
                            {
                                MsId = item.MsId,
                                MsCode = item.MsCode,
                                MsDate = item.MsDate,
                                FinancelYear = item.FinancelYear,
                                StatusName = item.StatusName,
                                TypeName = item.TypeName,
                                MsTitle = item.MsTitle,
                                MsMonth = item.MsMonth,
                                ApplicationCode = item.ApplicationCode,
                                Status = item.State,
                            };
                            resultList.Add(newRecord);
                        }
                        if (resultList.Any())
                            return Ok(await Result<List<HrPayrollFilterDto>>.SuccessAsync(resultList, ""));
                        return Ok(await Result<List<HrPayrollFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));
                    }
                    return Ok(await Result<List<HrPayrollFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));
                }
                return Ok(await Result<HrPayrollFilterDto>.FailAsync(items.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrPayrollFilterDto>.FailAsync(ex.Message));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long Id)
        {
            try
            {
                var chk = await permission.HasPermission(872, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await hrServiceManager.HrPayrollService.Remove(Id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete HR HRPayrollReciprocalController , MESSAGE: {ex.Message}"));
            }
        }


        #endregion


        #region AddPage Business
        [HttpPost("SearchInAdd")]
        public async Task<IActionResult> SearchInAdd(HRPayrollCreateSpFilterDto filter)
        {
            var chk = await permission.HasPermission(872, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                List<HRPayrollCreate2SpDto> result = new List<HRPayrollCreate2SpDto>();
                if (string.IsNullOrEmpty(filter.FinancelYear))
                {
                    return Ok(await Result<HrPayrollFilterDto>.FailAsync(" يجب اختيار السنة المالية"));
                }
                if (string.IsNullOrEmpty(filter.MSMonth))
                {
                    return Ok(await Result<HrPayrollFilterDto>.FailAsync(" يجب اختيار الشهر"));

                }
                DateTime firstDayOfNextMonth = new DateTime(Convert.ToInt32(filter.FinancelYear), Convert.ToInt32(filter.MSMonth), 1).AddMonths(1);
                DateTime lastDayOfMonth = firstDayOfNextMonth.AddDays(-1);
                var monthEnd = lastDayOfMonth.Day.ToString();
                if (monthEnd.Length == 1) monthEnd = "0" + monthEnd;
                filter.Branches = session.Branches;
                var propertyId = await sysConfigurationHelper.GetValue(178, session.FacilityId);
                if (propertyId == "1")
                {
                    if (filter.BRANCHID == null || filter.BRANCHID <= 0)
                    {
                        return Ok(await Result<HRPayrollCreate2SpDto>.FailAsync("اختر الفرع"));

                    }
                }

                var Check_JoinWork_Date = "0";
                Check_JoinWork_Date = await sysConfigurationHelper.GetValue(137, Convert.ToInt64(filter.FacilityID));

                if (filter.SalaryGroupID <= 0)
                {
                    filter.SalaryGroupID = null;
                }
                //if (filter.ContractTypeID <= 0)
                //{
                //    filter.ContractTypeID = null;
                //}
                if (filter.PaymentTypeID <= 0)
                {
                    filter.PaymentTypeID = null;

                }
                if (filter.SponsorsID <= 0)
                {
                    filter.SponsorsID = null;

                }
                if (filter.WagesProtection <= 0)
                {
                    filter.WagesProtection = null;

                }





                var items = await hrServiceManager.HrPayrollService.getHR_Payroll_Create_Sp(filter);
                if (items.Succeeded)
                {
                    if (items.Data.Count() > 0)
                    {
                        //  التشييك هل يوجد علاوات لم تطبق خلال هذا الشهر
                        string Emp_Have_Increements = "";
                        string Emp_JoinWork = "";
                        var getEmpIncvrementsAll = await hrServiceManager.HrIncrementService.GetAll(x => x.IsDeleted == false && x.ApplyType == 2);

                        foreach (var item in items.Data)
                        {
                            var getEmpIncvrements = getEmpIncvrementsAll.Data.Where(x => x.EmpId == item.ID).ToList();
                            if (getEmpIncvrementsAll.Succeeded)
                            {
                                if (getEmpIncvrements.Count() <= 0)
                                {
                                    result.Add(item);
                                    continue;
                                }
                                var filteredIncrements = getEmpIncvrements.Where(x => DateHelper.StringToDate(x.StartDate) >= DateHelper.StringToDate($"{filter.FinancelYear}/{filter.MSMonth}/01") && DateHelper.StringToDate(x.StartDate) <= DateHelper.StringToDate($"{filter.FinancelYear}/{filter.MSMonth}/{monthEnd}")).ToList();

                                if (filteredIncrements.Any())
                                {
                                    Emp_Have_Increements += item.Emp_ID + ",";
                                }
                                result.Add(item);
                            }
                            else
                            {
                                return Ok(await Result<HRPayrollCreate2SpDto>.FailAsync($" خطأ في جلب الزيادات للموظف رقم  {item.Emp_ID}"));

                            }

                        }


                        if (Emp_Have_Increements.Length > 0)
                        {
                            Emp_Have_Increements = $"{localization.GetHrResource("increasesNote")} \n {Emp_Have_Increements.TrimEnd(',')}";
                        }
                        if (Check_JoinWork_Date == "1")
                        {
                            var checkEmpJoinWork = await hrServiceManager.HrPayrollService.CheckJoinWorkForPayroll(filter);
                            if (checkEmpJoinWork.Succeeded)
                            {
                                if (!string.IsNullOrEmpty(checkEmpJoinWork.Data))
                                {
                                    Emp_JoinWork = $"{localization.GetHrResource("NoVacationEffectiveDateNotice")} \n {checkEmpJoinWork.Data.TrimEnd(',')}";
                                }
                            }
                        }


                        return Ok(await Result<object>.SuccessAsync(new { data = result, Increements = Emp_Have_Increements, JoinWork = Emp_JoinWork }, ""));
                    }
                    return Ok(await Result<List<HRPayrollCreate2SpDto>>.SuccessAsync(items.Data.ToList(), localization.GetResource1("NosearchResult")));
                }
                return Ok(await Result<HRPayrollCreate2SpDto>.FailAsync(items.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrPayrollDVw>.FailAsync(ex.Message));
            }
        }

        [HttpPost("Add")]
        public async Task<ActionResult> Add(HrPayrollAddDto obj)
        {
            try
            {
                var CurrentDate = DateTime.Now.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);
                var chk = await permission.HasPermission(872, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result<string>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));
                if (obj.SpDtos.Count <= 0)
                    return Ok(await Result<string>.FailAsync("يجب توافر بيانات المسير "));
                if (obj.FinancelYear <= 0)
                    return Ok(await Result<string>.FailAsync("يجب تحديد السنة المالية "));


                if (obj.FacilityId <= 0 || obj.FacilityId == null) obj.FacilityId = (int?)session.FacilityId;
                obj.PayrollTypeId = 1;
                obj.State = 1;
                obj.MsDate = CurrentDate;
                var add = await hrServiceManager.HrPayrollService.AddNewAutomaticPayroll1(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"====== Exp in Add HR   Payroll Controller  Add Method, MESSAGE: {ex.Message}"));
            }
        }



        #endregion




        #region EditPage


        [HttpGet("PayrollReciprocalView")]
        public async Task<IActionResult> PayrollReciprocalView(long Id)
        {
            try
            {
                var chk = await permission.HasPermission(872, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id <= 0)
                {
                    return Ok(await Result<HrHolidayVw>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var item = await hrServiceManager.HrPayrollService.GetOne(x => x.MsId == Id);
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
            var chk = await permission.HasPermission(168, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                filter.FinancelYear ??= 0;
                filter.FacilityId ??= 0;
                filter.DeptId ??= 0;
                filter.BranchId ??= 0;
                filter.Location ??= 0;
                filter.SponsorsId ??= 0;
                filter.WagesProtection ??= 0;
                filter.NationalityId ??= 0;
                filter.SalaryGroupId ??= 0;
                filter.PayrollTypeId ??= 0;
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
        [HttpGet("GetPayrollNotes")]
        public async Task<IActionResult> GetPayrollNotes(long Id)
        {
            try
            {
                var chk = await permission.HasPermission(872, PermissionType.Edit);
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
        #endregion
    }

}