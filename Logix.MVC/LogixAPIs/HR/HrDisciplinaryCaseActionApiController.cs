using iText.Commons.Bouncycastle.Asn1.X509;
using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Helpers;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Services;
using Logix.Application.Services.HR;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.HR.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System.Globalization;

namespace Logix.MVC.LogixAPIs.HR
{
    //الجزاءت 
    public class HrDisciplinaryCaseActionApiController : BaseHrApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly IDDListHelper listHelper;
        private readonly ISysConfigurationHelper sysConfigurationHelper;
        private readonly ILocalizationService localization;

        public HrDisciplinaryCaseActionApiController(IHrServiceManager hrServiceManager, IDDListHelper listHelper, IMainServiceManager mainServiceManager, ICurrentData session, IPermissionHelper permission, ISysConfigurationHelper sysConfigurationHelper, ILocalizationService localization)
        {
            this.mainServiceManager = mainServiceManager;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.permission = permission;
            this.listHelper = listHelper;
            this.sysConfigurationHelper = sysConfigurationHelper;
            this.localization = localization;
        }


        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrDisciplinaryCaseActionFilterDto filter)
        {
            var lang = session.Language;
            var chk = await permission.HasPermission(568, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var BranchesList = session.Branches.Split(',');
                filter.BranchId ??= 0;
                filter.DeptId ??= 0;
                filter.LocationProjectId ??= 0;
                filter.ActionTypeId ??= 0;
                filter.DisciplinaryCaseID ??= 0;
                //List<HrDisciplinaryCaseActionVM> DisciplinaryCaseActionVMList = new List<HrDisciplinaryCaseActionVM>();
                var items = await hrServiceManager.HrDisciplinaryCaseActionService.GetAllVW(x => x.IsDeleted == false
                && (filter.DisciplinaryCaseID == 0 || filter.DisciplinaryCaseID == x.DisciplinaryCaseId)
                && (filter.ActionTypeId == 0 || filter.ActionTypeId == x.ActionType)
                && (string.IsNullOrEmpty(filter.EmpCode) || filter.EmpCode == x.EmpCode)
                && (filter.BranchId != 0 ? x.BranchId == filter.BranchId :  BranchesList.Contains(x.BranchId.ToString()))
                && (filter.DeptId == 0 || filter.DeptId == x.DeptId)
                && (filter.LocationProjectId == 0 || filter.LocationProjectId == x.Location)
                );
                if (items.Data.Count() > 0)
                {
                    var res = items.Data.AsQueryable();
                    if (!string.IsNullOrEmpty(filter.FromDate))
                        res = res.Where(a => a.DueDate != null && DateHelper.StringToDate(a.DueDate) >= DateHelper.StringToDate(filter.FromDate));

                    if (!string.IsNullOrEmpty(filter.ToDate))
                        res = res.Where(a => a.DueDate != null && DateHelper.StringToDate(a.DueDate) <= DateHelper.StringToDate(filter.ToDate));                   

                    if (res.Count() <= 0)
                    {
                        return Ok(await Result<List<HrDisciplinaryCaseActionVw>>.SuccessAsync(items.Data.ToList(), localization.GetResource1("NosearchResult")));
                    }
                    return Ok(await Result<List<HrDisciplinaryCaseActionVw>>.SuccessAsync(items.Data.ToList(), ""));
                    
                }
                return Ok(await Result<List<HrDisciplinaryCaseActionVw>>.SuccessAsync(items.Data.ToList(), localization.GetResource1("NosearchResult")));

            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpPost("GetPagination")]
        public async Task<IActionResult> GetPagination([FromBody] HrDisciplinaryCaseActionFilterDto filter, int take = 5, long? lastSeenId = null)
        {
            try
            {
                var chk = await permission.HasPermission(568, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var BranchesList = session.Branches.Split(',');
                filter.BranchId ??= 0;
                filter.DeptId ??= 0;
                filter.LocationProjectId ??= 0;
                filter.ActionTypeId ??= 0;
                filter.DisciplinaryCaseID ??= 0;

                var dateConditions = new List<DateCondition>();
                dateConditions.Add(new DateCondition
                {
                    DatePropertyName = "DueDate",
                    ComparisonOperator = ComparisonOperator.GreaterThanOrEqual,
                    StartDateString = filter.FromDate ?? ""
                });
                dateConditions.Add(new DateCondition
                {
                    DatePropertyName = "DueDate",
                    ComparisonOperator = ComparisonOperator.LessThanOrEqual,
                    StartDateString = filter.ToDate ?? ""
                });

                var items = await hrServiceManager.HrDisciplinaryCaseActionService.GetAllWithPaginationVW(selector: x => x.Id,
                expression: x => x.IsDeleted == false
                && (filter.DisciplinaryCaseID == 0 || filter.DisciplinaryCaseID == x.DisciplinaryCaseId)
                && (filter.ActionTypeId == 0 || filter.ActionTypeId == x.ActionType)
                && (string.IsNullOrEmpty(filter.EmpCode) || filter.EmpCode == x.EmpCode)
                && (filter.BranchId != 0 ? x.BranchId == filter.BranchId : BranchesList.Contains(x.BranchId.ToString()))
                && (filter.DeptId == 0 || filter.DeptId == x.DeptId)
                && (filter.LocationProjectId == 0 || filter.LocationProjectId == x.Location),
                    take: take,
                    lastSeenId: lastSeenId,
                   dateConditions: (string.IsNullOrEmpty(filter.FromDate) || string.IsNullOrEmpty(filter.ToDate)) ? null : dateConditions);

                if (!items.Succeeded)
                    return Ok(await Result<List<HrDisciplinaryCaseActionVw>>.FailAsync(items.Status.message));
                
                    var paginatedData = new PaginatedResult<object>
                    {
                        Succeeded = items.Succeeded,
                        Data = items.Data,
                        Status = items.Status,
                        PaginationInfo = items.PaginationInfo
                    };
                    return Ok(paginatedData);
                
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add(HrDisciplinaryCaseActionDto obj)
        {
            var chk = await permission.HasPermission(568, PermissionType.Add);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (!ModelState.IsValid)
            {
                return Ok(obj);
            }
            if (string.IsNullOrEmpty(obj.EmpCode))
            {
                return Ok(await Result<object>.FailAsync("emp Code is Required"));
            }
            try
            {
                var addRes = await hrServiceManager.HrDisciplinaryCaseActionService.Add(obj);
                return Ok(addRes);

            }

            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long Id = 0)
        {
            var chk = await permission.HasPermission(568, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (Id <= 0)
            {
                return Ok(await Result.FailAsync("You do not Enter Any Value"));
            }
            try
            {
                var getItem = await hrServiceManager.HrDisciplinaryCaseActionService.GetAllVW(x => x.Id == Id);
                if (getItem.Succeeded && getItem.Data.Count() > 0)
                {
                    var singleItem = getItem.Data.FirstOrDefault();

                    var ItemForEdit = new HrDisciplinaryCaseActionEditDto
                    {
                        Id = singleItem.Id,
                        ActionType = singleItem.ActionType,
                        CountRept = singleItem.CountRept,
                        DeductedAmount = singleItem.DeductedAmount,
                        DeductedRate = singleItem.DeductedRate,
                        Description = singleItem.Description,
                        EmpCode = singleItem.EmpCode,
                        EmpName = singleItem.EmpName,
                        DisciplinaryCaseId = singleItem.DisciplinaryCaseId,
                        DueDate = singleItem.DueDate,
                        StatusId = singleItem.StatusId,

                    };
                    var files = await mainServiceManager.SysFileService.GetAll(x => x.IsDeleted == false && x.PrimaryKey == Id && x.TableId == 52);
                    return Ok(await Result<object>.SuccessAsync(new { data = ItemForEdit, fileDtos = files }, ""));
                    //return Ok(await Result<object>.SuccessAsync(ItemForEdit));
                }
                return Ok(await Result.FailAsync("هناك خطأ في جلب البيانات  "));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message)); ;
            }
        }

        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(HrDisciplinaryCaseActionEditDto obj)
        {
            var chk = await permission.HasPermission(568, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (!ModelState.IsValid)
            {
                return Ok(obj);
            }
            if (string.IsNullOrEmpty(obj.EmpCode))
            {
                return Ok(await Result<object>.FailAsync("emp Code is Required"));
            }
            try
            {
                var addRes = await hrServiceManager.HrDisciplinaryCaseActionService.Update(obj);
                return Ok(addRes);
            }

            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long Id = 0)
        {
            var chk = await permission.HasPermission(568, PermissionType.Delete);
            if (!chk)
                return Ok(await Result.AccessDenied("AccessDenied"));

            if (Id <= 0)
                return Ok(await Result.FailAsync("Please choose an entity to delete it, there is no id passed"));

            try
            {
                var checkDisciplinaryCaseAction = await hrServiceManager.HrDisciplinaryCaseActionService.GetAllVW(e => e.Id == Id);
                if (!checkDisciplinaryCaseAction.Succeeded || !checkDisciplinaryCaseAction.Data.Any())
                    return Ok(await Result.FailAsync($"There is no Disciplinary Action with id {Id} "));

                var data = checkDisciplinaryCaseAction.Data.First();
                if (data.IsDeleted)
                    return Ok(await Result.FailAsync("الجزاء محذوف مسبقا"));

                if (string.IsNullOrWhiteSpace(data.DueDate))
                    return Ok(await Result.FailAsync("تاريخ الاستحقاق غير متوفر"));

                var dueDate = DateHelper.SafeParseDate(data.DueDate);

                var empPayrollCheck = await hrServiceManager.HrPayrollDService.GetAllVW(e =>
                    e.EmpId == data.EmpId &&
                    e.PayrollTypeId == 1 &&
                    e.IsDeleted == false &&
                    !string.IsNullOrWhiteSpace(e.StartDate) &&
                    !string.IsNullOrWhiteSpace(e.EndDate)
                );

                if (!empPayrollCheck.Succeeded)
                    return Ok(await Result.FailAsync(empPayrollCheck.Status.message));

                bool isWithinPayroll = empPayrollCheck.Data.Any(e =>
                {
                    var start = DateHelper.SafeParseDate(e.StartDate);
                    var end = DateHelper.SafeParseDate(e.EndDate);
                    return dueDate >= start && dueDate <= end;
                });

                if (isWithinPayroll)
                    return Ok(await Result.FailAsync("لم يتم حذف الجزاءات  بسبب ارتباطها بمسير رواتب للموظف "));

                var deleteResult = await hrServiceManager.HrDisciplinaryCaseActionService.Remove(data.Id);

                await hrServiceManager.HrNotificationService.SendDisciplinaryDeletionNotice(data.EmpId ?? 0, data.CaseName);

                return Ok(deleteResult);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpPost("GetEmpDataByEmpIdOrDisciplinaryCaseIDChanged")]
        public async Task<IActionResult> GetEmpDataByEmpIdOrDisciplinaryCaseIDChanged(EmployeeIDChangedVM vM)
        {
            var chk = await permission.HasPermission(568, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (string.IsNullOrEmpty(vM.EmpCode) || vM.EmpCode == "")
            {
                return Ok(await Result.FailAsync("You do not Enter Any Value"));
            }
            if (string.IsNullOrEmpty(vM.Due_Date) || vM.Due_Date == "")
            { vM.Due_Date = DateTime.Now.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture); }

            try
            {
                var result = new EmpAutoDataVM();
                // string? parentCode;
                decimal TotalAllowance = 0;
                decimal TotalDeduction = 0;
                var getEmployeeData = await hrServiceManager.HrEmployeeService.GetOneVW(e => e.EmpId == vM.EmpCode);
                if (getEmployeeData.Succeeded)
                {
                    var getTotalAllowance = await hrServiceManager.HrAllowanceDeductionService.GetTotalAllowances(getEmployeeData.Data.Id);
                    if (getTotalAllowance.Succeeded) TotalAllowance = getTotalAllowance.Data;

                    var getTotalDeduction = await hrServiceManager.HrAllowanceDeductionService.GetTotalDeduction(getEmployeeData.Data.Id);
                    if (getTotalDeduction.Succeeded) TotalDeduction = getTotalDeduction.Data;

                    var NumberofRepetitionDays = "0";
                    var DeductionAmount = 0m;
                    var DeductionRate = 0m;
                    var RepeatCount = "0";
                    int ActionTypeId = 0;
                    //  بمعنى تم اختيار نوع المخالفة

                    if (vM.DisciplinaryCaseID > 0)
                    {
                        var getNumOfRepetitionDays = await sysConfigurationHelper.GetValue(117);
                        if (!string.IsNullOrEmpty(getNumOfRepetitionDays))
                        {
                            NumberofRepetitionDays = getNumOfRepetitionDays;
                        }
                        var Due_Date2BeforeConvert = DateHelper.StringToDate(vM.Due_Date).AddDays(-Convert.ToDouble(NumberofRepetitionDays));
                        var Due_Date2 = Due_Date2BeforeConvert.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);
                        var getCountReptDisciplinaryCase = await hrServiceManager.HrDisciplinaryCaseActionService.GetAll(x => x.DisciplinaryCaseId == vM.DisciplinaryCaseID && x.EmpId == getEmployeeData.Data.Id && x.IsDeleted == false);
                        var getCountReptDisciplinaryCaseFilter = getCountReptDisciplinaryCase.Data.Where(x => x.DueDate != null && DateHelper.StringToDate(x.DueDate) >= DateHelper.StringToDate(Due_Date2) && DateHelper.StringToDate(x.DueDate) <= DateHelper.StringToDate(vM.Due_Date));
                        if (getCountReptDisciplinaryCaseFilter.Any())
                        {
                            RepeatCount = getCountReptDisciplinaryCaseFilter.Count().ToString();
                        }
                        //  الاستعلام من قاعدة البيانات
                        var Amount = await hrServiceManager.HrDisciplinaryCaseActionService.Apply_Policies(session.FacilityId, 8, getEmployeeData.Data.Id);

                        var getFromHrDisciplinaryRule = await hrServiceManager.HrDisciplinaryRuleService.GetOne(x => x.DisciplinaryCaseId == vM.DisciplinaryCaseID && Convert.ToDecimal(RepeatCount) + 1 >= x.ReptFrom && Convert.ToDecimal(RepeatCount) + 1 <= x.ReptTo);
                        if (getFromHrDisciplinaryRule.Data != null)
                        {
                            ActionTypeId = getFromHrDisciplinaryRule.Data.ActionType ?? 0;
                            DeductionRate = (decimal)getFromHrDisciplinaryRule.Data.DeductedRate;
                            if (DeductionRate > 0)
                            {
                                DeductionAmount = Math.Round(Convert.ToDecimal(DeductionRate) * Convert.ToDecimal(Amount.Data) / 30);
                            }
                            else
                            {
                                DeductionAmount = Math.Round(Convert.ToDecimal(DeductionAmount), 2);

                            }
                        }

                    }
                    result = new EmpAutoDataVM
                    {
                        EmpName = session.Language == 1 ? getEmployeeData.Data.EmpName : getEmployeeData.Data.EmpName2,
                        SalryAllownce = TotalAllowance,
                        SalryDeduction = TotalDeduction,
                        DueDate = DateTime.Now,
                        DeductionAmount = DeductionAmount,
                        DeductionRate = DeductionRate,
                        NetSalary = getEmployeeData.Data.Salary + TotalAllowance - TotalDeduction,
                        NumberofRepetitionDays = RepeatCount,
                        Salary = getEmployeeData.Data.Salary,
                        ActionTypeId = ActionTypeId
                    };

                    return Ok(await Result<EmpAutoDataVM>.SuccessAsync(result, " "));
                }
                return Ok(await Result.FailAsync(getEmployeeData.Status.message));
            }

            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
        [HttpDelete("DeleteList")]
        public async Task<IActionResult> DeleteList(List<long> Ids)
        {
            var chk = await permission.HasPermission(568, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (Ids.Count <= 0)
            {
                return Ok(await Result.FailAsync("Please choose an entity to delete it, there is no id passed"));
            }

            try
            {
                foreach (var item in Ids)
                {                // نتأكد من وجود الجزاء 
                    var checkDisciplinaryCaseAction = await hrServiceManager.HrDisciplinaryCaseActionService.GetAllVW(e => e.Id == item && e.IsDeleted == false);
                    if (checkDisciplinaryCaseAction.Data.Count() <= 0)
                    {
                        return Ok(await Result.FailAsync($"الجزاء رقم  {item} غير موجود"));

                    }
                    var data = checkDisciplinaryCaseAction.Data.FirstOrDefault();
                    var IfEmpExistsInPayroll = await hrServiceManager.HrPayrollDService.GetAllVW(e => e.EmpId == data.EmpId && e.PayrollTypeId == 1);
                    if (IfEmpExistsInPayroll.Succeeded)
                    {
                        if (IfEmpExistsInPayroll.Data.Any())
                        {
                            var filterResult = IfEmpExistsInPayroll.Data.Where(e => DateHelper.SafeParseDate(data.DueDate) >= DateHelper.SafeParseDate(e.StartDate) && DateHelper.SafeParseDate(data.DueDate) <= DateHelper.SafeParseDate(e.EndDate));
                            if (filterResult.Any())
                            {
                                return Ok(await Result.FailAsync($"لم يتم حذف الجزاء رقم {item} بسبب ارتباطها بمسير رواتب للموظف "));

                            }
                        }
                        // هنا يحق لنا الحذف 
                        var del = await hrServiceManager.HrDisciplinaryCaseActionService.Remove(item);
                        if (!del.Succeeded)
                        {
                            return Ok(del);
                        }
                    }


                }
                return Ok(await Result<string>.SuccessAsync(localization.GetResource1("DeleteSuccess")));


            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

    }

}