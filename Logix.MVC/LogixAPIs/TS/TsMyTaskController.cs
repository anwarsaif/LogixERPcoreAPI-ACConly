using DevExpress.Xpo;
using Logix.Application.Common;
using Logix.Application.DTOs.PUR;
using Logix.Application.DTOs.TS;
using Logix.Application.Helpers;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Services;
using Logix.Application.Wrapper;
using Logix.Domain.TS;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;

namespace Logix.MVC.LogixAPIs.TS
{
    public class TsMyTaskController : BaseTsApiController
    {
        private readonly ITsServiceManager tsServiceManager;
        private readonly IMainServiceManager mainServiceManager;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;

        public TsMyTaskController(
            ITsServiceManager tsServiceManager,
            IMainServiceManager mainServiceManager,
            ICurrentData session,
            ILocalizationService localization
            )
        {
            this.tsServiceManager = tsServiceManager;
            this.mainServiceManager = mainServiceManager;
            this.session = session;
            this.localization = localization;
        }
        #region "Search"
      
        [HttpPost("Search")]
        public async Task<IActionResult> Search(PaginatedRequest<TsTasksVwFilterDto> request)
        {
            try
            {
                var filter = request.Filter;

                // تعيين القيم الافتراضية للفلاتر
                filter.ProjectCode ??= 0;
                filter.StatusId ??= 0;
                filter.ClassificationId ??= 0;
                filter.ParentId ??= 0;

                // استرجاع المستخدمين
                var users = await mainServiceManager.SysUserService.GetAll(x => x.Isdel == false);

                // استرجاع العناصر بناءً على الفلاتر
                var items = await tsServiceManager.TsTaskService.GetAllVW(x =>
                    x.Isdel==false &&
                    x.UserId == session.UserId &&
                    x.FacilityId == session.FacilityId &&
                    (string.IsNullOrEmpty(filter.Subject) || x.Subject.Contains(filter.Subject)) &&
                    (filter.ProjectCode == 0 || filter.ProjectCode == x.ProjectCode) &&
                    (string.IsNullOrEmpty(filter.ProjectName) || x.ProjectName.Contains(filter.ProjectName)) &&
                    (string.IsNullOrEmpty(filter.CustomerCode) || filter.CustomerCode == x.CustomerCode) &&
                    (string.IsNullOrEmpty(filter.CustomerName) || x.CustomerName.Contains(filter.CustomerName)) &&
                    (string.IsNullOrEmpty(filter.UserFullname) || x.UserFullname.Contains(filter.UserFullname)) &&
                    (filter.StatusId == 0 || filter.StatusId == x.StatusId) &&
                    (filter.ClassificationId == 0 || filter.ClassificationId == x.ClassificationId) &&
                    (filter.ParentId == 0 ? x.ParentId == 0 : filter.ParentId != -1 ? x.ParentId != 0 : true)
                );

                // تحقق من نجاح الاستعلام
                if (!items.Succeeded || items.Data == null)
                    return Ok(await Result<object>.FailAsync("Failed to retrieve data or no data available."));

                // تطبيق التصفية الإضافية
                var results = items.Data.AsQueryable();

                // تصفية التواريخ بناءً على الفلاتر
                if (!string.IsNullOrEmpty(filter.SendDate1))
                {
                    var fromDate = DateTime.ParseExact(filter.SendDate1, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                    results = results.Where(s => string.IsNullOrEmpty(s.SendDate) || DateTime.ParseExact(s.SendDate, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= fromDate);
                }

                if (!string.IsNullOrEmpty(filter.SendDate2))
                {
                    var toDate = DateTime.ParseExact(filter.SendDate2, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                    results = results.Where(s => string.IsNullOrEmpty(s.SendDate) || DateTime.ParseExact(s.SendDate, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= toDate);
                }

                if (!string.IsNullOrEmpty(filter.DueDate1))
                {
                    var dueFrom = DateTime.ParseExact(filter.DueDate1, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                    results = results.Where(s => string.IsNullOrEmpty(s.DueDate) || DateTime.ParseExact(s.DueDate, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= dueFrom);
                }

                if (!string.IsNullOrEmpty(filter.DueDate2))
                {
                    var dueTo = DateTime.ParseExact(filter.DueDate2, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                    results = results.Where(s => string.IsNullOrEmpty(s.DueDate) || DateTime.ParseExact(s.DueDate, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= dueTo);
                }

                // حساب النتائج المفلترة
                var currentDate = Bahsas.HDateNow3(session);
                // تحميل البيانات أولاً
                var resultsList = results.ToList();

                // معالجة البيانات في الذاكرة
var filteredResults = resultsList
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
            s.ColorValue,
            CntDay = Bahsas.DateDiff_Day(s.SendDate, s.DueDate),
            RemainingDays = Bahsas.DateDiff_Day(currentDate, s.SendDate),
        };
    }).ToList();


                // تحقق من وجود نتائج
                if (!filteredResults.Any())
                    return Ok(await Result<object>.SuccessAsync(new List<object>(), "No search results found."));

                // إذا كان هناك طلب للترقيم
                if (request.PageNumber > 0 && request.PageSize > 0)
                {
                    var paginatedData = await PaginatedResult<object>.SuccessAsync(
                        filteredResults.Cast<object>(),
                        request.PageNumber,
                        request.PageSize
                    );
                    return Ok(paginatedData);
                }

                // استجابة غير مرقمة
                return Ok(await Result<object>.SuccessAsync(filteredResults, $"Search completed with {filteredResults.Count} results."));
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"Error in {nameof(Search)}: {ex.Message}"));
            }
        }
        #endregion "Search"
    }
}
