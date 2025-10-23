using Logix.Application.Common;
using Logix.Application.DTOs.Integra;
using Logix.Application.DTOs.Main;
using Logix.Application.DTOs.PUR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Services;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.Integra
{
    public class IntegraMappingViewController : BaseIntegraApiController
    {
        private readonly ICurrentData session;
        private readonly IPermissionHelper permission;
        private readonly ILocalizationService localization;
        private readonly IIntegraServiceManager integraServiceManager;

        public IntegraMappingViewController(
            ICurrentData session,
            IPermissionHelper permission,
            ILocalizationService localization,
            IIntegraServiceManager integraServiceManager
            )
        {
            this.session = session;
            this.permission = permission;
            this.localization = localization;
            this.integraServiceManager = integraServiceManager;
        }
        /// <summary>
        /// هذا الملف يحتوي على كل العمليات الخاصة بجدول IntegraTableValue 
        /// 1- استرجاع الكل.
        /// 2- البحث.
        /// 3- إضافة سجل جديد.
        /// 4- تعديل سجل سابق.
        /// 5- حذف سجل سابق.
        /// 6- استرجاع سجل حسب الرقم للتعديل.
        /// 6- استرجاع سجل حسب الرقم.
        /// </summary>
        /// <returns>
        /// <see cref="IActionResult"/> يحتوي على نتيجة عملية الإضافة أو رسالة رفض الوصول إذا لم يكن لدى المستخدم الصلاحية.
        /// </returns>
        /// <remarks>
        /// <b>وصف الوظيفة:</b>  
        /// - تقوم هذه الطريقة بإضافة مهمة جديدة باستخدام البيانات المرسلة من الكائن `IntegraTableValueDto`.
        /// - تتطلب تحققًا من صلاحية المستخدم لإضافة المهمة.
        /// - تتحقق من صحة البيانات قبل تنفيذ الإضافة.
        /// - يجب ان يتم تحديد الجدول والنظام الذي سيتم التعامل مع بياناتهم
        /// <b>المخرجات:</b>  
        /// - إذا كانت البيانات صالحة ولدى المستخدم الصلاحية: يتم إضافة المهمة وإرجاع النتيجة.  
        /// - إذا لم يكن لدى المستخدم صلاحية الإضافة أو كانت البيانات غير صالحة: يتم إرجاع رسالة خطأ.
        /// </remarks>


        #region "GetAll - Search"
        [HttpPost("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var chk = await permission.HasPermission(1629, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var items = await integraServiceManager.IntegraTableValueService.GetAll();

                if (!items.Data.Any())
                    return Ok(await Result<List<IntegraTableValueDto>>.SuccessAsync(localization.GetResource1("NosearchResult")));

                return Ok(await Result<List<IntegraTableValueDto>>.SuccessAsync(items.Data.ToList(), $"Search Completed {items.Data.Count()}", 200));
            }
            catch (Exception ex)
            {
                return Ok(await Result<List<IntegraTableValueDto>>.FailAsync($"Error in {nameof(Search)}: {ex.Message}"));
            }
        }

        [HttpPost("Search")]
        public async Task<IActionResult> Search(IntegraTableValueFilterDto filter)
        {
            try
            {
                var chk = await permission.HasPermission(1629, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                filter.TableId ??= 0;
                filter.IntegraSytstemId ??= 0;
                var items = await integraServiceManager.IntegraTableValueService.GetAll(x =>
                    x.IsDeleted == false &&
                    x.FacilityId == session.FacilityId &&
                    (string.IsNullOrEmpty(filter.FieldValue) || x.FieldValue.Contains(filter.FieldValue)) &&
                    (string.IsNullOrEmpty(filter.FieldReference) || x.FieldReference.Contains(filter.FieldReference)) &&
                    (filter.TableId == 0 || x.TableId == filter.TableId) &&
                    (filter.IntegraSytstemId == 0 || x.IntegraSytstemId == filter.IntegraSytstemId));

                if (!items.Data.Any())
                    return Ok(await Result<List<IntegraTableValueDto>>.SuccessAsync(localization.GetResource1("NosearchResult")));

                return Ok(await Result<List<IntegraTableValueDto>>.SuccessAsync(items.Data.ToList(), $"Search Completed {items.Data.Count()}", 200));
            }
            catch (Exception ex)
            {
                return Ok(await Result<List<IntegraTableValueDto>>.FailAsync($"Error in {nameof(Search)}: {ex.Message}"));
            }
        }

        #endregion

        #region "Add - Edit"
        
        [HttpPost("Add")]
        public async Task<IActionResult> Add(IntegraTableValueDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(1629, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync(localization.GetMessagesResource("dataRequire")));

                if (string.IsNullOrEmpty(obj.FieldValue))
                    return Ok(await Result.FailAsync(localization.GetMessagesResource("dataRequire")));
                if (string.IsNullOrEmpty(obj.FieldReference))
                    return Ok(await Result.FailAsync(localization.GetMessagesResource("dataRequire")));
                if (obj.IntegraSytstemId <= 0)
                    return Ok(await Result.FailAsync(localization.GetMessagesResource("dataRequire")));
                if (obj.TableId <= 0)
                    return Ok(await Result.FailAsync(localization.GetMessagesResource("dataRequire")));

                var add = await integraServiceManager.IntegraTableValueService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(IntegraTableValueEditDto obj)
        {
            var chk = await permission.HasPermission(1629, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<IntegraTableValueEditDto>.FailAsync(localization.GetMessagesResource("dataRequire")));
                }

                if (string.IsNullOrEmpty(obj.FieldValue))
                    return Ok(await Result.FailAsync(localization.GetMessagesResource("dataRequire")));
                if (string.IsNullOrEmpty(obj.FieldReference))
                    return Ok(await Result.FailAsync(localization.GetMessagesResource("dataRequire")));
                if (obj.IntegraSytstemId <= 0)
                    return Ok(await Result.FailAsync(localization.GetMessagesResource("dataRequire")));
                if (obj.TableId <= 0)
                    return Ok(await Result.FailAsync(localization.GetMessagesResource("dataRequire")));

                var Edit = await integraServiceManager.IntegraTableValueService.Update(obj);
                return Ok(Edit);
            }
            catch (Exception ex)
            {
                return Ok(await Result<IntegraTableValueEditDto>.FailAsync($"======= Exp in edit: {ex.Message}"));
            }
        }
        
        #endregion "Add - Edit"

        #region "Delete"
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long Id)
        {
            var chk = await permission.HasPermission(1629, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var add = await integraServiceManager.IntegraTableValueService.Remove(Id);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<IntegraTableValueDto>.FailAsync($"======= Exp in Delete: {ex.Message}"));
            }
        }
        #endregion "Delete"

        #region "GetByIdForEdit - GetById"

        [HttpGet("GetByIdForEdit")]
        public async Task<IActionResult> GetByIdForEdit(long id)
        {
            try
            {
                var chk = await permission.HasPermission(1629, PermissionType.Show);
                if (!chk)
                {
                    return Ok(await Result.AccessDenied("AccessDenied"));
                }

                if (id <= 0)
                {
                    return Ok(await Result<IntegraTableValueEditDto>.FailAsync(localization.GetMessagesResource("InCorrectId")));
                }

                var getItem = await integraServiceManager.IntegraTableValueService.GetForUpdate<IntegraTableValueEditDto>(id);
                if (getItem.Succeeded)
                {
                    return Ok(await Result<IntegraTableValueEditDto>.SuccessAsync(getItem.Data, $""));
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
                var chk = await permission.HasPermission(1629, PermissionType.Show);
                if (!chk)
                {
                    return Ok(await Result.AccessDenied("AcessDenied"));
                }

                if (id <= 0)
                {
                    return Ok(await Result<IntegraTableValueDto>.FailAsync(localization.GetMessagesResource("NoIdInUpdate")));
                }

                var getItem = await integraServiceManager.IntegraTableValueService.GetOne(s => s.Id == id && s.IsDeleted == false);
                if (getItem.Succeeded)
                {
                    var obj = getItem.Data;
                    return Ok(await Result<IntegraTableValueDto>.SuccessAsync(obj, $""));
                }
                else
                {
                    return Ok(getItem);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<IntegraTableValueDto>.FailAsync($"====== Exp MESSAGE: {ex.Message}"));
            }
        }

        #endregion "GetByIdForEdit - GetById"
    }
}
