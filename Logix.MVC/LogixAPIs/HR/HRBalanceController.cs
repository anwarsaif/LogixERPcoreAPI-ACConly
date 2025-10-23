using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.HR
{
    //   الارصدة الإفتتاحية الأخرى
    public class HRBalanceController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ILocalizationService localization;

        public HRBalanceController(IHrServiceManager hrServiceManager,
            IPermissionHelper permission,
            ILocalizationService localization)
        {
            this.permission = permission;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
        }

        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrOpeningBalanceFilterDto filter)
        {
            try
            {
                var chk = await permission.HasPermission(970, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var items = await hrServiceManager.HrOpeningBalanceService.Search(filter);
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<List<HrOpeningBalanceFilterDto>>.FailAsync(ex.Message));
            }
        }
        [HttpPost("GetPagination")]
        public async Task<IActionResult> GetPagination([FromBody] HrOpeningBalanceFilterDto filter, int take = Pagination.take, long? lastSeenId = null)
        {
            try
            {
                var chk = await permission.HasPermission(970, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));


                filter.TypeId ??= 0;
                filter.BranchId ??= 0;

                var items = await hrServiceManager.HrOpeningBalanceService.GetAllWithPaginationVW(
                    selector: x => x.Id,
                    expression: x => x.IsDeleted == false
                                && (filter.TypeId == 0 || filter.TypeId == x.TypeId)
                                && (string.IsNullOrEmpty(filter.EmpCode) || x.EmpCode == filter.EmpCode)
                                && (filter.BranchId == 0 || filter.BranchId == x.BranchId),
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
        public async Task<ActionResult> Add(HrOpeningBalanceDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(970, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (string.IsNullOrEmpty(obj.EmpCode) || string.IsNullOrEmpty(obj.StartDate) || obj.ObValue == null || obj.TypeId <= 0)
                    return Ok(await Result<HrVacationsEditDto>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var add = await hrServiceManager.HrOpeningBalanceService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(HrOpeningBalanceEditDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(970, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (string.IsNullOrEmpty(obj.EmpCode) || string.IsNullOrEmpty(obj.StartDate) || obj.ObValue == null || obj.TypeId <= 0)
                    return Ok(await Result<HrVacationsEditDto>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var update = await hrServiceManager.HrOpeningBalanceService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrOpeningBalanceEditDto>.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long Id)
        {
            try
            {
                var chk = await permission.HasPermission(970, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var item = await hrServiceManager.HrOpeningBalanceService.GetOneVW(x => x.Id == Id);
                return Ok(item);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrDecisionsVw>.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var chk = await permission.HasPermission(970, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await hrServiceManager.HrOpeningBalanceService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }


        #region ================================ Emp Curr_Balance ================================
        [HttpPost("CurrBalance")]
        public async Task<IActionResult> CurrBalance(CurrentBalanceFilterDto filter)
        {
            try
            {
                var chk = await permission.HasPermission(971, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var result = await hrServiceManager.HrOpeningBalanceService.CurrBalanceSearch(filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
        #endregion ============================ End Emp Curr_Balance =============================

        #region ================================ Curr_Balance All ================================
        [HttpPost("CurrBalanceAll")]
        public async Task<IActionResult> CurrBalanceAll(CurrentBalanceFilterDto filter)
        {
            try
            {
                var chk = await permission.HasPermission(972, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var result = await hrServiceManager.HrOpeningBalanceService.CurrBalanceAllSearch(filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
        #endregion ============================ End Curr_Balance All =============================
    }
}