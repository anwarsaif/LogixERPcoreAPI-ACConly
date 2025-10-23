using Logix.Application.Common;
using Logix.Application.DTOs.Main;
using Logix.Application.DTOs.PM;
using Logix.Application.DTOs.PM.Shared;
using Logix.Application.Helpers;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.Main;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;

namespace Logix.MVC.LogixAPIs.PM
{
    //  الأرشفة الإلكترونية
    public class PMLibraryController : BasePMApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;



        public PMLibraryController(IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IMainServiceManager mainServiceManager)
        {
            this.permission = permission;
            this.session = session;
            this.localization = localization;
            this.mainServiceManager = mainServiceManager;
        }

        #region Index Page

        [HttpPost("Search")]
        public async Task<IActionResult> Search(PaginatedRequest<SysLibraryFileFilterDto> request)
        {
            var chk = await permission.HasPermission(252, PermissionType.Show);

            if (!chk)
                return Ok(await Result.AccessDenied("AccessDenied"));

            try
            {
                var filter = request.Filter;
                filter.ProjectCode ??= 0;
                filter.Type ??= 0;

                var items = await mainServiceManager.SysLibraryFileService.GetAllVW(e => e.IsDeleted == false
                                    && ((filter.ProjectCode == 0) || (e.ProjectCode == filter.ProjectCode))
                                    && (string.IsNullOrEmpty(filter.ProjectName) || (e.ProjectName != null && e.ProjectName.Contains(filter.ProjectName)))
                                    && (string.IsNullOrEmpty(filter.FileName) || (e.FileName != null && e.FileName.Contains(filter.FileName)))
                                    && (string.IsNullOrEmpty(filter.SourceFile) || (e.SourceFile != null && e.SourceFile.Contains(filter.SourceFile)))
                                    && (string.IsNullOrEmpty(filter.FileDescription) || (e.FileDescription != null && e.FileDescription.Contains(filter.FileDescription)))
                                    && (filter.Type == 0 || filter.Type == e.FileType)
                                );

                if (!items.Succeeded)
                    return Ok(await Result<object>.FailAsync(items.Status.message));

                if (!items.Data.Any())
                    return Ok(await Result<object>.SuccessAsync(new List<object>(), localization.GetResource1("NosearchResult")));
                // If pagination is requested
                if (request.PageNumber > 0 && request.PageSize > 0)
                {
                    // Use the updated SuccessAsync method for paginated result
                    var paginatedData = await PaginatedResult<object>.SuccessAsync(
                        items.Data.Cast<object>(),
                        request.PageNumber,
                        request.PageSize);
                    return Ok(paginatedData);
                }

                // Non-paginated response
                return Ok(await Result<object>.SuccessAsync(items.Data.ToList(), "Search Completed", 200));
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"Error in {nameof(Search)}: {ex.Message}"));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var chk = await permission.HasPermission(252, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await mainServiceManager.SysLibraryFileService.Remove(id);

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
                var check = await permission.HasPermission(252, PermissionType.Edit);

                if (!check)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetMessagesResource("NoIdInUpdate")));

                var item = await mainServiceManager.SysLibraryFileService.GetOneVW(x => x.IsDeleted == false && x.Id == id);

                return Ok(item);
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"Error in {this.GetType()}: {ex.Message}"));
            }
        }


        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(SysLibraryFileEditDto obj)
        {

            try
            {

                var chk = await permission.HasPermission(252, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (obj.Id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));

                // رقم المشروع 
                if (obj.ProjectCode <= 0) return Ok(await Result<object>.FailAsync(localization.GetPMResource("ProjectNo")));

                //اسم المستند
                if (string.IsNullOrEmpty(obj.FileName)) return Ok(await Result<object>.FailAsync(localization.GetCommonResource("doc_name")));

                //  رقم المرجع
                if (string.IsNullOrEmpty(obj.RefranceCode)) return Ok(await Result<object>.FailAsync(localization.GetPMResource("ReferenceNo")));

                //  تاريخ المستند
                if (string.IsNullOrEmpty(obj.FileDate)) return Ok(await Result<object>.FailAsync(localization.GetCommonResource("doc_date")));


                //  النوع
                if (obj.FileType <= 0) return Ok(await Result<object>.FailAsync(localization.GetCommonResource("type")));


                // الوصف
                if (string.IsNullOrEmpty(obj.FileDescription)) return Ok(await Result<object>.FailAsync(localization.GetPMResource("Description")));

                //  المصدر
                if (string.IsNullOrEmpty(obj.SourceFile)) return Ok(await Result<object>.FailAsync(localization.GetPMResource("Source")));

                //  تاريخ انتهاء المستند
                if (string.IsNullOrEmpty(obj.EndDateFile)) return Ok(await Result<object>.FailAsync(localization.GetPMResource("DocEndDate")));

                //مسار الملف
                if (string.IsNullOrEmpty(obj.FileUrl)) return Ok(await Result<object>.FailAsync(localization.GetCommonResource("file_path")));


                var result = await mainServiceManager.SysLibraryFileService.Update(obj);
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
        public async Task<IActionResult> Add(SysLibraryFileDto obj)
        {

            try
            {
                var chk = await permission.HasPermission(252, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                obj.ProjectCode ??= 0;

                // رقم المشروع 
                if (obj.ProjectCode <= 0) return Ok(await Result<object>.FailAsync(localization.GetPMResource("ProjectNo")));

                //اسم المستند
                if (string.IsNullOrEmpty(obj.FileName)) return Ok(await Result<object>.FailAsync(localization.GetCommonResource("doc_name")));

                //  رقم المرجع
                if (string.IsNullOrEmpty(obj.RefranceCode)) return Ok(await Result<object>.FailAsync(localization.GetPMResource("ReferenceNo")));

                //  تاريخ المستند
                if (string.IsNullOrEmpty(obj.FileDate)) return Ok(await Result<object>.FailAsync(localization.GetCommonResource("doc_date")));


                //  النوع
                if (obj.FileType <= 0) return Ok(await Result<object>.FailAsync(localization.GetCommonResource("type")));


                // الوصف
                if (string.IsNullOrEmpty(obj.FileDescription)) return Ok(await Result<object>.FailAsync(localization.GetPMResource("Description")));

                //  المصدر
                if (string.IsNullOrEmpty(obj.SourceFile)) return Ok(await Result<object>.FailAsync(localization.GetPMResource("Source")));

                //  تاريخ انتهاء المستند
                if (string.IsNullOrEmpty(obj.EndDateFile)) return Ok(await Result<object>.FailAsync(localization.GetPMResource("DocEndDate")));

                //مسار الملف
                if (string.IsNullOrEmpty(obj.FileUrl)) return Ok(await Result<object>.FailAsync(localization.GetCommonResource("file_path")));

                var result = await mainServiceManager.SysLibraryFileService.Add(obj);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"======= Exp in {this.GetType()}: {ex.Message}"));

            }

        }

        #endregion


        [HttpPost("VwPagination")]
        public async Task<IActionResult> VwPagination(PaginatedRequest<SysLibraryFileFilterDto> request)
        {
            var chk = await permission.HasPermission(252, PermissionType.Show);

            if (!chk)
                return Ok(await Result.AccessDenied("AccessDenied"));

            try
            {
                var filter = request.Filter;
                filter.ProjectCode ??= 0;
                filter.Type ??= 0;

                // Call the pagination method from the service
                var items = await mainServiceManager.SysLibraryFileService.GetAllWithPaginationVW(
                    e => e.Id,
                    e => e.IsDeleted == false
                        && ((filter.ProjectCode == 0) || (e.ProjectCode == filter.ProjectCode))
                        && (string.IsNullOrEmpty(filter.ProjectName) || (e.ProjectName != null && e.ProjectName.Contains(filter.ProjectName)))
                        && (string.IsNullOrEmpty(filter.FileName) || (e.FileName != null && e.FileName.Contains(filter.FileName)))
                        && (string.IsNullOrEmpty(filter.SourceFile) || (e.SourceFile != null && e.SourceFile.Contains(filter.SourceFile)))
                        && (string.IsNullOrEmpty(filter.FileDescription) || (e.FileDescription != null && e.FileDescription.Contains(filter.FileDescription)))
                        && (filter.Type == 0 || filter.Type == e.FileType),
                    request.PageSize,
                    request.LastSeenId // Pass LastSeenId as long?
                );

                if (!items.Succeeded)
                    return Ok(await Result<object>.FailAsync(items.Status.message));

                if (!items.Data.Any())
                    return Ok(await Result<object>.SuccessAsync(new List<object>(), localization.GetResource1("NosearchResult")));

                // Use the paginated result
                var paginatedData = new PaginatedResult<object>
                {
                    Succeeded = items.Succeeded,
                    Data = items.Data.Cast<object>(),
                    Status = items.Status,
                    PaginationInfo = items.PaginationInfo
                };

                return Ok(paginatedData);
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"Error in {nameof(VwPagination)}: {ex.Message}"));
            }
        }

        [HttpPost("TablePaginationWithDate")]
        public async Task<IActionResult> TablePaginationWithDate(PaginatedRequest<SysLibraryFileFilterDto> request)
        {
            var chk = await permission.HasPermission(252, PermissionType.Show);

            if (!chk)
                return Ok(await Result.AccessDenied("AccessDenied"));

            try
            {
                var filter = request.Filter;
                filter.ProjectCode ??= 0;
                filter.Type ??= 0;

                var dateConditions = new List<DateCondition>
        {
            new DateCondition
            {
                DatePropertyName = "FileDate",
                ComparisonOperator =ComparisonOperator.GreaterThanOrEqual,
                StartDateString = "2019/12/30"
            },
            // Add additional date conditions as needed
        };

                var items = await mainServiceManager.SysLibraryFileService.GetAllWithPagination(
                    e => (long)e.Id,
                    e => e.IsDeleted == false
                        && (string.IsNullOrEmpty(filter.FileName) || (e.FileName != null && e.FileName.Contains(filter.FileName)))
                        && (string.IsNullOrEmpty(filter.SourceFile) || (e.SourceFile != null && e.SourceFile.Contains(filter.SourceFile)))
                        && (string.IsNullOrEmpty(filter.FileDescription) || (e.FileDescription != null && e.FileDescription.Contains(filter.FileDescription)))
                        && (filter.Type == 0 || filter.Type == e.FileType),
                    request.PageSize,
                    request.LastSeenId,
                    dateConditions
                );

                if (!items.Succeeded)
                    return Ok(await Result<object>.FailAsync(items.Status.message));

                if (!items.Data.Any())
                    return Ok(await Result<object>.SuccessAsync(new List<object>(), localization.GetResource1("NosearchResult")));

                // Use the paginated result
                var paginatedData = new PaginatedResult<object>
                {
                    Succeeded = items.Succeeded,
                    Data = items.Data.Cast<object>(),
                    Status = items.Status,
                    PaginationInfo = items.PaginationInfo
                };

                return Ok(paginatedData);
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"Error in {nameof(TablePaginationWithDate)}: {ex.Message}"));
            }
        }


        [HttpPost("TablePaginationWithoutDate")]
        public async Task<IActionResult> TablePaginationWithoutDate(PaginatedRequest<SysLibraryFileFilterDto> request)
        {
            var chk = await permission.HasPermission(252, PermissionType.Show);

            if (!chk)
                return Ok(await Result.AccessDenied("AccessDenied"));

            try
            {
                var filter = request.Filter;
                filter.ProjectCode ??= 0;
                filter.Type ??= 0;



                var items = await mainServiceManager.SysLibraryFileService.GetAllWithPagination(
                    e => (long)e.Id,
                    e => e.IsDeleted == false
                        && (string.IsNullOrEmpty(filter.FileName) || (e.FileName != null && e.FileName.Contains(filter.FileName)))
                        && (string.IsNullOrEmpty(filter.SourceFile) || (e.SourceFile != null && e.SourceFile.Contains(filter.SourceFile)))
                        && (string.IsNullOrEmpty(filter.FileDescription) || (e.FileDescription != null && e.FileDescription.Contains(filter.FileDescription)))
                        && (filter.Type == 0 || filter.Type == e.FileType),
                    request.PageSize,
                    request.LastSeenId,
                    null
                );

                if (!items.Succeeded)
                    return Ok(await Result<object>.FailAsync(items.Status.message));

                if (!items.Data.Any())
                    return Ok(await Result<object>.SuccessAsync(new List<object>(), localization.GetResource1("NosearchResult")));

                // Use the paginated result
                var paginatedData = new PaginatedResult<object>
                {
                    Succeeded = items.Succeeded,
                    Data = items.Data.Cast<object>(),
                    Status = items.Status,
                    PaginationInfo = items.PaginationInfo
                };

                return Ok(paginatedData);
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"Error in {nameof(TablePaginationWithoutDate)}: {ex.Message}"));
            }
        }

    }
}
