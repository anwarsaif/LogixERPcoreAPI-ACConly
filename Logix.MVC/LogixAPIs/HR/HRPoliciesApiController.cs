using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.HR
{
    //  سياسات الإحتساب
    public class HRPoliciesApiController : BaseHrApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;

        public HRPoliciesApiController(IHrServiceManager hrServiceManager, IMainServiceManager mainServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization)
        {
            this.mainServiceManager = mainServiceManager;
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var chk = await permission.HasPermission(1246, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var items = await hrServiceManager.HrPoliciesTypeService.GetAll();
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    return Ok(await Result<List<HrPoliciesTypeDto>>.SuccessAsync(res.ToList(), items.Status.message));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpPost("GetPagination")]
        public async Task<IActionResult> GetPagination([FromBody] HrPoliciesTypeFilterDto filter, int take = Pagination.take, int? lastSeenId = null)
        {
            var chk = await permission.HasPermission(1246, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var items = await hrServiceManager.HrPoliciesTypeService.GetAllWithPaginationVW(
                    selector: e => e.TypeId,
                    expression: e =>
                        (
                            string.IsNullOrEmpty(filter.TypeName)
                            || (e.TypeName != null && e.TypeName.ToLower().Contains(filter.TypeName.ToLower()))
                            || (e.TypeName2 != null && e.TypeName2.ToLower().Contains(filter.TypeName.ToLower()))
                        ),
                    take: take,
                    lastSeenId: lastSeenId
                );

                if (!items.Succeeded)
                    return Ok(await Result<List<HrPoliciesTypeDto>>.FailAsync(items.Status.message));

                if (items.Data == null || !items.Data.Any())
                    return Ok(await Result<List<HrPoliciesTypeDto>>.SuccessAsync(new List<HrPoliciesTypeDto>()));

                var res = items.Data.OrderBy(x => x.TypeId).ToList();

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
                return Ok(await Result.FailAsync(ex.Message));
            }
        }



        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long TypeId)
        {
            var resultData = new HrPolicyEditDto();
            var chk = await permission.HasPermission(1246, PermissionType.Edit);
            if (!chk)
            {
                return Ok(Result<HrPolicyEditDto>.FailAsync($"Access Denied"));
            }
            if (TypeId <= 0)
                return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));

            try
            {
                var getItem = await hrServiceManager.HrPolicyService.GetOneVW(x => x.IsDeleted == false && x.PolicieId == TypeId && x.FacilityId == session.FacilityId);

                if (getItem.Data != null)
                {

                    resultData.RateType = getItem.Data.RateType;
                    //resultData.Id = getItem.Data.Id;
                    resultData.Id = getItem.Data.PolicieId ?? 0;
                    resultData.Salary = getItem.Data.Salary;
                    resultData.PolicieId = getItem.Data.PolicieId;
                    resultData.TotalRate = getItem.Data.TotalRate;
                    resultData.Allawance = getItem.Data.Allawance;
                    resultData.Deductions = getItem.Data.Deductions;
                    resultData.SalaryRate = getItem.Data.SalaryRate;
                    resultData.FacilityId = getItem.Data.FacilityId;
                    resultData.TypeName = getItem.Data.TypeName;
                    resultData.TypeName2 = getItem.Data.TypeName2;
                    resultData.Salary = getItem.Data.Salary;

                }
                else
                {
                    var GetPoliciesType = await hrServiceManager.HrPoliciesTypeService.GetAll(x => x.TypeId == 0 || x.TypeId == TypeId);
                    if (GetPoliciesType != null)
                    {
                        var singlePlicy = GetPoliciesType.Data.FirstOrDefault();
                        resultData.TypeName = singlePlicy.TypeName;
                        resultData.TypeName2 = singlePlicy.TypeName2;
                        resultData.Id = singlePlicy.TypeId;
                    }
                }

                if (resultData.Id > 0) return Ok(await Result<HrPolicyEditDto>.SuccessAsync(resultData));
                return Ok(await Result<HrPolicyEditDto>.FailAsync(localization.GetMessagesResource("NoDataFound")));
            }
            catch (Exception exp)
            {
                return Ok(await Result<HrPolicyEditDto>.FailAsync($"EXP in {this.GetType()}, Meesage: {exp.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(HrPolicyEditDto obj)
        {
            var chk = await permission.HasPermission(1246, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                if (obj.RateType <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetHrResource("Enterapercentageof")));

                var addRes = await hrServiceManager.HrPolicyService.Update(obj);
                return Ok(addRes);
            }

            catch (Exception ex)
            {
                return Ok(await Result<HrPolicyEditDto>.FailAsync($"{ex.Message}"));
            }
        }
    }

}