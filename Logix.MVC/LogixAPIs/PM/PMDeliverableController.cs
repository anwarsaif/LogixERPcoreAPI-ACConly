using DocumentFormat.OpenXml.Drawing;
using Logix.Application.Common;
using Logix.Application.DTOs.Main;
using Logix.Application.DTOs.OPM;
using Logix.Application.DTOs.PM;
using Logix.Application.DTOs.PM.PmProjects;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.OPM;
using Logix.Domain.PM;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;

namespace Logix.MVC.LogixAPIs.PM
{
    // اعتماد المخرجات
    public class PMDeliverableController : BasePMApiController
    {
        private readonly IPMServiceManager pMServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IWFServiceManager wFServiceManager;
        private readonly IMainServiceManager mainServiceManager;


        public PMDeliverableController(IPMServiceManager pMServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IWFServiceManager wFServiceManager, IMainServiceManager mainServiceManager)
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
        public async Task<IActionResult> Search(PmDeliverableTransactionFilterDto filter)
        {
            var chk = await permission.HasPermission(1750, PermissionType.Show);

            if (!chk)
                return Ok(await Result.AccessDenied("AccessDenied"));

            try
            {
                filter.ProjectCode ??= 0;


                var items = await pMServiceManager.PmDeliverableTransactionService.GetAllVW(e => e.IsDeleted == false
                && (string.IsNullOrEmpty(filter.ProjectName) || ((e.ProjectName != null && e.ProjectName.Contains(filter.ProjectName))))
                && ((filter.ProjectCode == 0) || ((e.ProjectCode == filter.ProjectCode)))
                && (string.IsNullOrEmpty(filter.Code) || ((e.Code != null && e.Code == filter.Code)))

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
                    g.Code,
                    ApprovalDate = g.Date1,
                    g.ProjectCode,
                    g.ProjectName
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
                var chk = await permission.HasPermission(1750, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await pMServiceManager.PmDeliverableTransactionService.Remove(id);

                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"======= Exp in {this.GetType()}: {ex.Message}"));
            }

        }

        #endregion

        #region Edit Page


        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long id)
        {
            try
            {
                // Check permissions
                var hasEditPermission = await permission.HasPermission(1750, PermissionType.Edit);
                var hasViewPermission = await permission.HasPermission(1750, PermissionType.Show);

                if (!hasEditPermission || !hasViewPermission)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetMessagesResource("NoIdInUpdate")));

                // Retrieve Deliverable Transactions data
                var DeliverableTransactionResult = await pMServiceManager.PmDeliverableTransactionService.GetOneVW(x => x.IsDeleted == false && x.Id == id);
                if (!DeliverableTransactionResult.Succeeded || DeliverableTransactionResult.Data == null)
                    return Ok(await Result<object>.FailAsync(DeliverableTransactionResult.Succeeded ? localization.GetMessagesResource("NoIdInUpdate") : DeliverableTransactionResult.Status.message));

                // Retrieve Workflow Data if available
                var wfData = new object();
                if (DeliverableTransactionResult.Data.AppId > 0)
                {
                    var wfApplicationResult = await wFServiceManager.WfApplicationService.GetOneVW(x => x.Isdecision == false && x.Id == DeliverableTransactionResult.Data.AppId && x.CreatedBy == session.UserId);
                    if (wfApplicationResult.Data != null)
                    {
                        wfData = new
                        {
                            AppTypeID = wfApplicationResult.Data.ApplicationsTypeId,
                            ApplicantsID = wfApplicationResult.Data.ApplicantsId,
                            StatusID = wfApplicationResult.Data.StatusId
                        };
                    }
                }

                // Retrieve Project Data
                var projectDataResult = await pMServiceManager.PMProjectsService.GetOneFromProjectsEditVw(e => e.IsDeleted == false && e.Id == DeliverableTransactionResult.Data.ProjectId && e.FacilityId == session.FacilityId);
                var projectData = projectDataResult.Data;

                // Prepare Project details with null checks
                var projectDetails = new
                {
                    ProjectName = projectData?.Name ?? string.Empty,
                    ProjectCode = projectData?.Code ?? 0,
                    ProjectId = projectData?.Id ?? 0,
                    ProjectValue = projectData?.ProjectValue ?? 0,
                    ProjectStart = projectData?.ProjectStart ?? string.Empty,
                    ProjectEnd = projectData?.ProjectEnd ?? string.Empty,
                    ProjectManagerName = projectData?.EmpName ?? string.Empty,
                    ProjectOwnerName = projectData?.OwnerName ?? string.Empty
                };
                // جلب جميع الكميات بحسب المشروع
                var TransactionsDetailsVwResult = await pMServiceManager.PmDeliverableTransactionsDetailService.GetAllVW(d => d.IsDeleted == false && d.ProjectId == DeliverableTransactionResult.Data.ProjectId);
                var transactionDetailsData = TransactionsDetailsVwResult.Succeeded ? TransactionsDetailsVwResult.Data : Enumerable.Empty<PmDeliverableTransactionsDetailsVw>();
                
                // جلب جميع الكميات بحسب الاعتماد

                var AllProjectDeliverable = transactionDetailsData.Where(x => x.TransId == id).ToList();


                var TransactionsDetails = AllProjectDeliverable.Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.DetailsNote,
                    x.TransId,
                    x.Qty,
                    x.QtyPrevious,
                    x.QtyApprove,
                    x.IsDeleted,
                    x.DeliverableId,
                    // العدد المعتمد سابقا
                    AllPreviousQty = transactionDetailsData.Where(d => d.DeliverableId == x.DeliverableId).Sum(t => t.Qty ?? 0)
                }).ToList();



                // Retrieve Files
                var fileResult = await mainServiceManager.SysFileService.GetFilesForUser(id, 1750);
                var fileDtos = fileResult.Data;

                // Prepare response
                var response = new
                {
                    Transaction = DeliverableTransactionResult.Data,
                    TransactionsDetails = TransactionsDetails,
                    FileDtos = fileDtos,
                    projectDetails,
                    WFData = wfData,
                };

                return Ok(await Result<object>.SuccessAsync(response, string.Empty, 200));
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"Error in PMChangeRequestController Edit method, MESSAGE: {ex.Message}"));
            }
        }


        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(PmDeliverableTransactionAddDto obj)
        {

            try
            {
                obj.ProjectCode ??= 0;
                obj.BranchId ??= 0;
                obj.AppTypeId = 0;
                obj.StatusId = 1;
                var chk = await permission.HasPermission(1750, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (obj.Id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));

                // المشروع 
                if (obj.ProjectCode <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("Project")));

                // رقم الموظف
                if (string.IsNullOrEmpty(obj.EmpCode))
                    return Ok(await Result<object>.FailAsync(localization.GetResource1("EmployeeIsNumber")));
                // تاريخ اعتماد المخرج
                if (string.IsNullOrEmpty(obj.DeliveryDate))
                    return Ok(await Result<object>.FailAsync("تاريخ اعتماد المخرج"));

                // التاريخ
                if (string.IsNullOrEmpty(obj.Date1))
                    return Ok(await Result<object>.FailAsync(localization.GetCommonResource("Tdate")));

                // المخرجات
                if (obj.Detailes.Count <= 0)
                    return Ok(await Result<object>.FailAsync("الرجاء اضافة المخرجات"));


                obj.BranchId = session.BranchId;

                var result = await pMServiceManager.PmDeliverableTransactionService.Update(obj);
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
        public async Task<IActionResult> Add(PmDeliverableTransactionAddDto obj)
        {

            try
            {
                obj.ProjectCode ??= 0;
                obj.BranchId ??= 0;
                obj.AppTypeId = 0;
                obj.StatusId = 1;
                var chk = await permission.HasPermission(1750, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                // المشروع 
                if (obj.ProjectCode <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("Project")));

                // رقم الموظف
                if (string.IsNullOrEmpty(obj.EmpCode))
                    return Ok(await Result<object>.FailAsync(localization.GetResource1("EmployeeIsNumber")));
                // تاريخ اعتماد المخرج
                if (string.IsNullOrEmpty(obj.DeliveryDate))
                    return Ok(await Result<object>.FailAsync("تاريخ اعتماد المخرج"));

                // التاريخ
                if (string.IsNullOrEmpty(obj.Date1))
                    return Ok(await Result<object>.FailAsync(localization.GetCommonResource("Tdate")));

                // المخرجات
                if (obj.Detailes.Count <= 0)
                    return Ok(await Result<object>.FailAsync("الرجاء اضافة المخرجات"));


                obj.BranchId = session.BranchId;

                var result = await pMServiceManager.PmDeliverableTransactionService.Add(obj);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"======= Exp in {this.GetType()}: {ex.Message}"));

            }

        }

        #endregion


    }
}
