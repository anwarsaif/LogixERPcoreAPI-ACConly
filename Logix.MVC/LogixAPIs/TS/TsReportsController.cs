using DevExpress.CodeParser;
using DocumentFormat.OpenXml.Office2010.Excel;
using Logix.Application.Common;
using Logix.Application.DTOs.TS;
using Logix.Application.Helpers;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.Main;
using Logix.Domain.PUR;
using Logix.Domain.TS;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Logix.MVC.LogixAPIs.TS
{
    public class TsReportsController : BaseTsApiController
    {
        private readonly ITsServiceManager tsServiceManager;
        private readonly IMainServiceManager mainServiceManager;
        private readonly IHrServiceManager hrServiceManager;
        private readonly ICurrentData session;
        private readonly IPermissionHelper permission;
        private readonly ILocalizationService localization;

        public TsReportsController(
            ITsServiceManager tsServiceManager,
            IMainServiceManager mainServiceManager,
            IHrServiceManager hrServiceManager,
            ICurrentData session,
            IPermissionHelper permission,
            ILocalizationService localization
            )
        {
            this.tsServiceManager = tsServiceManager;
            this.mainServiceManager = mainServiceManager;
            this.hrServiceManager = hrServiceManager;
            this.session = session;
            this.permission = permission;
            this.localization = localization;
        }

        #region =====================================  تقرير بالمهام لفريق العمل        

        /// <summary>
        /// استرجاع تقرير عن المهام الموكلة لفريق العمل بناءً على معايير التصفية الخاصة بالمدير المباشر.
        /// </summary>
        /// <param name="request">طلب يحتوي على معايير التصفية والبيانات المطلوبة بشكل مجزأ (Paginated).</param>
        /// <returns>
        /// <see cref="IActionResult"/> يحتوي على قائمة المهام المصفاة (مجزأة إذا تم طلب ذلك) 
        /// أو رسالة خطأ في حال حدوث مشكلة.
        /// </returns>
        /// <remarks>
        /// <b>الصلاحيات المطلوبة:</b> 952 (عرض).  
        /// <b>معايير التصفية:</b> 
        /// - رمز المشروع  
        /// - الحالة  
        /// - الموضوع  
        /// - اسم المشروع  
        /// - اسم العميل  
        /// - فترات التواريخ (تاريخ الإرسال، تاريخ الاستحقاق).  
        /// </remarks>
        
        [HttpPost("SearchTaskByDirectManager")]
        public async Task<IActionResult> SearchTaskByDirectManager(PaginatedRequest<TsTaskRep1FilterDto> request)
        {
            try
            {
                var chk = await permission.HasPermission(952, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var filter = request.Filter;

                filter.ProjectCode ??= 0;
                filter.StatusId ??= 0;
                var items = await tsServiceManager.TsTaskService.GetAllVW(x => x.Isdel == false
                && (string.IsNullOrEmpty(filter.Subject) || (x.Subject != null && x.Subject.Contains(filter.Subject)))
                && (filter.ProjectCode == 0 || x.ProjectCode == filter.ProjectCode)
                && (string.IsNullOrEmpty(filter.ProjectName) || (x.ProjectName != null && x.ProjectName.Contains(filter.ProjectName)))
                && (string.IsNullOrEmpty(filter.CustomerCode) || x.CustomerCode == filter.CustomerCode)
                && (string.IsNullOrEmpty(filter.CustomerName) || (x.CustomerName != null && x.CustomerName.Contains(filter.CustomerName)))
                && (string.IsNullOrEmpty(filter.EmpName) || (x.UserFullname != null && x.UserFullname.Contains(filter.EmpName)))
                && (filter.StatusId == 0 || filter.StatusId == x.StatusId)
                );

                if (!items.Succeeded || items.Data == null)
                {
                    return Ok(await Result<object>.FailAsync("Failed to retrieve data or no data available."));
                }

                var res = items.Data.AsQueryable();
                if (items.Succeeded)
                {
                    if (!string.IsNullOrEmpty(filter.FromDate))
                    {
                        DateTime fromDate = DateTime.ParseExact(filter.FromDate, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                        res = res.Where(s => DateTime.ParseExact(s.SendDate, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= fromDate);
                    }

                    if (!string.IsNullOrEmpty(filter.ToDate))
                    {
                        DateTime toDate = DateTime.ParseExact(filter.ToDate, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                        res = res.Where(s => DateTime.ParseExact(s.SendDate, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= toDate);
                    }

                    if (!string.IsNullOrEmpty(filter.DueFrom))
                    {
                        DateTime dueFrom = DateTime.ParseExact(filter.DueFrom, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                        res = res.Where(s => DateTime.ParseExact(s.DueDate, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= dueFrom);
                    }

                    if (!string.IsNullOrEmpty(filter.DueTo))
                    {
                        DateTime dueTo = DateTime.ParseExact(filter.DueTo, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                        res = res.Where(s => DateTime.ParseExact(s.DueDate, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= dueTo);
                    }
                }

                if (!items.Succeeded)
                    return Ok(await Result<object>.FailAsync(items.Status.message));


                var currentDate = Bahsas.HDateNow3(session);
                
                var HrEmp = await hrServiceManager.HrEmployeeService.GetAll(
                    x => x.ManagerId == session.EmpId && x.Isdel == false && x.IsDeleted == false
                );
               
                // Get the list of HrEmployee IDs
                var hrEmpIds = HrEmp.Data.Select(a => a.Id).ToList();

                // Get the SysUser IDs that match the HrEmployee IDs
                //var UserIds = await mainServiceManager.SysUserService.GetAllVW(
                //    x => x.EmpId.HasValue && x.Isdel == false && x.IsDeleted == false && hrEmpIds.Contains(x.EmpId.Value)
                //);
                var Users = await mainServiceManager.SysUserService.GetAllVW(
                    x => x.Isdel == false && x.IsDeleted == false && hrEmpIds.Contains((long)x.EmpId)
                );

                var UserIds = Users.Data.Select(a => a.UserId).ToList();

                var resultsList = res.ToList();
                // Filter tasks based on AssigneeToUserId matching UserId
                //var tasks = resultsList.Where(task =>
                //!string.IsNullOrEmpty(task.AssigneeToUserId) &&
                //    task.AssigneeToUserId
                //        .Split(',')
                //        .Select(id => Convert.ToInt64(id))
                //        .Any(id => UserIds.Data.Select(u => u.UserId).Contains(id))
                //).ToList();

                var tasks = resultsList.Where(task =>
                !string.IsNullOrEmpty(task.AssigneeToUserId) &&
                    task.AssigneeToUserId
                        .Split(',')
                        .Select(id => Convert.ToInt64(id))
                        .Any(id => UserIds.Contains(id))
                ).ToList();

                // معالجة البيانات في الذاكرة
                var filteredResults = tasks
                    .Where(s => !string.IsNullOrEmpty(s.SendDate) && !string.IsNullOrEmpty(s.DueDate))
                    .Select(s =>
                    {

                        return new
                        {
                            s.Id,
                            s.Subject,
                            s.ClassificationName,
                            s.SendDate,
                            s.DueDate,
                            s.ProjectCode,
                            s.ProjectName,
                            s.CustomerName,
                            s.StatusName,
                            s.AssigneeToUserName,
                            s.PriorityName,
                            s.UserFullname,
                            CntDay = Bahsas.DateDiff_Day(s.SendDate, s.DueDate),
                            RemainingDays = Bahsas.DateDiff_Day(currentDate, s.SendDate),
                            s.ColorValue,
                        };
                    }).OrderByDescending(x => x.DueDate).ToList();
                if (!res.Any())
                    return Ok(await Result<object>.SuccessAsync(new List<object>(), localization.GetResource1("NosearchResult")));

                if (request.PageNumber > 0 && request.PageSize > 0)
                {
                    var paginatedData = await PaginatedResult<object>.SuccessAsync(
                        filteredResults.Cast<object>(),
                        request.PageNumber,
                        request.PageSize);
                    return Ok(paginatedData);
                }

                return Ok(await Result<object>.SuccessAsync(filteredResults.ToList(), $"Search Completed {filteredResults.Count()}", 200));
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"Error in {nameof(SearchTaskByDirectManager)}: {ex.Message}"));
            }
        }

        #endregion

        #region =====================================  تقرير الانجاز       
        /// <summary>
        /// استرجاع تقرير ملخص عن إحصائيات إنجاز المهام للمستخدمين بناءً على معايير التصفية.
        /// </summary>
        /// <param name="request">طلب يحتوي على معايير التصفية والبيانات المطلوبة بشكل مجزأ (Paginated).</param>
        /// <returns>
        /// <see cref="IActionResult"/> يحتوي على إحصائيات المهام (مجزأة إذا تم طلب ذلك) 
        /// أو رسالة خطأ في حال حدوث مشكلة.
        /// </returns>
        /// <remarks>
        /// <b>الصلاحيات المطلوبة:</b> 952 (عرض).  
        /// <b>معايير التصفية:</b> إحصائيات خاصة بالمستخدمين بناءً على المهام المسندة لهم.  
        /// </remarks>
        [HttpPost("SearchSummaryTask")]
        public async Task<IActionResult> SearchSummaryTask(PaginatedRequest<TsTaskRep2FilterDto> request)
        {
            try
            {
                var chk = await permission.HasPermission(610, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var filter = request.Filter;

                var items = await tsServiceManager.TsTaskService.GetUsersTasksStatistics(filter);

                if (!items.Any())
                    return Ok(await Result<object>.SuccessAsync(new List<object>(), localization.GetResource1("NosearchResult")));

                if (request.PageNumber > 0 && request.PageSize > 0)
                {
                    var paginatedData = await PaginatedResult<object>.SuccessAsync(
                        items.Cast<object>(),
                        request.PageNumber,
                        request.PageSize);
                    return Ok(paginatedData);
                }

                return Ok(await Result<object>.SuccessAsync(items.ToList(), $"Search Completed {items.Count()}", 200));
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"Error in {nameof(SearchSummaryTask)}: {ex.Message}"));
            }
        }

        #endregion

        #region =====================================  تقرير بالمهام حسب الحالة        
        /// <summary>
        /// استرجاع تقرير عن المهام للمستخدمين بناءً على تصفية المهام حسب الحالة.
        /// </summary>
        /// <param name="request">طلب يحتوي على معايير التصفية والبيانات المطلوبة بشكل مجزأ (Paginated).</param>
        /// <returns>
        /// <see cref="IActionResult"/> يحتوي على قائمة المهام المصفاة للمستخدمين (مجزأة إذا تم طلب ذلك) 
        /// أو رسالة خطأ في حال حدوث مشكلة.
        /// </returns>
        /// <remarks>
        /// <b>الصلاحيات المطلوبة:</b> 1038 (عرض).  
        /// <b>معايير التصفية:</b> تصفية المهام حسب الحالة أو المستخدمين أو معايير أخرى.  
        /// </remarks>
        [HttpPost("SearchTasksUsers")]
        public async Task<IActionResult> SearchTasksUsers(PaginatedRequest<TsTaskRep2FilterDto> request)
        {
            try
            {
                var chk = await permission.HasPermission(1038, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var filter = request.Filter;

                var items = await tsServiceManager.TsTaskService.GetTasksUsers(filter);
                
                if (!items.Any())
                    return Ok(await Result<object>.SuccessAsync(new List<object>(), localization.GetResource1("NosearchResult")));

                if (request.PageNumber > 0 && request.PageSize > 0)
                {
                    var paginatedData = await PaginatedResult<object>.SuccessAsync(
                        items.Cast<object>(),
                        request.PageNumber,
                        request.PageSize);
                    return Ok(paginatedData);
                }

                return Ok(await Result<object>.SuccessAsync(items.ToList(), $"Search Completed {items.Count()}", 200));
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"Error in {nameof(SearchSummaryTask)}: {ex.Message}"));
            }
        }

        #endregion
    }
}
