using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Logix.MVC.LogixAPIs.HR
{
    //  طلب انتداب
    public class HRMandateRequestController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;

        public HRMandateRequestController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
        }

        #region Index Page

        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrMandateRequestsMasterFilterDto filter)
        {
            var chk = await permission.HasPermission(2041, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                filter.CountryClassificationId ??= 0;
                filter.TypeId ??= 0;
                filter.AppId ??= 0;
                filter.MandateLocationId ??= 0;
                var items = await hrServiceManager.HrMandateRequestsMasterService.GetAllVW(e => e.IsDeleted == false
                    && e.FacilityId == session.FacilityId
                    && (filter.TypeId == 0 || e.TypeId == filter.TypeId)
                    && (filter.AppId == 0 || e.AppId == filter.AppId)
                    && (filter.MandateLocationId == 0 || e.MandateLocationId == filter.MandateLocationId)
                    && (filter.CountryClassificationId == 0 || e.CountryClassificationId == filter.CountryClassificationId)
                    && (string.IsNullOrEmpty(filter.Name) || (e.Name != null && e.Name.ToLower().Contains(filter.Name.ToLower())))
                    && (string.IsNullOrEmpty(filter.Name2) || (e.Name2 != null && e.Name2.ToLower().Contains(filter.Name2.ToLower())))
                    && (string.IsNullOrEmpty(filter.FromLocation) || (e.FromLocation != null && e.FromLocation.ToLower().Contains(filter.FromLocation.ToLower())))
                    && (string.IsNullOrEmpty(filter.ToLocation) || (e.ToLocation != null && e.ToLocation.ToLower().Contains(filter.ToLocation.ToLower())))
                    && (string.IsNullOrEmpty(filter.Note) || (e.Note != null && e.Note.ToLower().Contains(filter.Note.ToLower())))
                );

                if (items.Succeeded)
                {
                    if (items.Data.Count() > 0)
                    {
                        var res = items.Data.AsQueryable();
                        return Ok(await Result<List<HrMandateRequestsMasterVw>>.SuccessAsync(res.ToList(), ""));
                    }
                    return Ok(await Result<List<object>>.SuccessAsync(new List<object>(), localization.GetResource1("NosearchResult")));
                }
                return Ok(await Result<object>.FailAsync(items.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync(ex.Message));
            }
        }


        [HttpPost("GetPagination")]
        public async Task<IActionResult> GetPagination(HrMandateRequestsMasterFilterDto filter, int take = Pagination.take, long? lastSeenId = null)
        {
            var chk = await permission.HasPermission(2041, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                filter.CountryClassificationId ??= 0;
                filter.TypeId ??= 0;
                filter.AppId ??= 0;
                filter.MandateLocationId ??= 0;

                var items = await hrServiceManager.HrMandateRequestsMasterService.GetAllWithPaginationVW(
                    selector: x => x.Id,
                    expression: e => e.IsDeleted == false
                                && e.FacilityId == session.FacilityId
                                && (filter.TypeId == 0 || e.TypeId == filter.TypeId)
                                && (filter.AppId == 0 || e.AppId == filter.AppId)
                                && (filter.MandateLocationId == 0 || e.MandateLocationId == filter.MandateLocationId)
                                && (filter.CountryClassificationId == 0 || e.CountryClassificationId == filter.CountryClassificationId)
                                && (string.IsNullOrEmpty(filter.Name) || (e.Name != null && e.Name.ToLower().Contains(filter.Name.ToLower())))
                                && (string.IsNullOrEmpty(filter.Name2) || (e.Name2 != null && e.Name2.ToLower().Contains(filter.Name2.ToLower())))
                                && (string.IsNullOrEmpty(filter.FromLocation) || (e.FromLocation != null && e.FromLocation.ToLower().Contains(filter.FromLocation.ToLower())))
                                && (string.IsNullOrEmpty(filter.ToLocation) || (e.ToLocation != null && e.ToLocation.ToLower().Contains(filter.ToLocation.ToLower())))
                                && (string.IsNullOrEmpty(filter.Note) || (e.Note != null && e.Note.ToLower().Contains(filter.Note.ToLower()))),
                    take: take,
                    lastSeenId: lastSeenId
                );

                if (!items.Succeeded)
                    return StatusCode(items.Status.code, items.Status.message);

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync(ex.Message));
            }
        }


        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var chk = await permission.HasPermission(2041, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await hrServiceManager.HrMandateRequestsMasterService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete HRMandateRequestController  Controller, MESSAGE: {ex.Message}"));
            }
        }


        [HttpGet("GetById")]

        public async Task<IActionResult> Edit(long Id)
        {
            try
            {
                var chk = await permission.HasPermission(2041, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var item = await hrServiceManager.HrMandateRequestsMasterService.GetOneVW(x => x.Id == Id && x.IsDeleted == false && x.FacilityId == session.FacilityId);
                if (item.Succeeded && item.Data != null)
                {
                    var newObject = new
                    {
                        Id = item.Data.Id,
                        Name = item.Data.Name,
                        Name2 = item.Data.Name2,
                        Description = item.Data.Note,
                        Objective = item.Data.Objective,
                        FromLocation = item.Data.FromLocation,
                        ToLocation = item.Data.ToLocation,
                        CountryClassificationId = item.Data.CountryClassificationId,
                        MandateLocationId = item.Data.MandateLocationId,
                        TypeId = item.Data.TypeId,
                        FromDate = (item.Data.FromDate.HasValue) ? item.Data.FromDate.Value.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture) : "",
                        ToDate = (item.Data.ToDate.HasValue) ? item.Data.ToDate.Value.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture) : "",

                    };

                    var getDetails = await hrServiceManager.HrMandateRequestsDetaileService.GetAllVW(x => x.MrId == Id);
                    return Ok(await Result<object>.SuccessAsync(new { item = newObject, Details = getDetails.Data.ToList() }));
                }
                return Ok(await Result<object>.FailAsync(item.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrLeaveVw>.FailAsync($"====== Exp in HRMandateRequestController  getById, MESSAGE: {ex.Message}"));
            }
        }

        #endregion


        #region Add Page

        //[HttpPost("OnJobLevelChanged")]
        //public async Task<ActionResult> OnJobLevelChanged(/*int TypeId,*/ int LevelId)
        //{
        //    try
        //    {
        //        var chkAdd = await permission.HasPermission(2041, PermissionType.Add);
        //        var chkEdit = await permission.HasPermission(2041, PermissionType.Edit);
        //        if (!chkAdd && !chkEdit)
        //            return Ok(await Result.AccessDenied("AccessDenied"));

        //        // Uncomment this block in the future if TypeId is needed
        //        // if (TypeId <= 0)
        //        //     return Ok(await Result<object>.FailAsync($"{localization.GetHrResource("SelectDelegation")}"));

        //        if (LevelId <= 0)
        //            return Ok(await Result<object>.SuccessAsync($""));

        //        var jobLevelResult = await hrServiceManager.HrJobLevelService.GetOne(x => x.Id == LevelId);
        //        var jobLevel = jobLevelResult.Data;

        //        if (jobLevel == null)
        //            return Ok(await Result<object>.SuccessAsync($""));

        //        var res = new HrMandateLocationDetaileDto
        //        {
        //            TransportAmount = jobLevel.TransportAmount > 0 ? (jobLevel.TransportAmount) / 30 : 0,
        //            RatePerNight = jobLevel.RatePerNight > 0 ? (jobLevel.RatePerNight / 30) : 0,
        //            TicketValue = jobLevel.TicketValue > 0 ? jobLevel.TicketValue : 0,
        //            JobLevelId = jobLevel.Id,
        //            AllowanceValue = (LevelId switch { 1 => jobLevel.Mandate, 2 => jobLevel.MandateOut, _ => 0 })
        //        };

        //        return Ok(await Result<HrMandateLocationDetaileDto>.SuccessAsync(res, $""));
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(await Result.FailAsync($"Error in OnJobLevelChanged: {ex.Message}"));
        //    }
        //}


        #endregion

        #region Edit Page

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(HrMandateRequestsMasterEditDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(2041, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (string.IsNullOrEmpty(obj.FromDateStr))
                    return Ok(await Result<object>.FailAsync($" يجب ادخال  من تاريخ   "));
                if (string.IsNullOrEmpty(obj.ToDateStr))
                    return Ok(await Result<object>.FailAsync($"يجب ادخال   الى تاريخ"));
                if (string.IsNullOrEmpty(obj.Objective))
                    return Ok(await Result<object>.FailAsync($"يجب ادخال   الغرض من الانتداب"));

                var update = await hrServiceManager.HrMandateRequestsMasterService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrMandateRequestsMasterEditDto>.FailAsync($"====== Exp in HRMandateRequestController Controller Edit, MESSAGE: {ex.Message}"));
            }
        }

        #endregion

    }
}