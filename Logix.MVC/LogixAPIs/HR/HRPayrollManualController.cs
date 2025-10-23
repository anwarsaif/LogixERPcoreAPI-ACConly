using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.DTOs.Main;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using Result = Logix.Application.Wrapper.Result;

namespace Logix.MVC.LogixAPIs.HR
{
    //  مسير الرواتب يدوي
    public class HRPayrollManualController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IMainServiceManager mainServiceManager;


        public HRPayrollManualController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IMainServiceManager mainServiceManager)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.mainServiceManager = mainServiceManager;
        }


        #region IndexPage


        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrPayrollFilterDto filter)
        {
            var chk = await permission.HasPermission(168, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                filter.FinancelYear ??= 0;
                filter.PayrollTypeId ??= 0;
                if (filter.FinancelYear == 0)
                    return Ok(await Result<HrPayrollFilterDto>.FailAsync(" يجب اختيار السنة المالية"));

                var BranchesList = session.Branches.Split(',');

                List<HrPayrollFilterDto> resultList = new List<HrPayrollFilterDto>();
                var items = await hrServiceManager.HrPayrollService.GetAllVW(e => e.IsDeleted == false &&

                (e.BranchId == 0 || BranchesList.Contains(e.BranchId.ToString())) &&
                (filter.FinancelYear == 0 || filter.FinancelYear == e.FinancelYear) &&
                (filter.PayrollTypeId == 0 || filter.PayrollTypeId == e.PayrollTypeId) &&
                (string.IsNullOrEmpty(filter.MsMonth) || filter.MsMonth == e.MsMonth));
                if (!items.Succeeded)
                    return Ok(await Result<HrPayrollFilterDto>.FailAsync(items.Status.message));

                if (!(items.Data.Count() > 0))
                    return Ok(await Result<List<HrPayrollFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));

                var res = items.Data.AsQueryable();
                if (session.FacilityId != 1)
                {
                    res = res.Where(e => e.FacilityId == session.FacilityId);
                }

                if (!(res.Count() > 0))
                    return Ok(await Result<List<HrPayrollFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));

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
                        MsMonth = item.MsMonth
                    };
                    resultList.Add(newRecord);
                }
                if (resultList.Any())
                    return Ok(await Result<List<HrPayrollFilterDto>>.SuccessAsync(resultList, ""));
                return Ok(await Result<List<HrPayrollFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));

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
                var chk = await permission.HasPermission(168, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await hrServiceManager.HrPayrollService.Remove(Id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete HR Payroll  Manuall Controller, MESSAGE: {ex.Message}"));
            }
        }


        #endregion


        #region AddPage Business


        [HttpPost("SearchInAdd")]
        public async Task<IActionResult> SearchInAdd(HRPayrollCreate2SpFilterDto filter)
        {
            var chk = await permission.HasPermission(168, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                List<HrPayrollFilter2ResultDto> result = new List<HrPayrollFilter2ResultDto>();

                filter.NationalityID ??= 0;
                filter.Location ??= 0;
                filter.FacilityID ??= 0;
                filter.DeptID ??= 0;
                filter.BRANCHID ??= 0;
                filter.NationalityID ??= 0;
                if (string.IsNullOrEmpty(filter.EmpCode))
                    filter.EmpCode = null;
                if (string.IsNullOrEmpty(filter.EmpName))
                    filter.EmpName = null;


                if (string.IsNullOrEmpty(filter.FinancelYear))
                {
                    return Ok(await Result<object>.FailAsync(" يجب اختيار السنة المالية"));
                }
                if (string.IsNullOrEmpty(filter.MSMonth))
                {
                    return Ok(await Result<object>.FailAsync(" يجب اختيار الشهر"));

                }
                var items = await hrServiceManager.HrPayrollService.getHR_Payroll_Create2_Sp(filter);
                if (!items.Succeeded)
                    return Ok(await Result<object>.FailAsync(items.Status.message));
                var res = items.Data.AsQueryable();

                if (!res.Any())
                    return Ok(await Result<List<HrPayrollFilter2ResultDto>>.SuccessAsync(result, localization.GetResource1("NosearchResult")));


                foreach (var item in res)
                {
                    var newRecord = new HrPayrollFilter2ResultDto
                    {
                        BasicSalary=item.BasicSalary,
                        EmpCode = item.Emp_Code,
                        Emp_ID = item.Emp_Code,
                        Emp_Name = item.Emp_Name ,
                        Salary = item.Salary,
                        Allowance = item.Allowance,
                        H_OverTime = item.H_OverTime,
                        OverTime = item.OverTime,
                        Commission = item.Commission,
                        Mandate = item.Mandate,
                        Total = item.Salary + item.Allowance + item.OverTime + item.Commission + item.Mandate,
                        Attendance = item.Attendance,
                        Absence = item.Absence,
                        Delay = item.Delay,
                        Loan = item.Loan,
                        Penalties = item.Penalties,
                        Deduction = item.Deduction,
                        TotalDeductions = item.Absence + item.Delay + item.Loan + item.Deduction + item.Penalties,
                        Net = (item.Salary + item.Allowance + item.OverTime + item.Commission + item.Mandate) - (item.Absence + item.Delay + item.Loan + item.Deduction + item.Penalties),
                        Bank_ID = item.Bank_ID,
                        Bank_Name = item.Bank_Name,
                        Account_No = item.Account_No,
                        Cnt_Absence = item.Cnt_Absence,
                        ID= item.ID,
                    };
                    result.Add(newRecord);
                }

                return Ok(await Result<List<HrPayrollFilter2ResultDto>>.SuccessAsync(result, ""));




            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync(ex.Message));
            }
        }

        [HttpPost("Add")]
        public async Task<ActionResult> Add(HrPayrollAddDto obj)
        {
            try
            {
                obj.FacilityId ??= 0;
                var CurrentDate = DateTime.Now.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);
                var chk = await permission.HasPermission(168, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (obj.SpDtos.Count <= 0)
                    return Ok(await Result<object>.FailAsync("يجب توافر بيانات المسير "));

                if (obj.FinancelYear <= 0)
                {
                    return Ok(await Result<object>.FailAsync(" يجب اختيار السنة المالية"));
                }
                if (string.IsNullOrEmpty(obj.MsMonth))
                {
                    return Ok(await Result<object>.FailAsync(" يجب اختيار الشهر"));
                }
                if (obj.FacilityId <= 0) obj.FacilityId = (int?)session.FacilityId;
                obj.PayrollTypeId = 1;
                obj.State = 1;
                obj.MsDate = CurrentDate;
                obj.MsTitle = localization.GetHrResource("Title");
                if (obj.FacilityId <= 0) obj.FacilityId = Convert.ToInt32(session.FacilityId);
                var add = await hrServiceManager.HrPayrollService.AddNewPayroll(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"====== Exp in Add HR   Payroll Controller  Add Method, MESSAGE: {ex.Message}"));
            }
        }
        #endregion




        #region EditPage


        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long Id)
        {
            try
            {
                var result = new HrPayrollEditDto();
                result.fileDtos = new List<SaveFileDto>();

                var chk = await permission.HasPermission(168, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id <= 0)
                {
                    return Ok(await Result<object>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var item = await hrServiceManager.HrPayrollService.GetOne(x => x.MsId == Id);
                if (item.Succeeded == false || item.Data == null) return Ok(item);
                result.MsId = item.Data.MsId;
                result.MsTitle = item.Data.MsTitle;
                result.DueDate = item.Data.DueDate;
                result.PaymentDate = item.Data.PaymentDate;
                result.FacilityId = item.Data.FacilityId;
                result.FinancelYear = item.Data.FinancelYear;
                result.MsMonth = item.Data.MsMonth;
                var GetSysFiles = await mainServiceManager.SysFileService.GetAll(x => x.IsDeleted == false && x.PrimaryKey == Id && x.TableId == 37);
                if (GetSysFiles.Data != null)
                {
                    foreach (var file in GetSysFiles.Data)
                    {
                        var singleFile = new SaveFileDto
                        {
                            Id = file.Id,
                            FileName = file.FileName ?? "",
                            FileURL = file.FileUrl,
                            IsDeleted = file.IsDeleted,
                            FileDate = file.FileDate


                        };
                        result.fileDtos.Add(singleFile);
                    }
                }

                return Ok(await Result<object>.SuccessAsync(result, "", 200));

            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"====== Exp in HR Manual Payroll Controller getById, MESSAGE: {ex.Message}"));
            }
        }
        [HttpPost("SearchInEdit")]
        public async Task<IActionResult> SearchInEdit(HrPayrollFilter2Dto filter)
        {
            List<HrPayrollFilter2ResultDto> result = new List<HrPayrollFilter2ResultDto>();
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
                        {
                            foreach (var item in res)
                            {
                                var newRecord = new HrPayrollFilter2ResultDto
                                {
                                    EmpCode = item.EmpCode,
                                     Emp_ID= item.EmpCode,
                                    Emp_Name = (session.Language == 1) ? item.EmpName : item.EmpName2,
                                    BraName = (session.Language == 1) ? item.BraName : item.BraName2,
                                    Dep_Name = (session.Language == 1) ? item.DepName : item.DepName2,
                                    Location_Name = (session.Language == 1) ? item.LocationName : item.LocationName2,
                                    Salary = item.Salary,
                                    Allowance = item.Allowance,
                                    H_OverTime = item.HOverTime,
                                    OverTime = item.OverTime,
                                    Commission = item.Commission,
                                    Mandate = item.Mandate,
                                    Total = item.Salary + item.Allowance + item.OverTime + item.Commission + item.Mandate,
                                    Attendance = item.Attendance,
                                    Absence = item.Absence,
                                    Delay = item.Delay,
                                    Loan = item.Loan,
                                    Penalties = item.Penalties,
                                    Deduction = item.Deduction,
                                    TotalDeductions = item.Absence + item.Delay + item.Loan + item.Deduction + item.Penalties,
                                    Net = (item.Salary + item.Allowance + item.OverTime + item.Commission + item.Mandate) - (item.Absence + item.Delay + item.Loan + item.Deduction + item.Penalties),
                                    CostCenterCode = item.CostCenterCode,
                                    Bank_ID=item.BankId,
                                    Bank_Name=item.BankName,

                                };
                                result.Add(newRecord);
                            }

                            return Ok(await Result<List<HrPayrollFilter2ResultDto>>.SuccessAsync(result, ""));

                        }
                        return Ok(await Result<List<HrPayrollFilter2ResultDto>>.SuccessAsync(result, localization.GetResource1("NosearchResult")));
                    }
                    return Ok(await Result<List<HrPayrollFilter2ResultDto>>.SuccessAsync(result, localization.GetResource1("NosearchResult")));
                }
                return Ok(await Result<HrPayrollFilter2ResultDto>.FailAsync(items.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrPayrollFilter2ResultDto>.FailAsync(ex.Message));
            }
        }

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(HrPayrollEditDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(168, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result<object>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));
                if (obj.MsId <= 0)
                    return Ok(await Result<object>.FailAsync("يجب تحديد رقم المسير"));
                if (string.IsNullOrEmpty(obj.MsTitle))
                    return Ok(await Result<object>.FailAsync("يجب تحديد عنوان المسير"));
                if (string.IsNullOrEmpty(obj.DueDate))
                    return Ok(await Result<object>.FailAsync("يجب ادخال  تاريخ الاستحقاق"));
                if (string.IsNullOrEmpty(obj.PaymentDate))
                    return Ok(await Result<object>.FailAsync("يجب ادخال تاريخ الدفع"));



                var update = await hrServiceManager.HrPayrollService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"====== Exp in Edit HR PayrollManuall   Controller, MESSAGE: {ex.Message}"));
            }
        }

        /// <summary>
        /// هذه الدالة تستخدم لجلب الملاحظات على المسير وذلك عند الانتقال الى واجهة تعديل مسير
        /// </summary>
        /// <param name="Id">رقم المسير</param>
        /// <returns></returns>
        [HttpGet("GetPayrollNotes")]
        public async Task<IActionResult> GetPayrollNotes(long Id)
        {
            try
            {
                var chk = await permission.HasPermission(168, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id <= 0)
                {
                    return Ok(await Result<object>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var item = await hrServiceManager.HrPayrollNoteService.GetAllVW(x => x.MsId == Id);
                return Ok(item);

            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"====== Exp in HR PayrollManuall Controller getById, MESSAGE: {ex.Message}"));
            }
        }
        #endregion
    }
}