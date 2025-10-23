using Castle.Components.DictionaryAdapter.Xml;
using Castle.MicroKernel.Registration;
using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Helpers;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.HR.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace Logix.MVC.LogixAPIs.HR
{

    //   القرارات
    public class HRDecisionsController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IApiDDLHelper ddlHelper;


        public HRDecisionsController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IApiDDLHelper ddlHelper)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.ddlHelper = ddlHelper;
        }

        #region Index Page

        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrDecisionFilterDto filter)
        {
            var chk = await permission.HasPermission(932, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                filter.DeptId ??= 0;
                filter.BranchId ??= 0;
                filter.Location ??= 0;
                filter.DecisionType ??= 0;
                filter.DecCode ??= 0;

                List<HrDecisionFilterDto> resultList = new List<HrDecisionFilterDto>();

                var items = await hrServiceManager.HrDecisionService.GetAll(e => e.IsDeleted == false &&
                    (filter.DecisionType == 0 || filter.DecisionType == e.DecType) &&
                    (filter.DecCode == 0 || filter.DecCode == e.DecCode)
                );

                if (!items.Succeeded)
                {
                    return Ok(await Result<HrDecisionFilterDto>.FailAsync(items.Status.message));
                }

                if (!items.Data.Any())
                {
                    return Ok(await Result<List<HrDecisionFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));
                }

                var res = items.Data.AsQueryable();
                if (!string.IsNullOrEmpty(filter.FromDate) && !string.IsNullOrEmpty(filter.ToDate))
                {
                    var FromDate = DateHelper.StringToDate(filter.FromDate);
                    var ToDate = DateHelper.StringToDate(filter.ToDate);
                    res = res.Where(r => r.DecDate != null &&
                        DateHelper.StringToDate(r.DecDate) >= FromDate &&
                        DateHelper.StringToDate(r.DecDate) <= ToDate
                    );
                }

                if (!res.Any())
                {
                    return Ok(await Result<List<HrDecisionFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));
                }

                var getAllDecisionsEmployee = await hrServiceManager.HrDecisionsEmployeeService.GetAllVW(x => x.IsDeleted == false && x.DecisionsId != null &&
                    (string.IsNullOrEmpty(filter.EmpCode) || x.EmpCode == filter.EmpCode) &&
                    (string.IsNullOrEmpty(filter.EmpName) || (x.EmpName != null && x.EmpName.Contains(filter.EmpName))) &&
                    (filter.DeptId == 0 || filter.DeptId == x.DeptId) &&
                    (filter.BranchId == 0 || filter.BranchId == x.BranchId) &&
                    (filter.Location == 0 || filter.Location == x.Location)
                );

                var decisionsIdsList = getAllDecisionsEmployee.Data.Select(x => x.DecisionsId).ToList();

                res = res.Where(x => decisionsIdsList.Contains(x.Id));

                if (!res.Any())
                {
                    return Ok(await Result<List<HrDecisionFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));
                }

                var getAllDecisionsTypes = await hrServiceManager.HrDecisionsTypeService.GetAll(e => e.IsDeleted == false);

                foreach (var item in res)
                {
                    var decType = getAllDecisionsTypes.Data.FirstOrDefault(x => x.Id == item.DecType)?.DecType ?? "";
                    var newRecord = new HrDecisionFilterDto
                    {
                        Id = item.Id,
                        DecCode = item.DecCode,
                        DecisionDate = item.DecDate,
                        DecisionTypeName = decType,
                    };
                    resultList.Add(newRecord);
                }

                return Ok(await Result<List<HrDecisionFilterDto>>.SuccessAsync(resultList, localization.GetResource1("SearchSuccess")));
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrDecisionFilterDto>.FailAsync(ex.Message));
            }
        }


        [HttpPost("GetPagination")]
        public async Task<IActionResult> SearchPagination(
            [FromBody] HrDecisionFilterDto filter,
            int take = 10,
            long? lastSeenId = null)
        {
            try
            {
                var chk = await permission.HasPermission(932, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                // Normalize filters
                filter.DeptId ??= 0;
                filter.BranchId ??= 0;
                filter.Location ??= 0;
                filter.DecisionType ??= 0;
                filter.DecCode ??= 0;

                // Date conditions
                List<DateCondition>? dateConditions = null;
                if (!string.IsNullOrEmpty(filter.FromDate) && !string.IsNullOrEmpty(filter.ToDate))
                {
                    dateConditions = new List<DateCondition>
            {
                new DateCondition
                {
                    DatePropertyName = "DecDate",
                    ComparisonOperator = ComparisonOperator.GreaterThanOrEqual,
                    StartDateString = filter.FromDate
                },
                new DateCondition
                {
                    DatePropertyName = "DecDate",
                    ComparisonOperator = ComparisonOperator.LessThanOrEqual,
                    StartDateString = filter.ToDate
                }
            };
                }

                // Step 1: Get decisions with pagination
                var items = await hrServiceManager.HrDecisionService.GetAllWithPaginationVW(
                    selector: e => e.Id,
                    expression: e => e.IsDeleted == false &&
                                     (filter.DecisionType == 0 || filter.DecisionType == e.DecType) &&
                                     (filter.DecCode == 0 || filter.DecCode == e.DecCode),
                    take: take,
                    lastSeenId: lastSeenId,
                    dateConditions: dateConditions
                );

                if (!items.Succeeded)
                    return Ok(await Result<List<HrDecisionsVw>>.FailAsync(items.Status.message));

                if (!items.Data.Any())
                    return Ok(await Result<List<HrDecisionsVw>>.SuccessAsync(new List<HrDecisionsVw>(), localization.GetResource1("NosearchResult")));

                // Step 2: Filter by employees
                var employees = await hrServiceManager.HrDecisionsEmployeeService.GetAllVW(x =>
                    x.IsDeleted == false && x.DecisionsId != null &&
                    (string.IsNullOrEmpty(filter.EmpCode) || x.EmpCode == filter.EmpCode) &&
                    (string.IsNullOrEmpty(filter.EmpName) || (x.EmpName != null && x.EmpName.Contains(filter.EmpName))) &&
                    (filter.DeptId == 0 || filter.DeptId == x.DeptId) &&
                    (filter.BranchId == 0 || filter.BranchId == x.BranchId) &&
                    (filter.Location == 0 || filter.Location == x.Location)
                );

                var decisionIds = employees.Data.Select(x => x.DecisionsId).ToHashSet();
                var filteredDecisions = items.Data.Where(x => decisionIds.Contains(x.Id)).ToList();

                if (!filteredDecisions.Any())
                    return Ok(await Result<List<HrDecisionsVw>>.SuccessAsync(new List<HrDecisionsVw>(), localization.GetResource1("NosearchResult")));

                // Step 3: Get decision types
                var allTypes = await hrServiceManager.HrDecisionsTypeService.GetAll(e => e.IsDeleted == false);

                // Step 4: Map result DTO
                var resultList = filteredDecisions.Select(item => new HrDecisionsVw
                {
                    Id = item.Id,
                    DecCode = item.DecCode,
                    DecDate = item.DecDate,
                    DecTypeName = allTypes.Data.FirstOrDefault(x => x.Id == item.DecType)?.DecType ?? ""
                }).ToList();

                // Step 5: Return with pagination metadata
                var paginatedData = new PaginatedResult<List<HrDecisionsVw>>
                {
                    Succeeded = true,
                    Data = resultList,
                    Status = items.Status,
                    PaginationInfo = items.PaginationInfo
                };

                return Ok(paginatedData);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrDecisionsVw>.FailAsync(ex.Message));
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long Id)
        {
            try
            {
                List<EmpInfo> empList = new List<EmpInfo>();
                var chk = await permission.HasPermission(932, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var item = await hrServiceManager.HrDecisionService.GetOneVW(x => x.Id == Id);
                var DecisionEmployee = await hrServiceManager.HrDecisionsEmployeeService.GetAllVW(e => e.DecisionsId == Id && e.IsDeleted == false);

                if (DecisionEmployee.Data.Count() > 0)
                {
                    foreach (var emp in DecisionEmployee.Data)
                    {
                        var NewEmpInfo = new EmpInfo
                        {
                            EmpCode = emp.EmpCode,
                            EmpName = emp.EmpName,
                            IsDeleted = emp.IsDeleted,
                            Id = emp.Id
                        };
                        empList.Add(NewEmpInfo);
                    }
                }
                return Ok(await Result<object>.SuccessAsync(new { Decision = item.Data, DecisionEmployeeData = empList }));


            }
            catch (Exception ex)
            {
                return Ok(await Result<HrDecisionsVw>.FailAsync($"====== Exp in Hr Decision Controller getById, MESSAGE: {ex.Message}"));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var chk = await permission.HasPermission(932, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await hrServiceManager.HrDecisionService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete Hr Decision Controller, MESSAGE: {ex.Message}"));
            }
        }

        #endregion



        #region Add Page


        [HttpPost("Add")]
        public async Task<ActionResult> Add(HrDecisionDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(932, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (obj.DecType <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetHrResource("DecisionType")}"));
                }
                if (obj.EmpInfo.Count() <= 0)
                {
                    return Ok(await Result.FailAsync($"يجب ادخال موظف واحد على الأقل"));
                }
                var add = await hrServiceManager.HrDecisionService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add Hr Decision  Controller, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("DecisionTypeChanged")]
        public async Task<IActionResult> DecisionTypeChanged(DecisionTypeChangedDto obj)
        {
            var chk = await permission.HasPermission(932, PermissionType.Add);
            if (!chk)
                return Ok(await Result.AccessDenied("AccessDenied"));

            if (obj.TypeId <= 0)
                return Ok(await Result<EmpIdChangedVM>.SuccessAsync("يجب اختيار نوع القرار"));

            try
            {
                var GetDecisionType = await hrServiceManager.HrDecisionsTypeService.GetOne(i => i.Id == obj.TypeId && i.IsDeleted == false);

                if (GetDecisionType.Data == null)
                    return Ok(await Result<object>.SuccessAsync("نوع القرار غير موجود"));

                string decisions = string.Empty;
                decisions = GetDecisionType.Data.Note ?? "";
                if (!string.IsNullOrEmpty(obj.ApplicationDate))
                    decisions = StripTags(decisions.Replace("Date", obj.ApplicationDate));

                if (obj.ApplicationCode > 0)
                    decisions = StripTags(decisions.Replace("Code", obj.ApplicationCode.ToString()));

                return Ok(await Result<object>.SuccessAsync(decisions, " ", 200));
            }
            catch (Exception exp)
            {
                return Ok(await Result<object>.FailAsync($"{exp.Message}"));
            }
        }

        // Function to strip HTML tags from a string

        [NonAction]
        private string StripTags(string html)
        {
            return Regex.Replace(html, "<.*?>", string.Empty);
        }


        #endregion

        #region Edit Page

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(HrDecisionEditDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(932, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (obj.DecType <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetHrResource("DecisionType")}"));
                }
                if (obj.EmpInfo.Where(x => x.IsDeleted == false).Count() <= 0)
                {
                    return Ok(await Result.FailAsync($"يجب ادخال موظف واحد على الأقل"));
                }
                var update = await hrServiceManager.HrDecisionService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrDecisionEditDto>.FailAsync($"====== Exp in Hr Decisions Controller getById, MESSAGE: {ex.Message}"));
            }
        }



        [HttpGet("SendEmail")]
        public async Task<IActionResult> SendEmail(long Id)
        {
            try
            {
                var chk = await permission.HasPermission(932, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var result = await hrServiceManager.HrDecisionService.SendEmail(Id);
                return Ok(result);


            }
            catch (Exception ex)
            {
                return Ok(await Result<HrDecisionsVw>.FailAsync($"====== Exp in Hr Decision Controller SendEmail, MESSAGE: {ex.Message}"));
            }
        }
        #endregion



    }
}