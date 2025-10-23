using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.Infrastructure.Repositories;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.HR
{
    //  تقرير الحضور والانصراف التفصيلي
    public class HRAttendanceReport3Controller : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;

        public HRAttendanceReport3Controller(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
        }

        [HttpPost("GetAttendanceData")]
        public async Task<IActionResult> GetAttendanceData(HRAttendanceReport4FilterDto filter)
        {
            var chk = await permission.HasPermission(886, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                //var BranchesList = session.Branches.Split(',');

                //if (string.IsNullOrEmpty(filter.EmpCode))
                //{
                //    filter.EmpCode = null;
                //}
                //if (string.IsNullOrEmpty(filter.EmpName))
                //{
                //    filter.EmpName = null;
                //}

                //if (filter.BranchID != null && filter.BranchID > 0)
                //{
                //    filter.BranchsID = null;

                //}

                //else
                //{
                //    filter.BranchID = 0;
                //    filter.BranchsID = session.Branches;
                //}


                //if (filter.DeptID == 0 || filter.DeptID == null)
                //{
                //    filter.DeptID = 0;
                //}

                //if (filter.StatusID <= 0 || filter.StatusID == null)
                //{
                //    filter.StatusID = 0;
                //}
                //if (filter.Location <= 0 || filter.Location == null)
                //{
                //    filter.Location = 0;
                //}

                //if (filter.AttendanceType <= 0 || filter.AttendanceType == null)
                //{
                //    filter.AttendanceType = 0;
                //}
                //if (!string.IsNullOrEmpty(filter.DayDateGregorian) && !string.IsNullOrEmpty(filter.DayDateGregorian2))
                //{
                //    filter.DayDateGregorian = filter.DayDateGregorian;
                //    filter.DayDateGregorian2 = filter.DayDateGregorian2;
                //}
                //else
                //{
                //    filter.DayDateGregorian = null;
                //    filter.DayDateGregorian2 = null;
                //}

                //if (filter.SponsorsID == 0 || filter.SponsorsID == null)
                //{
                //    filter.SponsorsID = 0;
                //}
                //filter.ManagerID = 0;
                //filter.TimeTableID = 0;
                //filter.ShitID = 0;
                var items = await hrServiceManager.HrAttendanceReport3Service.GetAttendanceData(filter); // Assuming GetAtt() returns a List<HRAttendanceReport4Dto>
                if (items.Succeeded)
                {
                    if (items.Data.Count() > 0)
                    {
                        return Ok(await Result<List<HRAttendanceReport4Dto>>.SuccessAsync(items.Data.ToList(), ""));
                    }
                    return Ok(await Result<List<HRAttendanceReport4Dto>>.SuccessAsync(items.Data.ToList(), localization.GetResource1("NosearchResult")));
                }
                return Ok(await Result<HRAttendanceReport4Dto>.FailAsync(items.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result<HRAttendanceReport4Dto>.FailAsync(ex.Message));
            }
        }


        [HttpPost("GetAbsenceData")]
        public async Task<IActionResult> GetAbsenceData(HRAttendanceReport4FilterDto filter)
        {
            var chk = await permission.HasPermission(886, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var BranchesList = session.Branches.Split(',');
                var items = await hrServiceManager.HrAbsenceService.GetAllVW(x => x.IsDeleted == false
                && (string.IsNullOrEmpty(filter.EmpCode) || x.EmpCode == filter.EmpCode)
                && (string.IsNullOrEmpty(filter.EmpName) || x.EmpName.ToLower().Contains(filter.EmpName.ToLower()))
                && (BranchesList.Contains(x.BranchId.ToString()))
                && (filter.DeptID == null || filter.DeptID == 0 || x.DeptId == filter.DeptID)
                && (filter.Location == null || filter.Location == 0 || x.Location == filter.Location)
                );
                if (items.Succeeded)
                {
                    if (items.Data.Count() > 0)
                    {

                        var res = items.Data.AsQueryable();
                        if (filter.BranchID != null && filter.BranchID > 0)
                        {
                            res = res.Where(c => c.BranchId != null && c.BranchId == filter.BranchID);
                        }
                        if (!string.IsNullOrEmpty(filter.DayDateGregorian) && !string.IsNullOrEmpty(filter.DayDateGregorian2))
                        {
                            res = res.Where(r => r.AbsenceDate != null &&
                            (DateHelper.StringToDate(r.AbsenceDate) >= DateHelper.StringToDate(filter.DayDateGregorian)) &&
                           (DateHelper.StringToDate(r.AbsenceDate) <= DateHelper.StringToDate(filter.DayDateGregorian2))
                           );
                        }


                        if (res.Count() > 0)
                            return Ok(await Result<IQueryable<HrAbsenceVw>>.SuccessAsync(res, ""));
                        return Ok(await Result<IQueryable<HrAbsenceVw>>.SuccessAsync(res, localization.GetResource1("NosearchResult")));
                    }
                    return Ok(await Result<IEnumerable<HrAbsenceVw>>.SuccessAsync(items.Data, localization.GetResource1("NosearchResult")));


                }
                return Ok(await Result<HrAbsenceVw>.FailAsync(items.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrVacationsVw>.FailAsync(ex.Message));
            }
        }


        [HttpPost("GetVacationData")]
        public async Task<IActionResult> GetVacationData(HRAttendanceReport4FilterDto filter)
        {
            var chk = await permission.HasPermission(886, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var BranchesList = session.Branches.Split(',');
                var items = await hrServiceManager.HrVacationsService.GetAllVW(x => x.IsDeleted == false && (string.IsNullOrEmpty(filter.EmpCode) || x.EmpCode == filter.EmpCode) &&
                (string.IsNullOrEmpty(filter.EmpName) || x.EmpName.ToLower().Contains(filter.EmpName.ToLower()))
                && (BranchesList.Contains(x.BranchId.ToString()))
                && (filter.DeptID == null || filter.DeptID == 0 || x.DeptId == filter.DeptID)
                && (filter.Location == null || filter.Location == 0 || x.Location == filter.Location)
                );
                if (items.Succeeded)
                {
                    if (items.Data.Count() > 0)
                    {

                        var res = items.Data.AsQueryable();
                        if (filter.BranchID != null && filter.BranchID > 0)
                        {
                            res = res.Where(c => c.BranchId != null && c.BranchId == filter.BranchID);
                        }

                        if (!string.IsNullOrEmpty(filter.DayDateGregorian) && !string.IsNullOrEmpty(filter.DayDateGregorian2))
                        {
                            res = res.Where(r => r.VacationSdate != null && r.VacationSdate != null &&
                            (
                        (
                            (DateHelper.StringToDate(r.VacationSdate) >= DateHelper.StringToDate(filter.DayDateGregorian)) &&
                           (DateHelper.StringToDate(r.VacationSdate) <= DateHelper.StringToDate(filter.DayDateGregorian2))
                        ) ||
                        (
                            (DateHelper.StringToDate(r.VacationEdate) >= DateHelper.StringToDate(filter.DayDateGregorian)) &&
                           (DateHelper.StringToDate(r.VacationEdate) <= DateHelper.StringToDate(filter.DayDateGregorian2))
                        )
                           )

                           );
                        }
                        if (res.Count() > 0)
                            return Ok(await Result<IQueryable<HrVacationsVw>>.SuccessAsync(res, ""));
                        return Ok(await Result<IQueryable<HrVacationsVw>>.SuccessAsync(res, localization.GetResource1("NosearchResult")));
                    }
                    return Ok(await Result<IEnumerable<HrVacationsVw>>.SuccessAsync(items.Data, localization.GetResource1("NosearchResult")));


                }
                return Ok(await Result<HrVacationsVw>.FailAsync(items.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrVacationsVw>.FailAsync(ex.Message));
            }
        }

        [HttpPost("GetPermissionsData")]
        public async Task<IActionResult> GetPermissionsData(HRAttendanceReport4FilterDto filter)
        {
            var chk = await permission.HasPermission(886, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var BranchesList = session.Branches.Split(',');
                var items = await hrServiceManager.HrPermissionService.GetAllVW(x => x.IsDeleted == false
                && (string.IsNullOrEmpty(filter.EmpCode) || x.EmpCode == filter.EmpCode) &&
                (string.IsNullOrEmpty(filter.EmpName) || x.EmpName.ToLower().Contains(filter.EmpName.ToLower()))
                && (BranchesList.Contains(x.BranchId.ToString()))

                );
                if (items.Succeeded)
                {
                    if (items.Data.Count() > 0)
                    {

                        var res = items.Data.AsQueryable();
                        if (filter.BranchID != null && filter.BranchID > 0)
                        {
                            res = res.Where(c => c.BranchId != null && c.BranchId == filter.BranchID);
                        }

                        if (!string.IsNullOrEmpty(filter.DayDateGregorian) && !string.IsNullOrEmpty(filter.DayDateGregorian2))
                        {
                            res = res.Where(r => r.PermissionDate != null &&
                            (DateHelper.StringToDate(r.PermissionDate) >= DateHelper.StringToDate(filter.DayDateGregorian)) &&
                           (DateHelper.StringToDate(r.PermissionDate) <= DateHelper.StringToDate(filter.DayDateGregorian2))
                           );
                        }


                        if (res.Count() > 0)
                            return Ok(await Result<IQueryable<HrPermissionsVw>>.SuccessAsync(res, ""));
                        return Ok(await Result<IQueryable<HrPermissionsVw>>.SuccessAsync(res, localization.GetResource1("NosearchResult")));
                    }
                    return Ok(await Result<IEnumerable<HrPermissionsVw>>.SuccessAsync(items.Data, localization.GetResource1("NosearchResult")));


                }
                return Ok(await Result<HrPermissionsVw>.FailAsync(items.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrPermissionsVw>.FailAsync(ex.Message));
            }
        }


        [HttpPost("GetMandate")]
        public async Task<IActionResult> GetMandate(HRAttendanceReport4FilterDto filter)
        {
            var chk = await permission.HasPermission(886, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var BranchesList = session.Branches.Split(',');
                var items = await hrServiceManager.HrMandateService.GetAllVW(x => x.IsDeleted == false
                && (string.IsNullOrEmpty(filter.EmpCode) || x.EmpCode == filter.EmpCode) &&
                (string.IsNullOrEmpty(filter.EmpName) || x.EmpName.ToLower().Contains(filter.EmpName.ToLower()))
                && (BranchesList.Contains(x.BranchId.ToString()))

                );
                if (items.Succeeded)
                {
                    if (items.Data.Count() > 0)
                    {

                        var res = items.Data.AsQueryable();
                        if (filter.BranchID != null && filter.BranchID > 0)
                        {
                            res = res.Where(c => c.BranchId != null && c.BranchId == filter.BranchID);
                        }

                        if (!string.IsNullOrEmpty(filter.DayDateGregorian) && !string.IsNullOrEmpty(filter.DayDateGregorian2))
                        {
                            var FromDate = DateHelper.StringToDate(filter.DayDateGregorian);
                            var ToDate = DateHelper.StringToDate(filter.DayDateGregorian2);
                            res = res.Where(r => r.FromDate != null &&
                            (DateHelper.StringToDate(r.FromDate) >= FromDate) &&
                           (DateHelper.StringToDate(r.FromDate) <= ToDate)
                           );
                        }


                        if (res.Count() > 0)
                            return Ok(await Result<IQueryable<HrMandateVw>>.SuccessAsync(res, ""));
                        return Ok(await Result<IQueryable<HrMandateVw>>.SuccessAsync(res, localization.GetResource1("NosearchResult")));
                    }
                    return Ok(await Result<IEnumerable<HrMandateVw>>.SuccessAsync(items.Data, localization.GetResource1("NosearchResult")));


                }
                return Ok(await Result<object>.FailAsync(items.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync(ex.Message));
            }
        }

        [HttpPost("GetAssignmen")]
        public async Task<IActionResult> GetAssignmen(HRAttendanceReport4FilterDto filter)
        {
            var chk = await permission.HasPermission(886, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var BranchesList = session.Branches.Split(',');
                var items = await hrServiceManager.HrAssignmenService.GetAllVW(x => x.IsDeleted == false
                && (string.IsNullOrEmpty(filter.EmpCode) || x.EmpCode == filter.EmpCode) &&
                (string.IsNullOrEmpty(filter.EmpName) || x.EmpName.ToLower().Contains(filter.EmpName.ToLower()))
                && (BranchesList.Contains(x.BranchId.ToString()))

                );
                if (items.Succeeded)
                {
                    if (items.Data.Count() > 0)
                    {

                        var res = items.Data.AsQueryable();
                        if (filter.BranchID != null && filter.BranchID > 0)
                        {
                            res = res.Where(c => c.BranchId != null && c.BranchId == filter.BranchID);
                        }

                        if (!string.IsNullOrEmpty(filter.DayDateGregorian) && !string.IsNullOrEmpty(filter.DayDateGregorian2))
                        {
                            var FromDate = DateHelper.StringToDate(filter.DayDateGregorian);
                            var ToDate = DateHelper.StringToDate(filter.DayDateGregorian2);
                            res = res.Where(r => r.FromDate != null &&
                            (DateHelper.StringToDate(r.FromDate) >= FromDate && (DateHelper.StringToDate(r.FromDate) <= ToDate)) ||
                            (DateHelper.StringToDate(r.ToDate) >= FromDate && (DateHelper.StringToDate(r.ToDate) <= ToDate))
                           );
                        }


                        if (res.Count() > 0)
                            return Ok(await Result<IQueryable<HrAssignmenVw>>.SuccessAsync(res, ""));
                        return Ok(await Result<IQueryable<HrAssignmenVw>>.SuccessAsync(res, localization.GetResource1("NosearchResult")));
                    }
                    return Ok(await Result<IEnumerable<HrAssignmenVw>>.SuccessAsync(items.Data, localization.GetResource1("NosearchResult")));


                }
                return Ok(await Result<object>.FailAsync(items.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync(ex.Message));
            }
        }


        /// <summary>
        /// تستخدم الدالة لاضافة اعتماد بيانات لموظف ويشترط فيها ارسال تاريخ الاعتماد وكود الموظف
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>

        [HttpPost("MakeApprove")]
        public async Task<ActionResult> MakeApprove(ApproveDto data)
        {
            try
            {
                var chk = await permission.HasPermission(886, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));
                if (string.IsNullOrEmpty(data.EmpCode))
                {
                    return Ok(await Result.FailAsync($"يجب اختيار موظف وحيد في الفلترة لتتمكن من اتمام العملية"));

                }
                if (string.IsNullOrEmpty(data.ApproveDate))
                {
                    return Ok(await Result.FailAsync($"يجب اختيار تاريخ الاعتماد"));

                }
                var add = await hrServiceManager.HrDelayService.MakeApprove(data.EmpCode, data.ApproveDate, data.HoursMins);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add  HRAttendanceReport3Controller  Controller, MESSAGE: {ex.Message}"));
            }
        }
    }
}