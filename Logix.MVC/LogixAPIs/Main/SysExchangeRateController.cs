using Logix.Application.Common;
using Logix.Application.DTOs.Main;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.Main;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Logix.MVC.LogixAPIs.Main
{

    public class SysExchangeRateController : BaseMainApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IPermissionHelper permission;
        private readonly IDDListHelper listHelper;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;

        public SysExchangeRateController(IMainServiceManager mainServiceManager,
            IPermissionHelper permission,
            IDDListHelper listHelper,
            ILocalizationService localization,
            ICurrentData session)
        {
            this.mainServiceManager = mainServiceManager;
            this.permission = permission;
            this.listHelper = listHelper;
            this.session = session;
            this.localization = localization;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult> GetAll()
        {
            try
            {
                var chk = await permission.HasPermission(405, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var items = await mainServiceManager.SysExchangeRateService.GetAllExRateVw();
                if (items.Succeeded)
                {
                    var res = items.Data.Where(r => r.IsDeleted == false).AsQueryable();
                    var final = res.ToList();
                    return Ok(await Result<List<SysExchangeRateVw>>.SuccessAsync(final));
                }

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<List<SysExchangeRateVw>>.FailAsync($"====== Exp in GetAlll SysExchangeRate, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Search")]
        public async Task<ActionResult> Search(SysExchangeRateFilterDto filter)
        {
            try
            {
                var chk = await permission.HasPermission(405, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var items = await mainServiceManager.SysExchangeRateService.GetAllExRateVw();
                if (items.Succeeded)
                {
                    var res = items.Data.Where(r => r.IsDeleted == false).AsQueryable();
                    if (filter == null)
                        return Ok(res);

                    if (filter.CurrencyFromID > 0)
                        res = res.Where(r => r.CurrencyFromId.Equals(filter.CurrencyFromID));

                    if (filter.CurrencyToID > 0)
                        res = res.Where(r => r.CurrencyToId.Equals(filter.CurrencyToID));

                    if (!string.IsNullOrEmpty(filter.ExchangeRate.ToString()))
                        res = res.Where(r => r.ExchangeRate.Equals(filter.ExchangeRate));

                    if (!string.IsNullOrEmpty(filter.FromDate))
                        res = res.Where(r => r.ExchangeDate != null && (DateTime.ParseExact(r.ExchangeDate, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= DateTime.ParseExact(filter.FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture)));
                    
                    if (!string.IsNullOrEmpty(filter.ToDate))
                        res = res.Where(r => r.ExchangeDate != null && (DateTime.ParseExact(r.ExchangeDate, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= DateTime.ParseExact(filter.ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture)));

                    var final = res.ToList();
                    return Ok(await Result<List<SysExchangeRateVw>>.SuccessAsync(final));
                }

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<List<SysExchangeRateVw>>.FailAsync($"====== Exp in Search SysExchangeRate, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Add")]
        public async Task<ActionResult> Add(SysExchangeRateDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(405, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var add = await mainServiceManager.SysExchangeRateService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add SysExchangeRate, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(long id)
        {
            try
            {
                var chk = await permission.HasPermission(405, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));

                var item = await mainServiceManager.SysExchangeRateService.GetOne(e => e.Id == id && e.IsDeleted == false);
                return Ok(item);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add SysExchangeRate, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetByIdForEdit")]
        public async Task<IActionResult> GetByIdForEdit(long id)
        {
            try
            {
                var chk = await permission.HasPermission(405, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));

                var item = await mainServiceManager.SysExchangeRateService.GetForUpdate<SysExchangeRateEditDto>(id);
                return Ok(item);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in GetByIdForEdit SysExchangeRate , MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(SysExchangeRateEditDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(405, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var update = await mainServiceManager.SysExchangeRateService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result<List<SysExchangeRateEditDto>>.FailAsync($"====== Exp in Edit SysExchangeRate, MESSAGE: {ex.Message}"));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var chk = await permission.HasPermission(405, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await mainServiceManager.SysExchangeRateService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result<List<SysExchangeRateDto>>.FailAsync($"====== Exp in Delete SysExchangeRate, MESSAGE: {ex.Message}"));
            }
        }


        [HttpPost("GetExchangeRateByCurrency")]
        public async Task<ActionResult> GetExchangeRateByCurrency(SysExchangeRateForGetByCurrencyFilterDto filter)
        {
            try
            {
                var chk = await permission.HasPermission(405, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var items = await mainServiceManager.SysExchangeRateService.GetAllExRateVw();
                if (items.Succeeded)
                {
                    var res = items.Data.Where(r => r.IsDeleted == false).AsQueryable();
                    if (filter == null)
                        return Ok(res);

                    if (filter.CurrencyFromID > 0)
                        res = res.Where(r => r.CurrencyFromId.Equals(filter.CurrencyFromID));
                    if (!string.IsNullOrEmpty(filter.ExchangeRate.ToString()))
                        res = res.Where(r => r.ExchangeRate.Equals(filter.ExchangeRate));
                    var final = res.OrderBy(x => DateHelper.StringToDate(x.ExchangeDate)).ToList();

                   
                    return Ok(await Result<SysExchangeRateVw>.SuccessAsync(final.FirstOrDefault()));
                }

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<SysExchangeRateVw>.FailAsync($"====== Exp in Search SysExchangeRate, MESSAGE: {ex.Message}"));
            }
        }

    }
}
