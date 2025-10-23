using Logix.Application.Common;
using Logix.Application.DTOs.PM;
using Logix.Application.DTOs.PM.Shared;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;

namespace Logix.MVC.LogixAPIs.PM
{
    //  إطلاق المشاريع
    public class PMKickOffController : BasePMApiController
    {
        private readonly IPMServiceManager pMServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IWFServiceManager wFServiceManager;
        private readonly IMainServiceManager mainServiceManager;


        public PMKickOffController(IPMServiceManager pMServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IWFServiceManager wFServiceManager, IMainServiceManager mainServiceManager)
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
        public async Task<IActionResult> Search(PMSharedFilterDto filter)
        {
            var chk = await permission.HasPermission(1842, PermissionType.Show);

            if (!chk)
                return Ok(await Result.AccessDenied("AccessDenied"));

            try
            {
                filter.ProjectCode ??= 0;
                filter.Id ??= 0;

                var items = await pMServiceManager.PmKickOffService.GetAllVW(e => e.IsDeleted == false
                && ((filter.ProjectCode == 0) || ((e.ProjectCode == filter.ProjectCode)))
                && (string.IsNullOrEmpty(filter.ProjectName) || ((e.ProjectName != null && e.ProjectName.Contains(filter.ProjectName))))
                && (filter.Id == 0 || filter.Id == e.Id)
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
                    res = res.Where(x => x.Date1 != null && x.Date1 != "" && DateHelper.StringToDate(x.Date1) >= DateFrom && DateHelper.StringToDate(x.Date1) <= DateTo);
                }

                if (!res.Any())
                    return Ok(await Result<object>.SuccessAsync(new List<object>(), localization.GetResource1("NosearchResult")));

                var result = res.Select(g => new
                {
                    g.Id,
                    g.Date1,
                    g.ProjectCode,
                    g.ProjectName,
                    g.KickOffPlace,
                    g.Members,

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
                var chk = await permission.HasPermission(1842, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await pMServiceManager.PmKickOffService.Remove(id);

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
                var check = await permission.HasPermission(1842, PermissionType.Edit);

                if (!check)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetMessagesResource("NoIdInUpdate")));

                var item = await pMServiceManager.PmKickOffService.GetOneVW(x => x.IsDeleted == false && x.Id == id);
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


                // Retrieve Files
                var fileResult = await mainServiceManager.SysFileService.GetFilesForUser(id, 117);
                var fileDtos = fileResult.Data;

                // Prepare response
                var response = new
                {
                    item.Data,
                    WFData = wfData,
                    FileDtos = fileDtos,

                };

                return Ok(await Result<object>.SuccessAsync(response, string.Empty, 200));
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"Error in {this.GetType()}: {ex.Message}"));
            }
        }


        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(PmKickOffEditDto obj)
        {

            try
            {

                var chk = await permission.HasPermission(1842, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (obj.Id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));

                obj.ProjectCode ??= 0;

                // المشروع 
                if (obj.ProjectCode <= 0) return Ok(await Result<object>.FailAsync(localization.GetPMResource("Project")));




                if (string.IsNullOrEmpty(obj.Description)) obj.Description = "";

                // تاريخ الإطلاق  
                if (string.IsNullOrEmpty(obj.Date1))
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("LaunchDate")));

                //  تاريخ النهاية
                if (string.IsNullOrEmpty(obj.ExpiryDate))
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("EndDate")));


                //  مكان الإجتماع 
                if (string.IsNullOrEmpty(obj.KickOffPlace))
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("MeetingPlace")));

                //  الحضور  
                if (string.IsNullOrEmpty(obj.Members))
                    return Ok(await Result<object>.FailAsync(localization.GetHrResource("Attendance")));

                //  تاريخ الإطلاق 
                var LaunchDate = DateHelper.StringToDate(obj.Date1);

                //   تاريخ النهاية
                var EndDate = DateHelper.StringToDate(obj.ExpiryDate);

                if (EndDate < LaunchDate) return Ok(await Result<string>.FailAsync($"عفوا لايمكن ان يكون تاريخ النهاية  قبل تاريخ الإطلاق للمشروع !"));

                var result = await pMServiceManager.PmKickOffService.Update(obj);
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
        public async Task<IActionResult> Add(PmKickOffDto obj)
        {

            try
            {
                var chk = await permission.HasPermission(1842, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                obj.ProjectCode ??= 0;
                obj.AppTypeId ??= 0;

                // المشروع 
                if (obj.ProjectCode <= 0) return Ok(await Result<object>.FailAsync(localization.GetPMResource("Project")));




                if (string.IsNullOrEmpty(obj.Description)) obj.Description = "";

                // تاريخ الإطلاق  
                if (string.IsNullOrEmpty(obj.Date1))
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("LaunchDate")));

                //  تاريخ النهاية
                if (string.IsNullOrEmpty(obj.ExpiryDate))
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("EndDate")));


                //  مكان الإجتماع 
                if (string.IsNullOrEmpty(obj.KickOffPlace))
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("MeetingPlace")));

                //  الحضور  
                if (string.IsNullOrEmpty(obj.Members))
                    return Ok(await Result<object>.FailAsync(localization.GetHrResource("Attendance")));

                //  تاريخ الإطلاق 
                var LaunchDate = DateHelper.StringToDate(obj.Date1);

                //   تاريخ النهاية
                var EndDate = DateHelper.StringToDate(obj.ExpiryDate);

                if (EndDate < LaunchDate) return Ok(await Result<string>.FailAsync($"عفوا لايمكن ان يكون تاريخ النهاية  قبل تاريخ الإطلاق للمشروع !"));


                var result = await pMServiceManager.PmKickOffService.Add(obj);
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
