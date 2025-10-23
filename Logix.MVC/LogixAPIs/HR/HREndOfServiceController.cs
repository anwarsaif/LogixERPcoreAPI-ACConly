using AutoMapper;
using DevExpress.CodeParser;
using DocumentFormat.OpenXml.Wordprocessing;
using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.DTOs.Main;
using Logix.Application.Helpers;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Services;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.Domain.PM;
using Logix.Infrastructure.Repositories;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Crypto;

namespace Logix.MVC.LogixAPIs.HR
{

    //   إنهاء الخدمة للموظفين
    public class HREndOfServiceController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IApiDDLHelper ddlHelper;
        private readonly IMainServiceManager mainServiceManager;
        private readonly IMapper mapper;


        public HREndOfServiceController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IApiDDLHelper ddlHelper, IMainServiceManager mainServiceManager, IMapper mapper)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.ddlHelper = ddlHelper;
            this.mainServiceManager = mainServiceManager;
            this.mapper = mapper;
        }


        #region Index Page

        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrLeaveFilterDto filter)
        {
            var chk = await permission.HasPermission(263, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                //var BranchesList = session.Branches.Split(',');
                //List<HrLeaveFilterDto> resultList = new List<HrLeaveFilterDto>();
                //var items = await hrServiceManager.HrLeaveService.GetAllVW(e => e.IsDeleted == false && BranchesList.Contains(e.BranchId.ToString()) && e.FacilityId == session.FacilityId);
                //if (items.Succeeded)
                //{
                //    if (items.Data.Count() > 0)
                //    {

                //        var res = items.Data.AsQueryable();
                //        if (!string.IsNullOrEmpty(filter.EmpCode))
                //        {
                //            res = res.Where(c => c.EmpCode != null && c.EmpCode == filter.EmpCode);
                //        }
                //        if (!string.IsNullOrEmpty(filter.EmpName))
                //        {
                //            res = res.Where(c => (c.EmpName != null && c.EmpName.Contains(filter.EmpName)));
                //        }
                //        if (!string.IsNullOrEmpty(filter.FromDate) && !string.IsNullOrEmpty(filter.ToDate))
                //        {
                //            res = res.Where(r => r.LeaveDate != null &&
                //            (DateHelper.StringToDate(r.LeaveDate) >= DateHelper.StringToDate(filter.FromDate)) &&
                //           (DateHelper.StringToDate(r.LeaveDate) <= DateHelper.StringToDate(filter.ToDate))
                //           );
                //        }
                //        if (filter.DeptId != null && filter.DeptId > 0)
                //        {
                //            res = res.Where(c => c.DeptId != null && c.DeptId == filter.DeptId);
                //        }
                //        if (filter.BranchId != null && filter.BranchId > 0)
                //        {
                //            res = res.Where(c => c.BranchId != null && c.BranchId == filter.BranchId);
                //        }
                //        if (filter.Location != null && filter.Location > 0)
                //        {
                //            res = res.Where(c => c.Location != null && c.Location == filter.Location);
                //        }
                //        if (filter.LeaveType != null && filter.LeaveType > 0)
                //        {
                //            res = res.Where(c => c.LeaveType != null && c.LeaveType == filter.LeaveType);
                //        }


                //        foreach (var item in res)
                //        {
                //            var newRecord = new HrLeaveFilterDto
                //            {

                //                Id = item.Id,
                //                EmpCode = item.EmpCode,
                //                EmpName = item.EmpName,
                //                BranchName = item.BraName,
                //                DepName = item.DepName,
                //                LocationName = item.LocationName,
                //                LeaveDate = item.LeaveDate,
                //                TypeName = item.TypeName,
                //                WorkYear = item.WorkYear,


                //            };
                //            resultList.Add(newRecord);
                //        }
                //        if (resultList.Count() > 0)
                //            return Ok(await Result<List<HrLeaveFilterDto>>.SuccessAsync(resultList, ""));
                //        return Ok(await Result<List<HrLeaveFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));
                //    }
                //    return Ok(await Result<List<HrLeaveFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));
                //}
                var items = await hrServiceManager.HrLeaveService.Search(filter);

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrLeaveFilterDto>.FailAsync(ex.Message));
            }
        }

        [HttpPost("GetPagination")]
        public async Task<IActionResult> GetPagination([FromBody] HrLeaveFilterDto filter, int take = 5, long? lastSeenId = null)
        {
            try
            {
                var chk = await permission.HasPermission(263, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                List<HrLeaveFilterDto> resultList = new List<HrLeaveFilterDto>();

                var BranchesList = session.Branches.Split(',');
                filter.LeaveType ??= 0;
                filter.DeptId ??= 0;
                filter.BranchId ??= 0;
                filter.Location ??= 0;

                var dateConditions = new List<DateCondition>();
                dateConditions.Add(new DateCondition
                {
                    DatePropertyName = "LeaveDate",
                    ComparisonOperator = ComparisonOperator.GreaterThanOrEqual,
                    StartDateString = filter.FromDate ?? ""
                });
                dateConditions.Add(new DateCondition
                {
                    DatePropertyName = "LeaveDate",
                    ComparisonOperator = ComparisonOperator.LessThanOrEqual,
                    StartDateString = filter.ToDate ?? ""
                });

                var items = await hrServiceManager.HrLeaveService.GetAllWithPaginationVW(selector: x => x.Id,
                expression: x => x.IsDeleted == false
                && x.FacilityId == session.FacilityId
                && (string.IsNullOrEmpty(filter.EmpCode) || x.EmpCode == filter.EmpCode)
                && (string.IsNullOrEmpty(filter.EmpName) || x.EmpName == filter.EmpName)
                && (filter.LeaveType == 0 || x.LeaveType == filter.LeaveType)
                && (filter.DeptId == 0 || x.DeptId == filter.DeptId)
                && (filter.BranchId != 0 ? x.BranchId == filter.BranchId : BranchesList.Contains(x.BranchId.ToString()))
                && (filter.Location == 0 || x.Location == filter.Location),
                    take: take,
                    lastSeenId: lastSeenId,
                   dateConditions: (string.IsNullOrEmpty(filter.FromDate) || string.IsNullOrEmpty(filter.ToDate)) ? null : dateConditions);

                if (!items.Succeeded)
                    return Ok(await Result<List<HrLeaveVw>>.FailAsync(items.Status.message));
                if (items.Data.Count() > 0)
                {
                    var res = items.Data.AsQueryable();
                    var lang = session.Language;
                    foreach (var item in res)
                    {
                        var newRecord = new HrLeaveFilterDto
                        {
                            Id = item.Id,
                            EmpCode = item.EmpCode,
                            EmpName = lang == 1 ? item.EmpName : item.EmpName2,
                            BranchName = lang == 1 ? item.BraName : item.BraName2,
                            DepName = lang == 1 ? item.DepName : item.DepName2,
                            LocationName = lang == 1 ? item.LocationName : item.LocationName2,
                            LeaveDate = item.LeaveDate,
                            TypeName = lang == 1 ? item.TypeName : item.TypeName2,
                            WorkYear = item.WorkYear,
                            LastWorkingDay = item.LastWorkingDay,
                            PayrollCode = item.PayrollCode
                        };
                        resultList.Add(newRecord);
                    }
                    var paginatedData = new PaginatedResult<object>
                    {
                        Succeeded = items.Succeeded,
                        Data = resultList,
                        Status = items.Status,
                        PaginationInfo = items.PaginationInfo
                    };
                    return Ok(paginatedData);

                }
                return Ok(resultList);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var chk = await permission.HasPermission(263, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await hrServiceManager.HrLeaveService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete Hr End Of Service  Controller, MESSAGE: {ex.Message}"));
            }
        }

        #endregion


        #region Add Page
        [HttpPost("Add")]
        public async Task<ActionResult> Add(HrLeaveAddDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(263, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var add = await hrServiceManager.HrLeaveService.AddNewLeave(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add Hr End Of Service  Controller, MESSAGE: {ex.Message}"));
            }
        }



        [HttpPost("GetData")]
        public async Task<IActionResult> GetData(HrLeaveGetDataDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(263, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (obj.DDLLeaveType <= 0)
                {
                    return Ok(await Result<object>.FailAsync("يجب ادخال سبب انهاء الخدمة"));
                }
                if (obj.DDLLeaveType2 <= 0)
                {
                    return Ok(await Result<object>.FailAsync("يجب ادخال  السبب"));
                }
                if (string.IsNullOrEmpty(obj.LastworkingDay))
                {
                    return Ok(await Result<object>.FailAsync("يجب ادخال  تاريخ اخر يوم عمل"));
                }
                if (string.IsNullOrEmpty(obj.EmpCode))
                {
                    return Ok(await Result<object>.FailAsync("يجب ادخال رقم الموظف"));
                }
                var getData = await hrServiceManager.HrLeaveService.GetEmployeeLeaveData(obj);
                return Ok(getData);


            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        #endregion


        #region Edit Page

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(HrLeaveEditDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(263, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result<HrLeaveEditDto>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var update = await hrServiceManager.HrLeaveService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrLeaveEditDto>.FailAsync($"====== Exp in Hr End Of Service Controller getById, MESSAGE: {ex.Message}"));
            }
        }


        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long Id)
        {
            try
            {
                var hasPermission = await permission.HasPermission(263, PermissionType.Edit);
                if (!hasPermission)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id <= 0)
                    return Ok(await Result.FailAsync(localization.GetMessagesResource("NoIdInUpdate")));

                var item = await hrServiceManager.HrLeaveService.GetOneVW(x => x.Id == Id);
                if (!item.Succeeded)
                    return Ok(await Result<object>.FailAsync(item.Status.message));

                var result = mapper.Map<HrLeaveGetByIdDto>(item.Data);

                var isEmployeeInBranch = await hrServiceManager.HrEmployeeService.CHeckEmpInBranch(item.Data.EmpId);
                if (!isEmployeeInBranch.Succeeded || isEmployeeInBranch.Data == false)
                    return Ok(await Result<object>.FailAsync(isEmployeeInBranch.Status.message));
                result = new HrLeaveGetByIdDto
                {
                    EmpName = item.Data.EmpName,
                    EmpName2 = item.Data.EmpName2,
                    EmpCode = item.Data.EmpCode,
                    EmpId = item.Data.EmpId,
                    Doappointment = item.Data.Doappointment,
                    NationalityId = item.Data.NationalityId,
                    NationalityName = item.Data.NationalityName,
                    NationalityName2 = item.Data.NationalityName2,
                    CatName = item.Data.CatName,
                    CatName2 = item.Data.CatName2,
                    Location = item.Data.Location,
                    LocationId = item.Data.LocationId,
                    LocationName = item.Data.LocationName,
                    LocationName2 = item.Data.LocationName2,
                    DepId = item.Data.DepId,
                    DepName = item.Data.DepName,
                    DepName2 = item.Data.DepName2,
                    BasicSalary = item.Data.BasicSalary,
                    Housing = item.Data.Housing,
                    Allowances = item.Data.Allowances,
                    Deduction = item.Data.Deduction,
                    TotalSalary = item.Data.BasicSalary + item.Data.Housing + item.Data.Allowances,
                    NetSalary = item.Data.BasicSalary + item.Data.Housing + item.Data.Allowances - item.Data.Deduction,
                    Iban = item.Data.Iban,
                    BankName = item.Data.BankName,
                    SalaryC = Math.Round(item.Data.SalaryC ?? 0, 2),
                    AllowanceC = Math.Round(item.Data.AllowanceC ?? 0, 2),
                    HousingC = Math.Round(item.Data.HousingC ?? 0, 2),
                    Gosi = item.Data.Gosi,
                    Loan = item.Data.Loan,
                    Delay = item.Data.Delay,
                    DelayCnt = item.Data.DelayCnt,
                    Absence = item.Data.Absence,
                    AbsenceCnt = item.Data.AbsenceCnt,
                    Penalties = item.Data.Penalties,
                    DedHousing = item.Data.DedHousing,
                    DedOhad = item.Data.DedOhad,
                    LastSalaryDate = item.Data.LastSalaryDate,
                    VacationBalance = item.Data.VacationBalance,
                    VacationBalanceAmount = Math.Round(item.Data.VacationBalanceAmount ?? 0, 2),
                    OtherAllowance = item.Data.OtherAllowance,
                    TickDueTotal = Math.Round(item.Data.TickDueTotal ?? 0, 2),
                    TickDueCnt = item.Data.TickDueCnt,
                    TickDueAmount = item.Data.TickDueAmount,
                    TotalAllowance = item.Data.TotalAllowance,
                    TotalDeduction = item.Data.TotalDeduction,
                    Net = item.Data.Net,
                    Note = item.Data.Note,
                    LeaveDate = item.Data.LeaveDate,
                    LeaveType = item.Data.LeaveType,
                    WorkYear = item.Data.WorkYear,
                    WorkMonth = item.Data.WorkMonth,
                    WorkDays = item.Data.WorkDays,
                    MdInsurance = item.Data.MdInsurance,
                    OtherDeduction = item.Data.OtherDeduction,
                    Bounce = item.Data.Bounce,
                    EndServiceBenefits = item.Data.EndServiceBenefits,
                    EndServiceIndemnity = item.Data.EndServiceIndemnity,
                    HaveBankLoan = item.Data.HaveBankLoan,
                    CountDayWork = item.Data.CountDayWork,
                    BankId = item.Data.BankId,
                    AccountNo = item.Data.AccountNo,
                    LeaveType2 = item.Data.LeaveType2,
                    BranchId = item.Data.BranchId,
                    BraName = item.Data.BraName,
                    BraName2 = item.Data.BraName2,
                    LastWorkingDay = item.Data.LastWorkingDay,
                    Id = item.Data.Id,
                    DeptId = item.Data.DeptId,
                    OtherAllowanceNote = item.Data.OtherAllowanceNote,
                    OtherDeductionNote = item.Data.OtherDeductionNote,
                    ProvEndServesAmount = item.Data.ProvEndServesAmount,
                    NetProvision = item.Data.NetProvision,
                    HaveCustody = item.Data.HaveCustody,

                };

                var hrLeaveAllowanceDeduction = await hrServiceManager.HrLeaveAllowanceDeductionService.GetAllVW(x => x.LeaveId == Id && x.IsDeleted == false && x.EmpId == item.Data.EmpId);
                if (hrLeaveAllowanceDeduction.Succeeded && hrLeaveAllowanceDeduction.Data != null)
                {
                    result.HrLeaveAllowanceVwDto = hrLeaveAllowanceDeduction.Data.Select(x => new HrLeaveAllowanceVwDto
                    {
                        Id = x.Id,
                        TypeId = x.TypeId,
                        AdId = x.AdId,
                        Rate = x.Rate,
                        Amount = x.Amount,
                        Name = x.Name,
                        NewAmount = x.NewAmount,
                        LeaveId = x.LeaveId,
                        EmpId = x.EmpId,
                        FixedOrTemporary = x.FixedOrTemporary,
                        IsDeleted = false,
                        IsNew = false
                    }).ToList();
                }
                else
                {
                    result.HrLeaveAllowanceVwDto = new List<HrLeaveAllowanceVwDto>();
                }

                int Housing_ID = 0;
                var hrSetting = await hrServiceManager.HrSettingService.GetAll(x => x.FacilityId == session.FacilityId);
                foreach (var setting in hrSetting.Data)
                {
                    Housing_ID = setting.HousingAllowance ?? 0;
                }

                foreach (var row in hrLeaveAllowanceDeduction.Data)
                {
                    if (row.AdId == Housing_ID)
                    {
                        result.HousingC = row.NewAmount;
                    }
                }

                result.fileDtos = new List<SaveFileDto>();
                var getFiles = await mainServiceManager.SysFileService.GetFilesForUser(Id, 56);
                result.fileDtos = getFiles.Data ?? new List<SaveFileDto>();
                if (item.Data.PayrollId > 0)
                {
                    var payroll = await hrServiceManager.HrPayrollService.GetOne(x => x.MsId == item.Data.PayrollId && x.IsDeleted == false);
                    if (payroll.Data != null)
                    {
                        var message = $"{localization.GetMessagesResource("EndOfServiceTransferMessage")}" + " <a href=/Apps/HR/Payroll/Payroll_Edit?MS_ID=" + item.Data.PayrollId + ">" + payroll.Data.MsCode + "</a> ";
                        return Ok(await Result<object>.SuccessAsync(new { Data = result, Message = message, MsCode = payroll.Data.MsCode }));
                    }
                }

                return Ok(await Result<object>.SuccessAsync(new { Data = result, Message = string.Empty, MsCode = 0 }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, await Result<object>.FailAsync($"An error occurred: {ex.Message}"));
            }
        }


        #endregion


        #region Other Operations
        [HttpGet("DDLLeaveType")]
        public async Task<IActionResult> DDLLeaveType(int parent = 0)
        {
            try
            {
                if (parent == 0)
                {
                    var list = await ddlHelper.GetAnyLis<HrLeaveTypeVw, int>(b => b.IsDeleted == false && b.ParentId == 0, "TypeId", session.Language == 2 ? "TypeName2" : "TypeName");
                    return Ok(await Result<SelectList>.SuccessAsync(list));
                }
                else
                {
                    var list = await ddlHelper.GetAnyLis<HrLeaveTypeVw, int>(b => b.IsDeleted == false && b.ParentId == parent, "TypeId", session.Language == 2 ? "TypeName2" : "TypeName");
                    return Ok(await Result<SelectList>.SuccessAsync(list));
                }

            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpPost("PayrollTransfer")]
        public async Task<IActionResult> PayrollTransfer(List<int> leaveIds)
        {
            try
            {
                var chk = await permission.HasPermission(263, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!leaveIds.Any() || leaveIds.Count() <= 0)
                {
                    return Ok(await Result.AccessDenied(localization.GetMessagesResource("DoCheckFirst")));
                }
                if (leaveIds.Any(x => string.IsNullOrEmpty(x.ToString())))
                {
                    return Ok(await Result.FailAsync(localization.GetMessagesResource("InvalidLeaveId")));
                }

                var transfer = await hrServiceManager.HrLeaveService.PayrollTransfer(leaveIds);
                return Ok(transfer);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Payroll Transfer Hr End Of Service  Controller, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("PayrollTransferFromEdit")]
        public async Task<IActionResult> PayrollTransferFromEdit(HrLeaveEditDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(263, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (obj.Id == 0)
                {
                    return Ok(await Result.AccessDenied(localization.GetMessagesResource("NoIdInUpdate")));
                }

                var transfer = await hrServiceManager.HrLeaveService.PayrollTransferFromEdit(obj);
                return Ok(transfer);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Payroll Transfer Hr End Of Service  Controller, MESSAGE: {ex.Message}"));
            }
        }

        #endregion


    }
}