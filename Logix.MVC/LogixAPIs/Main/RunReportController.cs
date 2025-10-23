using Logix.Application.Common;
using Logix.Application.DTOs.RPT;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using static DevExpress.Data.ODataLinq.Helpers.ODataLinqHelpers;
namespace Logix.MVC.LogixAPIs.Main
{
    public class RunReportController : BaseMainApiController
    {

        private readonly IRptServiceManager rptServiceManager;
        private readonly ILocalizationService localization;
        private readonly ICurrentData currentData;
        private readonly IDDListHelper listHelper;

        public RunReportController(IRptServiceManager rptServiceManager,
            ILocalizationService localization,
            ICurrentData currentData,
             IDDListHelper listHelper)
        {
            this.rptServiceManager = rptServiceManager;
            this.localization = localization;
            this.currentData = currentData;
            this.listHelper = listHelper;
        }
        #region "transactions"

        [HttpGet("DDLFields")]
        public async Task<IActionResult> DDLFields(int reportId)
        {
            try
            {
                long lang = currentData.Language;
                var list = new SelectList(new List<DDListItem<RptFieldDto>>());
                var allField = await rptServiceManager.RptFieldService.GetFieldsByReportId(reportId);
                if (allField.Succeeded && allField.Data.Any())
                {
                    var groups = allField.Data.OrderBy(x => x.Id).ToList();
                    list = listHelper.GetFromList<long>(groups.Select(s => new DDListItem<long> { Name = lang == 1 ? s.Name ?? "" : s.Name2 ?? "", Value = Convert.ToInt64(s.Id) }), hasDefault: false);
                    return Ok(await Result<SelectList>.SuccessAsync(list));
                }
                else
                    return Ok(await Result<SelectList>.SuccessAsync(list));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
        #endregion   "transactions"




        #region "transactions_GetById"

        [HttpGet("GetByIdForEdit")]
        public async Task<IActionResult> GetByIdForEdit(long id)
        {
            try
            {


                if (id <= 0)
                {
                    return Ok(await Result<RptReportDto>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var getItem = await rptServiceManager.RptReportService.GetForUpdate<RptReportDto>(id);
                if (getItem.Succeeded)
                {
                    var obj = getItem.Data;



                    return Ok(await Result<RptReportDto>.SuccessAsync(obj, $""));
                }
                else
                {
                    return Ok(getItem);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<RptReportDto>.FailAsync($"====== Exp in GetByIdForEdit Report, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(long id)
        {
            try
            {


                if (id <= 0)
                {
                    return Ok(await Result<RptReportDto>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var getItem = await rptServiceManager.RptReportService.GetOne(s => s.Id == id && s.IsDeleted == false);
                if (getItem.Succeeded)
                {
                    var obj = getItem.Data;
                    return Ok(await Result<RptReportDto>.SuccessAsync(obj, $""));
                }
                else
                {
                    return Ok(getItem);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<RptReportDto>.FailAsync($"====== Exp in GetById Report, MESSAGE: {ex.Message}"));
            }
        }

        #endregion "transactions_GetById"


        [HttpGet("RunReport")]
        public async Task<IActionResult> RunReport(long id)
        {
            try
            {
                if (id <= 0)
                {
                    var failMessage = localization.GetMessagesResource("NoIdInUpdate");
                    return Ok(await Result<IEnumerable<dynamic>>.FailAsync(failMessage));
                }

                var getItem = await rptServiceManager.RptReportService.RunReport(id);

                if (getItem.Succeeded)
                {
                    var obj = getItem.Data;
                    return Ok(await Result<IEnumerable<dynamic>>.SuccessAsync(obj, string.Empty));
                }
                else
                {
                    return Ok(getItem);
                }
            }
            catch (Exception ex)
            {
                var errorMessage = $"====== Exp in RunReport, MESSAGE: {ex.Message}";
                return Ok(await Result<IEnumerable<dynamic>>.FailAsync(errorMessage));
            }
        }

        [HttpPost("RunReportFiltered")]
        public async Task<IActionResult> RunReportFiltered(long id, List<RptReportFilterConditionDto> filters)
        {
            try
            {
                if (id <= 0)
                {
                    var failMessage = localization.GetMessagesResource("NoIdInUpdate");
                    return Ok(await Result<IEnumerable<dynamic>>.FailAsync(failMessage));
                }

                var getItem = await rptServiceManager.RptReportService.RunReport(id);
                if (!getItem.Succeeded)
                    return Ok(getItem);

                var data = getItem.Data; // نفترض data قائمة من objects ديناميكية

                // تطبيق الفلترة يدوياً في الكود (مثلاً باستخدام LINQ)
                var filteredData = ApplyFilters(data, filters);

                return Ok(await Result<IEnumerable<dynamic>>.SuccessAsync(filteredData, string.Empty));
            }
            catch (Exception ex)
            {
                var errorMessage = $"====== Exp in RunReportFiltered, MESSAGE: {ex.Message}";
                return Ok(await Result<IEnumerable<dynamic>>.FailAsync(errorMessage));
            }
        }
        private IEnumerable<dynamic> ApplyFilters(IEnumerable<dynamic> data, List<RptReportFilterConditionDto> filters)
        {
            var filteredData = data;

            foreach (var filter in filters)
            {
                if (filter.OperatorFun == "Contains")
                {
                    filteredData = filteredData.Where(d =>
                        ((string)GetPropertyValue(d, filter.FieldName))?.Contains(filter.FieldValue) == true);
                }
                else if (filter.OperatorFun == "=")
                {
                    filteredData = filteredData.Where(d =>
                        ((string)GetPropertyValue(d, filter.FieldName)) == filter.FieldValue);
                }
            }

            return filteredData;
        }

        private object GetPropertyValue(dynamic obj, string propertyName)
        {
            var prop = obj.GetType().GetProperty(propertyName);
            return prop?.GetValue(obj, null);
        }

    }

}
