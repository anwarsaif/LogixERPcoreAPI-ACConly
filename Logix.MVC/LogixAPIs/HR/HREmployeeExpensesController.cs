using DocumentFormat.OpenXml.Office2010.Excel;
using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.DTOs.Main;
using Logix.Application.DTOs.WF;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Services;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Graph.Models;
using PermissionType = Logix.MVC.Helpers.PermissionType;

namespace Logix.MVC.LogixAPIs.HR
{

    //   مصروفات الموظفين
    public class HREmployeeExpensesController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IAccServiceManager accServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IMainServiceManager mainServiceManager;

        public HREmployeeExpensesController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IAccServiceManager accServiceManager, IMainServiceManager mainServiceManager)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.accServiceManager = accServiceManager;
            this.mainServiceManager = mainServiceManager;
        }

        #region Index Page

        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrExpenseFilterDto filter)
        {
            var chk = await permission.HasPermission(2107, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                filter.AppCode ??= 0;
                var itemsResult = await hrServiceManager.HrExpenseService.GetAllVW(e =>
                    e.IsDeleted == false &&
                    e.FacilityId == session.FacilityId &&
                    (string.IsNullOrEmpty(filter.Code) || (e.Code != null && e.Code.ToLower().Contains(filter.Code.ToLower()))) &&
                    (string.IsNullOrEmpty(filter.EmpCode) || e.EmpCode == filter.EmpCode) &&
                    (string.IsNullOrEmpty(filter.Title) || (e.Title != null && e.Title.ToLower().Contains(filter.Title.ToLower()))) &&
                    (string.IsNullOrEmpty(filter.EmpName) || (e.EmpName != null && e.EmpName.ToLower().Contains(filter.EmpName.ToLower()))) &&
                    (filter.AppCode == 0 || e.AppCode == filter.AppCode)
                );

                if (!itemsResult.Succeeded)
                {
                    return Ok(await Result<object>.FailAsync(itemsResult.Status.message));

                }

                var filteredItems = itemsResult.Data?.AsQueryable();

                if (filteredItems == null || !filteredItems.Any())
                {
                    return Ok(await Result<List<object>>.SuccessAsync(new List<object>(), localization.GetResource1("NosearchResult")));
                }
                if (!string.IsNullOrEmpty(filter.FromDate) && !string.IsNullOrEmpty(filter.ToDate))
                {
                    var fromDate = DateHelper.StringToDate(filter.FromDate);
                    var toDate = DateHelper.StringToDate(filter.ToDate);
                    filteredItems = filteredItems.Where(r =>
                        DateHelper.StringToDate(r.ExDate) >= fromDate && DateHelper.StringToDate(r.ExDate) <= toDate
                    );
                }

                if (filteredItems.Any())
                {
                    return Ok(await Result<List<HrExpensesVw>>.SuccessAsync(filteredItems.ToList(), ""));
                }

                return Ok(await Result<List<object>>.SuccessAsync(new List<object>(), localization.GetResource1("NosearchResult")));
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync(ex.Message));
            }
        }
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var chk = await permission.HasPermission(2107, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await hrServiceManager.HrExpenseService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete HREmployeeExpensesController  Controller, MESSAGE: {ex.Message}"));
            }
        }




        #endregion


        #region Add Page

        [HttpGet("DDLExpTypeSelectedIndexChanged")]
        public async Task<IActionResult> DDLExpTypeSelectedIndexChanged(long id)
        {
            try
            {
                var chk = await permission.HasPermission(2107, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                // Retrieve the row based on the ID (replace with your actual data retrieval logic)
                var getData = await hrServiceManager.HrExpensesTypeService.GetOneVW(x => x.IsDeleted == false && x.Id == id);
                if (!getData.Succeeded)
                {
                    return Ok(await Result<object>.FailAsync(getData.Status.message));
                }
                if (getData.Data == null)
                {
                    return Ok(await Result<object>.FailAsync($"No data found for ID: {id}"));
                }

                // Parse and calculate values
                var VatRate = getData.Data.VatRate ?? 0; ;
                var Amount = Math.Round((getData.Data.Amount ?? 0), 2);
                var VatAmount = Math.Round(Amount * (VatRate / 100), 2);
                var Total = Amount + VatAmount;

                return Ok(await Result<object>.SuccessAsync(new { VatRate = VatRate, Amount = Amount, Total = Total, VatAmount = VatAmount }));
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"An error occurred: {ex.Message}"));
            }
        }


        [HttpPost("Add")]
        public async Task<ActionResult> Add(HrExpenseAddDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(2107, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (string.IsNullOrEmpty(obj.ApplicantCode))
                    return Ok(await Result<object>.FailAsync($" يجب ادخال  كود مقدم الطلب    "));
                if (string.IsNullOrEmpty(obj.RequestDate))
                    return Ok(await Result<object>.FailAsync($"يجب ادخال تاريخ الطلب "));
                if (string.IsNullOrEmpty(obj.Title))
                    return Ok(await Result<object>.FailAsync($"يجب ادخال  عنوان الطلب "));

                if (obj.employeeDetails.Count < 1)
                {
                    obj.SubTotal = Math.Round(0m, 2);
                    obj.Total = Math.Round(0m, 2); ;
                    obj.VatAmount = Math.Round(0m, 2); ;
                }
                else
                {
                    if (obj.employeeDetails.Any(x => x.ExpenseTypeId <= 0))
                    {
                        return Ok(await Result<object>.FailAsync($"يجب ادخال  نوع المصروف لجميع الحقول"));
                    }
                    if (obj.employeeDetails.Any(x => x.PaidBy <= 0))
                    {
                        return Ok(await Result<object>.FailAsync($"يجب ادخال متحمل المبلغ  لجميع الحقول  "));
                    }
                    if (obj.employeeDetails.Any(x => string.IsNullOrEmpty(x.EmpCode)))
                    {
                        return Ok(await Result<object>.FailAsync($"يجب ادخال كود الموظف لجميع الحقول "));
                    }
                    if (obj.employeeDetails.Any(x => string.IsNullOrEmpty(x.PaymentDate)))
                    {
                        return Ok(await Result<object>.FailAsync($"يجب ادخال تاريخ الفاتورة لجميع الحقول "));
                    }

                    foreach (var item in obj.employeeDetails)
                    {
                        var VatRate = item.VatRate ?? 0; ;
                        var Amount = Math.Round((item.Amount ?? 0), 2);
                        var VatAmount = Math.Round(Amount * (VatRate / 100), 2);
                        var Total = Amount + VatAmount;
                        item.Amount = Amount;
                        item.VatRate = VatRate;
                        item.VatAmount = VatAmount;
                        item.Total = Total;
                    }
                    obj.Total = Math.Round((obj.employeeDetails.Sum(x => x.Amount) ?? 0m), 2);
                    obj.VatAmount = Math.Round((obj.employeeDetails.Sum(x => x.VatAmount) ?? 0m), 2); ;
                    obj.SubTotal = obj.VatAmount + obj.Total;
                }
                var add = await hrServiceManager.HrExpenseService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add HREmployeeExpensesController  Controller, MESSAGE: {ex.Message}"));
            }
        }



        #endregion

        #region Edit Page


        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long Id)
        {
            try
            {
                var chk = await permission.HasPermission(2107, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var item = await hrServiceManager.HrExpenseService.GetOneVW(x => x.Id == Id && x.IsDeleted == false);
                if (!item.Succeeded)
                {
                    return Ok(await Result<object>.FailAsync(item.Status.message));

                }
                if (item.Data == null)
                {
                    return Ok(await Result<object>.FailAsync($"لا يوجد مصروف بهذا الرقم : {Id}"));

                }
                var files = new List<SaveFileDto>();
                var getFiles = await mainServiceManager.SysFileService.GetFilesForUser(Id, 137);
                files = getFiles.Data ?? new List<SaveFileDto>();

                var allDetails = await hrServiceManager.HrExpensesEmployeeService.GetAllVW(x => x.IsDeleted == false && x.ExpenseId == Id);
                //================================= رقم القيد
                var JCode = await accServiceManager.AccJournalMasterService.GetJCodeByReferenceNo(Id, 106);

                //================================= التشييك على انه لا يوجد سند صرف او تسويات دفعات
                var btnaddDisplay = false;
                var btnCreateDisplay = false;
                var btnShowDisplay = false;
                //long? SSID = 0;


                var CheckAllowEdit = await CheckAllowEditAsync(Id);
                if (CheckAllowEdit)
                {
                    btnaddDisplay = true;
                    btnCreateDisplay = true;
                    btnShowDisplay = false;
                }
                else
                {
                    btnaddDisplay = false;
                    btnCreateDisplay = false;
                    btnShowDisplay = true;

                }
                var GetHRExpensesScheduleByExpensesID = await hrServiceManager.HrExpensesScheduleService.GetAll(x => x.IsDeleted == false && x.ExpenseId == Id);
                //if (GetHRExpensesScheduleByExpensesID.Data.Any())
                //{
                //    SSID = GetHRExpensesScheduleByExpensesID.Data.LastOrDefault().SettlementScheduleId ?? 0;
                //}
                if (item.Data.StatusId != 2)
                {
                    btnCreateDisplay = false;
                    btnShowDisplay = false;
                }
                var result = new
                {
                    Expense = item.Data,
                    ExpenseDetails = allDetails.Data,
                    files,
                    JCode,
                    btnShowDisplay,
                    btnCreateDisplay,
                    btnaddDisplay

                };


                return Ok(await Result<object>.SuccessAsync(result));

            }
            catch (Exception ex)
            {
                return Ok(await Result<HrLeaveVw>.FailAsync($"====== Exp in HREmployeeExpensesController  getById, MESSAGE: {ex.Message}"));
            }
        }


        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(HrExpenseEditDto obj)
        {
            try
            {
                var FacilityId = session.FacilityId;

                var chk = await permission.HasPermission(2107, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (string.IsNullOrEmpty(obj.RequestDate))
                    return Ok(await Result<object>.FailAsync($"يجب ادخال تاريخ الطلب "));
                if (string.IsNullOrEmpty(obj.Title))
                    return Ok(await Result<object>.FailAsync($"يجب ادخال  عنوان الطلب "));

                if (obj.employeeDetails.Count < 1)
                {
                    obj.SubTotal = Math.Round(0m, 2);
                    obj.Total = Math.Round(0m, 2); ;
                    obj.VatAmount = Math.Round(0m, 2); ;
                }
                else
                {
                    if (obj.employeeDetails.Any(x => x.ExpenseTypeId <= 0))
                    {
                        return Ok(await Result<object>.FailAsync($"يجب ادخال  نوع المصروف لجميع الحقول"));
                    }
                    if (obj.employeeDetails.Any(x => x.PaidBy <= 0))
                    {
                        return Ok(await Result<object>.FailAsync($"يجب ادخال متحمل المبلغ  لجميع الحقول  "));
                    }
                    if (obj.employeeDetails.Any(x => string.IsNullOrEmpty(x.EmpCode)))
                    {
                        return Ok(await Result<object>.FailAsync($"يجب ادخال كود الموظف لجميع الحقول "));
                    }
                    if (obj.employeeDetails.Any(x => string.IsNullOrEmpty(x.PaymentDate)))
                    {
                        return Ok(await Result<object>.FailAsync($"يجب ادخال تاريخ الفاتورة لجميع الحقول "));
                    }

                    foreach (var item in obj.employeeDetails)
                    {
                        if (item.IsDeleted) continue;
                        var VatRate = item.VatRate ?? 0; ;
                        var Amount = Math.Round((item.Amount ?? 0), 2);
                        var VatAmount = Math.Round(Amount * (VatRate / 100), 2);
                        var Total = Amount + VatAmount;
                        item.Amount = Amount;
                        item.VatRate = VatRate;
                        item.VatAmount = VatAmount;
                        item.Total = Total;
                    }
                    obj.Total = Math.Round((obj.employeeDetails.Where(x => x.IsDeleted == false).Sum(x => x.Amount) ?? 0m), 2);
                    obj.VatAmount = Math.Round((obj.employeeDetails.Where(x => x.IsDeleted == false).Sum(x => x.VatAmount) ?? 0m), 2); ;
                    obj.SubTotal = obj.VatAmount + obj.Total;
                }
                var update = await hrServiceManager.HrExpenseService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"====== Exp in HREmployeeExpensesController Controller Edit, MESSAGE: {ex.Message}"));
            }
        }



        [HttpPost("CreateExpensesEntry")]
        public async Task<ActionResult> CreateExpensesEntry(HrCreateExpensesEntryDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(2107, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (obj.Id <= 0)
                    return Ok(await Result.FailAsync($"يجب التأكد من رقم المصروف"));
                if (string.IsNullOrEmpty(obj.RequestDate))
                    return Ok(await Result<object>.FailAsync($"يجب ادخال تاريخ الطلب "));
                if (string.IsNullOrEmpty(obj.JournalDate))
                    return Ok(await Result<object>.FailAsync($"يجب ادخال تاريخ القيد "));
                var add = await hrServiceManager.HrExpenseService.CreateExpensesEntry(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in CreateExpensesEntryDto HREmployeeExpensesController  Controller, MESSAGE: {ex.Message}"));
            }
        }

        [NonAction]
        private async Task<bool> CheckAllowEditAsync(long id)
        {
            const bool allowEdit = true;
            try
            {
                var payment = await hrServiceManager.HrExpensesPaymentService.GetAll(x => x.IsDeleted == false && x.ExpenseId == id);
                var schedule = await hrServiceManager.HrExpensesScheduleService.GetAll(x => x.IsDeleted == false && x.ExpenseId == id);


                if (payment.Data.Any() || schedule.Data.Any())
                {
                    return false;
                }
                return allowEdit;
            }
            catch (Exception ex)
            {
                // Log exception
                throw new InvalidOperationException("An error occurred while checking edit permissions.", ex);
            }
        }




        #endregion

    }

}