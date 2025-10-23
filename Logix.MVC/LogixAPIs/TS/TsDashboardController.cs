using DevExpress.Xpo;
using Logix.Application.Common;
using Logix.Application.DTOs.ACC;
using Logix.Application.DTOs.TS;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.TS
{
    public class TsDashboardController : BaseTsApiController
    {
        private readonly ITsServiceManager tsServiceManager;
        private readonly ICurrentData session;

        public TsDashboardController(ITsServiceManager tsServiceManager, ICurrentData session)
        {
            this.tsServiceManager = tsServiceManager;
            this.session = session;
        }

        #region "Dashboard"
        /// <summary>
        /// الشاشة الأولى في نظام إدارة المهام.
        /// استرجاع بيانات لوحة التحكم الخاصة بالمستخدم بناءً على المعرفات المرتبطة بالمنشئة والمستخدم الحالي.
        /// </summary>
        /// <returns>
        /// <see cref="IActionResult"/> يحتوي على قائمة بالإحصائيات المطلوبة للوحة التحكم أو رسالة خطأ في حالة حدوث استثناء.
        /// </returns>
        /// <remarks>
        /// <b>وصف الوظيفة:</b>  
        /// تقوم هذه الطريقة بجلب إحصائيات مرتبطة بالمنشئة والمستخدم الحالي عن طريق خدمة `TsTaskService`.
        ///
        /// <b>المخرجات:</b>  
        /// قائمة من الإحصائيات (`TsStatisticsDto`) تُعرض في لوحة التحكم.
        ///
        /// <b>الملاحظات:</b>  
        /// - تعتمد على معرف المنشئة (`FacilityId`) ومعرف المستخدم (`UserId`) المخزنة في جلسة المستخدم الحالية.
        /// - تُرجع رسالة خطأ إذا حدث استثناء أثناء التنفيذ.
        /// </remarks>
        [HttpGet("Dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            try
            {
                // تنفيذ منطق جلب الإحصائيات.
                List<TsStatisticsDto> statisticsList = new List<TsStatisticsDto>();
                var getStatistics = await tsServiceManager.TsTaskService.GetStatistics(session.FacilityId, session.UserId);
                return Ok(getStatistics);
            }
            catch (Exception ex)
            {
                // إرجاع رسالة خطأ في حالة وجود استثناء.
                return BadRequest(ex.Message);
            }
        }
        #endregion "Dashboard"
    }
}
