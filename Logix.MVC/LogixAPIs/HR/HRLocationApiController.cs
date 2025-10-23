using DocumentFormat.OpenXml.Office2010.Excel;
using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.HR.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.HR
{
    //مواقع التحضير 
    public class HRLocationApiController : BaseHrApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        public HRLocationApiController(IHrServiceManager hrServiceManager, IMainServiceManager mainServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization)
        {
            this.mainServiceManager = mainServiceManager;
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
        }

        [NonAction]
        private void setErrors()
        {
            var errors = new ErrorsHelper(ModelState);
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var chk = await permission.HasPermission(552, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var items = await hrServiceManager.HrAttLocationService.GetAll(e => e.IsDeleted == false);
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    res = res.OrderBy(e => e.Id);
                    return Ok(await Result<List<HrAttLocationDto>>.SuccessAsync(res.ToList(), items.Status.message));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }


        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrAttLocationFilterVM filter)
        {
            var chk = await permission.HasPermission(552, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var items = await hrServiceManager.HrAttLocationService.GetAll(e => e.IsDeleted == false
                && (string.IsNullOrEmpty(filter.LocationName) || (e.LocationName != null && e.LocationName.Contains(filter.LocationName)))
                );


                if (!items.Succeeded)
                    return Ok(await Result<object>.FailAsync(items.Status.message));

                if (!items.Data.Any())
                    return Ok(await Result<object>.SuccessAsync(new List<object>(), localization.GetResource1("NosearchResult")));

                var res = items.Data.AsQueryable();
                res = res.OrderBy(e => e.Id);
                return Ok(await Result<object>.SuccessAsync(res, ""));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add(HrAttLocationDto obj)
        {

            try
            {
                obj.ProjectId ??= 0;
                var chk = await permission.HasPermission(552, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (string.IsNullOrEmpty(obj.LocationName)) return Ok(await Result<object>.FailAsync(localization.GetCommonResource("name")));
                if (string.IsNullOrEmpty(obj.Latitude)) return Ok(await Result<object>.FailAsync(localization.GetHrResource("Latitude")));
                if (string.IsNullOrEmpty(obj.Longitude)) return Ok(await Result<object>.FailAsync(localization.GetHrResource("Longitude")));

                var addRes = await hrServiceManager.HrAttLocationService.Add(obj);
                return Ok(addRes);

            }

            catch (Exception ex)
            {
                return Ok(await Result<HrAttLocationDto>.FailAsync(ex.Message));
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long Id = 0)
        {
            setErrors();
            var chk = await permission.HasPermission(552, PermissionType.Edit);
            if (!chk)
            {
                return Ok(Result<HrAttLocationEditDto>.FailAsync($"Access Denied"));
            }
            if (Id <= 0)
            {
                return Ok(Result<HrAttLocationEditDto>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
            }

            try
            {
                var getItem = await hrServiceManager.HrAttLocationService.GetForUpdate<HrAttLocationEditDto>(Id);
                if (getItem.Succeeded && getItem.Data != null)
                {
                    return Ok(getItem);
                }
                return Ok(Result<HrAttLocationEditDto>.FailAsync(getItem.Status.message));
            }
            catch (Exception exp)
            {
                return Ok(Result<HrAttLocationEditDto>.FailAsync($"EXP in {this.GetType()}, Meesage: {exp.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(HrAttLocationEditDto obj)
        {

            try
            {
                var chk = await permission.HasPermission(552, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (obj.Id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));

                if (string.IsNullOrEmpty(obj.LocationName)) return Ok(await Result<object>.FailAsync(localization.GetCommonResource("name")));
                if (string.IsNullOrEmpty(obj.Latitude)) return Ok(await Result<object>.FailAsync(localization.GetHrResource("Latitude")));
                if (string.IsNullOrEmpty(obj.Longitude)) return Ok(await Result<object>.FailAsync(localization.GetHrResource("Longitude")));

                var res = await hrServiceManager.HrAttLocationService.Update(obj);
                return Ok(res);
            }

            catch (Exception ex)
            {
                return Ok(await Result<HrAttLocationEditDto>.FailAsync($"{ex.Message}"));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var chk = await permission.HasPermission(552, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await hrServiceManager.HrAttLocationService.Remove(id);

                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"======= Exp in {this.GetType()}: {ex.Message}"));
            }
        }


    }
}