using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Result = Logix.Application.Wrapper.Result;

namespace Logix.MVC.LogixAPIs.HR
{

    //   استحقاق التذاكر
    public class HRTicketDueController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IMainServiceManager mainServiceManager;
        public HRTicketDueController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IMainServiceManager mainServiceManager)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.mainServiceManager = mainServiceManager;
        }

        #region IndexPage

        [HttpPost("Search")]
        public async Task<IActionResult> Search(HRTicketDueFilterDto filter)
        {
            var chk = await permission.HasPermission(711, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var BranchesList = session.Branches.Split(',');
                List<HRTicketDueFilterDto> resultList = new List<HRTicketDueFilterDto>();
                var items = await hrServiceManager.HrEmployeeService.GetAllVW(e => e.IsDeleted == false && e.Isdel == false && e.StatusId == 1 && BranchesList.Contains(e.BranchId.ToString()) && e.FacilityId == session.FacilityId &&
                (string.IsNullOrEmpty(filter.EmpCode) || filter.EmpCode == e.EmpId) &&
                (string.IsNullOrEmpty(filter.EmpName) || e.EmpName.ToLower().Contains(filter.EmpName) || e.EmpName2.ToLower().Contains(filter.EmpName)) &&
                (filter.Location == 0 || filter.Location == null || filter.Location == e.Location) &&
                (filter.DepartmentId == 0 || filter.DepartmentId == null || filter.DepartmentId == e.DeptId) &&
                (filter.NationalityId == 0 || filter.NationalityId == null || filter.NationalityId == e.NationalityId) &&
                (filter.JobCategory == 0 || filter.JobCategory == null || filter.JobCategory == e.JobCatagoriesId)

                );
                if (items.Succeeded)
                {
                    if (items.Data.Count() > 0)
                    {

                        var res = items.Data.AsQueryable();

                        if (filter.BranchId != null && filter.BranchId > 0)
                        {
                            res = res.Where(c => c.BranchId != null && c.BranchId == filter.BranchId);
                        }

                        if (res.Any())
                        {
                            var getFromTickets = await hrServiceManager.HrTicketService.GetAll(x => x.IsDeleted == false);

                            foreach (var item in res)
                            {
                                decimal TotalAllowance = 0;

                                var getTotalAllowance = await hrServiceManager.HrAllowanceDeductionService.GetTotalAllowances(item.Id);
                                if (getTotalAllowance.Succeeded) TotalAllowance = getTotalAllowance.Data;

                                var totalAmount = getFromTickets.Data.Where(x => x.EmpId == item.Id).Sum(x => x.TotalAmount);
                                var numberOfTicket = Convert.ToDecimal(String.IsNullOrEmpty(item.TicketNoDependent) ? "0" : item.TicketNoDependent);
                                var newItem = new HRTicketDueFilterDto
                                {
                                    EmpCode = item.EmpId,
                                    EmpName = item.EmpName,
                                    NoOfTickets = numberOfTicket.ToString(),
                                    Salary = item.Salary ?? 0,
                                    NetSalary = item.Salary + TotalAllowance,
                                    DoAppointment = item.Doappointment ?? "",
                                    // قيمة التذكرة
                                    TicketAmount = item.ValueTicket ?? 0,
                                    Due = item.ValueTicket ?? 0 * numberOfTicket,
                                    PaidTicket = totalAmount ?? 0,
                                    Balance = (item.ValueTicket * numberOfTicket) > 0 ? (item.ValueTicket * numberOfTicket) : numberOfTicket - totalAmount,
                                    ValueTicket = item.ValueTicket ?? 0,
                                };
                                resultList.Add(newItem);
                            }
                            if (resultList.Count > 0) return Ok(await Result<List<HRTicketDueFilterDto>>.SuccessAsync(resultList));
                            return Ok(await Result<List<HRTicketDueFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));

                        }
                        return Ok(await Result<List<HRTicketDueFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));

                    }
                    return Ok(await Result<List<HRTicketDueFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));
                }
                return Ok(await Result<HRTicketDueFilterDto>.FailAsync(items.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result<HRTicketDueFilterDto>.FailAsync(ex.Message));
            }
        }

        #endregion

    }
}