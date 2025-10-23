using AutoMapper;
using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.HR.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Globalization;

namespace Logix.MVC.LogixAPIs.HR
{
    //الورديات  
    public class HRTimeTableApiController : BaseHrApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly IMapper _mapper;
        private readonly ILocalizationService localization;
        public HRTimeTableApiController(IHrServiceManager hrServiceManager, IMainServiceManager mainServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IMapper _mapper)
        {
            this.mainServiceManager = mainServiceManager;
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this._mapper = _mapper;
        }


        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var chk = await permission.HasPermission(547, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var items = await hrServiceManager.HrAttTimeTableService.Search();
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }




        [HttpPost("Add")]
        public async Task<IActionResult> Add(HrAttTimeTableDto obj)
        {
            var chk = await permission.HasPermission(547, PermissionType.Add);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (!ModelState.IsValid)
            {
                return Ok(obj);
            }
            if (string.IsNullOrEmpty(obj.OnDutyTimeString) || obj.OnDutyTimeString == "00:00")
            {
                return Ok(await Result<HrAttTimeTableDto>.FailAsync(" الحقل مطلوب"));

            }
            if (string.IsNullOrEmpty(obj.OffDutyTimeString) || obj.OffDutyTimeString == "00:00")
            {
                return Ok(await Result<HrAttTimeTableDto>.FailAsync(" الحقل مطلوب"));

            }
            if (string.IsNullOrEmpty(obj.BeginInString) || obj.BeginInString == "00:00")
            {
                return Ok(await Result<HrAttTimeTableDto>.FailAsync(" الحقل مطلوب"));

            }
            if (string.IsNullOrEmpty(obj.EndInString) || obj.EndInString == "00:00")
            {
                return Ok(await Result<HrAttTimeTableDto>.FailAsync(" الحقل مطلوب"));

            }
            if (string.IsNullOrEmpty(obj.BeginOutString) || obj.BeginOutString == "00:00")
            {
                return Ok(await Result<HrAttTimeTableDto>.FailAsync(" الحقل مطلوب"));

            }
            if (string.IsNullOrEmpty(obj.EndOutString) || obj.EndOutString == "00:00")
            {
                return Ok(await Result<HrAttTimeTableDto>.FailAsync(" الحقل مطلوب"));

            }
            if (obj.FlexibleAttendance == true)
            {
                if (string.IsNullOrEmpty(obj.FlexibleEndString) || obj.FlexibleEndString == "00:00")
                {
                    return Ok(await Result<HrAttTimeTableDto>.FailAsync(" الحقل مطلوب"));

                }
                if (string.IsNullOrEmpty(obj.FlexibleStartString) || obj.FlexibleStartString == "00:00")
                {
                    return Ok(await Result<HrAttTimeTableDto>.FailAsync(" الحقل مطلوب"));

                }
            }



            try
            {

                var addRes = await hrServiceManager.HrAttTimeTableService.Add(obj);

                return Ok(addRes);

            }

            catch (Exception ex)
            {
                return Ok(await Result<HrAttTimeTableDto>.FailAsync(ex.Message));
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long Id = 0)
        {
            var chk = await permission.HasPermission(547, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result<HrAttTimeTableEditDto>.FailAsync($"Access Denied"));
            }
            if (Id <= 0)
            {
                return Ok(await Result<HrAttTimeTableEditDto>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
            }

            try
            {
                var getItem = await hrServiceManager.HrAttTimeTableService.GetModel(Id); ;
                if (getItem.Succeeded && getItem.Data != null)
                {
                    var DaysNumbers = string.Empty;
                    var getHRAttTimeTableDays = await hrServiceManager.HrAttTimeTableDayService.GetAll(g => g.IsDeleted == false && g.TimeTableId == getItem.Data.Id);
                    if (getHRAttTimeTableDays.Succeeded && getHRAttTimeTableDays.Data != null)
                    {

                        var MappedEntity = _mapper.Map<HrAttTimeTableDto>(getItem.Data);
                        if (!string.IsNullOrEmpty(getItem.Data.OnDutyTime.ToString()))
                        {
                            MappedEntity.OnDutyTimeString = string.Format("{0:hh\\:mm}", getItem.Data.OnDutyTime);

                        }
                        if (!string.IsNullOrEmpty(getItem.Data.OffDutyTime.ToString()))
                        {
                            MappedEntity.OffDutyTimeString = string.Format("{0:hh\\:mm}", getItem.Data.OffDutyTime);

                        }
                        if (!string.IsNullOrEmpty(getItem.Data.BeginIn.ToString()))
                        {
                            MappedEntity.BeginInString = string.Format("{0:hh\\:mm}", getItem.Data.BeginIn);

                        }
                        if (!string.IsNullOrEmpty(getItem.Data.BeginOut.ToString()))
                        {
                            MappedEntity.BeginOutString = string.Format("{0:hh\\:mm}", getItem.Data.BeginOut);

                        }
                        if (!string.IsNullOrEmpty(getItem.Data.EndOut.ToString()))
                        {
                            MappedEntity.EndOutString = string.Format("{0:hh\\:mm}", getItem.Data.EndOut);

                        }
                        if (!string.IsNullOrEmpty(getItem.Data.EndIn.ToString()))
                        {
                            MappedEntity.EndInString = string.Format("{0:hh\\:mm}", getItem.Data.EndIn);

                        }
                        if (!string.IsNullOrEmpty(getItem.Data.FlexibleEnd.ToString()))
                        {
                            MappedEntity.FlexibleEndString = string.Format("{0:hh\\:mm}", getItem.Data.FlexibleEnd);

                        }
                        if (!string.IsNullOrEmpty(getItem.Data.FlexibleStart.ToString()))
                        {
                            MappedEntity.FlexibleStartString = string.Format("{0:hh\\:mm}", getItem.Data.FlexibleStart);

                        }

                        foreach (var item in getHRAttTimeTableDays.Data)
                        {
                            DaysNumbers += item.DayNo.ToString() + ",";
                        }
                        DaysNumbers = DaysNumbers.TrimEnd('\u002C');
                        MappedEntity.DaysNumbers = DaysNumbers;
                        return Ok(await Result<HrAttTimeTableDto>.SuccessAsync(MappedEntity));

                    }
                    return Ok(await Result<HrAttTimeTableEditDto>.FailAsync(getItem.Status.message));
                }
                return Ok(await Result<HrAttTimeTableEditDto>.FailAsync(getItem.Status.message));
            }
            catch (Exception exp)
            {
                return Ok(await Result<HrAttTimeTableEditDto>.FailAsync($"EXP in {this.GetType()}, Meesage: {exp.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(HrAttTimeTableEditDto obj)
        {
            var chk = await permission.HasPermission(547, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (string.IsNullOrEmpty(obj.OnDutyTimeString) || obj.OnDutyTimeString == "00:00")
            {
                return Ok(await Result<HrAttTimeTableDto>.FailAsync(" الحقل مطلوب"));

            }
            if (string.IsNullOrEmpty(obj.OffDutyTimeString) || obj.OffDutyTimeString == "00:00")
            {
                return Ok(await Result<HrAttTimeTableDto>.FailAsync(" الحقل مطلوب"));

            }
            if (string.IsNullOrEmpty(obj.BeginInString) || obj.BeginInString == "00:00")
            {
                return Ok(await Result<HrAttTimeTableDto>.FailAsync(" الحقل مطلوب"));

            }
            if (string.IsNullOrEmpty(obj.EndInString) || obj.EndInString == "00:00")
            {
                return Ok(await Result<HrAttTimeTableDto>.FailAsync(" الحقل مطلوب"));

            }
            if (string.IsNullOrEmpty(obj.BeginOutString) || obj.BeginOutString == "00:00")
            {
                return Ok(await Result<HrAttTimeTableDto>.FailAsync(" الحقل مطلوب"));

            }
            if (string.IsNullOrEmpty(obj.EndOutString) || obj.EndOutString == "00:00")
            {
                return Ok(await Result<HrAttTimeTableDto>.FailAsync(" الحقل مطلوب"));

            }
            if (obj.FlexibleAttendance == true)
            {
                if (string.IsNullOrEmpty(obj.FlexibleEndString) || obj.FlexibleEndString == "00:00")
                {
                    return Ok(await Result<HrAttTimeTableDto>.FailAsync(" الحقل مطلوب"));

                }
                if (string.IsNullOrEmpty(obj.FlexibleStartString) || obj.FlexibleStartString == "00:00")
                {
                    return Ok(await Result<HrAttTimeTableDto>.FailAsync(" الحقل مطلوب"));

                }
            }
            try
            {
                var addRes = await hrServiceManager.HrAttTimeTableService.Update(obj);
                return Ok(addRes);
            }

            catch (Exception ex)
            {
                return Ok(await Result<HrAttTimeTableEditDto>.FailAsync($"{ex.Message}"));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long Id = 0)
        {
            var chk = await permission.HasPermission(547, PermissionType.Delete);
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
                var del = await hrServiceManager.HrAttTimeTableService.Remove(Id);

                return Ok(del);
            }
            catch (Exception exp)
            {
                return Ok(await Result.FailAsync($"{exp.Message}"));
            }
        }


    }
}