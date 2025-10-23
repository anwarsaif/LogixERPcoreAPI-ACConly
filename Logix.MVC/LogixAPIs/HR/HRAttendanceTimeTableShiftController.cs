using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Result = Logix.Application.Wrapper.Result;

namespace Logix.MVC.LogixAPIs.HR
{

    //  المجموعات
    public class HRAttendanceTimeTableShiftController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;


        public HRAttendanceTimeTableShiftController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
        }
        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrAttShiftFilterDto filter)
        {
            var chk = await permission.HasPermission(549, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                List<HrAttShiftFilterDto> resultList = new List<HrAttShiftFilterDto>();
                var items = await hrServiceManager.HrAttShiftService.GetAll(e => e.IsDeleted == false);
                if (items.Succeeded)
                {
                    if (items.Data.Any())
                    {
                        var res = items.Data.AsQueryable();

                        if (!string.IsNullOrEmpty(filter.Name))
                        {
                            res = res.Where(r => r.Name != null && r.Name.ToLower().Contains(filter.Name.ToLower()));
                        }
                        foreach (var item in res)
                        {
                            var newRecord = new HrAttShiftFilterDto
                            {
                                Id = item.Id,
                                BeginDate=item.BeginDate,
                                EndDate=item.EndDate,
                                Name = item.Name,
                                OffDays = item.OffDays  
                            

                            };
                            resultList.Add(newRecord);
                        }
                        if (resultList.Any())
                            return Ok(await Result<List<HrAttShiftFilterDto>>.SuccessAsync(resultList, ""));
                        return Ok(await Result<List<HrAttShiftFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));
                    }
                    return Ok(await Result<List<HrAttShiftFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));
                }
                return Ok(await Result<HrAttShiftFilterDto>.FailAsync(items.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrAttShiftFilterDto>.FailAsync(ex.Message));
            }
        }

        [HttpGet("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var chk = await permission.HasPermission(549, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await hrServiceManager.HrAttShiftService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete HR Attendance TimeTable Shift Controller, MESSAGE: {ex.Message}"));
            }
        }


        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long Id)
        {
            List<HrAttTimeTableVw> resultList = new List<HrAttTimeTableVw>();
            try
            {
                var chk = await permission.HasPermission(549, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id <= 0)
                {
                    return Ok(await Result<HrAttShiftEdit2Dto>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var item = await hrServiceManager.HrAttShiftService.GetOne(x => x.Id == Id);
                if (item.Succeeded&&item.Data!=null)
                {
                    var GetHRAttShiftimeTable = await hrServiceManager.HrAttShiftTimeTableService.GetAllVW(x=>x.IsDeleted==false&&x.ShiftId==Id);    
                    if (GetHRAttShiftimeTable.Data.Any())
                    {
                        foreach (var singleItem in GetHRAttShiftimeTable.Data)
                        {
                            var newRecord = new HrAttTimeTableVw { 
                            OnDutyTime = singleItem.OnDutyTime,
                            OffDutyTime = singleItem.OffDutyTime,
                            Id = singleItem.Id,
                            TimeTableName = singleItem.TimeTableName,
                            };
                            resultList.Add(newRecord);

                        }
                    }
                    var returnedResult = new HrAttShiftEdit2Dto
                    {
                        timeTableVws = resultList,
                        Name = item.Data.Name,
                        BeginDate = item.Data.BeginDate,
                        EndDate = item.Data.EndDate,
                        OffDays = item.Data.OffDays,
                    };
                        return Ok(await Result<HrAttShiftEdit2Dto>.SuccessAsync(returnedResult));
                }

                return Ok(await Result<HrAttShiftEdit2Dto>.FailAsync(item.Status.message));

            }
            catch (Exception ex)
            {
                return Ok(await Result<HrAttShiftEdit2Dto>.FailAsync($"====== Exp in Delete HR Attendance TimeTable Shift Controller getById, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(HrAttShiftEditDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(549, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid|| obj.Shift.Count() <= 0)
                    return Ok(await Result<HrAttShiftEditDto>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                

                var update = await hrServiceManager.HrAttShiftService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrAttShiftEditDto>.FailAsync($"====== Exp in Edit HR Attendance TimeTable Shift  Controller, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Add")]
        public async Task<ActionResult> Add(HrAttShiftDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(549, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid || !obj.Shift.Any())
                    return Ok(await Result<HrAttShiftDto>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));
                var add = await hrServiceManager.HrAttShiftService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrAttShiftDto>.FailAsync($"====== Exp in Add HR   Attendance TimeTable Shift Controller  Add Method, MESSAGE: {ex.Message}"));
            }
        }
        //
        [HttpGet("DeleteShifTimeTabletOnEdit")]
        public async Task<IActionResult> DeleteShifTimeTabletOnEdit(long id)
        {
            try
            {
                var chk = await permission.HasPermission(549, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await hrServiceManager.HrAttShiftTimeTableService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete HR Attendance TimeTable Shift Controller, MESSAGE: {ex.Message}"));
            }
        }
        [HttpPost("AddShiftOnEdit")]
        public async Task<ActionResult> AddShiftTimeTableOnEdit(HrAttShiftTimeTableDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(549, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid || obj.ShiftId<=0)
                    return Ok(await Result<HrAttShiftDto>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));
                var add = await hrServiceManager.HrAttShiftTimeTableService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrAttShiftDto>.FailAsync($"====== Exp in Add HR   Attendance TimeTable Shift Controller  Add Method, MESSAGE: {ex.Message}"));
            }
        }
    }
}