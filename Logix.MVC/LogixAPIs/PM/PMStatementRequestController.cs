using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Logix.Application.Common;
using Logix.Application.DTOs.PM;
using Logix.Application.DTOs.PM.PmProjects;
using Logix.Application.DTOs.PM.PmProjectsStaff;
using Logix.Application.DTOs.PM.Shared;
using Logix.Application.DTOs.SAL;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.PM;
using Logix.Domain.SAL;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.IdentityModel.Tokens;
using System.Linq;

namespace Logix.MVC.LogixAPIs.PM
{
    //  طلب إفادة
    public class PMStatementRequestController : BasePMApiController
    {
        private readonly IPMServiceManager pMServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IWFServiceManager wFServiceManager;



        public PMStatementRequestController(IPMServiceManager pMServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IWFServiceManager wFServiceManager)
        {
            this.pMServiceManager = pMServiceManager;
            this.permission = permission;
            this.session = session;
            this.localization = localization;
            this.wFServiceManager = wFServiceManager;
        }

        #region Index Page


        [HttpPost("Search")]
        public async Task<IActionResult> Search(PmProjectsStatementRequestFilterDto filter)
        {
            var chk = await permission.HasPermission(1782, PermissionType.Show);

            if (!chk)
                return Ok(await Result.AccessDenied("AccessDenied"));

            try
            {
                filter.ProjectCode ??= 0;

                var items = await pMServiceManager.PmProjectsStatementRequestService.GetAllVW(e => e.IsDeleted == false
                && ((filter.ProjectCode == 0) || ((e.ProjectCode == filter.ProjectCode)))
                && (string.IsNullOrEmpty(filter.Code) || ((e.Code != null && e.Code == filter.Code)))
                && (string.IsNullOrEmpty(filter.ProjectName) || ((e.Name != null && e.Name.Contains(filter.ProjectName))))

                && (e.FacilityId == session.FacilityId)
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
                    res = res.Where(x => x.DateRequest != null && x.DateRequest != "" && DateHelper.StringToDate(x.DateRequest) >= DateFrom && DateHelper.StringToDate(x.DateRequest) <= DateTo);
                }

                if (!res.Any())
                    return Ok(await Result<object>.SuccessAsync(new List<object>(), localization.GetResource1("NosearchResult")));

                var result = res.Select(g => new
                {
                    g.Id,
                    g.Code,
                    g.AppId,
                    g.DateRequest,
                    ProjectName = g.Name,
                    g.EmpName,
                    g.StatusName,
                    g.Note,

                }).ToList();

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
                var chk = await permission.HasPermission(1782, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await pMServiceManager.PmProjectsStatementRequestService.Remove(id);

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
                var check = await permission.HasPermission(1782, PermissionType.Edit);

                if (!check)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetMessagesResource("NoIdInUpdate")));

                var item = await pMServiceManager.PmProjectsStatementRequestService.GetOneVW(x => x.IsDeleted == false && x.Id == id);
                if (!item.Succeeded || item.Data == null)
                    return Ok(await Result<object>.FailAsync(item.Succeeded ? localization.GetMessagesResource("NoIdInUpdate") : item.Status.message));

                // Retrieve Workflow Data if available
                var wfData = new object();
                if (item.Data.AppId > 0)
                {
                    var wfApplicationResult = await wFServiceManager.WfApplicationService.GetOneVW(x => x.Isdecision == false && x.Id == item.Data.AppId && x.CreatedBy == session.UserId);
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



                // Prepare response
                var response = new
                {
                    item.Data,
                    WFData = wfData,
                };

                return Ok(await Result<object>.SuccessAsync(response, string.Empty, 200));
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"Error in {this.GetType()}: {ex.Message}"));
            }
        }


        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(PmProjectsStatementRequestEditDto obj)
        {

            try
            {

                var chk = await permission.HasPermission(1782, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (obj.Id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));

                obj.IsArchived ??= false;
                obj.Reasonsrequest ??= 0;
                obj.ProjectCode ??= 0;

                // المشروع 
                if (obj.ProjectCode <= 0) return Ok(await Result<object>.FailAsync(localization.GetPMResource("Project")));




                if (string.IsNullOrEmpty(obj.Note)) obj.Note = "";

                //  تاريخ الطلب
                if (string.IsNullOrEmpty(obj.DateRequest))
                    return Ok(await Result<object>.FailAsync(localization.GetHrResource("AppData")));

                // أسباب طلب الإفادة 

                if (obj.Reasonsrequest <= 0) return Ok(await Result<object>.FailAsync(localization.GetPMResource("Reasonsforrequestingastatement")));

                var result = await pMServiceManager.PmProjectsStatementRequestService.Update(obj);
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
        public async Task<IActionResult> Add(PmProjectsStatementRequestDto obj)
        {

            try
            {
                var chk = await permission.HasPermission(1782, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                obj.IsArchived ??= false;
                obj.Reasonsrequest ??= 0;
                obj.ProjectCode ??= 0;
                obj.AppTypeId ??= 0;

                // المشروع 
                if (obj.ProjectsId.Count <= 0) return Ok(await Result<object>.FailAsync(localization.GetResource1("NoProjectSelected")));




                if (string.IsNullOrEmpty(obj.Note)) obj.Note = "";

                //  تاريخ الطلب
                if (string.IsNullOrEmpty(obj.DateRequest))
                    return Ok(await Result<object>.FailAsync(localization.GetHrResource("AppData")));

                //  اسم حالة المشروع
                if (string.IsNullOrEmpty(obj.StatusName))
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("ProjectStatus")));

                // أسباب طلب الإفادة 

                if (obj.Reasonsrequest <= 0) return Ok(await Result<object>.FailAsync(localization.GetPMResource("Reasonsforrequestingastatement")));

                obj.StatusId = 1;
                obj.FacilityId = session.FacilityId;
                var result = await pMServiceManager.PmProjectsStatementRequestService.AddMultiProjects(obj);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"======= Exp in {this.GetType()}: {ex.Message}"));

            }

        }

        [HttpGet("BindProjects")]
        public async Task<IActionResult> BindProjects(int StatusId)
        {
            try
            {
                var filter = new BindProjectsDto();

                filter.ProjectType ??= 0;
                filter.Type ??= 0;
                filter.Code ??= 0;
                filter.SystemId ??= 0;
                filter.OwnerDeptId ??= 0;
                filter.AmountFrom ??= 0;
                filter.AmountTo ??= 0;
                filter.PaymentType ??= 0;
                filter.ProjectValue ??= 0;
                filter.Iscase ??= 0;
                filter.TenderStatus ??= 0;
                filter.OwnerDeptId ??= 0;
                filter.StatusId = StatusId;
                filter.SystemId = 5;
                filter.Iscase = 0;
                filter.Isletter = false;
                filter.IsActive = false;
                filter.IsSubContract = false;
                filter.BranchId = 0;

                var result = await pMServiceManager.PMProjectsService.BindProjects(filter);
                return Ok(result);

            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"======= Exp in {this.GetType()}: {ex.Message}"));

            }
        }

        #endregion

        #region View 

        //  GetById  الدالة هي نفسها 
        // اذا كانت حالة الطلب تساوي 2 فانه يتم نقله الى صفحة التعديل مالم يتم عرض الصفحة مع اخفاء زر التعديل
        //  if Application Status=2  Edit Button Visible=true 




        #endregion





    }
}
