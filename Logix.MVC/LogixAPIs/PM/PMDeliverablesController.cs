using Logix.Application.Common;
using Logix.Application.DTOs.PM;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Services;
using Logix.Application.Wrapper;
using Logix.Domain.PM;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;

namespace Logix.MVC.LogixAPIs.PM
{

    //  المخرجات

    public class PMDeliverablesController : BasePMApiController
    {
        private readonly IPMServiceManager pMServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IWFServiceManager wFServiceManager;
        private readonly IMainServiceManager mainServiceManager;


        public PMDeliverablesController(IPMServiceManager pMServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IWFServiceManager wFServiceManager, IMainServiceManager mainServiceManager)
        {
            this.pMServiceManager = pMServiceManager;
            this.permission = permission;
            this.session = session;
            this.localization = localization;
            this.wFServiceManager = wFServiceManager;
            this.mainServiceManager = mainServiceManager;
        }

        #region Index Page


        [HttpPost("Search")]
        public async Task<IActionResult> Search(PmProjectsDeliverableFilterDto filter)
        {
            var chk = await permission.HasPermission(1906, PermissionType.Show);

            if (!chk)
                return Ok(await Result.AccessDenied("AccessDenied"));

            try
            {
                filter.ProjectCode ??= 0;

                var items = await pMServiceManager.PmProjectsDeliverableService.GetAllVW(e => e.IsDeleted == false && e.DeliveryDate != null
                && ((filter.ProjectCode == 0) || ((e.ProjectCode == filter.ProjectCode)))
                && (string.IsNullOrEmpty(filter.ProjectName) || ((e.ProjectName != null && e.ProjectName.Contains(filter.ProjectName))))
                && (string.IsNullOrEmpty(filter.Name) || ((e.Name != null && e.Name.Contains(filter.Name))))
                && (string.IsNullOrEmpty(filter.CustomerCode) || ((e.CustomerCode != null && e.CustomerCode.Contains(filter.CustomerCode))))
                && (string.IsNullOrEmpty(filter.CustomerName) || ((e.CustomerName != null && e.CustomerName.Contains(filter.CustomerName))))
                && (string.IsNullOrEmpty(filter.ItemName) || ((e.ItemName != null && e.ItemName.Contains(filter.ItemName))))
                );

                if (!items.Succeeded)
                    return Ok(await Result<object>.FailAsync(items.Status.message));
                if (!items.Data.Any())
                    return Ok(await Result<object>.SuccessAsync(new List<object>(), localization.GetResource1("NosearchResult")));

                var res = items.Data.AsQueryable();

                if (!string.IsNullOrEmpty(filter.From) && !string.IsNullOrEmpty(filter.To))
                {
                    var DateFrom = DateHelper.StringToDate(filter.From);
                    var DateTo = DateHelper.StringToDate(filter.To);
                    res = res.Where(x => x.DeliveryDate != null && x.DeliveryDate != "" && DateHelper.StringToDate(x.DeliveryDate) >= DateFrom && DateHelper.StringToDate(x.DeliveryDate) <= DateTo);
                }

                if (!res.Any())
                    return Ok(await Result<object>.SuccessAsync(new List<object>(), localization.GetResource1("NosearchResult")));

                var result = res.Select(g => new
                {
                    g.Id,
                    g.ProjectCode,
                    g.ProjectName,
                    g.CustomerCode,
                    g.CustomerName,
                    g.Name,
                    g.Type,
                    Qty = g.Qty,
                    g.DeliveryDate,
                    g.StatusName,
                    g.Note,
                    g.FilePdf,
                    g.PathFile,
                    g.FileUrl,
                })
                              .ToList();

                return Ok(await Result<object>.SuccessAsync(result, ""));
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"======= Exp in {this.GetType()}: {ex.Message}"));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var chk = await permission.HasPermission(1906, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await pMServiceManager.PmProjectsDeliverableService.Remove(id);

                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"======= Exp in {this.GetType()}: {ex.Message}"));
            }

        }


        [HttpPost("ChangeStatus")]
        public async Task<IActionResult> ChangeStatus(PmProjectsDeliverableChangeStatusDto obj)
        {

            try
            {

                var chk = await permission.HasPermission(1906, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                // ملاحظات 

                if (string.IsNullOrEmpty(obj.Note))
                    return Ok(await Result<object>.FailAsync(localization.GetCommonResource("notes")));

                // المرفق 

                if (string.IsNullOrEmpty(obj.FileUrl))
                    return Ok(await Result<object>.FailAsync("المرفق"));

                // الحالة
                if (obj.StatusId <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("Status")));


                // المخرجات
                if (obj.Ids.Count <= 0)
                    return Ok(await Result<object>.FailAsync("قم بعملية إختيار المخرج اولاً"));


                var result = await pMServiceManager.PmProjectsDeliverableService.ChangeStatus(obj);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"======= Exp in {this.GetType()}: {ex.Message}"));

            }

        }

        #endregion

        #region Edit Page


        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long id)
        {
            try
            {
                var hasPermission = await permission.HasPermission(1906, PermissionType.Edit);

                if (!hasPermission)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetMessagesResource("NoIdInUpdate")));

                var item = await pMServiceManager.PmProjectsDeliverableService.GetOneVW(x => x.IsDeleted == false && x.Id == id);
                if (!item.Succeeded)
                    return Ok(await Result<object>.FailAsync(item.Status.message));

                var GetTrackingStatus = await pMServiceManager.PmDeliverablesTrackingStatusService.GetAllVW(x => x.DeliverablesId == id);

                // Prepare response
                var response = new
                {
                    DeliverableData = item.Data,
                    AllTrackingStatus = GetTrackingStatus.Data.AsEnumerable(),
                };

                return Ok(await Result<object>.SuccessAsync(response, string.Empty, 200));

            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"Error in {this.GetType}, MESSAGE: {ex.Message}"));
            }
        }


        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(PmProjectsDeliverableEditDto obj)
        {

            try
            {

                var chk = await permission.HasPermission(1906, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (obj.Id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));

                obj.ProjectCode ??= 0;
                obj.StatusId ??= 0;
                obj.Cost ??= 0;
                // المشروع 
                if (obj.ProjectCode <= 0) return Ok(await Result<object>.FailAsync(localization.GetPMResource("Project")));

                if (string.IsNullOrEmpty(obj.Note)) obj.Note = "";
                // فحص تواجد المشروع
                var Project = await pMServiceManager.PMProjectsService.GetOne(x => x.IsDeleted == false && x.Code == obj.ProjectCode);
                if (!Project.Succeeded || Project.Data == null)
                    return Ok(await Result<object>.FailAsync(Project.Succeeded ? "المشروع غير موجود" : Project.Status.message));
                obj.ProjectId = Project.Data.Id;

                // المخرج 
                if (string.IsNullOrEmpty(obj.Name))
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("OutputName")));

                // النوع 
                if (string.IsNullOrEmpty(obj.Type))
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("Type")));

                //  العدد 
                if (obj.Qty <= 0) return Ok(await Result<object>.FailAsync(localization.GetPMResource("Number")));


                // موعد التسليم 
                if (string.IsNullOrEmpty(obj.DeliveryDate))
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("PlannedDeliveryDate")));

                var result = await pMServiceManager.PmProjectsDeliverableService.Update(obj);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"======= Exp in {this.GetType()}: {ex.Message}"));

            }

        }

        #endregion




        #region Add Page


        [HttpPost("Add")]
        public async Task<IActionResult> Add(PmProjectsDeliverableDto obj)
        {

            try
            {
                var chk = await permission.HasPermission(1906, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                obj.ProjectCode ??= 0;
                obj.StatusId ??= 0;
                obj.Cost ??= 0;
                // المشروع 
                if (obj.ProjectCode <= 0) return Ok(await Result<object>.FailAsync(localization.GetPMResource("Project")));

                if (string.IsNullOrEmpty(obj.Note)) obj.Note = "";
                // فحص تواجد المشروع
                var Project = await pMServiceManager.PMProjectsService.GetOne(x => x.IsDeleted == false && x.Code == obj.ProjectCode);
                if (!Project.Succeeded || Project.Data == null)
                    return Ok(await Result<object>.FailAsync(Project.Succeeded ? "المشروع غير موجود" : Project.Status.message));
                obj.ProjectId = Project.Data.Id;

                // المخرج 
                if (string.IsNullOrEmpty(obj.Name))
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("OutputName")));

                // النوع 
                if (string.IsNullOrEmpty(obj.Type))
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("Type")));

                //  العدد 
                if (obj.Qty <= 0) return Ok(await Result<object>.FailAsync(localization.GetPMResource("Number")));


                // موعد التسليم 
                if (string.IsNullOrEmpty(obj.DeliveryDate))
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("PlannedDeliveryDate")));
                obj.AddTrack = 1;

                var result = await pMServiceManager.PmProjectsDeliverableService.Add(obj);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"======= Exp in {this.GetType()}: {ex.Message}"));

            }

        }

        #endregion

        #region توليد المخرجات

        [HttpGet("GetProjectItemByCode")]
        public async Task<IActionResult> GetProjectItemByCode(long ProjectCode)
        {
            try
            {
                var hasPermission = await permission.HasPermission(1906, PermissionType.Show);

                if (!hasPermission)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                var item = await pMServiceManager.PMProjectsItemService.GetAllVW(x => x.IsDeleted == false && x.ProjectCode == ProjectCode);
                if (!item.Succeeded)
                    return Ok(await Result<object>.FailAsync(item.Status.message));

                return Ok(item);

            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"Error in {this.GetType}, MESSAGE: {ex.Message}"));
            }
        }




        [HttpPost("GeneratingDeliverable")]
        public async Task<IActionResult> GeneratingDeliverable(List<ProjectDeliverablesDefinitionDto> objects)
        {
            try
            {
                // Check permissions
                var hasPermission = await permission.HasPermission(1906, PermissionType.Show);
                if (!hasPermission)
                {
                    return Ok(await Result.AccessDenied("AccessDenied"));
                }

                // Validate each item in the input list
                foreach (var item in objects)
                {
                    if (item.Qty <= 0 || item.PaymentId <= 0 || string.IsNullOrEmpty(item.Date))
                    {
                        return Ok(await Result<string>.FailAsync("تأكد من إضافة الكمية أو الفترة أو التاريخ"));
                    }
                }

                // Proceed with generating deliverables
                var result = await pMServiceManager.PmProjectsDeliverableService.ProjectDeliverablesDefinition(objects);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the error (optional logging for better diagnostics)
                return Ok(await Result<string>.FailAsync($"Error in {nameof(GeneratingDeliverable)}: {ex.Message}"));
            }
        }




        #endregion

    }
}
