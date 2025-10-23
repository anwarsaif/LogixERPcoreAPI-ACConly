using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Helpers;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.HR
{
    // الانتداب 
    public class HRMandateController : BaseHrApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;

        public HRMandateController(IHrServiceManager hrServiceManager, IMainServiceManager mainServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization)
        {
            this.mainServiceManager = mainServiceManager;
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
        }

        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrMandateFilterDto filter)
        {
            try
            {
                var chk = await permission.HasPermission(593, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                filter.DeptId ??= 0; filter.JobCategory ??= 0; filter.LocationId ??= 0; filter.BranchId ??= 0; filter.MabdateCategory ??= 0;

                var items = await hrServiceManager.HrMandateService.GetAllVW(x => x.IsDeleted == false
                && (string.IsNullOrEmpty(filter.EmpCode) || x.EmpCode == filter.EmpCode)
                && (string.IsNullOrEmpty(filter.FromLocation) || (!string.IsNullOrEmpty(x.FromLocation) && x.FromLocation.Contains(filter.FromLocation)))
                && (string.IsNullOrEmpty(filter.ToLocation) || (!string.IsNullOrEmpty(x.ToLocation) && x.ToLocation.Contains(filter.ToLocation)))
                && (filter.DeptId == 0 || x.DeptId == filter.DeptId)
                && (filter.JobCategory == 0 || x.JobCatagoriesId == filter.JobCategory)
                && (filter.LocationId == 0 || x.Location == filter.LocationId)
                && (filter.BranchId == 0 || x.BranchId == filter.BranchId)
                && (filter.MabdateCategory == 0 || x.CatId == filter.MabdateCategory));
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();

                    if (!string.IsNullOrEmpty(filter.FromDate) && !string.IsNullOrEmpty(filter.ToDate))
                    {
                        res = res.Where(r => r.FromDate != null && r.ToDate != null
                        && DateHelper.StringToDate(r.FromDate) >= DateHelper.StringToDate(filter.FromDate)
                        && DateHelper.StringToDate(r.FromDate) <= DateHelper.StringToDate(filter.ToDate));
                    }
                    return Ok(await Result<List<HrMandateVw>>.SuccessAsync(res.ToList(), ""));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrVacationsFilterDto>.FailAsync(ex.Message));
            }
        }

        [HttpPost("GetPagination")]
        public async Task<IActionResult> GetPagination([FromBody] HrMandateFilterDto filter, int take = Pagination.take, long? lastSeenId = null)
        {
            try
            {
                var chk = await permission.HasPermission(593, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));


                filter.DeptId ??= 0;
                filter.JobCategory ??= 0;
                filter.LocationId ??= 0;
                filter.BranchId ??= 0;
                filter.MabdateCategory ??= 0;

                // إعداد شروط التواريخ لو موجودة
                List<DateCondition>? dateConditions = null;
                if (!string.IsNullOrEmpty(filter.FromDate) && !string.IsNullOrEmpty(filter.ToDate))
                {
                    dateConditions = new List<DateCondition>
        {
            new DateCondition
            {
                DatePropertyName = "FromDate",
                ComparisonOperator = ComparisonOperator.GreaterThanOrEqual,
                StartDateString = filter.FromDate
            },
            new DateCondition
            {
                DatePropertyName = "FromDate",
                ComparisonOperator = ComparisonOperator.LessThanOrEqual,
                StartDateString = filter.ToDate
            }
        };
                }

                var items = await hrServiceManager.HrMandateService.GetAllWithPaginationVW(
                    selector: e => e.Id,
                    expression: x => x.IsDeleted == false
                        && (string.IsNullOrEmpty(filter.EmpCode) || x.EmpCode == filter.EmpCode)
                        && (string.IsNullOrEmpty(filter.FromLocation) || (!string.IsNullOrEmpty(x.FromLocation) && x.FromLocation.Contains(filter.FromLocation)))
                        && (string.IsNullOrEmpty(filter.ToLocation) || (!string.IsNullOrEmpty(x.ToLocation) && x.ToLocation.Contains(filter.ToLocation)))
                        && (filter.DeptId == 0 || x.DeptId == filter.DeptId)
                        && (filter.JobCategory == 0 || x.JobCatagoriesId == filter.JobCategory)
                        && (filter.LocationId == 0 || x.Location == filter.LocationId)
                        && (filter.BranchId == 0 || x.BranchId == filter.BranchId)
                        && (filter.MabdateCategory == 0 || x.CatId == filter.MabdateCategory),
                    take: take,
                    lastSeenId: lastSeenId,
                    dateConditions: dateConditions
                );

                if (!items.Succeeded)
                    return Ok(await Result<List<HrMandateVw>>.FailAsync(items.Status.message));

                if (items.Data == null || !items.Data.Any())
                    return Ok(await Result<List<HrMandateVw>>.SuccessAsync(new List<HrMandateVw>()));

                var res = items.Data.OrderByDescending(x => x.FromDate).ToList();

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
                return Ok(await Result<HrMandateFilterDto>.FailAsync(ex.Message));
            }


        }


        [HttpPost("Add")]
        public async Task<ActionResult> Add(HrMandateAddDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(593, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                //if (obj.TypeId <= 0)
                //{
                //    return Ok(await Result<object>.FailAsync("اختر نوع الانتداب"));
                //}

                //if (obj.CatId <= 0)
                //{
                //    return Ok(await Result<object>.FailAsync("اختر تصنيف الانتداب"));
                //}
                //if (string.IsNullOrEmpty(obj.FromDate))
                //{
                //    return Ok(await Result<object>.FailAsync("يجب ادخال من تاريخ "));
                //}
                //if (string.IsNullOrEmpty(obj.ToDate))
                //{
                //    return Ok(await Result<object>.FailAsync("يجب ادخال الى تاريخ "));
                //}
                //if (string.IsNullOrEmpty(obj.Objective))
                //{
                //    return Ok(await Result<object>.FailAsync("يجب ادخال الغرض من الانتداب "));
                //}
                //if (string.IsNullOrEmpty(obj.FromLocation))
                //{
                //    return Ok(await Result<object>.FailAsync("يجب ادخال من موقع  "));
                //}
                //if (string.IsNullOrEmpty(obj.ToLocation))
                //{
                //    return Ok(await Result<object>.FailAsync("يجب ادخال الى موقع  "));
                //}
                //if (obj.VisaTravel <= 0)
                //{
                //    return Ok(await Result<object>.FailAsync("اختر تأشيرة سفر"));
                //}
                //if (obj.TravelBy <= 0)
                //{
                //    return Ok(await Result<object>.FailAsync("اختر السفر بواسطة"));
                //}
                //if (obj.Accommodation <= 0)
                //{
                //    return Ok(await Result<object>.FailAsync("اختر السكن"));
                //}
                //if (obj.RatePerNight <= 0)
                //{
                //    return Ok(await Result<object>.FailAsync("اختر سعر الليلة"));
                //}
                //if (obj.OtherExpenses <= 0)
                //{
                //    return Ok(await Result<object>.FailAsync("اختر مصاريف اخرى"));
                //}
                //if (obj.TicketType <= 0)
                //{
                //    return Ok(await Result<object>.FailAsync("اختر نوع التذكرة"));
                //}
                //if (obj.TicketValue <= 0)
                //{
                //    return Ok(await Result<object>.FailAsync("اختر قيمة التذكرة"));
                //}

                var add = await hrServiceManager.HrMandateService.AddNewMandate(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(HrMandateEditDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(593, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result<HrMandateEditDto>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var update = await hrServiceManager.HrMandateService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrMandateEditDto>.FailAsync($"====== Exp in Hr Mandate Controller getById, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long Id)
        {
            try
            {
                var chk = await permission.HasPermission(593, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }
                var PayrollBtnShow = true;
                var SaveBtnShow = true;

                var item = await hrServiceManager.HrMandateService.GetOneVW(x => x.Id == Id);
                if (item.Succeeded == false)
                {
                    return Ok(item);
                }
                long PayrollId = 0;
                if (item.Data.PayrollId > 0)
                {
                    PayrollId = item.Data.PayrollId ?? 0;
                    PayrollBtnShow = false;
                    SaveBtnShow = false;
                }
                //var fileDtos = new List<SaveFileDto>();
                //var getFiles = await mainServiceManager.SysFileService.GetFilesForUser(Id, 57);
                //fileDtos = getFiles.Data ?? new List<SaveFileDto>();
                var fileDtos = await mainServiceManager.SysFileService.GetAll(x => x.IsDeleted == false && x.TableId == 57 && x.PrimaryKey == Id);

                var result = new
                {
                    item.Data,
                    fileDtos = fileDtos.Data,
                    PayrollBtnShow,
                    PayrollId,
                    SaveBtnShow,
                };
                return Ok(await Result<object>.SuccessAsync(result));
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrMandateVw>.FailAsync($"====== Exp in Hr  Mandate Controller getById, MESSAGE: {ex.Message}"));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var chk = await permission.HasPermission(593, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await hrServiceManager.HrMandateService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete Hr Mandate Controller, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("View")]
        public async Task<IActionResult> View(long Id)
        {
            try
            {
                var chk = await permission.HasPermission(593, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var item = await hrServiceManager.HrMandateService.GetOneVW(x => x.Id == Id);
                if (item.Succeeded)
                {
                    if (item.Data.PayrollId > 0)
                    {
                        var getPayPayroll = await hrServiceManager.HrPayrollService.GetOne(x => x.MsId == item.Data.PayrollId && x.IsDeleted == false);
                        if (getPayPayroll.Data != null)
                        {
                            return Ok(await Result<HrMandateVw>.SuccessAsync(item.Data, $"تم تحويل الانتداب الى مسير انتداب برقم  {item.Data.PayrollId} "));
                        }
                    }
                    return Ok(item);

                }
                return Ok(await Result<HrMandateVw>.FailAsync(item.Status.message));

            }
            catch (Exception ex)
            {
                return Ok(await Result<HrMandateVw>.FailAsync($"====== Exp in Hr  Mandate Controller getById, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("OnEmpCodeChange")]
        public async Task<IActionResult> OnEmpCodeChange(string EmpCode, long MandateType)
        {
            var chk = await permission.HasPermission(593, PermissionType.Add);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (string.IsNullOrEmpty(EmpCode))
            {
                return Ok(await Result<object>.FailAsync("there is no id passed"));
            }
            if (MandateType <= 0)
            {
                return Ok(await Result<object>.FailAsync("اختر نوع الانتداب"));
            }

            try
            {
                var checkEmpId = await mainServiceManager.InvestEmployeeService.GetOne(i => i.EmpId == EmpCode && i.Isdel == false && i.IsDeleted == false);
                if (checkEmpId.Data != null)
                {
                    var getJobLevel = await hrServiceManager.HrJobLevelService.GetOne(j => j.Id == checkEmpId.Data.LevelId);
                    if (getJobLevel.Succeeded)
                    {
                        decimal RatePerNight = 0;
                        if (MandateType == 1) RatePerNight = getJobLevel.Data.Mandate ?? 0;
                        else if (MandateType == 2) RatePerNight = getJobLevel.Data.MandateOut ?? 0;
                        return Ok(await Result<object>.SuccessAsync(new
                        {
                            getJobLevel.Data.TicketType,
                            getJobLevel.Data.TicketValue,
                            RatePerNight,
                            checkEmpId.Data.EmpName
                        }));
                    }
                    return Ok(await Result.FailAsync($"No job level"));
                }
                else
                {
                    return Ok(await Result<object>.FailAsync("الموظف غير موجود في قائمة الموظفين"));
                }
            }
            catch (Exception exp)
            {
                return Ok(await Result.FailAsync($"{exp.Message}"));
            }
        }

        [HttpPost("PayrollAdd")]
        public async Task<ActionResult> PayrollAdd(HRMandatePayrollAddDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(593, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (string.IsNullOrEmpty(obj.PayDate))
                    return Ok(await Result<string>.FailAsync(localization.GetHrResource("MandatePaymentDate")));

                obj.PayrolllTypeId = 3;
                obj.State = 1;
                var add = await hrServiceManager.HrMandateService.PayrollAdd(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"====== Exp in {this.GetType()}, MESSAGE: {ex.Message}"));
            }
        }
    }
}