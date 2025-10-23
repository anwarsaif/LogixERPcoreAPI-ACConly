using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Interfaces.IServices.HR;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
namespace Logix.MVC.LogixAPIs.HR
{
    //   بوليصة التأمين 
    public class HRInsurancePolicyController : BaseHrApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IPermissionHelper permission;
        private readonly IHrServiceManager hrServiceManager;
        private readonly ILocalizationService localization;
        public HRInsurancePolicyController(IMainServiceManager mainServiceManager, IPermissionHelper permission, IHrServiceManager hrServiceManager, ILocalizationService localization)
        {
            this.mainServiceManager = mainServiceManager;
            this.permission = permission;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
        }

        #region Index Page


        [HttpPost("Search")]

        public async Task<IActionResult> Search(HrInsurancefilterPolicyDto filter)
        {
            List<HrInsurancefilterPolicyDto> InsurancePolicyList = new List<HrInsurancefilterPolicyDto>();

            var chk = await permission.HasPermission(1251, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var items = await hrServiceManager.HrInsurancePolicyService.GetAll(e => e.IsDeleted == false &&
                (string.IsNullOrEmpty(filter.Code) || (e.Code != null && e.Code.Contains(filter.Code))) &&
                (string.IsNullOrEmpty(filter.Name) || (e.Name != null && e.Name.Contains(filter.Name)))
                );

                if (!items.Succeeded)
                {
                    return Ok(await Result.FailAsync(items.Status.message));

                }
                var res = items.Data.AsQueryable();
                if (!string.IsNullOrEmpty(filter.StartDate) && (!string.IsNullOrEmpty(filter.EndDate)))
                {
                    var StartDate = DateHelper.StringToDate(filter.StartDate);
                    var EndDate = DateHelper.StringToDate(filter.EndDate);
                    res = res.Where(x => DateHelper.StringToDate(x.StartDate) >= StartDate
                    && DateHelper.StringToDate(x.StartDate) <= EndDate);
                }
                res = res.OrderBy(e => e.Id);
                if (res.Count() > 0)
                {
                    foreach (var item in res)
                    {
                        var newRow = new HrInsurancefilterPolicyDto
                        {
                            Id = item.Id,
                            Code = item.Code,
                            StartDate = item.StartDate,
                            Name = item.Name,
                            EndDate = item.EndDate,
                            Note = item.Note,
                        };
                        InsurancePolicyList.Add(newRow);
                    }
                    return Ok(await Result<List<HrInsurancefilterPolicyDto>>.SuccessAsync(InsurancePolicyList, ""));
                }
                return Ok(await Result<List<HrInsurancefilterPolicyDto>>.SuccessAsync(InsurancePolicyList, localization.GetResource1("NosearchResult")));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
       
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var chk = await permission.HasPermission(1251, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await hrServiceManager.HrInsurancePolicyService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete HRInsurancePolicyController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long Id)
        {
            try
            {
                var chk = await permission.HasPermission(1251, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var item = await hrServiceManager.HrInsurancePolicyService.GetOne(x => x.Id == Id);
                if (!item.Succeeded)
                {
                    return Ok(await Result<HrInsurancePolicyEditDto>.FailAsync(item.Status.message));

                }
                return Ok(item);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrNoteEditDto>.FailAsync($"====== Exp in HRInsurancePolicyController getById, MESSAGE: {ex.Message}"));
            }
        }


        #endregion

        #region Add


        [HttpPost("Add")]
        public async Task<ActionResult> Add(HrInsurancePolicyDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(1251, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));
                if (string.IsNullOrEmpty(obj.Code))
                {
                    return Ok(await Result.FailAsync($"{localization.GetHrResource("Code")}"));

                }
                if (string.IsNullOrEmpty(obj.Name))
                {
                    return Ok(await Result.FailAsync($"{localization.GetHrResource("Name")}"));

                }
                if (string.IsNullOrEmpty(obj.StartDate))
                {
                    return Ok(await Result.FailAsync($"{localization.GetCommonResource("FromDate")}"));

                }
                if (string.IsNullOrEmpty(obj.EndDate))
                {
                    return Ok(await Result.FailAsync($"{localization.GetCommonResource("ToDate")}"));

                }
                if (obj.CompanyId <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetHrResource("InsuranceCompany")}"));

                }
                var add = await hrServiceManager.HrInsurancePolicyService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add HRInsurancePolicyController, MESSAGE: {ex.Message}"));
            }
        }

        #endregion

        #region Edit Page

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(HrInsurancePolicyEditDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(1251, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result<HrInsurancePolicyEditDto>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));
                if (string.IsNullOrEmpty(obj.Code))
                {
                    return Ok(await Result.FailAsync($"{localization.GetHrResource("Code")}"));

                }
                if (string.IsNullOrEmpty(obj.Code))
                {
                    return Ok(await Result.FailAsync($"{localization.GetHrResource("Code")}"));

                }
                if (string.IsNullOrEmpty(obj.Name))
                {
                    return Ok(await Result.FailAsync($"{localization.GetHrResource("Name")}"));

                }
                if (string.IsNullOrEmpty(obj.StartDate))
                {
                    return Ok(await Result.FailAsync($"{localization.GetCommonResource("FromDate")}"));

                }
                if (string.IsNullOrEmpty(obj.EndDate))
                {
                    return Ok(await Result.FailAsync($"{localization.GetCommonResource("ToDate")}"));

                }
                if (obj.CompanyId <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetHrResource("InsuranceCompany")}"));

                }
                var update = await hrServiceManager.HrInsurancePolicyService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrInsurancePolicyEditDto>.FailAsync($"====== Exp in Edit HRInsurancePolicyController, MESSAGE: {ex.Message}"));
            }
        }
        #endregion





    }
}






