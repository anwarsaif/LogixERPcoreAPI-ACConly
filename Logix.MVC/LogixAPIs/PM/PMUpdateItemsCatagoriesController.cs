using DocumentFormat.OpenXml.Spreadsheet;
using Logix.Application.Common;
using Logix.Application.DTOs.PM;
using Logix.Application.DTOs.SAL;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Services;
using Logix.Application.Wrapper;
using Logix.Domain.Main;
using Logix.Domain.PM;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;

namespace Logix.MVC.LogixAPIs.PM
{
    //  ربط الأصناف بالفئات

    public class UpdateItemsFilterDto
    {
        public long ProjectCode { get; set; }
        public bool isSubContract { get; set; }
    }
    public class PMUpdateItemsCatagoriesController : BasePMApiController
    {
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IPMServiceManager pMServiceManager;



        public PMUpdateItemsCatagoriesController(IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IPMServiceManager pMServiceManager)
        {
            this.permission = permission;
            this.session = session;
            this.localization = localization;
            this.pMServiceManager = pMServiceManager;
        }

        #region Index Page

        [HttpPost("Search")]
        public async Task<IActionResult> Search(PaginatedRequest<UpdateItemsFilterDto> request)
        {
            try
            {
                var chk = await permission.HasPermission(908, PermissionType.Add);

                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));


                var filter = request.Filter;
                if (filter.ProjectCode <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("ProjectNo")));


                var GetProject = await pMServiceManager.PMProjectsService.GetPMProjectsByCode(filter.ProjectCode, session.FacilityId, filter.isSubContract);

                if (!GetProject.Succeeded)
                    return Ok(await Result<object>.FailAsync(GetProject.Status.message));

                if (GetProject.Data == null)
                    return Ok(await Result<object>.FailAsync($"{localization.GetResource1("TheProjectNumberIsNotFoundInTheProjectList")}"));


                var GetProjectItems = await pMServiceManager.PMProjectsItemService.GetAllVW(x => x.IsDeleted == false && x.ProjectId == GetProject.Data.Id);
                if (!GetProjectItems.Succeeded)
                    return Ok(await Result<object>.FailAsync(GetProject.Status.message));


                if (!GetProjectItems.Data.Any())
                    return Ok(await Result<object>.SuccessAsync(new List<object>(), localization.GetResource1("NosearchResult")));


                var result = GetProjectItems.Data.Select(x =>
                new
                {
                    x.Id,
                    x.ItemCode,
                    x.ItemName,
                    x.Price,
                    x.Qty,
                    x.Total,
                    x.CatName,
                });
                if (request.PageNumber > 0 && request.PageSize > 0)
                {
                    var paginatedData = await PaginatedResult<object>.SuccessAsync(
                        result,
                        request.PageNumber,
                        request.PageSize);
                    return Ok(paginatedData);
                }

                return Ok(await Result<object>.SuccessAsync(result.ToList(), "Search Completed", 200));
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"Error in {nameof(Search)}: {ex.Message}"));
            }
        }



        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(UpdateProjectItemsCatagoryDto obj)
        {

            try
            {

                var chk = await permission.HasPermission(908, PermissionType.Add);

                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (obj.CatId <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetPMResource("CCategory")));

                if (obj.ItemsIds.Count() <= 0)
                    return Ok(await Result<object>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));


                var result = await pMServiceManager.PMProjectsItemService.UpdateProjecttemsCatagory(obj);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"Error in {this.GetType().Name}.{nameof(Edit)}: {ex.Message}"));

            }

        }


        #endregion


    }
}
