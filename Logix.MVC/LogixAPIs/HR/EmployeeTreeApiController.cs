using Logix.Application.Common;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.HR.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using Result = Logix.Application.Wrapper.Result;

namespace Logix.MVC.LogixAPIs.HR
{
    //   شجرة الموظفين
    public class EmployeeTreeApiController : BaseHrApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IAccServiceManager accServiceManager;
        private readonly IPermissionHelper permission;
        private readonly IDDListHelper listHelper;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IHrServiceManager hrServiceManager;

        public EmployeeTreeApiController(IMainServiceManager mainServiceManager,
            IPermissionHelper permission,
            IDDListHelper listHelper,
            ILocalizationService localization,
            ICurrentData session,
            IHrServiceManager hrServiceManager,
            IAccServiceManager accServiceManager
            )
        {
            this.mainServiceManager = mainServiceManager;
            this.permission = permission;
            this.listHelper = listHelper;
            this.session = session;
            this.localization = localization;
            this.hrServiceManager = hrServiceManager;
            this.accServiceManager = accServiceManager;
        }

        //[HttpGet("GetlistDepartments")]
        //public async Task<IActionResult> GetDepartmentsWithChildren()
        //{
        //    var chk = await permission.HasPermission(1981, PermissionType.Show);
        //    if (!chk)
        //    {
        //        return Ok(await Result.AccessDenied("AccessDenied"));
        //    }
        //    try
        //    {
        //        var depManagerId = session.EmpId;
        //        List<SysDepartmentVM> sysDepartmentVMs = new List<SysDepartmentVM>();
        //        var items = await mainServiceManager.SysDepartmentService.GetAllVW(d => d.TypeId == 1 && d.DepMangerId != null && d.DepMangerId != 0 && d.IsDeleted == false && (d.IsShare == true || d.FacilityId == session.FacilityId));
        //        foreach (var item in items.Data)
        //        {

        //            sysDepartmentVMs.Add(new SysDepartmentVM
        //            {
        //                DeptId = item.Id,
        //                DeptName = item.Name,
        //                DeptName2 = item.Name2,
        //                EmpName1 = item.EmpName,
        //                EmpName2 = item.EmpName2,
        //                ParentId = item.ParentId
        //            });
        //        }
        //        return Ok(sysDepartmentVMs);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(await Result<SysDepartmentVM>.FailAsync($"======= Exp in Search SysDepartmentVw, MESSAGE: {ex.Message}"));
        //    }
        //}

        [HttpGet("GetlistDepartments")]
        public async Task<IActionResult> GetDepartmentsWithChildren(CancellationToken cancellationToken)
        {
            // تحقق من الصلاحيات
            var hasPermission = await permission.HasPermission(1981, PermissionType.Show);
            if (!hasPermission)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var departments = await mainServiceManager.SysDepartmentService.GetAllVW(
                    d => d.TypeId == 1
                         && d.DepMangerId != null
                         && d.DepMangerId != 0
                         && d.IsDeleted == false
                         && (d.IsShare == true || d.FacilityId == session.FacilityId),
                    cancellationToken
                );

                var result = departments.Data.Select(d => new SysDepartmentVM
                {
                    DeptId = d.Id,
                    DeptName = d.Name,
                    DeptName2 = d.Name2,
                    EmpName1 = d.EmpName,
                    EmpName2 = d.EmpName2,
                    ParentId = d.ParentId
                }).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<SysDepartmentVM>.FailAsync($"======= Exp in Search SysDepartmentVw, MESSAGE: {ex.Message}"));
            }
        }
        [HttpGet("GetEmployeesForDepartment")]
        public async Task<IActionResult> GetEmployeesForDepartment(long DeptId)
        {
            var chk = await permission.HasPermission(1981, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var items = await hrServiceManager.HrEmployeeService.GetAllVW(c => c.IsDeleted == false && c.DeptId == DeptId && c.StatusId != 2 && c.FacilityId == session.FacilityId);
                if (items.Succeeded)
                {
                    var res = items.Data.ToList();
                    return Ok(await Result<List<HrEmployeeVw>>.SuccessAsync(res, ""));

                }
                return Ok(items);
            }
            catch (Exception ex)
            {

                return Ok(await Result<HrEmployeeVw>.FailAsync($"======= Exp in GetEmployeesForDepartment, MESSAGE: {ex.Message}"));

            }
        }
        [HttpGet("PrintEmpData")]
        public async Task<IActionResult> PrintEmpData(long Id = 0)
        {
            var result = new PrintEmpDataVM();
            var chk = await permission.HasPermission(1981, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (Id <= 0)
            {
                return Ok(await Result<PrintEmpDataVM>.FailAsync($"there is no Data found For This Id:  {Id}"));
            }
            try
            {
                string? parentCode;
                decimal TotalAllowance = 0;
                decimal TotalDeduction = 0;
                var getFromInvest = await mainServiceManager.InvestEmployeeService.GetOne(e => e.Id == Id);
                if (getFromInvest.Data == null) return Ok();
                parentCode = getFromInvest.Data.ParentId.ToString();
                var getTotalAllowance = await hrServiceManager.HrAllowanceDeductionService.GetAllVW(e => e.EmpId == Id && e.IsDeleted == false && e.TypeId == 1 && e.FixedOrTemporary == 1);
                if (getTotalAllowance.Succeeded)
                {
                    foreach (var item in getTotalAllowance.Data)
                    {
                        TotalAllowance += (item.Amount != null ? item.Amount.Value : 0);
                    }


                }
                var getTotalDeduction = await hrServiceManager.HrAllowanceDeductionService.GetAllVW(e => e.EmpId == Id && e.IsDeleted == false && e.TypeId == 2 && e.FixedOrTemporary == 1);
                if (getTotalDeduction.Succeeded)
                {
                    foreach (var item in getTotalDeduction.Data)
                    {
                        TotalDeduction += (item.Amount != null ? item.Amount.Value : 0);
                    }
                }
                var getEmployeeData = await hrServiceManager.HrEmployeeService.GetOneVW(e => e.Id == Id);

                result = new PrintEmpDataVM
                {
                    EmpPhoto = getEmployeeData.Data.EmpPhoto,
                    //personla info
                    EmpId = getEmployeeData.Data.EmpId,
                    EmpName = getEmployeeData.Data.EmpName,
                    EmpName2 = getEmployeeData.Data.EmpName2,
                    NationalityName = (session.Language == 1) ? getEmployeeData.Data.NationalityName : getEmployeeData.Data.NationalityName2,
                    MaritalStatusName = (session.Language == 1) ? getEmployeeData.Data.MaritalStatusName : getEmployeeData.Data.MaritalStatusName2,
                    GenderName = (session.Language == 1) ? getEmployeeData.Data.GenderName : getEmployeeData.Data.GenderName2,
                    VisaNo = getEmployeeData.Data.VisaNo,
                    BirthDate = getEmployeeData.Data.BirthDate,
                    BirthPlace = getEmployeeData.Data.BirthPlace,
                    IdIssuer = getEmployeeData.Data.IdIssuer,
                    PassportNo = getEmployeeData.Data.PassportNo,
                    PassIssuerDate = getEmployeeData.Data.PassIssuerDate,
                    PassExpireDate = getEmployeeData.Data.PassExpireDate,
                    EntryNo = getEmployeeData.Data.EntryNo,
                    EntryDate = getEmployeeData.Data.EntryDate,
                    EntryPort = getEmployeeData.Data.EntryPort,
                    OccupationId = getEmployeeData.Data.OccupationId,
                    CardExpirationDate = getEmployeeData.Data.CardExpirationDate,
                    ReligionName = (session.Language == 1) ? getEmployeeData.Data.ReligionName : getEmployeeData.Data.ReligionName2,

                    //contact info
                    OfficePhone = getEmployeeData.Data.OfficePhone,
                    OfficePhoneEx = getEmployeeData.Data.OfficePhoneEx,
                    SponserName = (session.Language == 1) ? getEmployeeData.Data.SponserName : getEmployeeData.Data.SponserName2,
                    Mobile = getEmployeeData.Data.Mobile,
                    Pobox = getEmployeeData.Data.Pobox,
                    PostalCode = getEmployeeData.Data.PostalCode,
                    HomePhone = getEmployeeData.Data.HomePhone,
                    Address = getEmployeeData.Data.Address,
                    AddressCountry = getEmployeeData.Data.AddressCountry,
                    PhoneCountry = getEmployeeData.Data.PhoneCountry,
                    Email = getEmployeeData.Data.Email,
                    // identity info
                    IdNo = getEmployeeData.Data.IdNo,
                    //salary info
                    Salary = getEmployeeData.Data.Salary,
                    TotalSalary = TotalAllowance + getEmployeeData.Data.Salary,
                    NetSalary = getEmployeeData.Data.Salary + TotalAllowance - TotalDeduction,
                    BankName = (session.Language == 1) ? getEmployeeData.Data.BankName : getEmployeeData.Data.BankName2,
                    AccountNo = getEmployeeData.Data.AccountNo,
                    Iban = getEmployeeData.Data.Iban,
                    DailyWorkingHours = getEmployeeData.Data.DailyWorkingHours,
                    VacationDaysYear = getEmployeeData.Data.VacationDaysYear,
                    PaymentTypeName = (session.Language == 1) ? getEmployeeData.Data.PaymentTypeName : getEmployeeData.Data.PaymentTypeName2,
                    WagesProtectionName = getEmployeeData.Data.WagesProtectionName,

                    // residence info
                    // job info
                    BraName = (session.Language == 1) ? getEmployeeData.Data.BraName : getEmployeeData.Data.BranchCode,
                    CatName = (session.Language == 1) ? getEmployeeData.Data.CatName : getEmployeeData.Data.CatName2,
                    StatusName = (session.Language == 1) ? getEmployeeData.Data.StatusName : getEmployeeData.Data.StatusName2,
                    QualificationName = (session.Language == 1) ? getEmployeeData.Data.QualificationName : getEmployeeData.Data.QualificationName2,
                    SpecializationName = (session.Language == 1) ? getEmployeeData.Data.SpecializationName : getEmployeeData.Data.SpecializationName2,
                    Doappointment = getEmployeeData.Data.Doappointment,
                    DepName = (session.Language == 1) ? getEmployeeData.Data.DepName : getEmployeeData.Data.DepName2,
                    LocationName = (session.Language == 1) ? getEmployeeData.Data.LocationName : getEmployeeData.Data.LocationName2,
                    ManagerName = (session.Language == 1) ? getEmployeeData.Data.ManagerName : getEmployeeData.Data.ManagerName2,
                    Manager2Name = (session.Language == 1) ? getEmployeeData.Data.Manager2Name : getEmployeeData.Data.Manager2Name2,
                    Manager3Name = (session.Language == 1) ? getEmployeeData.Data.Manager3Name : getEmployeeData.Data.Manager3Name2,
                    //contact info
                    //allowance info
                    SalryAllownce = getTotalAllowance.Data.ToList(),
                    SalryDeduction = getTotalDeduction.Data.ToList(),
                    //deduction info
                    //contract info

                    TrialStatusName = (session.Language == 1) ? getEmployeeData.Data.TrialStatusName : getEmployeeData.Data.TrialStatusName2,
                    ContractExpiryDate = getEmployeeData.Data.ContractExpiryDate,
                    TrialExpiryDate = getEmployeeData.Data.TrialExpiryDate,
                    ContarctDate = getEmployeeData.Data.ContarctDate,
                    //GOSI info
                    GosiNo = getEmployeeData.Data.GosiNo,
                    GosiDate = getEmployeeData.Data.GosiDate,
                    GosiBiscSalary = getEmployeeData.Data.GosiBiscSalary,
                    GosiHouseAllowance = getEmployeeData.Data.GosiHouseAllowance,
                    GosiSalary = getEmployeeData.Data.GosiSalary,
                    GoisSubscriptionExpiryDate = getEmployeeData.Data.GoisSubscriptionExpiryDate,
                    WorkNo = getEmployeeData.Data.WorkNo,
                    GosiRateFacility = getEmployeeData.Data.GosiRateFacility,
                    GosiTypeName = getEmployeeData.Data.GosiTypeName,

                };
                return Ok(result);
            }


            catch (Exception ex)
            {

                return Ok(await Result<PrintEmpDataVM>.FailAsync($"======= Exp in PrintEmpData, MESSAGE: {ex.Message}"));
            }
        }
        // بيانات السلف
        [HttpGet("GetEmpLoanData")]
        public async Task<IActionResult> GetEmpLoanData(long Id = 0)
        {

            var chk = await permission.HasPermission(198, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (Id <= 0)
            {
                return Ok(await Result<HrLoanVw>.FailAsync($"there is no Data found For This Id:  {Id}"));
            }
            try
            {
                var result = await hrServiceManager.HrLoanService.GetAllVW(v => v.IsDeleted == false && v.EmpId == Id.ToString());

                return Ok(result);
            }


            catch (Exception ex)
            {

                return Ok(await Result<HrLoanVw>.FailAsync($"======= Exp in Get Loan Data, MESSAGE: {ex.Message}"));
            }
        }

        // بيانات الاجازات
        [HttpGet("GetVacationsData")]
        public async Task<IActionResult> GetVacationsData(long Id = 0)
        {

            var chk = await permission.HasPermission(198, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (Id <= 0)
            {
                return Ok(await Result<HrVacationsVw>.FailAsync($"there is no Data found For This Id:  {Id}"));
            }
            try
            {
                var result = await hrServiceManager.HrVacationsService.GetAllVW(v => v.IsDeleted == false && v.EmpId == Id);
                return Ok(result);
            }


            catch (Exception ex)
            {

                return Ok(await Result<HrVacationsVw>.FailAsync($"======= Exp in Get Vacations Data, MESSAGE: {ex.Message}"));
            }
        }
        // بيانات التاخير
        [HttpGet("GetDelayData")]
        public async Task<IActionResult> GetDelayData(long Id = 0)
        {

            var chk = await permission.HasPermission(198, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (Id <= 0)
            {
                return Ok(await Result<HrVacationsVw>.FailAsync($"there is no Data found For This Id:  {Id}"));
            }
            try
            {
                var result = await hrServiceManager.HrDelayService.GetAll(d => d.IsDeleted == false && d.EmpId == Id && d.DelayDate != null && d.DelayTime != null);

                if (result.Succeeded)
                {
                    var res = result.Data.AsQueryable();
                    var final = res.GroupBy(d => new { Year = DateHelper.StringToDate(d.DelayDate).Year, Month = DateHelper.StringToDate(d.DelayDate).Month })
                           .Select(g => new
                           {
                               DelayDate = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("yyyy-MM", CultureInfo.InvariantCulture),
                               SumOfDelayTime = TimeSpan.FromSeconds(g.Sum(d => d.DelayTime.Value.TotalSeconds))
                           }).OrderBy(g => g.DelayDate).ToList();

                    return Ok(final);

                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrDelay>.FailAsync($"======= Exp in Get Delay Data, MESSAGE: {ex.Message}"));
            }


        }
        // بيانات الغياب
        [HttpGet("GetAbsenceData")]
        public async Task<IActionResult> GetAbsenceData(long Id = 0)
        {

            var chk = await permission.HasPermission(198, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (Id <= 0)
            {
                return Ok(await Result<HrAbsenceVw>.FailAsync($"there is no Data found For This Id:  {Id}"));
            }
            try
            {
                var result = await hrServiceManager.HrAbsenceService.GetAllVW(d => d.IsDeleted == false && d.AbsenceDate != null && d.EmpId == Id);

                if (result.Succeeded && result.Data.Any())
                {
                    var res = result.Data.AsQueryable();
                    var final = res.Where(d => DateHelper.StringToDate(d.AbsenceDate).Year == DateTime.Now.Year).Select(g => new
                    {
                        AbsenceDate = g.AbsenceDate,
                    }).OrderBy(g => g.AbsenceDate).ToList();

                    return Ok(await Result<object>.SuccessAsync(final));
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrAbsenceVw>.FailAsync($"======= Exp in Get Absence Data, MESSAGE: {ex.Message}"));
            }


        }

        //مسير الرواتب
        [HttpGet("GetPayrollData")]
        public async Task<IActionResult> GetPayrollData(long Id = 0)
        {

            var chk = await permission.HasPermission(198, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (Id <= 0)
            {
                return Ok(await Result<HrPayrollDVw>.FailAsync($"there is no Data found For This Id:  {Id}"));
            }
            try
            {
                var FinYear = await accServiceManager.AccFinancialYearService.GetOne(x => x.FinYear == session.FinYear);

                var result = await hrServiceManager.HrPayrollDService.GetAllVW(d => d.IsDeleted == false && d.EmpId == Id && d.FinancelYear.Equals(FinYear.Data.FinYearGregorian) && d.State == 4);
                var res = result.Data.AsQueryable();
                res = res.OrderBy(o => o.MsdId);
                return Ok(res);

            }
            catch (Exception ex)
            {
                return Ok(await Result<HrPayrollDVw>.FailAsync($"======= Exp in Get Hr Payroll Data, MESSAGE: {ex.Message}"));
            }


        }


        // الوصف الوظيفي & البيانات الأساسية
        [HttpGet("GetEmpBasicData")]
        public async Task<IActionResult> GetEmpBasicData(long Id = 0)
        {

            var chk = await permission.HasPermission(198, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (Id <= 0)
            {
                return Ok(await Result<HrEmployeeVw>.FailAsync($"there is no Data found For This Id:  {Id}"));
            }
            try
            {
                var result = await hrServiceManager.HrEmployeeService.GetAllVW(v => v.EmpId == Id.ToString());
                return Ok(result);
            }


            catch (Exception ex)
            {

                return Ok(await Result<HrEmployeeVw>.FailAsync($"======= Exp in Get Employee Basic Data, MESSAGE: {ex.Message}"));
            }
        }

        // أرشيف الموظف 
        [HttpGet("GetEmpArchiveFiles")]
        public async Task<IActionResult> GetEmpArchiveFiles(long Id = 0)
        {

            var chk = await permission.HasPermission(198, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (Id <= 0)
            {
                return Ok(await Result<HrArchivesFilesVw>.FailAsync($"there is no Data found For This Id:  {Id}"));
            }
            try
            {
                var result = await hrServiceManager.HrArchivesFilesService.GetAllVW(f => f.EmpId == Id && f.IsDeleted == false && f.ShowEmp == true);
                return Ok(result);
            }


            catch (Exception ex)
            {

                return Ok(await Result<HrArchivesFilesVw>.FailAsync($"======= Exp in Get Employee Archives FilesVw, MESSAGE: {ex.Message}"));
            }
        }
        //رخص الموظف 
        [HttpGet("GetEmpLicense")]
        public async Task<IActionResult> GetEmpLicense(long Id = 0)
        {

            var chk = await permission.HasPermission(198, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (Id <= 0)
            {
                return Ok(await Result<HrLicensesVw>.FailAsync($"there is no Data found For This Id:  {Id}"));
            }
            try
            {
                var result = await hrServiceManager.HrLicenseService.GetAllVW(f => f.EmpId == Id && f.IsDeleted == false);
                return Ok(result);
            }


            catch (Exception ex)
            {

                return Ok(await Result<HrLicensesVw>.FailAsync($"======= Exp in Get Employee Licenses  Data, MESSAGE: {ex.Message}"));
            }
        }

        //تنقلات الموظف 
        [HttpGet("GetEmpTransfers")]
        public async Task<IActionResult> GetEmpTransfers(long Id = 0)
        {

            var chk = await permission.HasPermission(198, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (Id <= 0)
            {
                return Ok(await Result<HrTransfersVw>.FailAsync($"there is no Data found For This Id:  {Id}"));
            }
            try
            {
                var result = await hrServiceManager.HrTransferService.GetAllVW(f => f.EmpId == Id && f.IsDeleted == false);
                return Ok(result);
            }


            catch (Exception ex)
            {

                return Ok(await Result<HrTransfersVw>.FailAsync($"======= Exp in Get EmployeeTransfers  Data, MESSAGE: {ex.Message}"));
            }
        }

        //عُهد الموظف 
        [HttpGet("GetEmpOhadDetails")]
        public async Task<IActionResult> GetEmpOhadDetails(long Id = 0)
        {

            var chk = await permission.HasPermission(198, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (Id <= 0)
            {
                return Ok(await Result<HrOhadDetailsVw>.FailAsync($"there is no Data found For This Id:  {Id}"));
            }
            try
            {
                var result = await hrServiceManager.HrOhadDetailService.GetAllVW(f => f.EmpId == Id && f.IsDeleted == false);
                return Ok(result);
            }


            catch (Exception ex)
            {

                return Ok(await Result<HrOhadDetailsVw>.FailAsync($"======= Exp in Get Employee Ohad Details  Data, MESSAGE: {ex.Message}"));
            }
        }

        //انذارات الموظف 
        [HttpGet("GetEmpWarn")]
        public async Task<IActionResult> GetEmpWarn(long Id = 0)
        {

            var chk = await permission.HasPermission(198, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (Id <= 0)
            {
                return Ok(await Result<HrEmpWarnVw>.FailAsync($"there is no Data found For This Id:  {Id}"));
            }
            try
            {
                var result = await hrServiceManager.HrEmpWarnService.GetAllVW(f => f.EmpId == Id && f.IsDeleted == false);
                return Ok(result);
            }


            catch (Exception ex)
            {

                return Ok(await Result<HrEmpWarnVw>.FailAsync($"======= Exp in Get Employee Warn  Data, MESSAGE: {ex.Message}"));
            }
        }

        //رصيد الاجازات
        [HttpGet("GetVacationBalance")]
        public async Task<IActionResult> GetVacationBalance(long Id = 0)
        {

            var chk = await permission.HasPermission(198, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (Id <= 0)
            {
                return Ok(await Result<HrVacationBalanceVw>.FailAsync($"there is no Data found For This Id:  {Id}"));
            }
            try
            {
                var result = await hrServiceManager.HrVacationBalanceService.GetAllVW(f => f.EmpId == Id && f.IsDeleted == false);
                return Ok(result);
            }


            catch (Exception ex)
            {

                return Ok(await Result<HrVacationBalanceVw>.FailAsync($"======= Exp in Get Employee Vacation Balance Data, MESSAGE: {ex.Message}"));
            }
        }

        //الوقت الاضافي 
        [HttpGet("GetEmpOverTime")]
        public async Task<IActionResult> GetEmpOverTime(long Id = 0)
        {

            var chk = await permission.HasPermission(198, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (Id <= 0)
            {
                return Ok(await Result<HrOverTimeMVw>.FailAsync($"there is no Data found For This Id:  {Id}"));
            }
            try
            {
                var result = await hrServiceManager.HrOverTimeMService.GetAllVW(f => f.EmpId == Id && f.IsDeleted == false);
                return Ok(result);
            }


            catch (Exception ex)
            {

                return Ok(await Result<HrOverTimeMVw>.FailAsync($"======= Exp in Get Employee Over Time  Data, MESSAGE: {ex.Message}"));
            }
        }
        //نهاية الخدمة  
        [HttpGet("GetEmpEndService")]
        public async Task<IActionResult> GetEmpEndService(long Id = 0)
        {

            var chk = await permission.HasPermission(198, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (Id <= 0)
            {
                return Ok(await Result<HrLeaveVw>.FailAsync($"there is no Data found For This Id:  {Id}"));
            }
            try
            {
                var result = await hrServiceManager.HrLeaveService.GetAllVW(f => f.EmpId == Id && f.IsDeleted == false);
                return Ok(result);
            }


            catch (Exception ex)
            {

                return Ok(await Result<HrLeaveVw>.FailAsync($"======= Exp in Get Employee Leave  Data, MESSAGE: {ex.Message}"));
            }
        }

        //مواعيد الدوام 
        [HttpGet("GetEmpWorkTime")]
        public async Task<IActionResult> GetEmpWorkTime(long Id = 0)
        {

            var chk = await permission.HasPermission(198, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (Id <= 0)
            {
                return Ok(await Result<HrEmpWorkTimeVw>.FailAsync($"there is no Data found For This Id:  {Id}"));
            }
            try
            {
                var result = await hrServiceManager.HrEmpWorkTimeService.GetAllVW(f => f.EmpId == Id && f.IsDeleted == false);
                return Ok(result);
            }


            catch (Exception ex)
            {

                return Ok(await Result<HrEmpWorkTimeVw>.FailAsync($"======= Exp in Get Employee Work Time  Data, MESSAGE: {ex.Message}"));
            }
        }





        //التابعين للموظف 
        [HttpGet("GetEmpDependent")]
        public async Task<IActionResult> GetEmpDependent(long Id = 0)
        {

            var chk = await permission.HasPermission(198, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (Id <= 0)
            {
                return Ok(await Result<HrDependentsVw>.FailAsync($"there is no Data found For This Id:  {Id}"));
            }
            try
            {
                var result = await hrServiceManager.HrDependentService.GetAllVW(f => f.EmpId == Id && f.IsDeleted == false);
                return Ok(result);
            }


            catch (Exception ex)
            {

                return Ok(await Result<HrDependentsVw>.FailAsync($"======= Exp in Get Employee Dependents  Data, MESSAGE: {ex.Message}"));
            }
        }
        //العلاوات  
        [HttpGet("GetEmpIncrements")]
        public async Task<IActionResult> GetEmpIncrements(long Id = 0)
        {

            var chk = await permission.HasPermission(198, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (Id <= 0)
            {
                return Ok(await Result<HrIncrementsVw>.FailAsync($"there is no Data found For This Id:  {Id}"));
            }
            try
            {
                var result = await hrServiceManager.HrIncrementService.GetAllVW(f => f.EmpId == Id && f.IsDeleted == false);
                return Ok(result);
            }


            catch (Exception ex)
            {

                return Ok(await Result<HrIncrementsVw>.FailAsync($"======= Exp in Get Employee Increments  Data, MESSAGE: {ex.Message}"));
            }
        }


        //التقييم  
        [HttpGet("GetEmpKPI")]
        public async Task<IActionResult> GetEmpKPI(long Id = 0)
        {

            var chk = await permission.HasPermission(198, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (Id <= 0)
            {
                return Ok(await Result<HrIncrementsVw>.FailAsync($"there is no Data found For This Id:  {Id}"));
            }
            try
            {
                List<EmpKpiVM> final = new List<EmpKpiVM>();
                var result = await hrServiceManager.HrKpiService.GetAll(k => k.IsDeleted == false && k.EmpId == Id);

                if (result.Succeeded)
                {
                    foreach (var item in result.Data)
                    {
                        var HrKpiDetaile = await hrServiceManager.HrKpiDetaileService.GetAll(d => d.KpiId == item.Id);
                        var toTalDegree = HrKpiDetaile.Data.Sum(s => s.Degree);
                        var singleRecord = new EmpKpiVM
                        {
                            hrKpidto = item,
                            Degree_Total = toTalDegree
                        };

                        final.Add(singleRecord);
                    }
                    return Ok(final);
                }

                return Ok(result);

            }


            catch (Exception ex)
            {

                return Ok(await Result<HrIncrementsVw>.FailAsync($"======= Exp in Get Employee Increments  Data, MESSAGE: {ex.Message}"));
            }
        }

    }

}
