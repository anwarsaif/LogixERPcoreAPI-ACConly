using DevExpress.Pdf.Xmp;
using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Helpers;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Reflection.Emit;

namespace Logix.MVC.LogixAPIs.HR
{
    // التراخيص
    public class HRLicensesApiController : BaseHrApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        public HRLicensesApiController(IHrServiceManager hrServiceManager, IMainServiceManager mainServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization)
        {
            this.mainServiceManager = mainServiceManager;
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var chk = await permission.HasPermission(491, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var items = await hrServiceManager.HrLicenseService.GetAll(e => e.IsDeleted == false);
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    res = res.OrderBy(e => e.Id);
                    return Ok(await Result<List<HrLicenseDto>>.SuccessAsync(res.ToList(), items.Status.message));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrLicensesFilterDto filter)
        {
            var chk = await permission.HasPermission(491, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var items = await hrServiceManager.HrLicenseService.GetAllVW(e => e.IsDeleted == false && e.StatusId == 1);
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    if (filter == null)
                    {
                        return Ok(items);
                    }

                    if (!string.IsNullOrEmpty(filter.LicenseNo))
                    {
                        res = res.Where(c => (c.LicenseNo != null && c.LicenseNo.Contains(filter.LicenseNo)));
                    }
                    if (!string.IsNullOrEmpty(filter.EmpName))
                    {
                        res = res.Where(c => (c.EmpName != null && c.EmpName.Contains(filter.EmpName)));
                    }
                    if (!string.IsNullOrEmpty(filter.EmpCode))
                    {
                        res = res.Where(s => s.EmpCode != null && s.EmpCode.Equals(filter.EmpCode));
                    }
                    if (filter.LicenseType > 0)
                    {
                        res = res.Where(s => s.LicenseType != null && s.LicenseType.Equals(filter.LicenseType));
                    }
                    if (!string.IsNullOrEmpty(filter.IssuedDate))
                    {
                        res = res.Where(c => (c.IssuedDate != null && c.IssuedDate.Contains(filter.IssuedDate)));
                    }
                    if (!string.IsNullOrEmpty(filter.ExpiryDate))
                    {
                        res = res.Where(c => (c.ExpiryDate != null && c.ExpiryDate.Contains(filter.ExpiryDate)));
                    }

                    res = res.OrderBy(e => e.Id);
                    var final = res.ToList();
                    return Ok(await Result<List<HrLicensesVw>>.SuccessAsync(final, ""));
                }

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpPost("GetPagination")]
        public async Task<IActionResult> GetPagination([FromBody] HrLicensesFilterDto filter, int take = 5, long? lastSeenId = null)
        {
            var chk = await permission.HasPermission(491, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                filter.LicenseType ??= 0;
                var BranchesList = session.Branches.Split(',');

                var dateConditions = new List<DateCondition>
                {
                    new() {
                        DatePropertyName = "IssuedDate",
                        ComparisonOperator = ComparisonOperator.GreaterThanOrEqual,
                        StartDateString = filter.IssuedDate ?? ""
                    },
                    new() {
                        DatePropertyName = "ExpiryDate",
                        ComparisonOperator = ComparisonOperator.LessThanOrEqual,
                        StartDateString = filter.ExpiryDate ?? ""
                    }
                };
                var result = await hrServiceManager.HrLicenseService.GetAllWithPaginationVW(selector: x => x.Id,
                    expression: e => e.IsDeleted == false
                    && (string.IsNullOrEmpty(filter.EmpCode) || e.EmpCode.Contains(filter.EmpCode))
                    && (string.IsNullOrEmpty(filter.EmpName) || e.EmpName.Contains(filter.EmpName))
                    && (string.IsNullOrEmpty(filter.LicenseNo) || e.LicenseNo.Contains(filter.LicenseNo))
                    && (filter.LicenseType == 0 || e.LicenseType == filter.LicenseType)
                && BranchesList.Contains(e.BranchId.ToString())
                && (e.StatusId == 1),
                    take: take,
                    lastSeenId: lastSeenId,
                   dateConditions: (string.IsNullOrEmpty(filter.IssuedDate) || string.IsNullOrEmpty(filter.ExpiryDate)) ? null : dateConditions);

                if (!result.Succeeded)
                    return StatusCode(result.Status.code, result.Status.message);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync(ex.Message));

            }
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add(HrLicenseDto obj)
        {
            var chk = await permission.HasPermission(491, PermissionType.Add);
            if (!chk)
            {
                return Ok(await Result.AccessDenied(localization.GetMessagesResource("AccessDenied")));
            }

            if (string.IsNullOrEmpty(obj.EmpCode))
                return Ok(await Result<object>.FailAsync(localization.GetResource1("EmployeeIsNumber")));

            if (!string.IsNullOrEmpty(obj.ExpiryDate))
            {
                var checkDate = await DateHelper.CheckDate(obj.ExpiryDate, session.FacilityId, session.CalendarType);
                if(checkDate == false)
                    return Ok(await Result<HrLicenseDto>.FailAsync(localization.GetMessagesResource("InvalidLicenseExpiryDate")));
            }
                
            if (obj.JobCat <= 0)
            {
                return Ok(await Result<HrLicenseDto>.FailAsync(localization.GetMessagesResource("PleaseChooseJobCategory")));
            }
            if (obj.LicenseType <= 0)
            {
                return Ok(await Result<HrLicenseDto>.FailAsync(localization.GetMessagesResource("PleaseChooseLicenseNumber")));
            }
            if (!string.IsNullOrEmpty(obj.FileUrl))
            {
                var ext = Path.GetExtension(obj.FileUrl)?.ToLower().TrimStart('.');

                var allowedExtensions = await mainServiceManager.SysPropertyValueService.GetByProperty(223, session.FacilityId);
                if (!allowedExtensions.Data.PropertyValue.Split(",").Contains(ext))
                    return Ok(await Result<HrLicenseDto>.FailAsync(localization.GetMessagesResource("InvalidFileExtension")));
            }
            try
            {
                var addRes = await hrServiceManager.HrLicenseService.Add(obj);

                return Ok(addRes);
            }

            catch (Exception ex)
            {
                return Ok(await Result<HrLicenseDto>.FailAsync(ex.Message));
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(long Id = 0)
        {
            var chk = await permission.HasPermission(491, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied(localization.GetMessagesResource("AccessDenied")));
            }
            if (Id <= 0)
            {
                return Ok(Result<HrLicenseEditDto>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
            }

            try
            {
                var getItem = await hrServiceManager.HrLicenseService.GetForUpdate<HrLicenseEditDto>(Id);
                if (getItem.Succeeded && getItem.Data != null)
                {
                    if (getItem.Data.EmpId == null)
                    {
                        return Ok(await Result<object>.FailAsync(localization.GetResource1("EmployeeNotFoundInList")));
                    }
                    var emp = await mainServiceManager.InvestEmployeeService.GetById(getItem.Data.EmpId??0);
                    if (emp == null)
                    {
                        return Ok(await Result<object>.FailAsync(localization.GetResource1("EmployeeNotFoundInList")));
                    }

                    getItem.Data.EmpName = emp.Data.EmpName;
                    getItem.Data.EmpCode = emp.Data.EmpId;
                    return Ok(getItem);
                }
                return Ok(Result<HrLicenseEditDto>.FailAsync(getItem.Status.message));
            }
            catch (Exception exp)
            {
                return Ok(Result<HrLicenseEditDto>.FailAsync($"EXP in {this.GetType()}, Meesage: {exp.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(HrLicenseEditDto obj)
        {
            var chk = await permission.HasPermission(491, PermissionType.Edit);

            if (!ModelState.IsValid)
            {
                return Ok(obj);
            }

            if (!chk)
            {
                return Ok(await Result.AccessDenied(localization.GetMessagesResource("AccessDenied")));
            }

            if (string.IsNullOrEmpty(obj.EmpCode))
                return Ok(await Result<object>.FailAsync(localization.GetResource1("EmployeeIsNumber")));

            if (!string.IsNullOrEmpty(obj.ExpiryDate))
            {
                var checkDate = await DateHelper.CheckDate(obj.ExpiryDate, session.FacilityId, session.CalendarType);
                if (checkDate == false)
                    return Ok(await Result<HrLicenseDto>.FailAsync(localization.GetMessagesResource("InvalidLicenseExpiryDate")));
            }
            if (obj.JobCat <= 0)
            {
                return Ok(await Result<HrLicenseDto>.FailAsync(localization.GetMessagesResource("PleaseChooseJobCategory")));
            }
            if (obj.LicenseType <= 0)
            {
                return Ok(await Result<HrLicenseDto>.FailAsync(localization.GetMessagesResource("PleaseChooseLicenseNumber")));
            }
            if (!string.IsNullOrEmpty(obj.FileUrl))
            {
                var ext = Path.GetExtension(obj.FileUrl)?.ToLower().TrimStart('.');

                var allowedExtensions = await mainServiceManager.SysPropertyValueService.GetByProperty(223, session.FacilityId);
                if (!allowedExtensions.Data.PropertyValue.Split(",").Contains(ext))
                    return Ok(await Result<HrLicenseDto>.FailAsync(localization.GetMessagesResource("InvalidFileExtension")));
            }

            try
            {
                var addRes = await hrServiceManager.HrLicenseService.Update(obj);

                return Ok(addRes);
            }

            catch (Exception ex)
            {
                return Ok(await Result<HrLicenseEditDto>.FailAsync($"{ex.Message}"));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long Id = 0)
        {
            var chk = await permission.HasPermission(491, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (Id == 0)
            {
                return Ok(await Result.FailAsync("Please choose an entity to delete it, there is no id passed"));
            }

            try
            {
                var del = await hrServiceManager.HrLicenseService.Remove(Id);
                return Ok(del);
            }
            catch (Exception exp)
            {
                return Ok(await Result.FailAsync($"{exp.Message}"));
            }
        }


    }
}