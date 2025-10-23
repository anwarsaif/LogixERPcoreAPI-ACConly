using Logix.Application.Common;
using Logix.Application.DTOs.TS;
using Logix.Application.Helpers;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Logix.MVC.LogixAPIs.TS
{
    public class TsTasksByUserController : BaseTsApiController
    {
        private readonly ITsServiceManager tsServiceManager;
        private readonly IMainServiceManager mainServiceManager;
        private readonly ICurrentData session;
        private readonly IPermissionHelper permission;
        private readonly ILocalizationService localization;

        public TsTasksByUserController(
            ITsServiceManager tsServiceManager,
            IMainServiceManager mainServiceManager,
            ICurrentData session,
            IPermissionHelper permission,
            ILocalizationService localization
            )
        {
            this.tsServiceManager = tsServiceManager;
            this.mainServiceManager = mainServiceManager;
            this.session = session;
            this.permission = permission;
            this.localization = localization;
        }
        #region "Search"
        /// <summary>
        /// البحث عن المهام الصادرة باستخدام عوامل تصفية متعددة مع دعم التصفح الصفحي.
        /// </summary>
        /// <param name="request">
        /// <see cref="PaginatedRequest{TsTasksVwFilterDto}"/> يحتوي على الفلاتر المستخدمة لعملية البحث بالإضافة إلى إعدادات التصفح الصفحي.
        /// </param>
        /// <returns>
        /// <see cref="IActionResult"/> يحتوي على قائمة النتائج التي تطابق الفلاتر أو رسالة رفض الوصول إذا لم تتوفر الصلاحية.
        /// </returns>
        /// <remarks>
        /// <b>وصف الوظيفة:</b>  
        /// - تقوم هذه الطريقة بتنفيذ بحث ديناميكي على المهام باستخدام فلاتر متعددة مثل:  
        ///   - اسم المشروع.  
        ///   - الكود التعريفي للعميل.  
        ///   - التواريخ المرسلة.  
        ///   - التواريخ المستحقة.  
        /// - تدعم التصفح الصفحي إذا تم طلب ذلك.
        ///
        /// <b>المخرجات:</b>  
        /// - قائمة بالمهام التي تطابق الفلاتر.
        /// - رسالة رفض وصول إذا لم يكن للمستخدم صلاحية البحث.
        ///
        /// <b>الملاحظات:</b>  
        /// - يعتمد البحث على التحقق من الصلاحيات باستخدام خدمة `permission`.  
        /// - تدعم الطريقة فلاتر ديناميكية يمكن تخصيصها حسب الطلب.  
        /// - تحتوي على معالجة للتواريخ لتصفية النتائج بناءً على نطاق زمني محدد.
        /// </remarks>
        [HttpPost("Search")]
        public async Task<IActionResult> Search(PaginatedRequest<TsTasksVwFilterDto> request)
        {
            try
            {
                var chk = await permission.HasPermission(956, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var filter = request.Filter;

                var userId = filter.AssigneeToUserId.Split(',');

                filter.ProjectCode ??= 0;
                filter.StatusId ??= 0;
                filter.ClassificationId ??= 0;
                filter.ParentId ??= 0;
                filter.ProjectPlansId ??= 0;
                var emp = await mainServiceManager.SysUserService.GetAll(x => x.Isdel == false
                && x.UserFullname.Contains(filter.UserFullname) && userId.Contains(x.Id.ToString())
                );
                var items = await tsServiceManager.TsTaskService.GetAllVW(x => x.Isdel == false
                && x.UserId == session.UserId
                && (string.IsNullOrEmpty(filter.Subject) || x.Subject.Contains(filter.Subject))
                && (filter.ProjectCode == 0 || filter.ProjectCode == x.ProjectCode)
                && (string.IsNullOrEmpty(filter.ProjectName) || x.ProjectName.Contains(filter.ProjectName))
                && (string.IsNullOrEmpty(filter.CustomerName) || x.CustomerName.Contains(filter.CustomerName))
                && (filter.StatusId == 0 || filter.StatusId == x.StatusId)
                && (filter.ClassificationId == 0 || filter.ClassificationId == x.ClassificationId)
                && (filter.ParentId == 0 ? x.ParentId == 0 : filter.ParentId != -1 ? x.ParentId != 0 : true)
                && (string.IsNullOrEmpty(filter.EmpCode) || (emp.Data.Where(x => x.EmpCode.Contains(filter.EmpCode)).Any()))
                && (string.IsNullOrEmpty(filter.EmpName) || (emp.Data.Where(x => x.EmpName.Contains(filter.EmpName)).Any()))
                && (filter.ProjectPlansId == 0 || x.ProjectPlansId == filter.ProjectPlansId)
                && (string.IsNullOrEmpty(filter.ItemCode) || x.ItemCode.Contains(filter.ItemCode))
                );

                if (!items.Succeeded || items.Data == null)
                {
                    return Ok(await Result<object>.FailAsync("Failed to retrieve data or no data available."));
                }

                var res = items.Data.AsQueryable();
                if (items.Succeeded)
                {
                    //if((string.IsNullOrEmpty(filter.EmpCode) || 
                    //    (emp.Data.Where(x=>x.EmpCode.Contains(filter.EmpCode)).Any())) && 
                    //    (string.IsNullOrEmpty(filter.EmpName) || 
                    //    (emp.Data.Where(x => x.EmpName.Contains(filter.EmpName)).Any())))
                    //{

                    //}
                    if (!string.IsNullOrEmpty(filter.SendDate1))
                    {
                        DateTime Fromdate = DateTime.ParseExact(filter.SendDate1, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                        res = res.Where(s => string.IsNullOrEmpty(s.SendDate) || DateTime.ParseExact(s.SendDate, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= Fromdate);
                    }

                    if (!string.IsNullOrEmpty(filter.SendDate2))
                    {
                        DateTime Todate = DateTime.ParseExact(filter.SendDate2, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                        res = res.Where(s => string.IsNullOrEmpty(s.SendDate) || DateTime.ParseExact(s.SendDate, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= Todate);
                    }

                    if (!string.IsNullOrEmpty(filter.DueDate1))
                    {
                        DateTime DueFrom = DateTime.ParseExact(filter.DueDate1, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                        res = res.Where(s => string.IsNullOrEmpty(s.DueDate) || DateTime.ParseExact(s.DueDate, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= DueFrom);
                    }

                    if (!string.IsNullOrEmpty(filter.DueDate2))
                    {
                        DateTime DueTo = DateTime.ParseExact(filter.DueDate2, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                        res = res.Where(s => string.IsNullOrEmpty(s.DueDate) || DateTime.ParseExact(s.DueDate, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= DueTo);
                    }
                }

                if (!items.Succeeded)
                    return Ok(await Result<object>.FailAsync(items.Status.message));

                var currentDate = Bahsas.HDateNow3(session);
                var filteredResults = res.Select(s => new
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
                    s.ColorValue
                    //CntDay = (DateTime.ParseExact(s.DueDate, "yyyy/MM/dd", CultureInfo.InvariantCulture)
                    //            - DateTime.ParseExact(s.SendDate, "yyyy/MM/dd", CultureInfo.InvariantCulture)).Days,
                    //RemainingDays = (DateTime.ParseExact(currentDate, "yyyy/MM/dd", CultureInfo.InvariantCulture)
                    //            - DateTime.ParseExact(s.SendDate, "yyyy/MM/dd", CultureInfo.InvariantCulture)).Days
                }).ToList();

                if (!items.Succeeded)
                    return Ok(await Result<object>.FailAsync(items.Status.message));

                if (!items.Data.Any())
                    return Ok(await Result<object>.SuccessAsync(new List<object>(), localization.GetResource1("NosearchResult")));
                // If pagination is requested
                if (request.PageNumber > 0 && request.PageSize > 0)
                {
                    // Use the updated SuccessAsync method for paginated result
                    var paginatedData = await PaginatedResult<object>.SuccessAsync(
                        filteredResults.Cast<object>(),
                        request.PageNumber,
                        request.PageSize);
                    return Ok(paginatedData);
                }

                // Non-paginated response
                return Ok(await Result<object>.SuccessAsync(filteredResults.ToList(), $"Search Completed {filteredResults.Count()}", 200));
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"Error in {nameof(Search)}: {ex.Message}"));
            }
        }
        #endregion "Search"

    }
}
