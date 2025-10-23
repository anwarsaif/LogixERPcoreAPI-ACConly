using AutoMapper;
using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Result = Logix.Application.Wrapper.Result;

namespace Logix.MVC.LogixAPIs.HR
{
    //  مخصص التذاكر
    public class HRProvisionsTicketController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IMainServiceManager mainServiceManager;
        private readonly IMapper mapper;
        private readonly IAccServiceManager accServiceManager;


        public HRProvisionsTicketController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IMainServiceManager mainServiceManager, IMapper mapper, IAccServiceManager accServiceManager)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.mainServiceManager = mainServiceManager;
            this.mapper = mapper;
            this.accServiceManager = accServiceManager;
        }


    #region IndexPage

    [HttpPost("GetPagination")]
    public async Task<IActionResult> GetPagination([FromBody] HrProvisionFilterDto filter, int take = Pagination.take, int? lastSeenId = null)
    {
      try
      {
        var chk = await permission.HasPermission(1518, PermissionType.Show);
        if (!chk)
        {
          return Ok(await Result.AccessDenied("AccessDenied"));
        }
        filter.MonthId ??= 0;
        filter.FinYear ??= 0;
        filter.YearlyOrMonthly ??= 0;
        if (filter.YearlyOrMonthly <= 0)
          return Ok(await Result<object>.FailAsync($"{localization.GetHrResource("OperationType")}"));

        if (filter.FinYear <= 0)
          return Ok(await Result<object>.FailAsync($"{localization.GetHrResource("FinancialYear")}"));

        List<HrProvisionFilterDto> resultList = new List<HrProvisionFilterDto>();

        var items = await hrServiceManager.HrProvisionService.GetAllWithPaginationVW(selector: e => e.Id,
         expression: e =>
          e.IsDeleted == false &&
        e.FacilityId == session.FacilityId &&
        e.TypeId == 2 &&
        (filter.FinYear == 0 || filter.FinYear == e.FinYear) &&
        (filter.MonthId == 0 || filter.MonthId == e.MonthId) &&
        (filter.YearlyOrMonthly == 0 || filter.YearlyOrMonthly == e.YearlyOrMonthly) &&
        (string.IsNullOrEmpty(filter.Description) || (e.Description != null && e.Description.ToLower().Contains(filter.Description.ToLower()))) &&
        (string.IsNullOrEmpty(filter.Code) || e.Code == filter.Code),
          take: take,
          lastSeenId: lastSeenId

        );

        if (!items.Succeeded)
          return Ok(await Result<object>.FailAsync(items.Status.message));


        if (!items.Data.Any())
          return Ok(await Result<List<HrProvisionFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));

        var res = items.Data.AsQueryable();
        if (!string.IsNullOrEmpty(filter.FromDate) && !string.IsNullOrEmpty(filter.ToDate))
        {
          res = res.Where(r => r.PDate != null &&
          DateHelper.StringToDate(r.PDate) >= DateHelper.StringToDate(filter.FromDate) &&
           DateHelper.StringToDate(r.PDate) <= DateHelper.StringToDate(filter.ToDate));
        }

        foreach (var item in res)
        {
          var newItem = new HrProvisionFilterDto
          {
            Id = item.Id,
            Code = item.Code,
            PDate = item.PDate,
            YearlyOrMonthlyName = item.YearlyOrMonthlyName,
            Description = item.Description,
            MonthName = item.MonthName,
            FinYear = item.FinYear,
            FacilityName = item.FacilityName,
          };
          resultList.Add(newItem);
        }

        var paginatedResult = new PaginatedResult<object>
        {
          Succeeded = true,
          Data = resultList,
          Status = items.Status,
          PaginationInfo = items.PaginationInfo
        };

        return Ok(paginatedResult);
      }
      catch (Exception ex)
      {
        return Ok(await Result.FailAsync(ex.Message));
      }
    }

    [HttpPost("Search")]
        public async Task<IActionResult> Search(HrProvisionFilterDto filter)
        {
            var chk = await permission.HasPermission(1518, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                filter.MonthId ??= 0;
                filter.FinYear ??= 0;
                filter.YearlyOrMonthly ??= 0;
                if (filter.YearlyOrMonthly <= 0)
                    return Ok(await Result<object>.FailAsync($"{localization.GetHrResource("OperationType")}"));

                if (filter.FinYear <= 0)
                    return Ok(await Result<object>.FailAsync($"{localization.GetHrResource("FinancialYear")}"));

                List<HrProvisionFilterDto> resultList = new List<HrProvisionFilterDto>();

                var items = await hrServiceManager.HrProvisionService.GetAllVW(e => e.IsDeleted == false &&
                e.FacilityId == session.FacilityId &&
                e.TypeId == 2 &&
                (filter.FinYear == 0 || filter.FinYear == e.FinYear) &&
                (filter.MonthId == 0 || filter.MonthId == e.MonthId) &&
                (filter.YearlyOrMonthly == 0 || filter.YearlyOrMonthly == e.YearlyOrMonthly) &&
                (string.IsNullOrEmpty(filter.Description) || (e.Description != null && e.Description.ToLower().Contains(filter.Description.ToLower()))) &&
                (string.IsNullOrEmpty(filter.Code) || e.Code == filter.Code) 
                );

                if (!items.Succeeded)
                    return Ok(await Result<object>.FailAsync(items.Status.message));


                if (!items.Data.Any())
                    return Ok(await Result<List<HrProvisionFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));

                var res = items.Data.AsQueryable();
				if (!string.IsNullOrEmpty(filter.FromDate) && !string.IsNullOrEmpty(filter.ToDate))
				{
					res = res.Where(r => r.PDate != null &&
					DateHelper.StringToDate(r.PDate) >= DateHelper.StringToDate(filter.FromDate) &&
				   DateHelper.StringToDate(r.PDate) <= DateHelper.StringToDate(filter.ToDate));
				}

				foreach (var item in res)
                {
                    var newItem = new HrProvisionFilterDto
                    {
                        Id = item.Id,
                        Code = item.Code,
                        PDate = item.PDate,
                        YearlyOrMonthlyName = item.YearlyOrMonthlyName,
                        Description = item.Description,
                        MonthName = item.MonthName,
                        FinYear = item.FinYear,
                        FacilityName = item.FacilityName,
                    };
                    resultList.Add(newItem);
                }

                if (resultList.Count > 0)
                    return Ok(await Result<object>.SuccessAsync(resultList));

                return Ok(await Result<List<HrProvisionFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync(ex.Message));
            }
        }


        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long Id = 0)
        {
            var chk = await permission.HasPermission(1518, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (Id <= 0)
            {
                return Ok(await Result<object>.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));
            }

            try
            {
                var del = await hrServiceManager.HrProvisionService.Remove(Id);
                return Ok(del);
            }
            catch (Exception exp)
            {
                return Ok(await Result<object>.FailAsync($"{exp.Message}"));
            }
        }

    [HttpGet("GetById")]
    public async Task<IActionResult> Edit(long Id)
    {
      var chk = await permission.HasPermission(1518, PermissionType.Edit);
      if (!chk)
      {
        return Ok(await Result<HrProvisionEditDto>.FailAsync($"Access Denied"));
      }
      if (Id <= 0)
        return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));

      try
      {
        var JCode = "";
        var getProvision = await hrServiceManager.HrProvisionService.GetOneVW(p => p.Id == Id && p.IsDeleted == false);
        if (!getProvision.Succeeded)
        {
          return Ok(await Result<object>.FailAsync(getProvision.Status.message));

        }
        if (getProvision.Data == null)
          return Ok(await Result<object>.FailAsync($"{localization.GetMessagesResource("NoItemFoundToEdit")}"));


        var ProvisionData = new HrProvisionEditDto();
        ProvisionData = mapper.Map<HrProvisionEditDto>(getProvision.Data);



        var getProvisionEmp = await hrServiceManager.HrProvisionsEmployeeService.GetAllVW(e => e.IsDeleted == false && e.PId == Id);
        if (getProvisionEmp.Data.Any())
          ProvisionData.ProvisionsEmployee = mapper.Map<List<HrProvisionEmployeeResultDto>>(getProvisionEmp.Data.ToList());

        var getJournal = await accServiceManager.AccJournalMasterService.GetOne(j => j.ReferenceNo == Id && j.DocTypeId == 56 && j.FlagDelete == false);
        if (getJournal.Succeeded)
        {
          if (getJournal.Data != null)
            JCode = getJournal.Data.JCode;

          ProvisionData.JCode = JCode;
        }

        return Ok(await Result<HrProvisionEditDto>.SuccessAsync(ProvisionData));

      }
      catch (Exception exp)
      {
        return Ok(await Result<HrProvisionEditDto>.FailAsync($"EXP in {this.GetType()}, Meesage: {exp.Message}"));
      }
    }

    #endregion


    #region AddPage

    [HttpPost("SearchInAdd")]
        public async Task<IActionResult> SearchInAdd(ProvisionSearchOnAddFilter filter)
        {
            var chk = await permission.HasPermission(1518, PermissionType.Add);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                filter.MonthId ??= 0;
                filter.FinYear ??= 0;
                filter.YearlyOrMonthly ??= 0;
                filter.DepartmentId ??= 0;
                filter.LocationId ??= 0;
                filter.BranchId ??= 0;
                filter.NationalityId ??= 0;
                filter.JobCategory ??= 0;
                filter.SalaryGroupId ??= 0;
                filter.TypeId = 2;

                if (filter.YearlyOrMonthly <= 0)
                    return Ok(await Result<object>.FailAsync($"{localization.GetHrResource("OperationType")}"));
                // سنوي
                if (filter.YearlyOrMonthly == 1)
                {
                    filter.MonthId = 12;
                    filter.FinYear = DateTime.Now.Year;
                }
                // شهري
                else
                {
                    filter.FinYear = DateTime.Now.Year;

                    filter.MonthId = 0;

                }
                if (string.IsNullOrEmpty(filter.StatusList) || filter.StatusList == "0")
                {
                    filter.StatusList = "";
                }
                var GetData = await hrServiceManager.HrProvisionService.GetEmployeeProvisionTicketData(filter);
                return Ok(GetData);
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync(ex.Message));
            }
        }


        [HttpPost("Add")]
        public async Task<ActionResult> Add(HrProvisionDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(1518, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (obj.YearlyOrMonthly <= 0)
                    return Ok(await Result<object>.FailAsync($"{localization.GetHrResource("OperationType")}"));
                // سنوي
                if (obj.YearlyOrMonthly == 1)
                {
                    obj.MonthId = 12;
                    obj.FinYear = DateTime.Now.Year;
                }
                // شهري
                else
                {
                    obj.FinYear = DateTime.Now.Year;

                    if (obj.MonthId <= 0)
                        return Ok(await Result<object>.FailAsync($"{localization.GetHrResource("Month")}"));
                }

                if (string.IsNullOrEmpty(obj.Description))
                    return Ok(await Result.FailAsync($"{localization.GetAccResource("Description")}"));

                if (string.IsNullOrEmpty(obj.PDate))
                    return Ok(await Result.FailAsync($"{localization.GetCommonResource("Tdate")}"));
                //  مخصص التذاكر
                obj.TypeId = 2;
                var add = await hrServiceManager.HrProvisionService.AddTicketProvision(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add HRProvisionsTicketController, MESSAGE: {ex.Message}"));
            }
        }

        #endregion



        #region Edit Page

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(HrProvisionEditDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(1518, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (string.IsNullOrEmpty(obj.Description))
                    return Ok(await Result<object>.FailAsync($"{localization.GetAccResource("Description")}"));
                if (obj.Id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }
                if (obj.ProvisionsEmployee.Count() <= 0)
                    return Ok(await Result<object>.FailAsync($"لم يتم تحديد اي مخصص"));

                var update = await hrServiceManager.HrProvisionService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrProvisionEditDto>.FailAsync($"====== Exp in Edit HRProvisionsTicketController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("CreateJournal")]
        public async Task<ActionResult> CreateJournal(HrProvisionEntryAddDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(1518, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (obj.Id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                if (string.IsNullOrEmpty(obj.JournalDate))
                    return Ok(await Result<object>.FailAsync($"{localization.GetCommonResource("Tdate")}"));
                obj.DocTypeId = 56;// نوع القيد قيد مخصص التذاكر
                obj.Type = 2;// نوع القيد قيد مخصص التذاكر
                var result = await hrServiceManager.HrProvisionService.CreateProvisionEntry(obj);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrProvisionEditDto>.FailAsync($"====== Exp in CreateJournal HRProvisionsTicketController, MESSAGE: {ex.Message}"));
            }
        }

        #endregion
    }
}
