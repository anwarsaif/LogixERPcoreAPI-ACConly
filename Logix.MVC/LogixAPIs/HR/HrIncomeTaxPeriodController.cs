using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.HR
{
    public class HrIncomeTaxPeriodController : BaseHrApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly IDDListHelper listHelper;
        private readonly ILocalizationService localization;

        public HrIncomeTaxPeriodController(IHrServiceManager hrServiceManager, IMainServiceManager mainServiceManager, IPermissionHelper permission, ICurrentData session, IDDListHelper listHelper, ILocalizationService localization)
        {
            this.mainServiceManager = mainServiceManager;
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.listHelper = listHelper;
            this.localization = localization;
        }

        //[HttpPost("Search")]
        //public async Task<IActionResult> Search(HrIncomeTaxPeriodFilterDto filter)
        //{
        //    var chk = await permission.HasPermission(1977, PermissionType.Show);
        //    if (!chk)
        //    {
        //        return Ok(await Result.AccessDenied("AccessDenied"));
        //    }
        //    try
        //    {
        //        var items = await hrServiceManager.HrIncomeTaxPeriodService.GetAllVW(e => e.IsDeleted == false &&
        //            (string.IsNullOrEmpty(filter.TaxName) || e.TaxName.Contains(filter.TaxName)) &&
        //            (string.IsNullOrEmpty(filter.TaxCode) || e.TaxCode.Contains(filter.TaxCode))
        //        );
        //        if (items.Succeeded)
        //        {
        //            return Ok(items);
        //        }

        //        return Ok(items);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(await Result.FailAsync(ex.Message));
        //    }
        //}

        [HttpPost("Add")]
        public async Task<IActionResult> Add(HrIncomeTaxPeriodDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(1977, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync(localization.GetMessagesResource("dataRequire")));

                var add = await hrServiceManager.HrIncomeTaxPeriodService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(long id)
        {
            try
            {
                var chk = await permission.HasPermission(1799, PermissionType.Show);
                if (!chk)
                {
                    return Ok(await Result.AccessDenied("AcessDenied"));
                }

                if (id <= 0)
                {
                    return Ok(await Result<HrIncomeTaxPeriodDto>.FailAsync(localization.GetMessagesResource("NoIdInUpdate")));
                }

                var getItem = await hrServiceManager.HrIncomeTaxPeriodService.GetOne(s => s.Id == id);
                if (getItem.Succeeded)
                {
                    var obj = getItem.Data;
                    return Ok(await Result<HrIncomeTaxPeriodDto>.SuccessAsync(obj, $""));
                }
                else
                {
                    return Ok(getItem);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrIncomeTaxPeriodDto>.FailAsync($"====== Exp MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetByIncomeTaxId")]
        public async Task<IActionResult> GetByIncomeTaxId(int id)
        {
            try
            {
                var chk = await permission.HasPermission(1977, PermissionType.Show);
                if (!chk)
                {
                    return Ok(await Result.AccessDenied("AcessDenied"));
                }

                if (id <= 0)
                {
                    return Ok(await Result<HrIncomeTaxPeriodDto>.FailAsync(localization.GetMessagesResource("NoIdInUpdate")));
                }

                var getItem = await hrServiceManager.HrIncomeTaxPeriodService.GetAll(s => s.IsDeleted == false && s.IncomeTaxId == id);
                if (getItem.Succeeded)
                {
                    var obj = getItem.Data;
                    return Ok(await Result<IEnumerable<HrIncomeTaxPeriodDto>>.SuccessAsync(getItem.Data));
                }
                else
                {
                    return Ok(getItem);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrIncomeTaxPeriodDto>.FailAsync($"====== Exp MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(HrNotificationsSettingEditDto obj)
        {
            var chk = await permission.HasPermission(1977, PermissionType.Edit);
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
                var addRes = await hrServiceManager.HrNotificationsSettingService.Update(obj);
                if (addRes.Succeeded)
                {
                    return Ok(addRes);
                }

                else
                {
                    return Ok(await Result.FailAsync($"{addRes.Status.message}"));
                }
            }

            catch (Exception ex)
            {
                return Ok(await Result<HrNotificationsSettingEditDto>.FailAsync($"{ex.Message}"));
            }
        }


        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long Id = 0)
        {
            var chk = await permission.HasPermission(535, PermissionType.Delete);
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
                var del = await hrServiceManager.HrNotificationsSettingService.Remove(Id);
                if (del.Succeeded)
                {

                    return Ok(await Result.SuccessAsync("Item deleted successfully"));
                }
                return Ok(await Result.FailAsync($"{del.Status.message}"));
            }
            catch (Exception exp)
            {
                return Ok(await Result.FailAsync($"{exp.Message}"));
            }
        }
    }
}
