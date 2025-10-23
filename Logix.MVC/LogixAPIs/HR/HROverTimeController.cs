using AutoMapper;
using DocumentFormat.OpenXml.Office2010.Excel;
using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.DTOs.Main;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Services;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.HR
{
    //  الوقت الاضافي
    public class HROverTimeController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IMapper mapper;
        private readonly IPermissionHelper permission;
        private readonly IMainServiceManager mainServiceManager;




        public HROverTimeController(IHrServiceManager hrServiceManager, ICurrentData session, ILocalizationService localization, IMapper mapper, IPermissionHelper permission, IMainServiceManager mainServiceManager)
        {
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.mapper = mapper;
            this.permission = permission;
            this.mainServiceManager = mainServiceManager;
        }

        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrOverTimeMFilterDto filter)
        {
            var chk = await permission.HasPermission(431, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                //filter.BranchId ??= 0;
                //filter.DeptId ??= 0;
                //filter.Location ??= 0;
                //var BranchesList = session.Branches.Split(',');
                //var items = await hrServiceManager.HrOverTimeMService.GetAllVW(e => e.IsDeleted == false &&
                //BranchesList.Contains(e.BranchId.ToString()) &&
                //e.DateFrom != "" &&
                //e.DateTo != "" &&
                //(string.IsNullOrEmpty(filter.EmpCode) || e.EmpCode == filter.EmpCode) &&
                //(string.IsNullOrEmpty(filter.EmpName) || (e.EmpName != null && e.EmpName.ToLower().Contains(filter.EmpName.ToLower()))) &&
                //(string.IsNullOrEmpty(filter.RefranceId) || e.RefranceId == filter.RefranceId) &&
                //(filter.BranchId == 0 || e.BranchId == filter.BranchId) &&
                //(filter.DeptId == 0 || e.DeptId == filter.DeptId) &&
                //(filter.Location == 0 || e.Location == filter.Location)

                //);
                //if (!items.Succeeded)
                //    return Ok(await Result<HrOverTimeMVw>.FailAsync(items.Status.message));


                //if (items.Data.Count() <= 0)
                //    return Ok(await Result<List<HrOverTimeMVw>>.SuccessAsync(items.Data.ToList(), localization.GetResource1("NosearchResult")));


                //var res = items.Data.AsQueryable();

                //if (!string.IsNullOrEmpty(filter.FromDate) && !string.IsNullOrEmpty(filter.ToDate))
                //{
                //    var FromDate = DateHelper.StringToDate(filter.FromDate);
                //    var ToDate = DateHelper.StringToDate(filter.ToDate);
                //    res = res.Where(r =>
                //    (DateHelper.StringToDate(r.DateFrom) >= FromDate && DateHelper.StringToDate(r.DateFrom) <= ToDate) ||
                //   (DateHelper.StringToDate(r.DateTo) >= FromDate && DateHelper.StringToDate(r.DateTo) <= ToDate)
                //   );
                //}

                //if (res.Any())
                //    return Ok(await Result<List<HrOverTimeMVw>>.SuccessAsync(res.ToList(), ""));
                //return Ok(await Result<List<HrOverTimeMVw>>.SuccessAsync(res.ToList(), localization.GetResource1("NosearchResult")));
                var items = await hrServiceManager.HrOverTimeMService.Search(filter);
                return Ok(items);

			}
            catch (Exception ex)
            {
                return Ok(await Result<HrOverTimeMVw>.FailAsync(ex.Message));
            }
        }
        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long Id)
        {
            try
            {
                var chk = await permission.HasPermission(431, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));

                var HrOverTimeMGetByIdDto = new HrOverTimeMGetByIdDto
                {
                    fileDtos = new List<SaveFileDto>() // Initialize fileDtos here
                };

                var HrOverTimeVwMitem = await hrServiceManager.HrOverTimeMService.GetAllVW(X => X.Id == Id && X.IsDeleted == false);
                if (!HrOverTimeVwMitem.Data.Any())
                    return Ok(await Result<HrOverTimeMGetByIdDto>.FailAsync("Item Not Found"));

                HrOverTimeMGetByIdDto = mapper.Map<HrOverTimeMGetByIdDto>(HrOverTimeVwMitem.Data.FirstOrDefault());

                var HrOverTimeDVwItem = await hrServiceManager.HrOverTimeDService.GetAllVW(X => X.IdM == Id && X.IsDeleted == false);
                if (HrOverTimeDVwItem.Data.Any())
                {
                    HrOverTimeMGetByIdDto.hrOverTimeDVws = HrOverTimeDVwItem.Data.ToList();
                }

                // Retrieve Files
                HrOverTimeMGetByIdDto.fileDtos = new List<SaveFileDto>();
                var getFiles = await mainServiceManager.SysFileService.GetFilesForUser(Id, 101);
                HrOverTimeMGetByIdDto.fileDtos = getFiles.Data ?? new List<SaveFileDto>();

                return Ok(await Result<HrOverTimeMGetByIdDto>.SuccessAsync(HrOverTimeMGetByIdDto, "Item Found", 200));
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrOverTimeMGetByIdDto>.FailAsync($"====== Exp in HrOverTimeController getById, MESSAGE: {ex.Message}"));
            }
        }
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var chk = await permission.HasPermission(431, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await hrServiceManager.HrOverTimeMService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete HrOverTimeController, MESSAGE: {ex.Message}"));
            }
        }


        [HttpPost("Add")]
        public async Task<ActionResult> Add(HrOverTimeMAddDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(431, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (string.IsNullOrEmpty(obj.EmpCode)) return Ok(await Result<object>.FailAsync($"{localization.GetResource1("EmployeeIsNumber")} "));

                if (string.IsNullOrEmpty(obj.DateFrom))
                    return Ok(await Result<object>.FailAsync($"يجب ادخال  من تاريخ"));
                if (string.IsNullOrEmpty(obj.DateTo))
                    return Ok(await Result<object>.FailAsync($"يجب ادخال الى تاريخ"));
                if (obj.hrOverTimeDDtos.Count() < 1)
                    return Ok(await Result<object>.FailAsync($"قم بإضافة ساعات الإضافي اولاً"));
                foreach (var item in obj.hrOverTimeDDtos)
                {
                    item.CurrencyId = 1;
                }
                if (obj.Type <= 0)
                    return Ok(await Result<object>.FailAsync($"يجب ادخال احتساب الإضافي من"));
                var add = await hrServiceManager.HrOverTimeMService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add Hr HROverTimeController  Controller, MESSAGE: {ex.Message}"));
            }
        }


        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(HrOverTimeMEditDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(431, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (obj.Id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));

                if (string.IsNullOrEmpty(obj.EmpCode))
                {
                    return Ok(await Result<object>.FailAsync($"{localization.GetResource1("EmployeeIsNumber")} "));
                }
                if (string.IsNullOrEmpty(obj.DateFrom))
                {
                    return Ok(await Result<object>.FailAsync($"يجب ادخال  من تاريخ"));
                }
                if (string.IsNullOrEmpty(obj.DateTo))
                {
                    return Ok(await Result<object>.FailAsync($"يجب ادخال الى تاريخ"));
                }
                if (obj.hrOverTimeDDtos == null || obj.hrOverTimeDDtos.Count() < 1)
                    return Ok(await Result<object>.FailAsync($"قم بإضافة ساعات الإضافي اولاً"));
                var update = await hrServiceManager.HrOverTimeMService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrOverTimeMEditDto>.FailAsync($"====== Exp in Edit HRDependentsController, MESSAGE: {ex.Message}"));
            }
        }


        //احتساب الإضافي من


        [HttpGet("DDLTypeSelectedIndexChanged")]
        public async Task<IActionResult> DDLTypeSelectedIndexChanged(string EmpCode, int id)
        {
            try
            {
                var chk = await permission.HasPermission(431, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (id > 0 && string.IsNullOrEmpty(EmpCode)) return Ok(await Result.FailAsync("قم بإدخال رقم الموظف"));
                var result = await hrServiceManager.HrOverTimeMService.getEmpSalaryAndOverData(EmpCode, id);
               
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in DDLTypeSelectedIndexChanged HrOverTimeController, MESSAGE: {ex.Message}"));
            }
        }


        /// <summary>
        /// this action is  as  OverTime_Add2 page in logix asp and visual basic system
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>

        [HttpPost("AddUsingExcel")]
        public async Task<ActionResult> AddUsingExcel(HrOverTimeMAddUsingExcelDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(431, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));


                var add = await hrServiceManager.HrOverTimeMService.AddUsingExcel(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add Hr HROpeningHoursEmployeesController  Controller, MESSAGE: {ex.Message}"));
            }
        }


        [HttpPost("Add4")]
        public async Task<ActionResult> Add4(HrOverTimeMAdd4Dto obj)
        {
            try
            {
                var chk = await permission.HasPermission(431, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var add = await hrServiceManager.HrOverTimeMService.Add4(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add Hr HROverTimeController  Controller, MESSAGE: {ex.Message}"));
            }
        }



        #region this is region for OverTime_Att Page (سحب من سجلات الحضور والإنصراف )


        [HttpPost("GetAttendanceButtonClick")]
        public async Task<ActionResult> GetAttendanceButtonClick(HrGetAttendanceButtonClickDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(431, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));
                if (string.IsNullOrEmpty(obj.DateFrom)) return Ok(await Result.FailAsync($"يجب ادخال التاريخ  من"));

                if (string.IsNullOrEmpty(obj.DateTo)) return Ok(await Result.FailAsync($"يجب ادخال التاريخ  الى"));
                var add = await hrServiceManager.HrOverTimeMService.GetAttendanceData(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add Hr HROverTimeController  Controller, MESSAGE: {ex.Message}"));
            }
        }


        [HttpPost("AddListOfOverTimeDetails")]
        public async Task<ActionResult> AddListOfOverTimeDetails(IEnumerable<HrOverTimeDDto> objects)
        {
            try
            {
                var chk = await permission.HasPermission(431, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var add = await hrServiceManager.HrOverTimeMService.AddListOfOverTimeD(objects);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add Hr HROverTimeController  Controller, MESSAGE: {ex.Message}"));
            }
        }

        #endregion

        [HttpDelete("DeleteDetails")]
        public async Task<IActionResult> DeleteDetails(long id)
        {
            try
            {
                var chk = await permission.HasPermission(431, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await hrServiceManager.HrOverTimeDService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete HrOverTimeController, MESSAGE: {ex.Message}"));
            }
        }

    }
}