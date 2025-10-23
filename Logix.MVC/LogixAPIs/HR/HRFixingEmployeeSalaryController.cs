using DevExpress.CodeParser;
using Humanizer.Localisation;
using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Helpers;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Globalization;
using Result = Logix.Application.Wrapper.Result;

namespace Logix.MVC.LogixAPIs.HR
{
    // تثبيت راتب موظف
    public class HRFixingEmployeeSalaryController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly ISysConfigurationHelper sysConfigurationHelper;
        private readonly IMainServiceManager mainServiceManager;

        public HRFixingEmployeeSalaryController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, ISysConfigurationHelper sysConfigurationHelper, IMainServiceManager mainServiceManager)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.sysConfigurationHelper = sysConfigurationHelper;
            this.mainServiceManager = mainServiceManager;
        }

        #region IndexPage


        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrFixingEmployeeSalaryFilterDto filter)
        {
            var chk = await permission.HasPermission(2044, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var BranchesList = session.Branches.Split(',');
                long Emp_ID = 0;
                if (!string.IsNullOrEmpty(filter.empCode))
                {
                    var empData = await mainServiceManager.InvestEmployeeService.GetOne(x => x.EmpId == filter.empCode && x.Isdel == false);
                    if (empData == null || empData.Data == null)
                    {
                        return Ok(await Result.FailAsync(localization.GetResource1("EmployeeNotFound")));
                    }
                    else
                    {
                        Emp_ID = empData.Data.Id ?? 0;
                    }
                }

                filter.Status ??= 0;
                filter.FixingType ??= 0;
                filter.SentTo ??= 0;
                var items = await hrServiceManager.HrFixingEmployeeSalaryService.GetAllVW(e => e.IsDeleted == false &&
                (Emp_ID == 0 || e.EmpId == Emp_ID) &&
                (filter.Status == 0 || e.Status == filter.Status) &&
                (filter.FixingType == 0 || e.FixingType == filter.FixingType) &&
                (filter.SentTo == 0 || filter.SentTo == Convert.ToInt32(e.TheFixingSentTo)));
                if (items.Succeeded)
                {
                    if (items.Data.Count() > 0)
                    {
                        var res = items.Data.AsQueryable();
                        if (!string.IsNullOrEmpty(filter.FixingDate))
                        {

                            DateTime fixingDate;
                            bool isValidDate = DateTime.TryParseExact(filter.FixingDate, "yyyy/MM/dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out fixingDate);


                            if (!isValidDate)
                            {
                                return Ok(await Result<List<HrFixingEmployeeSalaryVw>>.FailAsync(localization.GetMessagesResource("InvalidDateFormat")));
                            }

                            res = res.Where(r => r.FixingDate != null &&
                                DateTime.ParseExact(r.FixingDate, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= fixingDate &&
                                DateTime.ParseExact(r.FixingDate, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= fixingDate
                            );
                        }
                        if (res.Any())
                            return Ok(await Result<List<HrFixingEmployeeSalaryVw>>.SuccessAsync(res.ToList(), ""));
                        return Ok(await Result<List<HrFixingEmployeeSalaryVw>>.SuccessAsync(res.ToList(), localization.GetResource1("NosearchResult")));
                    }
                    return Ok(await Result<List<HrFixingEmployeeSalaryVw>>.SuccessAsync(items.Data.ToList(), localization.GetResource1("NosearchResult")));
                }
                return Ok(await Result<HrFixingEmployeeSalaryVw>.FailAsync(items.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync(ex.Message));
            }
        }


        [HttpPost("GetPagination")]
        public async Task<IActionResult> GetPagination([FromBody] HrFixingEmployeeSalaryFilterDto filter, int take = 5, long? lastSeenId = null)
        {
            try
            {
                var chk = await permission.HasPermission(2044, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                long Emp_ID = 0;
                if (!string.IsNullOrEmpty(filter.empCode))
                {
                    var empData = await mainServiceManager.InvestEmployeeService.GetOne(x => x.EmpId == filter.empCode && x.Isdel == false);
                    if (empData == null || empData.Data == null)
                    {
                        return Ok(await Result.FailAsync(localization.GetResource1("EmployeeNotFound")));
                    }
                    else
                    {
                        Emp_ID = empData.Data.Id ?? 0;
                    }
                }

                var BranchesList = session.Branches.Split(',');
                filter.Status ??= 0;
                filter.FixingType ??= 0;
                filter.SentTo ??= 0;

                var dateConditions = new List<DateCondition>();
                dateConditions.Add(new DateCondition
                {
                    DatePropertyName = "FixingDate",
                    ComparisonOperator = ComparisonOperator.GreaterThanOrEqual,
                    StartDateString = filter.FixingDate ?? ""
                });
                dateConditions.Add(new DateCondition
                {
                    DatePropertyName = "FixingDate",
                    ComparisonOperator = ComparisonOperator.LessThanOrEqual,
                    StartDateString = filter.FixingDate ?? ""
                });

                var items = await hrServiceManager.HrFixingEmployeeSalaryService.GetAllWithPaginationVW(selector: e => e.Id,
                expression: e => e.IsDeleted == false &&
                (Emp_ID == 0 || e.EmpId == Emp_ID) &&
                (filter.Status == 0 || e.Status == filter.Status) &&
                (filter.FixingType == 0 || e.FixingType == filter.FixingType) &&
                (filter.SentTo == 0 || filter.SentTo == Convert.ToInt32(e.TheFixingSentTo)),
                    take: take,
                    lastSeenId: lastSeenId,
                   dateConditions: (string.IsNullOrEmpty(filter.FixingDate) || string.IsNullOrEmpty(filter.FixingDate)) ? null : dateConditions);

                if (!items.Succeeded)
                    return Ok(await Result<List<HrFixingEmployeeSalaryVw>>.FailAsync(items.Status.message));

                var res = items.Data.AsQueryable();
                var lang = session.Language;
                var paginatedData = new PaginatedResult<object>
                {
                    Succeeded = items.Succeeded,
                    Data = items.Data,
                    Status = items.Status,
                    PaginationInfo = items.PaginationInfo
                };
                return Ok(paginatedData);

            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long Id)
        {
            try
            {
                var chk = await permission.HasPermission(2044, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));
                var delete = await hrServiceManager.HrFixingEmployeeSalaryService.Remove(Id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete HRFixingEmployeeSalaryController, MESSAGE: {ex.Message}"));
            }
        }


        #endregion


        #region AddPage Business

        [HttpPost("Add")]
        public async Task<ActionResult> Add(HrFixingEmployeeSalaryDto obj)
        {
            try
            {
                var CurrentDate = DateTime.Now.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);
                var chk = await permission.HasPermission(2044, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result<string>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));
                if (obj.Status <= 0)
                    return Ok(await Result<string>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));
                if (obj.FixingType <= 0)
                    return Ok(await Result<string>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));
                if (obj.SentTo <= 0)
                    return Ok(await Result<string>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));
                if (string.IsNullOrEmpty(obj.empCode))
                    return Ok(await Result<string>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));
                if (string.IsNullOrEmpty(obj.FixingDate))
                    return Ok(await Result<string>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var add = await hrServiceManager.HrFixingEmployeeSalaryService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"====== Exp in Add    HRFixingEmployeeSalaryController  Add Method, MESSAGE: {ex.Message}"));
            }
        }

        #endregion




        #region EditPage


        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long Id)
        {
            try
            {
                var chk = await permission.HasPermission(2044, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id <= 0)
                {
                    return Ok(await Result<object>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var item = await hrServiceManager.HrFixingEmployeeSalaryService.GetOneVW(x => x.IsDeleted == false && x.Isdel == false && x.Id == Id);
                var files = await mainServiceManager.SysFileService.GetAll(x => x.IsDeleted == false && x.TableId == 130 && x.PrimaryKey == Id);

                return Ok(await Result<object>.SuccessAsync(new { data = item.Data, fileDtos = files.Data.ToList() }));
                //return Ok(item);

            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"====== Exp in HR HRFixingEmployeeSalaryController  getById, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(HrFixingEmployeeSalaryEditDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(2044, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result<object>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));
                if (obj.Status <= 0)
                    return Ok(await Result<string>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));
                if (obj.FixingType <= 0)
                    return Ok(await Result<string>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));
                if (obj.SentTo <= 0)
                    return Ok(await Result<string>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));
                if (string.IsNullOrEmpty(obj.empCode))
                    return Ok(await Result<string>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));
                if (string.IsNullOrEmpty(obj.FixingDate))
                    return Ok(await Result<string>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));


                var update = await hrServiceManager.HrFixingEmployeeSalaryService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"====== Exp in Edit HR Holiday  Controller, MESSAGE: {ex.Message}"));
            }
        }


        #endregion
    }
}