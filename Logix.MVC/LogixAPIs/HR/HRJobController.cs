using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.HR
{
    // الوظائف
    public class HRJobController : BaseHrApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;

        public HRJobController(IHrServiceManager hrServiceManager, IMainServiceManager mainServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization)
        {
            this.mainServiceManager = mainServiceManager;
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
        }

        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrJobFilterDto filter)
        {
            List<HrJobFilterDto> resultList = new List<HrJobFilterDto>();
            var chk = await permission.HasPermission(965, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var BranchesList = session.Branches.Split(',');

                var items = await hrServiceManager.HrJobEmployeeVwService.GetAll(e => e.IsDeleted == false && e.FacilityId == session.FacilityId && BranchesList.Contains(e.BranchId.ToString()));
                if (items.Succeeded)
                {
                    if (items.Data.Count() > 0)
                    {
                        var res = items.Data.AsQueryable();


                        if (filter.BranchId != null && filter.BranchId > 0)
                        {
                            res = res.Where(c => c.BranchId != null && c.BranchId.Equals(filter.BranchId));
                        }

                        if (filter.JobCatagoriesId != null && filter.JobCatagoriesId > 0)
                        {
                            res = res.Where(c => c.JobCatagoriesId != null && c.JobCatagoriesId.Equals(filter.JobCatagoriesId));
                        }
                        if (filter.StatusId != null && filter.StatusId > 0)
                        {
                            res = res.Where(c => c.StatusId != null && c.StatusId.Equals(filter.StatusId));
                        }

                        if (filter.DeptId != null && filter.DeptId > 0)
                        {
                            res = res.Where(c => c.DeptId != null && c.DeptId.Equals(filter.DeptId));
                        }

                        if (filter.LocationId != null && filter.LocationId > 0)
                        {
                            res = res.Where(c => c.LocationId != null && c.LocationId.Equals(filter.LocationId));
                        }
                        if (filter.LevelId != null && filter.LevelId > 0)
                        {
                            res = res.Where(c => c.LevelId != null && c.LevelId.Equals(filter.LevelId));
                        }
                        if (filter.SectorId != null && filter.SectorId > 0)
                        {
                            res = res.Where(c => c.SectorId != null && c.SectorId.Equals(filter.SectorId));
                        }

                        if (!string.IsNullOrEmpty(filter.JobName))
                        {
                            res = res.Where(r => r.JobName != null && r.JobName.Contains(filter.JobName));
                        }
                        if (!string.IsNullOrEmpty(filter.Note))
                        {
                            res = res.Where(r => r.Note != null && r.Note.Contains(filter.Note));
                        }
                        if (!string.IsNullOrEmpty(filter.JobNo))
                        {
                            res = res.Where(r => r.JobNo != null && r.JobNo == filter.JobNo);
                        }
                        if (!string.IsNullOrEmpty(filter.MofCode))
                        {
                            res = res.Where(r => r.MofCode != null && r.MofCode == filter.MofCode);
                        }

                        foreach (var item in res)
                        {
                            var newRecord = new HrJobFilterDto
                            {
                                Id = item.Id,
                                JobNo = item.JobNo,
                                JobName = item.JobName,
                                StatusName = item.StatusName,
                                StatusName2 = item.StatusName2,
                                BraName = item.BraName,
                                BraName2 = item.BraName2,
                                DepName = item.DepName,
                                DepName2 = item.DepName2,
                                LocationName = item.LocationName,
                                LocationName2 = item.LocationName2,
                                LevelName = item.LevelName,
                                EmpId = item.EmpId,
                                EmpName = item.EmpName,
                                EmpName2 = item.EmpName2,
                                CreateDate = item.CreateDate,
                                DecNo = item.DecNo,
                                DecDate = item.DecDate,
                                Note = item.Note,
                                MofCode = item.MofCode,
                                SectorName = item.SectorName,
                                SectorName2 = item.SectorName2,



                            };
                            resultList.Add(newRecord);

                        }
                        return Ok(await Result<List<HrJobFilterDto>>.SuccessAsync(resultList, ""));
                    }

                    return Ok(await Result<List<HrJobFilterDto>>.SuccessAsync(resultList));

                }

                return Ok(await Result<HrJobFilterDto>.FailAsync(items.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }
        [HttpPost("GetPagination")]
        public async Task<IActionResult> GetPagination(HrJobFilterDto filter, int take = Pagination.take, long? lastSeenId = null)
        {
            var chk = await permission.HasPermission(965, PermissionType.Show);
            if (!chk)
                return Ok(await Result.AccessDenied("AccessDenied"));

            try
            {
                var BranchesList = session.Branches.Split(',');

                var items = await hrServiceManager.HrJobEmployeeVwService.GetAllWithPaginationVW(
                    selector: x => x.Id,
                    expression: e => e.IsDeleted == false
                                && e.FacilityId == session.FacilityId
                                && BranchesList.Contains(e.BranchId.ToString())
                                && (filter.BranchId == null || filter.BranchId == 0 || e.BranchId == filter.BranchId)
                                && (filter.JobCatagoriesId == null || filter.JobCatagoriesId == 0 || e.JobCatagoriesId == filter.JobCatagoriesId)
                                && (filter.StatusId == null || filter.StatusId == 0 || e.StatusId == filter.StatusId)
                                && (filter.DeptId == null || filter.DeptId == 0 || e.DeptId == filter.DeptId)
                                && (filter.LocationId == null || filter.LocationId == 0 || e.LocationId == filter.LocationId)
                                && (filter.LevelId == null || filter.LevelId == 0 || e.LevelId == filter.LevelId)
                                && (filter.SectorId == null || filter.SectorId == 0 || e.SectorId == filter.SectorId)
                                && (string.IsNullOrEmpty(filter.JobName) || e.JobName.Contains(filter.JobName))
                                && (string.IsNullOrEmpty(filter.Note) || e.Note.Contains(filter.Note))
                                && (string.IsNullOrEmpty(filter.JobNo) || e.JobNo == filter.JobNo)
                                && (string.IsNullOrEmpty(filter.MofCode) || e.MofCode == filter.MofCode),
                    take: take,
                    lastSeenId: lastSeenId
                );


                if (!items.Succeeded)
                    return StatusCode(items.Status.code, items.Status.message);

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add(HrJobDto obj)
        {
            var chk = await permission.HasPermission(965, PermissionType.Add);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (!ModelState.IsValid || obj.LevelId <= 0 || obj.DeptId <= 0 || obj.LocationId <= 0 || obj.JobCatagoriesId <= 0 || obj.BranchId <= 0)
            {
                return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));
            }
            try
            {
                var addRes = await hrServiceManager.HrJobService.Add(obj);
                return Ok(addRes);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrJobDto>.FailAsync(ex.Message));
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long Id)
        {
            try
            {
                var chk = await permission.HasPermission(965, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var item = await hrServiceManager.HrJobService.GetOne(x => x.Id == Id);
                if (item.Succeeded)
                {
                    if (item.Data != null)
                    {
                        var result = new HrJobEditDto
                        {
                            Id = item.Data.Id,
                            BranchId = item.Data.BranchId,
                            CreateDate = item.Data.CreateDate,
                            DecDate = item.Data.DecDate,
                            DecNo = item.Data.DecNo,
                            LevelId = item.Data.LevelId,
                            DeptId = item.Data.DeptId,
                            JobCatagoriesId = item.Data.JobCatagoriesId,
                            JobName = item.Data.JobName,
                            JobNo = item.Data.JobNo,
                            LocationId = item.Data.LocationId,
                            StatusId = item.Data.StatusId,
                            Note = item.Data.Note,
                            SectorId = item.Data.SectorId,
                            MofCode = item.Data.MofCode,
                        };
                        return Ok(await Result<HrJobEditDto>.SuccessAsync(result));
                    }
                }
                return Ok(await Result<HrJobEditDto>.FailAsync(item.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrJobEditDto>.FailAsync($"====== Exp in HR job Controller getById, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(HrJobEditDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(965, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid || obj.LevelId <= 0 || obj.DeptId <= 0 || obj.LocationId <= 0 || obj.JobCatagoriesId <= 0 || obj.BranchId <= 0)
                {
                    return Ok(await Result<HrJobEditDto>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));
                }

                var update = await hrServiceManager.HrJobService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrJobEditDto>.FailAsync($"====== Exp in Edit Hr job Controller, MESSAGE: {ex.Message}"));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long Id = 0)
        {
            var chk = await permission.HasPermission(965, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (Id <= 0)
            {
                return Ok(await Result<object>.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));
            }

            try
            {
                var del = await hrServiceManager.HrJobService.Remove(Id);
                return Ok(del);
            }
            catch (Exception exp)
            {
                return Ok(await Result<object>.FailAsync($"{exp.Message}"));
            }
        }
    }
}