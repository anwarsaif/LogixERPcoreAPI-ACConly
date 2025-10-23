using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.HR;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Logix.MVC.LogixAPIs.Recruitment
{

    public class RecruitmentJobOfferController : BaseRecruitmentApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;

        public RecruitmentJobOfferController(IHrServiceManager hrServiceManager, IMainServiceManager mainServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization)
        {
            this.mainServiceManager = mainServiceManager;
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
        }

        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrJobOfferFilterDto filter)
        {
            var chk = await permission.HasPermission(1411, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                filter.ApplicantId ??= 0;

                List<HrJobOfferFilterDto> resultList = new List<HrJobOfferFilterDto>();

                var items = await hrServiceManager.HrJobOfferService.GetAllVW(e => e.IsDeleted == false &&
               (filter.ApplicantId == 0 || filter.ApplicantId == e.RecruApplicantId) &&
                (string.IsNullOrEmpty(filter.ApplicantName) || (e.Name != null && e.Name.ToLower().Contains(filter.ApplicantName.ToLower())) || (e.NameEn != null && e.NameEn.ToLower().Contains(filter.ApplicantName.ToLower())))
                );

                if (!items.Succeeded)
                    return Ok(await Result<object>.FailAsync(items.Status.message));


                if (!items.Data.Any())
                    return Ok(await Result<List<HrJobOfferFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));

                var res = items.Data.AsQueryable();

                if (!string.IsNullOrEmpty(filter.From) && !string.IsNullOrEmpty(filter.To))
                {
                    var FromDate = DateHelper.StringToDate(filter.From);
                    var ToDate = DateHelper.StringToDate(filter.To);
                    res = res.Where(c => (c.CreatedOn != null &&
                    c.CreatedOn >= FromDate &&
                    c.CreatedOn <= ToDate));
                }
                foreach (var item in res)
                {
                    var newItem = new HrJobOfferFilterDto
                    {
                        Id = item.Id,
                        Date = item.CreatedOn.HasValue ? item.CreatedOn.Value.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture) : "",
                        ApplicantName = (session.Language == 1) ? item.Name : item.NameEn,
                        FileUrl = string.IsNullOrEmpty(item.FileUrl) ? "" : item.FileUrl.Replace("~\\", ""),
                        VacancyName = item.VacancyName,
                        TotalSalary = item.TotalSalary
                    };
                    resultList.Add(newItem);
                }



                if (resultList.Count > 0)
                {
                    return Ok(await Result<object>.SuccessAsync(resultList));

                }
                return Ok(await Result<List<object>>.SuccessAsync(new List<object>(), localization.GetResource1("NosearchResult")));
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync(ex.Message));
            }
        }
        [HttpPost("Add")]
        public async Task<IActionResult> Add(HrJobOfferDto obj)
        {
            var chk = await permission.HasPermission(1411, PermissionType.Add);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                obj.ApplicantId ??= 0;
                obj.HousingAllowance ??= 0;
                obj.TransportAllowance ??= 0;
                obj.OtherAllowance ??= 0;
                obj.ContractTypeId ??= 0;
                if (obj.ApplicantId <= 0)
                    return Ok(await Result<HrRecruitmentVacancyDto>.FailAsync($"{localization.GetHrResource("ApplicantNo")}"));

                if (obj.JobCatId <= 0)
                    return Ok(await Result<HrRecruitmentVacancyDto>.FailAsync($"{localization.GetHrResource("JobName")}"));
                if (obj.BasicSalary <= 0)
                    return Ok(await Result<HrRecruitmentVacancyDto>.FailAsync($"{localization.GetHrResource("BasicSalary")}"));

                var add = await hrServiceManager.HrJobOfferService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrRecruitmentVacancyDto>.FailAsync(ex.Message));
            }
        }


    }
}