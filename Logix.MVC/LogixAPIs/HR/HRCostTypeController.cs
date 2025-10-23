using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Result = Logix.Application.Wrapper.Result;

namespace Logix.MVC.LogixAPIs.HR
{
    // أنواع التكاليف للموظفين 
    public class HRCostTypeController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;


        public HRCostTypeController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
        }


        [HttpPost("Search")]

        public async Task<IActionResult> Search([FromBody] HrCostTypeFilterDto filter)
        {
            var chk = await permission.HasPermission(1145, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                // تعيين قيم افتراضية إذا كانت null
                filter.TypeId = filter.TypeId == 0 ? 0 : filter.TypeId;
                filter.TypeCalculation = filter.TypeCalculation == 0 ? 0 : filter.TypeCalculation;

                // تطبيق الفلاتر داخل الـ LINQ expression نفسه (كما في HrNotificationsType)
                var items = await hrServiceManager.HrCostTypeService.GetAllVW(e =>
                    e.IsDeleted == false &&
                    (filter.TypeId == 0 || e.TypeId == filter.TypeId) &&
                    (string.IsNullOrEmpty(filter.TypeName) || (e.TypeName != null && e.TypeName.Contains(filter.TypeName))) &&
                    (string.IsNullOrEmpty(filter.TypeNameEn) || (e.TypeNameEn != null && e.TypeNameEn.Contains(filter.TypeNameEn))) &&
                    (filter.TypeCalculation == 0 || e.TypeCalculation == filter.TypeCalculation)
                );

                if (!items.Succeeded)
                    return Ok(await Result<List<HrCostTypeVw>>.FailAsync(items.Status.message));

                if (items.Data == null || !items.Data.Any())
                    return Ok(await Result<List<HrCostTypeVw>>.SuccessAsync(new List<HrCostTypeVw>(), localization.GetResource1("NosearchResult")));

                // ترتيب النتائج (اختياري لتحسين التناسق)
                var res = items.Data.OrderBy(x => x.Id).ToList();

                // إرجاع النتيجة بنجاح
                return Ok(await Result<List<HrCostTypeVw>>.SuccessAsync(res, ""));
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrCostTypeVw>.FailAsync(ex.Message));
            }
        }

        [HttpPost("GetPagination")]
        public async Task<IActionResult> GetPagination([FromBody] HrCostTypeFilterDto filter, int take = Pagination.take, int? lastSeenId = null)
        {
            var chk = await permission.HasPermission(1145, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                // لضمان أن القيم الافتراضية لا تسبب أخطاء في الفلترة
                filter.TypeId = filter.TypeId == 0 ? 0 : filter.TypeId;
                filter.TypeCalculation = filter.TypeCalculation == 0 ? 0 : filter.TypeCalculation;

                // جلب البيانات مع pagination مباشرة من الخدمة
                var items = await hrServiceManager.HrCostTypeService.GetAllWithPaginationVW(
                    selector: e => e.Id,
                    expression: e =>
                        e.IsDeleted == false &&
                        (filter.TypeId == 0 || e.TypeId == filter.TypeId) &&
                        (string.IsNullOrEmpty(filter.TypeName) || (e.TypeName != null && e.TypeName.Contains(filter.TypeName))) &&
                        (string.IsNullOrEmpty(filter.TypeNameEn) || (e.TypeNameEn != null && e.TypeNameEn.Contains(filter.TypeNameEn))) &&
                        (filter.TypeCalculation == 0 || e.TypeCalculation == filter.TypeCalculation),
                    take: take,
                    lastSeenId: lastSeenId
                );

                // التحقق من نجاح العملية
                if (!items.Succeeded)
                    return Ok(await Result<List<HrCostTypeVw>>.FailAsync(items.Status.message));

                // التحقق من وجود بيانات
                if (items.Data == null || !items.Data.Any())
                    return Ok(await Result<List<HrCostTypeVw>>.SuccessAsync(new List<HrCostTypeVw>()));

                // ترتيب النتائج حسب Id
                var res = items.Data.OrderBy(x => x.Id).ToList();

                // إعداد نتيجة التجزئة (pagination)
                var paginatedData = new PaginatedResult<object>
                {
                    Succeeded = items.Succeeded,
                    Data = res,
                    Status = items.Status,
                    PaginationInfo = items.PaginationInfo
                };

                return Ok(paginatedData);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }


        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var chk = await permission.HasPermission(1145, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await hrServiceManager.HrCostTypeService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete HR Attendance TimeTable Shift Controller, MESSAGE: {ex.Message}"));
            }
        }


        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long Id)
        {
            try
            {
                var chk = await permission.HasPermission(1145, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var item = await hrServiceManager.HrCostTypeService.GetOne(x => x.IsDeleted == false && x.Id == Id);
                return Ok(item);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrCostTypeEditDto>.FailAsync($"====== Exp in cost type getById, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(HrCostTypeEditDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(1145, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (obj.Id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));

                if (string.IsNullOrEmpty(obj.TypeName))
                    return Ok(await Result<HrCostTypeDto>.FailAsync($"{localization.GetPUResource("ExpenseArName")}"));

                if (string.IsNullOrEmpty(obj.TypeNameEn))
                    return Ok(await Result<HrCostTypeDto>.FailAsync($"{localization.GetPUResource("ExpenseEnName")}"));

                if (obj.TypeId <= 0)
                    return Ok(await Result<HrCostTypeDto>.FailAsync($"{localization.GetPUResource("ExpenseType")}"));

                if (obj.TypeNationality <= 0)
                    return Ok(await Result<HrCostTypeDto>.FailAsync($"{localization.GetHrResource("TheseExpensesApplyTo")}"));

                if (obj.TypeCalculation <= 0)
                    return Ok(await Result<HrCostTypeDto>.FailAsync($"{localization.GetPUResource("CalculationMethod")}"));



                var update = await hrServiceManager.HrCostTypeService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrCostTypeEditDto>.FailAsync($"====== Exp in Edit HR  Cost Type  Controller, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Add")]
        public async Task<ActionResult> Add(HrCostTypeDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(1145, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (string.IsNullOrEmpty(obj.TypeName))
                    return Ok(await Result<HrCostTypeDto>.FailAsync($"{localization.GetPUResource("ExpenseArName")}"));

                if (string.IsNullOrEmpty(obj.TypeNameEn))
                    return Ok(await Result<HrCostTypeDto>.FailAsync($"{localization.GetPUResource("ExpenseEnName")}"));

                if (obj.TypeId <= 0)
                    return Ok(await Result<HrCostTypeDto>.FailAsync($"{localization.GetPUResource("ExpenseType")}"));

                if (obj.TypeNationality <= 0)
                    return Ok(await Result<HrCostTypeDto>.FailAsync($"{localization.GetHrResource("TheseExpensesApplyTo")}"));

                if (obj.TypeCalculation <= 0)
                    return Ok(await Result<HrCostTypeDto>.FailAsync($"{localization.GetPUResource("CalculationMethod")}"));


                var add = await hrServiceManager.HrCostTypeService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrCostTypeDto>.FailAsync($"====== Exp in Add HR  Cost Type Controller  Add Method, MESSAGE: {ex.Message}"));
            }
        }
    }
}