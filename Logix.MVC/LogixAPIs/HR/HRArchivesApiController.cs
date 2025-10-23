using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.HR.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Logix.MVC.LogixAPIs.HR
{
    //الأرشيف 
    public class HRArchivesApiController : BaseHrApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        public HRArchivesApiController(IHrServiceManager hrServiceManager, IMainServiceManager mainServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization)
        {
            this.mainServiceManager = mainServiceManager;
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
        }


        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var chk = await permission.HasPermission(265, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var items = await hrServiceManager.HrArchivesFilesService.GetAll(e => e.IsDeleted == false);
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    res = res.OrderBy(e => e.ArchiveFileId);
                    return Ok(await Result<List<HrArchivesFileDto>>.SuccessAsync(res.ToList(), items.Status.message));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrArchievesFilterDto filter)
        {
            var chk = await permission.HasPermission(265, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var BranchesList = session.Branches.Split(',');
                filter.BranchId ??= 0;
                string[] fileTypeIdList = new string[] { };
                if (!string.IsNullOrEmpty(filter.FileTypeId))
                {
                    fileTypeIdList = filter.FileTypeId.Split(',');
                }
                var items = await hrServiceManager.HrArchivesFilesService.GetAllVW(e => e.IsDeleted == false &&
                e.EmpTypeId == 1 &&
                (string.IsNullOrEmpty(filter.EmpCode) || e.EmpCode.Contains(filter.EmpCode)) &&
                (string.IsNullOrEmpty(filter.EmpName) || e.EmpName.Contains(filter.EmpName)) &&
                (filter.BranchId != 0 ? e.BranchId == filter.BranchId : BranchesList.Contains(e.BranchId.ToString())) &&
                (fileTypeIdList.Any() || fileTypeIdList.Contains(e.FileTypeId))
                );

                if (items.Succeeded)
                {
                    return Ok(await Result<List<HrArchivesFilesVw>>.SuccessAsync(items.Data.ToList(), ""));
                }

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpPost("GetPagination")]
        public async Task<IActionResult> GetPagination([FromBody] HrArchievesFilterDto filter, int take = Pagination.take, long? lastSeenId = null)
        {
            var chk = await permission.HasPermission(265, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {

                var BranchesList = session.Branches.Split(',');
                filter.BranchId ??= 0;
                string[] fileTypeIdList = new string[] { };
                if (!string.IsNullOrEmpty(filter.FileTypeId))
                {
                    fileTypeIdList = filter.FileTypeId.Split(',');
                }
                var items = await hrServiceManager.HrArchivesFilesService.GetAllWithPaginationVW(selector: e => e.ArchiveFileId,
                expression: e => e.IsDeleted == false &&
                e.EmpTypeId == 1 &&
                (string.IsNullOrEmpty(filter.EmpCode) || e.EmpCode.Contains(filter.EmpCode)) &&
                (string.IsNullOrEmpty(filter.EmpName) || e.EmpName.Contains(filter.EmpName)) &&
                (filter.BranchId != 0 ? e.BranchId == filter.BranchId : BranchesList.Contains(e.BranchId.ToString())) &&
                (fileTypeIdList.Length == 0 || fileTypeIdList.Contains(e.FileTypeId)),
                    take: take,
                    lastSeenId: lastSeenId);

                if (!items.Succeeded)
                    return Ok(await Result<List<HrArchivesFilesVw>>.FailAsync(items.Status.message));

                if (items.Data.Count() > 0)
                {
                    var paginatedData = new PaginatedResult<object>
                    {
                        Succeeded = items.Succeeded,
                        Data = items.Data,
                        Status = items.Status,
                        PaginationInfo = items.PaginationInfo
                    };
                    return Ok(paginatedData);
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add(HrArchivesFileDto obj)
        {
            var chk = await permission.HasPermission(265, PermissionType.Add);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (!ModelState.IsValid)
            {
                return Ok(obj);
            }
            if (string.IsNullOrEmpty(obj.EmpCode))
            {
                return Ok(await Result<HrArchivesFileDto>.FailAsync(localization.GetMessagesResource("EmployeeNumberIsRequired")));
            }
            try
            {
                var addRes = await hrServiceManager.HrArchivesFilesService.Add(obj);
                if (addRes.Succeeded)
                {
                    return Ok(addRes);
                }
                else
                {
                    return Ok(await Result<HrArchivesFileDto>.FailAsync(addRes.Status.message));
                }
            }

            catch (Exception ex)
            {
                return Ok(await Result<HrArchivesFileDto>.FailAsync(ex.Message));
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(long Id = 0)
        {
            var chk = await permission.HasPermission(265, PermissionType.Edit);
            if (!chk)
            {
                return Ok(Result<HrArchivesFileEditDto>.FailAsync($"Access Denied"));
            }
            if (Id <= 0)
            {
                return Ok(Result<HrArchivesFileEditDto>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
            }

            try
            {
                var getItem = await hrServiceManager.HrArchivesFilesService.GetOneVW(x => x.ArchiveFileId == Id);
                return Ok(getItem);
            }
            catch (Exception exp)
            {
                return Ok(Result<HrArchivesFileEditDto>.FailAsync($"EXP in {this.GetType()}, Meesage: {exp.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(HrArchivesFileEditDto obj)
        {
            var chk = await permission.HasPermission(265, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (!ModelState.IsValid)
            {
                return Ok(obj);
            }
            try
            {
                var addRes = await hrServiceManager.HrArchivesFilesService.Update(obj);
                return Ok(addRes);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrArchivesFileEditDto>.FailAsync($"{ex.Message}"));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long Id = 0)
        {
            var chk = await permission.HasPermission(265, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (Id == 0)
            {
                return Ok(await Result.FailAsync("Please choose an entity to delete it, there is no id passed"));
            }

            try
            {
                var del = await hrServiceManager.HrArchivesFilesService.Remove(Id);
                return Ok(del);
            }
            catch (Exception exp)
            {
                return Ok(await Result.FailAsync($"{exp.Message}"));
            }
        }

        [HttpGet("GetFileDetails")]
        public async Task<IActionResult> GetFileDetails(long FileID)
        {
            var chk = await permission.HasPermission(265, PermissionType.Edit);
            if (!chk)
            {
                return Ok(Result<HrArchivesFileEditDto>.FailAsync($"Access Denied"));
            }
            if (FileID <= 0)
            {
                return Ok(Result<HrArchivesFileEditDto>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
            }

            try
            {
                var getItem = await hrServiceManager.HrArchiveFilesDetailService.GetAll(x => x.IsDeleted == false && x.ArchiveId == FileID);

                return Ok(getItem);

            }
            catch (Exception exp)
            {
                return Ok(Result<HrArchivesFileEditDto>.FailAsync($"EXP in {this.GetType()}, Meesage: {exp.Message}"));
            }
        }
        [HttpDelete("DeleteFileDetails")]
        public async Task<IActionResult> DeleteFileDetails(long Id)
        {
            var chk = await permission.HasPermission(265, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (Id <= 0)
            {
                return Ok(await Result.FailAsync("Please choose an entity to delete it, there is no id passed"));
            }

            try
            {
                var del = await hrServiceManager.HrArchiveFilesDetailService.Remove(Id);

                return Ok(del);
            }
            catch (Exception exp)
            {
                return Ok(await Result.FailAsync($"{exp.Message}"));
            }
        }
        [HttpPost("EditFileDetails")]
        public async Task<IActionResult> EditFileDetails(HrArchiveFilesDetailEditDto obj)
        {
            var chk = await permission.HasPermission(265, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (!ModelState.IsValid)
            {
                return Ok(obj);
            }
            try
            {
                var addRes = await hrServiceManager.HrArchiveFilesDetailService.Update(obj);
                return Ok(addRes);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrArchivesFileEditDto>.FailAsync($"{ex.Message}"));
            }
        }
        [HttpPost("AddFileDetails")]
        public async Task<IActionResult> AddFileDetails(HrArchiveFilesDetailDto obj)
        {
            var chk = await permission.HasPermission(265, PermissionType.Add);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (!ModelState.IsValid)
            {
                return Ok(obj);
            }
            try
            {
                var addRes = await hrServiceManager.HrArchiveFilesDetailService.Add(obj);
                return Ok(addRes);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrArchivesFileEditDto>.FailAsync($"{ex.Message}"));
            }
        }
    }
}