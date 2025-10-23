using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.HR
{

    // جهة الإنتداب
    public class HRMandateLocationController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;

        public HRMandateLocationController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
        }

        #region Index Page

        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrMandateLocationFilterDto filter)
        {
            var chk = await permission.HasPermission(2014, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                filter.CountryClassificationId ??= 0;
                filter.TypeId ??= 0;
                var items = await hrServiceManager.HrMandateLocationMasterService.GetAllVW(e => e.IsDeleted == false
                    && e.FacilityId == session.FacilityId
                    && (filter.TypeId == 0 || e.TypeId == filter.TypeId)
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
                        return Ok(await Result<List<HrMandateLocationMasterVw>>.SuccessAsync(res.ToList(), ""));
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
        public async Task<IActionResult> GetPagination(HrMandateLocationFilterDto filter, int take = Pagination.take, long? lastSeenId = null)
        {
            var chk = await permission.HasPermission(2014, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                filter.CountryClassificationId ??= 0;
                filter.TypeId ??= 0;

                var items = await hrServiceManager.HrMandateLocationMasterService.GetAllWithPaginationVW(
                    selector: x => x.Id,
                    expression: e => e.IsDeleted == false
                                && e.FacilityId == session.FacilityId
                                && (filter.TypeId == 0 || e.TypeId == filter.TypeId)
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
                var chk = await permission.HasPermission(2014, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await hrServiceManager.HrMandateLocationMasterService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete HRMandateLocation  Controller, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long Id)
        {
            try
            {
                var chk = await permission.HasPermission(2014, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var item = await hrServiceManager.HrMandateLocationMasterService.GetOneVW(x => x.Id == Id && x.IsDeleted == false && x.FacilityId == session.FacilityId);
                if (item.Succeeded)
                {
                    var getDetails = await hrServiceManager.HrMandateLocationDetaileService.GetAllVW(x => x.MlId == Id);

                    return Ok(await Result<object>.SuccessAsync(new { item = item.Data, Details = getDetails.Data.ToList() }));
                }
                return Ok(await Result<object>.FailAsync(item.Status.message));

            }
            catch (Exception ex)
            {
                return Ok(await Result<HrLeaveVw>.FailAsync($"====== Exp in HRMandateLocation  getById, MESSAGE: {ex.Message}"));
            }
        }



        #endregion


        #region Add Page


        [HttpPost("Add")]
        public async Task<ActionResult> Add(HrMandateLocationMasterDto obj)
        {
            try
            {
                obj.CountryClassificationId ??= 0;
                var chk = await permission.HasPermission(2014, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (string.IsNullOrEmpty(obj.Name))
                    return Ok(await Result<object>.FailAsync($" يجب ادخال اسم الجهة بالعربي   "));
                if (string.IsNullOrEmpty(obj.Name2))
                    return Ok(await Result<object>.FailAsync($"يجب ادخال اسم الجهة بالانجليزي  "));

                if (string.IsNullOrEmpty(obj.FromLocation))
                    return Ok(await Result<object>.FailAsync($"يجب ادخال من الجهة  "));

                if (string.IsNullOrEmpty(obj.ToLocation))
                    return Ok(await Result<object>.FailAsync($"يجب ادخال الى الجهة  "));

                if (string.IsNullOrEmpty(obj.Note))
                    return Ok(await Result<object>.FailAsync($"يجب ادخال الوصف  "));

                if (obj.TypeId <= 0)
                    return Ok(await Result<object>.FailAsync($" يجب ادخال نوع الانتداب  "));

                if (obj.Detaile.Count <= 0)
                {
                    return Ok(await Result<object>.FailAsync($" {localization.GetHrResource("AddLevel")}"));

                }

                if (obj.Detaile.Any(x => x.JobLevelId <= 0))
                {
                    return Ok(await Result<object>.FailAsync($" يجب ادخال  المستوى  لجميع الصفوف "));
                }
                var add = await hrServiceManager.HrMandateLocationMasterService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add HRMandateLocation  Controller, MESSAGE: {ex.Message}"));
            }
        }




        [HttpPost("OnJobLevelChanged")]
        public async Task<ActionResult> OnJobLevelChanged(/*int TypeId,*/ int LevelId)
        {
            try
            {
                var chkAdd = await permission.HasPermission(2014, PermissionType.Add);
                var chkEdit = await permission.HasPermission(2014, PermissionType.Edit);
                if (!chkAdd && !chkEdit)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                // Uncomment this block in the future if TypeId is needed
                // if (TypeId <= 0)
                //     return Ok(await Result<object>.FailAsync($"{localization.GetHrResource("SelectDelegation")}"));

                if (LevelId <= 0)
                    return Ok(await Result<object>.SuccessAsync($""));

                var jobLevelResult = await hrServiceManager.HrJobLevelService.GetOne(x => x.Id == LevelId);
                var jobLevel = jobLevelResult.Data;

                if (jobLevel == null)
                    return Ok(await Result<object>.SuccessAsync($""));

                var res = new HrMandateLocationDetaileDto
                {
                    TransportAmount = jobLevel.TransportAmount > 0 ? (jobLevel.TransportAmount) / 30 : 0,
                    RatePerNight = jobLevel.RatePerNight > 0 ? (jobLevel.RatePerNight / 30) : 0,
                    TicketValue = jobLevel.TicketValue > 0 ? jobLevel.TicketValue : 0,
                    JobLevelId = jobLevel.Id,
                    AllowanceValue = (LevelId switch { 1 => jobLevel.Mandate, 2 => jobLevel.MandateOut, _ => 0 })
                };

                return Ok(await Result<HrMandateLocationDetaileDto>.SuccessAsync(res, $""));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"Error in OnJobLevelChanged: {ex.Message}"));
            }
        }


        #endregion

        #region Edit Page

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(HrMandateLocationMasterEditDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(2014, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (string.IsNullOrEmpty(obj.Name))
                    return Ok(await Result<object>.FailAsync($" يجب ادخال اسم الجهة بالعربي   "));
                if (string.IsNullOrEmpty(obj.Name2))
                    return Ok(await Result<object>.FailAsync($"يجب ادخال اسم الجهة بالانجليزي  "));

                if (string.IsNullOrEmpty(obj.FromLocation))
                    return Ok(await Result<object>.FailAsync($"يجب ادخال من الجهة  "));

                if (string.IsNullOrEmpty(obj.ToLocation))
                    return Ok(await Result<object>.FailAsync($"يجب ادخال الى الجهة  "));

                if (string.IsNullOrEmpty(obj.Note))
                    return Ok(await Result<object>.FailAsync($"يجب ادخال الوصف  "));

                if (obj.TypeId <= 0)
                    return Ok(await Result<object>.FailAsync($" يجب ادخال نوع الانتداب  "));


                var update = await hrServiceManager.HrMandateLocationMasterService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrMandateLocationMasterEditDto>.FailAsync($"====== Exp in HRMandateLocation Controller Edit, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("AddDetails")]
        public async Task<ActionResult> AddDetails(HrMandateLocationDetaileDto obj)
        {
            try
            {
                obj.MlId ??= 0;
                obj.JobLevelId ??= 0;
                var chk = await permission.HasPermission(2014, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (obj.MlId <= 0)
                    return Ok(await Result<object>.FailAsync($" يجب ادخال   رقم الجهة "));

                if (obj.JobLevelId <= 0)
                    return Ok(await Result<object>.FailAsync($" يجب ادخال المستوى "));

                var add = await hrServiceManager.HrMandateLocationDetaileService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in AddDetails HRMandateLocation  Controller, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("EditDetails")]
        public async Task<ActionResult> EditDetails(HrMandateLocationDetaileEditDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(2014, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (obj.MlId <= 0)
                    return Ok(await Result<object>.FailAsync($" يجب ادخال   رقم الجهة "));

                if (obj.JobLevelId <= 0)
                    return Ok(await Result<object>.FailAsync($" يجب ادخال المستوى "));

                var update = await hrServiceManager.HrMandateLocationDetaileService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in EditDetails HRMandateLocation  Controller, MESSAGE: {ex.Message}"));
            }
        }

        [HttpDelete("DeleteDetails")]
        public async Task<IActionResult> DeleteDetails(long id)
        {
            try
            {
                var chk = await permission.HasPermission(2014, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await hrServiceManager.HrMandateLocationDetaileService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in DeleteDetails HRMandateLocation  Controller, MESSAGE: {ex.Message}"));
            }
        }

        #endregion

    }

}