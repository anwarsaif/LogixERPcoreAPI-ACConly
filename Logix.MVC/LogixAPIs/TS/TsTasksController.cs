using DevExpress.CodeParser;
using Logix.Application.Common;
using Logix.Application.DTOs.ACC;
using Logix.Application.DTOs.PUR;
using Logix.Application.DTOs.TS;
using Logix.Application.Helpers;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Services;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Logix.MVC.LogixAPIs.TS
{
    public class TsTasksController : BaseTsApiController
    {
        private readonly ITsServiceManager tsServiceManager;
        private readonly IMainServiceManager mainServiceManager;
        private readonly ICurrentData session;
        private readonly IPermissionHelper permission;
        private readonly ILocalizationService localization;

        public TsTasksController(
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
        #region "GetAll - Search"
        /// <summary>
        /// استرجاع جميع العناصر غير المحذوفة من قاعدة البيانات.
        /// </summary>
        /// <returns>
        /// <see cref="IActionResult"/> يحتوي على قائمة بالعناصر أو رسالة رفض الوصول في حالة عدم توفر الإذن.
        /// </returns>
        /// <remarks>
        /// <b>وصف الوظيفة:</b>  
        /// - تقوم هذه الطريقة بجلب جميع العناصر غير المحذوفة من الخدمة `TsTaskService`.
        /// - تتطلب تحققًا من صلاحية المستخدم لإظهار البيانات.
        ///
        /// <b>المخرجات:</b>  
        /// - إذا كان المستخدم يملك الإذن: قائمة العناصر.  
        /// - إذا لم يملك الإذن: رسالة رفض وصول (`AccessDenied`).  
        ///
        /// <b>الملاحظات:</b>  
        /// - تعتمد على الصلاحيات التي يتم التحقق منها عبر خدمة `permission`.
        /// - تُرجع رسالة خطأ إذا حدث استثناء أثناء التنفيذ.
        /// </remarks>
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var chk = await permission.HasPermission(243, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var items = await tsServiceManager.TsTaskService.GetAll(x=>x.Isdel == false);

                return Ok(await Result<object>.SuccessAsync(items, $"Search Completed {items.Data.Count()}", 200));
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        [HttpPost("Search")]
        /// <summary>
        /// البحث عن المهام باستخدام عوامل تصفية متعددة مع دعم التصفح الصفحي.
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
        ///   - العنوان.
        ///   - الحالة.
        ///   - رقم العميل. 
        ///   - رقم العميل. 
        ///   - رقم الموظف.
        ///   - اسم الموظف.
        ///   - اسم العميل.   
        ///   - التواريخ المرسلة.  
        ///   - التواريخ المستحقة.  
        ///   - تصنيف المهمة
        ///   - النوع.
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
        public async Task<IActionResult> Search(TsTasksVwFilterDto request)
        {
            try
            {
                var chk = await permission.HasPermission(243, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

				//var filter = request.Filter;

				//var userId = filter.AssigneeToUserId.Split(',');

				//// إعداد الفلاتر الافتراضية إذا لم يتم تحديدها.
				//filter.ProjectCode ??= 0;
				//filter.StatusId ??= 0;
				//filter.ClassificationId ??= 0;
				//filter.ParentId ??= 0;
				//filter.ProjectPlansId ??= 0;

				//// جلب البيانات وتطبيق الفلاتر.
				//var emp = await mainServiceManager.SysUserService.GetAll(x => x.Isdel == false
				////&& x.UserFullname.Contains(filter.UserFullname)
				////&& x.UserFullname.Contains(filter.UserFullname) && userId.Contains(x.Id.ToString())
				//);
				//var items = await tsServiceManager.TsTaskService.GetAllVW(x => x.Isdel ==false
				//&& x.FacilityId == session.FacilityId
				//&& (string.IsNullOrEmpty(filter.Subject) || x.Subject.Contains(filter.Subject))
				//&& (filter.ProjectCode == 0 || filter.ProjectCode == x.ProjectCode)
				//&& (string.IsNullOrEmpty(filter.ProjectName) || x.ProjectName.Contains(filter.ProjectName))
				//&& (string.IsNullOrEmpty(filter.CustomerCode) || filter.CustomerCode == x.CustomerCode) 
				//&& (string.IsNullOrEmpty(filter.CustomerName) || x.CustomerName.Contains(filter.CustomerName))
				//&& (string.IsNullOrEmpty(filter.EmpCode) || (emp.Data.Where(x => x.EmpCode.Contains(filter.EmpCode)).Any()))
				//&& (string.IsNullOrEmpty(filter.EmpName) || (emp.Data.Where(x => x.EmpName.Contains(filter.EmpName)).Any()))
				//&& (filter.StatusId == 0 || filter.StatusId == x.StatusId)
				//&& (filter.ClassificationId == 0 || filter.ClassificationId == x.ClassificationId)
				//&& (filter.ParentId == 0 ? x.ParentId == 0 : ( filter.ParentId != -1 ? x.ParentId != 0 : true))
				//&& (filter.ProjectPlansId == 0 || filter.ProjectPlansId == x.ProjectPlansId)
				//&& (string.IsNullOrEmpty(filter.ItemCode) || filter.ItemCode == x.ItemCode)
				//&& (string.IsNullOrEmpty(filter.ReferenceCode) || filter.ReferenceCode == x.ReferenceCode)
				//);

				//if (!items.Succeeded || items.Data == null)
				//{
				//    return Ok(await Result<object>.FailAsync("Failed to retrieve data or no data available."));
				//}

				//var res = items.Data.AsQueryable();
				//if (items.Succeeded)
				//{
				//    if (!string.IsNullOrEmpty(filter.SendDate1))
				//    {
				//        DateTime Fromdate = DateTime.ParseExact(filter.SendDate1, "yyyy/MM/dd", CultureInfo.InvariantCulture);
				//        res = res.Where(s => string.IsNullOrEmpty(s.SendDate) || DateTime.ParseExact(s.SendDate, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= Fromdate);
				//    }

				//    if (!string.IsNullOrEmpty(filter.SendDate2))
				//    {
				//        DateTime Todate = DateTime.ParseExact(filter.SendDate2, "yyyy/MM/dd", CultureInfo.InvariantCulture);
				//        res = res.Where(s => string.IsNullOrEmpty(s.SendDate) || DateTime.ParseExact(s.SendDate, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= Todate);
				//    }

				//    if (!string.IsNullOrEmpty(filter.DueDate1))
				//    {
				//        DateTime DueFrom = DateTime.ParseExact(filter.DueDate1, "yyyy/MM/dd", CultureInfo.InvariantCulture);
				//        res = res.Where(s => string.IsNullOrEmpty(s.DueDate) || DateTime.ParseExact(s.DueDate, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= DueFrom);
				//    }

				//    if (!string.IsNullOrEmpty(filter.DueDate2))
				//    {
				//        DateTime DueTo = DateTime.ParseExact(filter.DueDate2, "yyyy/MM/dd", CultureInfo.InvariantCulture);
				//        res = res.Where(s => string.IsNullOrEmpty(s.DueDate) || DateTime.ParseExact(s.DueDate, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= DueTo);
				//    }
				//}

				//if (!items.Succeeded)
				//    return Ok(await Result<object>.FailAsync(items.Status.message));

				//var currentDate = Bahsas.HDateNow3(session);
				//var resultsList = res.ToList();

				//// معالجة البيانات في الذاكرة
				//var filteredResults = resultsList
				//    .Where(s => !string.IsNullOrEmpty(s.SendDate) && !string.IsNullOrEmpty(s.DueDate))
				//    .Select(s =>
				//    {

				//        return new
				//        {
				//            s.Id,
				//            s.Subject,
				//            s.ClassificationName,
				//            s.SendDate,
				//            s.DueDate,
				//            s.ProjectCode,
				//            s.ProjectName,
				//            s.CustomerName,
				//            s.StatusName,
				//            s.AssigneeToUserName,
				//            s.PriorityName,
				//            s.UserFullname,
				//            s.ColorValue,
				//            CntDay = Bahsas.DateDiff_Day(s.SendDate, s.DueDate),
				//            RemainingDays = Bahsas.DateDiff_Day(currentDate, s.SendDate),
				//        };
				//    }).ToList();

				//if (!items.Succeeded)
				//    return Ok(await Result<object>.FailAsync(items.Status.message));

				//if (!items.Data.Any())
				//    return Ok(await Result<object>.SuccessAsync(new List<object>(), localization.GetResource1("NosearchResult")));
				//// اذا طلب التصفح الصفحي
				//if (request.PageNumber > 0 && request.PageSize > 0)
				//{
				//    var paginatedData = await PaginatedResult<object>.SuccessAsync(
				//        filteredResults.Cast<object>(),
				//        request.PageNumber,
				//        request.PageSize);
				//    return Ok(paginatedData);
				//}

				//// اذا لم يطلب التصفح الصفحي
				//return Ok(await Result<object>.SuccessAsync(filteredResults.ToList(), $"Search Completed {filteredResults.Count()}", 200));

				var items = await tsServiceManager.TsTaskService.Search(request);
				return Ok(items);
			}
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"Error in {nameof(Search)}: {ex.Message}"));
            }
        }
        #endregion "GetAll - Search"

        #region "Add - Edit"
        /// <summary>
        /// تسجيل مهمة جديد.
        /// إضافة مهمة جديدة إلى النظام.
        /// </summary>
        /// <param name="obj">
        /// كائن يحتوي على بيانات المهمة الجديدة.
        /// </param>
        /// <returns>
        /// <see cref="IActionResult"/> يحتوي على نتيجة عملية الإضافة أو رسالة رفض الوصول إذا لم يكن لدى المستخدم الصلاحية.
        /// </returns>
        /// <remarks>
        /// <b>وصف الوظيفة:</b>  
        /// - تقوم هذه الطريقة بإضافة مهمة جديدة باستخدام البيانات المرسلة من الكائن `TsTaskDto`.
        /// - تتطلب تحققًا من صلاحية المستخدم لإضافة المهمة.
        /// - تتحقق من صحة البيانات قبل تنفيذ الإضافة.
        ///
        /// <b>المخرجات:</b>  
        /// - إذا كانت البيانات صالحة ولدى المستخدم الصلاحية: يتم إضافة المهمة وإرجاع النتيجة.  
        /// - إذا لم يكن لدى المستخدم صلاحية الإضافة أو كانت البيانات غير صالحة: يتم إرجاع رسالة خطأ.
        /// </remarks>
        [HttpPost("Add")]
        public async Task<IActionResult> Add(TsTaskDto obj)
        {
            try
            {
                if (!(await permission.HasPermission(243, PermissionType.Add)) && !(await permission.HasPermission(956, PermissionType.Add)))
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync(localization.GetMessagesResource("dataRequire")));

                var add = await tsServiceManager.TsTaskService.AddTask(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add, MESSAGE: {ex.Message}"));
            }
        }

        /// <summary>
        /// تعديل مهمة موجودة في النظام.
        /// </summary>
        /// <param name="obj">
        /// كائن يحتوي على بيانات المهمة المعدلة.
        /// </param>
        /// <returns>
        /// <see cref="IActionResult"/> يحتوي على نتيجة عملية التعديل أو رسالة رفض الوصول إذا لم يكن لدى المستخدم الصلاحية.
        /// </returns>
        /// <remarks>
        /// <b>وصف الوظيفة:</b>  
        /// - تقوم هذه الطريقة بتعديل مهمة موجودة باستخدام البيانات المرسلة من الكائن `TsMainTaskEditDto`.
        /// - تتطلب تحققًا من صلاحية المستخدم لتعديل المهمة.
        /// - تتحقق من صحة البيانات قبل تنفيذ التعديل.
        ///
        /// <b>المخرجات:</b>  
        /// - إذا كانت البيانات صالحة ولدى المستخدم الصلاحية: يتم تعديل المهمة وإرجاع النتيجة.  
        /// - إذا لم يكن لدى المستخدم صلاحية التعديل أو كانت البيانات غير صالحة: يتم إرجاع رسالة خطأ.
        /// </remarks>
        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(TsMainTaskEditDto obj)
        {
            if (!(await permission.HasPermission(243, PermissionType.Edit) && await permission.HasPermission(243, PermissionType.Show)))
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<TsMainTaskEditDto>.FailAsync(localization.GetMessagesResource("dataRequire")));
                }

                var Edit = await tsServiceManager.TsTaskService.UpdateTask(obj);
                return Ok(Edit);
            }
            catch (Exception ex)
            {
                return Ok(await Result<TsMainTaskEditDto>.FailAsync($"======= Exp in edit: {ex.Message}"));
            }
        }
        #endregion "Add - Edit"

        #region "Delete"
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long Id)
        {
            var chk = await permission.HasPermission(243, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var del = await tsServiceManager.TsTaskService.Remove(Id);
                return Ok(del);
            }
            catch (Exception ex)
            {
                return Ok(await Result<TsTaskDto>.FailAsync($"======= Exp in Delete: {ex.Message}"));
            }
        }
        #endregion "Delete"

        #region "GetByIdForEdit - GetById - GetTasksByStatusId"

        [HttpGet("GetByIdForEdit")]
        public async Task<IActionResult> GetByIdForEdit(long id)
        {
            try
            {
                if (!(await permission.HasPermission(243, PermissionType.Show)) && !(await permission.HasPermission(956, PermissionType.Show)))
                {
                    return Ok(await Result.AccessDenied("AccessDenied"));
                }

                if (id <= 0)
                {
                    return Ok(await Result<TsMainTaskEditDto>.FailAsync(localization.GetMessagesResource("InCorrectId")));
                }

                var getItem = await tsServiceManager.TsTaskService.GetForUpdate<TsMainTaskEditDto>(id);
                if (getItem.Succeeded)
                {
                    return Ok(await Result<TsMainTaskEditDto>.SuccessAsync(getItem.Data, $""));
                }
                else
                {
                    return Ok(getItem);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(long id)
        {
            try
            {
                if (!(await permission.HasPermission(243, PermissionType.Show)) && !(await permission.HasPermission(956, PermissionType.Show)))
                {
                    return Ok(await Result.AccessDenied("AcessDenied"));
                }

                if (id <= 0)
                {
                    return Ok(await Result<TsTaskDto>.FailAsync(localization.GetMessagesResource("NoIdInUpdate")));
                }

                var getItem = await tsServiceManager.TsTaskService.GetOne(s => s.Id == id && s.Isdel == false);
                if (getItem.Succeeded)
                {
                    var obj = getItem.Data;
                    return Ok(await Result<TsTaskDto>.SuccessAsync(obj, $""));
                }
                else
                {
                    return Ok(getItem);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<TsTaskDto>.FailAsync($"====== Exp MESSAGE: {ex.Message}"));
            }
        }
        [HttpGet("GetTasksByStatusId")]
        public async Task<IActionResult> GetTasksByStatusId(long id)
        {
            try
            {
                if (!(await permission.HasPermission(243, PermissionType.Show)) && !(await permission.HasPermission(956, PermissionType.Show)))
                {
                    return Ok(await Result.AccessDenied("AccessDenied"));
                }

                if (id <= 0)
                {
                    return Ok(await Result<TsTaskDto>.FailAsync(localization.GetMessagesResource("InCorrectId")));
                }

                var getItem = await tsServiceManager.TsTaskService.GetAll(x => x.StatusId == id && x.Isdel == false);
                if (getItem.Succeeded)
                {
                    var obj = getItem.Data;
                    return Ok(await Result<List<TsTaskDto>>.SuccessAsync(obj.ToList(), $""));
                }
                else
                {
                    return Ok(getItem);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<List<TsTaskDto>>.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }

        #endregion "GetByIdForEdit - GetById - GetTasksByStatusId"
    }
}
