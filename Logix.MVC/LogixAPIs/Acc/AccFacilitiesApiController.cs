using Logix.Application.Common;
using Logix.Application.DTOs.ACC;
using Logix.Application.DTOs.Main;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Services;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;
using Logix.Domain.Main;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.Acc
{
    public class AccFacilitiesApiController : BaseAccApiController
    {
        private readonly IPermissionHelper permission;
        private readonly IAccServiceManager accServiceManager;
        private readonly ILocalizationService localization;
        private readonly ICurrentData session;

        public AccFacilitiesApiController(IPermissionHelper permission,
            IAccServiceManager accServiceManager,
            ILocalizationService localization,
            ICurrentData session)
        {
            this.permission = permission;
            this.accServiceManager = accServiceManager;
            this.localization = localization;
            this.session = session;
        }

        [HttpPost("Search")]
        public async Task<ActionResult> Search(AccFacilityFilterDto filter)
        {
            try
            {
                var chk = await permission.HasPermission(496, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var items = await accServiceManager.AccFacilityService.GetAllVW(f => f.FlagDelete == false);
                if (items.Succeeded)
                {
                    if (filter == null)
                    {
                        return Ok(items);
                    }

                    var res = items.Data.OrderBy(f => f.FacilityId).AsQueryable();

                    if (!string.IsNullOrEmpty(filter.FacilityName))
                        res = res.Where(r => (!string.IsNullOrEmpty(r.FacilityName) && r.FacilityName.Contains(filter.FacilityName)) || (!string.IsNullOrEmpty(r.FacilityName2) && r.FacilityName2.ToLower().Contains(filter.FacilityName.ToLower())));

                    if (!string.IsNullOrEmpty(filter.IdNumber))
                        res = res.Where(r => !string.IsNullOrEmpty(r.IdNumber) && r.IdNumber.Equals(filter.IdNumber));

                    if (!string.IsNullOrEmpty(filter.NoLabourOfficeFile))
                        res = res.Where(r => !string.IsNullOrEmpty(r.NoLabourOfficeFile) && r.NoLabourOfficeFile.Equals(filter.NoLabourOfficeFile));

                    if (!string.IsNullOrEmpty(filter.FacilityPhone))
                        res = res.Where(r => !string.IsNullOrEmpty(r.FacilityPhone) && r.FacilityPhone.Equals(filter.FacilityPhone));

                    if (!string.IsNullOrEmpty(filter.FacilityEmail))
                        res = res.Where(r => !string.IsNullOrEmpty(r.FacilityEmail) && r.FacilityEmail.Contains(filter.FacilityEmail));

                    if (!string.IsNullOrEmpty(filter.CommissionerName))
                        res = res.Where(r => !string.IsNullOrEmpty(r.CommissionerName) && r.CommissionerName.Contains(filter.CommissionerName));

                    if (!string.IsNullOrEmpty(filter.FacilityAddress))
                        res = res.Where(r => !string.IsNullOrEmpty(r.FacilityAddress) && r.FacilityAddress.Contains(filter.FacilityAddress));

                    var final = res.ToList();
                    return Ok(await Result<List<AccFacilitiesVw>>.SuccessAsync(final));
                }
                else
                {
                    return Ok(items);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Search AccFacilitiesApiController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("GetAll")]
        public async Task<ActionResult> GetAll()
        {
            try
            {
                var chk = await permission.HasPermission(496, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var items = await accServiceManager.AccFacilityService.GetAllVW(f => f.FlagDelete == false);
                if (items.Succeeded)
                {
                    var res = items.Data.OrderBy(f => f.FacilityId).AsQueryable();

                    var final = res.ToList();
                    return Ok(await Result<List<AccFacilitiesVw>>.SuccessAsync(final));
                }
                else
                {
                    return Ok(items);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in GetAll AccFacilitiesApiController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetByIdForEdit")]
        public async Task<IActionResult> GetByIdForEdit(long id)
        {
            try
            {
                var chk = await permission.HasPermission(496, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var getItem = await accServiceManager.AccFacilityService.GetForUpdate<AccFacilityEditDto>(id);
                return Ok(getItem);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in GetByIdForEdit AccFacilitiesApiController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(AccFacilityEditDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(496, PermissionType.Edit);
                if (!chk)
                {
                    return Ok(await Result.AccessDenied("AccessDenied"));
                }
                if (!ModelState.IsValid)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));
                }

                var update = await accServiceManager.AccFacilityService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Edit AccFacilitiesApiController, MESSAGE: {ex.Message}"));
            }
        }


        [HttpGet("GetFacilityStamp")]
        public async Task<ActionResult> GetFacilityStamp(long id)
        {
            var GetStamp = await accServiceManager.AccFacilityService.GetOne(s => s.Stamp, f => f.FacilityId == id);
            return Ok(GetStamp);
        }

        [HttpGet("EditStamp")]
        public async Task<ActionResult> EditStamp(long id, string newStampUrl)
        {
            try
            {
                var update = await accServiceManager.AccFacilityService.UpdateStamp(id, newStampUrl);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in EditStamp AccFacilitiesApiController, MESSAGE: {ex.Message}"));
            }
        }


        [HttpPost("GetMyFacilityProfile")]
        public async Task<ActionResult> GetMyFacilityProfile()
        {
            try
            {
                long facilityId = session.FacilityId;
                var myFacility = await accServiceManager.AccFacilityService.GetForUpdateProfile(facilityId);
                return Ok(myFacility);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in GetMyFacilityProfile AccFacilitiesApiController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("EditProfile")]
        public async Task<ActionResult> EditProfile(AccFacilityEditProfileDto obj)
        {
            try
            {
                var update = await accServiceManager.AccFacilityService.UpdateProfile(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in EditProfile AccFacilitiesApiController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("GetAccFacility")]
        public async Task<ActionResult> GetAccFacility()
        {
            try
            {
                long facilityId = session.FacilityId;
                var myFacility = await accServiceManager.AccFacilityService.GetOneVW(x=>x.FacilityId== facilityId);
               
                return Ok(await Result<AccFacilitiesVw>.SuccessAsync(myFacility.Data));

            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in GetMyFacilityProfile AccFacilitiesApiController, MESSAGE: {ex.Message}"));
            }
        }
    }
}
