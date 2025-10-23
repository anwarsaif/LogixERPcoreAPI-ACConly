using DevExpress.Data.ODataLinq.Helpers;
using DocumentFormat.OpenXml.Office2021.DocumentTasks;
using DocumentFormat.OpenXml.Spreadsheet;
using Logix.Application.Common;
using Logix.Application.DTOs.TS;
using Logix.Application.Helpers;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.TS;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Logix.MVC.LogixAPIs.TS
{
    public class TsTasksSchedulerController : BaseTsApiController
    {
        private readonly ITsServiceManager tsServiceManager;
        private readonly IMainServiceManager mainServiceManager;
        private readonly ICurrentData session;
        private readonly IPermissionHelper permission;
        private readonly ILocalizationService localization;

        public TsTasksSchedulerController(
            ITsServiceManager tsServiceManager,
            IMainServiceManager mainServiceManager,
            ICurrentData session,
            IPermissionHelper permission,
            ILocalizationService localization
            )
        {
            this.tsServiceManager = tsServiceManager;
            this.mainServiceManager = mainServiceManager;
            this.session = session;
            this.permission = permission;
            this.localization = localization;
        }

        #region "GetAll - Search"
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var chk = await permission.HasPermission(779, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var items = await tsServiceManager.TsTasksSchedulerService.GetAll(x => x.IsDeleted == false);

                return Ok(await Result<object>.SuccessAsync(items, $"Search Completed {items.Data.Count()}", 200));
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        [HttpPost("Search")]
        public async Task<IActionResult> Search(PaginatedRequest<TsTasksSchedulerFilterDto> request)
        {
            try
            {
                var chk = await permission.HasPermission(779, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var filter = request.Filter;

                //var userId = filter.AssigneeToUserId.Split(',');

                filter.StatusId ??= 0;
                filter.Type ??= 0;
                var emp = await mainServiceManager.SysUserService.GetAll(x => x.Isdel == false
                && x.UserFullname.Contains(filter.EmpName));

                var items = await tsServiceManager.TsTasksSchedulerService.GetAllVW(x => x.IsDeleted == false
                && x.StatusId != 4
                && x.CreatedBy == session.UserId
                && (string.IsNullOrEmpty(filter.Subject) || x.Subject.Contains(filter.Subject))
                && (filter.StatusId == 0 || filter.StatusId == x.StatusId)
                && (filter.Type == 0 || filter.Type == x.Type)
                );

                if (!items.Succeeded)
                    return Ok(await Result<object>.FailAsync(items.Status.message));

                var res = items.Data.AsQueryable();
                //foreach (var e in emp.Data)
                //{
                //    res = (IQueryable<TsTasksSchedulerVw>)(items?.Data?.Where(n =>
                //       !string.IsNullOrEmpty(n.AssigneeToUserId) && // Check if AssigneeToUserId is not null or empty
                //       n.AssigneeToUserId.Split(',').Contains(e.UserPkId.ToString()) // Check if UserId exists
                //   ) ?? Enumerable.Empty<TsTasksSchedulerVw>());
                //}
                //res = res.AsQueryable();
                if (items.Succeeded)
                {
                    if (!string.IsNullOrEmpty(filter.FromDate))
                    {
                        DateTime Fromdate = DateTime.ParseExact(filter.FromDate, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                        res = res.Where(s => string.IsNullOrEmpty(s.SendDate) || DateTime.ParseExact(s.SendDate, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= Fromdate);
                    }

                    if (!string.IsNullOrEmpty(filter.ToDate))
                    {
                        DateTime Todate = DateTime.ParseExact(filter.ToDate, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                        res = res.Where(s => string.IsNullOrEmpty(s.SendDate) || DateTime.ParseExact(s.SendDate, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= Todate);
                    }
                }

                if (!items.Succeeded)
                    return Ok(await Result<object>.FailAsync(items.Status.message));

                if (!items.Data.Any())
                    return Ok(await Result<object>.SuccessAsync(new List<object>(), localization.GetResource1("NosearchResult")));

                if (request.PageNumber > 0 && request.PageSize > 0)
                {
                    var paginatedData = await PaginatedResult<object>.SuccessAsync(
                        res.ToList().Cast<object>(),
                        request.PageNumber,
                        request.PageSize);
                    return Ok(paginatedData);
                }

                return Ok(await Result<object>.SuccessAsync(res.ToList(), $"Search Completed {res.Count()}", 200));
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"Error in {nameof(Search)}: {ex.Message} - {localization.GetResource1("NotAbleShowResults")}"));
            }
        }
        #endregion "GetAll - Search"

        #region "Add"

        [HttpPost("Add")]
        public async Task<IActionResult> Add(TsTasksSchedulerDto obj)
        {
            try
            {
                if (!(await permission.HasPermission(779, PermissionType.Add)) && !(await permission.HasPermission(956, PermissionType.Add)))
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync(localization.GetMessagesResource("dataRequire")));

                var add = await tsServiceManager.TsTasksSchedulerService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add, MESSAGE: {ex.Message}"));
            }
        }

        #endregion "Add"

        //#region "Edit"
        //[HttpPost("Edit")]
        //public async Task<IActionResult> Edit(TsMainTaskEditDto obj)
        //{
        //    if (await permission.HasPermission(243, PermissionType.Edit) && await permission.HasPermission(243, PermissionType.Show))
        //    {
        //        return Ok(await Result.AccessDenied("AccessDenied"));
        //    }
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            return Ok(await Result<TsMainTaskEditDto>.FailAsync(localization.GetMessagesResource("dataRequire")));
        //        }

        //        var Edit = await tsServiceManager.TsTaskService.UpdateTask(obj);
        //        return Ok(Edit);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(await Result<TsMainTaskEditDto>.FailAsync($"======= Exp in edit: {ex.Message}"));
        //    }
        //}
        //#endregion "Edit"

        #region "Delete"
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long Id)
        {
            var chk = await permission.HasPermission(779, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var del = await tsServiceManager.TsTasksSchedulerService.Remove(Id);
                return Ok(del);
            }
            catch (Exception ex)
            {
                return Ok(await Result<TsTaskDto>.FailAsync($"======= Exp in Delete: {ex.Message}"));
            }
        }
        #endregion "Delete"

    }
}
