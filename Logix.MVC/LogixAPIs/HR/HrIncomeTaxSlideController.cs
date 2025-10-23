using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.HR
{
    public class HrIncomeTaxSlideController : BaseHrApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly IDDListHelper listHelper;
        private readonly ILocalizationService localization;

        public HrIncomeTaxSlideController(IHrServiceManager hrServiceManager, IMainServiceManager mainServiceManager, IPermissionHelper permission, ICurrentData session, IDDListHelper listHelper, ILocalizationService localization)
        {
            this.mainServiceManager = mainServiceManager;
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.listHelper = listHelper;
            this.localization = localization;
        }

        //[HttpPost("Search")]
        //public async Task<IActionResult> Search(HrIncomeTaxFilterDto filter)
        //{
        //    var chk = await permission.HasPermission(1977, PermissionType.Show);
        //    if (!chk)
        //    {
        //        return Ok(await Result.AccessDenied("AccessDenied"));
        //    }
        //    try
        //    {
        //        var items = await hrServiceManager.HrIncomeTaxSlideService.GetAll(e => e.IsDeleted == false &&
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
        public async Task<IActionResult> Add(HrIncomeTaxSlideDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(1977, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync(localization.GetMessagesResource("dataRequire")));

                var add = await hrServiceManager.HrIncomeTaxSlideService.Add(obj);
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
                var chk = await permission.HasPermission(1977, PermissionType.Show);
                if (!chk)
                {
                    return Ok(await Result.AccessDenied("AcessDenied"));
                }

                if (id <= 0)
                {
                    return Ok(await Result<HrIncomeTaxSlideDto>.FailAsync(localization.GetMessagesResource("NoIdInUpdate")));
                }

                var getItem = await hrServiceManager.HrIncomeTaxSlideService.GetOne(s => s.Id == id);
                if (getItem.Succeeded)
                {
                    var obj = getItem.Data;
                    return Ok(await Result<HrIncomeTaxSlideDto>.SuccessAsync(obj, $""));
                }
                else
                {
                    return Ok(getItem);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrIncomeTaxSlideDto>.FailAsync($"====== Exp MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetByIncomeTaxPeriodId")]
        public async Task<IActionResult> GetByIncomeTaxPeriodId(long id)
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
                    return Ok(await Result<IEnumerable<HrIncomeTaxSlideDto>>.FailAsync(localization.GetMessagesResource("NoIdInUpdate")));
                }

                var getItem = await hrServiceManager.HrIncomeTaxSlideService.GetAll(s => s.IncomeTaxPeriodId == id && s.IsDeleted == false);
                if (getItem.Succeeded)
                {
                    var obj = getItem.Data;
                    return Ok(await Result<IEnumerable<HrIncomeTaxSlideDto>>.SuccessAsync(obj, $""));
                }
                else
                {
                    return Ok(getItem);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<IEnumerable<HrIncomeTaxSlideDto>>.FailAsync($"====== Exp MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(HrIncomeTaxSlideEditDto obj)
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
                var addRes = await hrServiceManager.HrIncomeTaxSlideService.Update(obj);
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
                return Ok(await Result<HrIncomeTaxSlideEditDto>.FailAsync($"{ex.Message}"));
            }
        }


        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(int Id)
        {
            var chk = await permission.HasPermission(1977, PermissionType.Delete);
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
                var del = await hrServiceManager.HrIncomeTaxSlideService.Remove(Id);
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
