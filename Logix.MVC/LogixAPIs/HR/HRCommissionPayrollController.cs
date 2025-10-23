using System.Globalization;
using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.DTOs.Main;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Result = Logix.Application.Wrapper.Result;

namespace Logix.MVC.LogixAPIs.HR
{

    //   مسير العمولات
    public class HRCommissionPayrollController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly ISysConfigurationHelper sysConfigurationHelper;
        private readonly IMainServiceManager mainServiceManager;
        private readonly IWFServiceManager wFServiceManager;


        public HRCommissionPayrollController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, ISysConfigurationHelper sysConfigurationHelper, IMainServiceManager mainServiceManager, IWFServiceManager wFServiceManager)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.sysConfigurationHelper = sysConfigurationHelper;
            this.mainServiceManager = mainServiceManager;
            this.wFServiceManager = wFServiceManager;
        }


        #region IndexPage


        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrPayrollFilterDto filter)
        {
            var chk = await permission.HasPermission(742, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (filter.FinancelYear == 0 || filter.FinancelYear == null)
                {
                    return Ok(await Result<HrPayrollFilterDto>.FailAsync(" يجب اختيار السنة المالية"));

                }

                List<HrPayrollFilterDto> resultList = new List<HrPayrollFilterDto>();
                var items = await hrServiceManager.HrPayrollService.GetAllVW(e => e.IsDeleted == false &&
                e.PayrollTypeId == 2 &&
                e.FinancelYear == filter.FinancelYear &&
                (string.IsNullOrEmpty(filter.MsMonth) || Convert.ToInt32(filter.MsMonth) == Convert.ToInt32(e.MsMonth)));
                if (items.Succeeded)
                {
                    if (items.Data.Count() > 0)
                    {
                        var res = items.Data.AsQueryable();
                        if (session.FacilityId != 1)
                        {
                            res = res.Where(x => x.FacilityId == session.FacilityId).AsQueryable();
                        }
                        foreach (var item in res)
                        {
                            var newRecord = new HrPayrollFilterDto
                            {
                                MsId = item.MsId,
                                MsCode = item.MsCode,
                                MsDate = item.MsDate,
                                FinancelYear = item.FinancelYear,
                                StatusName = item.StatusName,
                                TypeName = item.TypeName,
                                MsTitle = item.MsTitle,
                                MsMonth = item.MsMonth,
                                ApplicationCode = item.ApplicationCode,
                                Status = item.State,
                            };
                            resultList.Add(newRecord);
                        }
                        if (resultList.Any())
                            return Ok(await Result<List<HrPayrollFilterDto>>.SuccessAsync(resultList, ""));
                        return Ok(await Result<List<HrPayrollFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));
                    }
                    return Ok(await Result<List<HrPayrollFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));
                }
                return Ok(await Result<HrPayrollFilterDto>.FailAsync(items.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrPayrollFilterDto>.FailAsync(ex.Message));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long Id, int? State)
        {
            try
            {
                var chk = await permission.HasPermission(742, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await hrServiceManager.HrPayrollService.Remove(Id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete HR Payroll Controller, MESSAGE: {ex.Message}"));
            }
        }


        #endregion


        #region AddPage Business
        [HttpPost("SearchInAdd")]
        public async Task<IActionResult> SearchInAdd(HRPayrollCreateSpFilterDto filter)
        {
            var chk = await permission.HasPermission(742, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (string.IsNullOrEmpty(filter.FinancelYear))
                {
                    return Ok(await Result<HrPayrollFilterDto>.FailAsync(" يجب اختيار السنة المالية"));
                }
                if (string.IsNullOrEmpty(filter.MSMonth))
                {
                    return Ok(await Result<HrPayrollFilterDto>.FailAsync(" يجب اختيار الشهر"));

                }

                var items = await hrServiceManager.HrPayrollService.CommissionPayrollSearch(filter);
                if (items.Succeeded)
                {
                    if (items.Data.Count() > 0)
                    {
                        return Ok(await Result<List<HrPreparationSalariesVw>>.SuccessAsync(items.Data.ToList(), ""));

                    }
                    return Ok(await Result<List<HrPreparationSalariesVw>>.SuccessAsync(items.Data.ToList(), localization.GetResource1("NosearchResult")));
                }
                return Ok(await Result<HrPreparationSalariesVw>.FailAsync(items.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrPreparationSalariesVw>.FailAsync(ex.Message));
            }
        }

        [HttpPost("Add")]
        public async Task<ActionResult> Add(HrCommissionPayrollAddDto obj)
        {
            try
            {
                var CurrentDate = DateTime.Now.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);
                var chk = await permission.HasPermission(742, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result<string>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));
                if (obj.DetailsDto.Count <= 0)
                    return Ok(await Result<string>.FailAsync("يجب توافر بيانات المسير "));
                if (obj.FacilityID <= 0 || obj.FacilityID == null) obj.FacilityID = (int?)session.FacilityId;

                var add = await hrServiceManager.HrPayrollService.AddNewCommissionPayroll(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"====== Exp in Add HR   Payroll Controller  Add Method, MESSAGE: {ex.Message}"));
            }
        }

        #endregion



        #region EditPage

        [HttpGet("GetById")]

        public async Task<IActionResult> Edit(long Id, long appId = 0)
        {
            try
            {
                var result = new HrPayrollEditDto();
                result.fileDtos = new List<SaveFileDto>();

                var chk = await permission.HasPermission(742, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id <= 0)
                {
                    return Ok(await Result<object>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var item = await hrServiceManager.HrPayrollService.GetOne(x => x.MsId == Id);
                if (item.Succeeded == false || item.Data == null) return Ok(item);
                result.MsId = item.Data.MsId;
                result.MsTitle = item.Data.MsTitle;
                result.DueDate = item.Data.DueDate;
                result.PaymentDate = item.Data.PaymentDate;
                result.FacilityId = item.Data.FacilityId;
                result.FinancelYear = item.Data.FinancelYear;
                result.MsMonth = item.Data.MsMonth;
                var GetSysFiles = await mainServiceManager.SysFileService.GetAll(x => x.IsDeleted == false && x.PrimaryKey == Id && x.TableId == 37);
                if (GetSysFiles.Data != null)
                {
                    foreach (var file in GetSysFiles.Data)
                    {
                        var singleFile = new SaveFileDto
                        {
                            Id = file.Id,
                            FileName = file.FileName ?? "",
                            FileURL = file.FileUrl,
                            IsDeleted = file.IsDeleted,
                            FileDate = file.FileDate


                        };
                        result.fileDtos.Add(singleFile);
                    }
                }
                if (appId > 0)
                {
                    var GetApplicationsbyIDonly = await wFServiceManager.WfApplicationService.GetOneVW(x => x.IsDeleted == false && x.Id == appId);
                    if (GetApplicationsbyIDonly.Data != null)
                    {
                        result.ApplicationCode = GetApplicationsbyIDonly.Data.ApplicationCode;
                        result.ApplicationDate = GetApplicationsbyIDonly.Data.ApplicationDate;
                    }
                }

                return Ok(await Result<object>.SuccessAsync(result, "", 200));

            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"====== Exp in HR Commision Payroll Controller getById, MESSAGE: {ex.Message}"));
            }
        }


        [HttpPost("SearchInEdit")]
        public async Task<IActionResult> SearchInEdit(HrPayrollFilter2Dto filter)
        {
            var chk = await permission.HasPermission(742, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                filter.FinancelYear ??= 0;
                filter.FacilityId ??= 0;
                filter.DeptId ??= 0;
                filter.BranchId ??= 0;
                filter.Location ??= 0;
                filter.SponsorsId ??= 0;
                filter.WagesProtection ??= 0;
                filter.NationalityId ??= 0;
                filter.SalaryGroupId ??= 0;
                filter.PayrollTypeId ??= 0;
                var BranchesList = session.Branches.Split(',');
                if (filter.MsId <= 0)
                {
                    return Ok(await Result<object>.FailAsync("رقم المسير مطلوب"));

                }
                //  هنا يجب ارسال رقم المسير والسنة المالية والشهر للحصول على بيانات صحيحة
                var items = await hrServiceManager.HrPayrollDService.GetAllVW(e =>
                e.IsDeleted == false &&
                (filter.MsId == 0 || filter.MsId == e.MsId) &&
                (filter.FinancelYear == 0 || filter.FinancelYear == e.FinancelYear) &&
                (filter.FacilityId == 0 || filter.FacilityId == e.FacilityId) &&
                (filter.DeptId == 0 || filter.DeptId == e.DeptId) &&
                (filter.BranchId == 0 || filter.BranchId == e.BranchId) &&
                (filter.Location == 0 || filter.Location == e.Location) &&
                (filter.SponsorsId == 0 || filter.SponsorsId == e.SponsorsId) &&
                (filter.NationalityId == 0 || filter.NationalityId == e.NationalityId) &&
                (filter.WagesProtection == 0 || filter.WagesProtection == e.WagesProtection) &&
                (filter.SalaryGroupId == 0 || filter.SalaryGroupId == e.SalaryGroupId) &&
                (string.IsNullOrEmpty(filter.EmpCode) || filter.EmpCode == e.EmpCode) &&
                (string.IsNullOrEmpty(filter.EmpName) || filter.EmpName == e.EmpName) &&
                (filter.PayrollTypeId == 0 || filter.PayrollTypeId == e.PayrollTypeId) &&
                (string.IsNullOrEmpty(filter.MsMonth) || filter.MsMonth == e.MsMonth));
                if (items.Succeeded)
                {
                    if (items.Data.Count() > 0)
                    {
                        var res = items.Data.ToList();
                        if (filter.BranchId > 0)
                        {
                            res = res.Where(x => x.BranchId == filter.BranchId).ToList();
                        }

                        if (res.Any())
                            return Ok(await Result<List<HrPayrollDVw>>.SuccessAsync(res, ""));
                        return Ok(await Result<List<HrPayrollDVw>>.SuccessAsync(res, localization.GetResource1("NosearchResult")));
                    }
                    return Ok(await Result<List<HrPayrollDVw>>.SuccessAsync(items.Data.ToList(), localization.GetResource1("NosearchResult")));
                }
                return Ok(await Result<HrPayrollDVw>.FailAsync(items.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrPayrollDVw>.FailAsync(ex.Message));
            }
        }

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(HrPayrollEditDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(742, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result<object>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));
                if (obj.MsId <= 0)
                    return Ok(await Result<object>.FailAsync("يجب تحديد رقم المسير"));
                if (string.IsNullOrEmpty(obj.MsTitle))
                    return Ok(await Result<object>.FailAsync("يجب تحديد عنوان المسير"));
                if (string.IsNullOrEmpty(obj.DueDate))
                    return Ok(await Result<object>.FailAsync("يجب ادخال  تاريخ الاستحقاق"));
                if (string.IsNullOrEmpty(obj.PaymentDate))
                    return Ok(await Result<object>.FailAsync("يجب ادخال تاريخ الدفع"));



                var update = await hrServiceManager.HrPayrollService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"====== Exp in Edit HR Holiday  Controller, MESSAGE: {ex.Message}"));
            }
        }

        /// <summary>
        /// هذه الدالة تستخدم لجلب الملاحظات على المسير وذلك عند الانتقال الى واجهة تعديل مسير
        /// </summary>
        /// <param name="Id">رقم المسير</param>
        /// <returns></returns>
        [HttpGet("GetPayrollNotes")]
        public async Task<IActionResult> GetPayrollNotes(long Id)
        {
            try
            {
                var chk = await permission.HasPermission(742, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id <= 0)
                {
                    return Ok(await Result<object>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var item = await hrServiceManager.HrPayrollNoteService.GetAllVW(x => x.MsId == Id);
                return Ok(item);

            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"====== Exp in HR Commission Payroll Controller getById, MESSAGE: {ex.Message}"));
            }
        }
        #endregion
    }
}