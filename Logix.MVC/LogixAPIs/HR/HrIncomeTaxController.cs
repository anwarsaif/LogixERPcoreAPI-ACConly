using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.HR
{
    public class HrIncomeTaxController : BaseHrApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly IDDListHelper listHelper;
        private readonly ILocalizationService localization;
        private readonly IAccServiceManager accServiceManager;

        public HrIncomeTaxController(IHrServiceManager hrServiceManager, IMainServiceManager mainServiceManager, IPermissionHelper permission, ICurrentData session, IDDListHelper listHelper, ILocalizationService localization, IAccServiceManager accServiceManager)
        {
            this.mainServiceManager = mainServiceManager;
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.listHelper = listHelper;
            this.localization = localization;
            this.accServiceManager = accServiceManager;
        }




        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrIncomeTaxFilterDto filter)
        {
            var chk = await permission.HasPermission(1977, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var items = await hrServiceManager.HrIncomeTaxService.GetAllVW(e => e.IsDeleted == false &&
                    (string.IsNullOrEmpty(filter.TaxName) || e.TaxName.Contains(filter.TaxName)) &&
                    (string.IsNullOrEmpty(filter.TaxCode) || e.TaxCode.Contains(filter.TaxCode))
                );
                if (items.Succeeded)
                {
                    return Ok(items);
                }

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpPost("GetPagination")]
        public async Task<IActionResult> GetPagination([FromBody] HrIncomeTaxFilterDto filter, int take = Pagination.take, int? lastSeenId = null)
        {
            var chk = await permission.HasPermission(1977, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {


                var items = await hrServiceManager.HrIncomeTaxService.GetAllWithPaginationVW(
                    selector: e => e.Id,
                    expression: e =>
                        e.IsDeleted == false &&
                    (string.IsNullOrEmpty(filter.TaxName) || e.TaxName.Contains(filter.TaxName)) &&
                    (string.IsNullOrEmpty(filter.TaxCode) || e.TaxCode.Contains(filter.TaxCode))


                            ,
                    take: take,
                    lastSeenId: lastSeenId
                );

                if (!items.Succeeded)
                    return Ok(await Result<List<HrIncomeTaxVw>>.FailAsync(items.Status.message));

                if (items.Data == null || !items.Data.Any())
                    return Ok(await Result<List<HrIncomeTaxVw>>.SuccessAsync(new List<HrIncomeTaxVw>()));

                var res = items.Data.OrderBy(x => x.Id).ToList();

                var paginatedData = new PaginatedResult<object>
                {
                    Succeeded = items.Succeeded,
                    Data = res,
                    Status = items.Status,
                    PaginationInfo = items.PaginationInfo
                };

                return Ok(paginatedData);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }


        [HttpPost("Add")]
        public async Task<IActionResult> Add(HrIncomeTaxDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(1977, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync(localization.GetMessagesResource("dataRequire")));

                var add = await hrServiceManager.HrIncomeTaxService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var chk = await permission.HasPermission(243, PermissionType.Show);
                if (!chk)
                {
                    return Ok(await Result.AccessDenied("AcessDenied"));
                }

                if (id <= 0)
                {
                    return Ok(await Result<HrIncomeTaxDto>.FailAsync(localization.GetMessagesResource("NoIdInUpdate")));
                }

                var getItem = await hrServiceManager.HrIncomeTaxService.GetById(id);
                var account = await accServiceManager.AccAccountsSubHelpeVwService.GetOne(x => x.AccAccountCode, x => x.AccAccountId == getItem.Data.AccountId && x.Isdel == false && x.SystemId == 2 && x.FacilityId == session.FacilityId && x.IsActive == true);
                if (account.Succeeded)
                {
                    getItem.Data.AccountCode = account.Data.ToString();
                }

                if (getItem.Succeeded)
                {
                    var obj = getItem.Data;
                    return Ok(await Result<HrIncomeTaxDto>.SuccessAsync(obj, $""));
                }
                else
                {
                    return Ok(getItem);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrIncomeTaxDto>.FailAsync($"====== Exp MESSAGE: {ex.Message}"));
            }
        }

        //[HttpGet("GetIncomeTaxPeriodByIncomeTaxId")]
        //public async Task<IActionResult> GetIncomeTaxPeriodByIncomeTaxId(int id)
        //{
        //    try
        //    {
        //        var chk = await permission.HasPermission(1977, PermissionType.Show);
        //        if (!chk)
        //        {
        //            return Ok(await Result.AccessDenied("AcessDenied"));
        //        }

        //        if (id <= 0)
        //        {
        //            return Ok(await Result<IncomeTaxPeriod>.FailAsync(localization.GetMessagesResource("NoIdInUpdate")));
        //        }

        //        var getItem = await hrServiceManager.HrIncomeTaxService.GetOne(s => s.Id == id);
        //        if (getItem.Succeeded)
        //        {
        //            var obj = getItem.Data;
        //            return Ok(await Result<HrIncomeTaxDto>.SuccessAsync(obj, $""));
        //        }
        //        else
        //        {
        //            return Ok(getItem);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(await Result<HrIncomeTaxDto>.FailAsync($"====== Exp MESSAGE: {ex.Message}"));
        //    }
        //}

        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(HrIncomeTaxEditDto obj)
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
                var addRes = await hrServiceManager.HrIncomeTaxService.Update(obj);
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
