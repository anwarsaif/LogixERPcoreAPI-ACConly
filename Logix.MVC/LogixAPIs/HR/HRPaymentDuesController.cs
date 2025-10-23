using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.DTOs.Main;
using Logix.Application.DTOs.WF;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.HR.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Result = Logix.Application.Wrapper.Result;

namespace Logix.MVC.LogixAPIs.HR
{
    //   مسير مستحقات أخرى
    public class HRPaymentDuesController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly ISysConfigurationHelper sysConfigurationHelper;
        private readonly IWFServiceManager wFServiceManager;
        private readonly IMainServiceManager mainServiceManager;


        public HRPaymentDuesController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, ISysConfigurationHelper sysConfigurationHelper, IWFServiceManager wFServiceManager, IMainServiceManager mainServiceManager)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.sysConfigurationHelper = sysConfigurationHelper;
            this.wFServiceManager = wFServiceManager;
            this.mainServiceManager = mainServiceManager;
        }

        #region IndexPage


        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrPayrollFilterDto filter)
        {
            var chk = await permission.HasPermission(900, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var BranchesList = session.Branches.Split(',');
                filter.BranchId ??= 0;
                if (filter.FinancelYear == 0 || filter.FinancelYear == null)
                {
                    return Ok(await Result<HrPayrollFilterDto>.FailAsync(" يجب اختيار السنة المالية"));

                }

                List<HrPayrollFilterDto> resultList = new List<HrPayrollFilterDto>();
                var items = await hrServiceManager.HrPayrollService.GetAllVW(e => e.IsDeleted == false &&
                e.PayrollTypeId != 1 &&
                e.FinancelYear == filter.FinancelYear &&
                (filter.BranchId == 0 || BranchesList.Contains(e.BranchId.ToString())) &&
                (filter.PayrollTypeId == null || filter.PayrollTypeId == 0 || filter.PayrollTypeId == e.PayrollTypeId) &&
                (string.IsNullOrEmpty(filter.MsMonth) || filter.MsMonth == "0" || filter.MsMonth == "00" || Convert.ToInt32(filter.MsMonth) == Convert.ToInt32(e.MsMonth))

                );
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
        public async Task<IActionResult> Delete(long Id, int? State)
        {
            try
            {
                var chk = await permission.HasPermission(900, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await hrServiceManager.HrPayrollService.Remove(Id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete HR PaymentDues Controller, MESSAGE: {ex.Message}"));
            }
        }


        #endregion

        [HttpGet("EmpIdChanged")]
        public async Task<IActionResult> EmpIdChanged(string EmpId)
        {

            if (string.IsNullOrEmpty(EmpId))
            {
                return Ok(await Result<EmpIdChangedVM>.SuccessAsync("there is no id passed"));
            }

            try
            {
                var checkEmpId = await hrServiceManager.HrEmployeeService.GetOneVW(i => i.EmpId == EmpId && i.Isdel == false && i.IsDeleted == false);
                if (checkEmpId.Succeeded)
                {
                    if (checkEmpId.Data != null)
                    {
                        return Ok(await Result<object>.SuccessAsync(new { empName = checkEmpId.Data.EmpName, AccountNo = checkEmpId.Data.AccountNo, BankID = checkEmpId.Data.BankId }));
                    }
                    else
                    {
                        return Ok(await Result<object>.SuccessAsync($"There is No Employee with this Id:  {EmpId}"));

                    }
                }
                return Ok(await Result<object>.FailAsync($"{checkEmpId.Status.message}"));
            }
            catch (Exception exp)
            {
                return Ok(await Result<object>.FailAsync($"{exp.Message}"));
            }
        }
        #region AddPage Business
        [HttpPost("Add")]
        public async Task<ActionResult> Add(HrPaymentDueAddDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(900, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (string.IsNullOrEmpty(obj.MSDate))
                    return Ok(await Result<string>.FailAsync("يجب ادخال تاريخ صحيح  "));
                if (string.IsNullOrEmpty(obj.MSTitle))
                    return Ok(await Result<string>.FailAsync("يجب ادخال العنوان  "));
                if (obj.PayrolllTypeId <= 0)
                    return Ok(await Result<string>.FailAsync("يجب اختيار نوع المسير   "));
                if (obj.DetailsDto.Count <= 0)
                    return Ok(await Result<string>.FailAsync("يجب توافر بيانات الموظفين  "));
                obj.State = 1;
                var add = await hrServiceManager.HrPayrollService.AddNewPaymentDues(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"====== Exp in Add HR PaymentDues   Controller  Add Method, MESSAGE: {ex.Message}"));
            }
        }

        #endregion

        //  صفحة العرض لها تداخل مع نظام الخدمة الذاتية لذلك تم تأجيل العمل بالاضافة الى عدم فهم الألية فهم كافي

        #region EditPage


        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long MsId, long AppId = 0)
        {
            try
            {
                var BranchesList = session.Branches.Split(',');
                var fileDtos = new List<SaveFileDto>();

                var PayrollByAppId = new HrPayrollDto();
                var PayrollByAppIdOnly = new WfApplicationDto();
                var chk = await permission.HasPermission(900, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (MsId <= 0)
                {
                    return Ok(await Result<object>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var GetPayroll = await hrServiceManager.HrPayrollService.GetOne(x => x.IsDeleted == false && x.MsId == MsId);
                if (GetPayroll.Data == null)
                {
                    return Ok(await Result<object>.FailAsync($"لا يوجد مسير بهذا الرقم  {MsId}"));

                }
                var GetPayrollDetails = await hrServiceManager.HrPayrollDService.GetAllVW(x => x.IsDeleted == false && x.MsId == MsId && BranchesList.Contains(x.BranchId.ToString()));

                if (AppId > 0)
                {
                    var GetPayrollByAppId = await hrServiceManager.HrPayrollService.GetOne(x => x.IsDeleted == false && x.AppId == AppId && x.AppId != 0); ;
                    if (GetPayrollByAppId.Data != null)
                    {
                        PayrollByAppId = GetPayrollByAppId.Data;
                    }
                    var GetApplicationsByIDonly = await wFServiceManager.WfApplicationService.GetApplicationsByIDonly(AppId);
                    if (GetApplicationsByIDonly.Data != null)
                    {
                        PayrollByAppIdOnly = GetApplicationsByIDonly.Data.appData;
                    }
                }
                var GetSysFiles = await mainServiceManager.SysFileService.GetAll(x => x.IsDeleted == false && x.PrimaryKey == MsId && x.TableId == 37);
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
                        fileDtos.Add(singleFile);
                    }
                }

                return Ok(await Result<object>.SuccessAsync(new { payrollData = GetPayroll.Data, PayrollByAppId = PayrollByAppId, PayrollByAppIdOnly = PayrollByAppIdOnly, PayrollDetails = GetPayrollDetails.Data.ToList(), fileDtos = fileDtos }));

            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"====== Exp in HR HRPaymentDuesController Payroll Controller getById, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetPayrollNotes")]
        public async Task<IActionResult> GetPayrollNotes(long Id)
        {
            try
            {
                var chk = await permission.HasPermission(900, PermissionType.Edit);
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
                return Ok(await Result<HrHolidayVw>.FailAsync($"====== Exp in HR  HRPaymentDuesController Controller getById, MESSAGE: {ex.Message}"));
            }
        }

        #endregion




        #region مسير خارج دوام

        [HttpPost("PayrollOverTimeSearchInAdd")]
        public async Task<IActionResult> PayrollOverTimeSearchInAdd(PayrollDueSearchInAddDto filter)
        {
            // Ensure the user has permission (if required)
            var chk = await permission.HasPermission(900, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                // Fetch overtime master records
                var overtimeMasters = await hrServiceManager.HrOverTimeMService.GetAllVW(x => x.IsDeleted == false && x.DateTran != null && x.PaymentType == 2 && x.FacilityId == session.FacilityId);

                if (overtimeMasters.Succeeded && overtimeMasters.Data.Any())
                {
                    var resultList = overtimeMasters.Data.AsQueryable();
                    var HrOverTimeIds = resultList.Select(x => x.Id).ToList();
                    // Apply date filters for both master and detail tables if DateFrom and DateTo are not null
                    if (!string.IsNullOrEmpty(filter.FromDate) && !string.IsNullOrEmpty(filter.ToDate))
                    {
                        var DateFrom = DateHelper.StringToDate(filter.FromDate);
                        var DateTo = DateHelper.StringToDate(filter.ToDate);
                        resultList = resultList.Where(x => DateHelper.StringToDate(x.DateTran) >= DateFrom && DateHelper.StringToDate(x.DateTran) <= DateTo);
                    }

                    var overtimeDetails = await hrServiceManager.HrOverTimeDService.GetAll(x => x.IsDeleted == false && x.OverTimeDate != null && HrOverTimeIds.Contains((long)x.IdM));
                    var filteredOvertimeDetails = overtimeDetails.Data.ToList();

                    // Apply Date filtering and GroupBy for overtime details
                    List<HrOverTimeGroupedDto> groupedOvertimeDetails = new List<HrOverTimeGroupedDto>();
                    if (!string.IsNullOrEmpty(filter.FromDate) && !string.IsNullOrEmpty(filter.ToDate))
                    {
                        var DateFrom = DateHelper.StringToDate(filter.FromDate);
                        var DateTo = DateHelper.StringToDate(filter.ToDate);
                        groupedOvertimeDetails = filteredOvertimeDetails
                            .Where(d => DateHelper.StringToDate(d.OverTimeDate) >= DateFrom && DateHelper.StringToDate(d.OverTimeDate) <= DateTo)
                            .GroupBy(d => d.IdM)
                            .Select(g => new HrOverTimeGroupedDto
                            {
                                IdM = g.Key,
                                HoursSum = g.Sum(d => d.Hours),
                                TotalSum = g.Sum(d => d.Total)
                            }).ToList();
                    }

                    // NOT EXISTS Logic (Using Any)
                    var payrollRecords = await hrServiceManager.HrPayrollService.GetAll(x => x.IsDeleted == false && x.PayrollTypeId == 4);
                    var payrollDetails = await hrServiceManager.HrPayrollDService.GetAll(x => x.IsDeleted == false);

                    var finalList = from master in resultList
                                    join detail in groupedOvertimeDetails on master.Id equals detail.IdM
                                    where !payrollDetails.Data
                                        .Any(pd => payrollRecords.Data
                                            .Any(pr => pr.IsDeleted == false && pr.MsId == pd.MsId && pd.RefranceNo == master.Id))
                                    group new { master, detail } by new
                                    {
                                        master.EmpName,
                                        master.EmpCode,
                                        master.Id,
                                        master.RefranceId,
                                        master.EmpId,
                                        master.BankId,
                                        master.AccountNo
                                    } into grouped
                                    select new
                                    {
                                        grouped.Key.Id,
                                        grouped.Key.RefranceId,
                                        grouped.Key.EmpName,
                                        grouped.Key.EmpCode,
                                        grouped.Key.EmpId,
                                        grouped.Key.BankId,
                                        grouped.Key.AccountNo,
                                        Net = grouped.Sum(g => g.detail.TotalSum),
                                        Cnt = grouped.Sum(g => g.detail.HoursSum)
                                    };

                    var result = finalList.ToList();

                    if (result.Any())
                    {
                        return Ok(await Result<object>.SuccessAsync(result, ""));
                    }

                    return Ok(await Result<object>.SuccessAsync(new List<object>(), localization.GetResource1("NosearchResult")));
                }

                return Ok(await Result<object>.SuccessAsync(new List<object>(), localization.GetResource1("NosearchResult")));
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"====== Exp in {this.GetType()}, MESSAGE: {ex.Message}"));
            }
        }


        [HttpPost("PayrollOverTimeAdd")]
        public async Task<ActionResult> PayrollOverTimeAdd(PayrollOverTimeAddDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(900, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (string.IsNullOrEmpty(obj.MSDate))
                    return Ok(await Result<string>.FailAsync("يجب ادخال تاريخ صحيح  "));
                if (string.IsNullOrEmpty(obj.MSTitle))
                    return Ok(await Result<string>.FailAsync("يجب ادخال العنوان  "));
                if (obj.DetailsDto.Count <= 0)
                    return Ok(await Result<string>.FailAsync("يجب توافر بيانات الموظفين  "));
                obj.PayrolllTypeId = 4;
                obj.State = 1;

                var add = await hrServiceManager.HrPayrollService.PayrollOverTimeAdd(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"====== Exp in {this.GetType()}, MESSAGE: {ex.Message}"));
            }
        }


        #endregion



        #region مسير بدل سكن


        [HttpGet("PayrollHousingAllowanceEmpIdChanged")]
        public async Task<IActionResult> PayrollHousingAllowanceEmpIdChanged(string EmpCode)
        {

            if (string.IsNullOrEmpty(EmpCode))
                return Ok(await Result<object>.SuccessAsync(localization.GetResource1("EmployeeIsNumber")));

            try
            {
                decimal? Amount = 0;
                var checkEmpId = await hrServiceManager.HrEmployeeService.GetOneVW(i => i.EmpId == EmpCode && i.Isdel == false && i.IsDeleted == false);
                if (checkEmpId.Succeeded)
                {
                    if (checkEmpId.Data == null)
                        return Ok(await Result<object>.SuccessAsync(localization.GetResource1("EmployeeNotFound")));

                    var GetHrSettings = await hrServiceManager.HrSettingService.GetOne(x => x.FacilityId == session.FacilityId);
                    var GetAllowances = await hrServiceManager.HrAllowanceDeductionService.GetAllVW(x => x.Status == true && x.IsDeleted == false && x.FixedOrTemporary == 1 && x.AdId == GetHrSettings.Data.HousingAllowance && x.EmpCode == EmpCode);
                    if (GetAllowances.Data != null)
                    {
                        var getLastItem = GetAllowances.Data.ToList().OrderByDescending(x => x.Id).FirstOrDefault();
                        if (getLastItem != null)
                            Amount = getLastItem.Amount * 12;


                    }
                    var result = new
                    {
                        Net = Amount,
                        EmpName = checkEmpId.Data.EmpName,
                        EmpCode = EmpCode,
                    };
                    return Ok(await Result<object>.SuccessAsync(result));

                }
                return Ok(await Result<object>.SuccessAsync(localization.GetResource1("EmployeeNotFound")));
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"====== Exp in {this.GetType()}, MESSAGE: {ex.Message}"));
            }
        }



        [HttpPost("PayrollHousingAllowanceAdd")]
        public async Task<ActionResult> PayrollHousingAllowanceAdd(PayrollHousingAllowancesAddDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(900, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (string.IsNullOrEmpty(obj.MSDate))
                    return Ok(await Result<string>.FailAsync("يجب ادخال تاريخ صحيح  "));
                if (string.IsNullOrEmpty(obj.MSTitle))
                    return Ok(await Result<string>.FailAsync("يجب ادخال العنوان  "));
                if (obj.DetailsDto.Count <= 0)
                    return Ok(await Result<string>.FailAsync("يجب توافر بيانات الموظفين  "));
                obj.PayrolllTypeId = 8;
                obj.State = 1;

                var add = await hrServiceManager.HrPayrollService.PayrollHousingAllowanceAdd(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"====== Exp in {this.GetType()}, MESSAGE: {ex.Message}"));
            }
        }

        #endregion


        #region    مسير انتداب 


        [HttpPost("PayrollMandatSearchInAdd")]
        public async Task<IActionResult> PayrollMandatSearchInAdd(PayrollDueSearchInAddDto filter)
        {
            var chk = await permission.HasPermission(900, PermissionType.Add);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                // Fetch records from HR_Mandate_VW
                var mandates = await hrServiceManager.HrMandateService.GetAllVW(x => x.IsDeleted == false);

                if (mandates.Succeeded && mandates.Data.Any())
                {
                    var resultList = mandates.Data.AsQueryable();

                    // Apply date filters if FromDate and ToDate are provided
                    if (!string.IsNullOrEmpty(filter.FromDate) && !string.IsNullOrEmpty(filter.ToDate))
                    {
                        var FromDate1 = DateHelper.StringToDate(filter.FromDate);
                        var FromDate2 = DateHelper.StringToDate(filter.ToDate);
                        resultList = resultList.Where(x => DateHelper.StringToDate(x.FromDate) >= FromDate1 && DateHelper.StringToDate(x.FromDate) <= FromDate2);
                    }



                    var payrolls = await hrServiceManager.HrPayrollService.GetAll(x => x.IsDeleted == false);
                    resultList = resultList.Where(x => !payrolls.Data.Any(p => p.MsId == x.PayrollId));

                    var finalList = resultList.Select(x => new
                    {
                        x.Id,
                        x.EmpName,
                        x.EmpCode,
                        x.EmpId,
                        Net = x.ActualExpenses,
                        CountDays = x.NoOfNight,
                        Note = x.ToLocation,
                        x.TransportAmount,
                        Amount = x.ActualExpenses - x.TransportAmount,
                    }).ToList();

                    if (finalList.Any())
                    {
                        return Ok(await Result<object>.SuccessAsync(finalList, ""));
                    }

                    return Ok(await Result<object>.SuccessAsync(new List<object>(), localization.GetResource1("NosearchResult")));
                }

                return Ok(await Result<object>.SuccessAsync(new List<object>(), localization.GetResource1("NosearchResult")));
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"====== Exp in {this.GetType()}, MESSAGE: {ex.Message}"));
            }
        }


        [HttpPost("PayrollMandateAdd")]
        public async Task<ActionResult> PayrollMandateAdd(PayrollMandateAddDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(900, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (string.IsNullOrEmpty(obj.MSDate))
                    return Ok(await Result<string>.FailAsync("يجب ادخال تاريخ صحيح  "));
                if (string.IsNullOrEmpty(obj.MSTitle))
                    return Ok(await Result<string>.FailAsync("يجب ادخال العنوان  "));
                if (obj.DetailsDto.Count <= 0)
                    return Ok(await Result<string>.FailAsync("يجب توافر بيانات الموظفين  "));
                obj.PayrolllTypeId = 3;
                obj.State = 1;

                var add = await hrServiceManager.HrPayrollService.PayrollMandateAdd(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"====== Exp in {this.GetType()}, MESSAGE: {ex.Message}"));
            }
        }

        #endregion


        #region مسير تذاكر مستحقة


        [HttpGet("PayrollTicketAllowanceEmpIdChanged")]
        public async Task<IActionResult> PayrollTicketAllowanceEmpIdChanged(string EmpCode)
        {

            if (string.IsNullOrEmpty(EmpCode))
                return Ok(await Result<object>.SuccessAsync(localization.GetResource1("EmployeeIsNumber")));

            try
            {
                decimal? Amount = 0;
                var checkEmpId = await hrServiceManager.HrEmployeeService.GetOneVW(i => i.EmpId == EmpCode && i.Isdel == false && i.IsDeleted == false);
                if (checkEmpId.Succeeded)
                {
                    if (checkEmpId.Data == null)
                        return Ok(await Result<object>.SuccessAsync(localization.GetResource1("EmployeeNotFound")));

                    var GetHrSettings = await hrServiceManager.HrSettingService.GetOne(x => x.FacilityId == session.FacilityId);
                    var GetAllowances = await hrServiceManager.HrAllowanceDeductionService.GetAllVW(x => x.Status == true && x.IsDeleted == false && x.FixedOrTemporary == 1 && x.AdId == GetHrSettings.Data.TicketAllowance && x.EmpCode == EmpCode);
                    if (GetAllowances.Data != null)
                    {
                        var getLastItem = GetAllowances.Data.ToList().OrderByDescending(x => x.Id).FirstOrDefault();
                        if (getLastItem != null)
                            Amount = getLastItem.Amount * 12;


                    }
                    var result = new
                    {
                        Net = Amount,
                        EmpName = checkEmpId.Data.EmpName,
                        EmpCode = EmpCode,
                    };
                    return Ok(await Result<object>.SuccessAsync(result));

                }
                return Ok(await Result<object>.SuccessAsync(localization.GetResource1("EmployeeNotFound")));
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"====== Exp in {this.GetType()}, MESSAGE: {ex.Message}"));
            }
        }



        [HttpPost("PayrollTicketAllowanceAdd")]
        public async Task<ActionResult> PayrollTicketAllowanceAdd(PayrollTicketAllowancesAddDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(900, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (string.IsNullOrEmpty(obj.MSDate))
                    return Ok(await Result<string>.FailAsync("يجب ادخال تاريخ صحيح  "));
                if (string.IsNullOrEmpty(obj.MSTitle))
                    return Ok(await Result<string>.FailAsync("يجب ادخال العنوان  "));
                if (obj.DetailsDto.Count <= 0)
                    return Ok(await Result<string>.FailAsync("يجب توافر بيانات الموظفين  "));
                obj.PayrolllTypeId = 7;
                obj.State = 1;

                var add = await hrServiceManager.HrPayrollService.PayrollTicketAllowanceAdd(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"====== Exp in {this.GetType()}, MESSAGE: {ex.Message}"));
            }
        }

        #endregion



        #region خارج دوام يدوي

        /// <summary>
        /// 
        /// </summary>
        /// <param name="EmpCode">رقم الموظف </param>
        /// <param name="OverTime">  الساعات  وافتراضيا  تساوي صفر</param>
        /// <returns></returns>


        //[HttpGet("PayrollOverTime2EmpIdChanged")]
        //public async Task<IActionResult> PayrollOverTime2EmpIdChanged(string EmpCode, decimal OverTime)
        //{

        //    if (string.IsNullOrEmpty(EmpCode))
        //        return Ok(await Result<object>.SuccessAsync(localization.GetResource1("EmployeeIsNumber")));

        //    try
        //    {
        //        var BranchesList = session.Branches.Split(',');

        //        decimal? ContDaySlary = 0;
        //        decimal? CountDayallowance = 0;
        //        decimal? Salary = 0;
        //        decimal? TotalAllowance = 0;
        //        decimal Net = 0;
        //        var checkEmpId = await hrServiceManager.HrEmployeeService.GetOneVW(i => i.EmpId == EmpCode && i.Isdel == false && i.IsDeleted == false);
        //        if (!checkEmpId.Succeeded)
        //            return Ok(await Result<object>.FailAsync(localization.GetResource1("EmployeeNotFound")));
        //        if (checkEmpId.Data == null)
        //            return Ok(await Result<object>.FailAsync(localization.GetResource1("EmployeeNotFound")));
        //        Salary = checkEmpId.Data.Salary;
        //        var CHeckEmpInBranch = await hrServiceManager.HrEmployeeService.GetOne(x => x.Id == checkEmpId.Data.Id && x.IsDeleted == false && x.Isdel == false && BranchesList.Contains(x.BranchId.ToString()));

        //        if (CHeckEmpInBranch.Data == null)
        //            return Ok(await Result<object>.FailAsync(localization.GetHrResource("UserNotHasPermissionToEmp")));

        //        var getTotalAllowance = await hrServiceManager.HrAllowanceDeductionService.GetAll(e => e.EmpId == checkEmpId.Data.Id && e.IsDeleted == false && e.FixedOrTemporary == 1 && e.TypeId == 1);
        //        if (getTotalAllowance.Data != null)
        //        {
        //            foreach (var item in getTotalAllowance.Data)
        //            {
        //                TotalAllowance += (item.Amount != null ? item.Amount.Value : 0);
        //            }
        //        }
        //        var GetContDaySlary = await mainServiceManager.SysPropertyValueService.GetOne(x => x.PropertyValue == "129" && x.FacilityId == session.FacilityId);
        //        if (GetContDaySlary.Data != null)
        //        {
        //            if (!string.IsNullOrEmpty(GetContDaySlary.Data.PropertyValue))
        //            {
        //                ContDaySlary = Convert.ToDecimal(GetContDaySlary.Data.PropertyValue);
        //            }

        //        }
        //        var GetCountDayallowance = await mainServiceManager.SysPropertyValueService.GetOne(x => x.PropertyValue == "130" && x.FacilityId == session.FacilityId);
        //        if (GetCountDayallowance.Data != null)
        //        {
        //            if (!string.IsNullOrEmpty(GetCountDayallowance.Data.PropertyValue))
        //            {
        //                CountDayallowance = Convert.ToDecimal(GetCountDayallowance.Data.PropertyValue);
        //            }
        //        }
        //        decimal totalslary = (Salary * OverTime / ContDaySlary) ?? 0;
        //        decimal totalallowance = (TotalAllowance * OverTime / CountDayallowance) ?? 0;
        //        Net = Math.Round(totalslary + totalallowance);
        //        var result = new
        //        {
        //            Salary = Salary,
        //            EmpName = checkEmpId.Data.EmpName,
        //            EmpCode = EmpCode,
        //            Allowances = TotalAllowance,
        //            Net = Net
        //        };
        //        return Ok(await Result<object>.SuccessAsync(result));


        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(await Result<string>.FailAsync($"====== Exp in {this.GetType()}, MESSAGE: {ex.Message}"));
        //    }
        //}

        // عند تغيير عدد الساعات
        //[HttpGet("PayrollOverTime2OverTimeCountChanged")]
        //public async Task<IActionResult> PayrollOverTime2OverTimeCountChanged(decimal OverTime, decimal Salary, decimal Allowances)
        //{


        //    try
        //    {
        //        decimal? CountDayWork = 0;
        //        decimal? ContOverTime = 0;
        //        decimal? CountDayallowance = 0;
        //        decimal? ContDaySlary = 0;
        //        var GetAllProperties = await mainServiceManager.SysPropertyValueService.GetAll(x => (x.PropertyValue == "128" || x.PropertyValue == "129" || x.PropertyValue == "130" || x.PropertyValue == "131") && x.FacilityId == session.FacilityId);
        //        if (GetAllProperties.Data != null)
        //        {
        //            var allProperties = GetAllProperties.Data.AsQueryable();
        //            var GetContOverTime = allProperties.Where(x => x.PropertyValue == "128").FirstOrDefault();
        //            if (GetContOverTime != null)
        //            {
        //                if (!string.IsNullOrEmpty(GetContOverTime.PropertyValue))
        //                {
        //                    ContOverTime = Convert.ToDecimal(GetContOverTime.PropertyValue);
        //                }

        //            }
        //            ////////////////////////////
        //            ///
        //            var GetCountDayWork = allProperties.Where(x => x.PropertyValue == "131").FirstOrDefault();
        //            if (GetCountDayWork != null)
        //            {
        //                if (!string.IsNullOrEmpty(GetCountDayWork.PropertyValue))
        //                {
        //                    CountDayWork = Convert.ToDecimal(GetCountDayWork.PropertyValue);
        //                }

        //            }
        //            ////////////////////////////
        //            var GetContDaySlary = allProperties.Where(x => x.PropertyValue == "129").FirstOrDefault();
        //            if (GetContDaySlary != null)
        //            {
        //                if (!string.IsNullOrEmpty(GetContDaySlary.PropertyValue))
        //                {
        //                    ContDaySlary = Convert.ToDecimal(GetContDaySlary.PropertyValue);
        //                }

        //            }
        //            ////////////////////////////
        //            var GetCountDayallowance = allProperties.Where(x => x.PropertyValue == "130").FirstOrDefault();
        //            if (GetCountDayallowance != null)
        //            {
        //                if (!string.IsNullOrEmpty(GetCountDayallowance.PropertyValue))
        //                {
        //                    CountDayallowance = Convert.ToDecimal(GetCountDayallowance.PropertyValue);
        //                }
        //            }
        //        }
        //        if (OverTime <= ContOverTime)
        //        {
        //            decimal totalslary = (Salary * OverTime / ContDaySlary) ?? 0;
        //            decimal totalallowance = (Allowances * OverTime / CountDayallowance) ?? 0;
        //            var Net = Math.Round(totalslary + totalallowance);
        //            return Ok(await Result<object>.SuccessAsync(new { Net = Net }));

        //        }
        //        else
        //        {
        //            // ترجع عدد الساعات
        //            return Ok(await Result<object>.SuccessAsync(new { OverTime = ContOverTime }, "عدد إجمالي الساعات أكير من عدد اجمالي الساعات المسموح به في الإعدادات لمسير العمل الإضافي اليدوي"));

        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(await Result<string>.FailAsync($"====== Exp in {this.GetType()}, MESSAGE: {ex.Message}"));
        //    }
        //}


        // عند تغيير عدد الايام


        [HttpGet("PayrollOverTime2EmpCodeChanged")]
        public async Task<IActionResult> PayrollOverTime2EmpCodeChanged(string EmpCode)
        {


            try
            {
                string? CountDayWork = "";
                string? ContOverTime = "";
                string? CountDayallowance = "";
                string? ContDaySlary = "";
                decimal? Salary = 0;
                decimal? TotalAllowance = 0;
                var BranchesList = session.Branches.Split(',');

                var checkEmpId = await hrServiceManager.HrEmployeeService.GetOneVW(i => i.EmpId == EmpCode && i.Isdel == false && i.IsDeleted == false);
                if (!checkEmpId.Succeeded)
                    return Ok(await Result<object>.FailAsync(localization.GetResource1("EmployeeNotFound")));
                if (checkEmpId.Data == null)
                    return Ok(await Result<object>.FailAsync(localization.GetResource1("EmployeeNotFound")));
                Salary = checkEmpId.Data.Salary;
                var CHeckEmpInBranch = await hrServiceManager.HrEmployeeService.GetOne(x => x.Id == checkEmpId.Data.Id && x.IsDeleted == false && x.Isdel == false && BranchesList.Contains(x.BranchId.ToString()));

                if (CHeckEmpInBranch.Data == null)
                    return Ok(await Result<object>.FailAsync(localization.GetHrResource("UserNotHasPermissionToEmp")));

                var getTotalAllowance = await hrServiceManager.HrAllowanceDeductionService.GetTotalAllowances(checkEmpId.Data.Id);
                if (getTotalAllowance.Succeeded) TotalAllowance = getTotalAllowance.Data;

                var GetAllProperties = await mainServiceManager.SysPropertyValueService.GetAll(x => (x.PropertyValue == "128" || x.PropertyValue == "129" || x.PropertyValue == "130" || x.PropertyValue == "131") && x.FacilityId == session.FacilityId);
                if (GetAllProperties.Data != null)
                {
                    var allProperties = GetAllProperties.Data.AsQueryable();
                    var GetContOverTime = allProperties.Where(x => x.PropertyValue == "128").FirstOrDefault();
                    if (GetContOverTime != null)
                    {
                        if (!string.IsNullOrEmpty(GetContOverTime.PropertyValue))
                        {
                            ContOverTime = GetContOverTime.PropertyValue;
                        }

                    }
                    ////////////////////////////
                    ///
                    var GetCountDayWork = allProperties.Where(x => x.PropertyValue == "131").FirstOrDefault();
                    if (GetCountDayWork != null)
                    {
                        if (!string.IsNullOrEmpty(GetCountDayWork.PropertyValue))
                        {
                            CountDayWork = GetCountDayWork.PropertyValue;
                        }

                    }
                    ////////////////////////////
                    var GetContDaySlary = allProperties.Where(x => x.PropertyValue == "129").FirstOrDefault();
                    if (GetContDaySlary != null)
                    {
                        if (!string.IsNullOrEmpty(GetContDaySlary.PropertyValue))
                        {
                            ContDaySlary = GetContDaySlary.PropertyValue;
                        }

                    }
                    ////////////////////////////
                    var GetCountDayallowance = allProperties.Where(x => x.PropertyValue == "130").FirstOrDefault();
                    if (GetCountDayallowance != null)
                    {
                        if (!string.IsNullOrEmpty(GetCountDayallowance.PropertyValue))
                        {
                            CountDayallowance = GetCountDayallowance.PropertyValue;
                        }
                    }
                }

                return Ok(await Result<object>.SuccessAsync(new { CountDayWork = CountDayWork, ContOverTime = ContOverTime, ContDaySlary = ContDaySlary, CountDayallowance = CountDayallowance, Allowance = TotalAllowance, Salary = Salary }));

            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"====== Exp in {this.GetType()}, MESSAGE: {ex.Message}"));
            }
        }


        //[HttpGet("PayrollOverTime2DaysCountChanged")]
        //public async Task<IActionResult> PayrollOverTime2DaysCountChanged(decimal DaysCount, decimal OverTime, decimal Salary, decimal Allowances)
        //{


        //    try
        //    {
        //        decimal? CountDayWork = 0;
        //        decimal? ContOverTime = 0;
        //        decimal? CountDayallowance = 0;
        //        decimal? ContDaySlary = 0;
        //        var GetAllProperties = await mainServiceManager.SysPropertyValueService.GetAll(x => (x.PropertyValue == "128" || x.PropertyValue == "129" || x.PropertyValue == "130" || x.PropertyValue == "131") && x.FacilityId == session.FacilityId);
        //        if (GetAllProperties.Data != null)
        //        {
        //            var allProperties = GetAllProperties.Data.AsQueryable();
        //            var GetContOverTime = allProperties.Where(x => x.PropertyValue == "128").FirstOrDefault();
        //            if (GetContOverTime != null)
        //            {
        //                if (!string.IsNullOrEmpty(GetContOverTime.PropertyValue))
        //                {
        //                    ContOverTime = Convert.ToDecimal(GetContOverTime.PropertyValue);
        //                }

        //            }
        //            ////////////////////////////
        //            ///
        //            var GetCountDayWork = allProperties.Where(x => x.PropertyValue == "131").FirstOrDefault();
        //            if (GetCountDayWork != null)
        //            {
        //                if (!string.IsNullOrEmpty(GetCountDayWork.PropertyValue))
        //                {
        //                    CountDayWork = Convert.ToDecimal(GetCountDayWork.PropertyValue);
        //                }

        //            }
        //            ////////////////////////////
        //            var GetContDaySalary = allProperties.Where(x => x.PropertyValue == "129").FirstOrDefault();
        //            if (GetContDaySalary != null)
        //            {
        //                if (!string.IsNullOrEmpty(GetContDaySalary.PropertyValue))
        //                {
        //                    ContDaySlary = Convert.ToDecimal(GetContDaySalary.PropertyValue);
        //                }

        //            }
        //            ////////////////////////////
        //            var GetCountDayallowance = allProperties.Where(x => x.PropertyValue == "130").FirstOrDefault();
        //            if (GetCountDayallowance != null)
        //            {
        //                if (!string.IsNullOrEmpty(GetCountDayallowance.PropertyValue))
        //                {
        //                    CountDayallowance = Convert.ToDecimal(GetCountDayallowance.PropertyValue);
        //                }
        //            }
        //        }
        //        if (DaysCount <= CountDayWork)
        //        {
        //            decimal totalslary = ((Salary * OverTime) / ContDaySlary) ?? 0;
        //            decimal totalallowance = ((Allowances * OverTime) / CountDayallowance) ?? 0;
        //            var Net = Math.Round(totalslary + totalallowance);
        //            return Ok(await Result<object>.SuccessAsync(new { Net = Net }));

        //        }
        //        else
        //        {
        //            // ترجع عدد الايام
        //            return Ok(await Result<object>.SuccessAsync(new { DaysCount = CountDayWork }, "عدد الأيام أكبر من عدد الأيام المسموح بها في الإعدادات لمسيرات العمل الإضافي اليدوي"));

        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(await Result<string>.FailAsync($"====== Exp in {this.GetType()}, MESSAGE: {ex.Message}"));
        //    }
        //}



        // فحص هل يمكن عمل خارج دوام للموظف
        /* 
         ملاحظة:: يجب حعل من تاريخ والى تاريخ للقراءه فقط بعد اول عمليه اضافة حتى لايتم التلاعب بالتاريخ
         
         */
        [HttpGet("PayrollOverTime2CheckWeCanAdd")]
        public async Task<IActionResult> PayrollOverTime2CheckWeCanAdd(string EmpCode, string FromDate, string ToDate)
        {

            if (string.IsNullOrEmpty(EmpCode))
                return Ok(await Result<object>.SuccessAsync(localization.GetResource1("EmployeeIsNumber")));

            if (string.IsNullOrEmpty(FromDate))
                return Ok(await Result<object>.SuccessAsync(localization.GetCommonResource("FromDate")));

            if (string.IsNullOrEmpty(ToDate))
                return Ok(await Result<object>.SuccessAsync(localization.GetCommonResource("ToDate")));

            try
            {
                var checkEmpId = await hrServiceManager.HrEmployeeService.GetOneVW(i => i.EmpId == EmpCode && i.Isdel == false && i.IsDeleted == false);
                if (!checkEmpId.Succeeded)
                    return Ok(await Result<object>.FailAsync(localization.GetResource1("EmployeeNotFound")));
                if (checkEmpId.Data == null)
                    return Ok(await Result<object>.FailAsync(localization.GetResource1("EmployeeNotFound")));

                var CheckEmployeeInVacation = await hrServiceManager.HrVacationsService.GetAll(X => X.IsDeleted == false && X.EmpId == checkEmpId.Data.Id && X.VacationSdate != null && X.VacationEdate != null);
                var VacationResult = CheckEmployeeInVacation.Data.AsQueryable();

                var BeginDate = DateHelper.StringToDate(FromDate);
                var EndDate = DateHelper.StringToDate(ToDate);
                VacationResult = VacationResult.Where(x =>
                    ((BeginDate >= DateHelper.StringToDate(x.VacationSdate) && BeginDate <= DateHelper.StringToDate(x.VacationEdate)) || (EndDate >= DateHelper.StringToDate(x.VacationSdate) && EndDate <= DateHelper.StringToDate(x.VacationEdate))) ||
                    ((DateHelper.StringToDate(x.VacationSdate) >= BeginDate && DateHelper.StringToDate(x.VacationEdate) <= EndDate) || (DateHelper.StringToDate(x.VacationEdate) >= BeginDate && DateHelper.StringToDate(x.VacationEdate) <= EndDate))

                    );
                if (VacationResult.Count() > 0)
                {
                    return Ok(await Result<object>.SuccessAsync(false, " لايمكن عمل خارج دوام للموظف بسب وجود اجازة بنفس الفترة  "));

                }
                return Ok(await Result<object>.SuccessAsync(true, ""));


            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"====== Exp in {this.GetType()}, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("PayrollOverTime2Add")]
        public async Task<ActionResult> PayrollOverTime2Add(PayrollOverTime2AddDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(900, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (string.IsNullOrEmpty(obj.MSDate))
                    return Ok(await Result<string>.FailAsync("يجب ادخال تاريخ صحيح  "));
                if (string.IsNullOrEmpty(obj.MSTitle))
                    return Ok(await Result<string>.FailAsync("يجب ادخال العنوان  "));
                if (obj.DetailsDto.Count <= 0)
                    return Ok(await Result<string>.FailAsync("يجب توافر بيانات الموظفين  "));
                obj.PayrolllTypeId = 4;
                obj.State = 1;

                var add = await hrServiceManager.HrPayrollService.PayrollOverTime2Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"====== Exp in {this.GetType()}, MESSAGE: {ex.Message}"));
            }
        }

        #endregion



        #region    مسير دوام مرن 


        [HttpPost("PayrollFlexibleWorkingSearchInAdd")]
        public async Task<IActionResult> PayrollFlexibleWorkingSearchInAdd(PayrollDueSearchInAddDto filter)
        {
            var chk = await permission.HasPermission(900, PermissionType.Add);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                // Fetch flexible working records
                var flexibleWorkingRecords = await hrServiceManager.HrFlexibleWorkingService.GetAllVW(x => x.IsDeleted == false);

                if (flexibleWorkingRecords.Succeeded && flexibleWorkingRecords.Data.Any())
                {
                    var resultList = flexibleWorkingRecords.Data.AsQueryable();

                    // Apply date filters if From_Date and To_Date are not null or empty
                    if (!string.IsNullOrEmpty(filter.FromDate) && !string.IsNullOrEmpty(filter.ToDate))
                    {
                        var DateFrom = DateHelper.StringToDate(filter.FromDate);
                        var DateTo = DateHelper.StringToDate(filter.ToDate);
                        resultList = resultList.Where(x => DateHelper.StringToDate(x.AttendanceDate) >= DateFrom && DateHelper.StringToDate(x.AttendanceDate) <= DateTo);
                    }

                    // NOT EXISTS logic
                    var payrollRecords = await hrServiceManager.HrPayrollService.GetAll(x => x.IsDeleted == false && x.PayrollTypeId == 14);
                    var payrollDetails = await hrServiceManager.HrPayrollDService.GetAll(x => x.IsDeleted == false);

                    var finalList = resultList.Where(x => !payrollDetails.Data
                        .Any(pd => payrollRecords.Data
                            .Any(pr => pr.IsDeleted == false && pr.MsId == pd.MsId && pd.RefranceNo == x.Id)))
                        .ToList();

                    // Transform final list to apply ActualMinute calculation and return specific columns
                    var transformedList = finalList.Select(item =>
                    {
                        var minute = item.ActualMinute ?? 0;
                        if (minute > 0)
                        {
                            long hours = (minute / 60);
                            long minutes = (minute % 60);
                            string timeElapsed = $"{hours}:{minutes:D2}";
                            return new
                            {
                                item.Id,
                                item.EmpName,
                                item.EmpCode,
                                ReferenceCode = item.MasterId,
                                item.TotalPrice,
                                WorkHours = timeElapsed
                            };
                        }
                        else
                        {
                            return new
                            {
                                item.Id,
                                item.EmpName,
                                item.EmpCode,
                                ReferenceCode = item.MasterId,
                                item.TotalPrice,
                                WorkHours = ""
                            };
                        }
                    }).ToList();

                    if (transformedList.Any())
                    {
                        return Ok(await Result<object>.SuccessAsync(transformedList, ""));
                    }

                    return Ok(await Result<object>.SuccessAsync(new List<object>(), localization.GetResource1("NosearchResult")));
                }

                return Ok(await Result<object>.SuccessAsync(new List<object>(), localization.GetResource1("NosearchResult")));
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"An error occurred: {ex.Message}"));
            }
        }


        [HttpPost("PayrollFlexibleWorkingAdd")]
        public async Task<ActionResult> PayrollFlexibleWorkingAdd(PayrollFlexibleWorkingAddDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(900, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (string.IsNullOrEmpty(obj.MSDate))
                    return Ok(await Result<string>.FailAsync("يجب ادخال تاريخ صحيح  "));
                if (string.IsNullOrEmpty(obj.MSTitle))
                    return Ok(await Result<string>.FailAsync("يجب ادخال العنوان  "));
                if (obj.DetailsDto.Count <= 0)
                    return Ok(await Result<string>.FailAsync("يجب توافر بيانات الموظفين  "));
                obj.PayrolllTypeId = 14;
                obj.State = 1;

                var add = await hrServiceManager.HrPayrollService.PayrollFlexibleWorkingAdd(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"====== Exp in {this.GetType()}, MESSAGE: {ex.Message}"));
            }
        }

        #endregion
    }
}